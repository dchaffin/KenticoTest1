using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.PortalControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.SettingsProvider;

public partial class CMSWebParts_CommunityServices_Twitter_TwitterFollowButton : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// User to follow.
    /// </summary>
    public string UserToFollow
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UserToFollow"), string.Empty);
        }
        set
        {
            SetValue("UserToFollow", value);
        }
    }


    /// <summary>
    /// Language for the button.
    /// </summary>
    public string Language
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Language"), string.Empty);
        }
        set
        {
            SetValue("Language", value);
        }
    }


    /// <summary>
    /// Whether to display followers count or not.
    /// </summary>
    public bool DisplayFollowersCount
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFollowersCount"), false);
        }
        set
        {
            SetValue("DisplayFollowersCount", value);
        }
    }



    /// <summary>
    /// Whether to show screen name of the user to follow.
    /// </summary>
    public bool ShowScreenName
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowScreenName"), false);
        }
        set
        {
            SetValue("ShowScreenName", value);
        }
    }


    /// <summary>
    /// Alignment for the button.
    /// </summary>
    public string Alignment
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Alignment"), string.Empty);
        }
        set
        {
            SetValue("Alignment", value);
        }
    }


    /// <summary>
    /// Size for the button.
    /// </summary>
    public string Size
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Size"), string.Empty);
        }
        set
        {
            SetValue("Size", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 300);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Indicates if HTML 5 output should be generated.
    /// </summary>
    public bool UseHTML5
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseHTML5"), false);
        }
        set
        {
            SetValue("UseHTML5", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            if (string.IsNullOrEmpty(Language))
            {
                Language = CMSContext.CurrentDocumentCulture.CultureCode;
            }

            if (UseHTML5)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<a href=\"https://twitter.com/", UserToFollow, "\" class=\"twitter-follow-button\" data-show-count=\"",
                          DisplayFollowersCount.ToString().ToLowerCSafe(), "\" data-lang=\"", Language, "\" data-width=\"", Width,
                          "px\" data-align=\"", Alignment, "\" data-show-screen-name=\"", ShowScreenName.ToString().ToLowerCSafe(),
                          "\" data-size=\"", (Size.EqualsCSafe("m") ? "medium" : "large"), "\"></a>");

                // Register Twitter widget API
                ltlFollowButtonCode.Text = sb.ToString();
                ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "TwitterWidgetsAPI", "<script>!function(d,s,id){var js,fjs=d.getElementsByTagName(s)[0];if(!d.getElementById(id)){js=d.createElement(s);js.id=id;js.src=\"http://platform.twitter.com/widgets.js\";fjs.parentNode.insertBefore(js,fjs);}}(document,\"script\",\"twitter-wjs\");</script>");
            }
            else
            {
                // Build plugin code
                string query = null;
                string src = "//platform.twitter.com/widgets/follow_button.html";
                int height = (Size.EqualsCSafe("m")) ? 20 : 28;

                // Set webpart parameters
                query = URLHelper.AddUrlParameter(query, "screen_name", UserToFollow);
                query = URLHelper.AddUrlParameter(query, "lang", Language);
                query = URLHelper.AddUrlParameter(query, "show_count", DisplayFollowersCount.ToString());
                query = URLHelper.AddUrlParameter(query, "show_screen_name", ShowScreenName.ToString());
                query = URLHelper.AddUrlParameter(query, "align", Alignment);
                query = URLHelper.AddUrlParameter(query, "size", Size);

                src = URLHelper.EncodeQueryString(URLHelper.AppendQuery(src, query));

                // Output HTML code
                string output = "<iframe src=\"{0}\" class=\"twitter-follow-button\" scrolling=\"no\" frameborder=\"0\" allowtransparency=\"true\" style=\"width: {1}px; height: {2}px;\"></iframe>";
                ltlFollowButtonCode.Text = String.Format(output, src, Width, height);
            }
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}