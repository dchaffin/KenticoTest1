using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.UIControls;

public partial class CMSMessages_invalidlicensekey : MessagePage
{
    private const string CLIENT_PORTAL = "http://client.kentico.com/";

    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;
        // Set error state
        Response.StatusCode = 503;

        titleElem.TitleText = GetString("InvalidLicense.Header");
        titleElem.TitleImage = GetImageUrl("Others/Messages/denied.png");

        lblRawUrl.Text = GetString("InvalidLicense.RawUrl");
        lblResult.Text = GetString("InvalidLicense.Result");

        // URL
        string rawUrl = QueryHelper.GetString("rawUrl", String.Empty).Trim();
        if (rawUrl != String.Empty)
        {
            lblRawUrlValue.Text = HttpUtility.HtmlEncode(rawUrl);
        }
        else
        {
            lblRawUrl.Visible = false;
            lblRawUrlValue.Visible = false;
        }

        // Validation result
        LicenseValidationEnum mResult = (LicenseValidationEnum)QueryHelper.GetInteger("Result", Convert.ToInt32(LicenseValidationEnum.NotAvailable));
        lblResultValue.Text = LicenseHelper.GetValidationResultString(mResult);

        // URL 'Go to:'
        lnkGoToValue.ToolTip = URLHelper.GetAbsoluteUrl(URLHelper.WebApplicationVirtualPath) + "CMSSiteManager";
        lnkGoToValue.NavigateUrl = URLHelper.GetAbsoluteUrl(URLHelper.WebApplicationVirtualPath) + "CMSSiteManager/default.aspx?section=licenses";
        lblAddLicenseValue.Text = URLHelper.GetCurrentDomain();

        // How to get license options
        lnkOpt1.NavigateUrl = CLIENT_PORTAL;
        lnkOpt1.ToolTip = CLIENT_PORTAL;
        lnkOpt3.NavigateUrl = CLIENT_PORTAL;
        lnkOpt3.ToolTip = CLIENT_PORTAL;
    }
}