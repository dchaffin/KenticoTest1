using System;
using System.Collections;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.Reporting;
using CMS.UIControls;

public partial class CMSModules_Reporting_FormControls_Cloning_Reporting_ReportSubscriptionSettings : CloneSettingsControl
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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            txtEmail.Text = InfoToClone.GetStringValue("ReportSubscriptionEmail", "");
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[ReportingObjectType.REPORTSUBSCRIPTION + ".email"] = txtEmail.Text;
        return result;
    }

    #endregion
}