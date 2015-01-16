<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Delete"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Content - Delete"
    CodeFile="Delete.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectSinglePath.ascx"
    TagName="SelectSinglePath" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncBackground ID="backgroundElem" runat="server" />
        <div class="AsyncLogArea">
            <div>
                <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                    <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader">
                        <cms:PageTitle ID="titleElemAsync" runat="server" SetWindowTitle="false" />
                    </asp:Panel>
                    <asp:Panel ID="pnlCancel" runat="server" CssClass="PageHeaderLine">
                        <cms:CMSButton runat="server" ID="btnCancel" CssClass="SubmitButton" />
                    </asp:Panel>
                    <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                        <cms:AsyncControl ID="ctlAsync" runat="server" />
                    </asp:Panel>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="plcContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlContent">
        <asp:Panel ID="pnlDelete" runat="server">
            <asp:PlaceHolder runat="server" ID="plcDeleteRoot" Visible="false">
                <cms:LocalizedCheckBox ID="chkDeleteRoot" runat="server" CssClass="ContentCheckBox"
                    EnableViewState="false" ResourceString="Delete.RootConfirm" />
                <br />
                <br />
                <br />
            </asp:PlaceHolder>
            <cms:LocalizedLabel ID="lblQuestion" runat="server" CssClass="ContentLabel" EnableViewState="false"
                Font-Bold="true" />
            <br />
            <br id="brSeparator" runat="server" />
            <asp:Panel ID="pnlDocList" runat="server" Visible="false" CssClass="ScrollableList"
                EnableViewState="false">
                <asp:Label ID="lblDocuments" runat="server" CssClass="ContentLabel" EnableViewState="true" />
            </asp:Panel>
            <asp:PlaceHolder ID="plcCheck" runat="server" EnableViewState="false">
                <br />
                <asp:Panel ID="pnlDeleteDocument" runat="server" EnableViewState="false" CssClass="EditingFormFieldSet">
                    <div>
                        <asp:CheckBox ID="chkDestroy" runat="server" CssClass="ContentCheckBox" Visible="false"
                            EnableViewState="false" />
                    </div>
                    <div>
                        <asp:CheckBox ID="chkAllCultures" runat="server" CssClass="ContentCheckBox" EnableViewState="false" />
                    </div>
                </asp:Panel>
            </asp:PlaceHolder>
            <asp:Panel ID="pnlDeleteSKU" runat="server" Visible="false" CssClass="EditingFormFieldSet">
                <asp:Label ID="lblSKUActionInfo" runat="server" CssClass="ContentLabel" EnableViewState="true" />
                <br />
                <br />
                <div>
                    <asp:RadioButtonList ID="rblSKUAction" runat="server" RepeatDirection="Vertical"
                        RepeatLayout="Flow" />
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlSeo" runat="server" CssClass="EditingFormFieldSet">
                <cms:CMSUpdatePanel runat="server" ID="pnlAltPath" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cms:LocalizedCheckBox ID="chkUseDeletedPath" runat="server" ResourceString="content.delete.usealtpath"
                            AutoPostBack="true" CssClass="ContentCheckBox" EnableViewState="true" />
                        <br />
                        <br />
                        <asp:PlaceHolder runat="server" ID="plcDeleteSettings" Visible="false">
                            <table>
                                <tr>
                                    <td class="FieldLabel">
                                        <cms:LocalizedLabel runat="server" ID="lblAltPath" ResourceString="content.delete.altpath"
                                            DisplayColon="true" />
                                    </td>
                                    <td>
                                        <cms:SelectSinglePath ID="selAltPath" runat="server" IsLiveSite="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel">
                                        <cms:LocalizedLabel runat="server" AssociatedControlID="chkAltAliases" ID="lblAltAliases"
                                            ResourceString="content.delete.altaliases" DisplayColon="true" />
                                    </td>
                                    <td>
                                        <cms:LocalizedCheckBox runat="server" ID="chkAltAliases" Checked="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel">
                                        <cms:LocalizedLabel runat="server" AssociatedControlID="chkAltSubNodes" ID="lblAltUrl"
                                            ResourceString="content.delete.altsub" DisplayColon="true" />
                                    </td>
                                    <td>
                                        <cms:LocalizedCheckBox runat="server" ID="chkAltSubNodes" Checked="true" />
                                    </td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </asp:Panel>
            <br />
            <cms:LocalizedButton ID="btnOk" runat="server" CssClass="SubmitButton" OnClick="btnOK_Click"
                ResourceString="general.yes" EnableViewState="false" /><cms:LocalizedButton ID="btnNo"
                    runat="server" CssClass="SubmitButton" ResourceString="general.no" EnableViewState="false" />
        </asp:Panel>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
