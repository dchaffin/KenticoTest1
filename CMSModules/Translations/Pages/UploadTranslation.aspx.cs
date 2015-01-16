using System;
using System.Data;
using System.Threading;
using System.Collections;
using System.Text;

using CMS.GlobalHelper;
using CMS.ResourceManager;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.IO;
using CMS.EventLog;
using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.LicenseProvider;
using CMS.TranslationServices;

public partial class CMSModules_Translations_Pages_UploadTranslation : CMSTranslationServiceModalPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        uploadElem.SubmissionID = QueryHelper.GetInteger("submissionid", 0);
        uploadElem.SubmissionItemID = QueryHelper.GetInteger("itemid", 0);
        uploadElem.CheckTranslationsPermissions = true;

        this.CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_TranslationSubmission/object.png");
        this.CurrentMaster.Title.TitleText = GetString("translationservice.uploadtranslation");

        this.btnUpload.Click += new EventHandler(btnUpload_Click);
    }


    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (uploadElem.UploadTranslation())
        {
            ltlScript.Text = ScriptHelper.GetScript("if (wopener && wopener.ShowUploadSuccess) { wopener.ShowUploadSuccess(); }; CloseDialog();");
        }
    }

    #endregion
}