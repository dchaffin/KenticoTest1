<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_EmailTemplates_FormControls_EmailTemplateSelector"
    CodeFile="EmailTemplateSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ObjectType="cms.emailtemplate" SelectionMode= "SingleTextBox"
            OrderBy="EmailTemplateDisplayName" ResourcePrefix="emailtemplateselect" AllowEmpty="true"
            runat="server" ID="usTemplate" AllowEditTextBox="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
