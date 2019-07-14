<%@ Page Title="Libri utente" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="LibriUtente.aspx.cs" Inherits="SalveminiApi.BookMarket.LibriUtente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="ceneterdDiv">
        <!--Title-->
        <asp:Label Text="Utenti" runat="server" CssClass="titleLbl" ID="nomeTitolo" />

        <!--Scrollable listview-->
        <div style="overflow-x: auto">
            <asp:ListView ID="ListView1" runat="server"
                GroupPlaceholderID="groupPlaceHolder1"
                ItemPlaceholderID="itemPlaceHolder1">
                <LayoutTemplate>
                    <table cellpadding="2" cellspacing="0" border="1" class="lista">
                        <tr>
                            <th>Nome</th>
                            <th>Seriale</th>
                            <th>Prezzo</th>
                            <th>Codice</th>
                        </tr>
                        <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>

                    </table>
                </LayoutTemplate>
                <GroupTemplate>
                    <tr>

                        <asp:PlaceHolder runat="server" ID="itemPlaceHolder1"></asp:PlaceHolder>
                    </tr>
                </GroupTemplate>
                <ItemTemplate>
                    <td>
                        <asp:Label ID="lblName" runat="server" Text='<%# Eval("Nome")%>' CssClass="listNome" style="word-break: break-all;"></asp:Label>
                    </td>
                    <td>
                        <asp:Label runat="server" Text='<%# Eval("Seriale")%>' CssClass="listNome"></asp:Label>
                    </td>
                    <td style="width: 10%">
                        <asp:Label runat="server" Text='<%# String.Format("{0:#.00}", Eval("Prezzo"))%>' CssClass="listNome"></asp:Label>
                    </td>
                    <td style="width: 10%">
                        <asp:Label runat="server" Text='<%# Eval("Id")%>' CssClass="listNome"></asp:Label>
                    </td>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </div>
</asp:Content>
