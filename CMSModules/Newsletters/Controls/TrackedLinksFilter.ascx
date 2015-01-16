<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TrackedLinksFilter.ascx.cs"
    Inherits="CMSModules_Newsletters_Controls_TrackedLinksFilter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/VariantFilter.ascx" TagName="VariantFilter"
    TagPrefix="cms" %>
<asp:Panel ID="pnl" runat="server" DefaultButton="btnShow">
    <table cellpadding="0" cellspacing="2">
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblLink" runat="server" ResourceString="general.link" DisplayColon="true"
                    EnableViewState="false" />
            </td>
            <td>
                <cms:TextSimpleFilter ID="fltLink" runat="server" Column="LinkTarget" IncludeNULLCondition="false" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblDescription" runat="server" ResourceString="general.description"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:TextSimpleFilter ID="fltDescription" runat="server" Column="LinkDescription"
                    IncludeNULLCondition="false" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcVariants" runat="server" Visible="false">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblVariants" runat="server" ResourceString="abtesting.variants"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:VariantFilter ID="fltVariants" runat="server" AllowSelectAll="false" Visible="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
            </td>
            <td>
                <cms:LocalizedButton ID="btnShow" runat="server" CssClass="ContentButton" ResourceString="general.show"
                    EnableViewState="false" />
            </td>
        </tr>
    </table>
    <br />
</asp:Panel>
