using System;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;

using TreeElemNode = System.Web.UI.WebControls.TreeNode;

public partial class CMSModules_Content_CMSDesk_Properties_Advanced_EditableContent_Tree : CMSModalPage
{
    #region "Private & protected variables"

    private string selectedNodeType = null;
    private string selectedNodeName = null;

    #endregion


    #region "Properties"

    public override CMS.ExtendedControls.MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        DocumentManager.LocalMessagesPlaceHolder = plcMess;
        DocumentManager.OnSaveData += DocumentManager_OnSaveData;

        CurrentUserInfo user = CMSContext.CurrentUser;

        // Check 'read' permissions
        if (!user.IsAuthorizedPerResource("CMS.Content", "Read"))
        {
            RedirectToAccessDenied("CMS.Content", "Read");
        }

        // Check UIProfile
        if (!user.IsAuthorizedPerUIElement("CMS.Content", "Properties.General"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties.General");
        }

        if (!user.IsAuthorizedPerUIElement("CMS.Content", "General.Advanced"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "General.Advanced");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.FrameResizer.Visible = false;

        // Get query parameters
        selectedNodeType = QueryHelper.GetString("selectednodetype", "webpart");
        selectedNodeName = QueryHelper.GetString("selectednodename", string.Empty);
        hdnCurrentNodeType.Value = selectedNodeType;
        hdnCurrentNodeName.Value = selectedNodeName;

        // Images
        if (hdnCurrentNodeType.Value == "region")
        {
            imgNewItem.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditableContent/addeditableitemsmall.png");
            imgNewItem.DisabledImageUrl = GetImageUrl("CMSModules/CMS_Content/EditableContent/addeditableitemsmalldisabled.png");
        }
        else
        {
            imgNewItem.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditableContent/editablewebpart.png");
            imgNewItem.DisabledImageUrl = GetImageUrl("CMSModules/CMS_Content/EditableContent/editablewebpartdisabled.png");
        }
        imgDeleteItem.ImageUrl = GetImageUrl("Objects/CMS_WebPart/delete.png");
        imgDeleteItem.DisabledImageUrl = GetImageUrl("Objects/CMS_WebPart/deletedisabled.png");

        bool enabled = DocumentManager.AllowSave;
        string selectNodeName = QueryHelper.GetString("selectednodename", null);
        if (!string.IsNullOrEmpty(selectedNodeName))
        {
            pnlDeleteItem.Enabled = enabled;
            if (enabled)
            {
                lnkDeleteItem.CssClass = "NewItemLink";
                lnkDeleteItem.Attributes.Add("onclick", "return DeleteItem();");
            }
        }
        else
        {
            pnlDeleteItem.Enabled = false;
        }

        if (enabled)
        {
            lnkNewItem.CssClass = "NewItemLink";
            lnkNewItem.Attributes.Add("onclick", "return CreateNew();");
            pnlNewItem.Enabled = true;
        }
        else
        {
            lnkNewItem.CssClass = "NewItemLinkDisabled";
            pnlNewItem.Enabled = false;
        }

        // Resource strings
        lnkDeleteItem.Text = GetString("Development-WebPart_Tree.DeleteItem");
        lnkNewItem.Text = GetString("Development-WebPart_Tree.NewWebPart");

        string imageUrl = "Design/Controls/Tree";

        // Initialize page
        if (CultureHelper.IsUICultureRTL())
        {
            imageUrl = GetImageUrl("RTL/" + imageUrl, false, false);
        }
        else
        {
            imageUrl = GetImageUrl(imageUrl, false, false);
        }
        webpartsTree.LineImagesFolder = imageUrl;
        regionsTree.LineImagesFolder = imageUrl;

        if (Node != null)
        {
            string webpartRootAttributes = "class=\"ContentTreeItem\"";
            string regionRootAttributes = "class=\"ContentTreeItem\"";

            if (string.IsNullOrEmpty(selectedNodeName))
            {
                switch (selectedNodeType)
                {
                    case "webpart":
                        webpartRootAttributes = "class=\"ContentTreeSelectedItem\" id=\"treeSelectedNode\"";
                        regionRootAttributes = "class=\"ContentTreeItem\" ";
                        break;

                    case "region":
                        webpartRootAttributes = "class=\"ContentTreeItem\" ";
                        regionRootAttributes = "class=\"ContentTreeSelectedItem\" id=\"treeSelectedNode\"";
                        break;
                    default:
                        webpartRootAttributes = "class=\"ContentTreeSelectedItem\" id=\"treeSelectedNode\"";
                        regionRootAttributes = "class=\"ContentTreeItem\" ";
                        break;
                }
            }
            else
            {
                webpartRootAttributes = "class=\"ContentTreeSelectedItem\" id=\"treeSelectedNode\"";
                regionRootAttributes = "class=\"ContentTreeItem\" ";
            }

            // Create tree menus
            TreeElemNode rootWebpartNode = new TreeElemNode();
            rootWebpartNode.Text = "<span " + webpartRootAttributes + " onclick=\"SelectNode('','webpart', this);\"><span class=\"Name\">" + GetString("EditableWebparts.Root") + "</span></span>";
            rootWebpartNode.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditableContent/editablewebparts.png");
            rootWebpartNode.Expanded = true;
            rootWebpartNode.NavigateUrl = "#";

            TreeElemNode rootRegionNode = new TreeElemNode();
            rootRegionNode.Text = "<span " + regionRootAttributes + " onclick=\"SelectNode('','region', this);\"><span class=\"Name\">" + GetString("EditableRegions.Root") + "</span></span>";
            rootRegionNode.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditableContent/editableregions.png");
            rootRegionNode.Expanded = true;
            rootRegionNode.NavigateUrl = "#";

            // Editable web parts
            webpartsTree.Nodes.Add(rootWebpartNode);
            if (Node.DocumentContent.EditableWebParts.Count > 0)
            {
                foreach (DictionaryEntry webPart in Node.DocumentContent.EditableWebParts)
                {
                    string key = webPart.Key.ToString();
                    string name = DictionaryHelper.GetFirstKey(key);
                    AddNode(rootWebpartNode, name, "webpart");
                }
            }

            // Editable regions
            regionsTree.Nodes.Add(rootRegionNode);
            if (Node.DocumentContent.EditableRegions.Count > 0)
            {
                foreach (DictionaryEntry region in Node.DocumentContent.EditableRegions)
                {
                    string key = region.Key.ToString();
                    string name = DictionaryHelper.GetFirstKey(key);
                    AddNode(rootRegionNode, name, "region");
                }
            }
        }

        // Delete item if requested from query string
        string nodeType = QueryHelper.GetString("nodetype", null);
        string nodeName = QueryHelper.GetString("nodename", null);
        if (!RequestHelper.IsPostBack() && !String.IsNullOrEmpty(nodeType) && QueryHelper.GetBoolean("deleteItem", false))
        {
            DeleteItem(nodeType, nodeName);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Adds node to parent node.
    /// </summary>
    /// <param name="parentNode">Parent node</param>
    /// <param name="nodeName">Name of node</param>
    /// <param name="nodeKey">Node key</param>
    /// <param name="nodeType">Type of node</param>
    private void AddNode(TreeElemNode parentNode, string nodeName, string nodeType)
    {
        TreeElemNode newNode = new TreeElemNode();
        string cssClass = "ContentTreeItem";
        string elemId = string.Empty;

        // Select proper node
        if (selectedNodeName == nodeName)
        {
            if (webpartsTree.Nodes.Count > 0)
            {
                webpartsTree.Nodes[0].Text = webpartsTree.Nodes[0].Text.Replace("ContentTreeSelectedItem", "ContentTreeItem").Replace("treeSelectedNode", string.Empty);
            }
            else if (regionsTree.Nodes.Count > 0)
            {
                regionsTree.Nodes[0].Text = regionsTree.Nodes[0].Text.Replace("ContentTreeSelectedItem", "ContentTreeItem").Replace("treeSelectedNode", string.Empty);
            }
            if (nodeType == selectedNodeType)
            {
                cssClass = "ContentTreeSelectedItem";
                elemId = "id=\"treeSelectedNode\"";
            }
        }
        newNode.Text = "<span class=\"" + cssClass + "\" " + elemId + " onclick=\"SelectNode(" + ScriptHelper.GetString(nodeName) + ", " + ScriptHelper.GetString(nodeType) + ", this);\"><span class=\"Name\">" + HTMLHelper.HTMLEncode(nodeName) + "</span></span>";
        newNode.NavigateUrl = "#";
        parentNode.ChildNodes.Add(newNode);
    }


    /// <summary>
    /// Deletes item.
    /// </summary>
    protected void DeleteItem(string nodeType, string nodeName)
    {
        if (DocumentManager.AllowSave)
        {
            // Remove key from hashtable
            switch (nodeType)
            {
                case "webpart":
                    Node.DocumentContent.EditableWebParts.Remove(nodeName);
                    break;

                case "region":
                    Node.DocumentContent.EditableRegions.Remove(nodeName);
                    break;
            }

            // Save node
            SaveNode();

            // Refresh
            ltlScript.Text += ScriptHelper.GetScript("document.location.replace('" + ResolveUrl("~/CMSModules/Content/CMSDesk/Properties/Advanced/EditableContent/tree.aspx") + "?nodeid=" + Node.NodeID + "&selectednodetype=" + nodeType + "'); parent.frames['main'].SelectNode('','" + nodeType + "');");
        }
        else
        {
            ltlScript.Text += ScriptHelper.GetAlertScript(string.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), Node.NodeAliasPath));
        }
    }


    /// <summary>
    /// Saves node, ensures workflow.
    /// </summary>
    protected void SaveNode()
    {
        // Save content
        DocumentManager.SaveDocument();
    }


    void DocumentManager_OnSaveData(object sender, CMS.ExtendedControls.DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        // Get content
        string content = node.DocumentContent.GetContentXml();
        node.UpdateOriginalValues();
        node.SetValue("DocumentContent", content);
    }

    #endregion
}