using System;
using System.Collections;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_CustomTables_Tools_CustomTable_Data_SelectFields : CMSCustomTablesModalPage
{
    #region "Variables"

    protected int customTableId = 0;
    private DataClassInfo dci = null;

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        RequireSite = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("customtable.data.selectdisplayedfields");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_CustomTables/selectfields.png");
        CurrentMaster.DisplayActionsPanel = true;

        // Get custom table id from url
        customTableId = QueryHelper.GetInteger("customtableid", 0);

        dci = DataClassInfoProvider.GetDataClass(customTableId);
        // Set edited object
        EditedObject = dci;

        // If class exists
        if (dci != null)
        {
            // Check 'Read' permission
            if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.customtables", "Read") &&
                !CMSContext.CurrentUser.IsAuthorizedPerClassName(dci.ClassName, "Read"))
            {
                ShowError(String.Format(GetString("customtable.permissiondenied.read"), dci.ClassName));
                plcContent.Visible = false;
                return;
            }

            HeaderActions.AddAction(new HeaderAction()
            {
                ImageUrl = GetImageUrl("Design/Controls/UI/selectall.png"),
                Text = GetString("UniSelector.SelectAll"),
                OnClientClick = "ChangeFields(true); return false;"
            });

            HeaderActions.AddAction(new HeaderAction()
            {
                ImageUrl = GetImageUrl("Design/Controls/UI/deselectall.png"),
                Text = GetString("UniSelector.DeselectAll"),
                OnClientClick = "ChangeFields(false); return false;"
            });

            Hashtable reportFields = new Hashtable();

            FormInfo fi = null;

            if (!RequestHelper.IsPostBack())
            {
                btnOk.Text = GetString("General.OK");
                btnCancel.Text = GetString("General.Cancel");

                // Get report fields
                if (!String.IsNullOrEmpty(dci.ClassShowColumns))
                {
                    reportFields.Clear();

                    foreach (string field in dci.ClassShowColumns.Split(';'))
                    {
                        // Add field key to hastable
                        reportFields[field] = null;
                    }
                }

                // Get columns names
                fi = FormHelper.GetFormInfo(dci.ClassName, false);
                var columnNames = fi.GetColumnNames();

                if (columnNames != null)
                {
                    FormFieldInfo ffi = null;
                    ListItem item = null;
                    foreach (string name in columnNames)
                    {
                        ffi = fi.GetFormField(name);

                        // Add checkboxes to the list
                        item = new ListItem(HTMLHelper.HTMLEncode(ResHelper.LocalizeString(GetFieldCaption(ffi, name))), name);
                        if (reportFields.Contains(name))
                        {
                            // Select checkbox if field is reported
                            item.Selected = true;
                        }
                        chkListFields.Items.Add(item);
                    }
                }
            }
        }
    }

    #endregion


    #region "Button handling"

    /// <summary>
    /// Button OK clicked.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (dci != null)
        {
            string reportFields = null;
            bool itemSelected = (chkListFields.SelectedIndex != -1);

            if (itemSelected)
            {
                foreach (ListItem item in chkListFields.Items)
                {
                    // Display only selected fields
                    if (item.Selected)
                    {
                        reportFields += item.Value + ";";
                    }
                }
            }

            if (!string.IsNullOrEmpty(reportFields))
            {
                // Remove ending ';'
                reportFields = reportFields.TrimEnd(';');
            }


            // Save report fields
            if (CMSString.Compare(dci.ClassShowColumns, reportFields, true) != 0)
            {
                dci.ClassShowColumns = reportFields;
                DataClassInfoProvider.SetDataClass(dci);
            }

            // Close dialog window
            ScriptHelper.RegisterStartupScript(this, typeof(string), "CustomTable_SelectFields", "CloseAndRefresh();", true);
        }
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Returns field caption of the specified column.
    /// </summary>
    /// <param name="ffi">Form field info</param>
    /// <param name="columnName">Column name</param>    
    protected string GetFieldCaption(FormFieldInfo ffi, string columnName)
    {
        // Get field caption        
        return ((ffi == null) || (ffi.Caption == string.Empty)) ? columnName : ffi.Caption;
    }

    #endregion
}