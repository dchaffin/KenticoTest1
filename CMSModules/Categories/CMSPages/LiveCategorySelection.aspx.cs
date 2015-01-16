using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_Categories_CMSPages_LiveCategorySelection : CMSLiveModalPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get title text according to selection mode
        CurrentMaster.Title.TitleText = GetString(SelectionElem.AllowMultipleSelection ? "categories.selectmultiple" : "categories.select");
        CurrentMaster.Title.TitleImage = GetImageUrl("/CMSModules/CMS_Categories/module.png");

        // Set actions
        SelectionElem.Actions = actionsElem;
    }


    /// <summary>
    /// Load event handler
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #endregion
}