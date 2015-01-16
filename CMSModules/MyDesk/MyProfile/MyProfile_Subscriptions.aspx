<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Inherits="CMSModules_MyDesk_MyProfile_MyProfile_Subscriptions" Theme="Default"
    CodeFile="MyProfile_Subscriptions.aspx.cs" %>

<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/Membership/Controls/Subscriptions.ascx" TagName="Subscriptions"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel runat="server" ID="pnlSubscription" CssClass="MySubscriptions">
        <asp:Panel runat="server" ID="pnlHeaderSeparator" CssClass="HeaderSeparator">
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlTabsContainer" CssClass="VerticalTabsPanel" EnableViewState="false">
            <asp:Panel runat="server" ID="pnlLeft" CssClass="VerticalTabsTop">
                &nbsp;
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlTabs" CssClass="VerticalTabsTabs">
                <asp:Panel runat="server" ID="pnlVertical" CssClass="TabsVertical">
                    <cms:UITabs ID="tabControlElem" ShortID="t" runat="server" TabControlLayout="Vertical"
                        UseClientScript="true" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlRight" CssClass="VerticalTabsBottom">
                &nbsp;
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlContent" CssClass="PageContent">
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" WrapperControlID="pnlContent" />
            <cms:Subscriptions ID="elemSubscriptions" runat="server" IsLiveSite="false" />
        </asp:Panel>

    </asp:Panel>
</asp:Content>
