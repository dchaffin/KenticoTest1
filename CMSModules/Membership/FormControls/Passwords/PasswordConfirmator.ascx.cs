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
using CMS.SiteProvider;
using CMS.SettingsProvider;

public partial class CMSModules_Membership_FormControls_Passwords_PasswordConfirmator : FormEngineUserControl
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
            txtConfirmPassword.Enabled = value;
            passStrength.Enabled = value;
        }
    }


    /// <summary>
    /// Returns encrypted password.
    /// </summary>
    public override object Value
    {
        get
        {
            // Check if text is set
            if (string.IsNullOrEmpty(passStrength.Text))
            {
                passStrength.TextBoxAttributes.Add("value", string.Empty);
                txtConfirmPassword.Attributes.Add("value", string.Empty);
                return string.Empty;
            }

            // Check if password changed
            if (passStrength.Text == hiddenPassword)
            {
                return CryptedPassword;
            }

            // Get salt and format
            string salt = null;
            string format = SettingsKeyProvider.GetStringValue("CMSPasswordFormat");
            UserInfo ui = Form.EditedObject as UserInfo;
            if (ui != null)
            {
                salt = ui.UserGUID.ToString();
                format = ui.PasswordFormat;
            }

            passStrength.TextBoxAttributes.Add("value", hiddenPassword);
            txtConfirmPassword.Attributes.Add("value", hiddenPassword);

            CryptedPassword = UserInfoProvider.GetPasswordHash(passStrength.Text, format, salt);
            return CryptedPassword;
        }
        set
        {
            CryptedPassword = ValidationHelper.GetString(value, string.Empty);

            if (!string.IsNullOrEmpty(CryptedPassword))
            {
                passStrength.TextBoxAttributes.Add("value", hiddenPassword);
                txtConfirmPassword.Attributes.Add("value", hiddenPassword);
            }
            else
            {
                passStrength.TextBoxAttributes.Add("value", string.Empty);
                txtConfirmPassword.Attributes.Add("value", string.Empty);
            }
        }
    }

    #endregion


    #region "Page events" 

    /// <summary>
    /// Page load event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        passStrength.ShowStrengthIndicator = ValidationHelper.GetBoolean(GetValue("showstrength"), true);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (passStrength.Text == hiddenPassword)
        {
            return true;
        }

        if (passStrength.Text != txtConfirmPassword.Text)
        {
            ValidationError = GetString("PassConfirmator.PasswordDoNotMatch");
            return false;
        }

        // Check regular expresion
        if ((!String.IsNullOrEmpty(FieldInfo.RegularExpression)) && (new Validator().IsRegularExp(passStrength.Text, FieldInfo.RegularExpression, "error").Result == "error"))
        {
            ValidationError = GetString("PassConfirmator.InvalidPassword");
            return false;
        }

        // Check min lenght
        if ((FieldInfo.MinStringLength > 0) && (passStrength.Text.Length < FieldInfo.MinStringLength))
        {
            ValidationError = string.Format(GetString("PassConfirmator.PasswordLength"), FieldInfo.MinStringLength);
            return false;
        }

        // Check password policy
        if (!passStrength.IsValid())
        {
            ValidationError = AuthenticationHelper.GetPolicyViolationMessage(CMSContext.CurrentSiteName);
            return false;
        }

        return true;
    }

    #endregion
}