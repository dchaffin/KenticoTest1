<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_UniMenu_UniMenu"
    CodeFile="UniMenu.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.ExtendedControls" %>

<cms:ScrollPanel ID="pnlScrollControls" ShortID="sp" runat="server" CssClass="Horizontal" ScrollAreaDefaultSize="4000"
    BackwardScrollerControlID="pnlLeftScroller" ForwardScrollerControlID="pnlRightScroller" ScrollStep="200" >
</cms:ScrollPanel>
<asp:Panel ID="pnlLeftScroller" runat="server" CssClass="ContentMenuSlider LeftSlider" ></asp:Panel>
<asp:Panel ID="pnlRightScroller" runat="server" CssClass="ContentMenuSlider RightSlider" ></asp:Panel>
<cms:CMSPanel runat="server" ID="pnlControls" ShortID="pc" CssClass="Horizontal">
</cms:CMSPanel>
