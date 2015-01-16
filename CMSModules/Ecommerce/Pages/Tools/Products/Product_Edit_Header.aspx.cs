using System;
using System.Data;
using System.Linq;

using CMS.Ecommerce;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.CMSHelper;

using CMS.Controls.Configuration;

public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Header : CMSProductPropertiesPage
{
    #region "Variables"

    protected string productName = string.Empty;
    protected int optionCategoryId = 0;
    protected int sectionId = -1;
    protected int siteId = 0;
    protected string selectedTab = string.Empty;
    protected int nodeId = 0;
    protected SKUInfo sku = null;

    private bool dialogMode = false;

    #endregion


    protected void Page_PreInit(object sender, EventArgs e)
    {
        optionCategoryId = QueryHelper.GetInteger("categoryid", 0);
        sectionId = QueryHelper.GetInteger("sectionId", -1);

        if (optionCategoryId > 0)
        {
            IsProductOption = true;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get product name ane option category ID
        if (ProductID > 0)
        {
            sku = SKUInfoProvider.GetSKUInfo(ProductID);
            if (sku != null)
            {
                productName = ResHelper.LocalizeString(sku.SKUName);
                optionCategoryId = sku.SKUOptionCategoryID;
                siteId = sku.SKUSiteID;

                // Check if edited object belongs to configured site
                CheckEditedObjectSiteID(siteId);
            }
        }

        dialogMode = QueryHelper.GetBoolean("dialogmode", false);
        nodeId = QueryHelper.GetInteger("nodeId", 0);

        // Display tabs separator
        CurrentMaster.PanelSeparator.Visible = true;

        AddMenuButtonSelectScript("Products", string.Empty);
    }


    protected override void OnLoad(EventArgs e)
    {
        // Handle callback
        var parameters = Request.Form["params"];
        if (parameters != null)
        {
            bool handled = false;
            switch (parameters.ToLowerCSafe())
            {
                // Select preview tab
                case "previewtab":
                    UIContext.ProductTab = ProductTabEnum.Preview;
                    handled = true;

                    break;

                // Select live site tab
                case "livesitetab":
                    UIContext.ProductTab = ProductTabEnum.LiveSite;
                    handled = true;

                    break;
            }

            if (handled)
            {
                Response.Clear();
                Response.End();
            }
        }

        base.OnLoad(e);

        string productList = string.Empty;
        string productListText = string.Empty;
        string targetFrame = string.Empty;
        string titleText = string.Empty;
        string titleImage = string.Empty;

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "selectTab", ScriptHelper.GetScript(@"function SetTab(tab){ $j.ajax({type: 'POST', data: 'params='+tab, context: document.body, dataType: 'text'});}"));

        // When editing product option
        if (optionCategoryId > 0)
        {
            productListText = GetString("Prodect_Edit_Header.ProductOptionsLink");
            productList = "~/CMSModules/Ecommerce/Pages/Tools/ProductOptions/OptionCategory_Edit_Options.aspx?categoryid=" + optionCategoryId + "&siteId=" + SiteID;
            targetFrame = "OptionCategoryEdit";
        }
        // When editing product
        else
        {
            productListText = GetString(((nodeId > 0) || !DisplayTreeInProducts) ? "com.sku.productslist" : "com.sku.unassignedlist");
            productList = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_List.aspx?siteId=" + SiteID;
            if (sectionId > 0)
            {
                productList = URLHelper.AddParameterToUrl(productList, "nodeId", sectionId.ToString());
            }
            targetFrame = "_parent";

            titleText = GetString((nodeId > 0) ? "Product_Edit_Header.HeaderCaption" : "com.sku.unassignedproperties");
            titleImage = GetImageUrl("Objects/Ecommerce_SKU/object.png");
        }

        int hideBreadcrumbs = QueryHelper.GetInteger("hidebreadcrumbs", 0);

        if ((hideBreadcrumbs == 0) && !dialogMode)
        {
            // initializes page title control		
            IntializeBreadcrumbs(productListText, productList, targetFrame, productName);
        }

        // Ensure page with changes saved message is loaded initially if required
        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            CurrentMaster.Tabs.StartPageURL = "Product_Edit_General.aspx" + URLHelper.Url.Query;
        }

        // Initialize the master page elements
        IntializeMasterPage(titleImage, titleText);
    }


    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    /// <param name="productListText">Text of the product list</param>
    /// <param name="productList">Product list</param>
    /// <param name="targetFrame">Name of the target frame</param>
    /// <param name="currentProductName">Name of the current product</param>
    private void IntializeBreadcrumbs(string productListText, string productList, string targetFrame, string currentProductName)
    {
        PageTitle masterTitle = CurrentMaster.Title;

        if (Node != null)
        {
            masterTitle.Breadcrumbs = CreateNodeBreadcrumbs();
        }
        else
        {
            string[,] breadcrumbs = new string[2, 3];

            // Create ordinary breadcrumbs when no node supplied
            breadcrumbs[0, 0] = productListText;
            breadcrumbs[0, 1] = (dialogMode) ? null : productList;
            breadcrumbs[0, 2] = targetFrame;
            breadcrumbs[1, 0] = FormatBreadcrumbObjectName(currentProductName, siteId);
            breadcrumbs[1, 1] = string.Empty;
            breadcrumbs[1, 2] = string.Empty;

            // Set the master page breadcrumb property
            masterTitle.Breadcrumbs = breadcrumbs;
        }
    }


    /// <summary>
    /// Returns script executed after the breadcrumb is clicked.
    /// </summary>
    /// <param name="dr">Datarow containing document data.</param>
    protected override string CreateNodeBreadcrumbScript(DataRow dr)
    {
        return string.Format("parent.parent.SelectNode({0}); parent.parent.RefreshTree({1}, {0}); return false;", dr["NodeID"], dr["NodeParentID"]);
    }


    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    /// <param name="titleImage">Title of the page title element</param>
    /// <param name="titleText">URL of the image of the page title element</param>
    private void IntializeMasterPage(string titleImage, string titleText)
    {
        CMSMasterPage master = (CMSMasterPage)CurrentMaster;

        selectedTab = DataHelper.GetNotEmpty(QueryHelper.GetString("tab", String.Empty).ToLowerCSafe(), ProductTabCode.ToString(UIContext.ProductTab).ToLowerCSafe());

        if (!dialogMode)
        {
            master.Title.TitleText = titleText;
            master.Title.TitleImage = titleImage;
            master.Title.HelpTopicName = "new_product";
            master.Title.HelpName = "helpTopic";
        }

        master.Tabs.OnTabCreated += Tabs_OnTabCreated;
        master.Tabs.UrlTarget = "ProductContent";
        master.Tabs.ModuleName = "CMS.Ecommerce";
        master.Tabs.ElementName = (optionCategoryId > 0) ? "ProductOptions.Options" : "Products.Properties";
    }


    protected TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        bool splitViewSupported = false;
        string lowerElementName = element.ElementName.ToLowerCSafe();
        UIPageURLSettings settings = null;
        switch (lowerElementName)
        {
            case "products.general":
            case "products.attachments":
            case "products.metadata":
            case "products.categories":
            case "products.workflow":
            case "products.versions":
                splitViewSupported = true;

                break;
        }

        switch (lowerElementName)
        {
            case "products.attachments":
            case "products.metadata":
            case "products.categories":
            case "products.workflow":
            case "products.versions":
            case "products.relatedproducts":
                // Check if editing product with its document
                if (NodeID <= 0)
                {
                    return null;
                }

                break;

            case "products.documents":
                if ((NodeID <= 0) && DisplayTreeInProducts)
                {
                    if (!CurrentUser.IsGlobalAdministrator || (sku == null) || !sku.IsGlobal)
                    {
                        return null;
                    }
                }

                break;

            case "products.preview":
                // Check if editing product with its document
                if (NodeID <= 0)
                {
                    return null;
                }

                settings = new UIPageURLSettings
                {
                    Mode = "preview",
                    NodeID = Node.NodeID,
                    Culture = Node.DocumentCulture,
                    Node = Node,
                    AllowViewValidate = false
                };
                tab.RedirectUrl = GetDocumentPageUrl(settings);
                tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "nodeid", NodeID.ToString());
                tab.OnClientClick += "SetTab('PreviewTab')";

                break;

            case "products.livesite":
                // Check if editing product with its document
                if (NodeID <= 0)
                {
                    return null;
                }

                settings = new UIPageURLSettings
                {
                    Mode = "livesite",
                    NodeID = Node.NodeID,
                    Culture = Node.DocumentCulture,
                    Node = Node,
                    AllowViewValidate = false
                };
                tab.RedirectUrl = GetDocumentPageUrl(settings);
                tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "nodeid", NodeID.ToString());
                tab.OnClientClick += "SetTab('LiveSiteTab')";

                break;

            case "products.customfields":
            case "productoptions.options.customfields":
                // Check if SKU has any custom fields
                FormInfo formInfo = FormHelper.GetFormInfo("ecommerce.sku", false);
                if (!formInfo.GetFormElements(true, false, true).Any())
                {
                    return null;
                }

                break;

            case "products.advanced":
                tab.Expand = (NodeID <= 0);

                break;
        }

        // Open advanced tab when one of its subtabs is selected
        bool openAdvanced = false;
        switch (selectedTab)
        {
            case "options":
            case "volumediscounts":
            case "workflow":
            case "versions":
            case "documents":
            case "relatedproducts":
                openAdvanced = true;
                break;
        }

        // Ensure tab preselection
        if (lowerElementName.StartsWithCSafe("products.") &&
            ((lowerElementName.Substring("products.".Length) == selectedTab) || (openAdvanced && (lowerElementName == "products.advanced"))))
        {
            CurrentMaster.Tabs.SelectedTab = tabIndex;
        }

        // Add SiteId parameter to each tab
        if (!string.IsNullOrEmpty(tab.RedirectUrl))
        {
            tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "siteId", SiteID.ToString());
        }

        // Ensure split view mode
        if ((NodeID > 0) && splitViewSupported && CMSContext.DisplaySplitMode)
        {
            tab.RedirectUrl = GetSplitViewUrl(tab.RedirectUrl);
        }

        // Make URL absolute
        tab.RedirectUrl = URLHelper.GetAbsoluteUrl(tab.RedirectUrl);

        return tab;
    }
}