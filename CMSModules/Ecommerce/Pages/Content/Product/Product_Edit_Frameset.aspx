<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_Frameset"
    CodeFile="Product_Edit_Frameset.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Product - edit - Frameset</title>
</head>
<frameset border="0" cols="126, *" frameborder="0" runat="server" id="colsFrameset"
    enableviewstate="false">
    <frame name="ProductHeader" src="Product_Edit_Header.aspx"
        frameborder="0" scrolling="no" noresize="noresize" id="frmHeader" runat="server" />
    <frame name="ProductContent" frameborder="0" id="frmContent" runat="server" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
