<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_SecurityLog"
    CodeFile="SecurityLog.ascx.cs" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <asp:GridView runat="server" ID="gridSec" EnableViewState="false" GridLines="None"
        AutoGenerateColumns="false" ShowFooter="true">
        <AlternatingRowStyle CssClass="AltRow" />
        <FooterStyle CssClass="Footer" />
        <Columns>
            <asp:TemplateField>
                <ItemStyle CssClass="Index" />
                <ItemTemplate>
                    <strong>
                        <%#GetIndex(Eval("Indent"))%>
                        <%#GetDuplicity(Eval("Duplicit"), Eval("Indent"))%>
                    </strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetUserName(Eval("UserName"), Eval("Indent"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetBeginIndent(Eval("Indent"))%><strong><%#Eval("SecurityOperation")%></strong><%#GetEndIndent(Eval("Indent"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle CssClass="Cnt" />
                <HeaderStyle CssClass="Cnt" />
                <ItemTemplate>
                    <%#GetResult(Eval("Result"), Eval("Indent"), Eval("Important"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle CssClass="Cnt" />
                <HeaderStyle CssClass="Cnt" />
                <ItemTemplate>
                    <%#Eval("Resource")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Name")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle CssClass="Cnt" />
                <HeaderStyle CssClass="Cnt" />
                <ItemTemplate>
                    <%#Eval("SiteName")%>
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
