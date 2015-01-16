<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewCode.aspx.cs" Inherits="CMSModules_Content_CMSDesk_Validation_ViewCode"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Theme="Default" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server" ID="cntContent">
    <asp:Label runat="server" ID="lblError" CssClass="AdvancedLabel Error ValidatorError" EnableViewState="false" Visible="false" />
    <asp:Panel runat="server" ID="pnlCode">
        <cms:ExtendedTextArea runat="server" ID="txtCodeText" EnableViewState="false" EditorMode="Advanced"
            Language="HTML" Height="521" ShowLineNumbers="true" Width="100%" ReadOnly="true"
            Wrap="true" />
    </asp:Panel>
    <asp:HiddenField ID="hdnHTML" runat="server" EnableViewState="false" />
</asp:Content>
