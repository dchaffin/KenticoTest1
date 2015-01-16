<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Objects_Controls_Locking_ObjectEditMenu"
    CodeFile="ObjectEditMenu.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Panel ID="pnlMenu" runat="server">
    <cms:HeaderActions ID="menu" runat="server" UseSmallIcons="true" />
    <asp:Panel ID="pnlHelp" runat="server" CssClass="RightAlign" Visible="false" EnableViewState="false">
        <div class="EditMenuHelp">
            <cms:Help ID="helpElem" runat="server" TopicName="page_tab" HelpName="helpTopic"
                EnableViewState="false" />
        </div>
    </asp:Panel>
</asp:Panel>
<asp:Panel ID="pnlInfoWrapper" runat="server" CssClass="ObjectEditMenuInfoWrapper"
    EnableViewState="false" Visible="false">
    <asp:Panel ID="pnlInfo" runat="server" CssClass="ObjectEditMenuInfo" EnableViewState="false">
        <asp:Label runat="server" ID="lblInfo" Visible="false" />
    </asp:Panel>
</asp:Panel>
