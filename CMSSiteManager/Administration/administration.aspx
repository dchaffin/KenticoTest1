<%@ Page Language="C#" AutoEventWireup="true" Theme="default" Inherits="CMSSiteManager_Administration_administration"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="administration.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIGuide.ascx" TagName="UIGuide"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <uc1:UIGuide ID="uiGuide" runat="server" EnableViewState="false" ModuleName="CMS.SiteManager"
        ElementName="Administration" />
</asp:Content>
