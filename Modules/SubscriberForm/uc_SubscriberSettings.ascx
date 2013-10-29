<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_SubscriberSettings.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_SubscriberSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="0" border="0">
    <tr>
        <td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plSelectNewsModule" suffix=":" controlname="cboTabs" runat="server"></dnn:label></td>
        <td valign="top" class="Subhead">
            <asp:CheckBoxList ID="chkNewsModules" runat="server" CssClass="Normal">
            </asp:CheckBoxList>
            <asp:Label ID="lblNoModule" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" style="width:225px;height:35px"><dnn:label id="plProceedLink" suffix=":" controlname="drpTabs" runat="server"></dnn:label></td>
        <td valign="top" >
            <asp:DropDownList ID="drpTabs" runat="server" AutoPostBack="false" DataTextField="TabName"
                DataValueField="TabID" Width="200px">
            </asp:DropDownList>
        </td>
    </tr>
	<tr>
		<td class="SubHead" valign="top" style="height:35px;width:225px;"><dnn:label id="plTheme" suffix=":" runat="server" Text="Theme"></dnn:label></td>
		<td valign="top" style="height:30px">
			<asp:dropdownlist id="drpThemes" runat="server"></asp:dropdownlist></td>
	</tr>      
    <tr>
        <td class="SubHead" valign="top" style="width:225px;height:35px;">
            <dnn:label id="plModuletitle" suffix=":" runat="server"></dnn:label>
        </td>
        <td valign="top">
            <asp:PlaceHolder id="plhModultitle" runat="server"></asp:PlaceHolder>
        </td>
    </tr>
</table>
<p>
    <asp:linkbutton id="cmdUpdate" runat="server" resourcekey="cmdUpdate" CssClass="CommandButton">Update</asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton id="cmdCancel" runat="server" resourcekey="cmdCancel" CssClass="CommandButton">Cancel</asp:linkbutton>
</p>
