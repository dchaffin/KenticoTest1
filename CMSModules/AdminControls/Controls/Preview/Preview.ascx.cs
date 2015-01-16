using System;
using System.Data;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.SiteProvider;
using CMS.PortalEngine;

public partial class CMSModules_AdminControls_Controls_Preview_Preview : CMSPreviewControl
{
    #region "Properties"

    /// <summary>
    /// If true, controls values are loaded from cache (even when postback)
    /// </summary>
    public bool SetControls
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        imgShow.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/refresh.png");
        imgShow.ToolTip = GetString("general.refresh");
        ucPath.UseImageButtons = true;

        TrimPreviewValues();
        ProcessLanguageAndDeviceChange();
        pnlDevice.Visible = DeviceProfileInfoProvider.IsDeviceProfilesEnabled(CMSContext.CurrentSiteName);

        if (PreviewObjectName != String.Empty)
        {
            String script = "function ChangeLanguage(culture) {" + ControlsHelper.GetPostBackEventReference(btnLanguage, "#").Replace("'#'", "culture") + "}";
            script += "function ChangeDevice(device) {" + ControlsHelper.GetPostBackEventReference(btnDevice, "#").Replace("'#'", "device") + "}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(String), "PostbackScript", ScriptHelper.GetScript(script));
        }
    }


    /// <summary>
    /// Get values from session and set it to controls
    /// </summary>
    private void TrimPreviewValues()
    {
        if (String.IsNullOrEmpty(PreviewObjectName))
        {
            return;
        }

        string[] parameters = null;

        // For dialog mode first time load or when preview is initialized - set actual settings, not stored 
        if (LoadSessionValues)
        {
            // Get values from session
            parameters = SessionHelper.GetValue(PreviewObjectName) as string[] ?? PreviewObjectPreferredDocument;
        }

        if ((parameters != null) && (parameters.Length == 4))
        {
            // Store SiteID for path selector
            ucPath.SiteID = String.IsNullOrEmpty(parameters[1]) ? CMSContext.CurrentSiteID : ValidationHelper.GetInteger(parameters[1], 0);
            if (!RequestHelper.IsPostBack() || SetControls)
            {
                ucPath.Value = parameters[0];
            }

            ucSelectCulture.SelectedCulture = String.IsNullOrEmpty(parameters[2]) ? CMSContext.PreferredCultureCode : parameters[2];
        }
        else
        {
            if ((parameters == null) || (parameters.Length != 4))
            {
                // First time load
                parameters = new String[4];
                parameters[1] = CMSContext.CurrentSiteID.ToString();
                parameters[3] = CMSContext.CurrentDeviceProfileName;

                if (!RequestHelper.IsPostBack() || SetControls)
                {
                    parameters[2] = CMSContext.PreferredCultureCode;
                    parameters[0] = DefaultPreviewPath;
                }
            }

            // First try get alias path from property
            String aliasPath = DefaultAliasPath;
            if (String.IsNullOrEmpty(aliasPath))
            {
                // Then get path settings from query string (used in CMS Desk)
                aliasPath = QueryHelper.GetString("aliaspath", String.Empty);
            }

            if (!String.IsNullOrEmpty(aliasPath))
            {
                parameters[0] = aliasPath;
            }

            // Set selectors by parameters value
            ucPath.Value = parameters[0];
            ucPath.SiteID = ValidationHelper.GetInteger(parameters[1], CMSContext.CurrentSiteID);
            ucSelectCulture.SelectedCulture = parameters[2];
        }

        // Store new values for dialog mode
        if (!LoadSessionValues)
        {
            SessionHelper.SetValue(PreviewObjectName, parameters);
            StoreNewPreferredDocument(parameters);
        }

        ucPath.Config.ContentSites = AvailableSitesEnum.All;
        ucSelectCulture.SiteID = ucPath.SiteID;
        ucSelectDevice.SelectedDevice = parameters[3];

        ucPath.PathTextBox.WatermarkText = GetString("general.pleaseselectdots");
        ucPath.PathTextBox.WatermarkCssClass = "WatermarkText";
    }


    /// <summary>
    /// Registers script for resfresh preview
    /// </summary>
    public void RegisterRefreshPreviewScript()
    {
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "RefreshScript", ScriptHelper.GetScript("refreshPreviewParam('" + GetPreviewURL() + "');"));
    }


    /// <summary>
    /// Set new values to session for preview
    /// </summary>
    private void UpdatePreview()
    {
        string[] parameters = SessionHelper.GetValue(PreviewObjectName) as string[] ?? new String[4];

        // Store new values based on settings in controls
        int siteID = ucPath.SiteID;
        parameters[0] = ValidationHelper.GetString(ucPath.Value, String.Empty);
        parameters[1] = (siteID != 0) ? siteID.ToString() : parameters[1];
        parameters[2] = ValidationHelper.GetString(ucSelectCulture.SelectedCulture, CMSContext.PreferredCultureCode);
        parameters[3] = ValidationHelper.GetString(ucSelectDevice.SelectedDevice, String.Empty);

        SessionHelper.SetValue(PreviewObjectName, parameters);
        StoreNewPreferredDocument(parameters);
    }


    protected void ucPath_pathchanged(object sender, EventArgs ea)
    {
        string[] parameters = SessionHelper.GetValue(PreviewObjectName) as string[];

        // Get old site ID
        int SiteID = CMSContext.CurrentSiteID;
        if ((parameters != null) && (parameters.Length == 4))
        {
            SiteID = ValidationHelper.GetInteger(parameters[1], 0);
        }

        // If site ID changed - register postback for reload update panel with culture selector
        if (SiteID != ucPath.SiteID)
        {
            String script = Page.ClientScript.GetPostBackEventReference(imgShow, "reloadculture");
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "UpdateImageScript", ScriptHelper.GetScript(script));

            // If cultures from other site is shown
            DataSet siteCulturesDS = CultureInfoProvider.GetSiteCultures(SiteInfoProvider.GetSiteName(ucPath.SiteID));
            if (!DataHelper.DataSourceIsEmpty(siteCulturesDS))
            {
                DataTable siteCultures = siteCulturesDS.Tables[0];
                // SelectedCulture may not be in site culture list
                DataRow[] dr = siteCulturesDS.Tables[0].Select("CultureCode= '" + ucSelectCulture.SelectedCulture + "'");
                if (dr.Length == 0)
                {
                    // In such case, select first site's culture 
                    ucSelectCulture.SelectedCulture = ValidationHelper.GetString(siteCultures.Rows[0]["CultureCode"], CMSContext.CurrentUser.PreferredCultureCode);
                }
            }

        }

        UpdatePreview();
        RegisterRefreshPreviewScript();
    }


    protected void btnShow_clicked(object sender, EventArgs ea)
    {
        // If path was changed reload culture selector in case site was changed also
        string arg = Request["__EVENTARGUMENT"];
        if (arg != "reloadculture")
        {
            UpdatePreview();
            RegisterRefreshPreviewScript();
        }
    }


    protected void CurrentSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        UpdatePreview();
        RegisterRefreshPreviewScript();
    }


    private void ProcessLanguageAndDeviceChange()
    {
        string arg = Request["__EVENTARGUMENT"];
        string target = Request["__EVENTTARGET"];

        if (target == btnLanguage.UniqueID)
        {
            ucSelectCulture.SelectedCulture = arg;
            UpdatePreview();
            RegisterRefreshPreviewScript();
        }

        if (target == btnDevice.UniqueID)
        {
            ucSelectDevice.SelectedDevice = arg;
            UpdatePreview();
            RegisterRefreshPreviewScript();
        }
    }

    #endregion
}
