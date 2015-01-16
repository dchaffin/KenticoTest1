<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="CMSModules_DeviceProfile_Controls_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:LocalizedLabel ID="ltlInfo" runat="server" ResourceString="Device_Profile.ListInfo" CssClass="InfoLabel" />
<cms:UniGrid runat="server" ID="gridProfiles" ObjectType="cms.deviceprofile" OrderBy="ProfileOrder"
    Columns="ProfileID, ProfileName, ProfileDisplayName, ProfileOrder, ProfileEnabled"
    IsLiveSite="false" EditActionUrl="Frameset.aspx?profileid={0}">
    <GridActions Parameters="ProfileID">
        <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
        <ug:Action Name="#delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$" />
        <ug:Action Name="moveup" Caption="$General.Up$" Icon="Up.png" />
        <ug:Action Name="movedown" Caption="$General.Down$" Icon="Down.png" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="ProfileDisplayName" Caption="$Unigrid.DeviceProfiles.Columns.ProfileDisplayName$"
            Wrap="false" Localize="true" AllowSorting="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="ProfileEnabled" Caption="$general.enabled$" ExternalSourceName="#yesno"
            AllowSorting="false" />
        <ug:Column Width="100%" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>
