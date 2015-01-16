using System;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Controls.Configuration;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

public partial class CMSDesk_Default : CMSDeskPage
{
    #region "Variables"

    private bool exploreTreePermissionMissing = false;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Do not check document Read permission
        CheckDocPermissions = false;

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        OnTabCreated += CMSDesk_Default_OnTabCreated;
        OnCheckTabSecurity += CMSDesk_Default_OnCheckTabSecurity;

        paneFooter.Visible = SettingsKeyProvider.DevelopmentMode;
        InitActions();
    }

    
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        // Register title part
        ScriptHelper.RegisterTitleScript(Page, "CMS Desk");
    }

    #endregion


    #region "Tab events"

    protected bool CMSDesk_Default_OnCheckTabSecurity(ref string resource, ref string permission)
    {
        if (exploreTreePermissionMissing)
        {
            resource = "CMS.Content";
            permission = "ExploreTree";
        }
        return exploreTreePermissionMissing;
    }


    protected TabItem CMSDesk_Default_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Ensure additional permissions to 'Content' tab
        if (element.ElementName.ToLowerCSafe() == "content")
        {
            if (!IsUserAuthorizedPerContent())
            {
                exploreTreePermissionMissing = true;
                return null;
            }
        }
        else if (element.ElementName.ToLowerCSafe() == "ecommerce")
        {
            if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.Ecommerce, ModuleEntry.ECOMMERCE))
            {
                return null;
            }
        }
        else if (element.ElementName.ToLowerCSafe() == "onlinemarketing")
        {
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.ONLINEMARKETING))
            {
                return null;
            }
        }

        return tab;
    }


    protected void InitActions()
    {
        HeaderActions actions = (HeaderActions)ControlsHelper.GetChildControl(layoutElem, typeof(HeaderActions));
        if (actions != null)
        {
            actions.AddAction(new SaveAction(Page));
        }
    }

    #endregion
}