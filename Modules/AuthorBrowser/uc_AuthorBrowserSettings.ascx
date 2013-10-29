<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_AuthorBrowserSettings.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_AuthorBrowserSettings" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td class="SubHead" valign="top" style="height:35px;width:225px;">
		    <dnn:label id="plSelectNewsModule" suffix="" controlname="cboModules" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height:35px" class="Subhead">		    
			<asp:dropdownlist id="cboModules" runat="server" AutoPostBack="true"></asp:dropdownlist>
			<asp:Label ID="lblNoModule" runat="server"></asp:Label>
        </td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plSkin" suffix=":" controlname="drpSkin" runat="server"></dnn:label></td>
		<td valign="top">
		    <asp:dropdownlist id="drpSkin" runat="server">
		    </asp:dropdownlist>			
		</td>
	</tr>	
	<tr>
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plIncludeRoot" suffix=":" controlname="drpSkin" runat="server"></dnn:label></td>
		<td valign="top">
            <asp:CheckBox ID="chkIncludeRoot" runat="server" />		
		</td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plShowLines" suffix=":" controlname="drpSkin" runat="server"></dnn:label></td>
		<td valign="top">
            <asp:CheckBox ID="chkShowLines" runat="server" />		
		</td>
	</tr>	
	<tr>
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plShowFolderIcon" suffix=":" controlname="drpSkin" runat="server"></dnn:label></td>
		<td valign="top">
            <asp:CheckBox ID="chkShowFolderIcon" runat="server" />		
		</td>
	</tr>
    <tr>
        <td class="SubHead" valign="top" style="height:30px;width:225px;">
            <dnn:label id="plShowNumbers" suffix=":" controlname="chkShowNumbers" runat="server"></dnn:label>
        </td>
        <td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkShowNumbers" runat="server" />		
        </td>
    </tr>	
	<tr>
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
	<tr>
		<td class="SubHead" valign="top" style="height:30px;width:225px;">
		    <dnn:label id="plIncludeExpired" suffix=":" controlname="chkIncludeExpired" runat="server"></dnn:label>
	    </td>
		<td valign="top" style="height: 30px">
            <asp:CheckBox ID="chkIncludeExpired" runat="server" AutoPostBack="false" />
		</td>
	</tr>        			
	<tr>
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plModuletitle" suffix=":" runat="server"></dnn:label></td>
		<td valign="top">
			<asp:PlaceHolder id="plhModultitle" runat="server"></asp:PlaceHolder>
		</td>
	</tr>
</table>
<p>
    <asp:linkbutton id="cmdUpdate" runat="server" resourcekey="cmdUpdate" CssClass="CommandButton">Update</asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton id="cmdCancel" runat="server" resourcekey="cmdCancel" CssClass="CommandButton">Cancel</asp:linkbutton>
</p>
