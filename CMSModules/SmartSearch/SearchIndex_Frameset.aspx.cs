using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_SmartSearch_SearchIndex_Frameset : SiteManagerPage
{
    protected string parameters = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        parameters = "?indexid=" + QueryHelper.GetText("indexid", "");
    }
}