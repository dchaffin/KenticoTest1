<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_BadWords_BadWords_Edit_Cultures" Theme="Default" CodeFile="BadWords_Edit_Cultures.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        <cms:LocalizedRadioButton ID="radAll" runat="server" ResourceString="badwords.culturesall"
                            GroupName="grpCultures" AutoPostBack="true" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <cms:LocalizedRadioButton ID="radSelected" runat="server" ResourceString="badwords.culturesselected"
                            GroupName="grpCultures" AutoPostBack="true" />
                    </td>
                </tr>
            </table>
            <br />
            <cms:UniSelector ID="usWordCultures" runat="server" IsLiveSite="false" ObjectType="cms.culture"
                SelectionMode="Multiple" OrderBy="CultureName" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
