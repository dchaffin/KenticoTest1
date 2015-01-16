using System;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Pages_Tools_Products_Products_Frameset : CMSProductsPage
{
    #region "Variables"

    private int? mResultNodeID = null;
    private int? mResultDocumentID = null;
    private string mResultDevice = null;

    #endregion


    #region "Properties"

    private int SelectedNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(Request.Params["selectedNodeId"], 0);
        }
    }


    private int ExpandNodeID
    {
        get
        {
            return QueryHelper.GetInteger("expandnodeid", 0);
        }
    }


    private int SelectedDocumentID
    {
        get
        {
            return ValidationHelper.GetInteger(Request.Params["selectedDocId"], 0);
        }
    }


    private string SelectedCulture
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedCulture"], CMSContext.CurrentUser.PreferredCultureCode);
        }
    }


    private string SelectedDevice
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedDevice"], null);
        }
    }


    private TreeNode RootNode
    {
        get
        {
            // Root
            string baseDoc = "/";
            if (ProductsStartingPath != String.Empty)
            {
                // Change to user's root page
                baseDoc = ProductsStartingPath;
            }
            // Try to get culture-specific root node
            TreeNode rootNode = Tree.SelectSingleNode(CMSContext.CurrentSiteName, baseDoc, CMSContext.CurrentUser.PreferredCultureCode, false, null, false);
            if (rootNode == null)
            {
                // Get root node
                rootNode = Tree.SelectSingleNode(CMSContext.CurrentSiteName, baseDoc, TreeProvider.ALL_CULTURES, false, null, false);
            }
            return rootNode;
        }
    }


    protected int ResultNodeID
    {
        get
        {
            if (mResultNodeID == null)
            {
                // Get ID from query string
                mResultNodeID = NodeID;
                if (mResultNodeID <= 0)
                {
                    // Get ID selected by user
                    mResultNodeID = SelectedNodeID;
                    if (mResultNodeID <= 0)
                    {
                        // If no node specified, add the root node id
                        if (NodeID <= 0)
                        {
                            TreeNode rootNode = RootNode;
                            if (rootNode != null)
                            {
                                mResultNodeID = rootNode.NodeID;
                            }
                        }
                    }
                }
            }
            return mResultNodeID.Value;
        }
    }


    protected int ResultDocumentID
    {
        get
        {
            if (mResultDocumentID == null)
            {
                // Get ID from query string
                mResultDocumentID = DocumentID;
                if (mResultDocumentID <= 0)
                {
                    // Get ID selected by user
                    mResultDocumentID = SelectedDocumentID;
                    if ((mResultDocumentID <= 0) && (NodeID <= 0))
                    {
                        TreeNode rootNode = RootNode;
                        // If the culture match with selected culture
                        if ((rootNode != null) && rootNode.DocumentCulture.EqualsCSafe(SelectedCulture))
                        {
                            // Get identifier from the root node
                            mResultDocumentID = rootNode.DocumentID;
                        }
                    }
                }
            }
            return mResultDocumentID.Value;
        }
    }


    /// <summary>
    /// Resulting device. Prefers user choice over query string setting.
    /// </summary>
    protected string ResultDevice
    {
        get
        {
            if (mResultDevice == null)
            {
                mResultDevice = SelectedDevice;
                if (mResultDevice == null)
                {
                    mResultDevice = Device;
                    if (mResultDevice == null)
                    {
                        mResultDevice = CMSContext.CurrentDeviceProfileName;
                    }
                }
            }
            return mResultDevice;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Do not include document manager to the controls collection
        EnsureDocumentManager = false;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        contenttree.Values.AddRange(new[] { new UILayoutValue("NodeID", ResultNodeID), new UILayoutValue("ExpandNodeID", ExpandNodeID), new UILayoutValue("Culture", SelectedCulture) });

        string contentUrl = "Product_List.aspx" + URLHelper.Url.Query;

        if (NodeID <= 0)
        {
            // Root
            string baseDoc = "/";
            if (!string.IsNullOrEmpty(ProductsStartingPath))
            {
                // Change to products root node
                baseDoc = ProductsStartingPath.TrimEnd('/');
            }

            // Get the root node
            TreeNode rootNode = Tree.SelectSingleNode(CMSContext.CurrentSiteName, baseDoc, TreeProvider.ALL_CULTURES, false, null, false);
            if (rootNode != null)
            {
                string nodeString = rootNode.NodeID.ToString();
                contentUrl = URLHelper.AddParameterToUrl(contentUrl, "nodeId", nodeString);
            }
        }

        contentview.Src = contentUrl;

        AddMenuButtonSelectScript("Products", string.Empty);
        ScriptHelper.RegisterScriptFile(Page, @"~/CMSModules/Content/CMSDesk/Content.js");

        // Override content SetMode function, force page mode when in edit mode
        AddScript(@"
function SetMode(mode, passive) {
    if (!CheckChanges()) {
        return false;
    }
    if (mode == 'edit') mode = 'page';
    SetSelectedMode(mode);
    if (!passive) {
        DisplayDocument();
    }
    return true;
}
");
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), script.GetHashCode().ToString(), ScriptHelper.GetScript(script));
    }

    #endregion
}
