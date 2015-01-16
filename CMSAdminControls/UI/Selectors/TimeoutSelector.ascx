<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TimeoutSelector.ascx.cs"
    Inherits="CMSAdminControls_UI_Selectors_TimeoutSelector" %>

<%@ Register Src="~/CMSFormControls/Basic/TimeControl.ascx" TagName="TimeControl" TagPrefix="cms" %>

<cms:LocalizedRadioButton ID="rbNoneMode" runat="server" GroupName="mode" ResourceString="general.none"
    AutoPostBack="true" OnCheckedChanged="rbMode_CheckedChanged" />
<cms:LocalizedRadioButton ID="rbIntervalMode" runat="server" AutoPostBack="true"
    GroupName="mode" ResourceString="timeoutselector.specificinterval" OnCheckedChanged="rbMode_CheckedChanged" />
<cms:LocalizedRadioButton ID="rbDateMode" runat="server" AutoPostBack="true" GroupName="mode"
    ResourceString="timeoutselector.specificdate" OnCheckedChanged="rbMode_CheckedChanged" />
<asp:PlaceHolder ID="plcSep" runat="server">
    <br />
    <br />
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcInterval" runat="server">
    <table>
        <tr>
            <td colspan="3">
                <cms:FormLabel ID="lblAfter" runat="server" ResourceString="timeoutselector.after"
                    CssClass="FieldLabel" DisplayColon="true" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:CMSTextBox ID="txtQuantity" runat="server" CssClass="ShortTextBox" MaxLength="4" />
            </td>
            <td>
                <asp:DropDownList ID="drpScale" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpScale_SelectedIndexChanged" />
            </td>
            <td>
                <cms:CMSRangeValidator ID="rvQuantity" runat="server" ControlToValidate="txtQuantity"
                    Type="integer" Display="dynamic" />
                <cms:CMSRequiredFieldValidator ID="rfvQuantity" runat="server" ControlToValidate="txtQuantity"
                    Display="dynamic" />
            </td>
        </tr>
    </table>
    <asp:PlaceHolder ID="plcDateMode" runat="server">
        <br />
        <table>
            <asp:PlaceHolder ID="plcExactly" runat="server">
                <tr>
                    <td>
                        <cms:LocalizedRadioButton ID="rbExactly" runat="server" GroupName="date" ResourceString="timeoutselector.exactly"
                            AutoPostBack="true" OnCheckedChanged="rbNext_CheckedChanged" />
                    </td>
                    <td>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcNextDate" runat="server">
                <tr>
                    <td>
                        <cms:LocalizedRadioButton ID="rbNextDate" runat="server" GroupName="date" AutoPostBack="true"
                            OnCheckedChanged="rbNext_CheckedChanged" />
                    </td>
                    <td>
                        <asp:PlaceHolder ID="plcNextDateMonth" runat="server">
                            <asp:DropDownList ID="drpNextDateMonth" runat="server" OnSelectedIndexChanged="drpNextDate_DaysCountChanged"
                                AutoPostBack="true" />
                            &nbsp;</asp:PlaceHolder>
                        <asp:DropDownList ID="drpNextDate" runat="server" />
                        &nbsp;<cms:LocalizedLabel ID="lblNextDateOfTheMonth" runat="server" ResourceString="timeoutselector.ofthemonth" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcNextDay" runat="server">
                <tr>
                    <td>
                        <cms:LocalizedRadioButton ID="rbNextDay" GroupName="date" runat="server" ResourceString="timeoutselector.thenext"
                            AutoPostBack="true" OnCheckedChanged="rbNext_CheckedChanged" />
                    </td>
                    <td>
                        <asp:DropDownList ID="drpNextOrder" runat="server" />
                        &nbsp;<asp:DropDownList ID="drpNextDay" runat="server" />
                        &nbsp;<cms:LocalizedLabel ID="lblNextDayOfTheMonth" runat="server" ResourceString="timeoutselector.ofthemonth" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </asp:PlaceHolder>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcDate" runat="server" Visible="false">
    <table>
        <tr>
            <td style="min-width: 30px;">
                <cms:LocalizedLabel ID="lblDate" runat="server" ResourceString="general.date" DisplayColon="true" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:DateTimePicker ID="dateTimePicker" runat="server" SupportFolder="~/CMSAdminControls/Calendar"
                    EditTime="false" />
            </td>
        </tr>
    </table>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcSpecificTime" runat="server">
    <br />
    <table>
        <tr style="line-height: 23px">
            <td>
                <cms:LocalizedCheckBox ID="cbSpecificTime" runat="server" ResourceString="timeoutselector.inspecifictime"
                    AutoPostBack="true" OnCheckedChanged="cbSpecificTime_CheckedChanged" />
            </td>
            <td>
                <asp:PlaceHolder ID="plcHourMinute" runat="server">
                    <cms:TimeControl ID="timePicker" runat="server" />
                </asp:PlaceHolder>
            </td>
        </tr>
    </table>
</asp:PlaceHolder>
