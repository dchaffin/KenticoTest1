using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;
using CMS.URLRewritingEngine;

using Action = CMS.UIControls.UniGridConfig.Action;

public partial class CMSAdminControls_UI_UniGrid_UniGrid : UniGrid, IUniPageable
{
    #region "Constants"

    private const int halfPageCountLimit = 1000;

    #endregion


    #region "Variables"

    private Button showButton = null;

    private int rowIndex = 1;

    private bool resetSelection = false;

    private bool visiblePagesSet = false;

    private static string DEFAULT_ACTIONS_MENU = "~/CMSAdminControls/UI/UniGrid/Controls/UniGridMenu.ascx";

    private bool mShowObjectMenu = true;

    private bool mCheckRelative = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if control is used on live site
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            plcMess.IsLiveSite = value;
        }
    }


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
    /// Gets or sets GridView control of UniGrid.
    /// </summary>
    public override GridView GridView
    {
        get
        {
            return UniGridView;
        }
    }


    /// <summary>
    /// Hidden field containing selected items.
    /// </summary>
    public override HiddenField SelectionHiddenField
    {
        get
        {
            return hidSelection;
        }
    }


    /// <summary>
    /// Gets or sets UniGrid pager control of UniGrid.
    /// </summary>
    public override UniGridPager Pager
    {
        get
        {
            return pagerElem;
        }
    }


    /// <summary>
    /// If true, relative ancestor div is checked in context menu
    /// </summary>
    public bool CheckRelative
    {
        get
        {
            return mCheckRelative;
        }
        set
        {
            mCheckRelative = value;
        }
    }


    /// <summary>
    /// Gets selected items from UniGrid.
    /// </summary>
    public override List<string> SelectedItems
    {
        get
        {
            return GetSelectedItems();
        }
        set
        {
            SetSectedItems(value);
        }
    }


    /// <summary>
    /// Gets selected items from UniGrid.
    /// </summary>
    public override List<string> DeselectedItems
    {
        get
        {
            return GetDeselectedItems();
        }
    }


    /// <summary>
    /// Gets selected items from UniGrid.
    /// </summary>
    public override List<string> NewlySelectedItems
    {
        get
        {
            return GetNewlySelectedItems();
        }
    }


    /// <summary>
    /// Gets filter placeHolder from Unigrid.
    /// </summary>
    public override PlaceHolder FilterPlaceHolder
    {
        get
        {
            return filter;
        }
    }


    /// <summary>
    /// Gets page size dropdown from Unigrid Pager.
    /// </summary>
    public override DropDownList PageSizeDropdown
    {
        get
        {
            return Pager.PageSizeDropdown;
        }
    }


    /// <summary>
    /// Defines whether to show object menu (menu containing relationships, export/backup, destroy object, ... functionality) 
    /// </summary>
    public bool ShowObjectMenu
    {
        get
        {
            return mShowObjectMenu;
        }
        set
        {
            mShowObjectMenu = value;
        }
    }


    /// <summary>
    /// Gets filter form.
    /// </summary>
    public override BasicForm FilterForm
    {
        get
        {
            return filterForm;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Control's init event handler.
    /// </summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        advancedExportElem.UniGrid = this;
    }


    /// <summary>
    /// Control's load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
            FilterForm.StopProcessing = true;
        }
        else
        {
            SetPager();

            if (LoadGridDefinition())
            {
                // Do not load on callback
                if (!RequestHelper.IsCallback())
                {
                    ActionsHidden = hidActions;
                    ActionsHashHidden = hidActionsHash;

                    // Check whether current request is row action command and if so, raise action
                    if (!String.IsNullOrEmpty(hidCmdName.Value) && (Request.Form["__EVENTTARGET"] == UniqueID) && ((Request.Form["__EVENTARGUMENT"] == "UniGridAction")))
                    {
                        HandleAction(hidCmdName.Value, hidCmdArg.Value);
                    }

                    // Set order by clause
                    ProcessSorting();

                    // Set filter form
                    if (!string.IsNullOrEmpty(FilterFormName))
                    {
                        SetBasicFormFilter();
                        if (!EventRequest() && !DelayedReload)
                        {
                            ReloadData();
                        }
                    }
                    // Get data from database and set them to the grid view
                    else if (FilterByQueryString)
                    {
                        if (displayFilter)
                        {
                            SetFilter(true);
                        }
                        else
                        {
                            if (!EventRequest() && !DelayedReload)
                            {
                                ReloadData();
                            }
                        }
                    }
                    else
                    {
                        // Load the default filter value
                        if (!RequestHelper.IsPostBack() && displayFilter)
                        {
                            SetFilter(true);
                        }
                        else if (!EventRequest() && !DelayedReload)
                        {
                            ReloadData();
                        }
                    }
                }
            }
        }

        // Clear hidden action on load event. If unigrid is invisible, page pre render is not fired
        ClearActions();
    }


    /// <summary>
    /// Control's PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (FilterIsSet)
        {
            // Check for FilteredZeroRowsText
            if ((GridView.Rows.Count == 0) && !String.IsNullOrEmpty(FilteredZeroRowsText))
            {
                // Display filter zero rows text
                lblInfo.Text = FilteredZeroRowsText;
                lblInfo.Visible = true;
                pagerElem.Visible = false;
            }
            else
            {
                lblInfo.Visible = false;
                pagerElem.Visible = true;
            }
        }
        else
        {
            // Check for ZeroRowsText
            if (GridView.Rows.Count == 0)
            {
                if (!HideControlForZeroRows && !String.IsNullOrEmpty(ZeroRowsText))
                {
                    // Display zero rows text
                    lblInfo.Text = ZeroRowsText;
                    lblInfo.Visible = true;
                    pagerElem.Visible = false;
                    // Check additional filter visibility
                    CheckFilterVisibility();
                }
                else
                {
                    lblInfo.Visible = false;
                    pagerElem.Visible = false;
                    filter.Visible = false;
                }
            }
            else
            {
                lblInfo.Visible = false;
                pagerElem.Visible = true;
                // Check additional filter visibility
                CheckFilterVisibility();
            }
        }

        if (Visible && !StopProcessing)
        {
            RegisterCmdScripts();
        }

        if (Pager.CurrentPage > halfPageCountLimit)
        {
            // Enlarge direct page textbox
            TextBox txtPage = ControlsHelper.GetChildControl(pagerElem, typeof(TextBox), "txtPage") as TextBox;
            if (txtPage != null)
            {
                txtPage.Style.Add(HtmlTextWriterStyle.Width, "50px");
            }
        }

        advancedExportElem.Visible = ShowActionsMenu;

        // Hide info label when error message is displayed
        lblInfo.Visible = lblInfo.Visible && string.IsNullOrEmpty(plcMess.ErrorText);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Clears UniGrid's information on recently performed action. Under normal circumstances there is no need to perform this action.
    /// However sometimes forcing grid to clear the actions is required.
    /// </summary>
    public void ClearActions()
    {
        // Clear hidden fields
        hidCmdName.Value = null;
        hidCmdArg.Value = null;
    }


    /// <summary>
    /// Clears all selected items from hidden values.
    /// </summary>
    public void ClearSelectedItems()
    {
        ClearHiddenValues(SelectionHiddenField);
    }


    /// <summary>
    /// Loads the XML configuration of the grid.
    /// </summary>
    public bool LoadXmlConfiguration()
    {
        // If no configuration is given, do not process
        if (string.IsNullOrEmpty(GridName))
        {
            return true;
        }
        string xmlFilePath = Server.MapPath(GridName);

        // Check the configuration file
        if (!File.Exists(xmlFilePath))
        {
            ShowError(String.Format(GetString("unigrid.noxmlfile"), xmlFilePath));
            return false;
        }

        // Load the XML configuration
        XmlDocument document = new XmlDocument();
        document.Load(xmlFilePath);
        XmlNode node = document.DocumentElement;

        if (node != null)
        {
            // Load options definition
            XmlNode optionNode = node.SelectSingleNode("options");
            if (optionNode != null)
            {
                GridOptions = new UniGridOptions(optionNode);
            }

            // Load actions definition
            XmlNode actionsNode = node.SelectSingleNode("actions");
            if (actionsNode != null)
            {
                GridActions = new UniGridActions(actionsNode);
            }

            // Load pager definition
            XmlNode pagerNode = node.SelectSingleNode("pager");
            if (pagerNode != null)
            {
                PagerConfig = new UniGridPagerConfig(pagerNode);
            }

            // Select list of "column" nodes
            XmlNode columnsNode = node.SelectSingleNode("columns");
            if (columnsNode != null)
            {
                GridColumns = new UniGridColumns(columnsNode);
            }

            // Try to get ObjectType from definition
            XmlNode objectTypeNode = node.SelectSingleNode("objecttype");
            if (objectTypeNode != null)
            {
                // Get object type information
                LoadObjectTypeDefinition(objectTypeNode);
            }
            else
            {
                // Get query information
                XmlNode queryNode = node.SelectSingleNode("query");
                LoadQueryDefinition(queryNode);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Loads the grid definition.
    /// </summary>
    public override bool LoadGridDefinition()
    {
        if (GridView.Columns.Count == 0)
        {
            using (Table filterTable = new Table())
            {
                filterTable.CssClass = "UniGridFilterTable";
                filter.Controls.Clear();
                // Clear all columns from the grid view
                UniGridView.Columns.Clear();
                UniGridView.GridLines = GridLines.Horizontal;
                if (!LoadXmlConfiguration())
                {
                    return false;
                }
                // Load options
                if (GridOptions != null)
                {
                    LoadOptionsDefinition(GridOptions, filterTable);
                }
                if (GridActions == null && ShowActionsMenu)
                {
                    EmptyAction emptyAction = new EmptyAction();
                    GridActions = new UniGridActions();
                    GridActions.Actions.Add(emptyAction);
                }
                // Actions
                if (GridActions != null)
                {
                    LoadActionsDefinition(GridActions);
                }
                // Load pager configuration
                if (PagerConfig != null)
                {
                    LoadPagerDefinition(PagerConfig);
                }
                // Set direct page control id from viewstate
                if (ViewState["DirectPageControlID"] != null)
                {
                    Pager.DirectPageControlID = ViewState["DirectPageControlID"].ToString();
                }
                // Raise load columns event
                RaiseLoadColumns();
                // Load columns
                if (GridColumns != null)
                {
                    foreach (Column col in GridColumns.Columns)
                    {
                        // Load column definition
                        LoadColumnDefinition(col, filterTable);
                    }
                }
                if (displayFilter)
                {
                    // Finish filter form with "Show" button
                    CreateFilterButton(filterTable);
                }
            }
        }
        return true;
    }


    /// <summary>
    /// Reloads the grid data.
    /// </summary>
    public override void ReloadData()
    {
        try
        {
            // Ensure grid definition before realod data
            LoadGridDefinition();
            RestoreState();

            RaiseOnBeforeDataReload();
            rowIndex = 1;

            // Get Current TOP N
            if (CurrentPageSize > 0)
            {
                int currentPageIndex = Pager.CurrentPage;
                int pageSize = (CurrentPageSize > 0) ? CurrentPageSize : UniGridView.PageSize;

                CurrentTopN = pageSize * (currentPageIndex + Pager.CurrentPagesGroupSize);
            }

            if (CurrentTopN < TopN)
            {
                CurrentTopN = TopN;
            }

            // If first/last button and direct page contol in pager is hidden use current topN for better performance
            if (!Pager.ShowDirectPageControl && !Pager.ShowFirstLastButtons)
            {
                TopN = CurrentTopN;
            }

            // Retrieve data
            UniGridView.DataSource = RetrieveData();

            RaiseOnAfterDataReload();

            SetUnigridControls();

            // Check if datasource is loaded
            if (DataHelper.DataSourceIsEmpty(GridView.DataSource) && (pagerElem.CurrentPage > 1))
            {
                pagerElem.UniPager.CurrentPage = 1;
                ReloadData();
            }

            // Resolve the edit action URL
            if (!String.IsNullOrEmpty(EditActionUrl))
            {
                EditActionUrl = CMSContext.ResolveMacros(EditActionUrl);
            }

            SortColumns.Clear();
            UniGridView.DataBind();

            mRowsCount = DataHelper.GetItemsCount(UniGridView.DataSource);

            CheckFilterVisibility();
        }
        catch (Exception ex)
        {
            // Display tooltip only development mode is enabled
            string desc = null;
            if (SettingsKeyProvider.DevelopmentMode)
            {
                desc = ex.Message;
            }

            ShowError(GetString("unigrid.error.reload"), desc, null);

            // Log exception
            EventLogProvider ev = new EventLogProvider();
            ev.LogEvent("UniGrid", "RELOADDATA", ex.InnerException ?? ex);
        }
    }


    /// <summary>
    /// Gets a dataset with data based on UniGrid's settings.
    /// </summary>
    /// <returns>DataSet with data</returns>
    public override DataSet RetrieveData()
    {
        DataSet ds = null;

        // If datasource for unigrid is query (not dataset), then execute query
        if (!string.IsNullOrEmpty(Query))
        {
            // Reload the data with current parameters
            ds = ConnectionHelper.ExecuteQuery(Query, QueryParameters, CompleteWhereCondition, CurrentOrder, TopN, Columns, CurrentOffset, CurrentPageSize, ref pagerForceNumberOfResults);
        }
        // If UniGrid is in ObjectType mode, get the data according to given object type.
        else if (InfoObject != null)
        {
            // Get the result set
            ds = InfoObject.GetData(QueryParameters, CompleteWhereCondition, CurrentOrder, TopN, Columns, false, CurrentOffset, CurrentPageSize, ref pagerForceNumberOfResults);
        }
        // External dataset is used
        else
        {
            ds = RaiseDataReload();
            SortUniGridDataSource(ds);
            ds = DataHelper.TrimDataSetPage(ds, CurrentOffset, CurrentPageSize, ref pagerForceNumberOfResults);
        }
        // Add empty dataset
        if (ds == null)
        {
            ds = new DataSet();
            ds.Tables.Add();
        }

        // Raise event 'OnRetrieveData'
        ds = RaiseAfterRetrieveData(ds);

        return ds;
    }


    /// <summary>
    /// Returns where condition from unigrid filters.
    /// </summary>
    public override string GetFilter(bool isDataTable)
    {
        string where = string.Empty;

        // Count of the conditions in the 'where clause'
        int whereConditionCount = 0;

        // Process all filter fields
        foreach (UniGridFilterField filterField in FilterFields.Values)
        {
            string filterFormat = filterField.Format;
            // AND in 'where clause'  
            string andExpression;
            Control mainControl = filterField.OptionsControl;
            Control valueControl = filterField.ValueControl;
            string filterPath = filterField.ControlPath;

            if (valueControl is CMSAbstractBaseFilterControl)
            {
                // Custom filters (loaded controls)
                CMSAbstractBaseFilterControl customFilter = (CMSAbstractBaseFilterControl)valueControl;
                string customWhere = customFilter.WhereCondition;
                if (!String.IsNullOrEmpty(customWhere))
                {
                    andExpression = (whereConditionCount > 0) ? " AND " : "";
                    where += andExpression + customWhere;
                    whereConditionCount++;
                }

                // Prepare query string
                if (FilterByQueryString && RequestHelper.IsPostBack())
                {
                    queryStringHashTable[customFilter.ID] = customFilter.Value;
                }
            }
            else if (mainControl is DropDownList)
            {
                // Dropdown list filter
                DropDownList ddlistControl = (DropDownList)mainControl;
                TextBox txtControl = (TextBox)valueControl;

                string textboxValue = txtControl.Text;
                string textboxID = txtControl.ID;

                // Empty field -> no filter is set for this field
                if (textboxValue != "")
                {
                    string op = ddlistControl.SelectedValue;
                    string value = textboxValue.Replace("\'", "''");
                    string columnName = ddlistControl.ID.Trim().TrimStart('[').TrimEnd(']').Trim();

                    // Format {0} = column name, {1} = operator, {2} = value, {3} = default condition
                    string defaultFormat = null;

                    if (textboxID.EndsWithCSafe("TextValue"))
                    {
                        switch (op.ToLowerCSafe())
                        {
                            // LIKE operators
                            case "like":
                                defaultFormat = isDataTable ? "[{0}] {1} '%{2}%'" : "[{0}] {1} N'%{2}%'";
                                break;

                            case "not like":
                                defaultFormat = isDataTable ? "([{0}] is null or [{0}] {1} '%{2}%')" : "([{0}] is null or [{0}] {1} N'%{2}%')";
                                break;

                            case "<>":
                                defaultFormat = isDataTable ? "([{0}] is null or [{0}] {1} '{2}')" : "([{0}] is null or [{0}] {1} N'{2}')";
                                break;

                            // Standard operators
                            default:
                                defaultFormat = isDataTable ? "[{0}] {1} '{2}'" : "[{0}] {1} N'{2}'";
                                break;
                        }
                    }
                    else // textboxID.EndsWithCSafe("NumberValue")
                    {
                        if (ValidationHelper.IsDouble(value) || ValidationHelper.IsInteger(value))
                        {
                            defaultFormat = "[{0}] {1} {2}";

                            if (op == "<>")
                            {
                                defaultFormat = "([{0}] is null or [{0}] {1} {2})";
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(defaultFormat))
                    {
                        string defaultCondition = String.Format(defaultFormat, columnName, op, value);

                        string condition = defaultCondition;
                        if (filterFormat != null)
                        {
                            condition = String.Format(filterFormat, columnName, op, value, defaultCondition);
                        }

                        andExpression = (whereConditionCount > 0 ? " AND " : string.Empty);

                        // ddlistControl.ID                 - column name
                        // ddlistControl.SelectedValue      - condition option
                        // textboxSqlValue                  - condition value                        
                        where += String.Format("{0}({1})", andExpression, condition);
                        whereConditionCount++;
                    }
                }

                // Prepare query string
                if (FilterByQueryString)
                {
                    queryStringHashTable[ddlistControl.ID] = String.Format("{0};{1}", ddlistControl.SelectedValue, textboxValue);
                }
            }
            else if (valueControl is DropDownList)
            {
                // Checkbox filter
                DropDownList currentControl = (DropDownList)valueControl;
                string value = currentControl.SelectedValue;
                if (value != "")
                {
                    string columnName = currentControl.ID;
                    string condition = String.Format("{0} = {1}", columnName, value);
                    if (filterFormat != null)
                    {
                        condition = String.Format(filterFormat, columnName, "=", value, condition);
                    }
                    andExpression = (whereConditionCount > 0 ? " AND " : string.Empty);
                    where += String.Format("{0}({1})", andExpression, condition);
                    whereConditionCount++;
                }

                // Prepare query string
                if (FilterByQueryString)
                {
                    queryStringHashTable[currentControl.ID] = ";" + value;
                }
            }
        }

        return where;
    }


    /// <summary>
    /// Uncheck all checkboxes in selection column.
    /// </summary>
    public override void ResetSelection()
    {
        ResetSelection(true);
    }


    /// <summary>
    /// Uncheck all checkboxes in selection column.
    /// </summary>
    /// <param name="reset">Indicates if reset selection javascript should be registered</param>
    public void ResetSelection(bool reset)
    {
        SelectionHiddenField.Value = String.Empty;
        hidNewSelection.Value = String.Empty;
        hidDeSelection.Value = String.Empty;
        resetSelection = reset;
    }

    #endregion


    #region "UniGrid events"

    /// <summary>
    /// Process data from filter.
    /// </summary>
    protected void ShowButton_Click(object sender, EventArgs e)
    {
        ApplyFilter(sender, e);
    }


    protected void ResetButton_Click(object sender, EventArgs e)
    {
        Reset();
    }


    protected void pageSizeDropdown_SelectedIndexChanged(object sender, EventArgs e)
    {
        RaisePageSizeChanged();
    }


    protected void UniGridView_Sorting(object sender, EventArgs e)
    {
        RaiseBeforeSorting(sender, e);
    }


    /// <summary>
    /// After data bound event.
    /// </summary>
    protected void UniGridView_DataBound(object sender, EventArgs e)
    {
        // Set actions hash into hidden field
        SetActionsHash();

        SetPager();

        // Call page binding event
        if (OnPageBinding != null)
        {
            OnPageBinding(this, null);
        }
    }


    protected void UniGridView_RowCreating(object sender, GridViewRowEventArgs e)
    {
        // If row type is header
        if (e.Row.RowType == DataControlRowType.Header)
        {
            // Add sorting definition to list of sort columns
            SortColumns.Add(SortDirect.ToLowerCSafe());

            // Parse the sort expression
            string sort = SortDirect.ToLowerCSafe().Replace("[", "").Replace("]", "").Trim();
            if (sort.StartsWithCSafe("cast("))
            {
                sort = sort.Substring(5);
            }

            Match sortMatch = OrderByRegex.Match(sort);
            string sortColumn = null;
            string sortDirection = null;
            if (sortMatch.Success)
            {
                // Get column name
                if (sortMatch.Groups[1].Success)
                {
                    sortColumn = sortMatch.Groups[1].Value;
                }
                // Get sort direction
                sortDirection = sortMatch.Groups[2].Success ? sortMatch.Groups[2].Value : "asc";
            }
            else
            {
                // Get column name from sort expression
                int space = sort.IndexOfAny(new char[] { ' ', ',' });
                sortColumn = space > -1 ? sort.Substring(0, space) : sort;
                sortDirection = "asc";
            }

            // Check if displaying arrow indicating sorting is requested
            if (showSortDirection)
            {
                // Prepare the columns
                foreach (TableCell Cell in e.Row.Cells)
                {
                    // If there is some sorting expression
                    DataControlFieldCell dataField = (DataControlFieldCell)Cell;
                    string fieldSortExpression = dataField.ContainingField.SortExpression;
                    if (!DataHelper.IsEmpty(fieldSortExpression))
                    {
                        SortColumns.Add(fieldSortExpression.ToLowerCSafe());

                        // If actual sorting expressions is this cell
                        if (CMSString.Equals(sortColumn, fieldSortExpression.Replace("[", "").Replace("]", "").Trim(), true))
                        {
                            // Initialize sort arrow
                            Literal sortArrow = new Literal()
                                                    {
                                                        Text = String.Format("<span class=\"UniGridSort{0}\" >&nbsp;&nbsp;&nbsp;</span>", ((sortDirection == "desc") ? "Down" : "Up"))
                                                    };

                            if (DataHelper.IsEmpty(Cell.Text))
                            {
                                if ((Cell.Controls.Count != 0) && (Cell.Controls[0] != null))
                                {
                                    // Add original text
                                    Cell.Controls[0].Controls.Add(new LiteralControl(String.Format("<span class=\"UniGridSortLabel\">{0}</span>", ((LinkButton)(Cell.Controls[0])).Text)));
                                    // Add one space before image
                                    Cell.Controls[0].Controls.Add(new LiteralControl("&nbsp;"));
                                    Cell.Controls[0].Controls.Add(sortArrow);
                                }
                                else
                                {
                                    // Add one space before image
                                    Cell.Controls.Add(new LiteralControl("&nbsp;"));
                                    Cell.Controls.Add(sortArrow);
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.CssClass = (rowIndex % 2 == 0) ? "OddRow" : "EvenRow";
            rowIndex++;
        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.CssClass = "UniGridFooter";
        }
        else if (e.Row.RowType == DataControlRowType.Pager)
        {
            e.Row.CssClass = "UniGridPager";
        }
    }


    /// <summary>
    /// Handles the action event.
    /// </summary>
    /// <param name="cmdName">Command name</param>
    /// <param name="cmdValue">Command value</param>
    public void HandleAction(string cmdName, string cmdValue)
    {
        string action = cmdName.ToLowerCSafe();

        // Check action security and redirect if user not authorized
        CheckActionAndRedirect(action);
        GeneralizedInfo infoObj = null;
        int objectId = 0;
        switch (action)
        {
            case "#delete":
            case "#destroyobject":
                {
                    // Delete the object
                    objectId = ValidationHelper.GetInteger(cmdValue, 0);
                    if (objectId > 0)
                    {
                        infoObj = CMSObjectHelper.GetReadOnlyObject(ObjectType);
                        string objectType = infoObj.TypeInfo.Inherited ? infoObj.TypeInfo.OriginalObjectType : infoObj.ObjectType;
                        infoObj = BaseAbstractInfoProvider.GetInfoById(objectType, objectId);

                        if (infoObj != null)
                        {
                            switch (action)
                            {
                                case "#delete":
                                    // Check the dependencies
                                    AbstractProvider providerObj = infoObj.TypeInfo.ProviderObject;
                                    List<string> names = new List<string>();
                                    if (providerObj.CheckObjectDependencies(objectId, ref names))
                                    {
                                        string description = null;
                                        if (names.Count > 0)
                                        {
                                            // Encode and localize names
                                            StringBuilder sb = new StringBuilder();
                                            names.ForEach(item => sb.Append("<br />", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(item))));
                                            description = GetString(objectType.Replace(".", "_") + ".objectlist|unigrid.objectlist") + sb.ToString();
                                        }

                                        ShowError(GetString("ecommerce.deletedisabledwithoutenable"), description, null);
                                        return;
                                    }

                                    // Delete the object
                                    infoObj.DeleteObject();
                                    break;

                                case "#destroyobject":
                                    if (CMSContext.CurrentUser.IsAuthorizedPerObject(PermissionsEnum.Destroy, infoObj.ObjectType, CMSContext.CurrentSiteName))
                                    {
                                        using (CMSActionContext context = new CMSActionContext())
                                        {
                                            context.CreateVersion = false;

                                            Action ac = GridActions.GetAction("#delete");
                                            if (ac != null)
                                            {
                                                HandleAction("#delete", cmdValue);
                                            }
                                            else
                                            {
                                                ac = GridActions.GetAction("delete");
                                                if (ac != null)
                                                {
                                                    RaiseAction("delete", cmdValue);
                                                }
                                                else
                                                {
                                                    ShowError(GetString("objectversioning.destroyobject.nodeleteaction"));
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                break;


            default:
                RaiseAction(cmdName, cmdValue);
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Sets unigrid controls.
    /// </summary>
    private void SetUnigridControls()
    {
        filter.Visible = displayFilter;

        // Indicates whether unigrid datasource is empty or not
        isEmpty = DataHelper.DataSourceIsEmpty(UniGridView.DataSource);

        if (isEmpty)
        {
            // Try to reload data for previous page if action was used and no data loaded (mostly delete)
            if (onActionUsed && Pager.CurrentPage > 1)
            {
                Pager.UniPager.CurrentPage = Pager.CurrentPage - 1;
                ReloadData();
            }
            else if (HideControlForZeroRows && (WhereClause == ""))
            {
                // Hide filter
                filter.Visible = false;
            }
        }
        else
        {
            // Disable GridView paging because UniGridPager will provide paging
            UniGridView.AllowPaging = false;

            // Process paging if pager is displayed
            if (Pager.DisplayPager)
            {
                if (CurrentPageSize > 0)
                {
                    if (!visiblePagesSet)
                    {
                        Pager.VisiblePages = (((CurrentOffset / CurrentPageSize) + 1)) > halfPageCountLimit ? 5 : 10;
                    }

                    Pager.DirectPageControlID = ((float)PagerForceNumberOfResults / Pager.CurrentPageSize > 20.0f) ? "txtPage" : "drpPage";
                    // Save direct page control id in viewstate
                    ViewState["DirectPageControlID"] = Pager.DirectPageControlID;
                }
            }
        }
    }


    /// <summary>
    /// Load options definition.
    /// </summary>
    /// <param name="options">Options configuration</param>
    /// <param name="filterTable">Table for filter</param>
    private void LoadOptionsDefinition(UniGridOptions options, Table filterTable)
    {
        // Create filter table according to the key value "DisplayFilter"            
        displayFilter = options.DisplayFilter;
        if (displayFilter)
        {
            filter.Controls.Add(filterTable);
        }

        // Filter limit
        if (options.FilterLimit > -1)
        {
            FilterLimit = options.FilterLimit;
        }

        // Display sort direction images
        showSortDirection = options.ShowSortDirection;

        // Display selection column with checkboxes
        showSelection = options.ShowSelection;
        if (showSelection)
        {
            TemplateField chkColumn = new TemplateField();

            using (CheckBox headerBox = new CheckBox { ID = "headerBox" })
            {
                using (CheckBox itemBox = new CheckBox { ID = "itemBox" })
                {
                    // Set selection argument
                    itemBox.Attributes["selectioncolumn"] = options.SelectionColumn;
                    chkColumn.HeaderTemplate = new GridViewTemplate(ListItemType.Header, this, headerBox);
                    chkColumn.ItemTemplate = new GridViewTemplate(ListItemType.Item, this, itemBox);
                }
            }
            UniGridView.Columns.Add(chkColumn);
        }

        // Get pagesize options
        if (!String.IsNullOrEmpty(options.PageSize))
        {
            Pager.PageSizeOptions = options.PageSize;
        }

        // Set pagging acording to the key value "DisplayPageSizeDropdown"                
        if (options.DisplayPageSizeDropdown != null)
        {
            Pager.ShowPageSize = options.DisplayPageSizeDropdown.Value;
        }
    }


    /// <summary>
    /// Loads actions definition.
    /// </summary>
    /// <param name="actions">Configuration of the actions</param>
    private void LoadActionsDefinition(UniGridActions actions)
    {
        // Custom template field of the grid view
        TemplateField actionsColumn = new TemplateField();

        // Ensure width of the column
        if (!String.IsNullOrEmpty(actions.Width))
        {
            actionsColumn.ItemStyle.Width = new Unit(actions.Width);
        }

        // Add object menu if possible
        if ((actions.Actions.Count > 0 && !(actions.Actions.FirstOrDefault() is EmptyAction)) && ShowObjectMenu && UniGridFunctions.ShowUniGridObjectContextMenu(CMSObjectHelper.GetReadOnlyObject(ObjectType)))
        {
            actions.Actions.RemoveAll(a => a is EmptyAction);
            // Check if object menu already contained
            var menus = from action in actions.Actions.OfType<Action>()
                        where (action.Name.ToLowerCSafe() == "#objectmenu") || (!String.IsNullOrEmpty(action.ContextMenu))
                        select action;

            // Add object menu of necessary
            if ((menus.Count() == 0) && !IsLiveSite)
            {
                Action action = new Action("#objectmenu");
                action.ExternalSourceName = "#objectmenu";
                actions.Actions.Add(action);
            }
        }

        // Show header?
        if (actions.ShowHeader)
        {
            if (ShowActionsMenu && string.IsNullOrEmpty(actions.ContextMenu))
            {
                actions.ContextMenu = DEFAULT_ACTIONS_MENU;
                actions.Caption = "General.OtherActions";
            }
            // Fill in the custom template field
            string label = (ShowActionsLabel ? GetString("unigrid.actions") : "");

            GridViewTemplate headerTemplate = new GridViewTemplate(ListItemType.Header, this, actions, label, ImageDirectoryPath, DefaultImageDirectoryPath, Page);

            headerTemplate.ContextMenuParent = plcContextMenu;
            headerTemplate.CheckRelative = CheckRelative;

            actionsColumn.HeaderTemplate = headerTemplate;

            if (ShowActionsMenu)
            {
                if (actions.Actions.FirstOrDefault() is EmptyAction)
                {
                    actionsColumn.HeaderStyle.CssClass = "EmptyAC";
                }
                else
                {
                    actionsColumn.HeaderStyle.CssClass = "AC";
                }
            }
        }
        GridViewTemplate actionsTemplate = new GridViewTemplate(ListItemType.Item, this, actions, null, ImageDirectoryPath, DefaultImageDirectoryPath, Page);
        actionsTemplate.OnExternalDataBound += RaiseExternalDataBound;
        actionsTemplate.ContextMenuParent = plcContextMenu;
        actionsTemplate.CheckRelative = CheckRelative;
        actionsColumn.ItemTemplate = actionsTemplate;

        if (!IsLiveSite)
        {
            actionsColumn.ItemStyle.CssClass = "NW UniGridActions";
        }
        else
        {
            actionsColumn.ItemStyle.CssClass = "UniGridActions";
            actionsColumn.ItemStyle.Wrap = false;
        }

        // CSS class name
        string cssClass = actions.CssClass;
        if (cssClass != null)
        {
            actionsColumn.HeaderStyle.CssClass += " " + cssClass;
            actionsColumn.ItemStyle.CssClass += " " + cssClass;
            actionsColumn.FooterStyle.CssClass += " " + cssClass;
        }

        // Add custom column to grid view
        UniGridView.Columns.Add(actionsColumn);
    }


    /// <summary>
    /// Load unigrid pager configuration.
    /// </summary>
    /// <param name="config">Pager configuration</param>
    private void LoadPagerDefinition(UniGridPagerConfig config)
    {
        if (config.DisplayPager != null)
        {
            Pager.DisplayPager = config.DisplayPager.Value;
        }

        // Load definition only if pager is displayed
        if (Pager.DisplayPager)
        {
            if (config.PageSizeOptions != null)
            {
                Pager.PageSizeOptions = config.PageSizeOptions;
            }
            if (config.ShowDirectPageControl != null)
            {
                Pager.ShowDirectPageControl = config.ShowDirectPageControl.Value;
            }
            if (config.ShowFirstLastButtons != null)
            {
                Pager.ShowFirstLastButtons = config.ShowFirstLastButtons.Value;
            }
            if (config.ShowPageSize != null)
            {
                Pager.ShowPageSize = config.ShowPageSize.Value;
            }
            if (config.ShowPreviousNextButtons != null)
            {
                Pager.ShowPreviousNextButtons = config.ShowPreviousNextButtons.Value;
            }
            if (config.ShowPreviousNextPageGroup != null)
            {
                Pager.ShowPreviousNextPageGroup = config.ShowPreviousNextPageGroup.Value;
            }
            if (config.VisiblePages > 0)
            {
                Pager.VisiblePages = config.VisiblePages;
                visiblePagesSet = true;
            }
            if (config.DefaultPageSize > 0)
            {
                Pager.DefaultPageSize = config.DefaultPageSize;
            }

            // Try to get page size from request
            string selectedPageSize = Request.Form[Pager.PageSizeDropdown.UniqueID];
            int pageSize = 0;

            if (selectedPageSize != null)
            {
                pageSize = ValidationHelper.GetInteger(selectedPageSize, 0);
            }
            else if (config.DefaultPageSize > 0)
            {
                pageSize = config.DefaultPageSize;
            }

            if ((pageSize > 0) || (pageSize == -1))
            {
                Pager.CurrentPageSize = pageSize;
            }
        }
        else
        {
            // Reset page size
            Pager.CurrentPageSize = -1;
        }
    }


    /// <summary>
    /// Load single column definition.
    /// </summary>
    /// <param name="column">Column to use</param>
    /// <param name="filterTable">Table for filter</param>
    private void LoadColumnDefinition(Column column, Table filterTable)
    {
        DataControlField field = null;
        string cssClass = column.CssClass;
        string columnCaption = null;

        // Process the column type Hyperlink or BoundColumn based on the parameters
        if ((column.HRef != null) ||
            (column.ExternalSourceName != null) ||
            (column.Localize) ||
            (column.Icon != null) ||
            (column.Tooltip != null) ||
            (column.Action != null) ||
            (column.Style != null) ||
            (column.MaxLength > 0))
        {
            ExtendedBoundField linkColumn = new ExtendedBoundField();
            field = linkColumn;

            // Attribute "source"
            if (column.Source != null)
            {
                linkColumn.DataField = column.Source;
                if (column.AllowSorting)
                {
                    if (!String.IsNullOrEmpty(column.Sort))
                    {
                        linkColumn.SortExpression = column.Sort;
                    }
                    else if (column.Source.ToLowerCSafe() != ExtendedBoundField.ALL_DATA.ToLowerCSafe())
                    {
                        linkColumn.SortExpression = column.Source;
                    }
                }
            }

            // Action parameters
            if (column.Action != null)
            {
                linkColumn.Action = column.Action;

                // Action parameters
                if (column.CommandArgument != null)
                {
                    linkColumn.CommandArgument = column.CommandArgument;
                }
            }

            // Action parameters
            if (column.Parameters != null)
            {
                linkColumn.ActionParameters = column.Parameters;
            }

            // Navigate URL
            if (column.HRef != null)
            {
                linkColumn.NavigateUrl = column.HRef;
            }

            // External source
            if (column.ExternalSourceName != null)
            {
                linkColumn.ExternalSourceName = column.ExternalSourceName;
                linkColumn.OnExternalDataBound += RaiseExternalDataBound;
            }

            // Localize strings?
            linkColumn.LocalizeStrings = column.Localize;

            // Style
            if (column.Style != null)
            {
                linkColumn.Style = column.Style;
            }

            // Class name
            if (cssClass != null)
            {
                linkColumn.HeaderStyle.CssClass += " " + cssClass;
                linkColumn.ItemStyle.CssClass += " " + cssClass;
                linkColumn.FooterStyle.CssClass += " " + cssClass;
            }

            // Icon
            if (column.Icon != null)
            {
                if (linkColumn.DataField == "")
                {
                    linkColumn.DataField = ExtendedBoundField.ALL_DATA;
                }
                linkColumn.Icon = GetActionImage(column.Icon);
            }

            // Max length
            if (column.MaxLength > 0)
            {
                linkColumn.MaxLength = column.MaxLength;
            }

            // Process "tooltip" node
            ColumnTooltip tooltip = column.Tooltip;
            if (tooltip != null)
            {
                // If there is some tooltip register TooltipScript
                if ((tooltip.Source != null) || (tooltip.ExternalSourceName != null))
                {
                    ScriptHelper.RegisterTooltip(Page);
                }

                // Tooltip source
                if (tooltip.Source != null)
                {
                    linkColumn.TooltipSourceName = tooltip.Source;
                }

                // Tooltip external source
                if (tooltip.ExternalSourceName != null)
                {
                    linkColumn.TooltipExternalSourceName = tooltip.ExternalSourceName;
                    // Ensure external data bound event handler
                    if (string.IsNullOrEmpty(column.ExternalSourceName))
                    {
                        linkColumn.OnExternalDataBound += RaiseExternalDataBound;
                    }
                }

                // Tooltip width
                if (tooltip.Width != null)
                {
                    linkColumn.TooltipWidth = tooltip.Width;
                }

                // Encode tooltip
                linkColumn.TooltipEncode = tooltip.Encode;
            }
        }
        else
        {
            BoundField userColumn = new BoundField(); // Custom column of the grid view
            field = userColumn;

            // Attribute "source"
            if (column.Source != null)
            {
                userColumn.DataField = column.Source;

                // Allow sorting
                if (column.AllowSorting)
                {
                    if (column.Source.ToLowerCSafe() != ExtendedBoundField.ALL_DATA.ToLowerCSafe())
                    {
                        userColumn.SortExpression = column.Source;
                    }
                    else if (column.Sort != null)
                    {
                        userColumn.SortExpression = column.Sort;
                    }
                }
            }
        }

        if (!IsLiveSite)
        {
            field.HeaderStyle.CssClass = "NW";
        }
        else
        {
            field.HeaderStyle.Wrap = false;
        }

        // Column name
        if (column.Name != null)
        {
            NamedColumns[column.Name] = field;
        }

        // Caption
        if (column.Caption != null)
        {
            columnCaption = GetString(ResHelper.GetResourceName(column.Caption));
            field.HeaderText = columnCaption;
        }

        // Width
        if (column.Width != null)
        {
            if (GridView.ShowHeader)
            {
                field.HeaderStyle.Width = new Unit(column.Width);
            }
            else
            {
                field.ItemStyle.Width = new Unit(column.Width);
            }
        }

        // Visible
        field.Visible = column.Visible;

        // Is text?
        if (column.IsText && (column.Source != null))
        {
            TextColumns.Add(column.Source);
        }

        // Wrap?
        if (!column.Wrap)
        {
            if (!IsLiveSite)
            {
                field.ItemStyle.CssClass = "NW";
            }
            else
            {
                field.ItemStyle.Wrap = false;
            }
        }

        // Class name
        if (cssClass != null)
        {
            field.HeaderStyle.CssClass += " " + cssClass;
            field.ItemStyle.CssClass += " " + cssClass;
            field.FooterStyle.CssClass += " " + cssClass;
        }

        // Process "filter" node
        if (displayFilter)
        {
            // Filter
            ColumnFilter filter = column.Filter;
            if (filter != null)
            {
                object option = null;
                object value = null;

                // Filter via query string
                if (FilterByQueryString)
                {
                    if (String.IsNullOrEmpty(filter.Path))
                    {
                        string values = QueryHelper.GetString(column.Source, null);
                        if (!string.IsNullOrEmpty(values))
                        {
                            string[] pair = values.Split(';');
                            option = pair[0];
                            value = pair[1];
                        }
                    }
                    else
                    {
                        value = QueryHelper.GetString(column.Source, null);
                    }
                }

                // Add the filter field
                AddFilterField(filter, column.Source, columnCaption, filterTable, option, value);
            }
        }

        // Add custom column to gridview
        UniGridView.Columns.Add(field);

        // Store corresponding field
        column.Field = field;
    }


    /// <summary>
    /// Load query definition from XML.
    /// </summary>
    /// <param name="objectTypeNode">XML query definition node</param>
    private void LoadObjectTypeDefinition(XmlNode objectTypeNode)
    {
        if (objectTypeNode != null)
        {
            ObjectType = objectTypeNode.Attributes["name"].Value;

            // Set the columns property if columns are defined
            LoadColumns(objectTypeNode);
        }
    }


    /// <summary>
    /// Load query definition from XML.
    /// </summary>
    /// <param name="queryNode">XML query definition node</param>
    private void LoadQueryDefinition(XmlNode queryNode)
    {
        if (queryNode != null)
        {
            Query = queryNode.Attributes["name"].Value;

            // Set the columns property if columns are defined
            LoadColumns(queryNode);
            LoadAllColumns(queryNode);

            // Load the query parameters
            XmlNodeList parameters = queryNode.SelectNodes("parameter");
            if ((parameters != null) && (parameters.Count > 0))
            {
                QueryDataParameters newParams = new QueryDataParameters();

                // Process all parameters
                foreach (XmlNode param in parameters)
                {
                    object value = null;
                    string name = param.Attributes["name"].Value;

                    switch (param.Attributes["type"].Value.ToLowerCSafe())
                    {
                        case "string":
                            value = param.Attributes["value"].Value;
                            break;

                        case "int":
                            value = ValidationHelper.GetInteger(param.Attributes["value"].Value, 0);
                            break;

                        case "double":
                            value = Convert.ToDouble(param.Attributes["value"].Value);
                            break;

                        case "bool":
                            value = Convert.ToBoolean(param.Attributes["value"].Value);
                            break;
                    }

                    newParams.Add(name, value);
                }

                QueryParameters = newParams;
            }
        }
    }


    /// <summary>
    /// Sets the columns property if columns are defined.
    /// </summary>
    /// <param name="queryNode">Node from which to load columns</param>
    private void LoadAllColumns(XmlNode queryNode)
    {
        XmlAttribute allColumns = queryNode.Attributes["allcolumns"];
        if (allColumns != null)
        {
            AllColumns = DataHelper.GetNotEmpty(allColumns.Value, AllColumns);
        }
    }


    /// <summary>
    /// Sets the columns property if columns are defined.
    /// </summary>
    /// <param name="queryNode">Node from which to load columns</param>
    private void LoadColumns(XmlNode queryNode)
    {
        XmlAttribute columns = queryNode.Attributes["columns"];
        if (columns != null)
        {
            Columns = DataHelper.GetNotEmpty(columns.Value, Columns);
        }
    }


    /// <summary>
    /// Add filter field to the filter table.
    /// </summary>
    /// <param name="filter">Filter definition</param>
    /// <param name="columnSourceName">Column source field name</param>
    /// <param name="fieldDisplayName">Field display name</param>
    /// <param name="filterTable">Filter table</param>
    /// <param name="filterOption">Filter option</param>
    /// <param name="filterValue">Filter value</param>
    private void AddFilterField(ColumnFilter filter, string columnSourceName, string fieldDisplayName, Table filterTable, object filterOption, object filterValue)
    {
        string fieldType = filter.Type;
        string fieldPath = filter.Path;
        string filterFormat = filter.Format;
        string fieldSourceName = filter.Source ?? columnSourceName;
        int filterSize = filter.Size;
        Unit filterWidth = filter.Width;


        TableRow tRow = new TableRow();

        TableCell tCellName = new TableCell();
        TableCell tCellOption = new TableCell();
        TableCell tCellValue = new TableCell();

        // Ensure fieldSourceName is JavaScript valid
        fieldSourceName = fieldSourceName.Replace(ALL, "__ALL__");

        // Label
        Label textName = new Label
        {
            Text = String.IsNullOrEmpty(fieldDisplayName) ? "" : fieldDisplayName + ":",
            ID = fieldSourceName + "Name",
            EnableViewState = false
        };

        tCellName.Controls.Add(textName);
        tRow.Cells.Add(tCellName);

        // Filter option
        string option = null;
        if (filterOption != null)
        {
            option = ValidationHelper.GetString(filterOption, null);
        }

        // Filter value
        string value = null;
        if (filterValue != null)
        {
            value = ValidationHelper.GetString(filterValue, null);
        }

        // Filter definition
        UniGridFilterField filterDefinition = new UniGridFilterField();
        filterDefinition.Type = (fieldType != null) ? fieldType.ToLowerCSafe() : "custom";
        filterDefinition.Label = textName;
        filterDefinition.Format = filterFormat;
        filterDefinition.FilterRow = tRow;

        string customPath = null;

        // Set the filter default value
        string defaultValue = filter.DefaultValue;

        // Remember default values of filter field controls for potential UniGrid reset
        string optionFilterFieldValue = null;
        string valueFilterFieldValue = null;

        switch (filterDefinition.Type)
        {
            // Text filter
            case "text":
                {
                    DropDownList textOptionFilterField = new DropDownList();

                    textOptionFilterField.Items.Add(new ListItem("LIKE", "LIKE"));
                    textOptionFilterField.Items.Add(new ListItem("NOT LIKE", "NOT LIKE"));
                    textOptionFilterField.Items.Add(new ListItem("=", "="));
                    textOptionFilterField.Items.Add(new ListItem("<>", "<>"));
                    textOptionFilterField.CssClass = "ContentDropdown";
                    textOptionFilterField.ID = fieldSourceName;

                    // Set the value
                    SetDropdownValue(value, null, textOptionFilterField);
                    optionFilterFieldValue = textOptionFilterField.SelectedValue;

                    LocalizedLabel lblSelect = new LocalizedLabel
                    {
                        EnableViewState = false,
                        Display = false,
                        AssociatedControlID = textOptionFilterField.ID,
                        ResourceString = "general.select"
                    };

                    tCellOption.Controls.Add(lblSelect);
                    tCellOption.Controls.Add(textOptionFilterField);
                    tRow.Cells.Add(tCellOption);

                    // Add text field
                    TextBox textValueFilterField = new TextBox
                    {
                        ID = fieldSourceName + "TextValue",
                        CssClass = "FilterTextBox",
                    };

                    // Set value
                    SetTextboxValue(value, defaultValue, textValueFilterField);
                    valueFilterFieldValue = textValueFilterField.Text;

                    if (filterSize > 0)
                    {
                        textValueFilterField.MaxLength = filterSize;
                    }
                    if (!filterWidth.IsEmpty)
                    {
                        textValueFilterField.Width = filterWidth;
                    }
                    tCellValue.Controls.Add(textValueFilterField);
                    tRow.Cells.Add(tCellValue);
                    textName.AssociatedControlID = textValueFilterField.ID;

                    filterDefinition.OptionsControl = textOptionFilterField;
                    filterDefinition.ValueControl = textValueFilterField;
                }
                break;

            // Boolean filter
            case "bool":
                {
                    DropDownList booleanOptionFilterField = new DropDownList();

                    booleanOptionFilterField.Items.Add(new ListItem(GetString("general.selectall"), ""));
                    booleanOptionFilterField.Items.Add(new ListItem(GetString("general.yes"), "1"));
                    booleanOptionFilterField.Items.Add(new ListItem(GetString("general.no"), "0"));
                    booleanOptionFilterField.CssClass = "ContentDropdown";
                    booleanOptionFilterField.ID = fieldSourceName;
                    textName.AssociatedControlID = booleanOptionFilterField.ID;

                    // Set the value
                    SetDropdownValue(value, defaultValue, booleanOptionFilterField);
                    valueFilterFieldValue = booleanOptionFilterField.SelectedValue;

                    tCellValue.Controls.Add(booleanOptionFilterField);
                    tRow.Cells.Add(tCellValue);

                    filterDefinition.ValueControl = booleanOptionFilterField;
                }
                break;

            // Integer filter
            case "integer":
            case "double":
                {
                    DropDownList numberOptionFilterField = new DropDownList();
                    numberOptionFilterField.Items.Add(new ListItem("=", "="));
                    numberOptionFilterField.Items.Add(new ListItem("<>", "<>"));
                    numberOptionFilterField.Items.Add(new ListItem("<", "<"));
                    numberOptionFilterField.Items.Add(new ListItem(">", ">"));
                    numberOptionFilterField.CssClass = "ContentDropdown";
                    numberOptionFilterField.ID = fieldSourceName;

                    // Set the value
                    SetDropdownValue(value, null, numberOptionFilterField);
                    optionFilterFieldValue = numberOptionFilterField.SelectedValue;

                    LocalizedLabel lblSelect = new LocalizedLabel
                    {
                        EnableViewState = false,
                        Display = false,
                        AssociatedControlID = numberOptionFilterField.ID,
                        ResourceString = "general.select"
                    };

                    // Add filter field
                    tCellOption.Controls.Add(lblSelect);
                    tCellOption.Controls.Add(numberOptionFilterField);
                    tRow.Cells.Add(tCellOption);

                    TextBox numberValueFilterField = new TextBox
                    {
                        ID = fieldSourceName + "NumberValue",
                    };

                    // Set value
                    SetTextboxValue(value, defaultValue, numberValueFilterField);
                    valueFilterFieldValue = numberValueFilterField.Text;

                    if (filterSize > 0)
                    {
                        numberValueFilterField.MaxLength = filterSize;
                    }
                    if (!filterWidth.IsEmpty)
                    {
                        numberValueFilterField.Width = filterWidth;
                    }
                    numberValueFilterField.EnableViewState = false;

                    tCellValue.Controls.Add(numberValueFilterField);
                    tRow.Cells.Add(tCellValue);

                    filterDefinition.OptionsControl = numberOptionFilterField;
                    filterDefinition.ValueControl = numberValueFilterField;
                }
                break;

            case "site":
                {
                    // Site selector
                    customPath = "~/CMSFormControls/Filters/SiteFilter.ascx";
                }
                break;

            case "custom":
                // Load custom path
                {
                    if (String.IsNullOrEmpty(fieldPath))
                    {
                        throw new Exception("[UniGrid.AddFilterField]: Filter field path is not set");
                    }

                    customPath = fieldPath;
                }
                break;

            default:
                // Not supported filter type
                throw new Exception("[UniGrid.AddFilterField]: Filter type '" + filterDefinition.Type + "' is not supported. Supported filter types: integer, double, bool, text, site, custom.");
        }

        // Else if filter path is defined use custom filter
        if (customPath != null)
        {
            customPath = (customPath.StartsWithCSafe("~/") ? customPath : FilterDirectoryPath + customPath.TrimStart('/'));

            // Add to the controls collection
            CMSAbstractBaseFilterControl filterControl = LoadFilterControl(fieldPath, fieldSourceName, value, filterDefinition, customPath);
            if (filterControl != null)
            {
                // Set default value
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    filterControl.SelectedValue = defaultValue;
                }

                tCellValue.Controls.Add(filterControl);
            }

            tCellValue.Attributes["colspan"] = "2";
            tRow.Cells.Add(tCellValue);
        }

        RaiseOnFilterFieldCreated(fieldSourceName, filterDefinition);
        FilterFields[fieldSourceName] = filterDefinition;

        filterTable.Rows.Add(tRow);

        // Store initial filter state for potential UniGrid reset
        if (filterDefinition.OptionsControl != null)
        {
            InitialFilterStateControls.Add(new KeyValuePair<Control, object>(filterDefinition.OptionsControl, optionFilterFieldValue));
        }
        if (filterDefinition.ValueControl != null)
        {
            if (!(filterDefinition.ValueControl is CMSAbstractBaseFilterControl))
            {
                InitialFilterStateControls.Add(new KeyValuePair<Control, object>(filterDefinition.ValueControl, valueFilterFieldValue));
            }
        }
    }


    /// <summary>
    /// Sets the textbox value to the given value or default value if available
    /// </summary>
    /// <param name="value">Value to set</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="textBox">TextBox to set</param>
    private static void SetTextboxValue(string value, string defaultValue, TextBox textBox)
    {
        // Set default value
        if (!String.IsNullOrEmpty(defaultValue))
        {
            textBox.Text = defaultValue;
        }
        else
        {
            textBox.Text = value;
        }
    }


    /// <summary>
    /// Sets the DropDown value to the given value or default value if available
    /// </summary>
    /// <param name="value">Value to set</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="dropDown">DropDown to set</param>
    private static void SetDropdownValue(string value, string defaultValue, DropDownList dropDown)
    {
        // Select filter option
        try
        {
            if (!String.IsNullOrEmpty(defaultValue))
            {
                dropDown.SelectedValue = defaultValue;
            }
            else
            {
                dropDown.SelectedValue = value;
            }
        }
        catch
        {
        }
    }


    /// <summary>
    /// Loads the filter control
    /// </summary>
    /// <param name="fieldPath">Field path</param>
    /// <param name="fieldSourceName">Field source name</param>
    /// <param name="value">Field value</param>
    /// <param name="filterDefinition">Filter definition</param>
    /// <param name="path">Control path</param>
    private CMSAbstractBaseFilterControl LoadFilterControl(string fieldPath, string fieldSourceName, string value, UniGridFilterField filterDefinition, string path)
    {
        // Load the filter control
        CMSAbstractBaseFilterControl filterControl = this.LoadUserControl(path) as CMSAbstractBaseFilterControl;
        if (filterControl != null)
        {
            // Setup the filter control
            filterControl.ID = fieldSourceName;
            filterControl.FilteredControl = this;

            if (!RequestHelper.IsPostBack())
            {
                filterControl.Value = value;
            }

            filterDefinition.ValueControl = filterControl;
            filterDefinition.ControlPath = fieldPath;
        }

        return filterControl;
    }


    /// <summary>
    /// Creates filter show button.
    /// </summary>
    private void CreateFilterButton(Table filterTable)
    {
        if (!HideFilterButton && String.IsNullOrEmpty(FilterFormName))
        {
            // Add button to the bottom of the filter table
            showButton = new CMSButton();
            Literal ltlBreak = new Literal();
            TableRow tRow = new TableRow();
            TableCell tCell = new TableCell();
            showButton.ID = "btnShow";
            showButton.Text = GetString("general.show");
            showButton.CssClass = "ContentButton";
            showButton.Click += ShowButton_Click;
            showButton.EnableViewState = false;
            tCell.Controls.Add(showButton);

            // If we remember the grid state we need to add the reset button to allow the user to restore the initial state
            if (RememberState)
            {
                LinkButton resetButton = new LinkButton
                {
                    ID = "btnReset",
                    Text = GetString("general.reset"),
                    EnableViewState = false,
                };
                resetButton.Style.Add("line-height", "2em");
                resetButton.Style.Add("margin", "1em");
                resetButton.Click += ResetButton_Click;

                tCell.Controls.Add(resetButton);
            }

            ltlBreak.EnableViewState = false;
            ltlBreak.Text = "<br /><br />";
            tCell.Controls.Add(ltlBreak);
            tCell.ColumnSpan = 2;
            tCell.EnableViewState = false;
            TableCell indentCell = new TableCell
                                       {
                                           EnableViewState = false,
                                           Text = "&nbsp;"
                                       };
            tRow.Cells.Add(indentCell); // Indent 'Show' button
            tRow.Cells.Add(tCell);
            filterTable.Rows.Add(tRow);
            pnlHeader.DefaultButton = showButton.ID;
        }
    }


    /// <summary>
    /// Sets filter to the grid view and save it to the view state.
    /// </summary>
    /// <param name="reloadData">Reload data</param>
    protected override void SetFilter(bool reloadData, string where)
    {
        // Where can be empty string - it means that filter condition was added to WhereCondition property 
        if (where == null)
        {
            where = GetFilter();
        }

        // Filter by query string
        if (FilterByQueryString && !reloadData)
        {
            string url = URLRewriter.CurrentURL;
            foreach (string name in queryStringHashTable.Keys)
            {
                if (queryStringHashTable[name] != null)
                {
                    string value = HttpContext.Current.Server.UrlEncode(queryStringHashTable[name].ToString());
                    url = URLHelper.AddParameterToUrl(url, name, value);
                }
            }
            URLHelper.Redirect(url);
        }
        else
        {
            WhereClause = where;
            if (!String.IsNullOrEmpty(where))
            {
                FilterIsSet = true;
            }
            if ((!DelayedReload) && (reloadData))
            {
                // Get data from database and set them to the grid view
                ReloadData();
            }
        }
    }


    /// <summary>
    /// Sets filter visibility depending on the UniGrid's configuration and number of objects.
    /// </summary>
    private void CheckFilterVisibility()
    {
        if (displayFilter)
        {
            if (FilterLimit > 0)
            {
                if (!String.IsNullOrEmpty(FilterFormName))
                {
                    plcFilterForm.Visible = FilterIsSet || ShowFilter;
                }
                else
                {
                    filter.Visible = FilterIsSet || ShowFilter;
                }
            }
        }
        else
        {
            filter.Visible = false;
            plcFilterForm.Visible = false;
        }
    }


    /// <summary>
    /// Sorts UniGrid data source according to sort directive saved in viewstate.
    /// </summary>
    private void SortUniGridDataSource(object ds)
    {
        if (SortDirect != "")
        {
            // If source isn't empty
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Set sort directive from viewstate
                if (ds is DataTable)
                {
                    // Data table
                    try
                    {
                        ((DataTable)(ds)).DefaultView.Sort = SortDirect;
                    }
                    catch
                    {
                    }
                }
                else if (ds is DataSet)
                {
                    // DataSet
                    try
                    {
                        ((DataSet)(ds)).Tables[0].DefaultView.Sort = SortDirect;
                        ds = ((DataSet)(ds)).Tables[0].DefaultView;
                    }
                    catch
                    {
                    }
                }
                else if (ds is DataView)
                {
                    // Data view
                    try
                    {
                        ((DataView)(ds)).Sort = SortDirect;
                    }
                    catch
                    {
                    }
                }
            }
        }
    }


    /// <summary>
    /// Changes sorting direction by specified column.
    /// </summary>
    /// <param name="orderByColumn">Column name to order by</param>
    /// <param name="orderByString">Old order by string</param> 
    private void ChangeSortDirection(string orderByColumn, string orderByString)
    {
        orderByColumn = orderByColumn.Trim().TrimStart('[').TrimEnd(']').Trim();
        orderByString = orderByString.Trim().TrimStart('[');

        // If order by column is long text use CAST in ORDER BY part of query
        if (TextColumns.Contains(orderByColumn))
        {
            if (orderByString.EndsWithCSafe("desc"))
            {
                SortDirect = String.Format("CAST([{0}] AS nvarchar(32)) asc", orderByColumn);
            }
            else
            {
                SortDirect = String.Format("CAST([{0}] AS nvarchar(32)) desc", orderByColumn);
            }
        }
        else
        {
            string orderByDirection = "asc";
            Match orderByMatch = OrderByRegex.Match(orderByString);
            if (orderByMatch.Success)
            {
                if (orderByMatch.Groups[2].Success)
                {
                    orderByDirection = orderByMatch.Groups[2].Value;
                }
            }

            // Sort by the same column -> the other directon
            if (orderByString.StartsWithCSafe(orderByColumn))
            {
                SortDirect = (orderByDirection == "desc") ? String.Format("[{0}] asc", orderByColumn) : String.Format("[{0}] desc", orderByColumn);
            }
            // Sort by a new column -> implicitly direction is ASC
            else
            {
                SortDirect = String.Format("[{0}] asc", orderByColumn);
            }
        }
    }


    /// <summary>
    /// Returns List of selected Items.
    /// </summary>
    private List<string> GetSelectedItems()
    {
        return GetHiddenValues(SelectionHiddenField);
    }


    /// <summary>
    /// Sets selection values for UniGrid.
    /// </summary>
    /// <param name="values">Values to set</param>
    private void SetSectedItems(IEnumerable<string> values)
    {
        SetHiddenValues(values, SelectionHiddenField, null);
    }


    /// <summary>
    /// Returns List of deselected Items.
    /// </summary>
    private List<string> GetDeselectedItems()
    {
        return GetHiddenValues(hidDeSelection);
    }


    /// <summary>
    /// Returns List of newly selected Items.
    /// </summary>
    private List<string> GetNewlySelectedItems()
    {
        return GetHiddenValues(hidNewSelection);
    }


    /// <summary>
    /// Returns array list from hidden field.
    /// </summary>
    /// <param name="field">Hidden field with values separated with |</param>
    private static List<string> GetHiddenValues(HiddenField field)
    {
        string hiddenValue = field.Value.Trim('|');
        var list = new List<string>();
        string[] values = hiddenValue.Split('|');
        foreach (string value in values)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }
        list.Remove("");
        return list;
    }


    /// <summary>
    /// Sets values into hidden field.
    /// </summary>
    /// <param name="values">Values to set</param>
    /// <param name="actionsField">Hidden field</param>
    private static void SetHiddenValues(IEnumerable<string> values, HiddenField actionsField, HiddenField hashField)
    {
        if (values != null)
        {
            if (actionsField != null)
            {
                // Build the list of actions
                StringBuilder sb = new StringBuilder();
                sb.Append("|");

                foreach (string value in values)
                {
                    sb.Append(value);
                    sb.Append("|");
                }

                // Action IDs
                string actions = sb.ToString();
                actionsField.Value = actions;

                // Actions hash
                if (hashField != null)
                {
                    hashField.Value = ValidationHelper.GetHashString(actions);
                }
            }
        }
    }


    /// <summary>
    /// Clears all selected items from hidden values.
    /// </summary>
    /// <param name="field">Hidden field</param>
    private static void ClearHiddenValues(HiddenField field)
    {
        if (field != null)
        {
            field.Value = "";
        }
    }


    /// <summary>
    /// Sets hidden field with actions hashes.
    /// </summary>
    private void SetActionsHash()
    {
        if (ActionsID.Count > 0)
        {
            SetHiddenValues(ActionsID, hidActions, hidActionsHash);
        }
    }


    /// <summary>
    /// Sets pager control.
    /// </summary>
    private void SetPager()
    {
        Pager.PagedControl = this;
    }


    /// <summary>
    /// Sets the sort direction if current request is sorting.
    /// </summary>
    private void ProcessSorting()
    {
        // Get current event target
        string uniqieId = ValidationHelper.GetString(Request.Params["__EVENTTARGET"], String.Empty);
        // Get current argument
        string eventargument = ValidationHelper.GetString(Request.Params["__EVENTARGUMENT"], String.Empty);

        if ((uniqieId == GridView.UniqueID) && (eventargument.StartsWithCSafe("Sort")))
        {
            string orderByColumn = Convert.ToString(eventargument.Split('$')[1]);
            if (SortColumns.Contains(orderByColumn.ToLowerCSafe()))
            {
                // If sorting is called for the first time and default sorting (OrderBy property) is set
                if ((SortDirect == "") && !string.IsNullOrEmpty(OrderBy))
                {
                    ChangeSortDirection(orderByColumn, OrderBy);
                }
                else
                {
                    ChangeSortDirection(orderByColumn, SortDirect);
                }
            }
        }
    }


    /// <summary>
    /// Returns true if current request was fired by page change or filter show button.
    /// </summary>
    private bool EventRequest()
    {
        if (URLHelper.IsPostback())
        {
            // Get current event target
            string uniqieId = ValidationHelper.GetString(Request.Params["__EVENTTARGET"], String.Empty);
            // Get current argument
            string eventargument = ValidationHelper.GetString(Request.Params["__EVENTARGUMENT"], String.Empty).ToLowerCSafe();

            // Check whether current request is paging
            if (!String.IsNullOrEmpty(uniqieId) && (uniqieId == GridView.UniqueID) && eventargument.StartsWithCSafe("page"))
            {
                return true;
            }

            // Check whether show button is defined
            if (showButton != null)
            {
                // If button name is not empty => button fire postback
                if (!string.IsNullOrEmpty(Request.Params[showButton.UniqueID]))
                {
                    return true;
                }
            }

            // Check whether show button in basic form is defined
            if (FilterForm.SubmitButton != null)
            {
                // If submit button name is not empty => button fire postback
                if (!string.IsNullOrEmpty(Request.Params[FilterForm.SubmitButton.UniqueID]))
                {
                    return true;
                }
            }
        }

        // Non-paging request by default
        return false;
    }


    /// <summary>
    /// Returns icon file for current theme or from default if current doesn't exist.
    /// </summary>
    /// <param name="iconfile">Icon file name</param>
    private string GetActionImage(string iconfile)
    {
        if (File.Exists(MapPath(ImageDirectoryPath + iconfile)))
        {
            return (ImageDirectoryPath + iconfile);
        }

        // Short path to the icon
        if (ControlsExtensions.RenderShortIDs)
        {
            return UIHelper.GetShortImageUrl(UIHelper.UNIGRID_ICONS, iconfile);
        }

        return GetImageUrl("Design/Controls/UniGrid/Actions/" + iconfile);
    }


    /// <summary>
    /// Register unigrid commands scripts.
    /// </summary>
    private void RegisterCmdScripts()
    {
        StringBuilder builder = new StringBuilder();

        // Redir function
        if (EditActionUrl != null)
        {
            builder.Append("function UG_Redir(url) { document.location.replace(url); return false; }\n");
        }

        builder.Append("function ", RELOAD_PREFIX, ClientID, "() { ", Page.ClientScript.GetPostBackEventReference(this, "Reload"), " }\n");
        builder.Append("function UG_Reload() { ", RELOAD_PREFIX, ClientID, "(); }\n");

        // Actions
        builder.Append(
            @"
function Get(id) {
    return document.getElementById(id);
}

function ", CMD_PREFIX, ClientID, @"(name, arg) {
    var nameObj = Get('", hidCmdName.ClientID, @"');
    var argObj = Get('", hidCmdArg.ClientID, @"');
    if ((nameObj != null) && (argObj != null)) {
        nameObj.value = name;
        argObj.value = arg;
        ", Page.ClientScript.GetPostBackEventReference(this, "UniGridAction"), @"
    } 
    
    return false;
}

function ", DESTROY_OBJECT_PREFIX, ClientID, @"(arg) {"
            , CMD_PREFIX, ClientID, @"('#destroyobject',arg);
}");


        if (showSelection)
        {
            // Selection - click
            builder.Append(
                "function ", SELECT_PREFIX, ClientID, @"(checkBox, arg) {
    if (checkBox == null) return;
        
    var sel = Get('", GetSelectionFieldClientID(), @"');
    var newSel = Get('", hidNewSelection.ClientID, @"');
    var deSel = Get('", hidDeSelection.ClientID, @"');

    if ((sel == null) || (newSel == null) || (deSel == null)) return;
        
    if (newSel.value == '') {
        newSel.value = '|';
    }
    if (deSel.value == '') {
        deSel.value = '|'
    }
    if (sel.value == '') {
        sel.value = '|'
    }
    if (checkBox.checked) {
        sel.value += arg + '|'
        if (deSel.value.indexOf('|' + arg + '|') >= 0) {
            deSel.value = deSel.value.replace('|' + arg + '|', '|')
        }
        else {
            newSel.value += arg + '|'
        }
    }
    else {
        sel.value = sel.value.replace('|' + arg + '|', '|')
        if (newSel.value.indexOf('|' + arg + '|') >= 0) {
            newSel.value = newSel.value.replace('|' + arg + '|', '|')
        }
        else {
            deSel.value += arg + '|'
        }
    }
}
");

            // Selection - select all
            builder.Append(
                "function ", SELECT_ALL_PREFIX, ClientID, @"(chkBox) {
    var elems = document.getElementsByTagName('INPUT');
    var re = new RegExp('", ClientID, @"');
    for(i=0; i<elems.length; i++) {
        if(elems[i].type == 'checkbox') {
            if(elems[i].id.match(re)) {
                if((!elems[i].id != chkBox.id) && (chkBox.checked != elems[i].checked)) {
                    elems[i].click();
                }
            }
        }
    }
}
");

            // Selection - clear
            builder.Append(
                "function ", CLEAR_SELECTION_PREFIX, ClientID, @"() {
     var inp = document.getElementsByTagName('input');
     if (inp != null) {
         for (var i = 0; i< inp.length; i++) {
             if ((inp[i].type.toLowerCase() == 'checkbox') && (inp[i].id.match(/^", ClientID, @"/i))) {
                 inp[i].checked = false;
             }
         }
     }

     var sel = Get('", GetSelectionFieldClientID(), @"');
     var newSel = Get('", hidNewSelection.ClientID, @"');
     var deSel = Get('", hidDeSelection.ClientID, @"');
     if (sel != null) {
         sel.value = '';
     }
     if (newSel != null) {
         newSel.value = '';
     }
     if (deSel != null) {
         deSel.value = '';
     }
}
");

            // Selection - IsSelectionEmpty
            builder.Append(
                "function ", CHECK_SELECTION_PREFIX, ClientID, @"() {
    var sel = Get('", GetSelectionFieldClientID(), @"');
    var items = sel.value;
    return !(items != '' && items != '|');
}
");

            if (resetSelection)
            {
                builder.Append("if (", CLEAR_SELECTION_PREFIX, ClientID, ") { ", CLEAR_SELECTION_PREFIX, ClientID, "(); }");
            }
        }

        ScriptHelper.RegisterStartupScript(this, typeof(string), "UniGrid_" + ClientID, ScriptHelper.GetScript(builder.ToString()));
    }


    /// <summary>
    /// Sets basic form filter.
    /// </summary>
    private void SetBasicFormFilter()
    {
        // Get alternative form layout if defined
        AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(FilterFormName);
        if (afi != null)
        {
            // Get form info
            FormInfo fi = FormHelper.GetFormInfo(FilterFormName, true);
            if (fi != null)
            {
                FilterForm.OnAfterSave += BasicForm_OnAfterSave;
                // Set form info
                FilterForm.FormInformation = fi;

                // Set filter button
                FilterForm.SubmitButton.ID = "btnShow";
                FilterForm.SubmitButton.Text = GetString("general.show");
                FilterForm.SubmitButton.CssClass = "ContentButton";
                FilterForm.SubmitButton.RegisterHeaderAction = false;

                // Set layout
                if (!string.IsNullOrEmpty(afi.FormLayout))
                {
                    FilterForm.FormLayout = afi.FormLayout;
                }
                else
                {
                    // Indent filter
                    Literal ltlBreak = new Literal();
                    ltlBreak.EnableViewState = false;
                    ltlBreak.Text = "<br />";
                    plcFilterForm.Controls.Add(ltlBreak);
                }

                FilterForm.CheckFieldEmptiness = false;
                FilterForm.LoadData(FilterFormData);
            }
        }
    }


    /// <summary>
    /// Handles OnAfterSave event of basic form.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event argument</param>
    private void BasicForm_OnAfterSave(object sender, EventArgs e)
    {
        // Set where clause
        WhereClause = FilterForm.GetWhereCondition();
        FilterIsSet = !string.IsNullOrEmpty(WhereClause);
        Pager.UniPager.CurrentPage = 1;
        ReloadData();
    }


    /// <summary>
    /// Checks whether user is authorized for specified action.
    /// </summary>
    /// <param name="actionName">Action name</param>    
    private void CheckActionAndRedirect(string actionName)
    {
        // Get the action
        Action action = GridActions.GetAction(actionName);

        if ((action != null) && (!string.IsNullOrEmpty(action.ModuleName)))
        {
            CurrentUserInfo user = CMSContext.CurrentUser;
            string siteName = CMSContext.CurrentSiteName;

            // Check module permissions
            if (!string.IsNullOrEmpty(action.Permissions) && !user.IsAuthorizedPerResource(action.ModuleName, action.Permissions, siteName))
            {
                RedirectToAccessDenied(action.ModuleName, action.Permissions);
            }

            // Check module UI elements
            if (!string.IsNullOrEmpty(action.UIElements) && !user.IsAuthorizedPerUIElement(action.ModuleName, action.UIElements.Split(';'), siteName))
            {
                RedirectToUIElementAccessDenied(action.ModuleName, action.UIElements);
            }
        }
    }

    #endregion


    #region "IUniPageable Members"

    /// <summary>
    /// Pager data item.
    /// </summary>
    public object PagerDataItem
    {
        get
        {
            return UniGridView.DataSource;
        }
        set
        {
            UniGridView.DataSource = value;
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
    public virtual void ReBind()
    {
        if (OnPageChanged != null)
        {
            OnPageChanged(this, null);
        }

        ReloadData();
    }

    #endregion
}