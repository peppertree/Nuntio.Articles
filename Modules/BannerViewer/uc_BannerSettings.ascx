<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_BannerSettings.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_BannerSettings" %>
<%@ Register Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" TagPrefix="DNN" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<style type="text/css">
.GroupSuggestMenu {
    width: 250px;
}
</style>
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
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plHideOnAllNews" suffix=":" controlname="drpSkin" runat="server"></dnn:label></td>
		<td valign="top">
            <asp:CheckBox ID="chkHideOnAllNews" runat="server" />		
		</td>
	</tr>	
	<tr>
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plSource" runat="server" controlname="optSource" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:RadioButtonList id="optSource" runat="server" CssClass="NormalBold" RepeatDirection="Horizontal">
				<asp:ListItem Value="G" resourcekey="Host">Host</asp:ListItem>
				<asp:ListItem Value="L" resourcekey="Portal">Site</asp:ListItem>
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plType" runat="server" controlname="cboType" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:DropDownList ID="cboType" Runat="server" CssClass="NormalTextBox" Width="250px" DataTextField="BannerTypeName"
				DataValueField="BannerTypeId"></asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plCount" runat="server" controlname="txtCount" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:TextBox id="txtCount" Runat="server" CssClass="NormalTextBox" Columns="30" Width="250px"></asp:TextBox>
			<asp:RegularExpressionValidator id="valCount" ControlToValidate="txtCount" ValidationExpression="^[0-9]*$" Display="Dynamic"
				resourcekey="valCount.ErrorMessage" runat="server" CssClass="NormalRed" />
		</td>
	</tr>
	<tr>
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plTransitionType" runat="server" controlname="optSource" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:RadioButtonList id="rblTransitionType" runat="server" CssClass="NormalBold" RepeatDirection="Horizontal">
				<asp:ListItem Value="Scroll" resourcekey="Scroll">Scroll</asp:ListItem>
				<asp:ListItem Value="Slideshow" resourcekey="Slideshow">Slideshow</asp:ListItem>
			</asp:RadioButtonList>
		</td>
	</tr>	
	<tr>
		<td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plTransition" runat="server" controlname="cboType" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:DropDownList ID="drpTransition" Runat="server" CssClass="NormalTextBox" Width="250px">
				<asp:ListItem Value="NONE" resourcekey="None">NONE</asp:ListItem>
				<asp:ListItem Value="Barn">Barn</asp:ListItem>
				<asp:ListItem Value="Blinds">Blinds</asp:ListItem>			
				<asp:ListItem Value="Checkerboard">Checkerboard</asp:ListItem>
				<asp:ListItem Value="Fade">Fade</asp:ListItem>
				<asp:ListItem Value="GradientWipe">GradientWipe</asp:ListItem>
				<asp:ListItem Value="Inset">Inset</asp:ListItem>
				<asp:ListItem Value="Iris">Iris</asp:ListItem>
				<asp:ListItem Value="Pixelate">Pixelate</asp:ListItem>
				<asp:ListItem Value="RadialWipe">RadialWipe</asp:ListItem>				
				<asp:ListItem Value="RandomBars">RandomBars</asp:ListItem>
				<asp:ListItem Value="RandomDissolve">RandomDissolve</asp:ListItem>
				<asp:ListItem Value="Slide">Slide</asp:ListItem>
				<asp:ListItem Value="Spiral">Spiral</asp:ListItem>
				<asp:ListItem Value="Stretch">Stretch</asp:ListItem>
				<asp:ListItem Value="Strips">Strips</asp:ListItem>
				<asp:ListItem Value="Wheel">Wheel</asp:ListItem>
				<asp:ListItem Value="ZigZag">ZigZag</asp:ListItem>
                <asp:ListItem Value="RANDOM" resourcekey="Random">RANDOM</asp:ListItem>				
			</asp:DropDownList>
		</td>
	</tr>		
    <tr>
        <td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plScrollHeight" suffix=":" controlname="txtScrollHeight" runat="server"></dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtScrollHeight" runat="server"></asp:TextBox>
        </td>
    </tr>	
    <tr>
        <td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plScrollWidth" suffix=":" controlname="txtScrollWidth" runat="server"></dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtScrollWidth" runat="server"></asp:TextBox>
        </td>
    </tr>		
    <tr>
        <td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plScrollTimeout" suffix=":" controlname="txtScrollTimeout" runat="server"></dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtScrollTimeout" runat="server"></asp:TextBox>
        </td>
    </tr>	
    <tr>
        <td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plScrollSpeed" suffix=":" controlname="txtScrollSpeed" runat="server"></dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtScrollSpeed" runat="server"></asp:TextBox>
        </td>
    </tr>	            
    <tr>
        <td class="SubHead" valign="top" style="width:225px;height:35px;"><dnn:label id="plScrollDirection" suffix=":" controlname="drpScrollDirection" runat="server"></dnn:label></td>
        <td valign="top">
            <asp:DropDownList ID="drpScrollDirection" runat="server">
            
            </asp:DropDownList>  
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
