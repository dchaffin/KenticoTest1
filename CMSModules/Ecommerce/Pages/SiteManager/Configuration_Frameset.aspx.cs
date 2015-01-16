using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSModules_Ecommerce_Pages_SiteManager_Configuration_Frameset : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        configEdit.Attributes["src"] = "../Tools/Configuration/Configuration_Frameset.aspx?siteId=0";
    }
}