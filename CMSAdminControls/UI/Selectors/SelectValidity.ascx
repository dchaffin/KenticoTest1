<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectValidity.ascx.cs"
    Inherits="CMSAdminControls_UI_Selectors_SelectValidity" %>
<asp:Panel ID="pnlSelectValidity" runat="server">
    <asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
        Visible="false" />
    <%-- Valid for --%>
    <div>
        <cms:LocalizedRadioButton ID="radDays" runat="server" GroupName="Validity" ResourceString="general.selectvalidity.days"
            OnCheckedChanged="ValidityRadioGroup_CheckedChanged" />
        <cms:LocalizedRadioButton ID="radWeeks" runat="server" GroupName="Validity" ResourceString="general.selectvalidity.weeks"
            OnCheckedChanged="ValidityRadioGroup_CheckedChanged" />
        <cms:LocalizedRadioButton ID="radMonths" runat="server" GroupName="Validity" ResourceString="general.selectvalidity.months"
            OnCheckedChanged="ValidityRadioGroup_CheckedChanged" />
        <cms:LocalizedRadioButton ID="radYears" runat="server" GroupName="Validity" ResourceString="general.selectvalidity.years"
            OnCheckedChanged="ValidityRadioGroup_CheckedChanged" />
    </div>
    <div style="margin-top: 5px;">
        <cms:CMSTextBox ID="txtValidFor" runat="server" CssClass="TextBoxField" />
    </div>
    <%-- Valid until --%>
    <div style="margin-top: 5px;">
        <cms:LocalizedRadioButton ID="radUntil" runat="server" GroupName="Validity" ResourceString="general.selectvalidity.until"
            OnCheckedChanged="ValidityRadioGroup_CheckedChanged" Checked="true" />
    </div>
    <div style="margin-top: 5px;">
        <cms:DateTimePicker ID="untilDateElem" runat="server" EditTime="false" AllowEmptyValue="true"
            DisplayNA="true" />
    </div>
</asp:Panel>
