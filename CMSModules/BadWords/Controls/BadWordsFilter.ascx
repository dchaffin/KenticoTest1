<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BadWordsFilter.ascx.cs" Inherits="CMSModules_BadWords_Controls_BadWordsFilter" %>
<%@ Register Src="~/CMSModules/BadWords/FormControls/SelectBadWordAction.ascx" TagPrefix="cms"
    TagName="SelectBadWordAction" %>
<table>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblExpression" runat="server" ResourceString="Unigrid.BadWords.Columns.WordExpression"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtExpression" runat="server" CssClass="TextBoxField" MaxLength="50" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblAction" runat="server" ResourceString="Unigrid.BadWords.Columns.WordAction"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:SelectBadWordAction ID="ucBadWordAction" runat="server" AllowNoSelection="true" ReloadDataOnPostback="false" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:LocalizedButton ID="btnShow" runat="server" ResourceString="general.show" CssClass="ContentButton" OnClick="btnShow_Click"/>
                <asp:LinkButton ID="btnReset" runat="server" Style="line-height:2em;margin:1em" EnableViewState="false" />
            </td>
        </tr>
    </table>
    <br />