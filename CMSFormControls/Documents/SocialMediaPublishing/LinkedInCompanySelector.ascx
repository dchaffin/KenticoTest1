<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LinkedInCompanySelector.ascx.cs" Inherits="CMSFormControls_Documents_SocialMediaPublishing_LinkedInCompanySelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

 <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" ObjectType="cms.linkedincompanylist" ReturnColumnName="CompanyID" ResourcePrefix="LinkedInCompany" IsLiveSite="false"
            DialogGridName="~\CMSFormControls\Documents\SocialMediaPublishing\LinkedInCompanySelectorDisplayList.xml" AllowEditTextBox="true" SelectionMode="SingleTextBox" />
    </ContentTemplate>
</cms:CMSUpdatePanel>