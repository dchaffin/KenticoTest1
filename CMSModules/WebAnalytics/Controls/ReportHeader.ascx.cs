using System;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_WebAnalytics_Controls_ReportHeader : CMSAdminControl
{
    #region "Variables"

    private bool mManageData = true;
    private string mPrintPageURL = "~/CMSModules/Reporting/Tools/Analytics_Print.aspx";
    private HitsIntervalEnum mSelectedInterval = HitsIntervalEnum.None;
    private string mPanelCssClass = "CMSEditMenu";

    #endregion


    #region "Properties"

    /// <summary>
    /// Header actions
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return headerActions;
        }
    }


    /// <summary>
    /// CSS class for header actions container. Default class is CMSEditMenu.
    /// </summary>
    public string PanelCssClass
    {
        get
        {
            return mPanelCssClass;
        }
        set
        {
            mPanelCssClass = value;
        }
    }


    /// <summary>
    /// Datarow with report's parameters.
    /// </summary>
    public DataRow ReportParameters
    {
        get;
        set;
    }


    /// <summary>
    /// Report's name.
    /// </summary>
    public String ReportName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates report's interval (hour,day,week,month,year,none).
    /// </summary>
    public HitsIntervalEnum SelectedInterval
    {
        get
        {
            return mSelectedInterval;
        }
        set
        {
            mSelectedInterval = value;
        }
    }


    /// <summary>
    /// Set/get codename (for parameter datacodename) which is passed to manage analytics data dialog.
    /// </summary>
    public String ManageDataCodeName
    {
        get;
        set;
    }


    /// <summary>
    /// If true, button for manage analytics data is displayed.
    /// </summary>
    public bool DisplayManageData
    {
        get
        {
            return mManageData;
        }
        set
        {
            mManageData = value;
        }
    }


    /// <summary>
    /// Gets or sets the print page URL for the print action.
    /// </summary>
    public string PrintPageURL
    {
        get
        {
            return mPrintPageURL;
        }
        set
        {
            mPrintPageURL = value;
        }
    }

    #endregion


    #region "Events"

    public event CommandEventHandler ActionPerformed;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        headerActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    protected override void OnPreRender(EventArgs e)
    {
        string dataCodeName = string.IsNullOrEmpty(ManageDataCodeName) ? QueryHelper.GetString("dataCodeName", string.Empty) : ManageDataCodeName;

        string deleteScript = string.Format("modalDialog('{0}?statcodename={1}','AnalyticsManageData',{2},{3});",
            ResolveUrl("~/CMSModules/Reporting/WebAnalytics/Analytics_ManageData.aspx"),
            dataCodeName,
            AnalyticsHelper.MANAGE_WINDOW_WIDTH,
            AnalyticsHelper.MANAGE_WINDOW_HEIGHT);

        string printScript = string.Format("myModalDialog('{0}?reportname={1}&parameters={2}&UILang={3}','PrintReport {1}',800,700);return false",
            ResolveUrl(PrintPageURL),
            ReportName,
            AnalyticsHelper.GetQueryStringParameters(ReportParameters),
            CultureInfo.CurrentUICulture.IetfLanguageTag);

        string subscriptionScript = String.Format("modalDialog('{0}?reportname={1}&parameters={2}&interval={3}','Subscription',{4},{5});return false",
            ResolveUrl("~/CMSModules/Reporting/Dialogs/EditSubscription.aspx"),
            ReportName,
            AnalyticsHelper.GetQueryStringParameters(ReportParameters),
            HitsIntervalEnumFunctions.HitsConversionToString(SelectedInterval),
            AnalyticsHelper.SUBSCRIPTION_WINDOW_WIDTH,
            AnalyticsHelper.SUBSCRIPTION_WINDOW_HEIGHT);

        string refreshScript = "function RefreshPage() {" + ControlsHelper.GetPostBackEventReference(this, "") + "};";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshScript", ScriptHelper.GetScript(refreshScript));

        // Register special script for print window
        ScriptHelper.RegisterPrintDialogScript(Page);

        headerActions.PanelCssClass = PanelCssClass;

        // Create header actions
        SaveAction save = new SaveAction(Page);
        headerActions.ActionsList.Add(save);

        // Print
        HeaderAction print = new HeaderAction
                                 {
                                     ControlType = HeaderActionTypeEnum.LinkButton,
                                     Text = GetString("Analytics_Report.Print"),
                                     SmallImageUrl = GetImageUrl("General/printSmall.png"),
                                     OnClientClick = printScript
                                 };
        headerActions.ActionsList.Add(print);

        CurrentUserInfo cui = CMSContext.CurrentUser;

        // Manage data
        if (cui.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageData") && DisplayManageData)
        {
            HeaderAction delete = new HeaderAction
                                      {
                                          ControlType = HeaderActionTypeEnum.LinkButton,
                                          Text = GetString("Analytics_Report.ManageData"),
                                          SmallImageUrl = GetImageUrl("CMSModules/CMS_Reporting/managedataSmall.png"),
                                          OnClientClick = deleteScript
                                      };
            headerActions.ActionsList.Add(delete);
        }

        // Report subscription enabled test
        GeneralizedInfo ri = ModuleCommands.ReportingGetReportInfo(ReportName);
        if (ri != null)
        {
            bool enableSubscription = ValidationHelper.GetBoolean(ri.GetValue("ReportEnableSubscription"), true);

            // Show enable subscription only for users with subscribe or modify.            
            enableSubscription &= (cui.IsAuthorizedPerResource("cms.reporting", "subscribe") || cui.IsAuthorizedPerResource("cms.reporting", "modify"));

            if (enableSubscription)
            {
                // Subscription
                HeaderAction subscription = new HeaderAction
                                                {
                                                    ControlType = HeaderActionTypeEnum.LinkButton,
                                                    Text = GetString("notifications.subscribe"),
                                                    SmallImageUrl = GetImageUrl("CMSModules/CMS_Reporting/Subscription.png"),
                                                    OnClientClick = subscriptionScript
                                                };
                headerActions.ActionsList.Add(subscription);
            }
        }

        base.OnPreRender(e);
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (ActionPerformed != null)
        {
            ActionPerformed(sender, e);
        }
    }

    #endregion
}
