<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Forms_FieldSet" CodeFile="~/CMSWebParts/Wireframe/Forms/FieldSet.ascx.cs" %>
<asp:Panel runat="server" ID="pnlElem" CssClass="WireframeFieldset">
    <cms:EditableWebPartProperty runat="server" id="ltlText" CssClass="WireframeFieldsetLegend" PropertyName="Title" Type="TextBox" />
</asp:Panel>
<cms:WebPartResizer runat="server" id="resElem" RenderEnvelope="true" />
