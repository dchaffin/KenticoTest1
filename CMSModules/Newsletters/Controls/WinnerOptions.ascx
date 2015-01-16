<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WinnerOptions.ascx.cs"
    Inherits="CMSModules_Newsletters_Controls_WinnerOptions" %>
<cms:CMSUpdatePanel runat="server" ID="pnlU">
    <ContentTemplate>
        <asp:Panel runat="server" ID="pnlG">
            <div class="PageContent">
                <cms:LocalizedLabel runat="server" ID="lblWinnerAccordingTo" ResourceString="newsletterissue_send.winneraccordingto"
                    EnableViewState="false" />
                <br />
                <br />
                <table>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <cms:LocalizedRadioButton ID="radOpen" runat="server" GroupName="Send" AutoPostBack="true"
                                ResourceString="newsletterissue_send.openrate" Checked="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <cms:LocalizedRadioButton ID="radClicks" runat="server" GroupName="Send" AutoPostBack="true"
                                ResourceString="newsletterissue_send.totalclicks" Checked="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <cms:LocalizedRadioButton ID="radManually" runat="server" GroupName="Send" AutoPostBack="true"
                                ResourceString="newsletterissue_send.manually" Checked="false" />
                        </td>
                    </tr>
                </table>
                <br />
                <cms:LocalizedLabel runat="server" ID="lblSelectWinner" ResourceString="newsletterissue_send.selectwinner"
                    EnableViewState="false" />
                <asp:TextBox runat="server" ID="txtInterval" Text="60" CssClass="ShortTextBox" MaxLength="5" AutoPostBack="true" />
                <cms:LocalizedDropDownList runat="server" ID="drpInterval" CssClass="ExtraSmallDropDown" AutoPostBack="true" />
            </div>
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
