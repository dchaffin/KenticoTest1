using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_List : SiteManagerPage, IPostBackEventHandler
{
    #region "Variables"

    protected int categoryId = 0;

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        categoryId = QueryHelper.GetInteger("categoryid", 0);

        // Used for delete calls
        int webpartId = QueryHelper.GetInteger("webpartid", 0);

        string script = @"function RefreshAdditionalContent() {
                            if (parent.parent.frames['webparttree'])
                            {
                                parent.parent.frames['webparttree'].location.href = '" + URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/Development/WebPart_Tree.aspx?categoryid=" + categoryId) + @"';
                            }
                        }";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshTree", ScriptHelper.GetScript(script));

        // Configure the UniGrid
        WebPartCategoryInfo categoryInfo = WebPartCategoryInfoProvider.GetWebPartCategoryInfoById(categoryId);
        if (categoryInfo != null)
        {
            string categoryPath = categoryInfo.CategoryPath;
            // Add the slash character at the end of the categoryPath
            if (!categoryPath.EndsWithCSafe("/"))
            {
                categoryPath += "/";
            }
            webpartGrid.WhereCondition = "ObjectPath LIKE '" + SqlHelperClass.GetSafeQueryString(categoryPath, false) + "%' AND ObjectType = 'webpart'";
        }
        webpartGrid.OnAction += webpartGrid_OnAction;
        webpartGrid.ZeroRowsText = GetString("general.nodatafound");

        InitializeMasterPage();
    }

    #endregion


    #region "UniGrid events"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void webpartGrid_OnAction(string actionName, object actionArgument)
    {
        int webpartId = Convert.ToInt32(actionArgument);

        switch (actionName)
        {
            case "delete":
                // Check 'Modify' permission
                if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.webpart", "Modify"))
                {
                    RedirectToAccessDenied("cms.webpart", "Modify");
                }

                // Check if webpart has widgets
                DataSet dsWidgets = WidgetInfoProvider.GetWidgets("WidgetWebPartID = " + webpartId, null, 0, "WidgetWebPartID");
                if (!DataHelper.DataSourceIsEmpty(dsWidgets))
                {
                    string delPostback = ControlsHelper.GetPostBackEventReference(Page, webpartId.ToString());
                    delPostback = "if (confirm('" + GetString("webparts.deletewithwidgets") + "')) {\n " + delPostback + ";}\n";
                    ltlScript.Text = ScriptHelper.GetScript(delPostback);
                }
                else
                {
                    // Delete PageTemplateInfo object from database
                    WebPartInfoProvider.DeleteWebPartInfo(webpartId);

                    // Refresh tree
                    ltlScript.Text = ScriptHelper.GetScript("RefreshAdditionalContent();");
                }
                break;
        }
    }


    /// <summary>
    /// Handles delete action.
    /// </summary>
    /// <param name="eventArgument">WebPartID</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (!String.IsNullOrEmpty(eventArgument))
        {
            // Delete webpart with widgets
            int webpartId = ValidationHelper.GetInteger(eventArgument, 0);
            WebPartInfoProvider.DeleteWebPartInfo(webpartId);
            ltlScript.Text = String.Empty;
            webpartGrid.ReloadData();
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        Title = "Web part list";
    }

    #endregion
}