using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UniSelector_UniSelector : UniSelector, IPostBackEventHandler, ICallbackEventHandler
{
    #region "Private variables"

    private bool registerScripts = false;
    private string checkBoxClass = null;

    private TextHelper th = new TextHelper();
    private Hashtable parameters = new Hashtable();
    private GeneralizedInfo objectType = null;
    private GeneralizedInfo listingObject = null;
    private string mListingObjectType = string.Empty;
    private string mListingWhereCondition = String.Empty;
    private DataSet result = null;

    private bool mLoaded = false;
    private bool mPageChanged = false;
    private int mNewCurrentPage = 0;

    private string mEditWindowName = "EditWindow";
    private int mEditDialogWindowWidth = 1000;
    private int mEditDialogWindowHeight = 700;
    private bool mDynamicFirstColumnName = true;
    private bool mIsSiteManager = false;
    private Dictionary<int, string> mPriorityStyles = new Dictionary<int, string>();

    private StringBuilder javaScript = new StringBuilder();

    private bool? mButtonNewEnabled = null;
    private bool? mButtonEditEnabled = null;
    private bool? mButtonRemoveEnabled = null;

    #endregion


    #region "Delegates and events"

    /// <summary>
    /// Delegate for additional data bound event.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="sourceName">Source name for grid column</param>
    /// <param name="parameter">Column (row) grid values</param>
    /// <param name="value">Value given from classic grid data bound event</param>
    public delegate object AdditionalDataBoundEventHandler(object sender, string sourceName, object parameter, string value);


    /// <summary>
    /// Event for additional manipulation with uni-selector grid.
    /// </summary>
    public event AdditionalDataBoundEventHandler OnAdditionalDataBound;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Object of the specific given type.
    /// </summary>
    protected GeneralizedInfo Object
    {
        get
        {
            // Make sure that objects specified by given object type exist
            if ((objectType == null) && !String.IsNullOrEmpty(ObjectType))
            {
                objectType = CMSObjectHelper.GetObject(ObjectType);
            }

            return objectType;
        }
    }


    /// <summary>
    /// Type of alternative info object used in multiple picker grid view.
    /// </summary>
    public GeneralizedInfo ListingObject
    {
        get
        {
            // Make sure that objects specified by given object type exist
            if ((listingObject == null) && !String.IsNullOrEmpty(ListingObjectType))
            {
                listingObject = CMSObjectHelper.GetObject(ListingObjectType);
            }

            return listingObject;
        }
    }


    /// <summary>
    /// If true first column is dynamically named by object type info.
    /// </summary>
    public bool DynamicColumnName
    {
        get
        {
            return mDynamicFirstColumnName;
        }
        set
        {
            mDynamicFirstColumnName = value;
        }
    }


    /// <summary>
    /// If true, uni-selector is used in site manager.
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return mIsSiteManager;
        }
        set
        {
            mIsSiteManager = value;
        }
    }


    /// <summary>
    /// Name of alternative grid view listing object type.
    /// </summary>
    public string ListingObjectType
    {
        get
        {
            return mListingObjectType;
        }
        set
        {
            mListingObjectType = value;
        }
    }


    /// <summary>
    /// Name of alternative grid view listing object type.
    /// </summary>
    public string ListingWhereCondition
    {
        get
        {
            return mListingWhereCondition;
        }
        set
        {
            mListingWhereCondition = value;
        }
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    protected string Identifier
    {
        get
        {
            return EnsureIdentifier();
        }
    }


    /// <summary>
    /// Gets the value that indicates whether current selector in multiple mode displays some data or whether the dropdown contains some data.
    /// </summary>
    public override bool HasData
    {
        get
        {
            // Ensure the data
            if (!StopProcessing)
            {
                Reload(false);
            }

            return ValidationHelper.GetBoolean(ViewState["HasData"], false);
        }
        protected set
        {
            ViewState["HasData"] = value;
        }
    }


    /// <summary>
    /// Zero rows text.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ZeroRowsText"], null);
        }
        set
        {
            ViewState["ZeroRowsText"] = value;
        }
    }


    /// <summary>
    /// Name of the edit window.
    /// </summary>
    public string EditWindowName
    {
        get
        {
            return mEditWindowName;
        }
        set
        {
            mEditWindowName = value;
        }
    }


    /// <summary>
    /// Gets or sets the width of modal dialog window used for editing.
    /// </summary>
    public int EditDialogWindowWidth
    {
        get
        {
            return mEditDialogWindowWidth;
        }
        set
        {
            mEditDialogWindowWidth = value;
        }
    }


    /// <summary>
    /// Gets or sets the height of modal dialog window used for editing.
    /// </summary>
    public int EditDialogWindowHeight
    {
        get
        {
            return mEditDialogWindowHeight;
        }
        set
        {
            mEditDialogWindowHeight = value;
        }
    }


    /// <summary>
    /// Value of the "(all)" DDL record, -1 by default.
    /// </summary>
    public string AllRecordValue
    {
        get
        {
            return ValidationHelper.GetString(ViewState["AllRecordValue"], US_ALL_RECORDS.ToString());
        }
        set
        {
            ViewState["AllRecordValue"] = value;
        }
    }


    /// <summary>
    /// Value of the "(none)" DDL record, 0 by default.
    /// </summary>
    public string NoneRecordValue
    {
        get
        {
            return ValidationHelper.GetString(ViewState["NoneRecordValue"], US_NONE_RECORD.ToString());
        }
        set
        {
            ViewState["NoneRecordValue"] = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return GetValue();
        }
        set
        {
            SetValue(value);
        }
    }


    /// <summary>
    /// Gets the display name of the value item. Returns null if display name is not available.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return GetValueDisplayName();
        }
    }


    /// <summary>
    /// Gets or sets if control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["Enabled"], true);
        }
        set
        {
            ViewState["Enabled"] = value;
            SetEnabled();
        }
    }


    /// <summary>
    /// Sets a value indicating whether the New button should be enabled.
    /// </summary>
    public bool ButtonNewEnabled
    {
        set
        {
            mButtonNewEnabled = value;
        }
    }


    /// <summary>
    /// Sets a value indicating whether the Edit button should be enabled.
    /// </summary>
    public bool ButtonEditEnabled
    {
        set
        {
            mButtonEditEnabled = value;
        }
    }


    /// <summary>
    /// Sets a value indicating whether the Remove button should be enabled.
    /// </summary>
    public bool ButtonRemoveEnabled
    {
        set
        {
            mButtonRemoveEnabled = value;
        }
    }


    /// <summary>
    /// Indicates if UniSelector value was set.
    /// </summary>
    public new bool HasValue
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["HasValue"], false);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the UniSelector should apply WhereCondition for the selected value (default: false). This does not affect the modal dialog.
    /// </summary>
    public bool ApplyValueRestrictions
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ApplyValueRestrictions"], true);
        }
        set
        {
            ViewState["ApplyValueRestrictions"] = value;
        }
    }


    /// <summary>
    /// Sets minimum priority for which items will be shown.
    /// </summary>
    public int? MinimumPriority
    {
        get;
        set;
    }


    /// <summary>
    /// Defines whether to prioritize items or not.
    /// </summary>
    public bool PrioritizeItems
    {
        get;
        set;
    }


    /// <summary>
    /// Dictionary of css classes of prioritized items. 
    /// </summary>
    public Dictionary<int, string> PriorityStyles
    {
        get
        {
            return mPriorityStyles;
        }
    }


    /// <summary>
    /// Gets or sets site name. If set, only objects which belong to specified site are retrieved (if the object has SiteID column). If null or empty, all objects are retrieved.
    /// Use #currentsite for CurrentSite and #global for global objects.
    /// </summary>
    public string ObjectSiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectSiteName"), null);
        }
        set
        {
            SetValue("ObjectSiteName", value);
        }
    }

    #endregion


    #region "Controls properties"

    /// <summary>
    /// Gets the single select drop down field.
    /// </summary>
    public DropDownList DropDownSingleSelect
    {
        get
        {
            return drpSingleSelect;
        }
    }


    /// <summary>
    /// Drop down list selection edit button.
    /// </summary>
    public UniButton ButtonDropDownEdit
    {
        get
        {
            return btnDropEdit;
        }
    }


    /// <summary>
    /// Gets the Select button control.
    /// </summary>
    public UniButton ButtonSelect
    {
        get
        {
            return btnSelect;
        }
    }


    /// <summary>
    /// Gets the Clear button control.
    /// </summary>
    public UniButton ButtonClear
    {
        get
        {
            return btnClear;
        }
    }


    /// <summary>
    /// Gets the Remove selected items button.
    /// </summary>
    public Button ButtonRemoveSelected
    {
        get
        {
            return btnRemoveSelected;
        }
    }


    /// <summary>
    /// Gets the Add items button control.
    /// </summary>
    public Button ButtonAddItems
    {
        get
        {
            return btnAddItems;
        }
    }


    /// <summary>
    /// Gets the text box selection control.
    /// </summary>
    public override CMSTextBox TextBoxSelect
    {
        get
        {
            return txtSingleSelect;
        }
    }


    /// <summary>
    /// Textbox selection edit button.
    /// </summary>
    public UniButton ButtonEdit
    {
        get
        {
            return btnEdit;
        }
    }


    /// <summary>
    /// Multiple selection grid.
    /// </summary>
    public UniGrid UniGrid
    {
        get
        {
            return uniGrid;
        }
    }


    /// <summary>
    /// Button selection control.
    /// </summary>
    public override Button DialogButton
    {
        get
        {
            return btnDialog;
        }
    }


    /// <summary>
    /// Label selection control.
    /// </summary>
    public override LocalizedLabel DialogLabel
    {
        get
        {
            return lblLinkDialog;
        }
    }


    /// <summary>
    /// Link selection control.
    /// </summary>
    public override HyperLink DialogLink
    {
        get
        {
            return lnkDialog;
        }
    }


    /// <summary>
    /// Link selection image control.
    /// </summary>
    public override Image DialogImage
    {
        get
        {
            return imgDialog;
        }
    }


    /// <summary>
    /// New item button.
    /// </summary>
    public UniButton ButtonDropDownNew
    {
        get
        {
            return btnDropNew;
        }
    }

    #endregion


    #region "Methods"

    private void SetEnabled()
    {
        btnClear.Enabled = Enabled;
        btnSelect.Enabled = Enabled;
        btnDialog.Enabled = Enabled;
        lnkDialog.Enabled = Enabled;
        drpSingleSelect.Enabled = Enabled;
        txtSingleSelect.Enabled = Enabled;
        btnRemoveSelected.Enabled = Enabled;
        btnAddItems.Enabled = Enabled;
        btnMenu.Enabled = Enabled;
        btnEdit.Enabled = Enabled;
        btnNew.Enabled = Enabled;
        btnDropNew.Enabled = Enabled;
        btnDropEdit.Enabled = Enabled;
    }


    /// <summary>
    /// Gets the display name of the selected value
    /// </summary>
    private string GetValueDisplayName()
    {
        switch (SelectionMode)
        {
            case SelectionModeEnum.SingleDropDownList:
                {
                    // For dropdown, return the selected text
                    if (drpSingleSelect.SelectedIndex >= 0)
                    {
                        return drpSingleSelect.Items[drpSingleSelect.SelectedIndex].Text;
                    }
                    else
                    {
                        return drpSingleSelect.Text;
                    }
                }


            case SelectionModeEnum.SingleTextBox:
            case SelectionModeEnum.MultipleButton:
            case SelectionModeEnum.Multiple:
            case SelectionModeEnum.MultipleTextBox:
            case SelectionModeEnum.SingleButton:
                // Other types of selection - Get through the data items
                return GetDisplayName(Value);
        }

        return null;
    }


    /// <summary>
    /// Ensures the unique control identifier
    /// </summary>
    private string EnsureIdentifier()
    {
        string identifier = hdnIdentifier.Value;
        if (string.IsNullOrEmpty(identifier))
        {
            identifier = Request.Form[hdnIdentifier.UniqueID];
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = Guid.NewGuid().ToString();
            }
            hdnIdentifier.Value = identifier;
        }

        return identifier;
    }


    /// <summary>
    /// Sets the value to the selector
    /// </summary>
    /// <param name="value">New value</param>
    private void SetValue(object value)
    {
        // Reset the control if the value is null
        if (value == null)
        {
            hdnDialogSelect.Value = String.Empty;
            txtSingleSelect.Text = String.Empty;
            hiddenField.Value = String.Empty;
            drpSingleSelect.SelectedIndex = -1;
            ViewState["HasValue"] = false;

            return;
        }

        switch (SelectionMode)
        {
            // Dropdown mode
            case SelectionModeEnum.SingleDropDownList:
                hdnDialogSelect.Value = ValidationHelper.GetString(value, "");
                if (drpSingleSelect.Items.FindByValue(hdnDialogSelect.Value) != null)
                {
                    drpSingleSelect.SelectedValue = hdnDialogSelect.Value;
                }
                break;

            // Textbox mode
            case SelectionModeEnum.SingleTextBox:
            case SelectionModeEnum.MultipleTextBox:
                string text = ValidationHelper.GetString(value, String.Empty).Trim(ValuesSeparator.ToCharArray());
                if (AllowEditTextBox)
                {
                    txtSingleSelect.Text = text;
                }
                hiddenField.Value = String.Format(";{0};", text);
                break;

            // Other modes
            default:
                hiddenField.Value = ";" + ValidationHelper.GetString(value, "").Trim(ValuesSeparator.ToCharArray()) + ";";
                break;
        }
        ViewState["HasValue"] = true;
    }


    /// <summary>
    /// Gets the current value of the selector
    /// </summary>
    private object GetValue()
    {
        switch (SelectionMode)
        {
            // Dropdown mode
            case SelectionModeEnum.SingleDropDownList:
                if (!String.IsNullOrEmpty(hdnDialogSelect.Value))
                {
                    return hdnDialogSelect.Value;
                }
                else
                {
                    return drpSingleSelect.SelectedValue;
                }

            // Textbox mode
            case SelectionModeEnum.SingleTextBox:
            case SelectionModeEnum.MultipleTextBox:
                if (AllowEditTextBox)
                {
                    return txtSingleSelect.Text.Trim(ValuesSeparator.ToCharArray());
                }
                else
                {
                    return hiddenField.Value.Trim(ValuesSeparator.ToCharArray());
                }

            // Other modes
            default:
                return hiddenField.Value.Trim(ValuesSeparator.ToCharArray());
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }


    protected override void LoadViewState(object savedState)
    {
        base.LoadViewState(savedState);

        // Do not overwrite value if ViewState doesn't contain value for current control (e.g. disabled control)
        if (Request.Form.AllKeys.Contains(hdnIdentifier.UniqueID))
        {
            // Get values from form if the control is loaded dynamically
            hdnIdentifier.Value = Request.Form[hdnIdentifier.UniqueID];
        }
        if (Request.Form.AllKeys.Contains(hiddenField.UniqueID))
        {
            hiddenField.Value = Request.Form[hiddenField.UniqueID];
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (RequestHelper.IsCallback())
        {
            StopProcessing = true;
            return;
        }

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);
        ScriptHelper.RegisterWOpenerScript(Page);

        checkBoxClass = Guid.NewGuid().ToString().Replace("-", "");

        // Bound events
        drpSingleSelect.SelectedIndexChanged += drpSingleSelect_SelectedIndexChanged;

        uniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
        uniGrid.OnPageChanged += uniGrid_OnPageChanged;
        uniGrid.IsLiveSite = IsLiveSite;

        uniGrid.Pager.DefaultPageSize = 10;

        btnClear.Click += btnClear_Click;

        // If control is enabled, then display content
        if (!StopProcessing && (SelectionMode == SelectionModeEnum.Multiple))
        {
            if (!RequestHelper.CausedPostback(btnRemoveSelected, this, uniGrid.Pager.UniPager))
            {
                Reload(false);
            }
        }
    }


    /// <summary>
    /// Change header title for multiple selection.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // If control is enabled, then display content
        if ((SelectionMode != SelectionModeEnum.Multiple) || !mLoaded)
        {
            Reload(false);
        }

        if (registerScripts)
        {
            RegisterScripts();
        }

        // Display two columns when in multiple selection
        if ((SelectionMode == SelectionModeEnum.Multiple) && (uniGrid.GridView.HeaderRow != null))
        {
            CheckBox chkAll = new CheckBox();
            chkAll.ID = "chkAll";
            chkAll.ToolTip = GetString("General.CheckAll");
            chkAll.InputAttributes.Add("onclick", "US_SelectAllItems('" + ClientID + "', '" + ValuesSeparator + "', this, 'chk" + checkBoxClass + "')");
            chkAll.Enabled = Enabled;

            uniGrid.GridView.HeaderRow.Cells[0].Controls.Clear();
            uniGrid.GridView.HeaderRow.Cells[0].Controls.Add(chkAll);
            uniGrid.GridView.Columns[0].ItemStyle.CssClass = "UnigridSelection";

            if (DynamicColumnName)
            {
                uniGrid.GridView.HeaderRow.Cells[1].Text = GetString(ResourcePrefix + ".itemname|general.itemname");
            }
        }

        lblStatus.Visible = !String.IsNullOrEmpty(lblStatus.Text);

        // If the page was not changed, deselect all
        if (!mPageChanged)
        {
            hiddenSelected.Value = "";
        }

        // Always reset the new value from dialog
        hdnDialogSelect.Value = "";

        InitializeControlEnvelope();

        // Enable/disable buttons when manually set
        SetButtonsEnabled();

        base.OnPreRender(e);
    }


    /// <summary>   
    /// Initializes control envelope used in form controls definition.
    /// </summary>
    private void InitializeControlEnvelope()
    {
        if (!String.IsNullOrEmpty(CssClass))
        {
            if (!String.IsNullOrEmpty(ControlStyle))
            {
                ltlContentBefore.Text = String.Format("<div class=\"{0}\" style=\"{1}\" >", CssClass, ControlStyle);
                ltlContentAfter.Text = "</div>";

            }
            else
            {
                ltlContentBefore.Text = String.Format("<div class=\"{0}\" >", CssClass);
                ltlContentAfter.Text = "</div>";
            }
        }
        else
        {
            if (!String.IsNullOrEmpty(ControlStyle))
            {
                ltlContentBefore.Text = String.Format("<div style=\"{0}\" >", ControlStyle);
                ltlContentAfter.Text = "</div>";
            }
            else
            {
                ltlContentBefore.Visible = false;
                ltlContentAfter.Visible = false;
            }
        }
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if (StopProcessing)
        {
            return;
        }
        // Render starting tag of wrapping element
        switch (SelectionMode)
        {
            case SelectionModeEnum.SingleButton:
            case SelectionModeEnum.SingleDropDownList:
            case SelectionModeEnum.SingleTextBox:
                writer.Write("<span id=\"" + ClientID + "\">");
                break;

            case SelectionModeEnum.Multiple:
            case SelectionModeEnum.MultipleButton:
            case SelectionModeEnum.MultipleTextBox:
                writer.Write("<div id=\"" + ClientID + "\">");
                break;
        }

        // Render child controls
        RenderChildren(writer);

        // Render ending tag of wrapping element
        switch (SelectionMode)
        {
            case SelectionModeEnum.SingleButton:
            case SelectionModeEnum.SingleDropDownList:
            case SelectionModeEnum.SingleTextBox:
                writer.Write("</span>");
                break;

            case SelectionModeEnum.Multiple:
            case SelectionModeEnum.MultipleButton:
            case SelectionModeEnum.MultipleTextBox:
                writer.Write("</div>");
                break;
        }
    }


    /// <summary>
    /// Reloads all controls.
    /// </summary>
    /// <param name="forceReload">Indicates if data should be loaded from DB</param>
    public void Reload(bool forceReload)
    {
        if (!mLoaded || forceReload)
        {
            LoadObjects(forceReload);

            if (!StopProcessing)
            {
                SetupControls();

                registerScripts = true;

                ReloadData(forceReload);
            }

            mLoaded = true;
        }
    }


    /// <summary>
    /// Displays data according to selection mode.
    /// </summary>
    private void ReloadData(bool forceReload)
    {
        // Check that object type is not empty
        if (Object != null)
        {
            // Display form control content according to selection mode
            switch (SelectionMode)
            {
                case SelectionModeEnum.SingleTextBox:
                    DisplayTextBox();
                    break;

                case SelectionModeEnum.SingleDropDownList:
                    DisplayDropDownList(forceReload);
                    break;

                case SelectionModeEnum.Multiple:
                    DisplayMultiple(forceReload);
                    break;

                case SelectionModeEnum.MultipleTextBox:
                    DisplayMultipleTextBox();
                    break;

                case SelectionModeEnum.SingleButton:
                case SelectionModeEnum.MultipleButton:
                    DisplayButton();
                    break;
            }
        }
    }


    /// <summary>
    /// Setups controls.
    /// </summary>
    private void SetupControls()
    {
        SetEnabled();

        // Add resource strings
        bool useImages = UseImageButtons;
        if (useImages)
        {
            btnClear.ImageUrl = GetImagePath("Design/Controls/UniGrid/Actions/delete.png");
            btnClear.ImageAltText = GetString(ResourcePrefix + ".clear|general.clear");

            btnSelect.ImageUrl = GetImagePath("Design/Controls/UniGrid/Actions/select.png");
            btnSelect.ImageAltText = GetString(ResourcePrefix + ".select|general.select");

            txtSingleSelect.CssClass = "TextBoxField";
            drpSingleSelect.CssClass = "DropDownField";
        }
        else
        {
            btnClear.ResourceString = ResourcePrefix + ".clear|general.clear";
            btnClear.ShowAsButton = true;
            lblClear.ResourceString = ResourcePrefix + ".clear|general.clear";

            btnSelect.ResourceString = ResourcePrefix + ".select|general.select";
            btnSelect.ShowAsButton = true;
            lblSelect.ResourceString = ResourcePrefix + ".select|general.select";
        }

        btnDialog.ResourceString = ResourcePrefix + ".select|general.select";
        lblDialog.ResourceString = ResourcePrefix + ".select|general.select";
        if (string.IsNullOrEmpty(lblLinkDialog.ResourceString))
        {
            lblLinkDialog.ResourceString = ResourcePrefix + ".select|general.select";
        }
        lblSingleSelectTxt.ResourceString = ResourcePrefix + ".selectitem|general.selectitem";
        lblSingleSelectDrp.ResourceString = ResourcePrefix + ".selectitem|general.selectitem";

        // Add event handlers
        string selScript = "US_SelectionDialog_" + ClientID + "(); return false;";

        switch (SelectionMode)
        {
            // Dropdownlist mode
            case SelectionModeEnum.SingleDropDownList:
                {
                    if (!String.IsNullOrEmpty(EditItemPageUrl))
                    {
                        btnDropEdit.Visible = true;

                        if (useImages)
                        {
                            btnDropEdit.ImageUrl = GetImagePath("Design/Controls/UniGrid/Actions/edit.png");
                            btnDropEdit.ImageAltText = GetString(ResourcePrefix + ".edit|general.edit");
                        }
                        else
                        {
                            btnDropEdit.ResourceString = ResourcePrefix + ".edit|general.edit";
                            btnDropEdit.ShowAsButton = true;
                        }

                        btnDropEdit.ButtonControl.RenderScript = true;

                        lblDropEdit.Visible = true;
                        lblDropEdit.ResourceString = ResourcePrefix + ".edit|general.edit";
                        btnDropEdit.OnClientClick = "US_EditItem_" + ClientID + "(GetVal('" + drpSingleSelect.ClientID + "')); return false;";
                    }

                    // New button
                    if (!String.IsNullOrEmpty(NewItemPageUrl))
                    {
                        btnDropNew.Visible = true;

                        if (useImages)
                        {
                            btnDropNew.ImageUrl = GetImagePath("Design/Controls/UniGrid/Actions/new.png");
                            btnDropNew.ImageAltText = GetString(ResourcePrefix + ".new|general.new");
                        }
                        else
                        {
                            btnDropNew.ResourceString = ResourcePrefix + ".new|general.new";
                            btnDropNew.ShowAsButton = true;
                        }

                        lblDropNew.Visible = true;
                        lblDropNew.ResourceString = ResourcePrefix + ".new|general.new";
                        btnDropNew.OnClientClick = "US_NewItem_" + ClientID + "(GetVal('" + drpSingleSelect.ClientID + "')); return false;";
                    }
                }
                break;

            // Multiple selection mode
            case SelectionModeEnum.Multiple:
                {
                    btnRemoveSelected.ResourceString = ResourcePrefix + ".removeselected|general.removeselected";
                    lblRemoveSelected.ResourceString = ResourcePrefix + ".removeselected|general.removeselected";
                    btnAddItems.ResourceString = ResourcePrefix + ".additems|general.additems";
                    lblAddItems.ResourceString = ResourcePrefix + ".additems|general.additems";

                    string confirmation = RemoveConfirmation;
                    if (confirmation == null)
                    {
                        confirmation = GetString("general.confirmremove");
                    }
                    if (!String.IsNullOrEmpty(confirmation))
                    {
                        btnRemoveSelected.OnClientClick = "return confirm(" + ScriptHelper.GetString(confirmation) + ")";
                    }

                    btnAddItems.OnClientClick = selScript;

                    btnMenu.Parameter = ScriptHelper.GetString(ClientID);
                    btnMenu.MenuControlPath = "~/CMSAdminControls/UI/UniSelector/Controls/SelectorMenu.ascx";
                    btnMenu.IsLiveSite = IsLiveSite;
                }
                break;

            // Button mode
            case SelectionModeEnum.SingleButton:
            case SelectionModeEnum.MultipleButton:
                // Setup button
                if (ButtonImage == null)
                {
                    btnDialog.OnClientClick = selScript;
                }
                else
                {
                    if (Enabled)
                    {
                        lnkDialog.Attributes.Add("onclick", selScript);
                    }
                    else
                    {
                        lnkDialog.Attributes.Remove("onclick");
                    }
                    lnkDialog.NavigateUrl = "#";
                    if (ButtonImage != "")
                    {
                        imgDialog.ImageUrl = ButtonImage;
                        imgDialog.AlternateText = GetString(ResourcePrefix + ".select|general.select");
                        imgDialog.ToolTip = GetString(ResourcePrefix + ".select|general.select");
                    }
                    else
                    {
                        imgDialog.Visible = false;
                    }
                }
                break;

            // Single textbox mode
            case SelectionModeEnum.SingleTextBox:
                {
                    // Select button
                    if (AllowEditTextBox)
                    {
                        btnClear.OnClientClick = string.Format("SetVal('{0}', ''); return false;", txtSingleSelect.ClientID);
                        btnSelect.OnClientClick = "US_SelectionDialog_" + ClientID + "('$|' + GetVal('" + txtSingleSelect.ClientID + "')); return false;";
                    }
                    else
                    {
                        btnSelect.OnClientClick = selScript;
                    }

                    // Edit button
                    if (!String.IsNullOrEmpty(EditItemPageUrl))
                    {
                        btnEdit.Visible = true;

                        if (useImages)
                        {
                            btnEdit.ImageUrl = GetImagePath("Design/Controls/UniGrid/Actions/edit.png");
                            btnEdit.ImageAltText = GetString(ResourcePrefix + ".edit|general.edit");
                        }
                        else
                        {
                            btnEdit.ResourceString = ResourcePrefix + ".edit|general.edit";
                            btnEdit.ShowAsButton = true;
                        }

                        lblEdit.Visible = true;
                        lblEdit.ResourceString = ResourcePrefix + ".edit|general.edit";
                        if (AllowEditTextBox)
                        {
                            btnEdit.OnClientClick = "US_EditItem_" + ClientID + "(GetVal('" + txtSingleSelect.ClientID + "')); return false;";
                        }
                        else
                        {
                            btnEdit.OnClientClick = "US_EditItem_" + ClientID + "(GetVal('" + hiddenField.ClientID + "')); return false;";
                        }
                    }

                    // New button
                    if (!String.IsNullOrEmpty(NewItemPageUrl))
                    {
                        btnNew.Visible = true;

                        if (useImages)
                        {
                            btnNew.ImageUrl = GetImagePath("Design/Controls/UniGrid/Actions/new.png");
                            btnNew.ImageAltText = GetString(ResourcePrefix + ".new|general.new");
                        }
                        else
                        {
                            btnNew.ResourceString = ResourcePrefix + ".new|general.new";
                            btnNew.ShowAsButton = true;
                        }

                        lblNew.Visible = true;
                        lblNew.ResourceString = ResourcePrefix + ".new|general.new";

                        if (AllowEditTextBox)
                        {
                            btnNew.OnClientClick = "US_NewItem_" + ClientID + "(GetVal('" + txtSingleSelect.ClientID + "')); return false;";
                        }
                        else
                        {
                            btnNew.OnClientClick = "US_NewItem_" + ClientID + "(GetVal('" + hiddenField.ClientID + "')); return false;";
                        }
                    }
                }
                break;

            // Multiple textbox
            case SelectionModeEnum.MultipleTextBox:
                // Select button
                if (AllowEditTextBox)
                {
                    btnClear.OnClientClick = string.Format("SetVal('{0}', ''); return false;", txtSingleSelect.ClientID);
                    btnSelect.OnClientClick = "US_SelectionDialog_" + ClientID + "('$|' + GetVal('" + txtSingleSelect.ClientID + "')); return false;";
                }
                else
                {
                    btnSelect.OnClientClick = selScript;
                }
                break;

            default:
                btnSelect.OnClientClick = selScript;
                break;
        }
    }


    /// <summary>
    /// Loads objects from DB and stores it to variables.
    /// </summary>
    private void LoadObjects(bool forceReload)
    {
        if (Object != null)
        {
            // Set return column name
            if (String.IsNullOrEmpty(ReturnColumnName))
            {
                ReturnColumnName = Object.IDColumn;
            }

            // Open selection dialog depending if UniSelector is on live site
            string url = null;
            if (IsLiveSite)
            {
                url = "~/CMSAdminControls/UI/UniSelector/LiveSelectionDialog.aspx";
            }
            else
            {
                url = "~/CMSAdminControls/UI/UniSelector/SelectionDialog.aspx";
            }

            if (!String.IsNullOrEmpty(SelectItemPageUrl))
            {
                url = SelectItemPageUrl;
            }

            url += "?SelectionMode=" + SelectionMode + "&hidElem=" + hiddenField.ClientID + "&params=" + Server.UrlEncode(ScriptHelper.GetString(Identifier, false)) + "&clientId=" + ClientID + "&localize=" + (LocalizeItems ? 1 : 0) + AdditionalUrlParameters;

            // Create modal dialogs and datasets according to selection mode
            switch (SelectionMode)
            {
                // Single text box selection mode
                case SelectionModeEnum.SingleTextBox:
                    url += "&txtElem=" + txtSingleSelect.ClientID;
                    break;

                // Single drop down list selection mode
                case SelectionModeEnum.SingleDropDownList:
                    if (drpSingleSelect.Items.Count == 0)
                    {
                        forceReload = true;
                    }
                    result = GetResultSet(null, MaxDisplayedTotalItems + 1, 0, forceReload);

                    url += "&selectElem=" + hdnDialogSelect.ClientID;
                    break;

                // Multiple selection mode
                case SelectionModeEnum.Multiple:
                    {
                        uniGrid.GridName = GridName;
                        uniGrid.LoadGridDefinition();

                        // Set custom page according to settings to restrict size of data
                        if (ItemsPerPage > 0)
                        {
                            uniGrid.Pager.DefaultPageSize = ItemsPerPage;
                        }

                        // Ensure new current page number
                        if (mNewCurrentPage > 0)
                        {
                            uniGrid.Pager.UniPager.CurrentPage = mNewCurrentPage;
                        }

                        int currentOffset = (uniGrid.Pager.CurrentPage - 1) * uniGrid.Pager.CurrentPageSize;
                        int totalRecords = 0;

                        // Display only selected items
                        string ids = hiddenField.Value;

                        result = GetResultSet(ids, 0, 0, forceReload, currentOffset, uniGrid.Pager.CurrentPageSize, ref totalRecords);

                        // If not first page and no data loaded load first page
                        if (DataHelper.DataSourceIsEmpty(result) && (currentOffset > 0))
                        {
                            // Set uni grid page to 1 and reload data
                            uniGrid.Pager.UniPager.CurrentPage = 1;
                            result = GetResultSet(ids, 0, 0, forceReload, 0, uniGrid.Pager.CurrentPageSize, ref totalRecords);
                        }

                        uniGrid.PagerForceNumberOfResults = totalRecords;

                        javaScript.Append("function US_RemoveAll_", ClientID, "(){ if (confirm(", ScriptHelper.GetString(GetString("general.confirmremoveall")), ")) {", Page.ClientScript.GetPostBackEventReference(this, "removeall"), "; return false; }} \n");
                    }
                    break;

                // Multiple text box selection mode
                case SelectionModeEnum.MultipleTextBox:
                    url += "&txtElem=" + txtSingleSelect.ClientID;
                    break;

                // Button selection
                case SelectionModeEnum.SingleButton:
                case SelectionModeEnum.MultipleButton:
                    break;

                default:
                    url = null;
                    result = null;
                    break;
            }

            // Selection dialog window
            if (url != null)
            {
                // Add IsSiteManager parameter to handle edit and new window                
                url += IsSiteManager ? "&siteManager=true" : String.Empty;
                // Add hash
                string hash = ValidationHelper.GetHashString(url.Substring(url.IndexOfCSafe('?')));
                url += "&hash=" + hash;

                javaScript.Append("function US_SelectionDialog_", ClientID, "(values) { ", Page.ClientScript.GetCallbackEventReference(this, "values", "US_SelectionDialogReady_" + ClientID, "'" + ResolveUrl(url) + "'"), "; } \n");
            }

            // Create selection changed function
            if (OnSelectionChangedAvailable())
            {
                javaScript.Append("function US_SelectionChanged_", ClientID, "() { ", Page.ClientScript.GetPostBackEventReference(this, "selectionchanged"), "; } \n");
            }

            // New item window
            if (!String.IsNullOrEmpty(NewItemPageUrl))
            {
                string newUrl = URLHelper.AddParameterToUrl(NewItemPageUrl, "selectorId", ClientID) + AdditionalUrlParameters;

                javaScript.Append("function US_NewItem_", ClientID, "(selectedItem) {{ var url = '", ResolveUrl(newUrl), "';modalDialog(url.replace(/##ITEMID##/i, selectedItem),'NewItem', ", EditDialogWindowWidth, ", ", EditDialogWindowHeight, "); }} \n");
            }

            // Edit item window
            if (!String.IsNullOrEmpty(EditItemPageUrl))
            {
                string newUrl = URLHelper.AddParameterToUrl(EditItemPageUrl, "selectorId", ClientID) + AdditionalUrlParameters;

                javaScript.Append(
@"
function US_EditItem_", ClientID, @"(selectedItem) {
    if (selectedItem == '') { 
        alert(", ScriptHelper.GetString(GetString(ResourcePrefix + ".pleaseselectitem|general.pleaseselectitem")), @"); 
        return false; 
    }
    else if (selectedItem.indexOf('{%') >= 0) { 
        alert(", ScriptHelper.GetString(GetString(ResourcePrefix + ".cannoteditmacro|general.cannoteditmacro")), @"); 
        return false; 
    }
    var url = '", ResolveUrl(newUrl), @"'; 
    modalDialog(url.replace(/##ITEMID##/i, selectedItem),'", EditWindowName, "', ", EditDialogWindowWidth, ", ", EditDialogWindowHeight, @"); 
}
"
                );
            }

            string conf = "";
            if (!String.IsNullOrEmpty(SelectionConfirmation))
            {
                conf = "if(!confirm(" + ScriptHelper.GetString(SelectionConfirmation) + ")) { return false; }";
            }

            javaScript.Append(
                "function US_ReloadPage_", ClientID, "() { ", Page.ClientScript.GetPostBackEventReference(this, "reload"), "; return false; } \n",
                "function US_SelectItems_", ClientID, "(items){ ", conf, " document.getElementById('", hiddenField.ClientID, "').value = items; ", Page.ClientScript.GetPostBackEventReference(this, "selectitems"), "; return false; } \n",
                "function US_SelectNewValue_", ClientID, "(selValue){ document.getElementById('", hiddenSelected.ClientID, "').value = selValue; ", Page.ClientScript.GetPostBackEventReference(this, "selectnewvalue"), "; return false; }\n");
        }
        else
        {
            lblStatus.Text = "[UniSelector]: Object type '" + ObjectType + "' not found.";
            StopProcessing = true;
        }
    }


    /// <summary>
    /// Sets the dialog parameters to the context.
    /// </summary>
    protected void SetDialogParameters(string values)
    {
        parameters["SelectionMode"] = SelectionMode;
        parameters["ResourcePrefix"] = ResourcePrefix;
        parameters["ObjectType"] = ObjectType;
        parameters["ReturnColumnName"] = ReturnColumnName;
        parameters["IconPath"] = IconPath;
        parameters["AllowEmpty"] = AllowEmpty;
        parameters["AllowDefault"] = AllowDefault;
        parameters["AllowAll"] = AllowAll;
        parameters["NoneRecordValue"] = NoneRecordValue;
        parameters["AllRecordValue"] = AllRecordValue;
        parameters["FilterControl"] = FilterControl;
        parameters["UseDefaultNameFilter"] = UseDefaultNameFilter;
        parameters["WhereCondition"] = WhereCondition;
        parameters["OrderBy"] = OrderBy;
        parameters["ItemsPerPage"] = ItemsPerPage;
        parameters["EmptyReplacement"] = EmptyReplacement;
        parameters["Values"] = ";" + values + ";";
        parameters["DisplayNameFormat"] = DisplayNameFormat;
        parameters["DialogGridName"] = DialogGridName;
        parameters["AdditionalColumns"] = AdditionalColumns;
        parameters["CallbackMethod"] = CallbackMethod;
        parameters["AllowEditTextBox"] = AllowEditTextBox;
        parameters["FireOnChanged"] = OnSelectionChangedAvailable();
        parameters["DisabledItems"] = DisabledItems;
        parameters["AddGlobalObjectSuffix"] = AddGlobalObjectSuffix;
        parameters["AddGlobalObjectNamePrefix"] = AddGlobalObjectNamePrefix;
        parameters["GlobalObjectSuffix"] = GlobalObjectSuffix;
        parameters["RemoveMultipleCommas"] = RemoveMultipleCommas;
        parameters["IsSiteManager"] = IsSiteManager;
        parameters["FilterMode"] = GetValue("FilterMode");
        parameters["AdditionalSearchColumns"] = AdditionalSearchColumns;
        parameters["SiteWhereCondition"] = GetSiteWhereCondition();

        WindowHelper.Add(Identifier, parameters);
    }


    /// <summary>
    /// Displays single selection textbox.
    /// </summary>
    private void DisplayTextBox()
    {
        plcTextBoxSelect.Visible = true;

        if (!AllowEmpty && !AllowDefault)
        {
            btnClear.Visible = false;
            lblClear.Visible = false;
        }

        if (AllowEditTextBox)
        {
            // Load the selected value
            txtSingleSelect.ReadOnly = false;
        }
        else
        {
            // Get the item
            txtSingleSelect.Text = GetDisplayName(Value);
        }
    }


    /// <summary>
    /// Gets the item name for the given item ID (value)
    /// </summary>
    /// <param name="value">Item value</param>
    private string GetDisplayName(object value)
    {
        string ids = ValidationHelper.GetString(Value, null);
        if (!String.IsNullOrEmpty(ids))
        {
            // Load textbox with data
            DataSet ds = GetResultSet(ids, 0, 0, true);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Build the list of names
                List<string> names = new List<string>();
                StringSafeDictionary<bool> usedValues = new StringSafeDictionary<bool>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string val = ValidationHelper.GetString(dr[ReturnColumnName], "");
                    if (!usedValues.Contains(val))
                    {
                        // Add the display name
                        string name = GetItemName(dr);
                        if (!String.IsNullOrEmpty(name))
                        {
                            names.Add(name);
                        }

                        usedValues[val] = true;
                    }
                }

                // Each name on single line
                return names.Join("\n");
            }
        }

        return "";
    }


    /// <summary>
    /// Displays single selection drop down list.
    /// </summary>
    private void DisplayDropDownList(bool forceReload)
    {
        hiddenField.Visible = false;

        plcDropdownSelect.Visible = true;

        if (!RequestHelper.IsPostBack() || forceReload || (drpSingleSelect.Items.Count == 0) || !String.IsNullOrEmpty(EnabledColumnName))
        {
            string selectedValue = ValidationHelper.GetString(Value, String.Empty);

            // Prepare controls and variables
            drpSingleSelect.Items.Clear();

            bool hasData = !DataHelper.DataSourceIsEmpty(result);

            // Load data to drop-down list
            if (hasData && (Object != null))
            {
                drpSingleSelect.Items.Clear();

                bool maxExceeded = (result.Tables[0].Rows.Count > MaxDisplayedTotalItems);

                // Populate the dropdownlist
                int index = 0;
                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    drpSingleSelect.Items.Add(NewListItem(dr));

                    if (maxExceeded && (++index >= MaxDisplayedItems))
                    {
                        break;
                    }
                }

                // Check if all items were displayed or if '(more items)' item should be added
                if (maxExceeded)
                {
                    drpSingleSelect.Items.Add(new ListItem(GetString(ResourcePrefix + ".moreitems|general.moreitems"), US_MORE_RECORDS.ToString()));
                }
            }

            // Display special items
            if ((SpecialFields != null) && (SpecialFields.Length > 0))
            {
                string text = null;
                string value = null;
                ListItem existing = null;

                for (int i = 0; i < SpecialFields.Length / 2; i++)
                {
                    text = SpecialFields[i, 0];
                    value = SpecialFields[i, 1];

                    if (LocalizeItems)
                    {
                        text = ResHelper.LocalizeString(text);
                    }

                    // Insert only items that are not empty
                    if (!String.IsNullOrEmpty(text))
                    {
                        // Do not add items already in dropdownlist even if in special fields
                        existing = drpSingleSelect.Items.FindByValue(value, true);
                        if (existing == null || (existing.Text.CompareToCSafe(text) != 0))
                        {
                            drpSingleSelect.Items.Insert(i, new ListItem(text, value));
                        }
                    }
                }
            }

            // Display '(none)' item
            if (AllowEmpty)
            {
                // Get item name
                string name = GetString(ResourcePrefix + ".empty|general.empty");

                drpSingleSelect.Items.Insert(0, new ListItem(name, NoneRecordValue));
            }

            // Display '(default)' item
            if (AllowDefault)
            {
                // Get item name
                string name = GetString(ResourcePrefix + ".defaultchoice|general.defaultchoice");

                drpSingleSelect.Items.Insert(0, new ListItem(name, NoneRecordValue));
            }

            // Display '(all)' item
            if (AllowAll)
            {
                // Get item name
                string name = GetString(ResourcePrefix + ".selectall|general.selectall");

                drpSingleSelect.Items.Insert(0, new ListItem(name, AllRecordValue));
            }

            // Load selected value to drop-down list
            if (!String.IsNullOrEmpty(selectedValue))
            {
                // Pre-select item from Value field
                ListItem selectedItem = ControlsHelper.FindItemByValue(drpSingleSelect, selectedValue, false);

                // Select item which is already loaded in drop-down list
                if (selectedItem != null)
                {
                    drpSingleSelect.SelectedValue = selectedItem.Value;
                }
                // Select item which is not in drop-down list
                else
                {
                    // Find item by ID
                    DataSet item = GetResultSet(selectedValue, 1, 0, true);

                    if (!DataHelper.DataSourceIsEmpty(item))
                    {
                        ListItem newItem = NewListItem(item.Tables[0].Rows[0]);

                        // Add selected item to drop down list
                        if (!drpSingleSelect.Items.Contains(newItem))
                        {
                            drpSingleSelect.Items.Add(newItem);
                        }
                        drpSingleSelect.SelectedValue = newItem.Value;
                    }
                    else
                    {
                        try
                        {
                            drpSingleSelect.SelectedValue = selectedValue;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            // New item link
            if (!String.IsNullOrEmpty(NewItemPageUrl))
            {
                drpSingleSelect.Items.Add(new ListItem(GetString(ResourcePrefix + ".newitem|general.newitem"), US_NEW_RECORD.ToString()));
            }

            // If no data in drop-down list, show none and disable
            if (drpSingleSelect.Items.Count == 0)
            {
                // Get item name
                string name = GetString(ResourcePrefix + ".empty|general.empty");

                drpSingleSelect.Items.Insert(0, new ListItem(name, NoneRecordValue));
                drpSingleSelect.Enabled = false;
            }
            else
            {
                drpSingleSelect.Enabled = Enabled;
            }

            HasData = hasData;
        }

        if (drpSingleSelect.Enabled)
        {
            // Build onchange script
            string onChangeScript = "if (!US_ItemChanged(this, '" + ClientID + "')) return false;";
            if (!string.IsNullOrEmpty(OnBeforeClientChanged))
            {
                onChangeScript = OnBeforeClientChanged + onChangeScript;
            }
            if (!string.IsNullOrEmpty(OnAfterClientChanged))
            {
                onChangeScript += OnAfterClientChanged;
            }
            // Add open modal window JavaScript event
            drpSingleSelect.Attributes.Add("onchange", onChangeScript);
        }

        // Enable / disable the edit button
        switch (drpSingleSelect.SelectedValue)
        {
            case "":
            case "0":
            case "-1":
            case "-2":
            case "-3":
                btnDropEdit.Enabled = false;
                break;

            default:
                btnDropEdit.Enabled = Enabled;
                break;
        }
    }


    /// <summary>
    /// Creates a new list item based on the given DataRow.
    /// </summary>
    private ListItem NewListItem(DataRow dr)
    {
        string itemname = GetItemName(dr);
        if (LocalizeItems)
        {
            itemname = ResHelper.LocalizeString(itemname);
        }

        // Create new item
        ListItem newItem = new ListItem(itemname, dr[ReturnColumnName].ToString());

        RaiseOnListItemCreated(newItem);

        // Set disabled if disabled
        if (EnabledColumnName != null)
        {
            bool isEnabled = ValidationHelper.GetBoolean(dr[EnabledColumnName], false);
            string itemClass = isEnabled ? "DropDownItemEnabled" : "DropDownItemDisabled";

            newItem.Attributes.Add("class", itemClass);
        }

        // Set item icon
        if (ShowIcons)
        {
            var obj = CMSObjectHelper.GetObject(dr, ObjectType);
            var icon = UIHelper.GetImageUrl(this.Page, obj.Generalized.GetIconUrl(this.Page, null));

            newItem.Attributes.Add("title", icon);
        }

        if (PrioritizeItems)
        {
            // This code searches css dictionary for the best match - css style for highest possible priority.

            var allPossibleClasses = from kvp in PriorityStyles
                                     where kvp.Key <= (int)dr[objectType.TypeInfo.PriorityColumn]
                                     select kvp;

            string priorityClass = allPossibleClasses
                .Where(x => x.Key == allPossibleClasses.Max(z => z.Key))
                .FirstOrDefault()
                .Value;

            if (!String.IsNullOrEmpty(priorityClass))
            {
                newItem.Attributes["class"] += priorityClass;
            }
        }

        return newItem;
    }


    /// <summary>
    /// Displays multiple selection grid.
    /// </summary>
    private void DisplayMultiple(bool forceReload)
    {
        btnMenu.ContextMenuParent = plcContextMenu;
        pnlGrid.Visible = true;

        uniGrid.GridName = GridName;
        uniGrid.LoadGridDefinition();

        bool hasData = !DataHelper.DataSourceIsEmpty(result);

        // Load data to unigrid
        if (hasData || forceReload)
        {
            uniGrid.DataSource = result;
            if (!RequestHelper.IsPostBack())
            {
                uniGrid.Pager.DefaultPageSize = ItemsPerPage;
            }
            uniGrid.ReloadData();
        }

        // Display "No data" message
        if (!hasData)
        {
            lblStatus.Text = ZeroRowsText ?? GetString(ResourcePrefix + ".nodata|general.nodata");
        }
        else
        {
            lblStatus.Text = "";
        }

        btnRemoveSelected.Visible = hasData;
        lblRemoveSelected.Visible = btnRemoveSelected.Visible;

        btnMenu.Visible = hasData && !IsLiveSite;

        HasData = hasData;
    }


    /// <summary>
    /// Displays multiple selection textbox.
    /// </summary>
    private void DisplayMultipleTextBox()
    {
        plcTextBoxSelect.Visible = true;

        if (!AllowEmpty && !AllowDefault)
        {
            btnClear.Visible = false;
            lblClear.Visible = false;
        }

        // Setup the textbox
        if (AllowEditTextBox)
        {
            txtSingleSelect.ReadOnly = false;
        }
        else
        {
            txtSingleSelect.Text = ValidationHelper.GetString(Value, "").Trim(new char[] { ';', ' ' });
        }
    }


    /// <summary>
    /// Displays selection button.
    /// </summary>
    private void DisplayButton()
    {
        if (ButtonImage == null)
        {
            // Standard button
            plcButtonSelect.Visible = true;
        }
        else
        {
            // Link button
            plcImageSelect.Visible = true;
        }
    }


    /// <summary>
    /// Returns data set depending on specified properties.
    /// </summary>
    private DataSet GetResultSet(string id, int topN, int pageIndex, bool forceReload)
    {
        int totalRecords = 0;
        return GetResultSet(id, topN, pageIndex, forceReload, 0, 0, ref totalRecords);
    }


    /// <summary>
    /// Returns data set depending on specified properties.
    /// </summary>
    /// <param name="ids">ID(s) of the items to load. If null, all items are loaded</param>
    /// <param name="topN">Only take topN items</param>
    /// <param name="pageIndex">Index of the page to get</param>
    /// <param name="offset">Offset of the items</param>
    /// <param name="maxRecords">Maximum number of the records to get</param>
    /// <param name="forceReload">Force reloading of the data</param>
    /// <param name="totalRecords">Returns total number of records</param>
    private DataSet GetResultSet(string ids, int topN, int pageIndex, bool forceReload, int offset, int maxRecords, ref int totalRecords)
    {
        DataSet ds = null;

        // Init columns
        string columns = null;
        if (DisplayNameFormat == USER_DISPLAY_FORMAT)
        {
            // Ensure columns which are needed for USER_DISPLAY_FORMAT
            columns = "UserName;FullName;";
        }
        else if (DisplayNameFormat != null)
        {
            columns = DataHelper.GetNotEmpty(TextHelper.GetMacros(DisplayNameFormat, true), Object.DisplayNameColumn).Replace(";", ", ");
        }
        else
        {
            columns = Object.DisplayNameColumn;
        }

        // Add the default format name column to the query
        if (DefaultDisplayNameFormat != null)
        {
            string defaultColumn = DataHelper.GetNotEmpty(TextHelper.GetMacros(DefaultDisplayNameFormat, true), Object.DisplayNameColumn).Replace(";", ", ");
            columns = SqlHelperClass.MergeColumns(columns, defaultColumn);
        }

        // Add return column name
        columns = SqlHelperClass.MergeColumns(columns, ReturnColumnName);

        // Add additional columns
        columns = SqlHelperClass.MergeColumns(columns, AdditionalColumns);

        // Add enabled column name
        columns = SqlHelperClass.MergeColumns(columns, EnabledColumnName);

        // Add priority column name
        if (PrioritizeItems)
        {
            columns = SqlHelperClass.MergeColumns(columns, "isnull(" + objectType.TypeInfo.PriorityColumn + "," + (int)ObjectPriorityEnum.Low + ") as " + objectType.TypeInfo.PriorityColumn);
        }

        // Ensure SiteID column (for global object prefixes/suffixes)
        if (AddGlobalObjectNamePrefix || AddGlobalObjectSuffix)
        {
            if ((objectType != null) && (objectType.SiteIDColumn != TypeInfo.COLUMN_NAME_UNKNOWN))
            {
                columns = SqlHelperClass.MergeColumns(columns, objectType.SiteIDColumn);
            }
        }

        // Return result set for single selectors
        string itemsWhere = null;

        // Prepare the parameters
        QueryDataParameters parameters = new QueryDataParameters();

        using (var condition = new SelectCondition(parameters))
        {
            if (objectType.TypeInfo.IsVirtualObject)
            {
                condition.InlineLimit = -1;
            }


            switch (SelectionMode)
            {
                case SelectionModeEnum.Multiple:
                case SelectionModeEnum.MultipleButton:
                case SelectionModeEnum.MultipleTextBox:
                    if (ids != null)
                    {
                        // Get where condition for selected items
                        string[] items = ids.Split(ValuesSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        bool isString = !Object.IDColumn.EqualsCSafe(ReturnColumnName, true);
                        if (isString)
                        {
                            // Names
                            condition.Prepare(ReturnColumnName, items, false);
                        }
                        else
                        {
                            // IDs
                            condition.PrepareIDs(ReturnColumnName, items);
                        }

                        // Do not return anything if the selection is empty
                        if (condition.IsEmpty)
                        {
                            return null;
                        }

                        itemsWhere = condition.WhereCondition;
                    }
                    break;

                default:
                    // Build where condition
                    if (!String.IsNullOrEmpty(ids))
                    {
                        itemsWhere = ReturnColumnName + " = '" + SqlHelperClass.GetSafeQueryString(ids, false) + "'";
                    }
                    break;
            }

            // Modify WHERE condition
            string where = SqlHelperClass.AddWhereCondition(this.WhereCondition, itemsWhere);

            // Apply value restrictions
            if (ApplyValueRestrictions)
            {
                where = SqlHelperClass.AddWhereCondition(where, (ListingWhereCondition != String.Empty) ? ListingWhereCondition : WhereCondition);
            }

            // Apply priority restrictions
            if (MinimumPriority != null)
            {
                if (objectType.TypeInfo.PriorityColumn == TypeInfo.COLUMN_NAME_UNKNOWN)
                {
                    throw new GeneralCMSException("Priority column not set in TYPEINFO");
                }
                where = SqlHelperClass.AddWhereCondition(where, objectType.TypeInfo.PriorityColumn + " >= " + MinimumPriority);
            }

            // Apply site restrictions
            string siteWhere = GetSiteWhereCondition();
            if (!string.IsNullOrEmpty(siteWhere))
            {
                where = SqlHelperClass.AddWhereCondition(where, siteWhere);
            }

            // Order by
            string orderBy = String.Empty;

            if (PrioritizeItems)
            {
                orderBy = "isnull(" + objectType.TypeInfo.PriorityColumn + "," + (int)ObjectPriorityEnum.Low + ") DESC,";
            }

            if (String.IsNullOrEmpty(OrderBy))
            {
                orderBy += Object.DisplayNameColumn;
            }
            else
            {
                orderBy += OrderBy;
            }

            GeneralizedInfo obj = ListingObject ?? Object;

            try
            {
                // Get the result set
                ds = obj.GetData(parameters, where, orderBy, topN, columns, false, offset, maxRecords, ref totalRecords);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("UniSelector", "GetResultSet", ex);
            }
        }

        return ds;
    }


    /// <summary>
    /// Returns site WHERE condition according to ObjectSiteName property.
    /// </summary>
    private string GetSiteWhereCondition()
    {
        string siteWhere = null;
        if (!string.IsNullOrEmpty(ObjectSiteName))
        {
            int siteId = 0;
            bool global = false;
            switch (ObjectSiteName.ToLowerCSafe())
            {
                case "#global":
                    global = true;
                    break;

                case "#currentsite":
                case "#current":
                    siteId = CMSContext.CurrentSiteID;
                    break;

                case "#currentandglobal":
                    siteId = CMSContext.CurrentSiteID;
                    global = true;
                    break;

                default:
                    siteId = SiteInfoProvider.GetSiteID(ObjectSiteName);
                    break;
            }
            siteWhere = Object.TypeInfo.GetSiteWhereCondition(siteId, global);
        }
        return siteWhere;
    }


    /// <summary>
    /// Registers scripts.
    /// </summary>
    private void RegisterScripts()
    {
        // Register JavaScript
        ScriptHelper.RegisterDialogScript(Page);

        javaScript.Append(
            @"
function US_SelectionDialogReady_", ClientID, @"(rvalue, context) 
{ 
    modalDialog(context + ((rvalue != '') ? '&selectedvalue=' + rvalue : ''), '", DialogWindowName, "', ", DialogWindowWidth.ToString(), ", ", DialogWindowHeight.ToString(), @", null, null, true); 
    return false;
}");

        // Open dialog script
        ScriptHelper.RegisterScriptFile(Page, "Controls/uniselector.js");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UniSelectorReady" + ClientID, ScriptHelper.GetScript(javaScript.ToString()));

        if (SelectionMode == SelectionModeEnum.SingleDropDownList)
        {
            // DDL initialization
            ScriptHelper.RegisterStartupScript(this, typeof(string), "UniSelector_" + ClientID, ScriptHelper.GetScript("US_InitDropDown(document.getElementById('" + drpSingleSelect.ClientID + "'))"));
        }
    }


    /// <summary>
    /// Enables/disables buttons when their Enabled property is manually set
    /// </summary>
    private void SetButtonsEnabled()
    {
        if (mButtonNewEnabled.HasValue)
        {
            btnNew.Enabled = mButtonNewEnabled.Value;
            btnDropNew.Enabled = mButtonNewEnabled.Value;
            btnAddItems.Enabled = mButtonNewEnabled.Value;
        }

        if (mButtonEditEnabled.HasValue)
        {
            btnEdit.Enabled = mButtonEditEnabled.Value;
            btnDropEdit.Enabled = mButtonEditEnabled.Value;
        }

        if (mButtonRemoveEnabled.HasValue)
        {
            btnRemoveSelected.Enabled = mButtonRemoveEnabled.Value;
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Drop-down list event handler.
    /// </summary>
    private void drpSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Only raise selected index changed when other than (more items...) is selected
        if (drpSingleSelect.SelectedValue != US_MORE_RECORDS.ToString())
        {
            RaiseSelectionChanged();
        }
    }


    /// <summary>
    /// Unigrid external data bound handler.
    /// </summary>
    private object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        string val = String.Empty;

        switch (sourceName.ToLowerCSafe())
        {
            case "yesno":
                val = UniGridFunctions.ColoredSpanYesNo(parameter);
                break;

            case "select":
                {
                    // Get item ID
                    DataRowView drv = (parameter as DataRowView);
                    string itemID = drv[ReturnColumnName].ToString();

                    // Keep the check status if checked
                    bool isChecked = mPageChanged && (hiddenSelected.Value.IndexOfCSafe(ValuesSeparator + itemID + ValuesSeparator, true) >= 0);

                    val = "<input id=\"chk" + checkBoxClass + "_" + itemID + "\" type=\"checkbox\" onclick=\"US_ProcessItem('" + ClientID + "', '" + ValuesSeparator + "', this);\" class=\"chk" + checkBoxClass + "\"" + (isChecked ? "checked=\"checked\" " : "") + (Enabled ? "" : "disabled=\"disabled\"") + " />";
                    break;
                }

            case "itemname":
                {
                    DataRowView drv = (parameter as DataRowView);

                    // Get item ID
                    string itemID = drv[ReturnColumnName].ToString();

                    // Get item name
                    string itemName = GetItemName(drv.Row);

                    if (Enabled)
                    {
                        val = "<div class=\"SelectableItem\" onclick=\"US_ProcessItem('" + ClientID + "', '" + ValuesSeparator + "', document.getElementById('chk" + checkBoxClass + "_" + ScriptHelper.GetString(itemID).Trim('\'') + "'), true); return false;\">" + HTMLHelper.HTMLEncode(TextHelper.LimitLength(itemName, 100)) + "</div>";
                    }
                    else
                    {
                        val = "<div>" + HTMLHelper.HTMLEncode(TextHelper.LimitLength(itemName, 100)) + "</div>";
                    }

                    break;
                }
        }

        if (OnAdditionalDataBound != null)
        {
            val = ValidationHelper.GetString(OnAdditionalDataBound(this, sourceName, parameter, val), String.Empty);
        }

        return val;
    }


    /// <summary>
    /// Returns item display name based on DisplayNameFormat.
    /// </summary>
    /// <param name="dr">Source data row</param>    
    private string GetItemName(DataRow dr)
    {
        string itemName = null;

        // Special formatted user name
        if (DisplayNameFormat == USER_DISPLAY_FORMAT)
        {
            string userName = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, "UserName"), String.Empty);
            string fullName = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, "FullName"), String.Empty);

            itemName = Functions.GetFormattedUserName(userName, fullName, IsLiveSite);
        }
        else if (DisplayNameFormat == null)
        {
            itemName = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, Object.DisplayNameColumn), String.Empty);
        }
        else
        {
            itemName = th.MergeText(DisplayNameFormat, dr);
        }

        // Use the DefaultDisplayNameFormat if the resolved DisplayNameFormat is empty
        if (String.IsNullOrEmpty(itemName) && (!String.IsNullOrEmpty(DefaultDisplayNameFormat)))
        {
            itemName = th.MergeText(DefaultDisplayNameFormat, dr);
        }

        // Add items to unigrid
        if (String.IsNullOrEmpty(itemName))
        {
            itemName = EmptyReplacement;
        }

        if (RemoveMultipleCommas)
        {
            itemName = TextHelper.RemoveMultipleCommas(itemName);
        }

        if (AddGlobalObjectSuffix)
        {
            if ((objectType != null) && (objectType.SiteIDColumn != TypeInfo.COLUMN_NAME_UNKNOWN))
            {
                itemName += (ValidationHelper.GetInteger(DataHelper.GetDataRowValue(dr, objectType.SiteIDColumn), 0) > 0 ? "" : " " + GlobalObjectSuffix);
            }
        }

        return itemName;
    }


    /// <summary>
    /// Unigrid page index changed handler.
    /// </summary>
    protected void uniGrid_OnPageChanged(object sender, EventArgs e)
    {
        mNewCurrentPage = uniGrid.Pager.CurrentPage;
    }


    /// <summary>
    /// Button clear click.
    /// </summary>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Value = null;
        Reload(true);

        // Mark as changed on client side
        RegisterChangedScript();
    }


    /// <summary>
    /// Button "Remove selected items" click handler.
    /// </summary>
    protected void btnRemoveSelected_Click(object sender, EventArgs e)
    {
        // Unselect selected items
        if (!String.IsNullOrEmpty(hiddenSelected.Value))
        {
            hiddenField.Value = DataHelper.GetNewItemsInList(hiddenSelected.Value, hiddenField.Value, ValuesSeparator[0]);

            Reload(true);

            RaiseSelectionChanged();
        }
    }


    /// <summary>
    /// Removes all selected items.
    /// </summary>
    protected void RemoveAll()
    {
        // Unselect selected items
        if (!String.IsNullOrEmpty(hiddenField.Value))
        {
            hiddenField.Value = "";

            Reload(true);

            RaiseSelectionChanged();
        }
    }


    /// <summary>
    /// Overridden set value to collect parameters.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    public override void SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        // Set parameters for dialog
        parameters[propertyName] = value;
    }


    /// <summary>
    /// Overridden to get the parameters.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        if (parameters.Contains(propertyName))
        {
            return parameters[propertyName];
        }

        return base.GetValue(propertyName);
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Handling of the postback event.
    /// </summary>
    /// <param name="eventArgument">Event argument (selected value)</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        switch (eventArgument)
        {
            case "removeall":
                // Remove all items
                RemoveAll();
                break;

            case "selectitems":
                // Raise items selected event
                RaiseOnItemsSelected();
                break;

            case "reload":
                // Reload the data
                Reload(true);

                // Raise selection changed event
                RaiseSelectionChanged();
                break;

            case "selectionchanged":
                // Raise selection changed event
                RaiseSelectionChanged();
                break;

            case "selectnewvalue":
                // Select new item
                switch (SelectionMode)
                {
                    // Single textbox mode
                    case SelectionModeEnum.SingleTextBox:
                        txtSingleSelect.Text = hiddenSelected.Value;
                        break;

                    // Single dropdown list
                    case SelectionModeEnum.SingleDropDownList:

                        // Reload data and select new value
                        Reload(true);
                        ListItem selectedItem = ControlsHelper.FindItemByValue(drpSingleSelect, hiddenSelected.Value, false);

                        if (selectedItem != null)
                        {
                            drpSingleSelect.SelectedValue = selectedItem.Value;
                            btnDropEdit.Enabled = Enabled;
                            drpSingleSelect.Enabled = Enabled;
                        }
                        break;
                }

                hiddenSelected.Value = "";
                break;
        }
    }

    #endregion


    #region "ICallbackEventHandler Members"

    string ICallbackEventHandler.GetCallbackResult()
    {
        // Prepare the parameters for dialog
        SetDialogParameters((string)Value);

        return "";
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        // Adopt new value from callback
        if ((eventArgument != null) && eventArgument.StartsWithCSafe("$|"))
        {
            Value = eventArgument.Substring(2);
        }
    }

    #endregion
}