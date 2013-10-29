Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.UI.Navigation


Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Class uc_Detail
        Inherits ArticleModuleBase

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            BindCss()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim objNewsController As New ArticleController
            Dim objNewsItem As New ArticleInfo

            objNewsItem = objNewsController.GetArticle(ArticleId, Me.CurrentLocale, UseOriginalVersion)

            If Not objNewsItem Is Nothing Then

                Dim html As String = ""
                ProcessItemBody(html, objNewsItem, Me.DetailTemplate, "", True, NewsModuleTab)
                plhControls.Controls.Add(New LiteralControl(html))

            Else

                Me.ctlAjax.ResponseScripts.Add("CloseWindow();")

            End If

        End Sub

        Private Sub BindCss()

            Dim strCssUrl As String = Me.ModuleDirectory & "/templates/" & ArticleTheme & "/template.css"

            Dim blnAlreadyRegistered As Boolean = False
            For Each ctrl As Control In Me.Page.Header.Controls

                Try
                    Dim ctrlCss As HtmlLink = CType(ctrl, HtmlLink)
                    If ctrlCss.Href.ToLower = strCssUrl.ToLower Then
                        blnAlreadyRegistered = True
                        Exit For
                    End If
                Catch ex As Exception
                    'skip registration
                End Try

            Next

            If Not blnAlreadyRegistered Then

                Dim ctrlLink As New HtmlLink
                ctrlLink.Href = strCssUrl
                ctrlLink.Attributes.Add("rel", "stylesheet")
                ctrlLink.Attributes.Add("type", "text/css")
                ctrlLink.Attributes.Add("media", "screen")

                Me.Page.Header.Controls.Add(ctrlLink)

            End If

        End Sub

    End Class

End Namespace


