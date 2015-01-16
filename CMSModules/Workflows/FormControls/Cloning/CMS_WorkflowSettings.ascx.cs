using System;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;
using CMS.UIControls;

public partial class CMSModules_Workflows_FormControls_Cloning_CMS_WorkflowSettings : CloneSettingsControl
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
            return WorkflowObjectType.WORKFLOWSCOPE;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblWorkflowScope.ToolTip = GetString("clonning.settings.workflow.workflowscope.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[WorkflowObjectType.WORKFLOW + ".workflowscope"] = chkWorkflowScope.Checked;
        return result;
    }

    #endregion
}