using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.EmailEngine;
using CMS.UIControls;
using CMS.GlobalHelper;

// Set title
[Title("Objects/CMS_EmailTemplate/object.png", "EmailTemplate_Edit.Title", "newedit_e-mail_template")]
public partial class CMSModules_EmailTemplates_Pages_Header : CMSEmailTemplatesPage
{
    #region "Variables"

    private bool mDialog = false;

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        mDialog = QueryHelper.GetBoolean("editonlycode", false);
        if (mDialog)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Prepare site query
        string siteQuery = null;
        if (SiteID > 0)
        {
            siteQuery = "?siteid=" + SiteID;
        }

        if (SelectedSiteID > 0)
        {
            siteQuery = "?selectedsiteid=" + SelectedSiteID;
        }

        EmailTemplateInfo templateInfo = null;
        int templateId = QueryHelper.GetInteger("templateid", 0);
        if (templateId > 0)
        {
            templateInfo = EmailTemplateProvider.GetEmailTemplate(templateId);
        }
        else
        {
            string templateName = QueryHelper.GetString("name", String.Empty);
            templateInfo = EmailTemplateProvider.GetEmailTemplate(templateName, SiteID);
            if (templateInfo != null)
            {
                templateId = templateInfo.TemplateID;
            }
        }

        SetEditedObject(templateInfo, "Frameset.aspx");

        // Check that edited template belongs to selected site
        if ((SiteID > 0) && (templateInfo != null) && (templateInfo.TemplateSiteID != SiteID))
        {
            RedirectToAccessDenied(null);
        }

        if (!mDialog)
        {
            // Set Breadcrumbs
            CurrentMaster.Title.Breadcrumbs = new string[2, 3];
            SetBreadcrumb(0, GetString("EmailTemplate_Edit.EmailTemplates"), ResolveUrl("List.aspx" + siteQuery), "_parent", null);
            if (templateInfo != null)
            {
                SetBreadcrumb(1, templateInfo.TemplateDisplayName, null, null, null);
            }
        }
        else
        {
            RegisterModalPageScripts();
        }

        InitTabs("content");

        // Build general tab url
        StringBuilder url = new StringBuilder();
        url.Append("Edit.aspx", String.IsNullOrEmpty(siteQuery) ? "?" : siteQuery + "&", "tabmode=1");
        if (templateInfo != null)
        {
            url.Append("&templateid=", templateInfo.TemplateID);
        }
        if (mDialog)
        {
            url.Append("&editonlycode=1");

            string type = QueryHelper.GetString("templatetype", null);
            if (!String.IsNullOrEmpty(type))
            {
                type = "&templatetype=" + type;
                url.Append(type);
            }

            url.Append("&hash=", QueryHelper.GetHash("?editonlycode=1" + type));
        }
        string selector = QueryHelper.GetString("selectorid", null);
        if (!String.IsNullOrEmpty(selector))
        {
            url.Append("&selectorid=", selector);
        }

        SetTab(0, GetString("general.general"), url.ToString(), "SetHelpTopic('helpTopic', 'newedit_e-mail_template')");
    }
}