using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.DocumentEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_FormControls_Documents_SelectDocument : FormEngineUserControl
{
    #region "Variables"

    private bool mEnableSiteSelection = false;
    private DialogConfiguration mConfig = null;
    private TreeProvider mTreeProvider = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            if (FieldInfo.DataType == FormFieldDataTypeEnum.Text)
            {
                return txtName.Text;
            }

            return base.ValueDisplayName;
        }
    }


    /// <summary>
    /// Returns TreeProvider object.
    /// </summary>
    private TreeProvider TreeProvider
    {
        get
        {
            if (mTreeProvider == null)
            {
                mTreeProvider = new TreeProvider(CMSContext.CurrentUser);
            }
            return mTreeProvider;
        }
    }


    /// <summary>
    /// Gets the configuration for Copy and Move dialog.
    /// </summary>
    private DialogConfiguration Config
    {
        get
        {
            if (mConfig == null)
            {
                mConfig = new DialogConfiguration();
                mConfig.HideLibraries = true;
                mConfig.HideAnchor = true;
                mConfig.HideAttachments = true;
                mConfig.HideContent = false;
                mConfig.HideEmail = true;
                mConfig.HideLibraries = true;
                mConfig.HideWeb = true;
                mConfig.EditorClientID = txtGuid.ClientID;
                mConfig.ContentSelectedSite = CMSContext.CurrentSiteName;
                mConfig.OutputFormat = OutputFormatEnum.Custom;
                mConfig.CustomFormatCode = "selectpath";
                mConfig.SelectableContent = SelectableContentEnum.AllContent;
            }
            return mConfig;
        }
    }

    #endregion


    #region "Public properties"

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
            txtName.Enabled = value;
            btnSelect.Enabled = value;
            btnClear.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtGuid.Text.Trim();
        }
        set
        {
            if (value != null)
            {
                txtGuid.Text = value.ToString();
                SetAliasPath(value, FieldIsInteger());
            }
        }
    }


    /// <summary>
    /// Gets ClientID of the textbox with path.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtName.ClientID;
        }
    }


    /// <summary>
    /// Determines whether to enable site selection or not.
    /// </summary>
    public bool EnableSiteSelection
    {
        get
        {
            return mEnableSiteSelection;
        }
        set
        {
            mEnableSiteSelection = value;
            Config.ContentSites = (value ? AvailableSitesEnum.All : AvailableSitesEnum.OnlyCurrentSite);
        }
    }


    /// <summary>
    /// Gets or sets the content starting path.
    /// </summary>
    public string ContentStartingPath
    {
        get
        {
            return Config.ContentStartingPath;
        }
        set
        {
            Config.ContentStartingPath = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Hide guid textbox
        txtGuid.Attributes.Add("style", "display: none");

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ClearDocument", ScriptHelper.GetScript("function ClearDocument(txtClientID, hiddenClientId){ document.getElementById(txtClientID).value = ''; document.getElementById(hiddenClientId).value=''; }"));

        btnSelect.Text = GetString("general.select");
        btnSelect.OnClientClick = "modalDialog('" + GetDialogUrl() + "','PathSelection', '90%', '85%'); return false;";

        btnClear.Text = GetString("FormControls_SelectDocument.btnClear");
        btnClear.OnClientClick = "ClearDocument('" + txtName.ClientID + "', '" + txtGuid.ClientID + "'); return false;";

        txtName.Attributes.Add("readonly", "readonly");
        txtGuid.TextChanged += new EventHandler(txtGuid_TextChanged);
    }


    /// <summary>
    /// Determines whether parent form field is integer.
    /// </summary>
    /// <returns>TRUE if parent form field is integer.</returns>
    private bool FieldIsInteger()
    {
        return ((FieldInfo != null) && ((FieldInfo.DataType == FormFieldDataTypeEnum.Integer) || (FieldInfo.DataType == FormFieldDataTypeEnum.LongInteger)));
    }


    private void txtGuid_TextChanged(object sender, EventArgs e)
    {
        int nodeId = ValidationHelper.GetInteger(txtGuid.Text, 0);

        if (FieldIsInteger())
        {
            txtName.Text = GetNodeName(nodeId);
            return;
        }

        if (nodeId > 0)
        {
            TreeNode node = TreeProvider.SelectSingleNode(nodeId, TreeProvider.ALL_CULTURES, true);
            if (node != null)
            {
                string site = (node.NodeSiteID != CMSContext.CurrentSiteID ? ";" + node.NodeSiteName : "");
                txtName.Text = node.NodeAliasPath;
                txtGuid.Text = node.NodeGUID.ToString() + site;
            }
        }
    }


    /// <summary>
    /// Clears selected value.
    /// </summary>
    public void Clear()
    {
        txtGuid.Text = String.Empty;
        txtName.Text = String.Empty;
    }

    #endregion


    #region "Private methods"

    private void SetAliasPath(object valueObj, bool isNodeId)
    {
        if (valueObj != null)
        {
            if (!isNodeId)
            {
                string[] split = valueObj.ToString().Split(';');
                string siteName = null;
                // Check if site name is presented in value
                if (split.Length > 1)
                {
                    siteName = split[1];
                }
                else
                {
                    if ((Form != null) && (Form.EditedObject != null))
                    {
                        TreeNode editedNode = Form.EditedObject as TreeNode;
                        if (editedNode != null)
                        {
                            int linkedSiteId = editedNode.OriginalNodeSiteID;
                            if ((linkedSiteId > 0) && (linkedSiteId != CMSContext.CurrentSiteID))
                            {
                                siteName = SiteInfoProvider.GetSiteName(linkedSiteId);
                            }
                        }
                    }
                }

                if (ValidationHelper.GetGuid(split[0], Guid.Empty) != Guid.Empty)
                {
                    txtName.Text = GetNodeName(split[0], siteName);
                }
            }
            else
            {
                txtName.Text = GetNodeName(ValidationHelper.GetInteger(valueObj, 0));
            }
        }
    }


    /// <summary>
    /// Returns Correct URL of the copy or move dialog.
    /// </summary>
    private string GetDialogUrl()
    {
        string url = CMSDialogHelper.GetDialogUrl(Config, IsLiveSite, false, null, false);

        url = URLHelper.RemoveParameterFromUrl(url, "hash");
        url = URLHelper.AddParameterToUrl(url, "selectionmode", "single");
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));
        url = URLHelper.EncodeQueryString(url);
        return url;
    }


    /// <summary>
    /// Gets node name from guid.
    /// </summary>
    /// <param name="objGuid">Guid object</param>
    /// <param name="siteName">Site name</param>
    private string GetNodeName(string objGuid, string siteName)
    {
        Guid nodeGuid = ValidationHelper.GetGuid(objGuid, Guid.Empty);
        if (nodeGuid != Guid.Empty)
        {
            if (String.IsNullOrEmpty(siteName))
            {
                siteName = CMSContext.CurrentSiteName;
            }
            TreeNode node = TreeProvider.SelectSingleNode(nodeGuid, TreeProvider.ALL_CULTURES, siteName);
            if (node != null)
            {
                return node.NodeAliasPath;
            }
        }

        return "";
    }


    /// <summary>
    /// Gets node name from node ID.
    /// </summary>
    /// <param name="nodeId">Node ID</param>
    private string GetNodeName(int nodeId)
    {
        if (nodeId > 0)
        {
            TreeNode node = TreeProvider.SelectSingleNode(nodeId, TreeProvider.ALL_CULTURES, true);
            if (node != null)
            {
                return node.NodeAliasPath;
            }
        }
        return "";
    }


    /// <summary>
    /// Returns WHERE condition for selected form.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Return correct WHERE condition for integer if none value is selected
        if ((FieldInfo != null) && ((FieldInfo.DataType == FormFieldDataTypeEnum.Integer) || (FieldInfo.DataType == FormFieldDataTypeEnum.LongInteger)))
        {
            int id = ValidationHelper.GetInteger(Value, 0);
            if (id > 0)
            {
                return base.GetWhereCondition();
            }
        }
        return null;
    }

    #endregion
}