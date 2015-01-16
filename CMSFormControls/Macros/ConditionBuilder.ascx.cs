using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.SettingsProvider;

public partial class CMSFormControls_Macros_ConditionBuilder : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets ors sets value if the displayed text is User-Friendly representation of a macro.
    /// </summary>
    private string EditorValue
    {
        get
        {
            if (string.IsNullOrEmpty(hdnValue.Value))
            {
                return txtMacro.Text;
            }
            return hdnValue.Value;
        }
        set
        {
            hdnValue.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets macro resolver name which should be used for macro editor (if no name is given, CMSContext.CurrentResolver is used).
    /// </summary>
    public override string ResolverName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ResolverName"), "");
        }
        set
        {
            SetValue("ResolverName", value);
        }
    }


    /// <summary>
    /// Gets or sets name(s) of the Macro rule category(ies) which should be displayed in Rule designer. Items should be separated by semicolon.
    /// </summary>
    public string RuleCategoryNames
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RuleCategoryNames"), "");
        }
        set
        {
            SetValue("RuleCategoryNames", value);
        }
    }


    /// <summary>
    /// Determines which rules to display. 0 means all rules, 1 means only rules which does not require context, 2 only rules which require context.
    /// </summary>
    public int DisplayRuleType
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DisplayRuleType"), 0);
        }
        set
        {
            SetValue("DisplayRuleType", value);
        }
    }


    /// <summary>
    /// Gets or sets he maximum width of conditional builder in pixels. 
    /// 0 mean unlimited and default value is 600px.
    /// </summary>
    public int MaxWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxWidth"), 600);
        }
        set
        {
            SetValue("MaxWidth", value);
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return txtMacro.Editor.Enabled;
        }
        set
        {
            txtMacro.Editor.Enabled = value;
            btnEdit.Enabled = value;
            btnClear.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            string val = MacroResolver.RemoveDataMacroBrackets(EditorValue.Trim());
            if (!string.IsNullOrEmpty(val))
            {
                // Do not sign simple macros or Rule-based macros (the inner condition is already signed)
                if (!(MacroResolver.AllowOnlySimpleMacros || MacroResolver.IsSimpleMacro(val) || val.StartsWithCSafe("Rule(\"", true)))
                {
                    val = MacroResolver.AddMacroSecurityParams(val, CMSContext.CurrentUser.UserName);
                }
                return "{%" + val + "%}";
            }
            return "";
        }
        set
        {
            string val = MacroResolver.RemoveDataMacroBrackets(ValidationHelper.GetString(value, ""));
            string userName = null;
            val = MacroResolver.RemoveMacroSecurityParams(val, out userName);
            hdnValue.Value = MacroResolver.RemoveDataMacroBrackets(val.Trim());
            RefreshText();
        }
    }


    /// <summary>
    /// If true, auto completion is shown above the editor, otherwise it is below (default position is below).
    /// </summary>
    public bool ShowAutoCompletionAbove
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAutoCompletionAbove"), false);
        }
        set
        {
            SetValue("ShowAutoCompletionAbove", value);
            txtMacro.ShowAutoCompletionAbove = value;
        }
    }


    /// <summary>
    /// Gets ClientID of the textbox with emailinput.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtMacro.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the left offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int LeftOffset
    {
        get
        {
            return txtMacro.LeftOffset;
        }
        set
        {
            txtMacro.LeftOffset = value;
        }
    }


    /// <summary>
    ///  Gets or sets the top offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int TopOffset
    {
        get
        {
            return txtMacro.TopOffset;
        }
        set
        {
            txtMacro.TopOffset = value;
        }
    }

    #endregion


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set control style and css class
        if (!string.IsNullOrEmpty(ControlStyle))
        {
            txtMacro.Editor.Attributes.Add("style", ControlStyle);
        }
        if (!string.IsNullOrEmpty(CssClass))
        {
            txtMacro.Editor.CssClass = CssClass;
        }

        txtMacro.ShowAutoCompletionAbove = ShowAutoCompletionAbove;
        txtMacro.Editor.UseSmallFonts = true;
        txtMacro.MixedMode = false;

        txtMacro.Resolver = ResolverFactory.GetResolver(ResolverName);

        var ed = txtMacro.Editor;
        ed.ShowToolbar = false;
        ed.ShowLineNumbers = false;
        ed.DynamicHeight = true;

        string script = @"
function InsertMacroCondition" + this.ClientID + @"(text) {
    var hidden = document.getElementById('" + hdnValue.ClientID + @"');
    hidden.value = text;
    " + ControlsHelper.GetPostBackEventReference(btnRefresh, null) + @";
}";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "InsertMacroCondition" + this.ClientID, script, true);
        ScriptHelper.RegisterDialogScript(Page);

        btnEdit.ToolTip = GetString("macrodesigner.edit");
        btnEdit.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Edit.png");
        btnEdit.Click += new ImageClickEventHandler(btnEdit_Click);

        btnClear.ToolTip = GetString("macrodesigner.clearcondition");
        btnClear.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Clear.png");
        btnClear.OnClientClick = "if (confirm(" + ScriptHelper.GetString(GetString("macrodesigner.clearconditionconfirm")) + ")) { InsertMacroCondition" + this.ClientID + "(''); } return false;";

        btnRefresh.Click += new EventHandler(btnRefresh_Click);

        if (txtMacro.ReadOnly)
        {
            txtMacro.Editor.Language = LanguageEnum.Text;
        }

        if (MaxWidth > 0)
        {
            pnlConditionBuilder.Attributes["style"] += " max-width: " + MaxWidth + "px;";
        }
    }


    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        RefreshText();
    }


    private void RefreshText()
    {
        string hdnValueTrim = hdnValue.Value.Trim();
        if (hdnValueTrim.StartsWithCSafe("rule(", true))
        {
            // Empty rule designer condition is not considered as rule conditions
            if (hdnValueTrim.EqualsCSafe("Rule(\"\", \"<rules></rules>\")", true))
            {
                hdnValue.Value = "";
            }
            else
            {
                bool failToParseXml = false;
                string ruleText = MacroRuleTree.GetRuleText(hdnValueTrim, true, out failToParseXml);
                if (failToParseXml)
                {
                    // If failed to parse the rule, extract the condition
                    MacroExpression xml = MacroExpression.ExtractParameter(hdnValueTrim, "rule", 0);
                    if (xml != null)
                    {
                        string user = null;
                        hdnValue.Value = MacroResolver.RemoveMacroSecurityParams(ValidationHelper.GetString(xml.Value, ""), out user);
                    }
                }
                else
                {
                    // Display rule text
                    ltlMacro.Text = ruleText;
                    txtMacro.Visible = false;
                    pnlRule.Visible = true;
                    return;
                }
            }
        }

        txtMacro.Text = hdnValue.Value;
        hdnValue.Value = null;
        txtMacro.Visible = true;
        pnlRule.Visible = false;

        pnlUpdate.Update();
    }


    protected void btnEdit_Click(object sender, ImageClickEventArgs e)
    {
        SessionHelper.SetValue("ConditionBuilderCondition" + this.ClientID, EditorValue);

        string dialogUrl = String.Format("{0}?clientid={1}&module={2}&ruletype={3}", CMSContext.ResolveDialogUrl("~/CMSFormControls/Macros/ConditionBuilder.aspx"), ClientID, RuleCategoryNames, DisplayRuleType);
        if (!string.IsNullOrEmpty(ResolverName))
        {
            SessionHelper.SetValue("ConditionBuilderResolver" + this.ClientID, ResolverName);
        }

        ScriptHelper.RegisterStartupScript(this.Page, typeof(string), "ConditionBuilderDialog", "modalDialog('" + dialogUrl + "', 'editmacrocondition', '95%', 700);", true);
    }
}