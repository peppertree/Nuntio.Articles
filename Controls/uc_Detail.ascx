<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_Detail.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Detail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/texteditor.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/moduleauditcontrol.ascx" %>

<dnnWerk:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript">
       function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }     
        function CloseWindow(url) {
            var wnd = GetRadWindow();
            wnd.close();                       
        }        
    </script>
</dnnWerk:RadScriptBlock>
<dnnWerk:RadAjaxPanel ID="ctlAjax" runat="server">
    <asp:placeholder id="plhControls" runat="server"></asp:placeholder>
</dnnWerk:RadAjaxPanel>