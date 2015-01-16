<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="PageTemplate_List.aspx.cs" %>

<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <script type="text/javascript">
        //<![CDATA[
        function EditPageTemplate(templateId) {
            parent.location.replace("PageTemplate_Edit.aspx?templateid=" + templateId + "&categoryid=" + "<%=categoryId%>");
            parent.parent.frames['pt_tree'].location.href = "PageTemplate_Tree.aspx?templateid=" + templateId;
        }
        //]]>
    </script>
    <asp:Literal runat="server" ID="ltlScript" />
    <cms:UniGrid runat="server" ID="pageTemplatesGrid" OrderBy="DisplayName" Columns="ObjectID, DisplayName, PageTemplateIsReusable"
        IsLiveSite="false" ObjectType="cms.pagetemplatelist" RememberDefaultState="true">
        <GridActions Parameters="ObjectID">
            <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" OnClick="if (window.EditPageTemplate) {window.EditPageTemplate({0}); return false;}" />
            <ug:Action Name="delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="DisplayName" Caption="$general.displayname$" Wrap="false" Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="PageTemplateIsReusable" ExternalSourceName="#yesno" Caption="$pagetemplate.isreusable$">
                <Filter Type="bool" DefaultValue="1" />
            </ug:Column>
            <ug:Column Width="100%" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
