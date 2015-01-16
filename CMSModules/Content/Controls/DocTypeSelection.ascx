<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DocTypeSelection.ascx.cs"
    Inherits="CMSModules_Content_Controls_DocTypeSelection" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectSinglePath.ascx"
    TagName="SelectSinglePath" TagPrefix="cms" %>
<div class="ContentNewClasses">
    <asp:Label ID="lblInfo" runat="server" CssClass="ContentLabel" EnableViewState="false"
        Font-Bold="true" />
    <asp:Label ID="lblError" runat="server" CssClass="ContentLabel" ForeColor="Red" EnableViewState="false" />
</div>
<br />
<div class="ContentNewClasses UniGridClearPager">
    <cms:UniGrid runat="server" ID="gridClasses" IsLiveSite="false" ZeroRowsText="">
        <GridColumns>
            <ug:Column Source="##ALL##" ExternalSourceName="classname" Caption="$general.codename$"
                Wrap="false" />
            <ug:Column Source="ClassDisplayName" Caption="$documenttype.name$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</div>
<br />
<cms:UIPlaceHolder runat="server" ID="plcNewLinkNew" ElementName="New" ModuleName="CMS.Content">
    <cms:UIPlaceHolder runat="server" ID="plcNewLink" ElementName="New.LinkExistingDocument"
        ModuleName="CMS.Content">
        <asp:Panel runat="server" ID="pnlFooter" CssClass="PageSeparator">
            <asp:HyperLink runat="server" ID="lnkNewLink" CssClass="ContentNewLink" EnableViewState="false">
                <asp:Image ID="imgNewLink" runat="server" Width="16" EnableViewState="false" />
                <asp:Label ID="lblNewLink" runat="server" EnableViewState="false" />
            </asp:HyperLink>
        </asp:Panel>
    </cms:UIPlaceHolder>
</cms:UIPlaceHolder>
<cms:UIPlaceHolder runat="server" ID="plcNewABTestVariant" ElementName="New" ModuleName="CMS.Content">
    <cms:UIPlaceHolder runat="server" ID="plcNewVariantLink" ElementName="New.ABTestVariant"
        ModuleName="CMS.Content">
        <asp:Panel runat="server" ID="pnlABVariant" EnableViewState="false">
            <asp:HyperLink runat="server" ID="lnkNewVariant" CssClass="ContentNewLink" EnableViewState="false">
                <asp:Image ID="imgNewVariant" runat="server" Width="16" EnableViewState="false" />
                <asp:Label ID="lblNewVariant" runat="server" EnableViewState="false" />
            </asp:HyperLink>
        </asp:Panel>
    </cms:UIPlaceHolder>
</cms:UIPlaceHolder>
