<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CategoryEdit.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_CategoryEdit" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="ConditionBuilder" Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" %>
<asp:Panel ID="pnlCategory" runat="server" CssClass="FieldPanel">
    <table>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblCategoryName" runat="server" ShortID="cn" EnableViewState="false"
                    ResourceString="TemplateDesigner.CategoryName" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtCategoryName" runat="server" CssClass="TextBoxField" />
            </td>
        </tr>
        <cms:CMSUpdatePanel ID="pnlCollapsible" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblCollapsible" ShortID="ec" runat="server" EnableViewState="false"
                            ResourceString="categories.collapsible" DisplayColon="True" />
                    </td>
                    <td>
                        <cms:LocalizedCheckBox runat="server" ID="chkCollapsible" ShortID="cec" CssClass="CheckBoxMovedLeft"
                            OnCheckedChanged="chkCollapsible_Changed" AutoPostBack="True" />
                    </td>
                </tr>
                <asp:Panel runat="server" ID="pnlCollapsibleByDefault">
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel ID="lblCollapsedByDefault" ShortID="cbd" runat="server" EnableViewState="false"
                                ResourceString="categories.collapsedbydefault" DisplayColon="True" />
                        </td>
                        <td>
                            <cms:LocalizedCheckBox runat="server" ID="chkCollapsedByDefault" ShortID="ccbd" CssClass="CheckBoxMovedLeft" />
                        </td>
                    </tr>
                </asp:Panel>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lvlVisibility" runat="server" EnableViewState="false" ResourceString="formengine.visibilitylabel"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizedCheckBox ID="chkVisibility" runat="server" AutoPostBack="True" ShortID="ccv"
                    CssClass="CheckBoxMovedLeft" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblVisibleMacro" runat="server" EnableViewState="false" ResourceString="fieldeditor.visibilitymacro"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:ConditionBuilder LeftOffset="240" TopOffset="40" MaxWidth="550" ID="visibleMacro" runat="server" />
            </td>
        </tr>
    </table>
</asp:Panel>
