Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Xml
Imports System.Security.Cryptography
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Search
Imports dnnWerk.Libraries.Nuntio.Localization


Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class CategoryController

#Region "Public Methods"

        Public Function AddCategory(ByVal objCategoryItem As CategoryInfo, ByVal UserId As Integer) As Integer
            Dim itemId As Integer = Null.NullInteger
            If Not objCategoryItem Is Nothing Then
                itemId = CType(DotNetNuke.Data.DataProvider.Instance.ExecuteScalar("pnc_CategoryItem_Add", objCategoryItem.moduleId, objCategoryItem.PortalID, objCategoryItem.ParentID, objCategoryItem.ViewOrder), Integer)
                If itemId <> Null.NullInteger Then
                    objCategoryItem.CategoryID = itemId
                    UpdateLocalization(objCategoryItem, UserId)
                End If
            End If
            Return itemId
        End Function

        Public Sub DeleteCategory(ByVal itemId As Integer, ByVal moduleId As Integer)
            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("pnc_CategoryItem_Delete", itemId, moduleId)
        End Sub

        Public Function GetCategory(ByVal itemId As Integer, ByVal moduleId As Integer, ByVal Locale As String, ByVal UseFallback As Boolean) As CategoryInfo

            Dim objCategory As New CategoryInfo
            objCategory = CBO.FillObject(Of CategoryInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("pnc_CategoryItem_Get", itemId))

            LocalizeCategory(objCategory, Locale, UseFallback)

            Return objCategory

        End Function

        Public Function ListCategoryItems(ByVal moduleId As Integer, ByVal Locale As String, ByVal StartDate As DateTime, ByVal IncludeFutureArticles As Boolean, ByVal IncludePastArticles As Boolean, ByVal UseFallback As Boolean) As List(Of CategoryInfo)

            Dim categories As New List(Of CategoryInfo)
            categories = LocalizeCategoryList(CBO.FillCollection(Of CategoryInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("pnc_CategoryItem_List", moduleId, StartDate, IncludeFutureArticles, IncludePastArticles)), Locale, UseFallback)

            Return categories

        End Function

        Public Sub UpdateCategoryItem(ByVal objCategoryItem As CategoryInfo, ByVal UserId As Integer)
            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("pnc_CategoryItem_Update", objCategoryItem.CategoryID, objCategoryItem.moduleId, objCategoryItem.PortalID, objCategoryItem.ParentID, objCategoryItem.ViewOrder)
            Me.UpdateLocalization(objCategoryItem, UserId)
        End Sub

        Public Function AddRelation(ByVal ArticleId As Integer, ByVal catId As Integer) As Integer
            Dim itemId As Integer = Null.NullInteger
            itemId = CType(DotNetNuke.Data.DataProvider.Instance.ExecuteScalar("pnc_CategoryRelation_Add", catId, ArticleId), Integer)
            Return itemId
        End Function

        Public Sub DeleteRelation(ByVal ArticleId As Integer)
            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("pnc_CategoryRelation_DeleteByItem", ArticleId)
        End Sub

        Public Sub DeleteRelationByCategoryId(ByVal CategoryId As Integer)
            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("pnc_CategoryRelation_DeleteByCategory", CategoryId)
        End Sub

        Public Function GetRelationsByitemId(ByVal ArticleId As Integer) As List(Of Integer)
            Dim categories As New List(Of Integer)
            Dim dr As IDataReader = Nothing
            Try
                dr = DotNetNuke.Data.DataProvider.Instance.ExecuteReader("pnc_CategoryRelation_GetByItem", ArticleId)
                While dr.Read
                    categories.Add(Convert.ToInt32(dr("CategoryID")))
                End While
            Catch
                'nothing
            Finally
                If Not dr Is Nothing Then
                    dr.Close()
                    dr.Dispose()
                    dr = Nothing
                End If
            End Try
            Return categories
        End Function

#End Region

#Region "Helper Functions"

        Private Function GetNull(ByVal Field As Object) As Object
            Return Null.GetNull(Field, DBNull.Value)
        End Function

        Private Sub LocalizeCategory(ByRef objCategory As CategoryInfo, ByVal Locale As String, ByVal UseFallback As Boolean)

            If Not objCategory Is Nothing Then

                objCategory.IsLocaleTranslated = False

                Dim ctrl As New dnnWerk.Libraries.Nuntio.Localization.ContentController(objCategory.moduleId, Locale, True)

                If Not Locale Is Nothing Then

                    Dim value As String = ""
                    Dim isoriginal As Boolean = False

                    value = ctrl.GetContentByKey(objCategory.CategoryID, "CATEGORYNAME", isoriginal)
                    If Not String.IsNullOrEmpty(value) Then
                        objCategory.CategoryName = value
                        objCategory.Locale = Locale
                        objCategory.IsOriginal = isoriginal
                    Else
                        objCategory.CategoryName = ""
                    End If

                End If

                If objCategory.CategoryName = "" Then
                    If UseFallback Then

                        Dim strFallbackLocale As String = ""
                        Dim intCategoryId As Integer = objCategory.CategoryID
                        Dim lstContent As List(Of ContentInfo) = ctrl.GetAllContent

                        Dim objOriginalItem As ContentInfo = lstContent.Find(Function(x) (x.IsOriginal = True And x.SourceItemId = intCategoryId And x.Key = "CATEGORYNAME"))
                        If Not objOriginalItem Is Nothing Then
                            strFallbackLocale = objOriginalItem.Locale
                        End If

                        If strFallbackLocale <> "" Then
                            LocalizeCategory(objCategory, strFallbackLocale, False)
                        End If

                    End If
                Else

                    objCategory.IsLocaleTranslated = True
                    objCategory.Locale = Locale

                End If

            End If

        End Sub

        Private Function LocalizeCategoryList(ByVal Categories As List(Of CategoryInfo), ByVal Locale As String, ByVal UseFallback As Boolean) As List(Of CategoryInfo)

            Dim listout As New List(Of CategoryInfo)

            For Each objCategory As CategoryInfo In Categories

                LocalizeCategory(objCategory, Locale, UseFallback)
                listout.Add(objCategory)

            Next

            Return listout

        End Function

        Private Sub UpdateLocalization(ByVal objCategory As CategoryInfo, ByVal UserId As Integer)


            Dim ctrl As New ContentController(objCategory.moduleId, objCategory.Locale, True)
            ctrl.UpdateContent("CATEGORYNAME", objCategory.CategoryName, objCategory.PortalID, UserId, Date.Now, UserId, Date.Now, True, False, objCategory.CategoryID)

            'Dim locItems As LocalizedItemsCollection = LocalizedItems(objCategory.moduleId)

            'Dim objLocalizedItem As New LocalizedItem
            'objLocalizedItem.Key = "CATEGORYNAME"
            'objLocalizedItem.Content = objCategory.CategoryName
            'objLocalizedItem.ApprovedBy = Null.NullInteger
            'objLocalizedItem.ApprovedDate = Date.Now
            'objLocalizedItem.CreatedBy = Null.NullInteger
            'objLocalizedItem.CreatedDate = Date.Now
            'objLocalizedItem.IsOriginal = False
            'objLocalizedItem.IsApproved = True
            'objLocalizedItem.Locale = objCategory.Locale
            'objLocalizedItem.ModuleId = objCategory.moduleId
            'objLocalizedItem.PortalId = objCategory.PortalID
            'objLocalizedItem.SourceItemId = objCategory.CategoryID
            'objLocalizedItem.Version = 1

            'locItems.Update(objLocalizedItem)

        End Sub

#End Region


    End Class


End Namespace

