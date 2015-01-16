using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.CMSHelper;

public partial class CMSAdminControls_UI_PageElements_PageTitle : PageTitle
{
    #region "Variables"

    private bool mGenerateFullWidth = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether table should contain inline style with width:100%.
    /// </summary>
    public bool GenerateFullWidth
    {
        get
        {
            return mGenerateFullWidth;
        }
        set
        {
            mGenerateFullWidth = value;
        }
    }


    /// <summary>
    /// Title CSS class.
    /// </summary>
    public override string TitleCssClass
    {
        get
        {
            return pnlTitle.CssClass;
        }
        set
        {
            pnlTitle.CssClass = value;
        }
    }


    /// <summary>
    /// Help topic name.
    /// </summary>
    public override string HelpTopicName
    {
        get
        {
            return helpElem.TopicName;
        }
        set
        {
            helpElem.TopicName = value;
            helpBreadcrumbs.TopicName = value;
        }
    }


    /// <summary>
    /// Help name.
    /// </summary>
    public override string HelpName
    {
        get
        {
            return helpElem.HelpName;
        }
        set
        {
            helpElem.HelpName = value;
            helpBreadcrumbs.HelpName = value;
        }
    }


    /// <summary>
    /// Help icon URL for title.
    /// </summary>
    public override string HelpIconUrl
    {
        get
        {
            return helpElem.IconUrl;
        }
        set
        {
            helpElem.IconUrl = value;
        }
    }


    /// <summary>
    /// Placeholder after image and title text.
    /// </summary>
    public override PlaceHolder RightPlaceHolder
    {
        get
        {
            return plcMisc;
        }
        set
        {
            plcMisc = value;
        }
    }


    #region "Dialog properties"

    /// <summary>
    /// Indicates if the control should use the string from resource file.
    /// </summary>
    public bool UseFileStrings
    {
        get;
        set;
    }

    #endregion

    #endregion


    #region "Page events"

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Register jQuery
        ScriptHelper.RegisterJQuery(Page);

        EnsureDraggable();

        string titlePart = null;

        // Create the breadcrumbs
        if (Breadcrumbs != null)
        {
            pnlBreadCrumbs.Visible = true;

            // Generate the breadcrumbs controls
            int breadCrumbsLength = Breadcrumbs.GetUpperBound(0);
            for (int i = 0; i <= breadCrumbsLength; i++)
            {
                CreateBreadCrumbsItem(i, 0);
            }

            if (breadCrumbsLength >= 0)
            {
                titlePart = Breadcrumbs[breadCrumbsLength, 0];
            }
        }

        // Set the title text if set
        if (!string.IsNullOrEmpty(TitleText))
        {
            pnlTitle.Visible = true;
            lblTitle.Text = TitleText;

            titlePart = TitleText;
        }
        else
        {
            pnlTitle.Visible = false;
            helpBreadcrumbs.Visible = true;
        }

        // Set the title info if set
        if (!string.IsNullOrEmpty(TitleInformation))
        {
            lblTitleInfo.Text = TitleInformation;
            lblTitleInfo.Visible = true;
        }

        // Always render alt attribute
        if (!string.IsNullOrEmpty(AlternateText))
        {
            imgTitle.AlternateText = AlternateText;
        }
        else
        {
            imgTitle.Attributes.Add("alt", string.Empty);
        }

        if (!string.IsNullOrEmpty(TitleImage))
        {
            imgTitle.Visible = true;
            imgTitle.ImageUrl = TitleImage;
        }
        else
        {
            imgTitle.Visible = false;
        }

        // Register scripts only when needed
        if (pnlTitle.Visible)
        {
            EnsureFullScreenButton();

            EnsureCloseButton();
        }

        // Set the window title accordingly
        if (SetWindowTitle && (titlePart != null))
        {
            ScriptHelper.RegisterTitleScript(Page, ResHelper.LocalizeString(titlePart));
        }

        // Add style with width:100% if it is required
        if (GenerateFullWidth)
        {
            titleTable.Style.Add("width", "100%");
            titleCell.Style.Add("width", "100%");
        }

        if (!Wrap)
        {
            titleCell.Style.Add("white-space", "nowrap");
        }
    }

    #endregion


    #region "Methods"

    private void EnsureDraggable()
    {
        // Load draggable iframe script
        ScriptHelper.RegisterScriptFile(Page, "DragAndDrop/dragiframe.js");

        // Initialize draggable behavior
        ScriptHelper.RegisterStartupScript(this, typeof(string), "draggableFrame", ScriptHelper.GetScript(
@"$j(window).load(function(){
    if(window.wopener && top.visiblePopup)
    {
        var topFrame = GetTop();
        if(top.isTitleWindow(topFrame, window))
        {
            addHandle(document.getElementById('" + pnlBody.ClientID + @"'), window);
        }
    }
});"));
    }


    private void EnsureFullScreenButton()
    {
        if (ShowFullScreenButton)
        {
            pnlFullScreen.Attributes.Add("onclick", "return fs_" + ClientID + "($j(this));");
            pnlFullScreen.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
            pnlFullScreen.Style.Add(HtmlTextWriterStyle.Display, "none");
            string fsToolTip = GetString("general.fullscreen");
            pnlFullScreen.ToolTip = fsToolTip;

            string fullScreenScript = @"

function titleFullScreen(sender)
{
    if(top.toggleFullScreen)
    {
        top.toggleFullScreen();
        toggleFullScreen(sender);
    }
    return false;
}

function toggleFullScreen(sender)
{
    var btn = sender;
    btn.toggleClass('FullScreenBackButton');
    if(btn.hasClass('FullScreenBackButton'))
    {
        sender.attr('title', '" + GetString("general.restore") + @"');
    }
    else
    {
        sender.attr('title', '" + fsToolTip + @"');
    }
}
";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "fullScreenScript", ScriptHelper.GetScript(fullScreenScript));

            // Register fullscreen button for new dialogs
            string fsVar = @"



function fs_" + ClientID + @"(sender)
{
    return titleFullScreen(sender);
}";

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "fsVar_" + ClientID, ScriptHelper.GetScript(fsVar));

            string fsInit = @"
$j(document).ready(function(){
    if(window.wopener && top.visiblePopup)
    {
        var topFrame = GetTop();
        if(top.isTitleWindow(topFrame, window) && !topFrame.fullScreenButtonAvailable)
        {
            var fsButton = $j('#" + pnlFullScreen.ClientID + @"');
            if(top.isFullScreen())
            {
                toggleFullScreen(fsButton);
            }
            fsButton.show(); 
            $j('#" + pnlBody.ClientID + @"').dblclick(function(){return fs_" + ClientID + @"(fsButton);});
            topFrame.setToFullScreen = function () { return !top.isFullScreen() ? fs_" + ClientID + @"(fsButton) : false; };
            topFrame.fullScreenButtonAvailable = true;
            $j(window).unload(function() {
                topFrame.fullScreenButtonAvailable = false;
            });
        }
    }
});
";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "fsInit_" + ClientID, ScriptHelper.GetScript(fsInit));

        }
        else
        {
            cellFullScreen.Visible = false;
        }
    }


    private void EnsureCloseButton()
    {
        if (ShowCloseButton)
        {
            pnlClose.Attributes.Add("onclick", "return CloseDialog();");
            pnlClose.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
            pnlClose.ToolTip = GetString("general.close");

            // Do not hide close button on mobile devices (they open a new dialog in a new tab)
            CurrentDeviceInfo device = CMSContext.CurrentDevice;
            if (!device.IsMobile)
            {
                pnlClose.Style.Add(HtmlTextWriterStyle.Display, "none");
            }

            string closeButtonScript = @"

$j(document).ready(function(){
    if(window.wopener && top.visiblePopup)
    {
        var topFrame = GetTop();
        if(top.isTitleWindow(topFrame, window) && !topFrame.closeButtonAvailable)
        {
            $j('#" + pnlClose.ClientID + @"').show();
            topFrame.closeButtonAvailable = true;
            $j(window).unload(function() {
                topFrame.closeButtonAvailable = false;
            });
        }
    }
});
";

            ScriptHelper.RegisterStartupScript(this, typeof(string), "titleClose_" + ClientID, ScriptHelper.GetScript(closeButtonScript));
        }
        else
        {
            cellClose.Visible = false;
        }
    }


    /// <summary>
    /// Creates the breadcrumbs array from the given parameters.
    /// </summary>
    /// <param name="namepath">Name path</param>
    public void CreateStaticBreadCrumbs(string namepath)
    {
        if ((namepath == null) || (namepath.Trim().Trim('/') == ""))
        {
            Breadcrumbs = null;
            return;
        }

        string[] names = namepath.Trim('/').Split('/');
        string[,] bc = new string[names.Length, 3];

        int index = 0;
        foreach (string name in names)
        {
            bc[index, 0] = name;
            bc[index, 1] = string.Empty;
            bc[index, 2] = string.Empty;
            index++;
        }
        Breadcrumbs = bc;
    }


    private void CreateBreadCrumbsItem(int index, int startIndex)
    {
        // Add RTL suffix (&rlm;) to breadcrumbs in RTL culture -> corrects design if non-RTL resource strings are used
        string rtlSuffix = CultureHelper.IsUICultureRTL() ? "&rlm;" : string.Empty;

        // Add separator
        if (index != startIndex)
        {
            plcBreadcrumbs.Controls.Add(new LiteralControl(" "));
            Label sepLabel = new Label();
            sepLabel.Text = (!string.IsNullOrEmpty(SeparatorText) ? SeparatorText : "&nbsp;") + rtlSuffix;
            sepLabel.CssClass = "TitleBreadCrumbSeparator";
            plcBreadcrumbs.Controls.Add(sepLabel);
            plcBreadcrumbs.Controls.Add(new LiteralControl(" "));
        }

        // Make link if URL specified
        string text = ResHelper.LocalizeString(Breadcrumbs[index, 0]);
        text = (EncodeBreadcrumbs ? HTMLHelper.HTMLEncode(text) : text) + rtlSuffix;
        string cssClass = (index != Breadcrumbs.GetUpperBound(0)) ? "TitleBreadCrumb" : "TitleBreadCrumbLast";

        if (!string.IsNullOrEmpty(Breadcrumbs[index, 1]) && (index != Breadcrumbs.GetUpperBound(0)))
        {
            HyperLink newLink = new HyperLink();
            newLink.Text = text;
            newLink.NavigateUrl = Breadcrumbs[index, 1];
            newLink.Target = Breadcrumbs[index, 2];
            newLink.CssClass = cssClass;
            int yDimm = Breadcrumbs.GetUpperBound(1);
            // Javascript is specified add on click
            if (yDimm == 3)
            {
                newLink.Attributes.Add("onclick", Breadcrumbs[index, 3]);
            }
            plcBreadcrumbs.Controls.Add(newLink);
        }
        else // Make label if last item or URL not specified
        {
            Label newLabel = new Label();
            newLabel.Text = text;
            newLabel.CssClass = cssClass;
            plcBreadcrumbs.Controls.Add(newLabel);
        }
    }


    /// <summary>
    /// Returns localized string.
    /// </summary>
    /// <param name="stringName">String to localize</param>
    public override string GetString(string stringName)
    {
        if (UseFileStrings || !CMSAppBase.ConnectionAvailable)
        {
            return ResHelper.GetFileString(stringName);
        }
        else
        {
            return base.GetString(stringName);
        }
    }

    #endregion
}