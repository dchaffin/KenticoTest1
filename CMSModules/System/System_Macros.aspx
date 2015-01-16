<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_System_Macros" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - Macros" CodeFile="System_Macros.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="AsyncBackground" Src="~/CMSAdminControls/AsyncBackground.ascx" %>
<%@ Register TagPrefix="cms" TagName="AsyncControl" Src="~/CMSAdminControls/AsyncControl.ascx" %>
<%@ Register TagPrefix="cms" TagName="PageTitle" Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlRefreshSecurityParams" runat="server">
        <cms:localizedlabel ID="Localizedlabel1" runat="server" resourcestring="macros.refreshsecurityparams.description" />
        <%-- Form --%>
        <table style="margin-top: 10px;">
            <tbody>
                <%-- Old salt --%>
                <tr>
                    <td>
                        <cms:localizedlabel ID="Localizedlabel2" runat="server" resourcestring="macros.refreshsecurityparams.oldsalt" displaycolon="true" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtOldSalt" runat="server" CssClass="TextBoxField" Width="400" />
                        <cms:localizedcheckbox id="chkRefreshAll" runat="server" resourcestring="macros.refreshsecurityparams.refreshall" tooltipresourcestring="macros.refreshsecurityparams.refreshalltooltip" checked="false" autopostback="true" />
                    </td>
                </tr>
                <%-- New salt --%>
                <tr>
                    <td>
                        <cms:localizedlabel ID="Localizedlabel3" runat="server" resourcestring="macros.refreshsecurityparams.newsalt" displaycolon="true" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtNewSalt" runat="server" CssClass="TextBoxField" Width="400" />
                        <cms:localizedcheckbox id="chkUseCurrentSalt" runat="server" resourcestring="macros.refreshsecurityparams.usecurrentsalt" checked="true" autopostback="true" />
                    </td>
                </tr>
            </tbody>
        </table>
        <%-- Submit --%>
        <div style="margin-top: 10px;">
            <cms:cmsbutton id="btnRefreshSecurityParams" runat="server" cssclass="XXLongSubmitButton" />
        </div>
    </asp:Panel>
    <%-- Async log --%>
    <asp:PlaceHolder ID="plcAsyncLog" runat="server" Visible="false">
        <cms:asyncbackground ID="Asyncbackground1" runat="server" />
        <div class="AsyncLogArea">
            <div class="PageBody">
                <div class="PageHeader">
                    <cms:pagetitle id="ucTitle" runat="server" />
                </div>
                <div class="PageHeaderLine">
                    <cms:cmsbutton id="btnCancel" runat="server" cssclass="SubmitButton" />
                </div>
                <div class="PageContent">
                    <cms:asynccontrol id="ucAsync" runat="server" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
