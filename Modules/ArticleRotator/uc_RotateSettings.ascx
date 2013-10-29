<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_RotateSettings.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_RotateSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>
<table cellspacing="0" cellpadding="0" summary="Edit Links Design Table" border="0">
	<tr>
		<td class="SubHead" valign="top" style="height:30px;width:225px;"><dnn:label id="plTheme" suffix=":" runat="server" Text="Theme"></dnn:label></td>
		<td valign="top" style="height:30px">
			<asp:dropdownlist id="drpThemes" runat="server"></asp:dropdownlist></td>
	</tr>	
	<tr>
		<td class="SubHead" valign="top" style="height:30px;width:225px;">
		    <dnn:label id="plSelectNewsModule" suffix="" controlname="cboModules" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height:30px" class="Subhead">		    
			<asp:dropdownlist id="cboModules" runat="server" AutoPostBack="true"></asp:dropdownlist>
			<asp:Label ID="lblNoModule" runat="server"></asp:Label>
        </td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="height: 30px;width:225px;">
		    <dnn:label id="plCategories" suffix=":" controlname="drpCategories" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height: 30px">
            <dnnWerk:RadTreeView ID="treeCategories" runat="server" CheckBoxes="true">
            </dnnWerk:RadTreeView> 
        </td>
	</tr>	
	<tr>
		<td class="SubHead" valign="top" style="height:30px;width:225px;">
		    <dnn:label id="plRowCount" suffix=":" controlname="txtRowCount" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <dnnWerk:RadNumericTextBox ID="txtRowCount" runat="server" MinValue="0" ShowSpinButtons="true" NumberFormat-DecimalDigits="0">
            </dnnWerk:RadNumericTextBox>
		</td>
	</tr>
	<tr id="rowShowFutureItems" runat="server">
		<td class="SubHead" valign="top" style="height:30px;width:225px;">
		    <dnn:label id="plShowFutureItems" suffix=":" controlname="chkShowFutureItems" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkShowFutureItems" runat="server" AutoPostBack="false" />
		</td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="height:30px;width:225px;">
		    <dnn:label id="plShowPastItems" suffix=":" controlname="chkShowPastItems" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkShowPastItems" runat="server" AutoPostBack="false" />
		</td>
	</tr>	
	<tr id="rowSortOrder" runat="server">
		<td class="SubHead" valign="top" style="height:30px;width:225px;">
		    <dnn:label id="plSortOrder" suffix=":" controlname="drpSortOrder" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:RadioButtonList ID="rblSortOrder" runat="server" RepeatDirection="Horizontal" CssClass="Normal">
                <asp:ListItem Value="0" Text="asc"></asp:ListItem>
                <asp:ListItem Value="1" Text="desc"></asp:ListItem>
            </asp:RadioButtonList>
		</td>
	</tr>					
	<tr>
		<td class="SubHead" valign="top" style="height:30px;width:225px;">
		    <dnn:label id="plUseOriginalVersion" suffix=":" runat="server"></dnn:label>
		</td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkUseOriginalVersion" runat="server" />
		</td>
	</tr>	
    <tr>
        <td class="SubHead" valign="top" style="width:225px"><dnn:label id="plScrollHeight" suffix=":" controlname="txtScrollHeight" runat="server"></dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtScrollHeight" runat="server"></asp:TextBox>
        </td>
    </tr>	
    <tr>
        <td class="SubHead" valign="top" style="width:225px"><dnn:label id="plScrollWidth" suffix=":" controlname="txtScrollWidth" runat="server"></dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtScrollWidth" runat="server"></asp:TextBox>
        </td>
    </tr>		
    <tr>
        <td class="SubHead" valign="top" style="width:225px"><dnn:label id="plScrollTimeout" suffix=":" controlname="txtScrollTimeout" runat="server"></dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtScrollTimeout" runat="server"></asp:TextBox>
        </td>
    </tr>	
    <tr>
        <td class="SubHead" valign="top" style="width:225px"><dnn:label id="plScrollSpeed" suffix=":" controlname="txtScrollSpeed" runat="server"></dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtScrollSpeed" runat="server"></asp:TextBox>
        </td>
    </tr>            
    <tr>
        <td class="SubHead" valign="top" style="width:225px"><dnn:label id="plScrollDirection" suffix=":" controlname="drpScrollDirection" runat="server"></dnn:label></td>
        <td valign="top">
            <asp:DropDownList ID="drpScrollDirection" runat="server"></asp:DropDownList>  
        </td>
    </tr>									
	<tr>
		<td class="SubHead" valign="top" style="width:225px;"><dnn:label id="plModuletitle" suffix=":" runat="server"></dnn:label></td>
		<td valign="top"><asp:placeholder id="plhModultitle" runat="server"></asp:placeholder></td>
	</tr>
</table>
<p>
    <asp:linkbutton id="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate">Update</asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton id="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel">Cancel</asp:linkbutton>
</p>

<asp:Label ID="lblVersion" runat="server" CssClass="Normal"></asp:Label>

