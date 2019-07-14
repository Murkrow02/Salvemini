<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="EditBook.aspx.cs" Inherits="SalveminiApi.BookMarket.EditBook" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
    <div class="ceneterdDiv" style="margin-top: 12%">
        <p>
            <asp:Label class="titleLbl" runat="server">Modifica Libro</asp:Label>
        </p>
        <p>
            <asp:Label ID="errorLabel" CssClass="descLabel" runat="server" Visible="false" ForeColor="Red">Il libro non è stato aggiornato, assicurati di aver inserito un valore valido</asp:Label>
                         </p>
        <asp:TextBox runat="server" placeholder="Prezzo" CssClass="textInput" ID="prezzoTxt" ></asp:TextBox>
        <asp:Button Text="Vai" runat="server" CssClass="goButton" OnClick="editClick"/>
    </div>

</asp:Content>
