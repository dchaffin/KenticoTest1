using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.Chat;
using CMS.SiteProvider;
using CMS.ExtendedControls;

public partial class CMSModules_Chat_Controls_UI_ChatRoom_List : CMSAdminListControl
{
    #region "Properties"

    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return this.gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
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
            this.gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
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
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Results will be filtered to this site.
    /// </summary>
    public int SiteID { get; set; }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        Grid.OnExternalDataBound += new OnExternalDataBoundEventHandler(Grid_OnExternalDataBound);
        Grid.OnAction += new OnActionEventHandler(Grid_OnAction);

        // Don't display rooms scheduled to deletion and WhisperRooms (the same as ChatRoomInfo's IsWhisperRoom)
        string whereCondition = "ChatRoomScheduledToDelete IS NULL AND (ChatRoomIsOneToOne = 0 OR ChatRoomIsSupport = 1)";

        // Single site is selected.
        if (SiteID > 0)
        {
            whereCondition += string.Format(" AND ChatRoomSiteID = {0}", SiteID);
        }
        // Global is selected
        else if (SiteID == -4)
        {
            whereCondition += " AND ChatRoomSiteID IS NULL";
        }
        // Global and current is selected
        else if (SiteID == -5)
        {
            whereCondition += string.Format(" AND (ChatRoomSiteID = {0} OR ChatRoomSiteID IS NULL)", CMSContext.CurrentSiteID);
        }

        Grid.WhereCondition = whereCondition;

        Grid.EditActionUrl = "Frameset.aspx?roomId={0}&siteid=" + SiteID;

        RegisterRefreshUsingPostBackScript();
    }


    void Grid_OnAction(string actionName, object actionArgument)
    {
        int chatRoomID = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName)
        {
            case "edit":
                SelectedItemID = chatRoomID;

                // Parent page will handle editing
                RaiseOnEdit();

                break;

            case "safedelete":
            case "approve":
                ChatRoomInfo room = ChatRoomInfoProvider.GetChatRoomInfo(chatRoomID);

                ((CMSChatPage)Page).CheckModifyPermission(room.ChatRoomSiteID);

                if (actionName == "safedelete")
                {
                    ChatRoomInfoProvider.SafeDelete(chatRoomID);
                }
                else
                {
                    if (room.ChatRoomEnabled)
                    {
                        ChatRoomHelper.DisableChatRoom(chatRoomID);
                    }
                    else
                    {
                        ChatRoomHelper.EnableChatRoom(chatRoomID);
                    }
                }

                break;
        }
    }


    object Grid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName.ToLowerCSafe();

        switch (sourceName)
        {
            case "chatroomcreatedbychatuserid":

                ChatUserInfo user = ChatUserInfoProvider.GetChatUserInfo(ValidationHelper.GetInteger(parameter, 0));
                if (user != null)
                {
                    return ChatHelper.GetCMSDeskChatUserField(this, user);
                }
                else
                {
                    return GetString("general.na");
                }
            case "approve":
            case "safedelete":
                DataRow row = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

                ChatRoomInfo room = ChatRoomInfoProvider.GetChatRoomInfo(ValidationHelper.GetInteger(row["ChatRoomID"], 0));

                if (room == null)
                {
                    return null;
                }
                
                bool enabled = ((CMSChatPage)Page).HasUserModifyPermission(room.ChatRoomSiteID);

                string iconName;
                string toolTipResourceString;

                if (sourceName == "approve")
                {
                    bool approve = ValidationHelper.GetBoolean(row["ChatRoomEnabled"], false);

                    if (!approve)
                    {
                        toolTipResourceString = "general.enable";
                        iconName = "Approve";
                    }
                    else
                    {
                        toolTipResourceString = "general.disable";
                        iconName = "Reject";
                    }

                    // Disable 'approve' or 'reject' action if room is one to one support
                    if (room.IsOneToOneSupport)
                    {
                        enabled = false;
                    }
                }
                else
                {
                    toolTipResourceString = "general.delete";
                    iconName = "Delete";
                }

                ImageButton button = ((ImageButton)sender);

                if (!enabled)
                {
                    button.Enabled = false;
                    iconName += "disabled";
                }

                button.ToolTip = GetString(toolTipResourceString);
                button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/" + iconName + ".png");

                break;
        }

        return parameter;
    }


    private void RegisterRefreshUsingPostBackScript()
    {
        string script = @"
function RefreshUsingPostBack()
{{
    {0};
}}";
        script = string.Format(script, ControlsHelper.GetPostBackEventReference(btnHiddenPostBackButton, null));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshUsingPostBack", script, true);
    }

    #endregion
}