
Imports Telerik.Web.UI
Imports dnnWerk
Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Rotate
        Inherits ArticleModuleBase

#Region "Private Members"

        Private _lstNews As List(Of ArticleInfo)

        Public Property lstNews() As List(Of ArticleInfo)
            Get
                Return _lstNews
            End Get
            Set(ByVal value As List(Of ArticleInfo))
                _lstNews = value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            BindCss()

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Page.IsPostBack Then
                BindNews()
            End If
        End Sub

        Private Sub ctlRotate_ItemDataBound(ByVal sender As Object, ByVal e As RadRotatorEventArgs) Handles ctlRotate.ItemDataBound
            Try
                Dim html As String = ""
                Me.ProcessItemBody(html, CType(e.Item.DataItem, ArticleInfo), Me.ScrollingTemplate, "", True, NewsModuleTab)
                e.Item.Controls.Add(New LiteralControl(html))
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Private Methods"

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

        Private Sub BindNews()

            Dim objNewsController As New ArticleController

            Dim FromDate As DateTime = Nothing

            FromDate = Now

            'bind multiple items
            Dim arrNews As New ArrayList

            Dim month As Integer = Null.NullInteger
            Dim year As Integer = Null.NullInteger
            Dim rowstoreturn As Integer = RowCount

            If Not Request.QueryString("Month") Is Nothing Then
                month = Integer.Parse(Request.QueryString("Month"))
                rowstoreturn = Null.NullInteger
            End If
            If Not Request.QueryString("Year") Is Nothing Then
                year = Integer.Parse(Request.QueryString("Year"))
                rowstoreturn = Null.NullInteger
            End If

            'show multiple items static
            Me.lstNews = objNewsController.GetArticlesPaged(New Integer, NewsModuleId, Me.CurrentLocale, RowCount, 0, FromDate, month, year, Me.SortOrder, Me.CategoryList, UseOriginalVersion, AuthorId, ShowFutureItems, ShowPastItems, IncludeFeaturedItems, IncludeNonFeaturedItems, IncludePublications, IncludeNonPublications)

            Me.BindRepeater()


        End Sub

        Private Sub BindRepeater()


            ctlRotate.DataSource = Me.lstNews
            'ctlRotate.RotatorType = RotatorType.SlideShow
            ctlRotate.ScrollDirection = ScrollDirection
            ctlRotate.ScrollDuration = ScrollSpeed
            ctlRotate.FrameDuration = ScrollTimeOut
            'ctlRotate.FramesToShow = Me.ScrollFrames
            If Me.ScrollWidth.EndsWith("%") Then
                Try
                    Dim intWidth As Integer = Integer.Parse(ScrollWidth.Replace("%", "").Trim)
                    ctlRotate.Width = Unit.Percentage(intWidth)
                Catch
                    ctlRotate.Width = Unit.Percentage(100)
                End Try
            ElseIf Me.ScrollWidth.EndsWith("px") Then
                Try
                    Dim intWidth As Integer = Integer.Parse(ScrollWidth.ToLower.Replace("px", "").Trim)
                    ctlRotate.Width = Unit.Pixel(intWidth)
                Catch
                    ctlRotate.Width = Unit.Pixel(400)
                End Try
            End If
            If Me.ScrollHeight.EndsWith("%") Then
                Try
                    Dim intHeight As Integer = Integer.Parse(ScrollHeight.Replace("%", "").Trim)
                    ctlRotate.Height = Unit.Percentage(intHeight)
                Catch
                    ctlRotate.Height = Unit.Percentage(100)
                End Try
            ElseIf Me.ScrollHeight.EndsWith("px") Then
                Try
                    Dim intHeight As Integer = Integer.Parse(ScrollHeight.ToLower.Replace("px", "").Trim)
                    ctlRotate.Height = Unit.Pixel(intHeight)
                Catch
                    ctlRotate.Height = Unit.Pixel(300)
                End Try
            End If

            'AddHandler ctlRotate.ItemDataBound, AddressOf Me.ctlRotate_ItemDataBound
            ctlRotate.DataBind()

        End Sub

#End Region


    End Class
End Namespace
