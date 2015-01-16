using System;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.LicenseProvider;
using CMS.SettingsProvider;

[Title(ImageUrl = "Objects/CMS_TranslationSubmission/object.png", ResourceString = "translationservice.submittranslateddocuments")]
public partial class CMSModules_Translations_CMSPages_SubmitTranslation : CMSLiveModalPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check the license
        if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.TranslationServices);
        }

        if (!QueryHelper.ValidateHash("hash", null, false))
        {
            ShowError(GetString("general.badhashtext"));
            uploadElem.StopProcessing = true;
            uploadElem.Visible = false;
            return;
        }

        uploadElem.SubmissionID = QueryHelper.GetInteger("submissionid", 0);
        uploadElem.SubmissionItemID = QueryHelper.GetInteger("itemid", 0);

        this.CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_TranslationSubmission/object.png");
        this.CurrentMaster.Title.TitleText = GetString("translationservice.uploadtranslation");

        this.btnUpload.Click += new EventHandler(btnUpload_Click);
    }


    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (uploadElem.UploadTranslation())
        {
            ShowInformation(GetString("translationservice.translationsuploaded"));

            uploadElem.Visible = false;
            btnUpload.Visible = false;
        }
    }

    #endregion
}