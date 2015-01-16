<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Product_Section.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Section" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" enableviewstate="false">
    <title>Product section - Edit</title>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</head>
<frameset border="0" rows="0,*" runat="server" id="documentframeset">
    <frame name="editheader" id="documentheader" runat="server" src="about:blank" scrolling="no"
        noresize="noresize" frameborder="0" />
    <frame name="editview" src="<%=viewpage%>" scrolling="auto" noresize="noresize"
        frameborder="0" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
