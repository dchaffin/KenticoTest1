using System;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;

public partial class CMSFormControls_Classes_SelectTransformation : FormEngineUserControl
{
    #region "Variables"

    private bool mDisplayClearButton = true;
    private string mNewDialogPath = "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_New.aspx";
    private bool mShowHierarchicalTransformation = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// If true, control is in site manager.
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return uniSelector.IsSiteManager;
        }
        set
        {
            uniSelector.IsSiteManager = value;
        }
    }


    /// <summary>
    /// If true selector shows hierarchical transformation.
    /// </summary>
    public bool ShowHierarchicalTransformation
    {
        get
        {
            return mShowHierarchicalTransformation;
        }
        set
        {
            mShowHierarchicalTransformation = value;
        }
    }


    /// <summary>
    /// Returns ClientID of the textbox with transformation.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Name of the edit window.
    /// </summary>
    public string EditWindowName
    {
        get
        {
            return uniSelector.EditWindowName;
        }
        set
        {
            uniSelector.EditWindowName = value;
        }
    }


    /// <summary>
    /// Path to the dialog for uniselector.
    /// </summary>
    public string NewDialogPath
    {
        get
        {
            return mNewDialogPath;
        }
        set
        {
            mNewDialogPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to display Clear button.
    /// </summary>
    public bool DisplayClearButton
    {
        get
        {
            return mDisplayClearButton;
        }
        set
        {
            mDisplayClearButton = value;
            if (uniSelector != null)
            {
                uniSelector.AllowEmpty = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the codename of setting key with default value
    /// </summary>
    public string WatermarkValueSettingKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WatermarkValueSettingKey"), null);
        }
        set
        {
            SetValue("WatermarkValueSettingKey", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.ButtonClear.Visible = false;
        uniSelector.AllowEmpty = DisplayClearButton;
        uniSelector.SetValue("FilterMode", PortalObjectType.TRANSFORMATION);
        uniSelector.EditDialogWindowWidth = 1200;

        // Set default value from settings as textbox watermark
        if (!String.IsNullOrEmpty(WatermarkValueSettingKey))
        {
            string watermark = SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + "." + WatermarkValueSettingKey);
            if (!String.IsNullOrEmpty(watermark))
            {
                uniSelector.TextBoxSelect.WatermarkText = watermark;
            }
        }

        // Check if user can edit the transformation
        CurrentUserInfo currentUser = CMSContext.CurrentUser;
        bool deskAuthorized = currentUser.IsAuthorizedPerUIElement("CMS.Desk", "Content");
        bool contentAuthorized = currentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Design", "Design.WebPartProperties" }, CMSContext.CurrentSiteName);

        if (deskAuthorized && contentAuthorized)
        {
            bool editAuthorized = currentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "WebPartProperties.EditTransformations" }, CMSContext.CurrentSiteName);
            bool createAuthorized = currentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "WebPartProperties.NewTransformations" }, CMSContext.CurrentSiteName);

            // Alias path for preview transformation
            String aliasPath = QueryHelper.GetString("aliaspath", String.Empty);
            String aliasPathParam = (aliasPath == String.Empty) ? "" : "&aliaspath=" + aliasPath;

            // Instance GUID
            String instanceGUID = QueryHelper.GetString("instanceGUID", String.Empty);
            String instanceGUIDParam = (instanceGUID == String.Empty) ? "" : "&instanceguid=" + instanceGUID;

            // Transformation editing authorized
            if (editAuthorized)
            {
                string isSiteManagerStr = IsSiteManager ? "&siteManager=true" : String.Empty;
                string url = "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_Frameset.aspx?name=##ITEMID##" + isSiteManagerStr + "&editonlycode=1" + aliasPathParam + instanceGUIDParam;
                url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash("?editonlycode=1"));
                uniSelector.EditItemPageUrl = url;
            }

            // Creating of new transformation authorized
            if (createAuthorized)
            {
                string isSiteManagerStr = IsSiteManager ? "&siteManager=true" : String.Empty;
                string url = NewDialogPath + "?editonlycode=1" + isSiteManagerStr + "&selectedvalue=##ITEMID##" + aliasPathParam + instanceGUIDParam;
                url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash("?editonlycode=1"));
                uniSelector.NewItemPageUrl = url;
                uniSelector.EditDialogWindowHeight = 760;
            }
        }

        if (!ShowHierarchicalTransformation)
        {
            uniSelector.WhereCondition = "(TransformationIsHierarchical IS NULL) OR (TransformationIsHierarchical = 0)";
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // If macro or special value, do not validate
        string value = uniSelector.TextBoxSelect.Text.Trim();
        if (!ContextResolver.ContainsMacro(value) && (value != string.Empty))
        {
            // Check if culture exists
            TransformationInfo ti = TransformationInfoProvider.GetTransformation(value);
            if (ti == null)
            {
                ValidationError = GetString("formcontrols_selecttransformation.notexist").Replace("%%code%%", value);
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }

    #endregion
}