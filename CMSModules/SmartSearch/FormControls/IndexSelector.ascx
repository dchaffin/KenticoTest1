<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_FormControls_IndexSelector"
    CodeFile="IndexSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ObjectType="cms.SearchIndex" SelectionMode="MultipleTextBox" OrderBy="IndexDisplayName"
            runat="server" ID="usIndexes" AllowEditTextBox="true" ReturnColumnName="IndexName" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
