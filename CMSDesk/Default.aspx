<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSDesk_Default" CodeFile="Default.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Theme="Default" Title="CMS Desk" %>

<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/ScreenLock/ScreenLockDialog.ascx" TagName="ScreenLockDialog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/AdvancedPopupHandler.ascx" TagName="AdvancedPopupHandler"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="paneHeader" ShortID="h" runat="server" Direction="North" RenderAs="Div"
                ControlPath="~/CMSAdminControls/UI/Header.ascx" Resizable="False" SpacingOpen="0"
                SpacingClosed="0" />
            <cms:UILayoutPane ID="cmsdesktop" runat="server" Direction="Center" RenderAs="Iframe"
                ModuleName="CMS.Desk" ModuleAvailabilityForSiteRequired="true" />
            <cms:UILayoutPane ID="paneFooter" runat="server" ControlPath="~/CMSModules/SystemDevelopment/DevMenu.ascx" CheckPhysicalFile="true" Direction="South" RenderAs="Div" InitClosed="true" UseUpdatePanel="false" />
        </Panes>
    </cms:UILayout>
    <cms:AdvancedPopupHandler runat="server" ID="popupHandler" />
    <cms:ScreenLockDialog ID="screenLockDialogElem" runat="server" />
</asp:Content>
