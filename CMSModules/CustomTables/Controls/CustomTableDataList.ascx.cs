using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;
using CMS.ExtendedControls;

using Action = CMS.UIControls.UniGridConfig.Action;

public partial class CMSModules_CustomTables_Controls_CustomTableDataList : CMSUserControl
{
    #region "Private & protected variables"

    protected string editToolTip = String.Empty;
    protected string deleteToolTip = String.Empty;
    protected string viewToolTip = String.Empty;
    protected string upToolTip = String.Empty;
    protected string downToolTip = String.Empty;

    // Default pages
    private string mEditItemPage = "~/CMSModules/CustomTables/Tools/CustomTable_Data_EditItem.aspx";
    private string mViewItemPage = "~/CMSModules/CustomTables/Tools/CustomTable_Data_ViewItem.aspx";
    private string mEditItemPageAdditionalParams = null;
    private string mViewItemPageAdditionalParams = null;

    protected DataSet ds = null;
    private DataClassInfo mCustomTableClassInfo = null;
    private FormInfo mFormInfo = null;
    private TypeInfo ti = null;
        
    private readonly CustomTableItemProvider ctProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

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
    /// Gets or sets URL of the page where item is edited.
    /// </summary>
    public string EditItemPage
    {
        get
        {
            return mEditItemPage;
        }
        set
        {
            mEditItemPage = value;
        }
    }


    /// <summary>
    /// Gets or sets additional parameters for Edit page.
    /// </summary>
    public string EditItemPageAdditionalParams
    {
        get
        {
            return mEditItemPageAdditionalParams;
        }
        set
        {
            mEditItemPageAdditionalParams = value;
        }
    }


    /// <summary>
    /// Gets or sets URL of the page where whole item is displayed.
    /// </summary>
    public string ViewItemPage
    {
        get
        {
            return mViewItemPage;
        }
        set
        {
            mViewItemPage = value;
        }
    }


    /// <summary>
    /// Gets or sets additional parameters for View page.
    /// </summary>
    public string ViewItemPageAdditionalParams
    {
        get
        {
            return mViewItemPageAdditionalParams;
        }
        set
        {
            mViewItemPageAdditionalParams = value;
        }
    }


    /// <summary>
    /// Gets or sets the class info of custom table which data are displayed.
    /// </summary>
    public DataClassInfo CustomTableClassInfo
    {
        get
        {
            return mCustomTableClassInfo;
        }
        set
        {
            mCustomTableClassInfo = value;
        }
    }


    /// <summary>
    /// Gets or sets the form info.
    /// </summary>
    public FormInfo FormInfo
    {
        get
        {
            if (mFormInfo == null)
            {
                if (CustomTableClassInfo != null)
                {
                    mFormInfo = FormHelper.GetFormInfo(CustomTableClassInfo.ClassName, true);
                }
            }
            return mFormInfo;
        }
    }


    /// <summary>
    /// Determines whether custom table has ItemOrder field.
    /// </summary>
    public bool HasItemOrderField
    {
        get
        {
            if (FormInfo != null)
            {
                return (FormInfo.FieldExists("ItemOrder") && FormInfo.FieldExists("ItemID"));
            }
            else
            {
                // If form info is not available assume ItemOrder is not present to prevent further exceptions
                return false;
            }
        }
    }


    /// <summary>
    /// Gets custom table data unigrid.
    /// </summary>
    public UniGrid UniGrid
    {
        get
        {
            return gridData;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return gridData.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            gridData.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Register Javascripts
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DeleteEditView", ScriptHelper.GetScript(
            "var deleteConfirmation = ''; " +
            "function DeleteConfirm() { return confirm(deleteConfirmation); } " +
            "function EditItem(customtableid, itemId) { " +
            "  document.location.replace('" + ResolveUrl(EditItemPage) + "?" +
            (String.IsNullOrEmpty(mEditItemPageAdditionalParams) ? String.Empty : mEditItemPageAdditionalParams + "&") + "customtableid=' + customtableid + '&itemId=' + itemId); } " +
            "function ViewItem(customtableid, itemId) { " +
            "  modalDialog('" + ResolveUrl(ViewItemPage) + "?" +
            (String.IsNullOrEmpty(mViewItemPageAdditionalParams) ? String.Empty : mViewItemPageAdditionalParams + "&") + "customtableid=' + customtableid + '&itemId=' + itemId,'ViewItem',600,600); } "));

        // Buttons' tooltips
        editToolTip = GetString("general.edit");
        deleteToolTip = GetString("general.delete");
        viewToolTip = GetString("general.view");
        upToolTip = GetString("general.up");
        downToolTip = GetString("general.down");

        // Delete confirmation
        ltlScript.Text = ScriptHelper.GetScript("deleteConfirmation = '" + GetString("customtable.data.DeleteConfirmation") + "';");

        gridData.ObjectType = CustomTableItemProvider.GetObjectType(CustomTableClassInfo.ClassName);
        gridData.OnLoadColumns += gridData_OnLoadColumns;
        gridData.OnExternalDataBound += gridData_OnExternalDataBound;
        gridData.OnAction += gridData_OnAction;

        ti = CustomTableItemProvider.GetTypeInfo(CustomTableClassInfo.ClassName);

        if (HasItemOrderField)
        {
            gridData.OrderBy = "ItemOrder ASC";
        }
        else
        {
            gridData.OrderBy = ti.IDColumn;
        }
    }

    #endregion


    #region "Grid events"

    protected void gridData_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            // Delete item action
            case "delete":
                if (CheckPermissions("Delete"))
                {
                    if (CustomTableClassInfo != null)
                    {
                        CustomTableItem item = ctProvider.GetItem(ValidationHelper.GetInteger(actionArgument, 0), CustomTableClassInfo.ClassName);
                        if (item != null)
                        {
                            item.Delete();
                        }
                    }

                    URLHelper.Redirect(URLHelper.Url.ToString());
                }
                break;

            // Move item up action
            case "moveup":
                if (CheckPermissions("Modify"))
                {
                    if (CustomTableClassInfo != null)
                    {
                        ctProvider.MoveItemUp(ValidationHelper.GetInteger(actionArgument, 0), CustomTableClassInfo.ClassName);
                    }

                    URLHelper.Redirect(URLHelper.Url.ToString());
                }
                break;

            // Move item down action
            case "movedown":
                if (CheckPermissions("Modify"))
                {
                    if (CustomTableClassInfo != null)
                    {
                        ctProvider.MoveItemDown(ValidationHelper.GetInteger(actionArgument, 0), CustomTableClassInfo.ClassName);
                    }

                    URLHelper.Redirect(URLHelper.Url.ToString());
                }
                break;
        }
    }


    protected object gridData_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        string source = sourceName.ToLowerCSafe();
        // Get button and grid view row
        ImageButton button = sender as ImageButton;
        GridViewRow grv = parameter as GridViewRow;

        if (grv != null)
        {
            DataRowView drv = grv.DataItem as DataRowView;

            // Hide Move Up/Down buttons when there is no Order field
            switch (source)
            {
                case "edit":
                    if ((button != null) && (drv != null))
                    {
                        // Add edit script
                        button.OnClientClick = "EditItem(" + CustomTableClassInfo.ClassID + ", " + drv[ti.IDColumn] + "); return false;";
                    }
                    break;

                case "view":
                    if ((button != null) && (drv != null))
                    {
                        // Add view script
                        button.OnClientClick = "ViewItem(" + CustomTableClassInfo.ClassID + ", " + drv[ti.IDColumn] + "); return false;";
                    }
                    break;

                case "moveup":
                case "movedown":
                    if (!HasItemOrderField && (button != null))
                    {
                        // Hide button
                        button.Visible = false;
                    }
                    break;
            }
        }
        else
        {
            switch (source)
            {
                case "itemcreatedby":
                case "itemmodifiedby":
                    int userId = ValidationHelper.GetInteger(parameter, 0);
                    return HTMLHelper.HTMLEncode(GetUserName(userId));

                default:
                    return HTMLHelper.HTMLEncode(parameter.ToString());
            }
        }

        return parameter;
    }


    private string GetUserName(int userId)
    {
        string userName = null;

        if (userId != 0)
        {
            string key = "UserInfo_" + userId;
            // Get user name from request cache
            userName = RequestStockHelper.GetItem(key) as string;
            if (userName == null)
            {
                // Get user information
                DataSet users = UserInfoProvider.GetUsers("UserID=" + userId, null, 1, "UserName, FullName");
                if (!DataHelper.DataSourceIsEmpty(users))
                {
                    DataRow dr = users.Tables[0].Rows[0];
                    string usrName = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, "UserName"), null);
                    string usrFullName = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, "FullName"), null);
                    userName = Functions.GetFormattedUserName(usrName, usrFullName, IsLiveSite);
                    // Store to request cache
                    RequestStockHelper.Add(key, userName);
                }
            }
        }

        return userName;
    }


    protected void gridData_OnLoadColumns()
    {
        if (CustomTableClassInfo != null)
        {
            // Update the actions command argument
            foreach (Action action in gridData.GridActions.Actions)
            {
                action.CommandArgument = ti.IDColumn;
            }

            string columnNames = null;
            List<string> columnList = null;

            string columns = CustomTableClassInfo.ClassShowColumns;
            if (string.IsNullOrEmpty(columns))
            {
                columnList = new List<string>();
                columnList.AddRange(GetExistingColumns(false).Take(5));
                
                // Show info message
                ShowInformation(GetString("customtable.columnscount.default"));
            }
            else
            {
                // Get existing columns names
                List<string> existingColumns = GetExistingColumns(true);

                // Get selected columns
                List<string> selectedColumns = GetSelectedColumns(columns);

                columnList = new List<string>();
                StringBuilder sb = new StringBuilder();

                // Remove non-existing columns
                foreach (string col in selectedColumns)
                {
                    int index = existingColumns.BinarySearch(col, StringComparer.InvariantCultureIgnoreCase);
                    if (index >= 0)
                    {
                        string colName = existingColumns[index];
                        columnList.Add(colName);
                        sb.Append(",[", colName, "]");
                    }
                }

                // Ensure item order column
                selectedColumns.Sort();
                if ((selectedColumns.BinarySearch("ItemOrder", StringComparer.InvariantCultureIgnoreCase) < 0) && HasItemOrderField)
                {
                    sb.Append(",[ItemOrder]");
                }

                // Ensure itemid column
                if (selectedColumns.BinarySearch(ti.IDColumn, StringComparer.InvariantCultureIgnoreCase) < 0)
                {
                    sb.Insert(0, ",[" + ti.IDColumn + "]");
                }

                columnNames = sb.ToString().TrimStart(',');
            }

            // Loop trough all columns
            for (int i = 0; i < columnList.Count; i++)
            {
                string column = columnList[i];

                // Get field caption
                FormFieldInfo ffi = FormInfo.GetFormField(column);
                string fieldCaption = string.Empty;
                if (ffi == null)
                {
                    fieldCaption = column;
                }
                else
                {
                    fieldCaption = (ffi.Caption == string.Empty) ? column : ResHelper.LocalizeString(ffi.Caption);
                }

                Column columnDefinition = new Column
                {
                    Caption = fieldCaption,
                    Source = column,
                    ExternalSourceName = column,
                    AllowSorting = true,
                    Wrap = false
                };

                if (i == columnList.Count - 1)
                {
                    // Stretch last column
                    columnDefinition.Width = "100%";
                }

                gridData.GridColumns.Columns.Add(columnDefinition);
            }

            // Set column names
            gridData.Columns = columnNames;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Checks the specified permission.
    /// </summary>
    private bool CheckPermissions(string permissionName)
    {
        // Check 'Modify' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.customtables", permissionName) &&
            !CMSContext.CurrentUser.IsAuthorizedPerClassName(CustomTableClassInfo.ClassName, permissionName))
        {
            ShowError(String.Format(GetString("customtable.permissiondenied." + permissionName), CustomTableClassInfo.ClassName));
            return false;
        }
        return true;
    }


    /// <summary>
    /// Gets existing columns from form info
    /// </summary>
    /// <param name="sort">Indicates if the columns should be sorted</param>
    private List<string> GetExistingColumns(bool sort)
    {
        var existingColumns = FormInfo.GetColumnNames();
        if (sort)
        {
            existingColumns.Sort(StringComparer.InvariantCultureIgnoreCase);
        }

        return existingColumns;
    }


    /// <summary>
    /// Gets list of selected columns
    /// </summary>
    private static List<string> GetSelectedColumns(string columns)
    {
        return columns.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    #endregion
}