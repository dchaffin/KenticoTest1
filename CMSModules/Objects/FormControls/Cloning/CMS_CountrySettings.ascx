<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_CountrySettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_CountrySettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblTwoLetterCode" ResourceString="clonning.settings.country.twolettercode"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtTwoLetterCode" MaxLength="2" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblThreeLetterCode" ResourceString="clonning.settings.country.threelettercode"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtThreeLetterCode" MaxLength="3" />
        </td>
    </tr>
</table>
