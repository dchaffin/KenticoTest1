<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Templates_NewsletterTemplate_Edit"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" ValidateRequest="false"
    EnableEventValidation="false" CodeFile="NewsletterTemplate_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroSelector.ascx" TagName="MacroSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagName="File"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <table style="vertical-align: top" cellspacing="5">
        <tr>
            <td class="FieldLabel" style="width: auto;">
                <cms:LocalizedLabel runat="server" ID="lblTemplateDisplayName" EnableViewState="false"
                    ResourceString="general.displayname" DisplayColon="true" AssociatedControlID="txtTemplateDisplayName" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtTemplateDisplayName" runat="server" CssClass="TextBoxField"
                    MaxLength="250" />
                <cms:CMSRequiredFieldValidator ID="rfvTemplateDisplayName" runat="server" ControlToValidate="txtTemplateDisplayName:textbox"
                    Display="dynamic" EnableViewState="false"></cms:CMSRequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblTemplateName" EnableViewState="false" ResourceString="general.codename"
                    DisplayColon="true" AssociatedControlID="txtTemplateName" />
            </td>
            <td>
                <cms:CodeName ID="txtTemplateName" runat="server" CssClass="TextBoxField" MaxLength="250" />
                <cms:CMSRequiredFieldValidator ID="rfvTemplateName" runat="server" ControlToValidate="txtTemplateName"
                    Display="dynamic" EnableViewState="false"></cms:CMSRequiredFieldValidator>
            </td>
        </tr>
        <asp:PlaceHolder runat="server" ID="plcThumb" EnableViewState="false" Visible="false">
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblThumb" ResourceString="general.thumbnail"
                        DisplayColon="true" EnableViewState="false" AssociatedControlID="ucThumbnail" />
                </td>
                <td>
                    <cms:File ID="ucThumbnail" runat="server" Visible="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcSubject" EnableViewState="false" Visible="false">
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblTemplateSubject" ResourceString="general.subject"
                        DisplayColon="true" EnableViewState="false" AssociatedControlID="txtTemplateSubject" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtTemplateSubject" runat="server" CssClass="TextBoxField" MaxLength="250" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblTemplateHeader" runat="server" Text="Label" ResourceString="general.htmlheader"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="txtTemplateHeader" />
            </td>
            <td>
                <cms:LargeTextArea ID="txtTemplateHeader" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel" style="vertical-align: top; padding-top: 5px;">
                <cms:LocalizedLabel ID="lblTemplateBody" runat="server" ResourceString="general.body"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="htmlTemplateBody" />
            </td>
            <td class="EditingFormControl" style="width: auto;">
                <cms:CMSHtmlEditor ID="htmlTemplateBody" runat="server" Width="770px" Height="400px" />
                <asp:PlaceHolder ID="pnlEditableRegion" runat="server" Visible="false">
                    <table cellspacing="0" cellspacing="0">
                        <tr>
                            <td colspan="3">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cms:LocalizedLabel ID="lblInsertEditableRegion" runat="server" EnableViewState="false"
                                    ResourceString="NewsletterTemplate_Edit.TemplateInsertEditRegLabel" AssociatedControlID="txtName" />
                            </td>
                            <td>
                                <cms:LocalizedLabel ID="lblWidth" runat="server" EnableViewState="false" ResourceString="NewsletterTemplate_Edit.TemplateEditRegWidthLabel"
                                    AssociatedControlID="txtWidth" />
                            </td>
                            <td>
                                <cms:LocalizedLabel ID="lblHeight" runat="server" EnableViewState="false" ResourceString="NewsletterTemplate_Edit.TemplateEditRegHeightLabel"
                                    AssociatedControlID="txtHeight" />
                            </td>
                        </tr>
                        <tr>
                            <td style="vertical-align: top;">
                                <cms:CMSTextBox ID="txtName" runat="server" />
                            </td>
                            <td style="vertical-align: top;">
                                <cms:CMSTextBox ID="txtWidth" runat="server" CssClass="ShortTextBox" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtHeight" runat="server" CssClass="ShortTextBox" />&nbsp;<cms:LocalizedButton
                                    ID="btnInsertEditableRegion" runat="server" CssClass="ContentButton" OnClientClick="InsertEditableRegion(); return false;"
                                    ResourceString="NewsletterTemplate_Edit.Insert" />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblTemplateFooter" runat="server" ResourceString="general.htmlfooter"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="txtTemplateFooter" />
            </td>
            <td>
                <cms:LargeTextArea ID="txtTemplateFooter" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="FieldLabel" style="vertical-align: top; padding-top: 5px;">
                <cms:LocalizedLabel ID="lblTemplateStyleSheetText" runat="server" EnableViewState="false"
                    ResourceString="NewsletterTemplate_Edit.TemplateStylesheetTextLabel" AssociatedControlID="txtTemplateStyleSheetText" />
            </td>
            <td>
                <cms:ExtendedTextArea ID="txtTemplateStyleSheetText" runat="server" EnableViewState="false"
                    EditorMode="Advanced" Language="CSS" Height="200px" Width="770px" /><br />
            </td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
        </tr>
    </table>
</asp:Content>
