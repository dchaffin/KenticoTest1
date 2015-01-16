<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AssemblyClassSelector.ascx.cs" Inherits="CMSFormControls_Classes_AssemblyClassSelector" %>
<%@ Register Src="~/CMSFormControls/Basic/DropDownListControl.ascx" TagName="DropDownListControl"
    TagPrefix="cms" %>
<script type="text/javascript">
    // Javacript after async postback
    $j(window).load(function () {
        var timer = null;
        SetupUpdate();
    });
</script>
<div class="EditingFormControl" style="padding-left:5px;padding-right:1px">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <div>
                <cms:LocalizedLabel ID="lblAssembly" runat="server" CssClass="AssemblyClassSelector"
                    ResourceString="general.assembly" />
            </div>
            <div>
                <asp:Panel runat="server" ID="pnlAssemblyName" CssClass="AssemblyComboBox">
                    <cms:DropDownListControl ID="drpAssemblyName" runat="server" EditText="true" CssClass="ShortTextBox" />
                </asp:Panel>
            </div>
            <div>
                <cms:LocalizedLabel ID="lblClassName" runat="server" CssClass="AssemblyClassSelector"
                    ResourceString="general.class" />
            </div>
            <div>
                <asp:Panel runat="server" ID="pnlClassName" CssClass="AssemblyComboBox">
                    <cms:DropDownListControl ID="drpClassName" runat="server" EditText="true" />
                </asp:Panel>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</div>
