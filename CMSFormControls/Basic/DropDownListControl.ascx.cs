using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;

public partial class CMSFormControls_Basic_DropDownListControl : FormEngineUserControl
{
    #region "Variables"

    private string selectedValue = null;
    private bool? mEditText;
    private bool mFirstAsDefault = true;
    private bool valueSet = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return dropDownList.Enabled;
        }
        set
        {
            dropDownList.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (EditText)
            {
                // Try to find value in ddlist
                ListItem selectedItem = dropDownList.Items.FindByText(txtCombo.Text);
                if (selectedItem == null)
                {
                    // Return value from combo box
                    return txtCombo.Text;
                }
            }
            // Return selected value
            return dropDownList.SelectedValue;
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

                selectedValue = ValidationHelper.GetString(convertedValue, null);

                LoadAndSelectList();

                if (EditText)
                {
                    if ((dropDownList.SelectedValue != null) && (selectedValue == dropDownList.SelectedValue))
                    {
                        // Set value from ddlist
                        txtCombo.Text = dropDownList.SelectedItem.Text;
                    }
                    else
                    {
                        // Set value that is not in ddlist
                        txtCombo.Text = selectedValue;
                    }
                    
                }
                else
                {
                    dropDownList.SelectedValue = selectedValue;
                }

                // Indicates that a value was set from outside
                valueSet = true;
            }
        }
    }


    /// <summary>
    /// Returns display name of the value.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return (EditText || (dropDownList.SelectedItem == null) ? txtCombo.Text : dropDownList.SelectedItem.Text);
        }
    }


    /// <summary>
    /// Gets or sets selected value.
    /// </summary>
    public string SelectedValue
    {
        get
        {
            if (EditText)
            {
                return txtCombo.Text;
            }
            else
            {
                return dropDownList.SelectedValue;
            }
        }
        set
        {
            if (EditText)
            {
                txtCombo.Text = value;
            }
            else
            {
                dropDownList.SelectedValue = value;
            }

            // Indicates that a value was set from outside
            valueSet = true;
        }
    }


    /// <summary>
    /// Gets or sets selected index. Returns -1 if no element is selected.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            if (EditText)
            {
                if (dropDownList.Items.FindByValue(txtCombo.Text) != null)
                {
                    return dropDownList.SelectedIndex;
                }
                return -1;
            }
            else
            {
                return dropDownList.SelectedIndex;
            }
        }
        set
        {
            dropDownList.SelectedIndex = value;
            if (EditText)
            {
                txtCombo.Text = dropDownList.SelectedValue;
            }
        }
    }


    /// <summary>
    /// Enables to edit text from textbox and select values from dropdownlist.
    /// </summary>
    public bool EditText
    {
        get
        {
            return mEditText ?? ValidationHelper.GetBoolean(GetValue("edittext"), false);
        }
        set
        {
            mEditText = value;
        }
    }


    /// <summary>
    /// Gets dropdown list control.
    /// </summary>
    public DropDownList DropDownList
    {
        get
        {
            return dropDownList;
        }
    }


    /// <summary>
    /// Gets textbox control.
    /// </summary>
    public TextBox TextBoxControl
    {
        get
        {
            return txtCombo;
        }
    }


    /// <summary>
    /// Indicates whether or not to use first value as default if default value is empty.
    /// </summary>
    public bool FirstAsDefault 
    { 
        get
        {
            return mFirstAsDefault;
        }
        set
        {
            mFirstAsDefault = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadAndSelectList();

        // Apply CSS class
        if (!String.IsNullOrEmpty(CssClass))
        {
            dropDownList.CssClass = CssClass;
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(dropDownList.CssClass))
        {
            dropDownList.CssClass = "DropDownField";
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            dropDownList.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;

        if (EditText)
        {
            if (!RequestHelper.IsPostBack() && !valueSet && FirstAsDefault && dropDownList.SelectedItem != null)
            {
                txtCombo.Text = dropDownList.SelectedItem.Text;
            }
            txtCombo.Visible = true;
            dropDownList.Attributes.Add("style", "display: none");

            ScriptHelper.RegisterJQueryUI(Page);
            ScriptHelper.RegisterScriptFile(Page, "jquery/jquery-combobox.js");
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "HideList_" + ClientID, ScriptHelper.GetScript(
@"
jQuery(function() {
    jQuery(""#" + DropDownList.ClientID + @""").combobox();
});
"
            ));
        }
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if (EditText)
        {
            writer.Write("<div class=\"ComboBox\">");

            base.Render(writer);

            writer.Write("</div>");
        }
        else
        {
            base.Render(writer);
        }
    }


    /// <summary>
    /// Loads and selects control.
    /// </summary>
    private void LoadAndSelectList()
    {
        if (dropDownList.Items.Count == 0)
        {
            string options = ValidationHelper.GetString(GetValue("options"), null);
            string query = ValidationHelper.GetString(GetValue("query"), null);
            
            try
            {
                FormHelper.LoadItemsIntoList(options, query, dropDownList.Items, FieldInfo);
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }

            FormHelper.SelectSingleValue(selectedValue, dropDownList);
        }
    }


    /// <summary>
    /// Displays exception control with current error.
    /// </summary>
    /// <param name="ex">Thrown exception</param>
    private void DisplayException(Exception ex)
    {
        FormControlError ctrlError = new FormControlError();
        ctrlError.InnerException = ex;
        Controls.Add(ctrlError);
        dropDownList.Visible = false;
    }

    #endregion
}