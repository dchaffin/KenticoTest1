<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ecommerce_ExchangeTableSettings.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_ExchangeTableSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblValidFrom" ResourceString="clonning.settings.exchangetable.validfrom"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:DateTimePicker runat="server" ID="dtValidFrom" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblValidTo" ResourceString="clonning.settings.exchangetable.validto"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:DateTimePicker runat="server" ID="dtValidTo" />
        </td>
    </tr>
</table>
