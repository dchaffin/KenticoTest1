<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Controls_AnswerEdit"
    CodeFile="AnswerEdit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/SelectAlternativeForm.ascx" TagName="AlternativeFormSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlBody" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <asp:Panel ID="pnlGeneral" runat="server">
        <table style="vertical-align: top">
            <tr>
                <td class="FieldLabel" style="width: 100px">
                    <asp:Label runat="server" ID="lblAnswerText" EnableViewState="false" />
                </td>
                <td>
                    <cms:LocalizableTextBox ID="txtAnswerText" runat="server" CssClass="TextBoxField"
                        MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvAnswerText" runat="server" ControlToValidate="txtAnswerText:textbox"
                        ValidationGroup="required" EnableViewState="false" />
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcVotes">
                <tr>
                    <td class="FieldLabel">
                        <asp:Label runat="server" ID="lblVotes" EnableViewState="false" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtVotes" runat="server" CssClass="ShortTextBox" MaxLength="9" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblAnswerEnabled" EnableViewState="false"
                            AssociatedControlID="chkAnswerEnabled" ResourceString="general.enabled" DisplayColon="true" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkAnswerEnabled" runat="server" Checked="true" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </asp:Panel>
    <asp:PlaceHolder ID="plcOpenAnswer" runat="server" Visible="false">
        <table style="vertical-align: top">
            <tr>
                <td>
                    <asp:CheckBox ID="chkAnswerIsOpenEnded" runat="server" AutoPostBack="true" Checked="false"
                        OnCheckedChanged="chkAnswerIsOpenEnded_CheckedChanged" />
                </td>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblAnswerIsOpenEnded" EnableViewState="false"
                        AssociatedControlID="chkAnswerIsOpenEnded" ResourceString="polls.answerisopenended" />
                </td>
            </tr>
        </table>
        <asp:PlaceHolder runat="server" ID="plcOpenAnswerSettings">
            <table style="vertical-align: top">
                <tr>
                    <td>
                        <asp:CheckBox ID="chkAnswerHideForm" runat="server" Checked="false" />
                    </td>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblAnswerHideForm" EnableViewState="false"
                            AssociatedControlID="chkAnswerHideForm" ResourceString="poll.answerhideform" />
                    </td>
                </tr>
            </table>
            <cms:CMSUpdatePanel ID="updPanelForm" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <asp:Panel ID="pnlAnswerForm" runat="server">
                        <table style="vertical-align: top">
                            <tr>
                                <td class="FieldLabel" style="width: 100px">
                                    <cms:LocalizedLabel runat="server" ID="lblAnswerForm" EnableViewState="false" ResourceString="polls.answerform"
                                        DisplayColon="true" />
                                </td>
                                <td>
                                    <asp:PlaceHolder ID="plcBizFormSelector" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel">
                                    <cms:LocalizedLabel runat="server" ID="lblAnswerAlternativeForm" EnableViewState="false"
                                        ResourceString="polls.answeraltform" DisplayColon="true" />
                                </td>
                                <td>
                                    <cms:AlternativeFormSelector ID="alternativeFormElem" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
    <table style="vertical-align: top">
        <tr>
            <td>
            </td>
            <td>
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    CssClass="SubmitButton" ValidationGroup="required" ResourceString="General.OK" />
            </td>
        </tr>
    </table>
</asp:Panel>
