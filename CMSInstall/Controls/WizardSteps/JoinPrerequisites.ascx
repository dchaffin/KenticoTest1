<%@ Control Language="C#" AutoEventWireup="true" CodeFile="JoinPrerequisites.ascx.cs"
    Inherits="CMSInstall_Controls_WizardSteps_JoinPrerequisites" %>
<div class="InstallContent">
    <table class="InstallWizard" border="0" cellpadding="0" cellspacing="0">
        <asp:PlaceHolder ID="plcTasks" runat="server">
            <tr>
                <td style="width: 25px;">
                    &nbsp;
                </td>
                <td nowrap="nowrap" align="left" style="width: 150px;">
                    <cms:CMSUpdatePanel runat="server" ID="pnlUpdateText" UpdateMode="Always">
                        <ContentTemplate>
                            <cms:LocalizedLabel ID="lblTaskStatus" runat="server" ResourceString="separationDB.taskstatus"
                                EnableViewState="False" DisplayColon="True" CssClass="FieldLabel" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </td>
                <td style="height: 35px">
                    <div class="FloatLeft">
                        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Timer ID="timRefresh" runat="server" Interval="3000" EnableViewState="false" />
                                <cms:LocalizedLabel runat="server" ID="lblStatusValue" EnableViewState="false" CssClass="FloatLeft" />
                                <asp:Literal runat="server" ID="ltlStatus" EnableViewState="false" />
                                <cms:LocalizedLinkButton ID="btnStopTasks" runat="server" ResourceString="separationDB.stoptasks"  CssClass="FloatLeft" />
                                <asp:HiddenField runat="server" ID="hdnTurnedOff" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                    <asp:Image ID="imgHelp" runat="server" EnableViewState="False" CssClass="SeparationHelp FloatLeft" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcInfo" runat="server" EnableViewState="false" Visible="False">
            <tr>
                <td style="width: 25px;">
                    &nbsp;
                </td>
                <td colspan="2" align="left">
                    <cms:LocalizedLabel ID="lblInfo" runat="server" ResourceString="separationDB.joinready" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcSeparationError" runat="server" EnableViewState="False" Visible="False">
            <tr>
                <td colspan="3" align="left">
                    <asp:Label ID="lblErrorTasks" runat="server" CssClass="ErrorLabel" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</div>
