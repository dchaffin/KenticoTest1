<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_Controls_UI_TranslationSubmission_List"
    CodeFile="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:UniGrid runat="server" ID="gridElem" ObjectType="cms.translationsubmission"
    OrderBy="SubmissionDate DESC" Columns="SubmissionID,SubmissionServiceID,SubmissionStatus,SubmissionPrice,SubmissionStatusMessage,SubmissionDate,SubmissionSourceCulture,SubmissionTargetCulture,SubmissionName,SubmissionWordCount,SubmissionCharCount,SubmissionItemCount"
    IsLiveSite="false" EditActionUrl="Edit.aspx?submissionId={0}" ShowActionsMenu="false" ShowObjectMenu="false">
    <GridActions Parameters="SubmissionID">
        <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
        <ug:Action Name="delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$"
            ModuleName="CMS.TranslationServices" Permissions="modify" />
        <ug:ActionSeparator Text="<span class='UniGridActionSeparator'>&nbsp;</span>" />
        <ug:Action Name="downloadzip" Caption="$translationservice.downloadxliffinzip$" Icon="DownloadZip.png"
            OnClick="window.noProgress = true; if (window.HideIndicator) {HideIndicator(false);}" />
        <ug:ActionSeparator Text="<span class='UniGridActionSeparator'>&nbsp;</span>" />
        <ug:Action Name="process" ExternalSourceName="ProcessAction" Caption="$translationservice.importtranslationstooltip$"
            Icon="Synchronize.png" Confirmation="$translationservice.confirmprocesstranslations$" />
        <ug:Action Name="resubmit" ExternalSourceName="ResubmitAction" Caption="$translationservice.resubmittooltip$"
            Icon="Rebuild.png" />
        <ug:Action Name="cancel" ExternalSourceName="CancelAction" Caption="$translationservice.cancelsubmissiontooltip$"
            Icon="Undo.png" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="SubmissionName" Caption="$translationservice.sourcedocuments$"
            Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="SubmissionSourceCulture" Caption="$translationservice.sourceculture$"
            ExternalSourceName="#culturenamewithflag" Wrap="false">
        </ug:Column>
        <ug:Column Source="SubmissionTargetCulture" Caption="$translationservice.targetculture$"
            ExternalSourceName="#culturenamewithflag" Wrap="false">
        </ug:Column>
        <ug:Column Source="SubmissionDate" Caption="$translationservice.dateofsubmission$"
            Wrap="false" />
        <ug:Column Source="SubmissionStatus" ExternalSourceName="SubmissionStatus" Caption="$translationservice.status$"
            Wrap="false" />
        <ug:Column Source="SubmissionWordCount" Caption="$translationservice.wordcount$"
            Wrap="false">
            <Filter Type="integer" />
        </ug:Column>
        <ug:Column Source="SubmissionCharCount" Caption="$translationservice.charcount$"
            Wrap="false">
            <Filter Type="integer" />
        </ug:Column>
        <ug:Column Source="SubmissionPrice" ExternalSourceName="SubmissionPrice" Caption="$translationservice.submissionprice$"
            Wrap="false" />
        <ug:Column Source="SubmissionServiceID" Caption="$translationservice.translationservice$"
            ExternalSourceName="#transform: cms.translationservice.translationservicedisplayname"
            Wrap="false" />
        <ug:Column Width="100%" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>
