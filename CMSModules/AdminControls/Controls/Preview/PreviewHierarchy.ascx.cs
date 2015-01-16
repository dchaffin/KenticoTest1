using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;

public partial class CMSModules_AdminControls_Controls_Preview_PreviewHierarchy : CMSPreviewControl
{
    #region "Variables"

    UILayoutPane panePreview = null;
    UILayoutPane paneToolbar = null;
    int previewValue = 0;
    bool mStorePreviewScrollPosition = false;
    bool registerScrollScript = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the key under which the preview state is stored in the cookies
    /// </summary>
    public String CookiesPreviewStateName
    {
        get;
        set;
    }


    /// <summary>
    /// Path for content (edit) control. Must be set, before page load.
    /// </summary>
    public String ContentControlPath
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether show panel separator (used in vertical mode)
    /// </summary>
    public bool ShowPanelSeparator
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether the preview control should display the "Save" and "Save and close" buttons.
    /// </summary>
    public bool ShowSaveButtons
    {
        get;
        set;
    }


    /// <summary>
    /// If true, scroll position is stored between save requests
    /// </summary>
    public bool StorePreviewScrollPosition
    {
        get
        {
            return mStorePreviewScrollPosition;
        }
        set
        {
            mStorePreviewScrollPosition = value;
        }
    }


    /// <summary>
    /// Return UILayoutPane control.
    /// </summary>
    public UILayoutPane LayoutPane
    {
        get
        {
            return paneContent;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        paneContent.ControlPath = ContentControlPath;
        paneContent.Values.Add(new UILayoutValue("DialogMode", DialogMode));
        base.OnInit(e);
    }


    /// <summary>
    /// Adds content envelope ClientID. Must be added after page Init event to calculate right ClientID but before OnLoad event.
    /// </summary>
    public void RegisterEnvelopeClientID()
    {
        paneContent.Values.Add(new UILayoutValue("ParentClientID", paneContent.ClientID));
    }


    protected override void OnLoad(EventArgs e)
    {
        ICMSMasterPage master = Page.Master as ICMSMasterPage;
        if ((master != null) && (master.FooterContainer != null))
        {
            master.FooterContainer.Visible = false;
        }

        ScriptHelper.RegisterScriptFile(Page, "Controls/CodePreview.js");

        imgProgress.ImageUrl = GetImageUrl("/Design/Preloaders/preload16.gif");
        paneToolbar = new UILayoutPane();

        if (!DialogMode)
        {
            paneFooter.Visible = false;
        }

        previewValue = GetPreviewStateFromCookies(CookiesPreviewStateName);
        if (CMSContext.EditedObject == null)
        {
            previewValue = 0;
        }

        // Change preview state handling
        string args = Request["__EVENTARGUMENT"];
        string target = Request["__EVENTTARGET"];

        if (target == btnHidden.UniqueID)
        {
            switch (args)
            {
                case "vertical":
                    previewValue = 1;
                    break;

                case "horizontal":
                    previewValue = 2;
                    break;

                case "split":
                    previewValue = (previewValue == 0) ? 1 : 0;
                    if (previewValue != 0)
                    {
                        RegisterFullScreen();
                        PreviewInitialized = true;
                        paneToolbar.Values.Add(new UILayoutValue("PreviewInitialized", true));
                    }
                    break;
            }

            SetPreviewStateToCookies(CookiesPreviewStateName, previewValue);
            paneToolbar.Values.Add(new UILayoutValue("SetControls", true));
        }

        UILayout subLayout = paneContentMain.FindControl("layoutElem") as UILayout;
        panePreview = subLayout.FindControl("panePreview") as UILayoutPane;

        // Pane toolbar
        paneToolbar.ID = "paneToolbar";
        paneToolbar.Direction = PaneDirectionEnum.North;
        paneToolbar.ControlPath = "~/CMSModules/AdminControls/Controls/Preview/PreviewNavigationButtons.ascx";
        paneToolbar.Resizable = false;
        paneToolbar.Slidable = false;
        paneToolbar.RenderAs = HtmlTextWriterTag.Div;
        paneToolbar.SpacingOpen = 0;

        if (AddTitleToToolbar && (previewValue != 0))
        {
            // Add toolbar padding to show title
            paneToolbar.PaneClass = "PreviewTitlePadding";
        }

        paneToolbar.Values.Add(new UILayoutValue("PreviewURLSuffix", PreviewURLSuffix));
        paneToolbar.Values.Add(new UILayoutValue("CookiesPreviewStateName", CookiesPreviewStateName));
        paneToolbar.Values.Add(new UILayoutValue("PreviewObjectName", PreviewObjectName));
        paneToolbar.Values.Add(new UILayoutValue("DefaultAliasPath", DefaultAliasPath));
        paneToolbar.Values.Add(new UILayoutValue("DialogMode", DialogMode));
        paneToolbar.Values.Add(new UILayoutValue("IgnoreSessionValues", IgnoreSessionValues));
        paneToolbar.Values.Add(new UILayoutValue("DefaultPreviewPath", DefaultPreviewPath));

        paneContentMain.Visible = true;
        subLayout.StopProcessing = false;

        paneFooter.SpacingOpen = 0;
        paneFooter.Values.Add(new UILayoutValue("ShowSaveButtons", ShowSaveButtons));

        // Check if inner control denied displaying preview
        CMSPreviewControl pw = paneContent.UserControl as CMSPreviewControl;
        if (pw != null)
        {
            if (!pw.ShowPreview)
            {
                previewValue = 0;
            }
        }

        switch (previewValue)
        {
            // No split
            case 0:
                subLayout.StopProcessing = true;
                paneContentMain.Visible = false;
                paneContent.Direction = PaneDirectionEnum.Center;

                break;

            // Vertical
            case 1:
                int additionalSize = AddTitleToToolbar ? 38 : 0;
                paneContent.SpacingOpen = 14;
                paneContent.SpacingClosed = 14;
                paneContent.ResizerClass = "TransformationVerticalResizer";
                paneToolbar.Values.Add(new UILayoutValue("ShowPanelSeparator", ShowPanelSeparator));


                paneToolbar.SpacingOpen = 0;
                paneToolbar.Size = ShowPanelSeparator ? (41 + additionalSize).ToString() : (35 + additionalSize).ToString();

                layoutElem.Controls.Add(paneToolbar);
                layoutElem.Panes.Add(paneToolbar);
                paneContent.Direction = PaneDirectionEnum.West;

                break;

            // Horizontal
            case 2:
                paneToolbar.PaneClass = "HorizontalToolbar";
                paneContent.ResizerClass = "GrayResizer";
                paneToolbar.Size = "30";

                subLayout.Controls.Add(paneToolbar);
                subLayout.Panes.Add(paneToolbar);
                break;
        }

        if ((previewValue != 0) && !RequestHelper.IsPostBack())
        {
            RegisterFullScreen();
        }

        base.OnLoad(e);
    }


    /// <summary>
    /// Registers javascript for layout communication
    /// </summary>
    private void RegisterHierarchyScripts()
    {
        String refresh = @"             
function refreshPreview() {
    var preview = document.getElementById('" + panePreview.ClientID + @"');
    var scrollTop = 0 ;
    var scrollLeft = 0;
    if ('" + registerScrollScript.ToString() + @"' == 'True') {
        var jpreview = $j(preview);
        scrollTop = $j(jpreview).contents().scrollTop();
        scrollLeft = $j(jpreview).contents().scrollLeft();
    }

    var state = " + previewValue + @";
    if (preview != null) {
            var loc = preview.src;
            preview.src = preventCacheLocation(preview.src, preview.src);
    }
                                                
    if (state != 0) {                                                                                                     
        showProgressBar(preview, scrollTop, scrollLeft);
    }
} 

function showProgressBar(preview, scrollTop, scrollLeft) {
    var pr = $j('#" + paneContentMain.ClientID + @"');  
    var pnlProgress = $j('#" + pnlProgress.ClientID + @"');
    var jpreview = $j(preview); 
                                                    
    var isRTL = $j('body').hasClass('RTL');
    pnlProgress.show();
    var left = isRTL ? pr.width() : pr.offset().left;

    pnlProgress.offset({ top: pr.offset().top, left: left });
    var pnlBackground = $j('#" + pnlBackGround.ClientID + @"');
    pnlBackground.width(pr.width());                                                   
    pnlBackground.height(pr.height());
   
    $j(jpreview).load(function() 
    {
        if ((scrollTop != 0) || (scrollLeft != 0)) {
            $j(jpreview).contents().scrollTop(scrollTop);
            $j(jpreview).contents().scrollLeft(scrollLeft);
        }

        pnlProgress.hide();

        // Prevent multiple load events
        $j(jpreview).unbind('load');
    });
}

function refreshPreviewParam(src) {  
    var preview = document.getElementById('" + panePreview.ClientID + @"');
    var state = " + previewValue + @";
    if (preview != null) {
        preview.src = preventCacheLocation(src, preview.src);
    }
    if (state != 0) { 
        showProgressBar(preview, 0, 0);
    }
}                                                                                                                                                                                 
function performToolbarAction(action) {                                                    
    " + Page.ClientScript.GetPostBackEventReference(btnHidden, "#").Replace("'#'", "action") + @" 
}
function preventCacheLocation(src,contentSrc) { 
        var newLocation = src;
        if (src == contentSrc) {                                                            
            if (newLocation.indexOf('refreshtoken=0') != -1) {
                newLocation = newLocation.replace('refreshtoken=0','refreshtoken=1');
            } else {
                newLocation = newLocation.replace('refreshtoken=1','refreshtoken=0');
            }
        }
        return newLocation;
}
";

        // For page title, add resize scripts for content
        if (AddTitleToToolbar)
        {
            if (previewValue != 1)
            {
                refresh +=
@" 
function resizePreviewContent(setMargin) {
    var delimiters = " + (previewValue == 0 ? 0 : 4) + @";
    var title = $j('.DialogsPageHeader');
    var panel = $j('#" + paneContent.ClientID + @"');   
    var toolbar =  $j('#" + paneToolbar.ClientID + @"');           
    var footer = $j('#" + paneFooter.ClientID + @"');           
    var preview =  $j('#" + panePreview.ClientID + @"');           
        if (title != null) {
            var titleHeight = title.height();     
            if (setMargin) {
                panel.css('margin-top', titleHeight);;
            }                            
            panel.height($j(window).height() - toolbar.height() - preview.height() - titleHeight - footer.height() - delimiters);
            CM_FullScreen(IsFullScreen, EditorElem);
        }     
    }
    $j(window).ready(function () { 
        resizePreviewContent(true);
    });                               
    $j(window).resize(function() {
        setTimeout(function(){     
            resizePreviewContent(false);
        }, 300);
    });
";
            }
        }

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "RefreshWindow", ScriptHelper.GetScript(refresh));

    }


    protected override void OnPreRender(EventArgs e)
    {
        String url = GetPreviewURL();

        // Register scroll script only for same domains to prevent access denied js errors
        registerScrollScript = (StorePreviewScrollPosition && (URLHelper.GetDomain(url) == URLHelper.GetCurrentDomain()));
        RegisterHierarchyScripts();

        panePreview.Src = url;

        base.OnPreRender(e);
    }


    /// <summary>
    /// Add parameter to content parameter collection
    /// </summary>
    /// <param name="val">Content parameter</param>
    public void AddContentParameter(UILayoutValue val)
    {
        paneContent.Values.Add(val);
    }


    /// <summary>
    /// Register script for window fullscreen in modal dialog
    /// </summary>
    private void RegisterFullScreen()
    {
        if (DialogMode)
        {
            const string script = @"
$j(document).ready(function() {
    if (GetTop) 
    {
        var topFrame = GetTop();
        if(topFrame.setToFullScreen)
        {
            topFrame.setToFullScreen();
            $j(window).trigger('resize');
        }
        else
        {
            $j(topFrame).load(function(){ topFrame.setToFullScreen(); $j(window).trigger('resize'); });
        }
    }
})";
            ScriptHelper.RegisterStartupScript(Page, typeof(String), "FullScreenScript", ScriptHelper.GetScript(script));
        }
    }

    #endregion
}
