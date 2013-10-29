Imports DotNetNuke
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization.Localization

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Search
        Inherits ArticleModuleBase

        Private _newsmoduleid As Integer
        Private _rowcount As Integer
        Private _currentlocale As String
        Private _myfilename As String = "uc_Search.ascx"

        Private _ShowFutureItems As Boolean
        Private _UseOriginalVersion As Boolean
        Private _ShowPastItems As Boolean
        Private _IncludeFeaturedItems As Boolean
        Private _IncludeNonFeaturedItems As Boolean
        Private _IncludePublications As Boolean
        Private _IncludeNonPublications As Boolean


        Public Overloads Property RowCount() As Integer
            Get
                Return _rowcount
            End Get
            Set(ByVal value As Integer)
                _rowcount = value
            End Set
        End Property

        Public Property MyFileName()
            Get
                Return _myfilename
            End Get
            Set(ByVal value)
                _myfilename = value
            End Set
        End Property

        Public Overloads Property CurrentLocale() As String
            Get
                Return _currentlocale
            End Get
            Set(ByVal value As String)
                _currentlocale = value
            End Set
        End Property

        Public Overloads Property NewsModuleID() As Integer
            Get
                Return _newsmoduleid
            End Get
            Set(ByVal value As Integer)
                _newsmoduleid = value
            End Set
        End Property

        Public Overloads Property ShowFutureItems() As Boolean
            Get
                Return _ShowFutureItems
            End Get
            Set(ByVal value As Boolean)
                _ShowFutureItems = value
            End Set
        End Property

        Public Overloads Property UseOriginalVersion() As Boolean
            Get
                Return _UseOriginalVersion
            End Get
            Set(ByVal value As Boolean)
                _UseOriginalVersion = value
            End Set
        End Property

        Public Overloads Property ShowPastItems() As Boolean
            Get
                Return _ShowPastItems
            End Get
            Set(ByVal value As Boolean)
                _ShowPastItems = value
            End Set
        End Property

        Public Overloads Property IncludeFeaturedItems() As Boolean
            Get
                Return _IncludeFeaturedItems
            End Get
            Set(ByVal value As Boolean)
                _IncludeFeaturedItems = value
            End Set
        End Property

        Public Overloads Property IncludeNonFeaturedItems() As Boolean
            Get
                Return _IncludeNonFeaturedItems
            End Get
            Set(ByVal value As Boolean)
                _IncludeNonFeaturedItems = value
            End Set
        End Property

        Public Overloads Property IncludePublications() As Boolean
            Get
                Return _IncludePublications
            End Get
            Set(ByVal value As Boolean)
                _IncludePublications = value
            End Set
        End Property

        Public Overloads Property IncludeNonPublications() As Boolean
            Get
                Return _IncludeNonPublications
            End Get
            Set(ByVal value As Boolean)
                _IncludeNonPublications = value
            End Set
        End Property

#Region "Private Methods"


#End Region

#Region "Delegates"

        Public Delegate Sub SearchResultsBoundEventHandler(ByVal sender As Object, ByVal e As SearchResultEventArgs)

#End Region

#Region "Events"

        Public Event SearchResultsBound As SearchResultsBoundEventHandler

#End Region

#Region "Event Methods"

        Public Sub OnSearchResultBound(ByVal e As SearchResultEventArgs)

            RaiseEvent SearchResultsBound(Me, e)

        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            DotNetNuke.Framework.AJAX.RegisterScriptManager()

            'register enter key
            DotNetNuke.UI.Utilities.ClientAPI.RegisterKeyCapture(txtSearch, btnSearch, Asc(vbCr))

        End Sub

        Public Function GetStyle() As String
            If Page.IsPostBack Then
                Return "display:block"
            Else
                Return "display:none"
            End If
        End Function

        Public Function GetAdvancedText() As String
            Return GetString("hypSearch", GetResourceFile(Me, MyFileName))
        End Function

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load

            Me.btnSearch.Text = GetString("cmdSearch", GetResourceFile(Me, MyFileName))
            Me.btnAllNews.Text = GetString("cmdAllNews", GetResourceFile(Me, MyFileName))
            'Me.hypSearch.Text = GetString("hypSearch", GetResourceFile(Me, MyFileName))

            If Not Page.IsPostBack Then
                Me.txtSearch.Text = GetString("Search", GetResourceFile(Me, MyFileName))
            End If
            Me.txtSearch.Attributes.Add("onclick", "this.value='';")

            Me.rblSearchMode.Items(0).Text = GetString("And", GetResourceFile(Me, MyFileName))
            Me.rblSearchMode.Items(1).Text = GetString("Or", GetResourceFile(Me, MyFileName))

            If Page.IsPostBack = False Then
                Dim ctrlArticles As New ArticleController
                Dim ctrlCategories As New CategoryController


                Me.drpAuthors.DataSource = ArticleAuthors
                Me.drpAuthors.DataBind()
                Me.drpAuthors.Items.Insert(0, New ListItem(GetString("AllAuthors", GetResourceFile(Me, MyFileName)), "-1"))

                Me.drpCategories.DataSource = ctrlCategories.ListCategoryItems(NewsModuleID, CurrentLocale, Date.Now, ShowFutureItems, ShowPastItems, UseOriginalVersion)
                Me.drpCategories.DataBind()
                Me.drpCategories.Items.Insert(0, New ListItem(GetString("AllCategories", GetResourceFile(Me, MyFileName)), "-1"))

                

                Me.drpMonth.Items.Clear()
                Me.drpYear.Items.Clear()

                Me.drpMonth.Items.Add(New ListItem(GetString("AllMonths", GetResourceFile(Me, MyFileName)), "-1"))
                Me.drpYear.Items.Add(New ListItem(GetString("AllYears", GetResourceFile(Me, MyFileName)), "-1"))

                Dim currentyear As Integer = 0
                Dim currentmonth As Integer = 0
                For Each item As ArchiveInfo In ArticlesArchive
                    If item.Year <> currentyear Then
                        Me.drpYear.Items.Add(New ListItem(item.Year.ToString))
                        currentyear = item.Year
                    End If
                    If item.Month <> currentmonth Then
                        Dim archiveDate As DateTime = New DateTime(item.Year, item.Month, item.Day)
                        Me.drpMonth.Items.Add(New ListItem(archiveDate.ToString("MMMM"), item.Month.ToString))
                        currentmonth = item.Month
                    End If
                Next

                Try
                    'Me.drpYear.Items.FindByValue(Year(Now)).Selected = True
                    'Me.drpMonth.Items.FindByValue(Month(Now)).Selected = True
                Catch
                End Try
            End If






        End Sub

        Protected Sub cmdSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click

            Dim args As New SearchResultEventArgs
            If Me.txtSearch.Text <> GetString("Search", GetResourceFile(Me, MyFileName)) Then

                args.ContainsResult = True

                Dim SearchController As New ArticleController
                Dim Count As Integer = 0
                Dim iMonth As Integer = Integer.Parse(Me.drpMonth.SelectedValue)
                Dim iYear As Integer = Integer.Parse(Me.drpYear.SelectedValue)
                Dim iAuthor As Integer = Integer.Parse(Me.drpAuthors.SelectedValue)
                Dim iCategory As Integer = Integer.Parse(Me.drpCategories.SelectedValue)
                Dim sSearch As String = Me.txtSearch.Text.Trim.Replace("'", "''")
                Dim sLocale As String = CurrentLocale
                Dim sMode As String = Me.rblSearchMode.SelectedValue
                Dim dtDate As DateTime = Null.NullDate

                args.SearchResult = SearchController.GetSearchResult(args.Total, NewsModuleID, sLocale, -1, 0, dtDate, iMonth, iYear, ArticleBase.ArticleSortOrder.publishdatedesc, iCategory, UseOriginalVersion, iAuthor, ShowFutureItems, ShowPastItems, IncludeFeaturedItems, IncludeNonFeaturedItems, IncludePublications, IncludeNonPublications, sSearch, sMode)

                If args.Total > 0 Then
                    Me.lblResult.Text = String.Format(GetString("ItemsFound", GetResourceFile(Me, MyFileName)), args.Total.ToString)
                Else
                    Me.lblResult.Text = GetString("NoItemsFound", GetResourceFile(Me, MyFileName))
                End If

                Me.pnlResult.Visible = True
                Me.btnAllNews.Visible = True

                Me.OnSearchResultBound(args)

            Else
                args.ContainsResult = False
                Me.lblResult.Text = GetString("EnterSearchText", GetResourceFile(Me, MyFileName))
                Me.lblResult.CssClass = "NormalRed"
                Me.lblResult.Visible = True
                Me.pnlResult.Visible = True
                Me.btnAllNews.Visible = False
                Me.OnSearchResultBound(args)
            End If


        End Sub



#End Region

        Protected Sub cmdAllNews_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAllNews.Click
            Try
                Response.Redirect(NavigateURL(), True)
            Catch
            End Try            
        End Sub


    End Class

    Public Class SearchResultEventArgs
        Private _SearchResult As List(Of ArticleInfo)
        Private _containsResult As Boolean
        Private _Total As Integer = 0
        Public Property Total() As Integer
            Get
                Return _Total
            End Get
            Set(ByVal value As Integer)
                _Total = value
            End Set
        End Property
        Public Property ContainsResult() As Boolean
            Get
                Return _containsResult
            End Get
            Set(ByVal value As Boolean)
                _containsResult = value
            End Set
        End Property
        Public Property SearchResult() As List(Of ArticleInfo)
            Get
                Return _SearchResult
            End Get
            Set(ByVal value As List(Of ArticleInfo))
                _SearchResult = value
            End Set
        End Property
    End Class

End Namespace
