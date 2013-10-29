<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_List.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_ThreadedList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
<%@ Register TagPrefix="dw" TagName="SEARCH" Src="~/Desktopmodules/Nuntio.Articles/controls/uc_Search.ascx" %>

<Telerik:RadCodeBlock ID="radcodeblock" runat="server">
<script type="text/javascript" language="javascript">
    function ScrollTop() {
        window.scrollTo(0,0);
    }
    function Redirect(url) {
        window.location.href = url;
    }
    function TreeMenuClicked(sender, eventArgs) {
        
        var menuItem = eventArgs.get_menuItem();
        var menu = menuItem.get_menu();
        var menuItemValue = menuItem.get_value();
        var node = eventArgs.get_node();
        var nodeValue = node.get_value();
        var mid = '<%=NewsModuleId() %>'
        var tabid = '<%=TabId() %>'

        menu.hide();

        var oManager = GetRadWindowManager();
        if (menuItemValue == "Category_Delete") {
            $find("<%= ctlAjax.ClientID %>").ajaxRequest('Category_Delete,' + nodeValue);
        }
        else if (menuItemValue == "Category_Edit") {
            var oWnd = oManager.open('<%=ModuleDirectory() %>/Windows/Window.aspx?ctl=uc_Category&CategoryId=' + nodeValue + '&TabId=' + tabid + '&ModuleId=' + mid + '&mode=edit');
            oWnd.set_width(410);
            oWnd.set_height(300);
            oWnd.center();
        }
        else if (menuItemValue == "Article_Edit") {
            NuntioArticlesEditForm(nodeValue, '<%=NewsModuleId() %>', '<%=ModuleId() %>');
        }
        else if (menuItemValue == "Article_Add") {
            NuntioArticlesAddForm('<%=ModuleId() %>','<%=NewsModuleId() %>', nodeValue);
        }
        else if (menuItemValue == "Article_Delete") {
            $find("<%= ctlAjax.ClientID %>").ajaxRequest('Article_Delete,' + nodeValue);
        }
    }
    function refreshTree() {
        $find("<%= ctlAjax.ClientID %>").ajaxRequest('RefreshTree,0');
    }
</script>
</Telerik:RadCodeBlock>

<Telerik:RadAjaxPanel ID="ctlAjax" runat="server" LoadingPanelID="lpPaging">

    <asp:CheckBox ID="chkIsSearchResult" runat="server" visible="false" />
     
    <Telerik:RadSplitter ID="ctlSplitContainer" runat="server" LiveResize="true" ResizeMode="Proportional" ResizeWithParentPane="true" VisibleDuringInit="false">
        
        <Telerik:RadPane ID="ctlToc" runat="server">
            
            <div>
                <Telerik:RadToolBar ID="ctlTools" runat="server" Width="100%" Skin="Black" AutoPostBack="true">
                    <Items>
                        <Telerik:RadToolBarButton Text="Expand All" style="margin-left: 5px;" CommandName="expandcollapse"></Telerik:RadToolBarButton>
                        <Telerik:RadToolBarButton Text="Add Article" style="margin-left: 5px;" CommandName="addarticle"></Telerik:RadToolBarButton>
                        <Telerik:RadToolBarButton Text="Delete Selected" style="margin-left: 5px;" CommandName="deleteselected"></Telerik:RadToolBarButton>
                        <Telerik:RadToolBarDropDown Text="View">
                            <Buttons>
                                <Telerik:RadToolBarButton Text="Waiting for approval" CommandName="view_unapproved"></Telerik:RadToolBarButton>
                                <Telerik:RadToolBarButton Text="Expired Articles" CommandName="view_expired"></Telerik:RadToolBarButton>
                                <Telerik:RadToolBarButton Text="Not yet published" CommandName="view_notyetpublished"></Telerik:RadToolBarButton>
                            </Buttons>
                        </Telerik:RadToolBarDropDown>
                    </Items>
                </Telerik:RadToolBar>             
            </div>
            <Telerik:RadTreeView ID="CategoriesTree" runat="server" OnClientContextMenuItemClicked="TreeMenuClicked">
                <ContextMenus>
                    <Telerik:RadTreeViewContextMenu ID="CategoriesMenu" ClickToOpen="true">
                        <Items>
                            <Telerik:RadMenuItem Text="Edit Category" Value="Category_Edit" />
                            <Telerik:RadMenuItem Text="Delete Category" Value="Category_Delete" />                            
                            <Telerik:RadMenuItem Text="Add Article" Value="Article_Add" />    
                        </Items>
                    </Telerik:RadTreeViewContextMenu>
                    <Telerik:RadTreeViewContextMenu ID="ArticlesMenu" ClickToOpen="true">
                        <Items>
                            <Telerik:RadMenuItem Text="Edit Article" Value="Article_Edit" />                        
                            <Telerik:RadMenuItem Text="Delete Article" Value="Article_Delete" />                            
                        </Items>
                    </Telerik:RadTreeViewContextMenu>
                </ContextMenus>
            </Telerik:RadTreeView>     
               
        </Telerik:RadPane>

        <Telerik:RadSplitBar ID="ctlBar" runat="server"></Telerik:RadSplitBar>    
        
        <Telerik:RadPane ID="ctlContent" runat="server">
            
            <div class="toc-search" runat="server" id="dvSearch"><dw:SEARCH id="ctlSearch" runat="server"></dw:SEARCH></div>
            <asp:PlaceHolder id="plhItem" runat="server"></asp:PlaceHolder> 
            <iframe id="ctlFrame" src="http://www.google.com"  runat="server" visible="false" width="100%" scrolling="auto" frameborder="0" class="toc-iframe" />        

        </Telerik:RadPane>

    </Telerik:RadSplitter>
            
    <div style="clear:both;"></div>
    
    
</Telerik:RadAjaxPanel>

<Telerik:RadAjaxLoadingPanel ID="lpPaging" runat="server" Transparency="20" style="padding-top: 40px;background:#eee;" Skin="Black">          
</Telerik:RadAjaxLoadingPanel>


<Telerik:RadWindowManager ID="wndControls" runat="server" 
    Modal="true" 
    VisibleStatusbar="false" 
    VisibleTitlebar="true" 
    Skin="Black" 
    ReloadOnShow="true" Behaviors="Close,Maximize,Reload">
</Telerik:RadWindowManager>
