<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AuthorizationSetup.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_SalesForce_AuthorizationSetup" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" %>
<%@ Register TagPrefix="cms" TagName="SalesForceError" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/Error.ascx" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="CMS.SalesForce" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server" EnableViewState="false">
    <asp:HiddenField ID="CredentialsHiddenField" runat="server" EnableViewState="false" />
    <asp:HiddenField ID="MessageHiddenField" runat="server" EnableViewState="false" />
    <asp:Panel ID="MainMessagePanel" runat="server" CssClass="PageContent" EnableViewState="false" Visible="false">
        <p id="MessageLabel" runat="server" enableviewstate="false" visible="false"></p>
    </asp:Panel>
    <asp:Panel ID="MainPanel" runat="server" CssClass="PageContent" EnableViewState="false">
        <cms:SalesForceError ID="SalesForceError" runat="server" EnableViewState="false" MessagesEnabled="true" />
        <p><%= HTMLHelper.HTMLEncode(GetString("sf.authorization.instructions1"))%></p>
        <p style="font-weight:bold"><asp:Literal ID="RedirectUrlLiteral" runat="server" EnableViewState="false"></asp:Literal></p>
        <p id="HelpLinkContainer">
            <img id="HelpImage" runat="server" alt="Help" style="vertical-align:middle" />
            <a href="#" id="HelpLink"><%= HTMLHelper.HTMLEncode(GetString("sf.authorization.instructions1.help"))%></a>
        </p>
        <ol class="help" style="display:none">
            <% foreach (string instruction in GetString("sf.authorization.instructions2").Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => !String.IsNullOrEmpty(x))) { %>
            <li><%= instruction %></li><%-- No HTML encoding --%>
            <% } %>
        </ol>
        <table>
            <tbody>
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="ClientIdentifierLabel" runat="server" AssociatedControlID="ClientIdentifierTextBox" DisplayColon="true" EnableViewState="false" ResourceString="sf.consumerkey">Consumer key</cms:LocalizedLabel>
                    </td>
                    <td>
                        <asp:TextBox ID="ClientIdentifierTextBox" runat="server" EnableViewState="false" Columns="100"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="ClientSecretLabel" runat="server" AssociatedControlID="ClientSecretTextBox" DisplayColon="true" EnableViewState="false" ResourceString="sf.consumersecret"></cms:LocalizedLabel>
                    </td>
                    <td>
                        <asp:TextBox ID="ClientSecretTextBox" runat="server" EnableViewState="false" Columns="25"></asp:TextBox>
                    </td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    <script type="text/javascript">

        jQuery(document).ready(function() {
            var linkElement = jQuery("#HelpLink");
            var linkContainerElement = jQuery("#HelpLinkContainer");
            linkElement.click(function(e) {
                linkContainerElement.hide();
                jQuery(".help").show();
                e.preventDefault();
            });
        });

        jQuery(document).ready(function () {
            var credentialsField = document.getElementById('<%= CredentialsHiddenField.ClientID %>');
            if (credentialsField != null && credentialsField.value != null && credentialsField.value != '') {
                if (wopener) {
                    var sourceCredentialsField = wopener.document.getElementById('<%= SourceCredentialsHiddenFieldClientId %>');
                    if (sourceCredentialsField != null) {
                        sourceCredentialsField.value = credentialsField.value;
                    }
                    var messageField = document.getElementById('<%= MessageHiddenField.ClientID %>');
                    if (messageField != null && messageField.value != null && messageField.value != '') {
                        var sourceMessageLabel = wopener.document.getElementById('<%= SourceMessageLabelClientId %>');
                        if (sourceMessageLabel != null) {
                            jQuery(sourceMessageLabel).text(messageField.value);
                        }
                    }
                    CloseDialog();
                }
                else {
                    __doPostBack("SaveButton", null);
                }
            }
        });
        
    </script>
</asp:Content>
<asp:Content ID="FooterContent" ContentPlaceHolderID="plcFooter" runat="server">
    <div id="FooterPanel" runat="server" class="FloatRight">
        <cms:LocalizedButton ID="ConfirmButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="sf.authorize" />
        <cms:LocalizedButton ID="CancelButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="general.cancel" OnClientClick="window.close(); return false;" />
    </div>
</asp:Content>
