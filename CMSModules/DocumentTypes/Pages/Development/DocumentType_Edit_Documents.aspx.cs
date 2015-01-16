using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Documents : SiteManagerPage
{
    #region "Private variables"

    private int documentTypeId = 0;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        filterDocuments.ClassPlaceHolder.Visible = false;

        // Get current page template ID
        documentTypeId = QueryHelper.GetInteger("documenttypeid", 0);

        docElem.SiteName = filterDocuments.SelectedSite;
        docElem.UniGrid.OnBeforeDataReload += new OnBeforeDataReload(UniGrid_OnBeforeDataReload);
        docElem.UniGrid.OnAfterDataReload += new OnAfterDataReload(UniGrid_OnAfterDataReload);
    }

    #endregion


    #region "Grid events"

    protected void UniGrid_OnBeforeDataReload()
    {
        string where = "ClassName IN (SELECT ClassName FROM CMS_Class WHERE ClassID =" + documentTypeId + ")";
        where = SqlHelperClass.AddWhereCondition(where, filterDocuments.WhereCondition);
        docElem.UniGrid.WhereCondition = SqlHelperClass.AddWhereCondition(docElem.UniGrid.WhereCondition, where);
    }


    protected void UniGrid_OnAfterDataReload()
    {
        bool displayFilter = docElem.UniGrid.DisplayExternalFilter(filterDocuments.FilterIsSet);
        plcFilter.Visible = displayFilter;
        
        // Display title if unigrid is not empty
        plcTitle.Visible = !DataHelper.DataSourceIsEmpty(docElem.UniGrid.GridView.DataSource);
        
        if (displayFilter)
        {
            // If filter is visible, ZeroRowsText will be default FilteredZeroRowsText from unigrid.
            docElem.ZeroRowsText = docElem.UniGrid.FilteredZeroRowsText;
        }
        else
        {
            // If filter is not visible, ZeroRowsText will say, that DocumentType is not used for any document. 
            docElem.ZeroRowsText = GetString("documenttype_edit_general.documents.nodata");
        }
    }

    #endregion
}