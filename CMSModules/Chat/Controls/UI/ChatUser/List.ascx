<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Chat_Controls_UI_ChatUser_List" CodeFile="List.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<div style="margin-bottom: 20px;">
    <cms:LocalizedLabel ID="lblOnlineStateApplies" runat="server" ResourceString="chat.onlynonanonymoususersareshown"></cms:LocalizedLabel>
</div>

<cms:UniGrid runat="server" ID="gridElem" ObjectType="chat.user" OrderBy="ChatUserNickname" Columns="ChatUserID,ChatUserNickname,CASE WHEN EXISTS(SELECT * FROM Chat_OnlineUser COU WHERE COU.ChatOnlineUserChatUserID = ChatUserID AND COU.ChatOnlineUserJoinTime IS NOT NULL) THEN 1 ELSE 0 END AS IsOnline"
    IsLiveSite="false" EditActionUrl="Edit.aspx?userId={0}" WhereCondition="ChatUserUserID IS NOT NULL" ShowObjectMenu="false" RememberStateByParam="">
    <GridActions Parameters="ChatUserID">
        <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
    </GridActions>
    <GridOptions DisplayFilter="true" FilterLimit="10" />
    <GridColumns>
        <ug:Column Source="ChatUserNickname" Caption="$chat.user.nickname$" Wrap="false">
            <Filter Source="ChatUserNickname" Type="Text" />
        </ug:Column>
        <ug:Column Source="IsOnline" ExternalSourceName="#yesno" Caption="$chat.online$" AllowSorting="false">
            <%-- Sort does not support subqueries yet - only column names Sort="(CASE WHEN EXISTS(SELECT * FROM Chat_OnlineUser COU WHERE COU.ChatOnlineUserChatUserID = ChatUserID AND COU.ChatOnlineUserJoinTime IS NOT NULL) THEN 1 ELSE 0 END)" >--%>
            <Filter Source="ChatUserID" Path="~/CMSModules/Chat/Controls/UI/ChatUser/ChatUserIsOnlineFilter.ascx" />
        </ug:Column>
        <ug:Column Width="100%" />
    </GridColumns>
</cms:UniGrid>
