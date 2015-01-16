<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_BannerManagement_FormControls_BannerCategorySelector" CodeFile="BannerCategorySelector.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagPrefix="cms" TagName="UniSelector" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniSelector ObjectType="CMS.BannerCategory" ID="usBannerCategory" AllowEditTextBox="true" 
            AllowAll="false" ReturnColumnName="BannerCategoryName" OrderBy="BannerCategoryDisplayName"
            runat="server" SelectionMode="SingleTextBox"
            AddGlobalObjectSuffix="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
