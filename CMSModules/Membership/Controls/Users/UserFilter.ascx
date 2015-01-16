<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Controls_Users_UserFilter"
    CodeFile="UserFilter.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Roles/selectrole.ascx" TagName="SelectRole"
    TagPrefix="uc1" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<div>
    <asp:Panel ID="pnlAlphabet" CssClass="AlphabetFilter" runat="server" />
</div>
<br />
<cms:DisabledModule runat="server" ID="ucDisabledModule" Visible="false" />
<br />
<asp:Panel ID="pnlSimpleFilter" runat="server" DefaultButton="btnSimpleSearch">
    <cms:CMSTextBox ID="txtSearch" runat="server" CssClass="VeryLongTextBox" MaxLength="450" /><cms:CMSButton
        ID="btnSimpleSearch" runat="server" CssClass="ContentButton" OnClick="btnSearch_Click" />
    <asp:LinkButton ID="btnReset" runat="server" Style="line-height: 2em; margin: 1em"
        EnableViewState="false" />
    <br />
    <br />
    <div>
        <asp:Image runat="server" ID="imgShowAdvancedFilter" CssClass="NewItemImage" />
        <asp:LinkButton ID="lnkShowAdvancedFilter" runat="server" OnClick="lnkShowAdvancedFilter_Click" />
    </div>
</asp:Panel>
<asp:Panel ID="pnlAdvancedFilter" runat="server" DefaultButton="btnAdvancedSearch">
    <table cellpadding="0" cellspacing="2" class="UserFilter">
        <asp:PlaceHolder ID="plcUserName" runat="server">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblUserName" runat="server" CssClass="ContentLabel" ResourceString="general.username"
                        DisplayColon="true" />
                </td>
                <td colspan="2">
                    <cms:TextSimpleFilter ID="fltUserName" runat="server" Column="UserName" Size="100" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
                <asp:Label ID="lblFullName" runat="server" CssClass="ContentLabel" />
            </td>
            <td colspan="2">
                <cms:TextSimpleFilter ID="fltFullName" runat="server" Column="FullName" Size="450" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblEmail" runat="server" CssClass="ContentLabel" />
            </td>
            <td colspan="2">
                <cms:TextSimpleFilter ID="fltEmail" runat="server" Column="Email" Size="100" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcNickName" runat="server">
            <tr>
                <td>
                    <asp:Label ID="lblNickName" runat="server" CssClass="ContentLabel" />
                </td>
                <td colspan="2">
                    <cms:TextSimpleFilter ID="fltNickName" runat="server" Column="UserNickName" Size="200" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
                <asp:Label ID="lblInRoles" runat="server" CssClass="ContentLabel" />
            </td>
            <td>
                <asp:DropDownList CssClass="ExtraSmallDropDown" runat="server" ID="drpTypeSelectInRoles" />
            </td>
            <td>
                <uc1:SelectRole UserFriendlyMode="true" IsLiveSite="false" ID="selectRoleElem" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblNotInRoles" runat="server" CssClass="ContentLabel" />
            </td>
            <td>
                <asp:DropDownList CssClass="ExtraSmallDropDown" runat="server" ID="drpTypeSelectNotInRoles" />
            </td>
            <td>
                <uc1:SelectRole UserFriendlyMode="true" IsLiveSite="false" ID="selectNotInRole" runat="server" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcGroups" runat="server" Visible="false">
            <tr>
                <td>
                    <asp:Label ID="lblInGroups" runat="server" CssClass="ContentLabel" />
                </td>
                <td>
                    <asp:DropDownList CssClass="ExtraSmallDropDown" runat="server" ID="drpTypeSelectInGroups" />
                </td>
                <td>
                    <asp:PlaceHolder runat="server" ID="plcSelectInGroups" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblNotInGroups" runat="server" CssClass="ContentLabel" />
                </td>
                <td>
                    <asp:DropDownList CssClass="ExtraSmallDropDown" runat="server" ID="drpTypeSelectNotInGroups" />
                </td>
                <td>
                    <asp:PlaceHolder runat="server" ID="plcSelectNotInGroups" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcUserEnabled" runat="server">
            <tr>
                <td>
                    <asp:Label ID="lblEnabled" runat="server" CssClass="ContentLabel" />
                </td>
                <td colspan="2">
                    <asp:CheckBox runat="server" ID="chkEnabled" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblLockReason" runat="server" CssClass="ContentLabel" />
                </td>
                <td colspan="2">
                    <asp:DropDownList ID="drpLockReason" runat="server" CssClass="DropDownField" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcDisplayAnonymous" runat="server" Visible="False">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblDisplayAnonymous" runat="server" CssClass="ContentLabel"
                        EnableViewState="False" AssociatedControlID="chkDisplayAnonymous" ResourceString="userlist.displayguests"
                        DisplayColon="True" />
                </td>
                <td colspan="2">
                    <asp:CheckBox ID="chkDisplayAnonymous" runat="server" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcHidden" runat="server" Visible="False">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblHidden" runat="server" CssClass="ContentLabel" EnableViewState="False"
                        AssociatedControlID="chkDisplayHidden" ResourceString="user.showhidden" DisplayColon="True" />
                </td>
                <td colspan="2">
                    <asp:CheckBox ID="chkDisplayHidden" runat="server" Checked="True" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcScore" runat="server" Visible="False">
            <asp:PlaceHolder ID="plcSite" runat="server" Visible="False">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblSite" runat="server" CssClass="ContentLabel" EnableViewState="False"
                            AssociatedControlID="siteSelector" ResourceString="general.site" DisplayColon="True" />
                    </td>
                    <td colspan="2">
                        <cms:SiteSelector ID="siteSelector" runat="server" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblScore" runat="server" CssClass="ContentLabel" EnableViewState="False"
                        AssociatedControlID="selectScore" ResourceString="om.score" DisplayColon="True" />
                </td>
                <td colspan="2">
                    <cms:CMSUpdatePanel ID="pnlUpdateScore" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <asp:PlaceHolder ID="plcUpdateContent" runat="server"></asp:PlaceHolder>
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
            </td>
            <td>
                <cms:CMSButton ID="btnAdvancedSearch" runat="server" CssClass="ContentButton" OnClick="btnSearch_Click" />
            </td>
            <td>
                <asp:LinkButton ID="btnAdvancedReset" runat="server" Style="line-height: 2em; margin: 1em"
                    EnableViewState="false" />
            </td>
        </tr>
    </table>
    <br />
    <div>
        <asp:Image runat="server" ID="imgShowSimpleFilter" CssClass="NewItemImage" />
        <asp:LinkButton ID="lnkShowSimpleFilter" runat="server" OnClick="lnkShowSimpleFilter_Click" />
    </div>
</asp:Panel>
<asp:HiddenField ID="hdnAlpha" runat="server" />
