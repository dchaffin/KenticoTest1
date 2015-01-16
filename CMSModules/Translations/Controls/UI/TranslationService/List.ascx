<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_Controls_UI_TranslationService_List"
    CodeFile="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:UniGrid runat="server" ID="gridElem" ObjectType="cms.translationservice" OrderBy="TranslationServiceDisplayName"
    Columns="TranslationServiceID,TranslationServiceIsMachine,TranslationServiceAssemblyName,TranslationServiceDisplayName,TranslationServiceEnabled,TranslationServiceName,TranslationServiceClassName"
    IsLiveSite="false" EditActionUrl="Edit.aspx?serviceId={0}">
    <GridActions Parameters="TranslationServiceID">
        <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
        <ug:Action Name="#delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$"
            ModuleName="CMS.TranslationServices" Permissions="modify" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="TranslationServiceDisplayName" Caption="$general.displayname$"
            Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="TranslationServiceIsMachine" ExternalSourceName="#yesno" Caption="$translationservice.ismachine$"
            Wrap="false" />
        <ug:Column Source="TranslationServiceEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
            Wrap="false" />
        <ug:Column Width="100%" />
    </GridColumns>
</cms:UniGrid>
