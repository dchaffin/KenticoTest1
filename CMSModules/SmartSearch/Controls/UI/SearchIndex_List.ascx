<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_List"
    CodeFile="SearchIndex_List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Panel ID="pnlBody" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    <asp:Panel runat="server" ID="pnlPathTooLong" CssClass="DisabledInfoPanel" Visible="false">
        <cms:LocalizedLabel runat="server" ID="lblPathTooLong" EnableViewState="false" ResourceString="srch.pathtoolong"
            CssClass="ErrorLabel"></cms:LocalizedLabel>
    </asp:Panel>
    <cms:UniGrid runat="server" ID="UniGrid" GridName="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_List.xml"
        IsLiveSite="false" />
</asp:Panel>
