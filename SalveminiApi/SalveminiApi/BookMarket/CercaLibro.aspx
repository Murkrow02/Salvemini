<%@ Page Title="Cerca Libro" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CercaLibro.aspx.cs" Inherits="SalveminiApi.BookMarket.CercaLibro" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script>
        function RefreshUpdatePanel() {
            __doPostBack('<%= searchBar.ClientID %>', '');
        };
    </script>

    <div class="ceneterdDiv">
         <!--Back-->
        <asp:Button runat="server" Text="Indietro" OnClick="back_Click" class="goButton" Style="margin-top: 69px" />
        <!--Title-->
        <asp:Label Text="Libri approvati" runat="server" CssClass="titleLbl" />
        <!--Search-->
        <asp:TextBox autocomplete="off" runat="server" ID="searchBar" CssClass="textInput" placeholder="Inserisci il codice o il nome del libro" OnTextChanged="searching" onkeyup="RefreshUpdatePanel();"></asp:TextBox>
        <!--List-->
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
                                <tr>
                                    <th>Nome</th>
<%--                                    <th>Seriale</th>--%>
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
                    <%--<td>
                        <asp:Label runat="server" Text='<%# Eval("Seriale")%>' CssClass="listNome"></asp:Label>
                    </td>--%>
                    <td style="width: 10%">
                        <asp:Label runat="server" Text='<%# String.Format("{0:#.00}", Eval("Prezzo"))%>'  CssClass="listNome"></asp:Label>
                    </td>
                    <td style="width: 10%">
                        <asp:Label runat="server" Text='<%# Eval("Id")%>' CssClass="listNome"></asp:Label>
                    </td>
                            <td style="width: 15%">
                                <asp:Button ID="btnRemove" runat="server" Text='Modifica' CommandName="view" CssClass="goButton" Style="margin: 3px; width: 100%;" />
                            </td>
                </ItemTemplate>


                    </asp:ListView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="searchBar" EventName="TextChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

</asp:Content>
