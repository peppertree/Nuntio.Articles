<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_Subscribe.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Subscribe" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>
<dnnWerk:RadAjaxPanel ID="ctlAjax" runat="server" LoadingPanelID="lpAjax" EnablePageHeadUpdate="false">
<asp:Panel ID="pnlForm" runat="server" CssClass="nuntio-sub">
    <table class="nuntio-sub-container">
        <tr>
            <td class="nuntio-sub-normal">
                <asp:Literal ID="lblName" runat="server"></asp:Literal>
            </td>
            <td class="nuntio-sub-control">
                <dnnWerk:RadTextBox ID="txtName" runat="server" Width="160px">
                </dnnWerk:RadTextBox>
            </td>
        </tr>
        <tr>
            <td class="nuntio-sub-normal">
                <asp:Literal ID="lblEmail" runat="server"></asp:Literal>
            </td>
            <td class="nuntio-sub-control">
                <dnnWerk:RadTextBox ID="txtEmail" runat="server" Width="160px">
                </dnnWerk:RadTextBox>
            </td>
        </tr> 
        <tr id="rowLanguage" runat="server">
            <td class="nuntio-sub-normal">
                <asp:Literal ID="lblLanguage" runat="server"></asp:Literal>
            </td>
            <td class="nuntio-sub-control">
                <dnnWerk:RadComboBox ID="drpLanguage" runat="server" Width="160px">
                </dnnWerk:RadComboBox>
            </td>
        </tr> 
        <tr>
            <td colspan="2" class="nuntio-sub-help">
                <asp:Literal ID="lblHelp" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr id="rowModules" runat="server">            
            <td colspan="2" class="nuntio-sub-checkbox">
                <asp:CheckBoxList ID="chkModules" runat="server" RepeatColumns="1"></asp:CheckBoxList>
            </td>
        </tr>           
        <tr>
            <td colspan="2" class="nuntio-sub-button">
                <asp:Button ID="btnSubscribe" runat="server" />
                &nbsp;
                <asp:Button ID="btnUnSubscribe" runat="server" />
            </td>
        </tr>             
    </table>
</asp:Panel> 
<asp:Panel ID="pnlResult" runat="server">
    <div class="nuntio-sub-result">
        <asp:Literal ID="lblResult" runat="server"></asp:Literal>
    </div>
</asp:Panel> 
</dnnWerk:RadAjaxPanel>
<dnnWerk:RadAjaxLoadingPanel ID="lpAjax" runat="server" Skin="Sunset"></dnnWerk:RadAjaxLoadingPanel>
