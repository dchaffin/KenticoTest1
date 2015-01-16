using System.Web.UI;
using System.Text;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;

public partial class CMSWebParts_CommunityServices_Facebook_FacebookRecommendations : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// The domain to show recommandations for.
    /// </summary>
    public string Domain
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Domain"), "");
        }
        set
        {
            SetValue("Domain", value);
        }
    }


    /// <summary>
    /// Reference parameter.
    /// </summary>
    public string RefParameter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RefParameter"), "");
        }
        set
        {
            SetValue("RefParameter", value);
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
    /// Height of the web part in pixels.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), 300);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Indicates whether to show facebook header or not.
    /// </summary>
    public bool ShowHeader
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowHeader"), true);
        }
        set
        {
            SetValue("ShowHeader", value);
        }
    }


    /// <summary>
    /// Color scheme of the web part.
    /// </summary>
    public string ColorScheme
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ColorScheme"), "");
        }
        set
        {
            SetValue("ColorScheme", value);
        }
    }


    /// <summary>
    /// Font of the web part.
    /// </summary>
    public string Font
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Font"), "");
        }
        set
        {
            SetValue("Font", value);
        }
    }


    /// <summary>
    /// Border color of the web part.
    /// </summary>
    public string BorderColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BorderColor"), "");
        }
        set
        {
            SetValue("BorderColor", value);
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
            if (string.IsNullOrEmpty(Domain))
            {
                Domain = CMSContext.CurrentSite.DomainName;
            }

            string borderColor = null;
            if (!string.IsNullOrEmpty(BorderColor))
            {
                borderColor = BorderColor;

                // Replace # if it is present
                if (!UseHTML5)
                {
                    borderColor = borderColor.Replace("#", "%23");
                }
            }

            if (UseHTML5)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<div class=\"fb-recommendations\" data-width=\"", Width, "\" data-height=\"", Height,"\" data-site=\"",
                           Domain, "\" data-header=\"", ShowHeader, "\" data-colorscheme=\"", ColorScheme, "\"");

                if (!string.IsNullOrEmpty(Font))
                {
                    sb.Append(" data-font=\"", Font, "\"");
                }
                if (!string.IsNullOrEmpty(borderColor))
                {
                    sb.Append(" data-border-color=\"", borderColor, "\"");
                }
                if (!string.IsNullOrEmpty(RefParameter))
                {
                    sb.Append(" data-ref=\"", RefParameter, "\"");
                }
                sb.Append("></div>");

                // Register Facebook script SDK
                ScriptHelper.RegisterFacebookJavascriptSDK(Page, CMSContext.PreferredCultureCode);
                ltlRecommendations.Text = sb.ToString();
            }
            else
            {
                // Iframe code
                string query = null;
                string src = "http://www.facebook.com/plugins/recommendations.php";

                if (!string.IsNullOrEmpty(borderColor))
                {
                    query = URLHelper.AddUrlParameter(query, "border_color", borderColor);
                }

                if (!string.IsNullOrEmpty(Font))
                {
                    query = URLHelper.AddUrlParameter(query, "font", Font);
                }

                if (!string.IsNullOrEmpty(RefParameter))
                {
                    query = URLHelper.AddUrlParameter(query, "ref", RefParameter);
                }

                query = URLHelper.AddUrlParameter(query, "site", Domain);
                query = URLHelper.AddUrlParameter(query, "header", ShowHeader.ToString());
                query = URLHelper.AddUrlParameter(query, "width", Width.ToString());
                query = URLHelper.AddUrlParameter(query, "colorscheme", ColorScheme);
                query = URLHelper.AddUrlParameter(query, "height", Height.ToString());

                src = URLHelper.EncodeQueryString(URLHelper.AppendQuery(src, query));

                ltlRecommendations.Text = "<iframe src=\"" + src + "\"";
                ltlRecommendations.Text += " scrolling=\"no\" frameborder=\"0\" style=\"border:none; overflow:hidden; width:" + Width + "px; height:" + Height + "px;\"></iframe>";
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