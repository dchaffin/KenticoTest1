using System;
using System.Web.UI;
using System.Web;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.CMSHelper;

[Title("Design/Controls/Validation/codeviewtitle.png", "validation.viewcodetooltip", null)]
public partial class CMSModules_Content_CMSDesk_Validation_ViewCode : CMSPage, IPostBackEventHandler
{
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash"))
        {
            RedirectToAccessDenied(ResHelper.GetString("dialogs.badhashtitle"));
        }

        // Set CSS classes
        CurrentMaster.PanelFooter.CssClass = "FloatRight";

        // Add close button
        CurrentMaster.PanelFooter.Controls.Add(new LocalizedButton
                                                   {
                                                       ID = "btnClose",
                                                       ResourceString = "general.close",
                                                       EnableViewState = false,
                                                       OnClientClick = "return CloseDialog();",
                                                       CssClass = "SubmitButton"
                                                   });
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string url = HttpUtility.UrlDecode(QueryHelper.GetString("url", null));
        string format = QueryHelper.GetString("format", null);
        
        string message = HttpUtility.UrlDecode(QueryHelper.GetString("message", null));
        if (!String.IsNullOrEmpty(message))
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        txtCodeText.HighlightMacros = false;
        txtCodeText.AllowFullscreen = false;

        if (!String.IsNullOrEmpty(format))
        {
            txtCodeText.Language = LanguageCode.GetLanguageEnumFromString(format);
            SetTitle("Design/Controls/Validation/codeviewtitle.png", GetString("validation.css.viewcodetooltip"), null, null);
        }

        InitializeScripts(url);
    }


    /// <summary>
    /// Initializes the validation scripts
    /// </summary>
    /// <param name="url">URL of the page</param>
    private void InitializeScripts(string url)
    {
        ScriptHelper.RegisterScriptFile(Page, "Validation.js");
        ScriptHelper.RegisterJQuery(Page);

        // Disable minification on URL
        if (txtCodeText.Language == LanguageEnum.CSS)
        {
            url = DocumentValidationHelper.DisableMinificationOnUrl(url);
        }

        RegisterModalPageScripts();

        string script = @"
function ResizeCodeArea() {
    var height = $j(""#divContent"").height();
    $j(""#" + txtCodeText.ClientID + @""").parent().css(""height"", height + ""px"");
    $j("".CodeMirror-scroll"").css(""height"", height - 26 + ""px"");
}

$j(window).resize(function(){ResizeCodeArea()});
$j(document).ready(function(){setTimeout(""ResizeCodeArea()"",300);" + ((!RequestHelper.IsPostBack() && !String.IsNullOrEmpty(url)) ? "LoadHTMLToElement('" + hdnHTML.ClientID + "','" + url + "');" + ControlsHelper.GetPostBackEventReference(this, null) + ";" : "") + @"});$j(""#divContent"").css(""overflow"", ""hidden"");
";

        // Register script for resizing and scroll bar remove
        ScriptHelper.RegisterStartupScript(this, typeof(string), "AreaResizeAndScrollBarRemover", ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Initializes the default editor position
    /// </summary>
    private void InitPosition()
    {
        // Set the initial position
        int line = QueryHelper.GetInteger("line", 0);
        int ch = QueryHelper.GetInteger("ch", 0);

        if ((line > 0) || (ch > 0))
        {
            txtCodeText.SetPosition(true, line - 1, ch - 1);
            pnlCode.CssClass = "ValidationCode";
        }
    }


    #region "IPostBackEventHandler Members"

    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
        txtCodeText.Text = ValidationHelper.Base64Decode(hdnHTML.Value);
        if (txtCodeText.Language == LanguageEnum.CSS)
        {
            txtCodeText.Text = txtCodeText.Text.Trim(new char[] { '\r', '\n' });
        }

        InitPosition();

        hdnHTML.Value = "";
    }

    #endregion
}