<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MappingEditorItem.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_SalesForce_MappingEditorItem" %>
<%@ Import Namespace="CMS.SalesForce" %>
<tr>
    <td style="white-space:nowrap;padding-right:2em"><asp:Label ID="AttributeLabel" runat="server" EnableViewState="false"></asp:Label>:</td>
    <td>
        <p id="EmptyMessageControl" runat="server" enableviewstate="false"></p>
        <asp:DropDownList ID="SourceDropDownList" runat="server" EnableViewState="false" CssClass="SourceDropDownList"></asp:DropDownList>
    </td>
    <td style="width:100%;padding-left:1em"><asp:PlaceHolder ID="WarningPlaceHolder" runat="server" EnableViewState="false"></asp:PlaceHolder></td>
</tr>
