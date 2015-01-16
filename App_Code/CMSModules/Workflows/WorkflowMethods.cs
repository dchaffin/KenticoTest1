using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

using CMS.GlobalHelper;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.SettingsProvider;

/// <summary>
/// Workflow methods - wrapping methods for macro resolver.
/// </summary>
public static class WorkflowMethods
{
    /// <summary>
    /// Registers all workflow methods to macro resolver.
    /// </summary>
    public static void RegisterMethods()
    {
        MacroMethod passedThroughActions = new MacroMethod("PassedThroughActions", PassedThroughActions)
        {
            Comment = "Returns true if document passed through specified actions.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TreeNode) }
        };
        passedThroughActions.AddParameter("document", typeof(TreeNode), "Document to check.");
        passedThroughActions.AddParameter("actions", typeof(string), "Action names separated with a semicolon");
        passedThroughActions.AddParameter("allActions", typeof(string), "If true, document must have passed through all specified actions. One of them is sufficient otherwise.");
        MacroMethods.RegisterMethod(passedThroughActions);

        MacroMethod passedThroughSteps = new MacroMethod("PassedThroughSteps", PassedThroughSteps)
        {
            Comment = "Returns true if document passed through specified tags.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TreeNode) }
        };
        passedThroughSteps.AddParameter("document", typeof(TreeNode), "Document to check.");
        passedThroughSteps.AddParameter("steps", typeof(string), "Step names separated with a semicolon");
        passedThroughSteps.AddParameter("allSteps", typeof(bool), "If true, document must have passed through all specified steps. One of them is sufficient otherwise.");
        MacroMethods.RegisterMethod(passedThroughSteps);
    }


    /// <summary>
    /// Returns true if document passed through one/all of the selected workflow actions in history.
    /// </summary>
    /// <param name="parameters">
    /// Document to be checked;
    /// Workflow action names separated with a semicolon;
    /// Indicator whether all of the selected action should be required.
    /// </param>
    public static object PassedThroughActions(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return WorkflowFunctions.PassedThroughActions(parameters[0], ValidationHelper.GetString(parameters[1], null), false);

            case 3:
                return WorkflowFunctions.PassedThroughActions(parameters[0], ValidationHelper.GetString(parameters[1], null), ValidationHelper.GetBoolean(parameters[2], false));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if document passed through one/all of the selected workflow steps in history.
    /// </summary>
    /// <param name="parameters">
    /// Document to be checked;
    /// Workflow step names separated with a semicolon;
    /// Indicator whether all of the selected steps should be required.
    /// </param>
    public static object PassedThroughSteps(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return WorkflowFunctions.PassedThroughSteps(parameters[0], ValidationHelper.GetString(parameters[1], null), false);

            case 3:
                return WorkflowFunctions.PassedThroughSteps(parameters[0], ValidationHelper.GetString(parameters[1], null), ValidationHelper.GetBoolean(parameters[2], false));

            default:
                throw new NotSupportedException();
        }
    }
}