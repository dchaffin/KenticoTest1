using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;

using CMS.FormControls;
using CMS.SocialNetworking;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.ExtendedControls;
using CMS.CMSHelper;
using CMS.EventLog;

public partial class CMSFormControls_Documents_SocialMediaPublishing_FacebookAccessToken : FormEngineUserControl
{
    #region "Constants"

    /// <summary>
    /// Location of value control in form.
    /// </summary>
    private const int CONTROL_FORM_LOC = 6;


    /// <summary>
    /// Location of the value control in placeholder.
    /// </summary>
    private const int CONTROL_PLC_LOC = 0;

    #endregion


    #region "Private variables"

    DateTime tokenExpiration;

    #endregion


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
            if (String.IsNullOrEmpty(txtToken.Text))
            {
                lblMessage.Text = String.Empty;
                return String.Empty;
            }

            return txtToken.Text;
        }
        set
        {
            string stringValue = value.ToString();
            if (!String.IsNullOrEmpty(stringValue))
            {
                txtToken.Text = stringValue;
            }
            else
            {
                txtToken.Text = String.Empty;
                lblMessage.Text = String.Empty;
            }
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
        string apiKeyCid = null, apiSecretCid = null, pageIdCid = null;

        // Try to get values from other controls
        try
        {
            var controls = this.Parent.Parent.Controls;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] is Label)
                {
                    Label labelFB = (Label)controls[i];

                    // Try to get api key CID
                    if (labelFB.Text.EqualsCSafe(ResHelper.GetString("settingskey.cmsfacebookconnectapikey")))
                    {
                        apiKeyCid = ((CMSTextBox)controls[i + CONTROL_FORM_LOC].Controls[CONTROL_PLC_LOC]).ClientID;
                    }

                    // Try to get application secret CID
                    if (labelFB.Text.EqualsCSafe(ResHelper.GetString("settingskey.cmsfacebookapplicationsecret")))
                    {
                        apiSecretCid = ((CMSTextBox)controls[i + CONTROL_FORM_LOC].Controls[CONTROL_PLC_LOC]).ClientID;
                    }

                    // Try to get Facebook page ID CID
                    if (labelFB.Text.EqualsCSafe(ResHelper.GetString("settingskey.facebookpageid")))
                    {
                        pageIdCid = ((CMSTextBox)controls[i + CONTROL_FORM_LOC].Controls[CONTROL_PLC_LOC]).ClientID;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Something went wrong - log exception
            EventLogProvider ev = new EventLogProvider();
            ev.LogEvent("Facebook", "ACCESSTOKEN", ex);

            // Set message
            txtToken.Visible = false;
            btnSelect.Visible = false;
            lblMessage.Text = String.Empty;
            lblError.Text = (GetString("socialnetworking.facebook.wrongcontrollocation"));
            lblError.Style["display"] = "block";

            return;
        }

        btnSelect.OnClientClick = String.Format("tagSelect('{0}', '{1}', '{2}', '{3}', '{4}'); return false;", txtToken.ClientID, apiKeyCid, apiSecretCid, pageIdCid, lblMessage.ClientID);

        // Build script with modal dialog opener and set textbox value functions
        StringBuilder builder = new StringBuilder();

        string scriptString = @"
function fbShowError(msg){{
    var label = jQuery('#{2}');
    label.text(msg);
    label.show();
}}
function tagSelect(id, apiKeyId, apiSecretId, pageIdId, lblId) {{
    var textbox = jQuery('#' + id);
    var label = jQuery('#' + lblId);
    var apiKey = (apiKeyId != null) ? jQuery('#' + apiKeyId).val() : '';
    var apiSecret = (apiSecretId != null) ? jQuery('#' + apiSecretId).val() : '';
    var pageId = (pageIdId != null) ? jQuery('#' + pageIdId).val() : '';
    
    if ((textbox != null) && (label != null)) {{
        if (apiKey.length == 0)
        {{
            fbShowError('{3}');  
            return;
        }}
        if (apiSecret.length == 0)
        {{
            fbShowError('{4}');  
            return;
        }}
        if (pageId.length == 0)
        {{
            fbShowError('{5}');  
            return;
        }}
        
        jQuery('#{2}').hide();
        modalDialog('{0}?textbox=' + id + '&apiKey=' + apiKey + '&appSecret=' + apiSecret + '&pageId=' + pageId + '&lblId=' + lblId, 'FacebookAccessToken', 980, 560, null, null, null, true);
    }}
}}
function setAccessTokenToTextBox(textBoxId, accessTokenString, tokenExpiration, lblId, expRaw) {{
    if (textBoxId != '') {{
        var textbox = document.getElementById(textBoxId);
        if (textbox != null) {{
            textbox.value = accessTokenString + ';' + expRaw;
            var lblString = '{1}';
            var span = document.getElementById(lblId), text = '';
            if (tokenExpiration != '') {{
                text = document.createTextNode(lblString.replace('{{0}}', tokenExpiration));
            }}
            else {{
                text = document.createTextNode('');
            }}
            span.innerHTML = '';
            span.appendChild(text);
        }}
    }}
}}";

        builder.AppendFormat(scriptString,
            URLHelper.GetAbsoluteUrl(FacebookProvider.REDIRECT_URL),
            GetString("socialnetworking.facebook.tokenwillexpire"),
            lblError.ClientID,
            GetString("mem.facebook.enterapikey"),
            GetString("mem.facebook.enterappsecret"),
            GetString("mem.facebook.enterpageid")
            );

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this.Page);

        // Register tag script 
        ScriptHelper.RegisterStartupScript(this, typeof(string), "accessTokenScript", ScriptHelper.GetScript(builder.ToString()));
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Set token expiration
        tokenExpiration = FacebookProvider.GetAccessTokenExpirationFromString(txtToken.Text);

        // Set token expiration message if token seems to be valid
        if ((tokenExpiration != DateTime.MinValue) && !String.IsNullOrEmpty(txtToken.Text))
        {
            if (tokenExpiration <= DateTime.Now)
            {
                lblMessage.Text = GetString("socialnetworking.facebook.tokenexpired");
            }
            else
            {
                lblMessage.Text = String.Format(GetString("socialnetworking.facebook.tokenwillexpire"), tokenExpiration.ToString("g", CultureHelper.PreferredUICultureInfo));
            }
        }
    }

    #endregion
}
