<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Workflow_Step_Edit"
    EnableViewState="false" CodeFile="Workflow_Step_Edit.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Workflow Edit - Workflow Steps</title>
</head>
<frameset border="0" ID="rowsFrameset" runat="server">
    <frame name="wfStepMenu" ID="frameHeader" runat="server" frameborder="0" noresize="noresize" scrolling="no" />
    <frame name="wfStepContent" ID="frameContent" runat="server" frameborder="0" noresize="noresize" />
    <frame name="wfDialogFooter" ID="frameFooter" runat="server" frameborder="0" noresize="noresize" scrolling="no" Visible="false" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
