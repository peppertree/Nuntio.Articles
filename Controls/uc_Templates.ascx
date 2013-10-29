<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_Templates.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Templates" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="PF" %>
<style type="text/css">  
    .tmplForm   
    {   
        font-family: Verdana;
        font-size: 11px;
    }      
</style>  

<script type="text/javascript">
       function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }
        function CloseDialog() {
            var wnd = GetRadWindow();
            wnd.close();
        } 
</script>
<div class="tmplForm">
<PF:RadSplitter ID="radSplitter" runat="server" Width="100%" height="455px">
    <PF:RadPane runat="server" Id="pane_Left">

        <PF:RadSlidingZone id="LeftSlider" runat="server" ClickToOpen="true" DockedPaneId="pane_Tree" SlideDirection="Right" style="margin:0;padding:0;">
        
            <PF:RadSlidingPane runat="server" ID="pane_Help" Title="Template Help" Width="350px" height="455px" TabView="ImageOnly" style="margin:0;padding:0;">
                
                <div style="padding:10px;width:300px;">
                    <asp:Label id="lblHelp" runat="server"></asp:Label>
                </div>

                <PF:RadGrid ID="grdTemplateHelp" runat="server" Width="300px" style="margin:10px;">
                </PF:RadGrid>               
                
            </PF:RadSlidingPane>
            <PF:RadSlidingPane runat="server" ID="pane_Tree" Title="Templates" Width="250px" height="455px" TabView="ImageOnly" style="margin:0;padding:0;">
                <asp:Panel ID="pnlLeft" runat="server">
                    <PF:RadTreeView Runat="server" Id="tree_Skins" AllowNodeEditing="true" MultipleSelect="false" SingleExpandPath="true"></PF:RadTreeView>        
                </asp:Panel>
            </PF:RadSlidingPane>
            
        </PF:RadSlidingZone>
        
    </PF:RadPane>
    <PF:RadSplitBar ID="RadSplitBar1" Runat="server" />
    <PF:RadPane Runat="server" Id="Pane_Template" Width="" height="455px">      
        <PF:RadAjaxLoadingPanel ID="LoadingPanel" runat="server" cssclass="TemplatesLoadingPanel" Transparency="10" BackColor="#fefefe" IsSticky="false">    
            <div style="padding-top: 40px;font-size: 16px;color: #444;font-weight:bold;"><asp:Literal ID="lblUpdating" runat="server"></asp:Literal></div>
            <div style="padding-top: 20px;">
            <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Sunset.Ajax.Sunset.gif") %>'
                style="border: 0px;" />
            </div>
        </PF:RadAjaxLoadingPanel>      
        <asp:Panel ID="pnlRight" runat="server" height="340px">
            <table style="width:100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td valign="top">
                        <PF:RadToolBar Runat="server" id="TemplateTools" skin="Default" width="100%">
                            <collapseanimation duration="200" type="OutQuint" />
                            <Items>                           
                                <PF:RadToolBarButton runat="server" Text="Save File" CommandName="Save">
                                </PF:RadToolBarButton>
                                <PF:RadToolBarButton runat="server" Text="Copy Selected" CommandName="Create">
                                </PF:RadToolBarButton>
                                <PF:RadToolBarButton runat="server" Text="Delete Selected" CommandName="Delete">
                                </PF:RadToolBarButton>                                
                                <PF:RadToolBarDropDown runat="server" Text="Select Language">
                                </PF:RadToolBarDropDown>
                                <PF:RadToolBarButton runat="server" Text="Cancel" CommandName="Cancel">
                                </PF:RadToolBarButton> 
                                <PF:RadToolBarButton runat="server" Text="Close" CommandName="Close">
                                </PF:RadToolBarButton>                                   
                                   
                            </Items>
                        </PF:RadToolBar>                    
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                    
                        <asp:Panel ID="pnlFiles" runat="server">
                            <div style="clear:both;padding: 10px;">
                                
                                <PF:RadGrid ID="grdFiles" runat="server">
                                    <MasterTableView AutoGenerateColumns="false">
                                        <Columns>
                                            <PF:GridTemplateColumn HeaderText="Filename" ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                <%# GetFileName(DataBinder.Eval(Container.DataItem, "Name")) %>
                                                </ItemTemplate>
                                            </PF:GridTemplateColumn>
                                            <PF:GridTemplateColumn HeaderText="Preview" ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    <img src='<%# GetFileUrl(DataBinder.Eval(Container.DataItem, "Fullname")) %>' alt="Hallo" />
                                                </ItemTemplate>
                                            </PF:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </PF:RadGrid>
                                
                            </div>
                        </asp:Panel>
                        
                        <asp:Panel ID="pnlEditor" runat="server">
                            <div style="clear:both;padding: 10px;">
                                <asp:Literal ID="lblMessage" runat="server"></asp:Literal>
                            </div>
                            <div style="clear:both;padding: 10px;">
                                <asp:TextBox ID="txtTemplate" runat="server" Width="598px" Height="340" TextMode="MultiLine" Visible="true"></asp:TextBox>                         
                            </div>                             
                        </asp:Panel>
                        
                        <asp:Panel ID="pnlForm" runat="server">
                            <table style="width:100%" cellpadding="15">
                                <tr>
                                    <td colspan="2"><asp:Label ID="lblCopy" runat="server" CssClass="pnc_EditForm_Message"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblName" runat="server" Text="Name:" CssClass="pnc_EditForm_Message"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblMode" runat="server" Text="Mode:" CssClass="pnc_EditForm_Message"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:RadioButtonList ID="rblMode" RepeatDirection="Horizontal" runat="server" AutoPostBack="true" CssClass="pnc_EditForm_Message">
                                            <asp:ListItem Value="HOST" Text="Host"></asp:ListItem>
                                            <asp:ListItem Value="PORTAL" Text="Portal"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>   
                                <tr>
                                    <td colspan="2">
                                        <asp:LinkButton ID="cmdCreate" runat="server" CssClass="cmdCreate">Create</asp:LinkButton></td>
                                </tr>            
                            </table>        
                        </asp:Panel> 
                                            
                    </td>
                </tr>                
            </table>                 
        </asp:Panel>       
    </PF:RadPane>
</PF:RadSplitter>
</div>

<PF:RadAjaxManager ID="RadAjaxManager" runat="server" 
    DefaultLoadingPanelID="LoadingPanel">
    <AjaxSettings>
        <PF:AjaxSetting AjaxControlID="tree_Skins">
            <UpdatedControls>
                <PF:AjaxUpdatedControl ControlID="pnlRight" LoadingPanelID="LoadingPanel" />
                <PF:AjaxUpdatedControl ControlID="grdTemplateHelp" />
                <PF:AjaxUpdatedControl ControlID="lblHelp" />
            </UpdatedControls>
        </PF:AjaxSetting>        
        <PF:AjaxSetting AjaxControlID="rblMode">
            <UpdatedControls>
                <PF:AjaxUpdatedControl ControlID="pnlRight" LoadingPanelID="LoadingPanel" />
            </UpdatedControls>
        </PF:AjaxSetting>
        <PF:AjaxSetting AjaxControlID="TemplateTools">
            <UpdatedControls>
                <PF:AjaxUpdatedControl ControlID="pnlRight" LoadingPanelID="LoadingPanel" />
                <PF:AjaxUpdatedControl ControlID="pnlLeft" />
            </UpdatedControls>
        </PF:AjaxSetting>        
        <PF:AjaxSetting AjaxControlID="cmdCreate">
            <UpdatedControls>
                <PF:AjaxUpdatedControl ControlID="pnlRight" LoadingPanelID="LoadingPanel" />
                <PF:AjaxUpdatedControl ControlID="pnlLeft" />
            </UpdatedControls>
        </PF:AjaxSetting>        
    </AjaxSettings>
</PF:RadAjaxManager>

