<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DatabaseDialog.ascx.cs"
    Inherits="CMSInstall_Controls_WizardSteps_DatabaseDialog" %>
<div class="InstallContent">
    <table class="InstallWizard" border="0" cellpadding="0" cellspacing="0">
        <asp:PlaceHolder ID="plcRadNew" runat="server">
            <tr>
                <td colspan="3">
                    <asp:Label ID="lblDatabase" AssociatedControlID="radCreateNew" runat="server" CssClass="InstallGroupTitle UserServer"
                        EnableViewState="False" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcNewDB">
            <tr>
                <td colspan="3">
                    <asp:RadioButton ID="radCreateNew" runat="server" AutoPostBack="True" GroupName="DatabaseType"
                        Checked="True"></asp:RadioButton>
                </td>
            </tr>
            <tr>
                <td style="width: 25px;">
                    &nbsp;
                </td>
                <td nowrap="nowrap" align="left" style="width: 140px; padding-right: 8px;">
                    <asp:Label ID="lblNewDatabaseName" AssociatedControlID="txtNewDatabaseName" runat="server"
                        EnableViewState="False" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtNewDatabaseName" CssClass="InstallFormTextBox" runat="server"
                        Enabled="False" MaxLength="90" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcExistingRadio" runat="server">
            <tr>
                <td colspan="3">
                    <asp:RadioButton ID="radUseExisting" runat="server" AutoPostBack="True" GroupName="DatabaseType" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td style="width: 25px;">
                &nbsp;
            </td>
            <td nowrap="nowrap" align="left" style="width: 140px; padding-right: 8px;">
                <asp:Label ID="lblExistingDatabaseName" AssociatedControlID="txtExistingDatabaseName"
                    runat="server" EnableViewState="False" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtExistingDatabaseName" CssClass="InstallFormTextBox" runat="server"
                    MaxLength="90" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcCreateDatabaseObjects" runat="server">
            <tr>
                <td>
                </td>
                <td colspan="2">
                    <asp:CheckBox ID="chkCreateDatabaseObjects" runat="server" CssClass="InstallCreateDBObjects"
                        Checked="True"></asp:CheckBox>
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcAdvanced" runat="server" Visible="false">
            <tr>
                <td style="width: 25px;">
                    &nbsp;
                </td>
                <td nowrap="nowrap" align="left" style="width: 140px;" colspan="2">
                    <asp:HyperLink ID="lblAdvanced" AssociatedControlID="txtExistingDatabaseName" runat="server"
                        Style="cursor: pointer;" />
                </td>
            </tr>
            <tr>
                <td style="width: 25px;">
                    &nbsp;
                </td>
                <td nowrap="nowrap" align="left" style="width: 140px;">
                    <asp:Label ID="lblSchema" AssociatedControlID="txtSchema" runat="server" Style="display: none;"
                        EnableViewState="False" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtSchema" CssClass="InstallFormTextBox" runat="server" Style="display: none;"
                        Text="dbo" ReadOnly="true" Enabled="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcRunningTasks" runat="server" Visible="False">
            <asp:PlaceHolder ID="plcEmptyLine" runat="server">
                <tr>
                    <td colspan="3">
                        &nbsp;
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td style="width: 25px;">
                    &nbsp;
                </td>
                <td nowrap="nowrap" align="left" style="width: 140px;">
                    <cms:CMSUpdatePanel runat="server" ID="pnlUpdateText" UpdateMode="Always">
                        <ContentTemplate>
                            <cms:LocalizedLabel ID="lblTaskStatus" runat="server" ResourceString="separationDB.taskstatus"
                                EnableViewState="False" DisplayColon="True" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </td>
                <td>
                    <div class="FloatLeft">
                        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Timer ID="timRefresh" runat="server" Interval="3000" EnableViewState="false" />
                                <cms:LocalizedLabel runat="server" ID="lblStatusValue" EnableViewState="false" CssClass="FloatLeft" />
                                <asp:Literal runat="server" ID="ltlStatus" EnableViewState="false" />
                                <cms:LocalizedLinkButton ID="btnStopTasks" runat="server" ResourceString="separationDB.stoptasks" CssClass="FloatLeft" />
                                <asp:HiddenField runat="server" ID="hdnTurnedOff" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                    <asp:Image ID="imgHelp" runat="server" EnableViewState="False" CssClass="SeparationHelp FloatLeft" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcSeparationError" runat="server" EnableViewState="False" Visible="False">
            <tr>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3" align="left">
                    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</div>
