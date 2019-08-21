<%@ Page Title="Libri utente" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="LibriUtente.aspx.cs" Inherits="SalveminiApi.BookMarket.LibriUtente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="ceneterdDiv">
         <!--Back-->
        <asp:Button runat="server" Text="Indietro" OnClick="back_Click" class="goButton" Style="margin-top: 69px" />
        <!--Title-->
        <asp:Label Text="Utenti" runat="server" CssClass="titleLbl" ID="nomeTitolo" />
        <!--Totale-->
        <p>
                         <asp:Label runat="server" CssClass="descLabel" ID="totale" Visible="false"/>

        </p>
        <!--Scrollable listview-->
        <div style="overflow-x: auto">
            <asp:ListView ID="ListView1" runat="server"
                GroupPlaceholderID="groupPlaceHolder1" OnItemCommand="Commands"
                ItemPlaceholderID="itemPlaceHolder1">
                <LayoutTemplate>
                    <table cellpadding="2" cellspacing="0" border="1" class="lista">
                        <tr>
                            <th>Nome</th>
<%--                            <th>Seriale</th>--%>
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
                   <%-- <td>
                        <asp:Label runat="server" Text='<%# Eval("Seriale")%>' CssClass="listNome"></asp:Label>
                    </td>--%>
                    <td style="width: 10%">
                        <asp:Label runat="server" Text='<%# String.Format("{0:#.00}", Eval("Prezzo"))%>' CssClass="listNome"></asp:Label>
                    </td>
                    <td style="width: 10%">
                        <asp:Label runat="server" Text='<%# Eval("Id")%>' CssClass="listNome"></asp:Label>
                    </td>
                     </td>
                            <td style="width: 15%">
                                <asp:Button ID="btnRemove" runat="server" Text='Approva' CommandName="approve" CssClass="goButton" Style="margin: 3px; width: 100%;" Visible='<%# bool.Parse(Eval("Accettato").ToString()) == true ? false : true%>'/>
                                                       <asp:Label runat="server" Text='Approvato' CssClass="listNome" style="word-break: break-all;" Visible='<%# bool.Parse(Eval("Accettato").ToString()) == true ? true : false%>'></asp:Label>

                                </td>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </div>
</asp:Content>
