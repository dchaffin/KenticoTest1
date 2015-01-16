<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Iframe_Edit"
    Theme="Default" ValidateRequest="false" CodeFile="Newsletter_Iframe_Edit.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Edit issue</title>
    <style type="text/css">
        body
        {
            margin: 0;
            padding: 0;
        }
    </style>
</head>
<body class="<%=mBodyClass%>">
    <form id="form1" runat="server">
        <div id="CMSHeaderDiv">
            <div id="CKEditorToolbar">
            </div>
        </div>
        <asp:Panel runat="server" ID="pnlBody">
            <asp:Panel runat="server" ID="pnlContent" CssClass="EditIssueContent">
                <asp:PlaceHolder ID="plcContent" runat="server" />
            </asp:Panel>
        </asp:Panel>
        <div id="CMSFooterDiv">
            <div id="CKFooter">
            </div>
        </div>
    </form>
</body>
</html>
