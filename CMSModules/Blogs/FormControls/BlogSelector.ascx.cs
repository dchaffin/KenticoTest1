using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.Blogs;

public partial class CMSModules_Blogs_FormControls_BlogSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mIsLiveSite = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets display name from uniselector.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return UniSelector.ValueDisplayName;
        }
    }


    /// <summary>
    /// Gets or sets return column name.
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            string column = ValidationHelper.GetString(GetValue("ReturnColumnName"), "");

            if (string.IsNullOrEmpty(column))
            {
                column = "NodeAliasPath";
            }

            return column;
        }
        set
        {
            SetValue("ReturnColumnName", value);
        }
    }


    /// <summary>
    /// Gets or sets whether show only user's blogs.
    /// </summary>
    public bool OnlyUsersBlogs
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("OnlyUsersBlogs"), true);
        }
        set
        {
            SetValue("OnlyUsersBlogs", value);
        }
    }


    /// <summary>
    /// Gets or sets selected items.
    /// </summary>
    public override object Value
    {
        get
        {
            if ((FieldInfo != null) && (FieldInfo.DataType == FormFieldDataTypeEnum.GUID))
            {
                return blogSelector.Value;
            }

            string aliasPath = ValidationHelper.GetString(blogSelector.Value, null);
            if (aliasPath == "0")
            {
                return string.Empty;
            }
            if (!aliasPath.EndsWithCSafe("/%"))
            {
                aliasPath = aliasPath.TrimEnd('/') + "/%";
            }

            return aliasPath;
        }
        set
        {
            if (blogSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            if (blogSelector != null)
            {
                if ((FieldInfo != null) && (FieldInfo.DataType == FormFieldDataTypeEnum.GUID))
                {
                    blogSelector.Value = value;
                }
                else
                {
                    string aliasPath = ValidationHelper.GetString(value, String.Empty);

                    if (aliasPath == string.Empty)
                    {
                        blogSelector.Value = "0";
                    }
                    else
                    {
                        aliasPath = aliasPath.TrimEnd('%').TrimEnd('/');
                        if (aliasPath == string.Empty)
                        {
                            aliasPath = "/";
                        }
                        blogSelector.Value = aliasPath;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Gets the inner blogSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return blogSelector;
        }
    }


    /// <summary>
    /// Gets the single select drop down field.
    /// </summary>
    public DropDownList DropDownSingleSelect
    {
        get
        {
            EnsureChildControls();
            return blogSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets UpdatePanel of selector.
    /// </summary>
    public CMSUpdatePanel UpdatePanel
    {
        get
        {
            return pnlUpdate;
        }
    }


    /// <summary>
    /// Enables or disables site selector dropdown.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            DropDownSingleSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Enables or disables live site mode of selection dialog.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return mIsLiveSite;
        }
        set
        {
            mIsLiveSite = value;
            if (blogSelector != null)
            {
                blogSelector.IsLiveSite = value;
            }
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            blogSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        // Generate where condition
        UpdateWhereCondition();

        // Set return column
        blogSelector.ReturnColumnName = ReturnColumnName;

        string[,] fields = new string[,] { { GetString("blogselector.selectblog"), "0" } };
        blogSelector.SpecialFields = fields;

        blogSelector.IsLiveSite = IsLiveSite;
        
        try
        {
            blogSelector.Reload(false);
        }
        catch (Exception ex)
        {
            blogSelector.StopProcessing = true;
            ShowError("BlogSelector: " + GetString("report_error.executequery"), ex.ToString(), null);
        }
    }


    protected override void EnsureChildControls()
    {
        if (blogSelector == null)
        {
            pnlUpdate.LoadContainer();
        }
        base.EnsureChildControls();
    }


    /// <summary>
    /// Updates uni selector where condition based on current properties values.
    /// </summary>
    public void UpdateWhereCondition()
    {
        string where = String.Empty;

        if (OnlyUsersBlogs)
        {
            where = "NodeOwner=" + CMSContext.CurrentUser.UserID;
        }

        DataClassInfo dci = DataClassInfoProvider.GetDataClass("cms.blog");
        if (dci != null)
        {
            where = SqlHelperClass.AddWhereCondition(where, "NodeClassID=" + dci.ClassID);
        }

        where = SqlHelperClass.AddWhereCondition(where, "NodeSiteID=" + CMSContext.CurrentSiteID);

        blogSelector.WhereCondition = where;
    }


    /// <summary>
    /// Reloads all controls.
    /// </summary>
    /// <param name="forceReload">Indicates if data should be loaded from DB</param>
    public void Reload(bool forceReload)
    {
        blogSelector.Reload(forceReload);
    }

    #endregion
}