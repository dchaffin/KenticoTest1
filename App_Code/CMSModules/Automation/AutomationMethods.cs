using System;
using System.Collections.Generic;

using CMS.GlobalHelper;
using CMS.WorkflowEngine;

/// <summary>
/// Automation methods - wrapping methods for macro resolver.
/// </summary>
public class AutomationMethods
{
    /// <summary>
    /// Registers all automation methods to macro resolver.
    /// </summary>
    public static void RegisterMethods()
    {
        MacroMethod passedThroughActions = new MacroMethod("PassedThroughAutomationActions", PassedThroughActions)
        {
            Comment = "Returns true if process passed through specified actions.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(AutomationStateInfo) }
        };
        passedThroughActions.AddParameter("state", typeof(AutomationStateInfo), "Process instance to check.");
        passedThroughActions.AddParameter("actions", typeof(string), "Action names separated with a semicolon");
        passedThroughActions.AddParameter("allActions", typeof(string), "If true, process must have passed through all specified actions. One of them is sufficient otherwise.");
        MacroMethods.RegisterMethod(passedThroughActions);

        MacroMethod passedThroughSteps = new MacroMethod("PassedThroughAutomationSteps", PassedThroughSteps)
        {
            Comment = "Returns true if process passed through specified tags.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(AutomationStateInfo) }
        };
        passedThroughSteps.AddParameter("state", typeof(AutomationStateInfo), "Process instance to check.");
        passedThroughSteps.AddParameter("steps", typeof(string), "Step names separated with a semicolon");
        passedThroughSteps.AddParameter("allSteps", typeof(bool), "If true, process must have passed through all specified steps. One of them is sufficient otherwise.");
        MacroMethods.RegisterMethod(passedThroughSteps);
    }


    /// <summary>
    /// Returns true if process passed through one/all of the selected automation actions in history.
    /// </summary>
    /// <param name="parameters">
    /// Process instance to be checked;
    /// Automation action names separated with a semicolon;
    /// Indicator whether all of the selected action should be required.
    /// </param>
    public static object PassedThroughActions(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return AutomationFunctions.PassedThroughActions(parameters[0], ValidationHelper.GetString(parameters[1], null), false);

            case 3:
                return AutomationFunctions.PassedThroughActions(parameters[0], ValidationHelper.GetString(parameters[1], null), ValidationHelper.GetBoolean(parameters[2], false));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if process passed through one/all of the selected automation steps in history.
    /// </summary>
    /// <param name="parameters">
    /// Process instance to be checked;
    /// Automation step names separated with a semicolon;
    /// Indicator whether all of the selected steps should be required.
    /// </param>
    public static object PassedThroughSteps(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return AutomationFunctions.PassedThroughSteps(parameters[0], ValidationHelper.GetString(parameters[1], null), false);

            case 3:
                return AutomationFunctions.PassedThroughSteps(parameters[0], ValidationHelper.GetString(parameters[1], null), ValidationHelper.GetBoolean(parameters[2], false));

            default:
                throw new NotSupportedException();
        }
    }
}