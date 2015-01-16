<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Edit.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Objectworkflowtrigger properties" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Trigger_Edit"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/TriggerScoreProperties.ascx"
    TagName="ScoreProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Scoring/FormControls/SelectScore.ascx" TagName="ScoreTypeSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ActivityTypeSelector.ascx"
    TagName="ActivityTypeSelector" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UIForm runat="server" ID="editForm" ObjectType="cms.objectworkflowtrigger" RedirectUrlAfterCreate="Edit.aspx?objectworkflowtriggerid={%EditedObject.ID%}&saved=1"
                DefaultFieldLayout="Inline">
                <SecurityCheck Resource="CMS.OnlineMarketing" Permission="ManageProcesses" />
                <LayoutTemplate>
                    <cms:FormCategory runat="server" ID="pnlGeneral" ResourceString="general.general" DefaultFieldLayout="TwoColumns">
                        <cms:FormField runat="server" ID="tDisplayName" Field="TriggerDisplayName" FormControl="LocalizableTextBox"
                            ResourceString="general.displayname" DisplayColon="true" />
                        <cms:FormField runat="server" ID="tType" Field="TriggerType" FormControl="DropDownListControl"
                            Layout="Inline">
                            <tr>
                                <td class="FieldLabelTop">
                                    <cms:FormLabel ID="lblTriggerType" runat="server" EnableViewState="false"
                                        ResourceString="general.type" DisplayColon="true" />
                                </td>
                                <td>
                                    <cms:LocalizedDropDownList ID="ddlType" UseResourceStrings="true" CssClass="DropDownField"
                                        runat="server" AutoPostBack="true" />
                                </td>
                            </tr>
                        </cms:FormField>
                        <asp:panel runat="server" ID="pnlActivityProperties" ResourceString="ma.trigger.activityGroup" DefaultFieldLayout="TwoColumns">
                            <tr>
                                <td class="FieldLabel">
                                    <cms:FormLabel ID="lblActivityType" runat="server" EnableViewState="false" ResourceString="om.activity.type"
                                        DisplayColon="true" />
                                </td>
                                <td>
                                    <cms:ActivityTypeSelector runat="server" ID="ucActivityType" />
                                </td>
                            </tr>
                        </asp:panel>
                        <cms:FormField runat="server" ID="tMacroCondition" Field="TriggerMacroCondition" UseFFI="true"
                            FormControl="ConditionBuilder" ResourceString="ma.trigger.macrocondition" DisplayColon="true" />
                    </cms:FormCategory>
                    <cms:FormCategory runat="server" ID="pnlScoreProperties" ResourceString="ma.trigger.scoreGroup" DefaultFieldLayout="TwoColumns">
                        <tr>
                            <td class="FieldLabel">
                                <cms:FormLabel ID="lblScoreType" runat="server" EnableViewState="false" ResourceString="scoreselect.itemname"
                                    DisplayColon="true" />
                            </td>
                            <td>
                                <cms:ScoreTypeSelector runat="server" ID="ucScoreType" />
                            </td>
                        </tr>
                        <cms:ScoreProperties ID="ucScoreProperties" runat="server" />
                    </cms:FormCategory>
                    <cms:FormSubmit runat="server" ID="wSubmit" />
                </LayoutTemplate>
            </cms:UIForm>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
