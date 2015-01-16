using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.UIControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.MessageBoard;
using CMS.SettingsProvider;

public partial class CMSModules_MessageBoards_FormControls_SelectBoard : FormEngineUserControl
{
    #region "Variables"

    private bool mAddAllItemsRecord = false;
    private int mGroupId = 0;
    private int mSiteId = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or Sets return column name. Default is BoardID.
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            string returnColumnName = ValidationHelper.GetString(GetValue("ReturnColumnName"), "");

            if (String.IsNullOrEmpty(returnColumnName))
            {
                returnColumnName = "BoardID";
            }

            return returnColumnName;
        }
        set
        {
            SetValue("ReturnColumnName", value);
        }
    }


    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return uniSelector.ValueDisplayName;
        }
    }


    /// <summary>
    /// ID of the current group.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupId;
        }
        set
        {
            mGroupId = value;
        }
    }


    /// <summary>
    /// ID of the current site.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId == 0)
            {
                mSiteId = CMSContext.CurrentSiteID;
            }
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
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
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddAllItemsRecord
    {
        get
        {
            return mAddAllItemsRecord;
        }
        set
        {
            mAddAllItemsRecord = value;
        }
    }


    /// <summary>
    /// Gets the inner DDL control.
    /// </summary>
    public DropDownList DropDownSingleSelect
    {
        get
        {
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets the inner UniSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData(false);
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    /// <param name="forced">Indicates whether UniSelector Reload data should be called</param>
    public void ReloadData(bool forced)
    {
        uniSelector.WhereCondition = "BoardSiteID = " + SiteID + " AND " + (GroupID > 0 ? "BoardGroupID = " + GroupID : "((BoardGroupID = 0) OR (BoardGroupID IS NULL))");
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
        uniSelector.AllowAll = false;
        uniSelector.AllowEmpty = false;
        uniSelector.ReturnColumnName = ReturnColumnName;

        if (AddAllItemsRecord)
        {
            uniSelector.SpecialFields = new string[,] { { GetString("general.selectall"), "ALL" } };
        }

        if (forced)
        {
            uniSelector.Reload(true);
        }
    }
}