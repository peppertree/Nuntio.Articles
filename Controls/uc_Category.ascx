<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_Category.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Category" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>

            <script type="text/javascript">
        function CloseAndRebind()
        {
            GetRadWindow().Close();
            GetRadWindow().BrowserWindow.refreshTree();
        }		
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz as well)
				
            return oWindow;
        }
        function CancelEdit()
        {
            GetRadWindow().Close();		
        }     
        </script>
<dnnWerk:RadAjaxManager ID="ctlAjax" runat="server">
    <AjaxSettings>
        <dnnWerk:AjaxSetting AjaxControlID="btnCancel">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="plhControls" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>
    </AjaxSettings>
</dnnWerk:RadAjaxManager>
<div id="cat">
<div style="padding:15px;text-align:right">
    <asp:PlaceHolder ID="plhControls" runat="server"></asp:PlaceHolder>
</div>
<div style="padding:15px;text-align:right">
    <asp:Button ID="btnUpdate" runat="server" Text="Update" />
    &nbsp;
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
    &nbsp;
    <asp:Button ID="btnDelete" runat="server" Text="Delete" />    
</div>
</div>
<dnnWerk:RadFormDecorator ID="RadFormDecorator1" runat="server" Skin="Default" />
