<%@ Page Language="C#" AutoEventWireup="true" CodeFile="List.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" 
    Inherits="CMSModules_Chat_Pages_Tools_ChatSupportCannedResponse_List" Theme="Default"  %>
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatSupportCannedResponse/List.ascx" TagName="ChatSupportCannedResponseList" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteOrGlobalSelector" TagPrefix="cms" %>

<asp:Content ID="cntSiteSelector" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <asp:Panel CssClass="Filter" runat="server" ID="pnlSearch">
        <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true"
            ResourceString="General.Site" />
        <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="c" runat="server" />
    </asp:Panel>
</asp:Content>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatSupportCannedResponseList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
