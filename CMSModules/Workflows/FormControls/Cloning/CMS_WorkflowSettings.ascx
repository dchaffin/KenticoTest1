<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_WorkflowSettings.ascx.cs"
    Inherits="CMSModules_Workflows_FormControls_Cloning_CMS_WorkflowSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblWorkflowScope" ResourceString="clonning.settings.workflow.workflowscope"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkWorkflowScope" Checked="true" />
        </td>
    </tr>
</table>
