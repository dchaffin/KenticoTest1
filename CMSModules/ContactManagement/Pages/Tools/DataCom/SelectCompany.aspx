<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" CodeFile="SelectCompany.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_DataCom_SelectCompany" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/CompanyFilter.ascx" TagName="CompanyFilter" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/Company.ascx" TagName="Company" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:HiddenField ID="CompanyHiddenField" runat="server" EnableViewState="false" />
    <asp:Panel ID="MainPanel" runat="server" CssClass="PageContent">
        <cms:ErrorSummary ID="ErrorSummary" runat="server" EnableViewState="false" MessagesEnabled="true"></cms:ErrorSummary>
        <cms:CompanyFilter ID="CompanyFilterControl" runat="server"></cms:CompanyFilter>
        <br />
        <br />
        <table style="width:100%">
            <tbody>
                <tr>
                    <td style="vertical-align:top;width:50%">
                        <cms:UniGrid ID="CompanyGrid" runat="server" ShortID="g" Columns="CompanyId,Name,Country,City" IsLiveSite="false" DelayedReload="true" RememberState="false">
                            <gridactions>
                                <ug:Action Name="select" Caption="$general.select$" Icon="Next.png" />
                            </gridactions>
                            <gridcolumns>
                                <ug:Column Source="Name" Caption="$datacom.company.name$" Wrap="false"/>
                                <ug:Column Source="Country" Caption="$datacom.company.country$" Wrap="false"/>
                                <ug:Column Source="City" Caption="$datacom.company.city$" Wrap="false"/>
                            </gridcolumns>
                            <gridoptions showselection="false" />
                            <PagerConfig DefaultPageSize="10" PageSizeOptions="10,25,50,100" ShowDirectPageControl="false" ShowFirstLastButtons="false" />
                        </cms:UniGrid>
                    </td>
                    <td style="vertical-align:top;padding-left:30px">
                        <h2 style="margin-top:5px" id="TitleControl" runat="server" visible="false"><cms:LocalizedLabel runat="server" EnableViewState="false" ResourceString="datacom.selectedcompany"></cms:LocalizedLabel></h2>
                        <p id="EmptySelectionControl" runat="server" enableviewstate="false" visible="false"><cms:LocalizedLabel runat="server" EnableViewState="false" ResourceString="datacom.nocompanyselected"></cms:LocalizedLabel></p>
                        <cms:Company ID="CompanyControl" runat="server" EnableViewState="false"></cms:Company>
                    </td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    <script type="text/javascript">

        function DataCom_ConfirmSelection() {
            var element = document.getElementById('<%= CompanyHiddenField.ClientID %>');
            if (element != null) {
                var content = element.value;
                if (content != null && content != "") {
                    wopener.DataCom_SetCompany(content);
                    CloseDialog();
                }
            }
        }
    
    </script>
</asp:Content>
<asp:Content ID="FooterContent" ContentPlaceHolderID="plcFooter" runat="server">
    <div id="FooterPanel" runat="server" class="FloatRight">
        <cms:LocalizedButton ID="ConfirmButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="general.ok" OnClientClick="DataCom_ConfirmSelection(); return false;" Enabled="false" />
        <cms:LocalizedButton ID="CancelButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="general.cancel" OnClientClick="return CloseDialog()" />
    </div>
</asp:Content>
