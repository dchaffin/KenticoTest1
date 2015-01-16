using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSImportExport;
using CMS.GlobalHelper;
using CMS.DocumentEngine;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Export_Site_cms_document : ImportExportControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        chkDocuments.Text = GetString("ExportDocuments.ExportDocuments");
        chkDocumentsHistory.Text = GetString("ExportDocuments.ExportDocumentsHistory");
        chkRelationships.Text = GetString("ExportDocuments.ExportRelationships");
        chkACLs.Text = GetString("ExportDocuments.ExportACLs");
        chkEventAttendees.Text = GetString("ExportDocuments.ExportEventAttendees");
        chkBlogComments.Text = GetString("ExportDocuments.ExportBlogComments");

        // Javascript
        string script = "var parent = document.getElementById('" + chkDocuments.ClientID + "'); \n" +
                        "var childIDs = ['" + chkDocumentsHistory.ClientID + "', '" + chkRelationships.ClientID + "', '" + chkBlogComments.ClientID + "', '" + chkEventAttendees.ClientID + "', '" + chkACLs.ClientID + "']; \n" +
                        "InitCheckboxes(); \n";

        ltlScript.Text = ScriptHelper.GetScript(script);

        chkDocuments.Attributes.Add("onclick", "CheckChange();");
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        ProcessObjectEnum documents = (chkDocuments.Checked ? ProcessObjectEnum.All : ProcessObjectEnum.None);

        Settings.SetObjectsProcessType(documents, DocumentObjectType.DOCUMENT, true);

        Settings.SetSettings(ImportExportHelper.SETTINGS_DOC_HISTORY, chkDocumentsHistory.Checked);

        Settings.SetSettings(ImportExportHelper.SETTINGS_DOC_RELATIONSHIPS, chkRelationships.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_DOC_ACLS, chkACLs.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_EVENT_ATTENDEES, chkEventAttendees.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_BLOG_COMMENTS, chkBlogComments.Checked);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        chkDocuments.Checked = (Settings.GetObjectsProcessType(DocumentObjectType.DOCUMENT, true) != ProcessObjectEnum.None);
        chkDocumentsHistory.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_HISTORY), false);

        chkRelationships.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_RELATIONSHIPS), false);
        chkACLs.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_ACLS), false);
        chkEventAttendees.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_EVENT_ATTENDEES), false);
        chkBlogComments.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_BLOG_COMMENTS), false);
    }
}