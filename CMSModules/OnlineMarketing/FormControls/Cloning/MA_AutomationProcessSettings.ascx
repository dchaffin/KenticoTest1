<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MA_AutomationProcessSettings.ascx.cs"
    Inherits="CMSModules_OnlineMarketing_FormControls_Cloning_MA_AutomationProcessSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblWorkflowTrigger" ResourceString="clonning.settings.workflow.workflowtrigger"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkWorkflowTrigger" Checked="true" />
        </td>
    </tr>
</table>
