<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Edit_Edit"
    ValidateRequest="false" Theme="Default" EnableEventValidation="false" CodeFile="Edit.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ID="cntMenu" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" ShortID="m" runat="server" ShowProperties="false" ShowSpellCheck="true"
        IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <div id="CMSHeaderDiv">
        <div id="CKToolbar">
        </div>
    </div>
    <asp:Panel runat="server" ID="pnlContent" CssClass="ContentEditArea">
        <cms:CMSForm runat="server" ID="formElem" Visible="false" HtmlAreaToolbarLocation="Out:CKToolbar"
            ShowOkButton="false" IsLiveSite="false" ShortID="f" />
        <span class="ClearBoth"></span>
        <br />
        <%-- SKU binding --%>
        <asp:PlaceHolder ID="plcSkuBinding" runat="server" Visible="false">
            <div class="PageSeparator">
                <cms:LocalizedLabel ID="lblBindSKUInfo" runat="server" ResourceString="com.bindAnSkuInfo"
                    CssClass="InfoLabel EditingFormLabel" EnableViewState="false" />
                <asp:LinkButton ID="btnBindSku" runat="server" CssClass="ContentNewLink" EnableViewState="false">
                    <cms:CMSImage ID="imgBindSku" runat="server" EnableViewState="false" />
                    <cms:LocalizedLabel ID="lblBindSKU" runat="server" ResourceString="com.skubinding.bind"
                        EnableViewState="false" />
                </asp:LinkButton>
                <span class="ClearBoth"></span>
            </div>
        </asp:PlaceHolder>
    </asp:Panel>
    <cms:CMSButton ID="btnRefresh" runat="server" CssClass="HiddenButton" EnableViewState="false"
        OnClick="btnRefresh_Click" UseSubmitBehavior="false" />
    <asp:PlaceHolder ID="plcCKFooter" runat="server">
        <div id="CMSFooterDiv">
            <div id="CKFooter">
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
