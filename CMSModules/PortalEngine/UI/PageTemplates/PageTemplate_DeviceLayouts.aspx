<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_DeviceLayouts"
    Title="Page Template Edit - Device layouts" Theme="Default" CodeFile="PageTemplate_DeviceLayouts.aspx.cs" %>

<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <script type="text/javascript">
        //<![CDATA[
        function RefreshWOpener(window) {
            var url = document.location + '';
            if ((typeof (refreshPageLocationUrl) != 'undefined') && (refreshPageLocationUrl != null)) {
                url = refreshPageLocationUrl;
            }
            document.location.replace(url);
        }

        function EditDeviceLayout(deviceProfileId, win) {
            location.replace(GetEditDeviceUrl(deviceProfileId));
        }
        //]]>
    </script>
    <cms:UniGrid runat="server" ID="deviceLayoutsGrid" ShortID="dl" OrderBy="ProfileDisplayName"
        Columns="ProfileID, ProfileDisplayName, ProfileIconGUID" IsLiveSite="false" ObjectType="cms.deviceprofile">
        <GridActions Parameters="ProfileID">
            <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" OnClick="if (window.EditDeviceLayout) { window.EditDeviceLayout({0}); return false;}" />
            <ug:Action Name="delete" CommandArgument="ProfileID" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="ProfileDisplayName" Caption="$objecttype.cms_deviceprofile$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Width="100%" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
