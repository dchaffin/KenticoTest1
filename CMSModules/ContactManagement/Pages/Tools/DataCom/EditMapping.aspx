<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" CodeFile="EditMapping.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_DataCom_EditMapping" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ContactMapping.ascx" TagName="ContactMapping" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/CompanyMapping.ascx" TagName="CompanyMapping" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:HiddenField ID="MappingHiddenField" runat="server" EnableViewState="false" />
    <asp:Panel ID="MappingPanel" runat="server" EnableViewState="false">
        <cms:ContactMapping ID="ContactMappingControl" runat="server"></cms:ContactMapping>
        <cms:CompanyMapping ID="CompanyMappingControl" runat="server"></cms:CompanyMapping>
    </asp:Panel>
    <asp:Panel runat="server" ID="MainPanel" EnableViewState="false" CssClass="PageContent">
        <cms:ErrorSummary ID="ErrorSummary" runat="server" EnableViewState="false" MessagesEnabled="true" />
        <table>
            <tr>
                <td style="padding-bottom:0.5em"><strong><cms:LocalizedLiteral runat="server" EnableViewState="false" ResourceString="datacom.kenticocms"></cms:LocalizedLiteral></strong></td>
                <td style="padding-bottom:0.5em"><strong><cms:LocalizedLiteral runat="server" EnableViewState="false" ResourceString="datacom.datacom"></cms:LocalizedLiteral></strong></td>
            </tr>
            <asp:Repeater ID="MappingItemRepeater" runat="server" OnItemDataBound="MappingItemRepeater_ItemDataBound">
                <ItemTemplate>
                    <tr>
                        <td><asp:Literal ID="FieldNameLiteral" runat="server" EnableViewState="false"></asp:Literal>:</td>
                        <td><asp:DropDownList ID="AttributeNamesDropDownList" runat="server" EnableViewState="false" CssClass="DropDownField"></asp:DropDownList></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </asp:Panel>
    <script type="text/javascript">

        jQuery(document).ready(function($) {
            var element = document.getElementById('<%= MappingHiddenField.ClientID %>');
            if (element != null && element.value != null && element.value != '') {
                var mapping = element.value;
                var panelElement = document.getElementById('<%= MappingPanel.ClientID %>');
                if (panelElement != null) {
                    var sourceElement = wopener.document.getElementById('<%= SourceMappingHiddenFieldClientId %>');
                    var sourcePanelElement = wopener.document.getElementById('<%= SourceMappingPanelClientId %>');
                    if (sourceElement != null && sourcePanelElement != null) {
                        var html = jQuery(panelElement).html();
                        sourceElement.value = mapping;
                        jQuery(sourcePanelElement).html(html);
                        CloseDialog();
                    }
                }
            }
        });
        
    </script>
</asp:Content>
<asp:Content ID="FooterContent" ContentPlaceHolderID="plcFooter" runat="server">
    <div id="FooterPanel" runat="server" class="FloatRight" enableviewstate="false">
        <cms:LocalizedButton ID="ConfirmButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="general.ok" />
        <cms:LocalizedButton ID="CancelButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="general.cancel" OnClientClick="return CloseDialog()" />
    </div>
</asp:Content>
