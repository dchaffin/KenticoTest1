using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_NewFolder : CMSAdminControl
{
    #region "Delegates & Events"

    /// <summary>
    /// Delegate of event fired when cancel button is clicked.
    /// </summary>
    public delegate void OnCancelClickEventHandler();


    /// <summary>
    /// Delegate of event fired when folder has been deleted.
    /// </summary>
    public delegate void OnFolderChangeEventHandler(int nodeToSelect);


    /// <summary>
    /// Event raised when cancel button is clicked.
    /// </summary>
    public event OnCancelClickEventHandler CancelClick;


    /// <summary>
    /// Event raised when folder has been deleted.
    /// </summary>
    public event OnFolderChangeEventHandler OnFolderChange;

    #endregion


    #region "Private variables"

    private int mParentNodeId = 0;
    private string mParentCulture = null;

    private TreeNode mParentNode = null;
    private TreeProvider mTreeProvider = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets tree provider object for current user.
    /// </summary>
    private TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider(CMSContext.CurrentUser));
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
    /// ID of the parent of the newly created node.
    /// </summary>
    public int ParentNodeID
    {
        get
        {
            return mParentNodeId;
        }
        set
        {
            mParentNodeId = value;
        }
    }


    /// <summary>
    /// Culture of the parent of the newly created node.
    /// </summary>
    public string ParentCulture
    {
        get
        {
            return mParentCulture ?? (mParentCulture = CMSContext.PreferredCultureCode);
        }
        set
        {
            mParentCulture = value;
        }
    }


    /// <summary>
    /// Gets parent node object.
    /// </summary>
    public TreeNode ParentNode
    {
        get
        {
            if ((mParentNode == null) && (mParentNodeId > 0))
            {
                mParentNode = TreeProvider.SelectSingleNode(ParentNodeID, ParentCulture);
            }
            return mParentNode;
        }
        set
        {
            mParentNode = value;
        }
    }

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            // Initialize control
            SetupControl();
        }
        else
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Clears form controls content.
    /// </summary>
    public override void ClearForm()
    {
        txtFolderName.Text = string.Empty;
    }


    #region "Event handlers"

    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Check permissions
        string errMsg = CheckNodePermissions();

        // If validation succeeded
        if (errMsg == string.Empty)
        {
            try
            {
                // Validate form entry
                errMsg = ValidateForm();

                if (errMsg == string.Empty)
                {
                    if (ParentNode != null)
                    {
                        // Initialize and create new folder node
                        TreeNode newFolder = TreeNode.New("CMS.folder", TreeProvider);

                        // Set properties
                        newFolder.DocumentName = txtFolderName.Text.Trim();
                        newFolder.DocumentCulture = ParentCulture;
                        newFolder.SetValue("NodeOwner", CMSContext.CurrentUser.UserID);

                        // Create new folder
                        DocumentHelper.InsertDocument(newFolder, ParentNode, TreeProvider);

                        // Reload parent content
                        if (OnFolderChange != null)
                        {
                            OnFolderChange(newFolder.NodeID);
                        }
                    }
                }
                else
                {
                    // Display an error to the user
                    ShowError(errMsg);
                }
            }
            catch (Exception ex)
            {
                // Display an error to the user
                ShowError(GetString("general.erroroccurred") + " " + ex.Message);
            }
        }
        else
        {
            // Display an error to the user
            ShowError(errMsg);
        }
    }


    /// <summary>
    /// Checks user's permissions for the parent node.
    /// </summary>
    private string CheckNodePermissions()
    {
        string errMsg = string.Empty;

        if (ParentNodeID > 0)
        {
            if (ParentNode != null)
            {
                SiteInfo si = SiteInfoProvider.GetSiteInfo(ParentNode.NodeSiteID);
                if (si != null)
                {
                    // Check permissions and allowed document type
                    DataClassInfo folderClass = DataClassInfoProvider.GetDataClass("CMS.Folder");
                    if (folderClass != null)
                    {
                        // Check allowed document type
                        if (!DataClassInfoProvider.IsChildClassAllowed(ValidationHelper.GetInteger(ParentNode.GetValue("NodeClassID"), 0), folderClass.ClassID))
                        {
                            errMsg = GetString("dialogs.document.classnotallowed");
                        }

                        // Check document permissions
                        if (!CMSContext.CurrentUser.IsAuthorizedToCreateNewDocument(ParentNode, folderClass.ClassName))
                        {
                            errMsg = GetString("dialogs.document.NotAuthorizedToCreate");
                        }
                    }
                    else
                    {
                        errMsg = GetString("dialogs.document.folderclassnotfound");
                    }
                }
                else
                {
                    errMsg = GetString("dialogs.document.sitemissing");
                }
            }
            else
            {
                errMsg = GetString("dialogs.document.parentmissing");
            }
        }
        else
        {
            errMsg = GetString("dialogs.document.parentmissing");
        }

        return errMsg;
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // Let the parent control now about user's action
        if (CancelClick != null)
        {
            CancelClick();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the control.
    /// </summary>
    private void SetupControl()
    {
        // Setup labels' and buttons' text
        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.cancel");

        // Register for events
        btnCancel.Click += btnCancel_Click;
        rfvFolderName.ErrorMessage = GetString("media.folder.foldernameempty");
        ltlScript.Text = GetFocusScript();
    }


    /// <summary>
    /// Validates form entries.
    /// </summary>    
    private string ValidateForm()
    {
        return new Validator().NotEmpty(txtFolderName.Text, GetString("dialogs.folder.foldernameempty")).Result;
    }


    /// <summary>
    /// Returns script for focus folder name textbox.
    /// </summary>
    private string GetFocusScript()
    {
        string script = "function FocusFolderName(){\n" +
                        "var txtBox = document.getElementById('" + txtFolderName.ClientID + "');\n" +
                        "if (txtBox != null) { \n" +
                        "   try {\n" +
                        "       txtBox.focus();\n" +
                        "   } catch (e) {\n" +
                        "       setTimeout('FocusFolderName()',50);\n" +
                        "   }\n" +
                        "}\n" +
                        "}\n";

        return ScriptHelper.GetScript(script);
    }

    #endregion
}