using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_Forum_Edit : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    private string mItemType = null;
    private FormEngineUserControl selForum = null;
    private bool smartSearchEnabled = SettingsKeyProvider.GetBoolValue("CMSSearchIndexingEnabled");

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets item type.
    /// </summary>
    public string ItemType
    {
        get
        {
            return mItemType;
        }
        set
        {
            mItemType = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Show panel with message how to enable indexing
        ucDisabledModule.SettingsKeys = "CMSSearchIndexingEnabled";
        ucDisabledModule.InfoText = GetString("srch.searchdisabledinfo");

        // Module forums is not available
        if (!(ModuleEntry.IsModuleRegistered(ModuleEntry.FORUMS) && ModuleEntry.IsModuleLoaded(ModuleEntry.FORUMS)))
        {
            return;
        }

        selForum = this.LoadUserControl("~/CMSModules/Forums/FormControls/ForumSelector.ascx") as FormEngineUserControl;
        if (selForum != null)
        {
            selForum.IsLiveSite = false;
            plcForumSelector.Controls.Add(selForum);
        }

        selSite.AllowAll = false;
        selSite.UseCodeNameForSelection = true;

        string siteWhere = String.Empty;

        DataSet ds = SearchIndexSiteInfoProvider.GetIndexSites("SiteID", "IndexID = " + ItemID, null, 0);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                siteWhere += dr["SiteID"] + ",";
            }

            siteWhere = "," + siteWhere;

            // Preselect current site if it is assigned to index
            if (!RequestHelper.IsPostBack() && siteWhere.Contains("," + CMSContext.CurrentSiteID + ","))
            {
                selSite.Value = CMSContext.CurrentSiteName;
            }

            siteWhere = siteWhere.Trim(',');
            siteWhere = "SiteID IN (" + siteWhere + ")";
            selSite.UniSelector.WhereCondition = siteWhere;
        }
        else
        {
            selSite.Enabled = false;
            selForum.Enabled = false;
            btnOk.Enabled = false;

            ShowError(GetString("srch.index.nositeselected"));
        }

        // Set default vaules for forum selector
        selForum.SetValue("selectionmode", SelectionModeEnum.MultipleTextBox);
        selForum.SetValue("DisplayAdHocOption", false);
        selForum.SetValue("SiteName", Convert.ToString(selSite.Value));

        // Set events and default values for site selector
        selSite.UniSelector.OnSelectionChanged += new EventHandler(UniSelector_OnSelectionChanged);
        selSite.DropDownSingleSelect.AutoPostBack = true;

        LoadControls();

        if (ItemType == SearchIndexSettingsInfo.TYPE_ALLOWED)
        {
            selSite.AllowAll = true;
        }

        // Init controls
        if (!RequestHelper.IsPostBack())
        {
            selForum.Enabled = true;

            string siteName = ValidationHelper.GetString(selSite.Value, String.Empty);
            if (String.IsNullOrEmpty(siteName) || (siteName == "-1"))
            {
                selForum.Enabled = false;
            }
            else
            {
                selForum.SetValue("SiteName", siteName);
                SetControlsStatus(false);
            }
        }
    }


    private void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SetControlsStatus(true);
    }


    /// <summary>
    /// Enable or disable controls with dependence on current settings.
    /// </summary>
    /// <param name="clear">Indicates whether selector value should be cleared</param>
    protected void SetControlsStatus(bool clear)
    {
        if (clear)
        {
            selForum.Value = String.Empty;
        }

        selForum.Enabled = true;

        string siteName = ValidationHelper.GetString(selSite.Value, String.Empty);
        if (String.IsNullOrEmpty(siteName) || (siteName == "-1"))
        {
            selForum.Enabled = false;
        }
        else
        {
            selForum.SetValue("SiteName", siteName);
        }
    }


    /// <summary>
    /// Resets all boxes.
    /// </summary>
    public void LoadControls()
    {
        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);

        // If we are editing existing search index
        if (sii != null)
        {
            SearchIndexSettings sis = new SearchIndexSettings();
            sis.LoadData(sii.IndexSettings.GetData());
            SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(ItemGUID);
            if (sisi != null)
            {
                if (!RequestHelper.IsPostBack())
                {
                    selSite.Value = sisi.SiteName;
                    selForum.SetValue("SiteName", sisi.SiteName);
                    selForum.Value = sisi.ForumNames;
                }
                ItemType = sisi.Type;
            }
        }

        plcForumsInfo.Visible = true;
        if (ItemType == SearchIndexSettingsInfo.TYPE_EXLUDED)
        {
            plcForumsInfo.Visible = false;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Stores data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Perform validation
        string errorMessage = new Validator().NotEmpty(selSite.Value, GetString("srch.err.emptysite")).Result;

        if (String.IsNullOrEmpty(errorMessage) && (ItemType == SearchIndexSettingsInfo.TYPE_EXLUDED) && String.IsNullOrEmpty(Convert.ToString(selForum.Value)))
        {
            errorMessage = GetString("srch.err.emptyforum");
        }

        if (String.IsNullOrEmpty(errorMessage))
        {
            SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
            if (sii != null)
            {
                SearchIndexSettings sis = sii.IndexSettings;
                SearchIndexSettingsInfo sisi;

                // If we are updating existing Search Index Settings Info
                if (ItemGUID != Guid.Empty)
                {
                    sisi = sis.GetSearchIndexSettingsInfo(ItemGUID);
                }
                // If we are creating new Search Index Settings Info
                else
                {
                    sisi = new SearchIndexSettingsInfo();
                    sisi.ID = Guid.NewGuid();
                    sisi.Type = ItemType;
                }

                // Save values
                if (sisi != null)
                {
                    string siteName = selSite.Value.ToString();
                    if (siteName == "-1")
                    {
                        siteName = String.Empty;
                    }

                    sisi.SiteName = siteName;
                    sisi.ForumNames = selForum.Value.ToString();

                    // Update settings item
                    sis.SetSearchIndexSettingsInfo(sisi);
                    // Update xml value
                    sii.IndexSettings = sis;
                    SearchIndexInfoProvider.SetSearchIndexInfo(sii);
                    ItemGUID = sisi.ID;

                    ShowChangesSaved();

                    // Redirect to edit mode
                    if (smartSearchEnabled)
                    {
                        ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
                    }
                }
                // Error loading SearchIndexSettingsInfo
                else
                {
                    ShowError(GetString("srch.err.loadingsisi"));
                }
            }
            // Error loading SearchIndexInfo
            else
            {
                ShowError(GetString("srch.err.loadingsii"));
            }
        }
        else
        {
            ShowError(errorMessage);
        }
    }

    #endregion


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "saved")
        {
            if (SearchHelper.CreateRebuildTask(ItemID))
            {
                ShowInformation(GetString("srch.index.rebuildstarted"));
            }
            else
            {
                ShowError(GetString("index.nocontent"));
            }
        }
    }

    #endregion
}