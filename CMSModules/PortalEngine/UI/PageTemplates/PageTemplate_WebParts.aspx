<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_WebParts"
    Theme="Default" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Page Template Edit - Web Parts" CodeFile="PageTemplate_WebParts.aspx.cs" %>

<%@ Register Assembly="CMS.UIControls" Namespace="CMS.UIControls" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ObjectLockingPanel runat="server" ID="pnlObjectLocking">
        <asp:Label runat="server" ID="lblWPConfig" EnableViewState="false" /><br />
        <cms:ExtendedTextArea runat="server" ID="txtWebParts" EnableViewState="false" EditorMode="Advanced"
            Language="XML" Width="98%" Height="480px" />
    </cms:ObjectLockingPanel>
</asp:Content>
