<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TranslationServiceSelector.ascx.cs"
    Inherits="CMSModules_Translations_Controls_TranslationServiceSelector" %>
<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Translations/FormControls/TranslationPriority.ascx"
    TagName="TranslationPriority" TagPrefix="cms" %>
<div class="TranslationSelector">
    <asp:Literal runat="server" ID="ltlServices" EnableViewState="false" />
    <div class="TranslationSelectorSettings">
        <table>
            <tr>
                <td>
                    <cms:LocalizedLabel runat="server" ID="lblTranslateFromLang" ResourceString="translations.fromlanguage"
                        EnableViewState="false" DisplayColon="true" />
                </td>
                <td>
                    <cms:SiteCultureSelector runat="server" ID="selectCultureElem" AddDefaultRecord="false" />
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcTargetLang" Visible="false">
                <tr>
                    <td>
                        <cms:LocalizedLabel runat="server" ID="lblTranslateToLang" ResourceString="translations.tolanguage"
                            EnableViewState="false" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:SiteCultureSelector runat="server" ID="selectTargetCultureElem" AddDefaultRecord="false" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr id="pnlProcessBinary">
                <td>
                    <cms:LocalizedLabel runat="server" ID="lblProcessBinary" ResourceString="translations.processbinary"
                        EnableViewState="false" DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkProcessBinary" Checked="false" />
                </td>
            </tr>
            <tr id="pnlPriority">
                <td>
                    <cms:LocalizedLabel runat="server" ID="lblPriority" ResourceString="translations.priority"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:TranslationPriority runat="server" ID="drpPriority" />
                </td>
            </tr>
            <tr id="pnlDeadline">
                <td>
                    <cms:LocalizedLabel runat="server" ID="lblDeadline" ResourceString="translations.deadline"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:DateTimePicker runat="server" ID="dtDeadline" AllowEmptyValue="true" DisplayNA="true"
                        DisplayNow="false" />
                </td>
            </tr>
            <tr id="pnlInstructions">
                <td style="vertical-align: top;">
                    <cms:LocalizedLabel runat="server" ID="lblInstructions" ResourceString="translations.instructions"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox runat="server" ID="txtInstruction" TextMode="MultiLine" CssClass="TextAreaField" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="hdnSelectedName" />
    <asp:HiddenField runat="server" ID="hdnGenerateTarget" />
</div>
