<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_ImportExportSubscribers_Subscriber_Import"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter subscribers"
    CodeFile="Subscriber_Import.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="PageTitle" Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" %>
<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncBackground ID="backgroundElem" runat="server" />
        <div class="AsyncLogArea">
            <div>
                <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                    <asp:Panel ID="pnlCancel" runat="server" CssClass="PageHeaderLine">
                        <cms:LocalizedButton runat="server" ID="btnCancel" CssClass="SubmitButton" EnableViewState="false"
                            ResourceString="general.cancel" />
                    </asp:Panel>
                    <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                        <cms:AsyncControl ID="ctlAsync" runat="server" MaxLogLines="1000" />
                    </asp:Panel>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel runat="server" ID="pnlContent" CssClass="PageContent" EnableViewState="false">
        <cms:MessagesPlaceHolder ID="plcMessages" runat="server" IsLiveSite="false" />
        <strong>
            <cms:LocalizedLabel ID="lblActions" runat="server" CssClass="InfoLabel" EnableViewState="false"
                ResourceString="Subscriber_Import.lblActions" />
        </strong>
        <cms:LocalizedRadioButton ID="radSubscribe" runat="server" GroupName="ImportSubscribers"
            ResourceString="Subscriber_Import.SubscribeImported" CssClass="RadioImport" Checked="true" />
        <cms:LocalizedCheckBox ID="chkDoNotSubscribe" runat="server" ResourceString="Subscriber_Import.DoNotSubscribe"
            CssClass="UnderRadioContent" />
        <br />
        <cms:LocalizedRadioButton ID="radUnsubscribe" runat="server" GroupName="ImportSubscribers"
            ResourceString="Subscriber_Import.UnsubscribeImported" CssClass="RadioImport" />
        <cms:LocalizedRadioButton ID="radDelete" runat="server" GroupName="ImportSubscribers"
            ResourceString="Subscriber_Import.DeleteImported" CssClass="RadioImport" />
        <br />
        <strong>
            <cms:LocalizedLabel ID="lblImportedSub" runat="server" CssClass="InfoLabel" EnableViewState="false"
                ResourceString="Subscriber_Import.lblImportedSub" />
        </strong>
        <cms:LocalizedLabel ID="lblNote" runat="server" CssClass="ContentLabel" EnableViewState="false"
            ResourceString="Subscriber_Import.lblNote" />
        <br />
        <cms:CMSTextBox ID="txtImportSub" runat="server" TextMode="MultiLine" CssClass="TextAreaLarge"
            Height="170px" />
        <br />
        <br />
        <strong>
            <cms:LocalizedLabel ID="lblSelectSub" runat="server" CssClass="InfoLabel" EnableViewState="false"
                ResourceString="Subscriber_Import.lblSelectSub" />
        </strong>
        <cms:UniSelector ID="usNewsletters" runat="server" IsLiveSite="false" ObjectType="Newsletter.Newsletter"
            SelectionMode="Multiple" ResourcePrefix="newsletterselect" />
        <br />
        <div>
            <cms:LocalizedCheckBox ID="chkSendConfirmation" runat="server" ResourceString="Subscriber_Edit.SendConfirmation" /><br />
            <cms:LocalizedCheckBox ID="chkRequireOptIn" runat="server" ResourceString="newsletter.requireoptin" />
        </div>
        <br />
        <cms:LocalizedButton ID="btnImport" runat="server" CssClass="LongSubmitButton" EnableViewState="false"
            ResourceString="general.start" OnClick="btnImport_Click" />
    </asp:Panel>
</asp:Content>
