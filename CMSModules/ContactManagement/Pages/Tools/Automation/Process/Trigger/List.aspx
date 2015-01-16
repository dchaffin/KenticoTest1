<%@ Page Language="C#" AutoEventWireup="true" CodeFile="List.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Objectworkflowtrigger list" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Trigger_List"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteOrGlobalSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntSiteSelector" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true" ResourceString="General.Site" />
    <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="sg" runat="server" />
    <cms:SiteSelector ID="siteSelector" runat="server" ShortID="s" AllowGlobal="true"
        AllowAll="true" IsLiveSite="false" PostbackOnDropDownChange="true" />
</asp:Content>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <cms:CMSUpdatePanel ID="pnlActons" runat="server">
        <ContentTemplate>
            <div class="LeftAlign">
                <cms:HeaderActions ID="headerActions" runat="server" IsLiveSite="false" />
            </div>
            <cms:LocalizedLabel ID="lblWarnNew" runat="server" ResourceString="com.chooseglobalorsite"
                EnableViewState="false" Visible="false" CssClass="ActionsInfoLabel" />
            <div class="ClearBoth">
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridElem" ObjectType="cms.objectworkflowtrigger"
        OrderBy="TriggerDisplayName" Columns="TriggerID, TriggerMacroCondition, TriggerType, TriggerSiteID, TriggerDisplayName, TriggerObjectType, TriggerTargetObjectID, TriggerTargetObjectType, TriggerParameters"
        IsLiveSite="false" EditActionUrl="Edit.aspx?objectworkflowtriggerId={0}">
        <GridActions Parameters="TriggerID">
            <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
            <ug:Action Name="#delete" ExternalSourceName="delete" Caption="$General.Delete$"
                Icon="Delete.png" Confirmation="$General.ConfirmDelete$" ModuleName="CMS.OnlineMarketing" Permissions="ManageProcesses" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="TriggerDisplayName" Caption="$ma.trigger.name$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="##ALL##" Caption="$ma.trigger.type$" Wrap="false" ExternalSourceName="type"
                AllowSorting="false" />
            <ug:Column Source="TriggerMacroCondition" Caption="$ma.trigger.macrocondition$" Wrap="false"
                AllowSorting="false" ExternalSourceName="condition" />
            <ug:Column Source="TriggerSiteID" AllowSorting="false" Caption="$general.site$" Wrap="false"
                ExternalSourceName="#sitenameorglobal" Name="sitename" Localize="true" />
            <ug:Column Width="100%" /> 
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
