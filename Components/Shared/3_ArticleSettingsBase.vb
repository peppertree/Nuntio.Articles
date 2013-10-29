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
    Public Class ArticleSettings
        Inherits ArticleBase

        Public Overloads ReadOnly Property Settings() As Hashtable
            Get

                If Not MyBase.Settings Is Nothing Then
                    If MyBase.Settings.Count > 0 Then
                        Return MyBase.Settings
                    End If
                End If

                If Not Request.QueryString("ModuleId") Is Nothing Then
                    Dim objModuleController As New ModuleController
                    Dim modsettings As Hashtable
                    modsettings = objModuleController.GetModuleSettings(Integer.Parse(Request.QueryString("ModuleId")))
                    Return modsettings
                End If

                If Not Request.QueryString("amp;ModuleId") Is Nothing Then
                    Dim objModuleController As New ModuleController
                    Dim modsettings As Hashtable
                    modsettings = objModuleController.GetModuleSettings(Integer.Parse(Request.QueryString("amp;ModuleId")))
                    Return modsettings
                End If

                Return New Hashtable

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IsModerated() As Boolean
            Get
                If Settings.Contains("IsModerated") Then
                    If CType(Settings("IsModerated"), Boolean) = True Then
                        Return True
                    End If
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ModeratorRole() As String
            Get
                If (Settings.Contains("NUNTIO_MODERATORROLE")) Then
                    Return Settings("NUNTIO_MODERATORROLE").ToString()
                End If
                Return PortalSettings.AdministratorRoleName
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AuthorizedRoles() As String
            Get
                If (Settings.Contains("PNCNEWS_AUTHORIZEDROLES")) Then
                    Return Settings("PNCNEWS_AUTHORIZEDROLES").ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property FromAddress() As String
            Get
                If CType(Settings("FromAddress"), String) <> "" Then
                    Return CType(Settings("FromAddress"), String)
                End If
                Return PortalSettings.Email
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AllowTwitterPosts() As Boolean
            Get

                If CType(Settings("AllowTwitterPosts"), String) <> "" Then
                    Return CType(Settings("AllowTwitterPosts"), Boolean)
                End If
                Return False

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowLinksInline() As Boolean
            Get
                If CType(Settings("ShowLinksInline"), String) <> "" Then
                    Return CType(Settings("ShowLinksInline"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AllowJournalUpdates() As Boolean
            Get

                If JournalIntegration.JournalSupported = False Then

                    Return False

                Else

                    If CType(Settings("AllowJournalUpdates"), String) <> "" Then
                        Return CType(Settings("AllowJournalUpdates"), Boolean)
                    End If

                    Return True

                End If

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowArticleCount() As Boolean
            Get
                If CType(Settings("ShowArticleCount"), String) <> "" Then
                    Return CType(Settings("ShowArticleCount"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AllowCommentsForAnnonymous() As Boolean
            Get
                If CType(Settings("AllowCommentsForAnnonymous"), String) <> "" Then
                    Return CType(Settings("AllowCommentsForAnnonymous"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AutoApproveAnonymousComments() As Boolean
            Get
                If CType(Settings("AutoApproveAnonymousComments"), String) <> "" Then
                    Return CType(Settings("AutoApproveAnonymousComments"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AutoApproveAuthenticatedtComments() As Boolean
            Get
                If CType(Settings("AutoApproveAuthenticatedtComments"), String) <> "" Then
                    Return CType(Settings("AutoApproveAuthenticatedtComments"), Boolean)
                End If
                Return False
            End Get
        End Property

        Public ReadOnly Property DMXSupport() As Boolean
            Get
                Dim PathDMX As String = HttpRuntime.AppDomainAppPath & "bin\Bring2mind.DNN.Modules.DMX.dll"
                Dim PathDMXProvider As String = HttpRuntime.AppDomainAppPath & "bin\Bring2mind.DNN.Modules.DMX.RadEditorContentProvider.dll"
                If System.IO.File.Exists(PathDMX) AndAlso System.IO.File.Exists(PathDMXProvider) Then
                    Return True
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property CategoryList() As List(Of Integer)
            Get
                Dim _list As New List(Of Integer)

                If CategoryID <> Null.NullInteger Then
                    _list.Add(CategoryID)
                    Return _list
                End If

                If Settings.Contains("ShowCategory") Then
                    Dim strList As String = CType(Settings("ShowCategory"), String)
                    For Each strCategory As String In strList.Split(Char.Parse(";"))
                        If strCategory.Length > 0 Then
                            If IsNumeric(strCategory) Then
                                If Convert.ToInt32(strCategory) <> Null.NullInteger Then
                                    _list.Add(Convert.ToInt32(strCategory))
                                End If                                
                            End If
                        End If
                    Next
                End If

                Return _list

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property HideOnAllNews() As Boolean
            Get
                If CType(Settings("HideOnAllNews"), String) <> "" Then
                    Return CType(Settings("HideOnAllNews"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property EnableModuleSearch() As Boolean
            Get
                If CType(Settings("EnableModuleSearch"), String) <> "" Then
                    Return CType(Settings("EnableModuleSearch"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property UseOriginalVersion() As Boolean
            Get
                If CType(Settings("UseOriginalVersion"), String) <> "" Then
                    Return CType(Settings("UseOriginalVersion"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property EnableDnnSearch() As Boolean
            Get
                If CType(Settings("EnableDnnSearch"), String) <> "" Then
                    Return CType(Settings("EnableDnnSearch"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowLinesInAuthors() As Boolean
            Get
                If CType(Settings("ShowLines"), String) <> "" Then
                    Return CType(Settings("ShowLines"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowLinesInCategories() As Boolean
            Get
                If CType(Settings("ShowLines"), String) <> "" Then
                    Return CType(Settings("ShowLines"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowLinesInArchive() As Boolean
            Get
                If CType(Settings("ShowLines"), String) <> "" Then
                    Return CType(Settings("ShowLines"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowLinesInRSSTree() As Boolean
            Get
                If CType(Settings("ShowLines"), String) <> "" Then
                    Return CType(Settings("ShowLines"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowFolderIconsInAuthors() As Boolean
            Get
                If CType(Settings("ShowFolderIcons"), String) <> "" Then
                    Return CType(Settings("ShowFolderIcons"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowFolderIconsInCategories() As Boolean
            Get
                If CType(Settings("ShowFolderIcons"), String) <> "" Then
                    Return CType(Settings("ShowFolderIcons"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowFolderIconsInArchive() As Boolean
            Get
                If CType(Settings("ShowFolderIcons"), String) <> "" Then
                    Return CType(Settings("ShowFolderIcons"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IncludeRootInAuthors() As Boolean
            Get
                If CType(Settings("IncludeRoot"), String) <> "" Then
                    Return CType(Settings("IncludeRoot"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IncludeRootInCategories() As Boolean
            Get
                If CType(Settings("IncludeRoot"), String) <> "" Then
                    Return CType(Settings("IncludeRoot"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IncludeRootInArchive() As Boolean
            Get
                If CType(Settings("IncludeRoot"), String) <> "" Then
                    Return CType(Settings("IncludeRoot"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IncludeMonth() As Boolean
            Get
                If CType(Settings("IncludeMonth"), String) <> "" Then
                    Return CType(Settings("IncludeMonth"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ModuleView() As String
            Get
                If CType(Settings("ModuleView"), String) <> "" Then
                    Return CType(Settings("ModuleView"), String)
                End If
                Return ""
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property TreeSkinInArchive() As String
            Get
                If CType(Settings("TreeSkin"), String) <> "" Then
                    For Each Skin As String In UiSkins
                        If Skin.ToLower = CType(Settings("TreeSkin"), String).ToLower Then
                            Return CType(Settings("TreeSkin"), String)
                        End If
                    Next
                End If
                Return "Default"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property TreeSkinInRss() As String
            Get
                If CType(Settings("TreeSkin"), String) <> "" Then
                    For Each Skin As String In UiSkins
                        If Skin.ToLower = CType(Settings("TreeSkin"), String).ToLower Then
                            Return CType(Settings("TreeSkin"), String)
                        End If
                    Next
                End If
                Return "Default"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property TreeSkinInAuthors() As String
            Get
                If CType(Settings("TreeSkin"), String) <> "" Then
                    For Each Skin As String In UiSkins
                        If Skin.ToLower = CType(Settings("TreeSkin"), String).ToLower Then
                            Return CType(Settings("TreeSkin"), String)
                        End If
                    Next
                End If
                Return "Default"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property TreeSkinInCategories() As String
            Get
                If CType(Settings("TreeSkin"), String) <> "" Then
                    For Each Skin As String In UiSkins
                        If Skin.ToLower = CType(Settings("TreeSkin"), String).ToLower Then
                            Return CType(Settings("TreeSkin"), String)
                        End If
                    Next
                End If
                Return "Default"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property EnableSummary() As Boolean
            Get
                If Settings.Contains("EnableSummary") Then
                    Return CType(Settings("EnableSummary"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowFutureItems() As Boolean
            Get
                If Settings.Contains("ShowFutureItems") Then
                    Return CType(Settings("ShowFutureItems"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IncludeFeaturedItems() As Boolean
            Get
                If Settings.Contains("IncludeFeaturedItems") Then
                    Return CType(Settings("IncludeFeaturedItems"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IncludeNonFeaturedItems() As Boolean
            Get
                If Settings.Contains("IncludeNonFeaturedItems") Then
                    Return CType(Settings("IncludeNonFeaturedItems"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property MakeFeaturedSticky() As Boolean
            Get
                If Settings.Contains("MakeFeaturedSticky") Then
                    Return CType(Settings("MakeFeaturedSticky"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowPastItems() As Boolean
            Get
                If Settings.Contains("ShowPastItems") Then
                    Return CType(Settings("ShowPastItems"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IncludeExpired() As Boolean
            Get
                If Settings.Contains("IncludeExpired") Then
                    Return CType(Settings("IncludeExpired"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property EnableScrolling() As Boolean
            Get
                If Settings.Contains("EnableScrolling") Then
                    Return CType(Settings("EnableScrolling"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property SortOrder() As ArticleSortOrder
            Get
                Try
                    If Settings.Contains("SortOrder") Then
                        If CType(Settings("SortOrder"), Integer) = 0 Then
                            Return ArticleSortOrder.publishdateasc
                        End If
                        If CType(Settings("SortOrder"), Integer) = 1 Then
                            Return ArticleSortOrder.publishdatedesc
                        End If
                        If CType(Settings("SortOrder"), Integer) = 2 Then
                            Return ArticleSortOrder.authorasc
                        End If
                        If CType(Settings("SortOrder"), Integer) = 3 Then
                            Return ArticleSortOrder.authordesc
                        End If
                        If CType(Settings("SortOrder"), Integer) = 4 Then
                            Return ArticleSortOrder.titleasc
                        End If
                        If CType(Settings("SortOrder"), Integer) = 5 Then
                            Return ArticleSortOrder.titledesc
                        End If
                    End If
                Catch
                End Try
                Return ArticleSortOrder.publishdatedesc
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ScrollSpeed() As Integer
            Get
                If Settings.Contains("ScrollSpeed") Then
                    Return CType(Settings("ScrollSpeed"), Integer)
                End If
                Return 500
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ScrollTimeOut() As Integer
            Get
                If Settings.Contains("ScrollTimeOut") Then
                    Return CType(Settings("ScrollTimeOut"), Integer)
                End If
                Return 5000
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ScrollHeight() As String
            Get
                If Settings.Contains("ScrollHeight") Then
                    Return CType(Settings("ScrollHeight"), String)
                End If
                Return "200px"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ScrollWidth() As String
            Get
                If Settings.Contains("ScrollWidth") Then
                    Return CType(Settings("ScrollWidth"), String)
                End If
                Return "100%"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property BannerType() As String
            Get
                If Settings.Contains("BannerType") Then
                    Return CType(Settings("BannerType"), String)
                End If
                Return "1"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property BannerSource() As String
            Get
                If Settings.Contains("BannerSource") Then
                    Return CType(Settings("BannerSource"), String)
                End If
                Return "L"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property TransitionType() As String
            Get
                If Settings.Contains("TransitionType") Then
                    Return CType(Settings("TransitionType"), String)
                End If
                Return "Slideshow"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property TransitionEffect() As String
            Get
                If Settings.Contains("TransitionEffect") Then
                    Return CType(Settings("TransitionEffect"), String)
                End If
                Return "NONE"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property BannerCount() As Integer
            Get
                If Settings.Contains("BannerCount") Then
                    Return CType(Settings("BannerCount"), Integer)
                End If
                Return 0
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ScrollDirection() As RotatorScrollDirection
            Get
                If Settings.Contains("ScrollDirection") Then
                    Dim intDir As Integer = CType(Settings("ScrollDirection"), Integer)
                    Select Case intDir
                        Case 4
                            Return RotatorScrollDirection.Up
                        Case 8
                            Return RotatorScrollDirection.Down
                        Case 1
                            Return RotatorScrollDirection.Left
                        Case 2
                            Return RotatorScrollDirection.Right
                    End Select
                End If
                Return RotatorScrollDirection.Up
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property NewsModuleTab() As Integer
            Get
                If CType(Settings("NewsModuleTab"), String) <> "" Then
                    Return CType(Settings("NewsModuleTab"), Integer)
                End If
                Return TabId
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property RowCount() As Integer
            Get
                If CType(Settings("RowCount"), String) <> "" Then
                    Return CType(Settings("RowCount"), Integer)
                End If
                Return 10
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property NewsModuleSettings() As Hashtable
            Get
                Dim objModuleController As New ModuleController
                Dim modsettings As Hashtable
                modsettings = objModuleController.GetModuleSettings(Me.NewsModuleId)
                Return modsettings
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AllowSubscriptions() As Boolean
            Get

                If CType(Settings("AllowSubscriptions"), String) <> "" Then
                    Return CType(Settings("AllowSubscriptions"), Boolean)
                End If
                Return False

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowRegister() As Boolean
            Get
                If CType(Settings("ShowRegister"), String) <> "" Then
                    Return CType(Settings("ShowRegister"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property NotifyAdmin() As Boolean
            Get
                If CType(Settings("NotifyAdmin"), String) <> "" Then
                    Return CType(Settings("NotifyAdmin"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property EnablePaging() As Boolean
            Get
                If CType(Settings("EnablePaging"), String) <> "" Then
                    Return CType(Settings("EnablePaging"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property PagingMaxCount() As Integer
            Get
                If CType(Settings("PagingMaxCount"), String) <> "" Then
                    Return CType(Settings("PagingMaxCount"), Integer)
                End If
                Return Null.NullInteger
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property OnlyRegisteredUsersCanSubscribe() As Boolean
            Get
                If CType(Settings("OnlyRegisteredUsersCanSubscribe"), String) <> "" Then
                    Return CType(Settings("OnlyRegisteredUsersCanSubscribe"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property SubscribedRoles() As String
            Get
                If CType(Settings("SubscribedRoles"), String) <> "" Then
                    Return CType(Settings("SubscribedRoles"), String)
                End If
                Return Null.NullString
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ArticleTheme() As String
            Get
                If Not Request.QueryString("amp;ArticleTheme") Is Nothing Then
                    Return CType(Request.QueryString("amp;ArticleTheme"), String)
                End If
                If Not Request.QueryString("ArticleTheme") Is Nothing Then
                    Return CType(Request.QueryString("ArticleTheme"), String)
                End If
                If CType(Settings("ArticleTheme"), String) <> "" Then
                    Return CType(Settings("ArticleTheme"), String)
                End If
                Return "Default"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IncludePublications() As Boolean
            Get
                If Settings.Contains("IncludePublications") Then
                    Return CType(Settings("IncludePublications"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IncludeNonPublications() As Boolean
            Get
                If Settings.Contains("IncludeNonPublications") Then
                    Return CType(Settings("IncludeNonPublications"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ContentWidth() As Unit
            Get

                Dim value As Unit = Unit.Percentage(70)

                If CType(Settings("ContentWidth"), String) <> "" Then

                    Dim strUnit As String = CType(Settings("ContentWidth"), String)

                    If strUnit.EndsWith("%") Then
                        Try
                            value = Unit.Percentage(Convert.ToDouble(strUnit.Replace("%", "")))
                        Catch
                        End Try
                    End If

                    If IsNumeric(strUnit) Then
                        Try
                            value = Unit.Pixel(Convert.ToInt32(strUnit))
                        Catch
                        End Try
                    End If

                End If

                Return value

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property TocWidth() As Unit
            Get

                Dim value As Unit = Unit.Percentage(30)

                If CType(Settings("TocWidth"), String) <> "" Then

                    Dim strUnit As String = CType(Settings("TocWidth"), String)

                    If strUnit.EndsWith("%") Then
                        Try
                            value = Unit.Percentage(Convert.ToDouble(strUnit.Replace("%", "")))
                        Catch
                        End Try
                    End If

                    If IsNumeric(strUnit) Then
                        Try
                            value = Unit.Pixel(Convert.ToInt32(strUnit))
                        Catch
                        End Try
                    End If

                End If

                Return value

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property SplitterHeight() As Unit
            Get

                Dim value As Unit = Unit.Pixel(600)

                If CType(Settings("SplitterHeight"), String) <> "" Then

                    Dim strUnit As String = CType(Settings("SplitterHeight"), String)

                    If strUnit.EndsWith("%") Then
                        Try
                            value = Unit.Percentage(Convert.ToDouble(strUnit.Replace("%", "")))
                        Catch
                        End Try
                    End If

                    If IsNumeric(strUnit) Then
                        Try
                            value = Unit.Pixel(Convert.ToInt32(strUnit))
                        Catch
                        End Try
                    End If

                End If

                Return value

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property SplitterWidth() As Unit
            Get
                Dim value As Unit = Unit.Percentage(99)

                If CType(Settings("SplitterWidth"), String) <> "" Then

                    Dim strUnit As String = CType(Settings("SplitterWidth"), String)

                    If strUnit.EndsWith("%") Then
                        Try
                            value = Unit.Percentage(Convert.ToDouble(strUnit.Replace("%", "")))
                        Catch
                        End Try
                    End If

                    If IsNumeric(strUnit) Then
                        Try
                            value = Unit.Pixel(Convert.ToInt32(strUnit))
                        Catch
                        End Try
                    End If

                End If

                Return value

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property EnforceEditPermissions() As Boolean
            Get
                If Settings.Contains("EnforceEditPermissions") Then
                    Return CType(Settings("EnforceEditPermissions"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property WelcomeArticleId() As Integer
            Get
                If CType(Settings("WelcomeArticleId"), String) <> "" Then
                    Return CType(Settings("WelcomeArticleId"), Integer)
                End If
                Return Null.NullInteger
            End Get
        End Property

    End Class
End Namespace

