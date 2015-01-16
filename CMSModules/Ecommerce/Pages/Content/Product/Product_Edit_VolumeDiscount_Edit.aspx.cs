using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Content", UIElements = "ContentProduct.VolumeDiscounts")]
public partial class CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_VolumeDiscount_Edit : CMSContentProductPage
{
    #region "Variables"

    protected int volumeDiscountID = 0;
    protected int productID = 0;
    protected string currencyCode = "";
    protected CurrencyInfo currency = null;
    protected SKUInfo sku = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(this);

        // Get parameters from querystring		
        volumeDiscountID = QueryHelper.GetInteger("VolumeDiscountID", 0);
        productID = QueryHelper.GetInteger("ProductID", 0);

        sku = SKUInfoProvider.GetSKUInfo(productID);
        if (sku != null)
        {
            // Get currency of product price
            currency = CurrencyInfoProvider.GetMainCurrency(sku.SKUSiteID);

            currencyCode = (currency != null) ? currency.CurrencyCode : "";
            currencyCode = HTMLHelper.HTMLEncode(currencyCode);

            // Display product price
            lblProductPrice.Text = CurrencyInfoProvider.GetFormattedPrice(sku.SKUPrice, currency);

            // Check presence of main currency
            string currencyWarning = CheckMainCurrency(sku.SKUSiteID);
            if (!string.IsNullOrEmpty(currencyWarning))
            {
                ShowWarning(currencyWarning, null, null);
            }
        }

        // Init labels
        lblVolumeDiscountValue.Text = GetString("product_edit_volumediscount_edit.volumediscountvaluelabel");
        lblVolumeDiscountMinCount.Text = GetString("product_edit_volumediscount_edit.volumediscountmincountlabel");
        radDiscountAbsolute.Text = GetString("product_edit_volumediscount_edit.rbdiscountabsolute");
        radDiscountRelative.Text = GetString("product_edit_volumediscount_edit.rbdiscountrelative");

        // Init value validator error messages
        rfvDiscountValue.ErrorMessage = GetString("product_edit_volumediscount_edit.rfvdiscountvalue");
        rvDiscountValue.ErrorMessage = GetString("product_edit_volumediscount_edit.rvdiscountvalue");
        rvDiscountValue.MaximumValue = int.MaxValue.ToString();

        // Init min count validator error messages
        rfvMinCount.ErrorMessage = GetString("product_edit_volumediscount_edit.rfvmincount");
        rvMinCount.ErrorMessage = GetString("product_edit_volumediscount_edit.rvmincount");
        rvMinCount.MaximumValue = int.MaxValue.ToString();

        radDiscountAbsolute.Attributes["onclick"] = "jQuery('span[id*=\"lblCurrency\"]').html(" + ScriptHelper.GetString("(" + currencyCode + ")") + ")";
        radDiscountRelative.Attributes["onclick"] = "jQuery('span[id*=\"lblCurrency\"]').html('(%)')";

        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.cancel");

        // If true, edit the existing volume discount
        if (volumeDiscountID > 0)
        {
            // Check if there is already VolumeDiscountInfo with this volumeDiscountID 
            VolumeDiscountInfo volumeDiscountObj = VolumeDiscountInfoProvider.GetVolumeDiscountInfo(volumeDiscountID);
            EditedObject = volumeDiscountObj;

            if (volumeDiscountObj != null)
            {
                // Fill editing form with existing data when not postback
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(volumeDiscountObj);

                    // Show that the volumeDiscount was created or updated successfully
                    if (QueryHelper.GetString("saved", "") == "1")
                    {
                        ShowChangesSaved();

                        // Refresh parent page when editing in modal dialog
                        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshPage(); CloseDialog();");
                    }
                }
            }
            // Set page title to "volume discount properties"
            CurrentMaster.Title.TitleText = GetString("product_edit_volumediscount_edit.edittitletext");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Ecommerce_VolumeDiscount/object.png");
        }
        // Create new volume discount
        else
        {
            if (!RequestHelper.IsPostBack())
            {
                // Set defaults
                radDiscountRelative.Checked = true;
            }

            // Init page header to "new item"		
            CurrentMaster.Title.TitleText = GetString("product_edit_volumediscount_edit.newitemcaption");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Ecommerce_VolumeDiscount/new.png");
        }

        CurrentMaster.Title.HelpTopicName = "newedit_discount";
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.HeadElements.Text = "<base target=\"_self\" />";
        CurrentMaster.HeadElements.Visible = true;

        ScriptHelper.RegisterWOpenerScript(Page);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblCurrency.Text = string.Format("({0})", radDiscountAbsolute.Checked ? HTMLHelper.HTMLEncode(currencyCode) : "%");
    }


    /// <summary>
    /// Load data of editing volumeDiscount.
    /// </summary>
    /// <param name="volumeDiscountObj">VolumeDiscount object</param>
    protected void LoadData(VolumeDiscountInfo volumeDiscountObj)
    {
        // load data from database
        txtVolumeDiscountValue.Text = Convert.ToString(volumeDiscountObj.VolumeDiscountValue);
        txtVolumeDiscountMinCount.Text = Convert.ToString(volumeDiscountObj.VolumeDiscountMinCount);
        radDiscountAbsolute.Checked = (volumeDiscountObj.VolumeDiscountIsFlatValue);
        radDiscountRelative.Checked = !(volumeDiscountObj.VolumeDiscountIsFlatValue);
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (sku == null)
        {
            return;
        }

        if (CheckProductPermissions(sku))
        {
            // True if there is already same min count;
            bool isMinCountUnique = false;

            // Server side validation of user input 
            string errorMessage = new Validator().NotEmpty(txtVolumeDiscountValue.Text, "product_edit_volumediscount_edit.volumediscountvaluelabel").NotEmpty(txtVolumeDiscountMinCount.Text, "product_edit_volumediscount_edit.volumediscountmincountlabel").Result;

            // Discount value validation
            if (errorMessage == "")
            {
                // Relative
                if (radDiscountRelative.Checked && !ValidationHelper.IsInRange(0, 100, ValidationHelper.GetDouble(txtVolumeDiscountValue.Text.Trim(), -1)))
                {
                    errorMessage = GetString("com.error.relativediscountvalue");
                }
                // Absolute
                else if (radDiscountAbsolute.Checked && !ValidationHelper.IsPositiveNumber(ValidationHelper.GetDouble(txtVolumeDiscountValue.Text.Trim(), -1)))
                {
                    errorMessage = GetString("com.error.absolutediscountvalue");
                }
            }

            if (errorMessage == "")
            {
                VolumeDiscountInfo volumeDiscountObj = VolumeDiscountInfoProvider.GetVolumeDiscountInfo(volumeDiscountID);
                // if volumeDiscount doesnt already exist, create new one
                if (volumeDiscountObj == null)
                {
                    // Create new volume discount
                    volumeDiscountObj = new VolumeDiscountInfo();

                    // When creating new one, set its SKUID to productID (obtained from URL)
                    volumeDiscountObj.VolumeDiscountSKUID = productID;
                }

                // Set volumeDiscountObj values
                volumeDiscountObj.VolumeDiscountValue = Convert.ToDouble(txtVolumeDiscountValue.Text.Trim());
                volumeDiscountObj.VolumeDiscountMinCount = Convert.ToInt32(txtVolumeDiscountMinCount.Text.Trim());
                volumeDiscountObj.VolumeDiscountIsFlatValue = radDiscountAbsolute.Checked;

                // Set isMinCountUnique
                VolumeDiscountInfo vdi = VolumeDiscountInfoProvider.GetVolumeDiscountInfo(productID, volumeDiscountObj.VolumeDiscountMinCount);
                if (vdi == null)
                {
                    isMinCountUnique = true;
                }
                else
                {
                    isMinCountUnique = (vdi.VolumeDiscountMinCount != volumeDiscountObj.VolumeDiscountMinCount);
                }

                // Check if min count is unique or it is update of existing item
                if ((isMinCountUnique) || (vdi.VolumeDiscountID == volumeDiscountObj.VolumeDiscountID))
                {
                    // Sets data to database
                    VolumeDiscountInfoProvider.SetVolumeDiscountInfo(volumeDiscountObj);

                    string redirectUrl = "Product_Edit_VolumeDiscount_Edit.aspx?VolumeDiscountID=" + Convert.ToString(volumeDiscountObj.VolumeDiscountID) + "&saved=1&productID=" + productID;
                    URLHelper.Redirect(redirectUrl);
                }
                else
                {
                    // Show message
                    ShowError(GetString("product_edit_volumediscount_edit.minamountexists"));
                }
            }
            else
            {
                // Show message
                ShowError(errorMessage);
            }
        }
    }
}