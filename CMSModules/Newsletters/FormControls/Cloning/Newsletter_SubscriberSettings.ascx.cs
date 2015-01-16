using System;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.Newsletter;
using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_Newsletters_FormControls_Cloning_Newsletter_SubscriberSettings : CloneSettingsControl
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
        if (!RequestHelper.IsPostBack())
        {
            SubscriberInfo info = (SubscriberInfo)InfoToClone;
            txtEmail.Text = info.SubscriberEmail;
            txtFirstName.Text = info.SubscriberFirstName;
            txtLastName.Text = info.SubscriberLastName;
        }
    }

    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[NewsletterObjectType.NEWSLETTERSUBSCRIBER + ".email"] = txtEmail.Text;
        result[NewsletterObjectType.NEWSLETTERSUBSCRIBER + ".firstname"] = txtFirstName.Text;
        result[NewsletterObjectType.NEWSLETTERSUBSCRIBER + ".lastname"] = txtLastName.Text;
        return result;
    }

    #endregion
}