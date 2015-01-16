using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "mvtest.list", "List.aspx?nodeid={?nodeid?}", "")]
[Breadcrumb(1, "general.new")]
// Title (for breadcrumbs)
[Title("", "", "mvtest_general")]
public partial class CMSModules_OnlineMarketing_Pages_Content_MVTest_New : CMSMVTestContentPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the alias path of the current node
        if (Node != null)
        {
            editElem.AliasPath = Node.NodeAliasPath;
        }
        else
        {
            editElem.StopProcessing = true;
        }

        editElem.OnSaved += editElem_OnSaved;
    }


    private void editElem_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("frameset.aspx?saved=1&mvtestid=" + editElem.ItemID + "&nodeID=" + QueryHelper.GetInteger("nodeID", 0));
    }
}