using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_UI_PanelMenu_PanelMenuCategory : CMSUserControl
{
    #region "Variables"

    private string mCategoryName = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Category name.
    /// </summary>
    public string CategoryName
    {
        get
        {
            return mCategoryName;
        }
        set
        {
            mCategoryName = value;
        }
    }


    /// <summary>
    /// Category title.
    /// </summary>
    public string CategoryTitle
    {
        get
        {
            return lnkCategoryTitle.Text;
        }
        set
        {
            lnkCategoryTitle.Text = HTMLHelper.HTMLEncode(value);
            imgCategoryImage.AlternateText = HTMLHelper.HTMLEncode(value);
        }
    }


    /// <summary>
    /// Category URL.
    /// </summary>
    public string CategoryURL
    {
        get
        {
            return lnkCategoryTitle.NavigateUrl;
        }
        set
        {
            lnkCategoryTitle.NavigateUrl = value;
        }
    }


    /// <summary>
    /// Category image URL.
    /// </summary>
    public string CategoryImageUrl
    {
        get
        {
            return imgCategoryImage.ImageUrl;
        }
        set
        {
            imgCategoryImage.ImageUrl = value;
        }
    }


    /// <summary>
    /// Category tooltip.
    /// </summary>
    public string CategoryTooltip
    {
        get
        {
            return lnkCategoryTitle.ToolTip;
        }
        set
        {
            lnkCategoryTitle.ToolTip = value;
            imgCategoryImage.ToolTip = value;
        }
    }


    /// <summary>
    /// Category actions data {[n, 0] - Action name, [n, 1] - Action URL}.
    /// </summary>
    public string[,] CategoryActions
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlCategory.Attributes.Add("onclick", String.Format("SelectRibbonButton({0});window.location.href = '{1}';", ScriptHelper.GetString(CategoryName), URLHelper.ResolveUrl(CategoryURL)));
        lnkCategoryTitle.Attributes.Add("onclick", String.Format("SelectRibbonButton({0});", ScriptHelper.GetString(CategoryName)));
        LoadCategoryActions();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates hyperlinks for category actions.
    /// </summary>
    public void LoadCategoryActions()
    {
        if (CategoryActions != null)
        {
            int actionsCount = CategoryActions.GetLength(0);

            object[] actions = new object[actionsCount];

            for (int i = 0; i < actionsCount; i++)
            {
                string[] action = { HTMLHelper.HTMLEncode(CategoryActions[i, 0]), CategoryActions[i, 1], String.Format("SelectRibbonButton({0});", ScriptHelper.GetString(CategoryName)) };
                actions[i] = action;
            }

            rptCategoryActions.DataSource = actions;
            rptCategoryActions.DataBind();
        }
    }

    #endregion
}