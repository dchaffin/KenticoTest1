using System;
using System.Collections.Generic;

using CMS.GlobalHelper;

/// <summary>
/// Forum methods - wrapping methods for macro resolver.
/// </summary>
public static class ForumMethods
{
    /// <summary>
    /// Registers all forum methods to macro resolver.
    /// </summary>
    public static void RegisterMethods()
    {
        // Get post URL
        MacroMethod getPostURL = new MacroMethod("GetPostURL", GetPostURL)
        {
            Comment = "Returns link to selected post.",
            Type = typeof(string),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getPostURL.AddParameter("postIdPath", typeof(object), "Post ID path.");
        getPostURL.AddParameter("forumId", typeof(object), "Forum ID.");
        getPostURL.AddParameter("aliasPath", typeof(object), "Alias path.");
        MacroMethods.RegisterMethod(getPostURL);
    }


    /// <summary>
    /// Returns link to selected post.
    /// </summary>
    /// <param name="parameters">(Post ID path; Forum ID) OR (Post ID path; Forum ID; Alias path)</param>
    public static object GetPostURL(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return ForumFunctions.GetPostURL(parameters[0], parameters[1]);

            case 3:
                return ForumFunctions.GetPostURL(parameters[0], parameters[1], parameters[2]);

            default:
                throw new NotSupportedException();
        }
    }
}