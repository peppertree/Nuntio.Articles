Imports System.ComponentModel
Imports Telerik.Web.UI

Imports System.IO

Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Users
Imports System.Xml
Imports System.Security.Cryptography
Imports System.Globalization
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Controllers
Imports DotNetNuke.Services.FileSystem


Namespace dnnWerk.Modules.Nuntio.Articles
    Public Class ArticleBase
        Inherits PortalModuleBase


#Region "Private Members"

        Private _ArticleId As Integer = Null.NullInteger
        Private _NewsmoduleId As Integer = Null.NullInteger
        Private _RowCount As Integer = Null.NullInteger
        Private _ArticleController As ArticleController

#End Region

#Region "Common Properties"

        Protected ReadOnly Property ArticleController As ArticleController
            Get
                If _ArticleController Is Nothing Then
                    _ArticleController = New ArticleController
                End If
                Return _ArticleController
            End Get
        End Property

        Public ReadOnly Property ModuleDirectory() As String
            Get
                Return Me.ResolveUrl("~/Desktopmodules/Nuntio.Articles")
            End Get
        End Property

        Public ReadOnly Property ImagesDirectory() As String
            Get
                Return Me.ResolveUrl("~/Desktopmodules/Nuntio.Articles/Images/")
            End Get
        End Property

        Public Enum ArticleSortOrder
            publishdateasc = 0
            publishdatedesc = 1
            authorasc = 2
            authordesc = 3
            titleasc = 4
            titledesc = 5
        End Enum

        Public ReadOnly Property BasePage() As DotNetNuke.Framework.CDefault
            Get
                Return CType(Me.Page, DotNetNuke.Framework.CDefault)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Protected ReadOnly Property SharedResourceFile() As String
            Get
                Return ModuleDirectory & "/App_LocalResources/SharedResources.ascx"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Protected ReadOnly Property SettingsResourceFile() As String
            Get
                Return ModuleDirectory & "/App_LocalResources/Settings.ascx"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property CurrentLocale() As String
            Get
                Return CType(Page, PageBase).PageCulture.Name
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property SupportedLocales() As Dictionary(Of String, Locale)
            Get
                Dim ctrl As New LocaleController
                Return ctrl.GetLocales(PortalId)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property UiSkins() As String()
            Get
                Dim skins As String() = {"Default", "Black", "Forest", "Hay", "Office2007", "Outlook", "Sunset", "Vista", "Web20"}
                Return skins
            End Get
        End Property

#End Region

#Region "Querystring Properties"

        Public Overloads ReadOnly Property ModuleId() As Integer
            Get
                If Not Request.QueryString("ModuleId") Is Nothing Then
                    Return Convert.ToInt32(Request.QueryString("ModuleId"))
                End If
                Return MyBase.ModuleId
            End Get
        End Property

        Public Overloads ReadOnly Property OpenerModuleId() As Integer
            Get
                If Not Request.QueryString("OpenerModuleId") Is Nothing Then
                    Return Convert.ToInt32(Request.QueryString("OpenerModuleId"))
                End If
                Return MyBase.ModuleId
            End Get
        End Property

        Public Overloads ReadOnly Property TabId() As Integer
            Get
                Dim tid As Integer = Null.NullInteger
                tid = PortalSettings.ActiveTab.TabID
                If tid <> Null.NullInteger Then
                    Return tid
                End If
                Return MyBase.TabId
            End Get
        End Property

        Protected Property ItemId() As Integer
            Get
                If Not ViewState("ArticleItemId") Is Nothing Then
                    Return CType(ViewState("ArticleItemId"), Integer)
                End If
                If Not Request.QueryString("ItemId") Is Nothing Then
                    Return CType(Request.QueryString("ItemId"), Integer)
                End If
                Return Null.NullInteger
            End Get
            Set(ByVal value As Integer)
                ViewState("ArticleItemId") = value
            End Set
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property ArticleId() As Integer
            Get
                If Not Request.QueryString("ArticleId") Is Nothing Then
                    _ArticleId = Integer.Parse(Request.QueryString("ArticleId"))
                End If
                If Not Request.QueryString("ItemId") Is Nothing Then
                    _ArticleId = Integer.Parse(Request.QueryString("ItemId"))
                End If
                If Not Request.QueryString("NewsId") Is Nothing Then
                    _ArticleId = Integer.Parse(Request.QueryString("NewsId"))
                End If
                Return _ArticleId
            End Get
            Set(ByVal Value As Integer)
                _ArticleId = Value
            End Set
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property CommentId() As Integer
            Get
                Dim _Commentid As Integer = Null.NullInteger

                If Not Request.QueryString("CommentId") Is Nothing Then
                    _Commentid = Integer.Parse(Request.QueryString("CommentId"))
                    Return _Commentid
                End If

                Return _Commentid

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AuthorId() As Integer
            Get
                Dim _authorid As Integer = Null.NullInteger

                If Not Request.QueryString("AuthorId") Is Nothing Then
                    _authorid = Integer.Parse(Request.QueryString("AuthorId"))
                    Return _authorid
                End If

                If Not Request.QueryString("UserId") Is Nothing Then
                    _authorid = Integer.Parse(Request.QueryString("UserId"))
                    Return _authorid
                End If

                If Not Request.QueryString("uid") Is Nothing Then
                    _authorid = Integer.Parse(Request.QueryString("uid"))
                    Return _authorid
                End If

                Return _authorid

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property CategoryID() As Integer
            Get
                Dim _CategoryID As Integer = Null.NullInteger

                If Not Request.QueryString("CategoryID") Is Nothing Then
                    _CategoryID = Integer.Parse(Request.QueryString("CategoryID"))
                    Return _CategoryID
                End If

                Return _CategoryID

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property NewsModuleId() As Integer
            Get
                If CType(Settings("NewsModuleId"), String) <> "" Then
                    Return CType(Settings("NewsModuleId"), Integer)
                End If

                If Not Request.QueryString("ModuleId") Is Nothing Then
                    Return Convert.ToInt32(Request.QueryString("ModuleId"))
                End If

                Return ModuleId

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property UnapprovedArticlesCount() As Integer
            Get
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        Dim count As Integer = 0

                        If Not DataCache.GetCache("Nuntio_GetUnapprovedArticlesCount_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                count = CType(DataCache.GetCache("Nuntio_GetUnapprovedArticlesCount_" & NewsModuleId.ToString), Integer)
                                Return count
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        count = nc.GetUnapprovedArticleCount(NewsModuleId, PortalId)
                        DataCache.SetCache("Nuntio_GetUnapprovedArticlesCount_" & NewsModuleId.ToString, count)
                        Return count

                    End If
                End If
                Return 0
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property UnapprovedArticles() As List(Of ArticleInfo)
            Get
                Dim list As New List(Of ArticleInfo)
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        If Not DataCache.GetCache("Nuntio_GetUnapprovedArticles_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                list = CType(DataCache.GetCache("Nuntio_GetUnapprovedArticles_" & NewsModuleId.ToString), List(Of ArticleInfo))
                                Return list
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        list = nc.GetUnapprovedArticles(NewsModuleId, PortalId, CurrentLocale)
                        DataCache.SetCache("Nuntio_GetUnapprovedArticles_" & NewsModuleId.ToString, list)
                        Return list
                    End If
                End If
                Return list
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property DeletedArticlesCount() As Integer
            Get
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        Dim count As Integer = 0

                        If Not DataCache.GetCache("Nuntio_GetDeletedArticlesCount_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                count = CType(DataCache.GetCache("Nuntio_GetDeletedArticlesCount_" & NewsModuleId.ToString), Integer)
                                Return count
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        count = nc.GetDeletedCount(NewsModuleId, PortalId)
                        DataCache.SetCache("Nuntio_GetDeletedArticleCount_" & NewsModuleId.ToString, count)
                        Return count

                    End If
                End If
                Return 0
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property DeletedArticles() As List(Of ArticleInfo)
            Get
                Dim list As New List(Of ArticleInfo)
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        If Not DataCache.GetCache("Nuntio_GetDeletedArticles_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                list = CType(DataCache.GetCache("Nuntio_GetDeletedArticles_" & NewsModuleId.ToString), List(Of ArticleInfo))
                                Return list
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        list = nc.GetDeleted(NewsModuleId, PortalId, CurrentLocale)
                        DataCache.SetCache("Nuntio_GetDeletedArticles_" & NewsModuleId.ToString, list)
                        Return list
                    End If
                End If
                Return list
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ExpiredArticleCount() As Integer
            Get
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        Dim count As Integer = 0

                        If Not DataCache.GetCache("Nuntio_GetExpiredArticleCount_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                count = CType(DataCache.GetCache("Nuntio_GetExpiredArticleCount_" & NewsModuleId.ToString), Integer)
                                Return count
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        count = nc.GetExpiredArticleCount(NewsModuleId, PortalId)
                        DataCache.SetCache("Nuntio_GetExpiredArticleCount_" & NewsModuleId.ToString, count)
                        Return count

                    End If
                End If
                Return 0
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ExpiredArticles() As List(Of ArticleInfo)
            Get
                Dim list As New List(Of ArticleInfo)
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        If Not DataCache.GetCache("Nuntio_GetExpiredArticles_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                list = CType(DataCache.GetCache("Nuntio_GetExpiredArticles_" & NewsModuleId.ToString), List(Of ArticleInfo))
                                Return list
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        list = nc.GetExpiredArticles(NewsModuleId, PortalId, CurrentLocale)
                        DataCache.SetCache("Nuntio_GetExpiredArticles_" & NewsModuleId.ToString, list)
                        Return list

                    End If
                End If
                Return list
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property NotYetPublishedArticleCount() As Integer
            Get
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        Dim count As Integer = 0

                        If Not DataCache.GetCache("Nuntio_GetNotYetPublishedCount_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                count = CType(DataCache.GetCache("Nuntio_GetNotYetPublishedCount_" & NewsModuleId.ToString), Integer)
                                Return count
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        count = nc.GetNotYetPublishedCount(NewsModuleId, PortalId)
                        DataCache.SetCache("Nuntio_GetNotYetPublishedCount_" & NewsModuleId.ToString, count)
                        Return count

                    End If
                End If
                Return 0
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property NotYetPublishedArticles() As List(Of ArticleInfo)
            Get
                Dim list As New List(Of ArticleInfo)
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        If Not DataCache.GetCache("Nuntio_GetNotYetPublished_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                list = CType(DataCache.GetCache("Nuntio_GetNotYetPublished_" & NewsModuleId.ToString), List(Of ArticleInfo))
                                Return list
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        list = nc.GetNotYetPublished(NewsModuleId, PortalId, CurrentLocale)
                        DataCache.SetCache("Nuntio_GetNotYetPublished_" & NewsModuleId.ToString, list)
                        Return list

                    End If
                End If
                Return list
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property NeedsReviewingArticleCount() As Integer
            Get
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        Dim count As Integer = 0

                        If Not DataCache.GetCache("Nuntio_GetNeedsReviewingCount_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                count = CType(DataCache.GetCache("Nuntio_GetNeedsReviewingCount_" & NewsModuleId.ToString), Integer)
                                Return count
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        count = nc.GetNeedsReviewingCount(NewsModuleId, PortalId, Date.Now)
                        DataCache.SetCache("Nuntio_GetNeedsReviewingCount_" & NewsModuleId.ToString, count)
                        Return count

                    End If
                End If
                Return 0
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property NeedsReviewingArticles() As List(Of ArticleInfo)
            Get
                Dim list As New List(Of ArticleInfo)
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        If Not DataCache.GetCache("Nuntio_GetNeedsReviewing_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                list = CType(DataCache.GetCache("Nuntio_GetNeedsReviewing_" & NewsModuleId.ToString), List(Of ArticleInfo))
                                Return list
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        list = nc.GetNeedsReviewing(NewsModuleId, PortalId, CurrentLocale, Date.Now)
                        DataCache.SetCache("Nuntio_GetNeedsReviewing_" & NewsModuleId.ToString, list)
                        Return list

                    End If
                End If
                Return list
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property FeaturedArticlesCount() As Integer
            Get
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        Dim count As Integer = 0

                        If Not DataCache.GetCache("Nuntio_GetFeaturedCount_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                count = CType(DataCache.GetCache("Nuntio_GetFeaturedCount_" & NewsModuleId.ToString), Integer)
                                Return count
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        count = nc.GetFeaturedCount(NewsModuleId, PortalId)
                        DataCache.SetCache("Nuntio_GetFeaturedCount_" & NewsModuleId.ToString, count)
                        Return count

                    End If
                End If
                Return 0
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property FeaturedArticles() As List(Of ArticleInfo)
            Get
                Dim list As New List(Of ArticleInfo)
                If Request.IsAuthenticated Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                        If Not DataCache.GetCache("Nuntio_GetFeatured_" & NewsModuleId.ToString) Is Nothing Then
                            Try
                                list = CType(DataCache.GetCache("Nuntio_GetFeatured_" & NewsModuleId.ToString), List(Of ArticleInfo))
                                Return list
                            Catch
                            End Try
                        End If

                        Dim nc As New ArticleController
                        list = nc.GetFeatured(NewsModuleId, PortalId, CurrentLocale, True)
                        DataCache.SetCache("Nuntio_GetFeatured_" & NewsModuleId.ToString, list)
                        Return list

                    End If
                End If
                Return list
            End Get
        End Property

#End Region

#Region "Protected Methods"

        Protected Function CreateTheme(ByVal Mode As String, ByVal sourcepath As String, ByVal themename As String) As String


            Dim path As String = ""

            Select Case Mode
                Case "HOST"

                    path = Server.MapPath(Me.ModuleDirectory & "/templates/" & themename & "/")

                    If Not Directory.Exists(path) Then
                        Directory.CreateDirectory(path)
                    End If
                    If Not Directory.Exists(path & "\images") Then
                        Directory.CreateDirectory(path & "\images")
                    End If

                Case "PORTAL"

                    path = Server.MapPath(Me.ModuleDirectory & "/templates/portal/")

                    If Not Directory.Exists(path) Then
                        Directory.CreateDirectory(path)
                    End If
                    path += PortalSettings.PortalId.ToString & "\"
                    If Not Directory.Exists(path) Then
                        Directory.CreateDirectory(path)
                    End If
                    path += themename & "\"
                    If Not Directory.Exists(path) Then
                        Directory.CreateDirectory(path)
                    End If
                    If Not Directory.Exists(path & "\images") Then
                        Directory.CreateDirectory(path & "\images")
                    End If

            End Select

            For Each File As String In Directory.GetFiles(sourcepath)
                FileCopy(File, path & File.Substring(File.LastIndexOf("\") + 1))
            Next

            For Each File As String In Directory.GetFiles(sourcepath & "images")
                Dim newpath As String = path & "images\" & File.Substring(File.LastIndexOf("\") + 1)
                FileCopy(File, newpath)
            Next

            Return path

        End Function

#End Region

    End Class
End Namespace

