using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_SystemTables_Pages_Development_AlternativeForms_Layout : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = string.Empty;

        layoutElem.FormType = CMSModules_AdminControls_Controls_Class_Layout.FORMTYPE_SYSTEMTABLE;
        layoutElem.ObjectID = QueryHelper.GetInteger("altformid", 0);
        layoutElem.IsAlternative = true;
    }
}