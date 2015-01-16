<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Pages_Widgets_WidgetProperties_Variant"
    Theme="default" EnableEventValidation="false" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    CodeFile="WidgetProperties_Variant.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ContentPersonalizationVariant/Edit.ascx"
    TagName="ContentPersonalizationVariantEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/MVTVariant/Edit.ascx"
    TagName="MvtVariantEdit" TagPrefix="cms" %>
<asp:Content ID="plcContent" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[
        var wopener = parent.wopener;

        function RefreshPage() {
            if (wopener.RefreshPage) {
                wopener.RefreshPage();
            }
        }
        //]]>
    </script>
    <cms:ContentPersonalizationVariantEdit ID="cpEditElem" runat="server" IsLiveSite="false"
        Visible="false" />
    <cms:MvtVariantEdit ID="mvtEditElem" runat="server" IsLiveSite="false" Visible="false" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <cms:CMSButton ID="btnOnApply" runat="server" Visible="false" />
    <cms:CMSButton ID="btnOnOK" runat="server" Visible="false" />
    <asp:HiddenField runat="server" ID="hidRefresh" Value="0" />
</asp:Content>