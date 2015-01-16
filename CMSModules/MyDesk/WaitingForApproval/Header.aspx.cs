using System;

using CMS.UIControls;
using CMS.Controls.Configuration;
using CMS.SiteProvider;

[Tabs("CMS.MyDesk", "Pending", "content")]
public partial class CMSModules_MyDesk_WaitingForApproval_Header : CMSMyDeskPage
{
    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "onlinemarketing_contact_list";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Pending";
        CurrentMaster.Title.TitleText = "Pending";
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_MyDesk/WaitingForApproval/module.png");

        OnTabCreated += new UITabs.TabCreatedEventHandler(CMSModules_ContactManagement_Pages_Tools_ContactManagement_Header_OnTabCreated);
    }


    protected TabItem CMSModules_ContactManagement_Pages_Tools_ContactManagement_Header_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        switch (tabIndex)
        {
            case 0: tab.OnClientClick = "SetHelpTopic('helpTopic','waiting_for_my_approval')";
                break;
            
            case 1: tab.OnClientClick = "SetHelpTopic('helpTopic','onlinemarketing_pendingcontacts_list')";
                break;
        }

        return tab;
    }

    #endregion
}
