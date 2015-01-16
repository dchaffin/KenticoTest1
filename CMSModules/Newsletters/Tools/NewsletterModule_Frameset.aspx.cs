using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.ExtendedControls;

public partial class CMSModules_Newsletters_Tools_NewsletterModule_Frameset : CMSNewsletterPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        configurationMenu.Attributes["src"] = URLHelper.AppendQuery("Header.aspx", URLHelper.Url.Query);
    }

    #endregion
}