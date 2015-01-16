using System;
using System.Web.UI;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_Custom : CMSAdminControl, IPostBackEventHandler
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


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
            if (sii != null)
            {
                SearchIndexSettings sis = sii.IndexSettings;
                SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(SearchHelper.CUSTOM_INDEX_DATA);

                // Create new
                if (sisi != null)
                {
                    txtAssembly.Text = ValidationHelper.GetString(sisi.GetValue("AssemblyName"), String.Empty);
                    txtClassName.Text = ValidationHelper.GetString(sisi.GetValue("ClassName"), String.Empty);
                    txtData.TextArea.Text = ValidationHelper.GetString(sisi.GetValue("CustomData"), String.Empty);
                }
            }
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        string errorMessage = new Validator().NotEmpty(txtAssembly.Text.Trim(), GetString("srch.index.assemblyempty")).NotEmpty(txtClassName.Text.Trim(), GetString("srch.index.classnameempty")).Result;

        if (String.IsNullOrEmpty(errorMessage))
        {
            SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
            if (sii != null)
            {
                SearchIndexSettings sis = sii.IndexSettings;
                SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(SearchHelper.CUSTOM_INDEX_DATA);

                // Create new
                if (sisi == null)
                {
                    sisi = new SearchIndexSettingsInfo();
                    sisi.ID = SearchHelper.CUSTOM_INDEX_DATA;
                }

                sisi.SetValue("AssemblyName", txtAssembly.Text.Trim());
                sisi.SetValue("ClassName", txtClassName.Text.Trim());
                sisi.SetValue("CustomData", txtData.TextArea.Text);

                // Update settings item
                sis.SetSearchIndexSettingsInfo(sisi);
                // Update xml value
                sii.IndexSettings = sis;

                SearchIndexInfoProvider.SetSearchIndexInfo(sii);

                ShowChangesSaved();

                // Redirect to edit mode
                if (smartSearchEnabled)
                {
                    ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
                }
            }
        }
        else
        {
            ShowError(errorMessage);
            return;
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