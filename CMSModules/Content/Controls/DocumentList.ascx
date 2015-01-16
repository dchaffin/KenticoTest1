<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DocumentList.ascx.cs"
    Inherits="CMSModules_Content_Controls_DocumentList" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:cmsupdatepanel id="pnlGrid" runat="server" updatemode="Conditional">
    <ContentTemplate>
        <div class="Listing">
            <cms:UniGrid ID="gridDocuments" runat="server" ShortID="g" GridName="Listing.xml"
                EnableViewState="true" DelayedReload="true" IsLiveSite="false" ExportFileName="cms_document" />
        </div>
        <asp:Panel ID="pnlFooter" runat="server" CssClass="MassAction">
            <asp:DropDownList ID="drpWhat" runat="server" CssClass="DropDownFieldSmall" />
            <asp:DropDownList ID="drpAction" runat="server" CssClass="DropDownFieldSmall" />
            <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" CssClass="SubmitButton SelectorButton"
                EnableViewState="false" />
            <br />
            <br />
        </asp:Panel>
        <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
        <asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hdnMoveId" runat="server" />
    </ContentTemplate>
</cms:cmsupdatepanel>
