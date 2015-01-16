<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_WebPartToolbar"
    CodeFile="WebPartToolbar.ascx.cs" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/SelectWebpart.ascx"
    TagName="SelectWebpart" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.ExtendedControls" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls.DragAndDrop" Assembly="CMS.ExtendedControls" %>
<div id="wptPanel" class="WPTPanel">
    <div id="wptLayout" class="<%=LayoutClass%> <%=PositionClass%>">
        <asp:Panel ID="pnlMinimized" CssClass="WPTMinimized" runat="server">
            <asp:Panel ID="pnlMaximize" runat="server" CssClass="WPTMaximize" >
                <img src="<%=maximizeUrl%>" alt="maximize" />
            </asp:Panel>
        </asp:Panel>
        <cms:CMSPanel ID="pnlMax" ShortID="m" runat="server" CssClass="WPTMaximized">
            <asp:Literal ID="ltrScript" runat="server" EnableViewState="false"></asp:Literal>

            <div class="WPTMenu">
                <div class="WPTMenuContent">
                    <cms:SelectWebpart ID="categorySelector" ShortID="c" runat="server" ShowWebparts="false"
                        EnableCategorySelection="true" EnableViewState="false" ShowRoot="true" />
                    <div class="WPTBtnPad">
                        <asp:ImageButton ID="btnConfigBtn" runat="server" EnableViewState="false" CssClass="WPTImgBtn" AlternateText="configure" />
                    </div>
                    <div class="WPTSeach">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="WPTSearchTxt" MaxLength="200"
                            EnableViewState="true" />
                        <div class="WPTBtnPad">
                           <img src="<%= GetImageUrl("CMSModules/CMS_Content/Search/Search.png") %>" class="WPTImgBtn"
                                alt="search" />
                        </div>
                    </div>
                </div>
                <asp:Panel ID="pnlMenuSep" runat="server" CssClass="WPTMenuSep">
                </asp:Panel>
            </div>

            <asp:Panel ID="pnlTrash" runat="server" CssClass="WPTTrash">
                <asp:Panel ID="pnlTrashSep" runat="server" CssClass="WPTTrashSep">
                </asp:Panel>
                <img src="<%= GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/garbage_empty.png") %>" class="WPTTrashIcon"
                    alt="recycle bin" />
                <asp:Panel ID="pnlMinimize" runat="server" CssClass="WPTMinimize">
                    <img src="<%=minimizeUrl%>" alt="minimize" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel ID="pnlScrollBack" runat="server" CssClass="WPTBackSlider">
                <img src="<%=scrollBackImgUrl %>" alt="scroll back" />
            </asp:Panel>
            <asp:Panel ID="pnlScrollForward" runat="server" CssClass="WPTForwardSlider">
                <img src="<%=scrollForwardImgUrl %>" alt="scroll forward" />
            </asp:Panel>
            <cms:DragAndDropExtender ID="trashExtender" runat="server" Enabled="true" HighlightDropableAreas="false">
            </cms:DragAndDropExtender>
            <input type="hidden" id="removeWebpartText" value="<%=ResHelper.GetString("portalmanager.confirmremovewebpart", CMSContext.CurrentUser.PreferredUICultureCode) %>" />
            <cms:CMSUpdatePanel ID="pnlU" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField ID="hdnUpdater" runat="server" />
                    <cms:ScrollPanel ID="scrollPanel" ShortID="sp" runat="server" CssClass="WPTItemsContainer"
                        ScrollAreaCssClass="WPTItemsRow" BackwardScrollerControlID="pnlScrollBack"
                        ForwardScrollerControlID="pnlScrollForward" InnerItemClass="WPTSelectorEnvelope" InnerItemSeparatorClass="WPTSelectorSeparator"
                        UseScrollersForPadding="true" EnableViewState="false">
                        <asp:Button runat="server" ID="btnLoadMore" />
                        <cms:QueryRepeater ID="repItems" runat="server" QueryName="cms.webpart.selectall"
                            OrderBy="WebPartDisplayName" ShortID="r" StopProcessing="true">
                            <ItemTemplate><asp:Panel ID="i" CssClass="WPTSelectorEnvelope" runat="server">
                                    <asp:Panel ID="wptHandle" CssClass="WPTHandle" runat="server">
                                        <asp:Literal ID="ltrImage" runat="server" ></asp:Literal>
                                        <div class="WPTItemTxt"><%# HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(Eval("WebPartDisplayName")), prefferedUICultureCode))%></div></asp:Panel></asp:Panel></ItemTemplate>
                            <SeparatorTemplate><div ID="s" runat="server" class="WPTSelectorSeparator">&nbsp;</div></SeparatorTemplate>
                        <FooterTemplate>
                            <div class="WPTSelectorEnvelope">
                                <div class="WPTHandle AppearElement"></div>
                            </div>
                        </FooterTemplate>
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
<div class="WPTTrashDropHelper <%=LayoutClass%>">
    <asp:Panel ID="pnlTrashDropArea" runat="server" CssClass="WPTTrashDropArea" EnableViewState="false">
    </asp:Panel>
    <asp:Panel ID="pnlTrashCue" runat="server" CssClass="WPTTrashCue" EnableViewState="false">
    </asp:Panel>
</div>
<cms:ModalPopupDialog runat="server" ID="ucPopupDialog" BackgroundCssClass="WPTDialogBackground"
    CssClass="WPTDialog" CancelControlID="btnPopupCancel">
    <asp:Panel ID="pnlDialogBody" runat="server" CssClass="WPTDialogBody">
        <cms:PageTitle ID="titleElem" runat="server" SetWindowTitle="false" TitleCssClass="WPTDialogHeader" />
        <asp:Panel ID="pnlContent" runat="server" CssClass="WPTDialogContent">
            <div>
                <div class="WPTDialogCaption">
                    <cms:LocalizedLabel ID="lblWptEnabled" runat="server" DisplayColon="true" AssociatedControlID="chkWptEnabled" />
                </div>
                <div class="WPTDialogValue">
                    <asp:CheckBox ID="chkWptEnabled" runat="server" Checked="true" />
                </div>
            </div>
            <div style="clear: both">
            </div>
            <div class="WPTDialogCaption">
                <cms:LocalizedLabel ID="lblWptPosition" runat="server" DisplayColon="true" />
            </div>
            <div class="WPTDialogValue">
                <asp:DropDownList ID="drpPosition" runat="server" CssClass="WPTCategories">
                </asp:DropDownList>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlFooterContainer" CssClass="WPTDialogFooter">
            <cms:LocalizedButton runat="server" ID="btnPopupOK" CssClass="WPTSubmitButton" UseSubmitBehavior="false"
                OnClientClick="if (!wptOnSave()) { return false; } " /><cms:LocalizedButton runat="server"
                    OnClientClick="wptDialogRestoreValues(); return false;" ID="btnPopupCancel" CssClass="WPTSubmitButton" />
        </asp:Panel>
    </asp:Panel>
</cms:ModalPopupDialog>
<div id="wptLoader" class="WPTLoader">
    <div class="WPTLoaderBackground">
    </div>
    <asp:Panel ID="pnlLoader" runat="server" CssClass="WPTLoaderBox">
    </asp:Panel>
</div>
