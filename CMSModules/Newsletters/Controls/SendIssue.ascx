<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SendIssue.ascx.cs" Inherits="CMSModules_Newsletters_Controls_SendIssue" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <table>
            <tr>
                <td style="padding-bottom: 20px;">
                    <cms:LocalizedRadioButton ID="radSendNow" runat="server" GroupName="Send" AutoPostBack="true"
                        OnCheckedChanged="radGroupSend_CheckedChanged" ResourceString="newsletterissue_send.sendnow"
                        Checked="true" />
                </td>
            </tr>
            <asp:PlaceHolder ID="plcSendScheduled" runat="server">
                <tr>
                    <td style="padding-bottom: 20px;">
                        <cms:LocalizedRadioButton ID="radSchedule" runat="server" GroupName="Send" AutoPostBack="true"
                            OnCheckedChanged="radGroupSend_CheckedChanged" ResourceString="newsletterissue_send.schedule" />
                        <div class="UnderRadioContent">
                            <cms:LocalizedLabel ID="lblDateTime" runat="server" ResourceString="newsletterissue_send.datetime"
                                DisplayColon="true" AssociatedControlID="calendarControl" />&nbsp;
                            <cms:DateTimePicker ID="calendarControl" runat="server" Enabled="false" SupportFolder="~/CMSAdminControls/Calendar" />
                        </div>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcSendDraft" runat="server">
                <tr>
                    <td style="padding-bottom: 20px;">
                        <cms:LocalizedRadioButton ID="radSendDraft" runat="server" GroupName="Send" AutoPostBack="true"
                            OnCheckedChanged="radGroupSend_CheckedChanged" ResourceString="newsletterissue_send.senddraft" />
                        <div class="UnderRadioContent">
                            <cms:LocalizedLabel ID="lblDraftEmails" runat="server" ResourceString="newsletterissue_send.emails"
                                DisplayColon="true" AssociatedControlID="txtSendDraft" />&nbsp;
                            <cms:CMSTextBox ID="txtSendDraft" runat="server" MaxLength="450" Enabled="false"
                                CssClass="TextBoxField" />
                        </div>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcSendLater" runat="server">
                <tr>
                    <td>
                        <cms:LocalizedRadioButton ID="radSendLater" runat="server" GroupName="Send" AutoPostBack="true"
                            OnCheckedChanged="radGroupSend_CheckedChanged" ResourceString="newsletterissue_send.sendlater" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </ContentTemplate>
</cms:CMSUpdatePanel>
