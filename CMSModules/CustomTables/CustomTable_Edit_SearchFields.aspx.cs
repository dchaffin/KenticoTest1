using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_CustomTables_CustomTable_Edit_SearchFields : CMSCustomTablesPage
{
    private int classId = QueryHelper.GetInteger("classid", 0);


    protected void Page_Load(object sender, EventArgs e)
    {
        DataClassInfo dci = DataClassInfoProvider.GetDataClass(classId);
        // Set edited object
        EditedObject = dci;

        // Class exists
        if ((dci == null) || (!dci.ClassIsCoupledClass))
        {
            ShowWarning(GetString("customtable.ErrorNoFieldsSearch"), null, null);
            SearchFields.Visible = false;
            SearchFields.StopProcessing = true;
        }
        else
        {
            SearchFields.ItemID = QueryHelper.GetInteger("classid", 0);
        }
    }
}