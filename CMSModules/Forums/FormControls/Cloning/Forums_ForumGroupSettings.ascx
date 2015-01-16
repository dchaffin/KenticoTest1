<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Forums_ForumGroupSettings.ascx.cs"
    Inherits="CMSModules_Forums_FormControls_Cloning_Forums_ForumGroupSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblCloneForumPosts" ResourceString="clonning.settings.forum.cloneposts"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkCloneForumPosts" Checked="false" />
        </td>
    </tr>
</table>
