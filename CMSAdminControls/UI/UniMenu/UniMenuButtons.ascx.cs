using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;
using SubMenuItem = CMS.UIControls.UniMenuConfig.SubItem;
using CMS.CMSHelper;

public partial class CMSAdminControls_UI_UniMenu_UniMenuButtons : CMSUserControl
{
    #region "Variables"

    private bool mAllowSelection = false;

    protected int mSelectedIndex = 0;

    private List<Panel> mInnerControls = null;

    private int mMaximumItems = 1;

    private bool mHorizontalLayout = true;

    private List<MenuItem> mButtons = null;

    private StringBuilder mStartupScript = null;

    private bool mAllowToggleScriptRegistered = false;

    #endregion


    #region "Constants"

    public const string SelectedSuffix = " Selected";

    private const string BUTTON_PANEL_SHORTID = "pb";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Determines whether buttons are selectable.
    /// </summary>
    public bool AllowSelection
    {
        get
        {
            return mAllowSelection;
        }
        set
        {
            mAllowSelection = value;
        }
    }


    /// <summary>
    /// Determines whether buttons are draggable.
    /// </summary>
    public bool AllowDraggable
    {
        get;
        set;
    }


    /// <summary>
    /// If selection is enabled, determines which button is implicitly selected.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            return mSelectedIndex;
        }
        set
        {
            mSelectedIndex = value;
        }
    }


    /// <summary>
    /// Indicates if function 'CheckChanges' should be called before action.
    /// </summary>
    public bool CheckChanges
    {
        get;
        set;
    }


    /// <summary>
    /// List of buttons
    /// </summary>
    public List<MenuItem> Buttons
    {
        get
        {
            return mButtons ?? (mButtons = new List<MenuItem>());
        }
        set
        {
            mButtons = value;
        }
    }


    /// <summary>
    /// Description.
    /// </summary>
    public List<Panel> InnerControls
    {
        get
        {
            return mInnerControls;
        }
        set
        {
            mInnerControls = value;
        }
    }


    /// <summary>
    /// Indicates whether to repeat items horizontally. Value is true by default. Otherwise items will be rendered vertically.
    /// </summary>
    public bool HorizontalLayout
    {
        get
        {
            return mHorizontalLayout;
        }
        set
        {
            mHorizontalLayout = value;
        }
    }


    /// <summary>
    /// Specifies number of menu items to be rendered in single row or column (depending on RepeatHorizontal property).
    /// </summary>
    public int MaximumItems
    {
        get
        {
            return mMaximumItems;
        }
        set
        {
            mMaximumItems = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            if (Buttons != null)
            {
                ReloadButtons();
            }
            else
            {
                Controls.Add(GetError(GetString("unimenubuttons.wrongdimensions")));
            }
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Load buttons collection.
    /// </summary>
    public void ReloadButtons()
    {
        Initialize();
        AddMainTable();
        if (mStartupScript.Length > 0)
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "UniMenuStartupScript_" + ClientID, ScriptHelper.GetScript(mStartupScript.ToString()));
        }
    }


    /// <summary>
    /// Method for registering JS scripts for selecting button.
    /// </summary>
    private void RegisterAllowSelectionScript()
    {
        ScriptHelper.RegisterJQuery(Page);
        StringBuilder selectionScript = new StringBuilder();
        selectionScript.AppendLine("function SelectButton(elem)");
        selectionScript.AppendLine("{");
        selectionScript.AppendLine("    var selected = '" + SelectedSuffix + "';");
        selectionScript.AppendLine("    var jElem =$j(elem);");
        // Get first parent table
        selectionScript.AppendLine("    var parentTable = jElem.parents('table').get(0);");
        selectionScript.AppendLine("    if (parentTable != null) {");
        // Remove selected class from current group of buttons
        selectionScript.AppendLine("        $j(parentTable).find('.' + elem.className).removeClass(selected);");
        selectionScript.AppendLine("    }");
        selectionScript.AppendLine("    jElem.addClass(selected);");
        selectionScript.AppendLine("}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "selectionScript", ScriptHelper.GetScript(selectionScript.ToString()));

        StringBuilder indexSelection = new StringBuilder();
        indexSelection.AppendLine("function SelectButtonIndex_" + ClientID + "(index)");
        indexSelection.AppendLine("{");
        indexSelection.AppendLine("    var elem = document.getElementById('" + ClientID + "_" + BUTTON_PANEL_SHORTID + "'+index);");
        indexSelection.AppendLine("    SelectButton(elem);");
        indexSelection.AppendLine("");
        indexSelection.AppendLine("}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "indexSelection_" + ClientID, ScriptHelper.GetScript(indexSelection.ToString()));
    }


    /// <summary>
    /// Method for registering JS scripts for toggling buttons.
    /// </summary>
    private void EnsureAllowToggleScript()
    {
        if (mAllowToggleScriptRegistered)
        {
            return;
        }
        // Toggle script
        StringBuilder toggleScript = new StringBuilder();
        toggleScript.AppendLine("function ToggleButton(elem)");
        toggleScript.AppendLine("{");
        toggleScript.AppendLine("    var selected = '" + SelectedSuffix.Trim() + "';");
        toggleScript.AppendLine("    var jElem =$j(elem);");
        // Get first parent table
        toggleScript.AppendLine("    jElem.toggleClass(selected);");
        toggleScript.AppendLine("}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "toggleScript", ScriptHelper.GetScript(toggleScript.ToString()));
        mAllowToggleScriptRegistered = true;
    }


    /// <summary>
    /// Method for formatting draggable handler definition.
    /// </summary>
    /// <param name="html">html element of helper</param>
    /// <returns>clone if default</returns>
    private string GetDraggableHandler(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return "clone";
        }
        return "function(){ return $j(\"" + html + "\"); }";
    }


    /// <summary>
    /// Method for adding button to controls.
    /// </summary>
    /// <param name="outerPanel">Panel to be added to</param>
    /// <param name="identifier">Index of button</param>
    private void AddButtonTo(CMSPanel outerPanel, int identifier)
    {
        MenuItem button = Buttons[identifier];
        string caption = button.Text;
        string tooltip = button.Tooltip;
        string cssClass = button.CssClass;
        string onClick = button.OnClientClick;
        string redirectUrl = button.RedirectUrl;
        string imagePath = button.ImagePath;
        string alt = button.ImageAltText;
        int minWidth = button.MinimalWidth;
        ImageAlign imageAlign = GetImageAlign(button.ImageAlign);

        // Generate button image
        Image buttonImage = new Image();
        buttonImage.ID = "img" + identifier;
        buttonImage.CssClass = "MenuButtonImage";
        buttonImage.EnableViewState = false;
        buttonImage.AlternateText = alt ?? caption;
        if (!string.IsNullOrEmpty(imagePath))
        {
            buttonImage.ImageUrl = ResolveUrl(imagePath);
        }
        buttonImage.ImageAlign = imageAlign;

        // Generate button text
        Literal captionLiteral = new Literal();
        captionLiteral.ID = "ltlCaption" + identifier;
        captionLiteral.EnableViewState = false;
        string separator = (imageAlign == ImageAlign.Top) ? "<br />" : "\n";
        captionLiteral.Text = String.Format("{0}<span class=\"MenuButtonText\">{1}</span>", separator, caption);

        CMSPanel pnlSubItems = null;
        if (button.SubItems.Count > 0)
        {
            // Ensure jQuery tool is loaded
            ScriptHelper.RegisterJQueryTools(Page);

            // Append submenu toogle script
            onClick = "CMSUniMenu.ToogleSubMenu(this);" + onClick;

            // Append arrow down image for idication sub menu
            captionLiteral.Text += "&nbsp;<img style=\"width:8px; height:8px;\" src=\"" + GetImageUrl("Design/Controls/UniMenu/ArrowDown.png") + "\" alt=\"Sub menu items\" class=\"SubItemsArrow\" />";

            // Create submenu
            CreateSubMenu(button, ref pnlSubItems, cssClass, identifier);
        }

        // Generate button link
        HyperLink buttonLink = new HyperLink();
        buttonLink.ID = "btn" + identifier;
        buttonLink.EnableViewState = false;

        buttonLink.Controls.Add(buttonImage);
        buttonLink.Controls.Add(captionLiteral);

        if (!string.IsNullOrEmpty(redirectUrl))
        {
            buttonLink.NavigateUrl = ResolveUrl(redirectUrl);
        }

        // Generate left border
        CMSPanel pnlLeft = new CMSPanel();
        pnlLeft.ID = "pnlLeft" + identifier;
        pnlLeft.ShortID = "pl" + identifier;

        pnlLeft.EnableViewState = false;
        pnlLeft.CssClass = "Left" + cssClass;

        // Generate middle part of button
        CMSPanel pnlMiddle = new CMSPanel();
        pnlMiddle.ID = "pnlMiddle" + identifier;
        pnlMiddle.ShortID = "pm" + identifier;

        pnlMiddle.EnableViewState = false;
        pnlMiddle.CssClass = "Middle" + cssClass;
        pnlMiddle.Controls.Add(buttonLink);
        if (minWidth > 0)
        {
            pnlMiddle.Style.Add("min-width", minWidth + "px");

            // IE7 issue with min-width
            CMSPanel pnlMiddleTmp = new CMSPanel();
            pnlMiddleTmp.EnableViewState = false;
            pnlMiddleTmp.Style.Add("width", minWidth + "px");
            pnlMiddle.Controls.Add(pnlMiddleTmp);
        }

        // Add sub items if exists
        if (pnlSubItems != null)
        {
            pnlMiddle.Controls.Add(pnlSubItems);
        }

        // Generate right border
        CMSPanel pnlRight = new CMSPanel();
        pnlRight.ID = "pnlRight" + identifier;
        pnlRight.ShortID = "pr" + identifier;

        pnlRight.EnableViewState = false;
        pnlRight.CssClass = "Right" + cssClass;

        // Generate whole button
        CMSPanel pnlButton = new CMSPanel();
        pnlButton.ID = "pnlButton" + identifier;
        pnlButton.ShortID = BUTTON_PANEL_SHORTID + identifier;

        pnlButton.EnableViewState = false;
        if ((AllowSelection && identifier == SelectedIndex ) || (button.AllowToggle && button.IsToggled))
        {
            cssClass += SelectedSuffix;
        }

        pnlButton.CssClass = cssClass;

        if (button.AllowToggle)
        {
            pnlButton.CssClass += " Toggle";
            EnsureAllowToggleScript();
        }

        //Generate button table (IE7 issue)
        Table tabButton = new Table();
        TableRow tabRow = new TableRow();
        TableCell tabCellLeft = new TableCell();
        TableCell tabCellMiddle = new TableCell();
        TableCell tabCellRight = new TableCell();

        tabButton.CellPadding = 0;
        tabButton.CellSpacing = 0;

        tabButton.Rows.Add(tabRow);
        tabRow.Cells.Add(tabCellLeft);
        tabRow.Cells.Add(tabCellMiddle);
        tabRow.Cells.Add(tabCellRight);

        // Add inner controls
        tabCellLeft.Controls.Add(pnlLeft);
        tabCellMiddle.Controls.Add(pnlMiddle);
        tabCellRight.Controls.Add(pnlRight);

        pnlButton.Controls.Add(tabButton);

        pnlButton.ToolTip = tooltip ?? caption;

        outerPanel.Controls.Add(pnlButton);

        if (AllowDraggable)
        {
            mStartupScript.Append(String.Format("$j( '#{0}' ).draggable({{ helper:{1}, scope:'{2}' }});", pnlButton.ClientID, GetDraggableHandler(button.DraggableTemplateHandler), button.DraggableScope));
        }
        if (AllowSelection)
        {
            onClick = "SelectButton(this);" + onClick;
        }
        else if (button.AllowToggle)
        {
            onClick = "ToggleButton(this);" + onClick;
        }

        pnlButton.Attributes["onclick"] = CheckChanges ? "if (CheckChanges()) {" + onClick + "}" : onClick;

        // In case of horizontal layout
        if (HorizontalLayout)
        {
            // Stack buttons underneath
            pnlButton.Style.Add("clear", "both");
        }
        else
        {
            // Stack buttons side-by-side
            pnlButton.Style.Add("float", "left");
        }
    }


    /// <summary>
    /// Method for adding submenu.
    /// </summary>
    /// <param name="button">Menu item button</param>
    /// <param name="cssClass">Button css class</param>
    /// <param name="identifier">Button identifier</param>
    /// <param name="pnlSubItems">Sub menu panel</param>
    private void CreateSubMenu(MenuItem button, ref CMSPanel pnlSubItems, string cssClass, int identifier)
    {
        // Generate sub items container
        pnlSubItems = new CMSPanel();
        pnlSubItems.ID = "pnlSubItems" + identifier;
        pnlSubItems.ShortID = "ps" + identifier;
        pnlSubItems.CssClass = "SubMenuItems ContextMenu";

        // Forward scroll button
        Panel pnlForward = new Panel()
        {
            ID = "pnlForward",
            CssClass = "ForwardScroller"
        };

        // Backward scroll button
        Panel pnlBackward = new Panel()
        {
            ID = "pnlBackward",
            CssClass = "BackwardScroller"
        };

        // Scrollable area
        ScrollPanel pnlMenu = new ScrollPanel()
        {
            ID = "pnlCultureList",
            ShortID = "pcl" + identifier,
            CssClass = "ContextMenuContainer",
            ScrollAreaCssClass = "PortalContextMenu UniMenuContextMenu",
            Layout = RepeatDirection.Vertical,
            InnerItemClass = "Item",
            ForwardScrollerControlID = pnlForward.ID,
            BackwardScrollerControlID = pnlBackward.ID,
            ScrollStep = 200
        };

        pnlSubItems.Controls.Add(pnlForward);
        pnlSubItems.Controls.Add(pnlBackward);
        pnlSubItems.Controls.Add(pnlMenu);

        // Add menu items
        int subIdentifier = 0;
        foreach (SubMenuItem item in button.SubItems)
        {
            AddSubMenuItem(pnlMenu, item, cssClass, subIdentifier);
            subIdentifier++;
        }
    }


    /// <summary>
    /// Method for adding sub menu item into menu.
    /// </summary>
    /// <param name="pnlMenu">Menu container control</param>
    /// <param name="item">Sub menu item definition</param>
    /// <param name="cssClass">Button css class</param>
    /// <param name="subIdentifier">Button identifier</param>
    private void AddSubMenuItem(CMSPanel pnlMenu, SubMenuItem item, string cssClass, int subIdentifier)
    {
        if (String.IsNullOrEmpty(item.ControlPath))
        {
            // Generate sub item
            CMSPanel pnlItem = new CMSPanel();
            pnlItem.ID = "pnlSubItem" + subIdentifier;
            pnlItem.ShortID = "pi" + subIdentifier;
            pnlItem.CssClass = "Item ItemPadding";
            pnlItem.ToolTip = item.Tooltip;

            if (!String.IsNullOrEmpty(item.OnClientClick))
            {
                pnlItem.Attributes["onclick"] = item.OnClientClick.Replace("##BUTTON##", "$j(this).parents('." + cssClass + "').get(0)");
            }

            if (!String.IsNullOrEmpty(item.ImagePath))
            {
                string altText = String.IsNullOrEmpty(item.ImageAltText) ? item.Text : item.ImageAltText;

                // Generate sub items text
                CMSPanel pnlIcon = new CMSPanel();
                pnlIcon.ID = "pnlIcon";
                pnlIcon.ShortID = "pii" + subIdentifier;
                pnlIcon.CssClass = "Icon";
                Literal subItemImage = new Literal();
                subItemImage.ID = "ltlSubItemImage" + subIdentifier;
                subItemImage.EnableViewState = false;
                subItemImage.Text = String.Format("<img src=\"{0}\" alt=\"{1}\" />&nbsp;",
                        ResolveUrl(item.ImagePath),
                        GetString(altText));

                pnlIcon.Controls.Add(subItemImage);
                pnlItem.Controls.Add(pnlIcon);
            }

            // Generate sub items text
            CMSPanel pnlText = new CMSPanel();
            pnlText.ID = "pnlText";
            pnlText.ShortID = "pt" + subIdentifier;
            pnlText.CssClass = "Name";
            Label subItemText = new Label();
            subItemText.ID = "ltlSubItem" + subIdentifier;
            subItemText.EnableViewState = false;
            subItemText.Text = GetString(item.Text);

            pnlText.Controls.Add(subItemText);
            pnlItem.Controls.Add(pnlText);

            if (!String.IsNullOrEmpty(item.RightImagePath))
            {
                string altText = String.IsNullOrEmpty(item.RightImageAltText) ? item.Text : item.RightImageAltText;

                // Generate sub item right icon
                CMSPanel pnlRightIcon = new CMSPanel();
                pnlRightIcon.ID = "pnlRightIcon";
                pnlRightIcon.ShortID = "pri" + subIdentifier;
                pnlRightIcon.CssClass = "Icon IconRight";
                Literal subRightItemImage = new Literal();
                subRightItemImage.ID = "ltlSubRightItemImage" + subIdentifier;
                subRightItemImage.EnableViewState = false;
                subRightItemImage.Text = String.Format("<img class=\"RightIcon\" src=\"{0}\" alt=\"{1}\" />&nbsp;",
                        ResolveUrl(item.RightImagePath),
                        GetString(altText));

                pnlRightIcon.Controls.Add(subRightItemImage);
                pnlItem.Controls.Add(pnlRightIcon);
            }

            pnlMenu.Controls.Add(pnlItem);
        }
        else
        {
            // Load menu item control
            Control ctrl = this.LoadUserControl(item.ControlPath);
            if (ctrl != null)
            {
                pnlMenu.Controls.Add(ctrl);
            }
        }
    }


    /// <summary>
    /// Method for adding panel to group cell.
    /// </summary>
    /// <param name="tabGroupCell">Table cell to be added to</param>
    /// <param name="panelIndex">Index of panel to be added</param>
    private void AddPanelTo(TableCell tabGroupCell, int panelIndex)
    {
        int startIndex = (panelIndex * MaximumItems);

        // Create new instance of panel
        CMSPanel outerPanel = new CMSPanel();
        outerPanel.EnableViewState = false;
        outerPanel.ID = "pnlOuter" + panelIndex;
        outerPanel.ShortID = "po" + panelIndex;

        // In case of horizontal layout
        if (HorizontalLayout)
        {
            // Stack panels side-by-side
            outerPanel.Style.Add("float", CultureHelper.IsUICultureRTL() ? "right" : "left");
        }
        else
        {
            // Stack panels underneath
            outerPanel.Style.Add("clear", "both");
        }

        // Add panel to collection of panels
        tabGroupCell.Controls.Add(outerPanel);

        // Add buttons to panel
        for (int buttonIndex = startIndex; buttonIndex < startIndex + MaximumItems; buttonIndex++)
        {
            if (Buttons.Count > buttonIndex)
            {
                AddButtonTo(outerPanel, buttonIndex);
            }
        }
    }


    /// <summary>
    /// Method leading the menu construction.
    /// </summary>
    private void AddMainTable()
    {
        int panelCount = Buttons.Count / MaximumItems;
        if ((Buttons.Count % MaximumItems) > 0)
        {
            panelCount++;
        }

        Table tabGroup = new Table();
        tabGroup.CellPadding = 0;
        tabGroup.CellSpacing = 0;
        TableRow tabGroupRow = null;

        Controls.Clear();
        Controls.Add(tabGroup);

        // Add all panels to control
        for (int i = 0; i < panelCount; i++)
        {
            if (!HorizontalLayout || i == 0)
            {
                tabGroupRow = new TableRow();
                tabGroup.Rows.Add(tabGroupRow);
            }

            TableCell tabGroupCell = new TableCell();
            tabGroupCell.VerticalAlign = VerticalAlign.Top;
            tabGroupRow.Cells.Add(tabGroupCell);

            AddPanelTo(tabGroupCell, i);
        }
    }


    /// <summary>
    /// Method for script initialization.
    /// </summary>
    private void Initialize()
    {
        if (AllowSelection)
        {
            RegisterAllowSelectionScript();
        }

        mStartupScript = new StringBuilder();
    }


    /// <summary>
    /// Corrects image align for RTL cultures.
    /// </summary>
    /// <param name="align">Value to parse</param>
    /// <returns>Align of an image</returns>
    private static ImageAlign GetImageAlign(ImageAlign align)
    {
        // RTL switch
        if (CultureHelper.IsUICultureRTL())
        {
            if (align == ImageAlign.Left)
            {
                align = ImageAlign.Right;
            }
            else if (align == ImageAlign.Right)
            {
                align = ImageAlign.Left;
            }
        }

        return align;
    }


    /// <summary>
    /// Generates error label.
    /// </summary>
    /// <param name="message">Error message to display</param>
    /// <returns>Label with error message</returns>
    protected Label GetError(string message)
    {
        // If error occurs skip this group
        Label errorLabel = new Label();
        errorLabel.ID = "lblError";
        errorLabel.EnableViewState = false;
        errorLabel.Text = message;
        errorLabel.CssClass = "ErrorLabel";
        return errorLabel;
    }

    #endregion
}