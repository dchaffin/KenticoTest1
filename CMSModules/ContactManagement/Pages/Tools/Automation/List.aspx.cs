using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.UI;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.SettingsProvider;
using CMS.CMSHelper;
using CMS.OnlineMarketing;
using CMS.WorkflowEngine;

public partial class CMSModules_ContactManagement_Pages_Tools_Automation_List : CMSAutomationPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set master page elements
        InitializeMasterPage();

        // Check manage permission for object menu
        if (!WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName))
        {
            gridProcesses.ShowObjectMenu = false;
        }

        // Control initialization
        gridProcesses.OnAction += gridProcesses_OnAction;
        gridProcesses.OnExternalDataBound += gridProcesses_OnExternalDataBound;
        gridProcesses.ZeroRowsText = GetString("general.nodatafound");
    }


    /// <summary>
    ///  Initializes master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        if (!CurrentUser.IsGlobalAdministrator)
        {
            PageTitle title = CurrentMaster.Title;
            title.TitleText = GetString("ma.automationprocess.list");
            title.TitleImage = GetImageUrl("Objects/MA_AutomationProcess/object.png");
            title.HelpName = "helpTopic";
            title.HelpTopicName = "ma_process_list";
        }

        if (WorkflowInfoProvider.IsMarketingAutomationAllowed())
        {
            HeaderAction newProcess = new HeaderAction()
            {
                // New process link
                Text = GetString("ma.newprocess"),
                ImageUrl = GetImageUrl("Objects/MA_AutomationProcess/add.png"),
                RedirectUrl = AddSiteQuery("Process/New.aspx", null),
                Enabled = WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName)
            };

            AddHeaderAction(newProcess);
        }
    }


    protected object gridProcesses_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "recurrencetype":
                return WorkflowHelper.GetWorkflowRecurrenceTypeString((ProcessRecurrenceTypeEnum)ValidationHelper.GetInteger(parameter, (int)ProcessRecurrenceTypeEnum.Recurring));

            case "delete":
                if (!WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName))
                {
                    ImageButton btn = (ImageButton)sender;
                    btn.Enabled = false;
                    btn.Attributes.Add("src", GetImageUrl("Design/Controls/UniGrid/Actions/DeleteDisabled.png"));
                }
                break;
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridProcesses_OnAction(string actionName, object actionArgument)
    {
        int processId = Convert.ToInt32(actionArgument);

        switch (actionName)
        {
            case "edit":
                URLHelper.Redirect(AddSiteQuery("Process/Frameset.aspx?processid=" + processId, null));
                break;

            case "delete":
                if (WorkflowInfoProvider.CheckProcessDependencies(processId))
                {
                    ShowError(GetString("MA.process.CannotDeleteUsed"));
                    return;
                }
                else
                {
                    // Delete the workflow
                    WorkflowInfoProvider.DeleteWorkflowInfo(processId);
                }
                break;
        }
    }
}