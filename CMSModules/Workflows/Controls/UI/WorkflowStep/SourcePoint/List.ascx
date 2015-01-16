<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="CMSModules_Workflows_Controls_UI_WorkflowStep_SourcePoint_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:MessagesPlaceholder ID="plcMess" runat="server" />
<cms:UniGrid ID="gridElem" runat="server" Columns="">
    <GridActions Parameters="GUID">
        <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
        <ug:Action Name="delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$"
            ModuleName="CMS.WorkflowEngine" Permissions="modify" ExternalSourceName="allowaction" />
        <ug:ActionSeparator />
        <ug:Action Name="moveup" ExternalSourceName="allowaction" Caption="$General.MoveUp$"
            Icon="Up.png" />
        <ug:Action Name="movedown" ExternalSourceName="allowaction" Caption="$General.MoveDown$"
            Icon="Down.png" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="Label" Caption="$workflowstep.sourcepoint.label$" ExternalSourceName="label"
            AllowSorting="false" Width="50%" />
        <ug:Column Source="Condition" Caption="$workflowstep.sourcepoint.condition$" ExternalSourceName="condition"
            AllowSorting="false" Width="50%" />
    </GridColumns>
</cms:UniGrid>
