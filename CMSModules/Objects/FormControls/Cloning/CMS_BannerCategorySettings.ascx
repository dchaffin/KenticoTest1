<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_BannerCategorySettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_BannerCategorySettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblCloneBanners" ResourceString="clonning.settings.bannercategory.banners"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkCloneBanners" Checked="true" />
        </td>
    </tr>
</table>
