<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Chat room list"
    Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_List" Theme="Default" CodeFile="List.aspx.cs" %>
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatRoom/List.ascx" TagName="ChatRoomList" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteOrGlobalSelector" TagPrefix="cms" %>
    
<asp:Content ID="cntSiteSelector" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <asp:Panel CssClass="Filter" runat="server" ID="pnlSearch">
        <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true"
            ResourceString="General.Site" />
        <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="c" runat="server" />
    </asp:Panel>
</asp:Content>
        
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatRoomList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
