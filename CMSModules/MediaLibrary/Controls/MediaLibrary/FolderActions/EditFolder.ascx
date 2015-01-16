<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_EditFolder" CodeFile="EditFolder.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>

<asp:Panel ID="pnlFolderEdit" runat="server" EnableViewState="false">
    <cms:JQueryTabContainer ID="pnlTabs" runat="server" CssClass="Dialog_Tabs">
        <cms:JQueryTab ID="tabGeneral" runat="server">
            <ContentTemplate>
                <cms:HeaderActions ID="headerActions" runat="server" PanelCssClass="CMSEditMenu" />
                <asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
                    <table width="100%">
                        <tr>
                            <td colspan="2" class="FolderEditInfoArea">
                                <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td style="white-space: nowrap;" class="FolderEditLabelArea">
                                <cms:LocalizedLabel ID="lblFolderName" runat="server" CssClass="FieldLabel" DisplayColon="true"
                                    ResourceString="media.folder.foldername" EnableViewState="false"></cms:LocalizedLabel>
                            </td>
                            <td style="width: 100%">
                                <cms:CMSTextBox ID="txtFolderName" runat="server" CssClass="TextBoxField" MaxLength="50"
                                    EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td colspan="2">
                                <div id="divButtons" runat="server" enableviewstate="false" style="padding-top: 8px;">
                                        <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_Click"
                                            EnableViewState="false" />
                                    <asp:PlaceHolder ID="plcCancelArea" runat="server">
                                        <cms:CMSButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" CssClass="SubmitButton"
                                            EnableViewState="false" />
                                    </asp:PlaceHolder>
                                </div>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </ContentTemplate>
        </cms:JQueryTab>
    </cms:JQueryTabContainer>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Panel>
