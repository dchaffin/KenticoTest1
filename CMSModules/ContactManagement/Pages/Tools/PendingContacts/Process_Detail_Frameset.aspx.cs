using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.OnlineMarketing;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.CMSHelper;

public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_Process_Detail_Frameset : CMSContactManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterEscScript();

        frameHeader.Attributes.Add("src", "Process_Detail_Header.aspx");
        frameContent.Attributes.Add("src", "Process_Detail.aspx" + URLHelper.Url.Query);
        frameFooter.Attributes.Add("src", "../DetailFooter.aspx" + URLHelper.Url.Query);
    }
}
