<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DocumentToolbar.ascx.cs"
    Inherits="CMSModules_Content_Controls_SplitView_Documents_DocumentToolbar" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<asp:Panel ID="pnlMain" CssClass="SplitToolbar Vertical" runat="server">
    <div class="LeftAlign">
        <table>
            <tr>
                <td>
                    <div id="divHorizontal" runat="server" class="Button Selected">
                        <asp:Image ID="imgHorizontal" runat="server" />
                    </div>
                </td>
                <td>
                    <div id="divVertical" runat="server" class="Button">
                        <asp:Image ID="imgVertical" runat="server" />
                    </div>
                </td>
                <td>
                    <div class="SplitToolbarSeparator">
                    </div>
                </td>
                <td>
                    <div class="SplitToolbarSync">
                        <asp:Image ID="imgSync" runat="server" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="divRight" class="RightAlign" runat="server">
        <div class="LeftAlign">
            <table class="LangSelector">
                <tr>
                    <td>
                        <asp:DropDownList ID="drpCultures" runat="server" AutoPostBack="false" CssClass="DropDownField" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="RightAlign">
            <table class="SplitToolbarHelpClose">
                <tr>
                    <td>
                        <cms:help id="helpElem" runat="server" topicname="splitview" />
                    </td>
                    <td>
                        <asp:Image ID="imgClose" runat="server" CssClass="Close" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="Clear" />
</asp:Panel>
