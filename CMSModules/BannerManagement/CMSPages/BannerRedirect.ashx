<%@ WebHandler Language="C#" Class="BannerRedirect" %>

using System;
using System.Web;

using CMS.GlobalHelper;
using CMS.WebAnalytics;
using CMS.CMSHelper;
using CMS.SiteProvider;
using CMS.PortalEngine;

public class BannerRedirect : IHttpHandler
{
    #region "Public methods"

    /// <summary>
    /// Redirects user to the address specified in query parameter "redirectURL" and logs
    /// banner click for banner with ID specified in query parameter "bannerID".
    /// </summary>
    /// <param name="context"></param>
    public void ProcessRequest(HttpContext context)
    {
        int bannerID = ValidationHelper.GetInteger(context.Request.QueryString["bannerID"], 0);
        if (bannerID == 0)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Wrong id");

            return;
        }
        string redirectURL = URLHelper.ResolveUrl(context.Request.QueryString["redirectURL"]);

        
        // Count click only on live site
        if (CMSContext.ViewMode == ViewModeEnum.LiveSite)
        {
            string currentSiteName = CMSContext.CurrentSiteName;
            
            if (AnalyticsHelper.AnalyticsEnabled(currentSiteName))
            {
                HitLogProvider.LogHit("bannerclick", currentSiteName, null, null, bannerID);
            }

            BannerInfoProvider.DecrementClicksLeft(bannerID);
        }
        
        
        context.Response.Redirect(redirectURL, true);
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// This handler can be reused.
    /// </summary>
    public bool IsReusable
    {
        get { return true; }
    }

    #endregion
}