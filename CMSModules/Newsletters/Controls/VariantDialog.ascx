<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VariantDialog.ascx.cs"
    Inherits="CMSModules_Newsletters_Controls_VariantDialog" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:ModalPopupDialog ID="mdlVariants" runat="server" BackgroundCssClass="ModalBackground"
            CssClass="ModalPopupDialog IssueVariant">
            <asp:Panel ID="pnlVariants" runat="server" Visible="false" CssClass="DialogPageBody">
                <div style="height: auto; min-height: 0px;">
                    <div class="PageHeader">
                        <cms:PageTitle ID="ucTitle" runat="server" EnableViewState="false" GenerateFullWidth="false"
                            SetWindowTitle="false" DisplayMode="Simple" ShowFullScreenButton="false" ShowCloseButton="false" />
                    </div>
                </div>
                <asp:Panel ID="pnlScrollable" runat="server" CssClass="DialogPageContent DialogScrollableContent">
                    <div class="PageBody">
                        <asp:PlaceHolder ID="plcAddVariant" runat="server">
                            <table>
                                <tr>
                                    <td>
                                        <cms:LocalizedLabel ID="lblDisplayName" runat="server" ResourceString="general.name"
                                            DisplayColon="true" EnableViewState="false" CssClass="ContentLabel" AssociatedControlID="txtDisplayName" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDisplayName" runat="server" CssClass="TextBoxField" />
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
                                    <td colspan="2">
                                        <cms:LocalizedRadioButton ID="radEmpty" runat="server" ResourceString="newslettervariant.createempty"
                                            GroupName="templ" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <cms:LocalizedRadioButton ID="radBasedOnTemplate" runat="server" Checked="true" ResourceString="newslettervariant.createcontentfrom"
                                            GroupName="templ" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="UnderRadioContent">
                                        <asp:ListBox ID="lstTemplate" runat="server" Rows="5" Height="80px" Width="320px" EnableViewState="true" />
                                    </td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plcProperties" runat="server">
                            <table>
                                <tr>
                                    <td>
                                        <cms:LocalizedLabel ID="lblPropertyName" runat="server" ResourceString="general.name"
                                            DisplayColon="true" EnableViewState="false" CssClass="ContentLabel" AssociatedControlID="txtPropertyName" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPropertyName" runat="server" CssClass="TextBoxField" />
                                    </td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plcRemoveVariant" runat="server">
                            <table>
                                <tr>
                                    <td>
                                        <cms:LocalizedLabel ID="lblQuestion" runat="server" ResourceString="newslettervariant.deletevariantconfirm"
                                            EnableViewState="false" CssClass="ContentLabel" />
                                    </td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>
                    </div>
                </asp:Panel>
                <div class="PageFooterLine">
                    <div class="FloatLeft">
                        <cms:LocalizedLabel ID="lblError" runat="server" CssClass="ErrorLabel" ResourceString="newsletter.variantdisplaynamemissing"
                            EnableViewState="false" Style="display: none;" />
                        <cms:LocalizedLabel ID="lblError2" runat="server" CssClass="ErrorLabel" ResourceString="newsletter.variantselecttemplate"
                            EnableViewState="false" Style="display: none;" />
                    </div>
                    <div class="Buttons">
                        <cms:LocalizedButton ID="btnOKAdd" runat="server" EnableViewState="false" ResourceString="general.ok"
                            CssClass="SubmitButton" Visible="false" />
                        <cms:LocalizedButton ID="btnOKProperties" runat="server" EnableViewState="false"
                            ResourceString="general.ok" CssClass="SubmitButton" Visible="false" />
                        <cms:LocalizedButton ID="btnOKRemove" runat="server" EnableViewState="false" ResourceString="general.ok"
                            CssClass="SubmitButton" Visible="false" />
                        <cms:LocalizedButton ID="btnClose" runat="server" EnableViewState="false" ResourceString="general.cancel"
                            CssClass="SubmitButton" OnClick="btnClose_Click" CausesValidation="false" />
                    </div>
                    <div class="ClearBoth">
                        &nbsp;</div>
                </div>
            </asp:Panel>
        </cms:ModalPopupDialog>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:HiddenField ID="hdnParameter" runat="server" />
<asp:HiddenField ID="hdnSelected" runat="server" />
<asp:Button ID="btnFullPostback" runat="server" CssClass="HiddenButton" Style="display: none;" />
