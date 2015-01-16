using System;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[EditedObject(FormObjectType.ALTERNATIVEFORM, "altformid")]
public partial class CMSModules_SystemTables_Pages_Development_AlternativeForms_Edit_General : SiteManagerPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        AlternativeFormInfo editedObject = (AlternativeFormInfo)EditedObject;

        // Check if the 'Combine With User Settings' feature should be available
        if (editedObject.FormClassID > 0)
        {
            string className = DataClassInfoProvider.GetClassName(editedObject.FormClassID);
            if (className != null && (className.ToLowerCSafe().Trim() == SiteObjectType.USER.ToLowerCSafe()))
            {
                pnlCombineUserSettings.Visible = true;
                chkCombineUserSettings.Checked = (editedObject.FormCoupledClassID > 0);
            }
        }

        form.OnAfterSave += (s, ea) => ScriptHelper.RefreshTabHeader(Page, null);
    }

    #endregion
}