<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_PanelSettings.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_PanelSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>

<p>
    <asp:Literal ID="lblDefaultModule" runat="server"></asp:Literal>
</p>
<p>
    <Telerik:RadComboBox ID="drpModules" runat="server" Width="250px" AutoPostBack="true"></Telerik:RadComboBox>
</p>
<p>
    <asp:Literal ID="lblDefaultFilter" runat="server"></asp:Literal>
</p>
<p>
    <Telerik:RadComboBox ID="drpFilter" runat="server" Width="250px" AutoPostBack="false"></Telerik:RadComboBox>    
</p>
<p>
    <asp:Literal ID="lblDefaultMode" runat="server"></asp:Literal>
</p>
<p>
    <Telerik:RadComboBox ID="drpView" runat="server" Width="250px" AutoPostBack="false">
        <Items>
            <Telerik:RadComboBoxItem Text="Publications" />
            <Telerik:RadComboBoxItem Text="Articles" />
        </Items>
    </Telerik:RadComboBox>
</p>
<p>
    <asp:linkbutton id="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate">Update</asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton id="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel">Cancel</asp:linkbutton>
</p>

<asp:Label ID="lblVersion" runat="server" CssClass="Normal"></asp:Label>

