using System;
using System.Text;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Synchronization;

public partial class CMSModules_Objects_Controls_Locking_ObjectEditMenu : ObjectEditMenu, IObjectEditMenu
{
    #region "Variables"

    // Actions
    protected SaveAction save = null;
    protected HeaderAction checkin = null;
    protected HeaderAction checkout = null;
    protected HeaderAction undocheckout = null;

    private CMSObjectManager mObjectManager = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Menu control.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return menu;
        }
    }


    /// <summary>
    /// Show the check in with comment button.
    /// </summary>
    public bool ShowCheckInWithComment
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the menu is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return menu.Enabled;
        }
        set
        {
            menu.Enabled = value;
        }
    }


    /// <summary>
    /// Object instance.
    /// </summary>
    public override BaseInfo InfoObject
    {
        get
        {
            return ObjectManager.InfoObject;
        }
    }


    /// <summary>
    /// Object manager control
    /// </summary>
    public CMSObjectManager ObjectManager
    {
        get
        {
            if (mObjectManager == null)
            {
                mObjectManager = ControlsHelper.GetChildControl(Page, typeof(CMSObjectManager)) as CMSObjectManager;
                if (mObjectManager == null)
                {
                    throw new Exception("[ObjectEditMenu.ObjectManager]: Missing object manager.");
                }
            }

            return mObjectManager;
        }
    }


    /// <summary>
    /// Gets the associated object manager control.
    /// </summary>
    public ICMSObjectManager AbstractObjectManager
    {
        get
        {
            return ObjectManager;
        }
    }


    /// <summary>
    /// If true, the access permissions to the items are checked.
    /// </summary>
    public override bool CheckPermissions
    {
        get
        {
            return ObjectManager.CheckPermissions;
        }
        set
        {
            ObjectManager.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return ObjectManager.StopProcessing;
        }
        set
        {
            ObjectManager.StopProcessing = value;
        }
    }

    #endregion


    #region "Constructors"

    /// <summary>
    /// Constructor
    /// </summary>
    public CMSModules_Objects_Controls_Locking_ObjectEditMenu()
    {
        RefreshInterval = 500;
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        // Handle callback in OnLoad event because of ShortIDs
        var parameters = Request.Form["params"];
        if (parameters != null)
        {
            if (parameters.StartsWith(CALLBACK_ID + ClientID))
            {
                string[] args = parameters.Split(new string[] { CALLBACK_SEP }, StringSplitOptions.None);
                ObjectManager.Mode = FormModeEnum.Update;
                ObjectManager.ObjectType = ValidationHelper.GetString(args[1], null);
                ObjectManager.ObjectID = ValidationHelper.GetInteger(args[2], 0);
                return;
            }
        }

        base.OnLoad(e);

        if (!string.IsNullOrEmpty(HelpTopicName))
        {
            helpElem.TopicName = HelpTopicName;
            pnlHelp.Visible = true;
        }

        if (!string.IsNullOrEmpty(HelpName))
        {
            helpElem.HelpName = HelpName;
        }

        // Perform full post-back if not in update panel
        menu.PerformFullPostBack = !ControlsHelper.IsInUpdatePanel(this);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadMenu();

        if (menu.Visible || pnlHelp.Visible)
        {
            if (PreviewMode)
            {
                pnlMenu.CssClass = "CMSEditMenu";
            }

            if (CMSObjectHelper.UseCheckinCheckout && ObjectManager.ShowPanel && (InfoObject != null) && (InfoObject.Generalized.ObjectID > 0) && InfoObject.TypeInfo.SupportsLocking)
            {
                ISynchronizedInfo synchronizedInfo = InfoObject as ISynchronizedInfo;
                if (synchronizedInfo != null)
                {
                    ObjectSettingsInfo settings = synchronizedInfo.ObjectSettings;

                    int currentUserId = CurrentUser.UserID;

                    // Get edited info object type and name for use with info messages
                    var objectType = GetString(TypeHelper.GetObjectTypeResourceKey(InfoObject.ObjectType));
                    var objectName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(InfoObject.Generalized.ObjectDisplayName));

                    // Object is checked out by another user, disable editing for current user
                    if (currentUserId != settings.ObjectCheckedOutByUserID)
                    {
                        // Disable actions for current user
                        if (save != null)
                        {
                            save.Enabled = false;
                        }

                        if (InfoObject.Generalized.IsCheckedOut)
                        {
                            if (checkin != null)
                            {
                                checkin.Visible = false;
                            }
                            if (checkout != null)
                            {
                                checkout.Visible = false;
                            }
                            if (undocheckout != null)
                            {
                                // Always allow undo-checkout for Global Admin
                                undocheckout.Visible = CMSContext.CurrentUser.UserSiteManagerAdmin;
                            }

                            string userName = null;
                            UserInfo ui = UserInfoProvider.GetUserInfo(settings.ObjectCheckedOutByUserID);
                            if (ui != null)
                            {
                                userName = HTMLHelper.HTMLEncode(ui.GetFormattedUserName(IsLiveSite));
                            }

                            AddInfoText(string.Format(GetString("ObjectEditMenu.CheckedOutByAnotherUser"), objectType, objectName, userName));
                        }
                        else
                        {
                            AddInfoText(string.Format(GetString("ObjectEditMenu.CheckOutToEdit"), objectType, objectName));
                        }
                    }
                    else if (InfoObject.Generalized.IsCheckedOut)
                    {
                        AddInfoText(string.Format(GetString("ObjectEditMenu.CheckInToSubmit"), objectType, objectName));
                    }
                }
            }

            RegisterActionScripts();
        }

        // Hide menu if required
        pnlMenu.Visible &= menu.Visible || pnlHelp.Visible;

        // Set the information text
        if (!String.IsNullOrEmpty(InformationText))
        {
            lblInfo.Text = InformationText;
            lblInfo.Visible = true;
            pnlInfoWrapper.Visible = true;
        }
    }


    /// <summary>
    /// Adds information text to the information label
    /// </summary>
    /// <param name="text"></param>
    private void AddInfoText(string text)
    {
        if (!String.IsNullOrEmpty(InformationText))
        {
            InformationText = text + "<br />" + InformationText;
        }
        else
        {
            InformationText = text;
        }
    }


    /// <summary>
    /// Registers action scripts
    /// </summary>
    private void RegisterActionScripts()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("function CheckConsistency_", ClientID, "() { ", ObjectManager.GetJSFunction("CONS", null, null), "; } \n");

        if ((checkin != null) && checkin.Visible && ShowCheckInWithComment)
        {
            sb.Append("function AddComment_", ClientID, "(name, objectType, objectId, menuId) { ", ObjectManager.GetJSFunction(ComponentEvents.COMMENT, "name|objectType|objectId|menuId", null), "; } \n");
        }

        // Register the script
        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "AutoMenuActions" + ClientID, ScriptHelper.GetScript(sb.ToString()));
    }

    #endregion


    #region "Methods"

    private void ClearProperties()
    {
        // Clear actions
        save = null;
        checkin = null;
        checkout = null;
        undocheckout = null;

        // Clear security result
        ObjectManager.ClearProperties();
    }


    private void ReloadMenu()
    {
        if (StopProcessing)
        {
            return;
        }

        bool displayObjectMenu = false;
        BaseInfo editInfo = InfoObject ?? TranslationHelper.GetReadOnlyObject(ObjectManager.ObjectType);
        if (editInfo != null)
        {
            // Do not display items when object does not support locking and when there is no associated UIForm
            displayObjectMenu = editInfo.TypeInfo.SupportsLocking && ObjectManager.ShowPanel;
        }

        if (displayObjectMenu)
        {
            // Handle several reloads
            menu.ActionsList.Clear();
            ClearProperties();

            if (!HideStandardButtons)
            {
                // Handle save action
                if (ShowSave)
                {
                    save = new SaveAction(Page)
                    {
                        OnClientClick = (AllowSave ? RaiseGetClientValidationScript(ComponentEvents.SAVE, ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null)) : null),
                        Tooltip = ResHelper.GetString("EditMenu.Save", ResourceCulture),
                        Enabled = AllowSave,
                        EventName = "",
                        CommandName = ""
                    };

                    AddAction(save);
                }

                // Object update
                if (CMSObjectHelper.UseCheckinCheckout && (ObjectManager.Mode == FormModeEnum.Update))
                {
                    if (InfoObject != null)
                    {
                        if (ShowCheckOut)
                        {
                            checkout = new HeaderAction()
                            {
                                OnClientClick = (AllowCheckOut ? RaiseGetClientValidationScript(ComponentEvents.CHECKOUT, ObjectManager.GetJSFunction(ComponentEvents.CHECKOUT, null, null)) : null),
                                Tooltip = ResHelper.GetString("ObjectEditMenu.Checkout", ResourceCulture),
                                Text = ResHelper.GetString("EditMenu.IconCheckout", ResourceCulture),
                                ImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_Content/EditMenu/checkout.png"),
                                SmallImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_Content/EditMenu/16/checkout.png"),
                                Enabled = AllowCheckOut
                            };

                            AddAction(checkout);
                        }

                        if (ShowUndoCheckOut)
                        {
                            undocheckout = new HeaderAction()
                            {
                                OnClientClick = (AllowUndoCheckOut ? RaiseGetClientValidationScript(ComponentEvents.UNDO_CHECKOUT, ObjectManager.GetJSFunction(ComponentEvents.UNDO_CHECKOUT, null, null)) : null),
                                Tooltip = ResHelper.GetString("ObjectEditMenu.UndoCheckOut", ResourceCulture),
                                Text = ResHelper.GetString("EditMenu.IconUndoCheckout", ResourceCulture),
                                ImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_Content/EditMenu/undocheckout.png"),
                                SmallImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_Content/EditMenu/16/undocheckout.png"),
                                Enabled = AllowUndoCheckOut
                            };

                            AddAction(undocheckout);
                        }

                        if (ShowCheckIn)
                        {
                            checkin = new HeaderAction()
                            {
                                OnClientClick = (AllowCheckIn ? RaiseGetClientValidationScript(ComponentEvents.CHECKIN, ObjectManager.GetJSFunction(ComponentEvents.CHECKIN, null, null)) : null),
                                Tooltip = ResHelper.GetString("ObjectEditMenu.Checkin", ResourceCulture),
                                Text = ResHelper.GetString("EditMenu.IconCheckin", ResourceCulture),
                                ImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_Content/EditMenu/checkin.png"),
                                SmallImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_Content/EditMenu/16/checkin.png"),
                                Enabled = AllowCheckIn
                            };

                            if (ShowCheckInWithComment)
                            {
                                AddCommentAction(ComponentEvents.CHECKIN, checkin);
                            }
                            else
                            {
                                AddAction(checkin);
                            }
                        }
                    }
                }
            }

            // Add extra actions
            if (mExtraActions != null)
            {
                foreach (HeaderAction action in mExtraActions)
                {
                    AddAction(action);
                }
            }
        }

        menu.LinkCssClass = LinkCssClass;
    }


    /// <summary>
    /// Adds menu action.
    /// </summary>
    /// <param name="action">Action</param>
    protected void AddAction(HeaderAction action)
    {
        if (action != null)
        {
            // Action
            menu.ActionsList.Add(action);
        }
    }


    /// <summary>
    /// Adds comment action.
    /// </summary>
    /// <param name="name">Action name</param>
    /// <param name="action">Current action</param>
    private void AddCommentAction(string name, HeaderAction action)
    {
        ObjectManager.RenderScript = true;

        AddAction(action);

        CommentAction comment = new CommentAction(Page, name)
        {
            Text = ResHelper.GetString("ObjectEditMenu.Comment" + name, ResourceCulture),
            OnClientClick = string.Format("AddComment_{0}('{1}','{2}',{3},'{0}');", ClientID, name, InfoObject.ObjectType, InfoObject.Generalized.ObjectID),
        };
        action.AlternativeActions.Add(comment);
    }

    #endregion
}
