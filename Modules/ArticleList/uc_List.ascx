<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_List.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_List" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
<%@ Register TagPrefix="dw" TagName="SEARCH" Src="~/Desktopmodules/Nuntio.Articles/controls/uc_Search.ascx" %>
<script type="text/javascript" language="javascript">
    function ScrollTop() {
        window.scrollTo(0, 0);
    }
    function Redirect(url) {
        window.location.href = url;
    }                                          
</script>

<Telerik:RadAjaxPanel ID="ctlAjax" runat="server" LoadingPanelID="lpPaging" EnablePageHeadUpdate="false">
    <asp:CheckBox ID="chkIsSearchResult" runat="server" visible="false" />    
    <dw:SEARCH id="ctlSearch" runat="server"></dw:SEARCH>    
    <asp:Repeater ID="rptNews" runat="server">
        <HeaderTemplate></HeaderTemplate>                
        <ItemTemplate></ItemTemplate>
        <AlternatingItemTemplate></AlternatingItemTemplate>
        <FooterTemplate></FooterTemplate>                
    </asp:Repeater>    
    <asp:PlaceHolder id="plhItem" runat="server"></asp:PlaceHolder>        
    <asp:Panel ID="pnlPaging" runat="server" CssClass="nuntio-paging nuntio-clearfix">
        <div class="nuntio-paging-left">
            <span><asp:Literal ID="lblPaging" runat="server"></asp:Literal></span>
        </div>
        <div class="nuntio-paging-right">
            <asp:PlaceHolder ID="plhPaging" runat="server"></asp:PlaceHolder>
        </div>    
    </asp:Panel>     
</Telerik:RadAjaxPanel>

<Telerik:RadAjaxLoadingPanel ID="lpPaging" runat="server" Transparency="20" style="padding-top: 40px;background:#eee;" Skin="Black"></Telerik:RadAjaxLoadingPanel>

<Telerik:RadWindowManager ID="wndControls" runat="server" 
    Modal="true" 
    VisibleStatusbar="false" 
    VisibleTitlebar="true" 
    Skin="Black" 
    ReloadOnShow="true" Behaviors="Close,Maximize,Reload">
</Telerik:RadWindowManager>