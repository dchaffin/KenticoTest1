<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Header.aspx.cs" Inherits="CMSModules_OnlineMarketing_Pages_Header"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Theme="default" Title="On-line marketing - Header" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/FrameResizer.ascx" TagName="FrameResizer"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIToolbar.ascx" TagName="UIToolbar"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlBody" CssClass="ContentMenu">
        <cms:frameresizer id="frmResizer" runat="server" minsize="6, *" vertical="True" />
        <asp:Panel runat="server" ID="pnlContentMenu" CssClass="ContentMenuLeft">
            <cms:uitoolbar id="uiToolbarElem" targetframeset="content" runat="server" modulename="CMS.OnlineMarketing"
                queryparametername="resourcename" rememberselecteditem="true" />
        </asp:Panel>
    </asp:Panel>
</asp:Content>
