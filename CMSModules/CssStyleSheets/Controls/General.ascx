<%@ Control Language="C#" AutoEventWireup="true" CodeFile="General.ascx.cs" Inherits="CMSModules_CssStyleSheets_Controls_General" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Locking/ObjectEditPanel.ascx" TagName="ObjectEditPanel"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:ObjectEditPanel runat="server" ID="editMenuElem" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:Panel ID="pnlContainer" runat="server" CssClass="PreviewDefaultContent">
    <div class="PageContent">
        <cms:UIForm runat="server" ID="EditForm" ObjectType="cms.cssstylesheet" DefaultFieldLayout="Inline">
            <LayoutTemplate>
                <table>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblDisplayName" ResourceString="CssStylesheet_General.DisplayName" />
                        </td>
                        <td>
                            <cms:FormField runat="server" ID="fDisplayName" Field="StylesheetDisplayName" FormControl="LocalizableTextBox" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblCodeName" ResourceString="CssStylesheet_General.Name" />
                        </td>
                        <td>
                            <cms:FormField runat="server" ID="fName" Field="StylesheetName" FormControl="CodeName" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="LocalizedLabel1" ResourceString="CssStylesheet_General.Text" />
                        </td>
                        <td class="MaxWidth">
                            <cms:FormField runat="server" ID="fCode" Field="StylesheetText" ResourceString="CssStylesheet_General.Text"
                                CssClass="FormPanel">
                                <cms:MacroEditor ID="txtCssStylesheetText" runat="server" />
                            </cms:FormField>
                        </td>
                    </tr>
                </table>
            </LayoutTemplate>
        </cms:UIForm>
    </div>
</asp:Panel>
<asp:HiddenField runat="server" ID="hdnClose" EnableViewState="false" />
