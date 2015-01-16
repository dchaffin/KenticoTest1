<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_AdminControls_Controls_Class_FormBuilder_ControlSelector" CodeFile="ControlSelector.ascx.cs" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/SelectWebpart.ascx"
    TagName="SelectWebpart" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.ExtendedControls" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls.DragAndDrop" Assembly="CMS.ExtendedControls" %>
<%@ Register TagPrefix="cms" TagName="ControlTypeSelector" Src="~/CMSFormControls/System/UserControlTypeSelector.ascx" %>
<div id="wptPanel" class="WPTPanel">
    <div id="wptLayout" class="WPTPositionTop ">
        <div class="WPTSeparator">
            <asp:Panel ID="pnlMinimize" runat="server" CssClass="WPTMinimize">
                <img src="<%= GetImageUrl("Design/Controls/FrameResizer/Vertical/minimize.png")%>"
                    alt="" onclick="wptMinimize();" />
            </asp:Panel>
            <asp:Panel ID="pnlMaximize" Style="display: none;" runat="server" CssClass="WPTMaximize">
                <img src="<%= GetImageUrl("Design/Controls/FrameResizer/Vertical/maximize.png")%>"
                    alt="" onclick="wptMaximize();" />
            </asp:Panel>
        </div>
        <cms:CMSPanel ID="pnlMax" ShortID="m" runat="server" CssClass="WPTMaximized">
            <div class="fbLeftCategory">
                <asp:Panel ID="pnlCategoryDraggable" CssClass="WPTSelectorEnvelope" runat="server">
                    <asp:Panel ID="pnlCategoryItem" CssClass="WPTItem" runat="server">
                        <asp:Panel ID="WPTCategoryHandle" CssClass="WPTHandle" runat="server">
                            <asp:Literal ID="ltrCategoryImage" runat="server"></asp:Literal>
                            <div class="WPTItemTxt">
                                Category</div>
                        </asp:Panel>
                    </asp:Panel>
                    <asp:Panel ID="plcCatCue" runat="server">
                        <div style="display: none;">
                            <asp:Panel runat="server" ID="pnlCatCue" EnableViewState="false" Width="0" Height="0">
                            </asp:Panel>
                        </div>
                    </asp:Panel>
                    <cms:DragAndDropExtender ID="categoryDragDropExtender" runat="server" Enabled="true"
                        HighlightDropableAreas="false" />
                </asp:Panel>
                <asp:Panel ID="pnlCategorySep" runat="server" style="left: 66px; position:absolute;" CssClass="WPTMenuSep">
                </asp:Panel>
            </div>
            <div class="WPTMenu fbMenu">
                <div class="WPTMenuContent">
                    <cms:ControlTypeSelector CssClass="ShortDropDownList WPTCategories" IncludeAllItem="true"
                        runat="server" ID="ddTypeSelector" />
                    <div class="WPTSeach">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="WPTSearchTxt" MaxLength="200"
                            EnableViewState="true" />
                        <div class="WPTBtnPad">
                            <img src="<%= GetImageUrl("CMSModules/CMS_Content/Search/Search.png") %>" class="WPTImgBtn"
                                alt="" />
                        </div>
                    </div>
                </div>
                <asp:Panel ID="pnlMenuSep" runat="server" CssClass="WPTMenuSep">
                </asp:Panel>
            </div>
            <div class="WPTTrash">
                <asp:Panel ID="pnlTrashSep" runat="server" CssClass="WPTTrashSep">
                </asp:Panel>
                <asp:Image ID="imgTrash" runat="server" />
            </div>
            <asp:Panel ID="pnlScrollBack" runat="server" CssClass="WPTBackSlider fbBackSlider">
                <img src="<%= GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/ArrowLeft.png") %>"
                    alt="" />
            </asp:Panel>
            <asp:Panel ID="pnlScrollForward" runat="server" CssClass="WPTForwardSlider">
                <img src="<%= GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/ArrowRight.png") %>"
                    alt="" />
            </asp:Panel>
            <cms:CMSUpdatePanel ID="pnlU" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField ID="hdnUpdater" runat="server" />
                    <cms:ScrollPanel ID="scrollPanel" ShortID="sp" runat="server" CssClass="WPTItemsContainer"
                        ScrollAreaCssClass="fbItemsRow" BackwardScrollerControlID="pnlScrollBack" ForwardScrollerControlID="pnlScrollForward"
                        InnerItemClass="WPTSelectorEnvelope" InnerItemSeparatorClass="WPTSelectorSeparator"
                        UseScrollersForPadding="true">
                        <cms:QueryRepeater ID="repItems" runat="server" QueryName="cms.formusercontrol.selectall"
                            OrderBy="UserControlDisplayName" ShortID="r" Columns="UserControlID, UserControlDisplayName, UserControlDescription, UserControlThumbnailGUID"
                            StopProcessing="true">
                            <ItemTemplate>
                                <asp:Panel ID="i" CssClass="WPTSelectorEnvelope" runat="server">
                                    <asp:Panel ID="wptHandle" CssClass="WPTHandle" runat="server">
                                        <asp:Literal ID="ltrImage" runat="server"></asp:Literal>
                                        <div class="WPTItemTxt">
                                            <%# HTMLHelper.HTMLEncode(Convert.ToString(Eval("UserControlDisplayName")))%></div>
                                    </asp:Panel>
                                </asp:Panel>
                            </ItemTemplate>
                            <SeparatorTemplate>
                                <div class="WPTSelectorSeparator">
                                    &nbsp;</div>
                            </SeparatorTemplate>
                        </cms:QueryRepeater>
                    </cms:ScrollPanel>
                    <asp:PlaceHolder runat="server" ID="plcDrag" EnableViewState="false">
                        <div style="display: none;">
                            <asp:Panel runat="server" ID="pnlCue" EnableViewState="false" Width="0" Height="0">
                            </asp:Panel>
                        </div>
                        <cms:DragAndDropExtender ID="ddExtender" runat="server" Enabled="true" HighlightDropableAreas="false">
                        </cms:DragAndDropExtender>
                        <cms:DragAndDropExtender ID="ddExtenderHovered" runat="server" Enabled="true" HighlightDropableAreas="false">
                        </cms:DragAndDropExtender>
                    </asp:PlaceHolder>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </cms:CMSPanel>
    </div>
</div>
