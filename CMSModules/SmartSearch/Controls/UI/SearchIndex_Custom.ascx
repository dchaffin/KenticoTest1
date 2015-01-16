<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_Custom" CodeFile="SearchIndex_Custom.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea" TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<table>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblEnabled" runat="server" ResourceString="srch.index.assembly"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" runat="server" CssClass="TextBoxField" ID="txtAssembly" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblSite" runat="server" ResourceString="srch.index.classname"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" runat="server"  CssClass="TextBoxField"  ID="txtClassName" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblData" runat="server" ResourceString="srch.index.data"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:LargeTextArea ID="txtData" AllowMacros="false" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
        </td>
        <td>
            <cms:FormSubmitButton runat="server" ID="btnOk" CssClass="SubmitButton" ResourceString="general.ok" OnClick="btnOk_Click" EnableViewState="false" />
        </td>
    </tr>
</table>
