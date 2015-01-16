<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_Layout_SaveNewPageTemplate"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Page template - Save as new" CodeFile="SaveNewPageTemplate.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/PageTemplates/SelectPageTemplate.ascx"
    TagName="SelectPageTemplate" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[
        function SelectTemplate(templateId) {
            wopener.OnSelectPageTemplate(templateId);
            CloseDialog();
        }
        //]]>
    </script>
    <asp:Panel runat="server" ID="pnlContent" CssClass="PageContent">
        <table>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblTemplateDisplayName" runat="server" CssClass="ContentLabel"
                        ResourceString="PortalEngine.SaveNewPageTemplate.DisplayName" />
                </td>
                <td>
                    <cms:LocalizableTextBox ID="txtTemplateDisplayName" MaxLength="200" runat="server"
                        CssClass="TextBoxField" />
                    <br />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblTemplateCodeName" runat="server" CssClass="ContentLabel"
                        ResourceString="PortalEngine.SaveNewPageTemplate.CodeName" />
                </td>
                <td>
                    <cms:CodeName ID="txtTemplateCodeName" MaxLength="100" runat="server" CssClass="TextBoxField" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblTemplateCategory" runat="server" CssClass="ContentLabel"
                        ResourceString="PortalEngine.SaveNewPageTemplate.Category" />
                </td>
                <td>
                    <cms:SelectPageTemplate ID="categorySelector" runat="server" ShowTemplates="false"
                        EnableCategorySelection="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblTemplateDescription" runat="server" CssClass="ContentLabel"
                        ResourceString="PortalEngine.SaveNewPageTemplate.Description" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtTemplateDescription" runat="server" TextMode="MultiLine" CssClass="TextAreaField" />
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcKeep">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblKeep" runat="server" CssClass="ContentLabel" ResourceString="PortalEngine.SaveNewPageTemplate.Keep" />
                    </td>
                    <td>
                        <cms:LocalizedCheckBox ID="chkKeep" MaxLength="200" runat="server" Checked="true" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" CssClass="SubmitButton" EnableViewState="False"
            OnClick="btnOK_Click" /><cms:LocalizedButton ID="btnCancel" runat="server" CssClass="SubmitButton"
                EnableViewState="False" OnClientClick="CloseDialog()" />
    </div>
</asp:Content>
