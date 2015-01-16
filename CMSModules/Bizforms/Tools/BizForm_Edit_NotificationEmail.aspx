<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_BizForms_Tools_BizForm_Edit_NotificationEmail"
    Theme="Default" ValidateRequest="false" CodeFile="BizForm_Edit_NotificationEmail.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Content ID="aS" runat="server" ContentPlaceHolderID="plcActions">
    <cms:HeaderActions ID="menu" ShortID="m" runat="server" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="plcContent" runat="server" ContentPlaceHolderID="plcContent">
    <asp:CheckBox ID="chkSendToEmail" runat="server" AutoPostBack="true" OnCheckedChanged="chkSendToEmail_CheckedChanged" /><br />
    <br />
    <table>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblFromEmail" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.lblFromEmail" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtFromEmail" runat="server" MaxLength="200" CssClass="TextBoxField" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblToEmail" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.ToEmail" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtToEmail" runat="server" MaxLength="200" CssClass="TextBoxField" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSubject" runat="server" EnableViewState="False" ResourceString="general.subject"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="250" CssClass="TextBoxField" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <asp:CheckBox ID="chkAttachDocs" runat="server" AutoPostBack="false" />
            </td>
        </tr>
    </table>
    <div>
        <asp:CheckBox ID="chkCustomLayout" runat="server" AutoPostBack="true" OnCheckedChanged="chkCustomLayout_CheckedChanged" />
    </div>
    <asp:Panel ID="pnlCustomLayout" runat="server">
        <table cellpadding="0" cellspacing="0" border="0">
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
                    <asp:ListBox ID="lstAvailableFields" runat="server" CssClass="FieldsList" Height="230" />
                </td>
            </tr>
            <tr>
                <td style="vertical-align: bottom;" class="RightColumn">
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
        </table>
    </asp:Panel>
    <asp:Literal ID="ltlConfirmDelete" runat="server" />
</asp:Content>
