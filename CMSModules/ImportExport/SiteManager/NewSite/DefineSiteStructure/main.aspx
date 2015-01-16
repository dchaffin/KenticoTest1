<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_SiteManager_NewSite_DefineSiteStructure_main"
    Theme="Default" CodeFile="main.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[

        var selectedNodeId = 0;

        // Refresh action
        function RefreshNode(nodeId, selectNodeId) {
            parent.frames['definestructuretree'].RefreshNode(nodeId, selectNodeId);
        }

        // Selects the node within the tree
        function SelectNode(nodeId) {
            parent.frames['definestructuretree'].SelectNode(nodeId, null);
        }

        function SetSelectedNodeId(nodeId) {
            selectedNodeId = nodeId;
        }

        function GetSelectedNodeId() {
            return selectedNodeId;
        }
        //]]>                        
    </script>
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <table style="margin-top: 10px;" class="ObjectContent">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblPageName" runat="server" ResourceString="DefineSiteStructure.PageName" />
                    </td>
                    <td>
                        <cms:CMSTextBox CssClass="TextBoxField" Width="250" ID="txtPageName" runat="server"
                            MaxLength="450" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblPageTemplate" runat="server" ResourceString="DefineSiteStructure.PageTemplate" />
                    </td>
                    <td colspan="2">
                        <asp:HiddenField runat="server" ID="hdnSelected" />
                        <cms:CMSTextBox CssClass="TextBoxField" Width="250" ID="txtPageTemplate" runat="server"
                            ReadOnly="true" /><cms:LocalizedButton ID="btnSelect" CssClass="ContentButton" runat="server"
                                CausesValidation="false" ResourceString="DefineSiteStructure.SelectPageTemplate"
                                OnClick="btnSelect_Click" /><cms:CMSButton ID="btnInherit" CssClass="LongButton"
                                    OnClick="btnInherit_Click" runat="server" Width="150" CausesValidation="false" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td colspan="2">
                        <cms:LocalizedButton ID="btnSave" CssClass="SubmitButton" runat="server" OnClick="btnSave_Click"
                            ResourceString="general.ok" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:RequiredFieldValidator ID="reqItemName" runat="server" ControlToValidate="txtPageName">&nbsp;</asp:RequiredFieldValidator>
                        <asp:ValidationSummary ID="reqSummary" runat="server" DisplayMode="List" ShowMessageBox="true"
                            ShowSummary="false" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
