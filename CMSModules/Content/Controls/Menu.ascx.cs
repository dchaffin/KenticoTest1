using System;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.UniMenuConfig;
using CMS.PortalEngine;
using CMS.ExtendedControls;

public partial class CMSModules_Content_Controls_Menu : CMSUserControl
{
    #region "Properties"

    public string SelectedCulture
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedCulture"), null);
        }
        set
        {
            SetValue("SelectedCulture", value);
        }
    }


    public string SelectedMode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedMode"), null);
        }
        set
        {
            SetValue("SelectedMode", value);
        }
    }


    /// <summary>
    /// Gets, sets device.
    /// </summary>
    public String SelectedDevice
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedDevice"), null);
        }
        set
        {
            SetValue("SelectedDevice", value);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize menu
        DataSet siteCulturesDS = CultureInfoProvider.GetSiteCultures(CMSContext.CurrentSiteName);

        Group content = new Group
                            {
                                Caption = GetString("ContentMenu.ContentManagement"),
                                ControlPath = "~/CMSAdminControls/UI/UniMenu/Content/ContentMenu.ascx",
                                CssClass = "ContentMenuGroup"
                            };
        contentMenu.Groups.Add(content);

        CMSUserControl modeMenu = Page.LoadUserControl("~/CMSAdminControls/UI/UniMenu/Content/ModeMenu.ascx") as CMSUserControl;
        if (modeMenu != null)
        {
            modeMenu.ID = "grpMode";
            modeMenu.SetValue("SelectedMode", SelectedMode);
            Group view = new Group
                             {
                                 Caption = GetString("ContentMenu.ViewMode"),
                                 Control = modeMenu,
                                 CssClass = "ContentMenuGroup"
                             };
            contentMenu.Groups.Add(view);
        }

        if (DeviceProfileInfoProvider.IsDeviceProfilesEnabled(CMSContext.CurrentSiteName))
        {
            CMSUserControl devicesMenu = Page.LoadUserControl("~/CMSModules/DeviceProfile/Controls/ProfilesMenuControl.ascx") as CMSUserControl;
            if (devicesMenu != null)
            {
                devicesMenu.ID = "grpDevices";
                devicesMenu.SetValue("SelectedDevice", SelectedDevice);
                Group view = new Group
                                 {
                                     Caption = GetString("ContentMenu.Device"),
                                     Control = devicesMenu,
                                     CssClass = "ContentMenuGroup"
                                 };
                contentMenu.Groups.Add(view);
            }
        }

        // Do not display language menu
        if (!DataHelper.DataSourceIsEmpty(siteCulturesDS) && (siteCulturesDS.Tables[0].Rows.Count > 1))
        {
            CMSUserControl langMenu = Page.LoadUserControl("~/CMSModules/Content/Controls/LanguageMenu.ascx") as CMSUserControl;
            if (langMenu != null)
            {
                langMenu.ID = "grpLang";
                langMenu.SetValue("SelectedCulture", SelectedCulture);
                Group lang = new Group
                                 {
                                     Control = langMenu,
                                     Caption = GetString("contentmenu.language"),
                                     CssClass = "ContentMenuGroup"
                                 };
                contentMenu.Groups.Add(lang);
            }
        }

        Group other = new Group
                          {
                              Caption = GetString("contentmenu.other"),
                              ControlPath = "~/CMSAdminControls/UI/UniMenu/Content/OtherMenu.ascx",
                              CssClass = "ContentMenuGroup"
                          };
        contentMenu.Groups.Add(other);

    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ScriptHelper.RegisterTitleScript(Page, GetString("cmsdesk.ui.content"));
    }

    #endregion
}
