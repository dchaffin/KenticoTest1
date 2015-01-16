using System;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Avatars_AvatarFilter : CMSAbstractBaseFilterControl
{
    #region "Public properties"

    public override string WhereCondition
    {
        get
        {
            return BuildWhereCondition();
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!RequestHelper.IsPostBack())
        {
            InitializeForm();
        }

        btnReset.Text = GetString("general.reset");
        btnReset.Click += btnReset_Click;
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    /// <summary>
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }

    
    
    #endregion


    #region "Private methods"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtAvatarName.Text = String.Empty;
        drpAvatarType.SelectedIndex = 0;
        drpAvatarKind.SelectedIndex = 1;
        drpAvatarName.SelectedIndex = 0;
    }



    private void InitializeForm()
    {
        // Initialize first dropdown lists
        drpAvatarName.Items.Add(new ListItem("LIKE", "0"));
        drpAvatarName.Items.Add(new ListItem("NOT LIKE", "1"));
        drpAvatarName.Items.Add(new ListItem("=", "2"));
        drpAvatarName.Items.Add(new ListItem("<>", "3"));

        DataHelper.FillListControlWithEnum(typeof(AvatarTypeEnum), drpAvatarType, "avat.type", AvatarInfoProvider.GetAvatarTypeString);
        drpAvatarType.Items.Insert(0, new ListItem(GetString("general.selectall"), ""));

        drpAvatarKind.Items.Add(new ListItem(GetString("general.selectall"), "0"));
        drpAvatarKind.Items.Add(new ListItem(GetString("avat.filter.shared"), "1"));
        drpAvatarKind.Items.Add(new ListItem(GetString("avat.filter.custom"), "2"));
        // Preselect shared
        drpAvatarKind.SelectedIndex = 1;


    }


    /// <summary>
    /// Builds where condition and raises search event.
    /// </summary>
    private string BuildWhereCondition()
    {
        string whereCondition = String.Empty;

        // Avatar name
        string avatarName = txtAvatarName.Text.Trim().Replace("'", "''");
        if (!String.IsNullOrEmpty(avatarName))
        {
            // Get proper operator name
            int sqlOperatorNumber = ValidationHelper.GetInteger(drpAvatarName.SelectedValue, 0);
            string sqlOperatorName = "LIKE";
            switch (sqlOperatorNumber)
            {
                case 1:
                    sqlOperatorName = "NOT LIKE";
                    break;
                case 2:
                    sqlOperatorName = "=";
                    break;
                case 3:
                    sqlOperatorName = "<>";
                    break;
                default:
                    sqlOperatorName = "LIKE";
                    break;
            }

            if ((sqlOperatorName == "LIKE") || (sqlOperatorName == "NOT LIKE"))
            {
                avatarName = "%" + avatarName + "%";
            }

            whereCondition = "(AvatarName " + sqlOperatorName + " N'" + avatarName + "')";
        }

        // Avatar type
        if (!String.IsNullOrEmpty(drpAvatarType.SelectedValue))
        {
            if (!string.IsNullOrEmpty(whereCondition))
            {
                whereCondition += " AND ";
            }
            whereCondition += "(AvatarType = '" + SqlHelperClass.GetSafeQueryString(drpAvatarType.SelectedValue, false) + "')";
        }

        // Avatar kind
        int sqlKindNumber = ValidationHelper.GetInteger(drpAvatarKind.SelectedValue, 0);
        string sqlKindCode = "";
        switch (sqlKindNumber)
        {
            case 1:
                sqlKindCode = "AvatarIsCustom = 0";
                break;
            case 2:
                sqlKindCode = "AvatarIsCustom = 1";
                break;
            default:
                sqlKindCode = "";
                break;
        }

        if (!String.IsNullOrEmpty(sqlKindCode))
        {
            if (!String.IsNullOrEmpty(whereCondition))
            {
                whereCondition += " AND ";
            }
            whereCondition += "(" + sqlKindCode + ")";
        }

        return whereCondition;
    }

    #endregion

 
}