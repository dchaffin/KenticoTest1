<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Cultures_SupportedCultureSelector"
    CodeFile="SupportedCultureSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" ShortID="sc" runat="server" ObjectType="cms.culture"
            OrderBy="CultureName" AllowEditTextBox="true" LocalizeItems="true" SelectionMode="SingleTextBox" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
