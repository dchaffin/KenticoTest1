<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Newsletter_NewsletterSettings.ascx.cs"
    Inherits="CMSModules_Newsletters_FormControls_Cloning_Newsletter_NewsletterSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblSubscribers" ResourceString="clonning.settings.newsletter.subscribers"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkSubscribers" Checked="true" />
        </td>
    </tr>
</table>
