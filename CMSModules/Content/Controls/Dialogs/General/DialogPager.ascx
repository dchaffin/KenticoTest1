<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_General_DialogPager"
    CodeFile="DialogPager.ascx.cs" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/PageSize.ascx"
    TagName="PageSize" TagPrefix="cms" %>

<div class="DialogPager">
    <cms:UniPager ID="pagerElem" ShortID="p" runat="server" DirectPageControlID="txtPage">
        <PreviousPageTemplate>
            <td>
                <a class="UnigridPagerPrev" href="<%# Eval("PreviousURL") %>">&nbsp;</a>
            </td>
        </PreviousPageTemplate>
        <PreviousGroupTemplate>
            <td>
                <a class="UnigridPagerPage" href="<%# Eval("PreviousGroupURL") %>">...</a>
            </td>
        </PreviousGroupTemplate>
        <PageNumbersTemplate>
            <td>
                <a class="UnigridPagerPage" href="<%# Eval("PageURL") %>">
                    <%# Eval("Page") %></a>
            </td>
        </PageNumbersTemplate>
        <PageNumbersSeparatorTemplate>
        </PageNumbersSeparatorTemplate>
        <CurrentPageTemplate>
            <td>
                <span class="UnigridPagerSelectedPage">
                    <%# Eval("Page") %></span>
            </td>
        </CurrentPageTemplate>
        <NextGroupTemplate>
            <td>
                <a class="UnigridPagerPage" href="<%# Eval("NextGroupURL") %>">...</a>
            </td>
        </NextGroupTemplate>
        <NextPageTemplate>
            <td>
                <a class="UnigridPagerNext" href="<%# Eval("NextURL") %>">&nbsp;</a>
            </td>
        </NextPageTemplate>
        <LayoutTemplate>
            <table cellspacing="0" cellpadding="0">
                <tr>
                    <asp:PlaceHolder runat="server" ID="plcPreviousPage"></asp:PlaceHolder>
                    <td style="white-space: nowrap;" class="UnigridPagerPages">
                        <table class="UniGridPagerNoSeparator" cellpadding="0" cellspacing="0">
                            <tr>
                                <asp:PlaceHolder runat="server" ID="plcPreviousGroup"></asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" ID="plcPageNumbers"></asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" ID="plcNextGroup"></asp:PlaceHolder>
                            </tr>
                        </table>
                    </td>
                    <asp:PlaceHolder runat="server" ID="plcNextPage"></asp:PlaceHolder>
                </tr>
            </table>
        </LayoutTemplate>
    </cms:UniPager>
</div>
<div class="DialogPageSize">
    <cms:PageSize ID="pageSize" ShortID="ps" runat="server" />
</div>
