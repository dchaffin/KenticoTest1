using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.CMSHelper;
using CMS.EventLog;
using CMS.SiteProvider;

public partial class CMSModules_Objects_Dialogs_CloneObjectDialog : CMSModalPage
{
    string objectType = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query string parameters
        objectType = QueryHelper.GetString("objecttype", String.Empty);
        int objectId = QueryHelper.GetInteger("objectid", 0);

        // Get the object
        BaseInfo info = CMSObjectHelper.GetObjectById(objectType, objectId);

        string objTypeName = "";
        if (info != null)
        {
            objTypeName = GetString("objecttype." + TranslationHelper.GetSafeClassName(info.ObjectType));
        }

        if (objTypeName.StartsWithCSafe("objecttype."))
        {
            objTypeName = "";
            SetTitle("CMSModules/CMS_System/clone.png", String.Format(GetString("clonning.dialog.title"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(info.Generalized.ObjectDisplayName))), "objectclonning", "helpTopic");
        }
        else
        {
            SetTitle("CMSModules/CMS_System/clone.png", String.Format(GetString("clonning.dialog.title"), objTypeName), "objectclonning", "helpTopic");
        }

        btnClose.Text = GetString("General.Close");
        btnClone.Text = GetString("General.Clone");
        btnClone.Click += new EventHandler(btnClone_Click);

        if (info == null)
        {
            ShowInformation(GetString("clonning.dialog.objectdoesnotexist"));
            cloneObjectElem.Visible = false;
            return;
        }

        if (cloneObjectElem.HasNoSettings())
        {
            lblInfo.Text = string.Format(GetString("clonning.settings.emptyinfobox"), objTypeName, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(info.Generalized.ObjectDisplayName)));
        }
        else
        {
            lblInfo.Text = string.Format(GetString("clonning.settings.infobox"), objTypeName, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(info.Generalized.ObjectDisplayName)));
        }

        // Check permissions
        string sourceSiteName = CurrentSiteName;
        if (info.Generalized.ObjectSiteID > 0)
        {
            sourceSiteName = SiteInfoProvider.GetSiteName(info.Generalized.ObjectSiteID);
        }
        try
        {
            CurrentUser.IsAuthorizedPerObject(PermissionsEnum.Read, info, sourceSiteName, true);
        }
        catch (PermissionCheckException ex)
        {
            RedirectToCMSDeskAccessDenied(ex.ModuleName, ex.PermissionFailed);
        }

        cloneObjectElem.InfoToClone = info;

        // Register refresh script to refresh wopener
        StringBuilder script = new StringBuilder();
        script.Append(@"
function RefreshContent() {
  if (wopener != null) {
    if (wopener.RefreshWOpener)
    {
        window.refreshPageOnClose = true;
        wopener.RefreshWOpener(window);
    }
    else
    {
        wopener.window.location.replace(wopener.window.location);
    }
  }
}");
        // Register script
        ScriptHelper.RegisterWOpenerScript(this.Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "WOpenerRefresh", ScriptHelper.GetScript(script.ToString()));
    }


    protected void btnClone_Click(object sender, EventArgs e)
    {
        try
        {
            CloneResult result = cloneObjectElem.CloneObject();
            if (result != null)
            {
                if (result.Errors.Count > 0)
                {
                    ShowError(ResHelper.LocalizeString(string.Join("\n", result.Errors.ToArray())));
                    SwitchToErrorMode();
                }
                else if (result.Warnings.Count > 0)
                {
                    ShowWarning(GetString("cloning.savedwithwarnings"), ResHelper.LocalizeString(string.Join("<br/>", result.Warnings.ToArray())), null);
                    SwitchToErrorMode();
                }
                else
                {
                    ScriptHelper.RegisterStartupScript(this.Page, typeof(string), "CloneRefresh", cloneObjectElem.CloseScript, true);
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider provider = new EventLogProvider();
            provider.LogEvent(string.IsNullOrEmpty(objectType) ? "System" : objectType.ToLowerCSafe(), "CLONEOBJECT", ex);
            ShowError(ex.Message);

            if (!cloneObjectElem.UseTransaction)
            {
                SwitchToErrorMode();
            }
        }
    }


    private void SwitchToErrorMode()
    {
        plcForm.Visible = false;
        btnClone.Visible = false;
        btnClose.OnClientClick = "RefreshContent(); CloseDialog(); return false;";
    }
}