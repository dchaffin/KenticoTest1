<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SubscriptionEdit.ascx.cs"
    Inherits="CMSModules_Reporting_Controls_SubscriptionEdit" %>
<%@ Register Src="~/CMSAdminControls/UI/Selectors/ScheduleInterval.ascx" TagName="ScheduleInterval"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" TagName="ConditionBuilder"
    TagPrefix="cms" %>
<asp:PlaceHolder runat="server" ID="pnlInfo" Visible="false">
    <asp:Label runat="server" ID="lblInfo" />
    <br />
    <br />
</asp:PlaceHolder>
<asp:Panel runat="server" ID="pnlBasic" CssClass="SubscriptionGroupingBoxWidth">
    <table>
        <tr>
            <td>
                <cms:LocalizedLabel runat="server" ID="lblUser" ResourceString="reportsubscription.sendto"
                    DisplayColon="true" />
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtEmail" CssClass="ReportSubscriptionSubject" MaxLength="400" />
                <cms:CMSRequiredFieldValidator runat="server" ControlToValidate="txtEmail" ID="rfvEmail"
                    Display="Dynamic" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel runat="server" ID="lblSubject" ResourceString="general.subject"
                    DisplayColon="true" />
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtSubject" CssClass="ReportSubscriptionSubject"
                    MaxLength="200" />
                <cms:CMSRequiredFieldValidator runat="server" ControlToValidate="txtSubject" ID="rfvSubject"
                    Display="Dynamic" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
            </td>
        </tr>
        <asp:PlaceHolder runat="server" ID="pnlSubscription">
            <tr>
                <td>
                    <cms:LocalizedLabel runat="server" ID="lblItem" ResourceString="reportsubscription.item"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="drpItems" CssClass="ReportSubscriptionSubject" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="pnlFromToParams">
            <tr>
                <td>
                    <cms:LocalizedLabel runat="server" ResourceString="reportsubscription.timerange"
                        ID="lblLast" DisplayColon="true" />
                </td>
                <td>
                    <asp:RadioButton runat="server" ID="rbAll" GroupName="Time" OnClick="disableLast(true)" />
                    <br />
                    <asp:RadioButton runat="server" ID="rbTime" GroupName="Time" OnClick="disableLast(false)" />
                </td>
            </tr>
            <tr>
                <td class="ReportSubscriptionTimeParameters">
                    <cms:LocalizedLabel runat="server" ID="lblParametersLabel" ResourceString="reportsubscription.ParametersLabel"
                        DisplayColon="true" />
                </td>
                <td class="SubscriptionTimeInfo ReportSubscriptionTimeParameters">
                    <asp:TextBox runat="server" ID="txtLast" CssClass="SuperSmallTextBox" />
                    &nbsp;
                    <asp:DropDownList runat="server" ID="drpLast" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
                <cms:LocalizedLabel runat="server" ID="lblInterval" ResourceString="reportsubscription.interval"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:ScheduleInterval ID="ucInterval" runat="server" />
            </td>
        </tr>
        <asp:PlaceHolder runat="server" ID="pnlCondition">
            <tr>
                <td>
                    <cms:LocalizedLabel runat="server" ID="lblCondition" ResourceString="macros.macrorule.condition"
                        DisplayColon="true" />
                </td>
                <td>
                    <cms:ConditionBuilder ID="ucMacroEditor" runat="server" CssClass="ReportSubscription"
                        RuleCategoryNames="cms.reporting" MaxWidth="300" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
                <cms:LocalizedLabel runat="server" ID="lblOnlyNonEmpty" ResourceString="reportsubscription.notempty"
                    DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox runat="server" ID="chkNonEmpty" />
            </td>
        </tr>
        <asp:PlaceHolder runat="server" ID="pnlEnabled">
            <tr>
                <td>
                    <cms:LocalizedLabel runat="server" ID="lblEnabled" ResourceString="general.enabled"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkEnabled" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</asp:Panel>
<asp:Panel ID="pnlParametersEnvelope" runat="server">
    <br />
    <asp:Panel runat="server" ID="pnlParameters" CssClass="SubscriptionGroupingBoxWidth">
        <asp:Panel runat="server" ID="pnlBasicForm" CssClass="LeftAlign SubscriptionParameters">
            <cms:BasicForm runat="server" ID="formElem" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
</table> 