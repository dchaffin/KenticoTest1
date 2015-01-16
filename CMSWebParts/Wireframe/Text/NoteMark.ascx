<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Text_NoteMark" CodeFile="~/CMSWebParts/Wireframe/Text/NoteMark.ascx.cs" %>
<div class="WireframeNoteMarkBack">
    <cms:EditableWebPartColor runat="server" CssClass="WireframeNoteMarkHeader" id="colElem" />
</div>
<cms:EditableWebPartProperty runat="server" id="ltlText" CssClass="WireframeNoteMark" PropertyName="Text" Type="TextBox" />
