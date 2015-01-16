using System.Web.UI;
using System.Text;

using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.CMSHelper;

public partial class CMSWebParts_CommunityServices_Facebook_FacebookLikeBox : CMSAbstractWebPart
{
    #region "Constants"

    private const int heightDefault = 63;
    private const int heightStream = 392;
    private const int heightStreamFaces = 555;
    private const int heightFaces = 255;

    #endregion


    #region "Properties"

    /// <summary>
    /// Facebook page URL.
    /// </summary>
    public string FBPageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FBPageUrl"), "");
        }
        set
        {
            SetValue("FBPageUrl", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 292);
        }
        set
        {
            SetValue("Width", value);
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
    /// Indicates whether to show profile photos or not.
    /// </summary>
    public bool ShowFaces
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFaces"), true);
        }
        set
        {
            SetValue("ShowFaces", value);
        }
    }


    /// <summary>
    /// Indicates whether to display a stream of the latest posts.
    /// </summary>
    public bool ShowStream
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowStream"), true);
        }
        set
        {
            SetValue("ShowStream", value);
        }
    }


    /// <summary>
    /// Indicates whether to show Facebook header or not.
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

            // Default height if nothing additional is shown
            int height = heightDefault;

            // If faces and stream are shown
            if (ShowFaces && ShowStream)
            {
                height = heightStreamFaces;
            }
            // If only stream is shown
            else if (ShowStream)
            {
                height = heightStream;
            }
            // If only faces are shown
            else if (ShowFaces)
            {
                height = heightFaces;
            }

            // If stream or faces are shown and header is too
            if (ShowHeader && (ShowFaces || ShowStream))
            {
                height = height + 35;
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
                sb.Append("<div class=\"fb-like-box\" data-href=\"", URLHelper.EncodeQueryString(FBPageUrl),
                          "\" data-width=\"", Width, "\" data-height=\"", height, "\" data-header=\"",
                          ShowHeader, "\" data-stream=\"", ShowStream, "\" data-show-faces=\"",
                          ShowFaces, "\" data-colorscheme=\"", ColorScheme, "\"");
                if (!string.IsNullOrEmpty(borderColor))
                {
                    sb.Append(" data-border-color=\"", borderColor, "\"");
                }
                sb.Append("></div>");

                // Register Facebook javascript SDK
                ScriptHelper.RegisterFacebookJavascriptSDK(Page, CMSContext.PreferredCultureCode);
                ltlLikeBox.Text = sb.ToString();
            }
            else
            {
                // Iframe code
                string query = null;
                string src = "http://www.facebook.com/plugins/likebox.php";

                if (!string.IsNullOrEmpty(borderColor))
                {
                    query = URLHelper.AddUrlParameter(query, "border_color", borderColor);
                }

                query = URLHelper.AddUrlParameter(query, "href", FBPageUrl);
                query = URLHelper.AddUrlParameter(query, "header", ShowHeader.ToString());
                query = URLHelper.AddUrlParameter(query, "width", Width.ToString());
                query = URLHelper.AddUrlParameter(query, "show_faces", ShowFaces.ToString());
                query = URLHelper.AddUrlParameter(query, "stream", ShowStream.ToString());
                query = URLHelper.AddUrlParameter(query, "colorscheme", ColorScheme);
                query = URLHelper.AddUrlParameter(query, "height", height.ToString());

                src = URLHelper.EncodeQueryString(URLHelper.AppendQuery(src, query));

                ltlLikeBox.Text = "<iframe src=\"" + src + "\"";
                ltlLikeBox.Text += " scrolling=\"no\" frameborder=\"0\" style=\"border:none; overflow:hidden; width:" + Width + "px; height:" + height + "px;\"></iframe>";
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