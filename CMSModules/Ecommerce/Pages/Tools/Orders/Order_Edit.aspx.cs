using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;

public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit : CMSOrdersPage
{
    protected override void OnPreInit(EventArgs e)
    {
        CustomerID = QueryHelper.GetInteger("customerid", 0);
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (CustomerID > 0)
        {
            rowsFrameset.Attributes["rows"] = TabsBreadFrameHeight + ", *";
        }
        else
        {
            rowsFrameset.Attributes["rows"] = "96, *";
        }
    }
}