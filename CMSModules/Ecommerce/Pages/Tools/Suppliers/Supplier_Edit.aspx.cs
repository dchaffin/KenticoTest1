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

// Edited object
[EditedObject(PredefinedObjectType.SUPPLIER, "supplierid")]
// Title
[Title("Objects/Ecommerce_Supplier/object.png", "com.supplier.edit", "newedit_supplier", ExistingObject = true)]
[Title("Objects/Ecommerce_Supplier/new.png", "supplier_List.NewItemCaption", "newedit_supplier", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Suppliers_Supplier_Edit : CMSSuppliersPage
{
    #region "Variables"

    protected int mSupplierId = 0;
    protected int editedSiteId = -1;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        rfvDisplayName.ErrorMessage = GetString("supplier_Edit.errorEmptyDisplayName");

        string currentSupplier = GetString("supplier_Edit.NewItemCaption");

        // Get supplier ID from querystring
        mSupplierId = QueryHelper.GetInteger("supplierid", 0);
        editedSiteId = ConfiguredSiteID;

        if (mSupplierId > 0)
        {
            SupplierInfo supplierObj = EditedObject as SupplierInfo;

            if (supplierObj != null)
            {
                currentSupplier = supplierObj.SupplierDisplayName;
                // Store site id of edited supplier
                editedSiteId = supplierObj.SupplierSiteID;

                //Check site id of edited supplier
                CheckEditedObjectSiteID(editedSiteId);

                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(supplierObj);

                    // Show that the supplier was created or updated successfully
                    if (QueryHelper.GetBoolean("saved", false))
                    {
                        // Show message
                        ShowChangesSaved();
                    }
                }
            }
        }

        // Initializes page title breadcrumbs control		
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("supplier_Edit.ItemListLink");
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Suppliers/Supplier_List.aspx?siteId=" + SiteID;
        breadcrumbs[0, 2] = "";
        breadcrumbs[1, 0] = FormatBreadcrumbObjectName(currentSupplier, editedSiteId);
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = breadcrumbs;

        AddMenuButtonSelectScript("Suppliers", "");
    }


    /// <summary>
    /// Load data of editing supplier.
    /// </summary>
    /// <param name="supplierObj">Supplier object</param>
    protected void LoadData(SupplierInfo supplierObj)
    {
        EditedObject = supplierObj;

        txtSupplierFax.Text = supplierObj.SupplierFax;
        txtSupplierEmail.Text = supplierObj.SupplierEmail;
        txtSupplierPhone.Text = supplierObj.SupplierPhone;
        txtSupplierDisplayName.Text = supplierObj.SupplierDisplayName;
        txtSupplierName.Text = supplierObj.SupplierName;
        chkSupplierEnabled.Checked = supplierObj.SupplierEnabled;
    }


    /// <summary>
    /// Sets the data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check module permissions
        bool global = (editedSiteId <= 0);
        if (!ECommerceContext.IsUserAuthorizedToModifySupplier(global))
        {
            if (global)
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
            }
            else
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifySuppliers");
            }
        }

        string errorMessage = new Validator().
            NotEmpty(txtSupplierDisplayName.Text.Trim(), GetString("supplier_Edit.errorEmptyDisplayName")).
            IsCodeName(txtSupplierName.Text.Trim(), GetString("general.invalidcodename")).Result;

        // Validate email format if not empty
        if (errorMessage == "")
        {
            if ((txtSupplierEmail.Text.Trim() != "") && (!ValidationHelper.IsEmail(txtSupplierEmail.Text.Trim())))
            {
                errorMessage = GetString("supplier_Edit.errorEmailFormat");
            }
        }

        if (errorMessage == "")
        {
            SupplierInfo supplierObj = null;

            // Supplier code name must be unique
            string siteWhere = (ConfiguredSiteID > 0) ? " AND (SupplierSiteID = " + ConfiguredSiteID + " OR SupplierSiteID IS NULL)" : "";
            DataSet ds = SupplierInfoProvider.GetSuppliers("SupplierName = N'" + SqlHelperClass.GetSafeQueryString(txtSupplierName.Text.Trim(), false) + "'" + siteWhere, null, 1, null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                supplierObj = new SupplierInfo(ds.Tables[0].Rows[0]);
            }

            if ((supplierObj == null) || (supplierObj.SupplierID == mSupplierId))
            {
                if (supplierObj == null)
                {
                    supplierObj = SupplierInfoProvider.GetSupplierInfo(mSupplierId);
                    if (supplierObj == null)
                    {
                        // Create new supplier
                        supplierObj = new SupplierInfo();
                        supplierObj.SupplierSiteID = ConfiguredSiteID;
                    }
                }

                supplierObj.SupplierFax = txtSupplierFax.Text.Trim();
                supplierObj.SupplierEmail = txtSupplierEmail.Text.Trim();
                supplierObj.SupplierPhone = txtSupplierPhone.Text.Trim();
                supplierObj.SupplierDisplayName = txtSupplierDisplayName.Text.Trim();
                supplierObj.SupplierName = txtSupplierName.Text.Trim();
                supplierObj.SupplierEnabled = chkSupplierEnabled.Checked;

                // Save changes
                SupplierInfoProvider.SetSupplierInfo(supplierObj);

                URLHelper.Redirect("Supplier_Edit.aspx?supplierid=" + Convert.ToString(supplierObj.SupplierID) + "&saved=1&siteId=" + SiteID);
            }
            else
            {
                // Show error message
                ShowError(GetString("com.suppliernameexists"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }
}