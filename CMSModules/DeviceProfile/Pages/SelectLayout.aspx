<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" CodeFile="SelectLayout.aspx.cs" Inherits="CMSModules_DeviceProfile_Pages_SelectLayout"
    Theme="Default" EnableEventValidation="false" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniFlatSelector.ascx" TagName="UniFlatSelector"
    TagPrefix="cms" %>
<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="server">
    <script type="text/javascript" language="javascript">
    //<![CDATA[

        function pageLoad() {
            if (jQuery.isFunction(window.resizearea)) {
                resizearea();
            }

            setTimeout('Focus()', 100);
            var timer = null;
            SetupSearch();
        }

        function Client_ConfirmSelection() {
            var parameters = {
                sourceLayoutId: jQuery('#SourceLayoutId').val(),
                targetLayoutId: jQuery('#TargetLayoutId').val()
            };
            if (parameters.targetLayoutId == 0) {
                alert('<%= HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.selecttargetlayout"))%>');
            }
            else {
                wopener.Client_SetTargetLayout(parameters);
                CloseDialog();
            }

            return false;
        }

        function Client_SetTargetLayout(targetLayoutId) {
            var parameters = {
                sourceLayoutId: jQuery('#SourceLayoutId').val(),
                targetLayoutId: targetLayoutId
            };
            wopener.Client_SetTargetLayout(parameters);
            return CloseDialog();
        }

        // Move checkbox to the top of selector content
        jQuery(function () {
            jQuery(".DeviceProfileLayoutSelector .ShowMatchingLayouts").prependTo(".DeviceProfileLayoutSelector .UniFlatContentItems");
        });

    //]]>   
    </script>
    <div class="DeviceProfileLayoutSelector">
        <table class="SelectorTable" cellpadding="0" cellspacing="0">
            <tr>
                <td class="ItemSelectorArea">
                    <div class="ItemSelector DeviceProifleLayoutSelector">
                        <cms:UniFlatSelector ID="LayoutSelector" ShortID="f" runat="server" RememberSelectedItem="true">
                            <HeaderTemplate>
                                <div class="ShowMatchingLayouts">
                                    <input type="checkbox" id="chkDisplayOnlyCompatibleLayouts" <%= DisplayOnlyCompatibleLayouts ? "checked=\"checked\"" : ""%> value="true" name="chkDisplayOnlyCompatibleLayouts" onchange="__doPostBack();" />
                                    <label for="chkDisplayOnlyCompatibleLayouts"><cms:LocalizedLiteral runat="server" ResourceString="device_profile.layoutmapping.showmatchinglayoutsonly" ></cms:LocalizedLiteral></label>
                                </div>
                                <div class="SelectorFlatItems">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="SelectorEnvelope" style="overflow: hidden">
                                    <div class="SelectorFlatImage">
                                        <img alt="Template image" src="<%#LayoutSelector.GetFlatImageUrl(Eval("LayoutThumbnailGUID"))%>" />
                                    </div>
                                    <span class="SelectorFlatText">
                                        <%#HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(Eval("LayoutDisplayName"))))%></span>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div style="clear: both">
                                </div>
                                </div>
                            </FooterTemplate>
                        </cms:UniFlatSelector>
                        <div class="DeviceProfileLayoutMapping" style="position: relative;">
                            <div class="SelectorFlatDescription" style="overflow: hidden; position: absolute;
                                left: 0; right: 0; height: 55px">
                                <asp:Panel runat="server" ID="LayoutDescriptionPanel" CssClass="FlatDescription">
                                    <div class="SelectorName">
                                        <asp:Literal runat="server" ID="LayoutDescriptionLiteral" EnableViewState="false"></asp:Literal>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="ContentFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div id="__ButtonsArea">
        <div class="FloatRight">
            <cms:LocalizedButton ID="ConfirmButton" runat="server" CssClass="SubmitButton" EnableViewState="False"
                ResourceString="general.ok" OnClientClick="return Client_ConfirmSelection()" />
            <cms:LocalizedButton ID="CancelButton" runat="server" CssClass="SubmitButton" EnableViewState="False"
                ResourceString="general.cancel" OnClientClick="return CloseDialog()" />
        </div>
    </div>
</asp:Content>
