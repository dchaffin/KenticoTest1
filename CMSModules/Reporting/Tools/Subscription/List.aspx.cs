using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.CMSHelper;
using CMS.Reporting;


public partial class CMSModules_Reporting_Tools_Subscription_List : CMSReportingPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUserInfo cui = CMSContext.CurrentUser;
        if (!(cui.IsAuthorizedPerResource("cms.reporting", "subscribe") || cui.IsAuthorizedPerResource("cms.reporting", "modify")))
        {
            RedirectToAccessDenied("cms.reporting", "Subscribe");
        }

        InitActions();

        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.EditActionUrl = "Edit.aspx?subscriptionId={0}&reportid=" + QueryHelper.GetInteger("reportid", 0);
        gridElem.WhereCondition = String.Format("ReportSubscriptionReportID={0} AND ReportSubscriptionSiteID={1}", QueryHelper.GetInteger("ReportID", 0), CMSContext.CurrentSiteID);
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
    }


    void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            int id = ValidationHelper.GetInteger(actionArgument, 0);

            CurrentUserInfo cui = CMSContext.CurrentUser;
            bool haveModify = cui.IsAuthorizedPerResource("cms.reporting", "modify");
            if (!(cui.IsAuthorizedPerResource("cms.reporting", "subscribe") || haveModify))
            {
                RedirectToAccessDenied("cms.reporting", "Subscribe");
            }

            if (!cui.IsAuthorizedPerResource("cms.reporting", "modify"))
            {
                ReportSubscriptionInfo rsi = ReportSubscriptionInfoProvider.GetReportSubscriptionInfo(id);
                if ((rsi != null) && (rsi.ReportSubscriptionUserID != cui.UserID))
                {
                    RedirectToAccessDenied(GetString("reportsubscription.onlymodifyusersallowed"));
                }
            }

            ReportSubscriptionInfoProvider.DeleteReportSubscriptionInfo(id);
        }
    }


    object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "enabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "username":
                String userName = ValidationHelper.GetString(parameter, String.Empty);
                return Functions.GetFormattedUserName(userName);
        }
        return sender;
    }


    /// <summary>
    /// Gets string array representing header actions.
    /// </summary>
    /// <returns>Array of strings</returns>
    private void InitActions()
    {
        HeaderAction checkin = new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("reportsubscription.new"),
            SmallImageUrl = GetImageUrl("CMSModules/CMS_Reporting/Subscription.png"),
            RedirectUrl = "Edit.aspx?reportID=" + QueryHelper.GetInteger("reportID", 0)
        };
        CurrentMaster.HeaderActions.ActionsList.Add(checkin);
    }
}
