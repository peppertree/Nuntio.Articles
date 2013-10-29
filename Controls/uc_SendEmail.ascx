<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_SendEmail.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_SendEmail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>

        <script type="text/javascript">	
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz as well)
				
            return oWindow;
        }
        function CloseDialog()
        {
            GetRadWindow().Close();		
        }     
        </script>
<dnnWerk:RadAjaxManager ID="ctlAjax" runat="server">
    <AjaxSettings>
        <dnnWerk:AjaxSetting AjaxControlID="btnCancel">
            <UpdatedControls>
                <dnnWerk:AjaxUpdatedControl ControlID="plhControls" />
            </UpdatedControls>
        </dnnWerk:AjaxSetting>
    </AjaxSettings>
</dnnWerk:RadAjaxManager>
<div id="cat">

    <div style="padding:15px;text-align:left">

        <asp:Panel ID="pnlMessage" runat="server" Visible="false" style="padding: 10px; background: yellow; border: 1px dashed #ccc;">
            <asp:Literal ID="lblMessage" runat="server"></asp:Literal>
        </asp:Panel>
           
        <dnnwerk:RadTabStrip ID="ctlTabs" runat="server" SelectedIndex="0" MultiPageID="ctlPageViews" Skin="Sunset" Width="850px" ShowBaseLine="false" Align="Justify" AutoPostBack="true">
            <Tabs>
                <dnnWerk:RadTab Text="HTML Version" PageViewID="pvEditHTML"></dnnWerk:RadTab>
                <dnnWerk:RadTab Text="Text Version" PageViewID="pvEditText"></dnnWerk:RadTab>
                <dnnWerk:RadTab Text="Options" PageViewID="pvOptions"></dnnWerk:RadTab>
            </Tabs>
        </dnnwerk:RadTabStrip>

        <dnnWerk:RadMultiPage ID="ctlPageViews" runat="server" SelectedIndex="0" CssClass="tabviewContainer" Width="848px">
            
            <dnnWerk:RadPageView ID="pvEditHTML" runat="server" style="padding: 20px;" CssClass="pfEdit">
                <dnnWerk:RadEditor ID="ctlEditHtml" runat="server" Width="800px" Height="600px"></dnnWerk:RadEditor>
            </dnnWerk:RadPageView>

            <dnnWerk:RadPageView ID="pvEditText" runat="server" style="padding: 20px;" CssClass="pfEdit">
                <asp:TextBox ID="txtEditText" runat="server" Width="800" Height="400" TextMode="MultiLine"></asp:TextBox>
            </dnnWerk:RadPageView>

            <dnnWerk:RadPageView ID="pvOptions" runat="server" style="padding: 20px;" CssClass="pfEdit">
                <table cellpadding="5" cellspacing="0" width="100%">
                    <tr>
                        <td><asp:Literal ID="lblSenderUsername" runat="server" Text="From (Username):"></asp:Literal></td>
                        <td><dnnWerk:RadTextBox ID="txtSenderUsername" runat="server" Width="550px" Height="25px"></dnnWerk:RadTextBox></td>
                    </tr>
                    <tr>
                        <td><asp:Literal ID="lblSenderPassword" runat="server" Text="From (Password):"></asp:Literal></td>
                        <td><dnnWerk:RadTextBox ID="txtSenderPassword" runat="server" Width="550px" Height="25px"></dnnWerk:RadTextBox></td>
                    </tr>
                    <tr>
                        <td><asp:Literal ID="lblMailServer" runat="server" Text="List Server:"></asp:Literal></td>
                        <td><dnnWerk:RadTextBox ID="txtListServer" runat="server" Width="550px" Height="25px"></dnnWerk:RadTextBox></td>
                    </tr>
                    <tr>
                        <td><asp:Literal ID="lblRecipient" runat="server" Text="Recipient:"></asp:Literal></td>
                        <td><dnnWerk:RadTextBox ID="txtRecipient" runat="server" Width="550px" Height="25px"></dnnWerk:RadTextBox></td>
                    </tr>
                    <tr>
                        <td><asp:Literal ID="lblSubject" runat="server" Text="Subject:"></asp:Literal></td>
                        <td><dnnWerk:RadTextBox ID="txtSubject" runat="server" Width="550px" Height="25px"></dnnWerk:RadTextBox></td>
                    </tr>
                </table>
            </dnnWerk:RadPageView>

        </dnnWerk:RadMultiPage>

    </div>

    <div style="padding:15px;text-align:right">
        <asp:Button ID="btnSend" runat="server" Text="Send E-Mail" />
        &nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
        &nbsp;
        <asp:Button ID="btnRefresh" runat="server" Text="Reload Article" />
    </div>
</div>
<dnnWerk:RadFormDecorator ID="RadFormDecorator1" runat="server" Skin="Default" />
