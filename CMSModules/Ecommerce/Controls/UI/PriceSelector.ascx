<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_UI_PriceSelector"
    CodeFile="PriceSelector.ascx.cs" %>
<%-- Price value --%>
<cms:CMSTextBox ID="txtPrice" runat="server" CssClass="TextBoxField" MaxLength="20" />
<%-- Currency code --%>
<asp:Label ID="lblCurrencyCode" runat="server" EnableViewState="false" />
<%-- Messages --%>
<asp:Panel ID="pnlNewLineWrapper" runat="server" Visible="false" />
<cms:MessagesPlaceHolder ID="plcMessages" runat="server" BasicStyles="true" Visible="false" />
