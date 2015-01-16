using System;
using System.Data;
using System.Web.UI;
using System.Collections;

using CMS.CMSHelper;
using CMS.EventLog;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.SiteProvider;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_PortalEngine_Controls_WebParts_WebpartProperties : CMSUserControl
{
    #region "Variables"

    protected string mAliasPath = null;
    protected int mPageTemplateId = 0;
    protected string mZoneId = null;
    protected string mWebpartId = null;
    protected Guid mInstanceGUID = Guid.Empty;
    protected bool mWebPartIdChanged = false;
    protected string mBeforeFormDefinition = null;
    protected string mAfterFormDefinition = null;
    protected bool mIsNewWebPart = false;
    protected int mPosition = 0;
    protected bool mIsNewVariant = false;
    protected int mVariantId = 0;
    protected int mZoneVariantId = 0;
    protected int xmlVersion = 0;

    protected VariantModeEnum mVariantMode = VariantModeEnum.None;


    /// <summary>
    /// Current page info.
    /// </summary>
    private PageInfo pi = null;

    /// <summary>
    /// Page template info.
    /// </summary>
    private PageTemplateInfo pti = null;

    /// <summary>
    /// Current web part.
    /// </summary>
    private WebPartInstance webPartInstance = null;

    /// <summary>
    /// Current page template.
    /// </summary>
    private PageTemplateInstance templateInstance = null;

    /// <summary>
    /// Tree provider.
    /// </summary>
    private TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);

    /// <summary>
    /// Gets web part from instance.
    /// </summary>
    private WebPartInfo wpi = null;

    /// <summary>
    /// Parent web part
    /// </summary>
    private WebPartInfo parentWpi = null;

    /// <summary>
    /// Indicates whether the new variant should be chosen when closing this dialog
    /// </summary>
    private bool selectNewVariant = false;

    /// <summary>
    /// Preferred culture code to use along with alias path.
    /// </summary>
    private string mCultureCode = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Page alias path.
    /// </summary>
    public string AliasPath
    {
        get
        {
            return mAliasPath;
        }
        set
        {
            mAliasPath = value;
        }
    }


    /// <summary>
    /// Preferred culture code to use along with alias path.
    /// </summary>
    public string CultureCode
    {
        get
        {
            if (string.IsNullOrEmpty(mCultureCode))
            {
                mCultureCode = CMSContext.PreferredCultureCode;
            }
            return mCultureCode;
        }
        set
        {
            mCultureCode = value;
        }
    }


    /// <summary>
    /// Page template ID.
    /// </summary>
    public int PageTemplateId
    {
        get
        {
            return mPageTemplateId;
        }
        set
        {
            mPageTemplateId = value;
        }
    }


    /// <summary>
    /// Zone ID.
    /// </summary>
    public string ZoneId
    {
        get
        {
            return mZoneId;
        }
        set
        {
            mZoneId = value;
        }
    }


    /// <summary>
    /// Web part ID.
    /// </summary>
    public string WebpartId
    {
        get
        {
            return mWebpartId;
        }
        set
        {
            mWebpartId = value;
        }
    }


    /// <summary>
    /// Instance GUID.
    /// </summary>
    public Guid InstanceGUID
    {
        get
        {
            return mInstanceGUID;
        }
        set
        {
            mInstanceGUID = value;
        }
    }


    /// <summary>
    /// True if the web part ID has changed.
    /// </summary>
    public bool WebPartIdChanged
    {
        get
        {
            return mWebPartIdChanged;
        }
    }


    /// <summary>
    /// Before form definition.
    /// </summary>
    public string BeforeFormDefinition
    {
        get
        {
            return mBeforeFormDefinition;
        }
        set
        {
            mBeforeFormDefinition = value;
        }
    }


    /// <summary>
    /// After form definition.
    /// </summary>
    public string AfterFormDefinition
    {
        get
        {
            return mAfterFormDefinition;
        }
        set
        {
            mAfterFormDefinition = value;
        }
    }


    /// <summary>
    /// Indicates whether the web part is new (inserting) or not (updating).
    /// </summary>
    public bool IsNewWebPart
    {
        get
        {
            return mIsNewWebPart;
        }
        set
        {
            mIsNewWebPart = value;
        }
    }


    /// <summary>
    /// Gets or sets the position of the inserted web part.
    /// </summary>
    public int Position
    {
        get
        {
            return mPosition;
        }
        set
        {
            mPosition = value;
        }
    }



    /// <summary>
    /// Relative position of the web part from the left
    /// </summary>
    public int PositionLeft
    {
        get;
        set;
    }


    /// <summary>
    /// Relative position of the web part from the top
    /// </summary>
    public int PositionTop
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether is a new variant.
    /// </summary>
    public bool IsNewVariant
    {
        get
        {
            return mIsNewVariant;
        }
        set
        {
            mIsNewVariant = value;
        }
    }


    /// <summary>
    /// Gets or sets the actual web part variant ID.
    /// </summary>
    public int VariantID
    {
        get
        {
            return mVariantId;
        }
        set
        {
            mVariantId = value;
        }
    }


    /// <summary>
    /// Gets or sets the web part zone variant ID.
    /// This property is set when adding a new webpart into the zone variant, in all other cases is set to 0.
    /// </summary>
    public int ZoneVariantID
    {
        get
        {
            return mZoneVariantId;
        }
        set
        {
            mZoneVariantId = value;
        }
    }


    /// <summary>
    /// Gets the variant mode. Indicates whether there are MVT/ContentPersonalization/None variants active.
    /// </summary>
    public VariantModeEnum VariantMode
    {
        get
        {
            return mVariantMode;
        }
        set
        {
            mVariantMode = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadForm();

        // Setup info/error message placeholder
        if (MessagesPlaceHolder != null)
        {
            MessagesPlaceHolder.UseRelativePlaceHolder = false;
            form.MessagesPlaceHolder = MessagesPlaceHolder;
        }

        ScriptHelper.RegisterScriptFile(Page, "cmsedit.js");
        ScriptHelper.RegisterJQuery(Page);
    }


    /// <summary>
    /// Control ID validation.
    /// </summary>
    private void formElem_OnItemValidation(object sender, ref string errorMessage)
    {
        Control ctrl = (Control)sender;
        if (ctrl.ID.ToLowerCSafe() == "webpartcontrolid")
        {
            FormEngineUserControl ctrlTextbox = (FormEngineUserControl)ctrl;
            string newId = ValidationHelper.GetString(ctrlTextbox.Value, null);

            // Validate unique ID
            WebPartInstance existingPart = pti.GetWebPart(newId);
            if ((existingPart != null) && ((webPartInstance == null) || (existingPart.InstanceGUID != webPartInstance.InstanceGUID)))
            {
                // Error - duplicity IDs
                errorMessage = GetString("WebPartProperties.ErrorUniqueID");
            }
            else
            {
                string uniqueId = WebPartZoneInstance.GetUniqueWebPartId(newId, pi.TemplateInstance);
                if (!uniqueId.EqualsCSafe(newId, true))
                {
                    // Check if there is already a widget with the same id in the page
                    existingPart = pi.TemplateInstance.GetWebPart(newId);
                    if ((existingPart != null) && existingPart.IsWidget)
                    {
                        // Error - the ID collide with another widget which is already in the page
                        errorMessage = ResHelper.GetString("WidgetProperties.ErrorUniqueID");
                    }
                }
            }
        }
    }


    protected void lnkLoadDefaults_Click(object sender, EventArgs e)
    {
        // Get the web part form info
        FormInfo fi = GetWebPartFormInfo();
        if (fi != null)
        {
            // Create DataRow with default data
            var dr = fi.GetDataRow();

            // Load default values
            fi.LoadDefaultValues(dr);

            // Load overridden system values
            fi.LoadDefaultValues(dr, wpi.WebPartDefaultValues);

            // Load to the form
            form.LoadData(dr);
        }
    }


    /// <summary>
    /// Loads the web part form.
    /// </summary>
    protected void LoadForm()
    {
        // Load settings
        if (!string.IsNullOrEmpty(Request.Form[hdnIsNewWebPart.UniqueID]))
        {
            IsNewWebPart = ValidationHelper.GetBoolean(Request.Form[hdnIsNewWebPart.UniqueID], false);
        }
        if (!string.IsNullOrEmpty(Request.Form[hdnInstanceGUID.UniqueID]))
        {
            InstanceGUID = ValidationHelper.GetGuid(Request.Form[hdnInstanceGUID.UniqueID], Guid.Empty);
        }

        // Indicates whether the new variant should be chosen when closing this dialog
        selectNewVariant = IsNewVariant;

        // Try to find the web part variant in the database and set its VariantID
        if (IsNewVariant)
        {
            Hashtable properties = WindowHelper.GetItem("variantProperties") as Hashtable;
            if (properties != null)
            {
                // Get the variant code name from the WindowHelper
                string variantName = ValidationHelper.GetString(properties["codename"], string.Empty);

                // Check if the variant exists in the database
                int variantIdFromDB = 0;
                if (VariantMode == VariantModeEnum.MVT)
                {
                    variantIdFromDB = ModuleCommands.OnlineMarketingGetMVTVariantId(PageTemplateId, variantName);
                }
                else if (VariantMode == VariantModeEnum.ContentPersonalization)
                {
                    variantIdFromDB = ModuleCommands.OnlineMarketingGetContentPersonalizationVariantId(PageTemplateId, variantName);
                }

                // Set the variant id from the database
                if (variantIdFromDB > 0)
                {
                    VariantID = variantIdFromDB;
                    IsNewVariant = false;
                }
            }
        }

        if (!String.IsNullOrEmpty(WebpartId))
        {
            // Get the page info
            pi = CMSWebPartPropertiesPage.GetPageInfo(AliasPath, PageTemplateId, CultureCode);
            if (pi != null)
            {
                // Get template
                pti = pi.UsedPageTemplateInfo;

                // Get template instance
                templateInstance = pti.TemplateInstance;

                if (!IsNewWebPart)
                {
                    // Standard zone
                    webPartInstance = templateInstance.GetWebPart(InstanceGUID, WebpartId);

                    // If the web part not found, try to find it among the MVT/CP variants
                    if (webPartInstance == null)
                    {
                        // MVT/CP variant
                        templateInstance.LoadVariants(false, VariantModeEnum.None);
                        webPartInstance = templateInstance.GetWebPart(InstanceGUID, true);

                        // Set the VariantMode according to the selected web part/zone variant
                        if ((webPartInstance != null) && (webPartInstance.ParentZone != null))
                        {
                            VariantMode = (webPartInstance.VariantMode != VariantModeEnum.None) ? webPartInstance.VariantMode : webPartInstance.ParentZone.VariantMode;
                        }
                        else
                        {
                            VariantMode = VariantModeEnum.None;
                        }
                    }
                    else
                    {
                        // Ensure that the ZoneVarianID is not set when the web part was found in a regular zone
                        ZoneVariantID = 0;
                    }

                    if ((VariantID > 0) && (webPartInstance != null) && (webPartInstance.PartInstanceVariants != null))
                    {
                        // Check OnlineMarketing permissions.
                        if (CheckPermissions("Read"))
                        {
                            webPartInstance = webPartInstance.FindVariant(VariantID);
                        }
                        else
                        {
                            // Not authorized for OnlineMarketing - Manage.
                            RedirectToInformation(String.Format(GetString("general.permissionresource"), "Read", (VariantMode == VariantModeEnum.ContentPersonalization) ? "CMS.ContentPersonalization" : "CMS.MVTest"));
                        }
                    }

                    if (webPartInstance == null)
                    {
                        CMSPage.EditedObject = null;
                        return;
                    }
                }

                // Keep xml version
                if (webPartInstance != null)
                {
                    xmlVersion = webPartInstance.XMLVersion;
                }

                // Get the form info
                FormInfo fi = GetWebPartFormInfo();

                // Get the form definition
                if (fi != null)
                {
                    form.AllowMacroEditing = ((WebPartTypeEnum)wpi.WebPartType != WebPartTypeEnum.Wireframe);

                    // Get data row with required columns
                    DataRow dr = fi.GetDataRow(true, wpi.WebPartDefaultValues);

                    if (IsNewWebPart || (xmlVersion > 0))
                    {
                        fi.LoadDefaultValues(dr);

                        // Load overridden system values
                        fi.LoadDefaultValues(dr, wpi.WebPartDefaultValues);
                    }

                    if (IsNewWebPart)
                    {
                        // Set control ID
                        FormFieldInfo ffi = fi.GetFormField("WebPartControlID");
                        if (ffi != null)
                        {
                            ffi.DefaultValue = WebPartZoneInstance.GetUniqueWebPartId(wpi.WebPartName, templateInstance);
                            fi.UpdateFormField("WebPartControlID", ffi);
                        }
                    }

                    // Load values from existing web part
                    LoadDataRowFromWebPart(dr, webPartInstance, fi);

                    // Set a unique WebPartControlID for the new variant
                    if (IsNewVariant)
                    {
                        // Set control ID
                        dr["WebPartControlID"] = WebPartZoneInstance.GetUniqueWebPartId(wpi.WebPartName, templateInstance);
                    }

                    // Init the form
                    InitForm(form, dr, fi);

                    AddExportLink();
                }
                else
                {
                    CMSPage.EditedObject = null;
                }
            }
        }
    }


    /// <summary>
    /// Loads the web part info
    /// </summary>
    private void EnsureWebPartInfo()
    {
        if (wpi != null)
        {
            return;
        }

        if (!IsNewWebPart)
        {
            // Get web part by code name
            wpi = WebPartInfoProvider.GetWebPartInfo(webPartInstance.WebPartType);
            form.Mode = FormModeEnum.Update;
        }
        else
        {
            // Web part instance wasn't created yet, get by web part ID
            wpi = WebPartInfoProvider.GetWebPartInfo(ValidationHelper.GetInteger(WebpartId, 0));
            form.Mode = FormModeEnum.Insert;
        }
    }


    /// <summary>
    /// Loads the parent web part based on the current web part object
    /// </summary>
    private void EnsureParentWebPartInfo()
    {
        if (parentWpi != null)
        {
            return;
        }

        // Ensure the web part
        EnsureWebPartInfo();

        if ((wpi != null) && (wpi.WebPartParentID > 0))
        {
            parentWpi = WebPartInfoProvider.GetWebPartInfo(wpi.WebPartParentID);
        }
    }


    /// <summary>
    /// Gets the form info for the given web part
    /// </summary>
    private FormInfo GetWebPartFormInfo()
    {
        // Load parent
        EnsureParentWebPartInfo();

        if (wpi == null)
        {
            return null;
        }

        string wpProperties = wpi.WebPartProperties;

        // Use parent web part if is defined
        if (parentWpi != null)
        {
            wpProperties = parentWpi.WebPartProperties;
        }

        // Get before FormInfo
        FormInfo beforeFI = null;

        if (BeforeFormDefinition == null)
        {
            beforeFI = PortalFormHelper.GetPositionFormInfo((WebPartTypeEnum)wpi.WebPartType, PropertiesPosition.Before);
        }
        else
        {
            beforeFI = new FormInfo(BeforeFormDefinition);
        }

        // Get after FormInfo
        FormInfo afterFI = null;

        if (AfterFormDefinition == null)
        {
            afterFI = PortalFormHelper.GetPositionFormInfo((WebPartTypeEnum)wpi.WebPartType, PropertiesPosition.After);
        }
        else
        {
            afterFI = new FormInfo(AfterFormDefinition);
        }

        // Add 'General' category at the beginning if no one is specified
        if (!string.IsNullOrEmpty(wpProperties) && (!wpProperties.StartsWithCSafe("<form><category", true)))
        {
            wpProperties = wpProperties.Insert(6, "<category name=\"" + GetString("general.general") + "\" />");
        }

        // Get merged web part FormInfo 
        FormInfo fi = PortalFormHelper.GetWebPartFormInfo(wpi.WebPartName, wpProperties, beforeFI, afterFI, true);

        return fi;
    }


    /// <summary>
    /// Saves the given form.
    /// </summary>
    /// <param name="form">Form to save</param>
    private static bool SaveForm(BasicForm form)
    {
        if ((form != null) && form.Visible)
        {
            return form.SaveData("");
        }

        return true;
    }


    /// <summary>
    /// Checks permissions (depends on variant mode) 
    /// </summary>
    /// <param name="permissionName">Name of permission to test</param>
    private bool CheckPermissions(string permissionName)
    {
        CurrentUserInfo cui = CMSContext.CurrentUser;
        switch (VariantMode)
        {
            case VariantModeEnum.MVT:
                return cui.IsAuthorizedPerResource("cms.mvtest", permissionName);

            case VariantModeEnum.ContentPersonalization:
                return cui.IsAuthorizedPerResource("cms.contentpersonalization", permissionName);

            case VariantModeEnum.Conflicted:
            case VariantModeEnum.None:
                return cui.IsAuthorizedPerResource("cms.mvtest", permissionName) || cui.IsAuthorizedPerResource("cms.contentpersonalization", permissionName);
        }

        return true;
    }


    /// <summary>
    /// Saves webpart properties.
    /// </summary>
    public bool Save()
    {
        // Check MVT/CP security
        if (VariantID > 0)
        {
            // Check OnlineMarketing permissions.
            if (!CheckPermissions("Manage"))
            {
                ShowError("general.modifynotallowed");
                return false;
            }
        }

        // Save the data
        if ((pi != null) && (pti != null) && (templateInstance != null) && SaveForm(form))
        {
            if (CMSObjectHelper.IsCheckedOutByOtherUser(pti))
            {
                string userName = null;
                UserInfo ui = UserInfoProvider.GetUserInfo(pti.Generalized.IsCheckedOutByUserID);
                if (ui != null)
                {
                    userName = HTMLHelper.HTMLEncode(ui.GetFormattedUserName(IsLiveSite));
                }

                ShowError(string.Format(GetString("ObjectEditMenu.CheckedOutByAnotherUser"), pti.ObjectType, pti.DisplayName, userName));
                return false;
            }

            // Add web part if new
            if (IsNewWebPart)
            {
                int webpartId = ValidationHelper.GetInteger(WebpartId, 0);

                // Ensure layout zone flag
                if (QueryHelper.GetBoolean("layoutzone", false))
                {
                    WebPartZoneInstance zone = pti.EnsureZone(ZoneId);
                    zone.LayoutZone = true;
                }

                webPartInstance = PortalHelper.AddNewWebPart(webpartId, ZoneId, false, ZoneVariantID, Position, templateInstance, null);
            }

            webPartInstance.XMLVersion = 1;
            WebPartInstance originalWebPartInstance = webPartInstance;
            if (IsNewVariant)
            {
                webPartInstance = webPartInstance.Clone();
                webPartInstance.VariantMode = VariantModeFunctions.GetVariantModeEnum(QueryHelper.GetString("variantmode", String.Empty).ToLowerCSafe());
            }

            // Get basic form's data row and update web part
            SaveFormToWebPart(form);

            // Set new position if set
            if (PositionLeft > 0)
            {
                webPartInstance.SetValue("PositionLeft", PositionLeft);
            }
            if (PositionTop > 0)
            {
                webPartInstance.SetValue("PositionTop", PositionTop);
            }

            bool isWebPartVariant = (VariantID > 0) || (ZoneVariantID > 0) || IsNewVariant;
            if (!isWebPartVariant)
            {
                // Save the changes  
                CMSPortalManager.SaveTemplateChanges(pi, templateInstance, WidgetZoneTypeEnum.None, ViewModeEnum.Design, tree);
            }
            else
            {
                Hashtable properties = WindowHelper.GetItem("variantProperties") as Hashtable;
                // Save changes to the web part variant
                PortalHelper.SaveWebPartVariantChanges(webPartInstance, VariantID, ZoneVariantID, VariantMode, properties);
            }

            // Reload the form (because of macro values set only by JS)
            form.ReloadData();

            // Clear the cached web part
            if (InstanceGUID != null)
            {
                CacheHelper.TouchKey("webpartinstance|" + InstanceGUID.ToString().ToLowerCSafe());
            }

            ShowChangesSaved();

            return true;
        }
        else if ((webPartInstance != null) && (webPartInstance.ParentZone != null) && (webPartInstance.ParentZone.ParentTemplateInstance != null))
        {
            // Reload the zone/web part variants when saving of the form fails
            webPartInstance.ParentZone.ParentTemplateInstance.LoadVariants(true, VariantModeEnum.None);
        }

        return false;
    }


    /// <summary>
    /// Saves the given DataRow data to the web part properties.
    /// </summary>
    /// <param name="form">Form to save</param>
    private void SaveFormToWebPart(BasicForm form)
    {
        if (form.Visible && (webPartInstance != null))
        {
            // Keep the old ID to check the change of the ID
            string oldId = webPartInstance.ControlID.ToLowerCSafe();

            DataRow dr = form.DataRow;
            foreach (DataColumn column in dr.Table.Columns)
            {
                webPartInstance.MacroTable[column.ColumnName.ToLowerCSafe()] = form.MacroTable[column.ColumnName.ToLowerCSafe()];
                webPartInstance.SetValue(column.ColumnName, dr[column]);

                // If name changed, move the content
                if (column.ColumnName.ToLowerCSafe() == "webpartcontrolid")
                {
                    try
                    {
                        string newId = null;
                        if (!IsNewVariant)
                        {
                            newId = ValidationHelper.GetString(dr[column], "").ToLowerCSafe();
                        }

                        // Name changed
                        if ((!string.IsNullOrEmpty(newId)) && (newId != oldId))
                        {
                            if (!IsNewWebPart && !IsNewVariant)
                            {
                                mWebPartIdChanged = true;
                            }
                            WebpartId = newId;

                            // Move the document content if present
                            string currentContent = (string)(pi.EditableWebParts[oldId]);
                            if (currentContent != null)
                            {
                                TreeNode node = DocumentHelper.GetDocument(pi.DocumentID, tree);

                                // Move the content in the page info
                                pi.EditableWebParts[oldId] = null;
                                pi.EditableWebParts[newId] = currentContent;

                                // Update the document
                                node.SetValue("DocumentContent", pi.GetContentXml());
                                DocumentHelper.UpdateDocument(node, tree);
                            }

                            // Change the underlying zone names if layout web part
                            if ((wpi != null) && ((WebPartTypeEnum)wpi.WebPartType == WebPartTypeEnum.Layout))
                            {
                                string prefix = oldId + "_";

                                foreach (WebPartZoneInstance zone in pti.WebPartZones)
                                {
                                    if (zone.ZoneID.StartsWithCSafe(prefix, true))
                                    {
                                        // Change the zone prefix to the new one
                                        zone.ZoneID = newId + "_" + zone.ZoneID.Substring(prefix.Length);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogProvider ev = new EventLogProvider();
                        ev.LogEvent("Content", "CHANGEWEBPART", ex);
                    }
                }
            }

            // Save the collapsed/un-collapsed state of categories
            FormInfo fi = GetWebPartFormInfo();

            var categories = fi.GetCategoryNames();
            foreach (string category in categories)
            {
                FormCategoryInfo fci = fi.GetFormCategory(category);
                if (fci.CategoryCollapsible && fci.CategoryCollapsedByDefault)
                {
                    if (form.IsCategoryCollapsed(category))
                    {
                        webPartInstance.SetValue("cat_open_" + category, null);
                    }
                    else
                    {
                        webPartInstance.SetValue("cat_open_" + category, true);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Loads the data row data from given web part instance.
    /// </summary>
    /// <param name="dr">DataRow to fill</param>
    /// <param name="webPart">Source web part</param>
    private void LoadDataRowFromWebPart(DataRow dr, WebPartInstance webPart, FormInfo fi)
    {
        foreach (DataColumn column in dr.Table.Columns)
        {
            try
            {
                bool load = true;
                // switch by xml version
                switch (xmlVersion)
                {
                    case 1:
                        load = webPart.Properties.Contains(column.ColumnName.ToLowerCSafe()) || column.ColumnName.EqualsCSafe("webpartcontrolid", true);
                        break;
                    // Version 0
                    default:
                        // Load default value for Boolean type in old XML version
                        if ((column.DataType == typeof(bool)) && !webPart.Properties.Contains(column.ColumnName.ToLowerCSafe()))
                        {
                            FormFieldInfo ffi = fi.GetFormField(column.ColumnName);
                            if (ffi != null)
                            {
                                webPart.SetValue(column.ColumnName, ffi.DefaultValue);
                            }
                        }
                        break;
                }

                if (load)
                {
                    object value = webPart.GetValue(column.ColumnName);

                    // Convert value into default format
                    if ((value != null) && (ValidationHelper.GetString(value, String.Empty) != String.Empty))
                    {
                        if (column.DataType == typeof(decimal))
                        {
                            value = ValidationHelper.GetDouble(value, 0, "en-us");
                        }

                        if (column.DataType == typeof(DateTime))
                        {
                            value = ValidationHelper.GetDateTime(value, DateTime.MinValue, "en-us");
                        }
                    }

                    DataHelper.SetDataRowValue(dr, column.ColumnName, value);
                }
            }
            catch
            {
            }
        }
    }


    /// <summary>
    /// Initializes the form.
    /// </summary>
    /// <param name="form">Form</param>
    /// <param name="dr">Data row with the data</param>
    /// <param name="fi">Form info</param>
    private void InitForm(BasicForm form, DataRow dr, FormInfo fi)
    {
        if (form != null)
        {
            form.DataRow = dr;
            if (webPartInstance != null)
            {
                form.MacroTable = webPartInstance.MacroTable;
            }
            else
            {
                form.MacroTable = new Hashtable();
            }

            if (!RequestHelper.IsPostBack() && (webPartInstance != null))
            {
                fi = new FormInfo(fi.GetXmlDefinition());

                // Load the collapsed/un-collapsed state of categories
                var categories = fi.GetCategoryNames();
                foreach (string category in categories)
                {
                    FormCategoryInfo fci = fi.GetFormCategory(category);
                    if (fci.CategoryCollapsible && fci.CategoryCollapsedByDefault && ValidationHelper.GetBoolean(webPartInstance.GetValue("cat_open_" + category), false))
                    {
                        fci.CategoryCollapsedByDefault = false;
                    }
                }
            }

            form.SubmitButton.Visible = false;
            form.SiteName = CMSContext.CurrentSiteName;
            form.FormInformation = fi;
            form.ShowPrivateFields = true;
            form.OnItemValidation += formElem_OnItemValidation;
            form.ReloadData();
        }
    }


    /// <summary>
    /// Raised when the Save action is required.
    /// </summary>
    public bool OnSave()
    {
        if (Save())
        {
            hdnIsNewWebPart.Value = "false";

            if (webPartInstance != null)
            {
                hdnInstanceGUID.Value = webPartInstance.InstanceGUID.ToString();

                if (selectNewVariant)
                {
                    // Select the new variant
                    string script = "SendEvent('updatevariantposition', true, { itemCode: 'Variant_WP_" + webPartInstance.InstanceGUID.ToString("N") + "', variantId: -1 }); ";

                    ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CustomScripts", script, true);
                }
            }

            AddExportLink();

            return true;
        }

        return false;
    }


    /// <summary>
    /// Adds the export link.
    /// </summary>
    private void AddExportLink()
    {
        if (webPartInstance != null)
        {
            ltlExport.Text = "&nbsp;<a target=\"_parent\" href=\"GetWebPartProperties.aspx?webpartid=" + webPartInstance.ControlID + "&webpartguid=" + webPartInstance.InstanceGUID + "&aliaspath=" + AliasPath + "&zoneid=" + ZoneId + "\">" + GetString("WebpartProperties.Export") + "</a>";
        }
    }

    #endregion
}
