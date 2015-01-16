using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSMessages_SiteOffline : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;
        // Set service unavailable state
        Response.StatusCode = 503;

        // Set title
        titleElem.TitleText = GetString("Error.SiteOffline");
        titleElem.TitleImage = GetImageUrl("Others/Messages/warning.png");

        SiteInfo currentSite = CMSContext.CurrentSite;
        if (currentSite != null)
        {
            if (currentSite.SiteIsOffline)
            {
                // Site is offline
                if (!String.IsNullOrEmpty(currentSite.SiteOfflineMessage))
                {
                    lblInfo.Text = CMSContext.ResolveMacros(currentSite.SiteOfflineMessage);
                }
                else
                {
                    lblInfo.Text = ResHelper.GetString("error.siteisoffline");
                }
            }
            else
            {
                // Redirect to the root
                Response.Redirect("~/");
            }
        }
    }
}