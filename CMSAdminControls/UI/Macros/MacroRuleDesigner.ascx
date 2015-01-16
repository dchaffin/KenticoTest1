<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Macros_MacroRuleDesigner"
    CodeFile="MacroRuleDesigner.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlMain" DefaultButton="btnFilter">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <cms:MessagesPlaceHolder runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false" />
            <div class="MacroRuleDesigner">
                <table>
                    <tbody>
                        <tr>
                            <td style="width: 100%" valign="top">
                                <div class="MacroRuleRulesLabel">
                                    <cms:LocalizedLabel runat="server" ID="lblRuleAreaInfo" ResourceString="macros.macrorule.ruleifno"
                                        DisplayColon="true" />
                                </div>
                                <asp:Panel runat="server" ID="pnlButtons" CssClass="MacroRuleToolbar">
                                    <div class="MacroRuleToolbarButton">
                                        <asp:ImageButton runat="server" ID="btnDelete" AlternateText="Delete" /></div>
                                    <div class="MacroRuleToolbarButton">
                                        <asp:ImageButton runat="server" ID="btnClearAll" AlternateText="Clear all" /></div>
                                    <div class="MacroRuleToolbarSeparator">
                                        &nbsp;</div>
                                    <div class="MacroRuleToolbarButton">
                                        <asp:ImageButton runat="server" ID="btnUnindent" AlternateText="Unindent" /></div>
                                    <div class="MacroRuleToolbarButton">
                                        <asp:ImageButton runat="server" ID="btnIndent" AlternateText="Indent" /></div>
                                    <div class="MacroRuleToolbarSeparator">
                                        &nbsp;</div>
                                    <div class="MacroRuleToolbarButton">
                                        <asp:ImageButton runat="server" ID="btnAutoIndent" AlternateText="AutoIndent" /></div>
                                    <div class="MacroRuleToolbarSeparator">
                                        &nbsp;</div>
                                    <div class="MacroRuleToolbarButton">
                                        <asp:ImageButton runat="server" ID="btnViewCode" AlternateText="ViewCode" /></div>
                                </asp:Panel>
                                <div id="scrollDiv" class="MacroRuleAreaBorder">
                                    <asp:Panel runat="server" ID="pnlCondtion" CssClass="MacroRuleArea">
                                        <asp:Literal runat="server" ID="ltlText" EnableViewState="false" />
                                    </asp:Panel>
                                </div>
                            </td>
                            <td>
                                <asp:ImageButton runat="server" ID="imgAddClause" AlternateText="AutoIndent" />
                            </td>
                            <td valign="top">
                                <div style="width: 430px;">
                                    <asp:Panel runat="server" ID="pnlRules" CssClass="MacroRuleRules">
                                        <div class="MacroRuleRulesLabel">
                                            <cms:LocalizedLabel runat="server" ID="lblRules" ResourceString="macros.macrorule.ruleslist"
                                                DisplayColon="true" />
                                        </div>
                                        <div style="margin-top: 3px; margin-bottom: 8px;">
                                            <cms:LocalizedLabel runat="server" ID="lblFilter" ResourceString="macros.macrorule.searchrule"
                                                DisplayColon="true" />&nbsp;&nbsp;<cms:CMSTextBox runat="server" ID="txtFilter" CssClass="FilterTextBox" />&nbsp;&nbsp;<asp:Button
                                                    runat="server" ID="btnFilter" CssClass="ContentButton" />
                                        </div>
                                        <asp:ListBox runat="server" ID="lstRules" AutoPostBack="false" SelectionMode="Single"
                                            Width="100%" />
                                    </asp:Panel>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <cms:ModalPopupDialog ID="mdlDialog" runat="server" BackgroundCssClass="ModalBackground"
                    CssClass="ModalPopupDialog" Visible="false">
                    <asp:Panel runat="server" ID="pnlParameterPopup" Width="590px">
                        <div style="height: auto; min-height: 0px;">
                            <div class="PageHeader">
                                <cms:PageTitle ID="titleElem" runat="server" EnableViewState="false" GenerateFullWidth="true"
                                    SetWindowTitle="false" />
                            </div>
                        </div>
                        <asp:Panel runat="server" ID="pnlModalProperty" Visible="false" CssClass="DialogPageBody">
                            <div class="MacroRuleDialogBody">
                                <asp:Panel ID="pnlFormControl" runat="server" Width="550px">
                                    <cms:BasicForm runat="server" ID="formElem" Mode="Update" EnsureFirstLetterUpperCase="true" />
                                </asp:Panel>
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlFooter" Visible="false" CssClass="FloatRight PageFooterLine"
                            Width="570px">
                            <asp:Button runat="server" ID="btnSetParameter" CssClass="SubmitButton" />
                            <asp:Button runat="server" ID="btnCancel" CssClass="SubmitButton" />
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlViewCode" Visible="false" CssClass="DialogPageBody">
                            <div class="MacroRuleDialogBody">
                                <div style="width: 550px;">
                                    <cms:MacroEditor runat="server" ID="viewCodeElem" Width="100%" ReadOnly="true" />
                                </div>
                                <div class="FloatRight MacroRuleDialogButton">
                                    <asp:Button runat="server" ID="btnCodeOK" CssClass="SubmitButton" />
                                </div>
                            </div>
                        </asp:Panel>
                    </asp:Panel>
                </cms:ModalPopupDialog>
                <asp:HiddenField runat="server" ID="hdnScroll" EnableViewState="false" />
                <asp:HiddenField runat="server" ID="hdnSelected" EnableViewState="false" />
                <asp:HiddenField runat="server" ID="hdnParamSelected" EnableViewState="false" />
                <asp:HiddenField runat="server" ID="hdnLastParam" EnableViewState="false" />
                <asp:HiddenField runat="server" ID="hdnLastSelected" EnableViewState="false" />
                <asp:HiddenField runat="server" ID="hdnOpSelected" EnableViewState="false" />
                <asp:HiddenField runat="server" ID="hdnParam" EnableViewState="false" />
                <asp:HiddenField runat="server" ID="hdnParamEditShown" EnableViewState="false" />
                <asp:Button runat="server" ID="btnChangeOperator" CssClass="HiddenButton" />
                <asp:Button runat="server" ID="btnChangeParameter" CssClass="HiddenButton" />
                <asp:Button runat="server" ID="btnMove" CssClass="HiddenButton" />
                <asp:Button runat="server" ID="btnAddClause" CssClass="HiddenButton" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
