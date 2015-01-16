<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_WidgetSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_WidgetSettings" %>
<%@ Register Src="~/CMSModules/Widgets/FormControls/SelectWidgetCategory.ascx" TagName="SelectWidgetCategory"
    TagPrefix="cms" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblWidgetCategory" ResourceString="clonning.settings.widget.category"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:SelectWidgetCategory ID="categorySelector" runat="server" />
        </td>
    </tr>
</table>
