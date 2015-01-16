<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Friends_Controls_FriendsSearchBox"
    CodeFile="FriendsSearchBox.ascx.cs" %>
<asp:Panel ID="pnlFilter" runat="server" EnableViewState="false" DefaultButton="btnSearch">
    <cms:CMSTextBox ID="txtSearch" runat="server" EnableViewState="false" CssClass="TextBoxField" />
    <cms:LocalizedButton ID="btnSearch" runat="server" ResourceString="general.search"
        EnableViewState="false" CssClass="ContentButton" OnClick="btnShow_Click" />
    <asp:LinkButton ID="btnReset" runat="server" Style="line-height: 2em; margin: 1em"
        EnableViewState="false" />
    <br />
    <br />
</asp:Panel>
