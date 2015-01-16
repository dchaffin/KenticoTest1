<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_FormControls_Avatars_UserPictureEdit"
    CodeFile="UserPictureEdit.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UserPicture.ascx" TagName="UserPicture" TagPrefix="cms" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        <div>
            <asp:RadioButtonList ID="rdbMode" runat="server" AutoPostBack="True" RepeatDirection="Horizontal"
                Visible="False">
                <asp:ListItem Value="avatar">Avatar</asp:ListItem>
                <asp:ListItem Value="gravatar">Gravatar</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <asp:Panel runat="server" ID="pnlPreview" Style="display: none">
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlAvatarImage" Visible="false">
                        <cms:UserPicture ID="picUser" runat="server" Visible="true" />
                        &nbsp;<asp:ImageButton runat="server" ID="btnDeleteImage" EnableViewState="false" Visible="false" />
                        <asp:Image ID="imgHelp" runat="server" Visible="false" />
                        <br />
                    </asp:Panel>
                    <asp:PlaceHolder runat="server" ID="plcUploader" Visible="false">
                        <div class="Uploader">
                            <asp:Label runat="server" ID="lblUploader" CssClass="UploaderLabel" EnableViewState="false" />
                            <cms:CMSFileUpload runat="server" CssClass="UploaderInputFile" ID="uplFilePicture" />
                        </div>
                    </asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <td style="padding-left: 3px; padding-top: 3px;">
                    <asp:LinkButton runat="server" ID="btnShowGallery" EnableViewState="false" />
                    <asp:HiddenField runat="Server" ID="hiddenAvatarGuid" />
                    <asp:HiddenField runat="Server" ID="hiddenDeleteAvatar" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</cms:CMSUpdatePanel>
