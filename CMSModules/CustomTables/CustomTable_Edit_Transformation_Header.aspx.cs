using System;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

// Set edited object
[EditedObject("cms.transformation", "transformationid")]
// Add breadcrumbs
[Breadcrumbs(2)]
// Set page help
[Help("customtable_edit_newedit_transformation", "helpTopic")]
// Set tabs number
[Tabs("ctt_edit_content")]
public partial class CMSModules_CustomTables_CustomTable_Edit_Transformation_Header : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TransformationInfo ti = EditedObject as TransformationInfo;

        // Set breadcrumbs
        SetBreadcrumb(0, GetString("DocumentType_Edit_Transformation_Edit.Transformations"), ResolveUrl("CustomTable_Edit_Transformation_List.aspx?customtableid=" + ti.TransformationClassID), "_parent", null);
        SetBreadcrumb(1, ti.TransformationName, null, null, null);

        // Set general tab
        SetTab(0, GetString("general.general"), URLHelper.RemoveParameterFromUrl("CustomTable_Edit_Transformation_Edit.aspx" + URLHelper.Url.Query, "saved"), "SetHelpTopic('helpTopic', 'customtable_edit_newedit_transformation');");
    }
}