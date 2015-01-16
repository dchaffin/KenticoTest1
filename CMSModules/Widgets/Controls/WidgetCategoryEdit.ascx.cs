using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Widgets_Controls_WidgetCategoryEdit : CMSAdminEditControl
{
    #region "Variables"

    private WidgetCategoryInfo mCategoryInfo = null;
    private int mParentCategoryID = 0;

    #endregion


    #region "Properties"

    public WidgetCategoryInfo CategoryInfo
    {
        get
        {
            // If not loaded yet
            if ((mCategoryInfo == null) && (ItemID > 0))
            {
                mCategoryInfo = WidgetCategoryInfoProvider.GetWidgetCategoryInfo(ItemID);
            }

            return mCategoryInfo;
        }

        set
        {
            mCategoryInfo = value;
        }
    }


    /// <summary>
    /// Gets or sets the id of parent category of new widget category.
    /// </summary>
    public int ParentCategoryID
    {
        get
        {
            return mParentCategoryID;
        }

        set
        {
            mParentCategoryID = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            if (CategoryInfo != null)
            {
                txtCodeName.Text = CategoryInfo.WidgetCategoryName;
                txtDisplayName.Text = CategoryInfo.WidgetCategoryDisplayName;
                txtImagePath.Text = CategoryInfo.WidgetCategoryImagePath;

                // Disable editing of root widget category code name
                if (CategoryInfo.WidgetCategoryPath == "/")
                {
                    plcCodeName.Visible = false;
                }
            }
        }

        // Existing category saved - display text response and refresh widget tree and header
        if (QueryHelper.GetBoolean("saved", false) == true)
        {
            if (!URLHelper.IsPostback())
            {
                ShowChangesSaved();
            }

            if (CategoryInfo != null)
            {
                string script = "parent.parent.frames['widgettree'].location = '" + URLHelper.ResolveUrl("WidgetTree.aspx?categoryid=" + CategoryInfo.WidgetCategoryID) + "';";
                script += "parent.frames['categoryHeader'].location = '" + URLHelper.ResolveUrl("Category_Header.aspx?categoryid=" + CategoryInfo.WidgetCategoryID + "&saved=1") + "';";
                ScriptHelper.RegisterStartupScript(Page, typeof(string), "reloadwidgettree", ScriptHelper.GetScript(script));
            }
        }
        // New category saved - display this new category list and reload widget tree
        else if (QueryHelper.GetBoolean("new", false) == true)
        {
            string script = "parent.frames['widgettree'].location = '" + URLHelper.ResolveUrl("~/CMSModules/Widgets/UI/WidgetTree.aspx?categoryid=" + CategoryInfo.WidgetCategoryID + "&reload=1") + "';";
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "reloadwidgettree", ScriptHelper.GetScript(script));
        }

        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvCodeName.ErrorMessage = GetString("general.requirescodename");
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.widgetcategory", PERMISSION_MODIFY))
        {
            return;
        }

        // Limit length of inputs
        txtCodeName.Text = TextHelper.LimitLength(txtCodeName.Text.Trim(), 100, "");
        txtDisplayName.Text = TextHelper.LimitLength(txtDisplayName.Text.Trim(), 100, "");
        txtImagePath.Text = TextHelper.LimitLength(txtImagePath.Text.Trim(), 400, "");

        // Perform validation
        string errorMessage = new Validator().NotEmpty(txtCodeName.Text, rfvCodeName.ErrorMessage).NotEmpty(txtDisplayName.Text, rfvDisplayName.ErrorMessage).Result;

        if (String.IsNullOrEmpty(errorMessage))
        {
            // New category
            if (CategoryInfo == null)
            {
                CategoryInfo = new WidgetCategoryInfo();
                CategoryInfo.WidgetCategoryParentID = ParentCategoryID;
            }

            // Indicates whether current object is root 
            bool isRoot = (CategoryInfo.WidgetCategoryPath == "/");
            if (!isRoot)
            {
                // Check codename
                errorMessage = new Validator().IsCodeName(txtCodeName.Text, GetString("general.invalidcodename")).Result;
                if (!String.IsNullOrEmpty(errorMessage))
                {
                    // Error message - validation
                    ShowError(errorMessage);
                    return;
                }
            }

            WidgetCategoryInfo current = WidgetCategoryInfoProvider.GetWidgetCategoryInfo(txtCodeName.Text);

            // Check if code name is unique
            if ((current == null) || (CategoryInfo.WidgetCategoryID == current.WidgetCategoryID))
            {
                // Set category values
                if (!isRoot)
                {
                    CategoryInfo.WidgetCategoryName = txtCodeName.Text;
                }
                CategoryInfo.WidgetCategoryDisplayName = txtDisplayName.Text;
                CategoryInfo.WidgetCategoryImagePath = txtImagePath.Text;

                WidgetCategoryInfoProvider.SetWidgetCategoryInfo(CategoryInfo);

                // Redirect to edit mode
                RaiseOnSaved();
            }
            else
            {
                // Error message - code name already exists
                ShowError(GetString("general.codenameexists"));
            }
        }
        else
        {
            // Error message - validation
            ShowError(errorMessage);
        }
    }

    #endregion
}