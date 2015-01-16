using System;
using System.Collections.Generic;
using System.Web;

using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.WebAnalytics;

/// <summary>
/// Web analytics methods - wrapping methods for macro resolver.
/// </summary>
public static class WebAnalyticsMethods
{
    /// <summary>
    /// Registers all web analytics methods to macro resolver.
    /// </summary>
    public static void RegisterMethods()
    {
        MacroMethod isReturningVisitor = new MacroMethod("IsReturningVisitor", IsReturningVisitor)
        {
            Comment = "Returns true if current visitor is returning.",
            Type = typeof(bool),
            MinimumParameters = 0,
            AllowedTypes = new List<Type>() { typeof(VisitorNamespace) }
        };
        MacroMethods.RegisterMethod(isReturningVisitor);

        MacroMethod isFirstTimeVisitor = new MacroMethod("IsFirstTimeVisitor", IsFirstTimeVisitor)
        {
            Comment = "Returns true if current visitor comes to the website for the first time.",
            Type = typeof(bool),
            MinimumParameters = 0,
            AllowedTypes = new List<Type>() { typeof(VisitorNamespace) }
        };
        MacroMethods.RegisterMethod(isFirstTimeVisitor);

        MacroMethod getSearchEngineKeyword = new MacroMethod("GetSearchEngineKeyword", GetSearchEngineKeyword)
        {
            Comment = "Returns search keywords from search engine visitor came from.",
            Type = typeof(string),
            MinimumParameters = 0,
            AllowedTypes = new List<Type>() { typeof(VisitorNamespace) }
        };
        MacroMethods.RegisterMethod(getSearchEngineKeyword);

        MacroMethod getSearchEngine = new MacroMethod("GetSearchEngine", GetSearchEngine)
        {
            Comment = "Returns search engine visitor came from.",
            Type = typeof(string),
            MinimumParameters = 0,
            AllowedTypes = new List<Type>() { typeof(VisitorNamespace) }
        };
        MacroMethods.RegisterMethod(getSearchEngine);

        MacroMethod getUrlReferrer = new MacroMethod("GetUrlReferrer", GetUrlReferrer)
        {
            Comment = "Returns absolute URI of the URLRefferer from current HTTP context.",
            Type = typeof(string),
            MinimumParameters = 0,
            AllowedTypes = new List<Type>() { typeof(VisitorNamespace) }
        };
        MacroMethods.RegisterMethod(getUrlReferrer);

        MacroMethod getUrlReferrerParameter = new MacroMethod("GetUrlReferrerParameter", GetUrlReferrerParameter)
        {
            Comment = "Returns value of specified URLReferrer query string parameter.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(VisitorNamespace) }
        };
        getUrlReferrerParameter.AddParameter("parameterName", typeof(string), "Query string parameter name.");
        MacroMethods.RegisterMethod(getUrlReferrerParameter);

        MacroMethod getCurrentDistance = new MacroMethod("GetCurrentDistance", GetCurrentDistance)
        {
            Comment = "Returns current distance (in kilometers) from specified location (based on Geo IP).",
            Type = typeof(double),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(VisitorNamespace) }
        };
        getCurrentDistance.AddParameter("latitude", typeof(double), "Latitude of the place.");
        getCurrentDistance.AddParameter("longitude", typeof(double), "Longitude of the place.");
        MacroMethods.RegisterMethod(getCurrentDistance);
    }


    /// <summary>
    /// Returns current distance (in kilometers) from specified location (based on Geo IP).
    /// </summary>
    /// <param name="parameters">
    /// Latitude of the place;
    /// Longitude of the place;
    /// </param>
    public static object GetCurrentDistance(params object[] parameters)
    {
        if (parameters.Length == 2)
        {
            double lat = ValidationHelper.GetDouble(parameters[0], 0, CultureHelper.EnglishCulture.Name);
            double lon = ValidationHelper.GetDouble(parameters[1], 0, CultureHelper.EnglishCulture.Name);

            return GeoIPHelper.GetCurrentGeoLocation().Distance(lat, lon);
        }

        throw new NotSupportedException();
    }


    /// <summary>
    /// Returns true if current visitor is returning.
    /// </summary>
    /// <param name="parameters">Parameters</param>
    public static object IsReturningVisitor(params object[] parameters)
    {
        return WebAnalyticsFunctions.IsReturningVisitor();
    }


    /// <summary>
    /// Returns true if current visitor comes to the website for the first time.
    /// </summary>
    /// <param name="parameters">Parameters</param>
    public static object IsFirstTimeVisitor(params object[] parameters)
    {
        return WebAnalyticsFunctions.IsFirstTimeVisitor();
    }


    /// <summary>
    /// Returns search keywords from search engine visitor came from.
    /// </summary>
    /// <param name="parameters">Parameters</param>
    public static object GetSearchEngineKeyword(params object[] parameters)
    {
        return WebAnalyticsFunctions.GetSearchEngineKeyword();
    }


    /// <summary>
    /// Returns search engine visitor came from.
    /// </summary>
    /// <param name="parameters">Parameters</param>
    public static object GetSearchEngine(params object[] parameters)
    {
        return WebAnalyticsFunctions.GetSearchEngine();
    }


    /// <summary>
    /// Returns absolute URI of the URLRefferer from current HTTP context.
    /// </summary>
    /// <param name="parameters">Parameters</param>
    public static object GetUrlReferrer(params object[] parameters)
    {
        return WebAnalyticsFunctions.GetUrlReferrer();
    }


    /// <summary>
    /// Returns value of specified URLReferrer query string parameter.
    /// </summary>
    /// <param name="parameters">Query string parameter name</param>
    public static object GetUrlReferrerParameter(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return WebAnalyticsFunctions.GetUrlReferrerParameter(ValidationHelper.GetString(parameters[0], ""));

            default:
                throw new NotSupportedException();
        }
    }
}