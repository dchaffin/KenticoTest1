using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_SearchFields : SiteManagerPage
{
    private int classId = QueryHelper.GetInteger("documenttypeid", 0);


    protected void Page_Load(object sender, EventArgs e)
    {
        DataClassInfo dci = DataClassInfoProvider.GetDataClass(classId);
        if ((dci == null) || (!dci.ClassIsCoupledClass))
        {
            // Show error message
            ShowError(GetString("srch.doctype.ErrorIsNotCoupled"));

            SearchFields.StopProcessing = true;
            SearchFields.Visible = false;
        }
        else
        {
            SearchFields.ItemID = classId;
        }
    }
}