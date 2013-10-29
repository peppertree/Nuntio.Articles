<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_ListSettings.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_ThreadedSettings" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<table cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td class="SubHead" valign="top" style="height:30px;width:300px;"><dnn:label id="plTheme" suffix=":" runat="server" Text="Theme"></dnn:label></td>
		<td valign="top" style="height:30px">
			<asp:dropdownlist id="drpThemes" runat="server"></asp:dropdownlist></td>
	</tr>	
	<tr>
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plSelectNewsModule" suffix="" controlname="cboModules" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height:30px">		    
			<asp:dropdownlist id="cboModules" runat="server" AutoPostBack="true"></asp:dropdownlist>
        </td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="height: 30px;">
		    <dnn:label id="plCategories" suffix=":" controlname="drpCategories" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height: 30px">
            <dnnWerk:RadTreeView ID="treeCategories" runat="server" CheckBoxes="true">
            </dnnWerk:RadTreeView> 
        </td>
	</tr>  
	<tr>
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plWelcomeArticle" suffix="" controlname="drpWelcomeArticle" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height:30px">		    
			<dnnWerk:RadComboBox ID="drpWelcomeArticle" runat="server" EnableLoadOnDemand="true" Width="280px"></dnnWerk:RadComboBox>
        </td>
	</tr>    	
	<tr>
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plRowCount" suffix=":" controlname="txtRowCount" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <dnnWerk:RadNumericTextBox ID="txtRowCount" runat="server" MinValue="0" ShowSpinButtons="true" NumberFormat-DecimalDigits="0">
            </dnnWerk:RadNumericTextBox>
		</td>
	</tr>
	<tr id="rowPaging" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plEnablePaging" suffix=":" controlname="chkEnablePaging" runat="server"></dnn:label>
        </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkEnablePaging" runat="server" AutoPostBack="true" />
		</td>
	</tr>
	<tr id="rowPagingMaxCount" runat="server" visible="false">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plPagingMaxCount" suffix=":" controlname="txtPagingMaxCount" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height:30px">
            <dnnWerk:RadNumericTextBox ID="txtPagingMaxCount" runat="server" MinValue="0" ShowSpinButtons="true" NumberFormat-DecimalDigits="0">
            </dnnWerk:RadNumericTextBox>		    
	    </td>
	</tr>	
	<tr id="rowIncludePublications" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plIncludePublications" suffix=":" controlname="chkIncludePublications" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkIncludePublications" runat="server" AutoPostBack="false" />
		</td>
	</tr>
	<tr id="rowIncludeNonPublications" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plIncludeNonPublications" suffix=":" controlname="chkIncludeNonPublications" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkIncludeNonPublications" runat="server" AutoPostBack="false" />
		</td>
	</tr>
	<tr id="rowShowFutureItems" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plShowFutureItems" suffix=":" controlname="chkShowFutureItems" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkShowFutureItems" runat="server" AutoPostBack="false" />
		</td>
	</tr>
	<tr id="rowShowPastItems" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plShowPastItems" suffix=":" controlname="chkShowPastItems" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkShowPastItems" runat="server" AutoPostBack="false" />
		</td>
	</tr>	
	<tr id="rowShowFeatured" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plShowFeatured" suffix=":" controlname="chkShowFeatured" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkShowFeatured" runat="server" AutoPostBack="false" />
		</td>
	</tr>
	<tr id="rowShowNonFeatured" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plShowNonFeatured" suffix=":" controlname="chkShowNonFeatured" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkShowNonFeatured" runat="server" AutoPostBack="false" />
		</td>
	</tr>  
	<tr id="rowMakeFeaturedSticky" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plMakeFeaturedSticky" suffix=":" controlname="chkMakeFeaturedSticky" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkMakeFeaturedSticky" runat="server" AutoPostBack="false" />
		</td>
	</tr>        	
	<tr id="rowSortOrder" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plSortOrder" suffix=":" controlname="drpSortOrder" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:RadioButtonList ID="rblSortOrder" runat="server" RepeatDirection="Vertical" CssClass="Normal">
                <asp:ListItem Value="0" Text="publishdateasc"></asp:ListItem>
                <asp:ListItem Value="1" Text="publishdatedesc"></asp:ListItem>
                <asp:ListItem Value="2" Text="authorasc"></asp:ListItem>
                <asp:ListItem Value="3" Text="authordesc"></asp:ListItem>
                <asp:ListItem Value="4" Text="titleasc"></asp:ListItem>
                <asp:ListItem Value="5" Text="titledesc"></asp:ListItem>
            </asp:RadioButtonList>
		</td>
	</tr>					
	<tr>
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plUseOriginalVersion" suffix=":" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkUseOriginalVersion" runat="server" />
		</td>
	</tr>	
	<tr id="rowSearch" runat="server" visible="true">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plSearch" suffix=":" controlname="chkAllowSubscriptions" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
		    <asp:checkbox id="chkEnableDnnSearch" CssClass="Normal" runat="server" AutoPostBack="false"></asp:checkbox>
		    <br />
		    <asp:checkbox id="chkEnableModuleSearch" CssClass="Normal" runat="server" AutoPostBack="false"></asp:checkbox>
	    </td>
	</tr>	
	<tr id="rowEnableSummary" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plEnableSummary" suffix=":" controlname="chkEnableSummary" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkEnableSummary" runat="server" AutoPostBack="false" />
		</td>
	</tr>	
	<tr id="rowModerate" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plModerateNews" controlname="chkModerateNews" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height: 30px">
		    <asp:CheckBox ID="chkModerateNews" runat="server" AutoPostBack="True" />
	    </td>
	</tr>
	<tr id="rowModeratorRole" runat="server" visible="false">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plModeratorRole" controlname="drpModeratorRole" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height: 30px">
            <asp:DropDownList ID="drpModeratorRole" runat="server"></asp:DropDownList>
        </td>
	</tr>	
	<tr id="rowAuthorizedRoles" runat="server" visible="false">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plAuhorizedRoles" controlname="chkAuthorizedRoles" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height:30px;">
            <asp:checkboxlist id="chkAuthorizedRoles" runat="server" 
                CssClass="Normal" 
                RepeatDirection="Horizontal"
				cellpadding="0"
                cellspacing="0" 
				RepeatColumns="2">
	        </asp:checkboxlist>
        </td>
	</tr>			
	<tr id="rowEnforceEditPermissions" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plEnforceEditPermissions" suffix=":" controlname="chkEnforceEditPermissions" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkEnforceEditPermissions" runat="server" AutoPostBack="false" />
		</td>
	</tr>
	<tr id="rowAllowTwitter" runat="server" visible="true">
		<td class="SubHead" valign="top" style="height:35px;"><dnn:label id="plAllowTwitter" suffix=":" controlname="chkAllowTwitter" runat="server"></dnn:label></td>
		<td valign="top" style="height: 35px"><asp:checkbox id="chkAllowTwitter" runat="server" AutoPostBack="false"></asp:checkbox></td>
	</tr>
	
	<tr id="rowAllowSubscriptions" runat="server" visible="true">
		<td class="SubHead" valign="top" style="height:35px;"><dnn:label id="plAllowSubscriptions" suffix=":" controlname="chkAllowSubscriptions" runat="server"></dnn:label></td>
		<td valign="top" style="height: 35px"><asp:checkbox id="chkAllowSubscriptions" runat="server" AutoPostBack="True"></asp:checkbox></td>
	</tr>
	<tr id="rowNotificationDebug" runat="server" visible="False">
		<td class="SubHead" valign="top" style="height:35px;"><dnn:label id="plNotificationDebug" suffix=":" controlname="chkNotificationDebug" runat="server"></dnn:label></td>
		<td valign="top" style="height: 35px"><asp:checkbox id="chkNotificationDebug" runat="server"></asp:checkbox></td>
	</tr>	
	<tr id="rowOnlyRegisteredUsers" runat="server" visible="False">
		<td class="SubHead" valign="top" style="height:35px;"><dnn:label id="plOnlyRegisteredUsers" suffix=":" controlname="chkAllowSubscriptions" runat="server"></dnn:label></td>
		<td valign="top" style="height: 35px"><asp:checkbox id="chkOnlyRegisteredUsers" runat="server"></asp:checkbox></td>
	</tr>
	<tr id="rowNotifyAdmin" runat="server" visible="False">
		<td class="SubHead" valign="top" style="height:35px;"><dnn:label id="plNotifyAdmin" suffix="" controlname="chkNotifyAdmin" runat="server"></dnn:label></td>
		<td valign="top" style="height: 35px"><asp:checkbox id="chkNotifyAdmin" runat="server"></asp:checkbox></td>
	</tr>
	<tr id="rowFromAddress" runat="server" visible="False">
		<td class="SubHead" valign="top" style="height:35px;"><dnn:label id="plFromAddress" suffix=":" controlname="txtFromAddress" runat="server"></dnn:label></td>
		<td valign="top" style="height: 35px">
			<asp:TextBox id="txtFromAddress" runat="server" CssClass="NormalTextBox" Width="248px"></asp:TextBox></td>
	</tr>
	<tr id="rowAutoSubscribedRoles" runat="server" visible="False">
		<td class="SubHead" valign="top" style="height:35px;"><dnn:label id="plAutoSubscribeRoles" suffix=":" controlname="chkAutoSubscribeRoles" runat="server"></dnn:label></td>
		<td valign="top" style="height: 35px"><asp:checkboxlist id="chkAutoSubscribeRoles" runat="server" CssClass="Normal" RepeatDirection="Horizontal"
				cellpadding="0" cellspacing="0" RepeatColumns="2"></asp:checkboxlist>
		</td>
	</tr>	
	<tr id="rowAnonymousComments" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plAnonymousComments" controlname="chkAnonymousComments" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height: 30px">
		    <asp:CheckBox ID="chkAnonymousComments" runat="server" AutoPostBack="false" />
	    </td>
	</tr>  
	<tr id="rowAutoApproveAnonymousComments" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plAutoApproveAnonymousComments" controlname="chkAutoApproveAnonymousComments" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height: 30px">
		    <asp:CheckBox ID="chkAutoApproveAnonymousComments" runat="server" AutoPostBack="false" />
	    </td>
	</tr>  
	<tr id="rowAutoApproveAuthenticatedComments" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plAutoApproveAuthenticatedComments" controlname="chkAutoApproveAuthenticatedComments" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height: 30px">
		    <asp:CheckBox ID="chkAutoApproveAuthenticatedComments" runat="server" AutoPostBack="false" />
	    </td>
	</tr>      
	<tr>
		<td class="SubHead" valign="top" style="height:30px;"><dnn:label id="plSplitterWidth" runat="server"></dnn:label></td>
		<td valign="top" style="height: 30px"><asp:TextBox ID="txtSplitterWidth" Runat="server" Width="80"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="height:30px;"><dnn:label id="plSplitterHeight" runat="server"></dnn:label></td>
		<td valign="top" style="height: 30px"><asp:TextBox ID="txtSplitterHeight" Runat="server" Width="80"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="height:30px;"><dnn:label id="plTocWidth" runat="server"></dnn:label></td>
		<td valign="top" style="height: 30px"><asp:TextBox ID="txtTocWidth" Runat="server" Width="80"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="height:30px;"><dnn:label id="plContentWidth" runat="server"></dnn:label></td>
		<td valign="top" style="height: 30px"><asp:TextBox ID="txtContentWidth" Runat="server" Width="80"></asp:TextBox></td>
	</tr>
	<tr id="rowJournal" runat="server">
		<td class="SubHead" valign="top" style="height:30px;">
		    <dnn:label id="plAddToJournal" controlname="chkAddToJournal" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height: 30px">
		    <asp:CheckBox ID="chkAddToJournal" runat="server" AutoPostBack="false" />
	    </td>
	</tr>               					
	<tr>
		<td class="SubHead" valign="top" style="height:30px;"><dnn:label id="plModuletitle" suffix=":" runat="server"></dnn:label></td>
		<td valign="top"><asp:placeholder id="plhModultitle" runat="server"></asp:placeholder></td>
	</tr>




</table>
<p>
    <asp:linkbutton id="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate">Update</asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton id="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel">Cancel</asp:linkbutton>
</p>

<asp:Label ID="lblVersion" runat="server" CssClass="Normal"></asp:Label>

