<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_UniGrid_UniGrid"
    CodeFile="UniGrid.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Controls/UniGridPager.ascx" TagPrefix="cms"
    TagName="UniGridPager" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Controls/AdvancedExport.ascx" TagPrefix="cms"
    TagName="AdvancedExport" %>
<div id="<%=ClientID%>" class="UniGridBody NotCheckChanges">
    <asp:PlaceHolder ID="plcContextMenu" runat="server" />
    <asp:Panel ID="pnlHeader" runat="server" CssClass="UniGridHeader">
        <asp:PlaceHolder ID="filter" runat="server" />
        <asp:PlaceHolder ID="plcFilterForm" runat="server" Visible="false">
            <cms:BasicForm ID="filterForm" runat="server" />
            <br class="ClearBoth" />
        </asp:PlaceHolder>
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <asp:Label ID="lblInfo" runat="server" EnableViewState="false" CssClass="InfoLabel" />
    </asp:Panel>
    <asp:Panel ID="pnlContent" runat="server" CssClass="UniGridContent">
        <table cellpadding="0" cellspacing="0" class="UniGridMain">
            <tr>
                <td>
                    <cms:UIGridView ID="UniGridView" ShortID="v" runat="server" AllowPaging="True" PageSize="25"
                        AutoGenerateColumns="False" AllowSorting="true" OnSorting="UniGridView_Sorting"
                        OnRowCreated="UniGridView_RowCreating" CellPadding="3" CssClass="UniGridGrid" 
                        EnableViewState="false" OnDataBound="UniGridView_DataBound" GridLines="Horizontal">
                        <HeaderStyle HorizontalAlign="Left" CssClass="UniGridHead" />
                    </cms:UIGridView>
                    <cms:UniGridPager ID="pagerElem" ShortID="p" runat="server" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hidSelection" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidNewSelection" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidDeSelection" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidCmdName" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidCmdArg" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidActions" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidActionsHash" runat="server" EnableViewState="false" />
    </asp:Panel>
    <cms:AdvancedExport ID="advancedExportElem" runat="server" ShortID="a" />
</div>
