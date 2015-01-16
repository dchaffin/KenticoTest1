<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_BizForms_Tools_BizForm_Edit_Autoresponder"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    CodeFile="BizForm_Edit_Autoresponder.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>

<asp:Content ID="aS" runat="server" ContentPlaceHolderID="plcActions">
    <cms:HeaderActions ID="menu" ShortID="m" runat="server" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="plcContent" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlUsers" runat="server" CssClass="TabsPageContent">
        <table>
            <tr>
                <td style="padding-right: 5px;">
                    <cms:LocalizedLabel ID="lblEmailField" runat="server" EnableViewState="False" ResourceString="bizform_edit_autoresponder.lblemailfield" />
                </td>
                <td>
                    <asp:DropDownList ID="drpEmailField" runat="server" CssClass="DropDownField" AutoPostBack="true" />
                </td>
            </tr>
            <asp:PlaceHolder ID="pnlCustomLayout" runat="server">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblEmailFrom" runat="server" EnableViewState="False" ResourceString="bizform_edit_autoresponder.lblEmailFrom" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtEmailFrom" runat="server" MaxLength="200" CssClass="TextBoxField" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblEmailSubject" runat="server" EnableViewState="False" ResourceString="general.subject"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtEmailSubject" runat="server" MaxLength="200" CssClass="TextBoxField" />
                    </td>
                </tr>
        </table>
        <br />
        <table>
            <tr>
                <td colspan="2" class="GenerateButtonPadding">
                    <cms:LocalizedButton ID="btnGenerateLayout" runat="server" OnClientClick="SetContent(GenerateTableLayout()); return false;"
                        CssClass="XLongButton" ResourceString="Bizform_Edit_Autoresponder.btnGenerateLayout" />
                </td>
            </tr>
            <tr>
                <td rowspan="2">
                    <cms:CMSHtmlEditor ID="htmlEditor" runat="server" Width="550px" Height="300px" />
                </td>
                <td style="vertical-align: top; padding-left: 7px;" class="RightColumn">
                    <cms:LocalizedLabel ID="lblAvailableFields" runat="server" EnableViewState="false"
                        CssClass="AvailableFieldsTitle" ResourceString="Bizform_Edit_Autoresponder.AvailableFields" />
                    <asp:ListBox ID="lstAvailableFields" runat="server" CssClass="FieldsList" Width="152px" />
                </td>
            </tr>
            <tr>
                <td style="vertical-align: bottom; padding-bottom: 3px;" class="RightColumn">
                    <table cellspacing="0" cellpadding="1">
                        <tr>
                            <td>
                                <cms:LocalizedButton ID="btnInsertLabel" runat="server" CssClass="LongButton" ResourceString="Bizform_Edit_Autoresponder.btnInsertLabel" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cms:LocalizedButton ID="btnInsertInput" runat="server" CssClass="LongButton" ResourceString="Bizform_Edit_Autoresponder.btnInsertInput" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            </asp:PlaceHolder>
        </table>
        <asp:Literal ID="ltlConfirmDelete" runat="server" />
    </asp:Panel>
</asp:Content>
