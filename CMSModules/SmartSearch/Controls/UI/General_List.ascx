<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_UI_General_List" CodeFile="General_List.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/FormControls/ClassNameSelector.ascx" TagName="ClassNameSelector" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<table>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblVisible" runat="server" ResourceString="srch.index.objectname"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:ClassNameSelector runat="server" ID="selObjects" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblWhere" runat="server" ResourceString="srch.index.where"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:LargeTextArea ID="txtWhere" AllowMacros="false" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
        </td>
        <td>
            <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOk_Click" EnableViewState="false" />
        </td>
    </tr>
</table>