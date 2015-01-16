using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_SystemTables_Pages_Development_SystemTable_Edit_Search : SiteManagerPage
{
    private string className = QueryHelper.GetString("classname", String.Empty);


    protected void Page_Load(object sender, EventArgs e)
    {
        ShowWarning(GetString("systbl.search.warninfo"), null, null);

        DataClassInfo dci = DataClassInfoProvider.GetDataClass(className);
        if (dci != null)
        {
            ClassFields.ItemID = dci.ClassID;
            ClassFields.DisplayIField = false;
            ClassFields.DisplaySetAutomatically = false;
        }
    }
}