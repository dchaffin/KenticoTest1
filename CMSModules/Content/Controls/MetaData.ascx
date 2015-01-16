<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MetaData.ascx.cs" Inherits="CMSModules_Content_Controls_MetaData" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Tags/TagGroupSelector.ascx" TagName="TagGroupSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Tags/TagSelector.ascx" TagName="TagSelector"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<asp:Panel ID="pnlForm" runat="server">
    <cms:CMSUpdatePanel ID="ucPanel" runat="server">
        <ContentTemplate>
            <cms:UIPlaceHolder ID="pnlUIPage" runat="server" OnOnBeforeCheck="pnlUI_OnBeforeCheck" >
                <asp:Panel ID="pnlPageSettings" runat="server" CssClass="NodePermissions" GroupingText="Page settings">
                    <table>
                        <tr>
                            <td class="FieldLabel">
                                <asp:Label ID="lblPageTitle" runat="server" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtTitle" runat="server" CssClass="TextBoxField" />
                                <div>
                                    <asp:CheckBox runat="server" ID="chkTitle" AutoPostBack="true" OnCheckedChanged="chkTitle_CheckedChanged" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel">
                                <asp:Label ID="lblPageDescription" runat="server" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtDescription" runat="server" CssClass="TextAreaField" TextMode="MultiLine" />
                                <div>
                                    <asp:CheckBox runat="server" ID="chkDescription" AutoPostBack="true" OnCheckedChanged="chkDescription_CheckedChanged" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel">
                                <asp:Label ID="lblPageKeywords" runat="server" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtKeywords" runat="server" CssClass="TextAreaField" TextMode="MultiLine" />
                                <div>
                                    <asp:CheckBox runat="server" ID="chkKeyWords" AutoPostBack="true" OnCheckedChanged="chkKeyWords_CheckedChanged" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
            </cms:UIPlaceHolder>
            <cms:UIPlaceHolder ID="pnlUITags" runat="server" OnOnBeforeCheck="pnlUI_OnBeforeCheck">
                <asp:Panel ID="pnlTags" runat="server" CssClass="NodePermissions" GroupingText="Tags">
                    <table>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblTagInfo" runat="server" Visible="false" CssClass="infoLabel" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel">
                                <asp:Label ID="lblTagGroupSelector" runat="server" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:TagGroupSelector runat="server" ID="tagGroupSelectorElem" UseAutoPostback="true"
                                    UseGroupNameForSelection="false" UseDropdownList="true" />
                                <asp:CheckBox runat="server" ID="chkTagGroupSelector" AutoPostBack="true" OnCheckedChanged="chkTagGroupSelector_CheckedChanged" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel" style="vertical-align: top;">
                                <asp:Label ID="lblTagSelector" runat="server" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:TagSelector runat="server" ID="tagSelectorElem" IsLiveSite="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </cms:UIPlaceHolder>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
