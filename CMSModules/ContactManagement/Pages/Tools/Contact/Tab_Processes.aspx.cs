using System;

using CMS.OnlineMarketing;
using CMS.GlobalHelper;
using CMS.WorkflowEngine;
using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.SiteProvider;

[EditedObject("om.contact", "objectid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Processes : CMSContactManagementContactsPage
{
    #region "Properties"

    /// <summary>
    /// Currently edited contact.
    /// </summary>
    public ContactInfo Contact 
    { 
        get
        {
            return (ContactInfo)EditedObject;
        }
    }

    #endregion

    
    #region "Methods"
    
    /// <summary>
    /// Whether current user is authorized to edit the contact.
    /// </summary>
    /// <returns>True if so</returns>
    private bool IsAuthorized()
    {
        bool isSiteAuthorized = CMSContext.CurrentUser.IsAuthorizedPerResource(ModuleEntry.CONTACTMANAGEMENT, "ReadContacts", SiteInfoProvider.GetSiteName(Contact.ContactSiteID));
        bool isGlobalAuthorized = CMSContext.CurrentUser.IsAuthorizedPerResource(ModuleEntry.CONTACTMANAGEMENT, "ReadGlobalContacts", SiteInfoProvider.GetSiteName(Contact.ContactSiteID));
        bool isContactGlobal = Contact.ContactSiteID == 0;

        return isSiteAuthorized || (isContactGlobal && isGlobalAuthorized);
    }

    #endregion


    #region "Event handlers"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsAuthorized())
        {
            RedirectToAccessDenied(GetString("general.nopermission"));
        }

        // Initialize contact selector
        ucSelector.UniSelector.SelectionMode = SelectionModeEnum.SingleButton;
        ucSelector.UniSelector.ButtonImage = GetImageUrl("/Objects/MA_AutomationState/add.png");

        // Check permissions
        if (WorkflowStepInfoProvider.CanUserStartAutomationProcess(CurrentUser, SiteInfoProvider.GetSiteName(Contact.ContactSiteID)))
        {
            ucSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            ucSelector.UniSelector.DialogImage.CssClass = "NewItemImage";
            ucSelector.UniSelector.DialogLink.CssClass = "MenuItemEdit";
            ucSelector.UniSelector.SetValue("SiteID", SiteID);
            ucSelector.UniSelector.SetValue("IsLiveSite", false);
            ucSelector.IsSiteManager = ContactHelper.IsSiteManager;
            ucSelector.Enabled = true;
        }
        else
        {
            ucSelector.Enabled = false;
            ucSelector.UniSelector.DialogImage.CssClass = "NewItemImageDisabled";
            ucSelector.UniSelector.DialogLink.CssClass = "MenuItemEditDisabled";
        }


        listElem.ObjectID = Contact.ContactID;
        listElem.ObjectType = PredefinedObjectType.CONTACT;
        listElem.EditActionUrl = "Process_Detail.aspx?stateid={0}" + (IsSiteManager ? "&issitemanager=1" : String.Empty);
    }


    void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        try
        {
            int processId = ValidationHelper.GetInteger(ucSelector.Value, 0);
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);
            var infoObj = CMSObjectHelper.GetObjectById(listElem.ObjectType, listElem.ObjectID);
            using (CMSActionContext context = new CMSActionContext())
            {
                context.AllowAsyncActions = false;

                manager.StartProcess(infoObj, processId);
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

        listElem.UniGrid.ReloadData();
        pnlUpdate.Update();
    }

    #endregion
}
