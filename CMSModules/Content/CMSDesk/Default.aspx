<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Default"
    CodeFile="Default.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Theme="Default" Title="CMS Desk - Content" %>

<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<asp:Content runat="server" ID="cntContent" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
    //<![CDATA[
        window.ddNotScroll = true;
    //]]>
    </script>
    <cms:ContentUrlRetriever runat="server" ID="urlRetriever" />
    <input type="hidden" id="selectedNodeId" name="selectedNodeId" value="<%=ResultNodeID%>" />
    <input type="hidden" id="selectedCulture" name="selectedCulture" value="<%=CMSContext.PreferredCultureCode%>" />
    <input type="hidden" id="selectedSplitModeCulture" name="selectedSplitModeCulture"
        value="<%=CMSContext.SplitModeCultureCode %>" />
    <input type="hidden" id="selectedDevice" name="selectedDevice" value="<%= ResultDevice%>" />
    <input type="hidden" id="selectedMode" name="selectedMode" value="<%=ResultMode %>" />
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="paneMenu" runat="server" Direction="North" RenderAs="Div" ControlPath="~/CMSModules/Content/Controls/Menu.ascx"
                SpacingOpen="0" OnOpenEndScript="function ()  { ParametrizedMaximizeAll(window, true); }"
                OnCloseEndScript="function ()  { ParametrizedMinimizeAll(window, true); }" />
            <cms:UILayoutPane ID="contenttree" runat="server" Direction="West" RenderAs="div"
                ControlPath="~/CMSModules/Content/Controls/CMSDeskTree.ascx" SpacingOpen="7"
                SpacingClosed="7" PaneClass="TreeBody" Size="269" TogglerLengthOpen="33" TogglerLengthClosed="33" />
            <cms:UILayoutPane ID="contentview" runat="server" Direction="Center" RenderAs="Iframe"
                MaskContents="true" />
            <cms:UILayoutPane ID="contentfooter" runat="server" Direction="South" RenderAs="Div"
                PaneClass="ContentFooter" SpacingOpen="0" Size="7" />
        </Panes>
    </cms:UILayout>
</asp:Content>
