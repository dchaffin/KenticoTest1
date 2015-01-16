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
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSDesk_accessdenied : AccessDeniedPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnSignOut.OnClientClick = FacebookConnectHelper.FacebookConnectInitForSignOut(CMSContext.CurrentSiteName, ltlScript);

        // Setup page title text and image
        CurrentMaster.Title.TitleImage = GetImageUrl("Others/Messages/denied.png");

        string title = GetString("CMSDesk.AccessDenied");
        string message = GetString("CMSDesk.IsNotEditorMsg");

        bool hideLinks = GetTexts(ref message, ref title);

        lblMessage.Text = message;
        CurrentMaster.Title.TitleText = title;
        
        if (!hideLinks)
        {
            lnkGoBack.Text = GetString("CMSDesk.GoBack");

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
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Perfsignout",
            ScriptHelper.GetScript("parent.location.href= '../default.aspx';"));
    }

    #endregion


    #region "Button handling"

    protected void btnSignOut_Click(object sender, EventArgs e)
    {
        PerformSignOut();
    }

    #endregion
}