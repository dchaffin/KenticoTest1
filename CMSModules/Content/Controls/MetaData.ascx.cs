using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.FormControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_Controls_MetaData : CMSUserControl
{
    #region "Protected variables"

    protected bool hasModifyPermission = true;
    protected string siteName = null;
    protected bool mEnabled = true;
    private string mUIModuleName = "CMS.Content";
    private string mUIPageElementName = "MetaData.Page";
    private string mUITagsElementName = "MetaData.Tags";

    TreeNode node = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of the edited node.
    /// </summary>
    public int NodeID
    {
        get
        {
            return DocumentManager.NodeID;
        }
        set
        {
            DocumentManager.NodeID = value;
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
        }
    }


    /// <summary>
    /// Indicates whether the asynchronous post back occurs on the page.
    /// </summary>
    private bool IsAsyncPostback
    {
        get
        {
            return ScriptManager.GetCurrent(Page).IsInAsyncPostBack;
        }
    }


    /// <summary>
    /// Name of the module for UI personalization checks. 'CMS.Content' by default.
    /// </summary>
    public string UIModuleName
    {
        get
        {
            return mUIModuleName;
        }
        set
        {
            mUIModuleName = value;
        }
    }


    /// <summary>
    /// Name of the UI element affecting 'Page' section of metadata form. Default value is 'MetaData.Page'.
    /// </summary>
    public string UIPageElementName
    {
        get
        {
            return mUIPageElementName;
        }
        set
        {
            mUIPageElementName = value;
        }
    }


    /// <summary>
    /// Name of the UI element affecting 'Tags' section of metadata form. Default value is 'MetaData.Tags'.
    /// </summary>
    public string UITagsElementName
    {
        get
        {
            return mUITagsElementName;
        }
        set
        {
            mUITagsElementName = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Disable tag selectors
        tagGroupSelectorElem.StopProcessing = pnlUITags.IsHidden;
        tagSelectorElem.StopProcessing = pnlUITags.IsHidden;

        // Redirect to information page when no UI elements displayed
        if (pnlUIPage.IsHidden && pnlUITags.IsHidden)
        {
            RedirectToUINotAvailable();
        }

        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
        DocumentManager.OnValidateData += DocumentManager_OnValidateData;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        UIContext.PropertyTab = PropertyTabEnum.Metadata;

        // Register the scripts
        ScriptHelper.RegisterProgress(Page);

        // Initialize tag group selector
        tagGroupSelectorElem.AddNoneItemsRecord = false;
        tagGroupSelectorElem.UseAutoPostback = true;
        tagGroupSelectorElem.UseGroupNameForSelection = false;
        tagSelectorElem.IsLiveSite = false;

        // Init node
        node = DocumentManager.Node;

        if (node != null)
        {
            // Get site info of current document
            SiteInfo si = SiteInfoProvider.GetSiteInfo(node.NodeSiteID);
            if (si != null)
            {
                siteName = si.SiteName;
            }
        }

        SetCheckBoxes();

        // Get strings for labels
        lblPageDescription.Text = GetString("PageProperties.Decription");
        lblPageKeywords.Text = GetString("PageProperties.Keywords");
        lblPageTitle.Text = GetString("PageProperties.Title");
        lblTagSelector.Text = GetString("PageProperties.Tags");
        lblTagGroupSelector.Text = GetString("PageProperties.TagGroup");

        // Get string for checkboxes
        chkDescription.Text = GetString("Metadata.Inherit");
        chkKeyWords.Text = GetString("Metadata.Inherit");
        chkTitle.Text = GetString("Metadata.Inherit");
        chkTagGroupSelector.Text = GetString("Metadata.Inherit");

        // Get titles for panels
        pnlPageSettings.GroupingText = GetString("content.metadata.pagesettings");
        pnlTags.GroupingText = GetString("content.metadata.tags");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Enable form
        bool enable = hasModifyPermission && Enabled;
        pnlForm.Enabled = enable;
        tagSelectorElem.Enabled = enable;

        if (!IsAsyncPostback)
        {
            ReloadData();
        }

        if (!pnlUITags.IsHidden)
        {
            if (!tagGroupSelectorElem.UniSelector.HasData)
            {
                // Hide tag controls
                lblTagGroupSelector.Visible = false;
                tagGroupSelectorElem.Visible = false;
                chkTagGroupSelector.Visible = false;
                lblTagSelector.Visible = false;
                tagSelectorElem.Visible = false;

                // Show tag info
                lblTagInfo.Text = GetString("PageProperties.TagsInfo");
                lblTagInfo.Visible = true;
            }
            else
            {
                // Set the tag selector
                int tagGroup = ValidationHelper.GetInteger(tagGroupSelectorElem.Value, 0);
                if (tagGroup <= 0)
                {
                    tagSelectorElem.Enabled = false;
                    tagSelectorElem.Value = "";
                }
                else
                {
                    tagSelectorElem.SetValue("GroupID", tagGroup);
                }
            }
        }
    }

    #endregion

    
    #region "Control handling"

    void DocumentManager_OnValidateData(object sender, DocumentManagerEventArgs e)
    {
        e.IsValid = !pnlUIPage.IsHidden || !pnlUITags.IsHidden;
    }


    protected void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        // Update the data
        if (!pnlUIPage.IsHidden)
        {
            // Set Page title property
            node.SetValue("DocumentPageTitle", null);
            if (!chkTitle.Checked)
            {
                node.SetValue("DocumentPageTitle", txtTitle.Text);
            }

            // Set Page key words property
            node.SetValue("DocumentPageKeyWords", null);
            if (!chkKeyWords.Checked)
            {
                node.SetValue("DocumentPageKeyWords", txtKeywords.Text);
            }

            // Set Page description property
            node.SetValue("DocumentPageDescription", null);
            if (!chkDescription.Checked)
            {
                node.SetValue("DocumentPageDescription", txtDescription.Text);
            }
        }

        // Update the tags related data
        if (!pnlUITags.IsHidden)
        {
            node.SetValue("DocumentTagGroupID", null);
            if (!chkTagGroupSelector.Checked)
            {
                // If is tag group id
                node.SetValue("DocumentTagGroupID", (ValidationHelper.GetInteger(tagGroupSelectorElem.Value, 0) > 0) ? tagGroupSelectorElem.Value : DBNull.Value);
            }

            node.SetValue("DocumentTags", (ValidationHelper.GetInteger(tagGroupSelectorElem.Value, 0) > 0) ? tagSelectorElem.Value : DBNull.Value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles setting of module name and UI element name for UIPlaceholders according to UIModuleName, UIPageElementName and UITagsElementName properties.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    protected void pnlUI_OnBeforeCheck(object sender, EventArgs e)
    {
        UIPlaceHolder placeholder = sender as UIPlaceHolder;

        if (placeholder != null)
        {
            // Set module name
            placeholder.ModuleName = UIModuleName;

            // Set element names
            if (placeholder == pnlUIPage)
            {
                placeholder.ElementName = UIPageElementName;
            }
            else if (placeholder == pnlUITags)
            {
                placeholder.ElementName = UITagsElementName;
            }
        }
    }


    protected void SetCheckBoxes()
    {
        if (RequestHelper.IsPostBack())
        {
            return;
        }

        // Hide 'inherit' checkboxes for root document
        if ((node != null) && (node.NodeAliasPath == "/"))
        {
            chkDescription.Visible = false;
            chkKeyWords.Visible = false;
            chkTitle.Visible = false;
            chkTagGroupSelector.Visible = false;
        }
    }


    private void ReloadData()
    {
        if (node != null)
        {
            bool isRoot = (node.NodeAliasPath == "/");

            // Check read permissions
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
            {
                RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), node.NodeAliasPath));
            }
            // Check modify permissions
            else if (CMSContext.CurrentUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                hasModifyPermission = false;
                pnlForm.Enabled = false;
                tagSelectorElem.Enabled = false;
            }

            TreeNode tmpNode = node.Clone();

            // If values are inherited set nulls
            tmpNode.SetValue("DocumentPageTitle", DBNull.Value);
            tmpNode.SetValue("DocumentPageKeyWords", DBNull.Value);
            tmpNode.SetValue("DocumentPageDescription", DBNull.Value);
            tmpNode.SetValue("DocumentTagGroupID", DBNull.Value);

            // Load the inherited values
            SiteInfo si = SiteInfoProvider.GetSiteInfo(node.NodeSiteID);
            if (si != null)
            {
                tmpNode.LoadInheritedValues(new string[] { "DocumentPageTitle", "DocumentPageKeyWords", "DocumentPageDescription", "DocumentTagGroupID" }, SiteInfoProvider.CombineWithDefaultCulture(si.SiteName));
            }

            if (!pnlUIPage.IsHidden)
            {
                // Page title
                if (node.GetValue("DocumentPageTitle") != null)
                {
                    txtTitle.Text = node.GetValue("DocumentPageTitle").ToString();
                }
                else
                {
                    if (!isRoot)
                    {
                        txtTitle.Enabled = false;
                        chkTitle.Checked = true;
                        txtTitle.Text = ValidationHelper.GetString(tmpNode.GetValue("DocumentPageTitle"), "");
                    }
                }

                // Page key words
                if (node.GetValue("DocumentPageKeyWords") != null)
                {
                    txtKeywords.Text = node.GetValue("DocumentPageKeyWords").ToString();
                }
                else
                {
                    if (!isRoot)
                    {
                        txtKeywords.Enabled = false;
                        chkKeyWords.Checked = true;
                        txtKeywords.Text = ValidationHelper.GetString(tmpNode.GetValue("DocumentPageKeyWords"), "");
                    }
                }

                // Page description
                if (node.GetValue("DocumentPageDescription") != null)
                {
                    txtDescription.Text = node.GetValue("DocumentPageDescription").ToString();
                }
                else
                {
                    if (!isRoot)
                    {
                        txtDescription.Enabled = false;
                        chkDescription.Checked = true;
                        txtDescription.Text = ValidationHelper.GetString(tmpNode.GetValue("DocumentPageDescription"), "");
                    }
                }
            }

            if (!pnlUITags.IsHidden)
            {
                // Tag group
                if (node.GetValue("DocumentTagGroupID") != null)
                {
                    object tagGroupId = node.GetValue("DocumentTagGroupID");
                    tagGroupSelectorElem.Value = tagGroupId;
                }
                else
                {
                    if (!isRoot)
                    {
                        // Get the inherited tag group
                        int tagGroup = ValidationHelper.GetInteger(tmpNode.GetValue("DocumentTagGroupID"), 0);
                        if (tagGroup > 0)
                        {
                            tagGroupSelectorElem.Value = tagGroup;
                        }
                        else
                        {
                            tagGroupSelectorElem.AddNoneItemsRecord = true;
                        }
                        tagGroupSelectorElem.Enabled = false;
                        chkTagGroupSelector.Checked = true;
                    }
                    else
                    {
                        // Add 'none' option to Root document
                        tagGroupSelectorElem.AddNoneItemsRecord = true;
                    }
                }

                // Tags
                tagSelectorElem.Value = node.DocumentTags;
            }
        }
    }

    #endregion


    #region "Event handling"

    protected void chkTitle_CheckedChanged(object sender, EventArgs e)
    {
        if (chkTitle.Checked)
        {
            // Value is inherited
            txtTitle.Enabled = false;
            if (!String.IsNullOrEmpty(siteName))
            {
                txtTitle.Text = ValidationHelper.GetString(node.GetInheritedValue("DocumentPageTitle", SiteInfoProvider.CombineWithDefaultCulture(siteName)), "");
            }
        }
        else
        {
            // Textbox is enabled
            txtTitle.Enabled = true;
        }
    }


    protected void chkDescription_CheckedChanged(object sender, EventArgs e)
    {
        if (chkDescription.Checked)
        {
            // Value is inherited
            txtDescription.Enabled = false;
            if (!String.IsNullOrEmpty(siteName))
            {
                txtDescription.Text = ValidationHelper.GetString(node.GetInheritedValue("DocumentPageDescription", SiteInfoProvider.CombineWithDefaultCulture(siteName)), "");
            }
        }
        else
        {
            // Text area is enabled
            txtDescription.Enabled = true;
        }
    }


    protected void chkKeyWords_CheckedChanged(object sender, EventArgs e)
    {
        if (chkKeyWords.Checked)
        {
            // Value is inherited
            txtKeywords.Enabled = false;
            if (!String.IsNullOrEmpty(siteName))
            {
                txtKeywords.Text = ValidationHelper.GetString(node.GetInheritedValue("DocumentPageKeyWords", SiteInfoProvider.CombineWithDefaultCulture(siteName)), "");
            }
        }
        else
        {
            // Textbox is enabled
            txtKeywords.Enabled = true;
        }
    }


    protected void chkTagGroupSelector_CheckedChanged(object sender, EventArgs e)
    {
        if (chkTagGroupSelector.Checked)
        {
            // Value is inherited
            tagGroupSelectorElem.Enabled = false;

            if (!String.IsNullOrEmpty(siteName))
            {
                // Load parent value to tag group selector
                int value = ValidationHelper.GetInteger(node.GetInheritedValue("DocumentTagGroupID", SiteInfoProvider.CombineWithDefaultCulture(siteName)), 0);
                if (value == 0)
                {
                    tagGroupSelectorElem.AddNoneItemsRecord = true;
                }
                tagGroupSelectorElem.Value = value;
                tagGroupSelectorElem.ReloadData();
            }
        }
        else
        {
            tagGroupSelectorElem.Enabled = true;
        }
    }

    #endregion
}
