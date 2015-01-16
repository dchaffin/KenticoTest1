<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_Subscriptions_SubscriptionEdit" CodeFile="SubscriptionEdit.ascx.cs" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<br />
<table style="vertical-align: top">
    <tr>
        <td style="font-weight:bold;">
            <cms:LocalizedLabel runat="server" ID="lblSubscriptionEmail" EnableViewState="false"
                ResourceString="general.email" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtSubscriptionEmail" runat="server"  CssClass="TextBoxField" MaxLength="100" />
            <cms:CMSRequiredFieldValidator ID="rfvSubscriptionEmail" runat="server" ErrorMessage=""
                ControlToValidate="txtSubscriptionEmail" Display="Dynamic"></cms:CMSRequiredFieldValidator>
            <cms:CMSRegularExpressionValidator ID="rfvEmail" runat="server" ControlToValidate="txtSubscriptionEmail"
                Display="Dynamic"></cms:CMSRegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                CssClass="SubmitButton" /></td>
    </tr>
</table>
<asp:Panel runat="server" ID="pnlSendConfirmationEmail" Visible="true">
    <div style="padding: 10px;">
    </div>
    <asp:CheckBox runat="server" ID="chkSendConfirmationEmail" /><cms:LocalizedLabel
        runat="server" ID="lblSendConfirmation" CssClass="ContentLabel" ResourceString="forums.forumsubscription.sendemail"></cms:LocalizedLabel>
</asp:Panel>
