<%@ Control Language="C#" AutoEventWireup="True" Inherits="CMSModules_Workflows_Controls_UI_WorkflowAction_Edit"
    CodeFile="Edit.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register TagPrefix="cms" TagName="File" Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" %>
<%@ Register Src="~/CMSFormControls/SelectModule.ascx" TagName="ModuleSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/ObjectTypeSelector.ascx" TagName="ObjectTypeSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<cms:UIForm runat="server" ID="formElem" ObjectType="cms.workflowaction" DefaultFieldLayout="Inline"
    RedirectUrlAfterCreate="Frameset.aspx?actionid={%EditedObject.ID%}&saved=1">
    <SecurityCheck Resource="CMS.Workflow" Permission="modify" />
    <LayoutTemplate>
        <cms:FormCategory runat="server" ID="pnlGeneral" ResourceString="general.general"
            DefaultFieldLayout="TwoColumns">
            <cms:FormField runat="server" ID="aDisplayName" Field="ActionDisplayName" FormControl="LocalizableTextBox"
                ResourceString="general.displayname" DisplayColon="true" />
            <cms:FormField runat="server" ID="aName" Field="ActionName" FormControl="CodeName"
                ResourceString="general.codename" DisplayColon="true" />
            <cms:FormField runat="server" ID="aDescription" Field="ActionDescription">
                <tr>
                    <td class="FieldLabel">
                        <cms:FormLabel ID="lblDesc" runat="server" EnableViewState="true" ResourceString="general.description"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:LocalizableTextBox ID="txtDesc" runat="server" CssClass="TextAreaField" TextMode="MultiLine" />
                    </td>
                </tr>
            </cms:FormField>
        </cms:FormCategory>
        <cms:FormCategory runat="server" ID="pnlImages" ResourceString="workflowaction.images"
            DefaultFieldLayout="TwoColumns">
            <cms:FormField runat="server" ID="aIconGUID" Field="ActionIconGUID" Layout="Inline">
                <tr>
                    <td class="FieldLabel">
                        <cms:FormLabel ID="lblIcon" runat="server" EnableViewState="true" ResourceString="general.icon"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:File ID="ucIcon" runat="server" Enabled="true" />
                    </td>
                </tr>
            </cms:FormField>
            <cms:FormField runat="server" ID="aThumbnailGUID" Field="ActionThumbnailGUID" Layout="Inline">
                <tr>
                    <td class="FieldLabel">
                        <cms:FormLabel ID="lblMetaFile" runat="server" EnableViewState="true" ResourceString="general.thumbnail"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:File ID="ucFile" runat="server" Enabled="true" />
                    </td>
                </tr>
            </cms:FormField>
        </cms:FormCategory>
        <cms:FormCategory runat="server" ID="pnlAction" ResourceString="workflowaction.actionconfig"
            DefaultFieldLayout="TwoColumns">
            <cms:FormField runat="server" ID="aAssemblyName" Field="ActionAssemblyName" FormControl="TextBoxControl"
                ResourceString="general.assemblyname" DisplayColon="true" />
            <cms:FormField runat="server" ID="aClass" Field="ActionClass" FormControl="TextBoxControl"
                ResourceString="general.classdisplayname" DisplayColon="true" />
            <cms:FormField runat="server" ID="aAlowedObjects" Field="ActionAllowedObjects" Layout="Inline"
                DevelopmentModeOnly="true">
                <tr>
                    <td>
                        <cms:FormLabel ID="lblAllowedObjects" runat="server" ResourceString="workflowaction.allowedobjects"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:ObjectTypeSelector ID="objectTypeSelector" runat="server" AllowAll="true" EnableViewState="true" />
                    </td>
                </tr>
            </cms:FormField>
            <cms:FormField runat="server" ID="aResourceID" Field="ActionResourceID" DevelopmentModeOnly="true">
                <tr>
                    <td class="FieldLabel">
                        <cms:FormLabel ID="lblResourceID" runat="server" EnableViewState="true" ResourceString="general.module"
                            DisplayColon="false" />
                    </td>
                    <td>
                        <cms:ModuleSelector runat="server" ID="moduleSelector" IsLiveSite="false" DisplayAllModules="true"
                            DisplayNone="true" />
                    </td>
                </tr>
            </cms:FormField>
            <cms:FormField runat="server" ID="aEnabled" Field="ActionEnabled" FormControl="CheckBoxControl"
                ResourceString="general.Enabled" DisplayColon="true" />
        </cms:FormCategory>
        <cms:FormSubmit runat="server" ID="aSubmit" />
    </LayoutTemplate>
</cms:UIForm>
