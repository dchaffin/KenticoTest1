<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="CMSModules_Chat_Controls_UI_ChatRoomUser_Edit" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="UserSelector" TagPrefix="cms" %>

<cms:UIForm runat="server" ID="EditForm" ObjectType="chat.roomuser">
    <LayoutTemplate>
        <cms:FormField runat="server" ID="fUserID" Field="ChatRoomUserChatUserID" ResourceString="general.user" DisplayColon="true">
            <cms:UserSelector runat="server" ID="fUserSelector" IsLiveSite="false" AllowAll="false" AllowDefault="false" AllowEmpty="false" />
            <asp:Literal runat="server" ID="litChatUserLink"></asp:Literal>
        </cms:FormField>
        <cms:FormField runat="server" ID="fAdminLevel" Field="ChatRoomUserAdminLevel" ResourceString="chat.adminlevel" DisplayColon="true">
            <cms:ExtendedDropDownList ID="fdrpAdminLevel" runat="server" CssClass="DropDownField"></cms:ExtendedDropDownList>
        </cms:FormField>
        <cms:FormSubmit runat="server" ID="fSubmit" />
    </LayoutTemplate>
</cms:UIForm>