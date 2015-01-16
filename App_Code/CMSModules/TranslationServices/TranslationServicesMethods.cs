using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.DocumentEngine;
using CMS.TranslationServices;

/// <summary>
/// Translation services methods - wrapping methods for macro resolver.
/// </summary>
public class TranslationServicesMethods
{
    /// <summary>
    /// Registers all translation services methods to macro resolver.
    /// </summary>
    public static void RegisterMethods()
    {
        // Get user name
        MacroMethod isTranslationReady = new MacroMethod("IsTranslationReady", IsTranslationReady)
        {
            Comment = "Returns true if there is at least one translation submission item with target XLIFF ready to import.",
            Type = typeof(bool),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(CMS.DocumentEngine.TreeNode), typeof(DocumentInfo) }
        };
        isTranslationReady.AddParameter("document", typeof(CMS.DocumentEngine.TreeNode), "Document to check.");
        MacroMethods.RegisterMethod(isTranslationReady);

        MacroMethod getTranslationPriority = new MacroMethod("GetTranslationPriority", GetTranslationPriority)
        {
            Comment = "Returns priority name from priority integer constant.",
            Type = typeof(string),
            MinimumParameters = 1,
        };
        getTranslationPriority.AddParameter("priorty", typeof(int), "Priority integer constant.");
        MacroMethods.RegisterMethod(getTranslationPriority);
    }


    /// <summary>
    /// Returns true if there is at least one translation submission item with target XLIFF.
    /// </summary>
    /// <param name="parameters">Document to check</param>
    public static object IsTranslationReady(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return TranslationServicesFunctions.IsTranslationReady(parameters[0]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns translation priority text.
    /// </summary>
    /// <param name="parameters">Priority integer constant</param>
    public static string GetTranslationPriority(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return TranslationServiceHelper.GetPriorityText(ValidationHelper.GetInteger(parameters[0], 0));

            default:
                throw new NotSupportedException();
        }
    }
}