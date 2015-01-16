using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;
using CMS.FormControls;

using AbstractUserControl = CMS.ExtendedControls.AbstractUserControl;

public partial class CMSAdminControls_UI_PageElements_HeaderActions : HeaderActions, IPostBackEventHandler
{
    #region "Variables"

    private List<HeaderAction> mActionsList = new List<HeaderAction>();
    private string[,] mActions = null;
    private string mIconCssClass = "NewItemImage";
    private string mLinkCssClass = null;
    private string mPanelCssClass = null;
    private int mSeparatorWidth = 20;
    private bool? mRenderSeparator = null;
    private bool? mUseSmallIcons = null;
    private bool mEnabled = true;
    private HeaderAction shortcutAction = null;
    private bool mPerformFullPostBack = true;
    private List<AbstractUserControl> mAdditionalControls = null;
    private List<CMSButton> mProcessedBaseButtons = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// CSS class of the context menu
    /// </summary>
    public string ContextMenuCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Component name
    /// </summary>
    public override string ComponentName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ComponentName"], base.ComponentName);
        }
        set
        {
            base.ComponentName = value;
            ViewState["ComponentName"] = value;
        }
    }


    /// <summary>
    /// If true, div with class "clearboth" is rendered.
    /// </summary>
    public bool UseClearElement
    {
        get
        {
            return pnlClear.Visible;
        }
        set
        {
            pnlClear.Visible = value;
        }
    }


    /// <summary>
    /// Indicates if control is enabled
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Update panel
    /// </summary>
    public override CMSUpdatePanel UpdatePanel
    {
        get
        {
            return pnlUp;
        }
    }


    /// <summary>
    /// Indicates if the actions should perform full post-back
    /// </summary>
    public override bool PerformFullPostBack
    {
        get
        {
            return mPerformFullPostBack;
        }
        set
        {
            mPerformFullPostBack = value;
        }
    }


    /// <summary>
    /// Gets or sets the array of actions. The meaning of the indexes:
    /// (0,0): Type of the link (HyperLink or LinkButton), 
    /// (0,1): Text of the action link, 
    /// (0,2): JavaScript command to be performed OnClick action, 
    /// (0,3): NavigationUrl of the HyperLink (or PostBackUrl of LinkButton), 
    /// (0,4): Tooltip of the action link, 
    /// (0,5): Action image URL, 
    /// (0,6): CommandName of the LinkButton Command event, 
    /// (0,7): CommandArgument of the LinkButton Command event.
    /// (0,8): Register shortcut action (TRUE/FALSE)
    /// (0,9): Enabled state of the action (TRUE/FALSE)
    /// (0,10): Visibility of the action (TRUE/FALSE)
    /// (0,11): Hyperlink target (only if type is hyperlink).
    /// (0,12): Use ImageButton instead of Image (TRUE/FALSE).
    /// At least first two arguments must be defined.
    /// </summary>
    public override string[,] Actions
    {
        get
        {
            // Initialize actions from list for backward compatibility
            if ((mActions == null) && (ActionsList != null) && (ActionsList.Count > 0))
            {
                mActions = GetActionsFromList(ActionsList);
            }

            return mActions;
        }
        set
        {
            mActions = value;
        }
    }


    /// <summary>
    /// List of actions
    /// </summary>
    public override List<HeaderAction> ActionsList
    {
        get
        {
            return mActionsList;
        }
        set
        {
            mActionsList = value;
        }
    }


    /// <summary>
    /// Indicates if separator should be rendered
    /// </summary>
    public override bool RenderSeparator
    {
        get
        {
            if (mRenderSeparator == null)
            {
                mRenderSeparator = UseBasicStyles || IsLiveSite;
            }

            return mRenderSeparator.Value;
        }
        set
        {
            mRenderSeparator = value;
        }
    }


    /// <summary>
    /// Gets or sets the space between two actions (in pixels).
    /// </summary>
    public override int SeparatorWidth
    {
        get
        {
            return mSeparatorWidth;
        }
        set
        {
            mSeparatorWidth = value;
        }
    }


    /// <summary>
    /// Gets or sets CssClass of the Image element of the action.
    /// </summary>
    public override string IconCssClass
    {
        get
        {
            return mIconCssClass;
        }
        set
        {
            mIconCssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets CssClass of the HyperLink or LinkButton element of the action.
    /// </summary>
    public override string LinkCssClass
    {
        get
        {
            return mLinkCssClass ?? (mLinkCssClass = (UseBasicStyles || IsLiveSite) ? "NewItemLink" : "MenuItemEdit");
        }
        set
        {
            mLinkCssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets CssClass of the panel where all the actions are placed.
    /// </summary>
    public override string PanelCssClass
    {
        get
        {
            return mPanelCssClass;
        }
        set
        {
            mPanelCssClass = value;
        }
    }


    /// <summary>
    /// Help topic name.
    /// </summary>
    public override string HelpTopicName
    {
        get
        {
            return helpElem.TopicName;
        }
        set
        {
            helpElem.TopicName = value;
        }
    }


    /// <summary>
    /// Help name.
    /// </summary>
    public override string HelpName
    {
        get
        {
            return helpElem.HelpName;
        }
        set
        {
            helpElem.HelpName = value;
        }
    }


    /// <summary>
    /// Help icon URL.
    /// </summary>
    public override string HelpIconUrl
    {
        get
        {
            return helpElem.IconUrl;
        }
        set
        {
            helpElem.IconUrl = value;
        }
    }


    /// <summary>
    /// Indicates if small icons should be used for actions
    /// </summary>
    public override bool UseSmallIcons
    {
        get
        {
            if (mUseSmallIcons == null)
            {
                mUseSmallIcons = !UseBasicStyles && !IsLiveSite;
            }

            return mUseSmallIcons.Value;
        }
        set
        {
            mUseSmallIcons = value;
        }
    }


    /// <summary>
    /// Indicates if control is visible
    /// </summary>
    public override bool Visible
    {
        get
        {
            // Get visibility dynamically
            return base.Visible && (ActionsList.Any(action => action.IsVisible()) || !string.IsNullOrEmpty(HelpTopicName));
        }
        set
        {
            base.Visible = value;
        }
    }


    /// <summary>
    /// Additional controls list
    /// </summary>
    public override List<AbstractUserControl> AdditionalControls
    {
        get
        {
            if (mAdditionalControls == null)
            {
                mAdditionalControls = new List<AbstractUserControl>();
            }

            return mAdditionalControls;
        }
    }


    /// <summary>
    /// List of processed base buttons
    /// </summary>
    protected List<CMSButton> ProcessedBaseButtons
    {
        get
        {
            if (mProcessedBaseButtons == null)
            {
                mProcessedBaseButtons = new List<CMSButton>();
            }

            return mProcessedBaseButtons;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page init 
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        ReloadAdditionalControls();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Get type safe configuration
        InitializeActionsList(mActions);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();

        // Hide processed base buttons
        ProcessedBaseButtons.ForEach(b => b.Visible = false);
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if ((Context != null) && RenderContainer)
        {
            string cssClass = (UseBasicStyles || IsLiveSite) ? "PageHeaderLine" : "CMSEditMenu";
            writer.Write(string.Format("<div class\"{0}\">", cssClass));
        }

        base.Render(writer);

        if ((Context != null) && RenderContainer)
        {
            writer.Write("</div>");
        }

        if (shortcutAction != null)
        {
            PostBackOptions opt = new PostBackOptions(this, shortcutAction.CommandArgument)
            {
                PerformValidation = true,
                ValidationGroup = shortcutAction.ValidationGroup
            };

            Page.ClientScript.RegisterForEventValidation(opt);
        }
    }


    /// <summary>
    /// Initializes actions list
    /// </summary>
    /// <param name="actions">Actions</param>
    private void InitializeActionsList(string[,] actions)
    {
        // Convert array of actions
        if ((actions != null) && (ActionsList.Count == 0))
        {
            ActionsList = new List<HeaderAction>();

            // Get the number of actions
            int actionCount = actions.GetUpperBound(0) + 1;

            // Get the array size (number of arguments)
            int arraySize = actions.GetUpperBound(1) + 1;

            // Exit if nothing about the action is specified
            if (arraySize < 2)
            {
                return;
            }

            // Generate the actions
            for (int i = 0; i < actionCount; i++)
            {
                string type = (actions[i, 0] != null) ? actions[i, 0].ToLowerCSafe() : null;
                HeaderAction action = (type == TYPE_SAVEBUTTON) ? new SaveAction(Page) : new HeaderAction() { IsLiveSite = IsLiveSite };

                // Get the action parameters
                action.Text = (actions[i, 1]);
                action.Visible = (!(arraySize > 10) || ValidationHelper.GetBoolean(actions[i, 10], true));
                action.OnClientClick = ((arraySize > 2) ? actions[i, 2] : null);
                action.RedirectUrl = ((arraySize > 3) ? actions[i, 3] : null);
                action.Tooltip = ((arraySize > 4) ? actions[i, 4] : null);
                action.ImageUrl = ((arraySize > 5) ? actions[i, 5] : null);
                action.CommandName = ((arraySize > 6) ? actions[i, 6] : (arraySize > 13) ? actions[i, 13] : null);
                action.CommandArgument = ((arraySize > 7) ? actions[i, 7] : null);
                action.RegisterShortcutScript = ((arraySize > 8) && ValidationHelper.GetBoolean(actions[i, 8], false));
                action.Enabled = (!(arraySize > 9) || ValidationHelper.GetBoolean(actions[i, 9], true));
                action.Target = ((arraySize > 11) ? ValidationHelper.GetString(actions[i, 11], string.Empty) : string.Empty);
                action.UseImageButton = ((arraySize > 12) && ValidationHelper.GetBoolean(actions[i, 12], false));
                action.CssClass = LinkCssClass;

                ActionsList.Add(action);
            }
        }
    }


    /// <summary>
    /// Initializes additional controls list
    /// </summary>
    public override void ReloadAdditionalControls()
    {
        plcAdditionalControls.Controls.Clear();
        foreach (AbstractUserControl ctrl in AdditionalControls)
        {
            plcAdditionalControls.Controls.Add(ctrl);
        }
    }


    /// <summary>
    /// Gets actions from actions list
    /// </summary>
    /// <param name="actionsList">Actions list</param>
    private string[,] GetActionsFromList(List<HeaderAction> actionsList)
    {
        int actionCount = actionsList.Count;
        string[,] actions = new string[actionCount, 14];

        // Generate the actions
        for (int i = 0; i < actionCount; i++)
        {
            actions[i, 1] = actionsList[i].Text;
            actions[i, 2] = actionsList[i].OnClientClick;
            actions[i, 3] = actionsList[i].RedirectUrl;
            actions[i, 4] = actionsList[i].Tooltip;
            actions[i, 5] = actionsList[i].ImageUrl;
            actions[i, 6] = actionsList[i].CommandName;
            actions[i, 7] = actionsList[i].CommandArgument;
            actions[i, 8] = actionsList[i].RegisterShortcutScript.ToString();
            actions[i, 9] = actionsList[i].Enabled.ToString();
            actions[i, 10] = actionsList[i].Visible.ToString();
            actions[i, 11] = actionsList[i].Target;
            actions[i, 12] = actionsList[i].UseImageButton.ToString();
            actions[i, 13] = actionsList[i].CommandName;
            actions[i, 0] = TYPE_HYPERLINK;
        }

        return actions;
    }


    /// <summary>
    /// Reloads the actions.
    /// </summary>
    public override void ReloadData()
    {
        // Get type safe configuration
        InitializeActionsList(mActions);

        int actionsCount = ActionsList.Count;
        if (actionsCount > 0)
        {
            pnlActions.Controls.Clear();

            if (!String.IsNullOrEmpty(PanelCssClass))
            {
                pnlActions.CssClass = PanelCssClass;
            }

            // If there is more then one action render them to table
            bool useTable = (actionsCount > 1);

            CreateActions(ActionsList, pnlActions, 0, useTable, false);
        }

        pnlAdditionalControls.CssClass += " " + AdditionalControlsCssClass;

        // Use help icon
        pnlHelp.Visible = !string.IsNullOrEmpty(HelpTopicName);
    }


    /// <summary>
    /// Adds action.
    /// </summary>
    /// <param name="action">Action</param>
    public override void AddAction(HeaderAction action)
    {
        if (action == null)
        {
            return;
        }

        // Make sure the Save action is set only once
        string key = string.Format("HeaderActionsSaveSet_{0}_{1}", action.CommandArgument, ClientID);
        bool saveSet = ValidationHelper.GetBoolean(RequestStockHelper.GetItem(key), false);
        if (!(action is SaveAction) || !saveSet)
        {
            bool added = false;

            // Ensure correct index
            if (action.Index == -1)
            {
                action.Index = ActionsList.Count;
            }
            else
            {
                // Post processing of action attribute
                for (int i = 0; i < ActionsList.Count; i++)
                {
                    if (ActionsList[i].Index == action.Index)
                    {
                        // Replace action with the same index
                        ActionsList[i] = action;

                        // Button added
                        added = true;
                        break;
                    }
                }
            }

            // If action with the same index was not found, add it to the list
            if (!added)
            {
                ActionsList.Add(action);
            }

            // Keep flag
            if (action is SaveAction)
            {
                RequestStockHelper.Add(key, (action.BaseButton == null) || action.BaseButton.Visible);
            }
        }

        // Store base buttons
        if ((action.BaseButton != null) && !ProcessedBaseButtons.Contains(action.BaseButton))
        {
            ProcessedBaseButtons.Add(action.BaseButton);
        }
    }


    /// <summary>
    /// Inserts action at specified index.
    /// </summary>
    /// <param name="index">Index</param>
    /// <param name="action">Action</param>
    public override void InsertAction(int index, HeaderAction action)
    {
        if (action == null)
        {
            return;
        }

        action.Index = index;
        AddAction(action);
    }


    private void CreateActions(List<HeaderAction> actions, Control parent, int level, bool useTable, bool renderAsContextMenu)
    {
        int actionsCount = actions.Count;
        if (useTable)
        {
            parent.Controls.Add(new LiteralControl("<table cellpadding=\"0\" cellspacing=\"0\" class=\"_nodivs\"><tr>"));
        }

        // Sort actions by index to be sure the order is ensured for multiple actions
        if (actionsCount > 1)
        {
            // At least one action has index
            if (ActionsList.Exists(a => (a.Index != -1)))
            {
                // Sort the actions
                ActionsList.Sort((a1, a2) => a1.Index.CompareTo(a2.Index));
            }
        }

        // Generate the actions
        for (int i = 0; i < actionsCount; ++i)
        {
            var action = actions[i];

            // If the text is not specified or visibility is false, skip the action
            FormSubmitButton formButton = action.BaseButton as FormSubmitButton;
            if (!action.IsVisible() || ((action.BaseButton != null) && (!action.BaseButton.Visible || ((formButton != null) && !formButton.RegisterHeaderAction))))
            {
                // Skip empty action
                action.Visible = false;
                continue;
            }

            // Set live site flag for resource strings
            action.IsLiveSite = IsLiveSite;

            // Start tag of the table if more then one action is defined
            if (useTable)
            {
                if (action.GenerateSeparatorBeforeAction && (i > 0))
                {
                    parent.Controls.Add(new LiteralControl("<td class=\"PadSeparator\"><span></span></td>"));
                }
                parent.Controls.Add(new LiteralControl("<td>"));

            }

            // Context menu container
            ContextMenuContainer container = null;
            bool enabled = Enabled && action.Enabled;
            bool renderCtxMenu = (action.AlternativeActions.Count > 0);
            if (renderCtxMenu)
            {
                container = new ContextMenuContainer
                {
                    ID = "menuCont" + i,
                    MenuID = ClientID + "m_" + level + i,
                    ParentElementClientID = ClientID,
                    RenderAsTag = HtmlTextWriterTag.Div,
                    MouseButton = MouseButtonEnum.Both,
                    OffsetY = 1,
                    VerticalPosition = VerticalPositionEnum.Bottom,
                    HorizontalPosition = HorizontalPositionEnum.Left,
                    CssClass = "CtxContainer",
                    Enabled = enabled,
                    IsLiveSite = IsLiveSite
                };

                if (action.Inactive)
                {
                    parent.Controls.Add(container);
                }
            }


            // Get the action parameters
            string ctrlId = ID + "_HA_" + level + i;
            string cssClass = !string.IsNullOrEmpty(action.CssClass) ? action.CssClass : LinkCssClass;
            string commandName = !string.IsNullOrEmpty(action.CommandName) ? action.CommandName : action.EventName;
            string commandArgument = action.CommandArgument; 
            string url = ResolveUrl(action.RedirectUrl);

            // Add HyperLink
            HyperLink link = new HyperLink();
            link.ID = ctrlId;
            link.ToolTip = action.Tooltip;
            link.CssClass = enabled ? cssClass : (cssClass + "Disabled");
            link.Enabled = enabled;
            link.Text = action.Text;

            // Add control to the panel
            AddControlToPanel(link, (action.Inactive && (container != null)) ? container : parent, renderAsContextMenu);

            if (enabled && !action.Inactive)
            {
                // Perform post-back
                if (!String.IsNullOrEmpty(commandName) || !String.IsNullOrEmpty(commandArgument))
                {
                    string script = null;
                    // Register encapsulation function
                    if (!string.IsNullOrEmpty(action.OnClientClick))
                    {
                        link.Attributes.Add("onclick", action.OnClientClick);
                        if (action.RegisterShortcutScript)
                        {
                            script = "if(!PerfAction_" + ctrlId + "()) { return false; }";
                            string scriptFunction = "function PerfAction_" + ctrlId + "() { " + action.OnClientClick.TrimEnd(';') + "; return true; }";
                            ScriptHelper.RegisterStartupScript(Page, typeof(string), "PerfAction_" + ctrlId, scriptFunction, true);
                        }
                    }

                    string argument = string.Join(";", new string[] { commandName, commandArgument });
                    PostBackOptions opt = new PostBackOptions(this, argument)
                    {
                        PerformValidation = true,
                        ValidationGroup = action.ValidationGroup
                    };

                    string postbackScript = ControlsHelper.GetPostBackEventReference(this, opt, false, !PerformFullPostBack);
                    link.NavigateUrl = "javascript:" + postbackScript;

                    // Register the CRTL+S shortcut
                    if (action.RegisterShortcutScript)
                    {
                        // Store action information for shortcut event validation registration
                        shortcutAction = new HeaderAction() { IsLiveSite = IsLiveSite, CommandArgument = argument, ValidationGroup = action.ValidationGroup };

                        // Prepare action script
                        string actionScript = script + " " + postbackScript + ";";

                        // SaveObject function will simply simulate clicking the control
                        string scriptSave = string.Format(@"function SaveObject() {{ {0} }}", actionScript);
                        ScriptHelper.RegisterShortcuts(Page);
                        ScriptHelper.RegisterStartupScript(Page, typeof(string), ScriptHelper.SAVE_DOCUMENT_SCRIPT_KEY, scriptSave, true);
                    }
                }
                else
                {
                    link.Attributes.Add("onclick", action.OnClientClick);

                    // Use URL only for standard link
                    if (!String.IsNullOrEmpty(action.RedirectUrl))
                    {
                        link.NavigateUrl = url;
                        link.Target = action.Target;
                    }

                    // Register the CRTL+S shortcut for empty command name
                    if (action.RegisterShortcutScript)
                    {
                        // SaveObject function will simply simulate clicking the control
                        string scriptSave = string.Format(@"function SaveObject() {{ {0} }}", action.OnClientClick);
                        ScriptHelper.RegisterShortcuts(Page);
                        ScriptHelper.RegisterStartupScript(Page, typeof(string), ScriptHelper.SAVE_DOCUMENT_SCRIPT_KEY, scriptSave, true);
                    }
                }

            }

            // Create image if URL is not empty
            Image img = null;
            string imageUrl = (UseSmallIcons && !string.IsNullOrEmpty(action.SmallImageUrl)) ? action.SmallImageUrl : action.ImageUrl;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                if (action.UseImageButton)
                {
                    ImageButton imgButton = new ImageButton();
                    if (!action.Inactive && enabled)
                    {
                        imgButton.PostBackUrl = url;
                    }

                    img = imgButton;
                }
                else
                {
                    img = new Image();
                }

                // Set common properties
                img.ID = ID + "_HAI_" + level + i;
                img.ImageUrl = imageUrl;
                img.ToolTip = action.Tooltip;
                img.AlternateText = action.Text;
                img.CssClass = IconCssClass;
                img.Enabled = enabled;
            }

            // Add controls to the panel
            if (img != null)
            {
                link.Controls.Add(img);
            }

            if (!string.IsNullOrEmpty(action.Text))
            {
                link.Controls.Add(new Label { Text = action.Text, CssClass = "Text" });
            }

            // Alternative actions
            if (renderCtxMenu)
            {
                if (!action.Inactive)
                {
                    container.RenderAsTag = HtmlTextWriterTag.Span;
                    container.OffsetX = -1;
                    parent.Controls.Add(container);
                    container.Controls.Add(new LiteralControl("<span class=\"SepContextButton\"><span class=\"Arrow\"></span></span>"));
                }
                else
                {
                    link.Controls.Add(new LiteralControl("<span class=\"ContextButton\"><span class=\"Arrow\"></span></span>"));
                }

                // Context menu
                ContextMenu menu = new ContextMenu();
                container.MenuControl = menu;
                container.InitializeMenuControl(menu);

                if (!String.IsNullOrEmpty(ContextMenuCssClass))
                {
                    menu.OuterCssClass = this.ContextMenuCssClass;
                }
                menu.CssClass = "PortalContextMenu CMSEditMenu";

                CreateActions(action.AlternativeActions, menu, i, false, true);
            }

            // Do not insert padding for last item
            if (i < actionsCount - 1)
            {
                parent.Controls.Add(new LiteralControl("<span class=\"Pad\"></span>"));
            }

            // Add free separator cell in table if it's not the last action
            if (useTable)
            {
                parent.Controls.Add(new LiteralControl(((i < actionsCount - 1) && RenderSeparator ? "</td><td style=\"width:" + SeparatorWidth + "px; \" />" : "</td>")));
            }
        }

        // End tag of the table
        if (useTable)
        {
            parent.Controls.Add(new LiteralControl("</tr></table>"));
        }
    }


    private void AddControlToPanel(Control ctrl, Control parent, bool renderAsContextMenu)
    {
        if (renderAsContextMenu)
        {
            Panel pnl = new Panel { CssClass = "Item" };
            Panel pnlPad = new Panel { CssClass = "ItemPadding" };
            parent.Controls.Add(pnl);
            pnl.Controls.Add(pnlPad);
            pnlPad.Controls.Add(ctrl);
            pnlPad.Controls.Add(new LiteralControl("<div class=\"Clear\"></div>"));
        }
        else
        {
            parent.Controls.Add(ctrl);
        }
    }


    /// <summary>
    /// Clears content rendered by header actions control.
    /// </summary>
    public void Clear()
    {
        pnlActions.Controls.Clear();
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Raise post-back event
    /// </summary>
    /// <param name="eventArgument">Argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (!string.IsNullOrEmpty(eventArgument))
        {
            string[] argValues = eventArgument.Split(';');
            if (argValues.Length == 2)
            {
                CommandEventArgs args = new CommandEventArgs(argValues[0], argValues[1]);
                RaiseActionPerformed(this, args);

                // Update parent update panel in conditional mode
                var up = ControlsHelper.GetUpdatePanel(this);
                if ((up != null) && (up.UpdateMode == UpdatePanelUpdateMode.Conditional))
                {
                    up.Update();
                }
            }
        }
    }

    #endregion
}