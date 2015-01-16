<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Groups_GroupEdit"
    CodeFile="GroupEdit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/ThreeStateCheckBox.ascx" TagName="ThreeStateCheckBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/selectsinglepath.ascx"
    TagName="PathSelector" TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:Panel ID="pnlGroupEdit" runat="server" CssClass="ForumEdit">
    <asp:Panel ID="pnlGeneral" runat="server">
        <table>
            <tr>
                <td class="FieldLabel">
                    <asp:Label runat="server" ID="lblGroupDisplayName" EnableViewState="false" />
                </td>
                <td>
                    <cms:LocalizableTextBox ID="txtGroupDisplayName" runat="server" CssClass="TextBoxField"
                        MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvGroupDisplayName" runat="server" ControlToValidate="txtGroupDisplayName:textbox"
                        ErrorMessage="" ValidationGroup="vgForumGroup" Display="Dynamic"></cms:CMSRequiredFieldValidator>
                </td>
            </tr>
            <asp:PlaceHolder ID="plcCodeName" runat="Server">
                <tr>
                    <td class="FieldLabel">
                        <asp:Label runat="server" ID="lblGroupName" EnableViewState="false" />
                    </td>
                    <td>
                        <cms:CodeName ID="txtGroupName" runat="server" CssClass="TextBoxField" MaxLength="200" />
                        <cms:CMSRequiredFieldValidator ID="rfvGroupName" Display="Dynamic" runat="server"
                            ErrorMessage="" ControlToValidate="txtGroupName" ValidationGroup="vgForumGroup"></cms:CMSRequiredFieldValidator>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td style="vertical-align: top; padding-top: 5px" class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblDescription" EnableViewState="false" ResourceString="general.description"
                        DisplayColon="true" />
                </td>
                <td>
                    <cms:LocalizableTextBox ID="txtGroupDescription" runat="server" TextMode="MultiLine"
                        CssClass="TextAreaField" />
                </td>
            </tr>
            <asp:PlaceHolder ID="plcBaseAndUnsubUrl" runat="server" Visible="false">
                <tr>
                    <td class="FieldLabel">
                        <asp:Label runat="server" ID="lblForumBaseUrl" EnableViewState="false" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtForumBaseUrl" runat="server" CssClass="TextBoxField" MaxLength="200" />
                        <cms:LocalizedCheckBox runat="server" ID="chkInheritBaseUrl" Checked="true" ResourceString="Forums.InheritBaseUrl" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <asp:Label runat="server" ID="lblUnsubscriptionUrl" EnableViewState="false" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtUnsubscriptionUrl" runat="server" CssClass="TextBoxField"
                            MaxLength="200" />
                        <cms:LocalizedCheckBox runat="server" ID="chkInheritUnsubUrl" Checked="true" ResourceString="Forums.InheritUnsubsUrl" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlAdvanced" runat="server">
        <table>
            <tr>
                <td class="LeftColumnField">
                    <cms:LocalizedLabel runat="server" ID="lblForumRequireEmail" CssClass="FieldLabel"
                        EnableViewState="false" ResourceString="Forum_Edit.ForumRequireEmailLabel" />
                </td>
                <td>
                    <asp:CheckBox ID="chkForumRequireEmail" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="LeftColumnField">
                    <cms:LocalizedLabel runat="server" ID="lblForumDisplayEmails" CssClass="FieldLabel"
                        EnableViewState="false" ResourceString="Forum_Edit.ForumDisplayEmailsLabel" />
                </td>
                <td>
                    <asp:CheckBox ID="chkForumDisplayEmails" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblType" runat="server" CssClass="FieldLabel" ResourceString="forum.settings.type"
                        DisplayColon="true" />
                </td>
                <td class="RadioGroup">
                    <div>
                        <cms:LocalizedRadioButton ID="radTypeChoose" runat="server" GroupName="type" Checked="true"
                            ResourceString="forum.settings.typechoose" />
                    </div>
                    <div>
                        <cms:LocalizedRadioButton ID="radTypeDiscussion" runat="server" GroupName="type"
                            ResourceString="forum.settings.typediscussion" />
                    </div>
                    <div>
                        <cms:LocalizedRadioButton ID="radTypeAnswer" runat="server" GroupName="type" ResourceString="forum.settings.typeanswer" />
                    </div>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblIsAnswerLimit" EnableViewState="false"
                        ResourceString="forum.settings.isanswerlimit" DisplayColon="true" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtIsAnswerLimit" runat="server" CssClass="TextBoxField" MaxLength="9" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblImageMaxSideSize" EnableViewState="false"
                        ResourceString="forum.settings.maxsidesize" DisplayColon="true" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtImageMaxSideSize" runat="server" CssClass="TextBoxField" MaxLength="9" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblMaxAttachmentSize" EnableViewState="false"
                        ResourceString="forum.settings.maxattachmentsize" DisplayColon="true" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtMaxAttachmentSize" runat="server" CssClass="TextBoxField"
                        MaxLength="9" />
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcOnline" Visible="false">
                <tr>
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblLogActivity" EnableViewState="false" ResourceString="forum.settings.logactivity"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkLogActivity" runat="server" CssClass="CheckBoxMovedLeft" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlSecurity" runat="server">
        <table>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblAuthorEdit" EnableViewState="false" ResourceString="forum.settings.authoredit"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkAuthorEdit" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblAuthorDelete" EnableViewState="false" ResourceString="forum.settings.authordelete"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkAuthorDelete" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="LeftColumnField">
                    <cms:LocalizedLabel runat="server" ID="lblCaptcha" CssClass="FieldLabel" EnableViewState="false"
                        ResourceString="Forum_Edit.useCaptcha" />
                </td>
                <td>
                    <asp:CheckBox ID="chkCaptcha" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlEditor" runat="server">
        <table>
            <asp:PlaceHolder runat="server" ID="plcUseHtml">
                <tr>
                    <td class="LeftColumnField">
                        <cms:LocalizedLabel runat="server" ID="lblUseHTML" CssClass="FieldLabel" EnableViewState="false"
                            ResourceString="Forum_Edit.UseHtml" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkUseHTML" runat="server" CssClass="CheckBoxMovedLeft" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableUrl" EnableViewState="false" ResourceString="forum.settings.enablesimpleurl"
                        DisplayColon="true" />
                </td>
                <td>
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <cms:LocalizedRadioButton ID="radUrlNo" runat="server" ResourceString="general.no"
                                    GroupName="EnableImage" />
                            </td>
                            <td>
                                <cms:LocalizedRadioButton ID="radUrlSimple" runat="server" ResourceString="forum.settings.simpledialog"
                                    GroupName="EnableImage" CssClass="RightColumn" />
                            </td>
                            <td>
                                <cms:LocalizedRadioButton ID="radUrlAdvanced" runat="server" ResourceString="forum.settings.advanceddialog"
                                    GroupName="EnableImage" CssClass="RightColumn" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableImage" EnableViewState="false" ResourceString="forum.settings.enablesimpleimage"
                        DisplayColon="true" />
                </td>
                <td>
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <cms:LocalizedRadioButton ID="radImageNo" runat="server" ResourceString="general.no"
                                    GroupName="EnableURL" />
                            </td>
                            <td>
                                <cms:LocalizedRadioButton ID="radImageSimple" runat="server" ResourceString="forum.settings.simpledialog"
                                    GroupName="EnableURL" CssClass="RightColumn" />
                            </td>
                            <td>
                                <cms:LocalizedRadioButton ID="radImageAdvanced" runat="server" ResourceString="forum.settings.advanceddialog"
                                    GroupName="EnableURL" CssClass="RightColumn" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableQuote" EnableViewState="false" ResourceString="forum.settings.enablequote"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkEnableQuote" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableCode" EnableViewState="false" ResourceString="forum.settings.enablecode"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkEnableCode" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableBold" EnableViewState="false" ResourceString="forum.settings.enablebold"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkEnableBold" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableItalic" EnableViewState="false" ResourceString="forum.settings.enableitalic"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkEnableItalic" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableUnderline" EnableViewState="false"
                        ResourceString="forum.settings.enableunderline" DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkEnableUnderline" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableStrike" EnableViewState="false" ResourceString="forum.settings.enablestrike"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkEnableStrike" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableColor" EnableViewState="false" ResourceString="forum.settings.enablecolor"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkEnableColor" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlOptIn" runat="server">
        <table>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblEnableOptIn" runat="server" EnableViewState="false" ResourceString="general.enableoptin"
                        DisplayColon="true" AssociatedControlID="chkEnableOptIn" />
                </td>
                <td>
                    <cms:ThreeStateCheckBox ID="chkEnableOptIn" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblOptInURL" EnableViewState="false" ResourceString="general.optinurl"
                        DisplayColon="true" AssociatedControlID="txtOptInURL" />
                </td>
                <td>
                    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always" RenderMode="Inline">
                        <ContentTemplate>
                            <cms:PathSelector ID="txtOptInURL" runat="server" CssClass="Inline" MaxLength="450" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                    <cms:LocalizedCheckBox runat="server" ID="chkInheritOptInURL" Checked="true" ResourceString="Forums.InheritBaseUrl" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblSendOptInConfirmation" runat="server" EnableViewState="false"
                        ResourceString="general.sendoptinconfirmation" DisplayColon="true" AssociatedControlID="chkSendOptInConfirmation" />
                </td>
                <td>
                    <cms:ThreeStateCheckBox ID="chkSendOptInConfirmation" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" CssClass="SubmitButton"
                        EnableViewState="false" ValidationGroup="vgForumGroup" ResourceString="General.OK" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
