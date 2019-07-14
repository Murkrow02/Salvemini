<%@ Page Title="Pannello di Controllo" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AdminCp.aspx.cs" Inherits="SalveminiApi.BookMarket.AdminCp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .bigButton {
            height: 10vh;
            width: 45%;
            background-color: #802891;
            text-align: center;
            color: white;
            border:none;
            outline:none;
        }
    </style>

    <div class="ceneterdDiv" style="margin-top: 12%">
        <asp:Button runat="server" Text="Utenti" CssClass="bigButton" Style="float: left;" OnClick="utentiClick"/>
        <asp:Button runat="server" Text="Libri" CssClass="bigButton" Style="float: right;" OnClick="libriClick"/>
    </div>
</asp:Content>
