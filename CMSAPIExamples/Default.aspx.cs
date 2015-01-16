using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAPIExamples_Default : CMSAPIExamplePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        cmsdesktop.Src = ResolveUrl("~/CMSAPIExamples/Pages/Main.aspx") + URLHelper.Url.Query;
    }
}