using System.Web;

using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;

public partial class CMSWebParts_CommunityServices_YouTubeVideo : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    ///  Gets or sets the URL of YouTube video to be displayed.
    /// </summary>
    public string VideoURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("VideoURL"), "");
        }
        set
        {
            SetValue("VideoURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the video width.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 425);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Gets or sets the video height.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), 355);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether video start immediately after webpart load.
    /// </summary>
    public bool AutoPlay
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoPlay"), false);
        }
        set
        {
            SetValue("AutoPlay", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether video should loops after playback stops.
    /// </summary>
    public bool Loop
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Loop"), false);
        }
        set
        {
            SetValue("Loop", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether player should display related videos after playback stops.
    /// </summary>
    public bool Rel
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Rel"), false);
        }
        set
        {
            SetValue("Rel", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether video should support full screen playback.
    /// </summary>
    public bool FullScreen
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FullScreen"), false);
        }
        set
        {
            SetValue("FullScreen", value);
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
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            YouTubeVideoParameters ytParams = new YouTubeVideoParameters();
            ytParams.Url = VideoURL;
            ytParams.FullScreen = FullScreen;
            ytParams.AutoPlay = AutoPlay;
            ytParams.Loop = Loop;
            ytParams.RelatedVideos = Rel;
            ytParams.Width = Width;
            ytParams.Height = Height;

            ltlPlaceholder.Text = MediaHelper.GetYouTubeVideo(ytParams);
        }
    }

    #endregion
}