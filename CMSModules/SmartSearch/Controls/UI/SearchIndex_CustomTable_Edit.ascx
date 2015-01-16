<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_CustomTable_Edit"
    CodeFile="SearchIndex_CustomTable_Edit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/CustomTableSelector.ascx" TagName="CustomTableSelector"
    TagPrefix="uc1" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Panel ID="pnlConetnEdit" runat="server">
    <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <table>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblCustomTable" EnableViewState="false" DisplayColon="true"
                    ResourceString="srch.index.customtable"></cms:LocalizedLabel>
            </td>
            <td>
                <uc1:CustomTableSelector ID="customTableSelector" IsLiveSite="false" AllSites="true"
                    runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblWhere" EnableViewState="false" DisplayColon="true"
                    ResourceString="srch.index.where"></cms:LocalizedLabel>
            </td>
            <td>
                <cms:LargeTextArea ID="txtWhere" AllowMacros="false" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
            </td>
            <td>
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
            </td>
        </tr>
    </table>
</asp:Panel>
