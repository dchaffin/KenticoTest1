using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_List : CMSProductsPage
{
    #region "Variables and constants"

    private DialogConfiguration mConfig = null;
    private bool? mShowSections = null;
    private bool showProductsInTree = false;

    private string NO_DATA_CELL_VALUE = "<div style=\"text-align:center;\">-</div>";

    #endregion


    #region "Properties"

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
                mConfig.ContentSelectedSite = CMSContext.CurrentSiteName;
                mConfig.OutputFormat = OutputFormatEnum.Custom;
                mConfig.SelectableContent = SelectableContentEnum.AllContent;
                mConfig.HideAttachments = false;
            }
            return mConfig;
        }
    }


    /// <summary>
    /// Indicates whether product sections are to be included in listing.
    /// </summary>
    private bool ShowSections
    {
        get
        {
            return mShowSections ?? (mShowSections = QueryHelper.GetBoolean("showSections", false)).Value;
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


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        showProductsInTree = ECommerceSettings.DisplayProductsInSectionsTree(CurrentSiteName);

        if (NodeID > 0)
        {
            // Init document list
            docList.Node = DocumentHelper.GetDocument(NodeID, CultureCode, true, Tree);
            docList.Grid.GridName = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_List_Documents.xml";
            docList.AdditionalColumns = "SKUID, DocumentSKUName, NodeParentID, NodeID, NodeSKUID, SKUName, SKUNumber, SKUPrice, SKUAvailableItems, SKUEnabled, SKUSiteID, SKUPublicStatusID, SKUInternalStatusID, SKUReorderAt";
            docList.WhereCodition = GetDocumentWhereCondition();
            docList.OrderBy = ShowSections ? "CASE WHEN NodeSKUID IS NULL THEN 0 ELSE 1 END, DocumentName" : "DocumentName";
            docList.OnExternalAdditionalDataBound += gridData_OnExternalDataBound;
            docList.OnDocumentFlagsCreating += docList_OnDocumentFlagsCreating;
            docList.Grid.OnAction += gridData_OnAction;
            docList.Grid.RememberStateByParam = "";
            docList.SelectLanguageJSFunction = "EditProductInCulture";

            docList.DeleteReturnUrl = "~/CMSModules/Content/CMSDesk/Delete.aspx?multiple=true";
            docList.PublishReturnUrl = "~/CMSModules/Content/CMSDesk/PublishArchive.aspx?multiple=true";
            docList.ArchiveReturnUrl = "~/CMSModules/Content/CMSDesk/PublishArchive.aspx?multiple=true";
            docList.TranslateReturnUrl = "~/CMSModules/Translations/Pages/TranslateDocuments.aspx";

            if (!string.IsNullOrEmpty(ProductsStartingPath))
            {
                docList.CopyMoveLinkStartingPath = ProductsStartingPath;
            }

            string languageSelectionScript = "function EditProductInCulture(nodeId, culture, translated, url) {parent.RefreshTree(nodeId, nodeId); window.location.href = 'Product_Edit_Frameset.aspx?nodeid='+nodeId+'&culture='+culture;}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EditProductInCulture", ScriptHelper.GetScript(languageSelectionScript));

            plcSKUListing.Visible = false;

            // Stop processing SKU table
            gridData.StopProcessing = true;

            EditedObject = docList.Node;

            // Set title
            string title = docList.Node.IsRoot() ? GetString("com.sku.productslist") : docList.Node.GetDocumentName();
            SetTitle("Objects/Ecommerce_SKU/object.png", HTMLHelper.HTMLEncode(title), "product_list", "helpTopic");
        }
        else
        {
            // Init Unigrid
            gridData.OnAction += gridData_OnAction;
            gridData.OnExternalDataBound += gridData_OnExternalDataBound;

            // Stop processing product document listing
            docList.StopProcessing = true;
            plcDocumentListing.Visible = false;

            // Set title according display tree setting
            if (DisplayTreeInProducts)
            {
                SetTitle("Objects/Ecommerce_SKU/object.png", GetString("com.sku.unassignedlist"), "stand_alone_SKUs_list", "helpTopic");
            }
            else
            {
                SetTitle("Objects/Ecommerce_SKU/object.png", GetString("com.sku.productslist"), "SKUs_list", "helpTopic");
            }
        }

        // Show warning when exchange rate from global main currency is missing
        if (AllowGlobalObjects && ExchangeTableInfoProvider.IsExchangeRateFromGlobalMainCurrencyMissing(CMSContext.CurrentSiteID))
        {
            ShowWarning(GetString("com.NeedExchangeRateFromGlobal"), null, null);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register listing scripts
        ScriptHelper.RegisterScriptFile(this, @"~/CMSModules/Content/CMSDesk/View/Listing.js");

        // Register tooltip scripts
        RegisterTooltipScript();

        if (NodeID > 0)
        {
            // No data message for document mode
            docList.Grid.ZeroRowsText = GetString("general.nodatafound");

            // Uncomment following line to display product filter depending on number of rows specified in webconfig key CMSDefaultListingFilterLimit
            // docList.Grid.FilterLimit = ValidationHelper.GetInteger(SettingsHelper.AppSettings["CMSDefaultListingFilterLimit"], 25);

            if (ShowSections)
            {
                CreateCloseListingLink();
            }
        }
        else
        {
            // No data message for SKU mode
            gridData.ZeroRowsText = GetString("general.nodatafound");
            gridData.WhereCondition = GetWhereCondition();
        }

        InitializeMasterPage();

        AddMenuButtonSelectScript("Products", "");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide "Is global" flag when global products are not allowed
        if (NodeID <= 0)
        {
            gridData.NamedColumns["SKUSiteID"].Visible = AllowGlobalObjects;
        }
        else
        {
            docList.Grid.NamedColumns["SKUSiteID"].Visible = AllowGlobalObjects;

            if (ShowSections)
            {
                // Generalize column header when listing products mixed with sections
                docList.Grid.NamedColumns["documentname"].HeaderText = GetString("general.name");
            }
        }
    }

    #endregion


    #region "UniGrid event handlers"

    private object docList_OnDocumentFlagsCreating(object sender, string sourceName, object parameter)
    {
        DataRowView row = parameter as DataRowView;

        // Mark irrelevant cells in full listing mode
        if ((NodeID > 0) && ShowSections && (row != null) && (row["NodeSKUID"] == DBNull.Value))
        {
            return NO_DATA_CELL_VALUE;
        }

        return null;
    }


    private object gridData_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = parameter as DataRowView;

        if (NodeID > 0)
        {
            switch (sourceName.ToLowerCSafe())
            {
                case "skunumber":
                case "skuavailableitems":
                case "skuprice":
                case "publicstatusid":
                case "allowforsale":
                case "skusiteid":
                case "typename":
                    if (ShowSections && (row != null) && (row["NodeSKUID"] == DBNull.Value))
                    {
                        return NO_DATA_CELL_VALUE;
                    }

                    break;

                case "edititem":
                    row = ((GridViewRow)parameter).DataItem as DataRowView;

                    if ((row != null) && ((row["NodeSKUID"] == DBNull.Value) || showProductsInTree))
                    {
                        ImageButton btn = sender as ImageButton;
                        if (btn != null)
                        {
                            int currentNodeId = ValidationHelper.GetInteger(row["NodeID"], 0);
                            int nodeParentId = ValidationHelper.GetInteger(row["NodeParentID"], 0);

                            if (row["NodeSKUID"] == DBNull.Value)
                            {
                                btn.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/ViewProducts.png");
                                btn.ToolTip = GetString("com.sku.viewproducts");
                            }

                            // Go to the selected document
                            btn.OnClientClick = "EditItem(" + currentNodeId + ", " + nodeParentId + "); return false;";
                        }
                    }

                    break;

            }
        }

        switch (sourceName.ToLowerCSafe())
        {
            case "skunumber":
                string skuNumber = ValidationHelper.GetString(row["SKUNumber"], null);
                return HTMLHelper.HTMLEncode(skuNumber ?? "");

            case "skuavailableitems":
                int? count = row["SKUAvailableItems"] as int?;
                int? reorderAt = row["SKUReorderAt"] as int?;

                // Emphasise the number when product needs to be reordered
                if (count.HasValue && reorderAt.HasValue && (count <= reorderAt))
                {
                    // Format message informing about insufficient stock level
                    string reorderMsg = string.Format(GetString("com.sku.reorderatTooltip"), reorderAt);
                    return string.Format("<span class=\"OperationFailed\" onmouseout=\"UnTip()\" onmouseover=\"Tip('{1}')\">{0}</span>", count, reorderMsg);
                }

                return count;

            case "skuprice":
                double value = ValidationHelper.GetDouble(row["SKUPrice"], 0);
                int siteId = ValidationHelper.GetInteger(row["SKUSiteID"], 0);

                // Format price
                return CurrencyInfoProvider.GetFormattedPrice(value, siteId);

            case "publicstatusid":
                int id = ValidationHelper.GetInteger(row["SKUPublicStatusID"], 0);
                PublicStatusInfo publicStatus = PublicStatusInfoProvider.GetPublicStatusInfo(id);
                if (publicStatus != null)
                {
                    // Localize and encode
                    return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(publicStatus.PublicStatusDisplayName));
                }

                return string.Empty;

            case "allowforsale":
                // Get "on sale" flag
                return UniGridFunctions.ColoredSpanYesNo(ValidationHelper.GetBoolean(row["SKUEnabled"], false));

            case "skusiteid":
                // Get "is global" flag
                return UniGridFunctions.ColoredSpanYesNo(row["SKUSiteID"] == DBNull.Value);

            case "typename":
                string docTypeName = ValidationHelper.GetString(row["ClassDisplayName"], null);

                // Localize class display name
                if (!string.IsNullOrEmpty(docTypeName))
                {
                    return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(docTypeName));
                }

                return string.Empty;
        }

        return parameter;
    }


    private void gridData_OnAction(string actionName, object actionArgument)
    {
        int argument = ValidationHelper.GetInteger(actionArgument, 0);
        actionName = actionName.ToLowerCSafe();

        switch (actionName)
        {
            case "edit":
                string url = null;

                if (NodeID > 0)
                {
                    url = "Product_Edit_Frameset.aspx?sectionId=" + NodeID + "&nodeId=" + argument + "&culture=" + CultureCode;
                }
                else
                {
                    url = "Product_Edit_Frameset.aspx?productid=" + argument;
                }

                URLHelper.Redirect(url);

                break;

            case "delete":
                if (NodeID > 0)
                {
                    URLHelper.Redirect("Product_Section.aspx?action=delete&nodeId=" + argument);
                }
                else
                {
                    SKUInfo skuObj = SKUInfoProvider.GetSKUInfo(argument);

                    // Check module permissions
                    if (!ECommerceContext.IsUserAuthorizedToModifySKU(skuObj))
                    {
                        if (skuObj.IsGlobal)
                        {
                            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
                        }
                        else
                        {
                            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyProducts");
                        }
                    }

                    // Check dependencies
                    if (SKUInfoProvider.CheckDependencies(argument))
                    {
                        // Show error message
                        ShowError(GetString("Ecommerce.DeleteDisabled"));

                        return;
                    }

                    SKUInfoProvider.DeleteSKUInfo(skuObj);
                }

                break;
        }
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


    /// <summary>
    /// Creates link for closing full listing mode.
    /// </summary>
    private void CreateCloseListingLink()
    {
        string closeLink = "<a href=\"#\"><span class=\"ListingClose\" style=\"cursor: pointer;\" " +
                                       "onclick=\"parent.SetMode('edit'); return false;\">" + GetString("general.close") +
                                       "</span></a>";

        // Check if node has name
        if (!string.IsNullOrEmpty(docList.Node.DocumentName))
        {
            string docNamePath = "<span class=\"ListingPath\">" + HTMLHelper.HTMLEncode(docList.Node.DocumentName) + "</span>";
            lblListingInfo.Text = String.Format(GetString("com.productsection.listinginfo"), docNamePath, closeLink);
        }
        else
        {
            // Use link for root
            lblListingInfo.Text = String.Format(GetString("com.productsection.rootlistinginfo"), closeLink);
        }
    }


    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        HeaderActions hdrActions = CurrentMaster.HeaderActions;

        string url = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx";
        url = URLHelper.AddParameterToUrl(url, "siteId", CMSContext.CurrentSiteID.ToString());
        url = URLHelper.AddParameterToUrl(url, "parentNodeId", NodeID.ToString());

        TreeNode currentNode = null;

        if (NodeID <= 0)
        {
            // Add "New product" action when tree is visible (it has own 'New' action)
            if (!DisplayTreeInProducts)
            {
                hdrActions.ActionsList.Add(new HeaderAction
                {
                    Text = GetString("com.sku.newsku"),
                    RedirectUrl = url,
                    ImageUrl = GetImageUrl("Objects/Ecommerce_SKU/add.png")
                });
            }
        }
        else
        {
            currentNode = docList.Node;

            // Setup the link to the parent document
            if ((currentNode != null) && (currentNode.NodeClassName.ToLowerCSafe() != "cms.root") && (currentNode.NodeAliasPath.CompareToCSafe(ProductsStartingPath, true) != 0))
            {
                string actionUrl = ShowSections ? "javascript:SelectItem(" + currentNode.NodeParentID + ");" : "javascript:EditItem(" + currentNode.NodeParentID + ");";
                hdrActions.ActionsList.Add(new HeaderAction
                {
                    ControlType = HeaderActionTypeEnum.Hyperlink,
                    Text = GetString("com.ParentSection"),
                    RedirectUrl = actionUrl,
                    ImageUrl = GetImageUrl("CMSModules/CMS_Content/Listing/parent.png")
                });
            }

            // Display the "Edit section" action when section selected
            if ((currentNode != null) && (currentNode.NodeAliasPath != "/"))
            {
                hdrActions.ActionsList.Add(new HeaderAction
                {
                    Text = GetString("com.productsection.editthis"),
                    RedirectUrl = "#",
                    ImageUrl = ResolveUrl(GetDocumentTypeIconUrl(currentNode.ClassName)),
                    OnClientClick = "parent.SetMode('sectionedit'); return false;"
                });
            }
        }
    }


    /// <summary>
    /// Creates where condition for SKUs listing.
    /// </summary>
    private string GetWhereCondition()
    {
        // Display ONLY products - not product options
        string where = "SKUOptionCategoryID IS NULL";

        // Select only products without documents
        if ((NodeID <= 0) && DisplayTreeInProducts)
        {
            where = SqlHelperClass.AddWhereCondition(where, "SKUID NOT IN (SELECT NodeSKUID FROM View_CMS_Tree_Joined WHERE NodeSKUID IS NOT NULL AND NodeSiteID = " + CMSContext.CurrentSiteID + ")");
        }

        // Ordinary user can see only product from departments he can access
        CurrentUserInfo cui = CMSContext.CurrentUser;
        if (!cui.IsGlobalAdministrator && !cui.IsAuthorizedPerResource("CMS.Ecommerce", "AccessAllDepartments"))
        {
            where = SqlHelperClass.AddWhereCondition(where, "(SKUDepartmentID IS NULL) OR SKUDepartmentID IN (SELECT DepartmentID FROM COM_UserDepartment WHERE UserID=" + cui.UserID + ")");
        }

        // Reflect "Allow global products" setting
        string siteWhere = "SKUSiteID = " + CMSContext.CurrentSiteID;
        if (AllowGlobalObjects)
        {
            siteWhere += " OR SKUSiteID IS NULL";
        }

        return SqlHelperClass.AddWhereCondition(where, siteWhere);
    }


    /// <summary>
    /// Creates default where condition for product documents listing.
    /// </summary>
    private string GetDocumentWhereCondition()
    {
        string where = "NodeSKUID IS NOT NULL";
        if (ShowSections)
        {
            where += " OR NodeClassID IN (SELECT ClassID FROM CMS_Class WHERE ClassIsProductSection = 1)";
        }

        // Prepare site condition
        string siteWhere = "SKUSiteID = " + CMSContext.CurrentSiteID;
        if (AllowGlobalObjects)
        {
            siteWhere += " OR SKUSiteID IS NULL";
        }
        else if (ShowSections)
        {
            siteWhere += " OR (SKUSiteID IS NULL AND SKUID IS NULL)";
        }

        // Combine where conditions
        where = SqlHelperClass.AddWhereCondition(where, siteWhere);

        string path = docList.Node.NodeAliasPath ?? "";
        path = SqlHelperClass.GetSafeQueryString(path.TrimEnd('/'));
        string pathCondition = string.Format("NodeAliasPath LIKE N'{0}/%'", path);

        // Combine where conditions
        return SqlHelperClass.AddWhereCondition(where, pathCondition);
    }


    #endregion
}