<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Comment.ascx.cs" Inherits="CMSModules_Automation_Controls_Comment" %>
<cms:CMSPanel ID="pnlContainer" ShortID="pC" runat="server">
    <table width="100%">
        <asp:PlaceHolder ID="plcSteps" runat="server" Visible="false">
            <tr>
                <td style="white-space: nowrap;">
                    <cms:LocalizedLabel ID="lblSteps" runat="server" ResourceString="doc.stepslist" DisplayColon="true" />
                </td>
                <td style="width:100%">
                    <asp:DropDownList ID="drpSteps" runat="server" Width="100%" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td colspan="2">
                <cms:LocalizedLabel ID="lblComment" runat="server" ResourceString="worfklowproperties.comment" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <cms:CMSTextBox ID="txtComment" runat="server" TextMode="MultiLine" CssClass="TextAreaField"
                    Width="100%" Height="220px" />
            </td>
        </tr>
    </table>
</cms:CMSPanel>