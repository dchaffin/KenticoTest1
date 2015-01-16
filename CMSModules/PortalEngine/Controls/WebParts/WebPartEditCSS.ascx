<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WebPartEditCSS.ascx.cs"
    Inherits="CMSModules_PortalEngine_Controls_WebParts_WebPartEditCSS" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
    <ContentTemplate>
        <asp:Button CssClass="Hidden" runat="server" ID="btnAction" OnClick="btnAction_Clicked" />
        <div runat="server" id="pnlMenu" class="PreviewMenu">
            <div class="CMSEditMenu">
                <cms:HeaderActions runat="server" ID="headerActions" IsLiveSite="false" PerformFullPostBack="false" />
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:Panel ID="pnlContent" runat="server" CssClass="PreviewDefaultContent">
    <cms:MessagesPlaceHolder runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false"
        OffsetY="10" />
    <div class="PageContent">
        <table style="width: 100%;" cellpadding="0">
            <tr>
                <td>
                    <cms:LocalizedLabel runat="server" ID="lblCss" ResourceString="Container_Edit.ContainerCSS"
                        DisplayColon="true" EnableViewState="false" />
                    <br />
                    <cms:ExtendedTextArea ID="etaCSS" runat="server" EnableViewState="true" EditorMode="Advanced"
                        Language="CSS" Width="98%" Height="500px" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
