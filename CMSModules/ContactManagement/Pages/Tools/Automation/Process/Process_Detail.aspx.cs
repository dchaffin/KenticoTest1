using System;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;
using CMS.ExtendedControls;
using CMS.OnlineMarketing;

[RegisterTitle("ma.automationstate.edit")]

[Breadcrumbs(2)]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Process_Detail : CMSAutomationPage
{
    #region "Variables"

    private WorkflowInfo workflow = null;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        autoMan.OnAfterAction += AutomationManager_OnAfterAction;
        autoMan.StateObjectID = QueryHelper.GetInteger("stateid", 0);

        workflow = autoMan.Process;

        if (workflow != null)
        {
            menuElem.OnClientStepChanged = ClientScript.GetPostBackEventReference(pnlUp, null);
        }
    }

    protected void AutomationManager_OnAfterAction(object sender, CMS.ExtendedControls.AutomationManagerEventArgs e)
    {
        switch (e.ActionName)
        {
            case ComponentEvents.AUTOMATION_REMOVE:
            case ComponentEvents.AUTOMATION_START:
                URLHelper.Redirect(ResolveUrl(GetListingUrl()));
                break;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CurrentMaster.Title.Breadcrumbs[1, 0] = HTMLHelper.HTMLEncode(ContactInfoProvider.GetContactFullName(autoMan.ObjectID));
        CurrentMaster.Title.Breadcrumbs[0, 0] = GetString("ma.contact.contacts");
        CurrentMaster.Title.Breadcrumbs[0, 1] = ResolveUrl(GetListingUrl());

        pnlContainer.Enabled = !autoMan.ProcessingAction;
    }


    /// <summary>
    /// Gets URL of listing of processes of current contact.
    /// </summary>
    /// <returns>URL of listing</returns>
    private string GetListingUrl()
    {
        return "~/CMSModules/ContactManagement/Pages/Tools/Automation/Process/Tab_Contacts.aspx?processid=" + autoMan.Process.WorkflowID + (IsSiteManager ? "&issitemanager=1" : String.Empty) + "&siteid=" + QueryHelper.GetInteger("siteid", 0) ;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register the scripts
        if (!autoMan.RefreshActionContent)
        {
            ScriptHelper.RegisterProgress(Page);
        }
    }

    #endregion
}
