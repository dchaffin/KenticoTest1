using System;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_New : CMSProductPropertiesPage
{
    #region "Variables"

    protected int newObjectSiteId = -1;
    private OptionCategoryInfo mOptionCategory = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the value of the 'parentNodeId' URL parameter.
    /// </summary>
    public int ParentNodeID
    {
        get
        {
            return QueryHelper.GetInteger("parentNodeId", 0);
        }
    }


    /// <summary>
    /// Gets the value of the 'classId' URL parameter.
    /// </summary>
    public int DataClassID
    {
        get
        {
            return QueryHelper.GetInteger("classId", 0);
        }
    }


    /// <summary>
    /// Gets the value of the 'categoryId' URL parameter.
    /// </summary>
    public int OptionCategoryID
    {
        get
        {
            return QueryHelper.GetInteger("categoryId", 0);
        }
    }


    /// <summary>
    /// Gets a product option category object specified by the OptionCategoryID property.
    /// </summary>
    public OptionCategoryInfo OptionCategory
    {
        get
        {
            return mOptionCategory ?? (mOptionCategory = OptionCategoryInfoProvider.GetOptionCategoryInfo(OptionCategoryID));
        }
    }


    /// <summary>
    /// Gets the value of the 'content' URL parameter.
    /// </summary>
    public bool IsContent
    {
        get
        {
            return QueryHelper.GetBoolean("content", false);
        }
    }


    /// <summary>
    /// Gets a value that indicates if the document type selection should be displayed.
    /// </summary>
    private bool DisplayTypeSelection
    {
        get
        {
            return (ParentNodeID > 0) && (DataClassID == 0);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnInit(EventArgs e)
    {
        ScriptHelper.RegisterProgress(Page);
        ScriptHelper.RegisterScriptFile(Page, "cmsedit.js");

        base.OnInit(e);
        IsProductOption = OptionCategoryID > 0;

        newObjectSiteId = ConfiguredSiteID;

        if (IsProductOption)
        {
            // Initialize creation of a new product option
            GlobalObjectsKeyName = ECommerceSettings.ALLOW_GLOBAL_PRODUCT_OPTIONS;
            if (OptionCategory != null)
            {
                // A new product option wil be bound to the same site as the option category
                CheckEditedObjectSiteID(OptionCategory.CategorySiteID);
                newObjectSiteId = OptionCategory.CategorySiteID;
            }
        }
        else
        {
            // Initialize creation of a new product
            GlobalObjectsKeyName = ECommerceSettings.ALLOW_GLOBAL_PRODUCTS;
        }

        if (DisplayTypeSelection)
        {
            InitDocumentTypeSelection();
            DocumentManager.StopProcessing = true;
            productEditElem.StopProcessing = true;
        }
        else
        {
            InitProductEdit();
            docTypeElem.StopProcessing = true;
        }

        InitMasterPage();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "InitializePage", ScriptHelper.GetScript("InitializePage();"));
    }

    #endregion


    #region "Initialization"

    /// <summary>
    /// Initializes the document type selection.
    /// </summary>
    private void InitDocumentTypeSelection()
    {
        docTypeElem.ParentNodeID = ParentNodeID;
        docTypeElem.Caption = GetString("com.product.selectdoctype");
        docTypeElem.AllowNewABTest = false;
        docTypeElem.AllowNewLink = false;
        docTypeElem.RedirectWhenNoChoice = true;
        docTypeElem.SelectionUrl = "Product_New.aspx";
        docTypeElem.Where = "ClassIsProduct = 1";
        docTypeElem.NoDataMessage = GetString("com.products.nodocumenttypeallowed");
        plcDocTypeSelection.Visible = true;
    }


    /// <summary>
    /// Initializes the product edit control.
    /// </summary>
    private void InitProductEdit()
    {
        CurrentMaster.PanelContent.CssClass = null;

        plcProductEdit.Visible = true;
        productEditElem.ProductSaved += (sender, args) =>
        {
            if (IsContent)
            {
                // If not EditLive view mode, switch to form mode to keep editing the form
                if (CMSContext.ViewMode != ViewModeEnum.EditLive)
                {
                    CMSContext.ViewMode = ViewModeEnum.EditForm;
                }
            }
            else
            {
                UIContext.ProductTab = ProductTabEnum.General;
            }

            bool refreshEcommerceTree = (ECommerceSettings.ProductsTree(CMSContext.CurrentSiteName) == ProductsTreeModeEnum.Sections)
                && (ECommerceSettings.DisplayProductsInSectionsTree(CMSContext.CurrentSiteName));

            if (productEditElem.Product is TreeNode && (IsContent || refreshEcommerceTree))
            {
                var node = (TreeNode)productEditElem.Product;
                if (productEditElem.ProductSavedCreateAnother)
                {
                    ScriptHelper.RefreshTree(this, node.NodeParentID, node.NodeParentID);
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "Refresh", ScriptHelper.GetScript("CreateAnother();"));
                }
                else
                {
                    ScriptHelper.RefreshTree(this, node.NodeParentID, node.NodeID);
                }
            }
            else
            {
                if (productEditElem.ProductSavedCreateAnother)
                {
                    RedirectToNewProduct();
                }
                else
                {
                    RedirectToSavedProduct(productEditElem.Product);
                }
            }
        };
    }


    /// <summary>
    /// Initializes the master page.
    /// </summary>
    private void InitMasterPage()
    {
        string titleText = "";
        string titleImage = "";
        string productListText = "";
        string productListUrl = "";
        string productText = "";

        // Initialize creation of new product option
        if (OptionCategoryID > 0)
        {
            productText = GetString("product_new.newproductoption");
            productListText = GetString("product_new.productoptionslink");

            productListUrl = "~/CMSModules/Ecommerce/Pages/Tools/ProductOptions/OptionCategory_Edit_Options.aspx";
            productListUrl = URLHelper.AddParameterToUrl(productListUrl, "siteId", SiteID.ToString());
            productListUrl = URLHelper.AddParameterToUrl(productListUrl, "categoryId", OptionCategoryID.ToString());

            AddMenuButtonSelectScript("ProductOptions", "");

            CurrentMaster.Title.HelpTopicName = "CMS_Ecommerce_New_Product_Option";
            CurrentMaster.Title.HelpName = "helpTopic";
        }
        // Initialize creation of new product
        else
        {
            titleImage = GetImageUrl((newObjectSiteId > 0) ? "Objects/Ecommerce_SKU/new.png" : "Objects/Ecommerce_SKU/newglobal.png");
            if (ParentNodeID > 0)
            {
                titleText = GetString((newObjectSiteId > 0) ? "com_sku_edit_general.newitemcaption" : "com.product.newglobal");
            }
            else
            {
                titleText = GetString((newObjectSiteId > 0) ? "com.sku.newsku" : "com.sku.newglobalsku");
            }
            productText = titleText;
            productListText = GetString(((ParentNodeID > 0) || !DisplayTreeInProducts) ? "com.sku.productslist" : "com.sku.unassignedlist");

            productListUrl = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_List.aspx";
            productListUrl = URLHelper.AddParameterToUrl(productListUrl, "siteId", SiteID.ToString());
            productListUrl = AddNodeIDParameterToUrl(productListUrl);

            AddMenuButtonSelectScript("Products", "");

            CurrentMaster.Title.HelpTopicName = "new_product";
            CurrentMaster.Title.HelpName = "helpTopic";
        }

        // Prepare global title for product options
        if (OptionCategoryID > 0)
        {
            productText = FormatBreadcrumbObjectName(productText, newObjectSiteId);
        }

        // Create breadcrumbs when creating product document
        if (ParentNodeID > 0)
        {
            // Register script for selecting nodes from breadcrumbs
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "selectItem", ScriptHelper.GetScript(@"function SelectItem(nodeId, parentNodeId, documentId) { if (nodeId != 0) { parent.SelectNode(nodeId); parent.RefreshTree(parentNodeId, nodeId); }}"));

            TreeNode parentNode = DocumentHelper.GetDocument(ParentNodeID, TreeProvider.ALL_CULTURES, Tree);
            CurrentMaster.Title.Breadcrumbs = CreateNodeBreadcrumbs(parentNode, productText);
        }
        else
        {
            // Set master page breadcrumbs		
            string[,] breadcrumbs = new string[2, 3];

            breadcrumbs[0, 0] = productListText;
            breadcrumbs[0, 1] = productListUrl;
            breadcrumbs[1, 0] = productText;

            CurrentMaster.Title.Breadcrumbs = breadcrumbs;
        }

        CurrentMaster.Title.TitleText = titleText;
        CurrentMaster.Title.TitleImage = titleImage;
    }

    #endregion


    #region "Redirection"

    /// <summary>
    /// Redirects to the new product page.
    /// </summary>
    private void RedirectToNewProduct()
    {
        string url = "Product_New.aspx";
        url = URLHelper.AddParameterToUrl(url, "siteId", SiteID.ToString());
        url = URLHelper.AddParameterToUrl(url, "categoryId", OptionCategoryID.ToString());
        url = URLHelper.AddParameterToUrl(url, "parentNodeId", ParentNodeID.ToString());
        url = URLHelper.AddParameterToUrl(url, "classId", DataClassID.ToString());
        url = URLHelper.AddParameterToUrl(url, "saved", "1");
        URLHelper.Redirect(url);
    }


    /// <summary>
    /// Redirects to the edit page of the saved product.
    /// </summary>
    private void RedirectToSavedProduct(BaseInfo product)
    {
        string url = "Product_Edit_Frameset.aspx";
        url = URLHelper.AddParameterToUrl(url, "siteId", SiteID.ToString());
        url = URLHelper.AddParameterToUrl(url, "categoryId", OptionCategoryID.ToString());
        if (product is TreeNode)
        {
            int nodeId = product.GetIntegerValue("NodeID", 0);
            url = URLHelper.AddParameterToUrl(url, "nodeId", nodeId.ToString());
        }
        else if (product is SKUInfo)
        {
            int skuId = product.GetIntegerValue("SKUID", 0);
            url = URLHelper.AddParameterToUrl(url, "productId", skuId.ToString());
        }
        url = URLHelper.AddParameterToUrl(url, "saved", "1");
        URLHelper.Redirect(url);
    }

    #endregion
}
