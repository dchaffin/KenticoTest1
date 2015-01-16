<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Controls_Boards_BoardEdit"
    CodeFile="BoardEdit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/ThreeStateCheckBox.ascx" TagName="ThreeStateCheckBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/selectsinglepath.ascx"
    TagName="PathSelector" TagPrefix="cms" %>
<table class="GroupEditTable">
    <tr>
        <td colspan="2">
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <asp:Label ID="lblBoardOwner" runat="server" EnableViewState="false" CssClass="FieldLabel" />
        </td>
        <td>
            <asp:Label ID="lblBoardOwnerText" runat="server" EnableViewState="true" CssClass="FieldLabel" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <asp:Label ID="lblBoardDisplayName" runat="server" EnableViewState="false" CssClass="FieldLabel" />
        </td>
        <td>
            <cms:LocalizableTextBox ID="txtBoardDisplayName" MaxLength="250" runat="server" EnableViewState="false"
                CssClass="TextBoxField" />
            <cms:CMSRequiredFieldValidator ID="rfvBoardDisplayName" ControlToValidate="txtBoardDisplayName:textbox"
                runat="server" Display="Dynamic" EnableViewState="false" />
        </td>
    </tr>
    <asp:PlaceHolder ID="plcCodeName" runat="Server">
        <tr>
            <td class="FieldLabel">
                <asp:Label ID="lblBoardCodeName" runat="server" EnableViewState="false" CssClass="FieldLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtBoardCodeName" MaxLength="250" runat="server" CssClass="TextBoxField"
                    EnableViewState="true" Enabled="false" />
                <cms:CMSRequiredFieldValidator ID="rfvBoardCodeName" ControlToValidate="txtBoardCodeName"
                    runat="server" Display="Dynamic" EnableViewState="false" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td class="FieldLabel">
            <asp:Label ID="lblBoardDescription" runat="server" EnableViewState="false" CssClass="FieldLabel" />
        </td>
        <td>
            <cms:LocalizableTextBox ID="txtBoardDescription" runat="server" TextMode="MultiLine"
                CssClass="TextAreaField" EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <asp:Label ID="lblBoardEnable" runat="server" EnableViewState="false" CssClass="FieldLabel" />
        </td>
        <td>
            <asp:CheckBox ID="chkBoardEnable" runat="server" CssClass="CheckBoxMovedLeft" EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <asp:Label ID="lblBoardOpen" runat="server" Text="Label" EnableViewState="false"
                CssClass="FieldLabel" />
        </td>
        <td>
            <asp:CheckBox ID="chkBoardOpen" runat="server" CssClass="CheckBoxMovedLeft" EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <asp:Label ID="lblBoardOpenFrom" runat="server" EnableViewState="false" CssClass="FieldLabel" />
        </td>
        <td>
            <cms:DateTimePicker ID="dtpBoardOpenFrom" runat="server" EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <asp:Label ID="lblBoardOpenTo" runat="server" EnableViewState="false" CssClass="FieldLabel" />
        </td>
        <td>
            <cms:DateTimePicker ID="dtpBoardOpenTo" runat="server" EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblSubscriptionsEnable" runat="server" DisplayColon="true"
                ResourceString="board.edit.enablesubscriptions" EnableViewState="false" />
        </td>
        <td>
            <asp:CheckBox ID="chkSubscriptionsEnable" runat="server" CssClass="CheckBoxMovedLeft"
                EnableViewState="false" />
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="plcUnsubscription">
        <tr>
            <td class="FieldLabel">
                <asp:Label ID="lblBaseUrl" runat="server" EnableViewState="false" CssClass="FieldLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtBaseUrl" runat="server" CssClass="TextBoxField" EnableViewState="false" />
                <cms:LocalizedCheckBox runat="server" ID="chkInheritBaseUrl" Checked="true" ResourceString="boards.inheritbaseurl"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label ID="lblUnsubscriptionUrl" runat="server" EnableViewState="false" CssClass="FieldLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtUnsubscriptionUrl" runat="server" CssClass="TextBoxField"
                    EnableViewState="false" />
                <cms:LocalizedCheckBox runat="server" ID="chkInheritUnsubUrl" Checked="true" ResourceString="boards.inheritbaseurl"
                    EnableViewState="false" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td class="FieldLabel">
            <asp:Label ID="lblBoardRequireEmail" runat="server" EnableViewState="false" CssClass="FieldLabel" />
        </td>
        <td>
            <asp:CheckBox ID="chkBoardRequireEmail" runat="server" CssClass="CheckBoxMovedLeft"
                EnableViewState="false" />
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="plcOnline" Visible="false">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblLogActivity" EnableViewState="false" ResourceString="board.edit.logactivity"
                    DisplayColon="true" />
            </td>
            <td colspan="2">
                <asp:CheckBox ID="chkLogActivity" runat="server" CssClass="CheckBoxMovedLeft" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr style="height: 20px;">
        <td colspan="2">
        </td>
    </tr>
    <tr>
        <td colspan="2" style="font-weight: bold">
            <cms:LocalizedLabel ID="lblOptIn" runat="server" EnableViewState="false" ResourceString="general.OptIn"
                DisplayColon="true" />
        </td>
    </tr>
    <tr style="height: 15px">
        <td colspan="2">
        </td>
    </tr>
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
            <cms:LocalizedCheckBox runat="server" ID="chkInheritOptInURL" ResourceString="boards.inheritbaseurl" />
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
            &nbsp;
        </td>
        <td>
            <cms:FormSubmitButton ID="btnOk" runat="server" EnableViewState="false" OnClick="btnOk_Click"
                CssClass="SubmitButton" />
        </td>
    </tr>
</table>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />