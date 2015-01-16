using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.ExtendedControls;

public partial class CMSFormControls_Documents_SocialMediaPublishing_GooglePlusAccessToken : FormEngineUserControl
{
    #region "Methods"
    
    protected void Page_Load(object sender, EventArgs e)
    {
        string clientID = null, clientSecret = null;

        // Try to get values from other controls
        for (int i = 0; i < this.Parent.Parent.Controls.Count; i++)
        {
            var control = this.Parent.Parent.Controls[i];
            if (control is Label)
            {
                // Try to get client ID
                if (((Label)control).Text.EqualsCSafe(ResHelper.GetString("SocialNetworking.GooglePlus.ClientID")))
                {
                    clientID = ((CMSTextBox)this.Parent.Parent.Controls[i + 6].Controls[0]).ClientID;
                }

                // Try to get client secret
                if (((Label)control).Text.EqualsCSafe(ResHelper.GetString("SocialNetworking.GooglePlus.ClientSecret")))
                {
                    clientSecret = ((CMSTextBox)this.Parent.Parent.Controls[i + 6].Controls[0]).ClientID;
                }
            }
        }

        this.btnSelect.OnClientClick = "tagSelect('" + this.txtToken.ClientID + "', '" + this.txtTokenSecret.ClientID + "', '" + clientID + "', '" + clientSecret + "'); return false;";

        // Build script with modal dialog opener and set textbox value functions
        StringBuilder builder = new StringBuilder();
        builder.Append("function tagSelect(id, idsecret, clientidcid, clientsecretcid){");
        builder.Append("    var txtToken = document.getElementById(id);");
        builder.Append("    var txtTokenSecret = document.getElementById(idsecret);");
        builder.Append("    var clientid = document.getElementById(clientidcid).value;");
        builder.Append("    var clientsecret = document.getElementById(clientsecretcid).value;");
        builder.Append("    if (txtToken != null){");
        builder.Append("        modalDialog('" + URLHelper.GetAbsoluteUrl("~/CMSFormControls/Documents/SocialMediaPublishing/GooglePlusAccessTokenPage.aspx?txtToken=") + "'+ id + '&txtTokenSecret=' + idsecret + '&token_client_id=' + clientid + '&token_client_secret=' + clientsecret, 'GooglePlusAccessToken', 550, 400, null, null, null, true);");
        builder.Append("    }");
        builder.Append("}");
        builder.Append("function setAccessTokenToTextBox(textBoxId, accessTokenString, txtTokenSecret, accessTokenSecretString){");
        builder.Append("    if (textBoxId != '') {");
        builder.Append("        var textbox = document.getElementById(textBoxId);");
        builder.Append("        if (textbox != null){");
        builder.Append("            textbox.value = accessTokenString;");
        builder.Append("        }");
        builder.Append("        var textbox = document.getElementById(txtTokenSecret);");
        builder.Append("        if (textbox != null){");
        builder.Append("            textbox.value = accessTokenSecretString;");
        builder.Append("        }");
        builder.Append("    }");
        builder.Append("}");

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this.Page);

        // Register tag script 
        ScriptHelper.RegisterStartupScript(this, typeof(string), "accessTokenScript", ScriptHelper.GetScript(builder.ToString()));
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets enabled state.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return btnSelect.Enabled;
        }
        set
        {
            txtToken.Enabled = value;
            btnSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value of access token.
    /// </summary>
    public override object Value
    {
        get
        {
            if (!String.IsNullOrEmpty(txtTokenSecret.Text))
            {
                return txtToken.Text + ";" + txtTokenSecret.Text;
            }
            else
            {
                return String.Empty;
            }
        }
        set
        {
            string stringValue = value.ToString();
            if (!String.IsNullOrEmpty(stringValue))
            {
                txtToken.Text = stringValue.Substring(0, stringValue.IndexOfCSafe(";"));
            }
        }
    }

    #endregion
}