using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Ecommerce_Controls_UI_ProductEdit : CMSAdminControl, IPostBackEventHandler
{
    #region "Backing fields"

    private DataClassInfo mProductClass = null;
    private bool? mProductIsOrdered = null;

    private FormModeEnum mFormMode = FormModeEnum.Update;
    private List<BasicForm> mAllSKUForms = null;
    private List<BasicForm> mSKURepresentingForms = null;
    private bool mEnabled = true;
    private bool mShowRemoveSKUBindingPanel = false;

    #endregion


    #region "Properties - new product"

    /// <summary>
    /// Gets the parent node ID value for the new product.
    /// </summary>
    public int NewProductParentNodeID
    {
        get
        {
            return DocumentManager.ParentNodeID;
        }
    }


    /// <summary>
    /// Gets the data class ID value for the new product.
    /// </summary>
    public int NewProductDataClassID
    {
        get
        {
            return DocumentManager.NewNodeClassID;
        }
    }


    /// <summary>
    /// Gets or sets the parent product option category for the new product option.
    /// </summary>
    public OptionCategoryInfo NewProductOptionCategory
    {
        get;
        set;
    }

    #endregion


    #region "Properties - product"

    /// <summary>
    /// Gets or sets the product to be edited.
    /// </summary>
    public BaseInfo Product
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the value that indicates if the edited product is a new product.
    /// </summary>
    private bool ProductIsNew
    {
        get
        {
            return Product.GetIntegerValue("SKUID", 0) == 0;
        }
    }


    /// <summary>
    /// Gets the edited product site ID.
    /// </summary>
    public int ProductSiteID
    {
        get
        {
            return Product.GetIntegerValue("SKUSiteID", 0);
        }
    }


    /// <summary>
    /// Gets the value that indicates if the edite product is a product option.
    /// </summary>
    private bool ProductIsProductOption
    {
        get
        {
            return Product.GetIntegerValue("SKUOptionCategoryID", 0) > 0;
        }
    }


    /// <summary>
    /// Gets the value that indicates if the product is ordered.
    /// </summary>
    private bool ProductIsOrdered
    {
        get
        {
            int productId = Product.GetIntegerValue("SKUID", 0);
            if (productId <= 0)
            {
                return false;
            }

            if (!mProductIsOrdered.HasValue)
            {
                DataSet ds = OrderItemInfoProvider.GetOrderItems("OrderItemSKUID = " + productId, null, 1, "OrderItemID");
                mProductIsOrdered = !DataHelper.DataSourceIsEmpty(ds);
            }

            return mProductIsOrdered ?? true;
        }
    }

    #endregion


    #region "Properties - product data class"

    /// <summary>
    /// Gets the data class of the product.
    /// </summary>
    private DataClassInfo ProductClass
    {
        get
        {
            if (mProductClass == null)
            {
                if (NewProductDataClassID > 0)
                {
                    // Get from document manager
                    mProductClass = DocumentManager.NewNodeClass;
                }
                else
                {
                    // Get from DB
                    TreeNode node = Product as TreeNode;
                    if (node != null)
                    {
                        mProductClass = DataClassInfoProvider.GetDataClass(node.ClassName);
                    }
                }
            }

            return mProductClass;
        }
    }


    /// <summary>
    /// Gets the mappings between the SKU fields and the document fields (key = document field name, value = SKU field name).
    /// </summary>
    private StringSafeDictionary<string> SKUMappings
    {
        get
        {
            if (ProductClass != null)
            {
                return ProductClass.SKUMappings;
            }

            return new StringSafeDictionary<string>();
        }
    }


    /// <summary>
    /// Gets the value that indicates if an SKU should be created automatically when the document is created.
    /// </summary>
    private bool CreateSKUAutomatically
    {
        get
        {
            if (ProductClass != null)
            {
                return ProductClass.ClassCreateSKU && ProductIsNew && !IsBindSKUAction;
            }

            return false;
        }
    }

    #endregion


    #region "Properties - forms"

    /// <summary>
    /// Gets or sets the form mode.
    /// </summary>
    public FormModeEnum FormMode
    {
        get
        {
            return mFormMode;
        }
        set
        {
            mFormMode = value;
        }
    }


    /// <summary>
    /// Gets the SKUProductType form field value.
    /// </summary>
    private SKUProductTypeEnum SKURepresenting
    {
        get
        {
            object value = null;
            if (skuGeneralForm.FieldEditingControls != null)
            {
                value = skuGeneralForm.GetFieldValue("SKUProductType");
            }
            return SKUInfoProvider.GetSKUProductTypeEnum(ValidationHelper.GetString(value, null));
        }
    }


    /// <summary>
    /// Gets the SKU representing form.
    /// </summary>
    private BasicForm SKURepresentingForm
    {
        get
        {
            switch (SKURepresenting)
            {
                case SKUProductTypeEnum.Membership:
                    return skuMembershipForm;
                case SKUProductTypeEnum.EProduct:
                    return skuEproductForm;
                case SKUProductTypeEnum.Donation:
                    return skuDonationForm;
                case SKUProductTypeEnum.Bundle:
                    return skuBundleForm;
                default:
                    return null;
            }
        }
    }


    /// <summary>
    /// Gets the value that indicates if there are any custom SKU fields visible.
    /// </summary>
    private bool HasAnyCustomSKUFields
    {
        get
        {
            return skuCustomForm.IsAnyFieldVisible() && !DoNotCreateSKU && !UseExistingSKU;
        }
    }


    /// <summary>
    /// Gets the value that indicates if there are any document fields visible.
    /// </summary>
    private bool HasAnyDocumentFields
    {
        get
        {
            if (documentForm.BasicForm != null)
            {
                return documentForm.BasicForm.IsAnyFieldVisible();
            }
            return false;
        }
    }


    /// <summary>
    /// Gets the value that indicates if there are any custom properties fields visible.
    /// </summary>
    private bool HasAnyCustomPropertiesFields
    {
        get
        {
            return HasAnyCustomSKUFields || HasAnyDocumentFields;
        }
    }


    /// <summary>
    /// Gets the list of all the SKU forms.
    /// </summary>
    private List<BasicForm> SKUForms
    {
        get
        {
            if (mAllSKUForms == null)
            {
                mAllSKUForms = new List<BasicForm>
                {
                    skuGeneralForm,
                    skuMembershipForm,
                    skuEproductForm,
                    skuDonationForm,
                    skuBundleForm,
                    skuCustomForm,
                    skuOtherForm
                };
            }
            return mAllSKUForms;
        }
    }


    /// <summary>
    /// Gets the list of all the SKU "representing" forms.
    /// </summary>
    public List<BasicForm> SKURepresentingForms
    {
        get
        {
            if (mSKURepresentingForms == null)
            {
                mSKURepresentingForms = new List<BasicForm>
                {
                    skuMembershipForm,
                    skuEproductForm,
                    skuDonationForm,
                    skuBundleForm
                };
            }
            return mSKURepresentingForms;
        }
    }


    /// <summary>
    /// Gets the list of the SKU forms that are currently in use.
    /// </summary>
    private List<BasicForm> CurrentSKUForms
    {
        get
        {
            List<BasicForm> forms = new List<BasicForm>();
            forms.Add(skuGeneralForm);
            if (SKURepresentingForm != null)
            {
                forms.Add(SKURepresentingForm);
            }
            forms.Add(skuCustomForm);
            forms.Add(skuOtherForm);
            return forms;
        }
    }

    #endregion


    #region "Properties - general"

    /// <summary>
    /// Gets or sets the value that indicates if the control is displayed when the SKU is to be bound to an existing document.
    /// </summary>
    public bool IsBindSKUAction
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is displayed in CMS Desk - Content.
    /// </summary>
    public bool IsInContent
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is displayed in compare mode on the right side.
    /// </summary>
    public bool IsInCompare
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is in a dialog.
    /// </summary>
    public bool IsInDialog
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is enabled.
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
            headerActionsElem.Enabled = value;
            pnlCreateSkuBinding.Enabled = value;
            pnlRemoveSkuBinding.Enabled = value;
            SKUForms.ForEach(f => f.Enabled = value);
            documentForm.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is used on a live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            editMenuElem.IsLiveSite = value;
            headerActionsElem.IsLiveSite = value;
            plcMessages.IsLiveSite = value;
            quickNavigationElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            editMenuElem.StopProcessing = value;
            headerActionsElem.StopProcessing = value;
            SKUForms.ForEach(f => f.StopProcessing = value);
            documentForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Gets the messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }


    /// <summary>
    /// Gets the value that indicates if the "create SKU binding" panel should be visible.
    /// </summary>
    private bool ShowCreateSKUBindingPanel
    {
        get
        {
            string siteName = SiteInfoProvider.GetSiteName(ProductSiteID);

            return ProductIsNew && !ProductIsProductOption && !IsInDialog &&
                (CreateSKUAutomatically || selectSkuBindingElem.AllowCreateNewGlobal || selectSkuBindingElem.AllowUseExisting || selectSkuBindingElem.AllowDoNotCreate) &&
                (ECommerceSettings.AllowProductsWithoutDocuments(siteName) || ECommerceSettings.AllowGlobalProducts(siteName) || AllowDoNoCreateSKU || IsBindSKUAction);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the "remove SKU binding" panel should be visible.
    /// </summary>
    public bool ShowRemoveSKUBindingPanel
    {
        get
        {
            bool show = mShowRemoveSKUBindingPanel;
            show &= !ShowCreateSKUBindingPanel;
            show &= !IsInDialog;
            show &= FormMode != FormModeEnum.InsertNewCultureVersion;

            TreeNode node = Product as TreeNode;
            if (node != null)
            {
                WorkflowInfo wf = node.GetWorkflow();
                show &= wf == null;
            }

            return show;
        }
        set
        {
            mShowRemoveSKUBindingPanel = value;
        }
    }


    /// <summary>
    /// Gets the value that indicates if a new site SKU should be created.
    /// </summary>
    private bool CreateNewSiteSKU
    {
        get
        {
            return selectSkuBindingElem.GetValueFromRequest() == CMSModules_Ecommerce_FormControls_SelectSKUBinding.CREATE_NEW;
        }
    }


    /// <summary>
    /// Gets the value that indicates if a new global SKU should be created.
    /// </summary>
    private bool CreateNewGlobalSKU
    {
        get
        {
            return selectSkuBindingElem.GetValueFromRequest() == CMSModules_Ecommerce_FormControls_SelectSKUBinding.CREATE_NEW_GLOBAL;
        }
    }


    /// <summary>
    /// Gets the value that indicates if a new site or global SKU should be created.
    /// </summary>
    private bool CreateNewSKU
    {
        get
        {
            return (!CreateSKUAutomatically && (CreateNewSiteSKU || CreateNewGlobalSKU));
        }
    }


    /// <summary>
    /// Gets the value that indicates if an existing SKU should be used.
    /// </summary>
    private bool UseExistingSKU
    {
        get
        {
            return selectSkuBindingElem.GetValueFromRequest() == CMSModules_Ecommerce_FormControls_SelectSKUBinding.USE_EXISTING;
        }
    }


    /// <summary>
    /// Gets the value that indicates if an SKU does not need to be created for the product.
    /// </summary>
    public bool AllowDoNoCreateSKU
    {
        get
        {
            return IsInContent;
        }
    }


    /// <summary>
    /// Gets the value that indicates if the SKU should not be created.
    ///
    /// </summary>
    private bool DoNotCreateSKU
    {
        get
        {
            return selectSkuBindingElem.GetValueFromRequest() == CMSModules_Ecommerce_FormControls_SelectSKUBinding.DO_NOT_CREATE;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs after the product was saved succesfully.
    /// </summary>
    public event EventHandler ProductSaved;


    /// <summary>
    /// Raises the ProductSaved event.
    /// </summary>
    protected void RaiseProductSaved()
    {
        if (ProductSaved != null)
        {
            ProductSaved(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Gets a value that indicates if a new product should be created after the currently edited product is saved.
    /// </summary>
    public bool ProductSavedCreateAnother
    {
        get;
        private set;
    }


    /// <summary>
    /// Gets a value that indicates if the SKU binding was removed from the product on save.
    /// </summary>
    public bool ProductSavedSkuBindingRemoved
    {
        get;
        private set;
    }

    #endregion


    #region "Lifecycle"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (StopProcessing)
        {
            return;
        }

        IsInContent = QueryHelper.GetBoolean("content", false);
        IsInCompare = QueryHelper.GetBoolean("compare", false);
        IsInDialog = QueryHelper.GetBoolean("dialog", false);

        string action = QueryHelper.GetString("action", string.Empty).ToLowerCSafe();
        switch (action)
        {
            case "newculture":
                FormMode = FormModeEnum.InsertNewCultureVersion;
                break;

            case "bindsku":
                IsBindSKUAction = true;
                break;
        }

        int productId = QueryHelper.GetInteger("productId", 0);
        Product = SKUInfoProvider.GetSKUInfo(productId);

        int newProductOptionCategoryId = QueryHelper.GetInteger("categoryId", 0);
        NewProductOptionCategory = OptionCategoryInfoProvider.GetOptionCategoryInfo(newProductOptionCategoryId);

        InitDocumentManager();
        InitProduct();

        if (Product != null)
        {
            InitSkuForms();
            InitDocumentForm();

            InitHeaderActions();
            InitCreateSkuBinding();
            InitRemoveSkuBinding();

            InitPage();
        }
        else
        {
            // Stop further processing and hide navigation
            StopProcessing = true;
            quickNavigationElem.Visible = false;

            ShowError(GetString("com.products.notaproducttype"));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            return;
        }

        // Set product defaults
        var defaultsSet = ValidationHelper.GetBoolean(ViewState["SKUDefaultsSet"], false);
        if (ProductIsNew && !defaultsSet)
        {
            SetFieldDefaults();
            ViewState["SKUDefaultsSet"] = true;
        }

        // Set forms visibility
        SKURepresentingForms.ForEach(f => f.Visible = f == SKURepresentingForm);
        CurrentSKUForms.ForEach(f => f.Visible = CreateNewSKU);

        InitCustomProperties();
        InitQuickNavigation();

        if (Product is TreeNode)
        {
            Enabled = DocumentManager.AllowSave;

            pnlFormsInner.CssClass = ProductClass.ClassName.ToLowerCSafe().Replace('.', '_');

            imgRemoveSkuBinding.ImageUrl = Enabled ? GetImageUrl("Objects/Ecommerce_SKU/unbind.png") : GetImageUrl("Objects/Ecommerce_SKU/unbinddisabled.png");
            btnRemoveSkuBinding.OnClientClick = Enabled ? string.Format("return confirm('{0}');", GetString("com.skubinding.unbindconfirm")) : null;
        }

        // Set enabled value for the product type field
        ActOnField(SKUForms, "SKUProductType", f => f.Enabled &= !(ProductIsOrdered || IsInCompare));

        ApplyCompare();

        // Show warning if the main currency is not defined
        string currencyWarning = ECommerceHelper.CheckMainCurrency(ProductSiteID);
        if (!string.IsNullOrEmpty(currencyWarning))
        {
            ShowWarning(currencyWarning, null, null);
        }

        // Hide quick navigation in split mode
        if (CMSContext.DisplaySplitMode)
        {
            quickNavigationElem.Visible = false;
            pnlForms.Style.Add("margin", "0");
        }

        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "QuickNavigationSmoothScroll", ScriptHelper.GetScript(GetQuickNavigationSmoothScrollScript()));
    }

    #endregion


    #region "Initialization"

    /// <summary>
    /// Initializes the document manager.
    /// </summary>
    private void InitDocumentManager()
    {
        int parentNodeId = QueryHelper.GetInteger("parentNodeId", 0);
        string parentCulture = QueryHelper.GetString("parentCulture", null);
        int classId = QueryHelper.GetInteger("classId", 0);
        int sourceDocumentId = QueryHelper.GetInteger("sourceDocumentId", 0);
        int convertDocumentId = QueryHelper.GetInteger("convertDocumentId", 0);

        DocumentManager.ParentNodeID = parentNodeId;
        DocumentManager.NewNodeCultureCode = parentCulture;
        DocumentManager.NewNodeClassID = classId;
        DocumentManager.LocalMessagesPlaceHolder = MessagesPlaceHolder;
        DocumentManager.Mode = FormMode;

        // Copy content from another language
        if (sourceDocumentId > 0)
        {
            DocumentManager.SourceDocumentID = sourceDocumentId;
        }
        // Convert document
        else if (convertDocumentId > 0)
        {
            DocumentManager.SourceDocumentID = convertDocumentId;
            FormMode = FormModeEnum.Convert;
            DocumentManager.Mode = FormModeEnum.Convert;
        }

        if ((FormMode == FormModeEnum.Insert) && ((parentNodeId <= 0) || (classId <= 0)))
        {
            DocumentManager.Mode = FormModeEnum.Update;
        }

        DocumentManager.OnValidateData += delegate(object sender, DocumentManagerEventArgs args)
        {
            args.IsValid = (Product != null) && IsValid();
        };

        DocumentManager.OnSaveData += delegate(object sender, DocumentManagerEventArgs args)
        {
            if (CreateNewSKU)
            {
                CurrentSKUForms.ForEach(f => f.RaiseOnUploadFile(this, EventArgs.Empty));
            }

            ProductSavedCreateAnother = DocumentManager.CreateAnother;
            RaiseProductSaved();
        };
    }


    /// <summary>
    /// Initializes the edited product object.
    /// </summary>
    private void InitProduct()
    {
        if (Product != null)
        {
            return;
        }

        var newSku = new SKUInfo()
        {
            SKUEnabled = true
        };

        // Set SKU option category
        if (NewProductOptionCategory != null)
        {
            newSku.SKUOptionCategoryID = NewProductOptionCategory.CategoryID;
        }

        bool isNewSku = false;

        if (DocumentManager.NodeID > 0)
        {
            // Get the existing node
            var node = DocumentManager.Node as SKUTreeNode;
            if ((node != null) && !node.HasSKU && !UseExistingSKU)
            {
                node.SKU = newSku;
                isNewSku = true;
            }

            Product = node;
        }
        else if ((NewProductParentNodeID > 0) && (NewProductDataClassID > 0))
        {
            // Create a new node if all required information are specified
            var node = DocumentManager.Node as SKUTreeNode;
            if ((node != null) && !DoNotCreateSKU && !UseExistingSKU)
            {
                node.SKU = newSku;
                isNewSku = true;
            }

            Product = node;
        }
        else
        {
            // Create a new SKU
            Product = newSku;
            isNewSku = true;
        }

        if (isNewSku)
        {
            // Set SKU site ID
            if (NewProductOptionCategory != null)
            {
                if (NewProductOptionCategory.CategorySiteID > 0)
                {
                    Product.SetValue("SKUSiteID", NewProductOptionCategory.CategorySiteID);
                }
                else
                {
                    Product.SetValue("SKUSiteID", null);
                }
            }
            else if (CreateNewSiteSKU)
            {
                Product.SetValue("SKUSiteID", CMSContext.CurrentSiteID);
            }
            else if (CreateNewGlobalSKU)
            {
                Product.SetValue("SKUSiteID", null);
            }
        }
    }


    /// <summary>
    /// Sets the field defaults.
    /// </summary>
    private void SetFieldDefaults()
    {
        switch (SKURepresenting)
        {
            case SKUProductTypeEnum.Product:
            case SKUProductTypeEnum.Bundle:
                SetFieldValue(SKUForms, "SKUNeedsShipping", true);
                break;
        }

        if (ProductIsProductOption)
        {
            SetFieldValue(SKUForms, "SKUNeedsShipping", true);
        }
    }


    /// <summary>
    /// Initializes the "create SKU binding" panel.
    /// </summary>
    private void InitCreateSkuBinding()
    {
        // Check if user is authorized to create global products
        selectSkuBindingElem.AllowCreateNewGlobal = ECommerceContext.IsUserAuthorizedToModifySKU(true);

        if (CreateSKUAutomatically)
        {
            // Create SKU automatically
            selectSkuBindingElem.AllowCreateNew = selectSkuBindingElem.AllowCreateNewGlobal;
            selectSkuBindingElem.AllowUseExisting = false;
            selectSkuBindingElem.AllowDoNotCreate = false;
            selectSkuBindingElem.InfoText = GetString("com.productedit.createskuautomatically");
        }
        else
        {
            // Create SKU manually
            selectSkuBindingElem.AllowUseExisting = Product is TreeNode;
            selectSkuBindingElem.AllowDoNotCreate = AllowDoNoCreateSKU;
        }

        pnlCreateSkuBinding.GroupingText = GetString("com.productedit.skubinding");
        pnlCreateSkuBinding.Visible = ShowCreateSKUBindingPanel;
    }


    /// <summary>
    /// Initializes the "remove SKU binding" panel.
    /// </summary>
    private void InitRemoveSkuBinding()
    {
        pnlRemoveSkuBinding.GroupingText = GetString("com.skubinding");
        pnlRemoveSkuBinding.Visible = ShowRemoveSKUBindingPanel;

        imgRemoveSkuBinding.ImageUrl = Enabled ? GetImageUrl("Objects/Ecommerce_SKU/unbind.png") : GetImageUrl("Objects/Ecommerce_SKU/unbinddisabled.png");
        btnRemoveSkuBinding.OnClientClick = Enabled ? string.Format("return confirm('{0}');", GetString("com.skubinding.unbindconfirm")) : null;

        btnRemoveSkuBinding.Click += delegate(object sender, EventArgs args)
        {
            var node = Product as SKUTreeNode;
            if (node != null)
            {
                node.SKU = null;
                node.Update();

                ProductSavedSkuBindingRemoved = true;
                RaiseProductSaved();
                ShowChangesSaved();
            }
        };
    }


    /// <summary>
    /// Initializes the SKU forms.
    /// </summary>
    private void InitSkuForms()
    {
        // Initialize SKU forms
        skuGeneralForm.FormInformation = FormHelper.GetFormInfo(ECommerceObjectType.SKU + ".UpdateGeneral", true);
        skuMembershipForm.FormInformation = FormHelper.GetFormInfo(ECommerceObjectType.SKU + ".UpdateMembership", true);
        skuEproductForm.FormInformation = FormHelper.GetFormInfo(ECommerceObjectType.SKU + ".UpdateEproduct", true);
        skuDonationForm.FormInformation = FormHelper.GetFormInfo(ECommerceObjectType.SKU + ".UpdateDonation", true);
        skuBundleForm.FormInformation = FormHelper.GetFormInfo(ECommerceObjectType.SKU + ".UpdateBundle", true);
        skuCustomForm.FormInformation = FormHelper.GetFormInfo(ECommerceObjectType.SKU + ".UpdateCustom", true);
        skuOtherForm.FormInformation = FormHelper.GetFormInfo(ECommerceObjectType.SKU + ".UpdateOther", true);

        bool createEmptyDocument = (FormMode == FormModeEnum.InsertNewCultureVersion) && (DocumentManager.SourceDocumentID == 0);

        SKUForms.ForEach(f =>
        {
            f.Data = Product;
            f.Mode = FormMode;
            f.SubmitButton.Visible = false;
            f.MessagesPlaceHolder = MessagesPlaceHolder;

            // Set additional form data
            f.AdditionalData["DataClassID"] = NewProductDataClassID;
            f.AdditionalData["IsInCompare"] = IsInCompare;

            // Set additional form macro data
            f.ContextResolver.SetNamedSourceData("ProductSiteID", ProductSiteID);

            if (Product is TreeNode)
            {
                f.OnAfterDataLoad += delegate(object sender, EventArgs args)
                {
                    TreeNode productNode = Product as TreeNode;
                    BasicForm form = sender as BasicForm;

                    // Change image field label when product id under workflow and using metafiles for product images
                    if ((productNode != null) && (form != null) && ECommerceSettings.UseMetaFileForProductImage &&
                        (productNode.DocumentWorkflowStepID > 0) && form.FieldLabels.Contains("skuimagepath"))
                    {
                        form.FieldLabels["skuimagepath"].ResourceString = "com.sku.imagepathnotversioned";
                    }
                };
            }

            // Create empty document
            if (createEmptyDocument && !RequestHelper.IsPostBack())
            {
                f.OnAfterDataLoad += delegate(object sender, EventArgs args)
                {
                    // Clear the paired document fields
                    SetFieldValue(CurrentSKUForms, "SKUName", null);
                    SetFieldValue(CurrentSKUForms, "SKUShortDescription", null);
                    SetFieldValue(CurrentSKUForms, "SKUDescription", null);

                    // Clear the mapped document fields
                    foreach (DictionaryEntry mapping in SKUMappings)
                    {
                        string skuFieldName = Convert.ToString(mapping.Key);
                        SetFieldValue(CurrentSKUForms, skuFieldName, null);
                    }
                };
            }

            f.OnItemValidation += delegate(object sender, ref string errorMessage)
            {
                FormEngineUserControl control = sender as FormEngineUserControl;
                if (control != null)
                {
                    errorMessage = ValidateSkuField(control);
                }
            };
        });
    }


    /// <summary>
    /// Applies compare mode to the forms.
    /// </summary>
    private void ApplyCompare()
    {
        if (IsInCompare)
        {
            // Disable non-text fields
            foreach (string field in skuGeneralForm.Fields)
            {
                // Skip text fields which can be localized
                switch (field.ToLowerCSafe())
                {
                    case "skuname":
                    case "skudescription":
                    case "skushortdescription":
                        continue;
                }

                // Disable the non-localized field
                FormEngineUserControl fc = skuGeneralForm.FieldControls[field];
                if (fc != null)
                {
                    fc.Enabled = false;
                }
            }

            SetFormsCompare(skuMembershipForm, skuEproductForm, skuDonationForm, skuBundleForm, skuCustomForm, skuOtherForm);
        }
    }


    /// <summary>
    /// Sets the compare mode on the given list of forms.
    /// </summary>
    protected void SetFormsCompare(params BasicForm[] forms)
    {
        // Apply to all forms
        foreach (BasicForm form in forms)
        {
            form.Enabled = false;
            form.ToolTip = GetString("compare.notlocalizable");
        }
    }


    /// <summary>
    /// Initializes the document form.
    /// </summary>
    private void InitDocumentForm()
    {
        if (!(Product is TreeNode))
        {
            documentForm.StopProcessing = true;
            return;
        }

        documentForm.OnBeforeBasicFormDataLoad += delegate(object sender, EventArgs args)
        {
            HideRedundantDocumentFields();
        };

        // Force CMSForm.CreateChildControls to run
        bool basicFormInitialized = documentForm.BasicForm != null;

        documentForm.BasicForm.MessagesPlaceHolder = MessagesPlaceHolder;

        documentForm.OnBeforeSave += delegate(object sender, EventArgs args)
        {
            // Save the SKU data before saving to the database
            SaveSKUDataToProduct();
            SaveSKUDefaultDataToProduct();
        };

        // Get default page template from document type
        if (ProductClass != null)
        {
            documentForm.DefaultPageTemplateID = ProductClass.ClassDefaultPageTemplateID;
        }

        HideRedundantDocumentFields();
    }


    /// <summary>
    /// Initializes the header actions.
    /// </summary>
    private void InitHeaderActions()
    {
        if (Product is TreeNode)
        {
            // Initialize edit menu
            editMenuElem.Visible = true;
            editMenuElem.StopProcessing = false;
        }
        else
        {
            // Initialize header actions
            pnlHeaderActions.Visible = true;
            headerActionsElem.StopProcessing = false;
            headerActionsElem.AddAction(new SaveAction(Page));

            if (FormMode == FormModeEnum.Insert)
            {
                headerActionsElem.AddAction(new HeaderAction()
                {
                    ControlType = HeaderActionTypeEnum.LinkButton,
                    Text = GetString("editmenu.iconsaveandanother"),
                    Tooltip = GetString("editmenu.iconsaveandanother"),
                    ImageUrl = Enabled ? GetImageUrl("CMSModules/CMS_Content/EditMenu/16/saveandanother.png") : GetImageUrl("CMSModules/CMS_Content/EditMenu/16/savedisabled.png"),
                    Enabled = Enabled,
                    CommandName = "savecreateanother"
                });
            }

            headerActionsElem.ActionPerformed += delegate(object sender, CommandEventArgs args)
            {
                HandlePostbackCommand(args.CommandName);
            };
        }
    }


    /// <summary>
    /// Initializes the custom properties panel.
    /// </summary>
    private void InitCustomProperties()
    {
        skuCustomForm.Visible = HasAnyCustomSKUFields && !CreateSKUAutomatically;

        if (!documentForm.StopProcessing)
        {
            documentForm.Visible = HasAnyDocumentFields;
        }

        pnlCustomProperties.GroupingText = GetString("com.productedit.customproperties");
        pnlCustomProperties.Visible = HasAnyCustomPropertiesFields;

        string elementName = ProductIsProductOption ? "ProductOptions.Options.CustomFields" : "Products.CustomFields";
        pnlCustomProperties.Visible &= CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Ecommerce", elementName);
    }


    /// <summary>
    /// Initializes the quick navigation.
    /// </summary>
    private void InitQuickNavigation()
    {
        Action<BasicForm> addFormCategories = (form) =>
        {
            foreach (var item in form.CategoryList)
            {
                quickNavigationElem.Items[item.Key] = item.Value;
            }
        };

        quickNavigationElem.Items.Clear();

        if (pnlCreateSkuBinding.Visible)
        {
            string text = GetString("com.productedit.skubinding");
            quickNavigationElem.Items.Add(text, pnlCreateSkuBinding.ClientID);
        }

        if (skuGeneralForm.Visible)
        {
            addFormCategories(skuGeneralForm);
        }

        if ((SKURepresentingForm != null) && SKURepresentingForm.Visible)
        {
            addFormCategories(SKURepresentingForm);
        }

        if (HasAnyCustomPropertiesFields && pnlCustomProperties.Visible)
        {
            string text = GetString("com.productedit.customproperties");
            quickNavigationElem.Items.Add(text, pnlCustomProperties.ClientID);
        }

        if (skuOtherForm.Visible)
        {
            addFormCategories(skuOtherForm);
        }

        if (pnlRemoveSkuBinding.Visible)
        {
            string text = GetString("com.productedit.skubinding");
            quickNavigationElem.Items.Add(text, pnlRemoveSkuBinding.ClientID);
        }
    }


    /// <summary>
    /// Initializes the parent page.
    /// </summary>
    private void InitPage()
    {
        CMSDeskPage page = Page as CMSDeskPage;
        if (page != null)
        {
            if (page.CurrentMaster.PanelHeader != null)
            {
                page.CurrentMaster.PanelHeader.Controls.Add(plcHeaderActions);
            }

            if (page.CurrentMaster.PanelContent != null)
            {
                page.CurrentMaster.PanelContent.CssClass = null;
            }

            if (page.RequiresDialog)
            {
                editMenuElem.ShowSaveAndClose = true;

                // Add the product name to the dialog title
                if (Product is TreeNode)
                {
                    TreeNode node = Product as TreeNode;
                    string nodeName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(node.GetDocumentName()));
                    string text = string.Format("{0} \"{1}\"", GetString("content.edittitle"), nodeName);
                    page.SetTitle("Objects/Ecommerce_SKU/object.png", text, null, null);
                }
            }
        }
    }


    /// <summary>
    /// Builds and returns the quick navigation smooth scroll script.
    /// </summary>
    private string GetQuickNavigationSmoothScrollScript()
    {
        var script = new StringBuilder();

        script.AppendLine("jQuery('.QuickNavigation a[href^=#]').each(function () {");
        script.AppendLine("var jThis = jQuery(this);");
        script.AppendLine("jThis.click(function () {");
        script.AppendLine("var target = jQuery(this.hash);");
        script.AppendLine("target = target.length && target || jQuery('[name=' + this.hash.slice(1) + ']');");
        script.AppendLine("if (target.length) {");
        if (IsInDialog)
        {
            script.AppendLine("var scrollOffset = target.offset().top - cmsHeader.offsetHeight - cmsHeader.offsetTop - 10 + jQuery('.ModalDialogContent').scrollTop();");
            script.AppendLine("jQuery('.ModalDialogContent').animate({ scrollTop: scrollOffset }, 300);");
        }
        else
        {
            script.AppendLine("var scrollOffset = target.offset().top - cmsHeader.offsetHeight - cmsHeader.offsetTop - 10;");
            script.AppendLine("jQuery('html, body').animate({ scrollTop: scrollOffset }, 300);");
        }
        script.AppendLine("return false;");
        script.AppendLine("}");
        script.AppendLine("});");
        script.AppendLine("});");

        return script.ToString();
    }

    #endregion


    #region "Validation"

    /// <summary>
    /// Validates the forms.
    /// Returns true if the form data is valid, otherwise returns false.
    /// </summary>
    public bool IsValid()
    {
        bool isValid = true;

        // Validate new SKU binding
        if (!selectSkuBindingElem.IsValid())
        {
            ShowError(selectSkuBindingElem.ValidationError);
            isValid &= false;
        }

        // Validate general SKU forms
        if (CreateNewSKU)
        {
            isValid &= skuGeneralForm.ValidateData();
            if (SKURepresentingForm != null)
            {
                isValid &= SKURepresentingForm.ValidateData();
            }
            isValid &= skuCustomForm.ValidateData();
        }

        // Validate document form
        if (Product is TreeNode)
        {
            isValid &= documentForm.BasicForm.ValidateData();
        }

        // Validate other SKU form
        if (CreateNewSKU)
        {
            isValid &= skuOtherForm.ValidateData();
        }

        return isValid;
    }


    /// <summary>
    /// Performs an additional SKU validation on the specified form control.
    /// Returns an error message if the field is not valid, otherwise returns null.
    /// </summary>
    private string ValidateSkuField(FormEngineUserControl formControl)
    {
        // General validation
        switch (formControl.FieldInfo.Name)
        {
            case "SKUImagePath":
                {
                    string value = formControl.Value as string;

                    // Validate meta files file system permissions
                    string siteName = SiteInfoProvider.GetSiteName(ProductSiteID);
                    if (!string.IsNullOrEmpty(value) && MetaFileInfoProvider.StoreFilesInFileSystem(siteName))
                    {
                        string path = MetaFileInfoProvider.GetFilesFolderPath(siteName);
                        if (!DirectoryHelper.CheckPermissions(path))
                        {
                            return GetString("com.newproduct.accessdeniedtopath");
                        }
                    }
                }
                break;

            case "SKUMembershipGUID":
                {
                    Guid value = ValidationHelper.GetGuid(formControl.Value, Guid.Empty);

                    // Validate membership is selected
                    if (value == Guid.Empty)
                    {
                        return GetString("com.membership.nomembershipselectederror");
                    }
                }
                break;

            case "SKUWeight":
                {
                    double? value = GetNullableDouble(formControl.Value);

                    // Validate weight is > 0
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.packageweightinvalid");
                    }
                }
                break;

            case "SKUHeight":
                {
                    double? value = GetNullableDouble(formControl.Value);

                    // Validate height is > 0
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.packageheightinvalid");
                    }
                }
                break;

            case "SKUWidth":
                {
                    double? value = GetNullableDouble(formControl.Value);

                    // Validate width is > 0
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.packagewidthinvalid");
                    }
                }
                break;

            case "SKUDepth":
                {
                    double? value = GetNullableDouble(formControl.Value);

                    // Validate depth is > 0
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.packagedepthinvalid");
                    }
                }
                break;

            case "SKUMinItemsInOrder":
                {
                    int? value = GetNullableInteger(formControl.Value);

                    // Validate minimum items in one order is > 0
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.minorderitemsinvalid");
                    }

                    // Validate minimum items in one order is lower than maximum
                    int? maxItems = GetNullableInteger(GetFieldValue(CurrentSKUForms, "SKUMaxItemsInOrder"));
                    if (value.HasValue && maxItems.HasValue && (value.Value > maxItems.Value))
                    {
                        return GetString("com.productedit.minorderitemsexceedsmax");
                    }
                }
                break;

            case "SKUMaxItemsInOrder":
                {
                    int? value = GetNullableInteger(formControl.Value);

                    // Validate maximum items in one order
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.maxorderitemsinvalid");
                    }
                }
                break;
        }

        // Donation specific validation
        if (SKURepresenting == SKUProductTypeEnum.Donation)
        {
            switch (formControl.FieldInfo.Name)
            {
                case "SKUPrice":
                    {
                        double? value = GetNullableDouble(formControl.Value);

                        // Validate price against donation amount
                        if (value.HasValue)
                        {
                            double? minDonationAmount = GetNullableDouble(GetFieldValue(CurrentSKUForms, "SKUMinPrice"));
                            double? maxDonationAmount = GetNullableDouble(GetFieldValue(CurrentSKUForms, "SKUMaxPrice"));

                            if ((minDonationAmount.HasValue && minDonationAmount.Value > value.Value) || (maxDonationAmount.HasValue && maxDonationAmount < value.Value))
                            {
                                return GetString("com.productedit.donationpriceinvalid");
                            }
                        }
                    }
                    break;

                case "SKURetailPrice":
                    {
                        double? value = GetNullableDouble(formControl.Value);

                        // Validate retail price against donation amount
                        if (value.HasValue)
                        {
                            double? minDonationAmount = GetNullableDouble(GetFieldValue(CurrentSKUForms, "SKUMinPrice"));
                            double? maxDonationAmount = GetNullableDouble(GetFieldValue(CurrentSKUForms, "SKUMaxPrice"));

                            if ((minDonationAmount.HasValue && minDonationAmount.Value > value.Value) || (maxDonationAmount.HasValue && maxDonationAmount < value.Value))
                            {
                                return GetString("com.productedit.donationpriceinvalid");
                            }
                        }
                    }
                    break;

                case "SKUMinPrice":
                    {
                        double? value = GetNullableDouble(formControl.Value);

                        // Validate min donation amount is lower than maximum
                        if (value.HasValue)
                        {
                            double? maxDonationAmount = GetNullableDouble(GetFieldValue(CurrentSKUForms, "SKUMaxPrice"));

                            if (maxDonationAmount.HasValue && (value.Value > maxDonationAmount.Value))
                            {
                                return GetString("com.donation.donationamountrangeinvalid");
                            }
                        }
                    }
                    break;
            }
        }

        return null;
    }


    /// <summary>
    /// Checks if the user is authorized for the specified department and redirects to the access denied page when he isn't.
    /// </summary>
    private void CheckDepartmentPermission(DepartmentInfo department)
    {
        if ((department != null) && !ECommerceContext.IsUserAuthorizedForDepartment(department.DepartmentID))
        {
            string message = string.Format(ResHelper.GetString("cmsdesk.notauthorizedtperdepartment"), department.DepartmentDisplayName);
            RedirectToAccessDenied(message);
        }
    }

    #endregion


    #region "Save"

    /// <summary>
    /// Saves the SKU data to the edited product object.
    /// </summary>
    private void SaveSKUDataToProduct()
    {
        if (UseExistingSKU)
        {
            // Set existing SKU and synchronize its fields with mapped documents fields
            ((SKUTreeNode)Product).NodeSKUID = selectSkuBindingElem.SelectedProduct;
            ((SKUTreeNode)Product).SynchronizeFields();

            return;
        }

        // Clear the product 'representing' properties
        Product.SetValue("SKUMembershipGUID", null);
        Product.SetValue("SKUValidity", null);
        Product.SetValue("SKUValidFor", null);
        Product.SetValue("SKUValidUntil", null);
        Product.SetValue("SKUMaxDownloads", null);
        Product.SetValue("SKUBundleInventoryType", null);
        Product.SetValue("SKUPrivateDonation", null);
        Product.SetValue("SKUMinPrice", null);
        Product.SetValue("SKUMaxPrice", null);

        // Save the form data
        foreach (BasicForm form in CurrentSKUForms.Where(f => f.Visible))
        {
            form.SaveData(null);
        }

        // Fix valid until value
        var validUntil = ValidationHelper.GetDateTime(Product.GetValue("SKUValidUntil"), DateTimeHelper.ZERO_TIME);
        if (validUntil == DateTimeHelper.ZERO_TIME)
        {
            Product.SetValue("SKUValidUntil", null);
        }
    }


    /// <summary>
    /// Saves the edited product object to the database as a standalone SKU.
    /// </summary>
    private bool SaveStandaloneSKU()
    {
        var sku = Product as SKUInfo;

        if ((sku == null) || !IsValid())
        {
            return false;
        }

        DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo(sku.SKUDepartmentID);
        CheckDepartmentPermission(department);

        if (!ECommerceHelper.IsUserAuthorizedForPermission(EcommercePermissions.PRODUCTS_MODIFY, CMSContext.CurrentSiteName, CMSContext.CurrentUser))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "ModifyProducts OR EcommerceModify");
        }

        if (sku.IsGlobal)
        {
            if (!ECommerceHelper.IsUserAuthorizedForPermission(EcommercePermissions.ECOMMERCE_MODIFYGLOBAL, CMSContext.CurrentSiteName, CMSContext.CurrentUser))
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
            }
        }

        SaveSKUDataToProduct();

        SKUInfoProvider.SetSKUInfo(sku);

        if (sku.SKUID == 0)
        {
            return false;
        }

        CurrentSKUForms.ForEach(f => f.RaiseOnUploadFile(this, EventArgs.Empty));
        RaiseProductSaved();
        ShowChangesSaved();

        return true;
    }

    #endregion


    #region "IPostBackEventHandler members"

    /// <summary>
    /// Handles the postback event.
    /// </summary>
    /// <param name="arg">An optional postback event argument</param>
    public void RaisePostBackEvent(string arg)
    {
        HandlePostbackCommand(arg);
    }

    #endregion


    #region "SKU fields mapping"

    /// <summary>
    /// Hides redundant document fields from CMS form.
    /// </summary>
    private void HideRedundantDocumentFields()
    {
        // Do not process
        if (Product is SKUInfo)
        {
            return;
        }

        HideMappedDocumentFields();
        HideDocumentNameField();
    }


    /// <summary>
    /// Hides document fields which are mapped to the visible SKU fields.
    /// </summary>
    private void HideMappedDocumentFields()
    {
        // Hide redundant mappings
        foreach (DictionaryEntry mapping in SKUMappings)
        {
            // Get names of mapped fields
            string skuField = Convert.ToString(mapping.Key);
            string docField = Convert.ToString(mapping.Value);

            // Hide document field
            if (IsSKUFieldVisible(skuField))
            {
                HideDocumentField(docField);
            }
        }
    }


    /// <summary>
    /// Hides 'Document name' field if SKU name field (either SKUName field itself or another document field mapped to the SKUName) is already visible.
    /// </summary>
    private void HideDocumentNameField()
    {
        if (IsSKUFieldVisible("SKUName") || IsSKUFieldMappingVisible("SKUName"))
        {
            var node = DocumentManager.Node;
            if (node != null)
            {
                var fi = FormHelper.GetFormInfo(node.NodeClassName, false);
                if ((fi != null) && fi.FieldExists("DocumentName"))
                {
                    return;
                }
            }

            HideDocumentField("DocumentName");
        }
    }


    /// <summary>
    /// Indicates if the given SKU field is visible in some of the SKU forms.
    /// </summary>
    /// <param name="field">SKU field name to check</param>
    private bool IsSKUFieldVisible(string field)
    {
        if (UseExistingSKU)
        {
            // All SKU form are hidden but SKU has properties already specified
            return true;
        }
        else if (CreateSKUAutomatically || DoNotCreateSKU)
        {
            // All SKU forms are hidden
            return false;
        }
        else
        {
            // Some SKU forms should be visible
            foreach (BasicForm form in CurrentSKUForms)
            {
                if (form.IsFieldVisible(field))
                {
                    return true;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// Indicates if the given document field is visible in the CMS form.
    /// </summary>
    /// <param name="field">Document field name to check</param>
    private bool IsDocumentFieldVisible(string field)
    {
        return documentForm.BasicForm.IsFieldVisible(field);
    }


    /// <summary>
    /// Indicates if there is any visible document field (different from 'Document name' field) which is mapped to the given SKU field.
    /// </summary>
    /// <param name="field">SKU field to check</param>
    private bool IsSKUFieldMappingVisible(string field)
    {
        var checkField = GetMappedDocumentField(field);

        if (!String.IsNullOrEmpty(checkField))
        {
            return IsDocumentFieldVisible(checkField);
        }

        return false;
    }


    private string GetMappedDocumentField(string field)
    {
        string checkField = null;

        foreach (DictionaryEntry mapping in SKUMappings)
        {
            // Get names of mapped fields
            string skuField = Convert.ToString(mapping.Key);
            string docField = Convert.ToString(mapping.Value);

            if ((docField.ToLowerCSafe() != "documentname") &&
                (skuField.ToLowerCSafe() == field.ToLowerCSafe()))
            {
                checkField = docField;
                break;
            }
        }
        return checkField;
    }


    /// <summary>
    /// Hides the given document field from CMS form.
    /// </summary>
    /// <param name="field">Document field name</param>
    private void HideDocumentField(string field)
    {
        documentForm.BasicForm.FieldsToHide.Add(field);
    }


    /// <summary>
    /// Sets department and product type of the SKU if it is being created automatically.
    /// </summary>
    private void SaveSKUDefaultDataToProduct()
    {
        if (CreateSKUAutomatically)
        {
            // Ensure product type
            string skuType = ProductClass.ClassSKUDefaultProductType;
            if (string.IsNullOrEmpty(skuType))
            {
                skuType = SKUInfoProvider.GetSKUProductTypeString(SKUProductTypeEnum.Product);
            }
            Product.SetValue("SKUProductType", skuType);

            // Set product department
            DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo(ProductClass.ClassSKUDefaultDepartmentName, SiteInfoProvider.GetSiteName(ProductSiteID));
            if (department != null)
            {
                Product.SetValue("SKUDepartmentID", department.DepartmentID);
            }

            CheckDepartmentPermission(department);
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Handles the postback command.
    /// </summary>
    private void HandlePostbackCommand(string command)
    {
        switch (command.ToLowerCSafe())
        {
            case "save":
            case "savecreateanother":
                ProductSavedCreateAnother = command.ToLowerCSafe() == "savecreateanother";
                SaveStandaloneSKU();
                break;
        }
    }


    /// <summary>
    /// Finds and returns all the form controls for the specified form field name across all the specified forms.
    /// </summary>
    private IEnumerable<FormEngineUserControl> FindFieldFormControls(IEnumerable<BasicForm> forms, string fieldName)
    {
        return forms.Where(f => (f.FieldControls != null) && f.FieldControls.ContainsKey(fieldName)).Select(f => f.FieldControls[fieldName]);
    }


    /// <summary>
    /// Finds and returns the first form control for the specified form field name across all the specified forms.
    /// Returns null if the form control is not found.
    /// </summary>
    private FormEngineUserControl FindFieldFormControl(IEnumerable<BasicForm> forms, string fieldName)
    {
        return FindFieldFormControls(forms, fieldName).FirstOrDefault();
    }


    /// <summary>
    /// Invokes the specified action for the first specified field found across all the specified forms.
    /// Returns true if any fields were found and acted upon.
    /// </summary>
    private bool ActOnField(IEnumerable<BasicForm> forms, string fieldName, Action<FormEngineUserControl> action)
    {
        var formControl = FindFieldFormControl(forms, fieldName);
        if (formControl != null)
        {
            action.Invoke(formControl);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Sets the field value of the specified fields found across all the specified forms.
    /// </summary>
    private bool SetFieldValue(IEnumerable<BasicForm> forms, string fieldName, object value)
    {
        return ActOnField(forms, fieldName, (f) => f.Value = value);
    }


    /// <summary>
    /// Gets the value of the first field found across all the specified forms.
    /// </summary>
    private object GetFieldValue(IEnumerable<BasicForm> forms, string fieldName)
    {
        object value = null;
        ActOnField(forms, fieldName, (f) => value = f.Value);
        return value;
    }


    /// <summary>
    /// Gets the nullable integer representation of the value.
    /// </summary>
    private int? GetNullableInteger(object value)
    {
        if (value is int)
        {
            return (int)value;
        }

        if ((value != null) && !string.IsNullOrEmpty(value as string))
        {
            return ValidationHelper.GetInteger(value, 0);
        }

        return null;
    }


    /// <summary>
    /// Gets the nullable double representation of the value.
    /// </summary>
    private double? GetNullableDouble(object value)
    {
        if (value is double)
        {
            return (double)value;
        }

        if ((value != null) && !string.IsNullOrEmpty(value as string))
        {
            return ValidationHelper.GetDouble(value, 0);
        }

        return null;
    }

    #endregion
}