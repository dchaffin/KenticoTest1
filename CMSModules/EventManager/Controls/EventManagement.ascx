<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSModules/EventManager/Controls/EventManagement.ascx.cs"
    Inherits="CMSModules_EventManager_Controls_EventManagement" %>
<%@ Register Src="~/CMSModules/EventManager/Controls/EventAttendees.ascx" TagName="AttendeesList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/EventManager/Controls/EventList.ascx" TagName="EventList"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/EventManager/Controls/EventAttendeesSendEmail.ascx"
    TagName="EMailSender" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActionsControl"
    TagPrefix="cms" %>
<asp:HiddenField ID="hdnEventID" runat="server" />
<cms:EventList ID="eventList" runat="server" />
<asp:Panel runat="server" ID="pnlAttendees" Visible="false">
    <asp:Panel ID="pnlHeader" runat="server" CssClass="PageTitleBreadCrumbsPadding">
        <div class="HeaderTitleBreadcrumbs">
            <asp:Panel ID="pnlNewHeader" runat="server">
                <asp:LinkButton ID="lnkEditBack" runat="server" CausesValidation="false" />
                <asp:Label ID="lblEditBack" runat="server" />
                <asp:Label ID="lblEditNew" runat="server" /><br />
            </asp:Panel>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlTabsContainer" CssClass="TabsHeader LightTabs">
        <div class="TabsLeft"></div>
        <asp:Panel runat="server" ID="pnlTabs" CssClass="TabsTabs">
                <cms:UITabs ID="tabControlElem" runat="server" UseClientScript="true" OnOnTabClicked="tabControlElem_clicked" />                
        </asp:Panel>
    </asp:Panel>
    <div class="TabsContent" >
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" LiveSiteOnly="true" />
        <cms:AttendeesList ID="attendeesList" runat="server" />
        <asp:Panel runat="server" ID="pnlSendEmail" Visible="false">
            <br/>
            <cms:HeaderActionsControl ID="plcHeaderActions" runat="server" LiveSiteOnly="true" />
            <div class="ClearBoth"></div>
            <br/>
            <cms:EMailSender ID="emailSender" runat="server" />
        </asp:Panel>
    </div>
</asp:Panel>
