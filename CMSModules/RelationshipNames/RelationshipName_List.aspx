<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_RelationshipNames_RelationshipName_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Relationship names - List"
    CodeFile="RelationshipName_List.aspx.cs" %>

<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:UniGrid ID="UniGridRelationshipNames" runat="server" OrderBy="RelationshipDisplayName"
        IsLiveSite="false" ObjectType="cms.relationshipname" Columns="RelationshipNameID,RelationshipDisplayName,RelationshipName,RelationshipAllowedObjects">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
            <ug:Action Name="delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="RelationshipDisplayName" Caption="$general.displayname$" Wrap="false"
                Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="RelationshipName" Caption="$general.codename$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="RelationshipAllowedObjects" ExternalSourceName="type" Caption="$RelationshipName.Columns.RelationshipType$"
                Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="RelationshipNameID" Caption="$general.sitename$" Visible="false">
                <Filter Type="site" />
            </ug:Column>
            <ug:Column Width="100%" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
