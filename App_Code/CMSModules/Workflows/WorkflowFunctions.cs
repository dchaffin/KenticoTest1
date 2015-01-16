using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;
using CMS.CMSHelper;
using CMS.GlobalHelper;

/// <summary>
/// Functions for workflow macro methods.
/// </summary>
public static class WorkflowFunctions
{
    #region "Public methods"

    /// <summary>
    /// Returns true if document had passed through one/all selected workflow actions.
    /// </summary>
    /// <param name="document">Document to check</param>
    /// <param name="actions">Workflow action names separated with a semicolon</param>
    /// <param name="allActions">If true all actions must have been passed.</param>
    public static bool PassedThroughActions(object document, string actions, bool allActions)
    {
        TreeNode doc = document as TreeNode;
        if (doc != null)
        {
            if (!String.IsNullOrEmpty(actions))
            {
                // Get IDs of action steps this document visited in history
                string historyWhere = String.Format("StepType = {0} AND HistoryObjectID = {1} AND HistoryObjectType = '{2}'", (int)WorkflowStepTypeEnum.Action, doc.DocumentID, DocumentObjectType.DOCUMENT);
                var history = WorkflowHistoryInfoProvider.GetWorkflowHistories(historyWhere, null, 0, "StepID");

                // Get action IDs of actions associated to the visited action steps
                string stepWhere = SqlHelperClass.GetWhereCondition("StepID", history.Select<WorkflowHistoryInfo, int>(h => h.StepID).ToList());
                var actionSteps = WorkflowStepInfoProvider.GetWorkflowSteps(stepWhere, null, 0, "StepActionID");

                string[] selectedActions = actions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                // Get action infos to visited actions that have selected name
                string actionWhere = SqlHelperClass.GetWhereCondition("ActionID", actionSteps.Select<WorkflowStepInfo, int>(s => s.StepActionID).ToList());
                actionWhere = SqlHelperClass.AddWhereCondition(actionWhere, SqlHelperClass.GetWhereCondition("ActionName", selectedActions));

                var actionInfos = WorkflowActionInfoProvider.GetWorkflowActions(actionWhere, null, "ActionName").Items;

                // Return true if all/any actions were visited in history
                return allActions ? (actionInfos.Count == selectedActions.Length) : (actionInfos.Count > 0);
            }
            else if (allActions)
            {
                // No actions were selected
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Returns true if document had passed through one/all specified workflow steps.
    /// </summary>
    /// <param name="document">Document to check</param>
    /// <param name="steps">Workflow step names separated with a semicolon</param>
    /// <param name="allSteps">If true all specified steps must have been passed.</param>
    public static bool PassedThroughSteps(object document, string steps, bool allSteps)
    {
        TreeNode doc = document as TreeNode;
        if (doc != null)
        {
            if (!String.IsNullOrEmpty(steps))
            {
                string[] stepArray = steps.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                // Get only the step names of workflow steps visited in history that are selected by user
                string where = String.Format("[HistoryObjectID] = {0} AND HistoryObjectType = '{1}' AND {2}", doc.DocumentID, DocumentObjectType.DOCUMENT, SqlHelperClass.GetWhereCondition("StepName", stepArray));

                var history = WorkflowHistoryInfoProvider.GetWorkflowHistories(where, null, 0, "StepName").Items;
                if (history.Count > 0)
                {
                    // At least of the selected steps was found in the history
                    if (!allSteps)
                    {
                        return true;
                    }

                    // Check whether all of the selected steps are in the history
                    return stepArray.All(s => history.Any<WorkflowHistoryInfo>(h => h.StepName.EqualsCSafe(s, true)));
                }
            }
            else if (allSteps)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
