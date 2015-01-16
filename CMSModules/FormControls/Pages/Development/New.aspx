<%@ Page Language="C#" AutoEventWireup="true" CodeFile="New.aspx.cs" Inherits="CMSModules_FormControls_Pages_Development_New"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Form Controls - New form control"
    Theme="Default" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagPrefix="cms"
    TagName="FileSystemSelector" %>
<%@ Register Src="~/CMSFormControls/System/UserControlTypeSelector.ascx" TagPrefix="cms"
    TagName="TypeSelector" %>
<%@ Register Src="~/CMSModules/FormControls/FormControls/FormControlSelector.ascx" TagName="FormControlSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcSiteSelector" runat="server"
    Visible="true">
    <br />
    <cms:LocalizedRadioButton runat="server" ID="radNewControl" ResourceString="developmentformcontroleditnew"
        GroupName="fcSelect" Checked="true" OnCheckedChanged="radNewFormControl_CheckedChanged"
        AutoPostBack="true" />
    <br />
    <cms:LocalizedRadioButton runat="server" ID="radInheritedControl" GroupName="fcSelect" ResourceString="developmentformcontrolinherited"
        OnCheckedChanged="radNewFormControl_CheckedChanged" AutoPostBack="true" />
    <br />
    <br />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <table style="vertical-align: top">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblDisplayName" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    ResourceString="general.displayname" DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtControlDisplayName" runat="server" CssClass="TextBoxField"
                    MaxLength="100" />
                <cms:CMSRequiredFieldValidator ID="rfvControlDisplayName" runat="server" ControlToValidate="txtControlDisplayName:textbox"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblCodeName" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    ResourceString="general.codename" DisplayColon="true" />
            </td>
            <td>
                <cms:CodeName ID="txtControlName" runat="server" CssClass="TextBoxField" MaxLength="100" />
                <cms:CMSRequiredFieldValidator ID="rfvControlName" runat="server" ControlToValidate="txtControlName"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblType" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    ResourceString="formcontrols.type" DisplayColon="true" />
            </td>
            <td>
                <cms:TypeSelector ID="drpTypeSelector" IncludeAllItem="false" runat="server" CssClass="DropDownField" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcFileName" runat="server">
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblFileName" runat="server" CssClass="ContentLabel" EnableViewState="false"
                        ResourceString="general.filename" DisplayColon="true" />
                </td>
                <td>
                    <cms:FileSystemSelector ID="tbFileName" runat="server" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcFormControls" runat="server" Visible="false">
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblInheritControl" runat="server" CssClass="ContentLabel" EnableViewState="false"
                        ResourceString="developmentwebpartedit.inheritedwebpart" DisplayColon="true" />
                </td>
                <td>
                    <cms:FormControlSelector ID="drpFormControls" runat="server" ShowInheritedControls="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
            </td>
            <td>
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    CssClass="SubmitButton" ResourceString="general.ok" />
            </td>
        </tr>
    </table>
</asp:Content>
