<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_TemplateSelection"
    Theme="Default" CodeFile="TemplateSelection.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" %>

<%@ Register Src="New/TemplateSelection.ascx" TagName="TemplateSelection" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="NewPageDialog">
        <asp:Panel ID="pnlContent" runat="server" CssClass="PageContentFrame">
            <div class="PTSelection">
                <cms:CMSPanel ID="pnlMenu" runat="server" FixedPosition="true">
                    <cms:editmenu ID="menuElem" ShortID="m" runat="server" ShowProperties="false" ShowCreateAnother="false"
                        HelpTopicName="CMS_Content_Templates" IsLiveSite="false" />
                </cms:CMSPanel>
                <div style="padding: 10px 10px 0px 10px;">
                </div>
                <table cellpadding="0" cellspacing="0" class="Table" border="0">
                    <tr class="HeaderRow">
                        <td class="LeftBorder">
                        </td>
                        <td style="vertical-align: top;" class="Header">
                            <asp:Label runat="server" ID="lblChoose" /><br />
                        </td>
                        <td class="RightBorder">
                        </td>
                    </tr>
                </table>
                <table cellpadding="0" cellspacing="0" class="Table" border="0">
                    <tr class="Row">
                        <td style="vertical-align: top;" class="Content">
                            <cms:TemplateSelection ID="selTemplate" runat="server" />
                        </td>
                    </tr>
                </table>
                <div class="Footer">
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
