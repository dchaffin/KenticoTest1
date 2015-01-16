<%@ Page Title="Device profiles - Theme" Language="C#" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    AutoEventWireup="true" Theme="Default" CodeFile="Theme.aspx.cs" Inherits="CMSModules_DeviceProfile_Pages_Development_Theme"
    EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/CSS/ThemeEditor.ascx" TagName="ThemeEditor"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ThemeEditor ID="themeElem" runat="server" />
</asp:Content>
