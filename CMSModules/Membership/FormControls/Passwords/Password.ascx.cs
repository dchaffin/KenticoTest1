using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.SettingsProvider;

public partial class CMSModules_Membership_FormControls_Passwords_Password : FormEngineUserControl
{
    #region "Constants"

    const string hiddenPassword = "********";

    #endregion


    #region "Properties"

    /// <summary>
    /// Crypted password.
    /// </summary>
    private string CryptedPassword
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CryptedPassword"], string.Empty);
        }
        set
        {
            ViewState["CryptedPassword"] = value;
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
            txtPassword.Enabled = value;
        }
    }


    /// <summary>
    /// Returns encrypted password.
    /// </summary>
    public override object Value
    {
        get
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                txtPassword.Attributes.Add("value", string.Empty);
                return string.Empty;
            }

            if (txtPassword.Text == hiddenPassword)
            {
                return CryptedPassword;
            }

            // Get salt
            string salt = null;
            string format = SettingsKeyProvider.GetStringValue("CMSPasswordFormat");
            UserInfo ui = Form.EditedObject as UserInfo;
            if (ui != null)
            {
                salt = ui.UserGUID.ToString();
                format = ui.PasswordFormat;
            }

            txtPassword.Attributes.Add("value", hiddenPassword);
            return UserInfoProvider.GetPasswordHash(txtPassword.Text, format, salt);
        }
        set
        {
            CryptedPassword = ValidationHelper.GetString(value, string.Empty);

            if (!string.IsNullOrEmpty(CryptedPassword))
            {
                txtPassword.Attributes.Add("value", hiddenPassword);
            }
            else
            {
                txtPassword.Attributes.Add("value", string.Empty);
            }
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Apply CSS styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            txtPassword.CssClass = CssClass;
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(txtPassword.CssClass))
        {
            // Set automatic class to the textbox
            txtPassword.CssClass = "TextBoxField";
        }

        if (!String.IsNullOrEmpty(ControlStyle))
        {
            txtPassword.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // Check regular expression
        if ((!String.IsNullOrEmpty(FieldInfo.RegularExpression)) && (new Validator().IsRegularExp(txtPassword.Text, FieldInfo.RegularExpression, "error").Result == "error"))
        {
            ValidationError = GetString("PassConfirmator.InvalidPassword");
            return false;
        }

        // Check min length
        if ((FieldInfo.MinStringLength > 0) && (txtPassword.Text.Length < FieldInfo.MinStringLength))
        {
            ValidationError = String.Format(GetString("PassConfirmator.PasswordLength"), FieldInfo.MinStringLength);
            return false;
        }

        // Check max length
        if ((FieldInfo.MaxStringLength > 0) && (txtPassword.Text.Length > FieldInfo.MaxStringLength))
        {
            ValidationError = String.Format(GetString("basicform.errortexttoolong"));
            return false;
        }

        return true;
    }
}