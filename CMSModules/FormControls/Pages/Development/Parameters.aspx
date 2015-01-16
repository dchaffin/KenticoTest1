<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_FormControls_Pages_Development_Parameters"
    Theme="Default" Title="Form User Control Parameters" CodeFile="Parameters.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldEditor.ascx"
    TagName="FieldEditor" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/PortalEngine/UI/WebParts/Development/DefaultValueEditor.ascx"
    TagName="DefaultValueEditor" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:FieldEditor ID="fieldEditor" runat="server" IsLiveSite="false" />
    <asp:Panel ID="pnlDefaultEditor" runat="server" CssClass="FieldPanelError" Visible="false">
        <cms:DefaultValueEditor ID="defaultValueEditor" ShortID="d" runat="server" Visible="false" />
    </asp:Panel>
</asp:Content>
