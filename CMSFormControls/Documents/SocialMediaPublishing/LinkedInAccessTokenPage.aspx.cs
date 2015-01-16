using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;

using CMS.UIControls;
using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.EventLog;
using CMS.SocialNetworking;

using LinkedIn;

public partial class CMSFormControls_Documents_SocialMediaPublishing_LinkedInAccessTokenPage : CMSModalPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = ResHelper.GetString("socialnetworking.linkedin.accesstoken");
        CurrentMaster.Title.TitleImage = GetImageUrl("Others/LinkedIn/linkedin.png");

        string txtToken = QueryHelper.GetString("txtToken", String.Empty);
        string consumerKey = QueryHelper.GetString("apiKey", String.Empty);
        string consumerSecret = QueryHelper.GetString("apiSecret", String.Empty);
        string oauthToken = QueryHelper.GetString("oauth_token", String.Empty);
        string error = QueryHelper.GetString("oauth_problem", String.Empty);
        Dictionary<string, string> tokens = null;

        // Check Social networking DLL and settings
        if (!SiteManagerFunctions.CheckSocialNetworkingDLL())
        {
            lblStatus.Text = ResHelper.GetString("socialnetworking.renameddll");
        }
        else if ((String.IsNullOrEmpty(consumerKey) || String.IsNullOrEmpty(consumerSecret)) && String.IsNullOrEmpty(oauthToken) && String.IsNullOrEmpty(error))
        {
            lblStatus.Text = ResHelper.GetString("socialnetworking.linkedin.apisettingsmissing");
        }
        else
        {
            // If access denied
            if (error.EqualsCSafe("user_refused"))
            {
                // Close the window
                StringBuilder script = new StringBuilder("if(wopener.setAccessTokenToTextBox){ CloseDialog(); }");

                ScriptHelper.RegisterStartupScript(Page, typeof(string), "TokenScript", ScriptHelper.GetScript(script.ToString()));
            }
            else
            {
                // Authenticate and retrieve tokens
                tokens = LinkedInProvider.Authorize(txtToken);
                if (tokens.Count != 0)
                {
                    // Return access token values and close the window
                    StringBuilder script = new StringBuilder("if(wopener.setAccessTokenToTextBox){ wopener.setAccessTokenToTextBox('")
                        .AppendFormat("{0}', '{1}', '{2}'); CloseDialog(); }}", txtToken, tokens["AccessToken"], tokens["AccessTokenSecret"]);
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "TokenScript", ScriptHelper.GetScript(script.ToString()));
                }
                else
                {
                    // Error occured while communicating with LinkedIn
                    lblStatus.Text = ResHelper.GetString("socialnetworking.authorizationerror");
                }
            }
        }
    }

    #endregion
}
