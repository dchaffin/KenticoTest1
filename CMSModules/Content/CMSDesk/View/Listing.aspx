<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_View_Listing"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Content - Listing"
    CodeFile="Listing.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/DocumentList.ascx" TagName="DocumentList"
    TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSUpdatePanel ID="upList" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:DocumentList runat="server" ID="docList" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
