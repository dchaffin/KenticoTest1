<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_DocumentTypes_Controls_HierarchicalTransformations_Edit"
    CodeFile="HierarchicalTransformations_Edit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Classes/SelectTransformation.ascx" TagName="SelectTransformation"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/SelectClassNames.ascx" TagName="ClassSelector"
    TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<table>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ResourceString="documenttype_edit_transformation_edit.transformtype" />
        </td>
        <td>
            <asp:DropDownList runat="server" ID="drpTemplateType" CssClass="DropDownField" AutoPostBack="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblDocTypes" runat="server" ResourceString="development.documenttypes" />
        </td>
        <td>
            <cms:ClassSelector runat="server" ID="ucClassSelector" IsLiveSite="false" SiteID="0" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblLevel" runat="server" ResourceString="development.level" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtLevel" runat="server" CssClass="TextBoxField" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblName" runat="server" ResourceString="documenttype_edit_transformation_edit.transformname" />
        </td>
        <td>
            <cms:SelectTransformation ID="ucTransformations" runat="server" IsLiveSite="false"
                EditWindowName="EditLevel2" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
        <td>
            <cms:FormSubmitButton ID="btnOK" runat="server" OnClick="btnOK_Click" />
        </td>
    </tr>
</table>
