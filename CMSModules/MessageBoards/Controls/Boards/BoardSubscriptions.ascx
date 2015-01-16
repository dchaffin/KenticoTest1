<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Controls_Boards_BoardSubscriptions"
    CodeFile="BoardSubscriptions.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:UniGrid ID="boardSubscriptions" runat="server" GridName="~/CMSModules/MessageBoards/Tools/Boards/BoardSubscriptions.xml"
    Columns="SubscriptionID, SubscriptionUserID, SubscriptionEmail, SubscriptionApproved" OrderBy="SubscriptionEmail" ObjectType="board.subscription" />
<asp:Panel runat="server" ID="pnlSendConfirmationEmail" Visible="true">
    <div style="padding: 10px;">
    </div>
    <asp:CheckBox runat="server" ID="chkSendConfirmationEmail" Checked="true" />
    <cms:LocalizedLabel runat="server" ID="lblSendConfirmation" CssClass="ContentLabel"
        ResourceString="forums.forumsubscription.sendemail" />
</asp:Panel>
