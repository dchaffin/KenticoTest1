using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.ExtendedControls;
using CMS.EventLog;
using CMS.SocialNetworking;

public partial class CMSFormControls_Documents_SocialMediaPublishing_LinkedInAccessToken : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets enabled state.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return txtToken.Enabled;
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
            return txtToken.Text;
        }
        set
        {
            txtToken.Text = value.ToString();
        }
    }


    /// <summary>
    /// Gets ClientID of the control from which the Value is retrieved or 
    /// null if such a control can't be specified.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtToken.ClientID;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string apiKeyCid = null, apiSecretCid = null;

        // Try to get values from other controls
        try
        {
            var controls = this.Parent.Parent.Controls;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] is Label)
                {
                    Label labelLI = (Label)controls[i];

                    // Try to get api key client ID
                    if (labelLI.Text.EqualsCSafe(ResHelper.GetString("settingskey.cmslinkedinapikey")))
                    {
                        apiKeyCid = ((CMSTextBox)controls[i + 6].Controls[0]).ClientID;
                    }

                    // Try to get application secret client ID
                    if (labelLI.Text.EqualsCSafe(ResHelper.GetString("settingskey.cmslinkedinapplicationsecret")))
                    {
                        apiSecretCid = ((CMSTextBox)controls[i + 6].Controls[0]).ClientID;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Something went wrong - log exception
            EventLogProvider ev = new EventLogProvider();
            ev.LogEvent("LinkedIn", "ACCESSTOKEN", ex);

            // Set message
            txtToken.Visible = false;
            btnSelect.Visible = false;

            return;
        }

        this.btnSelect.OnClientClick = "tagSelect('" + txtToken.ClientID + "', '" + apiKeyCid + "', '" + apiSecretCid + "'); return false;";

        // Build script with modal dialog opener and set textbox value functions
        string scriptString = @"
function tagSelect(id, apikeycid, apisecretcid) {{
    var txtToken = document.getElementById(id);
    var apiKey = document.getElementById(apikeycid).value;
    var apiSecret = document.getElementById(apisecretcid).value;
    if (txtToken != null) {{
        modalDialog('{0}?txtToken=' + id + '&apiKey=' + apiKey + '&apiSecret=' + apiSecret, 'LinkedInAccessToken', '550', '200', null, null, null, true);
    }}
}}
function setAccessTokenToTextBox(textBoxId, accessTokenString, accessTokenSecretString) {{
    if (textBoxId != '') {{
        var textbox = document.getElementById(textBoxId);
        if (textbox != null) {{
            textbox.value = accessTokenString + ';' + accessTokenSecretString;
        }}
    }}
}}";

        StringBuilder builder = new StringBuilder();    
        builder.AppendFormat(scriptString, URLHelper.GetAbsoluteUrl(LinkedInProvider.ACCESS_TOKEN_PAGE));

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this.Page);

        // Register tag script 
        ScriptHelper.RegisterStartupScript(this, typeof(string), "accessTokenScript", ScriptHelper.GetScript(builder.ToString()));
    }

    #endregion
}