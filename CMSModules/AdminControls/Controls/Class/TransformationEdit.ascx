<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_TransformationEdit"
    CodeFile="TransformationEdit.ascx.cs" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Locking/ObjectEditPanel.ascx" TagName="ObjectEditPanel"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Layouts/TransformationCode.ascx" TagName="TransformationCode"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.UIControls" %>
<asp:Panel ID="pnlContent" runat="server" CssClass="SimpleHeader">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <cms:ObjectEditPanel runat="server" ID="editMenuElem" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:Panel ID="pnlScreen" runat="server" CssClass="PreviewDefaultContent">
        <div class="PageContent">
            <cms:UIForm runat="server" ID="editElem" ObjectType="cms.transformation" DefaultFieldLayout="Inline"
                RedirectUrlAfterCreate="" CssClass="Transformation">
                <LayoutTemplate>
                    <cms:FormCategory runat="server" ID="pnlGeneral" DefaultFieldLayout="TwoColumns" ResourceString="general.general">
                        <cms:FormField runat="server" ID="fDisplayName" Field="TransformationName" FormControl="TextBoxControl"
                            ResourceString="transformationlist.transformationname" DisplayColon="true" />
                    </cms:FormCategory>
                    <br />
                    <cms:FormCategory runat="server" ID="pnlCategory" ResourceString="objecttype.cms_transformation"
                        CssClass="TransformationCode" DefaultFieldLayout="Inline">
                        <cms:FormField runat="server" ID="fCode" Field="TransformationCode">
                            <cms:TransformationCode runat="server" ID="ucTransfCode" />
                        </cms:FormField>
                    </cms:FormCategory>
                </LayoutTemplate>
            </cms:UIForm>
        </div>
    </asp:Panel>
</asp:Panel>
<asp:HiddenField runat="server" ID="hdnClose" EnableViewState="false" />
