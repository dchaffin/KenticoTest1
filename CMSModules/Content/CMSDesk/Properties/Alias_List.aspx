<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Alias_List"
    Theme="Default" CodeFile="Alias_List.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Documents/DocumentURLPath.ascx" TagName="DocumentURLPath"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectCampaign.ascx" TagName="SelectCampaign"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" HandleWorkflow="false"
        HelpTopicName="doc_urls" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <asp:Panel ID="pnlPageContent" runat="server" CssClass="PropertiesPanel">
        <cms:UIPlaceHolder ID="pnlUIAlias" runat="server" ModuleName="CMS.Content" ElementName="URLs.Path">
            <asp:Panel ID="pnlAlias" runat="server" GroupingText="Alias">
                <table>
                    <tr>
                        <td class="FieldLabel" style="width: 125px;">
                            <asp:Label ID="lblAlias" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <cms:CMSTextBox ID="txtAlias" runat="server" CssClass="TextBoxField" MaxLength="50" />
                            <cms:CMSRequiredFieldValidator ID="valAlias" runat="server" ControlToValidate="txtAlias"
                                Display="dynamic" EnableViewState="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIPath" runat="server" ModuleName="CMS.Content" ElementName="URLs.URLPath">
            <asp:Panel ID="pnlURLPath" runat="server" GroupingText="URL Path">
                <cms:DocumentURLPath runat="server" ID="ctrlURL" />
            </asp:Panel>
            <br />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIExtended" runat="server" ModuleName="CMS.Content" ElementName="URLs.ExtendedProperties">
            <asp:Panel ID="pnlExtended" runat="server" GroupingText="Extended Properties">
                <table>
                    <tr>
                        <td class="FieldLabel" style="width: 125px; vertical-align: top;">
                            <asp:Label ID="lblExtensions" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <cms:CMSTextBox ID="txtExtensions" runat="server" CssClass="TextBoxField" MaxLength="100" /><br />
                            <asp:CheckBox ID="chkCustomExtensions" runat="server" OnCheckedChanged="chkCustomExtensions_CheckedChanged"
                                AutoPostBack="true" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIDocumentAlias" runat="server" ModuleName="CMS.Content"
            ElementName="URLs.Aliases">
            <asp:Panel runat="server" ID="pnlDocumentAlias" GroupingText="Document alias">
                <asp:Panel ID="pnlNewAlias" runat="server" CssClass="PageContent" BorderStyle="None">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <a runat="server" id="lnkNewAlias" class="NewItemLink">
                                    <cms:CMSImage ID="imgNewAlias" runat="server" CssClass="NewItemImage" EnableViewState="false" /></a>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <cms:UniGrid ID="UniGridAlias" runat="server" GridName="Alias_List.xml" IsLiveSite="false"
                        OrderBy="AliasID" />
                </asp:Panel>
            </asp:Panel>
        </cms:UIPlaceHolder>
    </asp:Panel>
    <div class="Clear">
    </div>
</asp:Content>
