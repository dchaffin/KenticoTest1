using System;

using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.UIControls;

public partial class CMSModules_Reporting_Tools_Category_Header : CMSReportingPage
{
    #region "Variables"

    protected int categoryId;

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        string currentReportCategory = "";

        if (QueryHelper.Contains("categoryid"))
        {
            categoryId = QueryHelper.GetInteger("categoryid", 0);
            ReportCategoryInfo category = ReportCategoryInfoProvider.GetReportCategoryInfo(categoryId);
            if (category != null)
            {
                currentReportCategory = category.CategoryDisplayName;
            }
        }

        if (!RequestHelper.IsPostBack())
        {
            InitalizeMenu();
        }

        InitializeMasterPage(currentReportCategory);
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Initializes master page.
    /// </summary>
    protected void InitializeMasterPage(string currentReportCategory)
    {
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "report_list";

        // initializes page title control		
        string[,] tabs = new string[2,3];
        tabs[0, 0] = GetString("Reporting.ReportCategoryList");
        tabs[0, 1] = URLHelper.ResolveUrl("~/CMSModules/Reporting/Tools/ReportCategory_List.aspx");
        tabs[0, 2] = "_parent";
        tabs[1, 0] = HTMLHelper.HTMLEncode(currentReportCategory);
        tabs[1, 1] = "";
        tabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = tabs;
        CurrentMaster.Title.TitleText = GetString("Reporting.ReportCategoryHeaderCaption");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Reporting_ReportCategory/object.png");
    }


    /// <summary>
    /// Initializes user edit menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        string generalString = GetString("general.general");
        string reportString = GetString("Reporting.Reports");

        string[,] tabs = new string[2,4];
        tabs[0, 0] = reportString;
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'report_list');";
        tabs[0, 2] = URLHelper.ResolveUrl("~/CMSModules/Reporting/Tools/Report_List.aspx?categoryid=" + categoryId);
        tabs[1, 0] = generalString;
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'general_tab5');";
        tabs[1, 2] = URLHelper.ResolveUrl("~/CMSModules/Reporting/Tools/ReportCategory_General.aspx?categoryid=" + categoryId);

        CurrentMaster.Tabs.UrlTarget = "categoryContent";
        CurrentMaster.Tabs.Tabs = tabs;
    }

    #endregion
}