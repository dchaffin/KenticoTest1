using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_SystemTables_Pages_Development_AlternativeForms_Fields : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get alternative form ID from URL
        int altFormId = QueryHelper.GetInteger("altformid", 0);
        CurrentMaster.BodyClass += " FieldEditorBody";

        // Get alternative form
        AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(altFormId);
        if (afi != null)
        {
            // Get name of the edited class
            string className = DataClassInfoProvider.GetClassName(afi.FormClassID);

            // Initialize field editor
            altFormFieldEditor.Mode = FieldEditorModeEnum.SystemTable;
            altFormFieldEditor.ShowFieldVisibility = (className.ToLowerCSafe().Trim() == SiteObjectType.USER.ToLowerCSafe());
            altFormFieldEditor.AlternativeFormID = altFormId;
            altFormFieldEditor.DisplayedControls = FieldEditorControlsEnum.SystemTables;

            if(className.ToLowerCSafe().Trim() == PredefinedObjectType.SKU)
            {
                altFormFieldEditor.ShowInheritanceSettings = true;
            }
        }
    }
}