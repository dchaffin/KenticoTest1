using System;

using CMS.CMSHelper;
using CMS.UIControls;
using CMS.Controls.Configuration;
using CMS.ExtendedControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_MyDesk_MyProfile_MyProfile_Subscriptions : CMSMyProfilePage
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if ((CMSContext.CurrentUser == null) || (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.MyDesk", "MyProfile.Subscriptions")))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.MyDesk", "MyProfile.Subscriptions");

        }

        InitializeTabMenu();
    }

    /// <summary>
    /// Sets tabs for tab menu
    /// </summary>
    private void InitializeTabMenu()
    {

        if (ModuleEntry.IsModuleLoaded(ModuleEntry.NEWSLETTER) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleEntry.NEWSLETTER, CMSContext.CurrentSiteName))
        {
            TabItem newsletter = new TabItem();
            newsletter.Text = GetString("newsletter_edit.newsletters");
            newsletter.RedirectUrl = "#NewsletterGroup";
            tabControlElem.TabItems.Add(newsletter);
        }

        if (ModuleEntry.IsModuleLoaded(ModuleEntry.BLOGS) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleEntry.BLOGS, CMSContext.CurrentSiteName))
        {
            TabItem blog = new TabItem();
            blog.Text = GetString("blog.blogs");
            blog.RedirectUrl = "#BlogGroup";
            tabControlElem.TabItems.Add(blog);
        }

        if (ModuleEntry.IsModuleLoaded(ModuleEntry.MESSAGEBOARD) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleEntry.MESSAGEBOARD, CMSContext.CurrentSiteName))
        {
            TabItem board = new TabItem();
            board.Text = GetString("board.header.messageboards");
            board.RedirectUrl = "#BoardGroup";
            tabControlElem.TabItems.Add(board);
        }

        if (ModuleEntry.IsModuleLoaded(ModuleEntry.FORUMS) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleEntry.FORUMS, CMSContext.CurrentSiteName))
        {
            TabItem forum = new TabItem();
            forum.Text = GetString("forum_list.headercaption");
            forum.RedirectUrl = "#ForumGroup";
            tabControlElem.TabItems.Add(forum);
        }

        if (ModuleEntry.IsModuleLoaded(ModuleEntry.REPORTING) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleEntry.REPORTING, CMSContext.CurrentSiteName))
        {
            TabItem report = new TabItem();
            report.Text = GetString("reporting.reports");
            report.RedirectUrl = "#ReportGroup";
            tabControlElem.TabItems.Add(report);
        }

        tabControlElem.SelectedTab = -1;
        tabControlElem.UrlTarget = "_self";
    }
}