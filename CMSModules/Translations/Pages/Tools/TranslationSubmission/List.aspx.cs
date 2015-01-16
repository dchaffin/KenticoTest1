using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.TranslationServices;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;
using CMS.SettingsProvider;

// Title
[Title("Objects/CMS_TranslationSubmission/object.png", "translationservice.translationsubmission.list", "translationservices_translationsubmission_list")]

public partial class CMSModules_Translations_Pages_Tools_TranslationSubmission_List : CMSTranslationServicePage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        this.btnUpdateStatuses.Click += new EventHandler(btnUpdateStatuses_Click);
    }


    protected void btnUpdateStatuses_Click(object sender, EventArgs e)
    {
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.TranslationServices", "Modify"))
        {
            RedirectToAccessDenied("CMS.TranslationServices", "Modify");
        }

        string err = TranslationServiceHelper.CheckAndDownloadTranslations(CMSContext.CurrentSiteName);
        if (!string.IsNullOrEmpty(err))
        {
            ShowError(err);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterDialogScript(this.Page);

        HeaderAction updateAction = new HeaderAction()
        {
            OnClientClick = ControlsHelper.GetPostBackEventReference(btnUpdateStatuses, null),
            Tooltip = GetString("translationservice.updatestatusestooltip"),
            Text = GetString("translationservice.updatestatuses"),
            Enabled = CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.TranslationServices", "Modify") && !listElem.Grid.IsEmpty 
        };
        updateAction.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Approve" + (updateAction.Enabled ? "" : "Disabled") + ".png");

        string translateUrl = CMSContext.ResolveDialogUrl("~/CMSModules/Translations/Pages/TranslateDocuments.aspx") + "?select=1&modal=1";
        translateUrl = URLHelper.AddParameterToUrl(translateUrl, "hash", QueryHelper.GetHash(URLHelper.GetQuery(translateUrl)));

        HeaderAction submitAction = new HeaderAction()
        {
            OnClientClick = "modalDialog('" + translateUrl + "', 'SubmitTranslation', 600, 570);",
            Tooltip = GetString("translationservice.submittranslationtooltip"),
            Text = GetString("translationservice.submittranslation"),
            Enabled = CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Content", "SubmitForTranslation")
        };
        submitAction.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/AddPost" + (submitAction.Enabled ? "" : "Disabled") + ".png");

        this.AddHeaderAction(submitAction);
        this.AddHeaderAction(updateAction);

        CurrentMaster.HeaderActions.ReloadData();

        if (!listElem.Grid.IsEmpty)
        {
            string statusCheck = SettingsKeyProvider.GetStringValue("CMSTranslationsLastStatusCheck");
            if (string.IsNullOrEmpty(statusCheck))
            {
                statusCheck = GetString("general.notavailable");
            }

            ShowInformation(string.Format(GetString("translationservice.laststatuscheck"), statusCheck));
        }
    }

    #endregion
}
