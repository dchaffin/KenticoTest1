<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_CategorySettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_CategorySettings" %>
    <%@ Register Src="~/CMSModules/Categories/Controls/SelectCategory.ascx" TagName="SelectCategory" TagPrefix="cms" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblCategory" ResourceString="clonning.settings.category.parentcategory"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:SelectCategory runat="server" ID="drpCategories" AddNoneRecord="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblSubcategories" ResourceString="clonning.settings.category.subcategories"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkSubcategories" Checked="true" />
        </td>
    </tr>
</table>
