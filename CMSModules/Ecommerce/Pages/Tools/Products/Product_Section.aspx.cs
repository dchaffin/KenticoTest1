using System;
using System.Web;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;

public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Section : CMSProductsPage
{
    #region "Constants"

    private const string VALIDATION_EXCLUDED_CLASS_NAMES = ";cms.file;";

    #endregion


    #region "Private & protected variables"

    protected string viewpage = "~/CMSPages/blank.htm";
    private int nodeId = 0;
    private TreeNode mTreeNode = null;
    private DataClassInfo classInfo = null;

    #endregion


    #region "Private properties"

    private TreeNode TreeNode
    {
        get
        {
            return mTreeNode;
        }
        set
        {
            mTreeNode = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Do not check changes
        DocumentManager.RegisterSaveChangesScript = false;

        // Do not redirect when document does not exist
        DocumentManager.RedirectForNonExistingDocument = false;

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script files
        ScriptHelper.RegisterScriptFile(this, @"~/CMSModules/Content/CMSDesk/ContentEditFrameset.js");

        bool checkCulture = false;
        bool splitViewSupported = false;
        nodeId = QueryHelper.GetInteger("nodeid", 0);
        string action = QueryHelper.GetString("action", "edit").ToLowerCSafe();

        switch (action)
        {
            // New dialog / new page form
            case "new":
                int classId = QueryHelper.GetInteger("classid", 0);
                if (classId <= 0)
                {
                    // Get by class name if specified
                    string className = QueryHelper.GetString("classname", string.Empty);
                    if (className != string.Empty)
                    {
                        classInfo = DataClassInfoProvider.GetDataClass(className);
                        if (classInfo != null)
                        {
                            classId = classInfo.ClassID;
                        }
                    }
                }
                
                nodeId = QueryHelper.GetInteger("parentnodeid", 0);

                if (classId > 0)
                {
                    viewpage = ResolveUrl("~/CMSModules/Content/CMSDesk/Edit/Edit.aspx");

                    // Check if document type is allowed under parent node
                    if (nodeId > 0)
                    {
                        // Get the node
                        TreeNode = Tree.SelectSingleNode(nodeId, TreeProvider.ALL_CULTURES);
                        if (TreeNode != null)
                        {
                            if (!DataClassInfoProvider.IsChildClassAllowed(ValidationHelper.GetInteger(TreeNode.GetValue("NodeClassID"), 0), classId))
                            {
                                viewpage = "~/CMSModules/Content/CMSDesk/NotAllowed.aspx?action=child";
                            }
                        }
                    }

                    // Use product page when product type is selected
                    classInfo = classInfo ?? DataClassInfoProvider.GetDataClass(classId);
                    if ((classInfo != null) && (classInfo.ClassIsProduct))
                    {
                        viewpage = ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx");
                    }
                }
                else
                {
                    if (nodeId > 0)
                    {
                        viewpage = "~/CMSModules/Ecommerce/Pages/Tools/Products/New_ProductOrSection.aspx";
                    }
                    else
                    {
                        viewpage = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx?parentNodeId=0";
                    }
                }
                break;

            case "delete":
                // Delete dialog
                viewpage = "~/CMSModules/Content/CMSDesk/Delete.aspx";
                break;

            default:
                // Edit mode
                viewpage = "~/CMSModules/Content/CMSDesk/Edit/edit.aspx?mode=editform";
                splitViewSupported = true;

                // Ensure class info
                if ((classInfo == null) && (Node != null))
                {
                    classInfo = DataClassInfoProvider.GetDataClass(Node.NodeClassName);
                }

                // Check explicit editing page url
                if ((classInfo != null) && !string.IsNullOrEmpty(classInfo.ClassEditingPageURL))
                {
                    viewpage = URLHelper.AppendQuery(ResolveUrl(classInfo.ClassEditingPageURL), URLHelper.Url.Query);
                }

                checkCulture = true;
                break;
        }

        // If culture version should be checked, check
        if (checkCulture)
        {
            // Check (and ensure) the proper content culture
            CheckPreferredCulture(true);

            // Check split mode 
            bool isSplitMode = CMSContext.DisplaySplitMode;
            bool combineWithDefaultCulture = !isSplitMode && SiteInfoProvider.CombineWithDefaultCulture(CMSContext.CurrentSiteName);

            TreeNode = Tree.SelectSingleNode(nodeId, CultureCode, combineWithDefaultCulture);
            if (TreeNode == null)
            {
                // Document does not exist -> redirect to new culture version creation dialog
                viewpage = "~/CMSModules/Content/CMSDesk/New/NewCultureVersion.aspx";
            }
        }

        // Apply the additional transformations to the view page URL
        viewpage = URLHelper.AppendQuery(viewpage, URLHelper.Url.Query);
        viewpage = URLHelper.RemoveParameterFromUrl(viewpage, "mode");
        viewpage = URLHelper.AddParameterToUrl(viewpage, "mode", "productssection");
        viewpage = ResolveUrl(viewpage);
        viewpage = URLHelper.AddParameterToUrl(viewpage, "hash", QueryHelper.GetHash(viewpage));

        // Split mode enabled
        if (splitViewSupported && CMSContext.DisplaySplitMode && (TreeNode != null) && (action == "edit" || action == "preview" || (TreeNode.IsPublished && action == "livesite")))
        {
            viewpage = GetSplitViewUrl(viewpage);
        }
    }


    #endregion
}
