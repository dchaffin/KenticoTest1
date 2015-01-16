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

public partial class CMSFormControls_Macros_MacroSyntaxHighlighter : FormEngineUserControl
{
    #region "Properties"

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
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtMacro.Text;
        }
        set
        {
            txtMacro.Text = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Gets or sets macro resolver of the macro editor.
    /// </summary>
    public ContextResolver Resolver
    {
        get
        {
            return txtMacro.Resolver;
        }
        set
        {
            txtMacro.Resolver = value;
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
        if (Form != null)
        {
            txtMacro.ResolverName = Form.ResolverName;
        }

        txtMacro.ShowAutoCompletionAbove = ShowAutoCompletionAbove;
        txtMacro.Editor.UseSmallFonts = true;
        txtMacro.Editor.Height = new Unit("80px");
        txtMacro.MixedMode = false;
        txtMacro.Editor.ShowToolbar = false;
        txtMacro.Editor.ShowLineNumbers = false;
    }
}