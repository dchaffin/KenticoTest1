using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.UIControls;
using CMS.URLRewritingEngine;

public partial class CMSMessages_invalidWebsite : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;
        // Set error state
        Response.StatusCode = 503;

        string mPrefix = "http://";

        titleElem.TitleText = GetString("Message.InvalidWebSite");
        titleElem.TitleImage = GetImageUrl("Others/Messages/denied.png");

        string mDomain = URLHelper.Url.Host;
        if (URLHelper.Url.Port != 80)
        {
            mDomain = mDomain + ":" + URLHelper.Url.Port.ToString();
        }

        if (URLHelper.IsSSL)
        {
            mPrefix = "https://";
        }

        lblMessage.Text = GetString("Message.TextInvalidWebSite") + " ";
        lblMessageUrl.Text = mPrefix + mDomain + HttpUtility.HtmlEncode(URLRewriter.CurrentURL);

        lblInfo1.Text = GetString("Message.InfoInvalidWebSite1") + " ";
        lnkSiteManager.Text = GetString("Message.LinkInvalidWebSite");
        lblInfo2.Text = " " + GetString("Message.InfoInvalidWebSite2") + " ";
        lblInfoDomain.Text = mDomain;

        if (LicenseHelper.CurrentLicenseInfo == null)
        {
            pnlLicense.Visible = true;
        }
    }
}