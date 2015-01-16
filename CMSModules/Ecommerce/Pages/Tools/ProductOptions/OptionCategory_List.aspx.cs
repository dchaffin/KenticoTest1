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

[Title("Objects/Ecommerce_OptionCategory/object.png", "optioncategory_edit.itemlistlink", "list_categories")]
public partial class CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_List : CMSProductOptionCategoriesPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        OptionCategoryGrid.OnAction += new OnActionEventHandler(OptionCategoryGrid_OnAction);
        OptionCategoryGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(OptionCategoryGrid_OnExternalDataBound);
        OptionCategoryGrid.ZeroRowsText = GetString("general.nodatafound");
        OptionCategoryGrid.OrderBy = "CategoryDisplayName ASC";

        // Init site selector
        SelectSite.Selector.SelectedIndexChanged += new EventHandler(Selector_SelectedIndexChanged);

        if (!RequestHelper.IsPostBack())
        {
            // Init site selector
            SelectSite.SiteID = SiteFilterStartupValue;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // New option category action
        hdrActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("OptionCategory_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("OptionCategory_New.aspx?siteId=" + SelectSite.SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_OptionCategory/add.png")
        });

        // Set site selector visibility
        CurrentMaster.DisplaySiteSelectorPanel = AllowGlobalObjects;

        OptionCategoryGrid.WhereCondition = SelectSite.GetSiteWhereCondition("CategorySiteID");

        AddMenuButtonSelectScript("ProductOptions", "");
    }


    protected override void OnPreRender(EventArgs e)
    {
        bool both = (SelectSite.SiteID == UniSelector.US_GLOBAL_OR_SITE_RECORD);

        // Hide header actions if (both) selected
        hdrActions.Enabled = !both;
        lblWarnNew.Visible = both;

        base.OnPreRender(e);
        OptionCategoryGrid.NamedColumns["CategorySiteID"].Visible = AllowGlobalObjects;
    }

    #endregion


    #region "Event Handlers"

    protected object OptionCategoryGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // convert selection type to text
            case "categoryselectiontype":
                switch (ValidationHelper.GetString(parameter, "").ToLowerCSafe())
                {
                    case "dropdown":
                        return GetString("OptionCategory_List.DropDownList");

                    case "checkboxhorizontal":
                        return GetString("OptionCategory_List.checkboxhorizontal");

                    case "checkboxvertical":
                        return GetString("OptionCategory_List.checkboxvertical");

                    case "radiobuttonhorizontal":
                        return GetString("OptionCategory_List.radiobuttonhorizontal");

                    case "radiobuttonvertical":
                        return GetString("OptionCategory_List.radiobuttonvertical");

                    case "textbox":
                        return GetString("optioncategory_selectiontype.textbox");

                    case "textarea":
                        return GetString("optioncategory_selectiontype.textarea");
                }
                break;

            case "categoryenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "categorysiteid":
                return UniGridFunctions.ColoredSpanYesNo(parameter == DBNull.Value);
        }

        return parameter;
    }


    /// <summary>
    /// Handles the OptionCategoryGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void OptionCategoryGrid_OnAction(string actionName, object actionArgument)
    {
        int categoryId = ValidationHelper.GetInteger(actionArgument, 0);

        // Set actions
        if (actionName == "edit")
        {
            URLHelper.Redirect("OptionCategory_Edit.aspx?CategoryID=" + categoryId + "&siteId=" + SelectSite.SiteID);
        }
        else if (actionName == "delete")
        {
            OptionCategoryInfo categoryObj = OptionCategoryInfoProvider.GetOptionCategoryInfo(categoryId);

            if (!ECommerceContext.IsUserAuthorizedToModifyOptionCategory(categoryObj))
            {
                // Check module permissions
                if (categoryObj.CategoryIsGlobal)
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
                }
                else
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyProducts");
                }
            }

            // Check dependencies
            if (OptionCategoryInfoProvider.CheckDependencies(categoryId))
            {
                // Show error message
                ShowError(GetString("Ecommerce.DeleteDisabled"));

                return;
            }

            // Delete option category from database
            OptionCategoryInfoProvider.DeleteOptionCategoryInfo(categoryObj);
        }
    }


    /// <summary>
    /// Handles the SiteSelector's selection changed event.
    /// </summary>
    private void Selector_SelectedIndexChanged(object sender, EventArgs e)
    {
        OptionCategoryGrid.WhereCondition = SelectSite.GetSiteWhereCondition("CategorySiteID");
        OptionCategoryGrid.ReloadData();

        // Save selected value
        StoreSiteFilterValue(SelectSite.SiteID);
    }

    #endregion
}