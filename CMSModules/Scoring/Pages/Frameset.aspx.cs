using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;

public partial class CMSModules_Scoring_Pages_Frameset : CMSScorePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Modify high of the first row...
        if (QueryHelper.GetBoolean("dialogmode", false))
        {
            //  in modal dialog
            frm.FrameHeight = TabsFrameHeight;
        }
        else
        {
            // in CMSDesk
            frm.FrameHeight = TabsBreadHeadFrameHeight;
        }

        // Set content page
        if (QueryHelper.GetBoolean("saved", false))
        {
            frm.ContentUrl = "Tab_General.aspx";
        }
        else
        {
            frm.ContentUrl = "Tab_Contacts.aspx";
        }
    }
}