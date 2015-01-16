using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.ResourceManager;
using CMS.SettingsProvider;
using CMS.SiteProvider;

[ValidationPropertyAttribute("Value")]
public partial class CMSFormControls_System_CodeName : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets whether the control is read only
    /// </summary>
    public bool ReadOnly
    {
        get
        {
            return textbox.ReadOnly;
        }
        set
        {
            textbox.ReadOnly = value;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (textbox != null)
            {
                textbox.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Get the text
            string text = textbox.Text;
            if (String.IsNullOrEmpty(text) && !ExternalValue)
            {
                // Automatic code name in case of empty value
                text = BaseInfo.CODENAME_AUTOMATIC;
            }

            return text;
        }
        set
        {
            string stringValue = ValidationHelper.GetString(value, "");

            // Mark external value
            if (!String.IsNullOrEmpty(stringValue) && !RequestHelper.IsPostBack())
            {
                ExternalValue = true;
            }

            textbox.Text = stringValue;
        }
    }


    /// <summary>
    /// Returns client ID of the textbox.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return textbox.ClientID;
        }
    }
    

    /// <summary>
    /// Publicly visible textbox 
    /// </summary>
    public TextBox TextBox
    {
        get
        {
            return textbox;
        }
    }


    /// <summary>
    /// Maximum length of plain text or resource string key. Validates in IsValid() method.
    /// </summary>
    public int MaxLength
    {
        get
        {
            return textbox.MaxLength;
        }
        set
        {
            textbox.MaxLength = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site. Default value is FALSE for localizable text box.
    /// </summary>
    public override bool IsLiveSite
    {
        get;
        set;
    }


    /// <summary>
    /// If true, the hint is shown
    /// </summary>
    public bool ShowHint
    {
        get
        {
            return imgHelp.Visible;
        }
        set
        {
            imgHelp.Visible = value;
        }
    }


    /// <summary>
    /// If true, the value was set externally
    /// </summary>
    protected bool ExternalValue
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ExternalValue"], false);
        }
        set
        {
            ViewState["ExternalValue"] = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        imgHelp.ImageUrl = GetImageUrl("CMSModules/CMS_Settings/help.png");
        imgHelp.ToolTip = GetString("codename.tooltip");

        if (Enabled)
        {
            imgHelp.ToolTip += " " + GetString("codename.tooltipedit");
        }

        // Apply CSS style
        if (!String.IsNullOrEmpty(CssClass))
        {
            textbox.CssClass = CssClass;
            CssClass = null;
        }
        else
        {
            textbox.CssClass = "TextBoxField";
        }

        if (String.IsNullOrEmpty(textbox.Text))
        {
            if (!ExternalValue)
            {
                textbox.WatermarkText = "(automatic)";
                textbox.WatermarkCssClass = "CodeNameTextBoxInactive";
            }
        }
        else
        {
            textbox.Attributes.Add("onfocus", "this.className = '" + textbox.CssClass + "'");
            textbox.CssClass += " CodeNameTextBoxInactive";
        }

        textbox.TabIndex = -1;
    }


    /// <summary>
    /// Validates control.
    /// </summary>
    public override bool IsValid()
    {
        // Check for maximum length
        if (MaxLength > 0)
        {
            return (textbox.Text.Length <= MaxLength);
        }
        else
        {
            return true;
        }
    }

    #endregion
}