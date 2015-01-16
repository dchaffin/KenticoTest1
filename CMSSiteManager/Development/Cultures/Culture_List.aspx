<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSSiteManager_Development_Cultures_Culture_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Cultures - Culture List"
    CodeFile="Culture_List.aspx.cs" %>

<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid ID="UniGridCultures" runat="server" OrderBy="CultureName" IsLiveSite="false"
        ObjectType="cms.culture" Columns="CultureID, CultureName, CultureCode, CultureShortName, CultureAlias">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
            <ug:Action Name="delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="##ALL##" ExternalSourceName="culturename" Caption="$Unigrid.CultureList.Columns.CultureName$"
                Wrap="false" Localize="true" Sort="CultureName">
                <Filter Type="text" Source="CultureName" />
            </ug:Column>
            <ug:Column Source="CultureCode" Caption="$Unigrid.CultureList.Columns.CultureCode$"
                Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="CultureShortName" Caption="$Unigrid.CultureList.Columns.CultureShortName$"
                Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="CultureAlias" Caption="$CultureEdit.CultureAlias$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="CultureID" Caption="$general.sitename$" Wrap="false" Visible="false">
                <Filter Type="site" />
            </ug:Column>
            <ug:Column Width="100%" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
