<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditToolbar.ascx.cs" Inherits="CMSModules_PortalEngine_Controls_OnSiteEdit_EditToolbar" %>
<%@ Register Src="~/CMSAdminControls/UI/UniMenu/UniMenu.ascx" TagName="UniMenu" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIToolbar.ascx" TagName="UIToolbar"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/AdvancedPopupHandler.ascx" TagName="AdvancedPopupHandler"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.ExtendedControls" %>
<script type="text/javascript" language="javascript">
    //<![CDATA[
    var refreshPageOnClose = false;

    function RefreshWOpener(w) {
        if (refreshPageOnClose || w.refreshPageOnClose) {
            var reloadUrl = '';

            if ((w.reloadPageUrl) && (w.reloadPageUrl.length > 0)) {
                reloadUrl = w.reloadPageUrl;
            }
            else {
                reloadUrl = window.location.href;
            }

            window.location = reloadUrl.replace('safemode=1', 'safemode=0');
        }
    }
    //]]>
</script>
<cms:AdvancedPopupHandler runat="server" ID="popupHandler" SetTitle="false" Visible="false" />
<asp:PlaceHolder runat="server" ID="plcEdit">
    <cms:CMSPanel ID="pnlToolbarSpace" ShortID="ts" runat="server" CssClass="OnSiteToolbarSpace">
    </cms:CMSPanel>
    <cms:CMSPanel ID="pnlToolbar" ShortID="t" runat="server" CssClass="OnSiteToolbar">
        <cms:CMSPanel ID="pnlBody" ShortID="b" runat="server" CssClass="OnSiteMenu">
            <cms:CMSPanel runat="server" ID="pnlLeft" ShortID="l" CssClass="OnSiteMenuLeft">
                <cms:UIToolbar ID="ucUIToolbar" ShortID="ut" runat="server" DisableScrollPanel="true"
                    GenerateElementCssClass="true" ModuleName="CMS.OnSiteEdit" RememberSelectedItem="false"
                    HighlightFirstItem="false" QueryParameterName="resourcename" DisableEditIcon="true" />
            </cms:CMSPanel>
            <div style="clear: both">
            </div>
        </cms:CMSPanel>
    </cms:CMSPanel>
    <cms:CMSPanel runat="server" ID="pnlSlider" ShortID="sb" CssClass="OnSiteSlider"
        Visible="false">
        <cms:CMSPanel ID="pnlButton" runat="server" CssClass="OnSiteSliderButton">
            <asp:Image ID="imgSliderButton" runat="server" />
            <br />
            <cms:LocalizedLabel ID="lblSliderText" runat="server" ResourceString="onsiteedit.editmode"></cms:LocalizedLabel>
        </cms:CMSPanel>
        <cms:CMSPanel ID="pnlMinimize" ShortID="ps" runat="server" CssClass="OnSiteSliderMinimize">
            <asp:Image ID="imgMaximize" runat="server" CssClass="OnSiteMaximize" />
            <asp:Image ID="imgMinimize" runat="server" CssClass="OnSiteMinimize" />
        </cms:CMSPanel>
    </cms:CMSPanel>
    <cms:CMSUpdateProgress ID="updateProgress" runat="server" ></cms:CMSUpdateProgress>
    <div class="OnSiteUPBg" onclick="javascript:ToggleUpdateProgress(false);" ></div>
    <asp:HiddenField ID="hdnPostbackValue" runat="server" />
</asp:PlaceHolder>

