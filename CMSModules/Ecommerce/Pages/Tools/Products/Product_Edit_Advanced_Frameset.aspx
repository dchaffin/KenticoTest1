<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Product_Edit_Advanced_Frameset.aspx.cs" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Advanced_Frameset" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>E-commerce - Product advanced properties</title>
</head>
<frameset border="0" cols="126, *" frameborder="0" runat="server" id="colsFrameset">
    <frame name="advancedHeader" scrolling="no" noresize="noresize"
        frameborder="0" runat="server" id="advancedHeader" />
    <frame name="advancedEdit" src="about:blank" noresize="noresize"
        frameborder="0" runat="server" id="advancedEdit" scrolling="auto" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
