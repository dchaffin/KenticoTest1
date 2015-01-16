<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Macros_ConditionBuilder"
    CodeFile="ConditionBuilder.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="0" cellspacing="0" class="Table100">
            <tr>
                <td class="Width100">
                    <cms:CMSPanel ID="pnlConditionBuilder" ShortID="pC" runat="server" CssClass="ConditionBuilder">
                        <cms:MacroEditor runat="server" ID="txtMacro" />
                        <asp:Panel runat="server" ID="pnlRule" Visible="false" CssClass="ConditionBuilderRule">
                            <asp:Literal runat="server" ID="ltlMacro" />
                        </asp:Panel>
                    </cms:CMSPanel>
                </td>
                <td valign="top" class="NoWrap">
                    <asp:Panel ID="pnlButtons" runat="server" CssClass="LocalizablePanel">
                        <cms:CMSImageButton ID="btnEdit" runat="server" CausesValidation="false" CssClass="LocalizableIcon"
                            EnableViewState="false" /><cms:CMSImageButton ID="btnClear" runat="server" CausesValidation="false"
                                CssClass="LocalizableIconLast" EnableViewState="false" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <asp:Button runat="server" ID="btnRefresh" CssClass="HiddenButton" />
        <asp:HiddenField runat="server" ID="hdnValue" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
