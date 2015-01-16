<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Controls_Membership_Edit"
    CodeFile="Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<table style="vertical-align: top">
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblMembershipName" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="membership.membershipname" AssociatedControlID="txtMembershipName" />
        </td>
        <td>
            <cms:LocalizableTextBox ID="txtMembershipName" runat="server" CssClass="TextBoxField" MaxLength="200"
                EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvMembershipName" runat="server" Display="Dynamic"
                ControlToValidate="txtMembershipName:textbox" ValidationGroup="vgMembership" EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblMembershipCodeName" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="membership.membershipcodename" AssociatedControlID="txtMembershipName" />
        </td>
        <td>
            <cms:CodeName ID="txtMembershipCodeName" runat="server" CssClass="TextBoxField" MaxLength="200"
                EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvMembershipCodeName" runat="server" Display="Dynamic"
                ControlToValidate="txtMembershipCodeName" ValidationGroup="vgMembership" EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblMembershipDescription" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="membership.description" AssociatedControlID="txtMembershipDescription" />
        </td>
        <td>
            <cms:LocalizableTextBox ID="txtMembershipDescription" runat="server" TextMode="MultiLine" CssClass="TextAreaField"
                EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false" CssClass="SubmitButton"
                OnClick="btnOk_Click" ValidationGroup="vgMembership" ResourceString="general.ok" />
        </td>
    </tr>
</table>
