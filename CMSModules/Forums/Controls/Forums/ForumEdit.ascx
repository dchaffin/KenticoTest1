<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Forums_ForumEdit"
    CodeFile="ForumEdit.ascx.cs" %>
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
                    <asp:Label runat="server" ID="lblForumDisplayName" CssClass="FieldLabel" EnableViewState="false" />
                </td>
                <td class="ForumEditControl">
                    <cms:LocalizableTextBox ID="txtForumDisplayName" runat="server" CssClass="TextBoxField"
                        MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvForumDisplayName" runat="server" ErrorMessage=""
                        ControlToValidate="txtForumDisplayName:textbox" Display="Dynamic" />
                </td>
                <td>
                </td>
            </tr>
            <asp:PlaceHolder ID="plcCodeName" runat="Server">
                <tr>
                    <td class="FieldLabel">
                        <asp:Label runat="server" ID="lblForumName" CssClass="FieldLabel" EnableViewState="false" />
                    </td>
                    <td class="ForumEditControl">
                        <cms:CodeName ID="txtForumName" runat="server" CssClass="TextBoxField" MaxLength="200" />
                        <cms:CMSRequiredFieldValidator Display="Dynamic" ID="rfvForumName" runat="server"
                            ErrorMessage="" ControlToValidate="txtForumName"></cms:CMSRequiredFieldValidator>
                    </td>
                    <td>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblDescription" CssClass="FieldLabel" EnableViewState="false"
                        ResourceString="general.description" DisplayColon="true" />
                </td>
                <td class="ForumEditControl">
                    <cms:LocalizableTextBox ID="txtForumDescription" runat="server" TextMode="MultiLine"
                        CssClass="TextAreaField" />
                </td>
                <td>
                </td>
            </tr>
            <asp:PlaceHolder ID="plcBaseAndUnsubUrl" runat="server" Visible="false">
                <tr>
                    <td style="vertical-align: top; padding-top: 5px;" class="FieldLabel">
                        <asp:Label runat="server" ID="lblBaseUrl" CssClass="FieldLabel" EnableViewState="false" />
                    </td>
                    <td class="ForumEditControl">
                        <cms:CMSTextBox ID="txtBaseUrl" runat="server" CssClass="TextBoxField" MaxLength="200" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkInheritBaseUrl" />
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; padding-top: 5px;" class="FieldLabel">
                        <asp:Label runat="server" ID="lblUnsubscriptionUrl" CssClass="FieldLabel" EnableViewState="false" />
                    </td>
                    <td class="ForumEditControl">
                        <cms:CMSTextBox ID="txtUnsubscriptionUrl" runat="server" CssClass="TextBoxField"
                            MaxLength="200" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkInheritUnsubscribeUrl" />
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
                    <asp:Label runat="server" ID="lblForumOpen" CssClass="FieldLabel" EnableViewState="false" />
                </td>
                <td class="ForumEditControl">
                    <asp:CheckBox ID="chkForumOpen" runat="server" CssClass="CheckBoxMovedLeft" Checked="true" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="LeftColumnField">
                    <asp:Label runat="server" ID="lblForumLocked" CssClass="FieldLabel" EnableViewState="false" />
                </td>
                <td class="ForumEditControl">
                    <asp:CheckBox ID="chkForumLocked" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="LeftColumnField">
                    <asp:Label runat="server" ID="lblForumRequireEmail" CssClass="FieldLabel" EnableViewState="false" />
                </td>
                <td class="ForumEditControl">
                    <cms:ThreeStateCheckBox ID="chkForumRequireEmail" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="LeftColumnField">
                    <asp:Label runat="server" ID="lblForumDisplayEmails" CssClass="FieldLabel" EnableViewState="false" />
                </td>
                <td class="ForumEditControl">
                    <cms:ThreeStateCheckBox ID="chkForumDisplayEmails" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblType" runat="server" CssClass="FieldLabel" ResourceString="forum.settings.type"
                        DisplayColon="true" />
                </td>
                <td class="ForumEditControl">
                    <div>
                        <cms:LocalizedRadioButton ID="radTypeChoose" runat="server" GroupName="type" ResourceString="forum.settings.typechoose" />
                    </div>
                    <div>
                        <cms:LocalizedRadioButton ID="radTypeDiscussion" runat="server" GroupName="type"
                            ResourceString="forum.settings.typediscussion" />
                    </div>
                    <div>
                        <cms:LocalizedRadioButton ID="radTypeAnswer" runat="server" GroupName="type" ResourceString="forum.settings.typeanswer" />
                    </div>
                </td>
                <td>
                    <cms:LocalizedCheckBox runat="server" ID="chkInheritType" ResourceString="forum.settings.inheritfromgroup" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblIsAnswerLimit" EnableViewState="false"
                        ResourceString="forum.settings.isanswerlimit" DisplayColon="true" />
                </td>
                <td class="ForumEditControl">
                    <cms:CMSTextBox ID="txtIsAnswerLimit" runat="server" CssClass="TextBoxField" MaxLength="9" />
                </td>
                <td>
                    <cms:LocalizedCheckBox runat="server" ID="chkInheritIsAnswerLimit" ResourceString="forum.settings.inheritfromgroup" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblImageMaxSideSize" EnableViewState="false"
                        ResourceString="forum.settings.maxsidesize" DisplayColon="true" />
                </td>
                <td class="ForumEditControl">
                    <cms:CMSTextBox ID="txtImageMaxSideSize" runat="server" CssClass="TextBoxField" MaxLength="9" />
                </td>
                <td>
                    <cms:LocalizedCheckBox runat="server" ID="chkInheritMaxSideSize" ResourceString="forum.settings.inheritfromgroup" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblMaxAttachmentSize" EnableViewState="false"
                        ResourceString="forum.settings.maxattachmentsize" DisplayColon="true" />
                </td>
                <td class="ForumEditControl">
                    <cms:CMSTextBox ID="txtMaxAttachmentSize" runat="server" CssClass="TextBoxField"
                        MaxLength="9" />
                </td>
                <td>
                    <cms:LocalizedCheckBox runat="server" ID="chkInheritMaxAttachmentSize" ResourceString="forum.settings.inheritfromgroup" />
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcOnline" Visible="false">
                <tr>
                    <td colspan="3">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblLogActivity" EnableViewState="false" ResourceString="forum.settings.logactivity"
                            DisplayColon="true" />
                    </td>
                    <td class="ForumEditControl">
                        <asp:CheckBox ID="chkLogActivity" runat="server" CssClass="CheckBoxMovedLeft" />
                    </td>
                    <td>
                        <cms:LocalizedCheckBox runat="server" ID="chkInheritLogActivity" ResourceString="forum.settings.inheritfromgroup" />
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
                <td class="ForumEditControl">
                    <cms:ThreeStateCheckBox ID="chkAuthorEdit" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblAuthorDelete" EnableViewState="false" ResourceString="forum.settings.authordelete"
                        DisplayColon="true" />
                </td>
                <td class="ForumEditControl">
                    <cms:ThreeStateCheckBox ID="chkAuthorDelete" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="LeftColumnField">
                    <asp:Label runat="server" ID="lblCaptcha" CssClass="FieldLabel" EnableViewState="false" />
                </td>
                <td colspan="2">
                    <cms:ThreeStateCheckBox ID="chkCaptcha" runat="server" />
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
                        <asp:Label runat="server" ID="lblUseHTML" CssClass="FieldLabel" EnableViewState="false" />
                    </td>
                    <td class="ForumEditControl">
                        <cms:ThreeStateCheckBox ID="chkUseHTML" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
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
                <td>
                    <cms:LocalizedCheckBox runat="server" ID="chkInheritDiscussion" ResourceString="forum.settings.inheritfromgroup" />
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
                <td colspan="2">
                    <asp:CheckBox ID="chkEnableQuote" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnbaleCode" EnableViewState="false" ResourceString="forum.settings.enablecode"
                        DisplayColon="true" />
                </td>
                <td colspan="2">
                    <asp:CheckBox ID="chkEnableCode" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableBold" EnableViewState="false" ResourceString="forum.settings.enableBold"
                        DisplayColon="true" />
                </td>
                <td colspan="2">
                    <asp:CheckBox ID="chkEnableBold" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableItalic" EnableViewState="false" ResourceString="forum.settings.enableItalic"
                        DisplayColon="true" />
                </td>
                <td colspan="2">
                    <asp:CheckBox ID="chkEnableItalic" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableUnderline" EnableViewState="false"
                        ResourceString="forum.settings.enableUnderline" DisplayColon="true" />
                </td>
                <td colspan="2">
                    <asp:CheckBox ID="chkEnableUnderline" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableStrike" EnableViewState="false" ResourceString="forum.settings.enableStrike"
                        DisplayColon="true" />
                </td>
                <td colspan="2">
                    <asp:CheckBox ID="chkEnableStrike" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblEnableColor" EnableViewState="false" ResourceString="forum.settings.enableColor"
                        DisplayColon="true" />
                </td>
                <td colspan="2">
                    <asp:CheckBox ID="chkEnableColor" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel runat="server" ID="pnlOptIn">
        <table>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblEnableOptIn" runat="server" EnableViewState="false" ResourceString="general.enableoptin"
                        DisplayColon="true" AssociatedControlID="chkEnableOptIn" />
                </td>
                <td colspan="2">
                    <cms:ThreeStateCheckBox ID="chkEnableOptIn" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblOptInURL" EnableViewState="false" ResourceString="general.optinurl"
                        DisplayColon="true" AssociatedControlID="txtOptInURL" />
                </td>
                <td class="ForumEditControl">
                    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always" RenderMode="Inline">
                        <ContentTemplate>
                            <cms:PathSelector ID="txtOptInURL" runat="server" CssClass="Inline" MaxLength="450" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </td>
                <td>
                    <cms:LocalizedCheckBox runat="server" ID="chkInheritOptInURL" ResourceString="forum.settings.inheritfromgroup" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblSendOptInConfirmation" runat="server" EnableViewState="false"
                        ResourceString="general.sendoptinconfirmation" DisplayColon="true" AssociatedControlID="chkSendOptInConfirmation" />
                </td>
                <td colspan="2">
                    <cms:ThreeStateCheckBox ID="chkSendOptInConfirmation" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" CssClass="SubmitButton"
                        EnableViewState="false" ResourceString="General.OK" />
                </td>
                <td>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
<asp:Literal runat="server" ID="ltrScript" EnableViewState="false" />