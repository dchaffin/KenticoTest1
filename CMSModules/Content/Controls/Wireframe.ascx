<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Wireframe.ascx.cs" Inherits="CMSModules_Content_Controls_Wireframe" %>
<%@ Register Src="~/CMSModules/PortalEngine/FormControls/PageTemplates/PageTemplateLevels.ascx"
    TagName="PageTemplateLevel" TagPrefix="cms" %>
<asp:Panel ID="pnlForm" runat="server">
    <cms:CMSUpdatePanel ID="ucPanel" runat="server">
        <ContentTemplate>
            <cms:LocalizedLabel ID="lblComment" runat="server" EnableViewState="false" ResourceString="Wireframe.Comment"
                DisplayColon="true" /><br />
            <br />
            <cms:CMSTextBox ID="txtComment" runat="server" CssClass="TextAreaField" TextMode="MultiLine"
                Width="600px" Height="250px" />
            <br />
            <br />
            <asp:Panel runat="server" ID="pnlInherits" CssClass="NodePermissions">
                <cms:PageTemplateLevel runat="server" ID="inheritElem" />
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
