<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Controls_UI_Workflow_Emails"
    CodeFile="Emails.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/EmailTemplates/FormControls/EmailTemplateSelector.ascx"
    TagName="TemplateSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/ThreeStateCheckBox.ascx" TagName="ThreeStateCheckBox"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlSet" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlSettings" runat="server">
            <table>
                <tr>
                    <td class="FieldLabel" style="min-width: 140px;">
                        <cms:LocalizedLabel ID="lblSendEmails" runat="server" EnableViewState="false" ResourceString="development-workflow_emails.sendemails"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:ThreeStateCheckBox ID="chkEmails" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<cms:CMSUpdatePanel ID="pnlTemp" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlTemplates" runat="server">
            <table>
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblReadyApprovalSend" runat="server" EnableViewState="false"
                            ResourceString="workflow.readyapprovaltemplatelabel" DisplayColon="true" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkReadyApproval" runat="server" AutoPostBack="true" />
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
                            <asp:CheckBox ID="chkApprove" runat="server" AutoPostBack="true" />
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
                        <asp:CheckBox ID="chkReject" runat="server" AutoPostBack="true" />
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
                <asp:PlaceHolder ID="plcRest" runat="server">
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cms:LocalizedLabel ID="lblPublishSend" runat="server" EnableViewState="false" ResourceString="workflow.publishtemplatelabel"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkPublish" runat="server" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cms:LocalizedLabel ID="lblPublishTemplate" runat="server" EnableViewState="false"
                                ResourceString="workflow.customtemplate" DisplayColon="true" />
                        </td>
                        <td>
                            <cms:TemplateSelector ID="ucPublish" runat="server" IsLiveSite="false" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cms:LocalizedLabel ID="lblArchiveSend" runat="server" EnableViewState="false" ResourceString="workflow.archivetemplatelabel"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkArchive" runat="server" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cms:LocalizedLabel ID="lblArchiveTemplate" runat="server" EnableViewState="false"
                                ResourceString="workflow.customtemplate" DisplayColon="true" />
                        </td>
                        <td>
                            <cms:TemplateSelector ID="ucArchive" runat="server" IsLiveSite="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </table>
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<cms:CMSUpdatePanel ID="pnlUs" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlUsers" runat="server">
            <table style="width: 100%">
                <tr>
                    <td style="min-width: 140px;">
                        <cms:LocalizedLabel ID="lblNotif" runat="server" EnableViewState="false" ResourceString="workflow.customtemplate"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:TemplateSelector ID="ucNotif" runat="server" IsLiveSite="false" />
                    </td>
                </tr>
            </table>
            <br />
            <cms:UniSelector ID="usUsers" runat="server" IsLiveSite="false" SelectionMode="Multiple"
                ResourcePrefix="addusers" DisplayNameFormat="##USERDISPLAYFORMAT##" />
            <br />
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
