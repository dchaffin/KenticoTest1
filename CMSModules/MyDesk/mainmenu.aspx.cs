using System;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_MyDesk_mainmenu : CMSMyDeskPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Filter unavailable modules
        ucUIToolbar.OnButtonFiltered += new CMSAdminControls_UI_UIProfiles_UIToolbar.ButtonFilterEventHandler(ucUIToolbar_OnButtonFiltered);
    }


    /// <summary>
    /// On button filtered handler.
    /// </summary>
    private bool ucUIToolbar_OnButtonFiltered(object sender, UniMenuArgs e)
    {
        if (e.UIElement == null)
        {
            return false;
        }

        bool moduleOnSite = true;
        string siteName = CMSContext.CurrentSiteName;

        // Check whether modules are assigned to current site
        switch (e.UIElement.ElementName.ToLowerCSafe())
        {
            case "myfriends":
                moduleOnSite = ResourceSiteInfoProvider.IsResourceOnSite("CMS.Friends", siteName);
                break;

            case "mymessages":
                moduleOnSite = ResourceSiteInfoProvider.IsResourceOnSite("CMS.Messaging", siteName);
                break;

            case "myblogs":
                moduleOnSite = ResourceSiteInfoProvider.IsResourceOnSite("CMS.Blog", siteName);
                break;

            case "myprojects":
                moduleOnSite = ResourceSiteInfoProvider.IsResourceOnSite("CMS.ProjectManagement", siteName);
                break;
        }

        // Check whether separable modules are loaded
        return moduleOnSite && IsMyDeskUIElementAvailable(e.UIElement.ElementName);
    }


    /// <summary>
    /// Register first page script.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        string url = null;

        // Preselect the item in the frame
        UIElementInfo selected = ucUIToolbar.SelectedUIElement;
        if (selected != null)
        {
            url = selected.ElementTargetURL;
            url = URLHelper.EnsureHashToQueryParameters(url);
            url = URLHelper.ResolveUrl(url);
        }

        // Prepare the script
        string script = null;
        if (!String.IsNullOrEmpty(url))
        {
            script = "try {parent.frames['frameMain'].location.href='" + url + "'} catch (err) {}\n";
        }
        else
        {
            script = "try {parent.frames['frameMain'].location.href='" + URLHelper.ResolveUrl(UIHelper.GetInformationUrl("uiprofile.uinotavailable")) + "'} catch (err) {}\n";
        }

        litScript.Text = ScriptHelper.GetScript(script);


        base.OnPreRender(e);
    }

    #endregion
}