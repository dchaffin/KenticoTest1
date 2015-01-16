<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_PageElements_HeaderActions"
    CodeFile="HeaderActions.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUp" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlActions" runat="server" Visible="true" EnableViewState="false"
            CssClass="LeftAlign Actions">
        </asp:Panel>
        <asp:Panel ID="pnlAdditionalControls" runat="server" Visible="true" EnableViewState="false"
            CssClass="LeftAlign NotCheckChanges">
            <asp:PlaceHolder ID="plcAdditionalControls" runat="server" />
        </asp:Panel>
        <asp:Panel ID="pnlHelp" runat="server" CssClass="RightAlign" Visible="false" EnableViewState="false">
            <div class="EditMenuHelp">
                <cms:Help ID="helpElem" runat="server" EnableViewState="false" />
            </div>
        </asp:Panel>
        <asp:Panel runat="server" Visible="false" ID="pnlClear" CssClass="ClearBoth">
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
