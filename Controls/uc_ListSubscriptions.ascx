<%@ Control Language="vb" AutoEventWireup="true" CodeBehind="uc_ListSubscriptions.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_ListSubscriptions" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<Telerik:RadAjaxPanel ID="ctlAjax" runat="server" LoadingPanelID="lpAjax" EnablePageHeadUpdate="false">


<p><asp:dropdownlist id="cboLocale" runat="server" AutoPostBack="True"></asp:dropdownlist></p>

<telerik:RadGrid ID="grdSubscriptions" runat="server" Skin="Default">
    <MasterTableView AutoGenerateColumns="false" DataKeyNames="ItemId">
        <Columns>
            <telerik:GridDateTimeColumn HeaderText="Subscribed" DataField="DateCreated"></telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn HeaderText="E-Mail" DataField="Email"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Name" DataField="Name"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Key" DataField="Key" ReadOnly="true"></telerik:GridBoundColumn>
            <telerik:GridCheckBoxColumn HeaderText="Verified" DataField="Verified"></telerik:GridCheckBoxColumn>
            <telerik:GridTemplateColumn>
                <ItemTemplate>
                    <asp:LinkButton ID="cmdDelete" runat="server" Text="Delete" OnClick="cmdDelete_Click" CommandArgument='<%# Databinder.Eval(Container.DataItem, "ItemId") %>'></asp:LinkButton>&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="cmdApprove" runat="server" Text="Approve" OnClick="cmdApprove_Click" CommandArgument='<%# Databinder.Eval(Container.DataItem, "ItemId") %>' Visible='<%# Databinder.Eval(Container.DataItem, "Verified") = false %>'></asp:LinkButton>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridEditCommandColumn EditText="Edit" CancelText="Cancel" UpdateText="Update"></telerik:GridEditCommandColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>

</Telerik:RadAjaxPanel>
<Telerik:RadAjaxLoadingPanel ID="lpAjax" runat="server" Transparency="20" Skin="Black"></Telerik:RadAjaxLoadingPanel>

<br /><br />
<table>
    <tr>
        <td colspan="2"><h2>Import from CSV</h2></td>
    </tr>
    <tr>
        <td>
            <asp:FileUpload ID="ctlImported" runat="server" />        
        </td>
        <td>
            <asp:Button ID="btnUpload" runat="server" Text="Import" />
        </td>
    </tr>
    <tr>
        <td colspan="2"><asp:Label ID="lblImportResult" runat="server"></asp:Label></td>
    </tr>    
</table>


