
Imports Telerik.Web.UI

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions
Imports dnnWerk.Modules.Nuntio.Articles.Helpers

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_ThreadedList
        Inherits ArticleModuleBase

#Region "Private Members"

        Private _enableAjaxBrowsing As Boolean = True


        Private _articles As List(Of ArticleInfo)
        Private _lstSearchResult As List(Of ArticleInfo)
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

#Region "Event Handlers"

        Private Sub SetNodeTexts()

            For Each node As RadTreeNode In CategoriesTree.GetAllNodes

                If Not IsArticleNode(node) Then

                    Dim strText As String = GetCategoryName(node)

                    Dim intArticles As Integer = GetArticleCount(node)
                    If intArticles > 0 Then
                        strText += " (" & intArticles.ToString & ")"
                    End If

                    node.Text = strText

                End If

            Next

        End Sub

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            wndControls.IconUrl = Me.ModuleDirectory & "/nuntio.articles.extension.png"

            Dim strScript As String = "<script type=""text/javascript""> " & vbCrLf
            strScript += "  function Nuntio_Refresh_" & ModuleId.ToString & "() {" & vbCrLf
            strScript += "      $find(""" & ctlAjax.ClientID & """).ajaxRequest('refresh,0');" & vbCrLf
            strScript += "  }" & vbCrLf
            strScript += "</script>" & vbCrLf

            'RadScriptBlock1.Controls.Add(New LiteralControl(strScript))

            Page.ClientScript.RegisterStartupScript(Me.[GetType](), "Nuntio_List_Refresh_" & ModuleId.ToString, strScript)

            DotNetNuke.Framework.AJAX.RegisterScriptManager()

            BindCss()
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
                    Me.dvSearch.Visible = False
                End If
            Else
                Me.ctlSearch.Visible = False
                Me.dvSearch.Visible = False
            End If

            If Not Page.IsPostBack Then
                BindData()
            End If

            If Not CanAdmin Then
                CategoriesTree.ContextMenus(0).Visible = False
                CategoriesTree.ContextMenus(1).Visible = False
                ctlTools.Items(1).Visible = False
                ctlTools.Items(2).Visible = False
            End If


        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ctlSplitContainer.Width = Me.SplitterWidth
            ctlSplitContainer.Height = Me.SplitterHeight
            ctlToc.Width = Me.TocWidth
            ctlContent.Width = Me.ContentWidth

            LocalizeForm()

        End Sub

        Private Sub CategoriesTree_NodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles CategoriesTree.NodeClick

            If IsArticleNode(e.Node) Then
                Dim id As Integer = GetItemIdFromNode(e.Node)
                If id <> Null.NullInteger Then
                    RenderArticle(id)
                End If
            End If

        End Sub

        Private Sub ctlAjax_AjaxRequest(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs) Handles ctlAjax.AjaxRequest

            Dim Id As Integer = Convert.ToInt32(e.Argument.Split(Char.Parse(","))(1))

            Select Case e.Argument.Split(Char.Parse(","))(0)
                Case "Category_Delete"

                    DeleteCategory(Id)
                    BindData()

                Case "Article_Delete"

                    DeleteArticle(Id)
                    BindData()

                Case "refresh"

                    BindData()

            End Select

        End Sub

        Private Sub ctlTools_ButtonClick(ByVal sender As Object, ByVal e As RadToolBarEventArgs) Handles ctlTools.ButtonClick
            Select Case CType(e.Item, RadToolBarButton).CommandName
                Case "expandcollapse" 'expand / collapse

                    If IsTreeViewCollapsed() Then
                        For Each node As RadTreeNode In CategoriesTree.GetAllNodes
                            If Not node.ParentNode Is Nothing Then
                                node.ParentNode.Expanded = True
                            End If
                        Next
                        e.Item.Text = "Collapse All"
                        e.Item.ImageUrl = Me.ModuleDirectory & "/images/zoom_out.png"
                    Else
                        For Each node As RadTreeNode In CategoriesTree.GetAllNodes
                            If Not node.ParentNode Is Nothing Then
                                node.ParentNode.Expanded = False
                            End If
                        Next
                        e.Item.Text = "Expand All"
                        e.Item.ImageUrl = Me.ModuleDirectory & "/images/zoom_in.png"
                    End If

                Case "addarticle" 'add article

                    Dim node As RadTreeNode = CategoriesTree.SelectedNode
                    If node Is Nothing Then
                        Me.ctlAjax.ResponseScripts.Add("javascript:NuntioArticlesAddForm(" & ModuleId.ToString & "," & NewsModuleId.ToString & ", -1)")
                    Else
                        Dim blnIsArticleNode As Boolean = False
                        If node.HasAttributes Then
                            If Not node.Attributes("IsArticleNode") Is Nothing Then
                                If Boolean.Parse(node.Attributes("IsArticleNode")) = True Then
                                    blnIsArticleNode = True
                                End If
                            End If
                        End If

                        Dim intCategory As Integer = CategoryID
                        If blnIsArticleNode Then
                            If Not node.ParentNode Is Nothing Then
                                intCategory = Convert.ToInt32(node.ParentNode.Value)
                            End If
                        End If

                        Me.ctlAjax.ResponseScripts.Add("javascript:NuntioArticlesAddForm(" & ModuleId.ToString & "," & NewsModuleId.ToString & ", " & intCategory.ToString & ")")

                    End If


                Case "deleteselected" 'delete

                    Dim ctrlArticles As New ArticleController

                    For Each node As RadTreeNode In CategoriesTree.CheckedNodes

                        Dim blnIsArticle As Boolean = False
                        Dim id As Integer = Integer.Parse(node.Value)

                        If node.HasAttributes Then
                            If Not node.Attributes("IsArticleNode") Is Nothing Then
                                If Boolean.Parse(node.Attributes("IsArticleNode")) = True Then
                                    blnIsArticle = True
                                End If
                            End If
                        End If

                        If blnIsArticle Then
                            ctrlArticles.DeleteArticle(id)                            
                        Else
                            DeleteCategory(id)
                        End If

                    Next

                    ClearArticleCache()
                    BindData()

                Case "view_unapproved"
                    Try
                        Response.Redirect(NavigateURL(TabId, "", "view=unapproved"))
                    Catch
                    End Try


                Case "view_expired"
                    Try
                        Response.Redirect(NavigateURL(TabId, "", "view=expired"))
                    Catch
                    End Try


                Case "view_notyetpublished"
                    Try
                        Response.Redirect(NavigateURL(TabId, "", "view=notyetpublished"))
                    Catch
                    End Try


            End Select
        End Sub

        Protected Sub Search_ResultBound(ByVal sender As Object, ByVal e As SearchResultEventArgs) Handles ctlSearch.SearchResultsBound

            If e.ContainsResult Then

                Me.IsSearchResult = True
                _pageindex = 0
                _lstSearchResult = e.SearchResult
                _totalcount = e.Total

                BindArticles()
                'SetupTree()
                BindSearchResultToTree()
                'RenderListResult(_lstSearchResult)

            Else

                Me.IsSearchResult = False
                BindArticles()

            End If



        End Sub

#End Region

#Region "Tree Binding Methods"

        Private Sub AddChildCategories(ByRef ParentNode As RadTreeNode, ByVal Categories As List(Of CategoryInfo))
            For Each Category As CategoryInfo In Categories

                If Category.ParentID = Null.NullInteger Then
                    If ParentNode.Value = "" Then
                        Dim NewNode As RadTreeNode = New RadTreeNode(Category.CategoryName, Category.CategoryID.ToString)
                        NewNode.Attributes.Add("ArticleCount", Category.Count)
                        NewNode.Attributes.Add("CategoryName", Category.CategoryName)
                        'NewNode.NavigateUrl = Navigate(Me.TabId, False, OnlyAlphaNumericChars(Category.CategoryName) & ".aspx", "CategoryID=" & Category.CategoryID.ToString)
                        If Me.ShowFolderIconsInCategories Then
                            NewNode.ImageUrl = Me.ImagesDirectory & "nuntio_folder.gif"
                            NewNode.ExpandedImageUrl = Me.ImagesDirectory & "nuntio_folderopen.gif"
                        End If
                        NewNode.PostBack = False
                        NewNode.ContextMenuID = "CategoriesMenu"
                        ParentNode.Nodes.Add(NewNode)
                        AddChildCategories(NewNode, Categories)
                        AddArticlesToNode(Category.CategoryID, NewNode)

                    End If
                Else
                    If ParentNode.Value <> "" Then
                        If Category.ParentID = Integer.Parse(ParentNode.Value) Then
                            Dim NewNode As RadTreeNode = New RadTreeNode(Category.CategoryName, Category.CategoryID.ToString)
                            'NewNode.NavigateUrl = Navigate(Me.TabId, False, OnlyAlphaNumericChars(Category.CategoryName) & ".aspx", "CategoryID=" & Category.CategoryID.ToString)
                            If Me.ShowFolderIconsInCategories Then
                                NewNode.ImageUrl = Me.ImagesDirectory & "nuntio_folder.gif"
                                NewNode.ExpandedImageUrl = Me.ImagesDirectory & "nuntio_folderopen.gif"
                            End If
                            NewNode.PostBack = False
                            NewNode.ContextMenuID = "CategoriesMenu"
                            ParentNode.Nodes.Add(NewNode)
                            AddChildCategories(NewNode, Categories)
                            AddArticlesToNode(Category.CategoryID, NewNode)
                        End If
                    End If
                End If

            Next
        End Sub

        Private Sub AddArticlesToNode(ByVal CategoryId As Integer, ByRef Node As RadTreeNode)

            Dim strArticles As String = ""

            If _articles.Count > 0 Then

                For Each oArticle As ArticleInfo In _articles

                    Dim blnAdd As Boolean = False

                    If oArticle.Categories Is Nothing Then
                        blnAdd = (CategoryId = Null.NullInteger)
                    Else
                        For Each iCategory As Integer In oArticle.Categories
                            If iCategory = CategoryId Then
                                blnAdd = True
                                Exit For
                            End If
                        Next
                    End If

                    If blnAdd Then

                        Dim ArticleUrl As String = Navigate(TabId, True, OnlyAlphaNumericChars(oArticle.Title) & ".aspx", "CategoryId=" & CategoryId.ToString, "ArticleId=" & oArticle.ItemId)

                        Dim ArticleNode As New RadTreeNode(oArticle.Title, oArticle.ItemId)
                        ArticleNode.ImageUrl = GetArticleIconPath(oArticle)
                        ArticleNode.Attributes.Add("IsArticleNode", Boolean.TrueString)
                        ArticleNode.Value = oArticle.ItemId.ToString
                        ArticleNode.ContextMenuID = "ArticlesMenu"

                        If Not Node Is Nothing Then

                            If _enableAjaxBrowsing Then

                                ArticleNode.PostBack = True
                                ArticleNode.Attributes.Add("PostBackNode", Boolean.TrueString)
                                ArticleNode.NavigateUrl = Nothing

                            Else

                                ArticleNode.PostBack = False
                                ArticleNode.Attributes.Add("PostBackNode", Boolean.FalseString)
                                ArticleNode.NavigateUrl = ArticleUrl

                            End If

                            Node.Nodes.Add(ArticleNode)

                        Else

                            Dim categories As New List(Of Integer)
                            Dim cc As New CategoryController
                            categories = cc.GetRelationsByitemId(oArticle.ItemId)
                            If categories.Count = 0 Then
                                CategoriesTree.Nodes.Add(ArticleNode)
                            End If

                        End If

                    End If

                Next

            End If

        End Sub

#Region "Tree Helpers"

        Private Function GetItemIdFromNode(ByVal node As RadTreeNode) As Integer

            Dim id As Integer = Null.NullInteger

            If IsNumeric(node.Value) Then
                id = Integer.Parse(node.Value)
            End If

            Return id

        End Function

        Private Function IsArticleNode(ByVal Node As RadTreeNode) As Boolean

            If Node.HasAttributes Then
                If Not Node.Attributes("IsArticleNode") Is Nothing Then
                    If Boolean.Parse(Node.Attributes("IsArticleNode")) = True Then
                        Return True
                    End If
                End If
            End If

            Return False

        End Function

        Private Function GetArticleCount(ByVal Node As RadTreeNode) As Integer

            If Node.HasAttributes Then
                If Not Node.Attributes("ArticleCount") Is Nothing Then
                    If IsNumeric(Node.Attributes("ArticleCount")) Then
                        Return Convert.ToInt32(Node.Attributes("ArticleCount"))
                    End If
                End If
            End If

            Return 0

        End Function

        Private Function GetCategoryName(ByVal Node As RadTreeNode) As String

            If Node.HasAttributes Then
                If Not Node.Attributes("CategoryName") Is Nothing Then
                    Return Node.Attributes("CategoryName").ToString
                End If
            End If

            Return Node.Text

        End Function

        Private Function IsTreeViewExpanded() As Boolean

            For Each node As RadTreeNode In CategoriesTree.GetAllNodes
                If node.Expanded Then
                    Return True
                End If
            Next

            Return False

        End Function

        Private Function IsTreeViewCollapsed() As Boolean

            For Each node As RadTreeNode In CategoriesTree.GetAllNodes
                If node.Expanded Then
                    Return False
                End If
            Next

            Return True

        End Function

        Private Sub UnselectAllNodes()
            Dim node As RadTreeNode
            For Each node In CategoriesTree.GetAllNodes()
                node.Selected = False
            Next node
        End Sub

#End Region

#End Region

#Region "Private Methods"

        Private Sub SelectArticleNode(ByVal ArticleId As Integer)

            Dim node As RadTreeNode
            For Each node In Me.CategoriesTree.GetAllNodes()
                If IsArticleNode(node) Then
                    If GetItemIdFromNode(node) = ArticleId Then
                        node.ExpandParentNodes()
                        node.Selected = True
                        Exit For
                    End If
                End If
            Next

        End Sub

        Private Sub SelectCategoryNode(ByVal CategoryId As Integer)

            Dim node As RadTreeNode
            For Each node In Me.CategoriesTree.GetAllNodes()
                If Not IsArticleNode(node) Then
                    If GetItemIdFromNode(node) = CategoryId Then
                        node.ExpandParentNodes()
                        node.Selected = True
                        Exit For
                    End If
                End If
            Next

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

        Private Sub BindData()

            UnselectAllNodes()
            BindArticles()
            SetupTree()

            If ArticleId <> Null.NullInteger Then
                SelectArticleNode(ArticleId)
            Else
                If CategoryID <> Null.NullInteger Then
                    SelectCategoryNode(CategoryID)
                End If
            End If

            If IsTreeViewCollapsed() Then
                ctlTools.Items(0).Text = "Expand All"
                ctlTools.Items(0).ImageUrl = Me.ModuleDirectory & "/images/zoom_in.png"
            Else
                ctlTools.Items(0).Text = "Collapse All"
                ctlTools.Items(0).ImageUrl = Me.ModuleDirectory & "/images/zoom_out.png"
            End If

            If ArticleId <> Null.NullInteger Then
                RenderArticle(ArticleId)
            Else
                If _view.Length > 0 AndAlso CanAdmin Then
                    BindSpecialView()
                Else
                    If WelcomeArticleId <> Null.NullInteger Then
                        RenderArticle(WelcomeArticleId)
                    Else
                        RenderListResult(_articles)
                    End If
                End If
            End If

        End Sub

        Private Sub BindCss()

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

        Private Sub RenderArticle(ByVal ArticleId As Integer)

            Dim objNewsController As New ArticleController

            'bind single item
            Dim objNewsItem As New ArticleInfo
            objNewsItem = objNewsController.GetArticle(ArticleId, Me.CurrentLocale, UseOriginalVersion)
            If Not objNewsItem Is Nothing Then

                Dim blnIframe As Boolean = False
                If objNewsItem.Url <> "" Then

                    If ShowLinksInline Then
                        blnIframe = True
                        ctlFrame.Attributes.Add("src", FormatURL(objNewsItem.Url, False))
                    End If

                End If

                If blnIframe Then

                    plhItem.Visible = False
                    ctlFrame.Visible = True

                Else

                    ctlFrame.Visible = False
                    plhItem.Visible = True

                    Dim html As String = ""
                    ProcessItemBody(html, objNewsItem, DetailTemplate, "", True, TabId)
                    plhItem.Controls.Add(New LiteralControl(html))

                End If


            End If

        End Sub

        Private Sub BindSearchResultToTree()

            If _lstSearchResult.Count > 0 Then

                CategoriesTree.Nodes.Clear()

                Dim SearchRoot As RadTreeNode = New RadTreeNode("Search Result")
                CategoriesTree.Nodes.Add(SearchRoot)

                For Each oArticle As ArticleInfo In _lstSearchResult

                    Dim ArticleUrl As String = Navigate(TabId, True, OnlyAlphaNumericChars(oArticle.Title) & ".aspx", "CategoryId=" & CategoryID.ToString, "ArticleId=" & oArticle.ItemId)

                    Dim ArticleNode As New RadTreeNode(oArticle.Title, oArticle.ItemId)
                    ArticleNode.ImageUrl = GetArticleIconPath(oArticle)
                    ArticleNode.Attributes.Add("IsArticleNode", Boolean.TrueString)
                    ArticleNode.Value = oArticle.ItemId.ToString
                    ArticleNode.ContextMenuID = "ArticlesMenu"

                    If Not SearchRoot Is Nothing Then

                        If _enableAjaxBrowsing Then

                            ArticleNode.PostBack = True
                            ArticleNode.Attributes.Add("PostBackNode", Boolean.TrueString)
                            ArticleNode.NavigateUrl = Nothing

                        Else

                            ArticleNode.PostBack = False
                            ArticleNode.Attributes.Add("PostBackNode", Boolean.FalseString)
                            ArticleNode.NavigateUrl = ArticleUrl

                        End If

                        SearchRoot.Nodes.Add(ArticleNode)

                    End If

                Next

            End If

            CategoriesTree.ExpandAllNodes()


        End Sub

        Private Sub RenderListResult(ByVal lst As List(Of ArticleInfo))

            ProcessSurroundingTemplates(Me.plhItem.Controls, HeaderTemplate)
            Dim i As Integer = 0
            For Each oArticle As ArticleInfo In lst

                i += 1
                'Dim ctrlImages As New ImageController
                'oArticle.Images = ctrlImages.GetImages(oArticle.ItemId, NewsModuleId, CurrentLocale, UseOriginalVersion)

                Dim strTemplate As String = ""
                Select Case i
                    Case 1
                        strTemplate = ItemTemplate
                    Case 2
                        strTemplate = AlternatingItemTemplate
                End Select

                Dim html As String = ""
                ProcessItemBody(html, oArticle, strTemplate, "", True, TabId)
                plhItem.Controls.Add(New LiteralControl(html))

                If i = 2 Then
                    i = 0
                End If

            Next
            ProcessSurroundingTemplates(Me.plhItem.Controls, FooterTemplate)

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
            End Select

        End Sub

        Private Sub SetupTree()

            CategoriesTree.Nodes.Clear()

            If CanAdmin Then
                Me.CategoriesTree.CheckBoxes = True
            End If

            Dim Categories As New List(Of CategoryInfo)
            Dim cc As New CategoryController
            Categories = cc.ListCategoryItems(NewsModuleId, Me.CurrentLocale, Date.Now, ShowFutureItems, ShowPastItems, (UseOriginalVersion))

            AddArticlesToNode(Null.NullInteger, Nothing)

            For Each Category As CategoryInfo In Categories
                If Category.ParentID = Null.NullInteger Then

                    Dim NewNode As RadTreeNode = New RadTreeNode(Category.CategoryName, Category.CategoryID.ToString)
                    NewNode.Attributes.Add("ArticleCount", Category.Count)
                    NewNode.Attributes.Add("CategoryName", Category.CategoryName)
                    'NewNode.NavigateUrl = Navigate(Me.TabId, False, OnlyAlphaNumericChars(Category.CategoryName) & ".aspx", "CategoryID=" & Category.CategoryID.ToString)

                    If Me.ShowFolderIconsInCategories Then
                        NewNode.ImageUrl = Me.ImagesDirectory & "nuntio_folder.gif"
                        NewNode.ExpandedImageUrl = Me.ImagesDirectory & "nuntio_folderopen.gif"
                    End If

                    NewNode.PostBack = False
                    NewNode.ContextMenuID = "CategoriesMenu"

                    AddChildCategories(NewNode, Categories)
                    AddArticlesToNode(Category.CategoryID, NewNode)

                    Me.CategoriesTree.Nodes.Add(NewNode)

                End If
            Next

        End Sub

        Private Sub LocalizeForm()

            ctlTools.Items(0).Text = "Expand All"
            ctlTools.Items(0).ImageUrl = Me.ModuleDirectory & "/images/zoom_in.png"
            ctlTools.Items(1).Text = "Add Article"
            ctlTools.Items(1).ImageUrl = Me.ModuleDirectory & "/images/nuntio_add.png"
            ctlTools.Items(2).Text = "Delete Selected"
            ctlTools.Items(2).ImageUrl = Me.ModuleDirectory & "/images/nuntio_delete.png"
            ctlTools.Items(3).Text = "View"
            ctlTools.Items(3).ImageUrl = Me.ModuleDirectory & "/images/zoom.png"

            If CanAdmin Then

                CType(ctlTools.Items(3), RadToolBarDropDown).Buttons(0).Text = "Waiting for approval (" & UnapprovedArticlesCount.ToString & ")"
                CType(ctlTools.Items(3), RadToolBarDropDown).Buttons(0).ImageUrl = Me.ModuleDirectory & "/images/nuntio_clock.png"
                CType(ctlTools.Items(3), RadToolBarDropDown).Buttons(1).Text = "Expired Articles (" & ExpiredArticleCount.ToString & ")"
                CType(ctlTools.Items(3), RadToolBarDropDown).Buttons(1).ImageUrl = Me.ModuleDirectory & "/images/nuntio_remove.png"
                CType(ctlTools.Items(3), RadToolBarDropDown).Buttons(2).Text = "Not yet published (" & NotYetPublishedArticleCount.ToString & ")"
                CType(ctlTools.Items(3), RadToolBarDropDown).Buttons(2).ImageUrl = Me.ModuleDirectory & "/images/nuntio_calendar.png"

            Else

                ctlTools.Items(3).Visible = False

            End If

            'contextmenus
            Me.CategoriesTree.ContextMenus(0).Items(0).Text = Localize("EditCategory.Menu")
            Me.CategoriesTree.ContextMenus(0).Items(0).ImageUrl = Me.ModuleDirectory & "/images/nuntio_edit.png"
            Me.CategoriesTree.ContextMenus(0).Items(1).Text = Localize("DeleteCategory.Menu")
            Me.CategoriesTree.ContextMenus(0).Items(1).ImageUrl = Me.ModuleDirectory & "/images/nuntio_delete.png"
            Me.CategoriesTree.ContextMenus(0).Items(2).Text = Localize("ADDARTICLE.Action")
            Me.CategoriesTree.ContextMenus(0).Items(2).ImageUrl = Me.ModuleDirectory & "/images/nuntio_add.png"

            Me.CategoriesTree.ContextMenus(1).Items(0).Text = Localize("EDITARTICLE.Action")
            Me.CategoriesTree.ContextMenus(1).Items(0).ImageUrl = Me.ModuleDirectory & "/images/nuntio_edit.png"
            Me.CategoriesTree.ContextMenus(1).Items(1).Text = Localize("DELETEARTICLE.Action")
            Me.CategoriesTree.ContextMenus(1).Items(1).ImageUrl = Me.ModuleDirectory & "/images/nuntio_delete.png"

        End Sub

#End Region

#Region "CRUD Methods"

        Private Sub DeleteArticle(ByVal Id As Integer)

            Dim ctrl As New ArticleController
            ctrl.DeleteArticle(Id)

        End Sub

        Private Sub DeleteCategory(ByVal CategoryId As Integer)

            Dim cc As New CategoryController
            Dim categories As New List(Of CategoryInfo)
            categories = cc.ListCategoryItems(NewsModuleId, CurrentLocale, Date.Now, True, True, True)
            DeleteChildCategories(CategoryId, categories)
            cc.DeleteCategory(CategoryId, NewsModuleId)
            cc.DeleteRelationByCategoryId(CategoryId)

        End Sub

        Private Sub DeleteChildCategories(ByVal ParentId As Integer, ByVal Categories As List(Of CategoryInfo))
            Dim cc As New CategoryController
            For Each oCat As CategoryInfo In Categories
                If oCat.ParentID = ParentId Then
                    DeleteChildCategories(oCat.CategoryID, Categories)
                    cc.DeleteCategory(oCat.CategoryID, Me.NewsModuleId)
                    cc.DeleteRelationByCategoryId(oCat.CategoryID)
                End If
            Next
        End Sub

#End Region

    End Class

End Namespace
