using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Reporting_Dialogs_ReportParametersSelector : CMSModalPage
{
    /// <summary>
    /// OnInit.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        ScriptHelper.RegisterWOpenerScript(Page);
        CurrentMaster.Title.TitleText = ResHelper.GetString("rep.webparts.reportparameterstitle");
        CurrentMaster.Title.TitleImage = GetImageUrl("Design/Selectors/selecttransformation.png");

        // Load data for this form
        int reportID = QueryHelper.GetInteger("ReportID", 0);
        hdnGuid.Value = QueryHelper.GetString("GUID", String.Empty);
        ReportInfo ri = ReportInfoProvider.GetReportInfo(reportID);
        if (ri == null)
        {
            return;
        }

        FormInfo fi = new FormInfo(ri.ReportParameters);
        bfParameters.FormInformation = fi;
        bfParameters.SubmitButton.Visible = false;
        bfParameters.Mode = FormModeEnum.Update;

        // Get dataset from cache
        DataSet ds = (DataSet)WindowHelper.GetItem(hdnGuid.Value);
        DataRow dr = fi.GetDataRow();
        fi.LoadDefaultValues(dr, true);

        if (ds == null)
        {
            if (dr.ItemArray.Length > 0)
            {
                // Set up grid
                if (RequestHelper.IsPostBack())
                {
                    bfParameters.DataRow = fi.GetDataRow();
                }
                else
                {
                    bfParameters.DataRow = dr;
                }
            }
        }

        // Set data set given from cache
        if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
        {
            //Merge with default data from report
            MergeDefaultValuesWithData(dr, ds.Tables[0].Rows[0]);
            //Set row to basic form
            bfParameters.DataRow = dr;
        }

        // Test if there is any item visible in report parameters
        bool itemVisible = false;
        List<IFormItem> items = fi.ItemsList;
        foreach (IFormItem item in items)
        {
            FormFieldInfo ffi = item as FormFieldInfo;
            if (ffi != null)
            {
                if (ffi.Visible)
                {
                    itemVisible = true;
                    break;
                }
            }
        }

        if (!itemVisible)
        {
            pnlInfo.Visible = true;
            lblInfo.Text = GetString("rep.parameters.noparameters");
            btnOK.Enabled = false;
        }

        base.OnInit(e);
    }


    /// <summary>
    /// Merge default values with data row. If column not exist in data row default value is loaded.
    /// </summary>
    /// <param name="defaultRow">Default data row</param>
    /// <param name="dataRow">Changed data row</param>
    protected void MergeDefaultValuesWithData(DataRow defaultRow, DataRow dataRow)
    {
        // Search all columns in defualt report parameters
        foreach (DataColumn col in dataRow.Table.Columns)
        {
            string sourceColumnName = col.ColumnName;
            if (defaultRow.Table.Columns.Contains(sourceColumnName))
            {
                // Use try-catch block, to prevent error in conversion among different data types (string->int).         
                try
                {
                    defaultRow[sourceColumnName] = dataRow[sourceColumnName];
                }
                catch
                { }
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        btnOK.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.cancel");
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        // If data are valid
        if (bfParameters.ValidateData())
        {
            // Fill BasicForm with user data
            bfParameters.SaveData(null);
            if (bfParameters.DataRow != null)
            {
                int reportID = QueryHelper.GetInteger("ReportID", 0);
                ReportInfo ri = ReportInfoProvider.GetReportInfo(reportID);
                if (ri == null)
                {
                    return;
                }

                FormInfo fi = new FormInfo(ri.ReportParameters);
                DataRow defaultRow = fi.GetDataRow();
                fi.LoadDefaultValues(defaultRow);

                // Load default parameters to items ,where displayed in edit form not checked
                List<IFormItem> items = fi.ItemsList;
                foreach (IFormItem item in items)
                {
                    FormFieldInfo ffi = item as FormFieldInfo;
                    if ((ffi != null) && (!ffi.Visible))
                    {
                        bfParameters.DataRow[ffi.Name] = defaultRow[ffi.Name];
                    }
                }

                WindowHelper.Add(hdnGuid.Value, bfParameters.DataRow.Table.DataSet);
            }
            // Refreshh opener update panel script
            ltlScript.Text = ScriptHelper.GetScript("wopener.refresh (); CloseDialog()");
        }
    }
}