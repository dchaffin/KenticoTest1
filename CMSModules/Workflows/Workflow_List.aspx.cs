using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.UI;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.SettingsProvider;
using CMS.CMSHelper;

public partial class CMSModules_Workflows_Workflow_List : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Workflows - Workflow List";

        // Set master page elements
        InitializeMasterPage();

        // Control initialization
        UniGridWorkflows.OnAction += UniGridWorkflows_OnAction;
        UniGridWorkflows.OnExternalDataBound += UniGridWorkflows_OnExternalDataBound;
        UniGridWorkflows.OnBeforeDataReload += UniGridWorkflows_OnBeforeDataReload;
        UniGridWorkflows.ZeroRowsText = GetString("general.nodatafound");
        UniGridWorkflows.WhereCondition = string.Format("WorkflowType != {0} OR WorkflowType IS NULL", (int)WorkflowTypeEnum.Automation);
    }


    /// <summary>
    ///  Initializes master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        HeaderAction basic = new HeaderAction()
        {
            Text = GetString("Development-Workflow_List.NewWorkflow"),
            ImageUrl = GetImageUrl("Objects/CMS_Workflow/add.png"),
            RedirectUrl = "~/CMSModules/Workflows/Workflow_New.aspx"
        };

        AddHeaderAction(basic);

        if (WorkflowInfoProvider.IsAdvancedWorkflowAllowed())
        {
            HeaderAction advanced = new HeaderAction()
            {
                // Advanced workflow link
                Text = GetString("Development-Workflow_List.NewAdvancedWorkflow"),
                ImageUrl = GetImageUrl("Objects/CMS_Workflow/add_advanced.png"),
                RedirectUrl = "~/CMSModules/Workflows/Workflow_New.aspx?type=1"
            };

            AddHeaderAction(advanced);
        }
    }


    protected object UniGridWorkflows_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "type":
                DataRowView drv = (DataRowView)parameter;
                bool versioning = ValidationHelper.GetBoolean(drv.Row["WorkflowAutoPublishChanges"], false);
                int type = ValidationHelper.GetInteger(drv.Row["WorkflowType"], 0);
                return WorkflowHelper.GetWorkflowTypeString((WorkflowTypeEnum)type, versioning);

            case "scope":
                string types = ValidationHelper.GetString(parameter, "");
                var typeList = TypeHelper.GetTypes(types);
                if (typeList.Count == 0)
                {
                    typeList.Add(PredefinedObjectType.GROUP_DOCUMENTS);
                }
                string list = "";
                typeList.ForEach(s => list += ResHelper.GetString(ResHelper.GetAPIString("ObjectTasks." + s.Replace(".", "_").Replace("#", "_"), s)) + ", ");
                return list.Trim().TrimEnd(',');
        }

        return parameter;
    }


    private void UniGridWorkflows_OnBeforeDataReload()
    {
        // Hide allowed objects column when not in development mode
        UniGridWorkflows.NamedColumns["scope"].Visible = SettingsKeyProvider.DevelopmentMode;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGridWorkflows_OnAction(string actionName, object actionArgument)
    {
        int workflowid = Convert.ToInt32(actionArgument);

        switch (actionName)
        {
            case "edit":
                URLHelper.Redirect("Workflow_Edit.aspx?workflowid=" + workflowid);
                break;

            case "delete":
                // Check if documents use the workflow
                List<string> documentNames = new List<string>();
                if (WorkflowInfoProvider.CheckDependencies(workflowid, ref documentNames))
                {
                    // Encode and localize names
                    StringBuilder sb = new StringBuilder();
                    documentNames.ForEach(item => sb.Append("<br />", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(item))));
                    ShowError(GetString("Workflow.CannotDeleteUsed"), GetString("workflow.documentlist") + sb.ToString(), null);
                    return;
                }
                else
                {
                    // Delete the workflow
                    WorkflowInfoProvider.DeleteWorkflowInfo(workflowid);
                }
                break;
        }
    }
}