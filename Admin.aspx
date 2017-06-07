<%@ Page Title="Admin Login" Language="C#" MasterPageFile="~/GoL.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="WebGameOfLife.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
    .auto-style4 {
        width: 93px;
        height: 34px;
    }
    .auto-style5 {
        height: 34px;
    }
    .auto-style6 {
        width: 93px;
        height: 25px;
    }
    .auto-style7 {
        height: 25px;
    }
    .auto-style8 {
        height: 34px;
        width: 269px;
    }
    .auto-style9 {
        height: 25px;
        width: 269px;
    }
</style>
    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<br />
<table style="width: 100%; margin-left: 85px;">
    <tr>
        <td class="auto-style4">&nbsp;</td>
        <td class="auto-style8">
            <asp:Label ID="LabelResult" runat="server" Text=""></asp:Label>
        </td>
        <td class="auto-style5">&nbsp;</td>
    </tr>
    <tr>
        <td class="auto-style4">
            <asp:Label ID="Label1" runat="server" Text="Email"></asp:Label>
        </td>
        <td class="auto-style8">
        <asp:TextBox ID="TextBoxEmail" runat="server" Height="22px" Width="164px" style="margin-left: 0px"></asp:TextBox>
        </td>
        <td class="auto-style5"></td>
    </tr>
    <tr>
        <td class="auto-style4">
            <asp:Label ID="Label2" runat="server" Text="Password"></asp:Label>
        </td>
        <td class="auto-style8">
            <asp:TextBox ID="TextBoxPwd" runat="server" TextMode="Password" style="margin-left: 0px" Width="164px"></asp:TextBox>
        </td>
        <td class="auto-style5"></td>
    </tr>
    <tr>
        <td class="auto-style6"></td>
        <td class="auto-style9">
            <asp:Button ID="Button1" runat="server" Text="Login" />
        </td>
        <td class="auto-style7"></td>
    </tr>
</table>
<div style="margin-left: 120px">

        </div>

</asp:Content>

