using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.WebAnalytics;

// Edited object
[EditedObject(OnlineMarketingObjectType.CONTACT, "contactId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Activities : CMSContactManagementContactsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedObject != null)
        {
            ContactInfo ci = (ContactInfo)EditedObject;

            // Check permission
            CheckReadPermission(ci.ContactSiteID);

            bool isGlobal = (ci.ContactSiteID == 0);
            bool isMerged = (ci.ContactMergedWithContactID > 0);

            // Show warning if activity logging is disabled
            string siteName = SiteInfoProvider.GetSiteName(ci.ContactSiteID);

            ucDisabledModule.SettingsKeys = "CMSEnableOnlineMarketing";
            ucDisabledModule.InfoText = GetString("om.onlinemarketing.disabled");
            ucDisabledModule.ParentPanel = pnlDis;

            pnlDis.Visible = !isGlobal && !ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(siteName);

            // Show IP addresses if enabled
            listElem.ShowIPAddressColumn = ActivitySettingsHelper.IPLoggingEnabled(siteName);
            listElem.ShowSiteNameColumn = IsSiteManager && isGlobal;

            // Restrict WHERE condition for activities of current site (if not in site manager)
            if (!IsSiteManager)
            {
                listElem.SiteID = CMSContext.CurrentSiteID;
            }
            else
            {
                // Show all records in Site Manager
                listElem.SiteID = UniSelector.US_ALL_RECORDS;
            }

            listElem.ContactID = ci.ContactID;
            listElem.IsMergedContact = isMerged;
            listElem.IsGlobalContact = isGlobal;
            listElem.ShowContactNameColumn = isGlobal;
            listElem.ShowSiteNameColumn = IsSiteManager && isGlobal;
            listElem.ShowRemoveButton = !isMerged && !isGlobal;
            listElem.OrderBy = "ActivityCreated DESC";

            // Init header action for new custom activities only if contact is not global, a custom activity type exists and user is authorized to manage activities
            if (!isGlobal && ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(siteName) && CMSContext.CurrentUser.IsAuthorizedPerResource(ModuleEntry.CONTACTMANAGEMENT, "ManageActivities"))
            {
                // Disable manual creation of activity if no custom activity type is available
                DataSet ds = ActivityTypeInfoProvider.GetActivityTypes("ActivityTypeIsCustom=1 AND ActivityTypeEnabled=1 AND ActivityTypeManualCreationAllowed=1", null, 1, "ActivityTypeID");
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    // Prepare target URL
                    string url = ResolveUrl(string.Format("~/CMSModules/ContactManagement/Pages/Tools/Activities/Activity/New.aspx?contactId={0}", ci.ContactID));
                    url = AddSiteQuery(url, ci.ContactSiteID);

                    // Init header action
                    HeaderAction action = new HeaderAction()
                    {
                        Text = GetString("om.activity.newcustom"),
                        ImageUrl = GetImageUrl("Objects/OM_Activity/add.png"),
                        RedirectUrl = url
                    };
                    CurrentMaster.HeaderActions.ActionsList.Add(action);
                }
            }

            if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
            {
                // Display 'Save' message after new custom activity was created
                ShowChangesSaved();
            }
        }
    }
}