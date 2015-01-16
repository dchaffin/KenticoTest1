<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Components_TagCloud" CodeFile="~/CMSWebParts/Wireframe/Components/TagCloud.ascx.cs" %>
<cms:EditableWebPartList runat="server" id="ltlText" CssClass="WireframeTagCloud" PropertyName="Items" Type="TextArea" />
<cms:WebPartResizer runat="server" id="resElem" RenderEnvelope="true" />