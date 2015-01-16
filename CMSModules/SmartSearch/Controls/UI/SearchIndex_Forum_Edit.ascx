<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_Forum_Edit" CodeFile="SearchIndex_Forum_Edit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SelectSite" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<cms:CMSUpdatePanel runat="server" ID="pnlForumEdit" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />      
        <table>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" ResourceString="srch.index.site"
                        DisplayColon="true" />
                </td>
                <td>
                    <cms:SelectSite IsLiveSite="false" ID="selSite" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblForum" EnableViewState="false" ResourceString="srch.index.forum"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:PlaceHolder runat="server" ID="plcForumSelector" />
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcForumsInfo">
                <tr>
                    <td>
                    </td>
                    <td>
                        <cms:LocalizedLabel runat="server" ID="lblClassNamesInfo" ResourceString="srch.index.forumsinfo"
                            CssClass="ContentLabelItalic"></cms:LocalizedLabel>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td>
                </td>
                <td>
                    <br />
                    <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</cms:CMSUpdatePanel>
