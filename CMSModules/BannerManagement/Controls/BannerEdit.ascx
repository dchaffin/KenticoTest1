<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_BannerManagement_Controls_BannerEdit" CodeFile="BannerEdit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/BannerManagement/FormControls/HitsClicksControl.ascx" TagName="HitsClicksControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/BannerManagement/FormControls/BannerContentEdit.ascx" TagName="BannerContentEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/BannerManagement/FormControls/BannerTypeSelector.ascx" TagName="BannerTypeSelector" TagPrefix="cms" %>
<cms:UIForm runat="server" ID="EditForm" ObjectType="cms.banner" RedirectUrlAfterCreate="Banner_Edit.aspx?id={%EditedObject.ID%}&siteid={?siteid?}&categoryid={?categoryid?}&saved=1"
    Width="100%" DefaultFieldLayout="Inline">
    <LayoutTemplate>
        <cms:FormCategory runat="server" ID="pnlGeneral" ResourceString="general.general" DefaultFieldLayout="TwoColumns">
            <cms:FormField runat="server" ID="fBannerDisplayName" Field="BannerDisplayName" FormControl="TextBoxControl" ResourceString="general.displayname" DisplayColon="true" />
            <cms:FormField runat="server" ID="fBannerCodeName" Field="BannerName" FormControl="CodeName" ResourceString="general.codename" DisplayColon="true" />
            <cms:FormField runat="server" ID="fBannerURL" Field="BannerURL" FormControl="TextBoxControl" ResourceString="general.url" DisplayColon="true" />
            <cms:FormField runat="server" ID="fBannerWeight" Field="BannerWeight" FormControl="TextBoxControl" ResourceString="banner.bannernew.weight" DisplayColon="true"
                Value="5" />
            <cms:FormField runat="server" ID="fBannerBlank" Field="BannerBlank" FormControl="CheckBoxControl" ResourceString="banner.bannernew.openblank" DisplayColon="true"
                Value="true" />
        </cms:FormCategory>
        <cms:FormCategory runat="server" ID="pnlVisibility" ResourceString="banner.fieldset.visibilitysettings" DefaultFieldLayout="TwoColumns">
            <cms:FormField runat="server" ID="fBannerEnabled" Field="BannerEnabled" FormControl="CheckBoxControl" ResourceString="general.enabled" DisplayColon="true"
                Value="true" />
            <cms:FormField runat="server" ID="fBannerValidFrom" Field="BannerFrom" FormControl="CalendarControl" ResourceString="banner.bannernew.bannerfrom" DisplayColon="true" />
            <cms:FormField runat="server" ID="fBannerValidTo" Field="BannerTo" FormControl="CalendarControl" ResourceString="banner.bannernew.bannerto" DisplayColon="true" />
            <cms:FormField runat="server" ID="fBannerHitsLeft" Field="BannerHitsLeft" ResourceString="banner.bannernew.hitsleft" DisplayColon="true">
                <cms:HitsClicksControl runat="server" ID="hccHits" AllowUnlimitedResourceString="banner.bannernew.hitsleftunlimited" AllowSpecificResourceString="banner.bannernew.hitsleftspecific"
                    AddNumberResourceString="banner.bannernew.addhits" NumberToAddResourceString="banner.bannernew.numberofhitstoadd" />
            </cms:FormField>
            <cms:FormField runat="server" ID="fBannerClicksLeft" Field="BannerClicksLeft" ResourceString="banner.bannernew.clicksleft" DisplayColon="true">
                <cms:HitsClicksControl runat="server" ID="hccClicks" AllowUnlimitedResourceString="banner.bannernew.clicksleftunlimited" AllowSpecificResourceString="banner.bannernew.clicksleftspecific"
                    AddNumberResourceString="banner.bannernew.addclicks" NumberToAddResourceString="banner.bannernew.numberofclickstoadd" />
            </cms:FormField>
        </cms:FormCategory>
        <cms:CMSUpdatePanel runat="server" ID="updBannerTypeSelector" UpdateMode="Conditional">
            <ContentTemplate>
                <cms:FormCategory runat="server" ID="pnlContent" ResourceString="general.content" DefaultFieldLayout="TwoColumns">
                    <cms:FormField runat="server" ID="fBannerType" Field="BannerType" ResourceString="banner.bannernew.type" DisplayColon="true">
                        <cms:BannerTypeSelector runat="server" ID="bannerTypeSelector" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fBannerContent" Field="BannerContent" Layout="Inline">
                        <cms:BannerContentEdit runat="server" ID="contentEdit"></cms:BannerContentEdit>
                    </cms:FormField>
                </cms:FormCategory>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <cms:FormSubmit runat="server" ID="fSubmit" />
    </LayoutTemplate>
</cms:UIForm>
