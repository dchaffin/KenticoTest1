using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.MediaLibrary;

public partial class CMSModules_MediaLibrary_FormControls_FullMediaLibrarySelector : FormEngineUserControl
{
    #region "Variables"

    private bool mAddNoneItemsRecord = false;
    private bool mAddCurrentLibraryRecord = true;
    private int mSiteId = 0;
    private int mGroupId = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of the site libraries should belongs to.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// ID of the group libraries should belongs to.
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
            EnsureChildControls();
            base.Enabled = value;
            uniSelector.Enabled = value;
            uniSelector.TextBoxSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return uniSelector.Value;
        }
        set
        {
            EnsureChildControls();
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddNoneItemsRecord
    {
        get
        {
            return mAddNoneItemsRecord;
        }
        set
        {
            mAddNoneItemsRecord = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add current media library record to the dropdownlist.
    /// </summary>
    public bool AddCurrentLibraryRecord
    {
        get
        {
            return mAddCurrentLibraryRecord;
        }
        set
        {
            mAddCurrentLibraryRecord = value;
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
            EnsureChildControls();
            base.IsLiveSite = value;
            uniSelector.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set uniselector
        uniSelector.DisplayNameFormat = "{%LibraryDisplayName%}";
        uniSelector.ReturnColumnName = "LibraryName";
        uniSelector.AllowEmpty = false;
        uniSelector.AllowAll = false;

        // Add current media library record if needed
        if (AddCurrentLibraryRecord)
        {
            uniSelector.SpecialFields = new string[2,2]
                                            {
                                                { GetString("general.selectall"), "" },
                                                { GetString("Media.Current"), MediaLibraryInfoProvider.CURRENT_LIBRARY }
                                            };
        }
        else
        {
            uniSelector.SpecialFields = new string[1,2] { { GetString("general.selectall"), "" } };
        }

        // Select non group libraries of current site
        uniSelector.WhereCondition = "(LibrarySiteId=" + ((SiteID > 0) ? SiteID : CMSContext.CurrentSite.SiteID) + " AND LibraryGroupID " +
                                     ((GroupID > 0) ? "=" + GroupID : "IS NULL") + ")";
    }


    /// <summary>
    /// Creates child controls and loads update panle container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load updat panel container
        if (uniSelector == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }

    #endregion
}