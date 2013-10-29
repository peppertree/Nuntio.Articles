<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_TabSelect.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_TabSelect" %>
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
        function setTab(tabname,tabid)
        {
            //Get a reference to the opener parent page using rad window
            var wnd = GetRadWindow();
            var openerPage = wnd.BrowserWindow;
            openerPage.setTab(tabname, tabid);
            //Close window
            wnd.close();
        }
        function dialogClose() {
            var wnd = GetRadWindow();
            wnd.close();
        }
</script>
</dnnWerk:RadScriptBlock>

<div style="width:250px; float:left;">
    <dnnWerk:RadTreeView ID="ctlPages" runat="server" style="margin:15px;"></dnnWerk:RadTreeView>
</div>

<div style="width:80px; float:left;padding-top:15px;">
    <asp:Button ID="btnClose" runat="server" Text="Close" />
</div>

<div style="clear:both;"></div>





<dnnWerk:RadFormDecorator ID="frmDecorate" runat="server" Skin="Black" />

<dnnWerk:RadAjaxManager ID="ctlAjax" runat="server">
</dnnWerk:RadAjaxManager>
