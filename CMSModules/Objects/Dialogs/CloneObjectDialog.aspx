<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CloneObjectDialog.aspx.cs"
    Inherits="CMSModules_Objects_Dialogs_CloneObjectDialog" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Clone Object" Theme="Default" %>

<%@ Register Src="~/CMSModules/Objects/Controls/CloneObject.ascx" TagPrefix="cms"
    TagName="CloneObject" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
        <asp:PlaceHolder runat="server" ID="plcForm">
            <asp:Label runat="server" ID="lblInfo" EnableViewState="false" /><br />
            <br />
            <cms:CloneObject ID="cloneObjectElem" runat="server" />
        </asp:PlaceHolder>
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:CMSButton ID="btnClone" runat="server" CssClass="SubmitButton" />
        <cms:CMSButton ID="btnClose" runat="server" CssClass="SubmitButton" OnClientClick="CloseDialog(); return false;" />
    </div>
</asp:Content>
