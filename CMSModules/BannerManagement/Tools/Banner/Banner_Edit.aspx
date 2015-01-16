<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_BannerManagement_Tools_Banner_Banner_Edit" CodeFile="Banner_Edit.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/TabsFrameset.ascx" TagName="TabsFrameset" TagPrefix="cms" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" enableviewstate="false">
    <title>Banner management settings</title>
</head>
<cms:TabsFrameset ID="frm" runat="server" FrameHeight="64" HeaderUrl="Banner_Edit_Header.aspx" ContentUrl="Banner_Edit_General.aspx" />
</html>
