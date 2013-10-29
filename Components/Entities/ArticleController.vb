Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Xml
Imports System.Security.Cryptography
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Search
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.FileSystem
Imports dnnWerk.Libraries.Nuntio.Localization

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class ArticleController

        Implements ISearchable
        Implements IPortable

#Region "For testing purposes"

        Public Sub GenerateTestArticles(PortalId As Integer, ModuleId As Integer, UserId As Integer, Count As Integer, Locales As List(Of String))



            For i As Integer = 0 To Count - 1



                Dim ArticleId As Integer = Null.NullInteger
                Dim ImageId As Integer = Null.NullInteger

                For l As Integer = 0 To Locales.Count - 1

                    Dim strContent As String = "<p>" & Locales(l) & " - " & i.ToString & " - Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.</p><p>At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.</p>"
                    Dim strSummary As String = "<p>" & Locales(l) & " - " & i.ToString & " - Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.</p>"
                    Dim strTitle As String = i.ToString & " - Consetetur Sadipscing Elitr" & " (" & Locales(l) & ")"

                    Dim objArticle As ArticleInfo = Nothing

                    If ArticleId = Null.NullInteger Then
                        objArticle = New ArticleInfo
                    Else
                        objArticle = GetArticle(ArticleId, Locales(l), True)
                    End If

                    objArticle.CreatedByUser = UserId
                    objArticle.CreatedDate = Date.Now
                    objArticle.ApprovedBy = UserId
                    objArticle.ApprovedDate = Date.Now
                    objArticle.IsApproved = True
                    objArticle.IsFeatured = False
                    objArticle.Summary = strSummary
                    objArticle.Content = strContent
                    objArticle.Title = strTitle
                    objArticle.PublishDate = Date.Now
                    objArticle.ExpiryDate = Null.NullDate
                    objArticle.IsNotified = True
                    objArticle.IsOriginal = (l = 0)
                    objArticle.Locale = Locales(l)
                    objArticle.moduleId = ModuleId
                    objArticle.PortalID = PortalId
                    objArticle.ViewOrder = 1
                    objArticle.Url = ""
                    objArticle.LastUpdatedBy = UserId
                    objArticle.LastUpdatedDate = Date.Now
                    objArticle.ReviewDate = Null.NullDate
                    objArticle.ParentId = Null.NullInteger
                    objArticle.IsPublication = False

                    If ArticleId = Null.NullInteger Then
                        ArticleId = AddArticle(objArticle)
                    Else
                        UpdateArticle(objArticle)
                    End If


                    'Dim objImage As New ImageInfo

                    'If ImageId <> Null.NullInteger Then
                    '    objImage.ImageId = ImageId
                    'End If

                    'objImage.ImageDescription = "Image Description"
                    'objImage.ImageTitle = "Image Title  "
                    'objImage.IsPrimary = True
                    'objImage.ArticleId = ArticleId
                    'objImage.Locale = Locales(l)

                    'Dim folderid As Integer = FolderManager.Instance.GetFolder(PortalId, "").FolderID
                    'objImage.FileId = FileManager.Instance.GetFile(PortalId, "tmp.png").FileId

                    'Dim ctrl As New ImageController

                    'If ImageId <> Null.NullInteger Then
                    '    ctrl.UpdateImage(objImage, ModuleId, PortalId, UserId, False)
                    'Else
                    '    ImageId = ctrl.AddImage(objImage, ModuleId, PortalId, UserId, True)
                    'End If

                Next 'l

            Next 'i

        End Sub

#End Region
#Region "Public CRUD Methods"

        Public Function GetRelatedArticles(ByVal ArticleId As Integer, ByVal StartDate As DateTime, ByVal Locale As String) As List(Of ArticleInfo)

            Dim articles As New List(Of ArticleInfo)
            articles = LocalizeArticleList(CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetRelatedArticles", ArticleId, GetNull(StartDate))), Locale, True, False)

            Return articles

        End Function

        Public Sub UpdateArticleRelation(ByVal ArticleId As Integer, ByVal RelatedArticles As List(Of Integer))

            Dim articles As New List(Of ArticleInfo)
            articles = CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetRelatedArticles", ArticleId, GetNull(Null.NullDate)))

            For Each oArticle As ArticleInfo In articles

                Dim blnIsRelated As Boolean = False
                For Each iRelated As Integer In RelatedArticles
                    If iRelated = oArticle.ItemId Then
                        blnIsRelated = True
                        Exit For
                    End If
                Next

                If blnIsRelated = False Then
                    DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_DeleteArticleRelation", ArticleId, oArticle.ItemId)
                End If

            Next

            For Each iRelated As Integer In RelatedArticles

                Dim blnIsRelated As Boolean = False

                For Each oArticle As ArticleInfo In articles
                    If oArticle.ItemId = iRelated Then
                        blnIsRelated = True
                        Exit For
                    End If
                Next

                If blnIsRelated = False Then
                    DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_AddArticleRelation", ArticleId, iRelated)
                End If

            Next

        End Sub

        Public Function AddArticle(ByVal objArticle As ArticleInfo) As Integer

            Dim itemId As Integer = Null.NullInteger

            itemId = CType(DotNetNuke.Data.DataProvider.Instance.ExecuteScalar("NuntioArticles_AddArticle", objArticle.moduleId, objArticle.PortalID, objArticle.CreatedByUser, objArticle.CreatedDate, objArticle.PublishDate, GetNull(objArticle.ExpiryDate), GetNull(objArticle.ViewOrder), objArticle.IsNotified, objArticle.IsApproved, GetNull(objArticle.ApprovedBy), GetNull(objArticle.ApprovedDate), objArticle.IsFeatured, objArticle.CreatedByUser, objArticle.CreatedDate, GetNull(objArticle.ParentId), GetNull(objArticle.ReviewDate), objArticle.IsPublication, objArticle.AnchorLink), Integer)

            If itemId <> Null.NullInteger Then
                objArticle.ItemId = itemId
                UpdateArticleLocalization(objArticle)
            End If

            Return itemId

        End Function

        Public Sub UpdateArticle(ByVal objArticle As ArticleInfo)

            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("NuntioArticles_UpdateArticle", objArticle.ItemId, objArticle.moduleId, objArticle.PortalID, objArticle.CreatedByUser, objArticle.CreatedDate, objArticle.PublishDate, GetNull(objArticle.ExpiryDate), GetNull(objArticle.ViewOrder), objArticle.IsNotified, objArticle.IsApproved, GetNull(objArticle.ApprovedBy), GetNull(objArticle.ApprovedDate), objArticle.IsFeatured, objArticle.LastUpdatedBy, objArticle.LastUpdatedDate, GetNull(objArticle.ParentId), GetNull(objArticle.ReviewDate), objArticle.IsPublication, objArticle.AnchorLink)

            Dim blnContentChanged As Boolean = True

            Dim objCompare As ArticleInfo = GetArticle(objArticle.ItemId, objArticle.Locale, False)
            If Not objCompare Is Nothing Then
                If objCompare.Title = objArticle.Title AndAlso objCompare.Content = objArticle.Content AndAlso objCompare.Summary = objArticle.Summary AndAlso objCompare.Url = objArticle.Url Then
                    blnContentChanged = False
                End If
            End If

            If blnContentChanged Then
                Me.UpdateArticleLocalization(objArticle)
            End If        

        End Sub

        Public Sub DeleteArticle(ByVal ArticleId As Integer)

            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("NuntioArticles_SoftDeleteArticle", ArticleId)

        End Sub

        Public Sub RestoreArticle(ByVal ArticleId As Integer)

            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("NuntioArticles_RestoreArticle", ArticleId)

        End Sub

        Public Sub HardDeleteArticle(ByVal ArticleId As Integer, ByVal ModuleId As Integer)

            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("NuntioArticles_DeleteArticle", ArticleId, ModuleId)

        End Sub

        Public Function GetArticle(ByVal ArticleId As Integer, ByVal Locale As String, ByVal UseFallback As Boolean) As ArticleInfo

            Dim objArticle As ArticleInfo = Nothing
            objArticle = CBO.FillObject(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetArticle", ArticleId))

            If objArticle.IsApproved = False Then
                LocalizeArticle(objArticle, Locale, UseFallback, True)
            Else
                LocalizeArticle(objArticle, Locale, UseFallback, False)
            End If

            If Not objArticle Is Nothing AndAlso objArticle.IsLocaleTranslated Then

                If objArticle.IsPublication Then
                    objArticle.ChildArticles = GetArticlesInPublication(ArticleId, objArticle.moduleId, False, False, Date.Now, Locale)
                End If

                objArticle.RelatedArticles = GetRelatedArticles(ArticleId, Date.Now, Locale)

                Return objArticle
            End If

            Return Nothing

        End Function

        Public Function GetArticle(ByVal ArticleId As Integer, ByVal Locale As String, ByVal UseFallback As Boolean, ByVal IncludeUnapproved As Boolean, ByVal IncludeExpired As Boolean) As ArticleInfo

            Dim objArticle As ArticleInfo = Nothing
            objArticle = CBO.FillObject(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetArticle", ArticleId))

            If objArticle.IsApproved = False Then
                LocalizeArticle(objArticle, Locale, UseFallback, True)
            Else
                LocalizeArticle(objArticle, Locale, UseFallback, False)
            End If

            If Not objArticle Is Nothing AndAlso objArticle.IsLocaleTranslated Then

                If objArticle.IsPublication Then
                    objArticle.ChildArticles = GetArticlesInPublication(ArticleId, objArticle.moduleId, IncludeUnapproved, IncludeExpired, Date.Now, Locale)
                End If

                objArticle.RelatedArticles = GetRelatedArticles(ArticleId, Date.Now, Locale)

                Return objArticle
            End If

            Return Nothing

        End Function

        Public Function GetSearchResult(ByRef ArticleCount As Integer, _
                                         ByVal ModuleId As Integer, _
                                         ByVal Locale As String, _
                                         ByVal PageSize As Integer, _
                                         ByVal PageIndex As Integer, _
                                         ByVal StartDate As Date, _
                                         ByVal Month As Integer, _
                                         ByVal Year As Integer, _
                                         ByVal SortOrder As ArticleBase.ArticleSortOrder, _
                                         ByVal CategoryID As Integer, _
                                         ByVal UseFallback As Boolean, _
                                         ByVal AuthorId As Integer, _
                                         ByVal IncludePublishDateInFuture As Boolean, _
                                         ByVal IncludePublishDateInPast As Boolean, _
                                         ByVal IncludeFeatured As Boolean, _
                                         ByVal IncludeNonFeatured As Boolean, _
                                         ByVal IncludePublications As Boolean, _
                                         ByVal IncludeNonPublications As Boolean, _
                                         ByVal SearchKey As String, _
                                         ByVal SearchSeparator As String) As List(Of ArticleInfo)

            Dim strCondition As String = ""
            For Each sWord As String In SearchKey.Split(Char.Parse(" "))

                If sWord.ToLower.Contains("ö") Or sWord.ToLower.Contains("ä") Or sWord.ToLower.Contains("ü") Or sWord.ToLower.Contains("ß") Then

                    Dim sEncodedWord As String = sWord.ToLower.Replace("ö", "&amp;ouml;").Replace("ä", "&amp;auml;").Replace("ü", "&amp;uuml;").Replace("ß", "&amp;szlig;")
                    Dim strConditionPart As String = "([Content] Like '%{0}%' OR [Content] Like '%{1}%') {2} "
                    strCondition += String.Format(strConditionPart, sWord, sEncodedWord, SearchSeparator)

                Else

                    strCondition += "Content Like '%" & sWord & "%' " & SearchSeparator & " "

                End If
            Next

            If strCondition.EndsWith(" " & SearchSeparator & " ") Then
                strCondition = strCondition.Substring(0, strCondition.LastIndexOf(" " & SearchSeparator & " "))
            End If

            Dim articles As New List(Of ArticleInfo)

            Dim xmlCategories As String = Null.NullString

            Dim strSort As String = "publishdatedesc"
            Select Case SortOrder
                Case ArticleBase.ArticleSortOrder.authorasc
                    strSort = "authorasc"
                Case ArticleBase.ArticleSortOrder.authordesc
                    strSort = "authordesc"
                Case ArticleBase.ArticleSortOrder.publishdateasc
                    strSort = "publishdateasc"
                Case ArticleBase.ArticleSortOrder.publishdatedesc
                    strSort = "publishdatedesc"
                Case ArticleBase.ArticleSortOrder.titleasc
                    strSort = "titleasc"
                Case ArticleBase.ArticleSortOrder.titledesc
                    strSort = "titledesc"
            End Select

            If CategoryID <> Null.NullInteger Then
                xmlCategories = "<Categories>"
                xmlCategories += "<Category CategoryId=""" & CategoryID.ToString & """ />"
                xmlCategories += "</Categories>"
            End If

            articles = LocalizeArticleList(CBO.FillCollection(Of ArticleInfo) _
                        (DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetSearchResult", _
                            GetNull(ModuleId), _
                            PageSize, _
                            PageIndex, _
                            GetNull(StartDate), _
                            GetNull(Month), _
                            GetNull(Year), _
                            GetNull(xmlCategories), _
                            strSort, _
                            GetNull(AuthorId), _
                            IncludePublishDateInFuture, _
                            IncludePublishDateInPast, _
                            IncludeFeatured, _
                            IncludeNonFeatured, _
                            IncludePublications, _
                            IncludeNonPublications, _
                            Locale, _
                            strCondition), _
                        ArticleCount), Locale, UseFallback, False)

            Return articles

        End Function

        Public Function GetArticles(ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal IncludeNotApproved As Boolean, ByVal IncludeExpired As Boolean, ByVal StartDate As DateTime, ByVal Localize As Boolean, ByVal Locale As String) As List(Of ArticleInfo)

            Dim articles As New List(Of ArticleInfo)
            articles = CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetArticles", GetNull(ModuleId), GetNull(PortalId), IncludeNotApproved, IncludeExpired, GetNull(StartDate)))

            If Localize Then
                Dim localizedarticles As New List(Of ArticleInfo)
                localizedarticles = LocalizeArticleList(articles, Locale, True, False)
                articles = localizedarticles
            End If

            Return articles

        End Function

        Public Function GetArticlesInPublication(ByVal PublicationId As Integer, ByVal ModuleId As Integer, ByVal IncludeNotApproved As Boolean, ByVal IncludeExpired As Boolean, ByVal StartDate As DateTime, ByVal Locale As String) As List(Of ArticleInfo)

            Dim articles As New List(Of ArticleInfo)
            articles = LocalizeArticleList(CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetArticlesInPublication", PublicationId, GetNull(ModuleId), IncludeNotApproved, IncludeExpired, GetNull(StartDate))), Locale, True, False)

            Return articles

        End Function

        Public Function GetArticles(ByVal ModuleId As Integer, _
                                    ByVal Locale As String, _
                                    ByVal StartDate As Date, _
                                    ByVal Month As Integer, _
                                    ByVal Year As Integer, _
                                    ByVal SortOrder As ArticleBase.ArticleSortOrder, _
                                    ByVal Categories As List(Of Integer), _
                                    ByVal UseFallback As Boolean, _
                                    ByVal AuthorId As Integer, _
                                    ByVal IncludePublishDateInFuture As Boolean, _
                                    ByVal IncludePublishDateInPast As Boolean, _
                                    ByVal IncludeFeatured As Boolean, _
                                    ByVal IncludeNonFeatured As Boolean, _
                                    ByVal IncludePublications As Boolean, _
                                    ByVal IncludeNonPublications As Boolean) As List(Of ArticleInfo)

            Return GetArticlesPaged(New Integer, ModuleId, Locale, -1, 0, StartDate, Month, Year, SortOrder, Categories, UseFallback, AuthorId, IncludePublishDateInFuture, IncludePublishDateInPast, IncludeFeatured, IncludeNonFeatured, IncludePublications, IncludeNonPublications)

        End Function

        Public Function GetArticlesPaged(ByRef ArticleCount As Integer, _
                                         ByVal ModuleId As Integer, _
                                         ByVal Locale As String, _
                                         ByVal PageSize As Integer, _
                                         ByVal PageIndex As Integer, _
                                         ByVal StartDate As Date, _
                                         ByVal Month As Integer, _
                                         ByVal Year As Integer, _
                                         ByVal SortOrder As ArticleBase.ArticleSortOrder, _
                                         ByVal Categories As List(Of Integer), _
                                         ByVal UseFallback As Boolean, _
                                         ByVal AuthorId As Integer, _
                                         ByVal IncludePublishDateInFuture As Boolean, _
                                         ByVal IncludePublishDateInPast As Boolean, _
                                         ByVal IncludeFeatured As Boolean, _
                                         ByVal IncludeNonFeatured As Boolean, _
                                         ByVal IncludePublications As Boolean, _
                                         ByVal IncludeNonPublications As Boolean) As List(Of ArticleInfo)

            Dim xml As String = Null.NullString

            Dim strSort As String = "publishdatedesc"
            Select Case SortOrder
                Case ArticleBase.ArticleSortOrder.authorasc
                    strSort = "authorasc"
                Case ArticleBase.ArticleSortOrder.authordesc
                    strSort = "authordesc"
                Case ArticleBase.ArticleSortOrder.publishdateasc
                    strSort = "publishdateasc"
                Case ArticleBase.ArticleSortOrder.publishdatedesc
                    strSort = "publishdatedesc"
                Case ArticleBase.ArticleSortOrder.titleasc
                    strSort = "titleasc"
                Case ArticleBase.ArticleSortOrder.titledesc
                    strSort = "titledesc"
            End Select

            If Categories.Count > 0 Then
                xml = "<Categories>"
                For Each CategoryId As Integer In Categories
                    xml += "<Category CategoryId=""" & CategoryId.ToString & """ />"
                Next
                xml += "</Categories>"
            End If

            Dim articles As New List(Of ArticleInfo)
            articles = LocalizeArticleList(CBO.FillCollection(Of ArticleInfo) _
                        (DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetArticlesPaged", _
                            ModuleId, _
                            PageSize, _
                            PageIndex, _
                            GetNull(StartDate), _
                            GetNull(Month), _
                            GetNull(Year), _
                            GetNull(xml), _
                            strSort, _
                            GetNull(AuthorId), _
                            IncludePublishDateInFuture, _
                            IncludePublishDateInPast, _
                            IncludeFeatured, _
                            IncludeNonFeatured, _
                            IncludePublications, _
                            IncludeNonPublications, _
                            Locale), _
                        ArticleCount), Locale, UseFallback, False)

            Return articles

        End Function

        Public Function GetArticlesForNotificationQueue(ByVal ModuleId As Integer, ByVal StartDate As DateTime) As List(Of ArticleInfo)

            Dim articles As New List(Of ArticleInfo)
            articles = CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetArticlesForSubscriptions", StartDate))

            Return articles

        End Function

        Public Function GetUnapprovedArticles(ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal Locale As String) As List(Of ArticleInfo)

            Dim articles As New List(Of ArticleInfo)
            articles = LocalizeArticleList(CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetUnapprovedArticles", ModuleId, PortalId)), Locale, True, True)

            Return articles

        End Function

        Public Function GetUnapprovedArticleCount(ByVal ModuleId As Integer, ByVal PortalId As Integer) As Integer

            Dim articles As New List(Of ArticleInfo)
            articles = CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetUnapprovedArticles", ModuleId, PortalId))

            Return articles.Count

        End Function

        Public Function GetExpiredArticles(ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal Locale As String) As List(Of ArticleInfo)

            Dim articles As New List(Of ArticleInfo)
            articles = LocalizeArticleList(CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetExpiredArticles", ModuleId, PortalId, Date.Now)), Locale, True, False)

            Return articles

        End Function

        Public Function GetExpiredArticleCount(ByVal ModuleId As Integer, ByVal PortalId As Integer) As Integer

            Dim articles As New List(Of ArticleInfo)
            articles = CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetExpiredArticles", ModuleId, PortalId, Date.Now))

            Return articles.Count

        End Function

        Public Function GetNotYetPublished(ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal Locale As String) As List(Of ArticleInfo)

            Dim articles As New List(Of ArticleInfo)
            articles = LocalizeArticleList(CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetUnpublishedArticles", ModuleId, PortalId, Date.Now)), Locale, True, False)

            Return articles

        End Function

        Public Function GetNotYetPublishedCount(ByVal ModuleId As Integer, ByVal PortalId As Integer) As Integer

            Dim articles As New List(Of ArticleInfo)
            articles = CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetUnpublishedArticles", ModuleId, PortalId, Date.Now))

            Return articles.Count

        End Function

        Public Function GetFeatured(ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal Locale As String, ByVal UseFallback As Boolean) As List(Of ArticleInfo)

            'todo: SPROC
            Dim articles As New List(Of ArticleInfo)
            articles = LocalizeArticleList(CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetFeaturedArticles", ModuleId, PortalId)), Locale, UseFallback, False)

            Return articles

        End Function

        Public Function GetFeaturedCount(ByVal ModuleId As Integer, ByVal PortalId As Integer) As Integer

            'todo: SPROC
            Dim articles As New List(Of ArticleInfo)
            articles = CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetFeaturedArticles", ModuleId, PortalId))

            Return articles.Count

        End Function

        Public Function GetNeedsReviewing(ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal Locale As String, ByVal SelectedDate As DateTime) As List(Of ArticleInfo)

            Dim articles As New List(Of ArticleInfo)
            articles = LocalizeArticleList(CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetNeedsReviewing", ModuleId, PortalId, SelectedDate)), Locale, True, False)

            Return articles

        End Function

        Public Function GetNeedsReviewingCount(ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal SelectedDate As DateTime) As Integer

            Dim articles As New List(Of ArticleInfo)
            articles = CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetNeedsReviewing", ModuleId, PortalId, SelectedDate))

            Return articles.Count

        End Function

        Public Function GetDeleted(ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal Locale As String) As List(Of ArticleInfo)

            Dim articles As New List(Of ArticleInfo)
            articles = LocalizeArticleList(CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetDeleted", ModuleId, PortalId)), Locale, True, False)

            Return articles

        End Function

        Public Function GetDeletedCount(ByVal ModuleId As Integer, ByVal PortalId As Integer) As Integer

            Dim articles As New List(Of ArticleInfo)
            articles = CBO.FillCollection(Of ArticleInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetDeleted", ModuleId, PortalId))

            Return articles.Count

        End Function

#End Region

#Region "Localization Helpers"

        Private Sub LocalizeArticle(ByRef objArticle As ArticleInfo, ByVal Locale As String, ByVal UseFallback As Boolean, ByVal IncludeUnapproved As Boolean)

            If Not objArticle Is Nothing Then

                objArticle.IsLocaleTranslated = False

                Dim ctrl As New ContentController(objArticle.moduleId, Locale, IncludeUnapproved)

                If Not Locale Is Nothing Then

                    Dim value As String = ""
                    Dim isoriginal As Boolean = False

                    value = ctrl.GetContentByKey(objArticle.ItemId, "CONTENT", isoriginal)
                    If Not String.IsNullOrEmpty(value) Then
                        If value.Trim = "&lt;p&gt;&amp;#160;&lt;/p&gt;" Or value.Trim = "&lt;div&gt;&amp;#160;&lt;/div&gt;" Then
                            objArticle.Content = ""
                            objArticle.IsOriginal = isoriginal
                        Else
                            objArticle.Content = value
                        End If
                    Else
                        objArticle.Content = ""
                    End If

                    value = ctrl.GetContentByKey(objArticle.ItemId, "SUMMARY", isoriginal)
                    If Not String.IsNullOrEmpty(value) Then
                        If value.Trim = "&lt;p&gt;&amp;#160;&lt;/p&gt;" Or value.Trim = "&lt;div&gt;&amp;#160;&lt;/div&gt;" Then
                            objArticle.Summary = ""
                            objArticle.IsOriginal = isoriginal
                        Else
                            objArticle.Summary = value
                        End If
                    Else
                        objArticle.Summary = ""
                    End If

                    value = ctrl.GetContentByKey(objArticle.ItemId, "URL", isoriginal)
                    If Not String.IsNullOrEmpty(value) Then
                        objArticle.Url = value
                    Else
                        objArticle.Url = ""
                    End If

                    value = ctrl.GetContentByKey(objArticle.ItemId, "TITLE", isoriginal)
                    If Not String.IsNullOrEmpty(value) Then
                        objArticle.Title = value
                        objArticle.IsOriginal = isoriginal
                    Else
                        objArticle.Title = ""
                    End If

                    value = ctrl.GetContentByKey(objArticle.ParentId, "TITLE", isoriginal)
                    If Not String.IsNullOrEmpty(value) Then
                        objArticle.PublicationTitle = value
                        objArticle.IsOriginal = isoriginal
                    Else
                        objArticle.PublicationTitle = ""
                    End If


                End If

                If objArticle.Content = "" Or objArticle.Title = "" Then
                    If UseFallback Then

                        Dim strFallbackLocale As String = ""
                        Dim intArticleId As Integer = objArticle.ItemId
                        Dim lstContent As List(Of ContentInfo) = ctrl.GetAllContent

                        If lstContent.Count > 0 Then

                            Dim objOriginalItem As ContentInfo = lstContent.Find(Function(x) (x.IsOriginal = True And x.SourceItemId = intArticleId And x.Key = "TITLE"))
                            If Not objOriginalItem Is Nothing Then
                                strFallbackLocale = objOriginalItem.Locale
                            End If

                            If strFallbackLocale <> "" Then
                                LocalizeArticle(objArticle, strFallbackLocale, False, IncludeUnapproved)
                            End If

                        End If

                    End If
                Else

                    objArticle.IsLocaleTranslated = True
                    objArticle.Locale = Locale

                End If

            End If

        End Sub

        Private Function LocalizeArticleList(ByVal Articles As List(Of ArticleInfo), ByVal Locale As String, ByVal UseFallback As Boolean, ByVal IncludeUnapproved As Boolean) As List(Of ArticleInfo)

            Dim listout As New List(Of ArticleInfo)

            For Each objArticle As ArticleInfo In Articles

                LocalizeArticle(objArticle, Locale, UseFallback, IncludeUnapproved)

                If objArticle.IsLocaleTranslated Then
                    listout.Add(objArticle)
                End If

            Next

            Return listout

        End Function

        Private Sub UpdateArticleLocalization(ByVal objArticle As ArticleInfo)

            Dim ctrl As New ContentController(objArticle.moduleId, objArticle.Locale, False)

            ctrl.UpdateContent("TITLE", objArticle.Title, objArticle.PortalID, objArticle.ApprovedBy, objArticle.ApprovedDate, objArticle.LastUpdatedBy, objArticle.LastUpdatedDate, True, objArticle.IsOriginal, objArticle.ItemId)
            ctrl.UpdateContent("SUMMARY", objArticle.Summary, objArticle.PortalID, objArticle.ApprovedBy, objArticle.ApprovedDate, objArticle.LastUpdatedBy, objArticle.LastUpdatedDate, True, objArticle.IsOriginal, objArticle.ItemId)
            ctrl.UpdateContent("CONTENT", objArticle.Content, objArticle.PortalID, objArticle.ApprovedBy, objArticle.ApprovedDate, objArticle.LastUpdatedBy, objArticle.LastUpdatedDate, True, objArticle.IsOriginal, objArticle.ItemId)
            ctrl.UpdateContent("URL", objArticle.Url, objArticle.PortalID, objArticle.ApprovedBy, objArticle.ApprovedDate, objArticle.LastUpdatedBy, objArticle.LastUpdatedDate, True, objArticle.IsOriginal, objArticle.ItemId)

        End Sub

#End Region

#Region "Optional Interfaces"

        Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) As SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems

            Dim SearchItemCollection As New SearchItemInfoCollection

            'added support to only get results from modules that have search support enabled
            Dim blnProceed As Boolean = True
            Dim blnUseFallBack As Boolean = True

            Try
                Dim modcontroller As New ModuleController
                Dim settings As Hashtable = modcontroller.GetModuleSettings(ModInfo.ModuleID)

                If CType(settings("NewsModuleId"), String) <> "" Then
                    If CType(settings("NewsModuleId"), Integer) <> ModInfo.ModuleID Then
                        blnProceed = False 'only search in central article list module instances
                    End If
                End If

                If CType(settings("UseOriginalVersion"), String) <> "" Then
                    blnUseFallBack = CType(settings("UseOriginalVersion"), Boolean)
                End If

                If blnProceed Then
                    If settings.Contains("EnableDnnSearch") Then
                        If CType(settings("EnableDnnSearch"), Boolean) = False Then
                            blnProceed = False
                        End If
                    End If
                End If

            Catch
            End Try

            If blnProceed Then

                Dim locale As String = ""
                'fixed to only index enabled locales
                Dim ctrl As New LocaleController
                Dim SupportedLocales As Dictionary(Of String, Locale) = ctrl.GetLocales(GetPortalSettings.PortalId)
                For Each objLocale As Locale In SupportedLocales.Values

                    Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
                    locale = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name).ToUpper
                    Dim arrNews As List(Of ArticleInfo) = GetArticles(ModInfo.ModuleID, ModInfo.PortalID, False, False, Date.Now, True, locale)
                    If arrNews.Count <> 0 Then
                        For Each NewsItem As ArticleInfo In arrNews
                            Dim SearchItem As SearchItemInfo = Nothing

                            'get searchable content
                            Dim sContent As String = HtmlUtils.Clean(HttpUtility.HtmlDecode(NewsItem.Title & " " & NewsItem.Summary & " " & NewsItem.Content), True)

                            'get a description
                            Dim sDescription As String = ""

                            If NewsItem.Summary <> "" Then
                                sDescription = HttpUtility.HtmlDecode(NewsItem.Summary)
                            Else
                                sDescription = HttpUtility.HtmlDecode(NewsItem.Content)
                            End If

                            sDescription = HtmlUtils.Clean(sDescription, False)

                            sDescription = HtmlUtils.Shorten(sDescription, 200, "...")

                            sDescription = sDescription.Trim

                            'get the search title
                            Dim sTitle As String = ModInfo.ModuleTitle & ": " & NewsItem.Title

                            'add tosearch collection
                            SearchItem = New SearchItemInfo(sTitle, sDescription, NewsItem.CreatedByUser, NewsItem.PublishDate, ModInfo.ModuleID, "ArticleId=" & NewsItem.ItemId.ToString, sContent, "ArticleId=" & NewsItem.ItemId.ToString)
                            SearchItemCollection.Add(SearchItem)
                        Next
                    End If
                Next

            End If

            Return SearchItemCollection

        End Function

        Public Function ExportModule(ByVal ModuleId As Integer) As String Implements Entities.Modules.IPortable.ExportModule


            Dim strXML As String = ""
            Dim locale As String = ""
            Dim ctrlLocales As New LocaleController
            Dim SupportedLocales As Dictionary(Of String, Locale) = ctrlLocales.GetLocales(GetPortalSettings.PortalId)

            strXML += vbNewLine & "<Articles>" & vbNewLine
            Dim arrNews As List(Of ArticleInfo) = GetArticles(ModuleId, Null.NullInteger, True, True, Date.Now, False, "")
            If arrNews.Count > 0 Then

                Dim news As ArticleInfo
                For Each news In arrNews
                    strXML += "   <Article>" & vbNewLine

                    strXML += "      <ViewOrder>" & XmlUtils.XMLEncode(news.ViewOrder.ToString) & "</ViewOrder>" & vbNewLine
                    strXML += "      <PublishDate>" & XmlUtils.XMLEncode(news.PublishDate.ToString) & "</PublishDate>" & vbNewLine
                    strXML += "      <ExpiryDate>" & XmlUtils.XMLEncode(news.ExpiryDate.ToString) & "</ExpiryDate>" & vbNewLine
                    strXML += "      <ReviewDate>" & XmlUtils.XMLEncode(news.ExpiryDate.ToString) & "</ReviewDate>" & vbNewLine
                    strXML += "      <IsNotified>" & XmlUtils.XMLEncode(news.IsNotified.ToString) & "</IsNotified>" & vbNewLine
                    strXML += "      <IsApproved>" & XmlUtils.XMLEncode(news.IsApproved.ToString) & "</IsApproved>" & vbNewLine
                    strXML += "      <IsFeatured>" & XmlUtils.XMLEncode(news.IsFeatured.ToString) & "</IsFeatured>" & vbNewLine
                    strXML += "      <IsOriginal>" & XmlUtils.XMLEncode(news.IsOriginal.ToString) & "</IsOriginal>" & vbNewLine
                    strXML += "      <ApprovedBy>" & XmlUtils.XMLEncode(GetExportedUser(news.ApprovedBy, news.PortalID)) & "</ApprovedBy>" & vbNewLine
                    strXML += "      <ApprovedDate>" & XmlUtils.XMLEncode(news.ApprovedDate.ToString) & "</ApprovedDate>" & vbNewLine
                    strXML += "      <CreatedByUser>" & XmlUtils.XMLEncode(GetExportedUser(news.CreatedByUser, news.PortalID)) & "</CreatedByUser>" & vbNewLine
                    strXML += "      <CreatedDate>" & XmlUtils.XMLEncode(news.CreatedDate.ToString) & "</CreatedDate>" & vbNewLine
                    strXML += "      <LastUpdatedDate>" & XmlUtils.XMLEncode(news.LastUpdatedDate.ToString) & "</LastUpdatedDate>" & vbNewLine
                    strXML += "      <LastUpdatedBy>" & XmlUtils.XMLEncode(GetExportedUser(news.LastUpdatedBy, news.PortalID)) & "</LastUpdatedBy>" & vbNewLine
                    strXML += "      <Comments>" & vbNewLine

                    Dim lstComments As New List(Of CommentInfo)
                    Dim ctrlComments As New CommentController
                    lstComments = ctrlComments.List(news.ItemId)

                    For Each objComment As CommentInfo In lstComments
                        strXML += "         <CommentInfo>" & vbNewLine
                        strXML += "            <ApprovedBy>" & XmlUtils.XMLEncode(GetExportedUser(objComment.ApprovedBy, news.PortalID)) & "</ApprovedBy>" & vbNewLine
                        strXML += "            <Comment>" & XmlUtils.XMLEncode(objComment.Comment.ToString) & "</Comment>" & vbNewLine
                        strXML += "            <CreatedBy>" & XmlUtils.XMLEncode(GetExportedUser(objComment.CreatedBy, news.PortalID)) & "</CreatedBy>" & vbNewLine
                        strXML += "            <CreatedDate>" & XmlUtils.XMLEncode(objComment.CreatedDate.ToString) & "</CreatedDate>" & vbNewLine
                        strXML += "            <Displayname>" & XmlUtils.XMLEncode(objComment.Displayname.ToString) & "</Displayname>" & vbNewLine
                        strXML += "            <Email>" & XmlUtils.XMLEncode(objComment.Email.ToString) & "</Email>" & vbNewLine
                        strXML += "            <IsApproved>" & XmlUtils.XMLEncode(objComment.IsApproved.ToString) & "</IsApproved>" & vbNewLine
                        strXML += "            <ItemId>" & XmlUtils.XMLEncode(objComment.ItemId.ToString) & "</ItemId>" & vbNewLine
                        strXML += "            <LoadGravatar>" & XmlUtils.XMLEncode(objComment.LoadGravatar.ToString) & "</LoadGravatar>" & vbNewLine
                        strXML += "            <ArticleId>" & XmlUtils.XMLEncode(objComment.ArticleId.ToString) & "</ArticleId>" & vbNewLine
                        strXML += "            <RemoteAddress>" & XmlUtils.XMLEncode(objComment.RemoteAddress.ToString) & "</RemoteAddress>" & vbNewLine
                        strXML += "         </CommentInfo>" & vbNewLine
                    Next

                    strXML += "      </Comments>" & vbNewLine

                    strXML += "      <ArticleFiles>" & vbNewLine
                    Dim lstAttachments As New List(Of ArticleFileInfo)
                    Dim ctrlImages As New ArticleFileController
                    lstAttachments = ctrlImages.GetImages(news.ItemId, news.moduleId, "", False)
                    For Each objFile As ArticleFileInfo In lstAttachments
                        strXML += "         <ArticleFileInfo>" & vbNewLine
                        strXML += "            <ArticleFileId>" & XmlUtils.XMLEncode(GetPathFromFileId(objFile.FileId, news.PortalID)) & "</ArticleFileId>" & vbNewLine
                        strXML += "            <IsPrimary>" & XmlUtils.XMLEncode(objFile.IsPrimary.ToString) & "</IsPrimary>" & vbNewLine
                        strXML += "            <IsImage>" & XmlUtils.XMLEncode(objFile.IsImage.ToString) & "</IsImage>" & vbNewLine
                        strXML += "            <Extension>" & XmlUtils.XMLEncode(objFile.Extension.ToString) & "</Extension>" & vbNewLine
                        strXML += "            <Localizations>" & vbNewLine
                        For Each objLocale As Locale In SupportedLocales.Values
                            strXML += "               <LocalizedContent>" & vbNewLine
                            Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
                            locale = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name).ToUpper
                            strXML += "                  <Locale>" & XmlUtils.XMLEncode(locale) & "</Locale>" & vbNewLine

                            Dim ctrl As New ContentController(ModuleId, locale, False)

                            Dim value As String = ""
                            Dim isoriginal As Boolean = False
                            Try
                                value = ctrl.GetContentByKey(objFile.ArticleFileId, "IMAGETITLE", isoriginal)
                                If Not String.IsNullOrEmpty(value) Then
                                    strXML += "                  <IsOriginal>" & XmlUtils.XMLEncode(isoriginal.ToString) & "</IsOriginal>" & vbNewLine
                                    strXML += "                  <Title>" & XmlUtils.XMLEncode(value) & "</Title>" & vbNewLine
                                Else
                                    strXML += "                  <Title></Title>" & vbNewLine
                                End If
                            Catch
                            End Try
                            Try
                                value = ctrl.GetContentByKey(objFile.ArticleFileId, "IMAGEDESCRIPTION", isoriginal)
                                If Not String.IsNullOrEmpty(value) Then
                                    strXML += "                  <Description>" & XmlUtils.XMLEncode(value) & "</Description>" & vbNewLine
                                Else
                                    strXML += "                  <Description></Description>" & vbNewLine
                                End If
                            Catch
                            End Try
                            strXML += "               </LocalizedContent>" & vbNewLine
                        Next
                        strXML += "            </Localizations>" & vbNewLine
                        strXML += "         </ArticleFileInfo>" & vbNewLine
                    Next
                    strXML += "      </ArticleFiles>" & vbNewLine

                    strXML += "      <Localizations>" & vbNewLine

                    For Each objLocale As Locale In SupportedLocales.Values
                        strXML += "         <LocalizedContent>" & vbNewLine
                        Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
                        locale = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name).ToUpper
                        strXML += "            <Locale>" & XmlUtils.XMLEncode(locale) & "</Locale>" & vbNewLine

                        Dim ctrl As New ContentController(ModuleId, locale, False)
                        Dim value As String = ""
                        Dim isoriginal As Boolean = False

                        Try
                            value = ctrl.GetContentByKey(news.ItemId, "CONTENT", isoriginal)
                            If Not String.IsNullOrEmpty(value) Then
                                strXML += "            <Content>" & XmlUtils.XMLEncode(value) & "</Content>" & vbNewLine
                                strXML += "            <IsOriginal>" & XmlUtils.XMLEncode(isoriginal) & "</IsOriginal>" & vbNewLine
                            Else
                                strXML += "            <Content></Content>" & vbNewLine
                            End If
                        Catch
                        End Try
                        Try
                            value = ctrl.GetContentByKey(news.ItemId, "SUMMARY", isoriginal)
                            If Not String.IsNullOrEmpty(value) Then
                                strXML += "            <Summary>" & XmlUtils.XMLEncode(value) & "</Summary>" & vbNewLine
                            Else
                                strXML += "            <Summary></Summary>" & vbNewLine
                            End If
                        Catch
                        End Try
                        Try
                            value = ctrl.GetContentByKey(news.ItemId, "URL", isoriginal)
                            If Not String.IsNullOrEmpty(value) Then
                                strXML += "            <Url>" & XmlUtils.XMLEncode(GetExportedUrl(value, news.PortalID)) & "</Url>" & vbNewLine
                            Else
                                strXML += "            <Url></Url>" & vbNewLine
                            End If
                        Catch
                        End Try
                        Try
                            value = ctrl.GetContentByKey(news.ItemId, "TITLE", isoriginal)
                            If Not String.IsNullOrEmpty(value) Then
                                strXML += "            <Title>" & XmlUtils.XMLEncode(value) & "</Title>" & vbNewLine
                            Else
                                strXML += "            <Title></Title>" & vbNewLine
                            End If
                        Catch
                        End Try
                        strXML += "         </LocalizedContent>" & vbNewLine
                    Next
                    strXML += "      </Localizations>" & vbNewLine
                    strXML += "   </Article>" & vbNewLine
                Next
            End If
            strXML += "</Articles>" & vbNewLine
            Return strXML

        End Function

        Public Sub ImportModule(ByVal moduleId As Integer, ByVal Content As String, ByVal Version As String, ByVal UserId As Integer) Implements Entities.Modules.IPortable.ImportModule

            Dim modulecontroller As New DotNetNuke.Entities.Modules.ModuleController
            Dim objModule As DotNetNuke.Entities.Modules.ModuleInfo
            objModule = modulecontroller.GetModule(moduleId, Null.NullInteger)

            If Not objModule Is Nothing Then
                Dim xmlNewsItem As XmlNode
                Dim xmlNews As XmlNode = GetContent(Content, "Articles")
                For Each xmlNewsItem In xmlNews
                    Dim news As New ArticleInfo
                    news.moduleId = moduleId

                    news.LastUpdatedBy = GetImportedUser(UserId, objModule.PortalID, XmlUtils.GetNodeValue(xmlNewsItem, "LastUpdatedBy"))
                    news.LastUpdatedDate = XmlUtils.GetNodeValueDate(xmlNewsItem, "LastUpdatedDate", Now)
                    news.CreatedByUser = GetImportedUser(UserId, objModule.PortalID, XmlUtils.GetNodeValue(xmlNewsItem, "CreatedByUser"))
                    news.CreatedDate = XmlUtils.GetNodeValueDate(xmlNewsItem, "CreatedDate", Now)
                    news.ApprovedBy = GetImportedUser(UserId, objModule.PortalID, XmlUtils.GetNodeValue(xmlNewsItem, "ApprovedBy"))
                    news.ApprovedDate = XmlUtils.GetNodeValueDate(xmlNewsItem, "ApprovedDate", Now)
                    news.ViewOrder = XmlUtils.GetNodeValueInt(xmlNewsItem, "ViewOrder")
                    news.PublishDate = XmlUtils.GetNodeValueDate(xmlNewsItem, "PublishDate", Now)
                    news.ExpiryDate = XmlUtils.GetNodeValueDate(xmlNewsItem, "ExpiryDate", Null.NullDate)
                    news.ReviewDate = XmlUtils.GetNodeValueDate(xmlNewsItem, "ReviewDate", Null.NullDate)
                    news.IsNotified = XmlUtils.GetNodeValueBoolean(xmlNewsItem, "IsNotified")
                    news.IsApproved = XmlUtils.GetNodeValueBoolean(xmlNewsItem, "IsApproved")
                    news.IsFeatured = XmlUtils.GetNodeValueBoolean(xmlNewsItem, "IsFeatured")
                    news.IsOriginal = XmlUtils.GetNodeValueBoolean(xmlNewsItem, "IsOriginal")
                    news.PortalID = objModule.PortalID
                    news.IsPublication = False


                    Dim itemId As Integer = CType(DotNetNuke.Data.DataProvider.Instance.ExecuteScalar("NuntioArticles_AddArticle", news.moduleId, news.PortalID, news.CreatedByUser, news.CreatedDate, news.PublishDate, GetNull(news.ExpiryDate), GetNull(news.ViewOrder), news.IsNotified, news.IsApproved, GetNull(news.ApprovedBy), GetNull(news.ApprovedDate), news.IsFeatured, GetNull(news.LastUpdatedBy), GetNull(news.LastUpdatedDate), GetNull(news.ParentId), GetNull(news.ReviewDate), False), Integer)

                    news.ItemId = itemId

                    For Each node As XmlNode In xmlNewsItem.ChildNodes

                        If node.Name = "Comments" Then
                            For Each commentnode As XmlNode In node.ChildNodes

                                Dim objComment As New CommentInfo
                                objComment.ApprovedBy = GetImportedUser(UserId, news.PortalID, XmlUtils.GetNodeValue(commentnode, "ApprovedBy"))
                                objComment.Comment = XmlUtils.GetNodeValue(commentnode, "Comment")
                                objComment.CreatedBy = GetImportedUser(UserId, news.PortalID, XmlUtils.GetNodeValue(commentnode, "CreatedBy"))
                                objComment.CreatedDate = XmlUtils.GetNodeValueDate(commentnode, "CreatedDate", Now)
                                objComment.Displayname = XmlUtils.GetNodeValue(commentnode, "Displayname", "")
                                objComment.Email = XmlUtils.GetNodeValue(commentnode, "Email", "")
                                objComment.IsApproved = XmlUtils.GetNodeValueBoolean(commentnode, "IsApproved")
                                objComment.ItemId = XmlUtils.GetNodeValueInt(commentnode, "ItemId")
                                objComment.LoadGravatar = XmlUtils.GetNodeValueBoolean(commentnode, "LoadGravatar")
                                objComment.ArticleId = news.ItemId
                                objComment.RemoteAddress = XmlUtils.GetNodeValue(commentnode, "Email", "")

                                Dim objCommentController As New CommentController
                                objCommentController.Add(objComment)

                            Next
                        End If

                        If node.Name = "ArticleFiles" Then
                            For Each imagenode As XmlNode In node.ChildNodes

                                Dim objFile As New ArticleFileInfo
                                objFile.ArticleId = news.ItemId
                                objFile.FileId = GetFileIdFromPath(XmlUtils.GetNodeValue(imagenode, "FileId"), news.PortalID)
                                objFile.IsPrimary = XmlUtils.GetNodeValueBoolean(imagenode, "IsPrimary")
                                objFile.IsImage = XmlUtils.GetNodeValueBoolean(imagenode, "IsImage")
                                objFile.Extension = XmlUtils.GetNodeValue(imagenode, "Extension")
                                objFile.ArticleFileId = CType(DotNetNuke.Data.DataProvider.Instance.ExecuteScalar("Nuntio_ArticleFile_Add", objFile.FileId, objFile.ArticleId, objFile.IsPrimary, objFile.IsImage, objFile.Extension), Integer)

                                For Each localenode As XmlNode In imagenode.ChildNodes
                                    If localenode.Name = "Localizations" Then
                                        For Each entry As XmlNode In localenode.ChildNodes

                                            Dim ctrl As New ContentController(news.moduleId, XmlUtils.GetNodeValue(entry, "Locale", ""), False)
                                            Dim blnIsOriginal As Boolean = XmlUtils.GetNodeValueBoolean(entry, "IsOriginal", False)

                                            ctrl.UpdateContent("IMAGETITLE", XmlUtils.GetNodeValue(entry, "Title", ""), news.PortalID, news.ApprovedBy, news.ApprovedDate, news.CreatedByUser, news.CreatedDate, True, blnIsOriginal, objFile.ArticleFileId)
                                            ctrl.UpdateContent("IMAGEDESCRIPTION", XmlUtils.GetNodeValue(entry, "Description", ""), news.PortalID, UserId, Date.Now, UserId, Date.Now, True, blnIsOriginal, objFile.ArticleFileId)

                                        Next
                                    End If
                                Next

                            Next
                        End If

                        If node.Name = "Localizations" Then
                            For Each localenode As XmlNode In node.ChildNodes

                                Dim ctrl As New ContentController(news.moduleId, XmlUtils.GetNodeValue(localenode, "Locale", ""), False)
                                Dim blnIsOriginal As Boolean = XmlUtils.GetNodeValueBoolean(localenode, "IsOriginal", False)

                                ctrl.UpdateContent("TITLE", XmlUtils.GetNodeValue(localenode, "Title", ""), news.PortalID, news.ApprovedBy, news.ApprovedDate, news.LastUpdatedBy, news.LastUpdatedDate, news.IsApproved, news.IsOriginal, news.ItemId)
                                ctrl.UpdateContent("SUMMARY", XmlUtils.GetNodeValue(localenode, "Summary", ""), news.PortalID, news.ApprovedBy, news.ApprovedDate, news.LastUpdatedBy, news.LastUpdatedDate, news.IsApproved, news.IsOriginal, news.ItemId)
                                ctrl.UpdateContent("CONTENT", XmlUtils.GetNodeValue(localenode, "Content", ""), news.PortalID, news.ApprovedBy, news.ApprovedDate, news.LastUpdatedBy, news.LastUpdatedDate, news.IsApproved, news.IsOriginal, news.ItemId)
                                ctrl.UpdateContent("URL", GetImportedUrl(XmlUtils.GetNodeValue(localenode, "Url", ""), news.PortalID), news.PortalID, news.ApprovedBy, news.ApprovedDate, news.LastUpdatedBy, news.LastUpdatedDate, news.IsApproved, news.IsOriginal, news.ItemId)

                            Next
                        End If
                    Next
                Next
            End If


        End Sub

#End Region

#Region "Private Internal Methods"

        Private Function GetNull(ByVal Field As Object) As Object
            Return Null.GetNull(Field, DBNull.Value)
        End Function

        Private Function GetKey() As String

            Dim strData As String = DateTime.Now.ToString().GetHashCode.ToString("x")

            Return strData

        End Function

        Private Function GetExportedUser(ByVal UserId As Integer, ByVal PortalId As Integer) As String

            Dim oUser As UserInfo = Nothing
            Try
                oUser = UserController.GetUserById(PortalId, UserId)
            Catch
            End Try

            If Not oUser Is Nothing Then
                Return oUser.Email
            End If

            Return ""

        End Function

        Private Function GetImportedUser(ByVal UserId As Integer, ByVal PortalId As Integer, ByVal Email As String) As String

            Dim oUser As UserInfo = Nothing
            Try
                oUser = CType(UserController.GetUsersByEmail(PortalId, Email, 0, 999, New Integer)(0), UserInfo)
            Catch
            End Try

            If Not oUser Is Nothing Then
                Return oUser.UserID
            End If

            Return UserId

        End Function

        Private Function GetExportedUrl(ByVal Url As String, ByVal PortalId As Integer) As String

            If Url.Length = 0 Then
                Return ""
            End If

            If Url.ToLower.StartsWith("fileid=") Then
                Dim FileId As Integer = Null.NullInteger
                Integer.TryParse(Url.ToLower.Replace("fileid=", ""), FileId)
                If FileId <> Null.NullInteger Then
                    Dim strPath As String = ""
                    strPath += GetPathFromFileId(FileId, PortalId)
                    If strPath.Trim.Length > 0 Then
                        Return "FILE:" & strPath.Trim
                    End If
                End If
            End If

            If Url.ToLower.StartsWith("userid=") Then
                Dim UserId As Integer = Null.NullInteger
                Integer.TryParse(Url.ToLower.Replace("userid=", ""), UserId)
                If UserId <> Null.NullInteger Then
                    Dim objUser As UserInfo = UserController.GetUserById(PortalId, UserId)
                    If Not objUser Is Nothing Then
                        Return "USER:" & objUser.Email
                    End If
                End If
            End If

            If IsNumeric(Url) Then
                Dim TabId As Integer = Null.NullInteger
                Integer.TryParse(Url, TabId)
                If TabId <> Null.NullInteger Then
                    Dim objTab As DotNetNuke.Entities.Tabs.TabInfo = Nothing
                    Dim objTabController As New DotNetNuke.Entities.Tabs.TabController
                    objTab = objTabController.GetTab(TabId, PortalId, False)
                    If Not objTab Is Nothing Then
                        Return "TAB:" & objTab.TabName
                    End If
                End If
            End If

            Return Url

        End Function

        Private Function GetImportedUrl(ByVal Url As String, ByVal PortalId As Integer) As String

            If Url.Length = 0 Then
                Return ""
            End If

            If Url.ToLower.StartsWith("file:") Then

                Dim strPath As String = Url.ToLower.Replace("file:", "")
                Dim FileId As Integer = Null.NullInteger

                FileId = GetFileIdFromPath(strPath, PortalId)
                If FileId <> Null.NullInteger Then
                    Return "FileId=" & FileId.ToString
                End If

            End If

            If Url.ToLower.StartsWith("user:") Then
                Dim userid As Integer = GetImportedUser(Null.NullInteger, PortalId, Url.ToLower.Replace("user:", ""))
                If userid = Null.NullInteger Then
                    Return ""
                Else
                    Return "UserId=" & userid.ToString
                End If
            End If

            If Url.ToLower.StartsWith("tab:") Then
                Dim TabId As Integer = Null.NullInteger
                Dim objTab As DotNetNuke.Entities.Tabs.TabInfo = Nothing
                Dim objTabController As New DotNetNuke.Entities.Tabs.TabController

                Dim strTabName As String = Url.ToLower.Replace("url:", "")
                objTab = objTabController.GetTabByName(strTabName, PortalId)
                If Not objTab Is Nothing Then
                    Return objTab.TabID.ToString
                End If
            End If


            Return ""

        End Function

        Private Function GetFileIdFromPath(ByVal strPath As String, ByVal PortalId As Integer) As Integer

            Dim FileId As Integer = Null.NullInteger

            Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = Nothing
            Dim objFolder As DotNetNuke.Services.FileSystem.FolderInfo

            Dim strFolder As String = strPath.Substring(0, strPath.LastIndexOf("/"))
            Dim strFile As String = strPath.Substring(strPath.LastIndexOf("/") + 1)

            objFolder = FolderManager.Instance.GetFolder(PortalId, strFolder)

            If Not objFolder Is Nothing Then
                objFile = FileManager.Instance.GetFile(objFolder, strFile)
            End If

            If Not objFile Is Nothing Then
                Return objFile.FileId
            End If

            Return Null.NullInteger

        End Function

        Private Function GetPathFromFileId(ByVal FileId As Integer, ByVal PortalId As Integer) As String
            Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = Nothing
            objFile = FileManager.Instance.GetFile(FileId)
            If Not objFile Is Nothing Then
                Return objFile.Folder & objFile.FileName
            End If
            Return ""
        End Function

#End Region

    End Class


End Namespace

