<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Newsletter_Issue_Send.aspx.cs"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Send" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/SendIssue.ascx" TagPrefix="cms"
    TagName="SendIssue" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/SendVariantIssue.ascx" TagPrefix="cms"
    TagName="SendVariant" %>
<asp:Content ID="cntBefore" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <div class="PageContent">
        <cms:CMSUpdatePanel ID="pnlU" runat="server">
            <ContentTemplate>
                <asp:Label runat="server" ID="lblInfo" CssClass="AdvancedLabel" Style="margin: 0"
                    EnableViewState="false" Visible="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:SendIssue ID="sendElem" runat="server" ShowScheduler="true" ShowSendDraft="true"
        ShowSendLater="false" ShortID="s" Visible="false" />
    <cms:SendVariant ID="sendVariant" runat="server" ShortID="sv" Visible="false" Mode="Send" />
</asp:Content>
