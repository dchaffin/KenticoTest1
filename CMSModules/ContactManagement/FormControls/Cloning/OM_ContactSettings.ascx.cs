using System;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.OnlineMarketing;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_FormControls_Cloning_OM_ContactSettings : CloneSettingsControl
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
    /// Exclueded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return OnlineMarketingObjectType.ACTIVITY + ";" + OnlineMarketingObjectType.USERAGENT + ";" + OnlineMarketingObjectType.IP;
        }
    }


    /// <summary>
    /// Exclueded binding types.
    /// </summary>
    public override string ExcludedBindingTypes
    {
        get
        {
            return OnlineMarketingObjectType.CONTACTGROUPMEMBERCONTACT;
        }
    }


    /// <summary>
    /// Hide display name.
    /// </summary>
    public override bool HideDisplayName
    {
        get
        {
            return true;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblMerged.ToolTip = GetString("clonning.settings.contact.merged.tooltip");
        lblAddressesAgents.ToolTip = GetString("clonning.settings.contact.ipaddressesuseragents.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            ContactInfo ci = (ContactInfo)InfoToClone;
            txtFirstName.Text = ci.ContactFirstName;
            txtLastName.Text = ci.ContactLastName;
        }
    }


    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        if (string.IsNullOrEmpty(txtLastName.Text))
        {
            ShowError(GetString("om.contact.enterlastname"));
            return false;
        }
        return true;
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[OnlineMarketingObjectType.CONTACT + ".merged"] = chkMerged.Checked;
        result[OnlineMarketingObjectType.CONTACT + ".addressesagents"] = chkAddressesAgents.Checked;
        result[OnlineMarketingObjectType.CONTACT + ".activity"] = chkActivity.Checked;
        result[OnlineMarketingObjectType.CONTACT + ".contactgroup"] = chkContactGroup.Checked;
        result[OnlineMarketingObjectType.CONTACT + ".firstname"] = txtFirstName.Text;
        result[OnlineMarketingObjectType.CONTACT + ".lastname"] = txtLastName.Text;
        return result;
    }

    #endregion
}