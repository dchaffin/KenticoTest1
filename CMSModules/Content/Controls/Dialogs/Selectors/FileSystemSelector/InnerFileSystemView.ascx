<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_InnerFileSystemView"
    CodeFile="InnerFileSystemView.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/DialogPager.ascx"
    TagName="DialogPager" TagPrefix="cms" %>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false"></asp:Literal>
<div class="DialogViewArea" style="height: 100%;">
    <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false"></asp:Label>
    <asp:PlaceHolder ID="plcViewArea" runat="server">
        <asp:PlaceHolder ID="plcListView" runat="server" Visible="false">
            <div class="ListView">
                <cms:UniGrid ID="gridList" ShortID="g" runat="server" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcTilesView" runat="server" Visible="false">
            <div class="TilesView">
                <cms:BasicRepeater ID="repTilesView" ShortID="t" runat="server">
                    <ItemTemplate>
                        <div class="DialogTileItemShadow">
                            <div id="<%#GetID(Container.DataItem)%>" class="DialogTileItem">
                                <asp:Panel ID="pnlTiles" runat="server" CssClass="DialogTileItemBox" EnableViewState="false">
                                    <asp:Panel ID="pnlImageContainer" runat="server" CssClass="DialogTileItemImageContainer">
                                        <div class="DialogTileItemImage">
                                            <asp:Image ID="imgElem" runat="server" />
                                        </div>
                                    </asp:Panel>
                                    <div class="DialogTileItemInfo">
                                        <asp:Label ID="lblFileName" runat="server"></asp:Label>
                                        <br />
                                        <div class="DialogTileItemInfoGreyText">
                                            <asp:Label ID="lblTypeValue" runat="server"></asp:Label>
                                            <br />
                                            <asp:Label ID="lblSizeValue" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                </asp:Panel>
                                <div class="DialogTileItemActions">
                                    <asp:ImageButton ID="btnSelect" runat="server" EnableViewState="false" />
                                    <asp:ImageButton ID="btnDelete" runat="server" EnableViewState="false" />
                                    <asp:ImageButton ID="btnEdit" runat="server" EnableViewState="false" />
                                    <div class="DialogTilesClear">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </cms:BasicRepeater>
                <div class="DialogTilesClear">
                </div>
                <cms:DialogPager ID="pagerElemTiles" ShortID="pti" runat="server" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcThumbnailsView" runat="server" Visible="false">
            <div class="ThumbnailsView">
                <cms:BasicRepeater ID="repThumbnailsView" ShortID="th" runat="server">
                    <ItemTemplate>
                        <div class="DialogThumbnailItemShadow">
                            <div id="<%#GetID(Container.DataItem)%>" class="DialogThumbnailItem">
                                <asp:Panel ID="pnlThumbnails" runat="server" CssClass="DialogThumbnailItemBox" EnableViewState="false">
                                    <asp:Panel ID="pnlImageContainer" runat="server" CssClass="DialogThumbItemImageContainer"
                                        EnableViewState="false">
                                        <table cellpadding="0" cellspacing="0" border="0" class="DialogThumbnailItemImage">
                                            <tr>
                                                <td>
                                                    <asp:Image ID="imgElem" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <div class="DialogThumbnailItemInfo">
                                        <asp:Label ID="lblFileName" runat="server" EnableViewState="false"></asp:Label>
                                    </div>
                                </asp:Panel>
                                <div class="DialogThumbnailActions" enableviewstate="false">
                                    <asp:ImageButton ID="btnSelect" runat="server" EnableViewState="false" />
                                    <asp:ImageButton ID="btnDelete" runat="server" EnableViewState="false" />
                                    <asp:ImageButton ID="btnEdit" runat="server" EnableViewState="false" />
                                    <div class="DialogTilesClear">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </cms:BasicRepeater>
                <div class="DialogTilesClear">
                </div>
                <cms:DialogPager ID="pagerElemThumbnails" ShortID="pth" runat="server" />
            </div>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
</div>
<asp:HiddenField ID="hdnItemToColorize" runat="server" />
