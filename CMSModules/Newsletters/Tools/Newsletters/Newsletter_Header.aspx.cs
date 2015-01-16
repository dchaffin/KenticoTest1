using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.UIControls;

[Title("Objects/Newsletter_Newsletter/object.png", "newsletters.newsletters", "issues_tab", HelpName = "helpTopic")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Header : CMSNewsletterNewslettersPage
{
    protected int newsletterId;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get newsletter id from querystring
        newsletterId = QueryHelper.GetInteger("newsletterid", 0);
        // Try to get nesletter display name
        string newsName = string.Empty;
        NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
        if (news != null)
        {
            newsName = news.NewsletterDisplayName;
        }

        // Initializes page title
        string[,] breadcrumbs = new string[2,3];
        breadcrumbs[0, 0] = GetString("Newsletter_Edit.Newsletters");
        breadcrumbs[0, 1] = "~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_List.aspx";
        breadcrumbs[0, 2] = "newslettersContent";
        breadcrumbs[1, 0] = newsName;
        breadcrumbs[1, 1] = string.Empty;
        breadcrumbs[1, 2] = string.Empty;

        CurrentMaster.Title.Breadcrumbs = breadcrumbs;

        // Initialize tabs
        InitalizeMenu(news);
    }


    /// <summary>
    /// Initializes newsletter menu.
    /// </summary>
    /// <param name="news">Newsletter object</param>
    protected void InitalizeMenu(NewsletterInfo news)
    {
        if (news != null)
        {
            InitTabs("newsletterContent");
            SetTab(0, GetString("Newsletter_Header.Issues"), "Newsletter_Issue_List.aspx?newsletterid=" + newsletterId, "SetHelpTopic('helpTopic', 'issues_tab');");
            SetTab(1, GetString("Newsletter_Header.Configuration"), "Newsletter_Configuration.aspx?newsletterid=" + newsletterId, "SetHelpTopic('helpTopic', 'configuration_tab');");
            SetTab(2, GetString("Newsletter_Header.Subscribers"), "Newsletter_Subscribers.aspx?newsletterid=" + newsletterId, "SetHelpTopic('helpTopic', 'subscribers_tab');");
            if (news.NewsletterType == NewsletterType.Dynamic)
            {
                SetTab(3, GetString("Newsletter_Header.Send"), "Newsletter_Send.aspx?newsletterid=" + newsletterId, "SetHelpTopic('helpTopic', 'send_tab');");
            }
            else if (news.NewsletterType == NewsletterType.TemplateBased)
            {
                SetTab(3, GetString("newsletter.issuetemplates"), "Tab_Templates.aspx?newsletterid=" + newsletterId, "SetHelpTopic('helpTopic','newsletter_templates')");
            }

            if (ValidationHelper.GetInteger(Request.QueryString["saved"], 0) > 0)
            {
                // User was redirected from Newsletter_New.aspx => show 'configuration' tab
                CurrentMaster.Tabs.SelectedTab = 1;
            }
        }
    }
}