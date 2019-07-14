<%@ Page Title="Admin login" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AdminLogin.aspx.cs" Inherits="SalveminiApi.BookMarket.AdminLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="ceneterdDiv" style="margin-top: 12%">
        <p>
            <asp:Label class="titleLbl" runat="server">Amministrazione</asp:Label>
        </p>
        <asp:TextBox runat="server" placeholder="Password" CssClass="textInput" ID="passwordTxt" type="password"></asp:TextBox>
        <asp:Button Text="Vai" runat="server" CssClass="goButton" OnClick="loginClick"/>
    </div>

</asp:Content>
