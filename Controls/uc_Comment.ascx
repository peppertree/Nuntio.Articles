<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_Comment.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Comment" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>
<dnnWerk:RadScriptBlock ID="RadScriptBlock1" runat="server">
<script type="text/javascript">
    function GetRadWindow() {
        var oWindow = null;
        if (window.radWindow) oWindow = window.radWindow;
        else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
        return oWindow;
    }        
</script>
</dnnWerk:RadScriptBlock>
<style type="text/css">
body 
{
    background: #eee;
}
</style>

<asp:Panel ID="pnlResult" runat="server" CssClass="nuntio-comment-form" Visible="false">
    <p>
        <asp:Literal ID="lblResult" runat="server"></asp:Literal>
    </p>
    <p>
        <asp:Button ID="btnTryAgain" runat="server" />
        &nbsp;
        <asp:Button ID="btnClose1" runat="server" />
    </p>
</asp:Panel>

<asp:Panel ID="pnlForm" runat="server" CssClass="nuntio-comment-form">

    <div class="comment_textbox">

        <div>
            <dnnWerk:RadTextBox ID="txtComment" runat="server" Textmode="MultiLine" Rows="10" Width="300px"></dnnWerk:RadTextBox>
        </div>

        <div style="padding-top:15px;">
            <dnnWerk:RadTextBox ID="txtName" runat="server" Width="300px" EmptyMessage="Your Name (required)..."></dnnWerk:RadTextBox>
        </div>

        <div style="padding-top:15px;">
            <dnnWerk:RadTextBox ID="txtEmail" runat="server" Width="300px" EmptyMessage="Your E-Mail address (required, will not be displayed) ..."></dnnWerk:RadTextBox>
        </div>

        <div style="padding-top:15px;font-size: 11px;">
            <asp:Literal ID="lblGravatar" runat="server"></asp:Literal>
        </div>

    </div>

    <div class="nuntio-comment-button">
        <asp:Button ID="btnSend" runat="server" Text="Send" />
        &nbsp;
        <asp:Button ID="btnDelete" runat="server" Text="Delete" Visible="false" />
        &nbsp;
        <asp:Button ID="btnClose2" runat="server" Text="Close" />
        &nbsp;
        <asp:checkbox ID="chkAuthorized" runat="server" />
    </div>
    
    <asp:Panel ID="pnlNeedsApproval" runat="server" Visible="false" CssClass="comment_needsapproval">
        <asp:Label ID="lblApprove" runat="server"></asp:Label>
    </asp:Panel>    
    
    <asp:Panel ID="pnlCommentBy" runat="server" Visible="false" CssClass="comment_by">
        <asp:Label ID="lblCommentBy" runat="server"></asp:Label>
    </asp:Panel>

</asp:Panel>

<asp:Panel ID="pnlNotAuthorized" runat="server" CssClass="nuntio-comment-form">

    <asp:Label ID="lblNotAuthorized" runat="server"></asp:Label>

    <div class="nuntio-comment-button">
        <asp:Button ID="btnLogin" runat="server" Text="Login" />
        &nbsp;
        <asp:Button ID="btnRegister" runat="server" Text="Register" />
        &nbsp;
        <asp:Button ID="btnClose3" runat="server" Text="Close" />
    </div>

</asp:Panel>

<dnnWerk:RadAjaxManager ID="ctlAjax" runat="server">
    <AjaxSettings>
        <dnnWerk:AjaxSetting AjaxControlID="btnSend">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="pnlForm" LoadingPanelID="lpComment" />
                <dnnWerk:AjaxUpdatedControl ControlID="pnlResult" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>
        <dnnWerk:AjaxSetting AjaxControlID="btnTryAgain">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="pnlForm" />
                <dnnWerk:AjaxUpdatedControl ControlID="pnlResult" LoadingPanelID="lpComment" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>
    </AjaxSettings>
</dnnWerk:RadAjaxManager>

<dnnWerk:RadAjaxLoadingPanel ID="lpComment" runat="server" Skin="Sunset"></dnnWerk:RadAjaxLoadingPanel>


<dnnWerk:RadFormDecorator ID="RadFormDecorator1" runat="server" skin="Sunset"/>
