using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;


public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_CustomTable_Edit : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    private string mItemType = null;

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


    protected void Page_Load(object sender, EventArgs e)
    {
        // Show panel with message how to enable indexing
        ucDisabledModule.SettingsKeys = "CMSSearchIndexingEnabled";
        ucDisabledModule.InfoText = GetString("srch.searchdisabledinfo");

        // Init controls
        if (!RequestHelper.IsPostBack())
        {
            LoadControls();
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
                customTableSelector.Value = sisi.ClassNames;
                txtWhere.TextArea.Text = sisi.WhereCondition;
            }
        }
    }


    /// <summary>
    /// Stores data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
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
                sisi.ClassNames = Convert.ToString(customTableSelector.Value);
                sisi.WhereCondition = txtWhere.TextArea.Text.Trim();

                DataClassInfo dci = DataClassInfoProvider.GetDataClass(sisi.ClassNames);
                if (dci != null)
                {
                    sisi.SetValue("DisplayName", dci.ClassDisplayName);
                }

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