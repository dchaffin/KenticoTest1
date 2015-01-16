using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.CONTACT, "contactId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Merge_Header : CMSContactManagementContactsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check read permission
        CheckReadPermission(ContactHelper.ObjectSiteID(EditedObject));

        ((CMSMasterPage)CurrentMaster).PanelBody.CssClass += " Separator";

        // Check if running under site manager (and distribute "site manager" flag to other tabs)
        string siteManagerParam = string.Empty;
        if (IsSiteManager)
        {
            siteManagerParam = "&issitemanager=1";
        }

        // Initialize tabs
        InitTabs("mergeContent");
        SetTab(0, GetString("om.contact.mergesuggested"), "Merge_Suggested.aspx?contactid=" + QueryHelper.GetInteger("contactId", 0) + siteManagerParam, "parent.parent.frames['menu'].SetHelpTopic('helpTopic', 'onlinemarketing_contact_merge');");
        SetTab(1, GetString("om.contact.mergechoose"), "Merge_Choose.aspx?contactid=" + QueryHelper.GetInteger("contactId", 0) + siteManagerParam, "parent.parent.frames['menu'].SetHelpTopic('helpTopic', 'onlinemarketing_contact_mergechoose');");
        SetTab(2, GetString("om.contact.split"), "Merge_Split.aspx?contactid=" + QueryHelper.GetInteger("contactId", 0) + siteManagerParam, "parent.parent.frames['menu'].SetHelpTopic('helpTopic', 'onlinemarketing_contact_split');");
    }
}