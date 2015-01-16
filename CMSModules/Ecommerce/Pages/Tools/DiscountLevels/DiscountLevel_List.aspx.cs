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

[Title("Objects/Ecommerce_DiscountLevel/object.png", "DiscountLevel_Edit.ItemListLink", "discount_level_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_DiscountLevels_DiscountLevel_List : CMSDiscountLevelsPage
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
            // Init site selector
            SelectSite.SiteID = SiteFilterStartupValue;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // New level action
        hdrActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("DiscountLevel_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("DiscountLevel_New.aspx?siteId=" + SelectSite.SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_DiscountLevel/add.png")
        });

        // Set site selector visibility
        CurrentMaster.DisplaySiteSelectorPanel = AllowGlobalObjects;

        AddMenuButtonSelectScript("DiscountLevels", "");

        InitWhereCondition();
    }


    protected override void OnPreRender(EventArgs e)
    {
        bool both = (SelectSite.SiteID == UniSelector.US_GLOBAL_OR_SITE_RECORD);

        // Hide header actions if (both) selected
        hdrActions.Enabled = !both;
        lblWarnNew.Visible = both;

        base.OnPreRender(e);
        gridElem.NamedColumns["DiscountLevelSiteID"].Visible = AllowGlobalObjects;
    }

    #endregion


    #region "Event Handlers"

    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "discountvalue":
                DataRowView row = (DataRowView)parameter;
                double value = ValidationHelper.GetDouble(row["DiscountLevelValue"], 0);
                return value.ToString() + "%";

            case "disclevelenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "discountlevelsiteid":
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
            URLHelper.Redirect("DiscountLevel_Edit_Frameset.aspx?discountLevelId=" + Convert.ToString(actionArgument) + "&siteId=" + SelectSite.SiteID);
        }
        else if (actionName == "delete")
        {
            int id = ValidationHelper.GetInteger(actionArgument, 0);

            DiscountLevelInfo dicountLevelInfoObj = DiscountLevelInfoProvider.GetDiscountLevelInfo(id);
            // Nothing to delete
            if (dicountLevelInfoObj == null)
            {
                return;
            }

            // Check module permissions
            if (!ECommerceContext.IsUserAuthorizedToModifyDiscountLevel(dicountLevelInfoObj))
            {
                if (dicountLevelInfoObj.IsGlobal)
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
                }
                else
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyDiscounts");
                }
            }

            // Check dependencies
            if (DiscountLevelInfoProvider.CheckDependencies(id))
            {
                //Show error message
                ShowError(GetString("Ecommerce.DeleteDisabled"));

                return;
            }

            // Delete DiscountlLevelInfo object from database
            DiscountLevelInfoProvider.DeleteDiscountLevelInfo(dicountLevelInfoObj);
        }
    }


    /// <summary>
    /// Handles the SiteSelector's selection changed event.
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
        gridElem.WhereCondition = SelectSite.GetSiteWhereCondition("DiscountLevelSiteID");
    }

    #endregion
}