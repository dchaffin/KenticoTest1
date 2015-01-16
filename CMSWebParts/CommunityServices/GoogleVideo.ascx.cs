using System.Web;

using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;

public partial class CMSWebParts_CommunityServices_GoogleVideo : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether the video is automatically activated.
    /// </summary>
    public bool AutoActivation
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoActivation"), false);
        }
        set
        {
            SetValue("AutoActivation", value);
        }
    }


    /// <summary>
    /// Gets or sets the URL of Google video to be displayed.
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
            // video url
            string url = null;

            // query string autoplay value
            string autoPlay = null;

            string videoUrl = HTMLHelper.HTMLEncode(VideoURL);

            // Index of query string parameter value 'docid' 
            int pos = videoUrl.LastIndexOfCSafe("?docid=");

            // Set real video url in accordance to VideoUrl property value
            if (pos != -1)
            {
                url = "http://video.google.com/googleplayer.swf" + videoUrl.Substring(pos) + "&amp;hl=en";
            }
            else
            {
                url = videoUrl;
            }

            // Ensure autoplay
            if (AutoPlay)
            {
                autoPlay = "&amp;autoPlay=true";
            }

            if (AutoActivation)
            {
                ltlPlaceholder.Text = "<div class=\"VideoLikeContent\" id=\"GVPlaceholder_" + ltlScript.ClientID + "\" ></div>";

                // Get external script function
                ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/CommunityServices/GoogleVideo_files/video.js");

                // Call function for video object insertion                
                ltlScript.Text = BuildScriptBlock(url);
            }
            else
            {
                //<embed style="width:400px; height:326px;" id="VideoPlayback" type="application/x-shockwave-flash" src="http://video.google.com/googleplayer.swf?docId=-8584609541409912644&hl=en" flashvars=""> </embed>
                ltlPlaceholder.Text = "<div class=\"VideoLikeContent\" ><object type=\"application/x-shockwave-flash\" data=\"" + url + "\" width=\"" + Width + "\" height=\"" + Height + "\" id=\"VideoPlayback\">" +
                                      "<param name=\"classid\" value=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" />\n" +
                                      "<param name=\"codebase\" value=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0\" />\n" +
                                      "<param name=\"movie\" value=\"" + url + "\" />" +
                                      "<param name=\"allowScriptAcess\" value=\"sameDomain\" />" +
                                      "<param name=\"quality\" value=\"best\" />" +
                                      "<param name=\"scale\" value=\"noScale\" />" +
                                      "<param name=\"pluginurl\" value=\"http://www.adobe.com/go/getflashplayer\" />\n" +
                                      "<param name=\"salign\" value=\"TL\" />" +
                                      "<param name=\"FlashVars\" value=\"playerMode=embedded" + autoPlay + "\" />" +
                                      "<param name=\"wmode\" value=\"transparent\" />\n" +
                                      GetString("Flash.NotSupported") + "\n" +
                                      "</object></div>";
            }
        }
    }


    /// <summary>
    /// Creates a script block which loads a Google video at runtime.
    /// </summary>
    /// <param name="url">URL to the video</param>
    /// <returns>Script block that will load a Google video</returns>
    private string BuildScriptBlock(string url)
    {
        string scriptBlock = string.Format("LoadGVideo('GVPlaceholder_{0}', '{1}', {2}, {3}, {4});",
                                           ltlScript.ClientID,
                                           url,
                                           Width,
                                           Height,
                                           AutoPlay.ToString().ToLowerCSafe());

        return ScriptHelper.GetScript(scriptBlock);
    }

    #endregion
}