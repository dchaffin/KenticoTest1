<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Forms_DateTime" CodeFile="~/CMSWebParts/Wireframe/Forms/DateTime.ascx.cs" %>
<asp:Panel ID="pnlElem" runat="server" CssClass="WireframeDateTime">
    <cms:EditableWebPartProperty runat="server" id="ltlText" CssClass="WireframeTextBox" PropertyName="Text" Type="TextBox" />
</asp:Panel>
<cms:WebPartResizer runat="server" id="resElem" HorizontalOnly="true" RenderEnvelope="true" />