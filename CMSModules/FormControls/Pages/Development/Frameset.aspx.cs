using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_FormControls_Pages_Development_Frameset : SiteManagerPage
{
    protected int controlId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get form control ID from url
        controlId = QueryHelper.GetInteger("controlId", 0);
    }
}