<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Categories_Controls_CategoryEdit"
    CodeFile="CategoryEdit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Categories/Controls/SelectCategory.ascx" TagName="SelectCategory"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>

<cms:HeaderActions ID="headerActions" runat="server" PanelCssClass="CMSEditMenu" />
<asp:Panel ID="pnlEdit" runat="server" DefaultButton="btnSaveCategory">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <table>
        <asp:PlaceHolder ID="plcCategorySite" runat="server">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblCategorySite" runat="server" CssClass="FieldLabel" ResourceString="category_edit.categorysite"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <asp:RadioButton runat="server" ID="radSite" GroupName="radSiteGlobal" Checked="true" /><asp:RadioButton
                        runat="server" ID="radGlobal" GroupName="radSiteGlobal" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblDisplayName" runat="server" CssClass="FieldLabel" ResourceString="general.displayname"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtDisplayName" runat="server" MaxLength="250" CssClass="TextBoxField" />
                &nbsp;<cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName$textbox"
                    Display="Dynamic" ValidationGroup="valGroupCategories" EnableViewState="false" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcCodeName" runat="server">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblCodeName" runat="server" CssClass="FieldLabel" ResourceString="general.codename"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:CodeName ID="txtCodeName" runat="server" CssClass="TextBoxField" MaxLength="250" />&nbsp;<cms:CMSRequiredFieldValidator
                        ID="rfvCodeName" runat="server" ControlToValidate="txtCodeName" Display="Dynamic"
                        ValidationGroup="valGroupCategories" EnableViewState="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcParentCategory" runat="server">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblParentCategory" runat="server" CssClass="FieldLabel" ResourceString="category_edit.parentcategory"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:SelectCategory ID="selectParentCategory" runat="server" AddNoneRecord="true" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td style="vertical-align: top">
                <cms:LocalizedLabel ID="lblDescription" runat="server" CssClass="FieldLabel" ResourceString="general.description"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="TextAreaField"
                    EnableViewState="false" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcEnabled" runat="server">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblEnabled" runat="server" CssClass="FieldLabel" ResourceString="general.enabled"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <asp:CheckBox ID="chkEnabled" runat="server" Checked="true" EnableViewState="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                <cms:FormSubmitButton ID="btnSaveCategory" runat="server" OnClick="btnSaveCategory_Click"
                    ValidationGroup="valGroupCategories" EnableViewState="false" />
            </td>
        </tr>
    </table>
</asp:Panel>
