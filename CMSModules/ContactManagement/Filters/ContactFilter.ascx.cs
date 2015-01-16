using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.SettingsProvider;

public partial class CMSModules_ContactManagement_Filters_ContactFilter : CMSAbstractBaseFilterControl
{
    #region "Methods"

    /// <summary>
    /// Create WHERE condition.
    /// </summary>
    private string CreateWhereCondition()
    {
        string text = txtContact.Text.Trim();
        return "(ContactFirstName LIKE N'%" + SqlHelperClass.GetSafeQueryString(text) + "%' OR ContactMiddleName LIKE N'%" + SqlHelperClass.GetSafeQueryString(text) + "%' OR ContactLastName LIKE N'%" + SqlHelperClass.GetSafeQueryString(text) + "%')";
    }


    /// <summary>
    /// Button search click.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        WhereCondition = CreateWhereCondition();
        RaiseOnFilterChanged();
    }

    #endregion
}