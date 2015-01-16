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

[Security(Resource = "CMS.Ecommerce", UIElements = "Products.VolumeDiscounts")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_VolumeDiscount_Edit : CMSProductsAdvancedPage
{
    protected int volumeDiscountID = 0;
    protected VolumeDiscountInfo volumeDiscountObj = null;
    protected string currencyCode = "";
    protected CurrencyInfo currency = null;
    protected bool dialog = false;
    protected SKUInfo sku = null;


    protected void Page_PreInit(object sender, EventArgs e)
    {
        // Remove "tabs design" when editing in modal dialog
        dialog = QueryHelper.GetBoolean("modaldialog", false);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(this);

        volumeDiscountID = QueryHelper.GetInteger("VolumeDiscountID", 0);

        sku = SKUInfoProvider.GetSKUInfo(ProductID);
        if (sku != null)
        {
            CheckEditedObjectSiteID(sku.SKUSiteID);

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

        string titleText = "";
        string titleImage = "";

        // Init labels
        lblVolumeDiscountValue.Text = GetString("product_edit_volumediscount_edit.volumediscountvaluelabel");
        lblVolumeDiscountMinCount.Text = GetString("product_edit_volumediscount_edit.volumediscountmincountlabel");
        radDiscountAbsolute.Text = GetString("product_edit_volumediscount_edit.rbdiscountabsolute");
        radDiscountRelative.Text = GetString("product_edit_volumediscount_edit.rbdiscountrelative");

        // Init value validator error messages
        rfvDiscountValue.ErrorMessage = GetString("product_edit_volumediscount_edit.rfvDiscountValue");
        rvDiscountValue.ErrorMessage = GetString("product_edit_volumediscount_edit.rvDiscountValue");
        rvDiscountValue.MaximumValue = int.MaxValue.ToString();

        // Init min count validator error messages
        rfvMinCount.ErrorMessage = GetString("product_edit_volumediscount_edit.rfvMinCount");
        rvMinCount.ErrorMessage = GetString("product_edit_volumediscount_edit.rvMinCount");
        rvMinCount.MaximumValue = int.MaxValue.ToString();

        radDiscountAbsolute.Attributes["onclick"] = "jQuery('span[id*=\"lblCurrency\"]').html(" + ScriptHelper.GetString("(" + currencyCode + ")") + ")";
        radDiscountRelative.Attributes["onclick"] = "jQuery('span[id*=\"lblCurrency\"]').html('(%)')";

        string currentVolumeDiscount = GetString("Product_Edit_VolumeDiscount_Edit.NewItemCaption");

        // If true, then we will edit existing record                
        if (volumeDiscountID > 0)
        {
            // Check if there is already VolumeDiscountInfo with this volumeDiscountID 
            volumeDiscountObj = VolumeDiscountInfoProvider.GetVolumeDiscountInfo(volumeDiscountID);
            if (volumeDiscountID > 0)
            {
                EditedObject = volumeDiscountObj;
            }

            if (volumeDiscountObj != null)
            {
                // Fill editing form with existing data when not postback
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(volumeDiscountObj);
                    // Set caption of edited item
                    currentVolumeDiscount = GetString("product_edit_volumediscount_edit.edittitletext");

                    // Show that the volumeDiscount was created or updated successfully
                    if (QueryHelper.GetString("saved", "") == "1")
                    {
                        // Show message
                        ShowChangesSaved();

                        // Refresh parent page when editing in modal dialog
                        if (dialog)
                        {
                            ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshPage();");
                        }
                    }
                }
            }
            // Set page title to "volume discount properties"
            titleText = GetString("product_edit_volumediscount_edit.edittitletext");
            titleImage = GetImageUrl("Objects/Ecommerce_VolumeDiscount/object.png");
        }
        // Do this when creating new discount
        else
        {
            // Set default radio button value
            if (!URLHelper.IsPostback())
            {
                radDiscountRelative.Checked = true;
                radDiscountAbsolute.Checked = false;
            }

            // Init page header to "new item"		
            titleText = GetString("product_edit_volumediscount_edit.newitemcaption");
            titleImage = GetImageUrl("Objects/Ecommerce_VolumeDiscount/new.png");
        }

        // Show breadcrumbs only when not editing in modal dialog
        if (!dialog)
        {
            // Initializes breadcrumb master page element
            InitializeBreadcrumbs(currentVolumeDiscount);
        }
        else
        {
            // Change validators location
            spanMinCount.Attributes["style"] = "display:block";
            spanValue.Attributes["style"] = "display:block";
        }

        InitializeMasterPage(titleText, titleImage);

        RegisterEscScript();
        RegisterModalPageScripts();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblCurrency.Text = string.Format("({0})", radDiscountAbsolute.Checked ? currencyCode : "%");
    }


    private void InitializeBreadcrumbs(string currentVolumeDiscount)
    {
        string listringUrl = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_Edit_VolumeDiscount_List.aspx?productID=" + ProductID.ToString() + "&siteId=" + SiteID;

        // Initializes page title control		
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("Product_Edit_VolumeDiscount_Edit.ItemListLink");
        breadcrumbs[0, 1] = AddNodeIDParameterToUrl(listringUrl);
        breadcrumbs[0, 2] = "";
        breadcrumbs[1, 0] = currentVolumeDiscount;
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = breadcrumbs;
    }


    /// <summary>
    /// Initializes master page elements.
    /// </summary>
    /// <param name="titleText">Master page title text</param>
    /// <param name="titleImage">URL of the image of the master page title</param>
    /// <param name="currentVolumeDiscount">Current volume discount</param>
    private void InitializeMasterPage(string titleText, string titleImage)
    {
        // Set the master page title
        CurrentMaster.Title.HelpTopicName = "newedit_discount";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Set master title
        if (DisplayTitle)
        {
            CurrentMaster.Title.TitleText = titleText;
            CurrentMaster.Title.TitleImage = titleImage;
        }
    }


    /// <summary>
    /// Load data of editing volumeDiscount.
    /// </summary>
    /// <param name="volumeDiscountObj">VolumeDiscount object</param>
    protected void LoadData(VolumeDiscountInfo volumeDiscountObj)
    {
        // Load data from database
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
            string errorMessage = new Validator().NotEmpty(txtVolumeDiscountValue.Text, "product_edit_volumediscount_edit.volumediscountvaluelabel")
                .NotEmpty(txtVolumeDiscountMinCount.Text, "product_edit_volumediscount_edit.volumediscountmincountlabel").Result;

            // Discount value validation
            if (errorMessage == "")
            {
                // Relative
                if (radDiscountRelative.Checked && !ValidationHelper.IsInRange(0, 100, ValidationHelper.GetDouble(txtVolumeDiscountValue.Text.Trim(), -1)))
                {
                    errorMessage = GetString("Com.Error.RelativeDiscountValue");
                }
                // Absolute
                else if (radDiscountAbsolute.Checked && !ValidationHelper.IsPositiveNumber(ValidationHelper.GetDouble(txtVolumeDiscountValue.Text.Trim(), -1)))
                {
                    errorMessage = GetString("Com.Error.AbsoluteDiscountValue");
                }
            }

            if (errorMessage == "")
            {
                VolumeDiscountInfo volumeDiscountObj = VolumeDiscountInfoProvider.GetVolumeDiscountInfo(volumeDiscountID);
                // If volumeDiscount doesnt already exist, create new one
                if (volumeDiscountObj == null)
                {
                    // Create new volume discount
                    volumeDiscountObj = new VolumeDiscountInfo();

                    // When creating new one, set his SKUID to productID (obtained from URL)
                    volumeDiscountObj.VolumeDiscountSKUID = ProductID;
                }

                // Set volumeDiscountObj values
                volumeDiscountObj.VolumeDiscountValue = Convert.ToDouble(txtVolumeDiscountValue.Text.Trim());
                volumeDiscountObj.VolumeDiscountMinCount = Convert.ToInt32(txtVolumeDiscountMinCount.Text.Trim());
                volumeDiscountObj.VolumeDiscountIsFlatValue = radDiscountAbsolute.Checked;

                // Set isMinCountUnique
                VolumeDiscountInfo vdi = VolumeDiscountInfoProvider.GetVolumeDiscountInfo(ProductID, volumeDiscountObj.VolumeDiscountMinCount);
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
                    string dialogString = "";
                    if (dialog)
                    {
                        dialogString = "&modaldialog=1";
                    }

                    // Sets data to database
                    VolumeDiscountInfoProvider.SetVolumeDiscountInfo(volumeDiscountObj);
                    string redirectUrl = "Product_Edit_VolumeDiscount_Edit.aspx?VolumeDiscountID=" + volumeDiscountObj.VolumeDiscountID + "&siteId=" + SiteID + "&saved=1&productID=" + ProductID + dialogString;
                    URLHelper.Redirect(AddNodeIDParameterToUrl(redirectUrl));
                }
                else
                {
                    // Show error message
                    ShowError(GetString("product_edit_volumediscount_edit.minamountexists"));
                }
            }
            else
            {
                // Show error message
                ShowError(errorMessage);
            }
        }
    }
}