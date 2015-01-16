using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.MembershipProvider;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSMessages_AccessDenied : AccessDeniedPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;

        btnSignOut.OnClientClick = FacebookConnectHelper.FacebookConnectInitForSignOut(CMSContext.CurrentSiteName, ltlScript);

        titleElem.TitleImage = GetImageUrl("Others/Messages/denied.png");

        string title = GetString("CMSDesk.AccessDenied");
        string message = GetString("CMSMessages.AccessDenied");

        GetTexts(ref message, ref title);

        lblMessage.Text = message;
        titleElem.TitleText = title;

        // Display SignOut button
        if (CMSContext.CurrentUser.IsAuthenticated())
        {
            if (!RequestHelper.IsWindowsAuthentication())
            {
                btnSignOut.Visible = true;
            }
        }
        else
        {
            btnLogin.Visible = true;
        }
    }

    #endregion


    #region "Methods"

    protected override void PerformSignOut()
    {
        base.PerformSignOut();

        ltlScript.Text = ScriptHelper.GetScript("window.top.location.href= '../default.aspx';");
    }

    #endregion


    #region "Button handling"

    protected void btnSignOut_Click(object sender, EventArgs e)
    {
        PerformSignOut();
    }


    protected void btnLogin_Click(object sender, EventArgs e)
    {
        // Get the logon page URL
        string logonPage = SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSSecuredAreasLogonPage");
        if (logonPage == string.Empty)
        {
            logonPage = "../CMSPages/Logon.aspx";
        }

        ltlScript.Text = ScriptHelper.GetScript("window.top.location.href = '" + ResolveUrl(logonPage) + "';");
    }

    #endregion
}