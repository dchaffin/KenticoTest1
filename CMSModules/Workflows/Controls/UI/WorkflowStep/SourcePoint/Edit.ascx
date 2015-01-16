<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="CMSModules_Workflows_Controls_UI_WorkflowStep_SourcePoint_Edit" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" TagName="ConditionBuilder"
    TagPrefix="cms" %>
<table>
    <asp:PlaceHolder ID="plcLabel" runat="server">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblLabel" runat="server" EnableViewState="false" ResourceString="workflowstep.sourcepoint.label"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtLabel" runat="server" CssClass="TextBoxField" MaxLength="450" />
                <cms:CMSRequiredFieldValidator ID="RequiredFieldValidatorLabel" runat="server" EnableViewState="false"
                    ControlToValidate="txtLabel:textbox" Display="Dynamic" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblText" runat="server" EnableViewState="false" ResourceString="workflowstep.sourcepoint.text"
                DisplayColon="true" />
        </td>
        <td>
            <cms:LocalizableTextBox ID="txtText" runat="server" CssClass="TextBoxField" MaxLength="450" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblTooltip" runat="server" EnableViewState="false" ResourceString="workflowstep.sourcepoint.tooltip"
                DisplayColon="true" />
        </td>
        <td>
            <cms:LocalizableTextBox ID="txtTooltip" runat="server" CssClass="TextBoxField" MaxLength="450"
                TextMode="MultiLine" />
        </td>
    </tr>
    <asp:PlaceHolder ID="plcCondition" runat="server">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblCondition" runat="server" EnableViewState="false" ResourceString="workflowstep.sourcepoint.condition"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:ConditionBuilder ID="cbCondition" runat="server" MaxWidth="720" />
            </td>
        </tr>
    </asp:PlaceHolder>
</table>
