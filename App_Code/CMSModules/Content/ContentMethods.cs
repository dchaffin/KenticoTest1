using System;
using System.Collections.Generic;

using CMS.GlobalHelper;
using CMS.DocumentEngine;

/// <summary>
/// Content methods - wrapping methods for macro resolver.
/// </summary>
public class ContentMethods
{
    /// <summary>
    /// Register all content macro methods to re
    /// </summary>
    public static void RegisterMethods()
    {
        MacroMethod isInCategories = new MacroMethod("IsInCategories", IsInCategories)
        {
            Comment = "Returns true if document is in selected categories.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TreeNode) }
        };
        isInCategories.AddParameter("document", typeof(TreeNode), "Document to check.");
        isInCategories.AddParameter("categories", typeof(string), "Category names separated with a semicolon.");
        isInCategories.AddParameter("allCategories", typeof(bool), "If true, document must be in all of the specified categories.");
        MacroMethods.RegisterMethod(isInCategories);

        MacroMethod hasTags = new MacroMethod("HasTags", HasTags)
        {
            Comment = "Returns true if document has specified tags.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TreeNode) }
        };
        hasTags.AddParameter("document", typeof(TreeNode), "Document to check.");
        hasTags.AddParameter("tags", typeof(string), "List of tags separated with a semicolon.");
        hasTags.AddParameter("allTags", typeof(bool), "If true, document must have all specified tags. If false, one of specified tags is sufficient.");
        MacroMethods.RegisterMethod(hasTags);

        MacroMethod isInRelationship = new MacroMethod("IsInRelationship", IsInRelationship)
        {
            Comment = "Returns true if document has left or right relationship with specified document.",
            Type = typeof(bool),
            MinimumParameters = 4,
            AllowedTypes = new List<Type>() { typeof(TreeNode) }
        };
        isInRelationship.AddParameter("document", typeof(TreeNode), "Document to check.");
        isInRelationship.AddParameter("side", typeof(string), "Side of checked document in the relationship.");
        isInRelationship.AddParameter("relationship", typeof(string), "Relationship name.");
        isInRelationship.AddParameter("relatedDocument", typeof(string), "Related document.");
        isInRelationship.AddParameter("relatedDocumentSite", typeof(string), "Related document site name.");
        MacroMethods.RegisterMethod(isInRelationship);

        MacroMethod isTranslatedTo = new MacroMethod("IsTranslatedTo", IsTranslatedTo)
        {
            Comment = "Returns true if document is translated to one of selected cultures.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TreeNode) }
        };
        isTranslatedTo.AddParameter("document", typeof(TreeNode), "Document to check.");
        isTranslatedTo.AddParameter("cultures", typeof(string), "Culture codes separated with a semicolon.");
        isTranslatedTo.AddParameter("publishedOnly", typeof(bool), "Indicates whether only published translations should be considered.");
        MacroMethods.RegisterMethod(isTranslatedTo);
    }


    /// <summary>
    /// Returns true if document is in one/all of the selected categories.
    /// </summary>
    /// <param name="parameters">
    /// Document to be checked;
    /// Category names separated with a semicolon;
    /// Indicator whether all the selected categories should be required.
    /// </param>
    public static object IsInCategories(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return ContentFunctions.IsInCategories(parameters[0], ValidationHelper.GetString(parameters[1], null), false);

            case 3:
                return ContentFunctions.IsInCategories(parameters[0], ValidationHelper.GetString(parameters[1], null), ValidationHelper.GetBoolean(parameters[2], false));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if document is translated to one/all of the selected cultures.
    /// </summary>
    /// <param name="parameters">
    /// Document to be checked;
    /// Culture codes separated with a semicolon;
    /// Indicator whether only published versions should be considered.
    /// </param>
    public static object IsTranslatedTo(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return ContentFunctions.IsTranslatedTo(parameters[0], ValidationHelper.GetString(parameters[1], null), false);

            case 3:
                return ContentFunctions.IsTranslatedTo(parameters[0], ValidationHelper.GetString(parameters[1], null), ValidationHelper.GetBoolean(parameters[2], false));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if document has any/all of the specified tags.
    /// </summary>
    /// <param name="parameters">
    /// Document to be checked;
    /// Tags separated with a semicolon;
    /// (optional)Flag indicating whether all specified tags must be present.
    /// </param>
    public static object HasTags(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return ContentFunctions.HasTags(parameters[0], ValidationHelper.GetString(parameters[1], null), false);

            case 3:
                return ContentFunctions.HasTags(parameters[0], ValidationHelper.GetString(parameters[1], null), ValidationHelper.GetBoolean(parameters[2], false));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if document is in specified relationship with selected document.
    /// </summary>
    /// <param name="parameters">
    /// Document to be checked;
    /// Relationship side;
    /// Relationship name;
    /// Alias path of selected document;
    /// </param>
    public static object IsInRelationship(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 5:
                return ContentFunctions.IsInRelationship(parameters[0], ValidationHelper.GetString(parameters[1], null), ValidationHelper.GetString(parameters[2], null), ValidationHelper.GetString(parameters[3], null), ValidationHelper.GetString(parameters[4], null));

            default:
                throw new NotSupportedException();
        }
    }
}