using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAPIExamples_Pages_Main : CMSAPIExamplePage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        frmMenu.Attributes["src"] = ResolveUrl("~/CMSAPIExamples/Pages/Menu.aspx") + URLHelper.Url.Query;


        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }
    }

    #endregion
}