<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TriggerScoreProperties.ascx.cs"
    Inherits="CMSModules_ContactManagement_Controls_UI_Automation_TriggerScoreProperties" %>
<%@ Register Src="~/CMSFormControls/Basic/TextBoxControl.ascx" TagName="TextBoxControl"
    TagPrefix="cms" %>
<tr>
    <td class="FieldLabel">
        <cms:FormLabel ID="lblScore" runat="server" EnableViewState="false" ResourceString="om.score.scorevalue"
            DisplayColon="true" />
    </td>
    <td>
        <cms:TextBoxControl ID="txtScore" FilterType="Numbers, Custom" runat="server" ValidChars="-" />
    </td>
</tr>
