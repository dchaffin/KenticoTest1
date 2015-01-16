<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Widgets_Controls_WidgetCategoryEdit" CodeFile="WidgetCategoryEdit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<table>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel runat="server" ID="lblDisplayName" EnableViewState="false" ResourceString="general.displayname"
                DisplayColon="true" AssociatedControlID="txtDisplayName" />
        </td>
        <td>
            <cms:LocalizableTextBox ID="txtDisplayName" runat="server" CssClass="TextBoxField" MaxLength="100" />
            <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName:textbox"
                Display="Dynamic" ValidationGroup="Required" />
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="plcCodeName">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblCodeName" EnableViewState="false" ResourceString="general.codename"
                    DisplayColon="true" AssociatedControlID="txtCodeName" />
            </td>
            <td>
                <cms:CodeName ID="txtCodeName" runat="server" CssClass="TextBoxField" MaxLength="100" />
                <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" ControlToValidate="txtCodeName"
                    Display="Dynamic" ValidationGroup="Required" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel runat="server" ID="lblImagePath" EnableViewState="false" ResourceString="widgets.cat.imagepath"
                DisplayColon="true" AssociatedControlID="txtImagePath" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtImagePath" runat="server" CssClass="TextBoxField" MaxLength="400" />
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                ResourceString="general.ok" CssClass="SubmitButton" ValidationGroup="Required" />
        </td>
    </tr>
</table>
