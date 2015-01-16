<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Security"
    Theme="Default" CodeFile="Security.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Security.ascx" TagName="Security"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" HandleWorkflow="false" ShowSave="false"
        HelpTopicName="security2" IsLiveSite="false" />
    <cms:CMSDocumentPanel ID="pnlDocInfo" runat="server" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlLog" Visible="false">
                <cms:AsyncBackground ID="backgroundElem" runat="server" />
                <div class="AsyncLogArea">
                    <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                        <asp:Panel ID="pnlTitle" runat="server" CssClass="PageHeader" EnableViewState="false">
                            <cms:PageTitle ID="titleElem" runat="server" EnableViewState="false" />
                        </asp:Panel>
                        <asp:Panel ID="pnlCancel" runat="server" CssClass="PageHeaderLine">
                            <cms:CMSButton runat="server" ID="btnCancel" CssClass="SubmitButton" />
                        </asp:Panel>
                        <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                            <cms:AsyncControl ID="ctlAsync" runat="server" />
                        </asp:Panel>
                    </asp:Panel>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlPageContent" runat="server" CssClass="PropertiesPanel">
                <cms:UIPlaceHolder ID="pnlUIPermissionsPart" runat="server" ModuleName="CMS.Content"
                    ElementName="Security.Permissions">
                    <asp:PlaceHolder ID="plcContainer" runat="server">
                        <asp:Panel ID="pnlPermissionsPart" runat="server" CssClass="NodePermissions">
                            <asp:Panel ID="pnlPermissionsPartBox" CssClass="NodePermissionBox" runat="server">
                                <asp:Label ID="lblLicenseInfo" runat="server" Visible="False" EnableViewState="false" />
                                <asp:Panel ID="pnlPermissions" runat="server">
                                    <asp:Label ID="lblInheritanceInfo" CssClass="InfoLabel" runat="server" EnableViewState="false" />
                                    <cms:LocalizedLinkButton ID="lnkInheritance" runat="server" OnClick="lnkInheritance_Click"
                                        EnableViewState="false" ResourceString="Security.Inheritance" />
                                    <br />
                                    <br />
                                    <cms:Security ID="securityElem" runat="server" IsLiveSite="false" />
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                        <br />
                    </asp:PlaceHolder>
                </cms:UIPlaceHolder>
                <asp:Panel ID="pnlAccessPart" runat="server" CssClass="NodePermissions" GroupingText="Access">
                    <asp:Panel ID="pnlAccessBox" CssClass="NodePermissionBox" runat="server">
                        <table width="100%">
                            <tr>
                                <cms:UIPlaceHolder ID="pnlUIAuth" runat="server" ModuleName="CMS.Content" ElementName="Security.Authentication">
                                    <td>
                                        <cms:LocalizedLabel ID="lblReqAuthent" runat="server" EnableViewState="false" ResourceString="Security.RadioCaption" />
                                        <br />
                                    </td>
                                </cms:UIPlaceHolder>
                                <cms:UIPlaceHolder ID="pnlUISsl" runat="server" ModuleName="CMS.Content" ElementName="Security.SSL">
                                    <td>
                                        <cms:LocalizedLabel ID="lblReqSSL" runat="server" EnableViewState="false" ResourceString="Security.RequiresSSL" />
                                        <br />
                                    </td>
                                </cms:UIPlaceHolder>
                            </tr>
                            <tr>
                                <cms:UIPlaceHolder ID="pnlUIAuthBtns" runat="server" ModuleName="CMS.Content" ElementName="Security.Authentication">
                                    <td style="vertical-align: top;">
                                        <table>
                                            <tr>
                                                <td>
                                                    <cms:LocalizedRadioButton GroupName="reqAuth" ID="radYes" runat="server" ResourceString="general.yes" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <cms:LocalizedRadioButton GroupName="reqAuth" ID="radNo" runat="server" ResourceString="general.no" />
                                                </td>
                                            </tr>
                                            <asp:PlaceHolder ID="plcAuthParent" runat="server">
                                                <tr>
                                                    <td>
                                                        <cms:LocalizedRadioButton GroupName="reqAuth" ID="radParent" runat="server" ResourceString="Security.Parent" />
                                                    </td>
                                                </tr>
                                            </asp:PlaceHolder>
                                        </table>
                                    </td>
                                </cms:UIPlaceHolder>
                                <cms:UIPlaceHolder ID="pnlUISslBtns" runat="server" ModuleName="CMS.Content" ElementName="Security.SSL">
                                    <td style="vertical-align: top;">
                                        <table>
                                            <tr>
                                                <td>
                                                    <cms:LocalizedRadioButton GroupName="reqSSL" ID="radYesSSL" runat="server" ResourceString="general.yes" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <cms:LocalizedRadioButton GroupName="reqSSL" ID="radNoSSL" runat="server" ResourceString="general.no" />
                                                </td>
                                            </tr>
                                            <asp:PlaceHolder ID="plcSSLParent" runat="server">
                                                <tr>
                                                    <td>
                                                        <cms:LocalizedRadioButton GroupName="reqSSL" ID="radParentSSL" runat="server" ResourceString="Security.Parent" />
                                                    </td>
                                                </tr>
                                            </asp:PlaceHolder>
                                            <tr>
                                                <td>
                                                    <cms:LocalizedRadioButton GroupName="reqSSL" ID="radNeverSSL" runat="server" ResourceString="Security.Never" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </cms:UIPlaceHolder>
                                <td style="text-align: right; vertical-align: bottom;">
                                    <cms:LocalizedButton ID="btnRadOk" runat="server" CssClass="SubmitButton" OnClick="btnRadOk_Click"
                                        EnableViewState="false" ResourceString="general.ok" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </asp:Panel>
                <asp:Panel ID="pnlInheritance" runat="server" Visible="False">
                    <asp:Panel ID="pnlCont" runat="server" CssClass="NodePermissionBox">
                        <asp:PlaceHolder runat="server" ID="plcRestore">
                            <cms:LocalizedLinkButton ID="lnkRestoreInheritance" runat="server" OnClick="lnkRestoreInheritance_Click"
                                EnableViewState="false" ResourceString="Security.RestoreInheritance" /><br />
                            <br />
                            <cms:LocalizedLinkButton ID="lnkRestoreInheritanceRecursively" runat="server" OnClick="lnkRestoreInheritanceRecursively_Click"
                                EnableViewState="false" ResourceString="Security.RestoreInheritanceRecursively" /><br />
                            <br />
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="plcBreakCopy">
                            <cms:LocalizedLinkButton ID="lnkBreakWithCopy" runat="server" OnClick="lnkBreakWithCopy_Click"
                                EnableViewState="false" ResourceString="Security.BreakWithCopy" /><br />
                            <br />
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="plcBreakClear">
                            <cms:LocalizedLinkButton ID="lnkBreakWithClear" runat="server" OnClick="lnkBreakWithClear_Click"
                                EnableViewState="false" ResourceString="Security.BreakWithClear" /><br />
                            <br />
                        </asp:PlaceHolder>
                        <br />
                        <cms:LocalizedButton ID="btnCancelAction" runat="server" CssClass="SubmitButton"
                            OnClick="btnCancelAction_Click" EnableViewState="false" ResourceString="general.cancel" />
                    </asp:Panel>
                </asp:Panel>
            </asp:Panel>
            <div class="Clear">
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="lnkRestoreInheritanceRecursively" />
            <asp:PostBackTrigger ControlID="lnkRestoreInheritance" />
            <asp:PostBackTrigger ControlID="lnkBreakWithCopy" />
            <asp:PostBackTrigger ControlID="lnkBreakWithClear" />
            <asp:PostBackTrigger ControlID="btnCancelAction" />
        </Triggers>
    </cms:CMSUpdatePanel>
</asp:Content>
