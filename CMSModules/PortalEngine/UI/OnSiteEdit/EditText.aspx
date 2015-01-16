<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditText.aspx.cs" Inherits="CMSModules_PortalEngine_UI_OnSiteEdit_EditText"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Edit text" %>

<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/Editable/EditableText.ascx"
    TagName="EditableText" TagPrefix="cms" %>
<asp:Content ID="cntMenu" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" ShortID="m" runat="server" ShowProperties="false" ShowSpellCheck="true"
        IsLiveSite="false" />
    <div id="CKToolbar">
    </div>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel ID="pnlEditor" runat="server" CssClass="EditableTextContainer" >
        <cms:EditableText ID="ucEditableText" runat="server" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <asp:Panel ID="pnlFooter" runat="server" CssClass="TextRight">
        <cms:LocalizedButton ID="btnClose" runat="server" ResourceString="general.close" CssClass="SubmitButton" OnClientClick="javascript:CloseDialog(); return false;" />
    </asp:Panel>
</asp:Content>