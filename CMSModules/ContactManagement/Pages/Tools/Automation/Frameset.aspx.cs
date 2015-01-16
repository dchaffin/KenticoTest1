using System;

using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Frameset : CMSAutomationPage
{
    protected string headerTargetUrl = "";
    protected string contentTargetUrl = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        string listUrl = URLHelper.AppendQuery("List.aspx", URLHelper.Url.Query);

        string selectedTab = QueryHelper.GetString("showtab", null);
        if (!string.IsNullOrEmpty(selectedTab))
        {
            switch (selectedTab.ToLowerCSafe())
            {
                case "actions":
                    contentTargetUrl = "Action/List.aspx";
                    break;

                default:
                    contentTargetUrl = listUrl;
                    break;
            }

            headerTargetUrl = URLHelper.AddParameterToUrl(AddSiteQuery("Header.aspx", null), "showtab", selectedTab);
        }
        else
        {
            contentTargetUrl = listUrl;
            headerTargetUrl = AddSiteQuery("Header.aspx", null);
        }
    }
}