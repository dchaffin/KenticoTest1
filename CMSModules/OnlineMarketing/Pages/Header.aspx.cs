using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.SettingsProvider;

public partial class CMSModules_OnlineMarketing_Pages_Header : CMSDeskPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        CheckDocPermissions = false;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        uiToolbarElem.OnButtonCreating += uiToolbarElem_OnButtonCreating;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        string url = null;

        // Preselect the item in the frame
        UIElementInfo selected = uiToolbarElem.SelectedUIElement;
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
            script = "try {parent.frames['content'].location.href='" + url + "'; SelectItem('" + selected.ElementName + "','',true);} catch (err) {}\n";
        }
        else
        {
            script = "try {parent.frames['content'].location.href='" + URLHelper.ResolveUrl(UIHelper.GetInformationUrl("uiprofile.uinotavailable")) + "'} catch (err) {}\n";
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "TabSelection", ScriptHelper.GetScript(script));
        ScriptHelper.RegisterTitleScript(this, GetString("cmsdesk.ui.marketing"));
    }

    
    /// <summary>
    /// OnButtonCreated event handler.
    /// </summary>
    protected void uiToolbarElem_OnButtonCreating(object sender, UniMenuArgs e)
    {
        if (e.UIElement == null)
        {
            return;
        }

        if (e.UIElement.ElementName.EqualsCSafe("processes", true) && CurrentUser.IsGlobalAdministrator)
        {
            e.UIElement.ElementTargetURL = "~/CMSModules/ContactManagement/Pages/Tools/Automation/Frameset.aspx";
        }
    }
}