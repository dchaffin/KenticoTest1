<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Controls_UI_Workflow_Edit"
    CodeFile="Edit.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<cms:UIForm runat="server" ID="editForm" ObjectType="cms.workflow" DefaultFieldLayout="Inline">
    <SecurityCheck Resource="CMS.Workflow" Permission="modify" />
    <LayoutTemplate>
        <cms:FormCategory runat="server" ID="pnlGeneral" DefaultFieldLayout="TwoColumns">
            <asp:PlaceHolder ID="plcType" runat="server">
                <cms:FormField runat="server" ID="wTypeLabel" Field="WorkflowType" Layout="Inline">
                    <tr>
                        <td class="FieldLabel">
                            <cms:FormLabel ID="lblType" runat="server" EnableViewState="true" ResourceString="workflow.workflowtype"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <cms:LocalizedLabel ID="lblTypeValue" runat="server" EnableViewState="true" />
                        </td>
                    </tr>
                </cms:FormField>
            </asp:PlaceHolder>
            <cms:FormField runat="server" ID="wDisplayName" Field="WorkflowDisplayName" FormControl="LocalizableTextBox"
                ResourceString="general.displayname" DisplayColon="true" UseFFI="True" />
            <cms:FormField runat="server" ID="wName" Field="WorkflowName" FormControl="CodeName"
                ResourceString="general.codename" DisplayColon="true" />
            <cms:DevelopmentMode ID="plcAllowedObjects" runat="server">
                <cms:FormField runat="server" ID="wAllowedObjects" Field="WorkflowAllowedObjects"
                    FormControl="ObjectTypeSelector" ResourceString="cms.workflow.allowedobjects"
                    DisplayColon="true" />
            </cms:DevelopmentMode>
            <asp:PlaceHolder ID="plcDocSettings" runat="server">
                <asp:PlaceHolder ID="plcVersioning" runat="server">
                    <cms:FormField runat="server" ID="wAutoPublish" Field="WorkflowAutoPublishChanges"
                        FormControl="CheckBoxControl" ResourceString="development-workflow_general.autopublish"
                        DisplayColon="true" />
                </asp:PlaceHolder>
                <cms:FormField runat="server" ID="wUseCheckInCheckOut" Field="WorkflowUseCheckinCheckout"
                    FormControl="RadioButtonsControl" Layout="Inline">
                    <tr>
                        <td class="FieldLabelTop">
                            <cms:FormLabel ID="lblUseCheckInCheckOut" runat="server" EnableViewState="false"
                                ResourceString="development-workflow_general.usecheckincheckout" DisplayColon="true" />
                        </td>
                        <td>
                            <cms:LocalizedRadioButtonList ID="rblUseCheckInCheckOut" UseResourceStrings="true"
                                RepeatDirection="Vertical" runat="server" CellPadding="0">
                                <asp:ListItem Text="development-workflow_general.sitesettings" Value="" />
                                <asp:ListItem Text="general.yes" Value="True" />
                                <asp:ListItem Text="general.no" Value="False" />
                            </cms:LocalizedRadioButtonList>
                        </td>
                    </tr>
                </cms:FormField>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcAutomation" runat="server">
                <cms:FormField runat="server" ID="wRecurrenceType" Field="WorkflowRecurrenceType"
                    Layout="Inline">
                    <tr>
                        <td class="FieldLabelTop">
                            <cms:FormLabel ID="lblRecurrenceType" runat="server" EnableViewState="false" ResourceString="cms.workflow.recurrency"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <cms:LocalizedRadioButtonList ID="rblRecurrenceType" UseResourceStrings="true" RepeatDirection="Vertical"
                                runat="server" CellPadding="0">
                                <asp:ListItem Text="cms.workflow.recurrency.recurring" Value="1" />
                                <asp:ListItem Text="cms.workflow.recurrency.notRecurring" Value="2" />
                                <asp:ListItem Text="cms.workflow.recurrency.singleton" Value="3" />
                            </cms:LocalizedRadioButtonList>
                        </td>
                    </tr>
                </cms:FormField>
            </asp:PlaceHolder>
        </cms:FormCategory>
        <cms:FormSubmit runat="server" ID="wSubmit" />
    </LayoutTemplate>
</cms:UIForm>
