<%@ Page Language="C#" AutoEventWireup="true" Theme="default" Inherits="CMSSiteManager_Tools_OnlineMarketing"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="OnlineMarketing.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIGuide.ascx" TagName="UIGuide"
    TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="plcContent" runat="server">
    <uc1:UIGuide ID="uiGuide" runat="server" EnableViewState="false" ModuleName="CMS.SiteManager"
        ElementName="Tools.OnlineMarketing" />
</asp:Content>
