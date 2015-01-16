<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_ItemsList"
    CodeFile="~/CMSModules/Reporting/Tools/ItemsList.ascx.cs" %>
<%@ Register Src="~/CMSModules/Reporting/FormControls/ReportItemSelector.ascx" TagName="BasicReportSelector"
    TagPrefix="cms" %>
<table>
    <tr>
        <td>
            <cms:BasicReportSelector runat="server" ID="brsItems" UseIDValue="true" />
        </td>
        <td>
            <cms:LocalizedButton runat="server" ID="btnInsert" EnableViewState="false" CssClass="SubmitButton"
                ResourceString="ItemList.Insert" />
            <cms:LocalizedButton runat="server" ID="btnAdd" EnableViewState="false" CssClass="ShortButton"
                ResourceString="general.new" />
            &nbsp;
            <asp:ImageButton runat="server" ID="btnPreview" EnableViewState="false" CssClass="ReportItemImageButton" />
            <asp:ImageButton runat="server" ID="btnEdit" EnableViewState="false" CssClass="ReportItemImageButton" />
            <asp:ImageButton runat="server" ID="btnDelete" EnableViewState="false" CssClass="ReportItemImageButton" />
            <asp:ImageButton runat="server" ID="btnClone" EnableViewState="false" CssClass="ReportItemImageButton" />
        </td>
    </tr>
</table>
<cms:CMSButton runat="server" ID="btnHdnDelete" CssClass="HiddenButton" OnClick="btnHdnDelete_Click"
    EnableViewState="false" />
<asp:HiddenField runat="server" ID="hdnItemId" />
