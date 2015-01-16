<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_Controls_WebParts_WebpartProperties"
    CodeFile="WebpartProperties.ascx.cs" %>
<div id="CMSHeaderDiv">
    <div id="CKToolbar" class="CKToolbar">
    </div>
</div>
<script type="text/javascript">
    //<![CDATA[
    var cmsHeader = null, cmsHeaderPad = null, cmsFooter = null, cmsFooterPad = null, disableQim = true;
    var resizeInterval = setInterval('if (window.ResizeToolbar) { ResizeToolbar(); }', 300);

    jQuery(function () {
        jQuery('.FormCategoryList a[href^=#]').each(function () {
            var jThis = jQuery(this);
            jThis.click(function () {
                var target = jQuery(this.hash);
                target = target.length && target || jQuery('[name=' + this.hash.slice(1) + ']');
                if (target.length) {
                    // target offset - toolbar height - padding
                    var targetOffset = target.offset().top - GetHeight().header - 5;
                    jQuery('html,body').animate({ scrollTop: targetOffset }, 300);
                    if (jQuery('#' + this.hash.slice(1)).find('.ToggleImage').next().val() == 'True') {
                        jQuery('#' + this.hash.slice(1)).find('.ToggleImage').click();
                    }
                    return false;
                }
            });
        });
    });
    //]]>
</script>
<asp:Panel runat="server" ID="pnlTab" CssClass="TabsPageContent">
    <asp:Panel runat="server" ID="pnlFormArea" CssClass="WebPartForm">
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" OffsetX="136" OffsetY="10" IsLiveSite="false" />
        <cms:BasicForm runat="server" ID="form" HtmlAreaToolbarLocation="Out:CKToolbar" Enabled="true"
            DefaultFormLayout="Tables" DefaultCategoryName="Default" AllowMacroEditing="true"
            IsLiveSite="false" RenderCategoryList="true" MarkRequiredFields="true" />
        <br class="ClearBoth" />
        <asp:Panel runat="server" ID="pnlExport" CssClass="InfoLabel">
            <asp:Literal runat="server" ID="ltlExport" EnableViewState="false" />
            &nbsp;
            <cms:LocalizedLinkButton runat="server" ID="lnkLoadDefaults" ResourceString="webpartproperties.loaddefaults"
                OnClick="lnkLoadDefaults_Click" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
<asp:HiddenField ID="hdnIsNewWebPart" runat="server" />
<asp:HiddenField ID="hdnInstanceGUID" runat="server" />
<div id="CMSFooterDiv">
</div>
<script type="text/javascript">
    //<![CDATA[
    // cmsedit.js function override for CKEditor
    function SaveDocument() { }
    //]]>
</script>
