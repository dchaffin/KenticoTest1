using System;
using System.Data;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

using Lucene.Net.Index;
using Lucene.Net.Search;

public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_General : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Constants"

    /// <summary>
    /// Rebuild event name
    /// </summary>
    private const string REBUILD = "rebuild";

    /// <summary>
    /// Optimize event name
    /// </summary>
    private const string OPTIMIZE = "optimize";

    #endregion


    #region "Variables"

    protected SearchIndexInfo sii = null;
    private int codeNameLength = 0;

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

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSSearchIndexingEnabled";
        ucDisabledModule.InfoText = GetString("srch.searchdisabledinfo");

        // Get file size and document count information resource strings        
        pnlInfo.GroupingText = GetString("srch.index.info");

        // Action buttons
        InitializeHeaderActions();

        // Init controls
        rfvCodeName.ErrorMessage = GetString("general.requirescodename");
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvBatchSize.ErrorMessage = GetString("general.requiresvalue");

        // Possible length of path - already taken, +1 because in MAX_INDEX PATH is count codename of length 1
        string indexPath = Path.Combine(SettingsKeyProvider.WebApplicationPhysicalPath, "App_Data\\CMSModules\\SmartSearch\\");
        codeNameLength = SearchHelper.MAX_INDEX_PATH - indexPath.Length + 1;
        pnlContent.Visible = true;
        txtCodeName.MaxLength = codeNameLength;

        // Get search index info
        sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);

        // Set edited object
        if (ItemID > 0)
        {
            EditedObject = sii;
        }

        if (sii != null)
        {
            string indexTypeStr = String.Empty;
            switch (sii.IndexType)
            {
                // Documents
                case PredefinedObjectType.DOCUMENT:
                    indexTypeStr = GetString("srch.index.doctype");
                    break;

                // Forums
                case PredefinedObjectType.FORUM:
                    indexTypeStr = GetString("srch.index.forumtype");
                    break;

                // Users
                case PredefinedObjectType.USER:
                    indexTypeStr = GetString("srch.index.usertype");
                    break;

                // Custom tables
                case SettingsObjectType.CUSTOMTABLE:
                    indexTypeStr = GetString("srch.index.customtabletype");
                    break;

                // General index
                case SearchHelper.GENERALINDEX:
                    indexTypeStr = GetString("srch.index.general");
                    break;

                // Custom search index
                case SearchHelper.CUSTOM_SEARCH_INDEX:
                    indexTypeStr = GetString("srch.index.customsearch");
                    break;

                // Documents crawler
                case SearchHelper.DOCUMENTS_CRAWLER_INDEX:
                    indexTypeStr = GetString("srch.index.doctypecrawler");
                    break;
            }
            lblTypeValue.Text = indexTypeStr;

            stopCustomControl.IndexInfo = sii;
            stopCustomControl.AnalyzerDropDown = drpAnalyzer;
        }

        if (!RequestHelper.IsPostBack())
        {
            LoadControls();
        }

        // Reload info panel
        ReloadInfoPanel();
    }


    /// <summary>
    /// Resets all boxes.
    /// </summary>
    public void LoadControls()
    {
        //Fill drop down list
        DataHelper.FillWithEnum<AnalyzerTypeEnum>(drpAnalyzer, "srch.index.", SearchIndexInfoProvider.AnalyzerEnumToString, true);

        // Fill textboxes        
        if (sii != null)
        {
            txtCodeName.Text = sii.IndexName;
            txtDisplayName.Text = sii.IndexDisplayName;
            drpAnalyzer.SelectedValue = SearchIndexInfoProvider.AnalyzerEnumToString(sii.IndexAnalyzerType);
            txtBatchSize.Text = sii.IndexBatchSize.ToString();

            // Community indexing is not yet supported
            //chkCommunity.Checked = sii.IndexIsCommunityGroup;
        }
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Trim blank space and too long string
        string codeName = txtCodeName.Text.Trim();
        if (codeName.Length > 200)
        {
            codeName = codeName.Substring(0, 200);
        }

        // Get code name
        codeName = ValidationHelper.GetCodeName(codeName, null, null);
        txtCodeName.Text = codeName;

        // Perform validation
        string errorMessage = new Validator().NotEmpty(codeName, rfvCodeName.ErrorMessage)
            .NotEmpty(txtDisplayName.Text, rfvDisplayName.ErrorMessage).Result;

        // Check CodeName for identifier format
        if (!ValidationHelper.IsCodeName(codeName))
        {
            errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        // Check length of code name
        if (codeName.Length > codeNameLength)
        {
            errorMessage = GetString("srch.codenameexceeded");
        }

        if (errorMessage == "")
        {
            // Check code name
            SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(codeName);

            // Get current item
            SearchIndexInfo current = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);


            // Check if code name is unique
            if ((sii == null) || (sii == current))
            {
                // Get original index path
                string originalPath = current.CurrentIndexPath;

                // Set the fields
                current.IndexName = codeName;

                // Trim blank space and too long string
                txtDisplayName.Text = txtDisplayName.Text.Trim();
                if (txtDisplayName.Text.Length > 200)
                {
                    txtDisplayName.Text = txtDisplayName.Text.Substring(0, 200);
                }

                if (current.IndexDisplayName != txtDisplayName.Text)
                {
                    // Refresh a breadcrumb if used in the tabs layout
                    ScriptHelper.RefreshTabHeader(Page, string.Empty);
                }

                current.IndexDisplayName = txtDisplayName.Text;

                // Check if analyzer type is changed
                bool analyzerTypeChanged = false;
                if ((current != null) &&
                    ((current.IndexAnalyzerType != SearchIndexInfoProvider.AnalyzerCodenameToEnum(drpAnalyzer.SelectedValue)) ||
                     (CMSString.Compare(current.StopWordsFile, stopCustomControl.StopWordsFile, true) != 0) ||
                     (CMSString.Compare(current.CustomAnalyzerAssemblyName, stopCustomControl.CustomAnalyzerAssemblyName, true) != 0) ||
                     (CMSString.Compare(current.CustomAnalyzerClassName, stopCustomControl.CustomAnalyzerClassName, true) != 0)))
                {
                    analyzerTypeChanged = true;
                }
                current.IndexAnalyzerType = SearchIndexInfoProvider.AnalyzerCodenameToEnum(drpAnalyzer.SelectedValue);

                // Community indexing is not yet supported
                //current.IndexIsCommunityGroup = chkCommunity.Checked;
                current.IndexIsCommunityGroup = false;
                current.CustomAnalyzerAssemblyName = stopCustomControl.CustomAnalyzerAssemblyName;
                current.CustomAnalyzerClassName = stopCustomControl.CustomAnalyzerClassName;
                current.StopWordsFile = stopCustomControl.StopWordsFile;
                current.IndexBatchSize = ValidationHelper.GetInteger(txtBatchSize.Text, 10);

                // Save the object
                SearchIndexInfoProvider.SetSearchIndexInfo(current);

                // Codename changed
                bool codenameChanged = false;
                if (sii == null)
                {
                    try
                    {
                        DirectoryHelper.MoveDirectory(originalPath, current.CurrentIndexPath);
                    }
                    catch (Exception ex)
                    {
                        EventLogProvider ep = new EventLogProvider();
                        ep.LogEvent("SmartSearch", "Rename", ex);
                    }

                    codenameChanged = true;
                }

                ShowChangesSaved();

                if (codenameChanged || analyzerTypeChanged)
                {
                    ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
                }

                // Redirect to edit mode
                RaiseOnSaved();
            }
            else
            {
                // Error message - code name already exists
                ShowError(GetString("srch.index.codenameexists"));
            }
        }
        else
        {
            // Error message - validation
            ShowError(errorMessage);
        }
    }


    /// <summary>
    ///  Rebuild click.
    /// </summary>    
    protected void Rebuild()
    {
        if (sii != null)
        {
            if (DataHelper.DataSourceIsEmpty(sii.IndexSettings.GetAll()) && (sii.IndexType.ToLowerCSafe() != PredefinedObjectType.USER))
            {
                ShowError(GetString("index.nocontent"));
                return;
            }

            if ((sii.IndexType.ToLowerCSafe() == PredefinedObjectType.DOCUMENT) || (sii.IndexType == SearchHelper.DOCUMENTS_CRAWLER_INDEX))
            {
                DataSet ds = SearchIndexCultureInfoProvider.GetSearchIndexCultures("IndexID = " + sii.IndexID, null, 0, "IndexID, IndexCultureID");
                if (DataHelper.DataSourceIsEmpty(ds))
                {
                    ShowError(GetString("index.noculture"));
                    return;
                }
            }

            SearchTaskInfoProvider.CreateTask(SearchTaskTypeEnum.Rebuild, sii.IndexType, null, sii.IndexName);
        }

        ShowInformation(GetString("srch.index.rebuildstarted"));

        // Reload info panel
        Thread.Sleep(100);
        ReloadInfoPanel();
    }


    /// <summary>
    ///  Optimize click.
    /// </summary>    
    protected void Optimize()
    {
        // Rebuild search index info
        if (sii != null)
        {
            SearchTaskInfoProvider.CreateTask(SearchTaskTypeEnum.Optimize, sii.IndexType, null, sii.IndexName);
        }

        ShowConfirmation(GetString("srch.index.optimizestarted"));

        // Reload info panel
        Thread.Sleep(100);
        ReloadInfoPanel();
    }


    /// <summary>
    /// Reloads info panel.
    /// </summary>
    protected void ReloadInfoPanel()
    {
        if (sii != null)
        {
            // Keep flag if is in action status
            bool isInAction = (sii.IndexStatus == IndexStatusEnum.REBUILDING || sii.IndexStatus == IndexStatusEnum.OPTIMIZING);

            // Keep flag if index is not usable
            bool isNotReady = (!isInAction && sii.IndexStatus != IndexStatusEnum.READY);

            // get status name
            string statusName = GetString("srch.status." + sii.IndexStatus.ToString());

            // Set progress if is action status
            ltrProgress.Text = String.Empty;
            if (isInAction)
            {
                ltrProgress.Text = "<img style=\"width:12px;height:12px;\" src=\"" + UIHelper.GetImageUrl(Page, "Design/Preloaders/preload16.gif") + "\" alt=\"" + statusName + "\" tooltip=\"" + statusName + "\"  />";
            }

            // Fill panel info with information about index
            lblNumberOfItemsValue.Text = ValidationHelper.GetString(sii.NumberOfIndexedItems, "0");
            lblIndexFileSizeValue.Text = DataHelper.GetSizeString(sii.IndexFileSize);
            lblIndexStatusValue.Text = statusName;

            // use coloring for status name
            if (isNotReady)
            {
                lblIndexStatusValue.Text = "<span class=\"StatusDisabled\">" + statusName + "</span>";
            }
            else if (sii.IndexStatus == IndexStatusEnum.READY)
            {
                lblIndexStatusValue.Text = "<span class=\"StatusEnabled\">" + statusName + "</span>";
            }

            lblLastRebuildTimeValue.Text = GetString("general.notavailable");
            lblLastUpdateValue.Text = sii.IndexLastUpdate.ToString();

            if (sii.IndexLastRebuildTime != DateTimeHelper.ZERO_TIME)
            {
                lblLastRebuildTimeValue.Text = ValidationHelper.GetString(sii.IndexLastRebuildTime, "");
            }
            lblIndexIsOptimizedValue.Text = UniGridFunctions.ColoredSpanYesNo(false);

            if (sii.IndexStatus == IndexStatusEnum.READY)
            {
                IndexSearcher searcher = sii.GetSearcher();
                if (searcher != null)
                {
                    IndexReader reader = searcher.GetIndexReader();
                    if (reader != null)
                    {
                        if (reader.IsOptimized())
                        {
                            lblIndexIsOptimizedValue.Text = UniGridFunctions.ColoredSpanYesNo(true);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Initializes header actions for rebuild and optimize.
    /// </summary>
    private void InitializeHeaderActions()
    {
        // Rebuild action
        AddHeaderAction(new HeaderAction()
        {
            ImageUrl = GetImageUrl("CMSModules/CMS_SMartSearch/rebuild.png"),
            Text = GetString("srch.index.rebuild"),
            OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("srch.index.confirmrebuild")) + ");",
            CommandName = REBUILD
        });
        ComponentEvents.RequestEvents.RegisterForEvent(REBUILD, (s, args) => { Rebuild(); });

        // Optimize action
        AddHeaderAction(new HeaderAction()
        {
            ImageUrl = GetImageUrl("CMSModules/CMS_SMartSearch/optimize.png"),
            Text = GetString("srch.index.optimize"),
            OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("srch.index.confirmoptimize")) + ");",
            CommandName = OPTIMIZE
        });
        ComponentEvents.RequestEvents.RegisterForEvent(OPTIMIZE, (s, args) => { Optimize(); });        
    }

    #endregion


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "saved")
        {
            if (SearchHelper.CreateRebuildTask(ItemID))
            {
                ShowConfirmation(GetString("srch.index.rebuildstarted"));
            }
            else
            {
                ShowError(GetString("index.nocontent"));
            }

            // Reload info panel
            Thread.Sleep(100);
            ReloadInfoPanel();
        }
    }

    #endregion
}