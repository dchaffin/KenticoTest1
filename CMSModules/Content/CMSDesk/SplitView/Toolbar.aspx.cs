using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_SplitView_Toolbar : CMSContentPage
{
    #region "Methods

    protected void Page_Load(object sender, EventArgs e)
    {
        documentToolbar.Node = Node;
    }

    #endregion
}