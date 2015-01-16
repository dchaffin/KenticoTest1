<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FieldAdvancedSettings.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_FieldAdvancedSettings" %>
<%@ Register TagPrefix="cms" TagName="ConditionBuilder" Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" %>
<asp:Panel ID="pnlFieldAdvancedSettings" runat="server" CssClass="FieldPanel">
    <table>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblVisibleMacro" runat="server" EnableViewState="false" ResourceString="fieldeditor.visibilitymacro"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:ConditionBuilder ShowAutoCompletionAbove="true" LeftOffset="240" TopOffset="40" MaxWidth="550" ID="visibleMacro" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblEnabledMacro" runat="server" EnableViewState="false" ResourceString="fieldeditor.enabledmacro"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:ConditionBuilder ShowAutoCompletionAbove="true" LeftOffset="240" TopOffset="40" MaxWidth="550" ID="enabledMacro" runat="server" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcDisplayInSimpleMode" runat="server">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblDisplayInSimpleMode" runat="server" EnableViewState="false" ResourceString="fieldeditor.displayinsimplemode" />
            </td>
            <td>
                <asp:CheckBox ID="chkDisplayInSimpleMode" runat="server" CssClass="CheckBoxMovedLeft" />
            </td>
        </tr>
        </asp:PlaceHolder>
         <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblHasDepending" runat="server" EnableViewState="false" ResourceString="fieldeditor.hasdepending"
                    DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox ID="chkHasDepending" runat="server" CssClass="CheckBoxMovedLeft" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblDependsOn" runat="server" EnableViewState="false" ResourceString="fieldeditor.dependson"
                    DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox ID="chkDependsOn" runat="server" CssClass="CheckBoxMovedLeft" />
            </td>
        </tr>
    </table>
</asp:Panel>
