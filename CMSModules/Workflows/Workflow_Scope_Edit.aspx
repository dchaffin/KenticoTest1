<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Workflow_Scope_Edit"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" EnableEventValidation="false"
    Theme="Default" Title="Workflows - Workflow Scopes" CodeFile="Workflow_Scope_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectSinglePath.ascx"
    TagName="SelectSinglePath" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/SelectClassNames.ascx" TagName="SelectClassNames"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" TagName="ConditionBuilder"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel ID="pnlForm" runat="server" CssClass="FormPanel cms_workflowscope">
        <asp:Panel ID="pnlGeneral" runat="server">
            <table>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblStartingAliasPath" runat="server" EnableViewState="false"
                            ResourceString="Development-Workflow_Scope_Edit.StartingAliasPath" />
                    </td>
                    <td>
                        <cms:SelectSinglePath runat="server" ID="pathElem" IsLiveSite="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="top">
                    <td class="FieldLabelTop">
                        <cms:LocalizedLabel ID="lblCover" runat="server" EnableViewState="false" ResourceString="Development-Workflow_Scope_Edit.Coverage" />
                    </td>
                    <td>
                        <cms:LocalizedRadioButton ID="rbDocAndChildren" runat="server" GroupName="Cover"
                            ResourceString="workflowscope.docandchildren" /><br />
                        <cms:LocalizedRadioButton ID="rbDoc" runat="server" GroupName="Cover" ResourceString="workflowscope.doc" /><br />
                        <cms:LocalizedRadioButton ID="rbChildren" runat="server" GroupName="Cover" ResourceString="workflowscope.children" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabelTop">
                        <cms:LocalizedLabel ID="lblType" runat="server" EnableViewState="false" ResourceString="Development-Workflow_Scope_Edit.ScopeType" />
                    </td>
                    <td>
                        <cms:LocalizedRadioButton ID="rbAllowed" runat="server" GroupName="Scope" ResourceString="general.allowedscope"
                            Checked="true" /><br />
                        <cms:LocalizedRadioButton ID="rbExcluded" runat="server" GroupName="Scope" ResourceString="general.excludedscope" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlAdvanced" runat="server">
            <table>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblDocumentTypes" runat="server" EnableViewState="false"
                            ResourceString="Development-Workflow_Scope_Edit.DocumentType" />
                    </td>
                    <td>
                        <cms:SelectClassNames ID="selectClassNames" runat="server" IsLiveSite="false" DisplayClearButton="false" HideWireframe="true" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="plcCulture" runat="server">
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel ID="lblCulture" runat="server" EnableViewState="false" ResourceString="general.culture"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <cms:SiteCultureSelector runat="server" ID="cultureSelector" IsLiveSite="false" AddDefaultRecord="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblCondition" runat="server" EnableViewState="false" ResourceString="cms.workflowscope.condition"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:ConditionBuilder ID="cbCondition" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOK_Click" CssClass="SubmitButton"
                            ResourceString="general.ok" EnableViewState="false" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
