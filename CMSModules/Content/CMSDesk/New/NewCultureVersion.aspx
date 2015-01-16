<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_New_NewCultureVersion"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Content - New culture version"
    CodeFile="NewCultureVersion.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Translations/Controls/TranslationServiceSelector.ascx"
    TagName="TranslationServiceSelector" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <asp:Label ID="lblInfo" runat="server" CssClass="ContentLabel" EnableViewState="false"
        Font-Bold="true" />
    <br />
    <br />
    <asp:Panel runat="server" ID="pnlNewVersion">
        <table>
            <tr>
                <td>
                    <asp:RadioButton ID="radEmpty" runat="server" GroupName="NewVersion" EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="radCopy" runat="server" GroupName="NewVersion" EnableViewState="false"
                        Checked="true" />
                </td>
            </tr>
            <tr id="divCultures">
                <td>
                    <div style="padding: 10px 10px 10px 20px;">
                        <fieldset>
                            <table>
                                <tr>
                                    <td>
                                        <cms:LocalizedLabel runat="server" ID="LocalizedLabel1" ResourceString="ContentNewCultureVersion.CopyFromLanguage"
                                            DisplayColon="true" EnableViewState="false" />
                                    </td>
                                    <td>
                                        <cms:SiteCultureSelector runat="server" ID="copyCulturesElem" AddDefaultRecord="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <cms:LocalizedLabel runat="server" ID="lblSaveBeforeEditing" ResourceString="ContentNewCultureVersion.SaveBeforeEditing"
                                            EnableViewState="false" DisplayColon="true" />
                                    </td>
                                    <td>
                                        <asp:CheckBox runat="server" ID="chkSaveBeforeEditing" Checked="true" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </div>
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcTranslationServices">
                <tr>
                    <td>
                        <asp:RadioButton ID="radTranslate" runat="server" GroupName="NewVersion" EnableViewState="false" />
                    </td>
                </tr>
                <tr id="divTranslations" style="display: none;">
                    <td>
                        <div style="padding: 10px 10px 10px 20px;">
                            <fieldset>
                                <cms:TranslationServiceSelector runat="server" ID="translationElem" DisplayOnlyServiceName="true" />
                            </fieldset>
                        </div>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divTranslate" style="display: none;">
                        <asp:Button runat="server" CssClass="LongSubmitButton" ID="btnTranslate" />
                    </div>
                    <div id="divCreate">
                        <asp:Button runat="server" CssClass="LongSubmitButton" ID="btnCreateDocument" />
                    </div>
                </td>
            </tr>
        </table>
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        <script type="text/javascript">
            //<![CDATA[
            var divCulturesElem = document.getElementById('divCultures');
            var divTranslationsElem = document.getElementById('divTranslations');
            var divTranslateElem = document.getElementById('divTranslate');
            var divCreateElem = document.getElementById('divCreate');

            // Displays the content based on current selection
            function ShowSelection() {
                if (radCopyElem.checked) {
                    divCulturesElem.style.display = "table-row";
                }
                else {
                    divCulturesElem.style.display = "none";
                }

                if ((divTranslationsElem != null) && (divTranslateElem != null)) {
                    if (radTranslations.checked) {
                        divTranslationsElem.style.display = "table-row";
                        divTranslateElem.style.display = '';
                        divCreateElem.style.display = 'none';
                    }
                    else {
                        divTranslationsElem.style.display = "none";
                        divTranslateElem.style.display = 'none';
                        divCreateElem.style.display = '';
                    }
                }
            }
            //]]>
        </script>
    </asp:Panel>
</asp:Content>
