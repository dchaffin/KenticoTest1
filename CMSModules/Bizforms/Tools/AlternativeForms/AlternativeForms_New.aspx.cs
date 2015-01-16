using System;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.Synchronization;
using CMS.UIControls;

[Breadcrumbs(2)]
[Breadcrumb(0, TargetUrl = "~/CMSModules/BizForms/Tools/AlternativeForms/AlternativeForms_List.aspx", ResourceString = "altforms.listlink")]
[Breadcrumb(1, ResourceString = "altform.newbread")]
[Security(Resource = "cms.form", Permission = "EditForm")]
// Set help
[Help("alternative_forms_general", "helpTopic")]
public partial class CMSModules_BizForms_Tools_AlternativeForms_AlternativeForms_New : CMSBizFormPage
{
    #region "Variables"

    private BizFormInfo bfi;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get needed IDs
        int formId = QueryHelper.GetInteger("formid", 0);
        bfi = BizFormInfoProvider.GetBizFormInfo(formId);

        altFormEdit.OnBeforeSave += (s, ea) => ((AlternativeFormInfo)altFormEdit.EditedObject).FormClassID = bfi.FormClassID;
        altFormEdit.OnAfterSave += altFormEdit_OnAfterSave;

        altFormEdit.RedirectUrlAfterCreate = String.Empty;
        CurrentMaster.Title.Breadcrumbs[0, 1] += "?formid=" + bfi.FormID;
    }


    protected void altFormEdit_OnAfterSave(object sender, EventArgs e)
    {
        // Required to log staging task, alternative form is not bound to bizform as a child
        using (CMSActionContext context = new CMSActionContext())
        {
            context.CreateVersion = false;

            // Log synchronization
            SynchronizationHelper.LogObjectChange(bfi, TaskTypeEnum.UpdateObject);
        }

        URLHelper.Redirect("AlternativeForms_Frameset.aspx?formid=" + bfi.FormID +
                           "&altformid=" + ((AlternativeFormInfo)EditedObject).FormID + "&saved=1");
    }

    #endregion
}