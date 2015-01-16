using System;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Breadcrumbs(2)]
[Breadcrumb(0, TargetUrl = "~/CMSModules/SystemTables/Pages/Development/AlternativeForms/List.aspx", ResourceString = "altforms.listlink")]
[Breadcrumb(1, ResourceString = "altform.newbread")]
// Set help
[Help("alternative_forms_general", "helpTopic")]
public partial class CMSModules_SystemTables_Pages_Development_AlternativeForms_New : SiteManagerPage
{
    #region "Variables"

    private int classId = 0;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        classId = QueryHelper.GetInteger("classid", 0);

        form.OnBeforeSave += form_OnBeforeSave;

        // Set redirect url - created form ID is needed => need late bound redirect.
        form.RedirectUrlAfterCreate = String.Empty;
        form.OnAfterSave += (o, ea) => URLHelper.Redirect("Frameset.aspx?classid=" + classId + "&altformid=" + ((AlternativeFormInfo)EditedObject).FormID + "&saved=1");

        CurrentMaster.Title.Breadcrumbs[0, 1] += "?classid=" + classId;

        // Check if the 'Combine With User Settings' feature should be available
        if (classId > 0)
        {
            string className = DataClassInfoProvider.GetClassName(classId);
            if (className != null && (className.ToLowerCSafe().Trim() == SiteObjectType.USER.ToLowerCSafe()))
            {
                pnlCombineUserSettings.Visible = true;
            }
        }
    }


    protected void form_OnBeforeSave(object sender, EventArgs e)
    {
        // Set parent's class ID
        AlternativeFormInfo info = (AlternativeFormInfo)form.EditedObject;

        DataClassInfo dci = DataClassInfoProvider.GetDataClass(SiteObjectType.USERSETTINGS);
        if (dci != null)
        {
            info.FormCoupledClassID = (chkCombineUserSettings.Checked) ? dci.ClassID : 0;
        }

        info.FormClassID = classId;
    }

    #endregion
}