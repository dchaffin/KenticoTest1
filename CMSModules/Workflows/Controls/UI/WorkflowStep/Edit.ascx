<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="CMSModules_Workflows_Controls_UI_WorkflowStep_Edit" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Selectors/TimeoutSelector.ascx" TagName="TimeoutSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Workflows/FormControls/SourcePointSelector.ascx" TagName="SourcePointSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/ObjectParameters.ascx" TagName="ObjectParameters"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Workflows/Controls/UI/WorkflowStep/SourcePoint/Edit.ascx"
    TagName="SourcePointEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Panel ID="pnlContainer" CssClass="FormPanel cms_workflowstep" runat="server">
    <asp:Panel runat="server" ID="pnlGeneral" CssClass="FieldPanel General">
        <table>
            <asp:PlaceHolder ID="plcStepType" runat="server" Visible="false">
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblStepType" runat="server" CssClass="EditingFormLabel"
                            EnableViewState="false" ResourceString="graph.stepType" DisplayColon="true"
                            AssociatedControlID="lblWorkflowStepTypeValue" />
                    </td>
                    <td>
                        <cms:LocalizedLabel ID="lblWorkflowStepTypeValue" runat="server" CssClass="FieldLabel"
                            EnableViewState="false" DisplayColon="false"/>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblDisplayName" runat="server" CssClass="EditingFormLabel"
                        EnableViewState="false" ResourceString="general.displayname" DisplayColon="true"
                        AssociatedControlID="txtWorkflowStepDisplayName" />
                </td>
                <td>
                    <cms:LocalizableTextBox ID="txtWorkflowStepDisplayName" runat="server" CssClass="TextBoxField"
                        MaxLength="450" />
                    <cms:CMSRequiredFieldValidator ID="RequiredFieldValidatorDisplayName" runat="server"
                        EnableViewState="false" ControlToValidate="txtWorkflowStepDisplayName:textbox"
                        Display="dynamic" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblCodeName" runat="server" CssClass="EditingFormLabel" EnableViewState="false"
                        ResourceString="general.codename" DisplayColon="true" AssociatedControlID="txtWorkflowStepCodeName" />
                </td>
                <td>
                    <cms:CodeName ID="txtWorkflowStepCodeName" runat="server" CssClass="TextBoxField"
                        MaxLength="440" />
                    <cms:CMSRequiredFieldValidator ID="RequiredFieldValidatorCodeName" runat="server"
                        EnableViewState="false" ControlToValidate="txtWorkflowStepCodeName" Display="dynamic" />
                </td>
            </tr>
            <asp:PlaceHolder ID="plcReject" runat="server">
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblAllowReject" runat="server" EnableViewState="false" CssClass="EditingFormLabel Wrap Width120"
                            ResourceString="WorkflowStep.AllowReject" DisplayColon="true" AssociatedControlID="chkAllowReject" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkAllowReject" runat="server" Checked="true" EnableViewState="false" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcPublish" runat="server">
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblAllowPublish" runat="server" EnableViewState="false" CssClass="EditingFormLabel"
                            ResourceString="WorkflowStep.AllowPublish" DisplayColon="true" AssociatedControlID="chkAllowPublish" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkAllowPublish" runat="server" Checked="false" EnableViewState="false" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlCondition" runat="server" CssClass="FieldPanel" Visible="false">
        <cms:SourcePointEdit runat="server" ID="ucSourcePointEdit" AllowConfirmation="false" />
    </asp:Panel>
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlTimeout" CssClass="FieldPanel">
                <cms:TimeoutSelector ID="ucTimeout" runat="server" AutomaticallyDisableInactiveControl="true" IsLiveSite="false" />
                <asp:PlaceHolder ID="plcTimeoutTarget" runat="server">
                    <br />
                    <table>
                        <tr>
                            <td>
                                <cms:LocalizedLabel ID="lblTimeoutTarget" runat="server" EnableViewState="false"
                                    ResourceString="workflowstep.timeouttarget" CssClass="FieldLabel" DisplayColon="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cms:SourcePointSelector ID="ucTimeoutTarget" runat="server" />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:Panel ID="pnlParameters" runat="server" CssClass="FieldPanel">
        <cms:ObjectParameters runat="server" ID="ucActionParameters" Visible="false" />
    </asp:Panel>
</asp:Panel>
<div class="Clear" />
