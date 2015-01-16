using System;
using System.Collections;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.PortalEngine;
using CMS.CMSHelper;
using CMS.UIControls;
using CMS.SiteProvider;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_UserSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return SiteObjectType.USERSETTINGS + ";" + PredefinedObjectType.FRIEND + ";" + PredefinedObjectType.OPENIDUSER + ";" + PredefinedObjectType.PROJECTADHOCTASK + ";" + PredefinedObjectType.PERSONALIZATION + ";" + PredefinedObjectType.DASHBOARD;
        }
    }


    /// <summary>
    /// Excluded binding types.
    /// </summary>
    public override string ExcludedBindingTypes
    {
        get
        {
            return PredefinedObjectType.IGNORELIST + ";" + PredefinedObjectType.CONTACTLIST;
        }
    }


    /// <summary>
    /// Excluded other binding types.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return PredefinedObjectType.REPORTSUBSCRIPTION + ";" + SiteObjectType.USERROLE + ";" + PredefinedObjectType.BOARDMODERATOR + ";" + PredefinedObjectType.FORUMMODERATOR + ";" + SiteObjectType.MEMBERSHIPUSER + ";" + PredefinedObjectType.WORKFLOWSTEPUSER + ";" + PredefinedObjectType.WORKFLOWUSER;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblEmail.ToolTip = GetString("clonning.settings.user.email.tooltip");
        lblGeneratePassword.ToolTip = GetString("clonning.settings.user.generatepassword.tooltip");
        lblPassword.ToolTip = GetString("clonning.settings.user.password.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            txtEmail.Text = InfoToClone.GetStringValue("Email", "");
        }

        string script = 
@"function EnableDisablePassword() {
  var elem = document.getElementById('" + txtPassword.ValueElementID + @"');
  var chk = document.getElementById('" + chkGeneratePassword.ClientID + @"');
  if ((chk!= null) && (elem != null)) {
    if (chk.checked) {
      elem.disabled = true;
    } else {
      elem.disabled = false;
    }
  }
}
EnableDisablePassword();
";
        ScriptHelper.RegisterStartupScript(this.Page, typeof(string), "EnableDisablePassword", script, true);
        chkGeneratePassword.Attributes.Add("onclick", "EnableDisablePassword();");
    }


    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        if (!chkGeneratePassword.Checked)
        {
            if (!txtPassword.IsValid())
            {
                ShowError(AuthenticationHelper.GetPolicyViolationMessage(CMSContext.CurrentSiteName));
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[PortalObjectType.USER + ".email"] = txtEmail.Text;
        result[PortalObjectType.USER + ".generatepassword"] = chkGeneratePassword.Checked;
        result[PortalObjectType.USER + ".password"] = txtPassword.Value;
        result[PortalObjectType.USER + ".permissions"] = chkPermissions.Checked;
        result[PortalObjectType.USER + ".personalization"] = chkPersonalization.Checked;
        return result;
    }

    #endregion
}