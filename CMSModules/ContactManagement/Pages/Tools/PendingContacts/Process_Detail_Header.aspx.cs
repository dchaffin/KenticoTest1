using System;

using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.CMSHelper;


public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_Process_Detail_Header : CMSContactManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set Title control
        titleElem.TitleText = "Process detail";
        titleElem.ShowCloseButton = true;
        titleElem.ShowFullScreenButton = true;
        titleElem.TitleImage = GetImageUrl("Objects/OM_Contact/object.png");

        RegisterModalPageScripts();
    }
}
