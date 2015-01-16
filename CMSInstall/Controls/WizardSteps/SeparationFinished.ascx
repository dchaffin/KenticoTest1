<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SeparationFinished.ascx.cs"
    Inherits="CMSInstall_Controls_WizardSteps_SeparationFinished" %>
<div class="InstallContent">
    <table class="InstallWizard" border="0" cellpadding="0" cellspacing="0">
        <asp:PlaceHolder ID="plcContent" runat="server">
            <tr>
                <td class="InstallCompleted SeparationFinished" colspan="3">
                    <cms:LocalizedLabel ID="lblCompleted" runat="server" ResourceString="separationDB.OK"
                        CssClass="FiledLabel" />
                </td>
            </tr>
            <asp:PlaceHolder ID="plcDeleteOldDB" runat="server">
                <tr>
                    <td style="width: 180px">
                        <cms:LocalizedLabel ID="lblDeleteOldDB" runat="server" EnableViewState="False" AssociatedControlID="chkDeleteOldDB"
                            CssClass="FieldLabel" DisplayColon="True" ResourceString="separationDB.deleteOldDB" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkDeleteOldDB" runat="server" Checked="True" CssClass="FloatLeft" /><asp:Image
                            ID="imgHelp" runat="server" EnableViewState="False" CssClass="SeparationHelp FloatLeft" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcChangeCollation" runat="server" Visible="false">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblChangeCollation" runat="server" EnableViewState="False"
                            CssClass="FieldLabel" />
                    </td>
                    <td>
                        <cms:LocalizedLinkButton ID="btnChangeCollation" runat="server" OnClick="btnChangeCollation_Click" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <cms:LocalizedLabel ID="lblError" runat="server" CssClass="SelectorError" Visible="true" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcAzureError" runat="server" Visible="False">
            <tr>
                <td colspan="3">
                    <div class="SeparationAzureError">
                        <cms:LocalizedLabel ID="lblAzureError" runat="server" CssClass="SelectorError" Visible="False" />
                    </div>
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</div>
