using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_List : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int documentTypeId = QueryHelper.GetInteger("documenttypeid", 0);

        // New item link
        string[,] actions = new string[2,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("DocumentType_Edit_Transformation_List.btnNew");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("DocumentType_Edit_Transformation_New.aspx?documenttypeid=" + documentTypeId.ToString() + "&hash=" + QueryHelper.GetHash("?documenttypeid=" + documentTypeId));
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/CMS_Transformation/add.png");

        actions[1, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[1, 1] = GetString("DocumentType_Edit_Transformation_List.btnHierarchicalNew");
        actions[1, 2] = null;
        actions[1, 3] = ResolveUrl("HierarchicalTransformations_New.aspx?documenttypeid=" + documentTypeId.ToString());
        actions[1, 4] = null;
        actions[1, 5] = GetImageUrl("Objects/CMS_Transformation/hierarchicalTransformation.png");


        CurrentMaster.HeaderActions.Actions = actions;

        // Set the query editor control
        classTransformations.ClassID = documentTypeId;
        classTransformations.EditPageUrl = "DocumentType_Edit_Transformation_Frameset.aspx";
        classTransformations.IsSiteManager = true;
    }
}