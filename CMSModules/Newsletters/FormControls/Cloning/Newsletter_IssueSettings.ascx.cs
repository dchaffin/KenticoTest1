using System;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.Newsletter;
using CMS.UIControls;

public partial class CMSModules_Newsletters_FormControls_Cloning_Newsletter_IssueSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Excluded other binding tasks.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return NewsletterObjectType.NEWSLETTEROPENEDEMAIL;
        }
    }


    /// <summary>
    /// Excluded other binding tasks.
    /// </summary>
    public override string  ExcludedChildTypes
    {
        get
        {
            return NewsletterObjectType.NEWSLETTERLINK;
        }
    }


    /// <summary>
    /// Hide the control
    /// </summary>
    public override bool DisplayControl
    {
        get
        {
            return false;
        }
    }

    #endregion
}