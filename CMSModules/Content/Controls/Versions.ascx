<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Versions.ascx.cs" Inherits="CMSModules_Content_Controls_Versions" %>
<%@ Register Src="~/CMSModules/Content/Controls/VersionList.ascx" TagName="VersionList"
    TagPrefix="cms" %>
<asp:Label ID="lblWorkflow" runat="server" CssClass="InfoLabel" EnableViewState="false"
    Visible="false" />
<asp:Panel runat="server" ID="pnlVersions">
    <div style="width: 100%">
        <table width="100%">
            <asp:PlaceHolder ID="plcForm" runat="server">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblCheckInfo" runat="server" Font-Bold="true" EnableViewState="false" />
                        <asp:Panel runat="server" ID="pnlForm">
                            <table>
                                <tr>
                                    <td style="vertical-align: top;">
                                        <cms:localizedlabel id="lblVersion" runat="server" resourcestring="VersionsProperties.Version" />
                                    </td>
                                    <td>
                                        <cms:cmstextbox id="txtVersion" runat="server" cssclass="TextBoxField" maxlength="50" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align: top;">
                                        <cms:localizedlabel id="lblComment" runat="server" resourcestring="VersionsProperties.Comment"
                                            enableviewstate="false" />
                                    </td>
                                    <td>
                                        <cms:cmstextbox id="txtComment" runat="server" textmode="MultiLine" cssclass="TextAreaField" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <cms:localizedbutton id="btnCheckout" runat="server" cssclass="SubmitButton" visible="false"
                                            onclick="btnCheckout_Click" resourcestring="VersionsProperties.btnCheckout" enableviewstate="false" />
                                        <cms:localizedbutton id="btnCheckin" runat="server" cssclass="SubmitButton" visible="false"
                                            onclick="btnCheckin_Click" resourcestring="VersionsProperties.btnCheckin" enableviewstate="false" />
                                        <cms:localizedbutton id="btnUndoCheckout" runat="server" visible="false" onclick="btnUndoCheckout_Click"
                                            resourcestring="VersionsProperties.btnUndoCheckout" cssclass="LongSubmitButton"
                                            enableviewstate="false" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td colspan="2">
                    <cms:versionlist id="versionsElem" runat="server" islivesite="false" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
