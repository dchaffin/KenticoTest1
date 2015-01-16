using System;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.Synchronization;
using CMS.UIControls;

[EditedObject(FormObjectType.ALTERNATIVEFORM, "altformid")]
[Security(Resource = "cms.form", Permission = "ReadForm")]
public partial class CMSModules_BizForms_Tools_AlternativeForms_AlternativeForms_Edit_General : CMSBizFormPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        altFormEdit.OnBeforeSave += altFormEdit_OnBeforeSave;
        altFormEdit.OnAfterSave += altFormEdit_OnAfterSave;
    }


    protected void altFormEdit_OnBeforeSave(object sender, EventArgs e)
    {
        // Check 'EditForm' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.form", "EditForm"))
        {
            RedirectToCMSDeskAccessDenied("cms.form", "EditForm");
        }
    }


    protected void altFormEdit_OnAfterSave(object sender, EventArgs e)
    {
        // Required to log staging task, alternative form is not bound to bizform as a child
        using (CMSActionContext context = new CMSActionContext())
        {
            context.CreateVersion = false;

            // Log synchronization
            int formId = QueryHelper.GetInteger("formid", 0);
            BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(formId);
            SynchronizationHelper.LogObjectChange(bfi, TaskTypeEnum.UpdateObject);
        }
    }

    #endregion
}