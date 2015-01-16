<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_AllLog"
    EnableViewState="false" CodeFile="AllLog.ascx.cs" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <asp:GridView runat="server" ID="gridDebug" EnableViewState="false" GridLines="None"
        AutoGenerateColumns="false" ShowFooter="true">
        <Columns>
            <asp:TemplateField>
                <ItemStyle CssClass="Index" />
                <ItemTemplate>
                    <strong>
                        <%#Eval("Index")%>
                    </strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("DebugType")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Information")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Result")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Context")%>
                </ItemTemplate>
                <FooterTemplate>
                    <strong>
                        <%#CMSVersion%></strong>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("TotalDuration")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle CssClass="Dur" />
                <ItemStyle CssClass="Dur" />
                <FooterStyle CssClass="Dur" />
                <ItemTemplate>
                    <%#Eval("Duration")%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
