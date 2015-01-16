using System;
using System.Data;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.Invoice")]
[Title("CMSModules/CMS_Ecommerce/invoice.png", "InvoiceTemplate.HeaderCaption", "invoice")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_InvoiceTemplate_InvoiceTemplate_Edit : CMSEcommerceSharedConfigurationPage
{
    #region "Variables"

    private SiteInfo mConfiguredSiteInfo = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// SiteInfo object of currently configured site.
    /// </summary>
    private SiteInfo ConfiguredSiteInfo
    {
        get
        {
            if (mConfiguredSiteInfo == null)
            {
                mConfiguredSiteInfo = SiteInfoProvider.GetSiteInfo(ConfiguredSiteID);
            }

            return mConfiguredSiteInfo;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        GlobalObjectsKeyName = ECommerceSettings.USE_GLOBAL_INVOICE;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        HeaderActions actions = CurrentMaster.HeaderActions;

        // Save action
        actions.ActionsList.Add(new SaveAction(this)
        {
            CommandName = "lnksave_click"
        });

        // Show "Copy from global" link when not configuring global invoice.
        if (ConfiguredSiteID != 0)
        {
            actions.ActionsList.Add(new HeaderAction()
            {
                ControlType = HeaderActionTypeEnum.LinkButton,
                Text = GetString("com.InvoiceFromGlobal"),
                OnClientClick = "return ConfirmCopyFromGlobal();",
                ImageUrl = GetImageUrl("CMSModules/CMS_Ecommerce/invoicefromglobal.png"),
                CommandName = "copyFromGlobal"
            });

            // Register javascript to confirm generate 
            string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmInvoiceFromGlobal")) + ");}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromGlobal", ScriptHelper.GetScript(script));
        }

        actions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);

        lblInsertMacro.Text = GetString("macroselector.insertmacro") + ":";
        AttachmentTitle.TitleText = GetString("general.attachments");

        macroSelectorElm.Resolver = EmailTemplateMacros.EcommerceResolver;
        macroSelectorElm.CKEditorID = htmlInvoiceTemplate.ClientID;
        AttachmentList.CheckObjectPermissions = false;
        
        if (ConfiguredSiteInfo != null)
        {          
            bool allowEdit = ECommerceContext.IsUserAuthorizedForPermission("ConfigurationModify");
            if (allowEdit)
            {
                // Display attachments tab in media dialog
                htmlInvoiceTemplate.MediaDialogConfig.MetaFileObjectID = ConfiguredSiteID;
                htmlInvoiceTemplate.MediaDialogConfig.MetaFileObjectType = SiteObjectType.SITE;
                htmlInvoiceTemplate.MediaDialogConfig.MetaFileCategory = MetaFileInfoProvider.OBJECT_CATEGORY_INVOICE;
                htmlInvoiceTemplate.MediaDialogConfig.HideAttachments = false;
            }
            else
            {
                // Hide attachments tab in media dialog
                htmlInvoiceTemplate.MediaDialogConfig.HideAttachments = true;
            }

            // Attachment list
            AttachmentList.ObjectID = ConfiguredSiteID;
            AttachmentList.ObjectType = SiteObjectType.SITE;
            AttachmentList.Category = MetaFileInfoProvider.OBJECT_CATEGORY_INVOICE;
            AttachmentList.SiteID = ConfiguredSiteID;

            // Attachment list permissions            
            AttachmentList.AllowEdit = allowEdit;            
        }
        else
        {
            // Disable attachments for global invoice
            plcAttachments.Visible = false;
            htmlInvoiceTemplate.MediaDialogConfig.HideAttachments = true;
        }

        DisplayHelperTable();

        if (!RequestHelper.IsPostBack())
        {
            htmlInvoiceTemplate.ResolvedValue = "";
            // Configuring global invoice
            if (ConfiguredSiteID == 0)
            {
                // Show global invoice
                htmlInvoiceTemplate.ResolvedValue = ECommerceSettings.InvoiceTemplate(null);
            }
            else
            {
                // Show site-specific invoice
                if (ConfiguredSiteInfo != null)
                {
                    htmlInvoiceTemplate.ResolvedValue = ECommerceSettings.InvoiceTemplate(ConfiguredSiteInfo.SiteName);
                }
            }

            InitHTMLEditor();
        }

        // Show "using global settings" info message only if showing global store settings
        if ((ConfiguredSiteID == 0) && (SiteID != 0))
        {
            ShowInformation(GetString("com.UsingGlobalInvoice"));
        }
    }


    /// <summary>
    /// Save button action.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "lnksave_click":
                // Check permissions
                CheckConfigurationModification();

                // Check if template doesn't contains more editable regions with same name
                Hashtable eRegions = new Hashtable();

                int count = 0;
                int textStart = 0;
                int editRegStart = htmlInvoiceTemplate.ResolvedValue.Trim().IndexOfCSafe("$$", textStart);

                while (editRegStart >= 0)
                {
                    count++;

                    // End of region
                    editRegStart += 2;
                    textStart = editRegStart;
                    if (editRegStart < htmlInvoiceTemplate.ResolvedValue.Trim().Length - 1)
                    {
                        int editRegEnd = htmlInvoiceTemplate.ResolvedValue.Trim().IndexOf("$$", editRegStart);
                        if (editRegEnd >= 0)
                        {
                            string region = htmlInvoiceTemplate.ResolvedValue.Trim().Substring(editRegStart, editRegEnd - editRegStart);
                            string[] parts = (region + ":" + ":").Split(':');

                            textStart = editRegEnd + 2;
                            try
                            {
                                string name = parts[0];
                                if (name.Trim() != "")
                                {
                                    if (eRegions[name.ToLowerCSafe()] != null)
                                    {
                                        break;
                                    }

                                    // Check if name is codename
                                    if (!ValidationHelper.IsCodeName(name))
                                    {
                                        break;
                                    }
                                    eRegions[name.ToLowerCSafe()] = 1;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }

                    editRegStart = htmlInvoiceTemplate.ResolvedValue.Trim().IndexOfCSafe("$$", textStart);
                }

                // Save template
                if (ConfiguredSiteInfo != null)
                {
                    SettingsKeyProvider.SetValue(ConfiguredSiteInfo.SiteName + "." + ECommerceSettings.INVOICE_TEMPLATE, htmlInvoiceTemplate.ResolvedValue.Trim());
                }
                else
                {
                    SettingsKeyProvider.SetValue(ECommerceSettings.INVOICE_TEMPLATE, htmlInvoiceTemplate.ResolvedValue.Trim());
                }

                // Show message
                ShowChangesSaved();

                break;

            case "copyfromglobal":
                // Read global invoice
                htmlInvoiceTemplate.ResolvedValue = ECommerceSettings.InvoiceTemplate(null);

                break;
        }
    }


    /// <summary>
    /// Initializes HTML editor's settings.
    /// </summary>
    protected void InitHTMLEditor()
    {
        htmlInvoiceTemplate.AutoDetectLanguage = false;
        htmlInvoiceTemplate.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlInvoiceTemplate.MediaDialogConfig.UseFullURL = true;
        htmlInvoiceTemplate.LinkDialogConfig.UseFullURL = true;
        htmlInvoiceTemplate.QuickInsertConfig.UseFullURL = true;
    }


    protected void FillHelperTable(Table tblHelp, string[,] tableColumns)
    {
        for (int i = 0; i < tableColumns.GetUpperBound(0) + 1; i++)
        {
            TableRow tRow = new TableRow();
            TableCell leftCell = new TableCell();
            leftCell.Wrap = false;

            TableCell rightCell = new TableCell();

            Label lblExample = new Label();
            Label lblExplanation = new Label();

            // init labels
            lblExample.Text = tableColumns[i, 0];
            lblExplanation.Text = tableColumns[i, 1];

            // add labels to the cells
            leftCell.Controls.Add(lblExample);
            rightCell.Controls.Add(lblExplanation);

            leftCell.Width = new Unit(250);

            // add cells to the row
            tRow.Cells.Add(leftCell);
            tRow.Cells.Add(rightCell);

            // add row to the table
            tblHelp.Rows.Add(tRow);
        }
    }


    /// <summary>
    /// Displays helper table with makro examples.
    /// </summary>
    protected void DisplayHelperTable()
    {
        string[,] tableColumns = new string[28, 2];
        int i = 0;

        // Currency
        tableColumns[i, 0] = "{%ShoppingCart.ShoppingCartCurrencyID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_ShoppingCart");

        // Order
        tableColumns[i, 0] = "{%Order.OrderID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_ColumnName");

        tableColumns[i, 0] = "{%Order.OrderInvoiceNumber%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_INVOICENUMBER");

        tableColumns[i, 0] = "{%Order.OrderNote|(encode)%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_ORDERNOTE");

        // Status
        tableColumns[i, 0] = "{%OrderStatus.StatusID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_OrderStatus");

        // Billing address
        tableColumns[i, 0] = "{%BillingAddress.AddressID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_BillingAddressColumn");

        tableColumns[i, 0] = "{%BillingAddress.Country.CountryID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_BillingAddressCountry");

        tableColumns[i, 0] = "{%BillingAddress.State.StateID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_BillingAddressState");

        // Shipping address
        tableColumns[i, 0] = "{%ShippingAddress.AddressID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_ShippingAddress");

        tableColumns[i, 0] = "{%ShippingAddress.ApplyTransformation()%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_ShippingAddressTransformation");

        tableColumns[i, 0] = "{%ShippingAddress.Country.CountryID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_ShippingAddressCountry");

        tableColumns[i, 0] = "{%ShippingAddress.State.StateID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_ShippingAddressState");

        // Company address
        tableColumns[i, 0] = "{%CompanyAddress.AddressID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_CompanyAddress");

        tableColumns[i, 0] = "{%CompanyAddress.Country.CountryID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_CompanyAddressCountry");

        tableColumns[i, 0] = "{%CompanyAddress.State.StateID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_CompanyAddressState");

        tableColumns[i, 0] = "{%ShippingOption.ShippingOptionID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_ShippingOptionColumn");

        tableColumns[i, 0] = "{%PaymentOption.PaymentOptionID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_PaymentOptionColumn");

        tableColumns[i, 0] = "{%Currency.CurrencyID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_Currency");

        // Customer
        tableColumns[i, 0] = "{%Customer.CustomerID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_Customer");

        tableColumns[i, 0] = "{%Customer.CustomerOrganizationID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_OrganizationID");

        tableColumns[i, 0] = "{%Customer.CustomerTaxRegistrationID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_TaxregistRationID");

        tableColumns[i, 0] = "{%DiscountCoupon.DiscountCouponID%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_DiscountCoupon");

        // Tables
        tableColumns[i, 0] = "{%ContentTable.ApplyTransformation()%}";
        tableColumns[i++, 1] = GetString("order_edit_invoice.help_productlist");

        tableColumns[i, 0] = "{%ContentTaxesTable.ApplyTransformation()%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_TaxRecapitulation");

        tableColumns[i, 0] = "{%ShippingTaxesTable.ApplyTransformation()%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_ShippingTaxRecapitulation");

        // Formating
        tableColumns[i, 0] = "{%TotalPrice.Format(Currency.CurrencyFormatString)%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_PriceFormatting");

        tableColumns[i, 0] = "{%Format(Order.OrderDate, \"{0:d}\")%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_ORDERDATE");

        tableColumns[i, 0] = "{%TotalShipping.Format(Currency.CurrencyFormatString)%}";
        tableColumns[i++, 1] = GetString("Order_Edit_Invoice.Help_TOTALSHIPPING");

        FillHelperTable(tblMore, tableColumns);

        lnkMoreMacros.Text = GetString("Order_Edit_Invoice.MoreMacros");
        lblMoreInfo.Text = GetString("Order_Edit_Invoice.MoreInfo");
    }
}