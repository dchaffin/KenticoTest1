<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PreviewHierarchy.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Preview_PreviewHierarchy" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register TagPrefix="cms" Src="PreviewNavigationButtons.ascx" TagName="nav" %>
<script type="text/javascript">
    function CM_FullScreen(isFullScreen, elemsrc) {
        PreviewEditorFullScreen(isFullScreen, elemsrc, '<%=paneContent.ClientID %>');
    }
</script>
<cms:UILayout runat="server" ID="layoutElem" OnResizeEndScript="LayoutResized">
    <Panes>
        <cms:UILayoutPane ID="paneContent" runat="server" Direction="North" RenderAs="Div"
            UseUpdatePanel="false" Size="50%" PaneClass="PreviewContentMain" Visible="true" />
        <cms:UILayoutPane ID="paneContentMain" runat="server" Direction="Center" RenderAs="Div"
            MaskContents="true" UseUpdatePanel="false" ResizerClass="PageHeaderLine">
            <Template>
                <cms:UILayout runat="server" ID="layoutElem">
                    <Panes>
                        <cms:UILayoutPane ID="panePreview" runat="server" Direction="Center" RenderAs="Iframe"
                            UseUpdatePanel="false" PaneClass="ui-layout-pane-scroll" Visible="true" Resizable="false" />
                    </Panes>
                </cms:UILayout>
            </Template>
        </cms:UILayoutPane>
        <cms:UILayoutPane ID="paneFooter" runat="server" Direction="South" RenderAs="Div"
            Size="43" Closable="false" FrameBorder="false" ControlPath="~/CMSModules/AdminControls/Controls/Preview/PreviewFooter.ascx"
            Resizable="false" />
    </Panes>
</cms:UILayout>
<asp:Button ID="btnHidden" CssClass="Hidden" runat="server" />
<%--For register watertext extender --%>
<cms:nav runat="server" ID="ucExtenderNav" Visible="false" />
<div id="pnlProgress" runat="server" class="WPTLoader" style="position: absolute">
    <div class="WPTLoaderBackground" runat="server" id="pnlBackGround">
    </div>
    <div class="WPTLoaderBox">
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <asp:Image runat="server" ID="imgProgress" />
                </td>
                <td>
                    <cms:LocalizedLabel ID="lblProgress" runat="server" ResourceString="toolstree.loading" />
                </td>
            </tr>
        </table>
    </div>
</div>
