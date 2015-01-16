<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MappingEditor.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_SalesForce_MappingEditor" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" %>
<%@ Register TagPrefix="cms" TagName="SalesForceError" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/Error.ascx" %>
<%@ Import Namespace="CMS.SalesForce" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register TagPrefix="cms" TagName="Mapping" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/Mapping.ascx" %>
<%@ Register TagPrefix="cms" TagName="MappingEditorItem" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/MappingEditorItem.ascx" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server" EnableViewState="false">
    <asp:HiddenField ID="MappingHiddenField" runat="server" EnableViewState="false" />
    <asp:Panel ID="MappingPanel" runat="server" EnableViewState="false" CssClass="Hidden">
        <cms:Mapping ID="MappingControl" runat="server" EnableViewState="false" />
    </asp:Panel>
    <asp:Panel ID="MainPanel" runat="server" CssClass="PageContent" EnableViewState="false">
        <cms:SalesForceError ID="SalesForceError" runat="server" EnableViewState="false" MessagesEnabled="true" />
        <table>
            <tbody>
                <tr>
                    <td colspan="3"><h2><%= HTMLHelper.HTMLEncode(GetString("sf.mapping.replicationattributes"))%></h2></td>
                </tr>
                <tr>
                    <td style="white-space:nowrap;padding-right:2em"><%= HTMLHelper.HTMLEncode(GetString("sf.mapping.externalidentifierattribute"))%>:</td>
                    <td colspan="2" style="width:100%">
                        <asp:DropDownList ID="ExternalAttributeDropDownListEx" runat="server" EnableViewState="false"></asp:DropDownList>
                        <span id="HelpLinkContainer" style="padding-left:1em">
                            <img id="HelpImage" runat="server" alt="Help" style="vertical-align:middle" />
                            &nbsp;
                            <a href="#" id="HelpLink"><%= HTMLHelper.HTMLEncode(GetString("sf.mapping.instructions1.help"))%></a>
                        </span>
                    </td>
                </tr>
                <tr class="help" style="display:none">
                    <td colspan="3">
                        <p><%= GetString("sf.mapping.instructions1") %></p>
                        <ul>
                        <% foreach (string instruction in GetString("sf.mapping.instructions2").Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => !String.IsNullOrEmpty(x))) { %>
                        <li><%= instruction %></li>
                        <% } %>
                        </ul>
                        <p><%= GetString("sf.mapping.instructions3") %></p>
                        <ul>
                        <% foreach (string instruction in GetString("sf.mapping.instructions4").Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => !String.IsNullOrEmpty(x))) { %>
                        <li><%= instruction %></li>
                        <% } %>
                        </ul>
                        <p><%= GetString("sf.mapping.instructions5") %></p>
                    </td>
                </tr>
                <tr>
                    <td colspan="3"><h2><%= HTMLHelper.HTMLEncode(GetString("sf.mapping.requiredattributes"))%></h2></td>
                </tr>
                <asp:Repeater ID="RequiredAttributeRepeater" runat="server" EnableViewState="false">
                    <ItemTemplate>
                        <cms:MappingEditorItem ID="MappingEditorItemControl" runat="server" EnableViewState="false" />
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td colspan="4"><h2><%= HTMLHelper.HTMLEncode(GetString("sf.mapping.otherattributes"))%></h2></td>
                </tr>
                <asp:Repeater ID="OtherAttributeRepeater" runat="server" EnableViewState="false">
                    <ItemTemplate>
                        <cms:MappingEditorItem ID="MappingEditorItemControl" runat="server" EnableViewState="false" />
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </asp:Panel>
    <script type="text/javascript">

        jQuery(document).ready(function () {
            var mappingField = document.getElementById('<%= MappingHiddenField.ClientID %>');
            var sourceMappingField = wopener.document.getElementById('<%= SourceMappingHiddenFieldClientId %>');
            if (mappingField != null && sourceMappingField != null && mappingField.value != null && mappingField.value != '') {
                jQuery(sourceMappingField).val(mappingField.value);
                var panelElement = document.getElementById('<%= MappingPanel.ClientID %>');
                var sourcePanelElement = wopener.document.getElementById('<%= SourceMappingPanelClientId %>');
                if (panelElement != null && sourcePanelElement != null) {
                    var content = jQuery(panelElement).html();
                    jQuery(sourcePanelElement).html(content);
                }
                CloseDialog();
            }
            else {
                var fieldLabel = '<%= HTMLHelper.HTMLEncode(GetString("sf.sourcetype.field"))%>';
                var metaFieldLabel = '<%= HTMLHelper.HTMLEncode(GetString("sf.sourcetype.metafield"))%>';
                var picklistEntryLabel = '<%= HTMLHelper.HTMLEncode(GetString("sf.sourcetype.picklistentry"))%>';
                jQuery("select.SourceDropDownList").each(function (index, select) {
                    jQuery(select).find('option[value^="Field"]').wrapAll("<optgroup label='" + fieldLabel + "'>");
                    jQuery(select).find('option[value^="MetaField"]').wrapAll("<optgroup label='" + metaFieldLabel + "'>");
                    jQuery(select).find('option[value^="PicklistEntry"]').wrapAll("<optgroup label='" + picklistEntryLabel + "'>");

                    var html = jQuery(select).html();
                    jQuery(select).html(html);
                });

                var linkElement = jQuery("#HelpLink");
                var linkContainerElement = jQuery("#HelpLinkContainer");
                linkElement.click(function (e) {
                    linkContainerElement.hide();
                    jQuery(".help").show();
                    e.preventDefault();
                });
            }
        });

        jQuery(document).ready(function() {
            var displayWarnings = function(comboElement) {
                comboElement = jQuery(comboElement);
                comboElement.parent().parent().find("img").hide().filter(".Warning" + comboElement.val()).show();
            };
            var comboElements = jQuery("select.SourceDropDownList");
            comboElements.each(function(index, comboElement) {
                displayWarnings(comboElement);
            });
            comboElements.change(function(e) {
                displayWarnings(e.target);
            });
        });

    </script>
</asp:Content>
<asp:Content ID="FooterContent" ContentPlaceHolderID="plcFooter" runat="server" EnableViewState="false">
    <div class="FloatRight">
        <cms:LocalizedButton ID="ConfirmButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="general.ok" />
        <cms:LocalizedButton ID="CancelButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="general.cancel" OnClientClick="return CloseDialog()" />
    </div>
</asp:Content>
