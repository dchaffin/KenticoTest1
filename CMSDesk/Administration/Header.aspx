<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSDesk_Administration_Header"
    Theme="Default" CodeFile="Header.aspx.cs" Title="Administration / Header" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/FrameResizer.ascx" TagName="FrameResizer"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIToolbar.ascx" TagName="UIToolbar"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlBody" CssClass="ContentMenu">
        <cms:frameresizer id="frmResizer" runat="server" minsize="6, *" vertical="True" />
        <asp:Panel runat="server" ID="pnlContentMenu" CssClass="ContentMenuLeft">
            <cms:uitoolbar id="uiToolbarElem" targetframeset="cmsdeskadmincontent" runat="server"
                modulename="CMS.Administration" queryparametername="resourcename" rememberselecteditem="true" />
        </asp:Panel>
    </asp:Panel>
</asp:Content>
