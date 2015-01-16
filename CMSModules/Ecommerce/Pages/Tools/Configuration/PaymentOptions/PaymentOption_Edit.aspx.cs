using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;

// Edited object
[EditedObject(PredefinedObjectType.PAYMENTOPTION, "paymentOptionId")]
// Title
[Title("Objects/Ecommerce_PaymentOption/object.png", "PaymentOption_Edit.HeaderCaption", "newedit_payment_method", ExistingObject = true)]
[Title("Objects/Ecommerce_PaymentOption/new.png", "PaymentOption_New.HeaderCaption", "newedit_payment_method", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_PaymentOptions_PaymentOption_Edit : CMSPaymentMethodsPage
{
    protected int mPaymentOptionId = 0;
    protected int mSiteId = -1;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Field validator error messages initialization
        rfvDisplayName.ErrorMessage = GetString("paymentoption_edit.errorEmptyDisplayName");
        rfvCodeName.ErrorMessage = GetString("paymentoption_edit.errorEmptyCodeName");

        string currentPaymentOption = GetString("PaymentOption_Edit.NewItemCaption");

        // Get paymentOption id from querystring		
        mPaymentOptionId = QueryHelper.GetInteger("paymentOptionId", 0);
        mSiteId = ConfiguredSiteID;
        if (mPaymentOptionId > 0)
        {
            PaymentOptionInfo paymentOptionObj = EditedObject as PaymentOptionInfo;

            if (paymentOptionObj != null)
            {
                currentPaymentOption = ResHelper.LocalizeString(paymentOptionObj.PaymentOptionDisplayName);
                mSiteId = paymentOptionObj.PaymentOptionSiteID;
                CheckEditedObjectSiteID(mSiteId);

                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(paymentOptionObj);

                    // Show that the paymentOption was created or updated successfully
                    if (QueryHelper.GetString("saved", "") == "1")
                    {
                        // Show message
                        ShowChangesSaved();
                    }
                }
            }
        }

        // Init file uploader
        file.ObjectID = mPaymentOptionId;
        file.ObjectType = ECommerceObjectType.PAYMENTOPTION;
        file.Category = MetaFileInfoProvider.OBJECT_CATEGORY_THUMBNAIL;
        file.SiteID = mSiteId;

        succeededElem.SiteID = mSiteId;
        failedElem.SiteID = mSiteId;

        // Initializes page title control		
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = GetString("PaymentOption_Edit.ItemListLink");
        pageTitleTabs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/PaymentOptions/PaymentOption_List.aspx?siteId=" + SiteID;
        pageTitleTabs[0, 2] = "";
        pageTitleTabs[1, 0] = FormatBreadcrumbObjectName(currentPaymentOption, mSiteId);
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }


    /// <summary>
    /// Load data of editing paymentOption.
    /// </summary>
    /// <param name="paymentOptionObj">PaymentOption object</param>
    protected void LoadData(PaymentOptionInfo paymentOptionObj)
    {
        txtPaymentOptionName.Text = paymentOptionObj.PaymentOptionName;
        chkPaymentOptionEnabled.Checked = paymentOptionObj.PaymentOptionEnabled;
        txtPaymentOptionDisplayName.Text = paymentOptionObj.PaymentOptionDisplayName;
        txtDescription.Text = paymentOptionObj.PaymentOptionDescription;
        txtGateUrl.Text = paymentOptionObj.PaymentOptionPaymentGateUrl;
        txtPaymentAssemblyName.Text = paymentOptionObj.PaymentOptionAssemblyName;
        txtPaymentClassName.Text = paymentOptionObj.PaymentOptionClassName;
        succeededElem.OrderStatusID = paymentOptionObj.PaymentOptionSucceededOrderStatusID;
        failedElem.OrderStatusID = paymentOptionObj.PaymentOptionFailedOrderStatusID;
        chkAllowIfNoShipping.Checked = paymentOptionObj.PaymentOptionAllowIfNoShipping;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check input values from textboxes and other contrlos
        string errorMessage = new Validator()
            .NotEmpty(txtPaymentOptionDisplayName.Text.Trim(), GetString("paymentoption_edit.errorEmptyDisplayName"))
            .NotEmpty(txtPaymentOptionName.Text.Trim(), GetString("paymentoption_edit.errorEmptyCodeName")).Result;

        if (!ValidationHelper.IsCodeName(txtPaymentOptionName.Text.Trim()))
        {
            errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        if (errorMessage == "")
        {
            // PaymentOptionName must be unique
            PaymentOptionInfo paymentOptionObj = null;
            string siteWhere = (mSiteId > 0) ? " AND (PaymentOptionSiteID = " + mSiteId + " OR PaymentOptionSiteID IS NULL)" : "";
            DataSet ds = PaymentOptionInfoProvider.GetPaymentOptions("PaymentOptionName = '" + txtPaymentOptionName.Text.Trim().Replace("'", "''") + "'" + siteWhere, null, 1, null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                paymentOptionObj = new PaymentOptionInfo(ds.Tables[0].Rows[0]);
            }

            if ((paymentOptionObj == null) || (paymentOptionObj.PaymentOptionID == mPaymentOptionId))
            {
                // Get the object
                if (paymentOptionObj == null)
                {
                    paymentOptionObj = PaymentOptionInfoProvider.GetPaymentOptionInfo(mPaymentOptionId);
                    if (paymentOptionObj == null)
                    {
                        paymentOptionObj = new PaymentOptionInfo();
                        paymentOptionObj.PaymentOptionSiteID = mSiteId;
                    }
                }

                // Fill object
                paymentOptionObj.PaymentOptionID = mPaymentOptionId;
                paymentOptionObj.PaymentOptionDisplayName = txtPaymentOptionDisplayName.Text.Trim();
                paymentOptionObj.PaymentOptionDescription = txtDescription.Text.Trim();
                paymentOptionObj.PaymentOptionName = txtPaymentOptionName.Text.Trim();
                paymentOptionObj.PaymentOptionEnabled = chkPaymentOptionEnabled.Checked;
                paymentOptionObj.PaymentOptionPaymentGateUrl = txtGateUrl.Text.Trim();
                paymentOptionObj.PaymentOptionClassName = txtPaymentClassName.Text.Trim();
                paymentOptionObj.PaymentOptionAssemblyName = txtPaymentAssemblyName.Text.Trim();
                paymentOptionObj.PaymentOptionSucceededOrderStatusID = succeededElem.OrderStatusID;
                paymentOptionObj.PaymentOptionFailedOrderStatusID = failedElem.OrderStatusID;
                paymentOptionObj.PaymentOptionAllowIfNoShipping = chkAllowIfNoShipping.Checked;

                CheckConfigurationModification(paymentOptionObj.PaymentOptionSiteID);

                bool isNew = (paymentOptionObj.PaymentOptionID <= 0);

                PaymentOptionInfoProvider.SetPaymentOptionInfo(paymentOptionObj);

                // Upload image teaser
                if (isNew)
                {
                    file.ObjectID = paymentOptionObj.PaymentOptionID;
                    file.UploadFile();
                }

                URLHelper.Redirect("PaymentOption_Edit.aspx?paymentOptionId=" + Convert.ToString(paymentOptionObj.PaymentOptionID) + "&saved=1&siteId=" + SiteID);
            }
            else
            {
                // Show error message
                ShowError(GetString("PaymentOption_Edit.PaymentOptionNameExists"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }
}