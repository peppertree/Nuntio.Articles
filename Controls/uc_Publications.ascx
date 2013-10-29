<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_Publications.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Publications" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>

<script type="text/javascript">
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz as well)				
            return oWindow;
        }
        function Close()
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

<table cellpadding="20">
    <tr>
        <td style="vertical-align:top;width:300px;">            
            <asp:Panel ID="pnlTree" runat="server">
                <div style="padding:10px 0 20px 0;"><asp:LinkButton ID="cmdNewPublication" runat="server" Text="Create new Publication"></asp:LinkButton></div>
                <asp:Panel ID="pnlCheck" runat="server" Visible="false"><asp:CheckBox ID="chkNew" runat="server" /></asp:Panel>
                <dnnWerk:RadTreeView ID="ctlTree" runat="server"></dnnWerk:RadTreeView>
            </asp:Panel>
        </td>
        <td style="vertical-align:top;padding-left:40px;">
            <asp:Panel ID="pnlAddForm" runat="server" Visible="false">
                <table>
                    <tr>
                        <td colspan="2" style="padding: 20px 0 20px 0;">
                            <asp:Literal ID="lblIntro" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dnnWerk:RadComboBox ID="drpRelatedPublicationItem" runat="server" 
                                EnableLoadOnDemand="true" 
                                Width="280px">
                            </dnnWerk:RadComboBox>                        
                        </td>
                        <td>
                            <asp:ImageButton ID="btnAddPublicationRelation" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 20px 0 20px 0;">
                            <strong><asp:Literal ID="lblResult" runat="server"></asp:Literal></strong>
                        </td>
                    </tr>
                </table>

            </asp:Panel>
            <asp:Panel ID="pnlRemoveForm" runat="server" Visible="false">
                <table>
                    <tr>
                        <td style="padding: 20px 0 20px 0;">
                            <asp:Literal ID="lblRemove" runat="server"></asp:Literal>
                        </td>
                    </tr>  
                    <tr>
                        <td>
                            <asp:LinkButton ID="cmdRemove" runat="server" Text="Remove from Publication"></asp:LinkButton>
                        </td>
                    </tr>
                </table>          
            </asp:Panel>
        </td>
    </tr>
</table>

