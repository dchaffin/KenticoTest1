using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;

using CMS.UIControls;
using CMS.SocialNetworking;
using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.EventLog;

public partial class CMSFormControls_Documents_SocialMediaPublishing_FacebookAccessTokenPage : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("socialnetworking.facebook.accesstoken");

        lblStatus.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
        lblStatus.Style.Add(HtmlTextWriterStyle.MarginTop, "230px");

        // Load data from settings
        string currentSite = CMSContext.CurrentSiteName;
        string apiKey = QueryHelper.GetString("apiKey", String.Empty);
        string apiSecret = QueryHelper.GetString("appSecret", String.Empty);
        string pageId = QueryHelper.GetString("pageId", String.Empty);
        string textboxId = QueryHelper.GetString("textbox", String.Empty);
        string lblId = QueryHelper.GetString("lblId", String.Empty);
        string redirectUrl = URLHelper.GetAbsoluteUrl(FacebookProvider.REDIRECT_URL) + "?textbox=" + textboxId + "&apiKey=" + apiKey + "&appSecret=" + apiSecret + "&pageId=" + pageId + "&lblId=" + lblId;
        redirectUrl = HttpContext.Current.Server.UrlEncode(redirectUrl);
        string code = QueryHelper.GetString("code", String.Empty);

        if (!String.IsNullOrEmpty(code))
        {
            // Second request on this site, already with code parameter
            WebClient wc = new WebClient();
            string request = String.Format(FacebookProvider.OAUTH_ACCESS_TOKEN_URL, apiKey, redirectUrl, apiSecret, code);

            try
            {
                string accessToken = wc.DownloadString(request);
                DateTime expiration = new DateTime();
                bool expirationSet = false;

                // Get values from response
                string[] pairs = accessToken.Split('&');

                foreach (var pair in pairs)
                {
                    string[] keyValue = pair.Split('=');

                    if (keyValue[0].CompareTo("access_token") == 0)
                    {
                        accessToken = keyValue[1];
                    }
                    else if (keyValue[0].CompareTo("expires") == 0)
                    {
                        expiration = DateTime.Now + TimeSpan.FromSeconds(Convert.ToInt32(keyValue[1]));
                        expirationSet = true;
                    }
                }

                // Get page access token - posting will be done under page identity
                string pageAccessToken = FacebookProvider.GetPageAccessToken(accessToken, pageId);

                // Check if page access token was retrieved correctly
                if (pageAccessToken == null)
                {
                    lblStatus.Text = GetString("socialnetworking.facebook.pageaccesstokenfail");
                }
                else
                {
                    // Set retrieved access token to appropriate setting box
                    StringBuilder sb = new StringBuilder("if(wopener.setAccessTokenToTextBox){wopener.setAccessTokenToTextBox('")
                            .AppendFormat("{0}', '{1}', '{2}', '{3}', '{4}'); CloseDialog();}}", textboxId, pageAccessToken, expirationSet ? expiration.ToString("g", CultureHelper.PreferredUICultureInfo) : "", lblId, expirationSet ? expiration.ToString() : "");
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "TokenScript", ScriptHelper.GetScript(sb.ToString()));
                }
            }
            catch (WebException ex)
            {
                // Log exception
                EventLogProvider ev = new EventLogProvider();
                ev.LogEvent("FacebookAccessToken", "Retrieval", ex);

                // Show exception message
                lblStatus.Text = GetString("socialnetworking.facebook.connectionfail") + ex.Message;
            }
        }
        else
        {
            // First time on this page - need to ask Facebook for code - will get redirected back as soon as user confirms permissions
            Response.Redirect(String.Format(FacebookProvider.OAUTH_AUTHORIZE_URL, apiKey, redirectUrl), true);
        }
    }
}
