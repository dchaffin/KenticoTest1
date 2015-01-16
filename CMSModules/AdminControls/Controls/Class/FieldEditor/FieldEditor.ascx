<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_FieldEditor"
    CodeFile="FieldEditor.ascx.cs" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/ControlSettings.ascx"
    TagName="ControlSettings" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/CSSsettings.ascx"
    TagName="CSSsettings" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/DatabaseConfiguration.ascx"
    TagName="DatabaseConfiguration" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/DocumentSource.ascx"
    TagName="DocumentSource" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldAppearance.ascx"
    TagName="FieldAppearance" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/ValidationSettings.ascx"
    TagName="ValidationSettings" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/SimpleMode.ascx"
    TagName="SimpleMode" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldAdvancedSettings.ascx"
    TagName="FieldAdvancedSettings" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/CategoryEdit.ascx"
    TagName="CategoryEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Panel ID="pnlFieldEditor" runat="server" CssClass="FieldEditor">
    <asp:Panel ID="pnlFieldEditorWrapper" runat="server">
        <asp:Panel ID="pnlLeft" runat="server" CssClass="FieldPanelLeft">
            <table border="0" cellpadding="0" cellspacing="0" class="FieldTableMenu">
                <tr>
                    <td>
                        <asp:ListBox ID="lstAttributes" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lstAttributes_SelectedIndexChanged"
                            EnableViewState="true" CssClass="AttributesList" />
                        <div>
                            <cms:LocalizedLinkButton ResourceString="FieldEditor.GenerateCode" runat="server"
                                ID="lnkCode" OnClick="lnkCode_Click" OnClientClick="noProgress=true;" /></div>
                        <div>
                            <cms:LocalizedLinkButton ResourceString="FieldEditor.GetFormDefinition" runat="server"
                                ID="lnkFormDef" OnClick="lnkFormDef_Click" OnClientClick="noProgress=true;" />
                        </div>
                    </td>
                    <td class="FieldMenuButtons">
                        <asp:PlaceHolder ID="plcActions" runat="server">
                            <div class="FieldGrouppedButtons">
                                <asp:ImageButton ID="btnUpAttribute" runat="server" OnClick="btnUpAttribute_Click" />
                                <asp:ImageButton ID="btnDownAttribute" runat="server" OnClick="btnDownAttribute_Click" />
                            </div>
                            <div class="FieldGrouppedButtons">
                                <asp:ImageButton ID="btnNewCategory" runat="server" OnClick="btnNewCategory_Click" />
                                <asp:ImageButton ID="btnNewSysAttribute" runat="server" OnClick="btnNewSysAttribute_Click" />
                                <asp:ImageButton ID="btnNewPrimaryAttribute" runat="server" OnClick="btnNewPrimaryAttribute_Click" />
                            </div>
                            <asp:ImageButton ID="btnNewAttribute" runat="server" OnClick="btnNewAttribute_Click" />
                            <asp:ImageButton ID="btnDeleteItem" runat="server" OnClick="btnDeleteItem_Click" />
                        </asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <td>
                        <cms:DocumentSource ID="documentSource" runat="server" ShortID="ds" />
                    </td>
                </tr>
            </table>
            <asp:PlaceHolder ID="plcQuickSelect" runat="server" EnableViewState="false">
                <cms:LocalizedLabel ID="lblQuickLinks" runat="server" EnableViewState="false" CssClass="Title"
                    ResourceString="fieldeditor.quicklinks" DisplayColon="true" />
                <ul>
                    <li><a href="#Database">
                        <cms:LocalizedLabel ID="lblDatabase" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.database" /></a></li>
                    <asp:PlaceHolder ID="plcQuickAppearance" runat="server">
                        <li><a href="#FieldAppearance">
                            <cms:LocalizedLabel ID="lblField" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.fieldappearance" /></a></li>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcQuickSettings" runat="server">
                        <li><a href="#ControlSettings">
                            <cms:LocalizedLabel ID="lblSettings" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.settings" /></a></li>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcQuickValidation" runat="server">
                        <li><a href="#ValidationSettings">
                            <cms:LocalizedLabel ID="lblValidation" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.validation" /></a></li>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcQuickStyles" runat="server">
                        <li><a href="#CSSStyles">
                            <cms:LocalizedLabel ID="lblStyles" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.styles" /></a></li>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcQuickAdvancedSettings" runat="server">
                        <li><a href="#FieldAdvancedSettings">
                            <cms:LocalizedLabel ID="lblAdvanced" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.fieldadvancedsettings" /></a></li>
                    </asp:PlaceHolder>
                </ul>
            </asp:PlaceHolder>
        </asp:Panel>
        <asp:Panel ID="pnlHeaderActions" runat="server" CssClass="FieldTopMenuPadding">
            <div class="CMSEditMenu">
                <cms:HeaderActions runat="server" ID="hdrActions" PerformFullPostBack="false" />
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlContent" runat="server" CssClass="FieldPanelRightContent">
            <asp:Panel ID="pnlContentPadding" runat="server" CssClass="FieldPanelRightContentPadding">
                <div class="FieldRightScrollPanel">
                    <div class="FieldPanelRightWizard">
                        <asp:Panel runat="server" ID="pnlRightWrapper" CssClass="FieldPanelRightWrapper">
                            <cms:MessagesPlaceHolder ID="plcMess" runat="server" WrapperControlID="pnlRightWrapper" />
                            <asp:Panel runat="server" ID="pnlField" class="FieldContentTable">
                                <asp:PlaceHolder ID="plcCategory" runat="server" Visible="false">
                                    <cms:CategoryEdit ID="categoryEdit" runat="server" ShortID="ce" />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcSimple" runat="server" Visible="false">
                                    <cms:CMSUpdatePanel ID="pnlUpdateSimpleMode" runat="server">
                                        <ContentTemplate>
                                            <cms:SimpleMode ID="simpleMode" runat="server" ShortID="sm" />
                                        </ContentTemplate>
                                    </cms:CMSUpdatePanel>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcAdvanced" runat="server" Visible="false">
                                    <asp:PlaceHolder ID="plcDatabase" runat="server">
                                        <div class="FieldAnchor" id="Database">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateDatabase" runat="server">
                                            <ContentTemplate>
                                                <cms:DatabaseConfiguration ID="databaseConfiguration" runat="server" ShortID="dc" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <cms:CMSUpdatePanel ID="pnlUpdateDisplay" runat="server">
                                        <ContentTemplate>
                                            <cms:LocalizedCheckBox ID="chkDisplayInForm" runat="server" AutoPostBack="True" OnCheckedChanged="chkDisplayInForm_CheckedChanged"
                                                CssClass="CheckBoxMovedLeft" ResourceString="templatedesigner.displayinform" />
                                            <asp:Panel runat="server" ID="pnlDisplayInDashBoard" Visible="false">
                                                <cms:LocalizedCheckBox ID="chkDisplayInDashBoard" runat="server" CssClass="CheckBoxMovedLeft"
                                                    ResourceString="templatedesigner.displayindashboard" />
                                            </asp:Panel>
                                        </ContentTemplate>
                                    </cms:CMSUpdatePanel>
                                    <asp:PlaceHolder ID="plcField" runat="server">
                                        <div class="FieldAnchor" id="FieldAppearance">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateAppearance" runat="server">
                                            <ContentTemplate>
                                                <cms:FieldAppearance ID="fieldAppearance" runat="server" ShortID="fa" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcSettings" runat="server">
                                        <div class="FieldAnchor" id="ControlSettings">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateSettings" runat="server">
                                            <ContentTemplate>
                                                <cms:ControlSettings ID="controlSettings" MinItemsToAllowSwitch="3" runat="server"
                                                    ShortID="cs" AllowModeSwitch="true" SimpleMode="true" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcValidation" runat="server">
                                        <div class="FieldAnchor" id="ValidationSettings">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateValidation" runat="server">
                                            <ContentTemplate>
                                                <cms:ValidationSettings ID="validationSettings" runat="server" ShortID="vs" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcCSS" runat="server">
                                        <div class="FieldAnchor" id="CSSStyles">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateCSS" runat="server">
                                            <ContentTemplate>
                                                <cms:CSSsettings ID="cssSettings" runat="server" ShortID="ss" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcFieldAdvancedSettings" runat="server">
                                        <div class="FieldAnchor" id="FieldAdvancedSettings">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlFieldAdvancedSettings" runat="server">
                                            <ContentTemplate>
                                                <cms:FieldAdvancedSettings ID="fieldAdvancedSettings" runat="server" ShortID="fas" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                </asp:PlaceHolder>
                            </asp:Panel>
                        </asp:Panel>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <div class="ClearBoth">
        </div>
    </asp:Panel>
</asp:Panel>
<asp:Literal ID="ltlConfirmText" runat="server" EnableViewState="false" />
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<script type="text/javascript">
    //<![CDATA[
    function confirmDelete() {
        return confirm(document.getElementById('confirmdelete').value);
    }
    //]]>
</script>
