using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_CustomTables_CustomTable_Edit_Form : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";

        layoutElem.FormType = CMSModules_AdminControls_Controls_Class_Layout.FORMTYPE_CUSTOMTABLE;
        layoutElem.ObjectID = QueryHelper.GetInteger("customtableid", 0);
    }
}