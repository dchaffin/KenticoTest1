using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.WorkflowEngine;

[EditedObject("cms.workflowaction", "actionId")]
public partial class CMSModules_Workflows_Pages_WorkflowAction_Tab_Parameters : CMSWorkflowPage
{
    #region "Private properties"

    private WorkflowActionInfo ActionInfo
    {
        get
        {
            return (WorkflowActionInfo)EditedObject;
        }
    }

    #endregion


    #region "Event handlers"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";
        CurrentMaster.BodyClass += " FieldEditorBody";
        InitializeFieldEditor();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes field editor.
    /// </summary>
    private void InitializeFieldEditor()
    {
        if (ActionInfo == null)
        {
            return;
        }
        fieldEditor.FormDefinition = ActionInfo.ActionParameters;
        fieldEditor.OnAfterDefinitionUpdate += fieldEditor_OnAfterDefinitionUpdate;
    }


    /// <summary>
    /// Field editor updated event.
    /// </summary>
    private void fieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        if (ActionInfo != null)
        {
            ActionInfo.ActionParameters = fieldEditor.FormDefinition;
            WorkflowActionInfoProvider.SetWorkflowActionInfo(ActionInfo);
        }
    }

    #endregion
}