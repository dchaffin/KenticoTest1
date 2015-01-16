using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.SocialNetworking;
using CMS.SettingsProvider;
using CMS.EventLog;

public partial class CMSFormControls_Documents_SocialMediaPublishing_GooglePlusAccessTokenPage : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = ResHelper.GetString("socialnetworking.googleplus.accesstoken");
        CurrentMaster.Title.TitleImage = GetImageUrl("Others/GooglePlus/googleplus.png");

        string accessToken = null, refreshToken = null;
        Dictionary<string, string> tokens = null;
        string clientID = Request.QueryString["token_client_id"];
        string clientSecret = Request.QueryString["token_client_secret"];
        string txtToken = null;
        string txtTokenSecret = null;
        string error = Request.QueryString["error"];
        string state = Request.QueryString["state"];

        // Check Social networking DLL and settings
        if (!SiteManagerFunctions.CheckSocialNetworkingDLL())
        {
            lblStatus.Text = ResHelper.GetString("socialnetworking.renameddll");
        }
        else if (String.IsNullOrEmpty(state) && (String.IsNullOrEmpty(clientID) || String.IsNullOrEmpty(clientSecret)))
        {
            lblStatus.Text = ResHelper.GetString("socialnetworking.googleplus.apisettingsmissing");
        }
        else
        {
            // If access denied
            if (error.EqualsCSafe("access_denied"))
            {
                // Close the window
                StringBuilder script = new StringBuilder("if(wopener.setAccessTokenToTextBox){CloseDialog();}");

                ScriptHelper.RegisterStartupScript(Page, typeof(string), "TokenScript", ScriptHelper.GetScript(script.ToString()));
            }
            else
            {
                // If this is OAuth callback -> get clientID and clientSecret from state
                if (state != null)
                {
                    foreach (string s in state.Split(new string[1] { "&" }, StringSplitOptions.None))
                    {
                        if (s.StartsWithCSafe("token_client_id="))
                        {
                            clientID = s.Substring(16);
                        }

                        if (s.StartsWithCSafe("token_client_secret="))
                        {
                            clientSecret = s.Substring(20);
                        }
                    }
                }

                // Authenticate and retrieve tokens
                tokens = GooglePlusProvider.Authorize(clientID, clientSecret);
                if (tokens.Count > 0)
                {
                    accessToken = tokens["AccessToken"];
                    refreshToken = tokens["RefreshToken"];

                    if (!String.IsNullOrEmpty(accessToken))
                    {
                        // Extract txtToken values from state
                        txtToken = null;
                        txtTokenSecret = null;
                        string[] stateParams = state.Split(new string[1] { "&" }, StringSplitOptions.None);
                        foreach (string s in stateParams)
                        {
                            if (s.StartsWithCSafe("txtToken="))
                            {
                                txtToken = s.Substring(9);
                            }

                            if (s.StartsWithCSafe("txtTokenSecret="))
                            {
                                txtTokenSecret = s.Substring(15);
                            }
                        }

                        // Return access token values and close the window
                        StringBuilder script = new StringBuilder("if(wopener.setAccessTokenToTextBox){wopener.setAccessTokenToTextBox('");
                        script.Append(txtToken);
                        script.Append("', '");
                        script.Append(accessToken);
                        script.Append("', '");
                        script.Append(txtTokenSecret);
                        script.Append("', '");
                        script.Append(refreshToken);
                        script.Append("');");
                        script.Append("CloseDialog();}");
                        ScriptHelper.RegisterStartupScript(Page, typeof(string), "TokenScript", ScriptHelper.GetScript(script.ToString()));
                    }
                }
                else
                {
                    // There was an error somewhere
                    lblStatus.Text = ResHelper.GetString("socialnetworking.authorizationerror");
                }
            }
        }
    }
}