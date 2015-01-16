using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_UICultures_Pages_Development_UICultures_Default : SiteManagerPage
{
    protected string contentUrl = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        contentUrl = ResolveUrl("ResourceString/List.aspx?uicultureid=");

        // Default UI culture strings
        string defaultCulture = CultureHelper.DefaultUICulture;
        try
        {
            UICultureInfo ci = UICultureInfoProvider.GetUICultureInfo(defaultCulture);
            if (ci != null)
            {
                contentUrl += ci.UICultureID;
            }
        }
        catch
        {
        }
    }
}