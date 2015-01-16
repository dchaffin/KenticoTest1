<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_PageLayouts_PageLayout_New"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Page Layout New" CodeFile="PageLayout_New.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="PageLayoutCode" Src="~/CMSFormControls/Layouts/PageLayoutCode.ascx" %>
<%@ Register TagPrefix="cms" TagName="CSSStylesEditor" Src="~/CMSFormControls/Layouts/CSSStylesEditor.ascx" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" ID="EditFormLayout" ObjectType="cms.layout" DefaultFieldLayout="Inline"
        RedirectUrlAfterCreate="PageLayout_Frameset.aspx?layoutid={%EditedObject.ID%}&saved=1">
        <LayoutTemplate>
            <cms:FormCategory runat="server" ID="pnlGeneral" ResourceString="general.general"
                DefaultFieldLayout="TwoColumns">
                <cms:FormField runat="server" ID="fDisplayName" Field="LayoutDisplayName" FormControl="LocalizableTextBox"
                    ResourceString="general.displayname" DisplayColon="true" />
                <cms:FormField runat="server" ID="fName" Field="LayoutCodeName" FormControl="CodeName"
                    ResourceString="general.codename" DisplayColon="true" />
                <cms:FormField runat="server" ID="fDescription" Field="LayoutDescription" FormControl="TextAreaControl"
                    ResourceString="general.description" DisplayColon="true" />
                <cms:FormField runat="server" ID="fIsConverible" Field="LayoutIsConvertible" FormControl="CheckboxControl" 
                    DisplayColon="true" ResourceString="pagelayout.isconvertible" />
                <cms:FormField runat="server" ID="fZoneCount" Field="LayoutZoneCount" FormControl="TextBoxControl" 
                    DisplayColon="true" ResourceString="pagelayout.zonecount" />
            </cms:FormCategory>
            <cms:FormCategory runat="server" ID="pnlLayout" ResourceString="general.layout" DefaultFieldLayout="Inline">
                <cms:FormField runat="server" ID="fLayoutCode" Field="LayoutCode">
                    <cms:PageLayoutCode runat="server" ID="codeLayoutElem" CodeColumn="LayoutCode" TypeColumn="LayoutType" />
                </cms:FormField>
                <cms:FormField runat="server" ID="fCSS" Field="LayoutCSS">
                    <cms:CSSStylesEditor runat="server" ID="cssEditor" />
                </cms:FormField>
            </cms:FormCategory>
            <cms:FormSubmitButton runat="server" ID="btnSave" />
        </LayoutTemplate>
    </cms:UIForm>
</asp:Content>
