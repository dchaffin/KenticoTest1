<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_LanguageMenu"
    CodeFile="LanguageMenu.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniMenu/UniMenuButtons.ascx" TagName="UniMenuButtons"
    TagPrefix="cms" %>
<div class="ActionButtons">
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td class="ContentMenuCultureButtons">
                <cms:UniMenuButtons ID="btnCultures" ShortID="b" runat="server" EnableViewState="false" />
                <cms:UniMenuButtons ID="buttons" ShortID="b" runat="server" EnableViewState="false"
                    AllowSelection="true" CheckChanges="true" />
            </td>
            <asp:PlaceHolder runat="server" ID="pnlCompare">
                <td>
                    <div class="ContentMenuSplitSeparator">
                    </div>
                </td>
                <td>
                    <div class="ContentMenuSplit">
                        <cms:UniMenuButtons ID="splitView" ShortID="s" runat="server" EnableViewState="false"
                            CheckChanges="true" />
                    </div>
                </td>
            </asp:PlaceHolder>
        </tr>
    </table>
</div>
