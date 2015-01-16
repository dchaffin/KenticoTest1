using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSFormControls_Inputs_USphone : FormEngineUserControl
{
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

            txt1st.Enabled = value;
            txt2nd.Enabled = value;
            txt3rd.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (IsEmpty())
            {
                return "";
            }

            return "(" + txt1st.Text + ") " + txt2nd.Text + "-" + txt3rd.Text;
        }
        set
        {
            string number = (string)value;
            Clear();

            // Parse numbers from incoming string.
            if ((number != null) && (number != ""))
            {
                try
                {
                    txt1st.Text = number.Substring(1, number.IndexOfCSafe(")") - 1);
                    txt2nd.Text = number.Substring(number.IndexOfCSafe(")") + 2, number.IndexOfCSafe("-") - number.IndexOfCSafe(")") - 2);
                    txt3rd.Text = number.Substring(number.IndexOfCSafe("-") + 1, number.Length - number.IndexOfCSafe("-") - 1);
                }
                catch
                {
                }
            }
        }
    }


    /// <summary>
    /// Clears current value.
    /// </summary>
    public void Clear()
    {
        txt1st.Text = "";
        txt2nd.Text = "";
        txt3rd.Text = "";
    }


    /// <summary>
    /// Returns true if the number is empty.
    /// </summary>
    public bool IsEmpty()
    {
        return (DataHelper.IsEmpty(txt1st.Text) && DataHelper.IsEmpty(txt2nd.Text) && DataHelper.IsEmpty(txt3rd.Text));
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (IsEmpty())
        {
            return true;
        }

        // US phone number must be in form: (ddd) ddd-dddd, where 'd' is digit
        Validator val = new Validator();
        string result = val.IsRegularExp(txt1st.Text, @"\d{3}", "error").IsRegularExp(txt2nd.Text, @"\d{3}", "error").IsRegularExp(txt3rd.Text, @"\d{4}", "error").Result;

        if (result != "")
        {
            ValidationError = GetString("USPhone.ValidationError");
            return false;
        }
        return true;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // WAI validation
        lbl2nd.Attributes.Add("style", "display:none;");
        lbl3rd.Attributes.Add("style", "display:none;");
    }
}