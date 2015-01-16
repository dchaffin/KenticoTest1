<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Controls_VariantSlider"
    CodeFile="VariantSlider.ascx.cs" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/VariantDialog.ascx" TagPrefix="cms"
    TagName="VariantDialog" %>
<cms:CMSUpdatePanel ID="pnlu" runat="server" EnableViewState="false">
    <ContentTemplate>
        <asp:PlaceHolder ID="plcSliderPanel" runat="server">
            <div class="SliderItem">
                <div class="SliderPartLabel">
                    <div class="FloatLeft">
                        <cms:LocalizedLabel ID="lblABVar" runat="server" ResourceString="newsletter.lblabvariant"
                            DisplayColon="true" />
                    </div>
                </div>
            </div>
            <div class="SliderItemSeparator">
            </div>
            <div class="FloatLeft">
                <asp:Panel ID="pnlSlider" runat="server" CssClass="SliderBarPanel">
                    <cms:CMSTextBox ID="txtSlider" runat="server" EnableViewState="false" />
                    <ajaxToolkit:SliderExtender ID="sliderExtender" runat="server" TargetControlID="txtSlider"
                        BoundControlID="hdnSliderPosition" EnableHandleAnimation="true" Length="150"
                        EnableViewState="false">
                    </ajaxToolkit:SliderExtender>
                    <cms:CMSTextBox ID="hdnSliderPosition" runat="server" EnableViewState="false" />
                </asp:Panel>
            </div>
            <div class="SliderItemSeparator">
            </div>
            <div class="SliderItem">
                <div class="SliderPartLabel">
                    <div class="FloatLeft">
                        <asp:Label ID="lblPart" runat="server" Text="1" />
                    </div>
                    <div class="FloatLeft">
                        /</div>
                    <div class="FloatLeft">
                        <asp:Label ID="lblTotal" runat="server" Text="1" />
                    </div>
                </div>
            </div>
            <div class="SliderItemSeparator">
            </div>
        </asp:PlaceHolder>
        <div class="SliderItem">
            <asp:ImageButton ID="btnDummy" runat="server" style="position:absolute;left:-1000px;top:-1000px;width:0px;height:0px;" OnClientClick="return false;" />
        </div>
        <asp:PlaceHolder ID="plcAddVariant" runat="server">
            <div class="SliderItem">
                <div class="SliderItemPadding">
                    <asp:ImageButton ID="imgAddVariant" runat="server" CssClass="SliderBtn" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcRemoveVariant" runat="server">
            <div class="SliderItem">
                <div class="SliderItemPadding">
                    <asp:ImageButton ID="imgRemoveVariant" runat="server" CssClass="SliderBtnEnabled" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcVariantProperties" runat="server">
            <div class="SliderItem">
                <div class="SliderItemPadding">
                    <asp:ImageButton ID="imgVariantProperties" runat="server" CssClass="SliderBtn" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="SliderItemSeparator">
        </div>
        <div class="SliderItem">
            <div class="SliderPartLabel">
                <div class="FloatLeft">
                    <asp:Label ID="lblVariantName" runat="server" />
                </div>
            </div>
        </div>
        <cms:VariantDialog ID="variantDailog" runat="server" />
        <asp:Button ID="btnSubmit" runat="server" CssClass="HiddenButton" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<div class="ClearBoth">
</div>
