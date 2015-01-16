using System.Web.UI;
using System.Text;

using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.CMSHelper;

public partial class CMSWebParts_CommunityServices_Facebook_FacebookFacepile : CMSAbstractWebPart
{
    #region "Constants"

    private const int DEFAULT_WIDTH = 200;

    #endregion


    #region "Properties"

    /// <summary>
    /// Facebook page URL
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
    /// Width of the web part in pixels
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), DEFAULT_WIDTH);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Size of the web part
    /// </summary>
    public string Size
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Size"), "");
        }
        set
        {
            SetValue("Size", value);
        }
    }


    /// <summary>
    /// Maximum number of rows with faces
    /// </summary>
    public int RowsNumber
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RowsNumber"), 1);
        }
        set
        {
            SetValue("RowsNumber", value);
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
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            if (UseHTML5)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<div class=\"fb-facepile\" data-href=\"", URLHelper.EncodeQueryString(FBPageUrl),
                          "\" data-size=\"", Size, "\" data-max-rows=\"", RowsNumber, "\" data-width=\"",
                          Width, "\" data-colorscheme=\"", ColorScheme, "\"></div>");

                // Register Facebook javascript SDK
                ScriptHelper.RegisterFacebookJavascriptSDK(Page, CMSContext.PreferredCultureCode);
                ltlLikeBox.Text = sb.ToString();
            }
            else
            {

                // Iframe code
                string query = null;
                string src = "http://www.facebook.com/plugins/facepile.php";

                query = URLHelper.AddUrlParameter(query, "href", URLHelper.EncodeQueryString(FBPageUrl));
                query = URLHelper.AddUrlParameter(query, "size", Size);
                query = URLHelper.AddUrlParameter(query, "width", Width.ToString());
                query = URLHelper.AddUrlParameter(query, "max_rows", RowsNumber.ToString());
                query = URLHelper.AddUrlParameter(query, "colorscheme", ColorScheme);

                src = URLHelper.EncodeQueryString(URLHelper.AppendQuery(src, query));

                ltlLikeBox.Text = "<iframe src=\"" + src + "\"";
                ltlLikeBox.Text += " scrolling=\"no\" frameborder=\"0\" style=\"border:none; overflow:hidden; width:" + Width + "px;\"></iframe>";
            }
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}