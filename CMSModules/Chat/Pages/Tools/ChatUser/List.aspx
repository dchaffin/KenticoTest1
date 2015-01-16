<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Chat user list"
    Inherits="CMSModules_Chat_Pages_Tools_ChatUser_List" Theme="Default" CodeFile="List.aspx.cs" %>
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatUser/List.ascx" TagName="ChatUserList" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatUserList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
