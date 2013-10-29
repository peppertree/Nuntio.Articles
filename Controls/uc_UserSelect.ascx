<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_UserSelect.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_UserSelect" %>
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
        function setUser(userid, displayname)
        {
            //Get a reference to the opener parent page using rad window
            var wnd = GetRadWindow();
            var openerPage = wnd.BrowserWindow;
            openerPage.setUser(userid, displayname);
            //Close window
            wnd.close();
        }
        function dialogClose() {
            var wnd = GetRadWindow();
            wnd.close();
        }
</script>
</dnnWerk:RadScriptBlock>

<style type="text/css">
    input.checked {text-decoration: underline;}
    div {font-family: Verdana; font-size: 11px;}
</style>
<div style="padding:20px;">
    <div style="width:260px;float:left;">
        <dnnWerk:RadTextBox ID="txtName" runat="server" Width="240px" AutoPostBack="true">
        </dnnWerk:RadTextBox>
    </div>
    <div style="width:220px;float:left;padding-left:10px;padding-top:0px;margin-top:0px;">
        <div style="width:140px; overflow:hidden;float:left;"><asp:Button ID="btnUpdate" runat="server" Text="Check User" CommandName="Check" /></div>
        <div style="width:80px; overflow:hidden;float:left;"><asp:Button ID="btnClose" runat="server" Text="Close" Width="70" /></div>
    </div>
    <div style="clear:both;">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </div>    
</div>

<asp:TextBox ID="txtUsername" runat="server" Visible="false"></asp:TextBox>

<dnnWerk:RadAjaxLoadingPanel ID="lpDefault" runat="server">
    <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Sunset.Ajax.loading.gif") %>'
        style="border: 0px;" />
</dnnWerk:RadAjaxLoadingPanel>

<dnnWerk:RadAjaxManager ID="ctlAjax" runat="server">
    <AjaxSettings>
        <dnnWerk:AjaxSetting AjaxControlID="btnUpdate">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="btnUpdate" LoadingPanelID="lpDefault" />
                <dnnWerk:AjaxUpdatedControl ControlID="txtName" />
                <dnnWerk:AjaxUpdatedControl ControlID="txtUsername" />
                <dnnWerk:AjaxUpdatedControl ControlID="lblMessage" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>  
        <dnnWerk:AjaxSetting AjaxControlID="txtName">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="btnUpdate" LoadingPanelID="lpDefault" />
                <dnnWerk:AjaxUpdatedControl ControlID="txtName" />
                <dnnWerk:AjaxUpdatedControl ControlID="txtUsername" />
                <dnnWerk:AjaxUpdatedControl ControlID="lblMessage" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>               
    </AjaxSettings>
</dnnWerk:RadAjaxManager>
<dnnWerk:RadFormDecorator ID="RadFormDecorator1" runat="server" Skin="Black" />
