<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Controls_UI_WorkflowStep_Emails"
    CodeFile="Emails.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/EmailTemplates/FormControls/EmailTemplateSelector.ascx"
    TagName="TemplateSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/ThreeStateCheckBox.ascx" TagName="ThreeStateCheckBox"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUp" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlSettings" runat="server">
            <table>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblSendEmails" runat="server" EnableViewState="false" ResourceString="development-workflow_emails.sendemails"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:ThreeStateCheckBox ID="chkEmails" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlTemplates" runat="server">
            <table>
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblReadyApprovalSend" runat="server" EnableViewState="false"
                            ResourceString="workflow.readyapprovaltemplatelabel" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:ThreeStateCheckBox ID="chkReadyApproval" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblReadyApprovalTemplate" runat="server" EnableViewState="false"
                            ResourceString="workflow.customtemplate" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:TemplateSelector ID="ucReadyApproval" runat="server" IsLiveSite="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <asp:PlaceHolder ID="plcApprove" runat="server">
                    <tr>
                        <td>
                            <cms:LocalizedLabel ID="lblApproveSend" runat="server" EnableViewState="false" ResourceString="workflow.approvedtemplatelabel"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <cms:ThreeStateCheckBox ID="chkApprove" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cms:LocalizedLabel ID="lblApproveTemplate" runat="server" EnableViewState="false"
                                ResourceString="workflow.customtemplate" DisplayColon="true" />
                        </td>
                        <td>
                            <cms:TemplateSelector ID="ucApprove" runat="server" IsLiveSite="false" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblRejectSend" runat="server" EnableViewState="false" ResourceString="workflow.rejecttemplatelabel"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:ThreeStateCheckBox ID="chkReject" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblRejectTemplate" runat="server" EnableViewState="false"
                            ResourceString="workflow.customtemplate" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:TemplateSelector ID="ucReject" runat="server" IsLiveSite="false" />
                    </td>
                </tr>
            </table>
            <br />
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
