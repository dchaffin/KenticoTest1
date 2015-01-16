<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Configuration"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter configuration"
    CodeFile="Newsletter_Configuration.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/NewsletterTemplateSelector.ascx"
    TagPrefix="cms" TagName="NewsletterTemplateSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/Selectors/ScheduleInterval.ascx" TagPrefix="cms"
    TagName="ScheduleInterval" %>
<%@ Register Src="~/CMSAdminControls/UI/System/ServerChecker.ascx" TagPrefix="cms"
    TagName="ServerChecker" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <%-- General config --%>
    <asp:Panel ID="pnlGeneral" runat="server">
        <table style="vertical-align:top">
            <%-- Display name --%>
            <tr>
                <td class="FieldLabel" style="width:200px">
                    <cms:LocalizedLabel runat="server" ID="lblNewsletterDisplayName" EnableViewState="false"
                        ResourceString="Newsletter_Edit.NewsletterDisplayNameLabel" DisplayColon="true"
                        AssociatedControlID="txtNewsletterDisplayName" />
                </td>
                <td>
                    <cms:LocalizableTextBox ID="txtNewsletterDisplayName" runat="server" CssClass="TextBoxField"
                        MaxLength="250" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterDisplayName" runat="server" ControlToValidate="txtNewsletterDisplayName:textbox"
                        Display="dynamic" EnableViewState="false" />
                </td>
            </tr>
            <%-- Code name --%>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblNewsletterName" EnableViewState="false"
                        ResourceString="Newsletter_Edit.NewsletterNameLabel" DisplayColon="true" AssociatedControlID="txtNewsletterName" />
                </td>
                <td>
                    <cms:CodeName ID="txtNewsletterName" runat="server" CssClass="TextBoxField" MaxLength="250" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterName" runat="server" ControlToValidate="txtNewsletterName"
                        Display="dynamic" EnableViewState="false" />
                </td>
            </tr>
            <%-- Subscription template --%>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblSubscriptionTemplate" EnableViewState="false"
                        ResourceString="Newsletter_Edit.SubscriptionTemplate" DisplayColon="true" AssociatedControlID="subscriptionTemplate" />
                </td>
                <td>
                    <cms:NewsletterTemplateSelector ID="subscriptionTemplate" runat="server" />
                </td>
            </tr>
            <%-- Unsubscription template --%>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblUnsubscriptionTemplate" EnableViewState="false"
                        ResourceString="Newsletter_Edit.UnsubscriptionTemplate" DisplayColon="true" AssociatedControlID="unsubscriptionTemplate" />
                </td>
                <td>
                    <cms:NewsletterTemplateSelector ID="unsubscriptionTemplate" runat="server" />
                </td>
            </tr>
            <%-- Sender name --%>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblNewsletterSenderName" EnableViewState="false"
                        ResourceString="Newsletter_Edit.NewsletterSenderNameLabel" DisplayColon="true"
                        AssociatedControlID="txtNewsletterSenderName" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtNewsletterSenderName" runat="server" CssClass="TextBoxField"
                        MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterSenderName" runat="server" ErrorMessage=""
                        ControlToValidate="txtNewsletterSenderName" EnableViewState="false" />
                </td>
            </tr>
            <%-- Sender email --%>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblNewsletterSenderEmail" EnableViewState="false"
                        ResourceString="Newsletter_Edit.NewsletterSenderEmailLabel" DisplayColon="true"
                        AssociatedControlID="txtNewsletterSenderEmail" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtNewsletterSenderEmail" runat="server" CssClass="TextBoxField"
                        MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterSenderEmail" runat="server" ErrorMessage=""
                        ControlToValidate="txtNewsletterSenderEmail" EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <%-- Base URL --%>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblNewsletterBaseUrl" EnableViewState="false"
                        ResourceString="Newsletter_Configuration.NewsletterBaseUrl" DisplayColon="true"
                        AssociatedControlID="txtNewsletterBaseUrl" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtNewsletterBaseUrl" runat="server" CssClass="TextBoxField"
                        MaxLength="500" />
                </td>
            </tr>
            <%-- Unsubscription URL --%>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblNewsletterUnsubscribeUrl" EnableViewState="false"
                        ResourceString="Newsletter_Configuration.NewsletterUnsubscribeUrl" DisplayColon="true"
                        AssociatedControlID="txtNewsletterUnsubscribeUrl" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtNewsletterUnsubscribeUrl" runat="server" CssClass="TextBoxField"
                        MaxLength="1000" />
                </td>
            </tr>
            <%-- Draft emails --%>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblDraftEmails" runat="server" EnableViewState="false" ResourceString="newsletter.draftemails"
                        DisplayColon="true" AssociatedControlID="txtDraftEmails" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtDraftEmails" runat="server" CssClass="TextBoxField" MaxLength="450" />
                </td>
            </tr>
            <%-- Use email queue --%>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblUseEmailQueue" runat="server" EnableViewState="false"
                        ResourceString="newsletter.useemailqueue" DisplayColon="true" AssociatedControlID="chkUseEmailQueue" />
                </td>
                <td>
                    <asp:CheckBox ID="chkUseEmailQueue" runat="server" />
                </td>
            </tr>
            <%-- Enable resending --%>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblEnableResending" runat="server" EnableViewState="false"
                        ResourceString="newsletter.enableresending" DisplayColon="true" AssociatedControlID="chkEnableResending" />
                </td>
                <td>
                    <asp:CheckBox ID="chkEnableResending" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <%-- Template based config --%>
    <asp:PlaceHolder ID="plcTemplate" runat="server">
        <asp:Panel ID="pnlTemplate" runat="server">
            <table>
                <tr>
                    <td class="FieldLabel" style="width:200px">
                        <cms:LocalizedLabel runat="server" ID="lblIssueTemplate" EnableViewState="false"
                            ResourceString="Newsletter_Edit.NewsletterTemplate" DisplayColon="true" AssociatedControlID="issueTemplate" />
                    </td>
                    <td>
                        <cms:NewsletterTemplateSelector ID="issueTemplate" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
    </asp:PlaceHolder>
    <%-- Dynamic config --%>
    <asp:PlaceHolder ID="plcDynamic" runat="server">
        <asp:Panel ID="pnlDynamic" runat="server">
            <table>
                <%-- Subject --%>
                <tr>
                    <td class="FieldLabel" style="width:200px;vertical-align:top;">
                        <cms:LocalizedLabel ID="lblSubject" runat="server" EnableViewState="false" ResourceString="general.subject"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:CMSUpdatePanel ID="pnlUpSubject" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="radPageTitle" />
                                <asp:AsyncPostBackTrigger ControlID="radFollowing" />
                            </Triggers>
                            <ContentTemplate>
                                <cms:LocalizedRadioButton ID="radPageTitle" runat="server" GroupName="Subject" ResourceString="Newsletter_Configuration.PageTitleSubject"
                                    AutoPostBack="True" OnCheckedChanged="radSubject_CheckedChanged" TextAlign="Right" />
                                <br />
                                <cms:LocalizedRadioButton ID="radFollowing" runat="server" GroupName="Subject" ResourceString="Newsletter_Configuration.PageTitleFollowing"
                                    AutoPostBack="True" OnCheckedChanged="radSubject_CheckedChanged" TextAlign="Right" />
                                <br />
                                <cms:LocalizableTextBox ID="txtSubject" runat="server" MaxLength="500" CssClass="TextBoxField"
                                    Style="margin-top: 5px" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </td>
                </tr>
                <tr style="height: 5px;">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <%-- Dynamic newsletter URL --%>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblNewsletterDynamicURL" EnableViewState="false"
                            ResourceString="Newsletter_Edit.SourcePageURL" DisplayColon="true" AssociatedControlID="txtNewsletterDynamicURL" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtNewsletterDynamicURL" runat="server" CssClass="TextBoxField"
                            MaxLength="500" />
                        <cms:ServerChecker runat="server" ID="serverChecker" TextBoxControlID="txtNewsletterDynamicURL" />
                    </td>
                </tr>
                <%-- Scheduler --%>
                <tr>
                    <td class="FieldLabel" style="vertical-align:top;">
                        <cms:LocalizedLabel runat="server" ID="lblSchedule" EnableViewState="false" ResourceString="Newsletter_Edit.Schedule"
                            DisplayColon="true" AssociatedControlID="chkSchedule" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkSchedule" runat="server" Checked="true" AutoPostBack="true"
                            OnCheckedChanged="chkSchedule_CheckedChanged" /><br />
                        <cms:CMSUpdatePanel ID="pnlUpScheduler" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="chkSchedule" />
                            </Triggers>
                            <ContentTemplate>
                                <cms:ScheduleInterval ID="schedulerInterval" runat="server" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
    </asp:PlaceHolder>
    <%-- Online marketing config --%>
    <asp:PlaceHolder ID="plcTracking" runat="server">
        <asp:Panel ID="pnlOM" runat="server">
            <table>
                <%-- Track opened emails --%>
                <tr>
                    <td class="FieldLabel" style="width:200px">
                        <cms:LocalizedLabel ID="lblTrackOpenedEmails" runat="server" EnableViewState="false"
                            ResourceString="newsletter.trackopenedemails" DisplayColon="true" AssociatedControlID="chkTrackOpenedEmails" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkTrackOpenedEmails" runat="server" />
                    </td>
                </tr>
                <%-- Track clicked links --%>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblTrackClickedLinks" runat="server" EnableViewState="false"
                            ResourceString="newsletter.trackclickedlinks" DisplayColon="true" AssociatedControlID="chkTrackClickedLinks" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkTrackClickedLinks" runat="server" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="plcOM" runat="server">
                    <%-- Log activities --%>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel ID="lblLogActivity" runat="server" EnableViewState="false" ResourceString="newsletter.trackactivities"
                                DisplayColon="true" AssociatedControlID="chkLogActivity" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkLogActivity" runat="server" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </table>
        </asp:Panel>
        <br />
    </asp:PlaceHolder>
    <%-- Double opt-in config --%>
    <asp:Panel ID="pnlDoubleOptIn" runat="server">
        <table>
            <%-- Enable double opt-in --%>
            <tr>
                <td class="FieldLabel" style="width:200px">
                    <cms:LocalizedLabel ID="lblEnableOptIn" runat="server" EnableViewState="false" ResourceString="newsletter_configuration.enableoptin"
                        DisplayColon="true" AssociatedControlID="chkEnableOptIn" />
                </td>
                <td>
                    <asp:CheckBox ID="chkEnableOptIn" runat="server" AutoPostBack="true" OnCheckedChanged="chkEnableOptIn_CheckedChanged" />
                </td>
            </tr>
        </table>
        <cms:CMSUpdatePanel ID="pnlUpOptIn" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="chkEnableOptIn" />
            </Triggers>
            <ContentTemplate>
                <asp:PlaceHolder ID="plcOptIn" runat="server" Visible="false">
                    <table>
                        <%-- Opt-in template --%>
                        <tr>
                            <td class="FieldLabel" style="width:200px">
                                <cms:LocalizedLabel runat="server" ID="lblOptInTemplate" EnableViewState="false"
                                    ResourceString="newsletter_configuration.optnintemplate" DisplayColon="true"
                                    AssociatedControlID="optInSelector" />
                            </td>
                            <td>
                                <cms:NewsletterTemplateSelector ID="optInSelector" runat="server" />
                            </td>
                        </tr>
                        <%-- Approval URL --%>
                        <tr>
                            <td class="FieldLabel">
                                <cms:LocalizedLabel runat="server" ID="lblOptInURL" EnableViewState="false" ResourceString="newsletter_configuration.optinurl"
                                    DisplayColon="true" AssociatedControlID="txtOptInURL" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtOptInURL" runat="server" CssClass="TextBoxField" MaxLength="500" />
                            </td>
                        </tr>
                        <%-- Send confirmation --%>
                        <tr>
                            <td class="FieldLabel">
                                <cms:LocalizedLabel ID="lblSendOptInConfirmation" runat="server" EnableViewState="false"
                                    ResourceString="newsletter_configuration.sendoptinconfirmation" DisplayColon="true"
                                    AssociatedControlID="chkSendOptInConfirmation" />
                            </td>
                            <td>
                                <asp:CheckBox ID="chkSendOptInConfirmation" runat="server" />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
