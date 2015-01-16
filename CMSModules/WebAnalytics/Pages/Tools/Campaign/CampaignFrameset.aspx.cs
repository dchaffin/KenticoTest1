using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_CampaignFrameset : CMSCampaignAndConversionPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        frmContent.Attributes["src"] = ResolveUrl("CampaignReport.aspx" + URLHelper.Url.Query + "&displayTitle=0");
        frmHeader.Attributes["src"] = "CampaignHeader.aspx" + URLHelper.Url.Query;
    }
}