using System;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.OnlineMarketing;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_FormControls_Cloning_OM_ContactGroupSettings : CloneSettingsControl
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
    /// Exclued other binding types.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return OnlineMarketingObjectType.CONTACTGROUPMEMBERACCOUNT + ";" + OnlineMarketingObjectType.CONTACTGROUPMEMBERCONTACT;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[OnlineMarketingObjectType.CONTACTGROUP + ".accounts"] = chkCloneAccounts.Checked;
        result[OnlineMarketingObjectType.CONTACTGROUP + ".contacts"] = chkCloneContacts.Checked;
        return result;
    }

    #endregion
}