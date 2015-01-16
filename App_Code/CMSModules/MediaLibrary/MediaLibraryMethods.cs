using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;

/// <summary>
/// Media library methods - wrapping methods for macro resolver.
/// </summary>
public static class MediaLibraryMethods
{
    /// <summary>
    /// Registers all media library methods to macro resolver.
    /// </summary>
    public static void RegisterMethods()
    {
        // Get media file URL
        MacroMethod getMediaFileUrl = new MacroMethod("GetMediaFileUrl", GetMediaFileUrl)
        {
            Comment = "Returns direct URL to the media file, user permissions are not checked.",
            Type = typeof(string),
            MinimumParameters = 5,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getMediaFileUrl.AddParameter("libraryId", typeof(object), "Media library ID.");
        getMediaFileUrl.AddParameter("filePath", typeof(object), "File path.");
        getMediaFileUrl.AddParameter("fileGuid", typeof(object), "File GUID.");
        getMediaFileUrl.AddParameter("fileName", typeof(object), "File name.");
        getMediaFileUrl.AddParameter("useSecureLinks", typeof(object), "Determines whether to generate secure link.");
        getMediaFileUrl.AddParameter("downloadlink", typeof(object), "Determines whether disposition parametr should be added to permanent link.");
        MacroMethods.RegisterMethod(getMediaFileUrl);

        // Get media file URL with check
        MacroMethod getMediaFileUrlWithCheck = new MacroMethod("GetMediaFileUrlWithCheck", GetMediaFileUrlWithCheck)
        {
            Comment = "Returns URL to media file which is rewritten to calling GetMediaFile.aspx page where user permissions are checked.",
            Type = typeof(string),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getMediaFileUrlWithCheck.AddParameter("fileGuid", typeof(object), "File GUID.");
        getMediaFileUrlWithCheck.AddParameter("fileName", typeof(object), "File name.");
        MacroMethods.RegisterMethod(getMediaFileUrlWithCheck);

        // Get media file direct URL
        MacroMethod getMediaFileDirectUrl = new MacroMethod("GetMediaFileDirectUrl", GetMediaFileDirectUrl)
        {
            Comment = "Returns direct URL to the media file, user permissions are not checked.",
            Type = typeof(string),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getMediaFileDirectUrl.AddParameter("libraryId", typeof(object), "Media library ID.");
        getMediaFileDirectUrl.AddParameter("filePath", typeof(object), "File path.");
        MacroMethods.RegisterMethod(getMediaFileDirectUrl);

        // Get media file detail URL
        MacroMethod getMediaFileDetailUrl = new MacroMethod("GetMediaFileDetailUrl", GetMediaFileDetailUrl)
        {
            Comment = "Returns URL to detail of media file.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getMediaFileDetailUrl.AddParameter("fileId", typeof(object), "File ID.");
        getMediaFileDetailUrl.AddParameter("parameter", typeof(string), "Query parameter name (\"fileId\" by default).");
        MacroMethods.RegisterMethod(getMediaFileDetailUrl);
    }


    /// <summary>
    /// Returns direct URL to the media file, user permissions are not checked.
    /// </summary>
    /// <param name="parameters">
    /// (Media library ID; File path; File GUID; File name; Generate secure link) OR 
    /// (Media library ID; File path; File GUID; File name; Generate secure link; Determines whether disposition parametr should be added to permanent link)
    /// </param>
    public static object GetMediaFileUrl(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 5:
                return MediaLibraryFunctions.GetMediaFileUrl(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);

            case 6:
                return MediaLibraryFunctions.GetMediaFileUrl(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns URL to media file which is rewritten to calling GetMediaFile.aspx page where user permissions are checked.
    /// </summary>
    /// <param name="parameters">File GUID; File name</param>
    public static object GetMediaFileUrlWithCheck(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return MediaLibraryFunctions.GetMediaFileUrl(parameters[0], parameters[1]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns direct URL to the media file, user permissions are not checked.
    /// </summary>
    /// <param name="libraryId">Media library ID</param>
    /// <param name="filePath">File path</param>
    public static string GetMediaFileDirectUrl(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return MediaLibraryFunctions.GetMediaFileDirectUrl(parameters[0], parameters[1]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns URL to detail of media file.
    /// </summary>
    /// <param name="parameters">(FileID) OR (File ID; Query parameter)</param>
    public static object GetMediaFileDetailUrl(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return MediaLibraryFunctions.GetMediaFileDetailUrl(parameters[0]);

            case 2:
                return MediaLibraryFunctions.GetMediaFileDetailUrl(ValidationHelper.GetString(parameters[1], ""), parameters[0]);

            default:
                throw new NotSupportedException();
        }
    }
}