<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_Pages_TranslateDocuments"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="TranslateDocuments.aspx.cs"
    Title="Content - Translate" %>

<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx" TagName="SelectPath"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Translations/Controls/TranslationServiceSelector.ascx"
    TagName="TranslationServiceSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncBackground ID="backgroundElem" runat="server" />
        <div class="AsyncLogArea">
            <div>
                <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                    <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader">
                        <cms:PageTitle ID="titleElemAsync" runat="server" SetWindowTitle="false" />
                    </asp:Panel>
                    <asp:Panel ID="pnlCancel" runat="server" CssClass="PageHeaderLine">
                        <cms:CMSButton runat="server" ID="btnCancel" CssClass="SubmitButton" />
                    </asp:Panel>
                    <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                        <cms:AsyncControl ID="ctlAsync" runat="server" />
                    </asp:Panel>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel runat="server" ID="pnlContent">
        <cms:MessagesPlaceHolder runat="server" ID="plcMessages" WrapperControlID="paneContent" />
        <asp:PlaceHolder runat="server" ID="plcInfo" EnableViewState="false"><strong>
            <cms:LocalizedLabel runat="server" ID="lblInfo" EnableViewState="false" ResourceString="content.translatefollowing"
                DisplayColon="true" /></strong>
            <br />
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlList" EnableViewState="false">
            <asp:Panel ID="pnlDocList" runat="server" CssClass="ScrollableList" EnableViewState="false">
                <asp:Label ID="lblDocuments" runat="server" CssClass="ContentLabel" EnableViewState="true" />
            </asp:Panel>
            <asp:Panel ID="pnlDocSelector" runat="server" Visible="false">
                <cms:LocalizedLabel ID="lblSelectDocInfo" runat="server" CssClass="ContentLabel"
                    EnableViewState="true" ResourceString="translationservice.selectdocuments" DisplayColon="true" /><br />
                <br />
                <cms:SelectPath runat="server" ID="pathElem" IsLiveSite="false" SubItemsNotByDefault="true" />
            </asp:Panel>
            <cms:LocalizedCheckBox ID="chkSkipTranslated" runat="server" Checked="true" EnableViewState="false"
                ResourceString="translations.skipalreadytranslated" /><br />
            <cms:LocalizedCheckBox ID="chkSeparateSubmissions" runat="server" Checked="true"
                Visible="false" EnableViewState="false" ResourceString="translations.useseparatesubmissions" />
            <br />
            <br />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSettings">
            <fieldset>
                <legend>
                    <cms:LocalizedLabel runat="server" ID="lblSelectTranslationService" EnableViewState="false"
                        ResourceString="content.selecttranslationservice" /></legend>
                <asp:Panel runat="server" ID="pnlServiceSelector">
                    <cms:TranslationServiceSelector runat="server" ID="translationElem" />
                </asp:Panel>
            </fieldset>
        </asp:Panel>
        <br />
        <br />
        <asp:Panel runat="server" ID="pnlButtons">
            <cms:LocalizedButton ID="btnOk" runat="server" CssClass="SubmitButton" OnClick="btnTranslate_Click"
                ResourceString="general.translate" EnableViewState="false" />&nbsp;<cms:LocalizedButton
                    ID="btnNo" runat="server" CssClass="SubmitButton" ResourceString="general.cancel"
                    EnableViewState="false" />
        </asp:Panel>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
