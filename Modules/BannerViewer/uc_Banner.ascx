<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_Banner.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_Banner" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>

<dnnWerk:RadRotator ID="lstBanners" runat="server">
    <ItemTemplate>
        <asp:Label ID="lblItem" Runat="server" Text='<%# FormatItem(DataBinder.Eval(Container.DataItem,"VendorId"),DataBinder.Eval(Container.DataItem,"BannerId"),DataBinder.Eval(Container.DataItem,"BannerTypeId"),DataBinder.Eval(Container.DataItem,"BannerName"),DataBinder.Eval(Container.DataItem,"ImageFile"),DataBinder.Eval(Container.DataItem,"Description"),DataBinder.Eval(Container.DataItem,"Url"),DataBinder.Eval(Container.DataItem,"Width"),DataBinder.Eval(Container.DataItem,"Height")) %>'></asp:Label>
    </ItemTemplate>
</dnnWerk:RadRotator>

