<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="CMSModules_Chat_Controls_UI_ChatRoomUser_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<div style="margin-bottom: 20px;">
    <cms:LocalizedLabel ID="lblDisplayedUsers" runat="server" ResourceString="chat.chatroomusersnote"></cms:LocalizedLabel>
</div>

<cms:UniGrid runat="server" ID="gridElem" ObjectType="chat.roomuser"
    Columns="ChatRoomUserID, ChatRoomUserRoomID, ChatRoomUserChatUserID, ChatRoomUserJoinTime, ChatRoomUserAdminLevel" IsLiveSite="false" ShowObjectMenu="false"
    OrderBy="ChatRoomUserAdminLevel DESC, ChatRoomUserJoinTime DESC" RememberStateByParam="">
    <GridActions Parameters="ChatRoomUserID">
        <ug:Action Name="edit" Caption="$general.edit$" Icon="Edit.png" CommandArgument="ChatRoomUserID" ExternalSourceName="action_edit" />
        <ug:Action Name="kick" Caption="$chat.kick$" Icon="Kick.png" CommandArgument="ChatRoomUserID" ExternalSourceName="action_kick" />
        <ug:Action Name="revoke" Caption="$chat.kickuserperm$" Icon="Block.png" CommandArgument="ChatRoomUserID" ExternalSourceName="action_revoke" />
    </GridActions>
    <GridOptions DisplayFilter="true" />
    
    <GridColumns>
        <ug:Column Source="ChatRoomUserChatUserID" ExternalSourceName="ChatRoomUserChatUserID" Caption="$chat.user.nickname$" Wrap="false" /> 
        <ug:Column Source="ChatRoomUserAdminLevel" ExternalSourceName="AdminLevel" Caption="chat.adminlevel"></ug:Column>
        <ug:Column Source="ChatRoomUserJoinTime" ExternalSourceName="OnlineStatus" Caption="chat.online"></ug:Column>
        <ug:Column Width="100%" />
    </GridColumns>
</cms:UniGrid>
