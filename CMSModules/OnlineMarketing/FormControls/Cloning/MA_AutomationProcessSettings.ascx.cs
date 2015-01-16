using System;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;
using CMS.UIControls;

public partial class CMSModules_OnlineMarketing_FormControls_Cloning_MA_AutomationProcessSettings : CloneSettingsControl
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
    /// Excluded binding types.
    /// </summary>
    public override string ExcludedBindingTypes
    {
        get
        {
            return WorkflowObjectType.WORKFLOWTRANSITION;
        }
    }


    /// <summary>
    /// Excluded other binding types.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return WorkflowObjectType.WORKFLOWTRANSITION;
        }
    }


    /// <summary>
    /// Excluded other binding types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return WorkflowObjectType.OBJECTWORKFLOWTRIGGER;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblWorkflowTrigger.ToolTip = GetString("clonning.settings.workflow.workflowtrigger.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[WorkflowObjectType.AUTOMATIONPROCESS + ".workflowtrigger"] = chkWorkflowTrigger.Checked;
        return result;
    }

    #endregion
}