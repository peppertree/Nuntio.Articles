<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_Search.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Search" %>


<div class="nuntio-simplesearch">    
    <div>
        <asp:TextBox ID="txtSearch" runat="server" CssClass="NormalTextBox" style="width:200px;"></asp:TextBox>
    </div>
    <div>
        <asp:RadioButtonList ID="rblSearchMode" runat="server" 
            RepeatDirection="Horizontal" 
            CssClass="Normal">
            <asp:ListItem Selected="True" Value="And">And</asp:ListItem>
            <asp:ListItem Value="Or">Or</asp:ListItem>
        </asp:RadioButtonList>    
    </div>
    <div>
        &nbsp;
        <asp:Button ID="btnSearch" runat="server" Text="Search" CausesValidation="false" />
    </div>
    <div class="nuntio-advancedbutton">
        &nbsp;
        <input id="btnShowAdv" type="button" value='<%= GetAdvancedText() %>' onclick="Nuntio_ShowAdvancedSearch();" />
    </div>
    <div>
        &nbsp;
        <asp:Button ID="btnAllNews" runat="server" Text="Search" Visible="false" CausesValidation="false" />
    </div>    
</div>




<div class="nuntio-advancedsearch" style="clear:both;display:none;">
    <div> 
        <asp:DropDownList ID="drpAuthors" runat="server" 
            DataTextField="Displayname" 
            DataValueField="UserID"
            style="width:170px;"></asp:DropDownList> 
        &nbsp;    
        <asp:DropDownList ID="drpCategories" runat="server" 
            DataTextField="CategoryName" 
            DataValueField="CategoryID"
            style="width:170px;">
            </asp:DropDownList> 
        &nbsp;   
        <asp:DropDownList ID="drpMonth" runat="server"></asp:DropDownList>
        &nbsp;
        <asp:DropDownList ID="drpYear" runat="server"></asp:DropDownList>                                                   
    </div>                        
</div>

<asp:Panel ID="pnlResult" runat="server" Visible="false" CssClass="nuntio-searchresult" style="clear:both;">
    <asp:Label ID="lblResult" runat="server" CssClass="Subhead"></asp:Label>
    <asp:Label ID="lblPaging" runat="server" CssClass="Subhead"></asp:Label>   
</asp:Panel>
 
<div class="nuntio-searchspacer"></div>    


