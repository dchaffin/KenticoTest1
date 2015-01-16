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

[Title("Objects/Ecommerce_Manufacturer/object.png", "Manufacturer_Edit.ItemListLink", "manufacturer_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_Manufacturers_Manufacturer_List : CMSManufacturersPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        UniGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(UniGrid_OnExternalDataBound);
        UniGrid.ZeroRowsText = GetString("general.nodatafound");

        // Init site selector
        SelectSite.Selector.SelectedIndexChanged += new EventHandler(Selector_SelectedIndexChanged);

        if (!RequestHelper.IsPostBack())
        {
            SelectSite.SiteID = SiteFilterStartupValue;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // New manufacturer action
        hdrActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("Manufacturer_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("Manufacturer_Edit.aspx?siteId=" + SelectSite.SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_Manufacturer/add.png")
        });

        // Set site selector visibility
        CurrentMaster.DisplaySiteSelectorPanel = AllowGlobalObjects;

        InitWhereCondition();

        // Try to select button in menu
        AddMenuButtonSelectScript("Manufacturers", "");
    }


    protected override void OnPreRender(EventArgs e)
    {
        bool both = (SelectSite.SiteID == UniSelector.US_GLOBAL_OR_SITE_RECORD);

        // Hide header actions if (both) selected
        hdrActions.Enabled = !both;
        lblWarnNew.Visible = both;

        base.OnPreRender(e);

        UniGrid.NamedColumns["ManufacturerSiteID"].Visible = AllowGlobalObjects;
    }

    #endregion


    #region "Event Handlers"

    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe() == "manufacturersiteid")
        {
            return UniGridFunctions.ColoredSpanYesNo(parameter == DBNull.Value);
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        int manufacturerId = ValidationHelper.GetInteger(actionArgument, 0);

        if (actionName == "edit")
        {
            URLHelper.Redirect("Manufacturer_Edit.aspx?manufacturerId=" + manufacturerId + "&siteId=" + SelectSite.SiteID);
        }
        else if (actionName == "delete")
        {
            // Check module permissions
            ManufacturerInfo manufacturerObj = ManufacturerInfoProvider.GetManufacturerInfo(manufacturerId);
            if (!ECommerceContext.IsUserAuthorizedToModifyManufacturer(manufacturerObj))
            {
                if (manufacturerObj.IsGlobal)
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
                }
                else
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyManufacturers");
                }
            }

            // Check dependencies
            if (ManufacturerInfoProvider.CheckDependencies(manufacturerId))
            {
                // Show error message
                ShowError(GetString("Ecommerce.DeleteDisabled"));

                return;
            }

            // Delete ManufacturerInfo object from database
            ManufacturerInfoProvider.DeleteManufacturerInfo(manufacturerObj);
            InitWhereCondition();
        }
    }


    /// <summary>
    /// Handles the SiteSelector's selection changed event.
    /// </summary>
    private void Selector_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitWhereCondition();
        UniGrid.ReloadData();

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
        UniGrid.WhereCondition = SelectSite.GetSiteWhereCondition("ManufacturerSiteID");
    }

    #endregion
}