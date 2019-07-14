<%@ Page Title="Cerca utenti" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="SearchUser.aspx.cs" Inherits="SalveminiApi.BookMarket.SearchUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <script>
        function RefreshUpdatePanel() {
            __doPostBack('<%= searchBar.ClientID %>', '');
        };
    </script>

    <div class="ceneterdDiv">
        <!--Title-->
        <asp:Label Text="Utenti" runat="server" CssClass="titleLbl" />
        <!--Search-->
        <asp:TextBox autocomplete="off" runat="server" ID="searchBar" CssClass="textInput" placeholder="Cerca fra gli utenti" OnTextChanged="searching" onkeyup="RefreshUpdatePanel();"></asp:TextBox>
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
                            <th>Telefono</th>
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
                                <asp:Label ID="lblName" runat="server" Text='<%# Eval("Nome") + " " + Eval("Cognome")%>' CssClass="listNome"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("Telefono")%>' CssClass="listNome"></asp:Label>
                            </td>
                             <td style="width: 15%">
                                <asp:Button ID="btnRemove" runat="server" Text='Vedi' CommandName="view" CssClass="goButton" Style="margin: 3px; width: 100%;" />
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
