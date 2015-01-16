<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditIssue.ascx.cs" Inherits="CMSModules_Newsletters_Controls_EditIssue" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/Newsletter_ContentEditor.ascx"
    TagPrefix="cms" TagName="Newsletter_ContentEditor" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/NewsletterTemplateSelector.ascx"
    TagPrefix="cms" TagName="NewsletterTemplateSelector" %>
<%-- Issue base properties --%>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="topPanel" class="PageContent PageHeaderLine" onmouseover="RememberFocusedRegion(); return false;">
            <div class="FloatLeft" style="width:75%">
                <table style="width:100%">
                    <tr>
                        <td>
                            <cms:LocalizedLabel ID="lblSubject" runat="server" CssClass="FieldLabel" ResourceString="general.subject"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="txtSubject" />
                        </td>
                        <td style="width:100%">
                            <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="450" style="width:100%" />
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="plcAdvanced" runat="server" Visible="false">
                        <tr>
                            <td>
                                <cms:LocalizedLabel ID="lblSenderName" runat="server" CssClass="FieldLabel" ResourceString="Sender name"
                                    DisplayColon="true" EnableViewState="false" AssociatedControlID="txtSenderName" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtSenderName" runat="server" CssClass="TextBoxField" MaxLength="200" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cms:LocalizedLabel ID="lblSenderEmail" runat="server" CssClass="FieldLabel" ResourceString="Sender e-mail address"
                                    DisplayColon="true" EnableViewState="false" AssociatedControlID="txtSenderEmail" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtSenderEmail" runat="server" CssClass="TextBoxField" MaxLength="200" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cms:LocalizedLabel ID="lblTemplate" runat="server" CssClass="FieldLabel" ResourceString="Template"
                                    DisplayColon="true" EnableViewState="false" AssociatedControlID="issueTemplate" />
                            </td>
                            <td>
                                <cms:NewsletterTemplateSelector ID="issueTemplate" runat="server" AutoPostBack="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cms:LocalizedLabel ID="lblArchive" runat="server" CssClass="FieldLabel" ResourceString="newslettertemplate_edit.showinarchive"
                                    DisplayColon="true" EnableViewState="false" AssociatedControlID="chkShowInArchive" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" ID="chkShowInArchive" TextAlign="Left" />
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </table>
            </div>
            <div class="FloatRight" style="margin:4px">
                <asp:Image runat="server" ID="imgToggleAdvanced" CssClass="NewItemImage" EnableViewState="false" />
                <asp:LinkButton ID="lnkToggleAdvanced" runat="server" OnClick="lnkToggleAdvanced_Click" />
            </div>
            <div class="ClearBoth">
                &nbsp;</div>
            <asp:HiddenField ID="hdnTemplateID" runat="server" EnableViewState="false" />
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="lnkToggleAdvanced" EventName="click" />
    </Triggers>
</cms:CMSUpdatePanel>
<%-- Newletter issue content editor --%>
<cms:CMSUpdatePanel ID="pnlBodyUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:Newsletter_ContentEditor ID="contentBody" runat="server" ShortID="ce" />
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="issueTemplate" />
    </Triggers>
</cms:CMSUpdatePanel>
<asp:HiddenField ID="hdnIssueContent" runat="server" EnableViewState="false" />
