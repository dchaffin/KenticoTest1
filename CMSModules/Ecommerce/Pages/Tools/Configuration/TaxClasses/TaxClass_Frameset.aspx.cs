using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;

public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_Frameset : CMSTaxClassesPage
{
    protected string HeaderUrlParams
    {
        get
        {
            // Get from query parameter
            if (QueryHelper.GetBoolean("hidebreadcrumbs", false))
            {
                return "&hidebreadcrumbs=1";
            }

            return "";
        }
    }


    protected string ContentUrlParams
    {
        get
        {
            // Get from query parameter
            if (QueryHelper.GetBoolean("saved", false))
            {
                return "&saved=1";
            }

            return "";
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        frameTaxClass.Attributes["rows"] = TabsBreadHeadFrameHeight + ",*";

        if (QueryHelper.GetBoolean("hidebreadcrumbs", false))
        {
            frameTaxClass.Attributes["rows"] = TabsFrameHeight + ",*";
        }
    }
}