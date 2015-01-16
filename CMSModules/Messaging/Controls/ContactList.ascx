<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Messaging_Controls_ContactList"
    CodeFile="ContactList.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlContactList" runat="server">
    <ContentTemplate>
        <div class="MessagingBox">
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
            <div class="ListPanel">
                <cms:LocalizedLabel ID="lblAvialable" runat="server" CssClass="BoldInfoLabel" ResourceString="contactlist.available"
                    DisplayColon="true" EnableViewState="false" />
                <cms:SelectUser ID="usUsers" runat="server" SelectionMode="Multiple" />
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
