<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ControlSettings.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_ControlSettings" %>
<asp:Panel ID="pnlSettings" runat="server" CssClass="FieldPanel">
    <cms:BasicForm ID="form" runat="server" />
    <asp:PlaceHolder runat="server" ID="plcSwitch" Visible="false">
        <cms:LocalizedLinkButton runat="server" ID="lnkAdvanced" ResourceString="fieldeditor.tabs.advanced" CssClass="LinkSortDown FloatRight FieldTableMenu" />
        <cms:LocalizedLinkButton runat="server" ID="lnkSimple" ResourceString="fieldeditor.tabs.simplified" CssClass="LinkSortUp FloatRight FieldTableMenu" />
    </asp:PlaceHolder>
</asp:Panel>
