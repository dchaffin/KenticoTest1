using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_Controls_WebParts_WebPartFlatSelector : CMSAdminControl
{
    #region "Variables"

    private string mSelectedItem = string.Empty;
    private WebPartCategoryInfo mSelectedCategory = null;
    private string mTreeSelectedItem = null;
    private bool mShowWidgetOnlyWebparts = false;

    #endregion


    #region "Flat selector properties"

    /// <summary>
    /// Indicates whether webpart of type "Widget only" will be displayed in selector.
    /// </summary>
    public bool ShowWidgetOnlyWebparts
    {
        get
        {
            return mShowWidgetOnlyWebparts;
        }
        set
        {
            mShowWidgetOnlyWebparts = value;
        }
    }


    /// <summary>
    /// Returns inner instance of UniFlatSelector control.
    /// </summary>
    public UniFlatSelector UniFlatSelector
    {
        get
        {
            return flatElem;
        }
    }


    /// <summary>
    /// Gets or sets selected item in flat selector.
    /// </summary>
    public string SelectedItem
    {
        get
        {
            return flatElem.SelectedItem;
        }
        set
        {
            flatElem.SelectedItem = value;
        }
    }


    /// <summary>
    /// Gets or sets the current webpart category.
    /// </summary>
    public WebPartCategoryInfo SelectedCategory
    {
        get
        {
            // If not loaded yet
            if (mSelectedCategory == null)
            {
                int categoryId = ValidationHelper.GetInteger(TreeSelectedItem, 0);
                if (categoryId > 0)
                {
                    mSelectedCategory = WebPartCategoryInfoProvider.GetWebPartCategoryInfoById(categoryId);
                }
            }

            return mSelectedCategory;
        }
        set
        {
            mSelectedCategory = value;
            // Update ID
            if (mSelectedCategory != null)
            {
                mTreeSelectedItem = mSelectedCategory.CategoryID.ToString();
            }
        }
    }


    /// <summary>
    /// Gets or sets the selected item in tree, usually the category id.
    /// </summary>
    public string TreeSelectedItem
    {
        get
        {
            return mTreeSelectedItem;
        }
        set
        {
            // Clear loaded category if change
            if (value != mTreeSelectedItem)
            {
                mSelectedCategory = null;
            }
            mTreeSelectedItem = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            flatElem.StopProcessing = value;
            EnableViewState = !value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Setup flat selector
        flatElem.QueryName = "cms.webpart.selectall";
        flatElem.ValueColumn = "WebPartID";
        flatElem.SearchLabelResourceString = "webpart.webpartname";
        flatElem.SearchColumn = "WebPartDisplayName";
        flatElem.SelectedColumns = "WebPartName, WebPartThumbnailGUID, WebPartDisplayName, WebPartID, WebPartSkipInsertProperties";
        flatElem.SkipPropertiesDialogColumn = "WebPartSkipInsertProperties";
        flatElem.OrderBy = "WebPartDisplayName";
        flatElem.NotAvailableImageUrl = GetImageUrl("Objects/CMS_WebPart/notavailable.png");
        flatElem.NoRecordsMessage = "webparts.norecordsincategory";
        flatElem.NoRecordsSearchMessage = "webparts.norecords";
        flatElem.SearchCheckBox.Visible = true;
        flatElem.SearchCheckBox.Text = GetString("webparts.searchindescription");

        if (!URLHelper.IsPostback())
        {
            // Search in description default value
            flatElem.SearchCheckBox.Checked = false;
        }

        if (flatElem.SearchCheckBox.Checked)
        {
            flatElem.SearchColumn += ";WebPartDescription";
        }

        flatElem.OnItemSelected += new UniFlatSelector.ItemSelectedEventHandler(flatElem_OnItemSelected);
    }


    /// <summary>
    /// On PreRender.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        if (!ShowWidgetOnlyWebparts)
        {
            // Do not display widget only web parts
            flatElem.WhereCondition = SqlHelperClass.AddWhereCondition(flatElem.WhereCondition, "WebPartType IS NULL OR WebPartType != " + Convert.ToInt32(WebPartTypeEnum.WidgetOnly));
        }

        // Restrict to items in selected category (if not root)
        if (SelectedCategory != null)
        {
            flatElem.WhereCondition = SqlHelperClass.AddWhereCondition(flatElem.WhereCondition, "WebPartCategoryID = " + SelectedCategory.CategoryID + " OR WebPartCategoryID IN (SELECT CategoryID FROM CMS_WebPartCategory WHERE CategoryPath LIKE N'" + SqlHelperClass.GetSafeQueryString(SelectedCategory.CategoryPath, false).TrimEnd('/') + "/%')");
        }

        // Recently used
        if (TreeSelectedItem.ToLowerCSafe() == "recentlyused")
        {
            flatElem.WhereCondition = SqlHelperClass.AddWhereCondition(flatElem.WhereCondition, SqlHelperClass.GetWhereCondition("WebPartName", CMSContext.CurrentUser.UserSettings.UserUsedWebParts.Split(';')));
        }

        // Description area and recently used
        litCategory.Text = ShowInDescriptionArea(SelectedItem);

        base.OnPreRender(e);
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// Updates description after item is selected in flat selector.
    /// </summary>
    protected string flatElem_OnItemSelected(string selectedValue)
    {
        return ShowInDescriptionArea(selectedValue);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        flatElem.ReloadData();
        flatElem.ResetToDefault();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Generates HTML text to be used in description area.
    /// </summary>
    ///<param name="selectedValue">Selected item for which generate description</param>
    private string ShowInDescriptionArea(string selectedValue)
    {
        string name = String.Empty;
        string description = String.Empty;

        if (!String.IsNullOrEmpty(selectedValue))
        {
            int webpartId = ValidationHelper.GetInteger(selectedValue, 0);
            WebPartInfo wi = WebPartInfoProvider.GetWebPartInfo(webpartId);
            if (wi != null)
            {
                name = wi.WebPartDisplayName;
                description = wi.WebPartDescription;
            }
        }
        // No selection show selected category
        else if (SelectedCategory != null)
        {
            name = SelectedCategory.CategoryDisplayName;
        }
        // Recently used
        else
        {
            name = GetString("webparts.recentlyused");
        }


        string text = "<div class=\"ItemName\">" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(name)) + "</div>";
        if (description != null)
        {
            text += "<div class=\"Description\">" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(description)) + "</div>";
        }

        return text;
    }

    #endregion
}