using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_OnlineMarketing_Pages_Tools_AbTest_ABTest_Frameset : CMSABTestPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        frmContent.Attributes["src"] = GetPageURL(QueryHelper.GetString("page", "overview")) + URLHelper.Url.Query;
    }


    /// <summary>
    /// Gets page url from constant.
    /// </summary>
    /// <param name="page">Page constant</param>
    private string GetPageURL(string page)
    {
        switch (page.ToLowerCSafe())
        {
            case "conversionsvalue":
                return "conversionsvalue.aspx";

            case "conversionscount":
                return "conversionscount.aspx";

            case "conversionsrate":
                return "conversionsrate.aspx";

            case "conversionsbyvariations":
                return "ConversionsByVariations.aspx";

            case "conversionssourcepages":
                return "ConversionsSourcePages.aspx";

            default:
                return "overview.aspx";
        }
    }
}