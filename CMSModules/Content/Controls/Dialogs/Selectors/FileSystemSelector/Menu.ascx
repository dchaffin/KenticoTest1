<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_Menu"
    CodeFile="Menu.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/MenuButton.ascx"
    TagName="MenuButton" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/ViewModeMenu.ascx"
    TagName="ViewModeMenu" TagPrefix="cms" %>
<asp:Panel ID="pnlActions" runat="server" CssClass="DialogMenu">
    <cms:CMSUpdatePanel ID="pnlUpdateActionsMenu" runat="server" UpdateMode="Conditional"
        ChildrenAsTriggers="false">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcLeft" runat="server">
                <asp:Panel runat="server" ID="pnlLeft" CssClass="DialogMenuLeft">
                    <div class="LeftAlign">
                        <cms:MenuButton ID="menuBtnNewFolder" runat="server" CssClass="MenuItemLeft" CssHoverClass="MenuItemLeftOver"
                            CssDisabledClass="DialogsFolderDisabled" />
                    </div>
                    <div class="LeftAlign DeleteFolder">
                        <cms:MenuButton ID="menuBtnDeleteFolder" runat="server" CssClass="MenuItemLeft" CssHoverClass="MenuItemLeftOver"
                            CssDisabledClass="DialogsDeleteFolderDisabled" />
                    </div>
                </asp:Panel>
            </asp:PlaceHolder>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:Panel runat="server" ID="pnlRight" CssClass="DialogMenuLeft">
        <asp:PlaceHolder ID="plcDirectFileUploader" runat="server">
            <div class="LeftAlign DialogFileUploader">
                <div id="dialogsUploaderDiv">
                    <cms:DirectFileUploader ID="fileUploader" runat="server" InsertMode="true" IncludeNewItemInfo="true"
                        CheckPermissions="false" ImageWidth="110" ImageHeight="36" InnerLoadingDivHtml="&nbsp;" />
                </div>
                <div id="dialogsUploaderDisabledDiv" class="DialogsUploaderDisabled" style="display: none;">
                    <img id="imgUploaderDisabled" runat="server" /><div class="DialogsFileDisabledTitle">
                        New file</div>
                </div>
            </div>
            <asp:PlaceHolder ID="plcNewFile" runat="server">
                <div class="LeftAlign NewFile">
                    <cms:MenuButton ID="btnNew" runat="server" CssClass="MenuItemLeft" CssHoverClass="MenuItemLeftOver"
                        CssDisabledClass="DialogsNewFileDisabled" />
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
    </asp:Panel>
    <cms:ViewModeMenu ID="menuView" runat="server" />
    <asp:Panel runat="server" ID="pnlHelp" CssClass="DialogMenuHelp" EnableViewState="false">
        <cms:Help ID="helpElem" runat="server" TopicName="dialogs_content" />
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false"></asp:Literal>
</asp:Panel>
