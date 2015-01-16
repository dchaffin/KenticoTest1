<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_TreeContextMenu"
    CodeFile="TreeContextMenu.ascx.cs" %>
<%@ Register Assembly="CMS.UIControls" Namespace="CMS.UIControls" TagPrefix="cms" %>
<cms:ContextMenu runat="server" ID="menuNew" MenuID="newMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" CssClass="TreeContextMenu TreeNewContextMenu"
    Dynamic="true" MenuLevel="1" ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlNewMenu" CssClass="TreeNewContextMenu">
        <asp:Panel runat="server" ID="pnlNoChild" CssClass="ItemPadding" Visible="false">
            <asp:Literal runat="server" ID="ltlNoChild" EnableViewState="false" />
        </asp:Panel>
        <asp:Repeater runat="server" ID="repNew">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlItem" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlItemPadding" CssClass="ItemPadding">
                        <asp:Image runat="server" ID="imgItem" CssClass="IconBig" EnableViewState="false"
                            ImageUrl='<%# GetDocumentTypeIconUrl(Convert.ToString(DataBinder.Eval(Container.DataItem, "ClassName"))) %>' />&nbsp;
                        <asp:Label runat="server" ID="lblItem" CssClass="Name" EnableViewState="false" Text='<%#HttpUtility.HtmlEncode(ResHelper.LocalizeString(DataBinder.Eval(Container.DataItem, "ClassDisplayName") as String))%>' />
                    </asp:Panel>
                </asp:Panel>
            </ItemTemplate>
        </asp:Repeater>
        <cms:UIPlaceHolder runat="server" ID="plcNewLinkNew" ElementName="New" ModuleName="CMS.Content">
            <cms:UIPlaceHolder runat="server" ID="plcNewLink" ElementName="New.LinkExistingDocument"
                ModuleName="CMS.Content">
                <cms:ContextMenuSeparator runat="server" ID="pnlSepNewLinked" />
                <cms:ContextMenuItem runat="server" ID="iNewLink" Last="true" ResourceString="contentnew.newlink" />
            </cms:UIPlaceHolder>
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder runat="server" ID="plcNewVariantNew" ElementName="New" ModuleName="CMS.Content">
            <cms:UIPlaceHolder runat="server" ID="plcNewVariant" ElementName="New.ABTestVariant"
                ModuleName="CMS.Content">
                <cms:ContextMenuSeparator runat="server" ID="pnlNewVariantSeparator" Visible="false" />
                <cms:ContextMenuItem runat="server" ID="iNewVariant" Last="true" ResourceString="abtesting.abtestvariant" />
            </cms:UIPlaceHolder>
        </cms:UIPlaceHolder>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuUp" MenuID="upMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MouseButton="Right"
    MenuLevel="1" ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlUpMenu" CssClass="TreeContextMenu">
        <cms:ContextMenuItem runat="server" ID="iTop" Last="true" ResourceString="UpMenu.IconTop" />
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuDown" MenuID="downMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MouseButton="Right"
    MenuLevel="1" ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlDownMenu" CssClass="TreeContextMenu">
        <cms:ContextMenuItem runat="server" ID="iBottom" Last="true" ResourceString="DownMenu.IconBottom" />
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuSort" MenuID="sortMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MouseButton="Both"
    MenuLevel="1" ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlSortMenu" CssClass="TreeContextMenu">
        <cms:ContextMenuItem runat="server" ID="iAlphaAsc" ResourceString="SortMenu.IconAlphaAsc" />
        <cms:ContextMenuItem runat="server" ID="iAlphaDesc" ResourceString="SortMenu.IconAlphaDesc" />
        <cms:ContextMenuItem runat="server" ID="iDateAsc" ResourceString="SortMenu.IconDateAsc" />
        <cms:ContextMenuItem runat="server" ID="iDateDesc" Last="true" ResourceString="SortMenu.IconDateDesc" />
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuProperties" MenuID="propertiesMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MenuLevel="1"
    ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlPropertiesMenu" CssClass="TreeContextMenu">
        <cms:UIPlaceHolder ID="pnlUIGeneral" runat="server" ModuleName="CMS.Content" ElementName="Properties.General">
            <cms:ContextMenuItem runat="server" ID="iGeneral" ResourceString="general.general" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIUrls" runat="server" ModuleName="CMS.Content" ElementName="Properties.URLs">
            <cms:ContextMenuItem runat="server" ID="iUrls" ResourceString="PropertiesMenu.iconurls" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUITemplate" runat="server" ModuleName="CMS.Content" ElementName="Properties.Template">
            <cms:ContextMenuItem runat="server" ID="iTemplate" ResourceString="PropertiesMenu.IconTemplate" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIMetadata" runat="server" ModuleName="CMS.Content" ElementName="Properties.MetaData">
            <cms:ContextMenuItem runat="server" ID="iMetadata" ResourceString="PropertiesMenu.IconMetadata" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUICategories" runat="server" ModuleName="CMS.Content" ElementName="Properties.Categories">
            <cms:ContextMenuItem runat="server" ID="iCategories" ResourceString="PropertiesMenu.iconcategories" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIMenu" runat="server" ModuleName="CMS.Content" ElementName="Properties.Menu">
            <cms:ContextMenuItem runat="server" ID="iMenu" ResourceString="PropertiesMenu.IconMenu" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIWorkflow" runat="server" ModuleName="CMS.Content" ElementName="Properties.Workflow">
            <cms:ContextMenuItem runat="server" ID="iWorkflow" ResourceString="PropertiesMenu.IconWorkflow" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIVersions" runat="server" ModuleName="CMS.Content" ElementName="Properties.Versions">
            <cms:ContextMenuItem runat="server" ID="iVersions" ResourceString="PropertiesMenu.IconVersions" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIRelated" runat="server" ModuleName="CMS.Content" ElementName="Properties.RelatedDocs">
            <cms:ContextMenuItem runat="server" ID="iRelated" ResourceString="PropertiesMenu.IconRelated" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUILinked" runat="server" ModuleName="CMS.Content" ElementName="Properties.LinkedDocs">
            <cms:ContextMenuItem runat="server" ID="iLinked" ResourceString="PropertiesMenu.IconLinked" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUISecurity" runat="server" ModuleName="CMS.Content" ElementName="Properties.Security">
            <cms:ContextMenuItem runat="server" ID="iSecurity" ResourceString="PropertiesMenu.IconSecurity" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIAttachments" runat="server" ModuleName="CMS.Content"
            ElementName="Properties.Attachments">
            <cms:ContextMenuItem runat="server" ID="iAttachments" ResourceString="PropertiesMenu.IconAttachments" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUILanguages" runat="server" ModuleName="CMS.Content" ElementName="Properties.Languages">
            <cms:ContextMenuItem runat="server" ID="iLanguages" ResourceString="PropertiesMenu.IconLanguages" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIWireframe" runat="server" ModuleName="CMS.Content" ElementName="Properties.Wireframe">
            <cms:ContextMenuItem runat="server" ID="iWireframe" ResourceString="content.ui.wireframe" />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUICPVariants" runat="server" ModuleName="CMS.Content" ElementName="Properties.Variants">
            <cms:ContextMenuItem runat="server" ID="iVariants" ResourceString="content.ui.propertiesvariants" />
        </cms:UIPlaceHolder>
    </asp:Panel>
</cms:ContextMenu>
<asp:Panel runat="server" ID="pnlNodeMenu" CssClass="TreeContextMenu">
    <cms:ContextMenuItem runat="server" ID="iNoNode" ResourceString="content.documentnotexistsinfoshort" Visible="False" />
    <asp:PlaceHolder ID="plcFirstLevelContainer" runat="server">
        <cms:ContextMenuContainer runat="server" ID="cmcNew" MenuID="newMenu" Parameter="GetContextMenuParameter('nodeMenu')">
            <cms:ContextMenuItem runat="server" ID="iNew" ResourceString="ContentMenu.ContextIconNew" />
        </cms:ContextMenuContainer>
        <asp:Panel runat="server" ID="pnlDelete" CssClass="ItemLast">
            <cms:ContextMenuItem runat="server" ID="iDelete" ResourceString="general.delete" />
        </asp:Panel>
        <cms:ContextMenuSeparator runat="server" ID="pnlSep1" />
        <cms:ContextMenuItem runat="server" ID="iCopy" ResourceString="ContentMenu.IconCopy" />
        <cms:ContextMenuItem runat="server" ID="iMove" Last="true" ResourceString="ContentMenu.IconMove" />
        <cms:ContextMenuSeparator runat="server" ID="pnlSep2" />
        <cms:ContextMenuContainer runat="server" ID="cmcUp" MenuID="upMenu">
            <cms:ContextMenuItem runat="server" ID="iUp" ResourceString="ContentMenu.ContextIconMoveUp" />
        </cms:ContextMenuContainer>
        <cms:ContextMenuContainer runat="server" ID="cmcDown" MenuID="downMenu">
            <cms:ContextMenuItem runat="server" ID="iDown" ResourceString="ContentMenu.ContextIconMoveDown" />
        </cms:ContextMenuContainer>
        <cms:ContextMenuContainer runat="server" ID="cmcSort" MenuID="sortMenu">
            <cms:ContextMenuItem runat="server" ID="iSort" ResourceString="ContentMenu.IconSort" />
        </cms:ContextMenuContainer>
        <cms:ContextMenuSeparator runat="server" ID="pnlSep3" />
        <asp:Panel runat="server" ID="pnlRefresh" CssClass="ItemLast">
            <cms:ContextMenuItem runat="server" ID="iRefresh" ResourceString="ContentMenu.IconRefresh" />
        </asp:Panel>
        <asp:PlaceHolder runat="server" ID="plcProperties">
            <cms:ContextMenuSeparator runat="server" ID="pnlSep4" />
            <cms:UIPlaceHolder ID="pnlUIcmcProperties" runat="server" ModuleName="CMS.Content"
                ElementName="Properties">
                <cms:ContextMenuContainer runat="server" ID="cmcProperties" MenuID="propertiesMenu">
                    <cms:ContextMenuItem runat="server" ID="iProperties" ResourceString="ContentMenu.IconProperties" />
                </cms:ContextMenuContainer>
            </cms:UIPlaceHolder>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
</asp:Panel>
