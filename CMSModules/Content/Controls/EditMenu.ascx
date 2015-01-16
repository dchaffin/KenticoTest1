<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_EditMenu"
    CodeFile="EditMenu.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<cms:CMSPanel ID="pnlContainer" ShortID="pC" runat="server">
    <asp:Panel ID="pnlMenu" runat="server" CssClass="LeftAlign">
        <cms:HeaderActions ID="menu" runat="server" />
    </asp:Panel>
    <asp:Label runat="server" ID="lblInfo" Visible="false" EnableViewState="false" />
    <asp:Panel ID="pnlHelp" runat="server" CssClass="RightAlign" Visible="false" EnableViewState="false">
        <div class="EditMenuHelp">
            <cms:Help ID="helpElem" runat="server" TopicName="page_tab" HelpName="helpTopic"
                EnableViewState="false" />
        </div>
    </asp:Panel>
    <asp:PlaceHolder ID="plcEmails" runat="server" Visible="false">
        <asp:Panel ID="pnlEmails" runat="server" CssClass="RightAlign AdditionalControls">
            <cms:LocalizedCheckBox ID="chkEmails" CssClass="CheckBoxField NotCheckChanges" runat="server" ResourceString="WorfklowProperties.SendMail" Checked="true" />
        </asp:Panel>
    </asp:PlaceHolder>
    <div class="Clear">
    </div>
</cms:CMSPanel>
<cms:CMSDocumentPanel ID="pnlDoc" runat="server" ShortID="pD" />
<asp:HiddenField ID="hdnParam" runat="server" EnableViewState="false" />
