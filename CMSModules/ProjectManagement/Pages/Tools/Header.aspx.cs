using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

[Tabs("CMS.ProjectManagement", "", "content")]
public partial class CMSModules_ProjectManagement_Pages_Tools_Header : CMSProjectManagementPage
{
    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the title
        PageTitle title = CurrentMaster.Title;
        title.TitleText = GetString("tools.ui.projectmanagement");
        title.TitleImage = GetImageUrl("Objects/PM_Project/object.png");
        title.HelpName = "helpTopic";
        title.HelpTopicName = "pm_projects";

        CurrentMaster.PanelSeparator.Visible = true;

        // Load action page directly, if set by URL
        switch (QueryHelper.GetString("action", null))
        {
            case "newproject":
                CurrentMaster.Tabs.StartPageURL = URLHelper.ResolveUrl("Project/Edit.aspx");
                break;
            case "newpersonaltask":
                CurrentMaster.Tabs.StartPageURL = URLHelper.ResolveUrl("ProjectTask/Edit.aspx");
                break;
        }
    }

    #endregion
}