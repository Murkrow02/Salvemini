<%@ Page Language="C#" Title="Aggiungi Libro" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AddBooks.aspx.cs" Inherits="SalveminiApi.BookMarket.AddBooks" %>

<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!--Aggiungi libro-->
    <div class="ceneterdDiv" style="margin-bottom: 20px;">
        <p>
            <asp:Label class="titleLbl" runat="server">Aggiungi libro</asp:Label>
        </p>
        <p>
            <asp:Label ID="descLabel" CssClass="descLabel" runat="server">Puoi aggiungere fino a 50 libri!</asp:Label>
        </p>
        <asp:TextBox runat="server" class="textInput" placeholder="Titolo del libro" ID="titleInput" MaxLength="199" autocomplete="off"></asp:TextBox>
<%--        <asp:TextBox runat="server" class="textInput" placeholder="Codice seriale (Facoltativo)" ID="serialInput" MaxLength="199"></asp:TextBox>--%>
        <asp:Button runat="server" Text="Aggiungi" class="goButton" OnClick="addBook" ID="addBtn" />
    </div>
    <!--Divider-->
    <div class="divider"></div>

    <div class="ceneterdDiv">
        <!--Info-->
        <p>
            <asp:Label runat="server" class="titleLbl" ID="nomeLbl" />
        </p>
        <p>
            <asp:Label ID="infoLbl" CssClass="descLabel" runat="server" Style="margin-bottom: 1%" />
        </p>
         <p>
            <asp:Label  Text="Sono riportati solo i libri ancora non approvati" CssClass="descLabel" runat="server" Style="margin-bottom: 1%" />
        </p>
        <!--Lista-->
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ChildrenAsTriggers="true" UpdateMode="Conditional" ID="PannelloDinamico" ClientIDMode="Static" runat="server">
            <ContentTemplate>

                <!--Scrollable listview-->
                <div style="overflow-x: auto">
                    <asp:ListView ID="ListView1" runat="server" OnItemCommand="Commands"
                        GroupPlaceholderID="groupPlaceHolder1"
                        ItemPlaceholderID="itemPlaceHolder1">
                        <LayoutTemplate>
                            <table cellpadding="2" cellspacing="0" border="1" class="lista">
                                <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>
                            </table>
                        </LayoutTemplate>
                        <GroupTemplate>
                            <tr>
                                <asp:PlaceHolder runat="server" ID="itemPlaceHolder1"></asp:PlaceHolder>
                            </tr>
                        </GroupTemplate>
                        <ItemTemplate>
                            <td style="width: auto">
                                <asp:Label ID="lblName" runat="server" Text='<%# Eval("Nome") %>' CssClass="listNome" style="word-break: break-all;"></asp:Label>
                            </td>
                            <td style="width: 15%">
                                <asp:Button ID="btnRemove" runat="server" Text='Elimina' CommandName="remove" CssClass="goButton" Style="margin: 3px; width: 100%;" Visible='<%# bool.Parse(Eval("Accettato").ToString()) == true ? false : true%>'/>
                             <asp:Label runat="server" Text='Approvato' CssClass="listNome" style="word-break: break-all;" Visible='<%# bool.Parse(Eval("Accettato").ToString()) == true ? true : false%>'></asp:Label>
                            </td>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="addBtn" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <!--Logout-->
        <asp:Button runat="server" Text="Logout" OnClick="logOut" class="goButton" Style="margin-top: 15px" />
    </div>
</asp:Content>

