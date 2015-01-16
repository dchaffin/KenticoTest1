using System;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.PortalEngine;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.CMSHelper;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;
using SubMenuItem = CMS.UIControls.UniMenuConfig.SubItem;

public partial class CMSModules_DeviceProfile_Controls_ProfilesMenuControl : CMSUserControl
{
    #region "Variables"

    private DeviceProfileInfo currentDevice = null;
    private bool mUseSmallLanguageButton = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, small buttons are used for menu.
    /// </summary>
    public bool UseSmallLanguageButton
    {
        get
        {
            return mUseSmallLanguageButton;
        }
        set
        {
            mUseSmallLanguageButton = value;
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


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        currentDevice = (SelectedDevice == null) ? DeviceProfileInfoProvider.GetCurrentDeviceProfileInfo(CMSContext.CurrentSiteName, true) : DeviceProfileInfoProvider.GetDeviceProfileInfo(SelectedDevice);
        LoadDevicesMenu();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        DeviceProfileInfo oldDevice = (currentDevice != null ? currentDevice.Clone() : null);
        currentDevice = DeviceProfileInfoProvider.GetCurrentDeviceProfileInfo(CMSContext.CurrentSiteName, true);

        // Reload device menu if profile changed by postback. Apply this, only if no device was selected from outside enviroment via 'SelectedDevice' property.
        if ((SelectedDevice == null) && DeviceChanged(oldDevice, currentDevice))
        {
            buttons.Buttons.Clear();
            LoadDevicesMenu();
            buttons.ReloadButtons();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns true if device changed.
    /// </summary>
    /// <param name="oldDevice">Old device profile info</param>
    /// <param name="newDevice">Current device profile info</param>
    private bool DeviceChanged(DeviceProfileInfo oldDevice, DeviceProfileInfo newDevice)
    {
        if (oldDevice != null)
        {
            if (newDevice != null)
            {
                return oldDevice.ProfileName != newDevice.ProfileName;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (newDevice != null)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Loads device profile menu.
    /// </summary>
    private void LoadDevicesMenu()
    {
        string defaultSmallIconUrl = GetImageUrl("CMSModules/CMS_DeviceProfile/default_list.png");
        string defaultBigIconUrl = GetImageUrl("CMSModules/CMS_DeviceProfile/default.png");
        if ((Page is PortalPage) || (Page is TemplatePage))
        {
            // Display grayscale icon in On-site editing
            defaultBigIconUrl = GetImageUrl("CMSModules/CMS_DeviceProfile/default_grayscale.png");
            defaultSmallIconUrl = defaultBigIconUrl;
        }

        string deviceSmallIconUrl = GetImageUrl("CMSModules/CMS_DeviceProfile/list.png");
        string deviceBigIconUrl = GetImageUrl("CMSModules/CMS_DeviceProfile/module.png");

        string defaultString = HTMLHelper.HTMLEncode(GetString("general.default"));

        MenuItem devMenuItem = new MenuItem
        {
            Text = defaultString,
            Tooltip = defaultString,
            ImagePath = defaultBigIconUrl,
            ImageAltText = defaultString
        };
        SetStyles(devMenuItem);
        buttons.Buttons.Add(devMenuItem);

        // Load enabled profiles
        InfoDataSet<DeviceProfileInfo> ds = DeviceProfileInfoProvider.GetDeviceProfileInfos("ProfileEnabled = 1", "ProfileOrder");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            // Create default item
            SubMenuItem defaultMenuItem = new SubMenuItem
                                              {
                                                  Text = defaultString,
                                                  Tooltip = defaultString,
                                                  ImagePath = defaultSmallIconUrl,
                                                  ImageAltText = defaultString,
                                                  OnClientClick = String.Format("CMSUniMenu.ChangeButton(##BUTTON##, {0}, {1}); ChangeDevice('');", ScriptHelper.GetString(defaultString), ScriptHelper.GetString(ResolveUrl(defaultBigIconUrl)))
                                              };

            devMenuItem.SubItems.Add(defaultMenuItem);

            foreach (DeviceProfileInfo profileInfo in ds.Items)
            {
                string bigIconUrl = null;
                string smallIconUrl = null;
                if (profileInfo.ProfileIconGuid == Guid.Empty)
                {
                    smallIconUrl = deviceSmallIconUrl;
                    bigIconUrl = deviceBigIconUrl;
                }
                else
                {
                    string iconUrl = MetaFileURLProvider.GetMetaFileUrl(profileInfo.ProfileIconGuid, profileInfo.ProfileName);
                    smallIconUrl = URLHelper.UpdateParameterInUrl(iconUrl, "maxsidesize", "16");
                    bigIconUrl = URLHelper.UpdateParameterInUrl(iconUrl, "maxsidesize", "24");
                }

                string profileName = GetString(profileInfo.ProfileDisplayName);
                SubMenuItem menuItem = new SubMenuItem
                                           {
                                               Text = HTMLHelper.HTMLEncode(profileName),
                                               Tooltip = HTMLHelper.HTMLEncode(profileName),
                                               ImagePath = smallIconUrl,
                                               ImageAltText = HTMLHelper.HTMLEncode(profileName),
                                               OnClientClick = String.Format("CMSUniMenu.ChangeButton(##BUTTON##, {0}, {1}); ChangeDevice({2});", ScriptHelper.GetString(profileName), ScriptHelper.GetString(ResolveUrl(bigIconUrl)), ScriptHelper.GetString(profileInfo.ProfileName))
                                           };

                devMenuItem.SubItems.Add(menuItem);

                // Update main button if current device profile is equal currently processed profile
                if ((currentDevice != null) && (currentDevice.ProfileName.CompareToCSafe(profileInfo.ProfileName, true) == 0))
                {
                    devMenuItem.Text = HTMLHelper.HTMLEncode(profileName);
                    devMenuItem.Tooltip = HTMLHelper.HTMLEncode(profileName);
                    devMenuItem.ImagePath = bigIconUrl;
                    devMenuItem.ImageAltText = HTMLHelper.HTMLEncode(profileName);
                }
            }
        }
    }


    /// <summary>
    /// Applies styles to menu item according to UseSmallLanguageButton property.
    /// </summary>
    /// <param name="menuItem">Menu item to apply styles to.</param>
    private void SetStyles(MenuItem menuItem)
    {
        if (UseSmallLanguageButton)
        {
            menuItem.CssClass = "SmallButton";
            menuItem.ImageAlign = ImageAlign.AbsMiddle;
            menuItem.MinimalWidth = 80;
        }
        else
        {
            menuItem.CssClass = "BigButton";
            menuItem.ImageAlign = ImageAlign.Top;
            menuItem.MinimalWidth = 48;
        }
    }

    #endregion
}
