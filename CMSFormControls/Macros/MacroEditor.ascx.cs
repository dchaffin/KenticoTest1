using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.ExtendedControls;

public partial class CMSFormControls_Macros_MacroEditor : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the value of this form control.
    /// </summary>
    /// <value>Text content of this editor</value>
    [Browsable(false)]
    public override object Value
    {
        get
        {
            return ucEditor.Text;
        }
        set
        {
            ucEditor.Text = (string)value;
        }
    }


    /// <summary>
    /// Gets or sets whether this form control is enabled.
    /// </summary>
    /// <value>True, if form control is enabled, otherwise false</value>
    [Browsable(true)]
    [Description("Determines whether this form control is enabled")]
    [Category("Form Control")]
    [DefaultValue(true)]
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = ucEditor.Editor.Enabled = value;
        }
    }


    /// <summary>
    /// Gets the editor control.
    /// </summary>
    public ExtendedTextArea Editor
    {
        get
        {
            return ucEditor.Editor;
        }
    }


    /// <summary>
    /// Gets or sets the left offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int LeftOffset
    {
        get
        {
            return ucEditor.LeftOffset;
        }
        set
        {
            ucEditor.LeftOffset = value;
        }
    }


    /// <summary>
    /// Gets or sets the top offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int TopOffset
    {
        get
        {
            return ucEditor.TopOffset;
        }
        set
        {
            ucEditor.TopOffset = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
    }
}