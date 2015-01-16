using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.Departments.General")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Departments_Department_General : CMSDepartmentsPage
{
    protected int mDepartmentId = 0;
    protected int mEditedSiteId = -1;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Field validator error messages initialization
        rfvDisplayName.ErrorMessage = GetString("Department_Edit.errorEmptyDisplayName");
        rfvCodeName.ErrorMessage = GetString("Department_Edit.errorEmptyCodeName");

        // Show that the department was created or updated successfully
        if (QueryHelper.GetString("saved", "") == "1")
        {
            // Show message
            ShowChangesSaved();
        }

        // Get department id from querystring		
        mDepartmentId = QueryHelper.GetInteger("departmentId", 0);
        mEditedSiteId = ConfiguredSiteID;
        if (mDepartmentId > 0)
        {
            DepartmentInfo departmentObj = DepartmentInfoProvider.GetDepartmentInfo(mDepartmentId);
            EditedObject = departmentObj;

            if (departmentObj != null)
            {
                mEditedSiteId = departmentObj.DepartmentSiteID;
                CheckEditedObjectSiteID(mEditedSiteId);

                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(departmentObj);
                }
            }
        }
    }


    /// <summary>
    /// Load data of editing department.
    /// </summary>
    /// <param name="departmentObj">Department object</param>
    protected void LoadData(DepartmentInfo departmentObj)
    {
        txtDepartmentName.Text = departmentObj.DepartmentName;
        txtDepartmentDisplayName.Text = departmentObj.DepartmentDisplayName;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check permissions
        CheckConfigurationModification(mEditedSiteId);

        // Check input values from textboxes and other controls - new Validator()
        string errorMessage = new Validator()
            .NotEmpty(txtDepartmentDisplayName.Text.Trim(), GetString("Department_Edit.errorEmptyDisplayName"))
            .NotEmpty(txtDepartmentName.Text.Trim(), GetString("Department_Edit.errorEmptyCodeName"))
            .IsCodeName(txtDepartmentName.Text.Trim(), GetString("General.ErrorCodeNameInIdentifierFormat")).Result;

        if (errorMessage == "")
        {
            // DepartmentName must be unique
            DepartmentInfo departmentObj = null;
            string siteWhere = (mEditedSiteId > 0) ? " AND (DepartmentSiteID = " + mEditedSiteId + " OR DepartmentSiteID IS NULL)" : "";
            DataSet ds = DepartmentInfoProvider.GetDepartments("DepartmentName = '" + txtDepartmentName.Text.Trim().Replace("'", "''") + "'" + siteWhere, null, 1, null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                departmentObj = new DepartmentInfo(ds.Tables[0].Rows[0]);
            }

            if ((departmentObj == null) || (departmentObj.DepartmentID == mDepartmentId))
            {
                // Get the object
                if (departmentObj == null)
                {
                    departmentObj = DepartmentInfoProvider.GetDepartmentInfo(mDepartmentId);
                    if (departmentObj == null)
                    {
                        departmentObj = new DepartmentInfo();
                        departmentObj.DepartmentSiteID = mEditedSiteId;
                    }
                }

                departmentObj.DepartmentID = mDepartmentId;
                departmentObj.DepartmentName = txtDepartmentName.Text.Trim();
                departmentObj.DepartmentDisplayName = txtDepartmentDisplayName.Text.Trim();

                // Save the object
                DepartmentInfoProvider.SetDepartmentInfo(departmentObj);

                // Show message
                ShowChangesSaved();
            }
            else
            {
                // Show error message
                ShowError(GetString("Department_Edit.DepartmentNameExists"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }
}