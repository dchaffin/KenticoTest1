<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Macros_Controls_UI_MacroRule_List"
    CodeFile="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:UniGrid runat="server" ID="gridElem" ObjectType="cms.macrorule" OrderBy="MacroRuleResourceName DESC, MacroRuleDisplayName"
    Columns="MacroRuleID,MacroRuleDisplayName,MacroRuleCondition,MacroRuleResourceName"
    IsLiveSite="false" EditActionUrl="Frameset.aspx?ruleId={0}&module={?module?}&displaytitle={?displaytitle?}" RememberStateByParam="">
    <GridActions Parameters="MacroRuleID">
        <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
        <ug:Action Name="#delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="MacroRuleDisplayName" Caption="$general.displayname$" Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="MacroRuleCondition" Caption="$macros.macrorule.condition$" Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="MacroRuleResourceName" ExternalSourceName="category" Caption="$macros.macrorule.resourcename$"
            Wrap="false">
            <Filter Path="~/CMSModules/Macros/Controls/UI/MacroRule/CategoryFilter.ascx" />
        </ug:Column>
        <ug:Column Width="100%" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>
