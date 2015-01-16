using System;

using CMS.CMSHelper;
using CMS.EmailEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

public partial class CMSModules_EmailTemplates_Pages_Edit : CMSEmailTemplatesPage
{
    #region "Variables"

    private bool mDialog = false;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        mDialog = QueryHelper.GetBoolean("editonlycode", false);
        if (mDialog)
        {
            // Check hash
            if (!QueryHelper.ValidateHash("hash", "saved;name;templateid;selectorid;tabmode;siteid;selectedsiteid", true, true, true))
            {
                URLHelper.Redirect(ResolveUrl(String.Format("~/CMSMessages/Error.aspx?title={0}&text={1}", GetString("dialogs.badhashtitle"), GetString("dialogs.badhashtext"))));
            }

            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
            CurrentMaster.PanelContent.CssClass = "PageContent";
        }

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initializes page title breadcrumbs
        int siteId = SiteID;
        string emailTemplateListUrl = "~/CMSModules/EmailTemplates/Pages/List.aspx";
        if (siteId != 0)
        {
            emailTemplateListUrl += "?siteid=" + siteId;
        }
        else
        {
            siteId = SelectedSiteID;
            emailTemplateListUrl += "?selectedsiteid=" + SelectedSiteID;
        }

        // Get edited template
        EmailTemplateInfo templateInfo = null;

        int templateId = QueryHelper.GetInteger("templateid", 0);
        if (templateId > 0)
        {
            templateInfo = EmailTemplateProvider.GetEmailTemplate(templateId);
        }
        else
        {
            string templateName = QueryHelper.GetString("name", String.Empty);
            templateInfo = EmailTemplateProvider.GetEmailTemplate(templateName, siteId);
            if (templateInfo != null)
            {
                templateId = templateInfo.TemplateID;
            }
        }

        if (templateInfo != null)
        {
            // Check that edited template belongs to selected site
            if ((siteId > 0) && (templateInfo != null) && (templateInfo.TemplateSiteID != siteId))
            {
                RedirectToAccessDenied(null);
            }

            SetEditedObject(templateInfo, "Frameset.aspx");


            CurrentMaster.Title.TitleText = GetString("EmailTemplate_Edit.Title");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_EmailTemplate/object.png");
        }
        else
        {
            CurrentMaster.Title.TitleText = GetString("EmailTemplate_Edit.TitleNew");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_EmailTemplate/new.png");
        }

        // Initialize the editing control
        emailTemplateEditElem.EmailTemplateID = templateId;
        emailTemplateEditElem.DialogMode = mDialog;

        if ((siteId == 0) && CMSContext.CurrentUser.UserSiteManagerAdmin)
        {
            emailTemplateEditElem.GlobalTemplate = true;
        }

        emailTemplateEditElem.SiteID = siteId;
        emailTemplateEditElem.OnCheckPermissions += emailTemplateEditElem_OnCheckPermissions;

        if (mDialog)
        {
            // Make sure user can't change template type to some that is not allowed in selector
            string templateType = QueryHelper.GetString("templatetype", null);
            if (!String.IsNullOrEmpty(templateType))
            {
                emailTemplateEditElem.TemplateType = EmailTemplateMacros.GetEmailTypeEnum(templateType);
            }

            // Other dialog specific initialization
            RegisterModalPageScripts();
            CreateFooterButtons();
        }
        else
        {
            string[,] pageTitleTabs = new string[2, 3];
            pageTitleTabs[0, 0] = GetString("EmailTemplate_Edit.EmailTemplates");
            pageTitleTabs[0, 1] = emailTemplateListUrl;

            pageTitleTabs[1, 0] = (templateInfo != null) ? templateInfo.TemplateDisplayName : GetString("EmailTemplate_Edit.CurrentTemplate");

            pageTitleTabs[1, 1] = string.Empty;
            pageTitleTabs[1, 2] = string.Empty;

            CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
        }

        // Pagetitle
        CurrentMaster.Title.HelpTopicName = "newedit_e-mail_template";
        CurrentMaster.Title.HelpName = "helpTopic";
    }

    #endregion


    #region "Private methods"

    private void CreateFooterButtons()
    {
        CurrentMaster.PanelFooter.CssClass = "FloatRight";

        // Add close button every time
        CurrentMaster.PanelFooter.Controls.Add(new LocalizedButton()
                                                   {
                                                       ID = "btnClose",
                                                       ResourceString = "general.close",
                                                       EnableViewState = false,
                                                       OnClientClick = "return CloseDialog();",
                                                       CssClass = "SubmitButton"
                                                   });
    }


    /// <summary>
    /// On check permissions event handler.
    /// </summary>
    protected void emailTemplateEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.emailtemplates", permissionType))
        {
            RedirectToCMSDeskAccessDenied("CMS.EmailTemplates", permissionType);
        }
    }

    #endregion
}