<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header.ascx.cs" Inherits="CMSAdminControls_UI_Header" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelectorControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/Controls/PasswordExpiration.ascx" TagPrefix="cms"
    TagName="PasswordExpiration" %>
<%@ Register Src="~/CMSAdminControls/UI/ScreenLock/ScreenLock.ascx" TagPrefix="cms"
    TagName="ScreenLock" %>
<cms:ScreenLock ID="screenLockElem" runat="server" EnableViewState="false" />
<asp:Literal ID="ltlFBConnectScript" runat="server" EnableViewState="false" />
<asp:Panel runat="server" ID="pnlTechPreview" CssClass="TechPreview">
    Please note: This is a technical preview version. Changes are directly saved to
    the development database.</asp:Panel>
<asp:Panel runat="server" ID="pnlTrial" CssClass="TrialVersion">
    <asp:Literal ID="ltlText" runat="server" EnableViewState="false" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlPwdExp" CssClass="HeaderWarning">
    <cms:PasswordExpiration ID="pwdExpiration" runat="server" EnableViewState="false"
        IsLiveSite="false" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlWrapper" CssClass="CMSHeader">
    <asp:Panel runat="server" ID="pnlBody">
        <asp:HyperLink ID="lnkLogo" runat="server" CssClass="HeaderLeft" EnableViewState="false"
            Font-Underline="false">
            &nbsp;            
        </asp:HyperLink>
        <asp:Panel runat="server" ID="pnlTabs" CssClass="HeaderTabs" EnableViewState="false">
            <cms:UITabs ID="tabControl" ShortID="t" runat="server" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlRight" CssClass="HeaderRight">
            <table cellpadding="0" cellspacing="0" class="RightAlign">
                <tr>
                    <td class="HeaderRightPaddedCell">
                        <asp:HyperLink ID="lnkTestingMode" CssClass="HeaderLink" runat="server" Visible="false" />
                    </td>
                    <asp:PlaceHolder runat="server" ID="plcLiveSite">
                        <td class="HeaderRightPaddedCell">
                            <asp:HyperLink ID="lnkLiveSite" CssClass="HeaderLink" runat="server" EnableViewState="false" />
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="plcCMSDeskRedirect" Visible="False">
                        <td>
                            <asp:HyperLink ID="lnkCMSDesk" CssClass="HeaderLink" runat="server" EnableViewState="false" />
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="plcSiteManagerRedirect" Visible="False">
                        <td>
                            <asp:HyperLink ID="lnkCMSSiteManager" CssClass="HeaderLink" runat="server" EnableViewState="false" />
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="plcSiteSelector">
                        <td class="HeaderRightPaddedCell">
                            <cms:LocalizedLabel ID="lblSite" runat="server" CssClass="HeaderSite" ResourceString="general.site"
                                DisplayColon="true" Visible="false" />
                            <cms:SiteSelector ID="siteSelector" ShortID="ss" runat="server" IsLiveSite="false" />
                        </td>
                    </asp:PlaceHolder>
                    <td class="HeaderUserIcon">
                        &nbsp;
                    </td>
                    <td>
                        <asp:Label ID="lblUserInfo" runat="server" CssClass="HeaderUserInfo" EnableViewState="false" />
                    </td>
                    <asp:PlaceHolder runat="server" ID="pnlUsers" Visible="false">
                        <td>
                            <div class="Hidden">
                                <cms:UniSelectorControl ID="ucUsers" ShortID="us" ObjectType="CMS.User" runat="server"
                                    ReturnColumnName="UserName" SelectionMode="SingleButton" IsLiveSite="false" DisplayNameFormat="##USERDISPLAYFORMAT##" />
                            </div>
                            &nbsp;
                            <cms:ContextMenuContainer runat="server" ID="menuCont">
                                <asp:ImageButton runat="server" ID="imgImpersonate" />
                            </cms:ContextMenuContainer>
                            <cms:CMSUpdatePanel runat="server" ID="pnlHiddenImpersonate">
                                <ContentTemplate>
                                    <asp:Button ID="btnHiddenImpersonate" runat="server" OnClick="btnHiddenImpersonate_Click" />
                                </ContentTemplate>
                            </cms:CMSUpdatePanel>
                        </td>
                    </asp:PlaceHolder>
                    <td class="HeaderRightPaddedCell">
                        <asp:Label runat="server" ID="lblVersion" EnableViewState="false" CssClass="HeaderVersion" />
                    </td>
                    <asp:PlaceHolder runat="server" ID="pnlSupportChat" Visible="false">
                        <td>
                            <asp:PlaceHolder runat="server" ID="plcSupportChatControl"></asp:PlaceHolder>
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="pnlCultures">
                        <td>
                            <div class="UIHeaderSelector">
                                <cms:UniSelectorControl ID="ucUICultures" ShortID="cs" ObjectType="CMS.UICulture"
                                    runat="server" ReturnColumnName="UICultureCode" SelectionMode="SingleButton"
                                    IsLiveSite="false" />
                            </div>
                        </td>
                    </asp:PlaceHolder>
                    <cms:DevelopmentMode runat="server" ID="plcLog">
                        <td>
                            <div class="UIHeaderSelector">
                                <asp:HyperLink runat="server" ID="lnkLog" Target="_blank" EnableViewState="false">
                                    <asp:Image runat="server" ID="imgLog" EnableViewState="false" />
                                </asp:HyperLink>
                            </div>
                        </td>
                    </cms:DevelopmentMode>
                    <cms:DevelopmentMode runat="server" ID="plcDebug">
                        <td>
                            <div class="UIHeaderSelector">
                                <asp:HyperLink runat="server" ID="lnkDebug" Target="_blank" EnableViewState="false">
                                    <asp:Image runat="server" ID="imgDebug" EnableViewState="false" />
                                </asp:HyperLink>
                            </div>
                        </td>
                    </cms:DevelopmentMode>
                    <td style="vertical-align: top;">
                        <asp:Panel runat="server" ID="pnlSignOut" CssClass="HeaderSignOutPnl" EnableViewState="false">
                            <asp:LinkButton runat="server" ID="lnkSignOut" OnClick="btnSignOut_Click" EnableViewState="false">
                                <asp:Label runat="server" ID="lblSignOut" EnableViewState="false" CssClass="HeaderSignOut"
                                    Font-Underline="false" />
                            </asp:LinkButton>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="<%=logoColspan %>">
                        <asp:Panel runat="server" ID="pnlExtraIcons" Visible="false" CssClass="ExtraIcons">
                            <asp:Image runat="server" ID="imgEnterpriseSolution" Visible="false" AlternateText=" " />
                            <asp:Image runat="server" ID="imgWindowsAzure" Visible="false" AlternateText=" " />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSeparator" CssClass="HeaderContentSeparator" EnableViewState="false"
            Visible="False">
            &nbsp;
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
