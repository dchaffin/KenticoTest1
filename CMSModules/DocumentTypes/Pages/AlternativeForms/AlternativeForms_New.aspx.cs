using System;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

[Breadcrumbs(2)]
[Breadcrumb(0, TargetUrl = "~/CMSModules/DocumentTypes/Pages/AlternativeForms/AlternativeForms_List.aspx", ResourceString = "altforms.listlink")]
[Breadcrumb(1, ResourceString = "altform.newbread")]
// Set help
[Help("alternative_forms_general", "helpTopic")]
public partial class CMSModules_DocumentTypes_Pages_AlternativeForms_AlternativeForms_New : SiteManagerPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        int classId = QueryHelper.GetInteger("classid", 0);
        altFormEdit.OnBeforeSave += (o, ea) => ((AlternativeFormInfo)altFormEdit.EditedObject).FormClassID = classId;

        // Set redirect url - created form ID is needed => need late bound redirect.
        altFormEdit.RedirectUrlAfterCreate = String.Empty;
        altFormEdit.OnAfterSave += (o, ea) => URLHelper.Redirect("AlternativeForms_Frameset.aspx?classid=" + classId + "&altformid=" + ((AlternativeFormInfo)EditedObject).FormID + "&saved=1");

        CurrentMaster.Title.Breadcrumbs[0, 1] += "?classid=" + classId;
    }

    #endregion
}