using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.PortalControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSWebParts_CommunityServices_Twitter_TwitterFeed : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Username.
    /// </summary>
    public string Username
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Username"), string.Empty);
        }
        set
        {
            SetValue("Username", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 250);
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
    /// Number of tweets to display.
    /// </summary>
    public int NumberOfTweets
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("NumberOfTweets"), 4);
        }
        set
        {
            SetValue("NumberOfTweets", value);
        }
    }


    /// <summary>
    /// Whether to poll for new tweets.
    /// </summary>
    public bool PollActive
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("PollActive"), false);
        }
        set
        {
            SetValue("PollActive", value);
        }
    }


    /// <summary>
    /// Whether to show scrollbar.
    /// </summary>
    public bool Scrollbar
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Scrollbar"), false);
        }
        set
        {
            SetValue("Scrollbar", value);
        }
    }


    /// <summary>
    /// Shell background color.
    /// </summary>
    public string ShellBackgroundColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ShellBackgroundColor"), String.Empty);
        }
        set
        {
            SetValue("ShellBackgroundColor", value);
        }
    }


    /// <summary>
    /// Shell text color.
    /// </summary>
    public string ShellTextColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ShellTextColor"), String.Empty);
        }
        set
        {
            SetValue("ShellTextColor", value);
        }
    }


    /// <summary>
    /// Tweet background color.
    /// </summary>
    public string TweetBackgroundColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TweetBackgroundColor"), String.Empty);
        }
        set
        {
            SetValue("TweetBackgroundColor", value);
        }
    }


    /// <summary>
    /// Tweet text color.
    /// </summary>
    public string TweetTextColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TweetTextColor"), String.Empty);
        }
        set
        {
            SetValue("TweetTextColor", value);
        }
    }


    /// <summary>
    /// Link color.
    /// </summary>
    public string LinkColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkColor"), String.Empty);
        }
        set
        {
            SetValue("LinkColor", value);
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
            // Build plugin code
            StringBuilder sb = new StringBuilder();
            sb.Append("<script src=\"http://widgets.twimg.com/j/2/widget.js\"></script><script>new TWTR.Widget({version: 2,type: 'profile',rpp: ");
            sb.Append(NumberOfTweets);
            sb.Append(",interval: 30000,width: ");
            sb.Append(Width);
            sb.Append(",height: ");
            sb.Append(Height);
            sb.Append(",theme: {shell: {background: '");
            sb.Append((String.IsNullOrEmpty(ShellBackgroundColor)) ? "#333333" : ShellBackgroundColor);
            sb.Append("',color: '");
            sb.Append((String.IsNullOrEmpty(ShellTextColor)) ? "#ffffff" : ShellTextColor);
            sb.Append("'},tweets: {background: '");
            sb.Append((String.IsNullOrEmpty(TweetBackgroundColor)) ? "#000000" : TweetBackgroundColor);
            sb.Append("',color: '");
            sb.Append((String.IsNullOrEmpty(TweetTextColor)) ? "#ffffff" : TweetTextColor);
            sb.Append("',links: '");
            sb.Append((String.IsNullOrEmpty(LinkColor)) ? "#4aed05" : LinkColor);
            sb.Append("'}},features: {scrollbar: ");
            sb.Append(Scrollbar.ToString().ToLowerCSafe());
            sb.Append(",loop: false,live: ");
            sb.Append(PollActive.ToString().ToLowerCSafe());
            sb.Append(",behavior: 'all'}}).render().setUser('");
            sb.Append(Username);
            sb.Append("').start();</script>");

            // Set plugin code
            ltlPluginCode.Text = sb.ToString();
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