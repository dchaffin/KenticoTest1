<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_Controls_WebParts_WebPartMenu"
    CodeFile="WebPartMenu.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<cms:ContextMenu runat="server" ID="menuMoveTo" MenuID="moveToMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MenuLevel="1"
    ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlZoneMenu" CssClass="PortalContextMenu WebPartContextMenu">
        <div class="NormalLayoutMenu">
            <cms:ContextMenuItem runat="server" ID="iTop" />
            <cms:ContextMenuItem runat="server" ID="iUp" />
            <cms:ContextMenuSeparator runat="server" ID="sm1" />
            <cms:ContextMenuItem runat="server" ID="iDown" />
            <cms:ContextMenuItem runat="server" ID="iBottom" />
            <cms:ContextMenuSeparator runat="server" ID="sm3" />
        </div>
        <cms:UIRepeater runat="server" ID="repZones" ShortID="z">
            <ItemTemplate>
                <cms:ContextMenuContainer runat="server" ID="cmcZoneVariants" MenuID="zoneVariants"
                    Parameter="GetContextMenuParameter('selectedZoneId')">
                    <cms:ContextMenuItem runat="server" ID="zoneElem" onmouseover='<%#(((CMSWebPartZone)Container.DataItem).HasVariants) ? "SetContextMenuParameter(\"selectedZoneId\", \"" + ((CMSWebPartZone)Container.DataItem).ID + "\");" : "CM_Close(\"zoneVariants\");"%>'
                        OnClick='<%#"CM_Close(\"zoneVariants\"); ContextMoveWebPartToZone(GetContextMenuParameter(\"webPartMenu\"), \"" + ((CMSWebPartZone)Container.DataItem).ID + "\");"%>'
                        Text='<%#(((CMSWebPartZone)Container.DataItem).ZoneTitle != "" ? HTMLHelper.HTMLEncode(((CMSWebPartZone)Container.DataItem).ZoneTitle) : ((CMSWebPartZone)Container.DataItem).ID) + (((CMSWebPartZone)Container.DataItem).HasVariants ? "..." : "")%>'
                        ImageUrl='<%# GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Variants/zone.png") %>' />
                </cms:ContextMenuContainer>
            </ItemTemplate>
        </cms:UIRepeater>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuWebPartMVTVariants" MenuID="webpartAllMVTVariants"
    VerticalPosition="Bottom" HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected"
    MenuLevel="1" Dynamic="true" ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlAllWebPartMVTVariants" CssClass="PortalContextMenu WebPartContextMenu">
        <asp:PlaceHolder ID="plcAddMVTVariant" runat="server">
            <asp:Panel runat="server" ID="pnlAddMVTVariant" CssClass="Item">
                <asp:Panel runat="server" ID="pnlAddMVTVariantPadding" CssClass="ItemPadding">
                    <asp:Image runat="server" ID="imgAddMVTVariant" CssClass="Icon" EnableViewState="false" />
                    <asp:Label runat="server" ID="lblAddMVTVariant" CssClass="Name" EnableViewState="false" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel1" CssClass="Separator">
                &nbsp;
            </asp:Panel>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlNoWebPartMVTVariants" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="Panel2" CssClass="ItemPadding">
                <asp:Image runat="server" ID="imgNoWebPartMVTVariants" CssClass="Icon" EnableViewState="false" />
                <asp:Label runat="server" ID="lblNoWebPartMVTVariants" CssClass="Name" EnableViewState="false" />
            </asp:Panel>
        </asp:Panel>
        <cms:UIRepeater runat="server" ID="repWebPartMVTVariants" ShortID="wpmvtv">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlVariantItem" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlItemPadding" CssClass="ItemPadding">
                        <asp:Image runat="server" ID="imgVariantItem" EnableViewState="false" CssClass="Icon" />
                        <asp:Label runat="server" ID="lblVariantItem" CssClass="Name" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </ItemTemplate>
        </cms:UIRepeater>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuWebPartCPVariants" MenuID="webpartAllCPVariants"
    VerticalPosition="Bottom" HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected"
    MenuLevel="1" Dynamic="true" ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlAllWebPartVariants" CssClass="PortalContextMenu WebPartContextMenu">
        <asp:PlaceHolder ID="plcAddCPVariant" runat="server">
            <asp:Panel runat="server" ID="pnlAddCPVariant" CssClass="Item">
                <asp:Panel runat="server" ID="pnlAddCPVariantPadding" CssClass="ItemPadding">
                    <asp:Image runat="server" ID="imgAddCPVariant" CssClass="Icon" EnableViewState="false" />
                    <asp:Label runat="server" ID="lblAddCPVariant" CssClass="Name" EnableViewState="false" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel5" CssClass="Separator">
                &nbsp;
            </asp:Panel>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlNoWebPartCPVariants" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="Panel3" CssClass="ItemPadding">
                <asp:Image runat="server" ID="imgNoWebPartCPVariants" CssClass="Icon" EnableViewState="false" />
                <asp:Label runat="server" ID="lblNoWebPartCPVariants" CssClass="Name" EnableViewState="false" />
            </asp:Panel>
        </asp:Panel>
        <cms:UIRepeater runat="server" ID="repWebPartCPVariants" ShortID="wpv">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlVariantItem" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlItemPadding" CssClass="ItemPadding">
                        <asp:Image runat="server" ID="imgVariantItem" EnableViewState="false" CssClass="Icon" />
                        <asp:Label runat="server" ID="lblVariantItem" CssClass="Name" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </ItemTemplate>
        </cms:UIRepeater>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuMoveToZoneVariants" MenuID="zoneVariants"
    VerticalPosition="Bottom" HorizontalPosition="Left" OffsetX="50" ActiveItemCssClass="ItemSelected"
    MenuLevel="2" Dynamic="true" ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlZoneVariants" CssClass="PortalContextMenu WebPartContextMenu">
        <asp:Panel runat="server" ID="pnlNoZoneVariants" CssClass="ItemPadding" Visible="false">
            <asp:Literal runat="server" ID="ltlNoZoneVariants" EnableViewState="false" />
        </asp:Panel>
        <cms:UIRepeater runat="server" ID="repMoveToZoneVariants" ShortID="zv">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlZoneVariantItem" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlZoneItemPadding" CssClass="ItemPadding">
                        <asp:Image runat="server" ID="imgZoneVariantItem" EnableViewState="false" CssClass="Icon" />
                        <asp:Label runat="server" ID="lblZoneVariantItem" CssClass="Name" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </ItemTemplate>
        </cms:UIRepeater>
    </asp:Panel>
</cms:ContextMenu>
<asp:Panel runat="server" ID="pnlWebPartMenu" CssClass="PortalContextMenu WebPartContextMenu">
    <cms:UIPlaceHolder ID="pnlUIProperties" runat="server" ModuleName="CMS.Content" ElementName="Design.WebPartProperties">
        <cms:ContextMenuItem runat="server" ID="iProperties" />
        <cms:ContextMenuSeparator runat="server" ID="sep1" />
    </cms:UIPlaceHolder>
    <div class="FreeLayoutMenu">
        <cms:ContextMenuItem runat="server" ID="iForwardAll" />
        <cms:ContextMenuItem runat="server" ID="iBackwardAll" />
        <cms:ContextMenuSeparator runat="server" ID="sepPos" />
    </div>
    <cms:ContextMenuItem runat="server" ID="iMoveTo" SubMenuID="moveToMenu" />
    <cms:UIPlaceHolder ID="pnlUIClone" runat="server" ModuleName="CMS.Content" ElementName="Design.AddWebParts">
        <cms:ContextMenuItem runat="server" ID="iClone" />
    </cms:UIPlaceHolder>
    <cms:ContextMenuSeparator runat="server" ID="sep2" />
    <cms:UIPlaceHolder ID="pnlUIDelete" runat="server" ModuleName="CMS.Content" ElementName="Design.RemoveWebParts">
        <cms:ContextMenuItem runat="server" ID="iDelete" Last="true" />
    </cms:UIPlaceHolder>
    <cms:UIPlaceHolder ID="pnlUIMVTVariants" runat="server" ModuleName="CMS.Content"
        ElementName="WebPartProperties.Variant">
        <asp:Panel ID="pnlContextMenuMVTVariants" runat="server" Visible="false">
            <asp:Panel runat="server" ID="pnlSep5" CssClass="Separator">
                &nbsp;
            </asp:Panel>
            <cms:ContextMenuContainer runat="server" ID="cmcAllMVTVariants" MenuID="webpartAllMVTVariants"
                Parameter="GetContextMenuParameter('webPartMenu')">
                <asp:Panel runat="server" ID="pnlMVTVariants" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlMVTVariantsPadding" CssClass="ItemPadding">
                        <asp:Image runat="server" ID="imgMVTVariants" CssClass="Icon" EnableViewState="false" />
                        <asp:Label runat="server" ID="lblMVTVariants" CssClass="NameInactive" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </cms:ContextMenuContainer>
        </asp:Panel>
    </cms:UIPlaceHolder>
    <cms:UIPlaceHolder ID="pnlUICPVariants" runat="server" ModuleName="CMS.Content" ElementName="WebPartProperties.Variant">
        <asp:Panel ID="pnlContextMenuCPVariants" runat="server" Visible="false">
            <asp:Panel runat="server" ID="pnlSep3" CssClass="Separator">
                &nbsp;
            </asp:Panel>
            <cms:ContextMenuContainer runat="server" ID="cmcAllCPVariants" MenuID="webpartAllCPVariants"
                Parameter="GetContextMenuParameter('webPartMenu')">
                <asp:Panel runat="server" ID="pnlCPVariants" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlCPVariantsPadding" CssClass="ItemPadding">
                        <asp:Image runat="server" ID="imgCPVariants" CssClass="Icon" EnableViewState="false" />
                        <asp:Label runat="server" ID="lblCPVariants" CssClass="NameInactive" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </cms:ContextMenuContainer>
        </asp:Panel>
    </cms:UIPlaceHolder>
</asp:Panel>
<script type="text/javascript">
    //<![CDATA[
    function ContextConfigureWebPart(definition) {
        ConfigureWebPart(escape(definition[0]), escape(definition[1]), escape(definition[2]), escape(definition[3]), escape(definition[4]));
    }

    function ContextMoveWebPartUp(definition) {
        MoveWebPartUp(definition[0], definition[1], definition[2], definition[3], definition[6]);
    }

    function ContextMoveWebPartDown(definition) {
        MoveWebPartDown(definition[0], definition[1], definition[2], definition[3], definition[6]);
    }

    function ContextMoveWebPartTop(definition) {
        MoveWebPartUp(definition[0], definition[1], definition[2], definition[3], definition[6], true);
    }

    function ContextMoveWebPartBottom(definition) {
        MoveWebPartDown(definition[0], definition[1], definition[2], definition[3], definition[6], true);
    }

    function ContextRemoveWebPart(definition) {
        RemoveWebPart(definition[0], definition[1], definition[2], definition[3]);
    }

    function ContextMoveWebPartToZone(definition, targetZoneId) {
        MoveWebPart(definition[0], definition[1], definition[2], definition[3], targetZoneId, 1000);
    }

    function ContextCloneWebPart(definition) {
        CloneWebPart(definition[0], definition[1], definition[2], definition[3]);
    }

    function ContextAddWebPartMVTVariant(definition) {
        AddMVTVariant(definition[0], definition[1], definition[2], definition[3], definition[4], 'webpart', '');
    }

    function ContextAddWebPartCPVariant(definition) {
        AddPersonalizationVariant(definition[0], definition[1], definition[2], definition[3], definition[4], 'webpart', '');
    }
    //]]>
</script>
