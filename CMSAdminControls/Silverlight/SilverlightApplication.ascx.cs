using System;
using System.Text;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_Silverlight_SilverlightApplication : CMSUserControl
{
    #region "Variables"

    private string mAlternateContent = null;
    private string mApplicationPath = "";
    private string mMinimumVersion = "";
    private string mParameters = "";
    private string mContainerWidth = "";
    private string mContainerHeight = "";
    private string mContainerBackground = "#00FFFFFF";
    private bool mIsWindowLess = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// HTML content which is displayed to user when Silverlight plugin is not installed.
    /// </summary>
    public string AlternateContent
    {
        get
        {
            // Try to load custom alternate content
            if (string.IsNullOrEmpty(mAlternateContent))
            {
                mAlternateContent = "var altHtml = \"<a href='{1}' style='text-decoration: none;'><img src='{2}' alt='{3}' style='border-style: none'/></a>\";" +
                                    "altHtml = altHtml.replace('{1}', 'http://go.microsoft.com/fwlink/?LinkID=124807');" +
                                    "altHtml = altHtml.replace('{2}', 'http://go.microsoft.com/fwlink/?LinkId=108181');" +
                                    "altHtml = altHtml.replace('{3}', 'Get Microsoft Silverlight');";
            }
            else
            {
                mAlternateContent = "var altHtml = " + ScriptHelper.GetString(mAlternateContent) + ";";
            }
            return mAlternateContent;
        }
        set
        {
            mAlternateContent = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating whether the silverlight application should be windowless or not. Default is <c>false</c>.
    /// </summary>
    public bool IsWindowless
    {
        get
        {
            return mIsWindowLess;
        }
        set
        {
            mIsWindowLess = value;
        }
    }


    /// <summary>
    /// Silverlight application path.
    /// </summary>
    public string ApplicationPath
    {
        get
        {
            return mApplicationPath;
        }
        set
        {
            mApplicationPath = value;
        }
    }


    /// <summary>
    /// Minimum version of the Microsoft Silverlight which is required by the current silverlight application.
    /// </summary>
    public string MinimumVersion
    {
        get
        {
            return mMinimumVersion;
        }
        set
        {
            mMinimumVersion = value;
        }
    }


    /// <summary>
    /// Silverlight application parameters.
    /// </summary>
    public string Parameters
    {
        get
        {
            return mParameters;
        }
        set
        {
            mParameters = value;
        }
    }


    /// <summary>
    /// Silverlight application container width.
    /// </summary>
    public string ContainerWidth
    {
        get
        {
            return mContainerWidth;
        }
        set
        {
            mContainerWidth = value;
        }
    }


    /// <summary>
    /// Silverlight application container height.
    /// </summary>
    public string ContainerHeight
    {
        get
        {
            return mContainerHeight;
        }
        set
        {
            mContainerHeight = value;
        }
    }


    /// <summary>
    /// Silverlight application container background.
    /// </summary>
    public string ContainerBackground
    {
        get
        {
            return mContainerBackground;
        }
        set
        {
            mContainerBackground = value;
        }
    }

    #endregion


    #region "Page Events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            ReloadData();
        }
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Initializes silverlight application container.
    /// </summary>
    private void ReloadData()
    {
        // Register silverlight support script
        ScriptHelper.RegisterSilverlight(Page);

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(@"<div id=""Silverlight_{0}""></div>", ClientID);

        // Create script initiating silverlight application
        string slApp = string.Format(@"function createSilverlight(controlHost)
        {{
            Silverlight.createObjectEx(
            {{
                source: {0},
                parentElement: controlHost,
                id: 'silverlightControl',
                properties: 
                {{
                    autoUpgrade: 'true',
                    width: '{1}',
                    alt: altHtml,
                    height: '{2}',
                    version: '{3}',
                    background: '{4}',
                    initParams: '{5}',
                    isWindowless: 'true',
                    enableHtmlAccess: 'true',
                    IsWindowless: '{7}'
                }},
                events: {{}}
            }});
        }}
        var hostElement = document.getElementById('Silverlight_{6}');
        createSilverlight(hostElement);",
                                     ScriptHelper.GetString(URLHelper.ResolveUrl(ApplicationPath)),
                                     ContainerWidth,
                                     ContainerHeight,
                                     MinimumVersion,
                                     ContainerBackground,
                                     Parameters,
                                     ClientID,
                                     IsWindowless);
        sb.Append(ScriptHelper.GetScript(AlternateContent + slApp));
        ltlSilverlight.Text = sb.ToString();
    }

    #endregion
}