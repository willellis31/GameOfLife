<%@ Page Title="Admin Home" Language="C#" MasterPageFile="~/GoL.Master" AutoEventWireup="true" CodeBehind="AdminHome.aspx.cs" Inherits="WebGameOfLife.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            height: 23px;
        }
        .auto-style2 {
            height: 23px;
            width: 108px;
        }
        .auto-style3 {
            width: 108px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="col-sm-3"></div>
    <div class="col-sm-6">
        <div class="row" style="margin-top:10%">
            <h2>Administration Menu</h2>
        </div>
        <div class="row" style="margin-top:10%">
            <asp:Button ID="ButtonUser" class="btn-block btn-lg btn-primary" runat="server" Text="User Management" OnClick="ButtonUser_Click1" />
        </div>
        <div class="row">
            <asp:Button ID="ButtonTemplate" class="btn-block btn-lg btn-primary" runat="server" Text="Template Management" OnClick="ButtonTemplate_Click1"/>
        </div>
        <div class="row" style="margin: 20px 0;">
            <div class="col-sm-6">
                <asp:Label ID="LabelUpload" Text="" runat="server"></asp:Label>
                <asp:FileUpload ID="FileUpload1" runat="server" />

            </div>
                <div class="col-sm-6">
                    <asp:Button ID="ButtonUpload" style="margin-top:10px;" class="btn-lg btn-primary" runat="server" Text="Upload Template" OnClick="ButtonUpload_Click" />

                </div>
        </div>
        <div class="row">
            <asp:Button ID="ButtonLogout" class="btn-block btn-lg btn-danger" runat="server" Text="Logout" OnClick="ButtonLogout_Click" />
        </div>
    </div>
    <div class="col-sm-3"></div>
</asp:Content>
