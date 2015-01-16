using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.ExtendedControls;

[RegisterTitle("content.ui.propertieslanguages")]
public partial class CMSModules_Content_CMSDesk_Properties_Languages : CMSPropertiesPage
{
    #region "Variables"

    private UserInfo currentUserInfo = null;
    private SiteInfo currentSiteInfo = null;
    private DateTime defaultLastModification = DateTimeHelper.ZERO_TIME;
    private DateTime defaultLastPublished = DateTimeHelper.ZERO_TIME;
    private string mDefaultSiteCulture = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Default culture of the site.
    /// </summary>
    protected string DefaultSiteCulture
    {
        get
        {
            return mDefaultSiteCulture ?? (mDefaultSiteCulture = CultureHelper.GetDefaultCulture(CMSContext.CurrentSiteName));
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.Languages"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties.Languages");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        gridLanguages.FilteredZeroRowsText = GetString("transman.nodocumentculture");
        gridLanguages.OnDataReload += gridDocuments_OnDataReload;
        gridLanguages.OnExternalDataBound += gridLanguages_OnExternalDataBound;
        gridLanguages.ShowActionsMenu = true;
        gridLanguages.Columns = "DocumentName,  Published";

        IDataClass nodeClass = DataClassFactory.NewDataClass("CMS.Tree");
        DocumentInfo di = new DocumentInfo();
        gridLanguages.AllColumns = SqlHelperClass.MergeColumns(SqlHelperClass.MergeColumns(di.ColumnNames), SqlHelperClass.MergeColumns(nodeClass.ColumnNames));

        pnlContainer.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterDialogScript(Page);

        // Check license limitations
        if (!CultureInfoProvider.LicenseVersionCheck())
        {
            LicenseHelper.GetAllAvailableKeys(FeatureEnum.Multilingual);
        }

        // Set selected tab
        UIContext.PropertyTab = PropertyTabEnum.Languages;
    }

    #endregion


    #region "Grid events"

    protected object gridLanguages_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        TranslationStatusEnum status = TranslationStatusEnum.NotAvailable;
        DataRowView drv = null;
        sourceName = sourceName.ToLowerCSafe();

        if (currentUserInfo == null)
        {
            currentUserInfo = CMSContext.CurrentUser;
        }
        if (currentSiteInfo == null)
        {
            currentSiteInfo = CMSContext.CurrentSite;
        }

        switch (sourceName)
        {
            case "translate":
            case "action":
                ImageButton img = sender as CMSImageButton;
                if (img != null)
                {
                    if ((sourceName == "translate") &&
                        (!CMS.TranslationServices.TranslationServiceHelper.AnyServiceAvailable(CurrentSiteName) ||
                         !CMS.TranslationServices.TranslationServiceHelper.IsTranslationAllowed(CurrentSiteName)))
                    {
                        img.Visible = false;
                        return img;
                    }

                    GridViewRow gvr = parameter as GridViewRow;
                    if (gvr != null)
                    {
                        // Get datarowview
                        drv = gvr.DataItem as DataRowView;

                        if ((drv != null) && (drv.Row["TranslationStatus"] != DBNull.Value))
                        {
                            // Get translation status
                            status = (TranslationStatusEnum)drv.Row["TranslationStatus"];
                        }
                        else
                        {
                            status = TranslationStatusEnum.NotAvailable;
                        }

                        string culture = (drv != null) ? ValidationHelper.GetString(drv.Row["DocumentCulture"], string.Empty) : string.Empty;

                        // Set appropriate icon
                        if (sourceName == "action")
                        {
                            switch (status)
                            {
                                case TranslationStatusEnum.NotAvailable:
                                    img.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Properties/addculture.png");
                                    img.ToolTip = GetString("transman.createnewculture");
                                    break;

                                default:
                                    img.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Properties/editculture.png");
                                    img.ToolTip = GetString("transman.editculture");
                                    break;
                            }

                            // Register redirect script
                            if (RequiresDialog)
                            {
                                if ((sourceName == "action") && (status == TranslationStatusEnum.NotAvailable))
                                {
                                    // New culture version
                                    img.OnClientClick = "parent.parent.parent.NewDocumentCulture(" + NodeID + ",'" + culture + "');";
                                }
                                else
                                {
                                    // Existing culture version
                                    ScriptHelper.RegisterWOpenerScript(Page);
                                    string url = ResolveUrl(CMSContext.GetUrl(Node.NodeAliasPath, Node.DocumentUrlPath, currentSiteInfo.SiteName));
                                    url = URLHelper.AppendQuery(url, "lang=" + culture);
                                    img.OnClientClick = "window.refreshPageOnClose = true; window.reloadPageUrl = " + ScriptHelper.GetString(url) + "; if (wopener.RefreshWOpener) { wopener.RefreshWOpener(window); } CloseDialog();";
                                }
                            }
                            else
                            {
                                img.OnClientClick = "RedirectItem(" + NodeID + ", '" + culture + "');";
                            }

                            img.ID = "imgAction";
                        }
                        else
                        {
                            // Add parameters identifier and hash, encode query string
                            if (LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.TranslationServices))
                            {
                                string returnUrl = CMSContext.ResolveDialogUrl("~/CMSModules/Translations/Pages/TranslateDocuments.aspx") + "?targetculture=" + culture + "&modal=1&nodeid=" + NodeID;
                                returnUrl = URLHelper.AddParameterToUrl(returnUrl, "hash", QueryHelper.GetHash(URLHelper.GetQuery(returnUrl)));

                                img.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Properties/translate.png");
                                img.ToolTip = GetString("transman.translate");
                                img.OnClientClick = "modalDialog('" + returnUrl + "', 'TranslateDocument', 550, 440); ";
                            }
                            else
                            {
                                img.Visible = false;
                            }
                            break;
                        }
                    }
                }
                return img;

            case "translationstatus":
                if (parameter == DBNull.Value)
                {
                    status = TranslationStatusEnum.NotAvailable;
                }
                else
                {
                    status = (TranslationStatusEnum)parameter;
                }
                string statusName = GetString("transman." + status);
                string statusHtml = "<span class=\"" + status + "\">" + statusName + "</span>";
                // .Outdated
                return statusHtml;

            case "documentculturedisplayname":
                drv = (DataRowView)parameter;
                // Add icon
                return UniGridFunctions.DocumentCultureFlag(drv, Page);

            case "documentmodifiedwhen":
            case "documentmodifiedwhentooltip":
                if (string.IsNullOrEmpty(parameter.ToString()))
                {
                    return "-";
                }
                else
                {
                    DateTime modifiedWhen = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                    bool displayGMT = (sourceName == "documentmodifiedwhentooltip");
                    return TimeZoneHelper.ConvertToUserTimeZone(modifiedWhen, displayGMT, currentUserInfo, currentSiteInfo);
                }

            case "versionnumber":
                if (string.IsNullOrEmpty(parameter.ToString()))
                {
                    return "-";
                }
                break;

            case "documentname":
                if (string.IsNullOrEmpty(parameter.ToString()))
                {
                    parameter = "-";
                }
                return HTMLHelper.HTMLEncode(parameter.ToString());

            case "published":
                bool published = ValidationHelper.GetBoolean(parameter, false);
                if (published)
                {
                    return "<span class=\"DocumentPublishedYes\">" + GetString("General.Yes") + "</span>";
                }
                else
                {
                    return "<span class=\"DocumentPublishedNo\">" + GetString("General.No") + "</span>";
                }
        }
        return parameter;
    }


    protected DataSet gridDocuments_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        string currentSiteName = CMSContext.CurrentSiteName;

        // Check if node is not null
        if (Node != null)
        {
            // Get documents
            int topN = gridLanguages.GridView.PageSize * (gridLanguages.GridView.PageIndex + 1 + gridLanguages.GridView.PagerSettings.PageButtonCount);
            columns = SqlHelperClass.MergeColumns(SqlHelperClass.MergeColumns(TreeProvider.SELECTNODES_REQUIRED_COLUMNS, columns), "DocumentModifiedWhen, VersionNumber, DocumentLastPublished, DocumentIsWaitingForTranslation");
            DataSet documentsDS = DocumentHelper.GetDocuments(currentSiteName, Node.NodeAliasPath, TreeProvider.ALL_CULTURES, false, null, null, null, -1, false, topN, columns, Tree);
            DataTable documents = documentsDS.Tables[0];

            if (!DataHelper.DataSourceIsEmpty(documents))
            {
                // Get site cultures
                DataSet allSiteCultures = CultureInfoProvider.GetSiteCultures(currentSiteName).Copy();

                // Rename culture column to enable row transfer
                allSiteCultures.Tables[0].Columns[2].ColumnName = "DocumentCulture";

                // Create where condition for row transfer
                string where = documents.Rows.Cast<DataRow>().Aggregate("DocumentCulture NOT IN (", (current, row) => current + ("'" + SqlHelperClass.GetSafeQueryString(ValidationHelper.GetString(row["DocumentCulture"], string.Empty)) + "',"));
                where = where.TrimEnd(',') + ")";

                // Transfer missing cultures, keep original list of site cultures
                DataHelper.TransferTableRows(documents, allSiteCultures.Copy().Tables[0], where, null);
                DataHelper.EnsureColumn(documents, "DocumentCultureDisplayName", typeof(string));

                // Ensure culture names
                foreach (DataRow cultDR in documents.Rows)
                {
                    string cultureCode = cultDR["DocumentCulture"].ToString();
                    DataRow[] cultureRow = allSiteCultures.Tables[0].Select("DocumentCulture='" + cultureCode + "'");
                    if (cultureRow.Length > 0)
                    {
                        cultDR["DocumentCultureDisplayName"] = cultureRow[0]["CultureName"].ToString();
                    }
                }

                // Ensure default culture to be first
                DataRow[] culturreDRs = documents.Select("DocumentCulture='" + DefaultSiteCulture + "'");
                if (culturreDRs.Length <= 0)
                {
                    throw new Exception("[ReloadData]: Default site culture '" + DefaultSiteCulture + "' is not assigned to the current site.");
                }

                DataRow defaultCultureRow = culturreDRs[0];

                DataRow dr = documents.NewRow();
                dr.ItemArray = defaultCultureRow.ItemArray;
                documents.Rows.InsertAt(dr, 0);
                documents.Rows.Remove(defaultCultureRow);

                // Get last modification date of default culture
                defaultCultureRow = documents.Select("DocumentCulture='" + DefaultSiteCulture + "'")[0];
                defaultLastModification = ValidationHelper.GetDateTime(defaultCultureRow["DocumentModifiedWhen"], DateTimeHelper.ZERO_TIME);
                defaultLastPublished = ValidationHelper.GetDateTime(defaultCultureRow["DocumentLastPublished"], DateTimeHelper.ZERO_TIME);

                // Add column containing translation status
                documents.Columns.Add("TranslationStatus", typeof(TranslationStatusEnum));

                // Get proper translation status and store it to datatable
                foreach (DataRow document in documents.Rows)
                {
                    TranslationStatusEnum status = TranslationStatusEnum.NotAvailable;
                    int documentId = ValidationHelper.GetInteger(document["DocumentID"], 0);
                    if (documentId == 0)
                    {
                        status = TranslationStatusEnum.NotAvailable;
                    }
                    else
                    {
                        string versionNumber = ValidationHelper.GetString(DataHelper.GetDataRowValue(document, "VersionNumber"), null);
                        DateTime lastModification = DateTimeHelper.ZERO_TIME;

                        if (ValidationHelper.GetBoolean(document["DocumentIsWaitingForTranslation"], false))
                        {
                            status = TranslationStatusEnum.WaitingForTranslation;
                        }
                        else
                        {
                            // Check if document is outdated
                            if (versionNumber != null)
                            {
                                lastModification = ValidationHelper.GetDateTime(document["DocumentLastPublished"], DateTimeHelper.ZERO_TIME);
                                status = (lastModification < defaultLastPublished) ? TranslationStatusEnum.Outdated : TranslationStatusEnum.Translated;
                            }
                            else
                            {
                                lastModification = ValidationHelper.GetDateTime(document["DocumentModifiedWhen"], DateTimeHelper.ZERO_TIME);
                                status = (lastModification < defaultLastModification) ? TranslationStatusEnum.Outdated : TranslationStatusEnum.Translated;
                            }
                        }
                    }
                    document["TranslationStatus"] = status;
                }

                // Bind datasource
                DataSet filteredDocuments = documentsDS.Clone();
                DataRow[] filteredDocs = documents.Select(gridLanguages.GetDataTableFilter());
                foreach (DataRow row in filteredDocs)
                {
                    filteredDocuments.Tables[0].ImportRow(row);
                }

                return filteredDocuments;
            }
        }

        return null;
    }

    #endregion
}