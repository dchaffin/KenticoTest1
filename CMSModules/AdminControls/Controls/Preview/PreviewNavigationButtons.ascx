<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PreviewNavigationButtons.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Preview_PreviewNavigationButtons" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Preview/Preview.ascx" TagName="Preview"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlHeaderSeparator" CssClass="HeaderSeparator">
    &nbsp;
</asp:Panel>
<div class="Preview">
    <asp:Panel ID="pnlMain" CssClass="SplitToolbar Vertical" runat="server">
            <div class="SplitLeftAlign">
                <table>
                    <tr>
                        <td>
                            <div id="divNoSplit" runat="server" class="Button">
                                <input type="image" runat="server" id="imgNoSplit" />
                            </div>
                        </td>
                        <td>
                            <div id="divHorizontal" runat="server" class="Button">
                                <input type="image" runat="server" id="imgHorizontal" />
                            </div>
                        </td>
                        <td>
                            <div id="divVertical" runat="server" class="Button">
                                <input type="image" runat="server" id="imgVertical" />
                            </div>
                        </td>
                        <td>
                            <div class="SplitToolbarSeparator">
                            </div>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </div>
            <div class="SplitRightAlign Buttons" runat="server" id="pnlButtons">
                <cms:Preview runat="server" ID="ucButtons" />
            </div>
    </asp:Panel>
</div>
