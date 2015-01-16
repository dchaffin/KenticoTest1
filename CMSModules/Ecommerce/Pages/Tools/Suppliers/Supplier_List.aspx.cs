using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

[Title("Objects/Ecommerce_Supplier/object.png", "supplier_Edit.ItemListLink", "suppliers_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_Suppliers_Supplier_List : CMSSuppliersPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.ZeroRowsText = GetString("general.nodatafound");

        // Init site selector
        SelectSite.Selector.SelectedIndexChanged += new EventHandler(Selector_SelectedIndexChanged);

        if (!RequestHelper.IsPostBack())
        {
            SelectSite.SiteID = SiteFilterStartupValue;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // New supplier action
        hdrActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("supplier_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("Supplier_Edit.aspx?siteId=" + SelectSite.SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_Supplier/add.png")
        });

        // Set site selector visibility
        CurrentMaster.DisplaySiteSelectorPanel = AllowGlobalObjects;

        InitWhereCondition();

        // Try to select button in menu
        AddMenuButtonSelectScript("Suppliers", "");
    }


    protected override void OnPreRender(EventArgs e)
    {
        bool both = (SelectSite.SiteID == UniSelector.US_GLOBAL_OR_SITE_RECORD);

        // Hide header actions if (both) selected
        hdrActions.Enabled = !both;
        lblWarnNew.Visible = both;

        base.OnPreRender(e);

        gridElem.NamedColumns["SupplierSiteID"].Visible = AllowGlobalObjects;
    }

    #endregion


    #region "Event Handlers"

    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "suppenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "suppsiteid":
                return UniGridFunctions.ColoredSpanYesNo(parameter == DBNull.Value);
        }
        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("Supplier_Edit.aspx?supplierid=" + Convert.ToString(actionArgument) + "&siteId=" + SelectSite.SiteID);
        }
        else if (actionName == "delete")
        {
            int supplierId = ValidationHelper.GetInteger(actionArgument, 0);
            SupplierInfo supplierObj = SupplierInfoProvider.GetSupplierInfo(supplierId);

            // Check module permissions
            if (!ECommerceContext.IsUserAuthorizedToModifySupplier(supplierObj))
            {
                if (supplierObj.IsGlobal)
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
                }
                else
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifySuppliers");
                }
            }

            if (SupplierInfoProvider.CheckDependencies(supplierId))
            {
                // Show error message
                ShowError(GetString("Ecommerce.DeleteDisabled"));

                return;
            }

            // Delete SupplierInfo object from database
            SupplierInfoProvider.DeleteSupplierInfo(supplierObj);
            InitWhereCondition();
        }
    }


    /// <summary>
    /// Selection changed event.
    /// </summary>
    private void Selector_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitWhereCondition();
        gridElem.ReloadData();

        // Save selected value
        StoreSiteFilterValue(SelectSite.SiteID);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates where condition for UniGrid.
    /// </summary>
    private void InitWhereCondition()
    {
        gridElem.WhereCondition = SelectSite.GetSiteWhereCondition("SupplierSiteID");
    }

    #endregion
}