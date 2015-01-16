using System;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_CustomTables_CustomTable_Edit_Transformation_List : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int customTableId = QueryHelper.GetInteger("customtableid", 0);

        // New item link
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("customtable.edit.newtransformation");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_New.aspx?documenttypeid=" + customTableId.ToString() + "&iscustomtable=1");
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/CMS_Transformation/add.png");
        CurrentMaster.HeaderActions.Actions = actions;

        // Set the query editor control
        classTransformations.ClassID = customTableId;
        classTransformations.EditPageUrl = "CustomTable_Edit_Transformation_Frameset.aspx";
    }
}