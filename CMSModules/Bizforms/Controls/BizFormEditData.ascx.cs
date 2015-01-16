using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;

public partial class CMSModules_BizForms_Controls_BizFormEditData : CMSAdminEditControl
{
    #region "Variables"

    protected int formId = 0;
    protected BizFormInfo bfi = null;
    protected DataClassInfo dci = null;
    protected FormInfo mFormInfo = null;
    protected bool mShowNewRecordButton = true;
    protected bool mShowSelectFieldsButton = true;
    protected string className = null;
    protected string primaryColumn = null;
    protected string columnNames = null;
    private bool isDialog = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the form info.
    /// </summary>
    private FormInfo FormInfo
    {
        get
        {
            if (mFormInfo == null)
            {
                if (dci != null)
                {
                    mFormInfo = FormHelper.GetFormInfo(dci.ClassName, false);
                }
            }
            return mFormInfo;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether New Record button should be visible.
    /// </summary>
    public bool ShowNewRecordButton
    {
        get
        {
            return mShowNewRecordButton;
        }
        set
        {
            mShowNewRecordButton = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether Select Fields button should be visible.
    /// </summary>
    public bool ShowSelectFieldsButton
    {
        get
        {
            return mShowSelectFieldsButton;
        }
        set
        {
            mShowSelectFieldsButton = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Get edited object
        if (CMSContext.EditedObject != null)
        {
            bfi = (BizFormInfo)CMSContext.EditedObject;
            formId = bfi.FormID;
        }

        isDialog = QueryHelper.GetBoolean("dialogmode", false);
        InitHeaderActions();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'ReadData' permission
        CheckPermissions("ReadData");

        // Register scripts
        ScriptHelper.RegisterDialogScript(Page);
        if (mShowSelectFieldsButton)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "SelectFields", ScriptHelper.GetScript("function SelectFields() { modalDialog('" +
                                                                                                            ResolveUrl("~/CMSModules/BizForms/Tools/BizForm_Edit_Data_SelectFields.aspx") + "?formid=" + formId + "'  ,'BizFormFields', 500, 500); }"));
        }
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "Edit", ScriptHelper.GetScript(
            "function EditRecord(formId, recordId) { " +
            "  document.location.replace('" + ResolveUrl("~/CMSModules/BizForms/Tools/BizForm_Edit_EditRecord.aspx") + "?formID=' + formId + '&formRecordID=' + recordId); } "));
        
        // Initialize unigrid
        gridData.OnExternalDataBound += gridData_OnExternalDataBound;
        gridData.OnLoadColumns += gridData_OnLoadColumns;
        gridData.OnAction += gridData_OnAction;

        if (bfi != null)
        {
            dci = DataClassInfoProvider.GetDataClass(bfi.FormClassID);
            if (dci != null)
            {
                className = dci.ClassName;

                // Set alternative form and data container
                gridData.ObjectType = BizFormItemProvider.GetObjectType(className);
                gridData.FilterFormName = className + "." + "filter";
                gridData.FilterFormData = bfi;

                // Get primary column name
                gridData.OrderBy = primaryColumn = GetPrimaryColumn(FormInfo, bfi.FormName);
            }
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        if (mShowNewRecordButton)
        {
            AddHeaderAction(new HeaderAction()
            {
                Text = GetString("bizform_edit_data.newrecord"),
                RedirectUrl = ResolveUrl("~/CMSModules/BizForms/Tools/BizForm_Edit_EditRecord.aspx?formid=" + formId.ToString()),
                ImageUrl = GetImageUrl("CMSModules/CMS_Form/newrecord.png")
            });
        }

        if (ShowSelectFieldsButton)
        {
            AddHeaderAction(new HeaderAction()
            {
                Text = GetString("bizform_edit_data.selectdisplayedfields"),
                OnClientClick = "javascript:SelectFields();",
                ImageUrl = GetImageUrl("CMSModules/CMS_Form/selectfields16.png")
            });
        }
    }


    protected void gridData_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                CheckPermissions("DeleteData");

                // Get record ID
                int formRecordID = ValidationHelper.GetInteger(actionArgument, 0);

                // Get BizFormInfo object
                if (bfi != null)
                {
                    // Get class object
                    if (dci != null)
                    {
                        // Get record object
                        IDataClass formRecord = DataClassFactory.NewDataClass(dci.ClassName, formRecordID);
                        if (!formRecord.IsEmpty())
                        {
                            // Delete all files of the record
                            BizFormInfoProvider.DeleteBizFormRecordFiles(dci.ClassFormDefinition, formRecord, CMSContext.CurrentSiteName);

                            // Delete the form record
                            formRecord.Delete();

                            // Update number of entries in BizFormInfo
                            if (bfi != null)
                            {
                                BizFormInfoProvider.RefreshDataCount(bfi.FormName, bfi.FormSiteID);
                            }
                        }
                    }
                }

                break;
        }
    }


    protected object gridData_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        ImageButton button = sender as ImageButton;
        GridViewRow grv = parameter as GridViewRow;
        DataRowView drv = grv.DataItem as DataRowView;

        switch (sourceName.ToLowerCSafe())
        {
            case "edit":
                if (button != null)
                {
                    if (!isDialog)
                    {
                        button.OnClientClick = "EditRecord(" + formId + ", " + drv[primaryColumn] + "); return false;";
                    }
                    else
                    {
                        button.Visible = false;
                    }
                }
                break;

            case "delete":
                if (button != null)
                {
                    button.CommandArgument = Convert.ToString(drv[primaryColumn]);
                }
                break;
        }

        return parameter;
    }


    protected void gridData_OnLoadColumns()
    {
        if (bfi != null)
        {
            var columnList = new List<string>();
            string columns = bfi.FormReportFields;
            if (string.IsNullOrEmpty(columns))
            {
                columnList = GetExistingColumns();
            }
            else
            {
                // Get existing columns names
                var existingColumns = GetExistingColumns();

                // Get selected columns
                var selectedColumns = GetSelectedColumns(columns);

                columns = string.Empty;
                columnNames = string.Empty;
                StringBuilder sb = new StringBuilder();

                // Remove nonexisting columns
                foreach (string col in selectedColumns)
                {
                    if (existingColumns.Contains(col))
                    {
                        columnList.Add(col);
                        sb.Append(",[").Append(col).Append("]");
                    }
                }
                columnNames = sb.ToString();

                // Ensure primary key
                if (!(columnNames.Contains(primaryColumn) || columnNames.Contains(primaryColumn)))
                {
                    columnNames = ",[" + primaryColumn + "]" + columnNames;
                }

                columnNames = columnNames.TrimStart(',');
            }

            string column = null;
            string fieldCaption = null;
            FormFieldInfo ffi = null;

            // Loop trough all columns
            for (int i = 0; i < columnList.Count; i++)
            {
                column = columnList[i].ToString();

                // Get field caption
                ffi = FormInfo.GetFormField(column);
                if (ffi == null)
                {
                    fieldCaption = column;
                }
                else
                {
                    fieldCaption = (string.IsNullOrEmpty(ffi.Caption)) ? column : ResHelper.LocalizeString(ffi.Caption);
                }

                Column columnDefinition = new Column
                {
                    Caption = fieldCaption,
                    Source = column,
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
        }
    }


    /// <summary>
    /// Overridden SetValue.
    /// </summary>
    /// <param name="propertyName">Name of the property to set</param>
    /// <param name="value">Value to set</param>
    public override void SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName.ToLowerCSafe())
        {
            case "shownewrecordbutton":
                ShowNewRecordButton = ValidationHelper.GetBoolean(value, false);
                break;
            case "showselectfieldsbutton":
                ShowSelectFieldsButton = ValidationHelper.GetBoolean(value, false);
                break;
        }

    }


    /// <summary>
    /// Checks the specified permission.
    /// </summary>
    private void CheckPermissions(string permissionName)
    {
        // Check 'Modify' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.form", permissionName))
        {
            CMSPage.RedirectToCMSDeskAccessDenied("cms.form", permissionName);
        }
    }


    /// <summary>
    /// Get form columns.
    /// </summary>
    private List<string> GetExistingColumns()
    {
        return FormInfo.GetColumnNames();
    }


    /// <summary>
    /// Get list of columns that have been selected to be visible.
    /// </summary>
    /// <param name="columns">Selected columns</param>
    private List<string> GetSelectedColumns(string columns)
    {
        return columns.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }


    /// <summary>
    /// Returns name of the primary key column.
    /// </summary>
    /// <param name="fi">Form info</param>
    /// <param name="bizFormName">Bizform code name</param>
    private static string GetPrimaryColumn(FormInfo fi, string bizFormName)
    {
        string result = null;

        if ((fi != null) && (!string.IsNullOrEmpty(bizFormName)))
        {
            // Seek primary key column in all fields
            var query = from field in fi.GetFields(true, true)
                        where field.PrimaryKey == true
                        select field.Name;

            // Try to get field with the name 'bizformnameID'
            result = query.FirstOrDefault(x => x.ToLowerCSafe() == (bizFormName + "ID").ToLowerCSafe());
            if (String.IsNullOrEmpty(result))
            {
                result = query.FirstOrDefault();
            }
        }

        return result;
    }

    #endregion
}