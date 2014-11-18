<%@ Control language="vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Edit" AutoEventWireup="false" Explicit="True" Codebehind="uc_Edit.ascx.vb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/texteditor.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/moduleauditcontrol.ascx" %>

<style type="text/css">
    #body {background-color: #eee;}
</style>

<dnnWerk:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript">
       function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }     
        function offsetParent (element) {
            if (element.offsetParent) return element.offsetParent;
            if (element == document.body) return element;

            while ((element = element.parentNode) && element != document.body)
            if (Element.getStyle(element, 'position') != 'static')
                return element;

            return document.body;
        }     
        function CategoryMenuClicked(sender, eventArgs)
        {
            var menuItem = eventArgs.get_menuItem();
            var menu = menuItem.get_menu();
            var menuItemValue = menuItem.get_value();
            var node = eventArgs.get_node();
            var nodeValue = node.get_value();            
            var mid = '<%=NewsModuleId() %>'
            var tabid = '<%=TabId() %>';
            var userid = '<%=UserId() %>';

            menu.hide();
            var oManager = GetRadWindowManager();
            if (menuItemValue == "Add") {
                var oWnd = oManager.open('<%=ModuleDirectory() %>/Windows/Window.aspx?ctl=uc_Category&CategoryId=' + nodeValue + '&TabId=' + tabid + '&ModuleId=' + mid + '&mode=add&UserId=' + userid);
                oWnd.set_width(410);
                oWnd.set_height(300);
                oWnd.center();
            }
            else if (menuItemValue == "Edit") {
                var oWnd = oManager.open('<%=ModuleDirectory() %>/Windows/Window.aspx?ctl=uc_Category&CategoryId=' + nodeValue + '&TabId=' + tabid + '&ModuleId=' + mid + '&mode=edit&UserId=' + userid);
                oWnd.set_width(410);
                oWnd.set_height(300);
                oWnd.center();
            }
            else if (menuItemValue == "Delete") {
                $find("<%= ctlAjax.ClientID %>").ajaxRequest('deletecategory,' + nodeValue);
            }             
        }
        function refreshTree()
        {
            $find("<%= ctlAjax.ClientID %>").ajaxRequest('categories,');
        }
        function openSelect(sType, sSelect) {
            var oManager = GetRadWindowManager();
            var oWnd;
            var tabid = '<%=TabId() %>';

            if (sType == 'DMX') {
                oWnd = oManager.open('<%=ModuleDirectory() %>/Windows/Window.aspx?ctl=uc_' + sType + 'Select&Select=' + sSelect + '&TabId=' + tabid + '&Type=' + sType);
                oWnd.set_width(620);
                oWnd.set_height(445);

            }
            if (sType == 'File') {
                oWnd = oManager.open('<%=ModuleDirectory() %>/Windows/Window.aspx?ctl=uc_' + sType + 'Select&Select=' + sSelect + '&TabId=' + tabid + '&Type=' + sType);
                oWnd.set_width(620);
                oWnd.set_height(445);

            }
            if (sType == 'Gallery') {
                oWnd = oManager.open('<%=ModuleDirectory() %>/Windows/Window.aspx?ctl=uc_FileSelect&Select=' + sSelect + '&TabId=' + tabid + '&Type=' + sType);
                oWnd.set_width(620);
                oWnd.set_height(445);

            }
            if (sType == 'User') {
                oWnd = oManager.open('<%=ModuleDirectory() %>/Windows/Window.aspx?ctl=uc_' + sType + 'Select&Select=' + sSelect + '&TabId=' + tabid + '&Type=' + sType);
                oWnd.set_width(550);
                oWnd.set_height(200);

            }
            if (sType == 'Tab') {
                oWnd = oManager.open('<%=ModuleDirectory() %>/Windows/Window.aspx?ctl=uc_' + sType + 'Select&Select=' + sSelect + '&TabId=' + tabid + '&Type=' + sType);
                oWnd.set_width(400);
                oWnd.set_height(500);

            }
            oWnd.center();
        }
        function AddDMXLink(oWnd, eventArgs) {
            var dmxLink = eventArgs.get_argument();
            if (dmxLink) 
            {            
                var txtUrl = $find("<%= txtUrl.ClientID %>");
                txtUrl.set_value(dmxLink.title);
                var txtPath = document.getElementById("<%= txtPath.ClientID %>");
                txtPath.value = dmxLink.href;
            }
        }         
        function OnFileSelected(wnd, path, name, extension)
        {
            var txtUrl = $find("<%= txtUrl.ClientID %>");
            txtUrl.set_value(name);

            var txtPath = document.getElementById("<%= txtPath.ClientID %>");
            txtPath.value = path;
        }
        function OnGalleryImageSelected(wnd, path, name, extension) {
            $find("<%= ctlAjax.ClientID %>").ajaxRequest('bindgallery,' + path);
        }
        function GalleryImageSelected(path) {
            $find("<%= ctlAjax.ClientID %>").ajaxRequest('bindgallery,' + path);
        }

        function setUser(userid,displayname) {
            var txtUrl = $find("<%= txtUrl.ClientID %>");
            txtUrl.set_value(displayname);

            var txtPath = document.getElementById("<%= txtPath.ClientID %>");
            txtPath.value = userid;
        }
        function setTab(tabname, tabid) {
            var txtUrl = $find("<%= txtUrl.ClientID %>");
            txtUrl.set_value(tabname);

            var txtPath = document.getElementById("<%= txtPath.ClientID %>");
            txtPath.value = tabid;
        }                
        function ModuleMessage(msg) 
        {
            radalert(msg);
        }            
        function setFocus()
        {
            var box = $find("<%= txtUrl.ClientID %>");
            box.focus();
        }
        function ShowImageManager() {
            var args = new Telerik.Web.UI.EditorCommandEventArgs("ImageManager", null, document.createElement("a"));
            args.CssClasses = [];
            $find('<%= ctlImages.ClientID %>').open('ImageManager', args);
        }
        function ShowFileManager(path) {
            var args = new Telerik.Web.UI.EditorCommandEventArgs("DocumentManager", null, document.createElement("a"));
            args.CssClasses = [];
            args.SelectedPath = path;
            $find('<%= ctlFiles.ClientID %>').open('DocumentManager', args);
        }
        function SetExplorerImage(sender, args) {
            var path = args.value[0].getAttribute("src", 2);
            $find("<%= ctlAjax.ClientID %>").ajaxRequest('bindgallery,' + path);
        }
        function SetExplorerFile(sender, args) {
            var path = args.value.pathname;
            $find("<%= ctlAjax.ClientID %>").ajaxRequest('bindattachments,' + path + ',' + args.value);
        }
    </script>
</dnnWerk:RadScriptBlock>

<dnnWerk:RadAjaxManager ID="ctlAjax" runat="server">
    <AjaxSettings>
        <dnnWerk:AjaxSetting AjaxControlID="rblType">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="pnlUrl" LoadingPanelID="lpDefault" />
                <dnnWerk:AjaxUpdatedControl ControlID="rblType" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>
        <dnnWerk:AjaxSetting AjaxControlID="ctlAjax">
            <UpdatedControls> 
                <dnnWerk:AjaxUpdatedControl ControlID="pnlUrl" />  
                <dnnWerk:AjaxUpdatedControl ControlID="txtPath" />  
                <dnnWerk:AjaxUpdatedControl ControlID="grdImages" />  
                <dnnWerk:AjaxUpdatedControl ControlID="grdAttachments" /> 
                <dnnWerk:AjaxUpdatedControl ControlID="wndUrlSelect" /> 
                <dnnWerk:AjaxUpdatedControl ControlID="treeCategories" LoadingPanelID="lpDefault" />                                        
            </UpdatedControls>
        </dnnWerk:AjaxSetting>   
        <dnnWerk:AjaxSetting AjaxControlID="cmdUpdateImages">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="ctlTabs" LoadingPanelID="lpDefault" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>   
        <dnnWerk:AjaxSetting AjaxControlID="grdImages">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="grdImages" LoadingPanelID="lpDefault" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>
        <dnnWerk:AjaxSetting AjaxControlID="grdAttachments">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="grdAttachments" LoadingPanelID="lpDefault" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>
        <dnnWerk:AjaxSetting AjaxControlID="treeCategories">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="treeCategories" LoadingPanelID="lpDefault" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting> 
        <dnnWerk:AjaxSetting AjaxControlID="btnLoadPublicationMembers">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="rptPublicationArticles" LoadingPanelID="lpDefault" />
                <dnnWerk:AjaxUpdatedControl ControlID="txtPublication" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting> 
        <dnnWerk:AjaxSetting AjaxControlID="btnAddPublicationRelation">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="pnlPublication" LoadingPanelID="lpDefault" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>  
        <dnnWerk:AjaxSetting AjaxControlID="drpPublicationMode">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="pnlPublication" LoadingPanelID="lpDefault" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>   
        <dnnWerk:AjaxSetting AjaxControlID="ctlTools">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="ctlTools" />
                <dnnWerk:AjaxUpdatedControl ControlID="pnlForm" LoadingPanelID="lpDefault" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>     
        <dnnWerk:AjaxSetting AjaxControlID="rptVersions">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="pnlForm" LoadingPanelID="lpDefault" />                
            </UpdatedControls>
        </dnnWerk:AjaxSetting>                                                                                                 
    </AjaxSettings>
</dnnWerk:RadAjaxManager>

<div class="frmMenu">
    <dnnWerk:RadToolBar ID="ctlTools" runat="server" Width="100%" Skin="Black" AutoPostBack="true">
        <Items>
            <dnnWerk:RadToolBarButton Text="Cancel" style="margin-left: 15px;"></dnnWerk:RadToolBarButton>
            <dnnWerk:RadToolBarButton Text="Save" style="margin-left: 15px;"></dnnWerk:RadToolBarButton>
            <dnnWerk:RadToolBarButton Text="Save & Translate" style="margin-left: 15px;"></dnnWerk:RadToolBarButton>
            <dnnWerk:RadToolBarButton Text="Save & Exit" style="margin-left: 15px;"></dnnWerk:RadToolBarButton>
            <dnnWerk:RadToolBarButton Text="Delete" style="margin-left: 15px;"></dnnWerk:RadToolBarButton>
            <dnnWerk:RadToolBarButton Text="Restore" style="margin-left: 15px;"></dnnWerk:RadToolBarButton>
        </Items>
    </dnnWerk:RadToolBar> 
</div>       

<asp:Panel ID="pnlForm" runat="server" CssClass="pfEdit" style="text-align:left;">
    
    <table cellpadding="4" cellspacing="0" style="width:98%;height:99%;margin-left:10px;">
        
        <tr>
            <td style="width:100px;padding-top:15px;">
                <asp:Literal ID="lblTitle" runat="server" Text="Title:"></asp:Literal>
            </td>
            <td style="padding-top:15px;">
                <dnnWerk:RadTextBox ID="txtTitle" Width="410px" Height="20px" CssClass="pfTextbox" 
                    runat="server" Skin="Sunset">
                </dnnWerk:RadTextBox>            
            </td>
            <td style="padding-top:15px;">
                <dnnWerk:RadComboBox ID="drpLocale" runat="server" Width="200px" Skin="Sunset" AutoPostBack="true">
                    <ItemTemplate>
                        <a href="#" class="pncComboLink"><table width="100%" cellpadding="7" cellspacing="0">
                            <tr>
                                <td valign="middle" style="width:20px;"><img src='<%# DataBinder.Eval(Container, "Attributes['ImagePath']") %>' style="border: none;" alt=" " /></td>
                                <td valign="middle" align="left"><%# DataBinder.Eval(Container, "Text") %></td>
                                <td valign="middle" style="width:20px;"><img src='<%# DataBinder.Eval(Container, "Attributes['IsTranslatedImage']") %>' style="border: none;" alt='<%# DataBinder.Eval(Container, "Attributes['IsTranslatedText']") %>' /></td>
                            </tr>
                        </table></a>                
                    </ItemTemplate>
                </dnnWerk:RadComboBox>            
            </td>
        </tr>

        <tr>
            <td style="padding-top:15px;">
                <asp:Literal ID="lblPublishDate" runat="server" Text="Publishdate:"></asp:Literal>
            </td>
            <td style="padding-top:15px;">
                <dnnWerk:RadDateTimePicker ID="ctlPublishDate" runat="server" Width="200px">
                </dnnWerk:RadDateTimePicker>            
            </td>
            <td rowspan="3" style="padding-top:15px;">
                <table cellpadding="0" cellspacing="0" style="width:100%;">
                    <tr>
                        <td><asp:CheckBox ID="chkPublished" runat="server" CssClass="pfCheck" Text="Is Published" Checked="true" /></td>
                    </tr>
                    <tr>
                        <td><asp:CheckBox ID="chkOriginal" runat="server" CssClass="pfCheck" Text="This is the original text" Checked="true" /></td>
                    </tr>
                    <tr>
                        <td><asp:CheckBox ID="chkFeatured" runat="server" CssClass="pfCheck" Text="Featured" /></td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td>
                <asp:Literal ID="lblExpiryDate" runat="server" Text="Expirydate:"></asp:Literal>
            </td>
            <td>
                <dnnWerk:RadDateTimePicker ID="ctlExpiryDate" runat="server" Width="200px">
                </dnnWerk:RadDateTimePicker>            
            </td>
        </tr>

        <tr>
            <td>
                <asp:Literal ID="lblReviewDate" runat="server" Text="Reviewdate:"></asp:Literal>
            </td>
            <td>
                <dnnWerk:RadDateTimePicker ID="ctlReviewDate" runat="server" Width="200px">
                </dnnWerk:RadDateTimePicker>            
            </td>
        </tr>

        <tr>
            <td colspan="3" style="vertical-align:top;padding-bottom:20px;padding-top:15px;">
            
                <div class="tabStripContainer">
                    <dnnWerk:RadTabStrip ID="ctlTabstrip" runat="server" MultiPageID="ctlTabs" 
                        SelectedIndex="0" CssClass="tabs" Skin="Sunset" Width="100%" ShowBaseLine="false" Align="Justify">
                        <Tabs>
                            <dnnWerk:RadTab Text="Article Body" PageViewID="tabBody"></dnnWerk:RadTab>                                                                    
                            <dnnWerk:RadTab Text="Article Summary" PageViewID="tabSummary"></dnnWerk:RadTab>                        
                            <dnnWerk:RadTab Text="Advanced" PageViewID="tabAdvanced"></dnnWerk:RadTab>
                            <dnnWerk:RadTab Text="Newsletter" PageViewID="tabNewsletter"></dnnWerk:RadTab>
                            <dnnWerk:RadTab Text="Attachments" PageViewID="tabAttachments"></dnnWerk:RadTab>
                            <dnnWerk:RadTab Text="Images" PageViewID="tabImages"></dnnWerk:RadTab>
                            <dnnWerk:RadTab Text="Related Content" PageViewID="tabRelated"></dnnWerk:RadTab>
                            <dnnWerk:RadTab Text="Versioning" PageViewID="tabVersioning"></dnnWerk:RadTab>
                        </Tabs>
                    </dnnWerk:RadTabStrip>
                </div>            
            
                <dnnWerk:RadMultiPage ID="ctlTabs" runat="server" 
                    SelectedIndex="0" 
                    CssClass="tabviewContainer" style="background-color:#fff;">
                    
                    <dnnWerk:RadPageView ID="tabBody" runat="server" CssClass="tabView">
                        
                        <dnn:TextEditor ID="txtBody" runat="server" 
                            Width="800" 
                            Height="435" />                    
                            
                    </dnnWerk:RadPageView>                
                    
                    <dnnWerk:RadPageView ID="tabSummary" runat="server" CssClass="tabView">
                        
                        <dnn:TextEditor ID="txtSummary" runat="server" 
                            Width="800" 
                            Height="435" />
                        
                    </dnnWerk:RadPageView>
                    
                    <dnnWerk:RadPageView ID="tabAdvanced" runat="server" CssClass="advView">

                        <div class="advBlock">
                        
                                                     
                            
                            <div class="hdEdit">
                                                            
                                <div class="hdEditIcon">
                                    <dnnWerk:RadCodeBlock ID="RadCodeBlock5" runat="server">
                                        <img src='<%=ModuleDirectory%>/Css/img/hdCategory.png' alt="" />
                                    </dnnWerk:RadCodeBlock>                                
                                </div>
                                <div class="hdEditText">
                                    <asp:Literal ID="lblArticleInformationHead" runat="server" Text="Article Information"></asp:Literal>
                                </div>
                                <div style="clear:both;"></div>
                                
                            </div>  
                            
                            <div class="ctEdit">                                                         
                                
                                <div class="lnkControl">

                                    <p><asp:Literal ID="lblArticleId" runat="server"></asp:Literal></p>
                                
                                    <p><asp:Literal ID="lblLastUpdated" runat="server"></asp:Literal></p>

                                    <p><asp:Literal ID="lblAnchorNote" runat="server"></asp:Literal></p>

                                    <p><dnnWerk:RadTextBox ID="txtAnchor" runat="server" Width="200px"></dnnWerk:RadTextBox></p>
                                                                                                                                 
                                </div>                                
                                
                            </div>    

                        </div> 
                        
                                                     
                            
                        <div class="advBlock">

                            <div class="hdEdit">
                                                            
                                <div class="hdEditIcon">
                                    <dnnWerk:RadCodeBlock runat="server">
                                        <img src='<%=ModuleDirectory%>/Css/img/hdCategory.png' alt="" />
                                    </dnnWerk:RadCodeBlock>                                
                                </div>
                                <div class="hdEditText">
                                    <asp:Literal ID="lblHeadCategories" runat="server" Text="Article Categories"></asp:Literal>
                                </div>
                                <div style="clear:both;"></div>
                                
                            </div>  
                            
                            <div class="ctEdit">  
                                                        
                                <div class="frmHelp">
                                    <asp:Literal ID="lblCategoriesHelp" runat="server" Text="You may select unlimited categories that the article is associated with."></asp:Literal>
                                </div>
                                
                                <div class="lnkControl categories" id="dvCategoryTree" runat="server">
                                
                                    <dnnWerk:RadTreeView ID="treeCategories" runat="server" CheckBoxes="true" OnClientContextMenuItemClicked="CategoryMenuClicked" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true" >
                                        <ContextMenus>
                                            <dnnWerk:RadTreeViewContextMenu ID="FirstNode" ClickToOpen="true">
                                                <Items>
                                                    <dnnWerk:RadMenuItem Text="Add Category" Value="Add" />                                                
                                                </Items>
                                            </dnnWerk:RadTreeViewContextMenu>                                  
                                            <dnnWerk:RadTreeViewContextMenu ID="CategoryNodes" ClickToOpen="true">
                                                <Items>
                                                    <dnnWerk:RadMenuItem Text="Add Category" Value="Add" />
                                                    <dnnWerk:RadMenuItem Text="Edit Category" Value="Edit" />                                            
                                                    <dnnWerk:RadMenuItem Text="Delete Category" Value="Delete" />                                            
                                                </Items>
                                            </dnnWerk:RadTreeViewContextMenu>
                                        </ContextMenus>
                                    </dnnWerk:RadTreeView>                               
                                                                                                                                 
                                </div>                                
                                
                            </div>    
                                                       
                            <div style="clear:both;"></div>

                        </div>
                        
                                                      
                        
                        <div style="clear:both;"></div>
                        
                    </dnnWerk:RadPageView> 
                                                                                           
                    <dnnWerk:RadPageView ID="tabNewsletter" runat="server" CssClass="advView">

                         <div class="advBlock" id="pnlQueue" runat="server">
                        
                            <div class="hdEdit">
                                                            
                                <div class="hdEditIcon">
                                    <dnnWerk:RadCodeBlock ID="RadCodeBlock1" runat="server">
                                        <img src='<%=ModuleDirectory%>/Css/img/hdProcess.png' />
                                    </dnnWerk:RadCodeBlock>                            
                                </div>
                                <div class="hdEditText">
                                    <asp:Literal ID="lblEmailHeader" runat="server" Text="Other Options"></asp:Literal>
                                </div>
                                <div style="clear:both;"></div>
                                
                            </div>
                            
                            <div class="ctEdit">
                            
                                <div class="frmHelp">
                                    <asp:Literal ID="lblMeilQueueHelp" runat="server" Text="Select whether the article should be added to the newsletter e-mail queue for the next run."></asp:Literal>
                                </div>
                                <div class="frmControl">
                                    <asp:CheckBox ID="chkAddToQueue" CssClass="pfCheck" runat="server" Text="Add to E-Mail Queue (Next run: 16:25)" />
                                </div>                                                       
                                
                            </div>
                                                                                   
                        </div>    

                        <div class="advBlock">

                            <div class="hdEdit">
                                                            
                                <div class="hdEditIcon">
                                    <dnnWerk:RadCodeBlock ID="RadCodeBlock6" runat="server">
                                        <img src='<%=ModuleDirectory%>/Css/img/hdLink.png' alt="" />
                                    </dnnWerk:RadCodeBlock>
                                    
                                </div>
                                <div class="hdEditText">
                                    <asp:Literal ID="lblHeadLink" runat="server" Text="Article Link"></asp:Literal>
                                </div>
                                <div style="clear:both;"></div>
                                
                            </div>                    
                        
                            <div class="ctEdit">
                                    
                                <div class="frmHelp">
                                    <asp:Literal ID="lblLinkHelp" runat="server" Text="You can link your article to several resources. Please select the type of resource that you want to link your article to."></asp:Literal>
                                </div>

                                <div class="frmControl">
                                    <asp:RadioButtonList ID="rblType" cssclass="pfRBL" runat="server" AutoPostBack="True" RepeatDirection="vertical" RepeatColumns="2" RepeatLayout="table" EnableViewState="true">                                    
                                        <asp:ListItem value="URL">External URL</asp:ListItem>
                                        <asp:ListItem value="TAB">Internal page</asp:ListItem>
                                        <asp:ListItem value="USER">User's Profile</asp:ListItem>
                                        <asp:ListItem value="DMX">DMX File</asp:ListItem>
                                    </asp:RadioButtonList>                                 
                                </div> 
                                
                                <asp:Panel ID="pnlUrl" runat="server" CssClass="frmControl">
                                    <div style="float:left">
                                        <dnnWerk:RadTextBox ID="txtUrl" Width="160px" Height="20px" CssClass="txtBox" 
                                            runat="server" Skin="Sunset" Text="http://">
                                        </dnnWerk:RadTextBox>
                                    </div>
                                    <div style="float:left;padding-top:0px;margin-top:-1px;">
                                        <asp:Button ID="btnLinkSelect" runat="server" />                                    
                                    </div>
                                    <div style="clear:both;"></div>
                                </asp:Panel>                                                                                 
                                                            
                                <div class="frmControl">
                                    <asp:CheckBox ID="chkNewWindow" runat="server" Text="Open link in new window" CssClass="pfCheck" />
                                    <br />
                                    <asp:CheckBox ID="chkTrackUsers" runat="server" Text="Track user information about clicks" CssClass="pfCheck" />
                                    <br />
                                    <asp:CheckBox ID="chkTrackClicks" runat="server" Text="Track number of clicks" CssClass="pfCheck" />
                                </div>                               
                                                                                               
                            </div>  

                        </div>
                        
                         <div class="advBlock" id="pnlTwitter" runat="server">
                        
                            <div class="hdEdit">
                                                            
                                <div class="hdEditIcon">
                                    <dnnWerk:RadCodeBlock ID="RadCodeBlock2" runat="server">
                                        <img src='<%=ModuleDirectory%>/Css/img/hdTwitter.png' />
                                    </dnnWerk:RadCodeBlock>                            
                                </div>
                                <div class="hdEditText">
                                    <asp:Literal ID="lblTwitterHeader" runat="server" Text="Twitter Integration"></asp:Literal>
                                </div>
                                <div style="clear:both;"></div>
                                
                            </div>
                            
                            <div class="ctEdit">
                            
                                <div class="frmHelp">
                                    <asp:Literal ID="lblTwitterHelp" runat="server" Text="Check this if you want to post the first 140 chars to your twitter account."></asp:Literal>
                                </div>
                                <div class="frmControl">
                                    <asp:CheckBox ID="chkTwitter" CssClass="pfCheck" runat="server" Text="Post to Twitter" />
                                </div>
                                
                                <div class="frmHelp">
                                    <asp:Literal ID="lblTwitterAccount" runat="server" Text=""></asp:Literal>
                                </div>
                                <div class="frmControl">
                                    <dnnWerk:RadTextBox ID="txtTwitterAccount" runat="server"></dnnWerk:RadTextBox>
                                </div>                                                                                       
                                
                            </div>
                                                                                   
                        </div>                         
                         
                         <div style="clear:both;"></div>
                                      
                    </dnnWerk:RadPageView>  
                    
                    <dnnWerk:RadPageView ID="tabAttachments" runat="server" CssClass="tabView">


                        <asp:Panel ID="pnlAttachmentsWarning" runat="server" CssClass="alert">
                            <h2><asp:Literal ID="lblAttachmentsWarning" runat="server"></asp:Literal></h2>
                        </asp:Panel>
                        <asp:Panel ID="pnlAttachmentsNote" runat="server" CssClass="note">
                            <h2><asp:Literal ID="lblAttachmentsIntro" runat="server"></asp:Literal></h2>
                        </asp:Panel>

                        <asp:Panel ID="Panel3" runat="server" style="padding-bottom: 20px;padding-top:20px;">
                            <dnnWerk:RadGrid ID="grdAttachments" runat="server">
                                <MasterTableView AutoGenerateColumns="false" DataKeyNames="ArticleFileId">
                                    <Columns>                                    
                                        <dnnWerk:GridTemplateColumn>
                                            <ItemTemplate>                                                                                               
                                                <table>
                                                    <tr>
                                                        <td><asp:Image ID="imgArticle" runat="server" Width="30" /></td>
                                                        <td><%# Databinder.Eval(Container.DataItem, "FileName") %></td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </dnnWerk:GridTemplateColumn>
                                        <dnnWerk:GridTemplateColumn>
                                            <ItemTemplate>
                                                <div><dnnWerk:RadTextBox ID="txtAttachmentTitle" runat="server" Width="250px"></dnnWerk:RadTextBox></div>
                                                <div><dnnWerk:RadTextBox ID="txtAttachmentDescription" runat="server" Width="250px" Height="40px" TextMode="MultiLine"></dnnWerk:RadTextBox></div>
                                            </ItemTemplate>
                                        </dnnWerk:GridTemplateColumn>  
                                        <dnnWerk:GridTemplateColumn>
                                            <ItemTemplate>
                                                <div><asp:LinkButton ID="cmdMakeAttachmentPrimary" runat="server" CssClass="cmd" OnClick="cmdMakeAttachmentPrimary_Click" CommandArgument='<%# Databinder.Eval(Container.DataItem, "ArticleFileId") %>'></asp:LinkButton></div>
                                                <div><asp:LinkButton ID="cmdDeleteAttachment" runat="server" CssClass="cmd" OnClick="cmdDeleteAttachment_Click" CommandArgument='<%# Databinder.Eval(Container.DataItem, "ArticleFileId") %>'></asp:LinkButton></div>
                                            </ItemTemplate>
                                        </dnnWerk:GridTemplateColumn>                                                                                                
                                    </Columns>
                                </MasterTableView>
                            </dnnWerk:RadGrid>
                        </asp:Panel>
                              
                        <div class="uplBtn">
                            <asp:LinkButton ID="cmdAddAttachment" runat="server" CssClass="cmdAddImage" />
                        </div>
                
                        <div class="clearfix"></div> 

                    </dnnWerk:RadPageView>

                    <dnnWerk:RadPageView ID="tabImages" runat="server" CssClass="tabView">
                                                                       
                        <asp:Panel ID="pnlImagesWarning" runat="server" CssClass="alert">
                            <h2><asp:Literal ID="lblImagesWarning" runat="server"></asp:Literal></h2>
                        </asp:Panel>
                        <asp:Panel ID="pnlImagesNote" runat="server" CssClass="note">
                            <h2><asp:Literal ID="lblImagesIntro" runat="server"></asp:Literal></h2>
                        </asp:Panel>

                        <asp:Panel ID="pnlImagesGrid" runat="server" style="padding-bottom: 20px;padding-top:20px;">
                            <dnnWerk:RadGrid ID="grdImages" runat="server">
                                <MasterTableView AutoGenerateColumns="false" DataKeyNames="ArticleFileId">
                                    <Columns>                                    
                                        <dnnWerk:GridTemplateColumn>
                                            <ItemTemplate>
                                                <asp:Image ID="imgArticle" runat="server" Width="60" ImageUrl='<%# Databinder.Eval(Container.DataItem, "Url") %>' />
                                            </ItemTemplate>
                                        </dnnWerk:GridTemplateColumn>
                                        <dnnWerk:GridTemplateColumn>
                                            <ItemTemplate>
                                                <div><dnnWerk:RadTextBox ID="txtImageTitle" runat="server" Width="250px"></dnnWerk:RadTextBox></div>
                                                <div><dnnWerk:RadTextBox ID="txtImageDescription" runat="server" Width="250px" Height="40px" TextMode="MultiLine"></dnnWerk:RadTextBox></div>
                                            </ItemTemplate>
                                        </dnnWerk:GridTemplateColumn>  
                                        <dnnWerk:GridTemplateColumn>
                                            <ItemTemplate>
                                                <div><asp:LinkButton ID="cmdMakeImagePrimary" runat="server" CssClass="cmd" OnClick="cmdMakeImagePrimary_Click" CommandArgument='<%# Databinder.Eval(Container.DataItem, "ArticleFileId") %>'></asp:LinkButton></div>
                                                <div><asp:LinkButton ID="cmdDeleteImage" runat="server" CssClass="cmd" OnClick="cmdDeleteImage_Click" CommandArgument='<%# Databinder.Eval(Container.DataItem, "ArticleFileId") %>'></asp:LinkButton></div>
                                            </ItemTemplate>
                                        </dnnWerk:GridTemplateColumn>                                                                                                
                                    </Columns>
                                </MasterTableView>
                            </dnnWerk:RadGrid>
                        </asp:Panel>
                              
                        <div class="uplBtn">
                            <asp:LinkButton ID="cmdAddImage" runat="server" CssClass="cmdAddImage" />
                        </div>
                
                        <div class="clearfix"></div>           
                            
                    </dnnWerk:RadPageView>                      
                    
                    <dnnWerk:RadPageView ID="tabRelated" runat="server" CssClass="advView">
                    
                        <div class="advBlock">

                            <div class="hdEdit">
                                                            
                                <div class="hdEditIcon">
                                    <dnnWerk:RadCodeBlock ID="RadCodeBlock4" runat="server">
                                        <img src='<%=ModuleDirectory%>/Css/img/hdCategory.png' alt="" />
                                    </dnnWerk:RadCodeBlock>                                
                                </div>
                                <div class="hdEditText">
                                    <asp:Literal ID="lblPublicationsHead" runat="server" Text="Publications"></asp:Literal>
                                </div>
                                <div style="clear:both;"></div>
                                
                            </div>  
                            
                            <div class="ctEdit">  
                                                        
                                <div class="frmHelp">
                                    <asp:Literal ID="lblPublicationsIntro" runat="server" Text="You may configure this article to be part of a publication"></asp:Literal>
                                </div>
                                
                                <asp:Panel cssClass="lnkControl categories" id="pnlPublication" runat="server">
                                                                                                      
                                    <table>
                                        <tr>
                                            <td colspan="2">
                                                <dnnWerk:RadComboBox ID="drpPublicationMode" runat="server" Width="280px" AutoPostBack="true">
                                                    <Items>
                                                        <dnnWerk:RadComboBoxItem Text="Default mode" />
                                                        <dnnWerk:RadComboBoxItem Text="Article is a publication" Value="IsPublication" />
                                                        <dnnWerk:RadComboBoxItem Text="Article is part of a publication" Value="IsChild" />
                                                    </Items>
                                                </dnnWerk:RadComboBox>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Literal ID="lblBelongsTo" runat="server"></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <dnnWerk:RadComboBox ID="drpRelatedPublicationItem" runat="server" EnableLoadOnDemand="true" Width="280px">
                                                </dnnWerk:RadComboBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="btnAddPublicationRelation" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                    
                                    <div>
                                        <dnnWerk:RadTreeView ID="treePublication" runat="server"></dnnWerk:RadTreeView>
                                    </div>
                                                                                                                                 
                                </asp:Panel>
                                
                            </div>    
                                                       
                        </div>  

                        <div class="advBlock">

                            <div class="hdEdit">
                                                            
                                <div class="hdEditIcon">
                                    <dnnWerk:RadCodeBlock ID="RadCodeBlock3" runat="server">
                                        <img src='<%=ModuleDirectory%>/Css/img/hdCategory.png' alt="" />
                                    </dnnWerk:RadCodeBlock>                                
                                </div>
                                <div class="hdEditText">
                                    <asp:Literal ID="lblRelatedHead" runat="server" Text="Related Articles"></asp:Literal>
                                </div>
                                <div style="clear:both;"></div>
                                
                            </div>  
                            
                            <div class="ctEdit">  
                                                        
                                <div class="frmHelp">
                                    <asp:Literal ID="lblRelatedIntro" runat="server" Text="You may related the article to other articles in this portal."></asp:Literal>
                                </div>
                                
                                <div class="lnkControl categories" id="Div1" runat="server">
                                
                                    <table>
                                        <tr>
                                            <td>
                                                <dnnWerk:RadComboBox ID="drpRelatedArticleItem" runat="server" EnableLoadOnDemand="true" Width="280px">
                                                </dnnWerk:RadComboBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="btnAddArticleRelation" runat="server" />
                                            </td>
                                        </tr>
                                    </table>

                                    <div>
                                        <dnnWerk:RadListBox ID="ctlRelatedArticles" runat="server" AllowDelete="true" AllowTransferDuplicates="false" Width="313px"></dnnWerk:RadListBox>
                                    </div>
                                                                                                                                 
                                </div>                                
                                
                            </div>    
                                                       
                        </div>  
                        
                        <div style="clear:both;"></div>

                    </dnnWerk:RadPageView>  
                    
                    <dnnWerk:RadPageView ID="tabVersioning" runat="server" CssClass="advView">
                    
                        <asp:Panel ID="pnlVersioning" runat="server" style="padding:20px;">
                            
                            <asp:Repeater ID="rptVersions" runat="server">
                                <HeaderTemplate>
                                    <table width="100%" cellspacing="0" cellpadding="5">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td style="background: #eee;"><%# DataBinder.Eval(Container.DataItem, "Version")%></td>
                                        <td style="background: #eee;"><%# DataBinder.Eval(Container.DataItem, "CreatedDate")%></td>
                                        <td style="background: #eee;"><%# GetDisplayname(DataBinder.Eval(Container.DataItem, "CreatedBy"))%></td>
                                        <td style="background: #eee;"><asp:LinkButton ID="cmdRestoreVersion" runat="server" Text="Restore" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Version")%>' OnClick="cmdRestoreVersion_Click"></asp:LinkButton></td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr>
                                        <td style="background: #fefefe;"><%# DataBinder.Eval(Container.DataItem, "Version")%></td>
                                        <td style="background: #fefefe;"><%# DataBinder.Eval(Container.DataItem, "CreatedDate")%></td>
                                        <td style="background: #fefefe;"><%# GetDisplayname(DataBinder.Eval(Container.DataItem, "CreatedBy"))%></td>
                                        <td style="background: #fefefe;"><asp:LinkButton ID="cmdRestoreVersion" runat="server" Text="Restore" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Version")%>' OnClick="cmdRestoreVersion_Click"></asp:LinkButton></td>
                                    </tr>                                
                                </AlternatingItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </asp:Panel>

                    </dnnWerk:RadPageView>         

                </dnnWerk:RadMultiPage>   
                 
            </td>
        </tr>        
    
    </table>       

</asp:Panel>        


<dnnWerk:RadAjaxLoadingPanel ID="lpDefault" runat="server" Skin="Default" MinDisplayTime="1000"></dnnWerk:RadAjaxLoadingPanel>
<dnnWerk:RadAjaxLoadingPanel ID="lpUpdating" runat="server" InitialDelayTime="1000" Skin="Black"></dnnWerk:RadAjaxLoadingPanel>
<dnnWerk:RadAjaxLoadingPanel ID="lpDelete" runat="server" Skin="Black"></dnnWerk:RadAjaxLoadingPanel>

<dnnWerk:RadWindowManager ID="wndManager" runat="server" 
    Overlay="true" 
    Animation="None" 
    VisibleOnPageLoad="true" 
    ReloadOnShow="true" 
    KeepInScreenBounds="true" 
    Behavior="Close" 
    VisibleStatusbar="false" VisibleTitlebar="false"
    Skin="Black" Modal="true" Style="z-index: 90001 !important">
        
</dnnWerk:RadWindowManager>

<dnnwerk:RadDialogOpener id="ctlImages" runat="server" VisibleTitlebar="false"></dnnwerk:RadDialogOpener>
<dnnwerk:RadDialogOpener id="ctlFiles" runat="server" VisibleTitlebar="false"></dnnwerk:RadDialogOpener>
<dnnwerk:RadDialogOpener id="ctlDMX" runat="server" VisibleTitlebar="false"></dnnwerk:RadDialogOpener>


<div style="display:none">
    <asp:TextBox ID="txtPath" runat="server"></asp:TextBox>
</div>

