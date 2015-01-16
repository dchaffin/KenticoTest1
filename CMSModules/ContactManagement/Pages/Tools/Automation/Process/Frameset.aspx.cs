using System;

using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Frameset : CMSAutomationPage
{
    protected string headerTargetUrl = "";
    protected string contentTargetUrl = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        string selectedTab = QueryHelper.GetString("showtab", null);
        if (!String.IsNullOrEmpty(selectedTab))
        {
            switch (selectedTab.ToLowerCSafe())
            {
                case "triggers":
                    contentTargetUrl = "Tab_Triggers.aspx";
                    break;

                case "steps":
                    contentTargetUrl = "Tab_Steps.aspx";
                    break;

                case "contacts":
                    contentTargetUrl = "Tab_Contacts.aspx";
                    break;

                default:
                    contentTargetUrl = "Tab_General.aspx";
                    break;
            }

            headerTargetUrl = URLHelper.AddParameterToUrl("Header.aspx", "showtab", selectedTab);
        }
        else
        {
            contentTargetUrl = "Tab_General.aspx";
            headerTargetUrl = "Header.aspx";
        }

        if (QueryHelper.GetBoolean("saved", false))
        {
            contentTargetUrl = URLHelper.AddParameterToUrl(contentTargetUrl, "saved", "1");
        }

        contentTargetUrl = URLHelper.AppendQuery(contentTargetUrl, URLHelper.Url.Query);
        headerTargetUrl = URLHelper.AppendQuery(headerTargetUrl, URLHelper.Url.Query);
    }
}