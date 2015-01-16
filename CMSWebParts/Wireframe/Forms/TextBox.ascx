<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Forms_TextBox" CodeFile="~/CMSWebParts/Wireframe/Forms/TextBox.ascx.cs" %>
<cms:EditableWebPartProperty runat="server" id="ltlText" CssClass="WireframeTextBox" PropertyName="Text" Type="TextBox" />
<div class="WireframePositionEnvelope">
    <cms:EditableWebPartImage runat="server" ID="imgElem" CssClass="WireframeTextBoxIcon" PropertyName="ImageUrl" Visible="false" />
</div>
<cms:WebPartResizer runat="server" id="resElem" HorizontalOnly="true" RenderEnvelope="true" />