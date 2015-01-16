<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Scheduler_Pages_Tab_SystemTasks"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Scheduled tasks - Task List"
    CodeFile="Tab_SystemTasks.aspx.cs" %>

<%@ Register Src="~/CMSModules/Scheduler/Controls/UI/List.ascx" TagName="TaskList" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="CMSEditMenu">
                <cms:HeaderActions ID="headerActions" runat="server" PanelCssClass="FloatLeft" />                
                <cms:HeaderActions ID="rightHeaderActions" runat="server" PanelCssClass="FloatRight" />
                <asp:Label ID="lblLastRun" runat="server" EnableViewState="false" CssClass="FloatRight ActionsInfoLabel" />
            </div>
            <asp:Panel ID="PanelUsers" runat="server" CssClass="PageContent">
                <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
                <cms:TaskList ID="listElem" runat="server" SystemTasks="true" />
            </asp:Panel>
            <asp:Button runat="server" ID="btnRefresh" CssClass="HiddenButton" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
