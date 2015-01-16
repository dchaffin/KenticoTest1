using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Text;

using AjaxControlToolkit;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.PortalControls;

public partial class CMSFormControls_Basic_NumericUpDown : FormEngineUserControl
{
    #region "Variables"

    private Dictionary<string, string> values = null;
    private string innerValue = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return textbox.Enabled;
        }
        set
        {
            textbox.Enabled = value;
            btnDown.Enabled = value;
            btnUp.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            if ((values != null) && (values.Count > 0))
            {
                string key = textbox.Text;
                if (values.ContainsKey(key))
                {
                    return values[key];
                }
                return values.First().Value;
            }

            return textbox.Text;
        }
        set
        {
            object convertedValue = value;
            if ((value != null) || ((FieldInfo != null) && FieldInfo.AllowEmpty))
            {
                if (FieldInfo != null)
                {
                    // Convert default boolean value to proper format
                    if (FieldInfo.DataType == FormFieldDataTypeEnum.Boolean)
                    {
                        convertedValue = ValidationHelper.GetBoolean(value, false);
                    }
                    // Ensure rendering of decimal in current culture format
                    else if (FieldInfo.DataType == FormFieldDataTypeEnum.Decimal)
                    {
                        convertedValue = FormHelper.GetDoubleValueInCurrentCulture(value);
                    }
                    // Ensure rendering of datetime in current culture format
                    else if (FieldInfo.DataType == FormFieldDataTypeEnum.DateTime)
                    {
                        convertedValue = FormHelper.GetDateTimeValueInCurrentCulture(value);
                    }
                }

                innerValue = ValidationHelper.GetString(convertedValue, null);
                LoadValues();
                if ((values != null) && (values.Count > 0))
                {
                    foreach (string key in values.Keys)
                    {
                        if (values[key] == innerValue)
                        {
                            textbox.Text = key;
                            break;
                        }
                    }
                }
                else
                {
                    textbox.Text = innerValue;
                }
            }
        }
    }

    #endregion


    #region "Custom properties"

    /// <summary>
    /// Step used for simple numeric incrementing and decrementing. The default value is 1.
    /// </summary>
    public int Step
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Step"), 1);
        }
        set
        {
            SetValue("Step", value);
        }
    }


    /// <summary>
    /// Minimal value of the selector.
    /// </summary>
    public double Minimum
    {
        get
        {
            return ValidationHelper.GetDouble(GetValue("Minimum"), 0);
        }
        set
        {
            SetValue("Minimum", value);
        }
    }


    /// <summary>
    /// Maximal value of the selector.
    /// </summary>
    public double Maximum
    {
        get
        {
            return ValidationHelper.GetDouble(GetValue("Maximum"), 0);
        }
        set
        {
            SetValue("Maximum", value);
        }
    }


    /// <summary>
    /// Combined size of the TextBox and Up/Down buttons (min value 25). This property is not used if you provide custom buttons.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 25);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// URL of the image to display as the Up button.
    /// </summary>
    public string UpButtonImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UpButtonImageUrl"), null);
        }
        set
        {
            SetValue("UpButtonImageUrl", value);
        }
    }


    /// <summary>
    /// URL of the image to display as the Down button.
    /// </summary>
    public string DownButtonImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DownButtonImageUrl"), null);
        }
        set
        {
            SetValue("DownButtonImageUrl", value);
        }
    }


    /// <summary>
    /// Web service method that returns the data used to get the previous value, or the name of a method declared on the Page which is decorated with the WebMethodAttribute.
    /// </summary>
    public string ServiceDownMethod
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ServiceDownMethod"), null);
        }
        set
        {
            SetValue("ServiceDownMethod", value);
        }
    }


    /// <summary>
    /// Path to a web service that returns the data used to get the previous value. This property should be left null if ServiceUpMethod or ServiceDownMethod refers to a page method. The web service should be decorated with the System.Web.Script.Services.ScriptService attribute.
    /// </summary>
    public string ServiceDownPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ServiceDownPath"), null);
        }
        set
        {
            SetValue("ServiceDownPath", value);
        }
    }


    /// <summary>
    /// Web service method that returns the data used to get the next value, or the name of a method declared on the Page which is decorated with the WebMethodAttribute.
    /// </summary>
    public string ServiceUpMethod
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ServiceUpMethod"), null);
        }
        set
        {
            SetValue("ServiceUpMethod", value);
        }
    }


    /// <summary>
    /// Path to a web service that returns the data used to get the next value. This property should be left null if ServiceUpMethod or ServiceDownMethod refers to a page method. The web service should be decorated with the System.Web.Script.Services.ScriptService attribute.
    /// </summary>
    public string ServiceUpPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ServiceUpPath"), null);
        }
        set
        {
            SetValue("ServiceUpPath", value);
        }
    }


    /// <summary>
    /// Specifies a custom parameter to pass to the Web Service.
    /// </summary>
    public string Tag
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Tag"), null);
        }
        set
        {
            SetValue("Tag", value);
        }
    }


    /// <summary>
    /// The alt text to show when the mouse is over the  Up button.
    /// </summary>
    public string UpButtonImageAlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UpButtonImageAlternateText"), ResHelper.GetString("general.up"));
        }
        set
        {
            SetValue("UpButtonImageAlternateText", value);
        }
    }


    /// <summary>
    /// The alt text to show when the mouse is over the  Down button.
    /// </summary>
    public string DownButtonImageAlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DownButtonImageAlternateText"), ResHelper.GetString("general.down"));
        }
        set
        {
            SetValue("DownButtonImageAlternateText", value);
        }
    }

    #endregion


    #region "Methods"

    private void LoadValues()
    {
        if (values == null)
        {
            string options = ValidationHelper.GetString(GetValue("options"), null);
            string query = ValidationHelper.GetString(GetValue("query"), null);
            ListItemCollection items = new ListItemCollection();
            values = new Dictionary<string, string>();

            try
            {
                FormHelper.LoadItemsIntoList(options, query, items, FieldInfo);

                foreach (ListItem item in items)
                {
                    values.Add(item.Text, item.Value);
                }
            }
            catch (Exception ex)
            {
                FormControlError ctrlError = new FormControlError();
                ctrlError.InnerException = ex;
                Controls.Add(ctrlError);
                pnlContainer.Visible = false;
            }
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Initialize properties
        PortalHelper.EnsureScriptManager(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Create extender
        NumericUpDownExtender exNumeric = new NumericUpDownExtender();
        exNumeric.ID = "exNum";
        exNumeric.TargetControlID = textbox.ID;
        exNumeric.EnableViewState = false;
        Controls.Add(exNumeric);

        // Initialize extender
        exNumeric.Minimum = Minimum;
        exNumeric.Maximum = Maximum;
        exNumeric.Step = Step;
        exNumeric.Width = Width;

        LoadValues();
        if ((values != null) && (values.Count > 0))
        {
            string refValues = null;
            foreach (string key in values.Keys)
            {
                refValues += key + ";";
            }
            exNumeric.RefValues = refValues.TrimEnd(';');
        }

        // Initialize up button
        if (!string.IsNullOrEmpty(UpButtonImageUrl))
        {
            btnUp.Visible = true;
            btnUp.ImageUrl = UpButtonImageUrl;
            btnUp.AlternateText = CMSContext.CurrentResolver.ResolveMacros(UpButtonImageAlternateText);
            btnUp.ImageAlign = ImageAlign.Middle;
            exNumeric.TargetButtonUpID = btnUp.ID;
        }

        // Initialize down button
        if (!string.IsNullOrEmpty(DownButtonImageUrl))
        {
            btnDown.Visible = true;
            btnDown.ImageUrl = DownButtonImageUrl;
            btnDown.AlternateText = CMSContext.CurrentResolver.ResolveMacros(DownButtonImageAlternateText);
            btnDown.ImageAlign = ImageAlign.Middle;
            exNumeric.TargetButtonDownID = btnDown.ID;
        }

        exNumeric.ServiceDownMethod = ServiceDownMethod;
        exNumeric.ServiceDownPath = ServiceDownPath;
        exNumeric.ServiceUpMethod = ServiceUpMethod;
        exNumeric.ServiceUpPath = ServiceUpPath;
        exNumeric.Tag = CMSContext.CurrentResolver.ResolveMacros(Tag);

        // Apply CSS styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            pnlContainer.CssClass = CssClass;
            CssClass = null;
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            pnlContainer.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (!Enabled)
        {
            DisableUpDown();
        }
    }


    /// <summary>
    /// Makes NumericUpDown extender read-only by removing javascript event handlers from buttons.
    /// </summary>
    private void DisableUpDown()
    {
        ScriptHelper.RegisterJQuery(Page);

        // Remove all javascript handlers from up and down buttons to make control read-only
        StringBuilder script = new StringBuilder();

        // Bind function to AJAX life cycle event
        // NumericUpDown extender elements are not created earlier
        script.Append(@"
Sys.Application.add_load(function (){
    $clearHandlers($j('#", textbox.ClientID, @"_bUp')[0]);
    $clearHandlers($j('#", textbox.ClientID, @"_bDown')[0]);
});");

        ScriptHelper.RegisterStartupScript(this, typeof(string), ClientID + "_disableUpDown", ScriptHelper.GetScript(script.ToString()));
    }

    #endregion
}