using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Pages_ObjectRelationships : CMSModalSiteManagerPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // Setup the control
        relElem.ObjectID = QueryHelper.GetInteger("objectid", 0);
        relElem.ObjectType = QueryHelper.GetString("objecttype", "");

        GeneralizedInfo obj = relElem.Object;
        if (obj != null)
        {
            // Set the master page title
            Title = String.Format(GetString("ObjectRelationships.Title"), GetString("objecttype." + obj.ObjectType.Replace(".", "_").Replace("#", "_")), obj.ObjectDisplayName);

            CurrentMaster.Title.TitleText = Title;
            CurrentMaster.Title.TitleImage = GetObjectIconUrl(relElem.ObjectType, "/object.png");

            ((Panel)CurrentMaster.PanelBody.FindControl("pnlContent")).CssClass = "";
        }
    }
}