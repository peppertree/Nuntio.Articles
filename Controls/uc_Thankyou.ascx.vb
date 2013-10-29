Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.UI.Navigation


Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Class uc_ThankYou
        Inherits ArticleModuleBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.hypBack.NavigateUrl = NavigateURL()
        End Sub

    End Class

End Namespace


