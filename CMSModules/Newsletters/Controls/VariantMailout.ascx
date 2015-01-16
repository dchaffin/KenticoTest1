<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VariantMailout.ascx.cs"
    Inherits="CMSModules_Newsletters_Controls_VariantMailout" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:CMSUpdatePanel ID="pnlUM" runat="server">
    <ContentTemplate>
        <asp:Panel runat="server" ID="pnlMailout">
            <asp:Panel runat="server" ID="pnlMailoutContent" CssClass="PageContent">
                <cms:UniGrid runat="server" ID="grdElem" ShortID="g" IsLiveSite="false" ObjectType="newsletter.issuevariant"
                    Columns="IssueID, IssueVariantName, IssueScheduledTaskID, IssueMailoutTime, IssueOpenedEmails, IssueStatus"
                    ShowActionsMenu="false" DelayedReload="true">
                    <GridActions Parameters="IssueID">
                        <ug:Action Name="selwinner" Caption="$newsletterissue_send.selectwinneraction$" Icon="winner.png"
                            OnClick="SelWinner({0}); return false;" />
                    </GridActions>
                    <GridColumns>
                        <ug:Column Source="##ALL##" ExternalSourceName="IssueVariantName" Sort="IssueVariantName" Caption="$newsletterissue_send.variantname$"
                            Wrap="false">
                        </ug:Column>
                        <ug:Column Source="##ALL##" ExternalSourceName="MailoutTime" Caption="$unigrid.newsletter_issue.columns.issuemailouttime$"
                            Wrap="false" />
                        <ug:Column Source="##ALL##" Caption="$unigrid.newsletter_issue.columns.issueopenedemails$"
                            Wrap="false" ExternalSourceName="IssueOpenedEmails" Name="opened" Sort="IssueOpenedEmails" />
                        <ug:Column Source="IssueID" Caption="$newsletterissue_mailout.uniqueclicks$" Wrap="false"
                            ExternalSourceName="UniqueClicks" Name="clicks" />
                        <ug:Column Width="100%" />
                        <ug:Column Source="IssueStatus" ExternalSourceName="IssueStatus" Caption="$newsletterissue_mailout.status$"
                            Wrap="false" Name="status" />
                        <ug:Column Source="IssueID" Wrap="false" Name="issueid" Visible="false" />
                    </GridColumns>
                    <GridOptions ShowSelection="true" DisplayFilter="false" />
                </cms:UniGrid>
                <asp:Panel ID="pMOut" runat="server">
                    <br />
                    <table>
                        <tr>
                            <td>
                                <cms:LocalizedLabel runat="server" ID="lblMailoutTime" ResourceString="newsletterissue_send.mailouttime"
                                    EnableViewState="false" />
                            </td>
                            <td>
                                <cms:LocalizedDropDownList runat="server" ID="drpAllSelected" CssClass="SmallDropDown" />
                            </td>
                            <td>
                                <cms:LocalizedLabel runat="server" ID="lblTo" ResourceString="newsletterissue_send.to"
                                    EnableViewState="false" />
                            </td>
                            <td>
                                <cms:DateTimePicker runat="server" ID="dtpMailout" SupportFolder="~/CMSAdminControls/Calendar" />
                            </td>
                            <td>
                                <cms:LocalizedButton runat="server" ID="btnOk" EnableViewState="false" CssClass="SubmitButton"
                                    ResourceString="general.ok" OnClick="btnOk_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
