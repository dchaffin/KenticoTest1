using System;

using CMS.OnlineMarketing;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;
using CMS.CMSHelper;
using CMS.SiteProvider;

// Actions
[Actions(1)]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Contacts : CMSAutomationPage
{
    #region "Variables"

    private int mProcessID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current workflow ID
    /// </summary>
    public int ProcessID
    {
        get
        {
            if (mProcessID <= 0)
            {
                mProcessID = QueryHelper.GetInteger("processid", 0);
            }
            return mProcessID;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set process identifier
        listContacts.ProcessID = ProcessID;
        ucSelector.UniSelector.DialogLabel.ResourceString = "ma.automationprocess.select";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsSiteManager)
        {
            // Init site filter when user is authorized for global and site contacts
            if (AuthorizedForGlobalContacts && AuthorizedForSiteContacts)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;

                // Set site selector
                if (!URLHelper.IsPostback())
                {
                    siteSelector.Visible = false;
                    siteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteid", CMSContext.CurrentSiteID);
                }

                listContacts.SiteID = siteOrGlobalSelector.SiteID;

                if (listContacts.SiteID == UniSelector.US_GLOBAL_OR_SITE_RECORD)
                {
                    lblWarnStart.Visible = true;
                }
            }
            else if (AuthorizedForSiteContacts)
            {
                // User is authorized only for site contacts so set current site id
                listContacts.SiteID = CMSContext.CurrentSiteID;
            }
            else if (AuthorizedForGlobalContacts)
            {
                // User can read only global contacts
                listContacts.SiteID = UniSelector.US_GLOBAL_RECORD;
            }
            else
            {
                // User has no permissions
                RedirectToCMSDeskAccessDenied(ModuleEntry.CONTACTMANAGEMENT, "ReadContacts");
            }
        }
        else
        {
            CurrentMaster.DisplaySiteSelectorPanel = true;

            if (siteSelector.SiteID == 0)
            {
                siteSelector.SiteID = SiteID;
            }

            siteOrGlobalSelector.Visible = false;
            listContacts.SiteID = siteSelector.SiteID;
        }

        // Add Refresh action button
        AddHeaderAction(new HeaderAction()
        {
            ImageUrl = GetImageUrl("Objects/MA_AutomationState/refresh.png"),
            Text = GetString("general.Refresh"),
            RedirectUrl = "Tab_Contacts.aspx?processid=" + listContacts.ProcessID + (IsSiteManager ? "&issitemanager=1" : String.Empty) + "&siteid=" + listContacts.SiteID
        });

        CurrentMaster.Title.HeaderActions.PanelCssClass = "FloatRight";

        InitContactSelector();
    }


    void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        try
        {
            int contactId = ValidationHelper.GetInteger(ucSelector.Value, 0);
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);
            var infoObj = ContactInfoProvider.GetContactInfo(contactId);
            using (CMSActionContext context = new CMSActionContext())
            {
                context.AllowAsyncActions = false;

                manager.StartProcess(infoObj, ProcessID);
            }
        }
        catch (ProcessRecurrenceException ex)
        {
            ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            LogAndShowError("Automation", "STARTPROCESS", ex);
        }

        listContacts.ReloadData();
        pnlUpdate.Update();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes contact selector.
    /// </summary>
    private void InitContactSelector()
    {
        // Initialize contact selector
        ucSelector.UniSelector.SelectionMode = SelectionModeEnum.SingleButton;
        ucSelector.UniSelector.ButtonImage = GetImageUrl("/Objects/MA_AutomationState/add.png");

        // Check permissions
        if (WorkflowStepInfoProvider.CanUserStartAutomationProcess(CurrentUser, SiteInfoProvider.GetSiteName(listContacts.SiteID)) && (listContacts.SiteID != UniSelector.US_GLOBAL_OR_SITE_RECORD))
        {
            ucSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            ucSelector.UniSelector.DialogImage.CssClass = "NewItemImage";
            ucSelector.UniSelector.DialogLink.CssClass = "MenuItemEdit";
            ucSelector.SiteID = listContacts.SiteID;
            ucSelector.IsLiveSite = false;
            ucSelector.IsSiteManager = ContactHelper.IsSiteManager;
            ucSelector.Enabled = true;
        }
        else
        {
            ucSelector.Enabled = false;
            ucSelector.UniSelector.DialogImage.CssClass = "NewItemImageDisabled";
            ucSelector.UniSelector.DialogLink.CssClass = "MenuItemEditDisabled";
        }
    }

    #endregion
}