<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_ValuesTable"
    CodeFile="ValuesTable.ascx.cs" %>
<div>
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <asp:GridView runat="server" ID="gridValues" EnableViewState="false" GridLines="None"
        AutoGenerateColumns="False">
        <AlternatingRowStyle CssClass="AltRow" />
        <FooterStyle CssClass="Footer" />
        <Columns>
            <asp:TemplateField>
                <ItemStyle CssClass="Index" />
                <ItemTemplate>
                    <strong>
                        <%#GetIndex()%>
                    </strong>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
