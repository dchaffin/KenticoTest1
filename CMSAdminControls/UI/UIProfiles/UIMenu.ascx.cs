using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UIProfiles_UIMenu : CMSUserControl
{
    #region "Variables"

    private string mRootTargetURL = String.Empty;

    private bool mModuleAvailabilityForSiteRequired = false;
    private bool mDisplayRootIcon = false;

    private UIElementInfo root = null;

    private int totalNodes = 0;
    private int mMaxRelativeLevel = -1;
    private bool mEnableRootSelect = true;

    private string mSelectedNode = String.Empty;
    private string mTargetFrame = "frameMain";

    #endregion


    #region "Properties"

    /// <summary>
    /// Default target frame for urls
    /// </summary>
    public string TargetFrame
    {
        get
        {
            return mTargetFrame;
        }
        set
        {
            mTargetFrame = value;
        }
    }


    /// <summary>
    /// Gets the root node of the tree.
    /// </summary>
    public TreeNode RootNode
    {
        get
        {
            return treeElem.CustomRootNode;
        }
    }


    /// <summary>
    /// Indicates if the root element is clickable.
    /// </summary>
    public bool EnableRootSelect
    {
        get
        {
            return mEnableRootSelect;
        }
        set
        {
            mEnableRootSelect = value;
        }
    }


    /// <summary>
    /// Code name of the UIElement.
    /// </summary>
    public string ElementName
    {
        get;
        set;
    }


    /// <summary>
    /// Code name of the module.
    /// </summary>
    public string ModuleName
    {
        get;
        set;
    }


    /// <summary>
    /// Query parameter name for the selection of the item.
    /// </summary>
    public string QueryParameterName
    {
        get;
        set;
    }


    /// <summary>
    /// Get selected node code name.
    /// </summary>
    private string SelectedNode
    {
        get
        {
            if (String.IsNullOrEmpty(mSelectedNode))
            {
                mSelectedNode = QueryHelper.GetString(QueryParameterName, String.Empty);
                if (mSelectedNode.StartsWithCSafe("cms.", true))
                {
                    mSelectedNode = mSelectedNode.Substring(4);
                }

                treeElem.SelectedItem = mSelectedNode;
            }
            return mSelectedNode.ToLowerCSafe();
        }
        set
        {
            treeElem.SelectedItem = SelectedNode;
            mSelectedNode = value;
        }
    }


    /// <summary>
    /// Indicates if all nodes should be expanded.
    /// </summary>
    public bool ExpandAll
    {
        get
        {
            return treeElem.ExpandAll;
        }
        set
        {
            treeElem.ExpandAll = value;
        }
    }


    /// <summary>
    /// Indicates number of expanded levels.
    /// </summary>
    public int ExpandLevel
    {
        get
        {
            return treeElem.ExpandLevel;
        }
        set
        {
            treeElem.ExpandLevel = value;
        }
    }


    /// <summary>
    /// Gets the value which indicates whether there is some tab displayed or not.
    /// </summary>
    public bool MenuEmpty
    {
        get
        {
            if (treeElem.CustomRootNode != null)
            {
                return (treeElem.CustomRootNode.ChildNodes.Count == 0);
            }
            return true;
        }
    }


    /// <summary>
    /// Root node target URL.
    /// </summary>
    public string RootTargetURL
    {
        get
        {
            return mRootTargetURL;
        }
        set
        {
            mRootTargetURL = value;
        }
    }


    /// <summary>
    /// Indicates if site availability of the corresponding module (module with name in format "cms.[ElementName]") is required for each UI element in the menu. Takes effect only when corresponding module exists.
    /// </summary>
    public bool ModuleAvailabilityForSiteRequired
    {
        get
        {
            return mModuleAvailabilityForSiteRequired;
        }
        set
        {
            mModuleAvailabilityForSiteRequired = value;
        }
    }


    /// <summary>
    /// Gets or sets maximal relative level displayed (depth of the tree to load).
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return mMaxRelativeLevel;
        }
        set
        {
            mMaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Indicates if the icon should be displayed in the root of the tree.
    /// </summary>
    public bool DisplayRootIcon
    {
        get
        {
            return mDisplayRootIcon;
        }
        set
        {
            mDisplayRootIcon = value;
        }
    }


    /// <summary>
    /// Add editing icon in development mode
    /// </summary>
    private string devIcon
    {
        get
        {
            if (SettingsKeyProvider.DevelopmentMode && CMSContext.CurrentUser.IsGlobalAdministrator)
            {
                ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(ModuleName);
                if (ri != null)
                {
                    return UIHelper.GetResourceUIElementsLink(Page, ri.ResourceId);
                }
            }

            return String.Empty;
        }
    }

    #endregion


    #region "Custom events"

    /// <summary>
    /// Node created delegate.
    /// </summary>
    public delegate TreeNode NodeCreatedEventHandler(UIElementInfo uiElement, TreeNode defaultNode);


    /// <summary>
    /// Node created event handler.
    /// </summary>
    public event NodeCreatedEventHandler OnNodeCreated;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        SetRootNode();

        SetTreeProvider();

        HandleThePreselection();

        treeElem.UsePostBack = false;
        treeElem.OnGetImage += treeElem_OnGetImage;
        treeElem.OnNodeCreated += treeElem_OnNodeCreated;

        SetNodeTemplate();

        treeElem.ReloadData();


        RegisterJavascriptFroHighlighting();
        RegisterJavascriptForPreselection();

        LogTheSecurity();
    }

    #endregion


    #region "Custom methods"

    /// <summary>
    /// Set path to preselected item
    /// </summary>
    private void HandleThePreselection()
    {
        if (!String.IsNullOrEmpty(SelectedNode))
        {
            UIElementInfo uiElement = UIElementInfoProvider.GetUIElementInfo(ModuleName, SelectedNode);
            if (uiElement != null)
            {
                treeElem.ExpandPath = uiElement.ElementIDPath;
            }
        }
    }


    /// <summary>
    /// Set tree provider
    /// </summary>
    protected void SetTreeProvider()
    {
        string levelWhere = (MaxRelativeLevel <= 0 ? "" : " AND (ElementLevel <= " + (root.ElementLevel + MaxRelativeLevel) + ")");
        string levelColumn = "CASE ElementLevel WHEN " + (root.ElementLevel + MaxRelativeLevel) + " THEN 0 ELSE ElementChildCount END AS ElementChildCount";

        UniTreeProvider provider = new UniTreeProvider();
        provider.RootLevelOffset = root.ElementLevel;
        provider.ObjectType = "cms.uielement";
        provider.DisplayNameColumn = "ElementDisplayName";
        provider.IDColumn = "ElementID";
        provider.LevelColumn = "ElementLevel";
        provider.OrderColumn = "ElementOrder";
        provider.ParentIDColumn = "ElementParentID";
        provider.PathColumn = "ElementIDPath";
        provider.ValueColumn = "ElementName";
        provider.ChildCountColumn = "ElementChildCount";
        provider.WhereCondition = "((ElementLevel = 0) OR ((ElementCaption IS NOT NULL) AND NOT (ElementCaption = '')))" + levelWhere;
        provider.Columns = "ElementID, ElementName, ElementDisplayName, ElementLevel, ElementOrder, ElementParentID, ElementIDPath, ElementCaption, ElementIconPath, ElementTargetURL, ElementResourceID, " + levelColumn;

        treeElem.ProviderObject = provider;
    }


    /// <summary>
    /// Set default template for nodes
    /// </summary>
    protected void SetNodeTemplate()
    {
        treeElem.SelectedNodeTemplate = "<span id=\"node_##NODECODENAME##\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        treeElem.NodeTemplate = "<span id=\"node_##NODECODENAME##\" name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
    }


    protected void SetRootNode()
    {
        if (String.IsNullOrEmpty(ElementName))
        {
            // Get the root UI element
            root = UIElementInfoProvider.GetRootUIElementInfo(ModuleName);
        }
        else
        {
            // Get the specified element
            root = UIElementInfoProvider.GetUIElementInfo(ModuleName, ElementName);
        }

        string codeName = root.ElementName.Replace(".", String.Empty).ToLowerCSafe();
        string caption = ResHelper.LocalizeString(!String.IsNullOrEmpty(root.ElementCaption) ? root.ElementCaption : root.ElementDisplayName);
        string rootIcon = DisplayRootIcon ? GetImageUrl(root.ElementIconPath) : "";
        string rootUrl = !String.IsNullOrEmpty(RootTargetURL) ? GetUrl(RootTargetURL) : GetUrl(root.ElementTargetURL);
        string rootText = devIcon + caption;

        if (EnableRootSelect)
        {
            rootText = String.Format("{0}<a href=\"{1}\" target=\"{2}\"><span id=\"node_{3}\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\"><span class=\"Name\">{4}</span></span></a>", devIcon, rootUrl, TargetFrame, codeName, caption);
            treeElem.SetRoot(rootText, root.ElementID.ToString(), rootIcon, rootUrl, TargetFrame);
            treeElem.EnableRootAction = true;
        }
        else
        {
            treeElem.SetRoot(rootText, root.ElementID.ToString(), rootIcon);
        }

        treeElem.ExpandPath = root.ElementIDPath;
    }


    /// <summary>
    /// Logs security info
    /// </summary>
    protected void LogTheSecurity()
    {
        DataRow sdr = SecurityHelper.ReserveSecurityLogItem("LoadUIMenu");

        if (sdr != null)
        {
            SecurityHelper.SetLogItemData(sdr, CMSContext.CurrentUser.UserName, ModuleName, ElementName, totalNodes, CMSContext.CurrentSiteName);
        }
    }


    /// <summary>
    /// Register JS for highlighting nodes after click
    /// </summary>
    protected void RegisterJavascriptFroHighlighting()
    {
        string script = @"
function SelectNode(node){
    $j('.ContentTreeSelectedItem').removeClass('ContentTreeSelectedItem').addClass('ContentTreeItem');
    $j(node).addClass('ContentTreeSelectedItem').removeClass('ContentTreeItem');    
}
$j(document).on('click', 'span[name=treeNode]', function(){
    SelectNode(this);
});";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "UIMenu_SelectItem", script, true);
    }


    /// <summary>
    /// Register JS for preselection node
    /// </summary>
    protected void RegisterJavascriptForPreselection()
    {
        if (!String.IsNullOrEmpty(SelectedNode))
        {
            string script = @"
$j(document).ready(SelectPreselectedNode('#node_" + SelectedNode.Replace(".", String.Empty) + @"'));
                    
function SelectPreselectedNode(node){
    var targetFrame = $j(node).parent().attr('target');
    var targetUrl = $j(node).parent().attr('href');

    parent.frames[targetFrame].location.href = targetUrl;

    SelectNode(node);
}";

            ScriptHelper.RegisterStartupScript(this, typeof(string), "UIMenu_PreSelectItem", script, true);
        }
    }


    /// <summary>
    /// Get completly resolved url (resolve macros, hash, url)
    /// </summary>
    private string GetUrl(string url)
    {
        url = CMSContext.ResolveMacros(url);
        url = URLHelper.EnsureHashToQueryParameters(url);
        url = URLHelper.ResolveUrl(url);

        return url;
    }

    #endregion


    #region "On Events"

    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        if (itemData != null)
        {
            UIElementInfo uiElement = new UIElementInfo(itemData);
            if (CMSContext.CurrentUser != null)
            {
                // Check permissions
                if (CMSContext.CurrentUser.IsAuthorizedPerUIElement(ModuleName, uiElement.ElementName, ModuleAvailabilityForSiteRequired))
                {
                    // Raise the node created handler
                    if (OnNodeCreated != null)
                    {
                        defaultNode = OnNodeCreated(uiElement, defaultNode);
                    }

                    // If url is empty -> don't show
                    if (defaultNode == null || String.IsNullOrEmpty(uiElement.ElementTargetURL))
                    {
                        return null;
                    }

                    // Prepare node info
                    string caption = !String.IsNullOrEmpty(uiElement.ElementCaption) ? uiElement.ElementCaption : uiElement.ElementDisplayName;
                    string targetFrame = !String.IsNullOrEmpty(defaultNode.Target) ? defaultNode.Target : TargetFrame;
                    string codeName = uiElement.ElementName.Replace(".", String.Empty).ToLowerCSafe();

                    // Set node
                    defaultNode.Target = targetFrame;
                    defaultNode.NavigateUrl = GetUrl(uiElement.ElementTargetURL);
                    defaultNode.Text = defaultNode.Text.Replace("##NODECODENAME##", codeName);

                    totalNodes++;

                    // Set first child node to be selected if root cant be selected
                    if (!EnableRootSelect && string.IsNullOrEmpty(SelectedNode))
                    {
                        // Is a page node (with page url)
                        if (uiElement.ElementTargetURL != "@")
                        {
                            SelectedNode = uiElement.ElementName;
                        }
                        else
                        {
                            // Try to display a child element
                            if (uiElement.ElementChildCount > 0)
                            {
                                defaultNode.Expanded = true;
                            }
                        }
                    }

                    // If url is '@' dont redirect, only collapse children
                    if (uiElement.ElementTargetURL == "@")
                    {
                        // Set text manualy, dont't use template                        
                        string imageUrl = uiElement.ElementIconPath;

                        // Get image URL
                        imageUrl = GetImagePath(imageUrl);

                        // Try to get default icon if requested icon not found
                        if (!FileHelper.FileExists(imageUrl))
                        {
                            imageUrl = GetImagePath("CMSModules/list.png");
                        }

                        // Onclick simulates click on '+' or '-' button
                        string onClick = "onClick=\"var js = $j(this).parents('tr').find('a').attr('href');eval(js); \"";

                        // Insert image manually - (some IE issues)
                        defaultNode.Text = "<a></a><table class=\"TreeNodeTable\" cellspacing=\"0\" cellpadding=\"0\"><tr><td><img src='" + UIHelper.ResolveImageUrl(imageUrl) + "'/></td><td><span id=\"node_" + uiElement.ElementName + "\" class=\"ContentTreeItem \"" + onClick + " ><span class=\"NodeName\">" + ResHelper.LocalizeString(caption) + "</span></span></td></tr></table>";
                    }

                    return defaultNode;
                }
            }
        }

        return null;
    }


    /// <summary>
    /// Gets image handler.
    /// </summary>
    /// <param name="node">Tree node</param>
    protected string treeElem_OnGetImage(UniTreeNode node)
    {
        UIElementInfo uiElement = new UIElementInfo((DataRow)node.ItemData);
        if (uiElement != null)
        {
            // Get icon path
            string imgUrl = uiElement.ElementIconPath;
            if (!String.IsNullOrEmpty(imgUrl))
            {
                if (!ValidationHelper.IsURL(imgUrl))
                {
                    imgUrl = UIHelper.GetImagePath(Page, imgUrl, false, false);

                    // Try to get default icon if requested icon not found
                    if (!FileHelper.FileExists(imgUrl))
                    {
                        imgUrl = GetImageUrl("CMSModules/list.png");
                    }
                }

                return UIHelper.ResolveImageUrl(imgUrl);
            }
        }

        return GetImageUrl("CMSModules/list.png");
    }

    #endregion
}