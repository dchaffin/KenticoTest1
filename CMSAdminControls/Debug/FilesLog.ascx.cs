using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.IO;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_FilesLog : FilesLog
{
    protected void Page_Load(object sender, EventArgs e)
    {
        EnableViewState = false;
        Visible = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        if (Log != null)
        {
            // Get the log table
            DataTable dt = Log.LogTable;
            ProcessData(dt);
            DataView dv = new DataView(dt);

            if (OperationTypeFilter != IOOperationType.All)
            {
                dv.RowFilter = "OperationType = '" + OperationTypeFilter.ToString() + "'";
            }

            if (!DataHelper.DataSourceIsEmpty(dv))
            {
                Visible = true;

                gridStates.Columns[1].HeaderText = GetString("FilesLog.Operation");
                gridStates.Columns[2].HeaderText = GetString("FilesLog.FilePath");
                gridStates.Columns[3].HeaderText = GetString("FilesLog.OperationType");
                gridStates.Columns[4].HeaderText = GetString("General.Context");

                // Hide the operation type column if only specific operation type is selected
                if (!StorageHelper.IsExternalStorage(null) || (OperationTypeFilter != IOOperationType.All))
                {
                    gridStates.Columns[3].Visible = false;
                }

                if (DisplayHeader)
                {
                    ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("FilesLog.Info") + "</div>";
                }

                MaxSize = DataHelper.GetMaximumValue<int>(dt, "FileSize");

                gridStates.DataSource = dv;
                gridStates.DataBind();
            }
        }
    }


    #region "Helper methods"

    protected int GetIndex()
    {
        return ++index;
    }

    #endregion
}