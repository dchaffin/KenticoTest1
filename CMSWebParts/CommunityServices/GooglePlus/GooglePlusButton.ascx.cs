using System;
using System.Web.UI;

using CMS.DocumentEngine;
using CMS.PortalControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;

public partial class CMSWebParts_CommunityServices_GooglePlus_GooglePlusButton : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Target URL.
    /// </summary>
    public string Url
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Url"), string.Empty);
        }
        set
        {
            SetValue("Url", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 110);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Size.
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
    /// Language.
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
    /// Annotation.
    /// </summary>
    public string Annotation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Annotation"), string.Empty);
        }
        set
        {
            SetValue("Annotation", value);
        }
    }


    /// <summary>
    /// Meta title.
    /// </summary>
    public string MetaTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MetaTitle"), string.Empty);
        }
        set
        {
            SetValue("MetaTitle", value);
        }
    }


    /// <summary>
    /// Meta description.
    /// </summary>
    public string MetaDescription
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MetaDescription"), string.Empty);
        }
        set
        {
            SetValue("MetaDescription", value);
        }
    }


    /// <summary>
    /// Meta image.
    /// </summary>
    public string MetaImage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MetaImage"), string.Empty);
        }
        set
        {
            SetValue("MetaImage", value);
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
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Get page's URL
            string pageUrl = String.Empty;
            if (string.IsNullOrEmpty(Url))
            {
                if (CMSContext.CurrentDocument != null)
                {
                    TreeNode node = CMSContext.CurrentDocument;
                    pageUrl = CMSContext.GetUrl(node.NodeAliasPath, node.DocumentUrlPath, CMSContext.CurrentSiteName);
                }
                else
                {
                    pageUrl = URLHelper.CurrentURL;
                }
            }
            else
            {
                pageUrl = ResolveUrl(Url);
            }
            pageUrl = URLHelper.GetAbsoluteUrl(HTMLHelper.HTMLEncode(pageUrl));

            if (string.IsNullOrEmpty(Language))
            {
                Language = CMSContext.CurrentDocumentCulture.CultureCode;
            }

            // Prepare HTML representation
            string output = "<div style=\"overflow: hidden; width: {0}px;\">";
            if (UseHTML5)
            {
                output += "<div class=\"g-plusone\" data-size=\"{1}\" data-annotation=\"{2}\" data-href=\"{3}\"></div>";
            }
            else
            {
                output += "<g:plusone size=\"{1}\" annotation=\"{2}\" href=\"{3}\"></g:plusone>";
            }
            output += "</div>";

            ltlPluginCode.Text = String.Format(output, Width, Size, Annotation, pageUrl);

            string script = @"
window.___gcfg = {
  lang: '" + Language + @"'
};

(function() {
  var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
  po.src = 'https://apis.google.com/js/plusone.js';
  var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
})();";
    
            // Register Google + render script
            ScriptHelper.RegisterStartupScript(this,typeof(string),"GooglePlusOneButton",script,true);
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


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Generate meta tags
            string metaTags = String.Empty;
            if (!String.IsNullOrEmpty(MetaTitle))
            {
                metaTags += String.Format("<meta property=\"og:title\" content=\"{0}\"/>", HTMLHelper.HTMLEncode(MetaTitle));
            }
            if (!String.IsNullOrEmpty(MetaDescription))
            {
                metaTags += String.Format("<meta property=\"og:description\" content=\"{0}\"/>", HTMLHelper.HTMLEncode(MetaDescription));
            }
            if (!String.IsNullOrEmpty(MetaImage))
            {
                metaTags += String.Format("<meta property=\"og:image\" content=\"{0}\"/>", URLHelper.GetAbsoluteUrl(ResolveUrl(HTMLHelper.HTMLEncode(MetaImage))));
            }

            Page.Header.Controls.Add(new LiteralControl(metaTags));
        }
    }

    #endregion
}



