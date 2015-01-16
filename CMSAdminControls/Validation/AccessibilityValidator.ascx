<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccessibilityValidator.ascx.cs"
    Inherits="CMSAdminControls_Validation_AccessibilityValidator" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Panel ID="pnlGrid" runat="server" CssClass="Validation PageContent">
    <cms:CMSUpdateProgress ID="up" runat="server" HandlePostback="true" EnableViewState="false" />
    <cms:MessagesPlaceholder ID="plcMess" runat="server" IsLiveSite="false" />
    <asp:Panel ID="pnlStatus" runat="server" CssClass="Status" Visible="false">
        <asp:Image runat="server" ID="imgStatus" AlternateText="Status image" EnableViewState="false" />
        <cms:LocalizedLabel ID="lblStatus" runat="server" EnableViewState="false" /><cms:RTLfix
            IsLiveSite="false" runat="server" ID="rtlFix1" />
    </asp:Panel>
    <cms:LocalizedLabel ID="lblResults" runat="server" EnableViewState="false" class="Results"
        Visible="false" DisplayColon="true" /><cms:RTLfix IsLiveSite="false" runat="server"
            ID="rtlFix" />
    <cms:UniGrid ID="gridValidationResult" runat="server" ExportFileName="Accessibility_validation_results" ShowActionsLabel="false">
        <GridActions>
            <ug:Action Name="view" ExternalSourceName="view" Icon="view.png" Caption="$validation.viewcode$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="line" Caption="$validation.accessibility.line$" Wrap="false" />
            <ug:Column Source="column" Caption="$validation.accessibility.column$" Wrap="false" AllowSorting="false" />
            <ug:Column Source="accessibilityrule" Caption="$validation.accessibility.accessibilityrule$" ExternalSourceName="accessibilityrule" AllowSorting="false"
                Width="25%" />
            <ug:Column Source="error" ExternalSourceName="error" Caption="$validation.accessibility.error$" AllowSorting="false"
                Width="25%" />
            <ug:Column Source="fixsuggestion" ExternalSourceName="fixsuggestion" Caption="$validation.accessibility.fixsuggestion$"
                Width="25%" AllowSorting="false" />
            <ug:Column Source="source" ExternalSourceName="source" Caption="$validation.accessibility.source$"
                Wrap="true" AllowSorting="false" />
        </GridColumns>
        <PagerConfig DefaultPageSize="10" />
    </cms:UniGrid>
</asp:Panel>
<asp:HiddenField ID="hdnHTML" runat="server" EnableViewState="false" />
