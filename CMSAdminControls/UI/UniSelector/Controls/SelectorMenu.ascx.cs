using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;

public partial class CMSAdminControls_UI_UniSelector_Controls_SelectorMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        iRemoveAll.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/Delete.png");
        iRemoveAll.Text = ResHelper.GetString("General.RemoveAll");
        iRemoveAll.Attributes.Add("onclick", "US_ContextRemoveAll(GetContextMenuParameter('" + ContextMenu.MenuID + "'));");

        if (!IsLiveSite)
        {
            pnlSelectorMenu.CssClass = "PortalContextMenu WebPartContextMenu";
        }
        const string removeScript = @"
    function US_ContextRemoveAll(clientId) {
        setTimeout('US_RemoveAll_' + clientId + '();');
    }
";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "US_CM_RemoveAll", ScriptHelper.GetScript(removeScript));
    }
}