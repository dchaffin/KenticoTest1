<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MyDesk_mainmenu"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" CodeFile="mainmenu.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/FrameResizer.ascx" TagName="FrameResizer"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIToolbar.ascx" TagName="UIToolbar"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlBody" CssClass="ContentMenu">
        <cms:frameresizer id="frmResizer" runat="server" minsize="6, *" vertical="True" />
        <asp:Panel runat="server" ID="pnlContentMenu" CssClass="ContentMenuLeft">
            <cms:uitoolbar id="ucUIToolbar" targetframeset="frameMain" runat="server" modulename="CMS.MyDesk"
                rememberselecteditem="true" highlightfirstitem="true" queryparametername="resourcename" />
        </asp:Panel>
    </asp:Panel>
    <asp:Literal runat="server" ID="litScript" EnableViewState="false"></asp:Literal>
</asp:Content>
