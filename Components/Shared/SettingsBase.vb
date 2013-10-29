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
Imports DotNetNuke.Entities.Controllers
Imports DotNetNuke.Security.Permissions


Namespace dnnWerk.Modules.Nuntio.Articles
    Public MustInherit Class SettingsBase

        Inherits PortalModuleBase

        Public MustOverride Sub LoadForm()

#Region "Common Properties"

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ShowArticleCount() As Boolean
            Get
                If CType(Settings("ShowArticleCount"), String) <> "" Then
                    Return CType(Settings("ShowArticleCount"), Boolean)
                End If
                Return True
            End Get
        End Property

        Public ReadOnly Property ModuleDirectory() As String
            Get
                Return Me.ResolveUrl("~/Desktopmodules/Nuntio.Articles")
            End Get
        End Property

        Public Function ImagesDirectory() As String
            Return Me.ModuleDirectory & "/images/"
        End Function

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
        Public ReadOnly Property SupportedLocales() As LocaleCollection
            Get

                Dim ctrl As New LocaleController
                Dim dicLocales As Dictionary(Of String, Locale) = ctrl.GetLocales(PortalId)
                Dim colLocales As New LocaleCollection
                For Each objKey As String In dicLocales.Keys
                    colLocales.Add(objKey, dicLocales(objKey))
                Next

                Return colLocales

            End Get
        End Property


        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property CategoryList() As String
            Get
                Dim _list As String = ""
                If Settings.Contains("ShowCategory") Then
                    _list = CType(Settings("ShowCategory"), String)
                End If
                Return _list
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
        Public ReadOnly Property UiSkins() As String()
            Get
                Dim skins As String() = {"Default", "Black", "Forest", "Hay", "Office2007", "Outlook", "Sunset", "Vista", "Web20"}
                Return skins
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
        Public ReadOnly Property IncludePublications() As Boolean
            Get
                If Settings.Contains("IncludePublications") Then
                    Return CType(Settings("IncludePublications"), Boolean)
                End If
                Return False
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
        Public ReadOnly Property IncludeNonPublications() As Boolean
            Get
                If Settings.Contains("IncludeNonPublications") Then
                    Return CType(Settings("IncludeNonPublications"), Boolean)
                End If
                Return True
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
        Public ReadOnly Property ContentWidth() As String
            Get
                If CType(Settings("ContentWidth"), String) <> "" Then
                    Return CType(Settings("ContentWidth"), String)
                End If
                Return "70%"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property TocWidth() As String
            Get
                If CType(Settings("TocWidth"), String) <> "" Then
                    Return CType(Settings("TocWidth"), String)
                End If
                Return "30%"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property SplitterHeight() As String
            Get
                If CType(Settings("SplitterHeight"), String) <> "" Then
                    Return CType(Settings("SplitterHeight"), String)
                End If
                Return "600"
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property SplitterWidth() As String
            Get
                If CType(Settings("SplitterWidth"), String) <> "" Then
                    Return CType(Settings("SplitterWidth"), String)
                End If
                Return "99%"
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
                Return 0
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
        Public ReadOnly Property AllowTwitterPosts() As Boolean
            Get
                If CType(Settings("AllowTwitterPosts"), String) <> "" Then
                    Return CType(Settings("AllowTwitterPosts"), Boolean)
                End If
                Return False
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AllowJournalUpdates() As Boolean
            Get
                If CType(Settings("AllowJournalUpdates"), String) <> "" Then
                    Return CType(Settings("AllowJournalUpdates"), Boolean)
                End If
                Return True
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property NotificationDebugEnabled() As Boolean
            Get
                Return System.IO.File.Exists(Server.MapPath("/Desktopmodules/Nuntio.Articles/NotificationDebug.txt"))
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
                If CType(Settings("ArticleTheme"), String) <> "" Then
                    Return CType(Settings("ArticleTheme"), String)
                End If
                Return "Default"
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
        Public ReadOnly Property WelcomeArticleId() As Integer
            Get
                If CType(Settings("WelcomeArticleId"), String) <> "" Then
                    Return CType(Settings("WelcomeArticleId"), Integer)
                End If
                Return Null.NullInteger
            End Get
        End Property

#End Region

#Region "Protected Methods"

        Public Sub ClearArticleCache()

            DotNetNuke.Common.Utilities.DataCache.SetCache("NUNTIO_ARTICLES_ISDIRTY_" & ModuleId.ToString, True)

            For i As Integer = 0 To 999
                If Not DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_" & CurrentLocale & "_" & ModuleId.ToString & "_" & i.ToString) Is Nothing Then
                    DotNetNuke.Common.Utilities.DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_" & CurrentLocale & "_" & ModuleId.ToString & "_" & i.ToString)
                Else
                    Exit For
                End If
            Next

            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetArchive_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetAuthors_" & NewsModuleId.ToString)

        End Sub

        Public Sub BindModules(ByRef drpModule As DropDownList, ByVal IncludeSelf As Boolean)

            drpModule.Items.Clear()

            If IncludeSelf Then
                drpModule.Items.Add(New ListItem("Show articles from this module", ""))
            End If

            Dim tabs As List(Of DotNetNuke.Entities.Tabs.TabInfo) = TabController.GetPortalTabs(PortalSettings.PortalId, Null.NullInteger, False, "", True, False, False, False, False)

            Dim objModules As New ModuleController
            Dim arrModules As New ArrayList
            Dim objModule As ModuleInfo

            Dim dicModules As System.Collections.Generic.Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo)

            For Each oTab As DotNetNuke.Entities.Tabs.TabInfo In tabs
                dicModules = objModules.GetTabModules(oTab.TabID)
                For Each objModule In dicModules.Values
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "VIEW", objModule) And objModule.IsDeleted = False Then
                        If IsArticleModule(objModule, oTab.TabID) Then
                            drpModule.Items.Add(New ListItem("[" & oTab.TabName & "] - " & objModule.ModuleTitle, oTab.TabID & ";" & objModule.ModuleID.ToString))
                        End If
                    End If
                Next
            Next

        End Sub

        Private Function GetSharedResource(ByVal key As String) As String
            Dim path As String = Me.ModuleDirectory & "/" & DotNetNuke.Services.Localization.Localization.LocalResourceDirectory.ToLower & "/" & DotNetNuke.Services.Localization.Localization.LocalSharedResourceFile.ToLower
            path = "~" & path.ToLower.Substring(path.ToLower.IndexOf("/desktopmodules/"), path.Length - path.ToLower.IndexOf("/desktopmodules/"))
            Return DotNetNuke.Services.Localization.Localization.GetString(key, path)
        End Function

        Public Function Localize(ByVal InputString As String) As String
            Dim LocalizedResource As String = Null.NullString
            If (InputString.IndexOf(".Token") > 1) Or (InputString.IndexOf(".Mail") > 1) Then
                LocalizedResource = GetSharedResource(InputString)
            Else
                LocalizedResource = Services.Localization.Localization.GetString(InputString, LocalResourceFile)
            End If
            If LocalizedResource = Null.NullString Then
                LocalizedResource = GetSharedResource(InputString)
            End If
            If LocalizedResource = Null.NullString Then
                Dim _resourcefile As String = "/Desktopmodules/Nuntio.Articles/Controls/App_LocalResources/uc_Edit.ascx"
                LocalizedResource = Services.Localization.Localization.GetString(InputString, _resourcefile)
            End If
            If LocalizedResource = Null.NullString Then
                LocalizedResource = "Localized String not found"
            End If
            Return LocalizedResource
        End Function

        Protected Function IsArticleModule(ByVal objModule As Entities.Modules.ModuleInfo, ByVal TabId As Integer) As Boolean
            'Return (objModule.ModuleName = "Nuntio Articles") And (objModule.ModuleID <> ModuleId)

            Dim mSettings As Hashtable = Nothing
            Dim mC As New ModuleController
            Dim mModuleId As Integer = Null.NullInteger
            Dim mTabId As Integer = Null.NullInteger

            mSettings = mC.GetModuleSettings(objModule.ModuleID)

            If Not mSettings Is Nothing Then

                If mSettings.Contains("NewsModuleId") Then
                    If IsNumeric(mSettings("NewsModuleId")) Then
                        mModuleId = CType(mSettings("NewsModuleId"), Integer)
                    End If
                End If

                If mSettings.Contains("NewsModuleTab") Then
                    If IsNumeric(mSettings("NewsModuleTab")) Then
                        mTabId = CType(mSettings("NewsModuleTab"), Integer)
                    End If
                End If

                If (mTabId = TabId) AndAlso (mModuleId = objModule.ModuleID) Then
                    Return True
                End If

            End If

            Return False

        End Function

#End Region

    End Class
End Namespace

