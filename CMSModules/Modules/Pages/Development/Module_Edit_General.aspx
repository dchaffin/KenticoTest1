<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Development_Module_Edit_General"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Module Edit - General"
    CodeFile="Module_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent" EnableViewState="false">
    <table>
        <tr>
            <td class="FieldLabel">
                <asp:Label ID="lbModuleDisplayName" runat="server" EnableViewState="false" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="tbModuleDisplayName" runat="server" CssClass="TextBoxField"
                    MaxLength="100" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvModuleDisplayName" runat="server" EnableViewState="false"
                    ControlToValidate="tbModuleDisplayName:textbox" Display="dynamic" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label ID="lbModuleCodeName" runat="server" EnableViewState="false" />
            </td>
            <td>
                <cms:CodeName ID="tbModuleCodeName" runat="server" CssClass="TextBoxField" MaxLength="100"
                    EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvModuleCodeName" runat="server" EnableViewState="false"
                    ControlToValidate="tbModuleCodeName" Display="dynamic" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblModuleDescription" runat="server" EnableViewState="false" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtModuleDescription" runat="server" CssClass="TextAreaField"
                    TextMode="MultiLine" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOK_Click" EnableViewState="false" />
            </td>
        </tr>
    </table>
</asp:Content>
