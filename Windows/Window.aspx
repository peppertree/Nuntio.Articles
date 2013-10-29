<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Window.aspx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.Window" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server" id="Head">
    <title><asp:Literal ID="lblTitle" runat="server" Text="Nuntio Articles"></asp:Literal></title>
    <asp:PlaceHolder ID="CSS" runat="server"></asp:PlaceHolder>
</head>
<body style="margin:0;padding:0" id="body">
    <form id="frmWindow" runat="server">    
    <asp:PlaceHolder ID="plhControls" runat="server"></asp:PlaceHolder>
    <input id="ScrollTop" runat="server" name="ScrollTop" type="hidden" />
    <input id="__dnnVariable" runat="server" name="__dnnVariable" type="hidden" />    
    </form>
</body>
</html>
