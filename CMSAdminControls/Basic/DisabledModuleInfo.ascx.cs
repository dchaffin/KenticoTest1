using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.SiteProvider;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.WebAnalytics;
using CMS.SettingsProvider;

public partial class CMSAdminControls_Basic_DisabledModuleInfo : CMSUserControl
{
    #region "Variables"

    private String mSettingsKeys = String.Empty;
    private List<String> mInfoTexts = new List<string>();
    private Panel mParentPanel;
    private String mInfoText = String.Empty;
    private bool settingsChecked = false;
    private bool mSettingsEnabled = false;
    private bool mSiteOrGlobal = false;
    private bool mShowButtons = true;
    private bool mAtLeastOne = false;
    private String mSiteObjects = String.Empty;
    private String mGlobalObjects = String.Empty;
    private DisabledModuleScope mKeyScope = DisabledModuleScope.Both;


    #endregion


    #region "Properties"

    /// <summary>
    /// Info text to show
    /// </summary>
    public String InfoText
    {
        get
        {
            return mInfoText;
        }
        set
        {
            mInfoText = value;
        }
    }


    /// <summary>
    /// Scope of key check. It's used only for 'SettingsKeys' property. Do not affect 'GlobalObjects' and 'SiteObjects' properties.
    /// </summary>
    public DisabledModuleScope KeyScope
    {
        get
        {
            return mKeyScope;
        }
        set
        {
            mKeyScope = value;
        }
    }


    /// <summary>
    /// Collection of texts, used for multiple info labels for multiple setting keys
    /// </summary>
    public List<String> InfoTexts
    {
        get
        {
            return mInfoTexts;
        }
        set
        {
            mInfoTexts = value;
        }
    }


    /// <summary>
    /// Get/set sitename. Store sitename to viewstate - in case of save, before sitename is known.
    /// </summary>
    public String SiteName
    {
        get
        {
            string msiteName = ValidationHelper.GetString(ViewState["SiteName"], null);
            return (msiteName == null) ? CMSContext.CurrentSiteName : msiteName;
        }
        set
        {
            ViewState["SiteName"] = value;
        }
    }


    /// <summary>
    /// Settings key delimited by ';'
    /// </summary>
    public String SettingsKeys
    {
        get
        {
            return mSettingsKeys;
        }
        set
        {
            mSettingsKeys = value;
        }
    }


    /// <summary>
    /// Parent panel, used for hiding info row if no module is disabled. Is automatically hidden when no module is disabled.
    /// </summary>
    public Panel ParentPanel
    {
        get
        {
            return mParentPanel;
        }
        set
        {
            mParentPanel = value;
        }
    }


    /// <summary>
    /// If true, global or site settings are check separately
    /// </summary>
    public bool SiteOrGlobal
    {
        get
        {
            return mSiteOrGlobal;
        }
        set
        {
            mSiteOrGlobal = value;
        }
    }


    /// <summary>
    /// This value contains result of settings checking
    /// </summary>
    public bool SettingsEnabled
    {
        get
        {
            return mSettingsEnabled;
        }
        set
        {
            mSettingsEnabled = value;
        }
    }


    /// <summary>
    /// Indicates whether show "site" and "global" buttons
    /// </summary>
    public bool ShowButtons
    {
        get
        {
            return mShowButtons;
        }
        set
        {
            mShowButtons = value;
        }
    }


    /// <summary>
    /// If true, settings are considered as checked if at least one setting is checked. This is used only for displaying general information.
    /// </summary>
    public bool AtLeastOne
    {
        get
        {
            return mAtLeastOne;
        }
        set
        {
            mAtLeastOne = value;
        }
    }


    /// <summary>
    /// List of keys (delimited by ';') - indicates which settings will be tested (and checked) only for site settings
    /// </summary>
    public String SiteObjects
    {
        get
        {
            return mSiteObjects;
        }
        set
        {
            mSiteObjects = value;
        }
    }


    /// <summary>
    /// List of keys (delimited by ';') - indicates which settings will be tested (and checked) only for global settings
    /// </summary>
    public String GlobalObjects
    {
        get
        {
            return mGlobalObjects;
        }
        set
        {
            mGlobalObjects = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        Visible = true;
        pnlGlobalButton.Visible = true;
        pnlSiteButton.Visible = true;

        // Set text for link
        btnGlobal.Text = GetString("module.allowglobal");
        btnSite.Text = GetString("module.allowsite");
        imgWarning.ImageUrl = GetImageUrl("/Others/Messages/Warning.png");
    }


    /// <summary>
    /// Displays info label, if any module is disabled
    /// </summary>
    private bool DisplayErrorText()
    {
        GlobalObjects = ";" + GlobalObjects + ";";
        SiteObjects = ";" + SiteObjects + ";";
        String invalidKeys = String.Empty;
        bool keyDisabled = false;
        bool isAnyKeySite = false;
        bool settingChecked = false;
        bool showSite = false;
        bool showGlobal = false;

        if (!String.IsNullOrEmpty(SettingsKeys))
        {
            // Iterate through all settings keys
            int i = 0;
            foreach (String key in SettingsKeys.Split(';'))
            {
                if (key != String.Empty)
                {
                    String objectKey = ";" + key + ";";
                    bool globalObject = GlobalObjects.Contains(objectKey);
                    bool siteObject = SiteObjects.Contains(objectKey);
                    String siteKeyName = SiteName + "." + key;
                    String keyName = (SiteOrGlobal || globalObject || KeyScope == DisabledModuleScope.Global) ? key : siteKeyName;

                    // If module disabled
                    if (!SettingsKeyProvider.GetBoolValue(keyName) || (siteObject && !SettingsKeyProvider.GetBoolValue(siteKeyName)))
                    {
                        // For siteorglobal settings check site setting separately
                        if (SiteOrGlobal && SettingsKeyProvider.GetBoolValue(siteKeyName))
                        {
                            settingChecked = true;
                            i++;
                            continue;
                        }

                        // If atleastone is checked, info error is set later
                        if (!AtLeastOne)
                        {
                            // If setting is global - hide site button
                            SettingsKeyInfo ski = SettingsKeyProvider.GetSettingsKeyInfo(key);
                            if ((ski != null) && (!ski.KeyIsGlobal))
                            {
                                isAnyKeySite = true;
                            }

                            // Get text (either from collection of text or from single text property)
                            String text = (InfoTexts.Count != 0 && InfoTexts.Count > i) ? InfoTexts[i] : InfoText;

                            // Add new text to label
                            lblText.Text += text;

                            // Add this key to collection of disabled keys
                            invalidKeys = key + ";";

                            // Make this info label visible
                            keyDisabled = true;

                            if (!siteObject && !globalObject)
                            {
                                showSite = (KeyScope != DisabledModuleScope.Global);
                                showGlobal = (KeyScope != DisabledModuleScope.Site);
                            }
                            else
                            {
                                showSite |= siteObject;
                                showGlobal |= globalObject;
                            }
                        }
                    }
                    else
                    {
                        settingChecked = true;
                    }
                }
                i++;
            }
        }

        // If atleastone is set, check if any setting is checked. If no, display warning message
        if (AtLeastOne && !settingChecked)
        {
            keyDisabled = true;
            lblText.Text = InfoText;
        }

        // If parent panel is set, show(hide) it 
        if (ParentPanel != null)
        {
            ParentPanel.Visible = keyDisabled;
        }

        // Show/hide this control if module disabled
        Visible = keyDisabled;

        // Show site button only if any key is site
        pnlSiteButton.Visible = isAnyKeySite;

        // Set result to property
        SettingsEnabled = !keyDisabled;

        pnlSiteButton.Visible &= showSite;
        pnlGlobalButton.Visible &= showGlobal;

        return !keyDisabled;
    }


    protected void btnGlobal_clicked(object sender, EventArgs ea)
    {
        Save(false);
    }


    protected void btnSiteOnly_clicked(object sender, EventArgs ea)
    {
        Save(true);
    }


    /// <summary>
    /// Check settings and return result.
    /// </summary>    
    public bool Check()
    {
        settingsChecked = true;
        return DisplayErrorText();
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!settingsChecked)
        {
            // Display text if some setting is disabled
            DisplayErrorText();
        }

        // Show delimiter if no parent panel is specified
        pnlDelimiter.Visible = (ParentPanel == null);

        if (!CMSContext.CurrentUser.IsGlobalAdministrator || !ShowButtons)
        {
            pnlGlobalButton.Visible = false;
            pnlSiteButton.Visible = false;
        }

        if (pnlGlobalButton.Visible && pnlSiteButton.Visible)
        {
            pnlGlobalButton.CssClass = "DisabledModuleButtons";
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Saves the changed settings 
    /// </summary>
    /// <param name="isSite">Indicates whether changed settings is global or site</param>
    private void Save(bool isSite)
    {
        // This action is permitted only for global administrators
        if (CMSContext.CurrentUser.IsGlobalAdministrator)
        {
            if (!String.IsNullOrEmpty(SettingsKeys))
            {
                String[] keys = SettingsKeys.Split(';');
                foreach (String key in keys)
                {
                    if (key != String.Empty)
                    {
                        String objectKey = ";" + key + ";";
                        String siteKeyName = SiteName + "." + key;
                        bool globalObject = (GlobalObjects.Contains(objectKey) || KeyScope == DisabledModuleScope.Global);
                        bool siteObject = SiteObjects.Contains(objectKey);

                        // If setting is global or site (or both), set global(site) settings no matter what button (site or global) was clicked
                        if (globalObject || siteObject)
                        {
                            if (globalObject)
                            {
                                if (!SettingsKeyProvider.GetBoolValue(key))
                                {
                                    ObjectHelper.SetSettingsKeyValue(key, true);
                                }
                            }

                            if (siteObject)
                            {
                                if (!SettingsKeyProvider.GetBoolValue(siteKeyName))
                                {
                                    ObjectHelper.SetSettingsKeyValue(siteKeyName, true);
                                }
                            }

                            continue;
                        }

                        // Test first if settings is disabled
                        if (!SettingsKeyProvider.GetBoolValue(siteKeyName))
                        {
                            String keyName = isSite ? siteKeyName : key;
                            try
                            {
                                ObjectHelper.SetSettingsKeyValue(keyName, true);
                            }
                            catch (Exception)
                            {
                                if (isSite)
                                {
                                    // Site settings does not exists. Save as global then
                                    ObjectHelper.SetSettingsKeyValue(key, true);
                                }
                            }

                            // If global enabled and site still disabled - enable it also
                            if (!isSite && (KeyScope != DisabledModuleScope.Global))
                            {
                                // If settings not enabled, inherit from global
                                if (!SettingsKeyProvider.GetBoolValue(siteKeyName))
                                {
                                    ObjectHelper.SetSettingsKeyValue(siteKeyName, null);
                                }
                            }
                        }
                    }
                }
            }

            URLHelper.Redirect(URLHelper.CurrentURL);
        }
    }

    #endregion
}


#region "DisabledModuleScopeEnum"

/// <summary>
/// Scope of disabled module key check
/// </summary>
public enum DisabledModuleScope : int
{
    /// <summary>
    /// Both site and global settings are checked
    /// </summary>
    Both = 0,

    /// <summary>
    /// Only site keys are checked
    /// </summary>
    Site = 1,

    /// <summary>
    /// Only global keys are checked
    /// </summary>
    Global = 2,
}

#endregion
