<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Comment.ascx.cs" Inherits="CMSModules_Objects_Controls_Locking_Comment" %>
<cms:CMSObjectManager ID="objectManager" runat="server" />
<cms:CMSPanel ID="pnlContainer" ShortID="pC" runat="server">
    <table width="100%">
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
    <asp:HiddenField ID="hdnArg" runat="server" />
</cms:CMSPanel>
