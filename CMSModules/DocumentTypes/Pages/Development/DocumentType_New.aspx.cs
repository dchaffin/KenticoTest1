using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_New : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the inner control theme property
        newDocWizard.Theme = Theme;

        // for all steps
        string[,] pageTitleTabs = new string[2,3];
        pageTitleTabs[0, 0] = GetString("DocumentType_New.DocumentTypes");
        pageTitleTabs[0, 1] = "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_List.aspx";
        pageTitleTabs[0, 2] = "";
        pageTitleTabs[1, 0] = GetString("DocumentType_New.CurrentDocumentType");
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
        CurrentMaster.Title.TitleText = GetString("DocumentType_New.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_DocumentType/new.png");
        CurrentMaster.Title.HelpTopicName = "new_document_type";
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.BodyClass += " FieldEditorWizardBody";
    }
}