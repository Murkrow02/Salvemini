<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddBooks.aspx.cs" Inherits="SalveminiApi.BookMarket.AddBooks" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Aggiungi libro</title>

    <link href="~/Content/DesktopStyle.css" type="text/css" rel="stylesheet" runat="server" id="stile" />

    <style>
        
    </style>
</head>
<body>
    <form id="form1" runat="server" class="container">
        <!--Aggiungi libro-->
        <div class="ceneterdDiv" style="margin-bottom: 20px;">
            <asp:Label class="titleLbl" runat="server">Aggiungi libro</asp:Label>
            <asp:Label ID="descLabel" CssClass="descLabel" runat="server">Puoi aggiungere fino a 50 libri!</asp:Label>
            <asp:TextBox runat="server" class="textInput" placeholder="Titolo del libro" ID="titleInput" MaxLength="199"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Codice seriale (Facoltativo)" ID="serialInput" MaxLength="199"></asp:TextBox>
            <asp:Button runat="server" Text="Aggiungi" class="goButton" OnClick="addBook" ID="addBtn"/>
        </div>
        <!--Divider-->
        <div class="divider"></div>

        <!--Info-->
        <asp:Label runat="server" class="titleLbl" ID="nomeLbl" />
        <asp:Label ID="infoLbl" CssClass="descLabel" runat="server" style="margin-bottom:1%"/>

        <!--Lista-->
        <div class="ceneterdDiv">
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
                                    <asp:Label ID="lblName" runat="server" Text='<%# Eval("Nome") %>' CssClass="listNome"></asp:Label>
                                </td>
                                <td style="width: 15%">
                                    <asp:Button ID="btnRemove" runat="server" Text='Elimina' CommandName="remove" CssClass="goButton" Style="margin: 3px; width: 100%;" />
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
    </form>
</body>
</html>
