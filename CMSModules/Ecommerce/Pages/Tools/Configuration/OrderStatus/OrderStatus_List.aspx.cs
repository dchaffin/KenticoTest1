using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

[Title("Objects/Ecommerce_OrderStatus/object.png", "OrderStatus_List.HeaderCaption", "order_status_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_OrderStatus_OrderStatus_List : CMSOrderStatusesPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionPerformed += HeaderActions_ActionPerformed;

        // New item action
        actions.ActionsList.Add(new HeaderAction()
        {
            Text = GetString("OrderStatus_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("OrderStatus_Edit.aspx?siteId=" + SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_OrderStatus/add.png"),
            ControlType = HeaderActionTypeEnum.Hyperlink
        });

        // Show copy from global link when not configuring global statuses.
        if (ConfiguredSiteID != 0)
        {
            // Show "Copy from global" link only if there is at least one global status
            DataSet ds = OrderStatusInfoProvider.GetOrderStatuses(0, false);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                actions.ActionsList.Add(new HeaderAction()
                {
                    Text = GetString("general.copyfromglobal"),
                    OnClientClick = "return ConfirmCopyFromGlobal();",
                    ImageUrl = GetImageUrl("Objects/Ecommerce_OrderStatus/fromglobal.png"),
                    CommandName = "copyFromGlobal",
                    ControlType = HeaderActionTypeEnum.LinkButton
                });

                // Register javascript to confirm generate 
                string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmOrderStatusFromGlobal")) + ");}";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromGlobal", ScriptHelper.GetScript(script));
            }
        }

        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.GridView.AllowSorting = false;

        // Configuring global records from specific site
        if ((ConfiguredSiteID == 0) && (SiteID != 0))
        {
            ShowInformation(GetString("com.UsingGlobalSettings"));
        }

        // Filter records by site
        gridElem.WhereCondition = "ISNULL(StatusSiteID, 0) = " + ConfiguredSiteID;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Check if at least one enabled status is present
        CheckEnabledStatusPresence();
    }

    #endregion


    #region "Event Handlers"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "copyfromglobal":
                // Check permissions
                CheckConfigurationModification();

                // Copy and reload
                OrderStatusInfoProvider.CopyFromGlobal(ConfiguredSiteID);
                gridElem.ReloadData();
                break;
        }
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "statuscolor":
                if (parameter != DBNull.Value)
                {
                    return "<div style=\"height:13px; widht=25px; background-color:" + parameter.ToString() + "\"></div>";
                }

                return "-";
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
        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("OrderStatus_Edit.aspx?orderstatusid=" + Convert.ToString(actionArgument) + "&siteId=" + SiteID);
                break;

            case "delete":
                CheckConfigurationModification();

                if (OrderStatusInfoProvider.CheckDependencies(ValidationHelper.GetInteger(actionArgument, 0)))
                {
                    // Show error message
                    ShowError(GetString("Ecommerce.DeleteDisabled"));

                    return;
                }

                // delete OrderStatusInfo object from database
                OrderStatusInfoProvider.DeleteOrderStatusInfo(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "moveup":
                CheckConfigurationModification();

                OrderStatusInfoProvider.MoveStatusUp(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "movedown":
                CheckConfigurationModification();

                OrderStatusInfoProvider.MoveStatusDown(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Checks if at least one enabled order status is present.
    /// </summary>
    private void CheckEnabledStatusPresence()
    {
        DataSet ds = OrderStatusInfoProvider.GetOrderStatuses(ConfiguredSiteID, true);
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            ShowWarning(GetString("com.orderstatus.noenabledfound"), null, null);
        }
    }

    #endregion
}