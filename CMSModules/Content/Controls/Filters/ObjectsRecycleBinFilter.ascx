<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ObjectsRecycleBinFilter.ascx.cs"
    Inherits="CMSModules_Content_Controls_Filters_ObjectsRecycleBinFilter" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Objects/FormControls/BinObjectTypeSelector.ascx" TagName="ObjectTypeSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlContent" runat="server" DefaultButton="btnShow">
    <table>
        <asp:PlaceHolder ID="plcUsers" runat="server">
            <tr>
                <td class="FieldLabel">
                    <cms:localizedlabel id="lblUsers" runat="server" displaycolon="true" resourcestring="general.user" />
                </td>
                <td>
                    <cms:selectuser id="userSelector" runat="server" islivesite="false" selectionmode="SingleDropDownList"
                        allowall="true" allowempty="false" showsitefilter="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcNameFilter" runat="server">
            <tr>
                <td class="FieldLabel">
                    <cms:localizedlabel id="lblName" runat="server" displaycolon="true" resourcestring="general.objectname" />
                </td>
                <td>
                    <cms:textsimplefilter id="nameFilter" runat="server" column="VersionObjectDisplayName" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcObjectTypeFilter" runat="server">
            <tr>
                <td class="FieldLabel">
                    <cms:localizedlabel id="lblObjectType" runat="server" displaycolon="true" resourcestring="general.objecttype" />
                </td>
                <td>
                    <cms:cmsupdatepanel id="pnlObjType" runat="server" updatemode="Conditional">
                        <ContentTemplate>
                            <cms:ObjectTypeSelector ID="objTypeSelector" runat="server" />
                        </ContentTemplate>
                    </cms:cmsupdatepanel>
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
            </td>
            <td>
                <cms:localizedbutton id="btnShow" runat="server" resourcestring="general.show" cssclass="ContentButton"
                    onclick="btnShow_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>
