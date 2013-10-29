<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_Panel.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Panel" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
<%@ Register TagPrefix="dw" TagName="SEARCH" Src="~/Desktopmodules/Nuntio.Articles/controls/uc_Search.ascx" %>

<Telerik:RadAjaxPanel ID="ctlAjax" runat="server" LoadingPanelID="ctlLoading" EnablePageHeadUpdate="false">

    <div style="padding-bottom: 20px;">        
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td><Telerik:RadComboBox ID="drpModules" runat="server" Width="250px" AutoPostBack="true"></Telerik:RadComboBox></td>
                <td style="padding-left: 20px;"><asp:Literal ID="lblStatus" runat="server" Text="Status:"></asp:Literal></td>
                <td style="padding-left: 20px;">
                    <Telerik:RadComboBox ID="drpFilter" runat="server" Width="250px" AutoPostBack="true">
                    </Telerik:RadComboBox>
                </td>
                <td style="padding-left: 20px;"><asp:Literal ID="lblView" runat="server" Text="View:"></asp:Literal></td>
                <td style="padding-left: 20px;">
                    <Telerik:RadComboBox ID="drpView" runat="server" Width="250px" AutoPostBack="true">
                    </Telerik:RadComboBox>
                </td>
            </tr>
        </table>
    </div>

    <asp:CheckBox ID="chkIsSearchResult" runat="server" visible="false" />    
    <dw:SEARCH id="ctlSearch" runat="server"></dw:SEARCH> 

    <Telerik:RadGrid ID="grdArticles" runat="server">
        <MasterTableView AutoGenerateColumns="false" AllowSorting="true" DataKeyNames="ItemId" AllowPaging="true" PageSize="25">
            <SortExpressions>
                <Telerik:GridSortExpression FieldName="PublishDate" SortOrder="Ascending" />
            </SortExpressions>
            <Columns>
                <Telerik:GridBoundColumn HeaderText="Title" DataField="Title"></Telerik:GridBoundColumn>
                <Telerik:GridDateTimeColumn HeaderText="Publish Date" DataField="PublishDate" DataFormatString="{0:dd.MM.yyyy - hh.mm}"></Telerik:GridDateTimeColumn>
                <Telerik:GridDateTimeColumn HeaderText="Expiry Date" DataField="ExpiryDate" DataFormatString="{0:dd.MM.yyyy - hh.mm}" EmptyDataText="n/a"></Telerik:GridDateTimeColumn>
                <Telerik:GridDateTimeColumn HeaderText="Review Date" DataField="ReviewDate" DataFormatString="{0:dd.MM.yyyy - hh.mm}" EmptyDataText="n/a"></Telerik:GridDateTimeColumn>
                <Telerik:GridCheckBoxColumn HeaderText="Is Publication" DataField="IsPublication"></Telerik:GridCheckBoxColumn>
                <Telerik:GridTemplateColumn>
                    <ItemTemplate>
                        <asp:HyperLink runat="server" Text="Manage" ID="lnkDetails" NavigateUrl="#"></asp:HyperLink>
                        <telerik:RadToolTip ID="RadToolTip1" runat="server" 
                            TargetControlID="lnkDetails" Width="450px"
                            RelativeTo="Element" 
                            Position="MiddleLeft" Skin="Black"
                            EnableShadow="true" Modal="false" ManualClose="true" ShowEvent="OnClick">
                                    <div style="padding:20px;">
                                        <p><%# GetContent(DataBinder.Eval(Container, "DataItem.Content"),DataBinder.Eval(Container, "DataItem.Summary"))%></p>
                                        <table class="nuntio_panel_manage" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td class="frm"><asp:Literal ID="lblCreated" runat="server" Text='<%# Localize("lblCreated") %>'></asp:Literal></td>
                                                <td class="frm"><%# GetDate(DataBinder.Eval(Container, "DataItem.CreatedDate"))%></td>
                                                <td class="frm"><%# GetAuthor(DataBinder.Eval(Container, "DataItem.CreatedByUser")) %></td>
                                            </tr>
                                            <tr>
                                                <td class="frm"><asp:Literal ID="lblLastUpdated" runat="server" Text='<%# Localize("lblLastUpdated") %>'></asp:Literal></td>
                                                <td class="frm"><%# GetDate(DataBinder.Eval(Container, "DataItem.LastUpdatedDate"))%></td>
                                                <td class="frm"><%# GetAuthor(DataBinder.Eval(Container, "DataItem.LastUpdatedBy")) %></td>
                                            </tr>
                                            <tr><td class="frm" colspan="3">&nbsp;</td></tr>
                                            <tr>
                                                <td class="frm">
                                                    <asp:HyperLink CssClass="nuntio_frm_lnk view" runat="server" Text='<%# Localize("ViewLink") %>' ID="HyperLink3" NavigateUrl='<%# GetArticleUrl(DataBinder.Eval(Container, "DataItem.ItemId")) %>' Target="_blank"></asp:HyperLink>
                                                </td>
                                                <td class="frm">
                                                    <asp:HyperLink CssClass="nuntio_frm_lnk delete" runat="server" Text='<%# IIF(DataBinder.Eval(Container, "DataItem.IsDeleted") = "true", Localize("HardDeleteLink"), Localize("DeleteLink")) %>' ID="HyperLink2" NavigateUrl='<%#  IIF(DataBinder.Eval(Container, "DataItem.IsDeleted") = "true", GetHardDeleteUrl(DataBinder.Eval(Container, "DataItem.ItemId")), GetDeleteUrl(DataBinder.Eval(Container, "DataItem.ItemId"))) %>'></asp:HyperLink>
                                                </td>
                                                <td class="frm">
                                                    <asp:HyperLink CssClass="nuntio_frm_lnk edit" runat="server" Text='<%# Localize("EditLink") %>' ID="HyperLink1" NavigateUrl='<%# GetEditUrl(DataBinder.Eval(Container, "DataItem.ItemId")) %>'></asp:HyperLink>
                                                </td> 
                                            </tr>
                                            <tr>
                                                <td colspan="3" style="text-align:left;" class="frm">

                                                    <asp:Panel ID="pnlDeleted" runat="server" Visible='<%# (DataBinder.Eval(Container, "DataItem.IsDeleted") = "true") %>'>
                                                        <p><asp:Hyperlink ID="lnkRestore" runat="server" Text='<%# Localize("RestoreLink") %>' NavigateUrl='<%# GetRestoreUrl(DataBinder.Eval(Container, "DataItem.ItemId")) %>'></asp:Hyperlink></p>
                                                    </asp:Panel>

                                                    <asp:Panel ID="pnlNotDeleted" runat="server" Visible='<%# (DataBinder.Eval(Container, "DataItem.IsDeleted") = "false") %>'>                                                    

                                                        <table class="nuntio_panel_manage" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td class="frm" colspan="3">
                                                                    <strong><asp:Literal ID="lblArticleType" runat="server" Text='<%# GetArticleTypeResource(DataBinder.Eval(Container, "DataItem.IsPublication"), DataBinder.Eval(Container, "DataItem.ParentId"), DataBinder.Eval(Container, "DataItem.PublicationTitle"), DataBinder.Eval(Container, "DataItem.Articles")) %>'></asp:Literal></strong>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="frm">
                                                                    <asp:Label ID="lblManage" runat="server" Text='<%# GetManageResource(DataBinder.Eval(Container, "DataItem.IsPublication"), DataBinder.Eval(Container, "DataItem.ParentId")) %>'></asp:Label>&nbsp;
                                                                </td>
                                                                <td class="frm">
                                                                    <Telerik:RadComboBox ID="drpRelatedPublicationItem" runat="server" 
                                                                        EnableLoadOnDemand="true" OnItemsRequested="drpRelatedPublicationItem_ItemsRequested" 
                                                                        Width="200px" Visible='<%# (DataBinder.Eval(Container, "DataItem.ParentId") = "-1") %>'>
                                                                    </Telerik:RadComboBox>                        
                                                                    <asp:TextBox ID="txtArticleId" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container, "DataItem.ItemId") %>'></asp:TextBox>
                                                                </td>
                                                                <td class="frm">
                                                                    &nbsp;<asp:ImageButton ID="btnManagePublicationRelation" runat="server" ImageUrl='<%# ManageRelationButtonUrl(DataBinder.Eval(Container, "DataItem.ParentId")) %>' CommandArgument='<%# DataBinder.Eval(Container, "DataItem.ItemId") %>' OnClick="btnManagePublicationRelation_Click" />
                                                                </td>                                                                
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>

                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                        </telerik:RadToolTip>                                                            
                    </ItemTemplate>
                </Telerik:GridTemplateColumn>
            </Columns>
            <DetailTables>
                <Telerik:GridTableView AutoGenerateColumns="false" ShowHeader="false" BorderStyle="None">
                    <Columns>
                        <Telerik:GridBoundColumn HeaderText="Title" DataField="Title"></Telerik:GridBoundColumn>
                        <Telerik:GridTemplateColumn>
                            <ItemTemplate>
                                <asp:HyperLink runat="server" Text="Manage" ID="lnkDetails" NavigateUrl="#"></asp:HyperLink>
                                <telerik:RadToolTip ID="RadToolTip1" runat="server" 
                                    TargetControlID="lnkDetails" Width="450px"
                                    RelativeTo="Element" 
                                    Position="MiddleLeft" Skin="Black"
                                    EnableShadow="true" Modal="false" ManualClose="true" ShowEvent="OnClick">
                                            <div style="padding:20px;">
                                                <p><%# GetContent(DataBinder.Eval(Container, "DataItem.Content"),DataBinder.Eval(Container, "DataItem.Summary"))%></p>
                                                <table class="nuntio_panel_manage" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td class="frm"><asp:Literal ID="lblCreated" runat="server" Text='<%# Localize("lblCreated") %>'></asp:Literal></td>
                                                        <td class="frm"><%# GetDate(DataBinder.Eval(Container, "DataItem.CreatedDate"))%></td>
                                                        <td class="frm"><%# GetAuthor(DataBinder.Eval(Container, "DataItem.CreatedByUser")) %></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="frm"><asp:Literal ID="lblLastUpdated" runat="server" Text='<%# Localize("lblLastUpdated") %>'></asp:Literal></td>
                                                        <td class="frm"><%# GetDate(DataBinder.Eval(Container, "DataItem.LastUpdatedDate"))%></td>
                                                        <td class="frm"><%# GetAuthor(DataBinder.Eval(Container, "DataItem.LastUpdatedBy")) %></td>
                                                    </tr>
                                                    <tr><td colspan="3">&nbsp;</td></tr>
                                                    <tr>
                                                        <td class="frm">
                                                            <asp:HyperLink CssClass="nuntio_frm_lnk view" runat="server" Text='<%# Localize("ViewLink") %>' ID="HyperLink3" NavigateUrl='<%# GetArticleUrl(DataBinder.Eval(Container, "DataItem.ItemId")) %>' Target="_blank"></asp:HyperLink>
                                                        </td>
                                                        <td class="frm">
                                                            <asp:HyperLink CssClass="nuntio_frm_lnk delete" runat="server" Text='<%# IIF(DataBinder.Eval(Container, "DataItem.IsDeleted") = "true", Localize("HardDeleteLink"), Localize("DeleteLink")) %>' ID="HyperLink2" NavigateUrl='<%# GetDeleteUrl(DataBinder.Eval(Container, "DataItem.ItemId")) %>'></asp:HyperLink>
                                                        </td>
                                                        <td class="frm">
                                                            <asp:HyperLink CssClass="nuntio_frm_lnk edit" runat="server" Text='<%# Localize("EditLink") %>' ID="HyperLink1" NavigateUrl='<%# GetEditUrl(DataBinder.Eval(Container, "DataItem.ItemId")) %>'></asp:HyperLink>
                                                        </td> 
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3" style="text-align:left;" class="frm">

                                                            <asp:Panel ID="pnlDeleted" runat="server" Visible='<%# (DataBinder.Eval(Container, "DataItem.IsDeleted") = "true") %>'>
                                                                <p><asp:Hyperlink ID="lnkRestore" runat="server" Text='<%# Localize("RestoreLink") %>' NavigateUrl='<%# GetRestoreUrl(DataBinder.Eval(Container, "DataItem.ItemId")) %>'></asp:Hyperlink></p>
                                                            </asp:Panel>
                                                            <asp:Panel ID="pnlNotDeleted" runat="server" Visible='<%# (DataBinder.Eval(Container, "DataItem.IsDeleted") = "false") %>'>                                                    
                                                                <p><asp:Literal ID="lblArticleType" runat="server" Text='<%# GetArticleTypeResource(DataBinder.Eval(Container, "DataItem.IsPublication"), DataBinder.Eval(Container, "DataItem.ParentId"), DataBinder.Eval(Container, "DataItem.PublicationTitle"), DataBinder.Eval(Container, "DataItem.Articles")) %>'></asp:Literal></p>
                                                                <table class="nuntio_panel_manage" cellpadding="0" cellspacing="0">
                                                                    <tr>
                                                                        <td class="frm">
                                                                            <asp:Label ID="lblManage" runat="server" Text='<%# GetManageResource(DataBinder.Eval(Container, "DataItem.IsPublication"), DataBinder.Eval(Container, "DataItem.ParentId")) %>'></asp:Label>&nbsp;
                                                                        </td>
                                                                        <td class="frm">
                                                                            <Telerik:RadComboBox ID="drpRelatedPublicationItem" runat="server" 
                                                                                EnableLoadOnDemand="true" OnItemsRequested="drpRelatedPublicationItem_ItemsRequested" 
                                                                                Width="200px" Visible='<%# (DataBinder.Eval(Container, "DataItem.ParentId") = "-1") %>'>
                                                                            </Telerik:RadComboBox> 
                                                                            <asp:TextBox ID="txtArticleId" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container, "DataItem.ItemId") %>'></asp:TextBox>                       
                                                                        </td>
                                                                        <td class="frm">
                                                                            &nbsp;<asp:ImageButton ID="btnManagePublicationRelation" runat="server" ImageUrl='<%# ManageRelationButtonUrl(DataBinder.Eval(Container, "DataItem.ParentId")) %>' CommandArgument='<%# DataBinder.Eval(Container, "DataItem.ItemId") %>' OnClick="btnManagePublicationRelation_Click" />
                                                                        </td>                                                                
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>

                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                </telerik:RadToolTip>                                                            
                            </ItemTemplate>
                        </Telerik:GridTemplateColumn>
                    </Columns>
                </Telerik:GridTableView>
            </DetailTables>
        </MasterTableView>
    </Telerik:RadGrid>    
    
</Telerik:RadAjaxPanel>

<Telerik:RadAjaxLoadingPanel ID="ctlLoading" runat="server" Skin="Default"></Telerik:RadAjaxLoadingPanel>

<Telerik:RadWindowManager ID="wndControls" runat="server" 
    Modal="true" 
    VisibleStatusbar="false" 
    VisibleTitlebar="true" 
    Skin="Black" 
    ReloadOnShow="true" Behaviors="Close,Maximize,Reload">
</Telerik:RadWindowManager>
