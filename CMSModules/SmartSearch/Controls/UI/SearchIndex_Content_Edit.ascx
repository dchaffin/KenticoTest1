<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_Content_Edit" CodeFile="SearchIndex_Content_Edit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Classes/selectclassnames.ascx" TagName="SelectClassnames"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/selectpath.ascx" TagName="selectpath" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Panel ID="pnlConetnEdit" runat="server">
    <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />       
    <table>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblLocation" EnableViewState="false" ResourceString="srch.index.location"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:selectpath ID="selectpath" runat="server" IsLiveSite="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblDocTypes" EnableViewState="false" ResourceString="development.doctypes"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:SelectClassnames ID="selectClassnames" runat="server" IsLiveSite="false" SiteID="0" />                
            </td>
        </tr>
        <tr>
            <td>                
            </td>
            <td>
                <cms:LocalizedLabel runat="server" ID="lblClassNamesInfo" ResourceString="srch.index.classnamesinfo" CssClass="ContentLabelItalic" ></cms:LocalizedLabel>
            </td>
        </tr>        
        <asp:PlaceHolder ID="pnlAllowed" runat="server">
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblInclForums" EnableViewState="false" ResourceString="srch.index.inclforums"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkInclForums" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblInclBlog" EnableViewState="false" ResourceString="srch.index.inclblog"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkInclBlog" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblInclBoards" EnableViewState="false" ResourceString="srch.index.inclboards"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkInclBoards" runat="server" />
                </td>
            </tr>
             <tr>
                <td class="FieldLabel">
                <br />
                </td>
                <td>
                </td>
            </tr>
             <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblInclCats" EnableViewState="false" ResourceString="srch.index.inclcats"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:CheckBox ID="chkInclCats" runat="server" />
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
</asp:Panel>
