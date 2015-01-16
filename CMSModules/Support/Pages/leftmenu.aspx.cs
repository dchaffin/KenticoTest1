using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Support_Pages_leftmenu : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        treeElem.OnNodeCreated += treeElem_OnNodeCreated;
    }


    protected TreeNode treeElem_OnNodeCreated(UIElementInfo uiElement, TreeNode defaultNode)
    {
        switch (uiElement.ElementName.ToLowerCSafe())
        {
            case "support.apiexamples":
                // Checks if APIExamples file exists
                if (!File.Exists(Server.MapPath("~/CMSAPIExamples/Default.aspx")))
                {
                    return null;
                }
                defaultNode.Target = "_top";
                break;

            case "support.documentation":
                // Edit documentation url dinamicaly version to version
                uiElement.ElementTargetURL = "http://devnet.kentico.com/Documentation/" + CMSContext.SYSTEM_VERSION.Replace(".", "_") + ".aspx";
                break;
        }

        return defaultNode;
    }
}