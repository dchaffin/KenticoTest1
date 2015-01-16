using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;

/// <summary>
/// Blog methods - wrapping methods for macro resolver.
/// </summary>
public class BlogMethods
{
    /// <summary>
    /// Registers all blog methods to macro resolver.
    /// </summary>
    public static void RegisterMethods()
    {
        // Get user name
        MacroMethod getUserName = new MacroMethod("GetUserName", GetUserName)
        {
            Comment = "Returns user name.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getUserName.AddParameter("userId", typeof(object), "User ID.");
        MacroMethods.RegisterMethod(getUserName);

        // Get user full name
        MacroMethod getUserFullName = new MacroMethod("GetUserFullName", GetUserFullName)
        {
            Comment = "Returns user full name.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getUserFullName.AddParameter("userId", typeof(object), "User ID.");
        MacroMethods.RegisterMethod(getUserFullName);

        // Get blog comment count
        MacroMethod getBlogCommentsCount = new MacroMethod("GetBlogCommentsCount", GetBlogCommentsCount)
        {
            Comment = "Returns number of comments of given blog.",
            Type = typeof(int),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getBlogCommentsCount.AddParameter("postId", typeof(object), "Post document ID.");
        getBlogCommentsCount.AddParameter("postAliasPath", typeof(object), "Post alias path.");
        getBlogCommentsCount.AddParameter("includingTrackbacks", typeof(bool), "Indicates if trackback comments should be included (true by default).");
        MacroMethods.RegisterMethod(getBlogCommentsCount);

        // Get document tags
        MacroMethod getDocumentTags = new MacroMethod("GetDocumentTags", GetDocumentTags)
        {
            Comment = "Gets a list of links of tags assigned for the specific document pointing to the page with URL specified.",
            Type = typeof(string),
            MinimumParameters = 3,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getDocumentTags.AddParameter("documentGroupId", typeof(object), "ID of the group document tags belong to.");
        getDocumentTags.AddParameter("documentTags", typeof(object), "String containing all the tags related to the document.");
        getDocumentTags.AddParameter("documentListPageUrl", typeof(string), "URL of the page displaying other documents of the specified tag.");
        MacroMethods.RegisterMethod(getDocumentTags);
    }


    /// <summary>
    /// Returns user name.
    /// </summary>
    /// <param name="parameters">User id</param>
    public static object GetUserName(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return BlogFunctions.GetUserName(parameters[0]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns user full name.
    /// </summary>
    /// <param name="parameters">User id</param>
    public static object GetUserFullName(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return BlogFunctions.GetUserFullName(parameters[0]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns number of comments of given blog.
    /// </summary>
    /// <param name="parameters">(Post document id; Post alias path) OR (Post document id; Post alias path; Indicates if trackback comments should be included)</param>
    public static object GetBlogCommentsCount(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return BlogFunctions.GetBlogCommentsCount(parameters[0], parameters[1]);

            case 3:
                return BlogFunctions.GetBlogCommentsCount(parameters[0], parameters[1], ValidationHelper.GetBoolean(parameters[2], true));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Gets a list of links of tags assigned for the specific document pointing to the page with URL specified.
    /// </summary>
    /// <param name="parameters">ID of the group document tags belong to; String containing all the tags related to the document; URL of the page displaying other documents of the specified tag</param>
    public static object GetDocumentTags(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 3:
                return BlogFunctions.GetDocumentTags(parameters[0], parameters[1], ValidationHelper.GetString(parameters[2], ""));

            default:
                throw new NotSupportedException();
        }
    }
}