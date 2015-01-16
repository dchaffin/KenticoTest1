<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewValidate.aspx.cs" Inherits="CMSModules_Content_CMSDesk_View_ViewValidate"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="View / Validate" %>

<%@ Register Assembly="CMS.ExtendedControls" Namespace="CMS.ExtendedControls" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/FrameResizer.ascx" TagName="FrameResizer"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/DeviceView.ascx" TagName="DeviceView"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <script type="text/javascript">
        //<![CDATA[
        var IsCMSDesk = true;

        // Refresh tree
        function RefreshTree(expandNodeId, selectNodeId) {
            if (parent.RefreshTree) {
                parent.RefreshTree(expandNodeId, selectNodeId);
            }
        }
        //]]>
    </script>
    <asp:Panel ID="pnlContainer" runat="server" CssClass="ViewTabs">
        <cms:FrameResizer ID="allResizer" runat="server" All="true" />
        <cms:JQueryTabContainer ID="pnlTabs" runat="server">
            <cms:JQueryTab ID="tabPreview" runat="server" CssClass="Tab">
                <ContentTemplate>
                    <div class="HeaderSeparator" id="sep1">
                        &nbsp;</div>
                    <cms:DeviceView ID="ucView" runat="server" />
                </ContentTemplate>
            </cms:JQueryTab>
            <cms:JQueryTab ID="tabValidate" runat="server" CssClass="Tab">
                <ContentTemplate>
                    <div class="HeaderSeparator" id="sep2">
                        &nbsp;</div>
                    <iframe width="100%" height="100%" id="pagevalidate" name="pagevalidate" scrolling="no"
                        frameborder="0" enableviewstate="false" src="<%=validatepage%>" onload="CMSViewValidate.ResizeContentArea();"
                        class="ContentFrame"></iframe>
                </ContentTemplate>
            </cms:JQueryTab>
        </cms:JQueryTabContainer>
    </asp:Panel>
    <div class="ViewHelp">
        <cms:Help ID="helpElem" runat="server" TopicName="viewvalidate" HelpName="helpTopic"
            EnableViewState="false" />
    </div>
    <div class="ViewRotate">
        <asp:CheckBox ID="chkRotate" runat="server" EnableViewState="false" CssClass="NotCheckChanges" />
    </div>
</asp:Content>
