using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

using TreeNode = CMS.DocumentEngine.TreeNode;

// Set page title
[Title("CMSModules/CMS_Content/Menu/Properties.png", "contentmenu.properties", "")]
public partial class CMSModules_Content_CMSDesk_Properties_Properties_Header : CMSModalPage
{
    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Display separator
		AbstractMasterPage mPage = CurrentMaster as AbstractMasterPage;
        if (mPage != null)
        {
            mPage.DisplaySeparatorPanel = true;
        }

        // Add the document name to the properties header title
        TreeNode node = DocumentManager.Node;
        if (node != null)
        {
            string nodeName = node.GetDocumentName();
            // Get name for root document
            if (node.NodeClassName.ToLowerCSafe() == "cms.root")
            {
                nodeName = CMSContext.CurrentSite.DisplayName;
            }

            CurrentMaster.Title.TitleText += " \"" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(nodeName)) + "\"";
        }
    }

    #endregion
}