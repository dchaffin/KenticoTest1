<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_FormControls_PageTemplates_PageTemplateLevels"
    CodeFile="PageTemplateLevels.ascx.cs" %>
<%@ Register Src="~/CMSModules/PortalEngine/FormControls/PageTemplates/LevelTree.ascx"
    TagPrefix="cms" TagName="LevelTree" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
    <ContentTemplate>
        <div style="padding: 5px;">
            <asp:RadioButton runat="server" ID="radInheritAll" GroupName="Inheritance" AutoPostBack="true" />
            <br />
            <asp:RadioButton runat="server" ID="radNonInherit" GroupName="Inheritance" AutoPostBack="true" />
            <br />
            <asp:RadioButton runat="server" ID="radInheritMaster" GroupName="Inheritance" AutoPostBack="true" />
            <br />
            <asp:RadioButton runat="server" ID="radSelect" GroupName="Inheritance" AutoPostBack="true" />
            <br />
            <asp:PlaceHolder runat="server" ID="plcTree">
                <cms:LevelTree runat="server" ID="treeElem" />
            </asp:PlaceHolder>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
