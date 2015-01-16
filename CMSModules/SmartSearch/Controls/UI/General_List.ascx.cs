using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_SmartSearch_Controls_UI_General_List : CMSAdminControl, IPostBackEventHandler
{
    #region "Variables"

    private bool smartSearchEnabled = SettingsKeyProvider.GetBoolValue("CMSSearchIndexingEnabled");
    private int mItemId = QueryHelper.GetInteger("indexid", 0);

    #endregion


    #region "Properties"

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
    /// Item ID.
    /// </summary>
    public int ItemID
    {
        get
        {
            return mItemId;
        }
        set
        {
            mItemId = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
            if (sii != null)
            {
                SearchIndexSettings sis = sii.IndexSettings;
                SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(SearchHelper.SIMPLE_ITEM_ID);

                if (sisi != null)
                {
                    selObjects.Value = sisi.ClassNames;
                    txtWhere.TextArea.Text = sisi.WhereCondition;
                }
            }
        }


        // Setup the UniSelector - exclude classes which have their special type of a search index
        selObjects.ExcludeSmartSearchClassNames = true;
        selObjects.ShowPleaseSelect = String.IsNullOrEmpty(ValidationHelper.GetString(selObjects.Value, String.Empty));
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Validate - check for selected object name
        if (string.IsNullOrEmpty(selObjects.Value.ToString()))
        {
            ShowError(GetString("srch.index.objectname.required"));
            return;
        }

        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
        if (sii != null)
        {
            SearchIndexSettings sis = sii.IndexSettings;
            SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(SearchHelper.SIMPLE_ITEM_ID);

            // Create new
            if (sisi == null)
            {
                sisi = new SearchIndexSettingsInfo();
                sisi.ID = SearchHelper.SIMPLE_ITEM_ID;
            }

            sisi.ClassNames = ValidationHelper.GetString(selObjects.Value, String.Empty);
            sisi.WhereCondition = txtWhere.TextArea.Text.Trim();

            // Update settings item
            sis.SetSearchIndexSettingsInfo(sisi);
            // Update xml value
            sii.IndexSettings = sis;

            SearchIndexInfoProvider.SetSearchIndexInfo(sii);

            selObjects.ShowPleaseSelect = false;
            selObjects.Reload(true);

            ShowChangesSaved();

            // Display a message
            if (smartSearchEnabled)
            {
                ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
            }
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