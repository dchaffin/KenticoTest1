<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Objects_Controls_CloneObject"
    CodeFile="CloneObject.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<script type="text/javascript">
        //<![CDATA[
    function ShowHideAdvancedSection() {
        var divAdvancedElem = document.getElementById('trAdvanced');
        var divSimpleElem = document.getElementById('trSimple');
        if ((divSimpleElem != null) && (divAdvancedElem != null)) {
            var style = divAdvancedElem.style.display;
            divAdvancedElem.style.display = divSimpleElem.style.display;
            divSimpleElem.style.display = style;

            if (divAdvancedElem.style.display == '') {
                jQuery('.trAdvancedItem').css("display", "");
            } else {
                jQuery('.trAdvancedItem').css("display", "none");
            }
        }
    }
//]]>
</script>
<fieldset>
    <legend>
        <%= GetString("general.general") %></legend>
    <table>
        <asp:PlaceHolder runat="server" ID="plcDisplayName" Visible="false">
            <tr>
                <td class="CloneParamTableCol">
                    <cms:LocalizedLabel runat="server" ID="lblDisplayName" EnableViewState="false" DisplayColon="true" />
                </td>
                <td>
                    <cms:LocalizableTextBox runat="server" ID="txtDisplayName" MaxLength="100" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcCodeName" Visible="false">
            <tr>
                <td class="CloneParamTableCol">
                    <cms:LocalizedLabel runat="server" ID="lblCodeName" EnableViewState="false" DisplayColon="true" />
                </td>
                <td>
                    <cms:CodeName runat="server" ID="txtCodeName" CssClass="TextBoxField" MaxLength="100" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr id="trSimple">
            <td colspan="2">
                <asp:Literal runat="server" EnableViewState="false" ID="ltlSpace" />
                <asp:Label runat="server" ID="lblShowAdvanced" EnableViewState="false" />
            </td>
        </tr>
        <tr id="trAdvanced" style="display: none;">
            <td colspan="2">
                <asp:Literal runat="server" EnableViewState="false" ID="ltlSpace2" />
                <asp:Label runat="server" ID="lblShowSimple" EnableViewState="false" />
                <br />
                <br />
            </td>
        </tr>
        <tr class="trAdvancedItem" style="display: none;">
            <td class="CloneParamTableCol">
                <cms:LocalizedLabel runat="server" ID="lblUseTransaction" ResourceString="clonning.settings.usetransaction"
                    EnableViewState="false" DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox runat="server" ID="chkUseTransaction" Checked="true" />
            </td>
        </tr>
        <tr class="trAdvancedItem" style="display: none;">
            <td class="CloneParamTableCol">
                <cms:LocalizedLabel runat="server" ID="lblKeepFieldsTranslated" ResourceString="clonning.settings.keepfieldstranslated"
                    EnableViewState="false" DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox runat="server" ID="chkKeepFieldsTranslated" Checked="false" />
            </td>
        </tr>
        <asp:PlaceHolder runat="server" ID="plcCloneUnderSite" Visible="false">
            <tr class="trAdvancedItem" style="display: none;">
                <td class="CloneParamTableCol">
                    <cms:LocalizedLabel runat="server" ID="lblCloneUnderSite" ResourceString="clonning.settings.cloneundersite"
                        EnableViewState="false" DisplayColon="true" />
                </td>
                <td>
                    <cms:SiteSelector runat="server" ID="siteElem" AllowAll="false" AllowEmpty="false"
                        AllowGlobal="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcChildren" Visible="false">
            <tr class="trAdvancedItem" style="display: none;">
                <td class="CloneParamTableCol">
                    <cms:LocalizedLabel runat="server" ID="lblChildren" EnableViewState="false" DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkChildren" Checked="true" />
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcChildrenLevel" Visible="true">
                <tr class="trAdvancedItem" style="display: none;">
                    <td class="CloneParamTableCol">
                        <cms:LocalizedLabel runat="server" ID="lblMaxRelativeLevel" ResourceString="clonning.settings.maxrelativelevel"
                            EnableViewState="false" DisplayColon="true" />
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="drpMaxRelativeLevel" CssClass="DropDownField" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcAssignToCurrentSite" Visible="false">
            <tr class="trAdvancedItem" style="display: none;">
                <td class="CloneParamTableCol">
                    <cms:LocalizedLabel runat="server" ID="lblAssignToCurrentSite" ResourceString="clonning.settings.assigntocurrentsite"
                        EnableViewState="false" DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkAssignToCurrentSite" Checked="true" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcSiteBindings" Visible="false">
            <tr class="trAdvancedItem" style="display: none;">
                <td class="CloneParamTableCol">
                    <cms:LocalizedLabel runat="server" ID="lblSiteBindings" ResourceString="clonning.settings.sitebindings"
                        EnableViewState="false" DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkSiteBindings" Checked="true" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcBindings" Visible="false">
            <tr class="trAdvancedItem" style="display: none;">
                <td class="CloneParamTableCol">
                    <cms:LocalizedLabel runat="server" ID="lblBindings" EnableViewState="false" DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkBindings" Checked="true" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcMetafiles" Visible="false">
            <tr class="trAdvancedItem" style="display: none;">
                <td class="CloneParamTableCol">
                    <cms:LocalizedLabel runat="server" ID="lblMetafiles" ResourceString="clonning.settings.metafiles"
                        EnableViewState="false" DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkMetafiles" Checked="true" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</fieldset>
<asp:PlaceHolder runat="server" ID="plcCustomParametersBox" Visible="false">
    <br />
    <fieldset>
        <legend>
            <%= GetCustomParametersTitle() %></legend>
        <asp:PlaceHolder runat="server" ID="plcCustomParameters" />
    </fieldset>
</asp:PlaceHolder>
