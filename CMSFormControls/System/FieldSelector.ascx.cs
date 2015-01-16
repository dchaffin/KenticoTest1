using System;

using CMS.FormControls;
using CMS.GlobalHelper;

public partial class CMSFormControls_System_FieldSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Class name
    /// </summary>
    public string ClassName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ClassName"), null);
        }
        set
        {
            SetValue("ClassName", value);
        }
    }


    /// <summary>
    /// Class type
    /// </summary>
    /// <remarks>0 - Custom tables, 1 - Document types, 2 - System tables</remarks>
    public int ClassType
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ClassType"), 1);
        }
        set
        {
            SetValue("ClassType", value);
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
            txtField.Enabled = value;
            btnClear.Enabled = value;
            btnSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (hdnField.Value.Length > 400)
            {
                return hdnField.Value.Substring(0, 400);
            }
            else
            {
                return hdnField.Value;
            }
        }
        set
        {
            if (value != null)
            {
                string stringValue = ValidationHelper.GetString(value, string.Empty);
                string[] valueAndText = stringValue.Split('|');
                if (valueAndText.Length == 2)
                {
                    txtField.Text = valueAndText[1];
                }
                hdnField.Value = stringValue;
            }
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize dialog script
        ScriptHelper.RegisterDialogScript(Page);
        string url = "~/CMSFormControls/Selectors/FieldSelection.aspx";
        if (!string.IsNullOrEmpty(ClassName))
        {
            url = URLHelper.AddParameterToUrl(url, "classname", ClassName);
        }
        if (ClassType != 1)
        {
            url = URLHelper.AddParameterToUrl(url, "classtype", ClassType.ToString());
        }
        url = URLHelper.AddParameterToUrl(url, "cid", ClientID);
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));

        ScriptHelper.RegisterStartupScript(this, typeof(string), "selectFields_" + ClientID, ScriptHelper.GetScript(
@"function SetValue_" + ClientID + @"(value, text) {
        document.getElementById('" + txtField.ClientID + @"').value = text;
        document.getElementById('" + hdnField.ClientID + @"').value = value + '|' + text;
    }"));

        btnSelect.OnClientClick = "modalDialog('" + ResolveUrl(url) + "','fieldSelector', 430, 170); return false;";

        // Initialize clear button
        btnClear.OnClientClick = "document.getElementById('" + txtField.ClientID + "').value = ''; document.getElementById('" + hdnField.ClientID + "').value='';return false;";
        // Disable editing of textbox
        txtField.Attributes.Add("readonly", "readonly");
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }

    #endregion
}