<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="CMSModules_Chat_Controls_UI_SupportOfflineMessage_Edit" %>

<table style="margin:10px">
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblSendersEmail" ResourceString="general.email" DisplayColon="true"></cms:LocalizedLabel>
        </td>
        <td>
             <asp:TextBox runat="server" ID="txtEmail" MaxLength="100" CssClass="TextBoxField"></asp:TextBox>
        </td>
        <td>
             <cms:LocalizedLabel runat="server" ID="lblEmailError" ResourceString="EmailInput.ValidationError" CssClass="ErrorLabel" ></cms:LocalizedLabel>
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblSubject" ResourceString="chat.support.offline.subject" DisplayColon="true"></cms:LocalizedLabel>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtSubject" MaxLength="100" CssClass="TextBoxField"></asp:TextBox>
        </td>
        <td>
             <asp:RequiredFieldValidator ID="rfvSubject" Display="Dynamic" runat="server" ControlToValidate="txtSubject" CssClass="ErrorLabel"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblMessage" ResourceString="chat.message" DisplayColon="true"></cms:LocalizedLabel>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtMessage" TextMode="MultiLine" CssClass="TextAreaField"></asp:TextBox>
        </td>
        <td>
             <asp:RequiredFieldValidator ID="rfvMessage" Display="Dynamic" runat="server" ControlToValidate="txtMessage" CssClass="ErrorLabel"></asp:RequiredFieldValidator>
        </td>
    </tr>
</table>
