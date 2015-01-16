<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="false" CodeFile="PageLayouts.aspx.cs" Inherits="CMSModules_DeviceProfile_Pages_Development_PageLayouts"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/DeviceProfile/Controls/LayoutBinding.ascx" TagPrefix="cms" TagName="LayoutBinding" %>
<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="server">
    <asp:HiddenField ID="SourceLayoutIdentifierHiddenField" runat="server" EnableViewState="false" />
    <asp:HiddenField ID="TargetLayoutIdentifierHiddenField" runat="server" EnableViewState="false" />
    <p style="margin-left: 5px">
        <cms:LocalizedLabel runat="server" ResourceString="device_profile.layoutmapping.introduction"></cms:LocalizedLabel>
    </p>
    <div class="DeviceProfileLayoutGrid UniGridClearPager">
        <cms:UniGrid ID="BindingGrid" runat="server" Columns="LayoutID, LayoutDisplayName"
            ObjectType="cms.layout" OrderBy="LayoutDisplayName" WhereCondition="LayoutIsConvertible = 1"
            ShowActionsLabel="false" ShowActionsMenu="false" ShowObjectMenu="false" RememberStateByParam="">
            <GridColumns>
                <ug:Column Source="LayoutID" Caption="$device_profile.layoutmapping$" ExternalSourceName="SourceLayout"
                    AllowSorting="false">
                </ug:Column>
                <ug:Column Source="LayoutDisplayName" Caption="$device_profile.layoutmapping.sourcelayoutname$"
                    Visible="false">
                    <Filter Type="text" />
                </ug:Column>
            </GridColumns>
            <GridOptions DisplayFilter="true" FilterLimit="5" />
        </cms:UniGrid>
    </div>

    <script type="text/javascript">
    //<![CDATA[

        function Client_SetTargetLayout(parameters) {
            jQuery('#<%= SourceLayoutIdentifierHiddenField.ClientID %>').val(parameters.sourceLayoutId);
            jQuery('#<%= TargetLayoutIdentifierHiddenField.ClientID %>').val(parameters.targetLayoutId);
            <%= ClientScript.GetPostBackEventReference(this, "SetTargetLayout") %>
        }

        function Client_UnsetTargetLayout(parameters) {
            if (confirm('<%= HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.confirmunset"))%>')) {
                jQuery('#<%= SourceLayoutIdentifierHiddenField.ClientID %>').val(parameters.sourceLayoutId);
                <%= ClientScript.GetPostBackEventReference(this, "UnsetTargetLayout") %>
            }
        }

    //]]>
    </script>
</asp:Content>
