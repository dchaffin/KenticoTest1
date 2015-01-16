<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ecommerce_TaxClassSettings.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_TaxClassSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblSKUAssignments" ResourceString="clonning.settings.taxclass.skuassignments"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
           <asp:CheckBox runat="server" ID="chkSKUAssignments" Checked="false" />
        </td>
    </tr>
</table>
