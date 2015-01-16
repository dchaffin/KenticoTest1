<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Preview.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Preview_Preview" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectSinglePath.ascx"
    TagName="pathSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/LanguageMenu.ascx" TagName="LanguageMenu"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/DeviceProfile/Controls/ProfilesMenuControl.ascx" TagName="SelectDevice"
    TagPrefix="cms" %>
<asp:Button ID="btnLanguage" CssClass="Hidden" runat="server" />
<asp:Button ID="btnDevice" CssClass="Hidden" runat="server" />
<asp:Panel runat="server" ID="pnlPreviewContent" DefaultButton="imgShow">
    <asp:Panel runat="server" ID="pnlHeader">
        <div class="NavigationButtons FloatLeft UniMenuContent">
            <table>
                <tr>
                    <td>
                        <cms:pathSelector runat="server" ID="ucPath" IsLiveSite="false" OnChanged="ucPath_pathchanged" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        <div class="SplitToolbarSeparator">
                        </div>
                    </td>
                    <td class="PreviewColumnPadding">
                        <cms:LanguageMenu runat="server" ID="ucSelectCulture" CompactMode="true" DisplayCompare="false"
                            UseSmallLanguageButton="true" />
                    </td>
                    <asp:PlaceHolder runat="server" ID="pnlDevice">
                        <td class="PreviewColumnPadding">
                            <cms:SelectDevice runat="server" ID="ucSelectDevice" UseSmallLanguageButton="true" />
                        </td>
                    </asp:PlaceHolder>
                    <td>
                        <div class="SplitToolbarSeparator">
                        </div>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        <asp:ImageButton runat="server" ID="imgShow" OnClick="btnShow_clicked" AlternateText="Refresh" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <div class="FloatRight PreviewHelp">
        <cms:Help ID="helpElem" runat="server" TopicName="preview_panel" />
    </div>
</asp:Panel>
