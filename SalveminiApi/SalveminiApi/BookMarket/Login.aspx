<%@ Page Language="C#" Title="Login BookMarket" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SalveminiApi.BookMarket.Login" %>

<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">  
     <div class="ceneterdDiv" style="margin-bottom: 20px;">
            <!--Registrati-->
          <p>
            <asp:Label class="titleLbl" runat="server">Registrati</asp:Label>
               </p>
                    <p>
            <asp:Label ID="descLabelR" CssClass="descLabel" runat="server">Effettua l'accesso prima di registrare i tuoi libri</asp:Label>
                         </p>
            <asp:TextBox runat="server" class="textInput" placeholder="Nome" ID="NomeInput" MaxLength="49"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Cognome" ID="CognomeInput" MaxLength="49"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Cellulare" ID="CellInputR" MaxLength="49"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Password" ID="PasswordInputR" type="password" MaxLength="29"></asp:TextBox>
            <asp:Button runat="server" Text="Registrati" OnClick="RegisterClick" class="goButton"/>
        </div>

        <div class="divider"></div>


        <div class="ceneterdDiv">
            <!--Accedi-->
             <p>
            <asp:Label runat="server" class="titleLbl">Accedi</asp:Label>
                  </p>
             <p>
            <asp:Label ID="descLabelA" CssClass="descLabel" runat="server">Accedi se hai già un account</asp:Label>
             </p>
            <asp:TextBox runat="server" class="textInput" placeholder="Cellulare" ID="CellInputA"></asp:TextBox>
            <asp:TextBox runat="server" class="textInput" placeholder="Password" ID="PasswordInputA" type="password"></asp:TextBox>
            <asp:Button runat="server" Text="Accedi" OnClick="AccediClick" class="goButton" />
        </div>
</asp:Content>  


