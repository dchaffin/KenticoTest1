using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;

public partial class CMSModules_Ecommerce_Pages_Tools_EcommerceModule_Frameset : CMSEcommercePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        frameMenu.Attributes.Add("src", "Header.aspx" + URLHelper.Url.Query);
    }
}