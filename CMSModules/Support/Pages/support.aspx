<%@ Page Language="C#" AutoEventWireup="true" Theme="default" Inherits="CMSModules_Support_Pages_support"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Support description"
    CodeFile="Support.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIGuide.ascx" TagName="UIGuide"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <uc1:UIGuide ID="uiGuide" runat="server" EnableViewState="false" ModuleName="CMS.SiteManager"
        ElementName="Support"/>
</asp:Content>
