<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_PageLayouts_PageLayout_CustomDeviceLayout"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="New device layout" CodeFile="PageLayout_CustomDeviceLayout.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelectorControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSPanel ID="pnlContent" runat="server" CssClass="DialogContentPanel">
        <div class="DeviceLayoutSection">
            <cms:CMSPanel ID="pnlNewDevice" Visible="false" runat="server" GroupingTextResourceString="devicelayout.createdevicelayout">
                <asp:Panel ID="pnlNewDeviceSelector" runat="server" CssClass="DeviceLayoutPadding">
                    <cms:UniSelectorControl ID="ucNewDeviceProfile" ShortID="ndps" ObjectType="CMS.DeviceProfile"
                        runat="server" ReturnColumnName="ProfileID" SelectionMode="SingleDropDownList"
                        AllowEmpty="false" IsLiveSite="false" />
                </asp:Panel>
            </cms:CMSPanel>
            <cms:LocalizedLabel ID="lblNewDevice" runat="server" CssClass="DeviceLayoutLabel"></cms:LocalizedLabel>
        </div>
        <br />
        <cms:CMSPanel ID="sdfsf" runat="server" CssClass="DeviceLayoutSection" GroupingTextResourceString="devicelayout.selectlayoutsource">
            <cms:LocalizedRadioButton ID="rbtnDevice" runat="server" ResourceString="devicelayout.copyfromdevice"
                GroupName="rbtnTypes" />
            <asp:Panel ID="pnlDevice" runat="server" CssClass="DeviceLayoutPadding">
                <cms:UniSelectorControl ID="ucDeviceProfile" ShortID="dps" ObjectType="CMS.DeviceProfile"
                    runat="server" ReturnColumnName="ProfileID" SelectionMode="SingleDropDownList"
                    AllowEmpty="false" IsLiveSite="false" />
            </asp:Panel>
            <cms:LocalizedRadioButton ID="rbtnLayout" runat="server" ResourceString="devicelayout.useexistinglayout"
                GroupName="rbtnTypes" />
            <asp:Panel ID="pnlLayout" runat="server" CssClass="DeviceLayoutPadding">
                <cms:UniSelectorControl ID="ucLayout" ShortID="ls" ObjectType="CMS.Layout" runat="server"
                    AllowEmpty="false" ReturnColumnName="LayoutID" SelectionMode="SingleDropDownList"
                    IsLiveSite="false" />
                <div class="DeviceLayoutPaddingCheckbox">
                    <cms:LocalizedCheckBox ID="chkCopy" runat="server" Checked="true" ResourceString="devicelayout.copyascustom"
                        ToolTipResourceString="devicelayout.copyasnew.tooltip" />
                </div>
            </asp:Panel>
            <cms:LocalizedRadioButton ID="rbtnEmptyLayout" runat="server" ResourceString="devicelayout.useemptylayout"
                GroupName="rbtnTypes" />
            <div class="DeviceLayoutPadding">
            </div>
        </cms:CMSPanel>
    </cms:CMSPanel>
</asp:Content>
