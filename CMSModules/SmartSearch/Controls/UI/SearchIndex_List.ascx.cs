using System;
using System.Data;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_List : CMSAdminListControl
{
    #region "Variables"

    private bool disableActions = false;

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

    #endregion


    #region "Methods and events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Show panel with message how to enable indexing
        if (!SettingsKeyProvider.GetBoolValue("CMSSearchIndexingEnabled"))
        {
            // Set disabled module info
            ucDisabledModule.SettingsKeys = "CMSSearchIndexingEnabled";
            ucDisabledModule.InfoText = GetString("srch.searchdisabledinfo");
            disableActions = true;
        }

        // Setup the grid
        UniGrid.OnAction += new OnActionEventHandler(UniGrid_OnAction);
        UniGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(UniGrid_OnExternalDataBound);
        UniGrid.ZeroRowsText = GetString("general.nodatafound");
        UniGrid.OrderBy = "IndexDisplayName";

        string indexPath = Path.Combine(SettingsKeyProvider.WebApplicationPhysicalPath, "App_Data\\CMSModules\\SmartSearch\\");
        if (indexPath.Length > SearchHelper.MAX_INDEX_PATH)
        {
            pnlPathTooLong.Visible = true;
            disableActions = true;
        }
    }


    /// <summary>
    /// On external databound.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>    
    private object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
                // Get index status
            case "indexstatus":
                int indexID = ValidationHelper.GetInteger(parameter, 0);
                SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(indexID);
                if (sii != null)
                {
                    return GetString("srch.status." + sii.IndexStatus.ToString());
                }
                break;

            case "indextype":
                string type = ValidationHelper.GetString(parameter, String.Empty);

                switch (type.ToLowerCSafe())
                {
                        // Documents
                    case PredefinedObjectType.DOCUMENT:
                        return GetString("srch.index.doctype");

                        // Forums
                    case PredefinedObjectType.FORUM:
                        return GetString("srch.index.forumtype");

                        // Users
                    case PredefinedObjectType.USER:
                        return GetString("srch.index.usertype");

                        // Custom tables
                    case SettingsObjectType.CUSTOMTABLE:
                        return GetString("srch.index.customtabletype");

                        // General index
                    case SearchHelper.GENERALINDEX:
                        return GetString("srch.index.general");

                        // Custom search
                    case "custom_search_index":
                        return GetString("srch.index.customsearch");

                        // Documents crawler
                    case "documents_crawler_index":
                        return GetString("srch.index.doctypecrawler");
                }
                break;
        }

        // Disable all actions
        if (disableActions)
        {
            ImageButton button = null;
            switch (sourceName.ToLowerCSafe())
            {
                case "edit":
                    button = ((ImageButton)sender);
                    button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/editdisabled.png");
                    button.Enabled = false;
                    break;
                case "delete":
                    button = ((ImageButton)sender);
                    button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/deletedisabled.png");
                    button.Enabled = false;
                    break;
                case "rebuild":
                    button = ((ImageButton)sender);
                    button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/rebuilddisabled.png");
                    button.Enabled = false;
                    break;
            }
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGrid_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "edit":
                SelectedItemID = Convert.ToInt32(actionArgument);
                RaiseOnEdit();
                break;

            case "delete":
                // Delete search index info object from database with it's dependencies
                SearchIndexInfoProvider.DeleteSearchIndexInfo(Convert.ToInt32(actionArgument));
                break;

            case "rebuild":
                if (SearchIndexInfoProvider.SearchEnabled)
                {
                    int indexID = ValidationHelper.GetInteger(actionArgument,0);

                    if (SearchHelper.CreateRebuildTask(indexID))
                    {
                        ShowInformation(GetString("srch.index.rebuildstarted"));
                        // Sleep
                        Thread.Sleep(100);
                    }
                    else
                    {
                        ShowError(GetString("index.nocontent"));
                    }
                }
                else
                {
                    ShowError(GetString("srch.index.searchdisabled"));
                }
                break;
        }
    }

    #endregion
}