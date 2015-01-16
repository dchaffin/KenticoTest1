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
using CMS.EventLog;

public partial class CMSMessages_FeatureNotAvailable : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string domain = QueryHelper.GetText("domainname", String.Empty);

        if (domain != String.Empty)
        {
            LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(domain);
            if (lki != null)
            {
                LabelMessage.Text = GetString("CMSSiteManager.FeatureNotAvailable");

                // Log message to event log
                EventLogProvider eventLog = new EventLogProvider();
                eventLog.LogEvent(EventLogProvider.EVENT_TYPE_WARNING, DateTime.Now, "Feature not available page", "FeatureNotAvailable", URLHelper.CurrentURL, "The requested feature is not available in the CMS edition you are using: " + LicenseHelper.GetEditionName(lki.Edition));
            }
            else
            {
                LabelMessage.Text = GetString("CMSSiteManager.LicenseNotFound").Replace("%%name%%", domain);
            }
        }
        titleElem.TitleText = GetString("CMSSiteManager.AccesDenied");
        titleElem.TitleImage = GetImageUrl("Others/Messages/denied.png");
    }
}