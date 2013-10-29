<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_Message.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Message" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>

<dnnWerk:RadScriptBlock ID="RadScriptBlock1" runat="server">
<script type="text/javascript">
        //A function that will return a reference to the parent radWindow in case the page is loaded in a RadWindow object
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }
        function Close()
        {
                var wnd = GetRadWindow();
                wnd.close();
        }

</script>
</dnnWerk:RadScriptBlock>
<asp:PlaceHolder ID="plhControls" runat="server"></asp:PlaceHolder>
<asp:Button ID="btnClose" runat="server" Text="OK" />
<dnnWerk:RadAjaxManager ID="ctlAjax" runat="server">
</dnnWerk:RadAjaxManager>

