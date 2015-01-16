<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_CMSDesk_Properties_Properties_Frameset"
    Theme="Default"  CodeFile="Properties_Frameset.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" enableviewstate="false">
    <title>Development - Document types - Edit transformation</title>
    <script type="text/javascript">
        window.onload = function ()
        {
            if (location.href.search('mode=editlive') != -1) {
                var frameset = document.getElementById('rowsFrameset');
                frameset.rows = "44, *, 43";
            }
        } 
    </script>
</head>
<frameset border="0" rows="44, *, 0" id="rowsFrameset">
    <frame name="header" src="Properties_Header.aspx?<%=QueryHelper.EncodedQueryString%>"
        scrolling="no" frameborder="0" noresize="noresize" />
    <frame name="content" src="Default.aspx?<%=QueryHelper.EncodedQueryString%>"
        frameborder="0" />
    <frame name="footer" src="Properties_Footer.aspx?<%=QueryHelper.EncodedQueryString%>"
        frameborder="0" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
