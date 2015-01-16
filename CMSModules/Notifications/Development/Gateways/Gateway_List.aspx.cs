using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.Notifications;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Notifications_Development_Gateways_Gateway_List : CMSNotificationsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // New item link
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("notification.gateways_list.newitem");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("Gateway_Edit.aspx");
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/Notification_Gateway/add.png");
        CurrentMaster.HeaderActions.Actions = actions;

        // Set up unigrid options
        unigridGatewayList.OrderBy = "GatewayDisplayName";
        unigridGatewayList.OnAction += new OnActionEventHandler(unigridGatewayList_OnAction);
        unigridGatewayList.OnExternalDataBound += new OnExternalDataBoundEventHandler(unigridGatewayList_OnExternalDataBound);
        unigridGatewayList.ZeroRowsText = GetString("general.nodatafound");
    }

    #endregion


    #region "Unigrid events"

    protected object unigridGatewayList_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe() == "gatewayenabled")
        {
            return UniGridFunctions.ColoredSpanYesNo(parameter);
        }
        return parameter;
    }


    protected void unigridGatewayList_OnAction(string actionName, object actionArgument)
    {
        int gatewayId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("Gateway_Edit.aspx?gatewayid=" + gatewayId);
                break;

            case "delete":
                NotificationGatewayInfoProvider.DeleteNotificationGatewayInfo(gatewayId);
                break;
        }
    }

    #endregion
}