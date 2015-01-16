<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ObjectFilter.ascx.cs"
    Inherits="CMSFormControls_Filters_ObjectFilter" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Panel CssClass="Filter" runat="server" ID="pnlObjectFilter">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <table>
                <asp:PlaceHolder ID="plcSite" runat="server">
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel ID="lblSite" runat="server" ResourceString="general.site" EnableViewState="false"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowAll="false" AllowEmpty="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcParentObject" runat="server">
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel ID="lblParent" runat="server" EnableViewState="false" DisplayColon="true" />
                        </td>
                        <td>
                            <cms:UniSelector ID="parentSelector" runat="server" AllowEmpty="false" AllowAll="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </table>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
