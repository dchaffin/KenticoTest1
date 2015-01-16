using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.DataEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UniGrid_UniMatrix : UniMatrix, ICallbackEventHandler, IUniPageable
{
    #region "Variables"

    private int mDefaultPageSize = 20;
    private string mPageSizeOptions = "10,20,50,100,##ALL##";
    private string mCallbackResult = null;
    private bool mLoaded = false;
    private DataSet ds = null;
    private int mTotalRows = 0;
    private string mNoRecordsMessage = null;
    private bool mUsePercentage = false;
    private string mCornerText = string.Empty;

    private Hashtable mColumnPermissions = new Hashtable();
    private Hashtable mRowPermissions = new Hashtable();

    #endregion


    #region "Properties"

    /// <summary>
    /// Page size options for pager.
    /// Numeric values or macro ##ALL## separated with comma.
    /// </summary>
    public string PageSizeOptions
    {
        get
        {
            return mPageSizeOptions;
        }
        set
        {
            mPageSizeOptions = value;
            pagerElem.PageSizeOptions = value;
        }
    }

    /// <summary>
    /// Default page size at first load.
    /// </summary>
    public virtual int DefaultPageSize
    {
        get
        {
            if ((mDefaultPageSize <= 0) && (mDefaultPageSize != -1))
            {
                mDefaultPageSize = ValidationHelper.GetInteger(SettingsHelper.AppSettings["CMSDefaultListingPageSize"], 20);
            }
            return mDefaultPageSize;
        }
        set
        {
            mDefaultPageSize = value;
        }
    }


    /// <summary>
    /// Items per page.
    /// </summary>
    public override int ItemsPerPage
    {
        get
        {
            return base.ItemsPerPage;
        }
        set
        {
            base.ItemsPerPage = value;
            pagerElem.UniPager.PageSize = value;
        }
    }


    /// <summary>
    /// Gets the value that indicates whether current selector in multiple mode displays some data or whether the dropdown contains some data.
    /// </summary>
    public override bool HasData
    {
        get
        {
            // Ensure the data
            if (!StopProcessing)
            {
                ReloadData(false);
            }
            else
            {
                return false;
            }

            return ValidationHelper.GetBoolean(ViewState["HasData"], false);
        }
        protected set
        {
            ViewState["HasData"] = value;
        }
    }


    /// <summary>
    /// Filter where condition.
    /// </summary>
    private string FilterWhere
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FilterWhere"], "");
        }
        set
        {
            ViewState["FilterWhere"] = value;
        }
    }


    /// <summary>
    /// Number of expected matrix columns.
    /// </summary>
    public int ColumnsCount
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ColumnsCount"], 10);
        }
        set
        {
            ViewState["ColumnsCount"] = value;
        }
    }


    /// <summary>
    /// Number of expected matrix columns.
    /// </summary>
    public string ColumnsPreferedOrder
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ColumnsPreferedOrder"], "");
        }
        set
        {
            ViewState["ColumnsPreferedOrder"] = value;
        }
    }


    /// <summary>
    /// Sets or gets fixed width of cell. If it's 0 no fixed width is inserted in style.
    /// </summary>
    public int FixedWidth
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["FixedWidth"], 75);
        }
        set
        {
            ViewState["FixedWidth"] = value;
        }
    }


    /// <summary>
    /// Sets or gets fixed width of first column.
    /// </summary>
    public int FirstColumnsWidth
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["FirstColumnsWidth"], 300);
        }
        set
        {
            ViewState["FirstColumnsWidth"] = value;
        }
    }


    /// <summary>
    /// Indicates if last column will have 100% width.
    /// </summary>
    public bool LastColumnFullWidth
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["LastColumnFullWidth"], false);
        }
        set
        {
            ViewState["LastColumnFullWidth"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed if there are no records.
    /// </summary>
    public string NoRecordsMessage
    {
        get
        {
            return mNoRecordsMessage;
        }
        set
        {
            mNoRecordsMessage = value;
        }
    }


    /// <summary>
    /// Indicates if percentage should be used in column width definition.
    /// </summary>
    public bool UsePercentage
    {
        get
        {
            return mUsePercentage;
        }
        set
        {
            mUsePercentage = value;
        }
    }


    /// <summary>
    /// UniPager control of UniMatrix.
    /// </summary>
    public UniPager Pager
    {
        get
        {
            return pagerElem.UniPager;
        }
    }


    /// <summary>
    /// Text displayed in the upper left corner of UniMatrix, if filter is not shown.
    /// </summary>
    public string CornerText
    {
        get
        {
            return mCornerText;
        }
        set
        {
            mCornerText = value;
        }
    }


    /// <summary>
    /// Gets or sets HTML content to be rendered as additional content on the top of the matrix.
    /// </summary>
    public string ContentBeforeRows
    {
        get
        {
            return ltlBeforeRows.Text;
        }
        set
        {
            ltlBeforeRows.Text = value;
        }
    }


    /// <summary>
    /// Indicates if content before rows should be displayed.
    /// </summary>
    public bool ShowContentBeforeRows
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets CSS class for content before rows.
    /// </summary>
    public string ContentBeforeRowsCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Gets order in which data will be rendered.
    /// </summary>
    public int[] ColumnOrderIndex
    {
        get
        {
            return ViewState["ColumnOrderIndex"] as int[];
        }
        set
        {
            ViewState["ColumnOrderIndex"] = value;
        }
    }


    /// <summary>
    /// Mark HTML code for the disabled column in header.
    /// </summary>
    public string DisabledColumnMark
    {
        get;
        set;
    }


    /// <summary>
    /// Mark HTML code for the disabled row in header.
    /// </summary>
    public string DisabledRowMark
    {
        get;
        set;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when data has been loaded. Allows manipulation with data.
    /// </summary>
    /// <param name="ds">Loaded dataset</param>
    public delegate void OnMatrixDataLoaded(DataSet ds);


    public event OnMatrixDataLoaded DataLoaded;


    /// <summary>
    /// Occurs if the matrix wants to check the permission to edit particular item.
    /// </summary>
    /// <param name="value">Column value</param>
    public delegate bool OnCheckPermissions(object value);


    public event OnCheckPermissions CheckColumnPermissions;

    public event OnCheckPermissions CheckRowPermissions;

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        mContentBefore = "<table class=\"UniGridGrid\" cellspacing=\"0\" cellpadding=\"0\" rules=\"rows\" border=\"1\" style=\"border-collapse:collapse;\">";

        pagerElem.PagedControl = this;
        pagerElem.UniPager.PageControl = "uniMatrix";
        pagerElem.PageSizeOptions = PageSizeOptions;
        pagerElem.UniPager.PageSize = ItemsPerPage;
        pagerElem.PageSizeDropdown.SelectedIndexChanged += drpPageSize_SelectedIndexChanged;

        ScriptHelper.RegisterJQuery(Page);
    }


    /// <summary>
    /// Returns true if the given row is editable.
    /// </summary>
    /// <param name="rowValue">Row value</param>
    protected bool IsRowEditable(object rowValue)
    {
        if (CheckRowPermissions == null)
        {
            return true;
        }

        // Try to get cached value
        object editableObj = mRowPermissions[rowValue];
        if (editableObj == null)
        {
            // Get by external function
            editableObj = CheckRowPermissions(rowValue);
            mRowPermissions[rowValue] = editableObj;
        }

        return ValidationHelper.GetBoolean(editableObj, true);
    }


    /// <summary>
    /// Returns true if the given column is editable.
    /// </summary>
    /// <param name="columnValue">Column value</param>
    protected bool IsColumnEditable(object columnValue)
    {
        if (CheckColumnPermissions == null)
        {
            return true;
        }

        // Try to get cached value
        object editableObj = mColumnPermissions[columnValue];
        if (editableObj == null)
        {
            // Get by external function
            editableObj = CheckColumnPermissions(columnValue);
            mColumnPermissions[columnValue] = editableObj;
        }

        return ValidationHelper.GetBoolean(editableObj, true);
    }


    protected void drpPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        pagerElem.UniPager.CurrentPage = 1;
        ItemsPerPage = ValidationHelper.GetInteger(pagerElem.PageSizeDropdown.SelectedValue, -1);

        if (pagerElem.UniPager.PagedControl != null)
        {
            pagerElem.UniPager.PagedControl.ReBind();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsCallback())
        {
            ReloadData(false);
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    /// <param name="forceReload">Force the reload of the control</param>
    public override void ReloadData(bool forceReload)
    {
        if (StopProcessing)
        {
            plcPager.Visible = false;
            ltlBeforeRows.Visible = false;
        }
        else
        {
            base.ReloadData(forceReload);
            SetPageSize(forceReload);

            // Clear filter if forced reload
            if (forceReload)
            {
                txtFilter.Text = "";
                FilterWhere = null;
            }

            if (forceReload || (!mLoaded && !StopProcessing))
            {
                StringBuilder sb = new StringBuilder();

                // Prepare the order by
                string orderBy = OrderBy;
                if (orderBy == null)
                {
                    orderBy = RowItemDisplayNameColumn + " ASC";
                    // Add additional sorting by codename for equal displaynames
                    if (!String.IsNullOrEmpty(RowItemCodeNameColumn))
                    {
                        orderBy += ", " + RowItemCodeNameColumn;
                    }

                    if (ColumnsCount > 1)
                    {
                        orderBy += ", " + ColumnItemDisplayNameColumn + " ASC";
                    }
                }

                int currentPage = pagerElem.UniPager.CurrentPage;
                string where = SqlHelperClass.AddWhereCondition(WhereCondition, FilterWhere);

                bool headersOnly = false;
                bool hasData = false;
                mTotalRows = 0;

                List<DataRow> columns = null;

                // Load the data
                while (true)
                {
                    // Get specific page
                    int pageItems = ColumnsCount * pagerElem.UniPager.PageSize;
                    ds = ConnectionHelper.ExecuteQuery(QueryName, QueryParameters, where, orderBy, 0, null, (currentPage - 1) * pageItems, pageItems, ref mTotalRows);

                    hasData = !DataHelper.DataSourceIsEmpty(ds);

                    // If no records found, get the records for the original dataset
                    if (!hasData && !String.IsNullOrEmpty(FilterWhere))
                    {
                        // Get only first line
                        ds = ConnectionHelper.ExecuteQuery(QueryName, QueryParameters, WhereCondition, orderBy, ColumnsCount);
                        hasData = !DataHelper.DataSourceIsEmpty(ds);
                        headersOnly = true;
                    }

                    // Load the list of columns
                    if (hasData)
                    {
                        if (DataLoaded != null)
                        {
                            DataLoaded(ds);
                        }

                        columns = DataHelper.GetUniqueRows(ds.Tables[0], ColumnItemIDColumn);
                        ColumnOrderIndex = GetColumnIndexes(columns, ColumnsPreferedOrder);

                        // If more than current columns count found, and there is more data, get the correct data again
                        if ((columns.Count <= ColumnsCount) || (mTotalRows < pageItems))
                        {
                            break;
                        }
                        else
                        {
                            ColumnsCount = columns.Count;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (hasData)
                {
                    bool manyColumns = columns.Count >= 10;

                    string imagesUrl = GetImageUrl("Design/Controls/UniMatrix/", IsLiveSite, true);

                    string firstColumnsWidth = (FirstColumnsWidth > 0) ? "width:" + FirstColumnsWidth + (UsePercentage ? "%;" : "px;") : "";

                    if (!headersOnly)
                    {
                        // Register the scripts
                        string script =
                            "var umImagesUrl = '" + GetImageUrl("Design/Controls/UniMatrix/", IsLiveSite, true) + "';" +
                            "function UM_ItemChanged_" + ClientID + "(item) {" + Page.ClientScript.GetCallbackEventReference(this, "item.id + ':' + (item.src.indexOf('denied.png') >= 0)", "UM_ItemSaved_" + ClientID, "item.id") + "; } \n" +
                            "function UM_ItemSaved_" + ClientID + "(rvalue, context) { var elem = document.getElementById(context); var values=rvalue.split('|'); \nif (values[0] == 'true') { elem.src = umImagesUrl + 'allowed.png'; } else { elem.src = umImagesUrl + 'denied.png'; } var contentBefore = $j(\"#contentbeforerows_" + ClientID + "\"); if(contentBefore){ contentBefore.empty().append(values[1]);}}";

                        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UniMatrix_" + ClientID, ScriptHelper.GetScript(script));
                    }

                    // Render header
                    ltlBeforeFilter.Text = "<tr class=\"UniGridHead\" align=\"left\"><th style=\"" + firstColumnsWidth + "white-space:nowrap;\" scope=\"col\"><div class=\"UniMatrixFilter\">";

                    StringBuilder headersb = new StringBuilder();

                    headersb.Append("</div></th>");

                    string width = (FixedWidth > 0) ? "width:" + FixedWidth.ToString() + (UsePercentage ? "%;" : "px;") : "";

                    // Render matrix header
                    foreach (int index in ColumnOrderIndex)
                    {
                        DataRow dr = (DataRow)columns[index];

                        if (ShowHeaderRow)
                        {
                            string header = HTMLHelper.HTMLEncode(CMSContext.ResolveMacros(Convert.ToString(dr[ColumnItemDisplayNameColumn])));

                            headersb.Append("<th scope=\"col\" style=\"text-align: center;");
                            if (!manyColumns)
                            {
                                headersb.Append(" white-space: nowrap;");
                                header = header.Replace(" ", "&nbsp;").Replace("-", "&minus;");
                            }
                            else
                            {
                                headersb.Append(" padding: 2px 7px 2px 5px;");
                            }

                            headersb.Append(" " + width + "\"");
                            if (ColumnItemTooltipColumn != null)
                            {
                                headersb.Append(" title=\"", GetTooltip(dr, ItemTooltipColumn), "\"");
                            }

                            // Disabled mark
                            object columnValue = dr[ColumnItemIDColumn];
                            if (!IsColumnEditable(columnValue))
                            {
                                header += DisabledColumnMark;
                            }

                            headersb.Append(">", header, "</th>\n");
                        }
                        else
                        {
                            headersb.Append("<th scope=\"col\" style=\"text-align: center; ", width, "\">&nbsp;</td>");
                        }
                    }

                    // Set the correct number of columns
                    ColumnsCount = columns.Count;
                    mTotalRows = mTotalRows / ColumnsCount;

                    if (!manyColumns)
                    {
                        headersb.Append("<th ", (LastColumnFullWidth ? "style=\"width:100%;\" " : ""), ">&nbsp;</th>");
                    }
                    headersb.Append("</tr>");

                    ltlAfterFilter.Text = headersb.ToString();

                    if (!headersOnly)
                    {
                        string lastId = "";
                        int colIndex = 0;
                        int rowIndex = 0;

                        bool evenRow = true;

                        // Render matrix rows
                        int step = columns.Count;
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i = i + step)
                        {
                            foreach (int index in ColumnOrderIndex)
                            {
                                DataRow dr = (DataRow)ds.Tables[0].Rows[i + index];

                                string id = ValidationHelper.GetString(dr[RowItemIDColumn], "");
                                if (id != lastId)
                                {
                                    if ((ItemsPerPage > 0) && (rowIndex++ >= ItemsPerPage))
                                    {
                                        break;
                                    }

                                    // New row
                                    if (lastId != "")
                                    {
                                        // Close the previous row
                                        if (!manyColumns)
                                        {
                                            sb.Append("<td style=\"white-space:nowrap;\">&nbsp;</td>");
                                        }
                                        sb.Append("</tr>");
                                    }
                                    sb.Append("<tr class=\"", (evenRow ? "EvenRow" : "OddRow"), GetAdditionalCssClass(dr), "\"><td class=\"MatrixHeader\" style=\"", firstColumnsWidth, "white-space:nowrap;\"");
                                    if (RowItemTooltipColumn != null)
                                    {
                                        sb.Append(" title=\"", GetTooltip(dr, RowItemTooltipColumn), "\"");
                                    }
                                    sb.Append(">");
                                    sb.Append(HTMLHelper.HTMLEncode(CMSContext.ResolveMacros(Convert.ToString(dr[RowItemDisplayNameColumn]))));

                                    // Disabled mark
                                    if (!IsRowEditable(id))
                                    {
                                        sb.Append(DisabledRowMark);
                                    }

                                    // Add global suffix if is required
                                    if ((index == 0) && (AddGlobalObjectSuffix) && (ValidationHelper.GetInteger(dr[SiteIDColumnName], 0) == 0))
                                    {
                                        sb.Append(" " + GetString("general.global"));
                                    }

                                    sb.Append("</td>\n");

                                    lastId = id;
                                    colIndex = 0;
                                    evenRow = !evenRow;
                                }

                                object columnValue = dr[ColumnItemIDColumn];

                                // Render cell
                                sb.Append("<td style=\"white-space:nowrap; text-align: center;\"><img src=\"");
                                sb.Append(imagesUrl);
                                if (!Enabled ||
                                    disabledColumns.Contains(colIndex) ||
                                    !IsColumnEditable(columnValue) ||
                                    !IsRowEditable(id)
                                    )
                                {
                                    // Disabled
                                    if (Convert.ToInt32(dr["Allowed"]) == 1)
                                    {
                                        sb.Append("alloweddisabled.png");
                                    }
                                    else
                                    {
                                        sb.Append("denieddisabled.png");
                                    }
                                }
                                else
                                {
                                    // Enabled
                                    if (Convert.ToInt32(dr["Allowed"]) == 1)
                                    {
                                        sb.Append("allowed.png");
                                    }
                                    else
                                    {
                                        sb.Append("denied.png");
                                    }

                                    sb.Append("\" id=\"chk:", id, ":", columnValue, "\" onclick=\"UM_ItemChanged_", ClientID, "(this);");
                                }

                                sb.Append("\" style=\"cursor:pointer;\" title=\"");

                                string tooltip = GetTooltip(dr, ItemTooltipColumn);

                                sb.Append(tooltip, "\" alt=\"", tooltip, "\" /></td>\n");

                                colIndex++;
                            }
                        }

                        // Close the latest row if present
                        if (!manyColumns)
                        {
                            sb.Append("<td>&nbsp;</td>");
                        }

                        sb.Append("</tr>");


                        int totalCols = (columns.Count + 2);
                        if (manyColumns)
                        {
                            totalCols--;
                        }

                        ltlPagerBefore.Text = "<tr style=\"border:0px none;\"><td style=\"border:0px none;\" colspan=\"" + totalCols + "\">";
                        ltlPagerAfter.Text = "</td></tr>";
                    }
                    else
                    {
                        lblInfoAfter.Text = NoRecordsMessage;
                        lblInfoAfter.Visible = true;
                    }

                    // Show filter / header
                    bool hideFilter = ((FilterLimit > 0) && String.IsNullOrEmpty(FilterWhere) && (mTotalRows < FilterLimit));
                    pnlFilter.Visible = !hideFilter;

                    // Show label in corner if text given and filter is hidden
                    if (hideFilter && !string.IsNullOrEmpty(CornerText))
                    {
                        ltlBeforeFilter.Text += HTMLHelper.HTMLEncode(CornerText);
                    }

                    if (ShowFilterRow && !hideFilter)
                    {
                        //this.lblFilter.ResourceString = this.ResourcePrefix + ".entersearch";
                        btnFilter.ResourceString = "general.search";
                    }
                    else if (!ShowHeaderRow)
                    {
                        plcFilter.Visible = false;
                    }
                }
                else
                {
                    pnlFilter.Visible = false;

                    // If norecords message set, hide everything and show message
                    if (!String.IsNullOrEmpty(NoRecordsMessage))
                    {
                        lblInfo.Text = NoRecordsMessage;
                        lblInfo.Visible = true;
                        ltlMatrix.Visible = false;
                        ltlContentAfter.Visible = false;
                        ltlContentBefore.Visible = false;
                    }
                }

                HasData = hasData;

                ltlContentBefore.Text = ContentBefore;
                ltlMatrix.Text = sb.ToString();
                ltlContentAfter.Text = ContentAfter;

                // Show content before rows and pager
                plcBeforeRows.Visible = ShowContentBeforeRows && hasData && !headersOnly;
                plcPager.Visible = hasData && !headersOnly;
                if (hasData)
                {
                    // Set correct ID for direct page contol
                    pagerElem.DirectPageControlID = ((float)mTotalRows / pagerElem.CurrentPageSize > 20.0f) ? "txtPage" : "drpPage";
                }


                mLoaded = true;

                // Call page binding event
                if (OnPageBinding != null)
                {
                    OnPageBinding(this, null);
                }
            }
        }
    }


    /// <summary>
    /// Filters the content.
    /// </summary>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        pagerElem.UniPager.CurrentPage = 1;

        // Get the expression
        string expr = txtFilter.Text.Trim();
        if (!String.IsNullOrEmpty(expr))
        {
            // Build the where condition for display name
            FilterWhere = RowItemDisplayNameColumn + " LIKE '%" + SqlHelperClass.GetSafeQueryString(expr, false) + "%'";
        }
        else
        {
            FilterWhere = null;
        }

        txtFilter.Focus();
    }


    /// <summary>
    /// Sets pager to first page.
    /// </summary>
    public void ResetPager()
    {
        pagerElem.UniPager.CurrentPage = 1;
    }


    /// <summary>
    /// Resets the pager and filter.
    /// </summary>
    public void ResetMatrix()
    {
        pagerElem.UniPager.CurrentPage = 1;
        txtFilter.Text = String.Empty;
        FilterWhere = null;
    }


    #region ICallbackEventHandler Members

    /// <summary>
    /// Gets the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        return mCallbackResult;
    }


    /// <summary>
    /// Processes the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        string[] parameters = eventArgument.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
        if ((parameters != null) && (parameters.Length == 4))
        {
            int rowItemId = ValidationHelper.GetInteger(parameters[1], 0);
            int colItemId = ValidationHelper.GetInteger(parameters[2], 0);
            bool newState = ValidationHelper.GetBoolean(parameters[3], false);

            // Raise the change
            RaiseOnItemChanged(rowItemId, colItemId, newState);

            mCallbackResult = newState.ToString().ToLowerCSafe() + "|" + ContentBeforeRows;
        }
    }

    #endregion


    #region "IUniPageable Members"

    /// <summary>
    /// Pager data item object.
    /// </summary>
    public object PagerDataItem
    {
        get
        {
            return ds;
        }
        set
        {
            ds = (DataSet)value;
        }
    }


    /// <summary>
    /// Pager control.
    /// </summary>
    public UniPager UniPagerControl
    {
        get;
        set;
    }


    /// <summary>
    /// Occurs when the control bind data.
    /// </summary>
    public event EventHandler<EventArgs> OnPageBinding;


    /// <summary>
    /// Occurs when the pager change the page and current mode is postback => reload data
    /// </summary>
    public event EventHandler<EventArgs> OnPageChanged;


    /// <summary>
    /// Evokes control databind.
    /// </summary>
    public void ReBind()
    {
        if (OnPageChanged != null)
        {
            OnPageChanged(this, null);
        }

        DataBind();
    }


    /// <summary>
    /// Gets or sets the number of result. Enables proceed "fake" datasets, where number 
    /// of results in the dataset is not correspondent to the real number of results
    /// This property must be equal -1 if should be disabled
    /// </summary>
    public int PagerForceNumberOfResults
    {
        get
        {
            return mTotalRows;
        }
        set
        {
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets an array of indexes which are sorted according to ColumnsPreferedOrder.
    /// i.e: Permission are "A","B","C" .. desired permission order is "C","A","B", then columnOrderIndex will be [2,0,1]
    /// </summary>
    /// <param name="columns">The columns</param>
    /// <param name="columnOrder">The column order</param>
    private int[] GetColumnIndexes(IList<DataRow> columns, string columnOrder)
    {
        List<string> order = new List<string>(columnOrder.Split(','));
        List<bool> indexIsSet = new List<bool>();
        List<int> columnOrderIndex = new List<int>();

        // Initialize array
        for (int i = 0; i < columns.Count; i++)
        {
            indexIsSet.Add(false);
        }

        // sort acording to defined order
        foreach (DataRow dr in columns)
        {
            int index = order.IndexOf(ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, "PermissionName"), ""));
            if (index > -1)
            {
                columnOrderIndex.Add(index);
                indexIsSet[index] = true;
            }
        }

        // Insert original colums which were not defined in columnOrder
        for (int i = 0; i < indexIsSet.Count; i++)
        {
            if (indexIsSet[i] == false)
            {
                columnOrderIndex.Add(i);
                indexIsSet[i] = true;
            }
        }

        return columnOrderIndex.ToArray();
    }


    /// <summary>
    /// Returns additional CSS class formatted in a way to be concatenated with other row CSS classes.
    /// </summary>
    /// <param name="dr">DataRow with matrix row data</param>
    private string GetAdditionalCssClass(DataRow dr)
    {
        string cssClass = RaiseGetRowItemCssClass(dr);
        if (!String.IsNullOrEmpty(cssClass))
        {
            cssClass = " " + cssClass;
        }
        return cssClass;
    }


    /// <summary>
    /// Sets page size dropdown list according to PageSize property.
    /// </summary>
    private void SetPageSize(bool forceReload)
    {
        if ((pagerElem.PageSizeDropdown.Items.Count == 0) || forceReload)
        {
            pagerElem.PageSizeDropdown.Items.Clear();

            string[] sizes = PageSizeOptions.Split(',');
            if (sizes.Length > 0)
            {
                List<int> sizesInt = new List<int>();
                // Indicates if contains 'Select ALL' macro
                bool containsAll = false;
                foreach (string size in sizes)
                {
                    if (size.ToUpperCSafe() == "##ALL##")
                    {
                        containsAll = true;
                    }
                    else
                    {
                        sizesInt.Add(ValidationHelper.GetInteger(size.Trim(), 0));
                    }
                }
                // Add default page size if not pressents
                if ((DefaultPageSize > 0) && !sizesInt.Contains(DefaultPageSize))
                {
                    sizesInt.Add(DefaultPageSize);
                }
                // Sort list of page sizes
                sizesInt.Sort();

                ListItem item = null;

                foreach (int size in sizesInt)
                {
                    // Skip zero values
                    if (size != 0)
                    {
                        item = new ListItem(size.ToString());
                        if (item.Value == DefaultPageSize.ToString())
                        {
                            item.Selected = true;
                        }
                        pagerElem.PageSizeDropdown.Items.Add(item);
                    }
                }
                // Add 'Select ALL' macro at the end of list
                if (containsAll)
                {
                    item = new ListItem(GetString("general.selectall"), "-1");
                    if (DefaultPageSize == -1)
                    {
                        item.Selected = true;
                    }
                    pagerElem.PageSizeDropdown.Items.Add(item);
                }
            }
        }
    }


    /// <summary>
    /// Returns safe and localized tooltip from the given source column.
    /// </summary>
    /// <param name="dr">Data row with the tooltip column</param>
    /// <param name="columnName">Name of the tooltip source column</param>
    private string GetTooltip(DataRow dr, string columnName)
    {
        // Get tooltip string
        string tooltip = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, columnName), "");

        // Get safe an localized tooltip
        if (!string.IsNullOrEmpty(tooltip))
        {
            return HTMLHelper.HTMLEncode(CMSContext.ResolveMacros(tooltip));
        }

        return "";
    }

    #endregion
}