using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.ExtendedControls.DragAndDrop;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SiteProvider;

public partial class CMSModules_Content_Controls_WebPartToolbar : CMSAbstractPortalUserControl
{

    #region "Variables"

    /// <summary>
    /// CategorySelector - DropDownList codes
    /// </summary>
    private const string CATEGORY_RECENTLY_USED = "##recentlyused##";
    private const string CATEGORY_ALL = "##all##";
    private const string CONFIGURED_WEBPARTS = "##configuredwebparts##";
    private const string SPACE_REPLACEMENT = "__SPACE__";
    private const string CATEGORY_CHANGED_CODE = "wptcategorychanged";

    private const string WEBPART_COLUMNS = "WebPartID, WebPartDisplayName, WebPartDescription, WebPartSkipInsertProperties, WebPartThumbnailGUID";
    private const int DEFAULT_WEBPART_COUNT = 30;

    private bool dataLoaded = false;
    private string mSelectedCategory = null;
    private WebPartToolbarPositionEnum? mPosition = null;
    private CurrentUserInfo currentUser = null;
    private bool isRTLCulture = false;
    private StringBuilder imageHTML = new StringBuilder();
    private string defaultWebPartImageUrl = null;

    protected string prefferedUICultureCode = string.Empty;
    protected string minimizeUrl = null;
    protected string maximizeUrl = null;
    protected string scrollBackImgUrl = null;
    protected string scrollForwardImgUrl = null;

    protected string COOKIE_SELECTED_CATEGORY = CookieName.WebPartToolbarCategory;
    protected bool limitItems = true;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets the selected web part category.
    /// </summary>
    private string SelectedCategory
    {
        get
        {
            if (mSelectedCategory == null)
            {
                // Get the selected category from the cookie
                mSelectedCategory = CookieHelper.GetValue(COOKIE_SELECTED_CATEGORY);
                if (string.IsNullOrEmpty(mSelectedCategory))
                {
                    // Select the All web parts category by default
                    mSelectedCategory = CATEGORY_ALL;

                    // Pre-select wireframe category for wireframe pages
                    string rootPath = categorySelector.RootPath;
                    if (String.IsNullOrEmpty(rootPath))
                    {
                        rootPath = "/";
                    }

                    if (!String.IsNullOrEmpty(rootPath))
                    {
                        WebPartCategoryInfo wpci = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName(rootPath);
                        if (wpci != null)
                        {
                            mSelectedCategory = wpci.CategoryID.ToString();
                        }
                    }
                }
            }

            return mSelectedCategory;
        }
        set
        {
            CookieHelper.SetValue(COOKIE_SELECTED_CATEGORY, value, "/", DateTime.Now.AddDays(30));
        }
    }


    /// <summary>
    /// Gets or sets the web part toolbar position.
    /// </summary>
    private WebPartToolbarPositionEnum Position
    {
        get
        {
            if (mPosition == null)
            {
                mPosition = WebPartToolbarPositionEnum.Right;
                if ((currentUser != null)
                    && (currentUser.UserSettings != null))
                {
                    // Get the toolbar position from the user's setting
                    mPosition = currentUser.UserSettings.UserWebPartToolbarPosition;
                }
            }

            return mPosition.Value;
        }
        set
        {
            mPosition = value;
        }
    }


    /// <summary>
    /// Gets or sets the CSS class which wraps the web part toolbar and indicates in which layout (horizontal/vertical) is the toolbar rendered.
    /// </summary>
    protected string LayoutClass
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the CSS class which wraps the web part toolbar and indicates in which position (top/left/right/bottom) is the toolbar rendered.
    /// </summary>
    protected string PositionClass
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        currentUser = CMSContext.CurrentUser;
        prefferedUICultureCode = currentUser.PreferredUICultureCode;

        // Populate the position drop down list
        drpPosition.Items.Add(new ListItem(ResHelper.GetString("general.top", prefferedUICultureCode), "top"));
        drpPosition.Items.Add(new ListItem(ResHelper.GetString("general.left", prefferedUICultureCode), "left"));
        drpPosition.Items.Add(new ListItem(ResHelper.GetString("general.right", prefferedUICultureCode), "right"));
        drpPosition.Items.Add(new ListItem(ResHelper.GetString("general.bottom", prefferedUICultureCode), "bottom"));

        if (IsWireframe() || (PortalContext.ViewMode == ViewModeEnum.Wireframe))
        {
            categorySelector.RootPath = "Wireframes";
            COOKIE_SELECTED_CATEGORY += "WF";
        }
        else if (!PortalHelper.IsWireframingEnabled(CMSContext.CurrentSiteName))
        {
            categorySelector.RootPath = "/";
        }

        base.OnInit(e);
    }


    /// <summary>
    /// Returns true, if the current document is wireframe
    /// </summary>
    private static bool IsWireframe()
    {
        var pageInfo = CMSContext.CurrentPageInfo;
        if (pageInfo == null)
        {
            return false;
        }
        else
        {
            return pageInfo.ClassName.EqualsCSafe("cms.wireframe", true);
        }
    }


    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Enable loading the web parts
        repItems.StopProcessing = false;

        // Setups the web part toolbar control
        SetupControl();

        if (URLHelper.IsPostback())
        {
            // Add the search text to the current url
            URLHelper.CurrentURL = URLHelper.AddParameterToUrl(URLHelper.CurrentURL, "wptSearchText", HttpUtility.UrlEncode(txtSearch.Text));
        }

        // Handle & set hidden reload button
        btnLoadMore.Style.Add("display", "none");
        btnLoadMore.Click += new EventHandler(btnLoadMore_Click);
    }


    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (QueryHelper.Contains("wptSearchText"))
        {
            // Get the search text from the current url
            txtSearch.Text = HttpUtility.UrlDecode(URLHelper.GetUrlParameter(URLHelper.CurrentURL, "wptSearchText").Replace(SPACE_REPLACEMENT, " "));

            // Hide the scrollPanel when the search filter will be applied (skip this when changing displayed category)
            if (Request.Form["__EVENTARGUMENT"] != CATEGORY_CHANGED_CODE)
            {
                scrollPanel.Style.Add("display", "none");
            }
        }

        // Add the javascript file and scripts into the page
        ScriptHelper.RegisterJQueryCookie(Page);
        ScriptHelper.RegisterScriptFile(Page, "DesignMode/WebPartToolbar.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/jquery/jquery-url.js");
        ScriptHelper.RegisterStartupScript(this, typeof(string), "wptPageScripts", ScriptHelper.GetScript(GetPageScripts()));
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "wptSetupToolbar", ScriptHelper.GetScript("wptSetupSearch();"));
        ScriptHelper.RegisterStartupScript(this, typeof(string), "wptImageBaseURL", ScriptHelper.GetScript("var WPTImgBaseSrc = '" + ResolveUrl("~/CMSPages/GetMetaFile.aspx?maxsidesize=64&fileguid=") + "';"));  

        // Adjust the page to accommodate the web part toolbar according to its position
        switch (Position)
        {
            case WebPartToolbarPositionEnum.Top:
                // Default position
                minimizeUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/Vertical/Minimize.png");
                maximizeUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/Vertical/Maximize.png");
                PositionClass = "WPTPositionTop";
                drpPosition.SelectedValue = "top";
                break;

            case WebPartToolbarPositionEnum.Left:
                maximizeUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/Horizontal/Maximize.png");
                minimizeUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/Horizontal/Minimize.png");
                LayoutClass = "WPTVerticalLayout";
                PositionClass = "WPTPositionLeft";
                drpPosition.SelectedValue = "left";
                scrollPanel.Layout = RepeatDirection.Vertical;
                break;

            case WebPartToolbarPositionEnum.Bottom:
                maximizeUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/Vertical/Minimize.png");
                minimizeUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/Vertical/Maximize.png");
                PositionClass = "WPTPositionBottom";
                drpPosition.SelectedValue = "bottom";
                break;

            case WebPartToolbarPositionEnum.Right:
                maximizeUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/Horizontal/Minimize.png");
                minimizeUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/Horizontal/Maximize.png");
                LayoutClass = "WPTVerticalLayout";
                PositionClass = "WPTPositionRight";
                drpPosition.SelectedValue = "right";
                scrollPanel.Layout = RepeatDirection.Vertical;
                break;
        }

        scrollPanel.IsRTL = isRTLCulture;

        if ((Position == WebPartToolbarPositionEnum.Top) || (Position == WebPartToolbarPositionEnum.Bottom))
        {
            // Horizontal layout
            scrollBackImgUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/Arrow" + (!isRTLCulture ? "Left" : "Right") + ".png");
            scrollForwardImgUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/Arrow" + (!isRTLCulture ? "Right" : "Left") + ".png");
        }
        else
        {
            // Vertical layout
            scrollBackImgUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/ArrowUp.png");
            scrollForwardImgUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/ArrowDown.png");
            pnlMenuSep.Visible = false;
            pnlTrashSep.Visible = false;
        }

        // Set the selected category
        if (!RequestHelper.IsPostBack())
        {
            if (SelectedCategory == CATEGORY_ALL)
            {
                // Select the "All web parts" category by index (value for the "All" category is not known)
                categorySelector.DropDownListControl.SelectedIndex = 1;
            }
            else
            {
                categorySelector.DropDownListControl.SelectedValue = SelectedCategory;
            }
        }
        else
        {
            SelectedCategory = categorySelector.DropDownListControl.SelectedValue;

            // Refresh scroll panel
            ScriptHelper.RegisterClientScriptBlock(pnlU, typeof(string), "wptRefreshScrollPanel", "jQuery(document).ready(function () { wptInit(true); wptReloadScrollPanel(true); wptLoadWebpartImages(); });", true);
        }

        // Load the web part items according to the selected category and the search text condition
        LoadWebParts(false);

        // Register script for web part lazy load
        if (limitItems)
        {
            string appearScript = @"
            jQuery(document).ready(
            function () {
                jQuery('.AppearElement').appear(
                    function () {
                        jQuery(this).html(wpLoadingMoreString); 
                        DoToolbarPostBack(); 
                     });
            });";

            string postBackScript = @"
            function DoToolbarPostBack(){
                " + ControlsHelper.GetPostBackEventReference(btnLoadMore, "") + @"
            };";

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "DoToolbarPostback", ScriptHelper.GetScript(appearScript + postBackScript));
        }

        bool isMinimized = ValidationHelper.GetBoolean(CookieHelper.GetValue(CookieName.WebPartToolbarMinimized), false);
        if (isMinimized)
        {
            // Renew the cookie
            CookieHelper.SetValue(CookieName.WebPartToolbarMinimized, "true", "/", DateTime.Now.AddDays(31), false);

            // Hide the expanded web part toolbar
            pnlMax.Style.Add("display", "none");
        }
        else
        {
            // Register the OnLoad javascript for the expanded bar
            ltrScript.Text = ScriptHelper.GetScript("function pageLoad(sender, args) { wptInit(true); }");

            // Hide the minimized toolbar
            pnlMinimized.Style.Add("display", "none");
        }

        ScriptHelper.RegisterJQueryAppear(Page);
    }


    /// <summary>
    /// Handles the ItemDataBound event of the repItems control.
    /// </summary>
    protected void repItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            object dataItem = e.Item.DataItem;

            if (dataItem.GetType() == typeof(DataRowView))
            {
                // Get data row
                DataRow dr = ((DataRowView)dataItem).Row;

                // Setup the web part item
                Panel wptFlatItem = ((Panel)e.Item.FindControl("i"));
                if (wptFlatItem != null)
                {
                    wptFlatItem.ToolTip = "<div class='WPTTH'>" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(dr["WebPartDisplayName"]), prefferedUICultureCode)) + "</div><div class='WPTTC'>" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(dr["WebPartDescription"]), prefferedUICultureCode)) + "</div>";

                    // Set the web part id
                    wptFlatItem.Attributes.Add("data-webpartid", Convert.ToString(dr["WebPartID"]));

                    // Ensure that when start dragging then a copy of the original web part item will be created
                    wptFlatItem.Attributes.Add("data-dragkeepcopy", "1");
                    wptFlatItem.Attributes.Add("onmouseover", "wptToggle(this, true);");
                    wptFlatItem.Attributes.Add("onmouseout", "wptToggle(this, false);");

                    // Skip the insert properties dialog when the web part allows this behavior
                    if (ValidationHelper.GetBoolean(dr["WebPartSkipInsertProperties"], false))
                    {
                        wptFlatItem.Attributes.Add("data-skipdialog", "1");
                    }
                }

                // Build the web part image html
                Literal ltrImage = ((Literal)e.Item.FindControl("ltrImage"));
                if (ltrImage != null)
                {
                    imageHTML.Length = 0;
                    imageHTML.Append("<img alt=\"\" class=\"WPTItemImg\" src=\"");
                    string metaFileGUIDStr = dr["WebPartThumbnailGUID"].ToString();
                    string imgParams = URLHelper.EncodeQueryString("?maxsidesize=64&fileguid=" + metaFileGUIDStr);
                    string imgSrc = defaultWebPartImageUrl;
                    if (!string.IsNullOrEmpty(metaFileGUIDStr))
                    {
                        imgSrc = ResolveUrl("~/CMSPages/GetMetaFile.aspx") + imgParams;
                    }

                    imageHTML.Append(imgSrc);
                    imageHTML.Append("\" />");
                    ltrImage.Text = imageHTML.ToString();
                }
            }
        }
        else if (e.Item.ItemType == ListItemType.Separator)
        {
            // Hide the item separator for vertical layout
            if (scrollPanel.Layout == RepeatDirection.Vertical)
            {
                e.Item.Visible = false;
            }
            else if (e.Item.ItemIndex >= (DEFAULT_WEBPART_COUNT - 1))
            {
                // Hide the separator if the web part count exceeded maximum allowed value
                HtmlGenericControl wptSeparator = ((HtmlGenericControl)e.Item.FindControl("s"));
                if (wptSeparator != null)
                {
                    wptSeparator.Style.Add("display", "none");
                }

            }
        }
    }


    /// <summary>
    /// Handles the Click event of the btnPopupOK control.
    /// </summary>
    protected void btnPopupOK_Click(object sender, EventArgs e)
    {
        Position = WebPartToolbarPositionEnumFunctions.GetWebPartToolbarPositionEnum(drpPosition.SelectedValue);

        // Set the position
        currentUser.UserSettings.UserWebPartToolbarEnabled = chkWptEnabled.Checked;
        currentUser.UserSettings.UserWebPartToolbarPosition = Position;
        UserSettingsInfoProvider.SetUserSettingsInfo(currentUser.UserSettings);

        if (!chkWptEnabled.Checked)
        {
            // Hide the toolbar
            Visible = false;
        }
    }


    /// <summary>
    /// Reloads the web part list.
    /// </summary>
    /// <param name="forceLoad">if set to <c>true</c>, reload the control even if the control has been already loaded</param>
    protected void LoadWebParts(bool forceLoad)
    {
        if (!dataLoaded || forceLoad)
        {
            // Get only required columns
            repItems.Columns = WEBPART_COLUMNS;

            // Setup the where condition
            if (SelectedCategory == CATEGORY_RECENTLY_USED)
            {
                // Recently used category
                string where = null;

                // Root category path
                string rootPath = categorySelector.RootPath;
                if (!String.IsNullOrEmpty(rootPath))
                {
                    where = "WebPartCategoryID IN (SELECT CategoryID FROM CMS_WebPartCategory WHERE CategoryPath LIKE N'" + rootPath.Replace("'", "''") + "%')";
                }

                repItems.WhereCondition = SqlHelperClass.AddWhereCondition(where, SqlHelperClass.GetWhereCondition("WebPartName", currentUser.UserSettings.UserUsedWebParts.Split(';')));
                repItems.OrderBy = "WebPartLastSelection DESC";
            }
            else
            {
                // Specific web part category
                int selectedCategoryId = ValidationHelper.GetInteger(SelectedCategory, 0);
                if (selectedCategoryId > 0)
                {
                    WebPartCategoryInfo categoryInfo = WebPartCategoryInfoProvider.GetWebPartCategoryInfoById(selectedCategoryId);
                    if (categoryInfo != null)
                    {
                        // Select also all subcategories (using "/%")
                        string categoryPath = categoryInfo.CategoryPath;
                        if (!categoryPath.EndsWith("/"))
                        {
                            categoryPath += "/";
                        }

                        // Do not limit items if not root category is selected
                        if (!categoryInfo.CategoryPath.EqualsCSafe("/"))
                        {
                            limitItems = false;
                        }

                        // Get all web parts for the selected category and its subcategories
                        repItems.WhereCondition = SqlHelperClass.AddWhereCondition(repItems.WhereCondition, "WebPartCategoryID = " + selectedCategoryId + " OR WebPartCategoryID IN (SELECT CategoryID FROM CMS_WebPartCategory WHERE CategoryPath LIKE N'" + categoryPath.Replace("'", "''") + "%')");
                    }
                }
            }

            // Do not display "Widget only" web parts in the toolbar
            repItems.WhereCondition = SqlHelperClass.AddWhereCondition(repItems.WhereCondition, "(WebPartType IS NULL) OR (WebPartType <> " + (int)WebPartTypeEnum.WidgetOnly + ")");

            // Limit items if required
            if (limitItems)
            {
                repItems.SelectTopN = DEFAULT_WEBPART_COUNT;
            }
            repItems.ReloadData(true);
            dataLoaded = true;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    // Setups the web part toolbar control.
    /// </summary>
    private void SetupControl()
    {
        // Initialize controls variables
        isRTLCulture = CultureHelper.IsUICultureRTL();
        defaultWebPartImageUrl = GetImageUrl("Objects/CMS_WebPart/notavailable.png");

        // Load the web part category selector
        categorySelector.ReloadData(false);

        // Add the "Recently used" item to the category selector
        ListItem listItem = new ListItem();
        listItem.Text = ResHelper.GetString("webparts.recentlyusedshort", prefferedUICultureCode);
        listItem.Value = CATEGORY_RECENTLY_USED;
        categorySelector.DropDownListControl.Items.Insert(0, listItem);
        categorySelector.DropDownListControl.CssClass = "WPTCategories";

        // Ensure that when selected item is changed then the web part repeater update panel will be also updated
        categorySelector.DropDownListControl.Attributes.Add("onchange", "wptCategoryChanged();");

        // Setup the handlers
        btnPopupOK.Click += new EventHandler(btnPopupOK_Click);
        repItems.ItemDataBound += new RepeaterItemEventHandler(repItems_ItemDataBound);

        // Setup the resource strings and tooltips
        lblWptEnabled.Text = ResHelper.GetString("adm.user.lblusershowwebparttoolbar", prefferedUICultureCode);
        lblWptPosition.Text = ResHelper.GetString("general.position", prefferedUICultureCode);

        btnPopupOK.Text = ResHelper.GetString("general.ok", prefferedUICultureCode);
        btnPopupCancel.Text = ResHelper.GetString("general.cancel", prefferedUICultureCode);

        pnlTrash.ToolTip = ResHelper.GetString("webparttoolbar.trashtooltip", prefferedUICultureCode);

        btnConfigBtn.ToolTip = ResHelper.GetString("general.settings", prefferedUICultureCode);
        btnConfigBtn.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/Configure.png");
        btnConfigBtn.OnClientClick = "showModalPopup('" + ucPopupDialog.ClientID + "', '" + ucPopupDialog.ClientID + "_background'); return false;";

        // Setup the toolbar web part list drag-and-drop extender
        ddExtender.TargetControlID = scrollPanel.ScrollAreaContainer.ID;
        ddExtender.DropCueID = pnlCue.ID;
        ddExtender.DragItemClass = "WPTSelectorEnvelope";
        ddExtender.DragItemHandleClass = "WPTHandle";
        ddExtender.OnClientDrop = "";
        ddExtender.OnClientBeforeDrop = "wptListOnBeforeDrop";
        
        // Setup the drag-and-drop extender for highlighted web parts as well.
        // This solves the issue when changing categories (using the drop down) and you highlight a web part before the init script for drag-and-drop is run.
        ddExtenderHovered.TargetControlID = scrollPanel.ScrollAreaContainer.ID;
        ddExtenderHovered.DropCueID = pnlCue.ID;
        ddExtenderHovered.DragItemClass = "WPTSelectorEnvelopeHover";
        ddExtenderHovered.DragItemHandleClass = "WPTHandle";
        ddExtenderHovered.OnClientDrop = "";
        ddExtenderHovered.OnClientBeforeDrop = "wptListOnBeforeDrop";

        // Setup the trash drag-and-drop extender
        trashExtender.TargetControlID = pnlTrashDropArea.ID;
        trashExtender.DropCueID = pnlTrashCue.ID;
        trashExtender.DragItemClass = "";
        trashExtender.DragItemHandleClass = "";
        trashExtender.OnClientDrop = "wptTrashOnDrop";
        trashExtender.OnClientBeforeDrop = "wptTrashOnBeforeDrop";

        // Hide the compulsory drag-and-drop panel Cue, it is not being used
        pnlCue.Style.Add("display", "none");
        pnlCue.Style.Add("position", "absolute");

        // Setup the modal popup title
        titleElem.TitleText = ResHelper.GetString("webparttoolbar.properties", prefferedUICultureCode);
        titleElem.HelpTopicName = "CMS_Design_WPT_Settings";
        titleElem.HelpName = "CMS_Design_WPT_Settings";
        titleElem.HelpIconUrl = GetImageUrl("General/HelpLargeDark.png");

        pnlLoader.Controls.Add(new LiteralControl(CMSUpdateProgress.GetDefaultContent(Page)));

        pnlMaximize.Attributes.Add("onclick", "wptMaximize();");
        pnlMinimize.Attributes.Add("onclick", "wptMinimize();");
    }


    /// <summary>
    /// Gets the page javascripts.
    /// </summary>
    private string GetPageScripts()
    {
        bool isVertical = ((Position == WebPartToolbarPositionEnum.Left) || (Position == WebPartToolbarPositionEnum.Right));

        // Generate toolbar scripts
        StringBuilder sb = new StringBuilder();
        sb.Append(
@"var wptIsMinimizedCookie = '", CookieName.WebPartToolbarMinimized, @"';
var wptPosition = '", WebPartToolbarPositionEnumFunctions.GetWebPartToolbarPositionString(Position), @"';
var wptIsVerticalLayout = ", (isVertical ? "true" : "false"), @";
var wptIsRTL = ", (isRTLCulture ? "true" : "false"), @";
        
var wpLoadingMoreString = '" + GetString("general.loading") + @"';

function wptTrashOnDrop(sender, e) {
    var item = e.get_droppedItem();
    var uid = item.getAttribute('id');
    var loc = webPartLocation[uid];
    if (item.getAttribute('data-removewebpart') == '1') {
        setZone(loc[0]); setWebPart(loc[1]); setAliasPath(loc[2]); setGuid(loc[3]);
        item.style.display = 'none'; 
        ", ControlsHelper.GetPostBackEventReference(PortalManager, "RemoveWebPart"), @";
    }
}

function wptSetupSearch()
{   
    jQuery('#", txtSearch.ClientID, @"')
        .keypress(function (e) {
            window.clearTimeout(wptFilterWebPartsTimer);
            return wptProceedSpecialKeys(this, e);
        })
        .keyup(function (e) {
            var ret = wptProceedSpecialKeys(this, e);
            wptFilterWebParts(this);
            return ret;
        });
}

function wptFilterWebParts(txtBoxElem) {
    window.clearTimeout(wptFilterWebPartsTimer);
    wptFilterWebPartsTimer = window.setTimeout(function () {
        var searchText = txtBoxElem.value.toLowerCase();
        UpdateRefreshPageUrlParam('wptSearchText', searchText.replace(/ /g,'", SPACE_REPLACEMENT, @"'));
        wptFilter(searchText);
        wptInit(true);
        wptReloadScrollPanel(true);
    }, 200);
}

function wptFilterAfterPostBack(){
    wptFilter(document.getElementById('", txtSearch.ClientID, @"').value);
    wptReloadScrollPanel(true);
}

function wptReloadScrollPanel(forceReload) {
    scrollPanelInit('", scrollPanel.ClientID, @"', forceReload);
}

function wptOnSave() {
    if (!jQuery('#", chkWptEnabled.ClientID, @"').is(':checked')) {
        if (confirm('", ResHelper.GetString("webparttoolbar.hidewarning", CMSContext.CurrentUser.PreferredUICultureCode), @"')) {
            return true;
        }
        else {
            wptDialogRestoreValues();
            jQuery('#", btnPopupCancel.ClientID, @"').click();
            return false;
        }
    }

    return true;
}

function wptDialogRestoreValues() {
    jQuery('#", chkWptEnabled.ClientID, @"').prop('checked', true);
    jQuery('#", drpPosition.ClientID, @"').val(wptPosition);
}

function wptCategoryChanged() {
    jQuery('#" + txtSearch.ClientID + @"').val(''); 
    // Remove all tooltip temporary nodes
    jQuery('.WPTTT').remove(); ",
    ControlsHelper.GetPostBackEventReference(hdnUpdater, CATEGORY_CHANGED_CODE), @";
}
");

        if (QueryHelper.Contains("wptSearchText"))
        {
            // Filter the web parts if the search text is set
            sb.Append("jQuery(document).ready(function () { wptFilterWebParts(document.getElementById('", txtSearch.ClientID, @"')); })");
        }

        return sb.ToString();
    }


    /// <summary>
    /// Load all items
    /// </summary>
    private void btnLoadMore_Click(object sender, EventArgs e)
    {
        limitItems = false;
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "WPTBLoadMoreFilter", ScriptHelper.GetScript("wptReload= true;  jQuery(document).ready(function() { spScrollSetPosition('" + scrollPanel.ClientID + "');  wptFilterAfterPostBack();  window.setTimeout(function(){ wptReload= false; }, 1000) });"));
    }

    #endregion
}


