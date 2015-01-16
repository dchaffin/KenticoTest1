using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSImportExport;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.WorkflowEngine;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Import_Site_cms_document : ImportExportControl
{
    /// <summary>
    /// True if import into existing site.
    /// </summary>
    protected bool ExistingSite
    {
        get
        {
            if (Settings != null)
            {
                return ((SiteImportSettings)Settings).ExistingSite;
            }
            return true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        chkDocuments.Text = GetString("ImportDocuments.ImportDocuments");
        chkDocumentsHistory.Text = GetString("ImportDocuments.ImportDocumentsHistory") + "<br />";
        chkRelationships.Text = GetString("ImportDocuments.ImportRelationships") + "<br />";
        chkACLs.Text = GetString("ImportDocuments.ImportACLs") + "<br />";
        chkEventAttendees.Text = GetString("ImportDocuments.ImportEventAttendees") + "<br />";
        chkBlogComments.Text = GetString("ImportDocuments.ImportBlogComments") + "<br />";
        chkUserPersonalizations.Text = GetString("CMSImport_UserPersonalizations.ImportUserPersonalizations") + "<br />";

        // Javascript
        string script = "var parent = document.getElementById('" + chkDocuments.ClientID + "'); \n" +
                        "var childIDs = ['" + chkDocumentsHistory.ClientID + "', '" + chkRelationships.ClientID + "', '" + chkBlogComments.ClientID + "', '" + chkEventAttendees.ClientID + "', '" + chkACLs.ClientID + "', '" + chkUserPersonalizations.ClientID + "']; \n" +
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
        Settings.SetSettings(ImportExportHelper.SETTINGS_USER_PERSONALIZATIONS, chkUserPersonalizations.Checked);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        chkDocuments.Checked = (Settings.GetObjectsProcessType(DocumentObjectType.DOCUMENT, true) != ProcessObjectEnum.None);
        chkDocumentsHistory.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_HISTORY), true);

        chkRelationships.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_RELATIONSHIPS), true);
        chkACLs.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_ACLS), true);
        chkEventAttendees.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_EVENT_ATTENDEES), true);
        chkBlogComments.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_BLOG_COMMENTS), true);
        chkUserPersonalizations.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_USER_PERSONALIZATIONS), !ExistingSite);

        // Visibility
        SiteImportSettings settings = (SiteImportSettings)Settings;

        chkACLs.Visible = settings.IsIncluded(DocumentObjectType.ACL, false);
        chkDocumentsHistory.Visible = settings.IsIncluded(DocumentObjectType.VERSIONHISTORY, false);
        chkRelationships.Visible = settings.IsIncluded(DocumentObjectType.RELATIONSHIP, false);
        //this.chkUserPersonalizations.Visible = settings.IsIncluded(PortalObjectType.PERSONALIZATION, false);
        pnlDocumentData.Visible = chkDocumentsHistory.Visible || chkACLs.Visible || chkRelationships.Visible || chkUserPersonalizations.Visible;

        chkBlogComments.Visible = settings.IsIncluded(PredefinedObjectType.BLOGCOMMENT, false);
        chkEventAttendees.Visible = settings.IsIncluded(PredefinedObjectType.EVENTATTENDEE, false);
        pnlModules.Visible = chkBlogComments.Visible || chkEventAttendees.Visible;
    }
}