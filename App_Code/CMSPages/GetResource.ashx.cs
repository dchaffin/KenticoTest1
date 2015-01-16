using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Text;
using System.Web;

using CMS.CMSHelper;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.IO.Compression;
using CMS.OutputFilter;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

using Microsoft.Ajax.Utilities;
using ThoughtWorks.QRCode.Codec;

/// <summary>
/// Handler that serves minified and compressed resources.
/// </summary>
public class ResourceHandler : IHttpHandler
{
    #region "Constants"

    /// <summary>
    /// List of the allowed general file extensions
    /// </summary>
    private static List<string> mAllowedFileExtensions = new List<string>() { ".woff", ".eot", ".svg", ".ttf", ".txt" };


    /// <summary>
    /// Supported querystring argument used to identify javascript files.
    /// </summary>
    private const string JS_FILE_ARGUMENT = "scriptfile";

    /// <summary>
    /// Supported querystring argument used to identify QR code
    /// </summary>
    private const string QR_CODE_ARGUMENT = "qrcode";

    /// <summary>
    /// Supported querystring argument used to identify image files.
    /// </summary>
    private const string IMAGE_FILE_ARGUMENT = "image";


    /// <summary>
    /// Supported querystring argument used to identify general files
    /// </summary>
    private const string FILE_ARGUMENT = "file";


    /// <summary>
    /// Supported querystring argument used to identify newsletter stylesheets stored in a database.
    /// </summary>
    private const string NEWSLETTERCSS_DATABASE_ARGUMENT = "newslettertemplatename";


    /// <summary>
    /// Extension of a CSS file.
    /// </summary>
    private const string CSS_FILE_EXTENSION = ".css";


    /// <summary>
    /// Extension of JS file.
    /// </summary>
    private const string JS_FILE_EXTENSION = ".js";


    /// <summary>
    /// Extension of image file.
    /// </summary>
    private const string IMAGE_FILE_EXTENSION = "##IMAGE##";


    /// <summary>
    /// Extension of general file.
    /// </summary>
    private const string FILE_EXTENSION = "##FILE##";


    /// <summary>
    /// Argument delimiter used for separating multiple values in a parameter.
    /// </summary>
    private static readonly char[] ARGUMENT_DELIMITER = { ';' };

    #endregion


    #region "Variables"

    /// <summary>
    /// List of QR code versions that are supported by the system
    /// </summary>
    private static List<bool?> mQRVersionsSupported = new List<bool?>();

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets a value indicating whether another request can use the IHttpHandler instance.
    /// </summary>
    public bool IsReusable
    {
        get
        {
            return true;
        }
    }


    /// <summary>
    /// Site name.
    /// </summary>
    private static string SiteName
    {
        get
        {
            return CMSAppBase.ApplicationInitialized ? CMSContext.CurrentSiteName : string.Empty;
        }
    }


    /// <summary>
    /// Gets if page belongs to the live site, otherwise it's a CMS or preview mode page.
    /// </summary>
    private static bool IsLiveSite
    {
        get
        {
            return (CMSContext.ViewMode == ViewModeEnum.LiveSite);
        }
    }


    /// <summary>
    /// Gets the number of minutes the resource will be cached on the server.
    /// </summary>
    private static int CacheMinutes
    {
        get
        {
            if (CMSAppBase.ApplicationInitialized)
            {
                return CacheHelper.CacheImageMinutes(SiteName);
            }
            else
            {
                return 0;
            }
        }
    }


    /// <summary>
    /// Gets the number of minutes the resource will be cached on the client.
    /// </summary>
    private static int ClientCacheMinutes
    {
        get
        {
            if (CMSAppBase.ApplicationInitialized)
            {
                return CacheHelper.ClientCacheMinutes(SiteName);
            }
            else
            {
                return 0;
            }
        }
    }


    /// <summary>
    /// Gets if the client caching is enabled.
    /// </summary>
    private static bool ClientCacheEnabled
    {
        get
        {
            return CacheHelper.ClientCacheRequested;
        }
    }


    /// <summary>
    /// Gets if the client cache should be revalidated to ensure all data is up-to-date.
    /// </summary>
    private static bool RevalidateClientCache
    {
        get
        {
            return CMSAppBase.ApplicationInitialized ? CacheHelper.RevalidateClientCache(SiteName) : true;
        }
    }


    /// <summary>
    /// Gets the number of minutes large files (those over maximum allowed size) will be cached on the server.
    /// </summary>
    private static int PhysicalFilesCacheMinutes
    {
        get
        {
            return CacheHelper.PhysicalFilesCacheMinutes;
        }
    }


    /// <summary>
    /// Gets if Not Found response should be used when the resouce cannot be located or if the reuqest should terminate normally.
    /// </summary>
    private static bool Throw404WhenNotFound
    {
        get
        {
            return true;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Processes the incoming HTTP request that and returns the specified stylesheets.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    public void ProcessRequest(HttpContext context)
    {
        // Disable debugging
        if (!DebugHelper.DebugResources)
        {
            // Disable the debugging
            RequestSettings requestSettings = RequestSettings.Current;

            requestSettings.DebugFiles = false;
            requestSettings.DebugSecurity = false;
            requestSettings.DebugCache = false;
            requestSettings.DebugOutput = false;
            requestSettings.DebugRequest = false;
            requestSettings.DebugSQLQueries = false;

            OutputHelper.LogCurrentOutputToFile = false;
        }

        // When no parameters are specified, simply end the response
        if (!context.Request.QueryString.HasKeys())
        {
            SendNoContent(context);
        }

        // Process JS file request
        if (QueryHelper.Contains(JS_FILE_ARGUMENT))
        {
            ProcessJSFileRequest(context);
            return;
        }

        // Process QR code request
        if (QueryHelper.Contains(QR_CODE_ARGUMENT))
        {
            ProcessQRCodeRequest(context);
            return;
        }

        // Process image file request
        if (QueryHelper.Contains(IMAGE_FILE_ARGUMENT))
        {
            ProcessImageFileRequest(context);
            return;
        }

        // Process JS file request
        if (QueryHelper.Contains(FILE_ARGUMENT))
        {
            ProcessFileRequest(context);
            return;
        }

        // Transfer to newsletter CSS
        string newsletterTemplateName = QueryHelper.GetString(NEWSLETTERCSS_DATABASE_ARGUMENT, "");
        if (!String.IsNullOrEmpty(newsletterTemplateName))
        {
            context.Server.Transfer("~/CMSModules/Newsletters/CMSPages/GetCSS.aspx?newslettertemplatename=" + newsletterTemplateName);
            return;
        }

        // Load the settings
        CMSCssSettings settings = new CMSCssSettings();
        settings.LoadFromQueryString();

        // Process the request
        ProcessRequest(context, settings);
    }


    /// <summary>
    /// Processes the given request.
    /// </summary>
    /// <param name="context">Http context</param>
    /// <param name="settings">CSS Settings</param>
    private static void ProcessRequest(HttpContext context, CMSCssSettings settings)
    {
        CMSOutputResource resource = null;

        // Get cache setting for physical files
        int cacheMinutes = PhysicalFilesCacheMinutes;
        int clientCacheMinutes = cacheMinutes;

        bool hasVirtualContent = settings.HasVirtualContent();
        if (hasVirtualContent)
        {
            // Use specific cache settings if DB resources are requested
            cacheMinutes = CacheMinutes;
            clientCacheMinutes = ClientCacheMinutes;
        }

        // Try to get data from cache (or store them if not found)
        using (CachedSection<CMSOutputResource> cs = new CachedSection<CMSOutputResource>(ref resource, cacheMinutes, true, null, "getresource", CMSContext.CurrentSiteName, URLHelper.GetCurrentDomain(), context.Request.QueryString, URLHelper.IsSSL))
        {
            // Not found in cache; load the data
            if (cs.LoadData)
            {
                // Load the data
                resource = GetResource(settings, URLHelper.Url.ToString(), cs.Cached);

                // Cache the file
                if ((resource != null) && cs.Cached)
                {
                    cs.CacheDependency = resource.CacheDependency;
                }

                cs.Data = resource;
            }
        }

        // Send response if there's something to send
        if (resource != null)
        {
            bool allowCache = (!hasVirtualContent || ClientCacheEnabled) && CacheHelper.AlwaysCacheResources;
            SendResponse(context, resource, allowCache, CSSHelper.StylesheetMinificationEnabled, clientCacheMinutes, false);
        }
        else
        {
            SendNotFoundResponse(context);
        }
    }

    #endregion


    #region "General methods"

    /// <summary>
    /// Combines the given list of resources into a single resource
    /// </summary>
    /// <param name="resources">Resources to combine</param>
    private static CMSOutputResource CombineResources(IEnumerable<CMSOutputResource> resources)
    {
        StringBuilder data = new StringBuilder();
        StringBuilder etag = new StringBuilder();

        DateTime lastModified = DateTimeHelper.ZERO_TIME;
        List<string> files = new List<string>();

        int count = 0;

        string fileName = null;

        // Build single resource
        foreach (CMSOutputResource resource in resources)
        {
            if (resource != null)
            {
                string newData = resource.Data;

                // Join the data into a single string
                if ((data.Length > 0) && !String.IsNullOrEmpty(newData))
                {
                    // Trim the charset
                    newData = CSSHelper.TrimCharset(newData);
                    if (String.IsNullOrEmpty(newData))
                    {
                        continue;
                    }

                    data.AppendLine();
                    data.AppendLine();
                }

                data.Append(newData);

                // Join e-tags
                if (etag.Length > 0)
                {
                    etag.Append('|');
                }
                etag.Append(resource.Etag);

                // Remember the largest last modified
                if (resource.LastModified > lastModified)
                {
                    lastModified = resource.LastModified;
                }

                files.AddRange(resource.ComponentFiles);

                fileName = resource.Name;
            }

            count++;
        }

        // Build the result
        CMSOutputResource result = new CMSOutputResource()
        {
            Data = data.ToString(),
            Etag = etag.ToString(),
            LastModified = lastModified,
            ComponentFiles = files,
            Extension = ".css"
        };

        // Set the file name
        if (count == 1)
        {
            result.FileName = ValidationHelper.GetSafeFileName(fileName) + ".css";
        }
        else
        {
            result.FileName = "components.css";
        }

        return result;
    }


    /// <summary>
    /// Sends a response containing the requested data.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    /// <param name="resource">Container with the data to serve</param>
    /// <param name="allowCache">True, if client caching is enabled, otherwise false</param>
    /// <param name="minificationEnabled">True, if the data can be served minified, otherwise false</param>
    /// <param name="clientCacheMinutes">Number of minutes after which the content in the client cache expires</param>
    /// <param name="staticContent">If true, the content is static</param>
    private static void SendResponse(HttpContext context, CMSOutputResource resource, bool allowCache, bool minificationEnabled, int clientCacheMinutes, bool staticContent)
    {
        // Set client cache revalidation
        SetRevalidation(context, staticContent);
        
        // Let client use data cached in browser if versions match and there was no change in data
        if (allowCache && IsResourceUnchanged(context, resource))
        {
            context.Response.ContentType = resource.ContentType;
            SendNotModifiedResponse(context, resource.LastModified, resource.Etag, clientCacheMinutes, true);
            return;
        }
        else
        {
            byte[] content = null;

            if (resource.Data == null)
            {
                // Binary content
                content = resource.BinaryData;
            }
            else
            {
                // Text content
                string contentCoding = null;
                content = GetOutputData(context, resource, minificationEnabled, out contentCoding);

                if (contentCoding != ContentCodingEnum.IDENTITY)
                {
                    context.Response.AppendHeader("Content-Encoding", contentCoding);
                    context.Response.AppendHeader("Vary", "Content-Encoding");
                }
            }

            if (content == null)
            {
                // 404 if no content found
                RequestHelper.Respond404();
            }

            // Set client caching
            if (allowCache)
            {
                SetClientCaching(context, allowCache, resource.LastModified, resource.Etag, clientCacheMinutes);
            }

            context.Response.ContentType = resource.ContentType;

            // Set the file name and extension if defined
            if (!String.IsNullOrEmpty(resource.FileName) && !String.IsNullOrEmpty(resource.Extension))
            {
                HTTPHelper.SetFileDisposition(resource.FileName, resource.Extension);
            }

            // Do not send output if there's none
            if ((content != null) && (content.Length > 0))
            {
                context.Response.OutputStream.Write(content, 0, content.Length);
            }
        }
    }


    /// <summary>
    /// Send a Not Found response when the requested data was not located successfully.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    private static void SendNotFoundResponse(HttpContext context)
    {
        if (Throw404WhenNotFound)
        {
            RequestHelper.LogRequestOperation("404NotFound", URLHelper.Url.ToString(), 1);
            RequestHelper.Respond404();
        }
        else
        {
            RequestHelper.EndResponse();
        }
    }


    /// <summary>
    /// Sends the Not Modified response when the data on the client matches those on the server.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    /// <param name="lastModified">Timestamp for the last modification of the data</param>
    /// <param name="etag">Etag used to identify the resources</param>
    /// <param name="clientCacheMinutes">Number of minutes after which the content in the client cache expires</param>
    /// <param name="publicCache">True, if the data can be cached by cache servers on the way, false if only by requesting client</param>
    private static void SendNotModifiedResponse(HttpContext context, DateTime lastModified, string etag, int clientCacheMinutes, bool publicCache)
    {
        // Set the status to Not modified
        context.Response.StatusCode = (int)HttpStatusCode.NotModified;
        context.Response.Cache.SetETag(etag);

        if (publicCache)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
        }

        DateTime expires = DateTime.Now.AddMinutes(clientCacheMinutes);

        // No not allow time in future
        if (lastModified >= DateTime.Now)
        {
            lastModified = DateTime.Now.AddSeconds(-1);
        }

        context.Response.Cache.SetLastModified(lastModified);
        context.Response.Cache.SetExpires(expires);

        RequestHelper.LogRequestOperation("304NotModified", etag, 1);
        RequestHelper.EndResponse();
    }


    /// <summary>
    /// Sends the No Content response when there was no data specified in request.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    private static void SendNoContent(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NoContent;

        RequestHelper.LogRequestOperation("204NoContent", string.Empty, 1);
        RequestHelper.EndResponse();
    }


    /// <summary>
    /// Sets the client cache revalidation.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    /// <param name="staticContent">If true, the content is static</param>
    private static void SetRevalidation(HttpContext context, bool staticContent)
    {
        if (!staticContent && RevalidateClientCache)
        {
            context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        }
        else
        {
            context.Response.Cache.SetRevalidation(HttpCacheRevalidation.None);
        }
    }


    /// <summary>
    /// Sets the client caching.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    /// <param name="lastModified">Timestamp for the last modification of the data</param>
    /// <param name="etag">Etag used to identify the resources</param>
    /// <param name="clientCacheMinutes">Number of minutes after which the content in the client cache expires</param>
    private static void SetClientCaching(HttpContext context, bool allowCache, DateTime lastModified, string etag, int clientCacheMinutes)
    {
        DateTime expires = allowCache ? DateTime.Now.AddMinutes(clientCacheMinutes) : DateTime.Now;

        // No not allow time in future
        if (lastModified >= DateTime.Now)
        {
            lastModified = DateTime.Now.AddSeconds(-1);
        }

        context.Response.Cache.SetLastModified(lastModified);
        context.Response.Cache.SetExpires(expires);

        context.Response.Cache.SetETag(etag);

        context.Response.Cache.SetCacheability(HttpCacheability.Public);
    }


    /// <summary>
    /// Checks if resource in the client cache matches the server version.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    /// <param name="resource">Resource to check</param>
    /// <returns>true, if resource is unchanged, otherwise false</returns>
    private static bool IsResourceUnchanged(HttpContext context, CMSOutputResource resource)
    {
        // Determine the last modified date and etag sent from the browser
        string currentETag = RequestHelper.GetHeader("If-None-Match", string.Empty);
        string ifModified = RequestHelper.GetHeader("If-Modified-Since", string.Empty);

        // If resources match, compare last modification timestamps
        if ((ifModified != string.Empty) && (currentETag == resource.Etag))
        {
            // Get first part of header (colons can delimit additional data)
            DateTime modifiedStamp;
            if (DateTime.TryParse(ifModified.Split(";".ToCharArray())[0], out modifiedStamp))
            {
                return (resource.LastModified.ToUniversalTime() <= modifiedStamp.ToUniversalTime().AddSeconds(1));
            }
        }

        return false;
    }


    /// <summary>
    /// Checks if this is a revalidation request for a physical file that did not change since the last time.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    /// <param name="path">Full physical path to the file</param>
    private static void CheckRevalidation(HttpContext context, string path)
    {
        var lastModified = File.GetLastWriteTime(path);

        // Virtual resource, used only to check if revalidation can be short-circuited
        CMSOutputResource fileResource = new CMSOutputResource()
        {
            Etag = lastModified.ToString(),
            LastModified = lastModified
        };

        if (IsResourceUnchanged(context, fileResource))
        {
            SendNotModifiedResponse(context, fileResource.LastModified, fileResource.Etag, PhysicalFilesCacheMinutes, true);
        }
    }


    /// <summary>
    /// Reads a file in the given path.
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="fileExtension">File extension to check against</param>
    /// <returns>Content of the file</returns>
    private static byte[] ReadBinaryFile(string path, string fileExtension)
    {
        // Return empty string if file doesn't exist or is not supported
        if (!File.Exists(path) || (Path.GetExtension(path) != fileExtension))
        {
            return null;
        }

        // Try to read the contents of the file
        try
        {
            return File.ReadAllBytes(path);
        }
        catch
        {
            return null;
        }
    }


    /// <summary>
    /// Reads a file in the given path.
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="fileExtension">File extension to check against</param>
    /// <returns>Content of the file</returns>
    private static string ReadFile(string path, string fileExtension)
    {
        // Return empty string if file doesn't exist or is not supported
        if (!File.Exists(path) || (Path.GetExtension(path) != fileExtension))
        {
            return null;
        }

        // Try to read the contents of the file
        try
        {
            return File.ReadAllText(path);
        }
        catch
        {
            return string.Empty;
        }
    }


    /// <summary>
    /// Compresses a given text.
    /// </summary>
    /// <param name="resource">Text to compress</param>
    /// <returns>Compressed text</returns>
    private static byte[] Compress(string resource)
    {
        byte[] compressedBuffer = null;

        // Uses in-memory deflate stream to compress the resource
        using (MemoryStream memory = MemoryStream.New())
        {
            using (DeflateStream compressor = DeflateStream.New(memory, CompressionMode.Compress))
            {
                using (StreamWriter writer = StreamWriter.New(compressor))
                {
                    writer.Write(resource);
                }
            }
            compressedBuffer = memory.ToArray();
        }

        return compressedBuffer;
    }


    /// <summary>
    /// Decompresses a given text.
    /// </summary>
    /// <param name="resource">Text to decompress</param>
    /// <returns>Decompressed text</returns>
    private static string Decompress(byte[] resource)
    {
        // Uses in-memory deflate stream to decompress the resource
        using (MemoryStream memory = MemoryStream.New(resource))
        {
            using (DeflateStream decompressor = DeflateStream.New(memory, CompressionMode.Decompress))
            {
                using (StreamReader reader = StreamReader.New(decompressor))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }


    /// <summary>
    /// Minify supplied source according to settings.
    /// </summary>
    /// <param name="resource">Resource to minifz</param>
    /// <param name="minifier">Minifier to use when creating minified version of the data</param>
    /// <param name="minificationEnabled">True, if the data should be minified, otherwise false</param>
    /// <param name="compressionEnabled">True, if data should be compressed, otherwise false</param>
    private static void MinifyResource(CMSOutputResource resource, IResourceMinifier minifier, bool minificationEnabled, bool compressionEnabled)
    {
        if (resource == null)
        {
            return;
        }

        // Set up the settings
        if (minificationEnabled && (minifier != null))
        {
            resource.MinifiedData = minifier.Minify(resource.Data);
        }

        // Check whether the compression is enabled
        if (compressionEnabled && CMSAppBase.ConnectionAvailable)
        {
            compressionEnabled &= RequestHelper.AllowResourceCompression;
        }

        // Compress
        if (compressionEnabled)
        {
            resource.CompressedData = Compress(resource.Data);
        }

        // Compress and minify
        if (minificationEnabled && compressionEnabled)
        {
            resource.MinifiedCompressedData = Compress(resource.MinifiedData);
        }
    }


    /// <summary>
    /// Returns the data which will be served to client depending on minification and compression settings.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    /// <param name="resource">Data container with the data to serve</param>
    /// <param name="minificationEnabled">True, if the data should be minified, otherwise false</param>
    /// <param name="contentCoding">The content coding to use when sending a response</param>
    /// <returns>Data to serve in a form of a byte block</returns>
    private static byte[] GetOutputData(HttpContext context, CMSOutputResource resource, bool minificationEnabled, out string contentCoding)
    {
        // minification must be allowed by the server and minified data must be available
        bool minified = minificationEnabled && resource.ContainsMinifiedData;

        // compression must be allowed by server, supported by client and compressed data must be available
        bool allowCompression = false;
        if (CMSAppBase.ConnectionAvailable)
        {
            allowCompression = RequestHelper.AllowResourceCompression;
        }

        bool compressed = allowCompression && RequestHelper.IsGZipSupported() && resource.ContainsCompressedData;

        // Set default content encoding
        contentCoding = ContentCodingEnum.IDENTITY;

        // Get the proper version of resource to serve based on the settings
        if (!minified && !compressed)
        {
            return Encoding.UTF8.GetBytes(resource.Data);
        }
        else if (minificationEnabled && !compressed)
        {
            return Encoding.UTF8.GetBytes(resource.MinifiedData);
        }
        else if (!minified && compressed)
        {
            contentCoding = ContentCodingEnum.DEFLATE;
            return resource.CompressedData;
        }
        else if (minified && compressed)
        {
            contentCoding = ContentCodingEnum.DEFLATE;
            return resource.MinifiedCompressedData;
        }
        else
        {
            return new byte[0];
        }
    }

    #endregion


    #region "Specialized methods"

    /// <summary>
    /// Processes a request for a QR code
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    private static void ProcessQRCodeRequest(HttpContext context)
    {
        string code = QueryHelper.GetString(QR_CODE_ARGUMENT, string.Empty);
        
        string correction = QueryHelper.GetString("ec", "M");
        string encoding = QueryHelper.GetString("e", "B");

        int size = QueryHelper.GetInteger("s", 4);
        int version = QueryHelper.GetInteger("v", 4);
        int maxSideSize = QueryHelper.GetInteger("maxsidesize", 0);

        Color fgColor = QueryHelper.GetColor("fc", Color.Black);
        Color bgColor = QueryHelper.GetColor("bc", Color.White);

        CMSOutputResource resource = null;

        // Try to get data from cache (or store them if not found)
        using (CachedSection<CMSOutputResource> cs = new CachedSection<CMSOutputResource>(ref resource, PhysicalFilesCacheMinutes, true, null, "getresource|qrcode|", code, encoding, size, version, correction, maxSideSize, fgColor, bgColor, URLHelper.IsSSL))
        {
            // Not found in cache; load the data
            if (cs.LoadData)
            {
                // Validate the request hash
                if (QueryHelper.ValidateHash("hash", null, false))
                {
                    // Prepare the QR code response
                    resource = GetQRCode(code, encoding, size, version, correction, maxSideSize, fgColor, bgColor);
                }

                cs.Data = resource;
            }
        }

        // Send response if there's something to send
        if (resource != null)
        {
            bool allowCache = CacheHelper.AlwaysCacheResources;

            SendResponse(context, resource, allowCache, false, PhysicalFilesCacheMinutes, true);
        }
        else
        {
            SendNotFoundResponse(context);
        }
    }


    /// <summary>
    /// Processes a request for a general file identified by its URL.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    private static void ProcessFileRequest(HttpContext context)
    {
        // Process physical file
        ProcessFileRequest(context, FILE_ARGUMENT, FILE_EXTENSION, false);
    }

    
    /// <summary>
    /// Processes a request for a image file identified by its URL.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    private static void ProcessImageFileRequest(HttpContext context)
    {
        // Get URL to the resource file, resolve it in case it's virtual and map to physical path        
        string url = QueryHelper.GetString(IMAGE_FILE_ARGUMENT, string.Empty);

        // Trim any potential query from path
        int queryIndex = url.IndexOf('?');
        if (queryIndex >= 0)
        {
            url = url.Substring(0, queryIndex);
        }

        if (!url.StartsWith("/") && !url.StartsWith("~/"))
        {
            // Map path to the default folder and a real file
            url = StorageHelper.DEFAULT_IMAGES_PATH + url;

            // If file not found in physical folder, try to map the file
            if (!File.ExistsRelative(url))
            {
                Path.GetMappedPath(ref url);
            }
        }

        bool useCache = (QueryHelper.GetString("chset", null) == null);

        // Process physical file
        ProcessPhysicalFileRequest(context, url, IMAGE_FILE_EXTENSION, false, useCache);
    }


    /// <summary>
    /// Processes a request for a JavaScript file identified by its URL.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    private static void ProcessJSFileRequest(HttpContext context)
    {
        // Check whether to use minification
        bool useMinification = false;
        if (CMSAppBase.ConnectionAvailable)
        {
            useMinification = ScriptHelper.ScriptMinificationEnabled;
        }

        ProcessFileRequest(context, JS_FILE_ARGUMENT, JS_FILE_EXTENSION, useMinification);
    }

    
    /// <summary>
    /// Processes a request for a file.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    /// <param name="queryArgument">Name of the argument whose value specifies the location of the data</param>
    /// <param name="fileExtension">File extension to check against (to prevent serving unauthorized content)</param>
    /// <param name="minificationEnabled">True, if the data should be minified, otherwise false</param>
    private static void ProcessFileRequest(HttpContext context, string queryArgument, string fileExtension, bool minificationEnabled)
    {
        // Get URL to the resource file, resolve it in case it's virtual and map to physical path        
        string url = QueryHelper.GetString(queryArgument, string.Empty);

        // Process physical file
        ProcessPhysicalFileRequest(context, url, fileExtension, minificationEnabled, true);
    }


    /// <summary>
    /// Processes a request for a file.
    /// </summary>
    /// <param name="context">An HTTPContext object that provides references to the intrinsic server objects used to service HTTP requests</param>
    /// <param name="url">File URL</param>
    /// <param name="fileExtension">File extension to check against (to prevent serving unauthorized content)</param>
    /// <param name="minificationEnabled">True, if the data should be minified, otherwise false</param>
    /// <param name="useCache">If true, cache is allowed to be used</param>
    private static void ProcessPhysicalFileRequest(HttpContext context, string url, string fileExtension, bool minificationEnabled, bool useCache)
    {
        // Get physical path
        string path = URLHelper.GetPhysicalPath(URLHelper.GetVirtualPath(url));

        // If this is revalidation request, try quick revalidation check before reading the file
        CheckRevalidation(context, path);

        CMSOutputResource resource = null;

        // Try to get data from cache (or store them if not found)
        using (CachedSection<CMSOutputResource> cs = new CachedSection<CMSOutputResource>(ref resource, PhysicalFilesCacheMinutes, useCache, null, "getresource", path, URLHelper.IsSSL))
        {
            // Not found in cache; load the data
            if (cs.LoadData)
            {
                // Check the image extension
                switch (fileExtension)
                {
                    case IMAGE_FILE_EXTENSION:
                    case FILE_EXTENSION:
                        {
                            // Extension sets
                            string fileExt = Path.GetExtension(path);

                            if ((fileExtension == IMAGE_FILE_EXTENSION) && ImageHelper.IsImage(fileExt))
                            {
                                // Image file
                                fileExtension = fileExt;
                            }
                            else if ((fileExtension == FILE_EXTENSION) && mAllowedFileExtensions.Contains(fileExt.ToLowerCSafe()))
                            {
                                // General file
                                fileExtension = fileExt;
                            }

                            resource = GetFile(new CMSItem(url), fileExtension, false, true);
                        }
                        break;

                    default:
                        {
                            // Retrieve the file resource, rebase client URLs and wrap it in output container
                            resource = GetFile(new CMSItem(url), fileExtension, false, false);

                            MinifyResource(resource, new JavaScriptMinifier(), minificationEnabled, true);
                        }
                        break;

                }
                
                // Cache the file
                if ((resource != null) && cs.Cached)
                {
                    path = StorageHelper.GetRealFilePath(path);

                    if (File.Exists(path))
                    {
                        cs.CacheDependency = new CMSCacheDependency(path);
                    }
                }

                cs.Data = resource;
            }
        }

        // Send response if there's something to send
        if (resource != null)
        {
            bool allowCache = CacheHelper.AlwaysCacheResources;

            SendResponse(context, resource, allowCache, minificationEnabled, PhysicalFilesCacheMinutes, true);
        }
        else
        {
            SendNotFoundResponse(context);
        }
    }


    /// <summary>
    /// Retrieves the specified resources and wraps them in an data container.
    /// </summary>
    /// <param name="settings">CSS settings</param>
    /// <param name="name">Resource name</param>
    /// <param name="cached">If true, the result will be cached</param>
    /// <returns>The data container with the resulting stylesheet data</returns>
    private static CMSOutputResource GetResource(CMSCssSettings settings, string name, bool cached)
    {
        List<CMSOutputResource> resources = new List<CMSOutputResource>();

        // Add files
        if (settings.Files != null)
        {
            foreach (CMSItem item in settings.Files)
            {
                // Get the resource
                CMSOutputResource resource = GetFile(item, CSS_FILE_EXTENSION, true, false);
                resources.Add(resource);
            }
        }

        // Add stylesheets
        if (settings.Stylesheets != null)
        {
            foreach (CMSItem item in settings.Stylesheets)
            {
                // Get the resource
                CMSOutputResource resource = GetStylesheet(item);
                resources.Add(resource);
            }
        }

        // Add web part containers
        if (settings.Containers != null)
        {
            foreach (CMSItem item in settings.Containers)
            {
                // Get the resource
                CMSOutputResource resource = GetContainer(item);
                resources.Add(resource);
            }
        }

        // Add web parts
        if (settings.WebParts != null)
        {
            foreach (CMSItem item in settings.WebParts)
            {
                // Get the resource
                CMSOutputResource resource = GetWebPart(item);
                resources.Add(resource);
            }
        }

        // Add templates
        if (settings.Templates != null)
        {
            foreach (CMSItem item in settings.Templates)
            {
                // Get the resource
                CMSOutputResource resource = GetTemplate(item);
                resources.Add(resource);
            }
        }

        // Add layouts
        if (settings.Layouts != null)
        {
            foreach (CMSItem item in settings.Layouts)
            {
                // Get the resource
                CMSOutputResource resource = GetLayout(item);
                resources.Add(resource);
            }
        }

        // Add device layouts
        if (settings.DeviceLayouts != null)
        {
            foreach (CMSItem item in settings.DeviceLayouts)
            {
                // Get the resource
                CMSOutputResource resource = GetDeviceLayout(item);
                resources.Add(resource);
            }
        }


        // Add transformation containers
        if (settings.Transformations != null)
        {
            foreach (CMSItem item in settings.Transformations)
            {
                // Get the resource
                CMSOutputResource resource = GetTransformation(item);
                resources.Add(resource);
            }
        }

        // Add web part layouts
        if (settings.WebPartLayouts != null)
        {
            foreach (CMSItem item in settings.WebPartLayouts)
            {
                // Get the resource
                CMSOutputResource resource = GetWebPartLayout(item);
                resources.Add(resource);
            }
        }

        // Combine to a single output
        CMSOutputResource result = CombineResources(resources);
        settings.ComponentFiles = result.ComponentFiles;
        result.ContentType = MimeTypeHelper.GetMimetype(CSS_FILE_EXTENSION, "text/css");

        // Resolve the macros
        if (CSSHelper.ResolveMacrosInCSS)
        {
            var context = new MacroContext()
            {
                TrackCacheDependencies = cached
            };

            if (cached)
            {
                // Add the default dependencies
                context.AddCacheDependencies(settings.GetCacheDependencies());
                context.AddFileCacheDependencies(settings.GetFileCacheDependencies());
            }

            result.Data = CMSContext.ResolveMacros(result.Data, context);

            if (cached)
            {
                // Add cache dependency
                result.CacheDependency = CacheHelper.GetCacheDependency(context.FileCacheDependencies, context.CacheDependencies);
            }
        }
        else if (cached)
        {
            // Only the cache dependency from settings
            result.CacheDependency = settings.GetCacheDependency();
        }

        result.Data = HTMLHelper.ResolveCSSClientUrls(result.Data, URLHelper.GetAbsoluteUrl("~/CMSPages/GetResource.ashx"));

        // Minify
        MinifyResource(result, new CssMinifier(), CSSHelper.StylesheetMinificationEnabled && settings.EnableMinification, settings.EnableCompression);

        return result;
    }


    /// <summary>
    /// Retrieves the stylesheet from file
    /// </summary>
    /// <param name="item">File item</param>
    /// <param name="extension">File extension</param>
    /// <param name="resolveCSSUrls">If true, the CSS URLs are resolved in the output</param>
    /// <param name="binary">If true, the file is a binary file</param>
    /// <returns>The stylesheet data (plain version only)</returns>    
    private static CMSOutputResource GetFile(CMSItem item, string extension, bool resolveCSSUrls, bool binary)
    {
        string url = item.Name;
        string path = URLHelper.GetPhysicalPath(URLHelper.GetVirtualPath(url));

        // Get the file content
        string fileContent = null;
        byte[] binaryData = null;

        if (binary)
        {
            // Binary file
            binaryData = ReadBinaryFile(path, extension);
        }
        else
        {
            // Text file
            fileContent = ReadFile(path, extension);
            if (resolveCSSUrls)
            {
                fileContent = HTMLHelper.ResolveCSSClientUrls(fileContent, URLHelper.GetAbsoluteUrl(url));
            }
        }

        var lastModified = File.GetLastWriteTime(path);

        // Build the result
        var resource = new CMSOutputResource()
        {
            Data = fileContent,
            BinaryData = binaryData,
            Name = path,
            Etag = "file|" + lastModified.ToString(),
            LastModified = lastModified,
            FileName = Path.GetFileName(path),
            Extension = extension
        };

        switch (extension.ToLowerCSafe())
        {
            case CSS_FILE_EXTENSION:
                resource.ContentType = "text/css";
                break;
            case JS_FILE_EXTENSION:
                resource.ContentType = "application/x-javascript";
                break;
            default:
                resource.ContentType = MimeTypeHelper.GetMimetype(extension);
                break;
        }

        return resource;
    }


    /// <summary>
    /// Returns true if the given QR code version if supported
    /// </summary>
    /// <param name="version">Version to check</param>
    private static bool QRVersionSupported(int version)
    {
        // Ensure the number of items in the list
        while (mQRVersionsSupported.Count < version)
        {
            mQRVersionsSupported.Add(null);
        }

        bool? supported = mQRVersionsSupported[version - 1];
        if (supported == null)
        {
            // Validate by the file existence if the version is supported
            string path = "~/App_Data/CMSModules/QRCode/[Resources.zip]/qrvfr" + version + ".dat";
            path = StorageHelper.GetFullFilePhysicalPath(path, null);

            supported = File.Exists(path);
            mQRVersionsSupported[version - 1] = supported;
        }

        return supported.Value;
    }


    /// <summary>
    /// Generates the QR code
    /// </summary>
    /// <param name="code">Code to generate by the QR code</param>
    /// <param name="size">Image size, image is rendered with recommended resolution for the QR code</param>
    /// <param name="encoding">Encoding, possible options (B - Byte, AN - Alphanumeric, N - Numeric)</param>
    /// <param name="version">QR code version (by default supported 1 to 10), additional data templates may be put into ~/App_Data/CMS_Modules/QRCode/Resources.zip</param>
    /// <param name="correction">Correction type, possible options (L, M, Q, H)</param>
    /// <param name="maxSideSize">Maximum size of the code in pixels, the code will be resized if larger than this size</param>
    /// <param name="fgColor">Foreground color</param>
    /// <param name="bgColor">Background color</param>
    private static CMSOutputResource GetQRCode(string code, string encoding, int size, int version, string correction, int maxSideSize, Color fgColor, Color bgColor)
    {
        QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

        try
        {
            // Set encoding
            switch (encoding.ToLowerCSafe())
            {
                case "an":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                    break;

                case "n":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                    break;

                default:
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
            }

            // Set scale
            try
            {
                qrCodeEncoder.QRCodeScale = size;
            }
            catch
            {
                qrCodeEncoder.QRCodeScale = 4;
            }

            // Set error correction
            switch (correction.ToLowerCSafe())
            {
                case "l":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                    break;

                case "q":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                    break;

                case "h":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;

                default:
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                    break;
            }

            // Set colors
            qrCodeEncoder.QRCodeForegroundColor = fgColor;
            qrCodeEncoder.QRCodeBackgroundColor = bgColor;

            Image image = null;

            // Attempt to process all versions
            while (version <= 40)
            {
                if (!QRVersionSupported(version))
                {
                    // Move to higher version
                    version++;

                    if (version > 40)
                    {
                        throw new Exception("Version higher than 40 is not supported.");
                    }
                    continue;
                }

                try
                {
                    // Try to get requested version
                    qrCodeEncoder.QRCodeVersion = version;

                    image = qrCodeEncoder.Encode(code);
                    
                    break;
                }
                catch (IndexOutOfRangeException ex)
                {
                    // Try next version to fit the data
                    version++;

                    if (version > 40)
                    {
                        throw ex;
                    }
                }
            }

            byte[] bytes = ImageHelper.GetBytes(image, ImageFormat.Png);

            // Resize to a proper size
            if (maxSideSize > 0)
            {
                // Resize the image by image helper to a proper size
                ImageHelper ih = new ImageHelper(bytes);
                image = ih.GetResizedImage(maxSideSize);

                bytes = ImageHelper.GetBytes(image, ImageFormat.Png);
            }

            // Build the result
            var resource = new CMSOutputResource()
            {
                BinaryData = bytes,
                Name = code,
                Etag = "QRCode",
                LastModified = new DateTime(2012, 1, 1),
                ContentType = MimeTypeHelper.GetMimetype(".png"),
                FileName = "QRCode.png",
                Extension = ".png"
            };

            return resource;
        }
        catch (Exception ex)
        {
            // Report too long text
            string message = "[GetResource.GetQRCode]: Failed to generate QR code with text '" + code + "', this may be caused by the text being too long. Original message: " + ex.Message;

            var newEx = new Exception(message, ex);

            EventLogProvider.LogException("GetResource", "QRCODE", newEx);

            throw newEx;
        }
    }


    /// <summary>
    /// Retrieve the stylesheet either from the database or file if checked out.
    /// </summary>
    /// <param name="item">Stylesheet item</param>
    /// <returns>The stylesheet data (plain version only)</returns>    
    private static CMSOutputResource GetStylesheet(CMSItem item)
    {
        // Get the stylesheet
        CssStylesheetInfo stylesheetInfo = null;
        if (item.Name != null)
        {
            stylesheetInfo = CssStylesheetInfoProvider.GetCssStylesheetInfo(item.Name);
        }
        else
        {
            stylesheetInfo = CssStylesheetInfoProvider.GetCssStylesheetInfo(item.ID);
        }
        if (stylesheetInfo == null)
        {
            return null;
        }

        // Get last modified date with dependency on external storage
        DateTime lastModified = stylesheetInfo.StylesheetLastModified;
        FileInfo fi = stylesheetInfo.GetFileInfo(CssStylesheetInfo.EXTERNAL_COLUMN_CSS);
        if (fi != null)
        {
            lastModified = fi.LastWriteTime.ToUniversalTime();
        }

        string stylesheetName = stylesheetInfo.StylesheetName;

        // Build the output
        var resource = new CMSOutputResource()
        {
            Data = HTMLHelper.ResolveCSSUrls(stylesheetInfo.StylesheetText, URLHelper.ApplicationPath),
            Name = stylesheetName,
            LastModified = lastModified,
            Etag = "cssstylesheet|" + stylesheetInfo.StylesheetVersionGUID.ToString(),
            FileName = stylesheetName + ".css",
            Extension = ".css"
        };

        // Add file dependency on component css file
        if (fi != null)
        {
            resource.ComponentFiles.Add(stylesheetInfo.GetVirtualFileRelativePath(CssStylesheetInfo.EXTERNAL_COLUMN_CSS));
        }

        return resource;
    }


    /// <summary>
    /// Retrieves the stylesheets of the web part container from the database.
    /// </summary>
    /// <param name="item">Container item</param>
    /// <returns>The stylesheet data (plain version only)</returns>
    private static CMSOutputResource GetContainer(CMSItem item)
    {
        WebPartContainerInfo containerInfo = null;
        if (item.Name != null)
        {
            containerInfo = WebPartContainerInfoProvider.GetWebPartContainerInfo(item.Name);
        }
        else
        {
            containerInfo = WebPartContainerInfoProvider.GetWebPartContainerInfo(item.ID);
        }
        if (containerInfo == null)
        {
            return null;
        }

        DateTime lastModified = containerInfo.ContainerLastModified;

        // Build the result
        var resource = new CMSOutputResource()
        {
            Data = HTMLHelper.ResolveCSSUrls(containerInfo.ContainerCSS, URLHelper.ApplicationPath),
            LastModified = lastModified,
            Etag = "webpartcontainer|" + lastModified.ToString(),
            FileName = ValidationHelper.GetSafeFileName(containerInfo.ContainerName),
            Extension = ".css"
        };

        return resource;
    }


    /// <summary>
    /// Retrieves the stylesheets of the web part layout from the database.
    /// </summary>
    /// <param name="item">Layout item</param>
    /// <returns>The stylesheet data (plain version only)</returns>
    private static CMSOutputResource GetWebPartLayout(CMSItem item)
    {
        WebPartLayoutInfo layoutInfo = null;
        if (item.Name != null)
        {
            layoutInfo = WebPartLayoutInfoProvider.GetWebPartLayoutInfo(item.Name);
        }
        else
        {
            layoutInfo = WebPartLayoutInfoProvider.GetWebPartLayoutInfo(item.ID);
        }
        if (layoutInfo == null)
        {
            return null;
        }

        // Get last modified date with dependency on external storage
        DateTime lastModified = layoutInfo.WebPartLayoutLastModified;
        FileInfo fi = layoutInfo.GetFileInfo(WebPartLayoutInfo.EXTERNAL_COLUMN_CSS);
        if (fi != null)
        {
            lastModified = fi.LastWriteTime.ToUniversalTime();
        }

        // Build the result
        var resource = new CMSOutputResource()
        {
            Data = HTMLHelper.ResolveCSSUrls(layoutInfo.WebPartLayoutCSS, URLHelper.ApplicationPath),
            LastModified = lastModified,
            Etag = "webpartlayout|" + layoutInfo.WebPartLayoutVersionGUID,
            FileName = ValidationHelper.GetSafeFileName(layoutInfo.WebPartLayoutFullName) + ".css",
            Extension = ".css"
        };

        if (fi != null)
        {
            resource.ComponentFiles.Add(layoutInfo.GetVirtualFileRelativePath(WebPartLayoutInfo.EXTERNAL_COLUMN_CSS));
        }

        return resource;
    }


    /// <summary>
    /// Retrieves the stylesheets of the page template from the database.
    /// </summary>
    /// <param name="item">Template item</param>
    /// <returns>The stylesheet data (plain version only)</returns>
    private static CMSOutputResource GetTemplate(CMSItem item)
    {
        // Try to get global one
        PageTemplateInfo templateInfo = null;
        if (item.Name != null)
        {
            string templateName = item.Name;

            templateInfo = PageTemplateInfoProvider.GetPageTemplateInfo(templateName);

            // Try to get site one (ad-hoc) if not found
            if (templateInfo == null)
            {
                templateInfo = PageTemplateInfoProvider.GetPageTemplateInfo(templateName, CMSContext.CurrentSiteID);
            }
        }
        else
        {
            templateInfo = PageTemplateInfoProvider.GetPageTemplateInfo(item.ID);
        }
        if (templateInfo == null)
        {
            return null;
        }

        // Get last modified date with dependency on external storage
        DateTime lastModified = templateInfo.PageTemplateLastModified;
        FileInfo fi = templateInfo.GetFileInfo(PageTemplateInfo.EXTERNAL_COLUMN_CSS);
        if (fi != null)
        {
            lastModified = fi.LastWriteTime.ToUniversalTime();
        }
		
        // Build the result
        var resource = new CMSOutputResource()
        {
            Data = HTMLHelper.ResolveCSSUrls(templateInfo.PageTemplateCSS, URLHelper.ApplicationPath),
            LastModified = lastModified,
            Etag = "template|" + templateInfo.PageTemplateVersionGUID,
            FileName = ValidationHelper.GetSafeFileName(templateInfo.CodeName),
            Extension = ".css"
        };

        // Add file dependency on component css file
        if (fi != null)
        {
            resource.ComponentFiles.Add(templateInfo.GetVirtualFileRelativePath(PageTemplateInfo.EXTERNAL_COLUMN_CSS));
        }

        return resource;
    }


    /// <summary>
    /// Retrieves the stylesheets of the layout from the database.
    /// </summary>
    /// <param name="item">Layout item</param>
    /// <returns>The stylesheet data (plain version only)</returns>
    private static CMSOutputResource GetLayout(CMSItem item)
    {
        LayoutInfo layoutInfo = null;
        if (item.Name != null)
        {
            layoutInfo = LayoutInfoProvider.GetLayoutInfo(item.Name);
        }
        else
        {
            layoutInfo = LayoutInfoProvider.GetLayoutInfo(item.ID);
        }
        if (layoutInfo == null)
        {
            return null;
        }

        // Get last modified date with dependency on external storage
        DateTime lastModified = layoutInfo.LayoutLastModified;
        FileInfo fi = layoutInfo.GetFileInfo(LayoutInfo.EXTERNAL_COLUMN_CSS);
        if (fi != null)
        {
            lastModified = fi.LastWriteTime.ToUniversalTime();
        }

        // Build the result
        var resource = new CMSOutputResource()
        {
            Data = HTMLHelper.ResolveCSSUrls(layoutInfo.LayoutCSS, URLHelper.ApplicationPath),
            LastModified = lastModified,
            Etag = "layout|" + layoutInfo.LayoutVersionGUID,
            FileName = ValidationHelper.GetSafeFileName(layoutInfo.LayoutCodeName),
            Extension = ".css"
        };

        // Add file dependency on component css file
        if (fi != null)
        {
            resource.ComponentFiles.Add(layoutInfo.GetVirtualFileRelativePath(LayoutInfo.EXTERNAL_COLUMN_CSS));
        }

        return resource;
    }


    /// <summary>
    /// Retrieves the stylesheets of the device layout from the database.
    /// </summary>
    /// <param name="item">Device layout item</param>
    /// <returns>The stylesheet data (plain version only)</returns>
    private static CMSOutputResource GetDeviceLayout(CMSItem item)
    {
        PageTemplateDeviceLayoutInfo layoutInfo = null;
        if (item.Name != null)
        {
            // Not supported, device layout doesn't contain name
        }
        else
        {
            layoutInfo = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(item.ID);
        }

        if (layoutInfo == null)
        {
            return null;
        }

        // Get last modified date with dependency on external storage
        DateTime lastModified = layoutInfo.LayoutLastModified;
        FileInfo fi = layoutInfo.GetFileInfo(PageTemplateDeviceLayoutInfo.EXTERNAL_COLUMN_CSS);
        if (fi != null)
        {
            lastModified = fi.LastWriteTime.ToUniversalTime();
        }

        // Build the result
        var resource = new CMSOutputResource()
        {
            Data = HTMLHelper.ResolveCSSUrls(layoutInfo.LayoutCSS, URLHelper.ApplicationPath),
            LastModified = lastModified,
            Etag = "layout|" + layoutInfo.LayoutVersionGUID,
            FileName = Convert.ToString(layoutInfo.LayoutID),
            Extension = ".css"
        };

        // Add file dependency on component css file
        if (fi != null)
        {
            resource.ComponentFiles.Add(layoutInfo.GetVirtualFileRelativePath(PageTemplateDeviceLayoutInfo.EXTERNAL_COLUMN_CSS));
        }

        return resource;
    }


    /// <summary>
    /// Retrieves the stylesheet of the web part from the database.
    /// </summary>
    /// <param name="item">Web part item</param>
    /// <returns>The stylesheet data (plain version only)</returns>
    private static CMSOutputResource GetWebPart(CMSItem item)
    {
        WebPartInfo webPartInfo = null;
        if (item.Name != null)
        {
            webPartInfo = WebPartInfoProvider.GetWebPartInfo(item.Name);
        }
        else
        {
            webPartInfo = WebPartInfoProvider.GetWebPartInfo(item.ID);
        }
        if (webPartInfo == null)
        {
            return null;
        }

        // Build the result
        var resource = new CMSOutputResource()
        {
            Data = HTMLHelper.ResolveCSSUrls(webPartInfo.WebPartCSS, URLHelper.ApplicationPath),
            LastModified = webPartInfo.WebPartLastModified,
            Etag = "webpart|" + webPartInfo.WebPartName,
            FileName = ValidationHelper.GetSafeFileName(webPartInfo.WebPartName) + ".css",
            Extension = ".css"
        };

        return resource;
    }


    /// <summary>
    /// Retrieves the stylesheets of the web part layout from the database.
    /// </summary>
    /// <param name="item">Layout item</param>
    /// <returns>The stylesheet data (plain version only)</returns>
    private static CMSOutputResource GetTransformation(CMSItem item)
    {
        TransformationInfo transformationInfo = null;
        if (item.Name != null)
        {
            transformationInfo = TransformationInfoProvider.GetTransformation(item.Name);
        }
        else
        {
            transformationInfo = TransformationInfoProvider.GetTransformation(item.ID);
        }
        if (transformationInfo == null)
        {
            return null;
        }

        // Get last modified date with dependency on external storage
        DateTime lastModified = transformationInfo.TransformationLastModified;
        FileInfo fi = transformationInfo.GetFileInfo(TransformationInfo.EXTERNAL_COLUMN_CSS);
        if (fi != null)
        {
            lastModified = fi.LastWriteTime.ToUniversalTime();
        }

        // Build the result
        var resource = new CMSOutputResource()
        {
            Data = HTMLHelper.ResolveCSSUrls(transformationInfo.TransformationCSS, URLHelper.ApplicationPath),
            LastModified = lastModified,
            Etag = "transformation|" + transformationInfo.TransformationVersionGUID,
            FileName = ValidationHelper.GetSafeFileName(transformationInfo.TransformationFullName) + ".css",
            Extension = ".css"
        };

        // Add file dependency on component css file
        if (fi != null)
        {
            resource.ComponentFiles.Add(transformationInfo.GetVirtualFileRelativePath(TransformationInfo.EXTERNAL_COLUMN_CSS));
        }

        return resource;
    }

    #endregion
}


#region "Minifiers"

/// <summary>
/// Defines an interface a compliant minifier must implement to be usable in resource handler.
/// </summary>
public interface IResourceMinifier
{
    /// <summary>
    /// Returns a minified version of a given resource.
    /// </summary>
    /// <param name="resource">Text to be minified</param>
    /// <returns>Minified resource</returns>
    string Minify(string resource);
}


/// <summary>
/// Minifier for Cascading StyleSheets.
/// </summary>
public class CssMinifier : IResourceMinifier
{
    /// <summary>
    /// Indicates if parsing failed for specific reason
    /// </summary>
    private CssException minificationError = null;


    private bool? mLogMinifierParseError = null;

    // Indicates if errors from minification should be logged
    private bool LogMinifierParseError
    {
        get
        {
            if (mLogMinifierParseError == null)
            {
                mLogMinifierParseError = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSLogCSSMinifierParseError"], false);
            }

            return (bool)mLogMinifierParseError;
        }
    }


    /// <summary>
    /// Returns a minified version of a given CSS.
    /// </summary>
    /// <param name="resource">CSS to be minified</param>
    /// <returns>Minified CSS</returns>
    public string Minify(string resource)
    {
        if (String.IsNullOrEmpty(resource))
        {
            return resource;
        }

        // Reset error
        minificationError = null;

        try
        {
            CssParser parser = new CssParser();
            // Ignore error: 'An underscore is not a valid CSS1 or CSS2 identifier character'
            parser.Settings.IgnoreErrorList = "CSS1008";
            parser.Settings.AllowEmbeddedAspNetBlocks = true;
            parser.CssError += parser_CssError;

            // Parse the resource
            string parsed = parser.Parse(resource);
            if (!String.IsNullOrEmpty(parsed) && (minificationError == null))
            {
                resource = parsed;
            }
        }
        catch (CssException ex)
        {
            minificationError = ex;
        }

        if (minificationError != null)
        {
            if (LogMinifierParseError)
            {
                // Log exception to event log if allowed
                EventLogProvider.LogException("CSS Compression", "MINIFYCSS", minificationError);
            }

            // Add the error info to the end of non-minified resource
            resource += string.Format("\r\n\r\n/* Minification failed (line {0}, error number {1}): {2} */", minificationError.Line, minificationError.Error, minificationError.Message);
        }

        return resource;
    }


    /// <summary>
    /// Parsing error occured - event handler.
    /// </summary>
    private void parser_CssError(object sender, CssErrorEventArgs e)
    {
        // Do not minify in the case of parse exception and log the error
        minificationError = e.Exception;
    }
}


/// <summary>
/// Minifier for JavaScript.
/// </summary>
public class JavaScriptMinifier : IResourceMinifier
{
    /// <summary>
    /// Indicates if parsing failed for specific reason
    /// </summary>
    private JScriptException minificationError = null;


    /// <summary>
    /// Indicates if the parser can recover when an error occurs during minification
    /// </summary>
    private bool canRecover = true;


    private bool? mLogMinifierParseError = null;

    // Indicates if errors from minification should be logged
    private bool LogMinifierParseError
    {
        get
        {
            if (mLogMinifierParseError == null)
            {
                mLogMinifierParseError = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSLogJSMinifierParseError"], false);
            }

            return (bool)mLogMinifierParseError;
        }
    }


    /// <summary>
    /// Returns a minified version of a given JavaScript.
    /// </summary>
    /// <param name="resource">JavaScript to be minified</param>
    /// <returns>Minified JavaScript</returns>
    public string Minify(string resource)
    {
        if (String.IsNullOrEmpty(resource))
        {
            return resource;
        }

        // Reset error
        minificationError = null;
        canRecover = true;

        try
        {
            JSParser parser = new JSParser(resource);
            parser.CompilerError += parser_CompilerError;

            // Parse the resource
            Block scriptBlock = parser.Parse(null);

            // Get minified code if no error occurs or parser was able to recover
            if ((scriptBlock != null) && ((minificationError == null) || ((minificationError != null) && canRecover)))
            {
                resource = scriptBlock.ToCode();
            }
        }
        catch (JScriptException ex)
        {
            minificationError = ex;
            canRecover = false;
        }

        if (minificationError != null)
        {
            if (LogMinifierParseError)
            {
                // Log exception to event log if allowed
                EventLogProvider.LogException("JS Compression", "MINIFYJS", minificationError);
            }

            if (!canRecover)
            {
                // Add error info in front of non-minified resource
                resource += "\r\n\r\n// Minification failed (line " + minificationError.Line.ToString() + "): " + minificationError.Message;
            }
        }

        return resource;
    }


    private void parser_CompilerError(object sender, JScriptExceptionEventArgs e)
    {
        // Store exception and info if the compiler can recover
        minificationError = e.Exception;
        canRecover = e.Exception.CanRecover;
    }
}

#endregion