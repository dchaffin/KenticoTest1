<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" CodeFile="BuyContact.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_DataCom_BuyContact" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:HiddenField ID="ContactHiddenField" runat="server" />
    <asp:Panel runat="server" ID="TopPanel" CssClass="PageContent">
        <cms:ErrorSummary ID="ErrorSummary" runat="server" EnableViewState="false" MessagesEnabled="true"></cms:ErrorSummary>
        <p>
            <cms:LocalizedLiteral ID="IntroductionLiteral" runat="server" ResourceString="datacom.buycontact.introduction"></cms:LocalizedLiteral>
        </p>
        <table>
            <tbody>
                <tr>
                    <td><cms:LocalizedLabel ID="FirstNameLabel" runat="server" AssociatedControlID="FirstNameLiteral" DisplayColon="true" ResourceString="datacom.contact.firstname"></cms:LocalizedLabel></td>
                    <td><asp:Literal ID="FirstNameLiteral" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                    <td><cms:LocalizedLabel ID="LastNameLabel" runat="server" AssociatedControlID="LastNameLiteral" DisplayColon="true" ResourceString="datacom.contact.lastname"></cms:LocalizedLabel></td>
                    <td><asp:Literal ID="LastNameLiteral" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                    <td><cms:LocalizedLabel ID="CompanyNameLabel" runat="server" AssociatedControlID="CompanyNameLiteral" DisplayColon="true" ResourceString="datacom.contact.companyname"></cms:LocalizedLabel></td>
                    <td><asp:Literal ID="CompanyNameLiteral" runat="server"></asp:Literal></td>
                </tr>
            </tbody>
        </table>
        <p>
            <asp:Literal ID="AccountPointsLiteral" runat="server"></asp:Literal>
            <cms:LocalizedHyperlink ID="PurchasePointsLink" runat="server" Target="_blank" ResourceString="datacom.buycontact.purchasepoints" Visible="false"></cms:LocalizedHyperlink>
        </p>
    </asp:Panel>
    <asp:LinkButton ID="DummyLinkButton" runat="server"></asp:LinkButton>
    <script type="text/javascript">

        jQuery(document).ready(function($) {
            var element = document.getElementById('<%= ContactHiddenField.ClientID %>');
            if (element != null && element.value != null && element.value != "") {
                wopener.DataCom_SetContact(element.value);
                CloseDialog();
            }
        });
        
    </script>
</asp:Content>
<asp:Content ID="FooterContent" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight" id="FooterPanel" runat="server">
        <cms:LocalizedButton ID="BuyButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="datacom.buycontact.buy" OnClick="BuyButton_Click" />
        <cms:LocalizedButton ID="CancelButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="general.cancel" OnClientClick="return CloseDialog()" />
    </div>
</asp:Content>
