<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Settings.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellpadding="0" cellspacing="0" border="0" align="left">
	<tr>
		<td class="SubHead" valign="top" style="height:30px;width:225px;">
		    <dnn:label id="plViewMode" suffix=":" runat="server" Text="View Mode"></dnn:label>
		</td>
		<td valign="top" style="height:30px">
            <asp:DropDownList ID="drpView" runat="server" AutoPostBack="true">
                <asp:ListItem value="select" Text="Select view to load..."></asp:ListItem>
                <asp:ListItem value="articlepanel" Text="Article Panel"></asp:ListItem>
                <asp:ListItem value="articlelist" Text="Article Listing"></asp:ListItem>
                <asp:ListItem value="threadedlist" Text="Threaded Listing"></asp:ListItem>
                <asp:ListItem value="articlerotator" Text="Article Rotator"></asp:ListItem>
                <asp:ListItem value="rssbrowser" Text="RSS Browser"></asp:ListItem>
                <asp:ListItem value="authorbrowser" Text="Author Browser"></asp:ListItem>
                <asp:ListItem value="categorybrowser" Text="Category Browser"></asp:ListItem>
                <asp:ListItem value="subscribeform" Text="Subscription Form"></asp:ListItem>
                <asp:ListItem value="archivebrowser" Text="Archive Browser"></asp:ListItem>
                <asp:ListItem value="bannerviewer" Text="Banner Viewer"></asp:ListItem>
            </asp:DropDownList>		
		</td>
	</tr>   
</table>
<div style="text-align:left;clear:both;">
    <asp:PlaceHolder ID="plhControls" runat="server"></asp:PlaceHolder>
</div>
<div style="display:none;">
    <asp:TextBox ID="txtControl" runat="server"></asp:TextBox>
</div>

