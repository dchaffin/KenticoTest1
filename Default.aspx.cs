using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class _Default : TemplatePage
{
    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        PageManager = manPortal;
        ManagersContainer = plcManagers;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblText.Text = "The web site doesn't contain any content. Sign in to <a href=\"" + ResolveUrl("~/cmsdesk/default.aspx") + "\">CMS Desk</a> and edit the content.";
        ltlTags.Text = HeaderTags;
    }
}