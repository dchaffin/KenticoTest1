using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Categories_Controls_MultipleCategoriesSelector : CMSAdminEditControl
{
    #region "Variables"

    private bool mSelectOnlyEnabled = true;
    private int mDocumentID = 0;
    private bool mFormControlMode = false;
    private int mUserID = 0;
    private bool mEnabled = true;
    private bool isSaved = false;
    private bool mDisplaySavedMessage = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the DocumentID for which the categories should be loaded.
    /// </summary>
    public int DocumentID
    {
        get
        {
            return mDocumentID;
        }
        set
        {
            mDocumentID = value;
        }
    }


    /// <summary>
    /// Gets or sets the UserID whose categories should be displayed.
    /// </summary>
    public int UserID
    {
        get
        {
            if (mUserID > 0)
            {
                return mUserID;
            }
            else
            {
                return CMSContext.CurrentUser.UserID;
            }
        }
        set
        {
            mUserID = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to display only enabled categories.
    /// </summary>
    public bool SelectOnlyEnabled
    {
        get
        {
            return mSelectOnlyEnabled;
        }
        set
        {
            mSelectOnlyEnabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines if form control mode is On or not.
    /// </summary>
    public bool FormControlMode
    {
        get
        {
            return mFormControlMode;
        }
        set
        {
            mFormControlMode = value;
        }
    }


    public UniSelector UniSelector
    {
        get
        {
            return selectCategory;
        }
    }


    /// <summary>
    /// Enabled state of the control.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
            selectCategory.Enabled = value;
        }
    }


    /// <summary>
    /// Indicates if control has to display its own 'The changes were saved' message.
    /// </summary>
    public bool DisplaySavedMessage
    {
        get
        {
            return mDisplaySavedMessage;
        }
        set
        {
            mDisplaySavedMessage = value;
        }
    }


    /// <summary>
    /// Indicates whether global categories are allowed for selected site.
    /// </summary>
    private bool AllowGlobalCategories
    {
        get
        {
            return SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSAllowGlobalCategories");
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs after data is saved to the database.
    /// </summary>
    public delegate void OnAfterSaveEventHandler();

    /// <summary>
    /// OnAfterSave event.
    /// </summary>
    public event OnAfterSaveEventHandler OnAfterSave;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Propagate options
            selectCategory.IsLiveSite = IsLiveSite;
            selectCategory.GridName = "~/CMSModules/Categories/Controls/Categories.xml";
            selectCategory.OnAdditionalDataBound += selectCategory_OnAdditionalDataBound;
            selectCategory.UniGrid.OnAfterRetrieveData += UniGrid_OnAfterRetrieveData;
            selectCategory.ItemsPerPage = 25;

            // Select appropriate dialog window
            if (IsLiveSite)
            {
                selectCategory.SelectItemPageUrl = "~/CMSModules/Categories/CMSPages/LiveCategorySelection.aspx";
            }
            else
            {
                selectCategory.SelectItemPageUrl = "~/CMSModules/Categories/Dialogs/CategorySelection.aspx";
            }

            if (!RequestHelper.IsPostBack())
            {
                ReloadData();
            }

            isSaved = QueryHelper.GetBoolean("saved", false);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (DisplaySavedMessage && !FormControlMode && isSaved)
        {
            // Changes saved
            ShowChangesSaved();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads control data
    /// </summary>
    public override void ReloadData(bool forceReload)
    {
        // Prepare selected values
        DataSet ds = CategoryInfoProvider.GetDocumentCategories(DocumentID, GetWhereCondition(), null, 0, "CMS_Category.CategoryID");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            selectCategory.Value = TextHelper.Join(";", SystemDataHelper.GetStringValues(ds.Tables[0], "CategoryID"));
        }

        if (forceReload)
        {
            selectCategory.Reload(true);
        }
    }


    protected object selectCategory_OnAdditionalDataBound(object sender, string sourceName, object parameter, string value)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Resolve category name
            case "name":
                string namePath = parameter as string;
                if (!string.IsNullOrEmpty(namePath))
                {
                    namePath = namePath.TrimStart('/');
                    namePath = namePath.Replace("/", "&nbsp;&gt;&nbsp;");
                    value = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(namePath));
                }

                break;
        }

        return value;
    }


    private DataSet UniGrid_OnAfterRetrieveData(DataSet ds)
    {
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            DataTable table = ds.Tables[0];
            Hashtable toDelete = new Hashtable();

            // Remove categories having child in the table
            foreach (DataRow dr in table.Rows)
            {
                string namePath = ValidationHelper.GetString(dr["CategoryNamePath"], "");
                int id = ValidationHelper.GetInteger(dr["CategoryID"], 0);

                // Check if table contains any child
                foreach (DataRow drChild in table.Rows)
                {
                    string childNamePath = ValidationHelper.GetString(drChild["CategoryNamePath"], "");
                    if (!toDelete.Contains(id) && childNamePath.StartsWithCSafe(namePath + "/"))
                    {
                        // Place parent on the black list
                        toDelete.Add(id, dr);

                        string ids = ValidationHelper.GetString(selectCategory.Value, "");
                        selectCategory.Value = ids.Replace(id.ToString(), "").Replace(";;", ";");

                        break;
                    }
                }
            }

            // Remove categories from blacklist
            foreach (DataRow row in toDelete.Values)
            {
                row.Delete();
            }

            // Accept changes
            ds.AcceptChanges();
        }

        return ds;
    }


    /// <summary>
    /// Saves the values.
    /// </summary>
    public void Save()
    {
        if (!RaiseOnCheckPermissions(PERMISSION_MODIFY, this))
        {
            CurrentUserInfo cui = CMSContext.CurrentUser;
            if ((cui == null) || ((UserID != cui.UserID) && !cui.IsAuthorizedPerResource("CMS.Users", PERMISSION_MODIFY)))
            {
                RedirectToAccessDenied("CMS.Users", PERMISSION_MODIFY);
            }
        }

        // Prepare selected values
        string mCurrentValues = "";
        DataSet ds = CategoryInfoProvider.GetDocumentCategories(DocumentID, GetWhereCondition(), null, 0, "CMS_Category.CategoryID");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            mCurrentValues = TextHelper.Join(";", SystemDataHelper.GetStringValues(ds.Tables[0], "CategoryID"));
        } 

        // Remove old items
        string newValues = ValidationHelper.GetString(selectCategory.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, mCurrentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to user
                foreach (string item in newItems)
                {
                    int categoryId = ValidationHelper.GetInteger(item, 0);
                    DocumentCategoryInfoProvider.RemoveDocumentFromCategory(mDocumentID, categoryId);
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(mCurrentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to user
                foreach (string item in newItems)
                {
                    int categoryId = ValidationHelper.GetInteger(item, 0);

                    // Make sure, that category still exists
                    if (CategoryInfoProvider.GetCategoryInfo(categoryId) != null)
                    {
                        DocumentCategoryInfoProvider.AddDocumentToCategory(mDocumentID, categoryId);
                    }
                }

                // Find all categories which child categories are bound to edited document
                DataSet catsToRemove = CategoryInfoProvider.GetDocumentCategories(mDocumentID, "EXISTS (SELECT CategoryID FROM CMS_Category AS CC WHERE CC.CategoryIDPath LIKE CMS_Category.CategoryIDPath + '/%' AND CC.CategoryID IN (SELECT CategoryID FROM CMS_DocumentCategory WHERE DocumentID = " + mDocumentID + "))", null, 0, "CMS_Category.CategoryID AS ID");
                if (!DataHelper.DataSourceIsEmpty(catsToRemove))
                {
                    foreach (DataRow dr in catsToRemove.Tables[0].Rows)
                    {
                        // Remove categories covered by their children from document
                        DocumentCategoryInfoProvider.RemoveDocumentFromCategory(mDocumentID, ValidationHelper.GetInteger(dr["ID"], 0));
                    }
                }
            }
        }

        // Raise on after save
        if (OnAfterSave != null)
        {
            OnAfterSave();
        }

        isSaved = true;
    }


    /// <summary>
    /// Creates where condition filtering only categories allowed on current site and for user specified by UserID property.
    /// </summary>
    private string GetWhereCondition()
    {
        string where = "CategorySiteID = " + CMSContext.CurrentSiteID;

        if (AllowGlobalCategories)
        {
            where = SqlHelperClass.AddWhereCondition(where, "CategorySiteID IS NULL AND CategoryUserID IS NULL", "OR");
        }

        if (UserID > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "CategoryUserID =" + UserID, "OR");
        }

        return string.Format("({0})", where);
    }

    #endregion
}