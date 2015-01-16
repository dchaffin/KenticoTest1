using System;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.CMSHelper;

public partial class CMSMasterPages_UI_Dialogs_ModalDialogPage : CMSMasterPage
{
    #region "Properties"

    /// <summary>
    /// PageTitle control.
    /// </summary>
    public override PageTitle Title
    {
        get
        {
            return titleElem;
        }
    }


    /// <summary>
    /// HeaderActions control.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            if (base.HeaderActions != null)
            {
                return base.HeaderActions;
            }
            return actionsElem.HeaderActions;
        }
    }


    /// <summary>
    /// Header container.
    /// </summary>
    public override Panel HeaderContainer
    {
        get
        {
            return pnlContainerHeader;
        }
    }


    /// <summary>
    /// Body panel.
    /// </summary>
    public override Panel PanelBody
    {
        get
        {
            return pnlBody;
        }
    }


    /// <summary>
    /// Gets the content panel.
    /// </summary>    
    public override Panel PanelContent
    {
        get
        {
            return pnlContent;
        }
    }


    /// <summary>
    /// Footer panel.
    /// </summary>
    public override Panel PanelFooter
    {
        get
        {
            return pnlFooter;
        }
    }


    /// <summary>
    /// Gets the header panel
    /// </summary>
    public override Panel PanelHeader
    {
        get
        {
            return pnlHeader;
        }
    }


    /// <summary>
    /// Gets the labels container.
    /// </summary>
    public override PlaceHolder PlaceholderLabels
    {
        get
        {
            return plcLabels;
        }
    }


    /// <summary>
    /// Body object.
    /// </summary>
    public override HtmlGenericControl Body
    {
        get
        {
            return bodyElem;
        }
    }


    /// <summary>
    /// Prepared for specifying the additional HEAD elements.
    /// </summary>
    public override Literal HeadElements
    {
        get
        {
            return ltlHeadElements;
        }
        set
        {
            ltlHeadElements = value;
        }
    }


    /// <summary>
    /// Panel containing title actions displayed above scrolling content.
    /// </summary>
    public override Panel PanelTitleActions
    {
        get
        {
            return pnlTitleActions;
        }
    }


    /// <summary>
    /// Footer container.
    /// </summary>
    public override Panel FooterContainer
    {
        get
        {
            return pnlFooterContainer;
        }
    }


    /// <summary>
    /// Gets placeholder located after form element.
    /// </summary>
    public override PlaceHolder AfterFormPlaceholder
    {
        get
        {
            return plcAfterForm;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        PageStatusContainer = plcStatus;
        if (MessagesPlaceHolder != null)
        {
            MessagesPlaceHolder.OffsetY = 10;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Display panel with additional controls place holder if required
        if (DisplayControlsPanel)
        {
            pnlAdditionalControls.Visible = true;
        }

        // Display panel with site selector
        if (DisplaySiteSelectorPanel)
        {
            pnlSiteSelector.Visible = true;
        }

        bodyElem.Attributes["class"] = mBodyClass;

        titleElem.HelpIconUrl = UIHelper.GetImageUrl(Page, "General/HelpLargeDark.png");

        CurrentDeviceInfo device = CMSContext.CurrentDevice;
        if (!device.IsMobile)
        {
            // Footer - apply fixed position
            pnlFooterContent.Style.Add("position", "fixed");
            pnlFooterContent.Style.Add("width", "100%");
            pnlFooterContent.Style.Add("bottom", "0px");
        }

        StringBuilder resizeScript = new StringBuilder();
        resizeScript.Append(@"
var headerElem = null;
var footerElem = null;
var contentElem = null;
var jIframe = null;
var jIframeContents = null;
var oldClientWidth = 0;
var oldClientHeight = 0;
var dialogCMSHeaderPad = null;
var dialogCKFooter = null;


function ResizeWorkingAreaIE()
{
    ResizeWorkingArea();
    window.onresize = function() { ResizeWorkingArea(); };
}

function ResizeWorkingArea()
{
    if (headerElem == null)
    {
       headerElem = document.getElementById('divHeader');
    }
    if (footerElem == null)
    {
       footerElem = document.getElementById('divFooter');
    }
    if (contentElem == null)
    {
       contentElem = document.getElementById('divContent');
    }

    if (dialogCMSHeaderPad == null)
    {
       dialogCMSHeaderPad = document.getElementById('CMSHeaderPad');
    }

    if (dialogCKFooter == null)
    {
       dialogCKFooter = document.getElementById('CKFooter');
    }

    if ((headerElem != null) && (footerElem != null) && (contentElem != null))
    {
        var headerHeight = headerElem.offsetHeight + ((dialogCMSHeaderPad != null) ? dialogCMSHeaderPad.offsetHeight : 0);
        var footerHeight = footerElem.offsetHeight + ((dialogCKFooter != null) ? dialogCKFooter.offsetHeight : 0);
        var height = (document.body.offsetHeight - headerHeight - footerHeight);
        if (height > 0)
        {
            var h = (height > 0 ? height : '0') + 'px';
            if (contentElem.style.height != h)
            {
                contentElem.style.height = h;
            }
        }");

        if (device.IsMobile) 
        {
            resizeScript.Append(@"
        if ((jIframe == null) || (!jIframe.length)) {
            jIframe = jQuery('.EditableTextEdit iframe:first');
        }
        if ((jIframeContents == null) || (!jIframeContents.length)) {
            jIframeContents = jIframe.contents();
        }

        // Set height of the iframe manually for mobile devices 
        jIframe.css('height', jIframeContents.height());
        // WebKit browsers fix - width of the parent element of the iframe needs to be defined
        jIframe.parent().width(jIframeContents.width());");
        }

        if (BrowserHelper.IsIE())
        {
            resizeScript.Append(@"
        var pnlBody = null;
        var formElem = null;
        var bodyElement = null;
        if (pnlBody == null)
        {
            pnlBody = document.getElementById('", pnlBody.ClientID, @"');
        }
        if (formElem == null)
        {
            formElem = document.getElementById('", form1.ClientID, @"');
        }
        if (bodyElement == null)
        {
            bodyElement = document.getElementById('", bodyElem.ClientID, @"');
        }
        if ((bodyElement != null) && (formElem != null) && (pnlBody != null))
        {
            var newClientWidth = document.documentElement.clientWidth;
            var newClientHeight = document.documentElement.clientHeight;
            if  (newClientWidth != oldClientWidth)
            {
                bodyElement.style.width = newClientWidth;
                formElem.style.width = newClientWidth;
                pnlBody.style.width = newClientWidth;
                headerElem.style.width = newClientWidth;
                contentElem.style.width = newClientWidth;
                oldClientWidth = newClientWidth;
            }
            if  (newClientHeight != oldClientHeight)
            {
                bodyElement.style.height = newClientHeight;
                formElem.style.height = newClientHeight;
                pnlBody.style.height = newClientHeight;
                oldClientHeight = newClientHeight;
            }
        }");
        }

        resizeScript.Append(@"
    }
    if (window.afterResize) {
        window.afterResize();
    }
}");
        if (BrowserHelper.IsIE())
        {
            resizeScript.Append(@"
var timer = setInterval('ResizeWorkingAreaIE();', 50);");
        }
        else
        {
            resizeScript.Append(@"
window.onresize = function() { ResizeWorkingArea(); };
window.onload = function() { ResizeWorkingArea(); };");
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "resizeScript", ScriptHelper.GetScript(resizeScript.ToString()));

        if (BrowserHelper.IsIE7())
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "ie7ResizeFix", ScriptHelper.GetScript("document.getElementById('divContent').style.height = '0px';"));
        }

        // Add custom CSS class for dialog page
        CMSContentPage page = Page as CMSContentPage;
        if ((page != null) && page.RequiresDialog && page.UseDialogContentClass)
        {
            pnlContent.CssClass = "DialogContentPanel";
        }

        // Register a script that will re-calculate content height when the CKToolbar is displayed
        const string ckEditorScript = @"
if (window.CKEDITOR) {
    CKEDITOR.on('instanceCreated', function(e) {
        e.editor.on('instanceReady', function(e) { setTimeout(function() { ResizeWorkingArea(); }, 200); });
    });
}";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "ckEditorScript", ckEditorScript, true);
    }


    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        // Hide actions panel if no actions are present and DisplayActionsPanel is false
        if (!DisplayActionsPanel)
        {
            if (!HeaderActions.Visible && (plcActions.Controls.Count == 0))
            {
                pnlActions.Visible = false;
            }
        }

        base.Render(writer);
    }

    #endregion
}