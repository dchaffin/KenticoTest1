using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;


public partial class CMSModules_Reporting_Tools_ReportCategory_header : CMSReportingPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Report header";
        CurrentMaster.Title.HelpTopicName = "ReportCategories";
        CurrentMaster.Title.HelpName = "helpTopic";

        CurrentMaster.Title.TitleText = GetString("tools.ui.reporting");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Reporting_ReportCategory/object.png");
    }
}