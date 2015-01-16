using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.WorkflowEngine;

[EditedObject(WorkflowObjectType.AUTOMATIONACTION, "actionId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Action_Tab_Parameters : CMSContactManagementConfigurationPage
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

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Only global administrator can access automation process actions
        if (!CurrentUser.IsGlobalAdministrator)
        {
            RedirectToAccessDenied(GetString("security.accesspage.onlyglobaladmin"));
        }
    }


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