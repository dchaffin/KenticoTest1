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

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSSiteManager_accessdenied : AccessDeniedPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnSignOut.OnClientClick = FacebookConnectHelper.FacebookConnectInitForSignOut(CMSContext.CurrentSiteName, ltlScript);

        // Setup page title text and image
        CurrentMaster.Title.TitleImage = GetImageUrl("Others/Messages/denied.png");

        string title = GetString("CMSSiteManager.AccessDenied");
        string message = GetString("CMSSiteManager.IsNotAdminMsg");

        bool hideLinks = GetTexts(ref message, ref title);

        lblMessage.Text = message;
        CurrentMaster.Title.TitleText = title;

        // Override displaying of links
        hideLinks = QueryHelper.GetBoolean("hidelinks", hideLinks);

        if (!hideLinks)
        {
            lnkGoBack.Text = GetString("CMSSiteManager.GoBack");

            // Hide for windows authentication
            if (RequestHelper.IsWindowsAuthentication())
            {
                btnSignOut.Visible = false;
            }
        }
        else
        {
            btnSignOut.Visible = false;
            lnkGoBack.Visible = false;
        }
    }

    #endregion


    #region "Methods"

    protected override void PerformSignOut()
    {
        base.PerformSignOut();
        ltlScript.Text = ScriptHelper.GetScript("parent.location.href='../default.aspx';");
    }

    #endregion


    #region "Button handling"

    protected void btnSignOut_Click(object sender, EventArgs e)
    {
        PerformSignOut();
    }


    /// <summary>
    /// Disable handler base tag.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        UseBaseTagForHandlerPage = false;
        base.OnInit(e);
    }

    #endregion
}