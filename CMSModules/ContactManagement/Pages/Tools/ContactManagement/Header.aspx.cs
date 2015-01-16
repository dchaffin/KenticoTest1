using System;

using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.Controls.Configuration;
using CMS.SiteProvider;

[Tabs("CMS.OnlineMarketing", "ContactsFrameset", "content")]
public partial class CMSModules_ContactManagement_Pages_Tools_ContactManagement_Header : CMSContactManagementPage
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
        CurrentMaster.Title.TitleText = "Contacts";
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/OM_Contact/object.png");

        OnTabCreated += new UITabs.TabCreatedEventHandler(CMSModules_ContactManagement_Pages_Tools_ContactManagement_Header_OnTabCreated);
    }


    protected TabItem CMSModules_ContactManagement_Pages_Tools_ContactManagement_Header_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        switch (tabIndex)
        {
            case 0: tab.OnClientClick = "SetHelpTopic('helpTopic','onlinemarketing_contact_list')";
                break;

            case 1: tab.OnClientClick = "SetHelpTopic('helpTopic','onlinemarketing_pendingcontacts_list')";
                break;

            case 2: tab.OnClientClick = "SetHelpTopic('helpTopic','onlinemarketing_onlineusers_list')";
                break;

            case 3: tab.OnClientClick = "SetHelpTopic('helpTopic','onlinemarketing_account_list')";
                break;
        }
        return tab;
    }

    #endregion
}
