using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Modules_Pages_Development_Module_UI_General : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int elementId = QueryHelper.GetInteger("elementid", 0);
        int parentId = QueryHelper.GetInteger("parentId", 0);
        int moduleId = QueryHelper.GetInteger("moduleid", 0);

        if (parentId > 0)
        {
            editElem.ElementID = elementId;
            editElem.ParentID = parentId;
            editElem.ResourceID = moduleId;
        }
    }
}