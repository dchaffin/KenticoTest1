<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditGroup.ascx.cs" Inherits="CMSAdminControls_UI_UniMenu_UniGraphToolbar_EditGroup" %>
<%@ Register Src="~/CMSAdminControls/UI/UniMenu/UniMenuButtons.ascx" TagName="UniMenuButtons"
    TagPrefix="cms" %>
<div>
    <div class="UniMenuLeftBorder">
    </div>
    <div class="UniMenuContent">
        <div class="ActionSearch">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="ActionSearchTxt" MaxLength="200"
                EnableViewState="true" />
            <div class="SearchBtnPad">
                <asp:LinkButton runat="server" ID="btnSearch" />
            </div>
        </div>
        <div class="FloatLeft">
            <cms:UniMenuButtons ID="btnsLeft" runat="server" />
        </div>
        <div class="FloatRight">
            <cms:UniMenuButtons ID="btnsRight" runat="server" />
        </div>
    </div>
    <div class="UniMenuRightBorder">
    </div>
</div>
<div class="UniMenuSeparator">
</div>
