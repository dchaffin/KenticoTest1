<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_WebFarmLog"
    CodeFile="WebFarmLog.ascx.cs" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <asp:GridView runat="server" ID="gridQueries" EnableViewState="false" GridLines="None"
        AutoGenerateColumns="false" ShowFooter="true" >
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
            <asp:TemplateField>
                <ItemTemplate>
                    <strong>
                        <%#Eval("TaskType")%>
                    </strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <strong>
                        <%#HTMLHelper.HTMLEncode(ValidationHelper.GetString(Eval("Target"), ""))%>
                    </strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <strong>
                        <%#GetData(Eval("TextData"), Eval("BinaryData"))%>
                    </strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#TextHelper.EnsureLineEndings(ValidationHelper.GetString(Eval("Context"), ""), "<br />")%>
                </ItemTemplate>
                <FooterTemplate>
                    <strong>
                        <%#cmsVersion%></strong>
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
