<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Trees_TreeBorder"
    CodeFile="TreeBorder.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/FrameResizer.ascx" TagName="FrameResizer"
    TagPrefix="cms" %>
<cms:frameresizer id="frmResizer" runat="server" minsize="7,*" framesetname="colsFrameset" />
<asp:Panel runat="server" ID="pnlBorder" CssClass="TreeBorder">
    &nbsp;</asp:Panel>
