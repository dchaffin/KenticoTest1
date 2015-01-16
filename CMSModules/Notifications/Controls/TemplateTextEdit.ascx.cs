using System;
using System.Data;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.Notifications;

public partial class CMSModules_Notifications_Controls_TemplateTextEdit : TemplateTextEdit
{
    #region "Variables"

    public string noneStyle = string.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Sets/Gets TemplateSubject textbox field.
    /// </summary>
    public override string TemplateSubject
    {
        get
        {
            return DataHelper.GetNotEmpty(txtSubject.Text, "");
        }
        set
        {
            txtSubject.Text = value;
        }
    }


    /// <summary>
    /// Sets/Gets TemplatePlainText textarea field.
    /// </summary>
    public override string TemplatePlainText
    {
        get
        {
            return DataHelper.GetNotEmpty(txtPlainText.Text, "");
        }
        set
        {
            txtPlainText.Text = value;
        }
    }


    /// <summary>
    /// Sets/Gets TemplateHTMLText textarea field.
    /// </summary>
    public override string TemplateHTMLText
    {
        get
        {
            return DataHelper.GetNotEmpty(htmlText.ResolvedValue, "");
        }
        set
        {
            htmlText.Value = value;
        }
    }

    #endregion


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // If ID´s not specified return 
        if ((TemplateID == 0) || (GatewayID == 0))
        {
            return;
        }

        // Get resource strings  
        lblHTMLText.ResourceString = "notification.template.html";
        lblPlainText.ResourceString = "notification.template.plain";
        lblSubject.ResourceString = "general.subject";

        // Get gateway name
        NotificationGatewayInfo ngi = NotificationGatewayInfoProvider.GetNotificationGatewayInfo(GatewayID);
        if (ngi == null)
        {
            throw new Exception("NotificationGatewayInfo with this GatewayID does not exist.");
        }

        // Setup control according to NotificationGatewayInfo
        plcSubject.Visible = ngi.GatewaySupportsEmail;
        plcPlainText.Visible = ngi.GatewaySupportsPlainText;
        plcHTMLText.Visible = ngi.GatewaySupportsHTMLText;

        if (plcHTMLText.Visible)
        {
            // Initialize HTML editor
            htmlText.AutoDetectLanguage = false;
            htmlText.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            htmlText.EditorAreaCSS = FormHelper.GetHtmlEditorAreaCss(CMSContext.CurrentSiteName);
            htmlText.ToolbarSet = "Basic";
            htmlText.MediaDialogConfig.UseFullURL = true;
            htmlText.LinkDialogConfig.UseFullURL = true;
            htmlText.QuickInsertConfig.UseFullURL = true;
        }

        // If gateway does not support any of text fields inform about it. 
        if (!ngi.GatewaySupportsEmail && !ngi.GatewaySupportsHTMLText && !ngi.GatewaySupportsPlainText)
        {
            noneStyle = "width:100%";
            plcNoTextbox.Visible = true;
            lblNoTextbox.Text = string.Format(GetString("notifications.templatetext.notextbox"), HTMLHelper.HTMLEncode(ngi.GatewayDisplayName));
        }

        // Get existing TemplateTextInfoObject or create new object
        NotificationTemplateTextInfo ntti = NotificationTemplateTextInfoProvider.GetNotificationTemplateTextInfo(GatewayID, TemplateID);
        if (ntti == null)
        {
            ntti = new NotificationTemplateTextInfo();
        }

        // Setup properties
        if (!URLHelper.IsPostback())
        {
            TemplateSubject = ntti.TemplateSubject;
            TemplateHTMLText = ntti.TemplateHTMLText;
            TemplatePlainText = ntti.TemplatePlainText;
        }
    }

    #endregion
}