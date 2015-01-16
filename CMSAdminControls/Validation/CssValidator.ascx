<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Validation_CssValidator"
    CodeFile="CssValidator.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlLog" Visible="false">
    <cms:AsyncBackground ID="backgroundElem" runat="server" />
    <div class="AsyncLogArea">
        <div>
            <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader">
                    <cms:PageTitle ID="titleElemAsync" runat="server" SetWindowTitle="false" />
                </asp:Panel>
                <asp:Panel ID="pnlCancel" runat="server" CssClass="PageHeaderLine">
                    <cms:CMSButton runat="server" ID="btnCancel" CssClass="SubmitButton" />
                </asp:Panel>
                <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                    <cms:AsyncControl ID="ctlAsync" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
</asp:Panel>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<asp:Panel ID="pnlGrid" runat="server" CssClass="Validation PageContent">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
    <asp:Panel ID="pnlStatus" runat="server" CssClass="Status" Visible="false">
        <asp:Image runat="server" ID="imgStatus" AlternateText="Status image" EnableViewState="false" /><cms:RTLfix
            IsLiveSite="false" runat="server" ID="rtlFix1" />
        <cms:LocalizedLabel ID="lblStatus" runat="server" EnableViewState="false" />
        <cms:RTLfix IsLiveSite="false" runat="server" ID="rtlFix" />
    </asp:Panel>
    <cms:LocalizedLabel ID="lblResults" runat="server" EnableViewState="false" CssClass="Results"
        Visible="false" DisplayColon="true" />
    <cms:UniGrid ID="gridValidationResult" runat="server" ExportFileName="CSS_validation_results"
        ShowActionsLabel="false">
        <GridActions>
            <ug:Action Name="view" ExternalSourceName="viewcss" Icon="view.png" Caption="$validation.viewcode$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="line" Caption="$validation.css.line$" Wrap="false" />
            <ug:Column Source="context" Caption="$validation.css.context$" AllowSorting="false" />
            <ug:Column Source="message" Caption="$validation.css.message$" AllowSorting="false"
                Width="60%" />
            <ug:Column Source="source" ExternalSourceName="source" Caption="$validation.css.source$"
                Width="30%" />
        </GridColumns>
        <PagerConfig DefaultPageSize="10" />
    </cms:UniGrid>
</asp:Panel>
<asp:HiddenField ID="hdnHTML" runat="server" />
