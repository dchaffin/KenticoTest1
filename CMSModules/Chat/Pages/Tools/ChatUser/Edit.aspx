<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Chat user properties" Inherits="CMSModules_Chat_Pages_Tools_ChatUser_Edit" Theme="Default" CodeFile="Edit.aspx.cs" %>
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatUser/Edit.ascx"
    TagName="ChatUserEdit" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatUserEdit ID="editElem" runat="server" IsLiveSite="false" IsOnModalDialog="false" />
</asp:Content>