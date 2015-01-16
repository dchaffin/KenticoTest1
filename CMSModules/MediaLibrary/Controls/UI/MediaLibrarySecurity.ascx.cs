using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.MediaLibrary;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_MediaLibrary_Controls_UI_MediaLibrarySecurity : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    private int mMediaLibraryID = 0;
    private bool mNoRolesAvailable = false;
    private bool mEnable = true;

    private MediaLibraryInfo mLibraryInfo = null;
    private ResourceInfo mResLibrary = null;

    // HashTable holding information on all permissions that 'OnlyAuthorizedRoles' access is selected for
    private Hashtable onlyAuth = new Hashtable();

    private ArrayList permissionArray = new ArrayList();

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current library info.
    /// </summary>
    private MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (MediaLibraryID > 0))
            {
                mLibraryInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(MediaLibraryID);
            }
            return mLibraryInfo;
        }
    }


    /// <summary>
    /// Current library resource info.
    /// </summary>
    private ResourceInfo ResLibrary
    {
        get
        {
            if (mResLibrary == null)
            {
                mResLibrary = ResourceInfoProvider.GetResourceInfo("CMS.MediaLibrary");
            }
            return mResLibrary;
        }
    }

    #endregion


    #region "Public properties"

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
    /// Indicates if control is used on a live site
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
    /// Gets or sets the ID of the library to edit.
    /// </summary>
    public int MediaLibraryID
    {
        get
        {
            return mMediaLibraryID;
        }
        set
        {
            mMediaLibraryID = value;
            mLibraryInfo = null;
        }
    }


    /// <summary>
    /// Indicates whether permissions matrix is enabled.
    /// </summary>
    public bool Enable
    {
        get
        {
            return mEnable;
        }
        set
        {
            mEnable = value;
        }
    }

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        if (MediaLibraryID > 0)
        {
            // Render permission matrix
            CreateMatrix();
        }

        base.OnPreRender(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        if (!Visible)
        {
            EnableViewState = false;
        }

        if (MediaLibraryID > 0)
        {
            // Get information on current library            
            permissionArray.Add("filecreate");
            permissionArray.Add("foldercreate");
            permissionArray.Add("filedelete");
            permissionArray.Add("folderdelete");
            permissionArray.Add("filemodify");
            permissionArray.Add("foldermodify");
            permissionArray.Add("libraryaccess");

            if ((ResLibrary != null) && (LibraryInfo != null))
            {
                // Retrieve permission matrix data
                QueryDataParameters parameters = new QueryDataParameters();
                parameters.Add("@ID", ResLibrary.ResourceId);
                parameters.Add("@LibraryID", MediaLibraryID);
                parameters.Add("@SiteID", LibraryInfo.LibrarySiteID);

                // Exclude generic roles from matrix
                string where = "(RoleName NOT IN ('_authenticated_', '_everyone_', '_notauthenticated_')) AND ";

                if (LibraryInfo.LibraryGroupID > 0)
                {
                    where += "RoleGroupID=" + LibraryInfo.LibraryGroupID.ToString();
                }
                else
                {
                    where += "RoleGroupID IS NULL";
                }

                if (permissionArray != null)
                {
                    where += " AND PermissionName IN (";
                    foreach (string permission in permissionArray)
                    {
                        where += "'" + permission + "',";
                    }
                    where = where.TrimEnd(',');
                    where += ") ";
                }

                // Setup matrix control            
                gridMatrix.QueryParameters = parameters;
                gridMatrix.WhereCondition = where;
                gridMatrix.ContentBefore = "<table class=\"PermissionMatrix\" cellspacing=\"0\" cellpadding=\"0\" rules=\"rows\" border=\"1\" style=\"border-collapse:collapse;\">";
                gridMatrix.ContentAfter = "</table>";
                gridMatrix.OnItemChanged += new UniMatrix.ItemChangedEventHandler(gridMatrix_OnItemChanged);

                // Check 'Modify' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "manage"))
                {
                    Enable = false;
                    gridMatrix.Enabled = false;
                    ShowError(String.Format(GetString("CMSSiteManager.AccessDeniedOnPermissionName"), "Manage"));
                }
            }
        }
    }


    /// <summary>
    /// Generates the permission matrix for the current library.
    /// </summary>
    private void CreateMatrix()
    {
        // Get library resource info             
        if ((ResLibrary != null) && (LibraryInfo != null))
        {
            // Get permissions for the current library resource                       
            DataSet permissions = PermissionNameInfoProvider.GetResourcePermissions(ResLibrary.ResourceId);
            if (DataHelper.DataSourceIsEmpty(permissions))
            {
                lblInfo.ResourceString = "general.emptymatrix";
                lblInfo.Visible = true;
            }
            else
            {
                TableRow headerRow = new TableRow();
                headerRow.CssClass = "UniGridHead";
                TableCell newCell = new TableCell();
                TableHeaderCell newHeaderCell = new TableHeaderCell();
                newHeaderCell.Text = "&nbsp;";
                newHeaderCell.CssClass = "MatrixHeader";
                newHeaderCell.Attributes["style"] = "width:28%;";
                headerRow.Cells.Add(newHeaderCell);

                DataView dv = permissions.Tables[0].DefaultView;
                dv.Sort = "PermissionDisplayName ASC";

                // Generate header cells                
                foreach (DataRowView drv in dv)
                {
                    string permissionName = drv.Row["PermissionName"].ToString();
                    if (permissionArray.Contains(permissionName.ToLowerCSafe()))
                    {
                        newHeaderCell = new TableHeaderCell();
                        newHeaderCell.CssClass = "MatrixHeader";
                        newHeaderCell.Attributes["style"] = "width:12%;text-align:center;white-space:nowrap;";
                        newHeaderCell.Text = HTMLHelper.HTMLEncode(drv.Row["PermissionDisplayName"].ToString());
                        newHeaderCell.ToolTip = Convert.ToString(drv.Row["PermissionDescription"]);
                        newHeaderCell.HorizontalAlign = HorizontalAlign.Center;

                        headerRow.Cells.Add(newHeaderCell);
                    }
                }

                // Insert the empty cell at the end
                newHeaderCell = new TableHeaderCell();
                newHeaderCell.Text = "&nbsp;";
                headerRow.Cells.Add(newHeaderCell);
                tblMatrix.Rows.Add(headerRow);

                // Render library access permissions
                object[,] accessNames = new object[5,2];
                accessNames[0, 0] = GetString("security.nobody");
                accessNames[0, 1] = SecurityAccessEnum.Nobody;
                accessNames[1, 0] = GetString("security.allusers");
                accessNames[1, 1] = SecurityAccessEnum.AllUsers;
                accessNames[2, 0] = GetString("security.authenticated");
                accessNames[2, 1] = SecurityAccessEnum.AuthenticatedUsers;
                accessNames[3, 0] = GetString("security.groupmembers");
                accessNames[3, 1] = SecurityAccessEnum.GroupMembers;
                accessNames[4, 0] = GetString("security.authorizedroles");
                accessNames[4, 1] = SecurityAccessEnum.AuthorizedRoles;

                TableRow newRow = null;
                int rowIndex = 0;

                for (int access = 0; access <= accessNames.GetUpperBound(0); access++)
                {
                    SecurityAccessEnum currentAccess = ((SecurityAccessEnum)accessNames[access, 1]);
                    // If the security isn't displayed as part of group section
                    if (((currentAccess == SecurityAccessEnum.GroupAdmin) || (currentAccess == SecurityAccessEnum.GroupMembers)) && (!(LibraryInfo.LibraryGroupID > 0)))
                    {
                        // Do not render this access item
                    }
                    else
                    {
                        // Generate cell holding access item name
                        newRow = new TableRow();
                        newRow.CssClass = ((rowIndex % 2 == 0) ? "EvenRow" : "OddRow");
                        newCell = new TableCell();
                        newCell.CssClass = "MatrixHeader";
                        newCell.Text = accessNames[access, 0].ToString();
                        newCell.Wrap = false;
                        newRow.Cells.Add(newCell);
                        rowIndex++;

                        // Render the permissions access items
                        int permissionIndex = 0;
                        for (int permission = 0; permission < (tblMatrix.Rows[0].Cells.Count - 2); permission++)
                        {
                            newCell = new TableCell();
                            newCell.HorizontalAlign = HorizontalAlign.Center;
                            int accessEnum = Convert.ToInt32(accessNames[access, 1]);
                            // Check if the currently processed access is applied for permission
                            bool isAllowed = CheckPermissionAccess(accessEnum, permission, tblMatrix.Rows[0].Cells[permission + 1].Text);

                            // Disable column in roles grid if needed
                            if ((currentAccess == SecurityAccessEnum.AuthorizedRoles) && !isAllowed)
                            {
                                gridMatrix.DisableColumn(permissionIndex);
                            }

                            // Insert the radio button for the current permission
                            string permissionText = tblMatrix.Rows[0].Cells[permission + 1].Text;
                            string elemId = ClientID + "_" + permission + "_" + access;
                            newCell.Text = "<label style=\"display:none;\" for=\"" + elemId + "\">" + permissionText + "</label><input type=\"radio\" id=\"" + elemId + "\" name=\"" + permissionText + "\" " + (Enable ? "" : "disabled=\"disabled\"") + " onclick=\"" + Page.ClientScript.GetPostBackEventReference(this, permission + "|" + accessEnum) + "\" " + ((isAllowed) ? "checked = \"checked\"" : "") + "/>";

                            newCell.Wrap = false;
                            newRow.Cells.Add(newCell);
                            permissionIndex++;
                        }

                        newCell = new TableCell();
                        newCell.Text = "&nbsp;";
                        newRow.Cells.Add(newCell);
                        // Add the access row to the table
                        tblMatrix.Rows.Add(newRow);
                    }
                }

                // Get permission matrix for roles of the current site/group            
                mNoRolesAvailable = !gridMatrix.HasData;
                if (!mNoRolesAvailable)
                {
                    // Security - Role separator
                    newRow = new TableRow();
                    newCell = new TableCell();
                    newCell.Text = "&nbsp;";
                    newCell.Attributes.Add("colspan", Convert.ToString(tblMatrix.Rows[0].Cells.Count));
                    newRow.Controls.Add(newCell);
                    tblMatrix.Rows.Add(newRow);

                    // Security - Role separator text
                    newRow = new TableRow();
                    newCell = new TableCell();
                    newCell.CssClass = "MatrixLabel";
                    newCell.Text = GetString("SecurityMatrix.RolesAvailability");
                    newCell.Attributes.Add("colspan", Convert.ToString(tblMatrix.Rows[0].Cells.Count));
                    newRow.Controls.Add(newCell);
                    tblMatrix.Rows.Add(newRow);
                }
            }
        }
    }


    /// <summary>
    /// Indicates the permission access.
    /// </summary>
    /// <param name="currentAccess">Currently processed integer representation of item from SecurityAccessEnum</param>    
    /// <param name="currentPermission">Currently processed integer representation of permission to check</param>    
    private bool CheckPermissionAccess(int currentAccess, int currentPermission, string currentPermissionName)
    {
        bool result = false;

        if (LibraryInfo != null)
        {
            switch (currentPermission)
            {
                case 6:
                    result = ((int)LibraryInfo.Access == currentAccess);
                    break;

                case 0:
                    result = ((int)LibraryInfo.FileCreate == currentAccess);
                    break;

                case 2:
                    result = ((int)LibraryInfo.FileDelete == currentAccess);
                    break;

                case 4:
                    result = ((int)LibraryInfo.FileModify == currentAccess);
                    break;

                case 1:
                    result = ((int)LibraryInfo.FolderCreate == currentAccess);
                    break;

                case 3:
                    result = ((int)LibraryInfo.FolderDelete == currentAccess);
                    break;

                case 5:
                    result = ((int)LibraryInfo.FolderModify == currentAccess);
                    break;

                default:
                    break;
            }
        }

        // Make note about type of permission with access set to 'OnlyAuthorizedRoles'
        if (result && (currentAccess == 2))
        {
            onlyAuth[currentPermissionName] = true;
        }
        return result;
    }


    /// <summary>
    /// On item changed event.
    /// </summary>    
    private void gridMatrix_OnItemChanged(object sender, int roleId, int permissionId, bool allow)
    {
        // Check 'Modify' permission
        if (MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "manage"))
        {
            if (allow)
            {
                MediaLibraryRolePermissionInfoProvider.AddRoleToLibrary(roleId, MediaLibraryID, permissionId);
            }
            else
            {
                MediaLibraryRolePermissionInfoProvider.RemoveRoleFromLibrary(roleId, MediaLibraryID, permissionId);
            }
        }
    }


    #region "PostBack event handler"

    public void RaisePostBackEvent(string eventArgument)
    {
        // Check 'Modify' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "manage"))
        {
            return;
        }

        string[] args = eventArgument.Split('|');

        if (args.Length == 2)
        {
            // Get info on currently selected item
            int permission = Convert.ToInt32(args[0]);
            int access = Convert.ToInt32(args[1]);

            if (LibraryInfo != null)
            {
                // Update library permission access information
                switch (permission)
                {
                    case 0:
                        LibraryInfo.FileCreate = ((SecurityAccessEnum)access);
                        break;

                    case 2:
                        LibraryInfo.FileDelete = (SecurityAccessEnum)access;
                        break;

                    case 4:
                        LibraryInfo.FileModify = ((SecurityAccessEnum)access);
                        break;

                    case 1:
                        LibraryInfo.FolderCreate = (SecurityAccessEnum)access;
                        break;

                    case 3:
                        LibraryInfo.FolderDelete = (SecurityAccessEnum)access;
                        break;

                    case 5:
                        LibraryInfo.FolderModify = (SecurityAccessEnum)access;
                        break;

                    case 6:
                        LibraryInfo.Access = (SecurityAccessEnum)access;
                        break;

                    default:
                        break;
                }

                // Save changes to the library
                MediaLibraryInfoProvider.SetMediaLibraryInfo(LibraryInfo);
            }
        }
    }

    #endregion
}