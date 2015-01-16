<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Components_Grid" CodeFile="~/CMSWebParts/Wireframe/Components/Grid.ascx.cs" %>
<cms:EditableWebPartList runat="server" id="tbElem" SubItemFormat="<td>{0}</td>" RenderAsTag="table" PropertyName="Data" Type="TextArea" CssClass="WireframeGrid" />
<cms:WebPartResizer runat="server" id="resElem" RenderEnvelope="true" />
