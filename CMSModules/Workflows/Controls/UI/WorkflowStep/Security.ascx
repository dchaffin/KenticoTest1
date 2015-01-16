<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Controls_UI_WorkflowStep_Security"
    CodeFile="Security.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<table style="width: 100%">
    <tr class="RowTop">
        <td class="ColumnPanel">
            <cms:CMSUpdatePanel ID="pnlUpdateRoles" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnlRoles" runat="server">
                        <div class="Height80">
                            <asp:RadioButtonList ID="rbRoleType" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" />
                            <asp:PlaceHolder ID="plcRolesBox" runat="server">
                                <br />
                                <cms:LocalizedLabel ID="lblSite" runat="server" EnableViewState="false" ResourceString="siteselect.selectitem"
                                    DisplayColon="true" />
                                <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                            </asp:PlaceHolder>
                        </div>
                        <cms:UniSelector ID="usRoles" runat="server" IsLiveSite="false" SelectionMode="Multiple"
                            ResourcePrefix="addroles" />
                    </asp:Panel>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </td>
        <td>
        </td>
        <td class="ColumnPanel">
            <cms:CMSUpdatePanel ID="pnlUpdateUsers" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnlUsers" runat="server">
                        <div class="Height80">
                            <asp:RadioButtonList ID="rbUserType" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" />
                        </div>
                        <cms:UniSelector ID="usUsers" runat="server" IsLiveSite="false" SelectionMode="Multiple"
                            ResourcePrefix="addusers" DisplayNameFormat="##USERDISPLAYFORMAT##" />
                    </asp:Panel>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </td>
    </tr>
</table>
