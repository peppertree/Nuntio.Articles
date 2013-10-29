Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Xml
Imports System.Security.Cryptography
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Search
Imports DotNetNuke.Entities.Users
Imports dnnWerk.Libraries.Nuntio.Localization


Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class ArticleFileController

#Region "Public Image CRUD methods"

        Public Function AddFile(ByVal objFile As ArticleFileInfo, ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal UserId As Integer, ByVal IsOriginalLocale As Boolean) As Integer

            Dim itemId As Integer = Null.NullInteger

            itemId = CType(DotNetNuke.Data.DataProvider.Instance.ExecuteScalar("NuntioArticles_AddFile", objFile.FileId, objFile.ArticleId, objFile.IsPrimary, objFile.IsImage, objFile.Extension), Integer)

            If itemId <> Null.NullInteger Then
                objFile.ArticleFileId = itemId
                UpdateFileLocalization(objFile, ModuleId, PortalId, UserId, IsOriginalLocale)
            End If

            Return itemId

        End Function

        Public Sub UpdateFile(ByVal objFile As ArticleFileInfo, ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal UserId As Integer, ByVal IsOriginalLocale As Boolean)

            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("NuntioArticles_UpdateFile", objFile.ArticleFileId, objFile.FileId, objFile.ArticleId, objFile.IsPrimary, objFile.IsImage, objFile.Extension)

            Me.UpdateFileLocalization(objFile, ModuleId, PortalId, UserId, IsOriginalLocale)

        End Sub

        Public Sub DeleteFile(ByVal ArticleFileId As Integer)
            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("NuntioArticles_DeleteFile", ArticleFileId)
        End Sub

        Public Function GetFile(ByVal FileId As Integer, ByVal ModuleId As Integer, ByVal Locale As String, ByVal UseFallBack As Boolean) As ArticleFileInfo

            Dim dr As IDataReader = Nothing
            Dim objImage As ArticleFileInfo = Nothing

            dr = DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetFile", FileId)

            objImage = CBO.FillObject(Of ArticleFileInfo)(dr)

            LocalizeFile(objImage, ModuleId, Locale, UseFallBack)

            If Not dr Is Nothing Then
                dr.Close()
                dr.Dispose()
                dr = Nothing
            End If

            Return objImage

        End Function

        Public Function GetImages(ByVal ArticleId As Integer, ByVal ModuleId As Integer, ByVal Locale As String, ByVal UseFallBack As Boolean) As List(Of ArticleFileInfo)

            Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_ListImages", ArticleId)

            Dim images As New List(Of ArticleFileInfo)
            images = LocalizeFileList(CBO.FillCollection(Of ArticleFileInfo)(dr), ModuleId, Locale, UseFallBack)

            If Not dr Is Nothing Then
                dr.Close()
                dr.Dispose()
                dr = Nothing
            End If

            Return images

        End Function

        Public Function GetAttachments(ByVal ArticleId As Integer, ByVal ModuleId As Integer, ByVal Locale As String, ByVal UseFallBack As Boolean) As List(Of ArticleFileInfo)

            Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_ListAttachments", ArticleId)

            Dim attachments As New List(Of ArticleFileInfo)
            attachments = LocalizeFileList(CBO.FillCollection(Of ArticleFileInfo)(dr), ModuleId, Locale, UseFallBack)

            If Not dr Is Nothing Then
                dr.Close()
                dr.Dispose()
                dr = Nothing
            End If

            Return attachments

        End Function

#End Region

#Region "Localization Helpers"

        Private Sub LocalizeFile(ByRef objFile As ArticleFileInfo, ByVal ModuleId As Integer, ByVal Locale As String, ByVal UseFallback As Boolean)

            If Not objFile Is Nothing Then

                objFile.IsLocaleTranslated = False

                Dim ctrl As New dnnWerk.Libraries.Nuntio.Localization.ContentController(ModuleId, Locale, True)

                If Not Locale Is Nothing Then

                    Dim value As String = ""
                    Dim isoriginal As Boolean = False

                    objFile.Locale = Locale

                    value = ctrl.GetContentByKey(objFile.ArticleFileId, "FILETITLE", isoriginal)
                    If Not String.IsNullOrEmpty(value) Then
                        objFile.ImageTitle = value
                        objFile.Locale = Locale
                        objFile.IsOriginal = isoriginal
                    Else
                        objFile.ImageTitle = ""
                    End If

                    value = ctrl.GetContentByKey(objFile.ArticleFileId, "FILEDESCRIPTION", isoriginal)
                    If Not String.IsNullOrEmpty(value) Then
                        objFile.ImageDescription = value
                        objFile.Locale = Locale
                        objFile.IsOriginal = isoriginal
                    Else
                        objFile.ImageDescription = ""
                    End If

                End If

                If objFile.ImageTitle = "" Then
                    If UseFallback Then

                        Dim strFallbackLocale As String = ""
                        Dim intImageId As Integer = objFile.ArticleFileId
                        Dim lstContent As List(Of ContentInfo) = ctrl.GetAllContent

                        Dim objOriginalItem As ContentInfo = lstContent.Find(Function(x) (x.IsOriginal = True And x.SourceItemId = intImageId And x.Key = "IMAGETITLE"))
                        If Not objOriginalItem Is Nothing Then
                            strFallbackLocale = objOriginalItem.Locale
                        End If

                        If strFallbackLocale <> "" Then
                            LocalizeFile(objFile, ModuleId, strFallbackLocale, False)
                        End If

                    End If
                Else

                    objFile.IsLocaleTranslated = True
                    objFile.Locale = Locale

                End If

            End If

        End Sub

        Private Function LocalizeFileList(ByVal Images As List(Of ArticleFileInfo), ByVal ModuleId As Integer, ByVal Locale As String, ByVal UseFallback As Boolean) As List(Of ArticleFileInfo)

            Dim listout As New List(Of ArticleFileInfo)

            For Each objFile As ArticleFileInfo In Images

                LocalizeFile(objFile, ModuleId, Locale, UseFallback)
                listout.Add(objFile)

            Next

            Return listout

        End Function

        Private Sub UpdateFileLocalization(ByVal objFile As ArticleFileInfo, ByVal ModuleId As Integer, ByVal PortalId As Integer, ByVal UserId As Integer, ByVal IsOriginalLocale As Boolean)

            Dim ctrl As New ContentController(ModuleId, objFile.Locale, True)
            ctrl.UpdateContent("FILETITLE", objFile.ImageTitle, PortalId, UserId, Date.Now, UserId, Date.Now, True, IsOriginalLocale, objFile.ArticleFileId)
            ctrl.UpdateContent("FILEDESCRIPTION", objFile.ImageDescription, PortalId, UserId, Date.Now, UserId, Date.Now, True, IsOriginalLocale, objFile.ArticleFileId)

        End Sub

#End Region

    End Class

End Namespace

