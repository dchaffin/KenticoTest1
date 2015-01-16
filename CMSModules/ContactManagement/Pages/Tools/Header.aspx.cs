using System;
using System.Text;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.Controls.Configuration;

public partial class CMSModules_ContactManagement_Pages_Tools_Header : CMSContactManagementPage
{
    #region "Variables"

    private int selectedTab;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets SiteID.
    /// </summary>
    public override int SiteID
    {
        get
        {
            if (IsSiteManager)
            {
                // Site selector has '0' as 'none' or 'global' record. Return '-4' as 'global' record instead.
                if (siteSelector.SiteID == UniSelector.US_NONE_RECORD)
                {
                    return UniSelector.US_GLOBAL_RECORD;
                }
                return siteSelector.SiteID;
            }
            else
            {
                return base.SiteID;
            }
        }
    }

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            selectedTab = QueryHelper.GetInteger("selectedTab", 0);
        }

        // CMSDesk
        if (!IsSiteManager)
        {
            pnlActions.Visible = false;
        }
        // Site manager
        else
        {
            // Set site selector
            siteSelector.OnlyRunningSites = false;
            siteSelector.AllowEmpty = false;
            siteSelector.AllowAll = true;
            siteSelector.AllowGlobal = true;
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.UniSelector.SpecialFields = new string[1,2] { { GetString("general.global"), "0" } };
            siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
            siteSelector.Reload(true);
        }

        string append = null;
        if (IsSiteManager)
        {
            ScriptHelper.RegisterStartupScript(this, GetType(), "selectedSiteId", ScriptHelper.GetScript(string.Format("var selectedSiteId = {0};", SiteID)));
            append = "?siteid=' + selectedSiteId+'&issitemanager=1";
        }
        
        if (IsSiteManager)
        {
            // Activities tab
            BasicTabControlMenu.AddTab(new TabItem()
            {
                Text = GetString("om.activities"),
                OnClientClick = "SetHelpTopic('helpTopic', 'onlinemarketing_activities'); return false;",
                RedirectUrl = "Activities/Frameset.aspx" + append
            });
        }

        // Contacts tab
        BasicTabControlMenu.AddTab(new TabItem()
        {
            Text = GetString("om.contact.list"),
            OnClientClick = "SetHelpTopic('helpTopic', 'onlinemarketing_contact_list'); return false;",
            RedirectUrl = "Contact/List.aspx" + append
        });

        // Pending contacts tab
        BasicTabControlMenu.AddTab(new TabItem()
        {
            Text = GetString("om.ui.pendingcontacts"),
            OnClientClick = "SetHelpTopic('helpTopic', 'onlinemarketing_pendingcontact_list'); return false;",
            RedirectUrl = "PendingContacts/List.aspx" + append
        });

        // Accounts tab
        BasicTabControlMenu.AddTab(new TabItem()
        {
            Text = GetString("om.account.list"),
            OnClientClick = "SetHelpTopic('helpTopic', 'onlinemarketing_account_list'); return false;",
            RedirectUrl = "Account/List.aspx" + append
        });

        // Contact groups tab
        BasicTabControlMenu.AddTab(new TabItem()
        {
            Text = GetString("om.contactgroup.list"),
            OnClientClick = "SetHelpTopic('helpTopic', 'onlinemarketing_contactgroup_list'); return false;",
            RedirectUrl = "ContactGroup/List.aspx" + append
        });

        // Configuration tab
        BasicTabControlMenu.AddTab(new TabItem()
        {
            Text = GetString("om.configuration"),
            OnClientClick = "SetHelpTopic('helpTopic', 'onlinemarketing_configuration'); return false;",
            RedirectUrl = "Configuration/Frameset.aspx" + append
        });

        BasicTabControlMenu.UrlTarget = "contactManagementContent";
        BasicTabControlMenu.SelectedTab = selectedTab;

        titleElem.TitleText = GetString("om.contactmanagement");
        titleElem.TitleImage = GetImageUrl("Objects/OM_ContactManagement/object.png");
        titleElem.HelpTopicName = "contactmanagement_general";
        titleElem.HelpName = "helpTopic";
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        StringBuilder script = new StringBuilder();
        script.Append(@"
function OnSelectedIndex() {
selectedTab = document.getElementById('TabControlSelItemNo');
if ((selectedTab != null) && (typeof(selectedSiteId) != 'undefined')) {
");

        int tabIndex = 0;

        // Activities tab is for site manager only
        if (IsSiteManager)
        {
            script.Append(@"if (selectedTab.value == '", tabIndex++, @"') {
     parent.frames['contactManagementContent'].location = 'Activities/Frameset.aspx?siteid=' + selectedSiteId + '&issitemanager=1';
   } else ");
        }

        // Other tabs are in CMSDesk as well
        string sm = "issitemanager=" + (IsSiteManager ? 1 : 0);
        script.Append(@"if (selectedTab.value == '", tabIndex++, @"') {
     parent.frames['contactManagementContent'].location = 'Contact/List.aspx?siteid=' + selectedSiteId + '&", sm, @"';
   } else if (selectedTab.value == '", tabIndex++, @"') {
     parent.frames['contactManagementContent'].location = 'PendingContacts/List.aspx?siteid=' + selectedSiteId + '&", sm, @"';
   } else if (selectedTab.value == '", tabIndex++, @"') {   
     parent.frames['contactManagementContent'].location = 'Account/List.aspx?siteid=' + selectedSiteId + '&", sm, @"';
   } else if (selectedTab.value == '", tabIndex++, @"') {
     parent.frames['contactManagementContent'].location = 'ContactGroup/List.aspx?siteid=' + selectedSiteId + '&", sm, @"';
   } else {
     parent.frames['contactManagementContent'].location = 'Configuration/Frameset.aspx?siteid=' + selectedSiteId + '&", sm, @"';
   }
}}
OnSelectedIndex();");

        ScriptHelper.RegisterStartupScript(this, GetType(), "OnSelectedIndex", ScriptHelper.GetScript(script.ToString()));
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Set panel menu breadcrumbs
        if (!IsSiteManager)
        {
            string[,] breadcrumbs;

            // Create new breadcrumbs
            if (titleElem.Breadcrumbs == null)
            {
                breadcrumbs = new string[2,4];

                breadcrumbs[0, 0] = "Tools";
                breadcrumbs[0, 1] = "~/CMSDesk/Tools/Menu.aspx";
                breadcrumbs[0, 2] = "toolscontent";

                if (!String.IsNullOrEmpty(titleElem.TitleText))
                {
                    breadcrumbs[1, 0] = titleElem.TitleText;
                    breadcrumbs[1, 1] = "";
                    breadcrumbs[1, 2] = "";
                }

                // Register frame height correction script
                string resizeScript = @"
var frames = window.parent.frames;
var rows = window.parent.document.body.rows.replace(' ', '').split(',');
for (var i = 0; i < frames.length; i++) {
    if ((frames[i].name == window.name) && (rows[i] != '*')) {
        rows[i] = parseInt(rows[i], 10) + 30;
        window.parent.document.body.rows = rows.join(',');
        break;
    }
}";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(String), "FrameHeightCorrection", ScriptHelper.GetScript(resizeScript));
                titleElem.Breadcrumbs = breadcrumbs;
            }
        }
    }

    #endregion
}