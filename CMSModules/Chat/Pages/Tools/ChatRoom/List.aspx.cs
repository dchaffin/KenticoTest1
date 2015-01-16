using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Chat;
using CMS.FormControls;

[Actions(1)]
[Action(0, ResourceString = "chat.chatroom.new", TargetUrl = "New.aspx?siteID={%SelectedSiteID%}", ImageUrl = "Objects/CMS_ChatRoom/add.png")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_List : CMSChatPage
{
    #region "Private fields"

    private int selectedSiteID;

    #endregion

    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // If user can view global and local rooms, display site selector
        if (ReadAllowed && ReadGlobalAllowed)
        {
            CurrentMaster.DisplaySiteSelectorPanel = true;

            if (!RequestHelper.IsPostBack())
            {
                siteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteid", CMSContext.CurrentSiteID);
            }

            // Get site id from site selector
            selectedSiteID = siteOrGlobalSelector.SiteID;

            // Security check: user can select global (-4) this site and global (-5) or current site, if something else was selected, set it back to current site
            if ((selectedSiteID != -4) && (selectedSiteID != -5) && (selectedSiteID != CMSContext.CurrentSiteID))
            {
                selectedSiteID = CMSContext.CurrentSiteID;
                siteOrGlobalSelector.SiteID = selectedSiteID;
            }
        }
        else
        {
            if (ReadAllowed)
            {
                selectedSiteID = CMSContext.CurrentSiteID;
            }
            else
            {
                selectedSiteID = -4;
            }
        }

        listElem.SiteID = selectedSiteID;

        // Store selected site ID to MacroResolver, so it can be retrieved in ActionAttribute's Apply method (on second pass, which is called on PreRender)
        CMSContext.CurrentResolver.SetNamedSourceData("SelectedSiteID", selectedSiteID);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Disable action and display label if site is set to "this site and global"
        if (selectedSiteID == -5)
        {
            HeaderActions.ActionsList[0].Enabled = false;

            FormEngineUserControl label = this.LoadUserControl("~/CMSFormControls/Basic/LabelControl.ascx") as FormEngineUserControl;
            label.Value = GetString("chat.chooseglobalorsitetoaddnewroom");

            HeaderActions.AdditionalControls.Add(label);
            HeaderActions.AdditionalControlsCssClass = "HeaderActionsLabel";
            HeaderActions.ReloadAdditionalControls();
        }
        else
        {
            HeaderActions.ActionsList[0].Enabled = HasUserModifyPermission(selectedSiteID);
        }
    }

    #endregion
}
