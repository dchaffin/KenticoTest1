<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_FilesLog"
    EnableViewState="false" CodeFile="FilesLog.ascx.cs" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <asp:GridView runat="server" ID="gridStates" EnableViewState="false" GridLines="None"
        AutoGenerateColumns="false" ShowFooter="true">
        <AlternatingRowStyle CssClass="AltRow" />
        <FooterStyle CssClass="Footer" />
        <Columns>
            <asp:TemplateField>
                <ItemStyle CssClass="Index" />
                <ItemTemplate>
                    <strong>
                        <%#GetIndex()%><%#GetWarning(Eval("FileNotClosed"), Eval("FileOperation"))%>
                    </strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetFileOperation(Eval("FileOperation"), Eval("FileParameters"))%><br />
                    <%#GetSizeAndAccesses(MaxSize, Eval("FileSize"), Eval("FileAccesses"), false)%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetPath(Eval("FilePath"))%>
                    <%#GetText(Eval("FileText"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("OperationType")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetContext(Eval("Context"))%>
                </ItemTemplate>
                <FooterTemplate>
                    <strong>
                        <%#CMSVersion%></strong>
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
