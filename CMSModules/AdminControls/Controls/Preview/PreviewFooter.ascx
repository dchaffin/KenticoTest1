<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PreviewFooter.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Preview_PreviewFooter" %>
<div class="FloatRight PageFooterLine">
    <asp:Button runat="server" CssClass="SubmitButton" ID="btnSave" OnClientClick="if (typeof(actionPerformed) =='function') actionPerformed('save'); return false;" />
    <asp:Button runat="server" CssClass="LongSubmitButton" ID="btnSaveAndClose" OnClientClick="if (typeof(actionPerformed) =='function') actionPerformed('saveandclose');return false;" />
    <asp:Button runat="server" CssClass="SubmitButton" ID="btnCancel" OnClientClick="CloseDialog();return false;" />
</div>
