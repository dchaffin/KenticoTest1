using System;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.UIControls.UniMenuConfig;
using CMS.GlobalHelper;

public partial class CMSAdminControls_UI_UniMenu_UniGraphToolbar_EditGroup : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Handles value sent to control.
    /// </summary>
    public string JsGraphObject
    {
        get
        {
            return (string)GetValue("JsGraphObject");
        }
        set
        {
            SetValue("JsGraphObject", value);
        }
    }


    /// <summary>
    /// Pattern to be searched.
    /// </summary>
    public string Search
    {
        get
        {
            return txtSearch.Text;
        }
    }

    #endregion


    #region "Event handlers"

    protected void Page_Load(object sender, EventArgs e)
    {
        AddDeleteButton();
        AddSnapToGridButton();
        InitSearch();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Search initialization.
    /// </summary>
    private void InitSearch()
    {
        btnSearch.ToolTip = ResHelper.GetString("unigraphToolbar.searchTooltip");
        btnSearch.Text = string.Format("<img src=\"{0}\" class=\"SearchImgBtn\" />", GetImageUrl("CMSModules/CMS_Content/Search/Search.png"));
    }


    /// <summary>
    /// Method adding delete button.
    /// </summary>
    private void AddDeleteButton()
    {
        Item newItem = new Item();
        newItem.Text = GetString("general.delete");
        newItem.Tooltip = GetString("unigraphToolbar.deleteTooltip");
        newItem.OnClientClick = JsGraphObject + ".graph.removeSelectedItem();";
        newItem.ImagePath = GetImageUrl("Objects/CMS_WorkflowAction/delete.png");
        newItem.ImageAltText = GetString("unigraphToolbar.deleteTooltip");
        newItem.CssClass = "SmallButton";
        newItem.ImageAlign = ImageAlign.AbsMiddle;
        newItem.MinimalWidth = 66;
        btnsLeft.Buttons.Add(newItem);
    }


    /// <summary>
    /// Method adding button for snapping to grid.
    /// </summary>
    private void AddSnapToGridButton()
    {
        Item newItem = new Item(); 
        newItem.Tooltip = GetString("unigraphToolbar.snapToGridTooltip");
        newItem.OnClientClick = JsGraphObject + ".graph.toggleSnapToGrid();";
        newItem.ImagePath = GetImageUrl("Objects/CMS_WorkflowAction/grid.png");
        newItem.ImageAltText = GetString("unigraphToolbar.snapToGridTooltip");
        newItem.AllowToggle = true;
        newItem.IsToggled = ValidationHelper.GetBoolean(CookieHelper.GetValue("CMSUniGraph"), true);
        newItem.CssClass = "SmallButton";
        newItem.ImageAlign = ImageAlign.AbsMiddle;
        newItem.MinimalWidth = 22;
        btnsRight.Buttons.Add(newItem);
    }

    #endregion
}