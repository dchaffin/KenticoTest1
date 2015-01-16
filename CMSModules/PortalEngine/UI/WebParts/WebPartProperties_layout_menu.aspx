<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebPartProperties_layout_menu.aspx.cs"
    Inherits="CMSModules_PortalEngine_UI_WebParts_WebPartProperties_layout_menu"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalSimplePage.master" Theme="Default" %>

<%@ Register Src="~/CMSModules/PortalEngine/FormControls/WebPartLayouts/WebPartLayoutSelector.ascx"
    TagPrefix="cms" TagName="LayoutSelector" %>
<asp:Content ContentPlaceHolderID="plcSiteSelector" ID="cntContent" runat="server">
    <div class="WebPartLayoutMenu" id="pnlContent" runat="server">
        <table>
            <tr>
                <td>
                    <asp:Label ID="lblLayouts" runat="server" />
                </td>
                <td>
                    <cms:LayoutSelector runat="server" ID="selectLayout" OnChanged="drpLayouts_Changed"
                        IsLiveSite="false" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
