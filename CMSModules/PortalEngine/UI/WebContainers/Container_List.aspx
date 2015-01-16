<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_WebContainers_Container_List"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Web part container"
    CodeFile="Container_List.aspx.cs" %>

<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UniGrid runat="server" ID="UniGrid" OrderBy="ContainerDisplayName"
        IsLiveSite="false" ObjectType="cms.webpartcontainer" Columns="ContainerID, ContainerDisplayName">
        <GridActions Parameters="ContainerID">
            <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
            <ug:Action Name="delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="ContainerDisplayName" Caption="$general.displayname$" Wrap="false"
                Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="ContainerID" Caption="$general.sitename$" Visible="false">
                <Filter Type="site" />
            </ug:Column>
            <ug:Column Width="100%" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
