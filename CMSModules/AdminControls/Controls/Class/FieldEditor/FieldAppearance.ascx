<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FieldAppearance.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_FieldAppearance" %>
<%@ Register Src="~/CMSFormControls/System/UserControlTypeSelector.ascx" TagPrefix="cms"
    TagName="TypeSelector" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/FormControls/FormControls/FormControlSelector.ascx"
    TagPrefix="cms" TagName="FormControlSelector" %>
<asp:Panel ID="pnlAppearance" runat="server" Enabled="true" CssClass="FieldPanel">
    <table>
        <asp:PlaceHolder ID="plcPublicField" runat="server">
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblPublicField" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.PublicField" />
                </td>
                <td>
                    <asp:CheckBox ID="chkPublicField" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcVisibility" runat="server" Visible="false">
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblVisibility" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.Visibility" />
                </td>
                <td>
                    <cms:VisibilityControl ID="ctrlVisibility" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblVisibilityControl" runat="server" EnableViewState="false"
                        ResourceString="TemplateDesigner.VisibilityControl" />
                </td>
                <td>
                    <asp:DropDownList ID="drpVisibilityControl" runat="server" DataValueField="UserControlCodeName"
                        DataTextField="UserControlDisplayName" CssClass="DropDownField" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblChangeVisibility" runat="server" EnableViewState="false"
                        ResourceString="TemplateDesigner.ChangeVisibility" />
                </td>
                <td>
                    <asp:CheckBox ID="chkChangeVisibility" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblFieldCaption" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.FieldCaption" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtFieldCaption" runat="server" CssClass="TextBoxField" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblField" runat="server" EnableViewState="false" ResourceString="objecttype.cms_formusercontrol"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:FormControlSelector ID="drpControl" runat="server" AutoPostBack="True" PrioritizeItems="true" ForcedValueSet="true" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblFieldDescription" runat="server" EnableViewState="false"
                    ResourceString="TemplateDesigner.FieldDescription" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtDescription" runat="server" CssClass="TextAreaField"
                    TextMode="MultiLine" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcInheritance" runat="server" Visible="false">
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblControlInheritable" runat="server" DisplayColon="true"
                        EnableViewState="false" ResourceString="fieldeditor.allowinheritance" />
                </td>
                <td>
                    <asp:CheckBox ID="chkControlInheritable" runat="server" CssClass="CheckBoxMovedLeft" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</asp:Panel>
