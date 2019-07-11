<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SalveminiApi.BookMarket.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>BookMarket</title>

    <link href="~/Content/DesktopStyle.css" type="text/css" rel="stylesheet" runat="server" id="stile" />

    <style>
        
        
    </style>
</head>

<body>
    <form id="form1" runat="server" class="container" style="padding-top: 2%">
        <div class="ceneterdDiv" style="margin-bottom: 20px;">
            <!--Registrati-->
            <asp:Label class="titleLbl" runat="server">Registrati</asp:Label>
            <asp:Label ID="descLabelR" CssClass="descLabel" runat="server">Effettua l'accesso prima di registrare i tuoi libri</asp:Label>
            <asp:TextBox runat="server" class="textInput" placeholder="Nome" ID="NomeInput" MaxLength="49"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Cognome" ID="CognomeInput" MaxLength="49"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Cellulare" ID="CellInputR" MaxLength="49"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Password" ID="PasswordInputR" type="password" MaxLength="29"></asp:TextBox>
            <asp:Button runat="server" Text="Registrati" OnClick="RegisterClick" class="goButton"/>
        </div>

        <div class="divider"></div>


        <div class="ceneterdDiv">
            <!--Accedi-->
            <asp:Label runat="server" class="titleLbl">Accedi</asp:Label>
            <asp:Label ID="descLabelA" CssClass="descLabel" runat="server">Accedi se hai già un account</asp:Label>
            <asp:TextBox runat="server" class="textInput" placeholder="Cellulare" ID="CellInputA"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Password" ID="PasswordInputA" type="password"></asp:TextBox>
            <asp:Button runat="server" Text="Accedi" OnClick="AccediClick" class="goButton" />
        </div>
    </form>
</body>
</html>
