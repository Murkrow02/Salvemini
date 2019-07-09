<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SalveminiApi.BookMarket.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <link href="../Content/bttn.min.css" rel="stylesheet" type="text/css"/>

    <style>
    #loginDiv {
        border: 2px solid #802891;
        background-color: white;
        border-radius: 20px;
        overflow-y: auto;
        overflow-x: hidden;
        display: inline-block;
        -moz-box-shadow: 0px 0px 77px -2px rgba(0,0,0,0.51);
        box-shadow: 0px 0px 77px -2px rgba(0,0,0,0.51);
    }

    .titleLbl {
        width: 100%;
        text-align: center;
        font-family: Montserrat;
        color: #802891;
        font-size: 50px;
        margin: 0;
        float: right;
    }

    .textInput {
        display: block;
        margin-left: auto;
        margin-right: auto;
        width: 90%;
        margin-bottom: 1%;
    }

        .textInput:focus {
            outline-color: #802891;
        }

    #descLabel {
        color: gray;
        width: 100%;
        text-align: center;
        margin: 0;
        font-family: Montserrat;
        margin-bottom: 1%;
    }

    .goButton{
        background-color:#802891;
        color:white;
        font-family:Montserrat;
        border:none;
        outline:none;
    }

    
</style>
</head>

<body style="text-align: center;">
    <form id="form1" runat="server">
        <div id="loginDiv">
              <!--Registrati-->
            <asp:Label class="titleLbl" runat="server">Registrati</asp:Label>
            <asp:Label ID="descLabel" runat="server">Effettua l'accesso prima di registrare i tuoi libri</asp:Label>
            <asp:TextBox runat="server" class="textInput" placeholder="Nome" ID="NomeInput"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Cognome" ID="CognomeInput"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Cellulare" ID="CellInputR"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Password" ID="PasswordInputR" type="password"></asp:TextBox>
            <asp:Button runat="server" Text="Registrati" OnClick="RegisterClick" class="goButton"/>
            <!--Accedi-->
            <asp:Label runat="server" class="titleLbl">Accedi</asp:Label>
            <asp:TextBox runat="server" class="textInput" placeholder="Cellulare" ID="CellInputA"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Password" ID="PasswordInputA" type="password"></asp:TextBox>
            <asp:Button runat="server" Text="Accedi" OnClick="AccediClick" class="goButton"/>

        </div>
    </form>
</body>
</html>
