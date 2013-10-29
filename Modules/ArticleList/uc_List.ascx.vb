
Imports Telerik.Web.UI

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions
Imports dnnWerk.Modules.Nuntio.Articles.Helpers

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_List
        Inherits ArticleModuleBase

#Region "Private Members"

        Private _articles As List(Of ArticleInfo)
        Private _pageindex As Integer = 0
        Private _totalcount As Integer = 0
        Private _totalpages As Integer = 0
        Private _view As String = ""
        Private _year As Integer = Null.NullInteger
        Private _month As Integer = Null.NullInteger

        Public Property IsSearchResult() As Boolean
            Get
                Return Me.chkIsSearchResult.Checked
            End Get
            Set(ByVal value As Boolean)
                Me.chkIsSearchResult.Checked = value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            wndControls.IconUrl = Me.ModuleDirectory & "/nuntio.articles.extension.png"

            RegisterScripts()
            RegisterCss()
            ReadQueryString()



            If Me.EnableModuleSearch Then
                If NewsModuleId = ModuleId Then
                    Me.ctlSearch.NewsModuleID = NewsModuleId
                    Me.ctlSearch.ShowFutureItems = Me.ShowFutureItems
                    Me.ctlSearch.ShowPastItems = Me.ShowPastItems
                    Me.ctlSearch.IncludeFeaturedItems = Me.IncludeFeaturedItems
                    Me.ctlSearch.IncludeNonFeaturedItems = Me.IncludeNonFeaturedItems
                    Me.ctlSearch.IncludePublications = Me.IncludePublications
                    Me.ctlSearch.IncludeNonPublications = Me.IncludeNonPublications
                    Me.ctlSearch.UseOriginalVersion = Me.UseOriginalVersion
                    Me.ctlSearch.CurrentLocale = Me.CurrentLocale
                    Me.ctlSearch.RowCount = Me.RowCount
                Else
                    Me.ctlSearch.Visible = False
                End If
            Else
                Me.ctlSearch.Visible = False
            End If

            If Not Page.IsPostBack Then
                RenderDefaultTemplateMessage()
                SetupControl()
            End If


        End Sub

        Private Sub RenderDefaultTemplateMessage()


            If ModulePermissionController.CanAdminModule(Me.ModuleConfiguration) Then

                If Me.ArticleTheme = "Default" Then

                    Dim objLiteral As New Literal
                    objLiteral.Text = "<div class=""dnnFormMessage dnnFormInfo""><span>" & Localize("DefaultTemplateWarning") & "</span></div>"
                    ctlAjax.Controls.AddAt(0, objLiteral)

                End If
                
            End If


        End Sub

        Private Sub ctlAjax_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs) Handles ctlAjax.AjaxRequest

            SetupControl()

        End Sub

        Protected Sub Search_ResultBound(ByVal sender As Object, ByVal e As SearchResultEventArgs) Handles ctlSearch.SearchResultsBound

            If e.ContainsResult Then

                Me.IsSearchResult = True

                rptNews.DataSource = e.SearchResult
                rptNews.DataBind()

                pnlPaging.Visible = False

            Else

                Me.IsSearchResult = False
                BindArticles()

            End If

        End Sub

        Private Sub rptNews_ItemDataBound(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptNews.ItemDataBound
            Try

                If (e.Item.ItemType = ListItemType.Header) Then
                    ProcessSurroundingTemplates(e.Item.Controls, Me.HeaderTemplate)
                End If

                Dim sKey As String = ""
                If Me.IsSearchResult Then
                    sKey = ctlSearch.txtSearch.Text
                End If


                If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then

                    Dim objNews As ArticleInfo = CType(e.Item.DataItem, ArticleInfo)
                    Dim strTemplate As String = ""

                    If ArticleId <> Null.NullInteger And Not IsSearchResult Then
                        strTemplate = DetailTemplate
                    Else
                        If objNews.IsFeatured Then
                            strTemplate = FeaturedItemTemplate
                        Else
                            If e.Item.ItemIndex = 0 Then
                                strTemplate = FirstItemTemplate
                            Else
                                If e.Item.ItemType = ListItemType.AlternatingItem Then
                                    strTemplate = AlternatingItemTemplate
                                Else
                                    strTemplate = ItemTemplate
                                End If
                            End If
                        End If
                    End If

                    Dim html As String = ""
                    ProcessItemBody(html, objNews, strTemplate, sKey, True, NewsModuleTab)
                    e.Item.Controls.Add(New LiteralControl(html))

                End If

                If (e.Item.ItemType = ListItemType.Footer) Then
                    ProcessSurroundingTemplates(e.Item.Controls, Me.FooterTemplate)
                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

#Region "Caching Helpers"

        Private Sub LoadArticlesCache()

            Dim blnIsDirty As Boolean = False

            If Not DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_ISDIRTY_" & NewsModuleId.ToString) Is Nothing Then
                blnIsDirty = CType(DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_ISDIRTY_" & NewsModuleId.ToString), Boolean)
            End If

            If blnIsDirty Then
                _articles = Nothing
                Exit Sub
            End If

            If Not DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_" & CurrentLocale & "_" & ModuleId.ToString & "_" & _pageindex.ToString) Is Nothing Then
                Try
                    _articles = CType(DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_" & CurrentLocale & "_" & ModuleId.ToString & "_" & _pageindex.ToString), List(Of ArticleInfo))
                Catch
                End Try
            End If

            If Not DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_TOTAL_" & CurrentLocale & "_" & ModuleId.ToString) Is Nothing Then
                Try
                    _totalcount = CType(DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_TOTAL_" & CurrentLocale & "_" & ModuleId.ToString), Integer)
                Catch
                End Try
            End If

        End Sub

        Private Sub SetArticlesCache(ByVal Articles As List(Of ArticleInfo), ByVal TotalCount As Integer)
            DotNetNuke.Common.Utilities.DataCache.SetCache("NUNTIO_ARTICLES_LIST_" & CurrentLocale & "_" & ModuleId.ToString & "_" & _pageindex.ToString.ToString, Articles)
            DotNetNuke.Common.Utilities.DataCache.SetCache("NUNTIO_ARTICLES_TOTAL_" & CurrentLocale & "_" & ModuleId.ToString, TotalCount)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("NUNTIO_ARTICLES_ISDIRTY_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("NUNTIO_ARTICLES_ISDIRTY_" & ModuleId.ToString)
        End Sub

#End Region

#Region "Initialization"

        Private Sub RegisterScripts()

            Dim strScript As String = "<script type=""text/javascript""> " & vbCrLf
            strScript += "  function Nuntio_Refresh_" & ModuleId.ToString & "() {" & vbCrLf
            strScript += "      $find(""" & ctlAjax.ClientID & """).ajaxRequest('refresh');" & vbCrLf
            strScript += "  }" & vbCrLf
            strScript += "</script>" & vbCrLf

            'RadScriptBlock1.Controls.Add(New LiteralControl(strScript))

            Page.ClientScript.RegisterStartupScript(Me.[GetType](), "Nuntio_List_Refresh_" & ModuleId.ToString, strScript)

            DotNetNuke.Framework.AJAX.RegisterScriptManager()

        End Sub

        Private Sub RegisterCss()

            Dim strCssUrl As String = Me.ModuleDirectory & "/templates/" & ArticleTheme & "/template.css"

            Dim blnAlreadyRegistered As Boolean = False
            For Each ctrl As Control In Me.Page.Header.Controls

                If TypeOf (ctrl) Is HtmlLink Then
                    Dim ctrlCss As HtmlLink = CType(ctrl, HtmlLink)
                    If ctrlCss.Href.ToLower = strCssUrl.ToLower Then
                        blnAlreadyRegistered = True
                        Exit For
                    End If
                End If

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

        Private Sub ReadQueryString()

            If Not Request.QueryString("Page") Is Nothing Then
                Integer.TryParse(Request.QueryString("Page"), _pageindex)
            End If

            If Not Request.QueryString("View") Is Nothing Then
                _view = Request.QueryString("View")
            End If

            If Not Request.QueryString("Year") Is Nothing Then
                Integer.TryParse(Request.QueryString("Year"), _year)
            End If

            If Not Request.QueryString("Month") Is Nothing Then
                Integer.TryParse(Request.QueryString("Month"), _month)
            End If

            If Not Request.QueryString("IsSearchResult") Is Nothing Then
                Boolean.TryParse(Request.QueryString("IsSearchResult"), IsSearchResult)
            End If

        End Sub

        Private Sub SetupPaging()

            _totalpages = Convert.ToInt32(Math.Ceiling(CType(_totalcount / Me.RowCount, Double)))

            If _totalpages > 1 Then

                If Me.PagingMaxCount <> Null.NullInteger Then
                    If PagingMaxCount < _totalpages Then
                        _totalpages = PagingMaxCount
                    End If
                End If

                Me.pnlPaging.Visible = True
                Me.lblPaging.Text = String.Format(Localize("lblPagingSites.Text"), (_pageindex + 1).ToString, _totalpages.ToString)
                Me.plhPaging.Controls.Clear()

                For i As Integer = 1 To _totalpages
                    Dim cmdPage As New HyperLink
                    cmdPage.Text = i.ToString
                    If _pageindex = i - 1 Then
                        cmdPage.CssClass = "nuntio-pager"
                    Else
                        cmdPage.CssClass = "nuntio-pager-active"
                    End If
                    cmdPage.EnableViewState = True
                    cmdPage.ID = DotNetNuke.Common.Globals.CreateValidID("nuntio_pageingbutton" & i.ToString)
                    cmdPage.NavigateUrl = Navigate(True, "", "Page=" & (i - 1).ToString)
                    If Me.IsSearchResult Then
                        cmdPage.NavigateUrl = Navigate(Me.NewsModuleTab, True, "Pages.aspx", "Page=" & (i - 1).ToString, "IsSearchResult=true")
                    Else
                        cmdPage.NavigateUrl = Navigate(Me.NewsModuleTab, True, "Pages.aspx", "Page=" & (i - 1).ToString)
                    End If
                    Me.plhPaging.Controls.Add(cmdPage)
                Next

            Else
                Me.pnlPaging.Visible = False
            End If


        End Sub

#End Region

#Region "Private Methods"

        Private Sub SetupControl()


            If Not Request.QueryString("TestMode") Is Nothing Then
                If Not Request.QueryString("ModId") Is Nothing Then
                    If IsNumeric(Request.QueryString("ModId")) Then
                        If Not Request.QueryString("PortId") Is Nothing Then
                            If IsNumeric(Request.QueryString("PortId")) Then
                                If Not Request.QueryString("UseId") Is Nothing Then
                                    If IsNumeric(Request.QueryString("UseId")) Then
                                        If Not Request.QueryString("Count") Is Nothing Then
                                            If IsNumeric(Request.QueryString("Count")) Then

                                                Dim AvailableLocales As New List(Of String)
                                                Dim info As System.Globalization.CultureInfo = CType(Page, PageBase).PageCulture

                                                For Each objLocale As Locale In SupportedLocales.Values
                                                    info = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
                                                    AvailableLocales.Add(info.Name)
                                                Next

                                                Dim ac As New ArticleController
                                                ac.GenerateTestArticles(Request.QueryString("PortId"), Request.QueryString("ModId"), Request.QueryString("UseId"), Request.QueryString("Count"), AvailableLocales)

                                                Response.Redirect(NavigateURL(TabId))

                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
            If Not Me.IsSearchResult Then
                If ArticleId = Null.NullInteger Then

                    If _view.Length > 0 AndAlso CanAdmin Then

                        BindSpecialView()

                    Else

                        BindArticles()

                        If EnablePaging Then
                            Me.pnlPaging.Visible = True
                            SetupPaging()
                        Else
                            Me.pnlPaging.Visible = False
                        End If

                    End If

                Else
                    BindArticle()
                End If

            End If

        End Sub

        Private Sub BindArticle()

            Dim objNewsController As New ArticleController

            'bind single item
            Dim objNewsItem As New ArticleInfo
            objNewsItem = objNewsController.GetArticle(ArticleId, Me.CurrentLocale, UseOriginalVersion)

            If Not objNewsItem Is Nothing Then

                Dim ctrlAttachments As New ArticleFileController
                objNewsItem.Images = ctrlAttachments.GetImages(objNewsItem.ItemId, NewsModuleId, CurrentLocale, UseOriginalVersion)
                objNewsItem.Attachments = ctrlAttachments.GetAttachments(objNewsItem.ItemId, NewsModuleId, CurrentLocale, UseOriginalVersion)

                Dim articles As New List(Of ArticleInfo)
                articles.Add(objNewsItem)

                rptNews.DataSource = articles
                rptNews.DataBind()

                'update page description and title
                Dim strDescription As String = articles(0).Summary
                If String.IsNullOrEmpty(strDescription) Then
                    strDescription = articles(0).Content
                End If

                Dim count As Integer = 155
                If (StripHtml(Server.HtmlDecode(strDescription)).TrimStart().Length > count) Then
                    strDescription = Left(StripHtml(Server.HtmlDecode(strDescription)).TrimStart(), count) & "..."
                Else
                    strDescription = Left(StripHtml(Server.HtmlDecode(strDescription)).TrimStart(), count)
                End If

                Dim strTitle As String = articles(0).Title

                Me.BasePage.Description = strDescription
                Me.BasePage.Title = strTitle

                'support for Facebook meta tags
                Dim ctlTitle As New HtmlMeta
                ctlTitle.Name = "og:title"
                ctlTitle.Content = strTitle
                Page.Header.Controls.Add(ctlTitle)

                Dim ctlDescription As New HtmlMeta
                ctlDescription.Name = "og:description"
                ctlDescription.Content = strDescription
                Page.Header.Controls.Add(ctlDescription)

                Try
                    If objNewsItem.Images.Count > 0 Then
                        For Each objImage As ArticleFileInfo In objNewsItem.Images
                            If objImage.IsPrimary Then
                                Dim ctlImage As New HtmlMeta
                                ctlImage.Name = "og:image"
                                ctlImage.Content = "http://" & PortalSettings.PortalAlias.HTTPAlias & objImage.Url
                                Page.Header.Controls.Add(ctlImage)
                            End If
                        Next
                    End If
                Catch
                End Try

            End If

        End Sub

        Private Sub BindArticles()

            Dim objArticlesController As New ArticleController

            Dim blnLookupCache As Boolean = True
            Dim dtCurrent As DateTime = Date.Now
            Dim blnIncludeFutureItems As Boolean = ShowFutureItems
            Dim blnIncludePastItems As Boolean = ShowPastItems
            Dim intRows As Integer = RowCount

            If _year <> Null.NullInteger Then
                intRows = Null.NullInteger
                blnIncludeFutureItems = True
                blnIncludePastItems = True
                blnLookupCache = False
            End If

            If CategoryID <> Null.NullInteger Then
                blnLookupCache = False
            End If

            If blnLookupCache Then
                LoadArticlesCache()
            End If

            If _articles Is Nothing Then 'either hasn't been cached yet or bypass caching is enabled

                _articles = objArticlesController.GetArticlesPaged(_totalcount, NewsModuleId, Me.CurrentLocale, RowCount, _pageindex, dtCurrent, _month, _year, Me.SortOrder, Me.CategoryList, UseOriginalVersion, AuthorId, blnIncludeFutureItems, blnIncludePastItems, IncludeFeaturedItems, IncludeNonFeaturedItems, IncludePublications, IncludeNonPublications)

                If _pageindex > 0 And _articles.Count = 0 Then
                    Try
                        If CategoryID <> Null.NullInteger Then
                            Response.Redirect(NavigateURL(TabId, "", "CategoryID=" & CategoryID.ToString))
                        Else
                            Response.Redirect(NavigateURL(TabId))
                        End If
                    Catch
                    End Try
                End If

                If MakeFeaturedSticky Then

                    Dim lstFeatured As New List(Of ArticleInfo)
                    Dim lstOthers As New List(Of ArticleInfo)

                    For Each objArticle As ArticleInfo In _articles
                        If objArticle.IsFeatured Then
                            lstFeatured.Add(objArticle)
                        Else
                            lstOthers.Add(objArticle)
                        End If
                    Next

                    _articles = New List(Of ArticleInfo)

                    For Each objFeatured As ArticleInfo In lstFeatured
                        _articles.Add(objFeatured)
                    Next
                    For Each objOther As ArticleInfo In lstOthers
                        _articles.Add(objOther)
                    Next

                End If

                If blnLookupCache Then
                    SetArticlesCache(_articles, _totalcount)
                End If

            End If

            Me.rptNews.DataSource = _articles
            Me.rptNews.DataBind()

        End Sub

        Private Sub BindSpecialView()

            _articles = New List(Of ArticleInfo)

            Select Case _view.ToLower
                Case "unapproved"
                    _articles = UnapprovedArticles
                Case "expired"
                    _articles = ExpiredArticles
                Case "notyetpublished"
                    _articles = NotYetPublishedArticles
                Case "deleted"
                    _articles = DeletedArticles
            End Select

            rptNews.DataSource = _articles
            rptNews.DataBind()

        End Sub

#End Region

    End Class
End Namespace