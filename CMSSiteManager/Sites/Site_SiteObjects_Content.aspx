<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Site_SiteObjects_Content.aspx.cs" Theme="Default"
    Inherits="CMSSiteManager_Sites_Site_SiteObjects_Content" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSSiteManager/Sites/Controls/SiteObjectsSelector.ascx" TagName="SiteObjectsSelector"
    TagPrefix="cms" %>
<asp:Content runat="server" ContentPlaceHolderID="plcContent" ID="plcContent">
    <cms:SiteObjectsSelector ID="selectorElem" runat="server" IsLiveSite="False" />
</asp:Content>
