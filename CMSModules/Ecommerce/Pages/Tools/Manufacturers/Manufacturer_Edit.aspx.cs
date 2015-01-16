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
[EditedObject(PredefinedObjectType.MANUFACTURER, "manufacturerid")]
// Title
[Title("Objects/Ecommerce_Manufacturer/object.png", "com.manufacturer.edit", "newedit_manufacturer", ExistingObject = true)]
[Title("Objects/Ecommerce_Manufacturer/new.png", "Manufacturer_List.NewItemCaption", "newedit_manufacturer", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Manufacturers_Manufacturer_Edit : CMSManufacturersPage
{
    #region "Variables"

    protected int manufacturerId = 0;
    protected int editedSiteId = -1;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Field validator error messages initialization
        rfvDisplayName.ErrorMessage = GetString("manufacturer_Edit.errorEmptyDisplayName");

        string currentManufacturer = GetString("Manufacturer_Edit.NewItemCaption");

        // Get manufacturer id from querystring		
        manufacturerId = QueryHelper.GetInteger("manufacturerId", 0);
        editedSiteId = ConfiguredSiteID;
        if (manufacturerId > 0)
        {
            ManufacturerInfo manufacturerObj = EditedObject as ManufacturerInfo;

            if (manufacturerObj != null)
            {
                currentManufacturer = manufacturerObj.ManufacturerDisplayName;

                editedSiteId = manufacturerObj.ManufacturerSiteID;

                // Check edited object site ID
                CheckEditedObjectSiteID(editedSiteId);

                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(manufacturerObj);

                    // Show that the manufacturer was created or updated successfully
                    if (ValidationHelper.GetString(Request.QueryString["saved"], "") == "1")
                    {
                        ShowChangesSaved();
                    }
                }
            }
        }

        // Init file uploader
        file.ObjectID = manufacturerId;
        file.ObjectType = ECommerceObjectType.MANUFACTURER;
        file.Category = MetaFileInfoProvider.OBJECT_CATEGORY_THUMBNAIL;
        file.SiteID = editedSiteId;

        // Initializes page title control		
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("Manufacturer_Edit.ItemListLink");
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Manufacturers/Manufacturer_List.aspx?siteId=" + SiteID;
        breadcrumbs[0, 2] = "";
        breadcrumbs[1, 0] = FormatBreadcrumbObjectName(currentManufacturer, editedSiteId);
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = breadcrumbs;

        AddMenuButtonSelectScript("Manufacturers", "");
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Load data of editing manufacturer.
    /// </summary>
    /// <param name="manufacturerObj">Manufacturer object</param>
    protected void LoadData(ManufacturerInfo manufacturerObj)
    {
        txtManufacturerDisplayName.Text = manufacturerObj.ManufacturerDisplayName;
        txtManufacturerName.Text = manufacturerObj.ManufacturerName;
        txtDescription.Text = manufacturerObj.ManufacturerDescription;
        txtManufactureHomepage.Text = manufacturerObj.ManufacturerHomepage;
        chkManufacturerEnabled.Checked = manufacturerObj.ManufacturerEnabled;
        chkIsImportant.Checked = manufacturerObj.ManufacturerIsImportant;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check module permissions
        bool global = (editedSiteId <= 0);
        if (!ECommerceContext.IsUserAuthorizedToModifyManufacturer(global))
        {
            if (global)
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
            }
            else
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyManufacturers");
            }
        }

        // Check input values from textboxes and other controls - new Validator()
        string errorMessage = new Validator().
            NotEmpty(txtManufacturerDisplayName.Text.Trim(), GetString("manufacturer_Edit.errorEmptyDisplayName")).
            IsCodeName(txtManufacturerName.Text.Trim(), GetString("general.invalidcodename")).Result;

        if (errorMessage == "")
        {
            ManufacturerInfo manufacturerObj = null;

            // Manufacturer code name must be unique
            string siteWhere = (ConfiguredSiteID > 0) ? " AND (ManufacturerSiteID = " + ConfiguredSiteID + " OR ManufacturerSiteID IS NULL)" : "";
            DataSet ds = ManufacturerInfoProvider.GetManufacturers("ManufacturerName = N'" + SqlHelperClass.GetSafeQueryString(txtManufacturerName.Text.Trim(), false) + "'" + siteWhere, null, 1, null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                manufacturerObj = new ManufacturerInfo(ds.Tables[0].Rows[0]);
            }

            if ((manufacturerObj == null) || (manufacturerObj.ManufacturerID == manufacturerId))
            {
                if (manufacturerObj == null)
                {
                    manufacturerObj = ManufacturerInfoProvider.GetManufacturerInfo(manufacturerId);
                    if (manufacturerObj == null)
                    {
                        // Create new manufacturer
                        manufacturerObj = new ManufacturerInfo();
                        manufacturerObj.ManufacturerSiteID = ConfiguredSiteID;
                    }
                }

                manufacturerObj.ManufacturerID = manufacturerId;
                manufacturerObj.ManufacturerDisplayName = txtManufacturerDisplayName.Text.Trim();
                manufacturerObj.ManufacturerName = txtManufacturerName.Text.Trim();
                manufacturerObj.ManufacturerDescription = txtDescription.Text.Trim();
                manufacturerObj.ManufacturerHomepage = txtManufactureHomepage.Text.Trim();
                manufacturerObj.ManufacturerEnabled = chkManufacturerEnabled.Checked;
                manufacturerObj.ManufacturerIsImportant = chkIsImportant.Checked;

                bool isNew = (manufacturerObj.ManufacturerID <= 0);

                // Save changes
                ManufacturerInfoProvider.SetManufacturerInfo(manufacturerObj);

                if (isNew)
                {
                    file.ObjectID = manufacturerObj.ManufacturerID;
                    file.UploadFile();
                }

                URLHelper.Redirect("Manufacturer_Edit.aspx?manufacturerId=" + Convert.ToString(manufacturerObj.ManufacturerID) + "&saved=1&siteId=" + SiteID.ToString());
            }
            else
            {
                // Show error message
                ShowError(GetString("com.manufacturernameexists"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }

    #endregion
}