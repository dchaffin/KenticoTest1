using System;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.Newsletter;
using CMS.UIControls;

public partial class CMSModules_Newsletters_FormControls_Cloning_Newsletter_NewsletterSettings : CloneSettingsControl
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
    /// Excluded other binding tasks.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return NewsletterObjectType.NEWSLETTERSUBSCRIBERNEWSLETTER + ";" + NewsletterObjectType.NEWSLETTEROPENEDEMAIL;
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
        result[NewsletterObjectType.NEWSLETTER + ".subscribers"] = chkSubscribers.Checked;
        return result;
    }

    #endregion
}