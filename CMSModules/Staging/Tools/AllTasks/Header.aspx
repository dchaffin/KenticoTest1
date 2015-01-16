<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Staging_Tools_AllTasks_Header"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" CodeFile="Header.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Staging/FormControls/ServerSelector.ascx" TagName="ServerSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/FrameResizer.ascx" TagName="FrameResizer"
    TagPrefix="cms" %>
<asp:Content ID="plcContent" runat="server" ContentPlaceHolderID="plcContent">
    <cms:frameresizer id="frmResizer" runat="server" minsize="6, *" vertical="True" />
    <asp:Panel ID="pnlTitle" runat="server" CssClass="PageHeader">
        <cms:pagetitle id="titleElem" runat="server" helptopicname="staging_all_tasks" />
    </asp:Panel>
    <asp:Panel ID="pnlServers" runat="server" CssClass="PageHeaderLine SiteHeaderLine">
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <cms:localizedlabel id="lblServers" runat="server" enableviewstate="false" resourcestring="Tasks.SelectServer" />
                    &nbsp;
                </td>
                <td>
                    <cms:serverselector id="selectorElem" runat="server" islivesite="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div class="HeaderSeparator">
    </div>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
