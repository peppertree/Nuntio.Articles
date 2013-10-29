<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_FileSelect.ascx.vb" Inherits="dnnWerk.Modules.Nuntio.Articles.uc_FileSelect" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="dnnWerk" %>

<dnnWerk:RadScriptBlock ID="RadScriptBlock1" runat="server">
<script type="text/javascript">
        //A function that will return a reference to the parent radWindow in case the page is loaded in a RadWindow object
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }
        function OnFileLinkSelect(sender, args)
        {
            var item = args.get_item();

            //If file (and not a folder) is selected - call the OnFileSelected method on the parent page
            if (item.get_type() == Telerik.Web.UI.FileExplorerItemType.File)
            {
                args.set_cancel(true);
                //Get a reference to the opener parent page using rad window
                var wnd = GetRadWindow();
                var openerPage = wnd.BrowserWindow;
                //if you need the URL for the item, use get_url() instead of get_path()
                openerPage.OnFileSelected(wnd, item.get_path(), item.get_name(), item.get_extension());
                //Close window
                wnd.close();
            }
        }

        function OnGalleryImageSelect(sender, args) {
            var item = args.get_item();

            //If file (and not a folder) is selected - call the OnFileSelected method on the parent page
            if (item.get_type() == Telerik.Web.UI.FileExplorerItemType.File) {
                args.set_cancel(true);
                //Get a reference to the opener parent page using rad window
                var wnd = GetRadWindow();
                var openerPage = wnd.BrowserWindow;
                //if you need the URL for the item, use get_url() instead of get_path()
                openerPage.OnGalleryImageSelected(wnd, item.get_path(), item.get_name(), item.get_extension());
                //Close window
                wnd.close();
            }
        }

        function SetGalleryImage(sender, args) {
            var selectedItem = args.get_value();
            var path = args.value.getAttribute("src", 2);
            var wnd = GetRadWindow();
            var openerPage = wnd.BrowserWindow;
            openerPage.GalleryImageSelected(path);
            wnd.close();
        }

</script>
</dnnWerk:RadScriptBlock>

<dnnWerk:RadFileExplorer ID="ctlFiles" runat="server" Width="600px" Height="400px" Skin="Default">
    <Configuration maxuploadfilesize="20480000"></Configuration>
</dnnWerk:RadFileExplorer>

