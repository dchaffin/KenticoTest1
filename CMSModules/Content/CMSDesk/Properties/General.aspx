<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_General"
    Theme="Default" MaintainScrollPositionOnPostback="true" CodeFile="General.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/ContentRating/Controls/Stars.ascx" TagName="Rating"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/SelectCssStylesheet.ascx" TagName="SelectCssStylesheet"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" HandleWorkflow="false" HelpTopicName="general"
        IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server" CssClass="PropertiesPanel">
        <cms:UIPlaceHolder ID="pnlUIDesign" runat="server" ModuleName="CMS.Content" ElementName="General.Design">
            <asp:Panel ID="pnlDesign" runat="server" CssClass="NodePermissions" GroupingText="Design">
                <table>
                    <tr>
                        <td class="FieldLabel" style="width: 130px;">
                            <asp:Label ID="lblCssStyle" runat="server" CssClass="ContentLabel" EnableViewState="false" />
                        </td>
                        <td>
                            <cms:SelectCssStylesheet IsLiveSite="false" ID="ctrlSiteSelectStyleSheet" runat="server" />
                            <asp:CheckBox runat="server" ID="chkCssStyle" AutoPostBack="true" OnCheckedChanged="chkCssStyle_CheckedChanged" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIOther" runat="server" ModuleName="CMS.Content" ElementName="General.OtherProperties">
            <asp:Panel ID="pnlOther" runat="server" CssClass="NodePermissions" GroupingText="Document properties">
                <table>
                    <tr>
                        <td class="FieldLabel" style="width: 130px;">
                            <asp:Label ID="lblNameTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblName" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblTypeTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblType" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblCreatedByTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblCreatedBy" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblCreatedTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblCreated" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblLastModifiedByTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblLastModifiedBy" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblLastModifiedTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblLastModified" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="plcRating" runat="server">
                        <tr>
                            <td class="FieldLabel">
                                <cms:LocalizedLabel ID="lblContentRating" runat="server" ResourceString="GeneralProperties.ContentRating"
                                    DisplayColon="true" EnableViewState="false" />
                            </td>
                            <td class="Uploader">
                                <table style="width: 100%" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td class="RatingControlCell">
                                            <cms:Rating ID="ratingControl" runat="server" />
                                        </td>
                                        <td>
                                            <cms:CMSButton ID="btnResetRating" runat="server" CssClass="ContentButton" OnClick="btnResetRating_Click"
                                                EnableViewState="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Label ID="lblContentRatingResult" runat="server" EnableViewState="false" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblNodeIDTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblNodeID" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblDocIDTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblDocID" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblGUIDTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblGUID" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblDocGUIDTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblDocGUID" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblAliasPathTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblAliasPath" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblCultureTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblCulture" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblNamePathTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblNamePath" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblLiveURLTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:HyperLink ID="lnkLiveURL" runat="server" Target="_blank" EnableViewState="false" />
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="plcPreview" runat="server" Visible="false">
                        <tr>
                            <td class="FieldLabel" style="vertical-align: bottom;">
                                <asp:Label ID="lblPreviewURLTitle" runat="server" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:CMSUpdatePanel ID="pnlUpdatePreviewUrl" runat="server" UpdateMode="conditional">
                                    <ContentTemplate>
                                        <cms:LocalizedLabel ID="lblNoPreviewGuid" runat="server" EnableViewState="false"
                                            Visible="false" />
                                        <asp:HyperLink ID="lnkPreviewURL" runat="server" Target="_blank" EnableViewState="false" />
                                        <asp:ImageButton runat="server" ID="btnResetPreviewGuid" EnableViewState="false" />
                                    </ContentTemplate>
                                </cms:CMSUpdatePanel>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td class="FieldLabel">
                            <asp:Label ID="lblPublishedTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblPublished" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIOwner" runat="server" ModuleName="CMS.Content" ElementName="General.Owner">
            <asp:Panel ID="pnlOwner" runat="server" CssClass="NodePermissions" GroupingText="Owner">
                <table>
                    <tr>
                        <td class="FieldLabel" style="width: 130px;">
                            <asp:Label ID="lblOwnerTitle" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblOwner" runat="server" EnableViewState="false" />
                            <asp:PlaceHolder runat="server" ID="plcUsrOwner"></asp:PlaceHolder>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="plcOwnerGroup" runat="server" Visible="false" />
                </table>
            </asp:Panel>
            <br />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUICache" runat="server" ModuleName="CMS.Content" ElementName="General.Cache">
            <asp:Panel ID="pnlCache" runat="server" CssClass="NodePermissions" GroupingText="Cache">
                <cms:CMSUpdatePanel ID="ucCache" runat="server">
                    <ContentTemplate>
                        <table>
                            <tr>
                                <td class="FieldLabel" style="width: 130px;">
                                    <cms:LocalizedLabel ID="lblOutputCache" runat="server" EnableViewState="false" ResourceString="generalproperties.outputcache"
                                        DisplayColon="true" />
                                </td>
                                <td>
                                    <cms:LocalizedRadioButton GroupName="useCache" ID="radYes" runat="server" AutoPostBack="True"
                                        OnCheckedChanged="radInherit_CheckedChanged" ResourceString="general.yes" />
                                    <cms:LocalizedRadioButton GroupName="useCache" ID="radNo" runat="server" AutoPostBack="True"
                                        OnCheckedChanged="radInherit_CheckedChanged" ResourceString="general.no" />
                                    <cms:LocalizedRadioButton GroupName="useCache" ID="radInherit" runat="server" AutoPostBack="True"
                                        OnCheckedChanged="radInherit_CheckedChanged" ResourceString="GeneralProperties.radInherit" />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: left;">
                                    <asp:Label ID="lblCacheMinutes" runat="server" EnableViewState="False" />
                                </td>
                                <td>
                                    <cms:CMSTextBox ID="txtCacheMinutes" runat="server" CssClass="ShortTextBox" MaxLength="8" /><cms:LocalizedButton
                                        runat="server" ID="btnClear" ResourceString="GeneralProperties.ClearCache" CssClass="LongButton"
                                        OnClick="btnClear_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel">
                                    <cms:LocalizedLabel ID="LocalizedLabel1" runat="server" EnableViewState="false" ResourceString="generalproperties.outputcacheinfilesystem"
                                        DisplayColon="true" />
                                </td>
                                <td>
                                    <cms:LocalizedRadioButton GroupName="useFSCache" ID="radFSYes" runat="server" ResourceString="general.yes" />
                                    <cms:LocalizedRadioButton GroupName="useFSCache" ID="radFSNo" runat="server" ResourceString="general.no" />
                                    <cms:LocalizedRadioButton GroupName="useFSCache" ID="radFSInherit" runat="server"
                                        ResourceString="GeneralProperties.radInherit" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </asp:Panel>
            <br />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIOnlineMarketing" runat="server" ModuleName="CMS.Content"
            ElementName="General.OnlineMarketing">
            <asp:Panel ID="pnlOnlineMarketing" runat="server" CssClass="NodePermissions" EnableViewState="false">
                <table>
                    <tr>
                        <td style="vertical-align: top;">
                            <cms:LocalizedLabel ID="lblLogActivity" runat="server" EnableViewState="false" ResourceString="om.logpagevisitactivity"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:CheckBox runat="server" ID="chkLogPageVisit" />
                            <div>
                                <cms:LocalizedCheckBox runat="server" ID="chkPageVisitInherit" ResourceString="om.propertiesinherit"
                                    AutoPostBack="true" OnCheckedChanged="chkPageVisitInherit_CheckedChanged" />
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIAdvanced" runat="server" ModuleName="CMS.Content" ElementName="General.Advanced">
            <asp:Panel ID="pnlAdvanced" runat="server" GroupingText="Advanced" CssClass="NodePermissions"
                EnableViewState="false">
                <table cellspacing="0" cellpadding="0">
                    <tr>
                        <td>
                            <asp:LinkButton ID="lnkEditableContent" runat="server" EnableViewState="false" CssClass="MenuItemEdit">
                                <asp:Image ID="imgEditableContent" runat="server" />
                                <%=mEditableContent%>
                            </asp:LinkButton>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="plcAdHocBoards" runat="server" Visible="false">
                        <tr>
                            <td>
                                <asp:LinkButton ID="lnkMessageBoards" runat="server" EnableViewState="false" CssClass="MenuItemEdit">
                                    <asp:Image ID="imgMessageBoards" runat="server" />
                                    <%=mMessageBoards%>
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcAdHocForums" runat="server" Visible="false">
                        <tr>
                            <td>
                                <asp:LinkButton ID="lnkForums" runat="server" EnableViewState="false" CssClass="MenuItemEdit">
                                    <asp:Image ID="imgForums" runat="server" />
                                    <%=mForums%>
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcWireframe" runat="server" Visible="false">
                        <cms:UIPlaceHolder ID="plcUIWireframe" runat="server" ModuleName="CMS.Design" PermissionName="Wireframing">
                            <tr>
                                <td>
                                    <asp:LinkButton ID="lnkWireframe" runat="server" EnableViewState="false" CssClass="MenuItemEdit">
                                        <asp:Image ID="imgWireframe" runat="server" />
                                        <%=mWireframe%>
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </cms:UIPlaceHolder>
                    </asp:PlaceHolder>
                </table>
            </asp:Panel>
        </cms:UIPlaceHolder>
    </asp:Panel>
    <div class="Clear">
    </div>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
