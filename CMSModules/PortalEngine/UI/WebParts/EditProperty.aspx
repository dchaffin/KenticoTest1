<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" CodeFile="EditProperty.aspx.cs" Inherits="CMSModules_PortalEngine_UI_WebParts_EditProperty"
    Theme="Default" %>

<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="server">
    <div class="PageContent">
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
            <ContentTemplate>
                <asp:PlaceHolder runat="server" ID="plcControl" />
                <asp:Button runat="server" ID="btnLoad" CssClass="HiddenButton" OnClick="btnLoad_Click" />
                <asp:HiddenField runat="server" ID="hdnValue" EnableViewState="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <asp:Panel ID="pnlFooter" runat="server" CssClass="TextRight">
        <cms:LocalizedButton ID="btnOK" runat="server" ResourceString="general.ok" CssClass="SubmitButton"
            OnClick="btnOK_Click" />
        <cms:LocalizedButton ID="btnClose" runat="server" ResourceString="general.close"
            CssClass="SubmitButton" OnClientClick="javascript:CloseDialog(); return false;" />
    </asp:Panel>
</asp:Content>
