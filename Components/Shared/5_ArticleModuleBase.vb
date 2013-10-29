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
Imports dnnWerk.Libraries.Nuntio.Localization


Namespace dnnWerk.Modules.Nuntio.Articles
    Public Class ArticleModuleBase
        Inherits TemplateBase

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property CanPublish() As Boolean
            Get

                Dim ctrl As New ModuleController
                Dim objModule As ModuleInfo = ctrl.GetModule(NewsModuleId)
                Dim blnCanEdit As Boolean = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", objModule)

                If blnCanEdit Then Return True

                If Me.AuthorizedRoles <> Null.NullString Then
                    'check if visitor is in of the subscribed roles
                    Dim roles As String() = Me.AuthorizedRoles.Split(Char.Parse(","))
                    If Not roles Is Nothing And roles.Length > 0 Then
                        For Each role As String In roles
                            If UserInfo.IsInRole(role) Then
                                Return True
                            End If
                        Next
                    End If
                End If

                Return False

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property CanAdmin() As Boolean
            Get

                Dim ctrl As New ModuleController
                Dim objModule As ModuleInfo = ctrl.GetModule(NewsModuleId)
                Dim blnCanEdit As Boolean = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", objModule)

                If blnCanEdit Then Return True

                Return False

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property CanEditArticle() As Boolean
            Get

                Dim ctrl As New ModuleController
                Dim objModule As ModuleInfo = ctrl.GetModule(NewsModuleId)
                Dim blnCanEdit As Boolean = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", objModule)

                If blnCanEdit Then Return True

                If ArticleId <> Null.NullInteger Then
                    Dim ctrlArticles As New ArticleController
                    Dim oArticle As ArticleInfo = ctrlArticles.GetArticle(ArticleId, CurrentLocale, True)
                    If Not oArticle Is Nothing Then
                        If oArticle.CreatedByUser = UserInfo.UserID Then
                            Return True
                        End If
                    End If
                End If

                Return False

            End Get
        End Property

        Public ReadOnly Property IsModuleCall() As Boolean
            Get
                If Request.QueryString("NewsModule") Is Nothing Then
                    Return True
                Else
                    If Integer.Parse(Request.QueryString("NewsModule")) = Me.NewsModuleId Then
                        Return True
                    Else
                        Return False
                    End If
                End If
            End Get
        End Property

#Region "Protected Helper Methods"

        Protected Function IsArticleModule(ByVal objModule As Entities.Modules.ModuleInfo) As Boolean
            Return (objModule.DesktopModule.ModuleName = "Nuntio Articles") And (objModule.ModuleID <> ModuleId)
        End Function

        Protected Function GetArticleCountInTreeNode(ByVal intCount As Integer) As String
            If ShowArticleCount Then

                If intCount = 0 Then
                    Return ""
                End If

                Return " (" & intCount.ToString & ")"

            End If
            Return ""
        End Function

        Protected Function IsRoleSubscribed(ByVal LookupSettings As Boolean, ByVal moduleId As Integer) As Boolean
            Dim mc As New Entities.Modules.ModuleController
            Dim s As New Hashtable
            s = mc.GetModuleSettings(moduleId)
            If s.Contains("SubscribedRoles") Then
                Dim roles As String() = CType(s("SubscribedRoles"), String).Split(Char.Parse(","))
                If Not roles Is Nothing And roles.Length > 0 Then
                    For Each role As String In roles
                        If UserInfo.IsInRole(role) Then
                            Return True
                        End If
                    Next
                End If
            End If
            Return False
        End Function

        Protected Function IsRoleSubscribed(ByVal moduleId As Integer) As Boolean
            If Me.SubscribedRoles <> Null.NullString Then
                'check if visitor is in of the subscribed roles
                Dim roles As String() = Me.SubscribedRoles.Split(Char.Parse(","))
                If Not roles Is Nothing And roles.Length > 0 Then
                    For Each role As String In roles
                        If UserInfo.IsInRole(role) Then
                            Return True
                        End If
                    Next
                End If
            End If
            Return False
        End Function

        Protected Function MySubscribedRoles() As String
            Dim sRoles As String = ""
            If Me.SubscribedRoles <> Null.NullString Then
                If Request.IsAuthenticated Then
                    Dim roles As String() = Me.SubscribedRoles.Split(Char.Parse(","))

                    If Not roles Is Nothing And roles.Length > 0 Then
                        For Each role As String In roles
                            If UserInfo.IsInRole(role) Then
                                sRoles = sRoles & role & ","
                            End If
                        Next
                        If sRoles <> "" Then
                            sRoles = sRoles.Substring(0, sRoles.Length - 2)
                        End If
                    End If
                End If
            End If
            Return sRoles
        End Function

        Protected Function IsSubscribed(ByVal moduleId As Integer) As Boolean
            If Request.IsAuthenticated Then
                Dim objSubscribe As SubscriptionInfo
                Dim SubscriptionController As New SubscriptionController
                'let's assume that subscription is allowed
                Dim blnIsSubscribed As Boolean = True
                Dim IsMigrated As Boolean = False
                'user is logged on
                'check wether subscription exists with email address of logged on user
                objSubscribe = SubscriptionController.GetSubscription(NewsModuleId, UserInfo.Email, Null.NullInteger)
                If objSubscribe Is Nothing Then
                    'no subscription active
                    'check wether subscription exists with provided user account
                    objSubscribe = SubscriptionController.GetSubscription(NewsModuleId, Null.NullString, UserInfo.UserID)
                    If objSubscribe Is Nothing Then
                        blnIsSubscribed = False
                    End If
                Else
                    'user has subscribed before adding a user accout
                    'we delete the existing subscription and add a new one
                    'with the user account
                    SubscriptionController.UnSubscribe(objSubscribe)
                    objSubscribe = New SubscriptionInfo(UserInfo.UserID, NewsModuleId)
                    SubscriptionController.Subscribe(objSubscribe)
                End If
                'check if user is in autosubscribed role
                If blnIsSubscribed = False Then
                    blnIsSubscribed = IsRoleSubscribed(moduleId)
                End If
                Return blnIsSubscribed
            Else
                Return False
            End If
        End Function

        Protected Function FormatURL(ByVal Link As String, ByVal TrackClicks As Boolean) As String
            Return LinkClick(Link, TabId, ModuleId, TrackClicks)
        End Function

        Public Function GetArticleIconPath(ByVal objItem As ArticleInfo) As String

            Dim strIcon As String = "article.png"

            If objItem.Url <> "" Then

                If IsNumeric(objItem.Url) Then

                    'link to tab
                    strIcon = "tab.gif"

                ElseIf objItem.Url.StartsWith("FileId=") Then

                    'link to file
                    strIcon = "file.gif"

                    Try
                        Dim f As DotNetNuke.Services.FileSystem.FileInfo = FileManager.Instance.GetFile(Integer.Parse(objItem.Url.Replace("FileId=", "")))
                        strIcon = f.Extension.Replace(".", "") & ".gif"
                    Catch
                    End Try

                ElseIf objItem.Url.StartsWith("UserId=") Then
                    'link to user
                    strIcon = "user.png"
                Else

                    'external url
                    strIcon = "html.gif"

                End If

            End If

            Return ImagesDirectory() & "ext/" & strIcon

        End Function

        Public Function Navigate(ByVal PreserveQuerystring As Boolean, ByVal PageName As String, ByVal ParamArray AdditionalParams() As String) As String
            Return Navigate(Null.NullInteger, PreserveQuerystring, PageName, AdditionalParams)
        End Function

        Public Function Navigate(ByVal NavigateTab As Integer, ByVal PreserveQuerystring As Boolean, ByVal PageName As String, ByVal ParamArray AdditionalParams() As String) As String

            If String.IsNullOrEmpty(PageName) Then PageName = "default.aspx"

            Dim strURL As String

            If NavigateTab <> Null.NullInteger Then
                strURL = ApplicationURL(NavigateTab)
            Else
                If TabId = Null.NullInteger Then
                    strURL = ApplicationURL()
                Else
                    strURL = ApplicationURL(TabId)
                End If
            End If

            Dim params As String = ""
            If PreserveQuerystring Then
                For Each key As String In Request.QueryString.AllKeys
                    'Me.Controls.Add(New LiteralControl(key & ","))
                    If Not String.IsNullOrEmpty(key) Then
                        If Not key.ToLower = "tabid" Then
                            If Not Helpers.IsInParamList(key, AdditionalParams) Then
                                strURL += "&" & key & "=" & Request.QueryString(key)
                            End If
                        End If
                    End If
                Next
            End If

            If params.EndsWith(", ") Then
                params = params.Substring(0, params.Length - 2)
            End If

            If Not (AdditionalParams Is Nothing) Then
                For Each parameter As String In AdditionalParams
                    strURL += "&" & parameter
                Next
            End If


            If DotNetNuke.Entities.Host.Host.UseFriendlyUrls = True Then

                Dim tabs As New List(Of DotNetNuke.Entities.Tabs.TabInfo)
                tabs = TabController.GetPortalTabs(PortalSettings.PortalId, Null.NullInteger, False, "", True, False, True, True, True)

                For Each objTab As DotNetNuke.Entities.Tabs.TabInfo In tabs

                    If objTab.TabID = NavigateTab Then
                        Return FriendlyUrl(objTab, strURL, PageName, PortalSettings)
                    End If

                Next

                Return FriendlyUrl(Nothing, strURL, PageName, PortalSettings)

            Else

                Return ResolveUrl(strURL)

            End If
        End Function

        Public Sub NotfiyWebmaster(ByVal SubscribedEmail As String, ByVal Locale As String, ByVal IsPortalUser As Boolean, ByVal PortalUserAccount As String, ByVal SubscribedName As String)
            If PortalUserAccount = Nothing Then
                PortalUserAccount = "-"
            End If
            DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, PortalSettings.Email, "", Localize("NewSubscriptionSubject.Mail"), String.Format(Localize("NewSubscriptionBody.Mail"), PortalSettings.PortalName, "<a href=""" & NavigateURL() & """>" & Me.ModuleConfiguration.ModuleTitle & "</a>", Locale, Localize(IsPortalUser.ToString.ToLower), PortalUserAccount, SubscribedEmail, SubscribedName), "", "HTML", "", "", "", "")
        End Sub

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

        Public Sub ClearArticleCache()

            DotNetNuke.Common.Utilities.DataCache.SetCache("NUNTIO_ARTICLES_ISDIRTY_" & NewsModuleId.ToString, True)

            For i As Integer = 0 To 999
                If Not DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_" & CurrentLocale & "_" & NewsModuleId.ToString & "_" & i.ToString) Is Nothing Then
                    DotNetNuke.Common.Utilities.DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_" & CurrentLocale & "_" & NewsModuleId.ToString & "_" & i.ToString)
                Else
                    Exit For
                End If
            Next

            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetFeatured_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetFeaturedCount_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetNeedsReviewing_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetNeedsReviewingCount_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetNotYetPublished_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetNotYetPublishedCount_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetExpiredArticles_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetExpiredArticleCount_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetUnapprovedArticles_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetUnapprovedArticlesCount_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetDeletedArticles_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetDeletedArticlesCount_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetArchive_" & NewsModuleId.ToString)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Nuntio_GetAuthors_" & NewsModuleId.ToString)

        End Sub

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ArticlesArchive() As List(Of ArchiveInfo)
            Get
                Dim list As New List(Of ArchiveInfo)


                If Not DataCache.GetCache("Nuntio_GetArchive_" & NewsModuleId.ToString) Is Nothing Then
                    Try
                        list = CType(DataCache.GetCache("Nuntio_GetArchive_" & NewsModuleId.ToString), List(Of ArchiveInfo))
                        Return list
                    Catch
                    End Try
                End If

                Dim ctrlArchive As New ArchiveController
                list = ctrlArchive.GetNewsArchive(NewsModuleId, CurrentLocale, Date.Now, ShowFutureItems, ShowPastItems)
                DataCache.SetCache("Nuntio_GetArchive_" & NewsModuleId.ToString, list)
                Return list

                Return list
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ArticleAuthors() As List(Of AuthorInfo)
            Get
                Dim list As New List(Of AuthorInfo)


                If Not DataCache.GetCache("Nuntio_GetAuthors_" & NewsModuleId.ToString) Is Nothing Then
                    Try
                        list = CType(DataCache.GetCache("Nuntio_GetAuthors_" & NewsModuleId.ToString), List(Of AuthorInfo))
                        Return list
                    Catch
                    End Try
                End If

                Dim authors As New List(Of AuthorInfo)
                Dim ctrlAuthor As New AuthorController
                authors = ctrlAuthor.GetAuthors(NewsModuleId, Date.Now, ShowFutureItems, ShowPastItems, IncludeExpired)
                DataCache.SetCache("Nuntio_GetAuthors_" & NewsModuleId.ToString, list)
                Return list


                Return list
            End Get
        End Property

        Private Function GetSharedResource(ByVal key As String) As String
            Dim path As String = Me.ModuleDirectory & "/" & DotNetNuke.Services.Localization.Localization.LocalResourceDirectory.ToLower & "/" & DotNetNuke.Services.Localization.Localization.LocalSharedResourceFile.ToLower
            path = "~" & path.ToLower.Substring(path.ToLower.IndexOf("/desktopmodules/"), path.Length - path.ToLower.IndexOf("/desktopmodules/"))
            Return DotNetNuke.Services.Localization.Localization.GetString(key, path)
        End Function

        Private Function GetMailResource(ByVal key As String) As String
            Dim path As String = Me.ModuleDirectory & "/" & DotNetNuke.Services.Localization.Localization.LocalResourceDirectory.ToLower & "/NotificationResources.resx"
            path = "~" & path.ToLower.Substring(path.ToLower.IndexOf("/desktopmodules/"), path.Length - path.ToLower.IndexOf("/desktopmodules/"))
            Return DotNetNuke.Services.Localization.Localization.GetString(key, path)
        End Function

#End Region

#Region "Protected Template Methods"

        Protected Sub ProcessAttachmentTemplate(ByRef strTemplate As String, ByVal objArticle As ArticleInfo, ByVal objFile As ArticleFileInfo)

            strTemplate = strTemplate.Replace("[FILEURL]", objFile.Url)
            strTemplate = strTemplate.Replace("[ITEMID]", objFile.ArticleId.ToString)
            strTemplate = strTemplate.Replace("[FILEID]", objFile.FileId)
            strTemplate = strTemplate.Replace("[FOLDERNAME]", objFile.Folder)

            strTemplate = strTemplate.Replace("[FILEDESCRIPTION]", objFile.ImageDescription)
            strTemplate = strTemplate.Replace("[FILETITLE]", objFile.ImageTitle)
            strTemplate = strTemplate.Replace("[FILEID]", objFile.ArticleFileId.ToString)
            strTemplate = strTemplate.Replace("[ARTICLETITLE]", objArticle.Title)
            strTemplate = strTemplate.Replace("[ARTICLESUMMARY]", objArticle.Summary)
            strTemplate = strTemplate.Replace("[ARTICLECONTENT]", objArticle.Content)
            strTemplate = strTemplate.Replace("[PORTALID]", objArticle.PortalID)

            Dim filename As String = objFile.FileName.Substring(0, objFile.FileName.LastIndexOf(".")).ToLower
            Dim fileextension As String = objFile.FileName.Substring(objFile.FileName.LastIndexOf(".") + 1).ToLower

            strTemplate = strTemplate.Replace("[FILENAME]", filename)
            strTemplate = strTemplate.Replace("[FILEEXTENSION]", fileextension)

        End Sub

        Protected Sub ProcessImageTemplate(ByRef strTemplate As String, ByVal objArticle As ArticleInfo, ByVal objImage As ArticleFileInfo)

            strTemplate = strTemplate.Replace("[IMAGEURL]", objImage.Url)
            strTemplate = strTemplate.Replace("[ITEMID]", objImage.ArticleId.ToString)
            strTemplate = strTemplate.Replace("[FILEID]", objImage.FileId)
            strTemplate = strTemplate.Replace("[FOLDERNAME]", objImage.Folder)

            strTemplate = strTemplate.Replace("[IMAGEDESCRIPTION]", objImage.ImageDescription)
            strTemplate = strTemplate.Replace("[IMAGETITLE]", objImage.ImageTitle)
            strTemplate = strTemplate.Replace("[IMAGEID]", objImage.ArticleFileId.ToString)
            strTemplate = strTemplate.Replace("[ARTICLETITLE]", objArticle.Title)
            strTemplate = strTemplate.Replace("[ARTICLESUMMARY]", objArticle.Summary)
            strTemplate = strTemplate.Replace("[ARTICLECONTENT]", objArticle.Content)
            strTemplate = strTemplate.Replace("[PORTALID]", objArticle.PortalID)

            Dim filename As String = objImage.FileName.Substring(0, objImage.FileName.LastIndexOf(".")).ToLower
            Dim fileextension As String = objImage.FileName.Substring(objImage.FileName.LastIndexOf(".") + 1).ToLower


            strTemplate = strTemplate.Replace("[FILENAME]", filename)
            strTemplate = strTemplate.Replace("[FILEEXTENSION]", fileextension)

        End Sub

        '<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId:="System.Int32.ToString")> _
        'Protected Sub ProcessItemBody(ByRef controls As System.Web.UI.ControlCollection, ByVal objItem As ArticleInfo, ByVal Template As String, ByVal SearchKey As String, ByVal LoadFiles As Boolean, ByVal TargetTabId As Integer)

        '    Dim categories As New List(Of Integer)
        '    Dim CategoryList As New List(Of CategoryInfo)
        '    Dim cc As New CategoryController
        '    categories = cc.GetRelationsByitemId(objItem.ItemId)
        '    For Each item As Integer In categories
        '        Try
        '            CategoryList.Add(cc.GetCategory(item, objItem.moduleId, Me.CurrentLocale, UseOriginalVersion))
        '        Catch
        '        End Try
        '    Next

        '    If LoadFiles Then
        '        Dim ctrlAttachments As New ArticleFileController
        '        objItem.Images = ctrlAttachments.GetImages(objItem.ItemId, NewsModuleId, CurrentLocale, UseOriginalVersion)
        '        objItem.Attachments = ctrlAttachments.GetAttachments(objItem.ItemId, NewsModuleId, CurrentLocale, UseOriginalVersion)
        '    End If

        '    'Dim literal As New Literal
        '    Dim delimStr As String = "[]"
        '    Dim delimiter As Char() = delimStr.ToCharArray()

        '    Dim templateArray As String()
        '    templateArray = Template.Split(delimiter)

        '    For iPtr As Integer = 0 To templateArray.Length - 1 Step 2

        '        controls.Add(New LiteralControl(Helpers.ProcessImages(templateArray(iPtr).ToString())))

        '        If iPtr < templateArray.Length - 1 Then

        '            Select Case templateArray(iPtr + 1)

        '                Case "ANCHORLINK"


        '                    If String.IsNullOrEmpty(objItem.AnchorLink) Then
        '                        controls.Add(New LiteralControl("NUA-" & objItem.ItemId.ToString))
        '                    Else
        '                        controls.Add(New LiteralControl(objItem.AnchorLink))
        '                    End If

        '                Case "PORTALID"

        '                    controls.Add(New LiteralControl(PortalId.ToString))

        '                Case "ATTACHMENTS"

        '                    'process attachments
        '                    For Each objFile As ArticleFileInfo In objItem.Attachments
        '                        Dim strFile As String = Me.AttachmentTemplate
        '                        ProcessAttachmentTemplate(strFile, objItem, objFile)
        '                        controls.Add(New LiteralControl(strFile))
        '                    Next


        '                Case "HASATTACHMENTS"

        '                    If objItem.Attachments.Count = 0 Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASATTACHMENTS") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If


        '                Case "HASNOATTACHMENTS"

        '                    If objItem.Attachments.Count > 0 Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASNOATTACHMENTS") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASPRIMARYATTACHMENT"

        '                    Dim blnHas As Boolean = False
        '                    For Each objFile As ArticleFileInfo In objItem.Attachments
        '                        If objFile.IsPrimary Then
        '                            blnHas = True
        '                            Exit For
        '                        End If
        '                    Next

        '                    If blnHas = False Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASPRIMARYATTACHMENT") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASNOPRIMARYATTACHMENT"

        '                    Dim blnHas As Boolean = False
        '                    For Each objFile As ArticleFileInfo In objItem.Attachments
        '                        If objFile.IsPrimary Then
        '                            blnHas = True
        '                            Exit For
        '                        End If
        '                    Next

        '                    If blnHas = True Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASNOPRIMARYATTACHMENT") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If


        '                Case "PRIMARYATTACHMENTURL"

        '                    Dim strUrl As String = ""
        '                    For Each objFile As ArticleFileInfo In objItem.Attachments
        '                        If objFile.IsPrimary Then
        '                            strUrl = objFile.Url
        '                            Exit For
        '                        End If
        '                    Next
        '                    controls.Add(New LiteralControl(strUrl))

        '                Case "PRIMARYATTACHMENTTITLE"

        '                    Dim strTitle As String = ""
        '                    For Each objFile As ArticleFileInfo In objItem.Attachments
        '                        If objFile.IsPrimary Then
        '                            strTitle = objFile.ImageTitle
        '                            Exit For
        '                        End If
        '                    Next
        '                    controls.Add(New LiteralControl(strTitle))

        '                Case "PRIMARYATTACHMENTDESCRIPTION"

        '                    Dim strDescription As String = ""
        '                    For Each objFile As ArticleFileInfo In objItem.Attachments
        '                        If objFile.IsPrimary Then
        '                            strDescription = objFile.ImageDescription
        '                            Exit For
        '                        End If
        '                    Next
        '                    controls.Add(New LiteralControl(strDescription))

        '                Case "IMAGEGALLERY"

        '                    'process images
        '                    For Each objImage As ArticleFileInfo In objItem.Images
        '                        Dim strImage As String = Me.ImageTemplate
        '                        ProcessImageTemplate(strImage, objItem, objImage)
        '                        controls.Add(New LiteralControl(strImage))
        '                    Next

        '                Case "HASIMAGES"

        '                    If objItem.Images.Count = 0 Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASIMAGES") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If


        '                Case "HASNOIMAGES"

        '                    If objItem.Images.Count > 0 Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASNOIMAGES") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASPRIMARYIMAGE"

        '                    Dim blnHas As Boolean = False
        '                    For Each objImage As ArticleFileInfo In objItem.Images
        '                        If objImage.IsPrimary Then
        '                            blnHas = True
        '                            Exit For
        '                        End If
        '                    Next

        '                    If blnHas = False Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASPRIMARYIMAGE") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASNOPRIMARYIMAGE"

        '                    Dim blnHas As Boolean = False
        '                    For Each objImage As ArticleFileInfo In objItem.Images
        '                        If objImage.IsPrimary Then
        '                            blnHas = True
        '                            Exit For
        '                        End If
        '                    Next

        '                    If blnHas = True Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASNOPRIMARYIMAGE") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If


        '                Case "PRIMARYIMAGEURL"

        '                    Dim strUrl As String = ""
        '                    For Each objImage As ArticleFileInfo In objItem.Images
        '                        If objImage.IsPrimary Then
        '                            strUrl = objImage.Url
        '                            Exit For
        '                        End If
        '                    Next
        '                    controls.Add(New LiteralControl(strUrl))

        '                Case "PRIMARYIMAGETITLE"

        '                    Dim strTitle As String = ""
        '                    For Each objImage As ArticleFileInfo In objItem.Images
        '                        If objImage.IsPrimary Then
        '                            strTitle = objImage.ImageTitle
        '                            Exit For
        '                        End If
        '                    Next
        '                    controls.Add(New LiteralControl(strTitle))

        '                Case "PRIMARYIMAGEDESCRIPTION"

        '                    Dim strDescription As String = ""
        '                    For Each objImage As ArticleFileInfo In objItem.Images
        '                        If objImage.IsPrimary Then
        '                            strDescription = objImage.ImageDescription
        '                            Exit For
        '                        End If
        '                    Next
        '                    controls.Add(New LiteralControl(strDescription))

        '                Case "ITEMID"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.ItemId.ToString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "ArticleId"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = "NEWS_" & objItem.ItemId.ToString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "TEMPLATEPATH"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = Me.ModuleDirectory & "/templates"
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "ORIGINALLINK"

        '                    If objItem.IsOriginal = False Then

        '                        Dim sOrgLocale As String = ""
        '                        Dim ctrl As New ContentController(objItem.moduleId, CurrentLocale, False)
        '                        Dim lst As List(Of ContentInfo) = ctrl.GetAllContent

        '                        If lst.Count > 0 Then
        '                            For Each item As ContentInfo In lst
        '                                If item.SourceItemId = objItem.ItemId And item.IsOriginal Then
        '                                    sOrgLocale = item.Locale
        '                                    Exit For
        '                                End If
        '                            Next
        '                        End If
        '                        If sOrgLocale <> "" Then
        '                            Dim params As New ArrayList
        '                            For Each key As String In Request.QueryString.AllKeys
        '                                Dim sValue As String = Request.QueryString(key)
        '                                If key.ToLower <> "tabid" And key.ToLower <> "language" Then
        '                                    params.Add(key & "=" & sValue)
        '                                End If
        '                            Next
        '                            params.Add("language=" & sOrgLocale)
        '                            Dim pArr(params.Count - 1) As String
        '                            For i As Integer = 0 To params.Count - 1
        '                                pArr(i) = params(i).ToString
        '                            Next

        '                            Dim objLiteral As New HyperLink
        '                            objLiteral.Text = Localize("ViewOriginal.Token")
        '                            Dim tid As Integer = NewsModuleTab
        '                            If TargetTabId <> Null.NullInteger Then
        '                                tid = TargetTabId
        '                            End If

        '                            objLiteral.NavigateUrl = NavigateURL(tid, "", pArr)
        '                            objLiteral.EnableViewState = False
        '                            controls.Add(objLiteral)
        '                        End If

        '                    End If

        '                Case "EDIT"

        '                    Dim ctrl As New ModuleController
        '                    Dim objModule As ModuleInfo = ctrl.GetModule(NewsModuleId)

        '                    Dim blnHasEditPermission As Boolean = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", objModule)
        '                    Dim blnIsOwner As Boolean = (Request.IsAuthenticated = True AndAlso UserInfo.UserID = objItem.CreatedByUser)
        '                    Dim blnCanEdit As Boolean = False

        '                    If blnHasEditPermission Then
        '                        blnCanEdit = True
        '                    Else
        '                        If blnIsOwner Then
        '                            If EnforceEditPermissions = True Then
        '                                blnCanEdit = False
        '                            Else
        '                                blnCanEdit = True
        '                            End If
        '                        Else
        '                            blnCanEdit = False
        '                        End If
        '                    End If

        '                    If blnCanEdit Then
        '                        Dim objLiteral As New HyperLink
        '                        objLiteral.NavigateUrl = "javascript:NuntioArticlesEditForm('" & objItem.ItemId.ToString & "','" & NewsModuleId.ToString & "','" & ModuleId.ToString & "')"
        '                        objLiteral.ImageUrl = ImagesDirectory() & "nuntio_edit.png"
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If

        '                Case "PUBLISHDATE"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.PublishDate.ToShortDateString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "LASTUPDATEDDATE"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.LastUpdatedDate.ToShortDateString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "LASTUPDATEDTIME"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.LastUpdatedDate.ToShortTimeString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "PUBLISHDAY"

        '                    Dim strDay As String = objItem.PublishDate.Day.ToString
        '                    If strDay.Length = 1 Then
        '                        strDay = "0" & strDay
        '                    End If

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = strDay
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "PUBLISHMONTH"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.PublishDate.Month.ToString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "PUBLISHMONTHSHORT"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = MonthName(objItem.PublishDate.Month).Substring(0, 3)
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "PUBLISHMONTHNAME"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = MonthName(objItem.PublishDate.Month)
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "PUBLISHYEAR"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.PublishDate.Year.ToString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "PUBLISHDATELONG"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.PublishDate.ToLongDateString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "PUBLISHDATESHORT"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.PublishDate.ToShortDateString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)


        '                Case "CREATEDDATE", "CREATEDDATESHORT"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.CreatedDate.ToShortDateString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)


        '                Case "CREATEDDATELONG"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.CreatedDate.ToLongDateString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)


        '                Case "CREATEDTIME"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.CreatedDate.ToShortTimeString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "CREATEDBYUSER"

        '                    Dim objUser As New UserInfo
        '                    objUser = UserController.GetUserById(PortalId, objItem.CreatedByUser)
        '                    If Not objUser Is Nothing Then
        '                        Dim objLiteral As New Literal
        '                        objLiteral.Text = objUser.DisplayName
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If

        '                Case "CREATEDBYUSERID"

        '                    Dim objUser As New UserInfo
        '                    objUser = UserController.GetUserById(PortalId, objItem.CreatedByUser)
        '                    If Not objUser Is Nothing Then
        '                        Dim objLiteral As New Literal
        '                        objLiteral.Text = objUser.UserID.ToString
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If

        '                Case "LASTUPDATEDBYUSER"

        '                    Dim objUser As New UserInfo
        '                    objUser = UserController.GetUserById(PortalId, objItem.LastUpdatedBy)
        '                    If Not objUser Is Nothing Then
        '                        Dim objLiteral As New Literal
        '                        objLiteral.Text = objUser.DisplayName
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If

        '                Case "LASTUPDATEDBYUSERID"

        '                    Dim objUser As New UserInfo
        '                    objUser = UserController.GetUserById(PortalId, objItem.LastUpdatedBy)
        '                    If Not objUser Is Nothing Then
        '                        Dim objLiteral As New Literal
        '                        objLiteral.Text = objUser.UserID.ToString
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If

        '                Case "SUMMARY"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = Helpers.FormatSearchString(Server.HtmlDecode(objItem.Summary), SearchKey)
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)


        '                Case "CONTENT"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = Helpers.FormatSearchString(Server.HtmlDecode(objItem.Content), SearchKey)
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "TITLE"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = Helpers.FormatSearchString(objItem.Title, SearchKey)
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "NEWSLINK"

        '                    If objItem.Url <> "" Then

        '                        Dim Trackingcontroller As New DotNetNuke.Common.Utilities.UrlController
        '                        Dim Tracking As DotNetNuke.Common.Utilities.UrlTrackingInfo = Trackingcontroller.GetUrlTracking(PortalId, objItem.Url, NewsModuleId)
        '                        Dim Track As Boolean = False
        '                        Dim Target As String = "_self"
        '                        If Not Tracking Is Nothing Then
        '                            Track = Tracking.TrackClicks
        '                            If Tracking.NewWindow Then
        '                                Target = "_blank"
        '                            End If
        '                        End If

        '                        Dim objLiteral As New HyperLink
        '                        'objLiteral.ID = CreateValidID("hypNewsLink" & objItem.itemId.ToString & ModuleId.ToString)
        '                        objLiteral.Text = Localize("ReadMore.Token")
        '                        objLiteral.NavigateUrl = FormatURL(objItem.Url, objItem.TrackClicks)
        '                        objLiteral.Target = Target

        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If


        '                Case "EXTENSIONICON"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = GetArticleIconPath(objItem)
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "NEWSLINKURL"

        '                    If objItem.Url <> "" Then
        '                        Dim objLiteral As New Literal
        '                        objLiteral.Text = FormatURL(objItem.Url, objItem.TrackClicks)
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If

        '                Case "NEWSLINKTITLE"

        '                    Dim strText As String = ""

        '                    If objItem.Url <> "" Then

        '                        If IsNumeric(objItem.Url) Then

        '                            'link to tab
        '                            strText = Localize("ReadMore.Token")

        '                        ElseIf objItem.Url.StartsWith("FileId=") Then
        '                            'link to file
        '                            Dim strFilename As String = Localize("File")
        '                            Try
        '                                Dim f As DotNetNuke.Services.FileSystem.FileInfo = FileManager.Instance.GetFile(Integer.Parse(objItem.Url.Replace("FileId=", "")))
        '                                strFilename = f.FileName
        '                            Catch
        '                            End Try
        '                            strText = String.Format(Localize("DownloadFile.Token"), strFilename)

        '                        ElseIf objItem.Url.StartsWith("UserId=") Then
        '                            'link to user
        '                            Dim strUser As String = Localize("User")
        '                            Try
        '                                strUser = UserController.GetUserById(PortalSettings.PortalId, Integer.Parse(objItem.Url.Replace("UserId=", ""))).DisplayName
        '                            Catch
        '                            End Try
        '                            strText = String.Format(Localize("VisitUser.Token"), strUser)
        '                        Else

        '                            'external url
        '                            strText = Localize("ReadMore.Token")

        '                        End If

        '                        Dim objLiteral As New Literal
        '                        objLiteral.Text = strText
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)

        '                    End If

        '                Case "NEWSLINKTARGET"

        '                    If objItem.Url <> "" Then

        '                        Dim Trackingcontroller As New DotNetNuke.Common.Utilities.UrlController
        '                        Dim Tracking As DotNetNuke.Common.Utilities.UrlTrackingInfo = Trackingcontroller.GetUrlTracking(PortalId, objItem.Url, NewsModuleId)
        '                        Dim Target As String = "_self"
        '                        If Not Tracking Is Nothing Then
        '                            If Tracking.NewWindow Then
        '                                Target = "_blank"
        '                            End If
        '                        End If

        '                        Dim objLiteral As New Literal
        '                        objLiteral.Text = Target
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)

        '                    End If


        '                Case "DETAILLINK", "ITEMLINK"

        '                    Dim objLiteral As New HyperLink
        '                    'objLiteral.ID = CreateValidID("hypItemItem" & objItem.itemId.ToString & ModuleId.ToString)
        '                    objLiteral.Text = Localize("ReadItem.Token")
        '                    Dim tid As Integer = NewsModuleTab
        '                    If TargetTabId <> Null.NullInteger Then
        '                        tid = TargetTabId
        '                    End If
        '                    objLiteral.NavigateUrl = Navigate(tid, True, Helpers.OnlyAlphaNumericChars(objItem.Title) & ".aspx", "ArticleId=" & objItem.ItemId)

        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "DETAILLINKURL", "ITEMLINKURL"

        '                    Dim tid As Integer = NewsModuleTab
        '                    If TargetTabId <> Null.NullInteger Then
        '                        tid = TargetTabId
        '                    End If

        '                    Dim url As String = Navigate(tid, True, Helpers.OnlyAlphaNumericChars(objItem.Title) & ".aspx", "ArticleId=" & objItem.ItemId)

        '                    Dim objLiteral As New Literal
        '                    'objLiteral.ID = CreateValidID("hypItemItemUrl" & objItem.itemId.ToString & ModuleId.ToString)
        '                    objLiteral.Text = url
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "DETAILWINDOWLINK"

        '                    Dim objLiteral As New HyperLink
        '                    'objLiteral.ID = CreateValidID("hypItemItem" & objItem.itemId.ToString & ModuleId.ToString)
        '                    objLiteral.Text = Localize("ReadItem.Token")
        '                    objLiteral.NavigateUrl = "javascript:NuntioArticlesShowArticle('" & objItem.ItemId.ToString & "','" & ModuleId.ToString & "','" & ArticleTheme & "')"
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "YEARLINK"

        '                    Dim objLiteral As New HyperLink
        '                    'objLiteral.ID = CreateValidID("hypItemDetail" & objItem.itemId.ToString & ModuleId.ToString)
        '                    objLiteral.Text = Localize("ReadItem.Token")
        '                    Dim tid As Integer = NewsModuleTab
        '                    If TargetTabId <> Null.NullInteger Then
        '                        tid = TargetTabId
        '                    End If
        '                    objLiteral.NavigateUrl = NavigateURL(tid, "", "Year=" & Year(objItem.PublishDate).ToString, "NewsModule=" & NewsModuleId.ToString) & "#NEWS" & objItem.ItemId.ToString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "YEARLINKURL"

        '                    Dim objLiteral As New Literal
        '                    Dim tid As Integer = NewsModuleTab
        '                    If TargetTabId <> Null.NullInteger Then
        '                        tid = TargetTabId
        '                    End If
        '                    'objLiteral.ID = CreateValidID("hypItemDetailUrl" & objItem.itemId.ToString & ModuleId.ToString)
        '                    objLiteral.Text = NavigateURL(tid, "", "Year=" & Year(objItem.PublishDate).ToString, "NewsModule=" & NewsModuleId.ToString) & "#NEWS" & objItem.ItemId.ToString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "MONTHLINK"

        '                    Dim objLiteral As New HyperLink
        '                    'objLiteral.ID = CreateValidID("hypItemDetail" & objItem.itemId.ToString & ModuleId.ToString)
        '                    objLiteral.Text = Localize("ReadItem.Token")
        '                    Dim tid As Integer = NewsModuleTab
        '                    If TargetTabId <> Null.NullInteger Then
        '                        tid = TargetTabId
        '                    End If
        '                    objLiteral.NavigateUrl = NavigateURL(tid, "", "Year=" & Year(objItem.PublishDate).ToString, "Month=" & Month(objItem.PublishDate).ToString, "NewsModule=" & NewsModuleId.ToString) & "#NEWS" & objItem.ItemId.ToString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "MONTHLINKURL"

        '                    Dim objLiteral As New Literal
        '                    'objLiteral.ID = CreateValidID("hypItemDetailUrl" & objItem.itemId.ToString & ModuleId.ToString)
        '                    Dim tid As Integer = NewsModuleTab
        '                    If TargetTabId <> Null.NullInteger Then
        '                        tid = TargetTabId
        '                    End If
        '                    objLiteral.Text = NavigateURL(tid, "", "Year=" & Year(objItem.PublishDate).ToString, "Month=" & Month(objItem.PublishDate).ToString, "NewsModule=" & NewsModuleId.ToString) & "#NEWS" & objItem.ItemId.ToString
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)


        '                Case "AUTHORLINKURL"

        '                    Dim objLiteral As New Literal
        '                    'objLiteral.ID = CreateValidID("hypItemDetailUrl" & objItem.itemId.ToString & ModuleId.ToString)
        '                    Dim tid As Integer = NewsModuleTab
        '                    If TargetTabId <> Null.NullInteger Then
        '                        tid = TargetTabId
        '                    End If
        '                    objLiteral.Text = NavigateURL(tid, "", "AuthorId=" & objItem.CreatedByUser.ToString)
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "ALLNEWSLINK"

        '                    If Me.ArticleId <> Null.NullInteger Then
        '                        Dim objLiteral As New HyperLink
        '                        ' objLiteral.ID = CreateValidID("hypAllNews" & ModuleId.ToString)
        '                        objLiteral.Text = Localize("AllNews.Token")

        '                        Dim tid As Integer = NewsModuleTab
        '                        If TargetTabId <> Null.NullInteger Then
        '                            tid = TargetTabId
        '                        End If
        '                        objLiteral.NavigateUrl = NavigateURL(tid)
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If

        '                Case "ALLNEWSLINKURL"

        '                    If Me.ArticleId <> Null.NullInteger Then
        '                        Dim objLiteral As New Literal
        '                        'objLiteral.ID = CreateValidID("hypAllNewsUrl" & ModuleId.ToString)
        '                        Dim tid As Integer = NewsModuleTab
        '                        If TargetTabId <> Null.NullInteger Then
        '                            tid = TargetTabId
        '                        End If
        '                        objLiteral.Text = NavigateURL(tid)
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If

        '                Case "FILEDUNDER"

        '                    If CategoryList.Count > 0 Then
        '                        Dim objLiteral As New Literal
        '                        'objLiteral.ID = CreateValidID("FiledUnder" & ModuleId.ToString)
        '                        objLiteral.Text = Localize("FiledUnder")
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If

        '                Case "CATEGORYLIST"
        '                    If CategoryList.Count > 0 Then
        '                        Dim sCategories As String = ""
        '                        For Each oCat As CategoryInfo In CategoryList
        '                            sCategories = sCategories & "<a href=""" & Navigate(False, Helpers.OnlyAlphaNumericChars(oCat.CategoryName) & ".aspx", "CategoryID=" & oCat.CategoryID.ToString) & """>" & oCat.CategoryName & "</a>, "
        '                        Next
        '                        If sCategories.EndsWith(", ") Then
        '                            sCategories = sCategories.Substring(0, sCategories.Length - 2)
        '                        End If
        '                        Dim objLiteral As New Literal
        '                        ' objLiteral.ID = CreateValidID("FiledUnderTags" & ModuleId.ToString)
        '                        objLiteral.Text = sCategories
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)
        '                    End If

        '                Case "HASSUMMARY"

        '                    If objItem.Summary = Null.NullString Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASSUMMARY") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASNOSUMMARY"

        '                    If objItem.Summary <> Null.NullString Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASNOSUMMARY") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASRELATEDARTICLES"

        '                    If objItem.RelatedArticles.Count = 0 Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASRELATEDARTICLES") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASNORELATEDARTICLES"

        '                    If objItem.RelatedArticles.Count > 0 Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASNORELATEDARTICLES") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "RELATEDARTICLESLIST"

        '                    If objItem.RelatedArticles.Count > 0 Then



        '                        For Each oArticle As ArticleInfo In objItem.RelatedArticles

        '                            Dim strMarkup As String = "<li>"
        '                            strMarkup += "<a href=""" & Navigate(TabId, True, Helpers.OnlyAlphaNumericChars(oArticle.Title) & ".aspx", "ArticleId=" & oArticle.ItemId) & """>"
        '                            strMarkup += oArticle.Title
        '                            strMarkup += "</a></li>"

        '                            controls.Add(New LiteralControl(strMarkup))

        '                        Next

        '                    End If


        '                Case "PUBLICATIONMEMBERS"

        '                    If objItem.ParentId <> Null.NullInteger Then

        '                        Dim oParent As ArticleInfo = ArticleController.GetArticle(objItem.ParentId, CurrentLocale, True)
        '                        If Not oParent Is Nothing Then
        '                            For Each oArticle As ArticleInfo In oParent.ChildArticles

        '                                Dim strMarkup As String = "<li>"
        '                                strMarkup += "<a href=""" & Navigate(TabId, True, Helpers.OnlyAlphaNumericChars(oArticle.Title) & ".aspx", "ArticleId=" & oArticle.ItemId) & """>"
        '                                strMarkup += oArticle.Title
        '                                strMarkup += "</a></li>"

        '                                controls.Add(New LiteralControl(strMarkup))

        '                            Next


        '                        End If
        '                    End If

        '                Case "PUBLICATIONURL"

        '                    Dim oParent As ArticleInfo = ArticleController.GetArticle(objItem.ParentId, CurrentLocale, True)

        '                    If oParent Is Nothing Then
        '                        oParent = ArticleController.GetArticle(objItem.ItemId, CurrentLocale, True)
        '                    End If

        '                    If Not oParent Is Nothing Then
        '                        Dim strParent As String = Navigate(TabId, True, Helpers.OnlyAlphaNumericChars(oParent.Title) & ".aspx", "ArticleId=" & oParent.ItemId)

        '                        controls.Add(New LiteralControl(strParent))
        '                    End If


        '                Case "ISPUBLICATIONMEMBER"

        '                    If objItem.ParentId = Null.NullInteger Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/ISPUBLICATIONMEMBER") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "ISPUBLICATION"

        '                    If Not objItem.IsPublication Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/ISPUBLICATION") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "ISNOPUBLICATION"

        '                    If objItem.IsPublication Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/ISNOPUBLICATION") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "PUBLICATIONARTICLES"

        '                    If objItem.IsPublication Then
        '                        For Each oArticle As ArticleInfo In objItem.ChildArticles

        '                            Dim strMarkup As String = "<li>"
        '                            strMarkup += "<a href=""" & Navigate(TabId, True, Helpers.OnlyAlphaNumericChars(oArticle.Title) & ".aspx", "ArticleId=" & oArticle.ItemId) & """>"
        '                            strMarkup += oArticle.Title
        '                            strMarkup += "</a></li>"

        '                            controls.Add(New LiteralControl(strMarkup))

        '                        Next
        '                    End If

        '                Case "HASCOMMENTS"

        '                    If objItem.CreatedByUser = UserInfo.UserID Then

        '                        Dim commentscontroller As New CommentController
        '                        Dim comments As New List(Of CommentInfo)
        '                        comments = commentscontroller.List(objItem.ItemId)
        '                        If comments.Count = 0 Then
        '                            While (iPtr < templateArray.Length - 1)
        '                                If (templateArray(iPtr + 1) = "/HASCOMMENTS") Then
        '                                    Exit While
        '                                End If
        '                                iPtr = iPtr + 1
        '                            End While
        '                        End If

        '                    Else
        '                        If objItem.Comments = 0 Then
        '                            While (iPtr < templateArray.Length - 1)
        '                                If (templateArray(iPtr + 1) = "/HASCOMMENTS") Then
        '                                    Exit While
        '                                End If
        '                                iPtr = iPtr + 1
        '                            End While
        '                        End If
        '                    End If


        '                Case "HASNOCOMMENTS"

        '                    If objItem.Comments > 0 Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASNOCOMMENTS") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "COMMENTURL"

        '                    Dim objLiteral As New Literal
        '                    'objLiteral.ID = CreateValidID("hypItemItem" & objItem.itemId.ToString & ModuleId.ToString)
        '                    objLiteral.Text = "javascript:NuntioArticlesCommentForm('" & objItem.ItemId.ToString & "','-1','" & TabId.ToString & "','" & NewsModuleId.ToString & "','" & PortalId.ToString & "','" & ArticleTheme.ToString & "')"
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "COMMENTCOUNT"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = objItem.Comments.ToString
        '                    objLiteral.EnableViewState = False

        '                    If objItem.CreatedByUser = UserInfo.UserID Then

        '                        Dim commentscontroller As New CommentController
        '                        Dim comments As New List(Of CommentInfo)
        '                        comments = commentscontroller.List(objItem.ItemId)
        '                        objLiteral.Text = comments.Count.ToString

        '                    Else
        '                        objLiteral.Text = objItem.Comments.ToString
        '                    End If

        '                    controls.Add(objLiteral)


        '                Case "COMMENTLIST"

        '                    ProcessComments(controls, CommentTemplate, objItem)

        '                Case "ISNOTIMAGE", "HASNOIMAGE"

        '                    Dim blnHasImage As Boolean = False

        '                    If objItem.Url <> Null.NullString Then
        '                        If objItem.Url.StartsWith("FileId=") Then

        '                            Dim strFilename As String = ""
        '                            Try
        '                                Dim f As DotNetNuke.Services.FileSystem.FileInfo = FileManager.Instance.GetFile(Integer.Parse(objItem.Url.Replace("FileId=", "")))
        '                                strFilename = f.FileName
        '                            Catch
        '                            End Try
        '                            If strFilename.ToLower.EndsWith(".jpg") Or _
        '                                strFilename.ToLower.EndsWith(".jpeg") Or _
        '                                strFilename.ToLower.EndsWith(".png") Or _
        '                                strFilename.ToLower.EndsWith(".gif") Then

        '                                blnHasImage = True

        '                            End If

        '                        End If
        '                    End If

        '                    If blnHasImage = True Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/ISNOTIMAGE") Or (templateArray(iPtr + 1) = "/HASNOIMAGE") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASIMAGE"

        '                    Dim blnHasImage As Boolean = False

        '                    If objItem.Url <> Null.NullString Then
        '                        If objItem.Url.StartsWith("FileId=") Then

        '                            Dim strFilename As String = ""
        '                            Try
        '                                Dim f As DotNetNuke.Services.FileSystem.FileInfo = FileManager.Instance.GetFile(Integer.Parse(objItem.Url.Replace("FileId=", "")))
        '                                strFilename = f.FileName
        '                            Catch
        '                            End Try
        '                            If strFilename.ToLower.EndsWith(".jpg") Or _
        '                                strFilename.ToLower.EndsWith(".jpeg") Or _
        '                                strFilename.ToLower.EndsWith(".png") Or _
        '                                strFilename.ToLower.EndsWith(".gif") Then

        '                                blnHasImage = True

        '                            End If

        '                        End If
        '                    End If

        '                    If blnHasImage = False Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASIMAGE") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASLINK"
        '                    If objItem.Url = Null.NullString Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASLINK") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASNOLINK"
        '                    If objItem.Url <> Null.NullString Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASNOLINK") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASCONTENT"
        '                    If objItem.Content = Null.NullString Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASCONTENT") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASNOCONTENT"
        '                    If objItem.Content <> Null.NullString Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASNOCONTENT") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASCATEGORY"
        '                    If Not CategoryList.Count > 0 Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASCATEGORY") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "HASNOCATEGORY"
        '                    If Not CategoryList.Count = 0 Then
        '                        While (iPtr < templateArray.Length - 1)
        '                            If (templateArray(iPtr + 1) = "/HASNOCATEGORY") Then
        '                                Exit While
        '                            End If
        '                            iPtr = iPtr + 1
        '                        End While
        '                    End If

        '                Case "CLOSEDETAILURL"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = "javascript:CloseWindow()"
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "TWITTERTEXT"

        '                    Dim objLiteral As New Literal
        '                    Dim count As Integer = 160
        '                    If objItem.Summary.Length > 0 Then
        '                        objLiteral.Text = Server.UrlEncode(Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Summary)).TrimStart(), count))
        '                    Else
        '                        objLiteral.Text = Server.UrlEncode(Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart(), count))
        '                    End If
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "SHAREDTITLE"

        '                    Dim objLiteral As New Literal
        '                    objLiteral.Text = Server.UrlEncode(objItem.Title)
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case "SHAREDURL"

        '                    Dim objLiteral As New Literal

        '                    Dim tid As Integer = NewsModuleTab
        '                    If TargetTabId <> Null.NullInteger Then
        '                        tid = TargetTabId
        '                    End If

        '                    objLiteral.Text = Server.UrlEncode(Navigate(tid, True, Helpers.OnlyAlphaNumericChars(objItem.Title) & ".aspx", "ArticleId=" & objItem.ItemId))
        '                    objLiteral.EnableViewState = False
        '                    controls.Add(objLiteral)

        '                Case Else

        '                    If (templateArray(iPtr + 1).ToUpper().StartsWith("SCRIPT:")) Then

        '                        Dim strScript As String = templateArray(iPtr + 1).Substring(7, templateArray(iPtr + 1).Length - 7)
        '                        If strScript.Contains(";") Then
        '                            Dim strScriptUrl As String = strScript.Split(Char.Parse(";"))(1)
        '                            Dim strScriptKey As String = strScript.Split(Char.Parse(";"))(0)
        '                            If Not Page.ClientScript.IsClientScriptIncludeRegistered(strScriptKey) Then
        '                                Me.Page.ClientScript.RegisterClientScriptInclude(strScriptKey, strScriptUrl)
        '                            End If
        '                        End If

        '                    End If

        '                    If (templateArray(iPtr + 1).ToUpper().StartsWith("CSS:")) Then

        '                        Dim strCssUrl As String = templateArray(iPtr + 1).Substring(4, templateArray(iPtr + 1).Length - 4)

        '                        Dim blnAlreadyRegistered As Boolean = False
        '                        For Each ctrl As Control In Me.Page.Header.Controls

        '                            If TypeOf (ctrl) Is HtmlLink Then
        '                                Dim ctrlCss As HtmlLink = CType(ctrl, HtmlLink)
        '                                If ctrlCss.Href.ToLower = strCssUrl.ToLower Then
        '                                    blnAlreadyRegistered = True
        '                                    Exit For
        '                                End If
        '                            End If

        '                        Next

        '                        If Not blnAlreadyRegistered Then

        '                            Dim ctrlLink As New HtmlLink
        '                            ctrlLink.Href = strCssUrl
        '                            ctrlLink.Attributes.Add("rel", "stylesheet")
        '                            ctrlLink.Attributes.Add("type", "text/css")
        '                            ctrlLink.Attributes.Add("media", "screen")

        '                            Me.Page.Header.Controls.Add(ctrlLink)

        '                        End If

        '                    End If


        '                    If (templateArray(iPtr + 1).ToUpper().StartsWith("CREATEDBY:")) Then

        '                        Dim objUser As New UserInfo
        '                        Dim strProperty As String = templateArray(iPtr + 1).Substring(10, templateArray(iPtr + 1).Length - 10)

        '                        objUser = UserController.GetUserById(PortalId, objItem.CreatedByUser)

        '                        If Not objUser Is Nothing Then
        '                            Try
        '                                Dim objLiteral As New Literal

        '                                If strProperty.ToLower = "displayname" Then
        '                                    objLiteral.Text = objUser.DisplayName
        '                                ElseIf strProperty.ToLower = "email" Then
        '                                    objLiteral.Text = objUser.Email
        '                                ElseIf strProperty.ToLower = "photo" Then
        '                                    objLiteral.Text = objUser.Profile.Photo
        '                                ElseIf strProperty.ToLower = "photourl" Then
        '                                    objLiteral.Text = objUser.Profile.PhotoURL
        '                                Else
        '                                    objLiteral.Text = Server.HtmlDecode(objUser.Profile.GetPropertyValue(strProperty))
        '                                End If

        '                                objLiteral.EnableViewState = False
        '                                controls.Add(objLiteral)
        '                            Catch
        '                                'property could not be loaded
        '                            End Try
        '                        End If

        '                    End If

        '                    If (templateArray(iPtr + 1).ToUpper().StartsWith("CONTENT:")) Then

        '                        Dim objLiteral As New Literal
        '                        Dim count As Integer = Convert.ToInt32(templateArray(iPtr + 1).Substring(8, templateArray(iPtr + 1).Length - 8))
        '                        If (Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart().Length > count) Then
        '                            objLiteral.Text = Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart(), count) & "..."
        '                        Else
        '                            objLiteral.Text = Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart(), count)
        '                        End If
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)

        '                    End If

        '                    If (templateArray(iPtr + 1).ToUpper().StartsWith("SUMMARY:")) Then

        '                        Dim objLiteral As New Literal
        '                        Dim count As Integer = Convert.ToInt32(templateArray(iPtr + 1).Substring(8, templateArray(iPtr + 1).Length - 8))
        '                        If (Helpers.StripHtml(Server.HtmlDecode(objItem.Summary)).TrimStart().Length > count) Then
        '                            objLiteral.Text = Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Summary)).TrimStart(), count) & "..."
        '                        Else
        '                            objLiteral.Text = Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Summary)).TrimStart(), count)
        '                        End If
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)

        '                    End If

        '                    If (templateArray(iPtr + 1).ToUpper().StartsWith("DETAILWINDOWLINK:")) Then
        '                        Dim strOptions As String = templateArray(iPtr + 1).Substring(17, templateArray(iPtr + 1).Length - 17)
        '                        Dim lstOptions As Array = strOptions.Split(Char.Parse(":"))

        '                        Dim skin As String = "Default"
        '                        Dim width As Integer = 600
        '                        Dim height As Integer = 500

        '                        Try
        '                            width = Convert.ToInt32(lstOptions(0))
        '                        Catch
        '                        End Try
        '                        Try
        '                            height = Convert.ToInt32(lstOptions(1))
        '                        Catch
        '                        End Try
        '                        Try
        '                            skin = lstOptions(2)
        '                        Catch

        '                        End Try

        '                        Dim objLiteral As New HyperLink
        '                        'objLiteral.ID = CreateValidID("hypItemItem" & objItem.itemId.ToString & ModuleId.ToString)
        '                        objLiteral.Text = Localize("ReadItem.Token")
        '                        objLiteral.NavigateUrl = "javascript:NuntioArticlesShowArticle('" & objItem.ItemId.ToString & "','" & ModuleId.ToString & "'," & width & "," & height & ",'" & ArticleTheme & "')"
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)

        '                        Try
        '                            CType(Me.FindControl("wndControls"), RadWindowManager).Skin = skin
        '                        Catch
        '                        End Try

        '                    End If

        '                    If (templateArray(iPtr + 1).ToUpper().StartsWith("DETAILWINDOWURL:")) Then
        '                        Dim strOptions As String = templateArray(iPtr + 1).Substring(16, templateArray(iPtr + 1).Length - 16)
        '                        Dim lstOptions As Array = strOptions.Split(Char.Parse(":"))

        '                        Dim skin As String = "Default"
        '                        Dim width As Integer = 600
        '                        Dim height As Integer = 500

        '                        Try
        '                            width = Convert.ToInt32(lstOptions(0))
        '                        Catch
        '                        End Try
        '                        Try
        '                            height = Convert.ToInt32(lstOptions(1))
        '                        Catch
        '                        End Try
        '                        Try
        '                            skin = lstOptions(2)
        '                        Catch

        '                        End Try

        '                        Dim objLiteral As New Literal
        '                        'objLiteral.ID = CreateValidID("hypItemItemUrl" & objItem.itemId.ToString & ModuleId.ToString)
        '                        objLiteral.Text = "javascript:NuntioArticlesShowArticle('" & objItem.ItemId.ToString & "','" & ModuleId.ToString & "'," & width & "," & height & ",'" & ArticleTheme & "')"
        '                        objLiteral.EnableViewState = False
        '                        controls.Add(objLiteral)

        '                        Try
        '                            CType(Me.FindControl("wndControls"), RadWindowManager).Skin = skin
        '                        Catch
        '                        End Try

        '                    End If

        '            End Select

        '        End If
        '    Next

        'End Sub

        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId:="System.Int32.ToString")> _
        Protected Sub ProcessItemBody(ByRef html As String, ByVal objItem As ArticleInfo, ByVal Template As String, ByVal SearchKey As String, ByVal LoadFiles As Boolean, ByVal TargetTabId As Integer)

            Dim categories As New List(Of Integer)
            Dim CategoryList As New List(Of CategoryInfo)
            Dim cc As New CategoryController
            categories = cc.GetRelationsByitemId(objItem.ItemId)
            For Each item As Integer In categories
                Try
                    CategoryList.Add(cc.GetCategory(item, objItem.moduleId, Me.CurrentLocale, UseOriginalVersion))
                Catch
                End Try
            Next

            If LoadFiles Then
                Dim ctrlAttachments As New ArticleFileController
                objItem.Images = ctrlAttachments.GetImages(objItem.ItemId, NewsModuleId, CurrentLocale, UseOriginalVersion)
                objItem.Attachments = ctrlAttachments.GetAttachments(objItem.ItemId, NewsModuleId, CurrentLocale, UseOriginalVersion)
            End If

            'Dim literal As New Literal
            Dim delimStr As String = "[]"
            Dim delimiter As Char() = delimStr.ToCharArray()

            Dim templateArray As String()
            templateArray = Template.Split(delimiter)

            For iPtr As Integer = 0 To templateArray.Length - 1 Step 2

                html += Helpers.ProcessImages(templateArray(iPtr).ToString())

                If iPtr < templateArray.Length - 1 Then

                    Select Case templateArray(iPtr + 1)

                        Case "ANCHORLINK"


                            If String.IsNullOrEmpty(objItem.AnchorLink) Then
                                html += "NUA-" & objItem.ItemId.ToString
                            Else
                                html += objItem.AnchorLink
                            End If

                        Case "PORTALID"

                            html += PortalId.ToString

                        Case "ATTACHMENTS"

                            'process attachments
                            For Each objFile As ArticleFileInfo In objItem.Attachments
                                Dim strFile As String = Me.AttachmentTemplate
                                ProcessAttachmentTemplate(strFile, objItem, objFile)
                                html += strFile
                            Next


                        Case "HASATTACHMENTS"

                            If objItem.Attachments.Count = 0 Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASATTACHMENTS") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If


                        Case "HASNOATTACHMENTS"

                            If objItem.Attachments.Count > 0 Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOATTACHMENTS") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASPRIMARYATTACHMENT"

                            Dim blnHas As Boolean = False
                            For Each objFile As ArticleFileInfo In objItem.Attachments
                                If objFile.IsPrimary Then
                                    blnHas = True
                                    Exit For
                                End If
                            Next

                            If blnHas = False Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASPRIMARYATTACHMENT") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASNOPRIMARYATTACHMENT"

                            Dim blnHas As Boolean = False
                            For Each objFile As ArticleFileInfo In objItem.Attachments
                                If objFile.IsPrimary Then
                                    blnHas = True
                                    Exit For
                                End If
                            Next

                            If blnHas = True Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOPRIMARYATTACHMENT") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If


                        Case "PRIMARYATTACHMENTURL"

                            Dim strUrl As String = ""
                            For Each objFile As ArticleFileInfo In objItem.Attachments
                                If objFile.IsPrimary Then
                                    strUrl = objFile.Url
                                    Exit For
                                End If
                            Next
                            html += strUrl

                        Case "PRIMARYATTACHMENTTITLE"

                            Dim strTitle As String = ""
                            For Each objFile As ArticleFileInfo In objItem.Attachments
                                If objFile.IsPrimary Then
                                    strTitle = objFile.ImageTitle
                                    Exit For
                                End If
                            Next
                            html += strTitle

                        Case "PRIMARYATTACHMENTDESCRIPTION"

                            Dim strDescription As String = ""
                            For Each objFile As ArticleFileInfo In objItem.Attachments
                                If objFile.IsPrimary Then
                                    strDescription = objFile.ImageDescription
                                    Exit For
                                End If
                            Next
                            html += strDescription

                        Case "IMAGEGALLERY"

                            'process images
                            For Each objImage As ArticleFileInfo In objItem.Images
                                Dim strImage As String = Me.ImageTemplate
                                ProcessImageTemplate(strImage, objItem, objImage)
                                html += strImage
                            Next

                        Case "HASIMAGES"

                            If objItem.Images.Count = 0 Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASIMAGES") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If


                        Case "HASNOIMAGES"

                            If objItem.Images.Count > 0 Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOIMAGES") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASPRIMARYIMAGE"

                            Dim blnHas As Boolean = False
                            For Each objImage As ArticleFileInfo In objItem.Images
                                If objImage.IsPrimary Then
                                    blnHas = True
                                    Exit For
                                End If
                            Next

                            If blnHas = False Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASPRIMARYIMAGE") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASNOPRIMARYIMAGE"

                            Dim blnHas As Boolean = False
                            For Each objImage As ArticleFileInfo In objItem.Images
                                If objImage.IsPrimary Then
                                    blnHas = True
                                    Exit For
                                End If
                            Next

                            If blnHas = True Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOPRIMARYIMAGE") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If


                        Case "PRIMARYIMAGEURL"

                            Dim strUrl As String = ""
                            For Each objImage As ArticleFileInfo In objItem.Images
                                If objImage.IsPrimary Then
                                    strUrl = objImage.Url
                                    Exit For
                                End If
                            Next
                            html += strUrl

                        Case "PRIMARYIMAGETITLE"

                            Dim strTitle As String = ""
                            For Each objImage As ArticleFileInfo In objItem.Images
                                If objImage.IsPrimary Then
                                    strTitle = objImage.ImageTitle
                                    Exit For
                                End If
                            Next
                            html += strTitle

                        Case "PRIMARYIMAGEDESCRIPTION"

                            Dim strDescription As String = ""
                            For Each objImage As ArticleFileInfo In objItem.Images
                                If objImage.IsPrimary Then
                                    strDescription = objImage.ImageDescription
                                    Exit For
                                End If
                            Next
                            html += strDescription

                        Case "ITEMID"

                            html += objItem.ItemId.ToString

                        Case "ArticleId"


                            html += "NEWS_" & objItem.ItemId.ToString


                        Case "TEMPLATEPATH"

                            html += Me.ModuleDirectory & "/templates"

                        Case "ORIGINALLINK"

                            If objItem.IsOriginal = False Then

                                Dim sOrgLocale As String = ""
                                Dim ctrl As New ContentController(objItem.moduleId, CurrentLocale, False)
                                Dim lst As List(Of ContentInfo) = ctrl.GetAllContent

                                If lst.Count > 0 Then
                                    For Each item As ContentInfo In lst
                                        If item.SourceItemId = objItem.ItemId And item.IsOriginal Then
                                            sOrgLocale = item.Locale
                                            Exit For
                                        End If
                                    Next
                                End If
                                If sOrgLocale <> "" Then
                                    Dim params As New ArrayList
                                    For Each key As String In Request.QueryString.AllKeys
                                        Dim sValue As String = Request.QueryString(key)
                                        If key.ToLower <> "tabid" And key.ToLower <> "language" Then
                                            params.Add(key & "=" & sValue)
                                        End If
                                    Next
                                    params.Add("language=" & sOrgLocale)
                                    Dim pArr(params.Count - 1) As String
                                    For i As Integer = 0 To params.Count - 1
                                        pArr(i) = params(i).ToString
                                    Next

                                    Dim tid As Integer = NewsModuleTab
                                    If TargetTabId <> Null.NullInteger Then
                                        tid = TargetTabId
                                    End If

                                    html += "<a href=""" & NavigateURL(tid, "", pArr) & """>" & Localize("ViewOriginal.Token") & "</a>"
                                    
                                End If

                            End If

                        Case "EMAIL"

                            Dim ctrl As New ModuleController
                            Dim objModule As ModuleInfo = ctrl.GetModule(NewsModuleId)

                            Dim blnHasEditPermission As Boolean = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", objModule)
                            Dim blnIsOwner As Boolean = (Request.IsAuthenticated = True AndAlso UserInfo.UserID = objItem.CreatedByUser)
                            Dim blnCanEdit As Boolean = False

                            If blnHasEditPermission Then
                                blnCanEdit = True
                            Else
                                If blnIsOwner Then
                                    If EnforceEditPermissions = True Then
                                        blnCanEdit = False
                                    Else
                                        blnCanEdit = True
                                    End If
                                Else
                                    blnCanEdit = False
                                End If
                            End If

                            If blnCanEdit Then

                                html += "<a href=""javascript:NuntioArticlesEmailForm('" & objItem.ItemId.ToString & "','" & NewsModuleId.ToString & "','" & ModuleId.ToString & "')""><img src=""" & ImagesDirectory() & "nuntio_sendmail.png"" /></a>"

                            End If

                        Case "EDIT"

                            Dim ctrl As New ModuleController
                            Dim objModule As ModuleInfo = ctrl.GetModule(NewsModuleId)

                            Dim blnHasEditPermission As Boolean = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", objModule)
                            Dim blnIsOwner As Boolean = (Request.IsAuthenticated = True AndAlso UserInfo.UserID = objItem.CreatedByUser)
                            Dim blnCanEdit As Boolean = False

                            If blnHasEditPermission Then
                                blnCanEdit = True
                            Else
                                If blnIsOwner Then
                                    If EnforceEditPermissions = True Then
                                        blnCanEdit = False
                                    Else
                                        blnCanEdit = True
                                    End If
                                Else
                                    blnCanEdit = False
                                End If
                            End If

                            If blnCanEdit Then

                                html += "<a href=""javascript:NuntioArticlesEditForm('" & objItem.ItemId.ToString & "','" & NewsModuleId.ToString & "','" & ModuleId.ToString & "')""><img src=""" & ImagesDirectory() & "nuntio_edit.png"" /></a>"

                            End If

                        Case "PUBLISHDATE"

                            html += objItem.PublishDate.ToShortDateString

                        Case "LASTUPDATEDDATE"

                            html += objItem.LastUpdatedDate.ToShortDateString
                            
                        Case "LASTUPDATEDTIME"

                            html += objItem.LastUpdatedDate.ToShortTimeString
                            
                        Case "PUBLISHDAY"

                            Dim strDay As String = objItem.PublishDate.Day.ToString
                            If strDay.Length = 1 Then
                                strDay = "0" & strDay
                            End If

                            html += strDay

                        Case "PUBLISHMONTH"

                            html += objItem.PublishDate.Month.ToString
                            
                        Case "PUBLISHMONTHSHORT"

                            html += MonthName(objItem.PublishDate.Month).Substring(0, 3)
                            
                        Case "PUBLISHMONTHNAME"

                            html += MonthName(objItem.PublishDate.Month)

                        Case "PUBLISHYEAR"

                            html += objItem.PublishDate.Year.ToString

                        Case "PUBLISHDATELONG"

                            html += objItem.PublishDate.ToLongDateString

                        Case "PUBLISHDATESHORT"

                            html += objItem.PublishDate.ToShortDateString

                        Case "CREATEDDATE", "CREATEDDATESHORT"

                            html += objItem.CreatedDate.ToShortDateString

                        Case "CREATEDDATELONG"

                            html += objItem.CreatedDate.ToLongDateString

                        Case "CREATEDTIME"

                            html += objItem.CreatedDate.ToShortTimeString

                        Case "CREATEDBYUSER"

                            Dim objUser As New UserInfo
                            objUser = UserController.GetUserById(PortalId, objItem.CreatedByUser)
                            If Not objUser Is Nothing Then
                                html += objUser.DisplayName
                            End If

                        Case "CREATEDBYUSERID"

                            Dim objUser As New UserInfo
                            objUser = UserController.GetUserById(PortalId, objItem.CreatedByUser)
                            If Not objUser Is Nothing Then
                                html += objUser.UserID.ToString
                            End If

                        Case "LASTUPDATEDBYUSER"

                            Dim objUser As New UserInfo
                            objUser = UserController.GetUserById(PortalId, objItem.LastUpdatedBy)
                            If Not objUser Is Nothing Then
                                html += objUser.DisplayName
                            End If

                        Case "LASTUPDATEDBYUSERID"

                            Dim objUser As New UserInfo
                            objUser = UserController.GetUserById(PortalId, objItem.LastUpdatedBy)
                            If Not objUser Is Nothing Then
                                html += objUser.UserID.ToString
                            End If

                        Case "SUMMARY"

                            html += Helpers.FormatSearchString(Server.HtmlDecode(objItem.Summary), SearchKey)

                        Case "CONTENT"

                            html += Helpers.FormatSearchString(Server.HtmlDecode(objItem.Content), SearchKey)

                        Case "TITLE"

                            html += Helpers.FormatSearchString(objItem.Title, SearchKey)

                        Case "NEWSLINK"

                            If objItem.Url <> "" Then

                                Dim Trackingcontroller As New DotNetNuke.Common.Utilities.UrlController
                                Dim Tracking As DotNetNuke.Common.Utilities.UrlTrackingInfo = Trackingcontroller.GetUrlTracking(PortalId, objItem.Url, NewsModuleId)
                                Dim Track As Boolean = False
                                Dim Target As String = "_self"
                                If Not Tracking Is Nothing Then
                                    Track = Tracking.TrackClicks
                                    If Tracking.NewWindow Then
                                        Target = "_blank"
                                    End If
                                End If

                                html += "<a href=""" & FormatURL(objItem.Url, objItem.TrackClicks) & """ target=""" & Target & """>" & Localize("ReadMore.Token") & "</a>"

                            End If


                        Case "EXTENSIONICON"

                            html += GetArticleIconPath(objItem)

                        Case "NEWSLINKURL"

                            If objItem.Url <> "" Then
                                html += FormatURL(objItem.Url, objItem.TrackClicks)
                            End If

                        Case "NEWSLINKTITLE"

                            Dim strText As String = ""

                            If objItem.Url <> "" Then

                                If IsNumeric(objItem.Url) Then

                                    'link to tab
                                    strText = Localize("ReadMore.Token")

                                ElseIf objItem.Url.StartsWith("FileId=") Then
                                    'link to file
                                    Dim strFilename As String = Localize("File")
                                    Try
                                        Dim f As DotNetNuke.Services.FileSystem.FileInfo = FileManager.Instance.GetFile(Integer.Parse(objItem.Url.Replace("FileId=", "")))
                                        strFilename = f.FileName
                                    Catch
                                    End Try
                                    strText = String.Format(Localize("DownloadFile.Token"), strFilename)

                                ElseIf objItem.Url.StartsWith("UserId=") Then
                                    'link to user
                                    Dim strUser As String = Localize("User")
                                    Try
                                        strUser = UserController.GetUserById(PortalSettings.PortalId, Integer.Parse(objItem.Url.Replace("UserId=", ""))).DisplayName
                                    Catch
                                    End Try
                                    strText = String.Format(Localize("VisitUser.Token"), strUser)
                                Else

                                    'external url
                                    strText = Localize("ReadMore.Token")

                                End If

                                html += strText

                            End If

                        Case "NEWSLINKTARGET"

                            If objItem.Url <> "" Then

                                Dim Trackingcontroller As New DotNetNuke.Common.Utilities.UrlController
                                Dim Tracking As DotNetNuke.Common.Utilities.UrlTrackingInfo = Trackingcontroller.GetUrlTracking(PortalId, objItem.Url, NewsModuleId)
                                Dim Target As String = "_self"
                                If Not Tracking Is Nothing Then
                                    If Tracking.NewWindow Then
                                        Target = "_blank"
                                    End If
                                End If

                                html += Target

                            End If


                        Case "DETAILLINK", "ITEMLINK"

                            Dim tid As Integer = NewsModuleTab
                            If TargetTabId <> Null.NullInteger Then
                                tid = TargetTabId
                            End If

                            html += "<a href=""" & Navigate(tid, True, Helpers.OnlyAlphaNumericChars(objItem.Title) & ".aspx", "ArticleId=" & objItem.ItemId) & """>" & Localize("ReadItem.Token") & "</a>"


                        Case "DETAILLINKURL", "ITEMLINKURL"

                            Dim tid As Integer = NewsModuleTab
                            If TargetTabId <> Null.NullInteger Then
                                tid = TargetTabId
                            End If

                            html += Navigate(tid, True, Helpers.OnlyAlphaNumericChars(objItem.Title) & ".aspx", "ArticleId=" & objItem.ItemId)

                        Case "DETAILWINDOWLINK"

                            html += "<a href=""javascript:NuntioArticlesShowArticle('" & objItem.ItemId.ToString & "','" & ModuleId.ToString & "','" & ArticleTheme & "')"">" & Localize("ReadItem.Token") & "</a>"

                        Case "YEARLINK"

                            Dim tid As Integer = NewsModuleTab
                            If TargetTabId <> Null.NullInteger Then
                                tid = TargetTabId
                            End If
                            html += "<a href=""" & NavigateURL(tid, "", "Year=" & Year(objItem.PublishDate).ToString, "NewsModule=" & NewsModuleId.ToString) & "#NEWS" & objItem.ItemId.ToString & """>" & Localize("ReadItem.Token") & "</a>"
                           

                        Case "YEARLINKURL"

                            Dim tid As Integer = NewsModuleTab
                            If TargetTabId <> Null.NullInteger Then
                                tid = TargetTabId
                            End If

                            html += NavigateURL(tid, "", "Year=" & Year(objItem.PublishDate).ToString, "NewsModule=" & NewsModuleId.ToString) & "#NEWS" & objItem.ItemId.ToString

                        Case "MONTHLINK"

                            Dim tid As Integer = NewsModuleTab
                            If TargetTabId <> Null.NullInteger Then
                                tid = TargetTabId
                            End If                        

                            html += "<a href=""" & NavigateURL(tid, "", "Year=" & Year(objItem.PublishDate).ToString, "Month=" & Month(objItem.PublishDate).ToString, "NewsModule=" & NewsModuleId.ToString) & "#NEWS" & objItem.ItemId.ToString & """>" & Localize("ReadItem.Token") & "</a>"

                        Case "MONTHLINKURL"

                            Dim tid As Integer = NewsModuleTab
                            If TargetTabId <> Null.NullInteger Then
                                tid = TargetTabId
                            End If

                            html += NavigateURL(tid, "", "Year=" & Year(objItem.PublishDate).ToString, "Month=" & Month(objItem.PublishDate).ToString, "NewsModule=" & NewsModuleId.ToString) & "#NEWS" & objItem.ItemId.ToString

                        Case "AUTHORLINKURL"

                            Dim tid As Integer = NewsModuleTab
                            If TargetTabId <> Null.NullInteger Then
                                tid = TargetTabId
                            End If
                            html += NavigateURL(tid, "", "AuthorId=" & objItem.CreatedByUser.ToString)
                           

                        Case "ALLNEWSLINK"

                            If Me.ArticleId <> Null.NullInteger Then

                                Dim tid As Integer = NewsModuleTab
                                If TargetTabId <> Null.NullInteger Then
                                    tid = TargetTabId
                                End If

                                html += "<a href=""" & NavigateURL(tid) & """>" & Localize("AllNews.Token") & "</a>"

                            End If

                        Case "ALLNEWSLINKURL"

                            If Me.ArticleId <> Null.NullInteger Then

                                Dim tid As Integer = NewsModuleTab
                                If TargetTabId <> Null.NullInteger Then
                                    tid = TargetTabId
                                End If
                                html += NavigateURL(tid)
                                
                            End If

                        Case "FILEDUNDER"

                            If CategoryList.Count > 0 Then

                                html += Localize("FiledUnder")

                            End If

                        Case "CATEGORYLIST"
                            If CategoryList.Count > 0 Then
                                Dim sCategories As String = ""
                                For Each oCat As CategoryInfo In CategoryList
                                    sCategories = sCategories & "<a href=""" & Navigate(False, Helpers.OnlyAlphaNumericChars(oCat.CategoryName) & ".aspx", "CategoryID=" & oCat.CategoryID.ToString) & """>" & oCat.CategoryName & "</a>, "
                                Next
                                If sCategories.EndsWith(", ") Then
                                    sCategories = sCategories.Substring(0, sCategories.Length - 2)
                                End If

                                html += sCategories

                            End If

                        Case "HASSUMMARY"

                            If objItem.Summary = Null.NullString Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASSUMMARY") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASNOSUMMARY"

                            If objItem.Summary <> Null.NullString Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOSUMMARY") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASRELATEDARTICLES"

                            If objItem.RelatedArticles.Count = 0 Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASRELATEDARTICLES") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASNORELATEDARTICLES"

                            If objItem.RelatedArticles.Count > 0 Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNORELATEDARTICLES") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "RELATEDARTICLESLIST"

                            If objItem.RelatedArticles.Count > 0 Then

                                For Each oArticle As ArticleInfo In objItem.RelatedArticles

                                    Dim strMarkup As String = "<li>"
                                    strMarkup += "<a href=""" & Navigate(TabId, True, Helpers.OnlyAlphaNumericChars(oArticle.Title) & ".aspx", "ArticleId=" & oArticle.ItemId) & """>"
                                    strMarkup += oArticle.Title
                                    strMarkup += "</a></li>"

                                    html += strMarkup

                                Next

                            End If


                        Case "PUBLICATIONMEMBERS"

                            If objItem.ParentId <> Null.NullInteger Then

                                Dim oParent As ArticleInfo = ArticleController.GetArticle(objItem.ParentId, CurrentLocale, True)
                                If Not oParent Is Nothing Then
                                    For Each oArticle As ArticleInfo In oParent.ChildArticles

                                        Dim strMarkup As String = "<li>"
                                        strMarkup += "<a href=""" & Navigate(TabId, True, Helpers.OnlyAlphaNumericChars(oArticle.Title) & ".aspx", "ArticleId=" & oArticle.ItemId) & """>"
                                        strMarkup += oArticle.Title
                                        strMarkup += "</a></li>"

                                        html += strMarkup

                                    Next


                                End If
                            End If

                        Case "PUBLICATIONURL"

                            Dim oParent As ArticleInfo = ArticleController.GetArticle(objItem.ParentId, CurrentLocale, True)

                            If oParent Is Nothing Then
                                oParent = ArticleController.GetArticle(objItem.ItemId, CurrentLocale, True)
                            End If

                            If Not oParent Is Nothing Then
                                Dim strParent As String = Navigate(TabId, True, Helpers.OnlyAlphaNumericChars(oParent.Title) & ".aspx", "ArticleId=" & oParent.ItemId)
                                html += strParent
                            End If


                        Case "ISPUBLICATIONMEMBER"

                            If objItem.ParentId = Null.NullInteger Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/ISPUBLICATIONMEMBER") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "ISPUBLICATION"

                            If Not objItem.IsPublication Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/ISPUBLICATION") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "ISNOPUBLICATION"

                            If objItem.IsPublication Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/ISNOPUBLICATION") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "PUBLICATIONARTICLES"

                            If objItem.IsPublication Then
                                For Each oArticle As ArticleInfo In objItem.ChildArticles

                                    Dim strMarkup As String = "<li>"
                                    strMarkup += "<a href=""" & Navigate(TabId, True, Helpers.OnlyAlphaNumericChars(oArticle.Title) & ".aspx", "ArticleId=" & oArticle.ItemId) & """>"
                                    strMarkup += oArticle.Title
                                    strMarkup += "</a></li>"

                                    html += strMarkup

                                Next
                            End If

                        Case "HASCOMMENTS"

                            If objItem.CreatedByUser = UserInfo.UserID Then

                                Dim commentscontroller As New CommentController
                                Dim comments As New List(Of CommentInfo)
                                comments = commentscontroller.List(objItem.ItemId)
                                If comments.Count = 0 Then
                                    While (iPtr < templateArray.Length - 1)
                                        If (templateArray(iPtr + 1) = "/HASCOMMENTS") Then
                                            Exit While
                                        End If
                                        iPtr = iPtr + 1
                                    End While
                                End If

                            Else
                                If objItem.Comments = 0 Then
                                    While (iPtr < templateArray.Length - 1)
                                        If (templateArray(iPtr + 1) = "/HASCOMMENTS") Then
                                            Exit While
                                        End If
                                        iPtr = iPtr + 1
                                    End While
                                End If
                            End If


                        Case "HASNOCOMMENTS"

                            If objItem.Comments > 0 Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOCOMMENTS") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "COMMENTURL"


                            html += "javascript:NuntioArticlesCommentForm('" & objItem.ItemId.ToString & "','-1','" & TabId.ToString & "','" & NewsModuleId.ToString & "','" & PortalId.ToString & "','" & ArticleTheme.ToString & "')"


                        Case "COMMENTCOUNT"

                            If objItem.CreatedByUser = UserInfo.UserID Then

                                Dim commentscontroller As New CommentController
                                Dim comments As New List(Of CommentInfo)
                                comments = commentscontroller.List(objItem.ItemId)
                                html += comments.Count.ToString

                            Else
                                html += objItem.Comments.ToString
                            End If


                        Case "COMMENTLIST"

                            ProcessComments(html, CommentTemplate, objItem)

                        Case "ISNOTIMAGE", "HASNOIMAGE"

                            Dim blnHasImage As Boolean = False

                            If objItem.Url <> Null.NullString Then
                                If objItem.Url.StartsWith("FileId=") Then

                                    Dim strFilename As String = ""
                                    Try
                                        Dim f As DotNetNuke.Services.FileSystem.FileInfo = FileManager.Instance.GetFile(Integer.Parse(objItem.Url.Replace("FileId=", "")))
                                        strFilename = f.FileName
                                    Catch
                                    End Try
                                    If strFilename.ToLower.EndsWith(".jpg") Or _
                                        strFilename.ToLower.EndsWith(".jpeg") Or _
                                        strFilename.ToLower.EndsWith(".png") Or _
                                        strFilename.ToLower.EndsWith(".gif") Then

                                        blnHasImage = True

                                    End If

                                End If
                            End If

                            If blnHasImage = True Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/ISNOTIMAGE") Or (templateArray(iPtr + 1) = "/HASNOIMAGE") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASIMAGE"

                            Dim blnHasImage As Boolean = False

                            If objItem.Url <> Null.NullString Then
                                If objItem.Url.StartsWith("FileId=") Then

                                    Dim strFilename As String = ""
                                    Try
                                        Dim f As DotNetNuke.Services.FileSystem.FileInfo = FileManager.Instance.GetFile(Integer.Parse(objItem.Url.Replace("FileId=", "")))
                                        strFilename = f.FileName
                                    Catch
                                    End Try
                                    If strFilename.ToLower.EndsWith(".jpg") Or _
                                        strFilename.ToLower.EndsWith(".jpeg") Or _
                                        strFilename.ToLower.EndsWith(".png") Or _
                                        strFilename.ToLower.EndsWith(".gif") Then

                                        blnHasImage = True

                                    End If

                                End If
                            End If

                            If blnHasImage = False Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASIMAGE") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASLINK"
                            If objItem.Url = Null.NullString Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASLINK") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASNOLINK"
                            If objItem.Url <> Null.NullString Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOLINK") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASCONTENT"
                            If objItem.Content = Null.NullString Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASCONTENT") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASNOCONTENT"
                            If objItem.Content <> Null.NullString Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOCONTENT") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASCATEGORY"
                            If Not CategoryList.Count > 0 Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASCATEGORY") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASNOCATEGORY"
                            If Not CategoryList.Count = 0 Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOCATEGORY") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "CLOSEDETAILURL"

                            html += "javascript:CloseWindow()"

                        Case "TWITTERTEXT"

                            Dim count As Integer = 160
                            If objItem.Summary.Length > 0 Then
                                html += Server.UrlEncode(Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Summary)).TrimStart(), count))
                            Else
                                html += Server.UrlEncode(Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart(), count))
                            End If                

                        Case "SHAREDTITLE"

                            html += Server.UrlEncode(objItem.Title)

                        Case "SHAREDURL"

                            Dim tid As Integer = NewsModuleTab
                            If TargetTabId <> Null.NullInteger Then
                                tid = TargetTabId
                            End If

                            html += Server.UrlEncode(Navigate(tid, True, Helpers.OnlyAlphaNumericChars(objItem.Title) & ".aspx", "ArticleId=" & objItem.ItemId))                           

                        Case Else

                            If (templateArray(iPtr + 1).ToUpper().StartsWith("SCRIPT:")) Then

                                Dim strScript As String = templateArray(iPtr + 1).Substring(7, templateArray(iPtr + 1).Length - 7)
                                If strScript.Contains(";") Then
                                    Dim strScriptUrl As String = strScript.Split(Char.Parse(";"))(1)
                                    Dim strScriptKey As String = strScript.Split(Char.Parse(";"))(0)
                                    If Not Page.ClientScript.IsClientScriptIncludeRegistered(strScriptKey) Then
                                        Me.Page.ClientScript.RegisterClientScriptInclude(strScriptKey, strScriptUrl)
                                    End If
                                End If

                            End If

                            If (templateArray(iPtr + 1).ToUpper().StartsWith("CSS:")) Then

                                Dim strCssUrl As String = templateArray(iPtr + 1).Substring(4, templateArray(iPtr + 1).Length - 4)

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

                            End If


                            If (templateArray(iPtr + 1).ToUpper().StartsWith("CREATEDBY:")) Then

                                Dim objUser As New UserInfo
                                Dim strProperty As String = templateArray(iPtr + 1).Substring(10, templateArray(iPtr + 1).Length - 10)

                                objUser = UserController.GetUserById(PortalId, objItem.CreatedByUser)

                                If Not objUser Is Nothing Then
                                    Try

                                        If strProperty.ToLower = "displayname" Then
                                            html += objUser.DisplayName
                                        ElseIf strProperty.ToLower = "email" Then
                                            html += objUser.Email
                                        ElseIf strProperty.ToLower = "photo" Then
                                            html += objUser.Profile.Photo
                                        ElseIf strProperty.ToLower = "photourl" Then
                                            html += objUser.Profile.PhotoURL
                                        Else
                                            html += Server.HtmlDecode(objUser.Profile.GetPropertyValue(strProperty))
                                        End If

                                    Catch
                                        'property could not be loaded
                                    End Try
                                End If

                            End If

                            If (templateArray(iPtr + 1).ToUpper().StartsWith("CONTENT:")) Then

                                Dim count As Integer = Convert.ToInt32(templateArray(iPtr + 1).Substring(8, templateArray(iPtr + 1).Length - 8))
                                If (Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart().Length > count) Then
                                    html += Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart(), count) & "..."
                                Else
                                    html += Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart(), count)
                                End If

                            End If

                            If (templateArray(iPtr + 1).ToUpper().StartsWith("SUMMARY:")) Then

                                Dim count As Integer = Convert.ToInt32(templateArray(iPtr + 1).Substring(8, templateArray(iPtr + 1).Length - 8))
                                If (Helpers.StripHtml(Server.HtmlDecode(objItem.Summary)).TrimStart().Length > count) Then
                                    html += Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Summary)).TrimStart(), count) & "..."
                                Else
                                    html += Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Summary)).TrimStart(), count)
                                End If

                            End If

                            If (templateArray(iPtr + 1).ToUpper().StartsWith("DETAILWINDOWLINK:")) Then
                                Dim strOptions As String = templateArray(iPtr + 1).Substring(17, templateArray(iPtr + 1).Length - 17)
                                Dim lstOptions As Array = strOptions.Split(Char.Parse(":"))

                                Dim skin As String = "Default"
                                Dim width As Integer = 600
                                Dim height As Integer = 500

                                Try
                                    width = Convert.ToInt32(lstOptions(0))
                                Catch
                                End Try
                                Try
                                    height = Convert.ToInt32(lstOptions(1))
                                Catch
                                End Try
                                Try
                                    skin = lstOptions(2)
                                Catch

                                End Try

                                html += "<a href=""javascript:NuntioArticlesShowArticle('" & objItem.ItemId.ToString & "','" & ModuleId.ToString & "'," & width & "," & height & ",'" & ArticleTheme & "')"">" & Localize("ReadItem.Token") & "</a>"
                                

                                Try
                                    CType(Me.FindControl("wndControls"), RadWindowManager).Skin = skin
                                Catch
                                End Try

                            End If

                            If (templateArray(iPtr + 1).ToUpper().StartsWith("DETAILWINDOWURL:")) Then
                                Dim strOptions As String = templateArray(iPtr + 1).Substring(16, templateArray(iPtr + 1).Length - 16)
                                Dim lstOptions As Array = strOptions.Split(Char.Parse(":"))

                                Dim skin As String = "Default"
                                Dim width As Integer = 600
                                Dim height As Integer = 500

                                Try
                                    width = Convert.ToInt32(lstOptions(0))
                                Catch
                                End Try
                                Try
                                    height = Convert.ToInt32(lstOptions(1))
                                Catch
                                End Try
                                Try
                                    skin = lstOptions(2)
                                Catch

                                End Try

                                html += "javascript:NuntioArticlesShowArticle('" & objItem.ItemId.ToString & "','" & ModuleId.ToString & "'," & width & "," & height & ",'" & ArticleTheme & "')"

                                Try
                                    CType(Me.FindControl("wndControls"), RadWindowManager).Skin = skin
                                Catch
                                End Try

                            End If

                    End Select

                End If
            Next

        End Sub

        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId:="System.Int32.ToString")> _
        Protected Sub ProcessJournalItem(ByRef strHTML As String, ByVal objItem As ArticleInfo, ByVal Template As String, ByVal ArticleImage As String)

            Dim ctrlAttachments As New ArticleFileController
            objItem.Images = ctrlAttachments.GetImages(objItem.ItemId, NewsModuleId, CurrentLocale, UseOriginalVersion)

            Dim literal As New Literal
            Dim delimStr As String = "[]"
            Dim delimiter As Char() = delimStr.ToCharArray()

            Dim templateArray As String()
            templateArray = Template.Split(delimiter)

            For iPtr As Integer = 0 To templateArray.Length - 1 Step 2

                strHTML += Helpers.ProcessImages(templateArray(iPtr).ToString())

                If iPtr < templateArray.Length - 1 Then

                    Select Case templateArray(iPtr + 1)

                        Case "HASPRIMARYIMAGE"

                            Dim blnHas As Boolean = ArticleImage.Length > 0

                            If blnHas = False Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASPRIMARYIMAGE") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASNOPRIMARYIMAGE"

                           Dim blnHas As Boolean = ArticleImage.Length > 0

                            If blnHas = True Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOPRIMARYIMAGE") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "PRIMARYIMAGEURL"

                            strHTML += ArticleImage

                        Case "TITLE"

                            strHTML += objItem.Title

                        Case "HASSUMMARY"

                            If objItem.Summary = Null.NullString Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASSUMMARY") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "HASNOSUMMARY"

                            If objItem.Summary <> Null.NullString Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/HASNOSUMMARY") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "SUMMARY"

                            strHTML += Server.HtmlDecode(objItem.Summary)

                        Case "CONTENT"

                            strHTML += Server.HtmlDecode(objItem.Content)

                        Case Else

                            If (templateArray(iPtr + 1).ToUpper().StartsWith("CONTENT:")) Then

                                Dim count As Integer = Convert.ToInt32(templateArray(iPtr + 1).Substring(8, templateArray(iPtr + 1).Length - 8))
                                If (Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart().Length > count) Then
                                    strHTML += Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart(), count) & "..."
                                Else
                                    strHTML += Left(Helpers.StripHtml(Server.HtmlDecode(objItem.Content)).TrimStart(), count)
                                End If

                            End If

                    End Select

                End If
            Next

        End Sub

        Protected Sub ProcessComments(ByRef html As String, ByVal Template As String, ByVal objArticle As ArticleInfo)

            Dim cc As New CommentController
            Dim comments As New List(Of CommentInfo)

            comments = cc.List(objArticle.ItemId)

            For Each objItem As CommentInfo In comments

                If objItem.IsApproved Or (objItem.IsApproved = False And objArticle.CreatedByUser = UserInfo.UserID) Then


                    Dim strDisplayname As String = ""
                    Dim strEmail As String = ""
                    Dim strGravatarUrl As String = ""
                    Dim oUser As UserInfo = Nothing
                    Try
                        oUser = UserController.GetUserById(PortalId, objItem.CreatedBy)
                    Catch
                    End Try

                    If Not oUser Is Nothing Then
                        strDisplayname = oUser.DisplayName
                        strEmail = oUser.Email
                    Else
                        strDisplayname = objItem.Displayname
                        strEmail = objItem.Email
                    End If

                    Dim literal As New Literal
                    Dim delimStr As String = "[]"
                    Dim delimiter As Char() = delimStr.ToCharArray()

                    Dim templateArray As String()
                    templateArray = Template.Split(delimiter)

                    For iPtr As Integer = 0 To templateArray.Length - 1 Step 2

                        html += Helpers.ProcessImages(templateArray(iPtr).ToString())

                        If iPtr < templateArray.Length - 1 Then
                            Select Case templateArray(iPtr + 1)
                                Case "ISARTICLEOWNER"
                                    If Not objArticle.CreatedByUser = UserInfo.UserID Then
                                        While (iPtr < templateArray.Length - 1)
                                            If (templateArray(iPtr + 1) = "/ISARTICLEOWNER") Then
                                                Exit While
                                            End If
                                            iPtr = iPtr + 1
                                        End While
                                    End If
                                Case "IPADDRESS"
                                    If objArticle.CreatedByUser = UserInfo.UserID Then
                                        html += objItem.RemoteAddress
                                    End If
                                Case "APPROVE"
                                    If objArticle.CreatedByUser = UserInfo.UserID Then
                                        html += "<a href=""javascript:NuntioArticlesCommentForm('" & objArticle.ItemId.ToString & "','" & objItem.ItemId.ToString & "','" & TabId.ToString & "','" & ModuleId.ToString & "','" & PortalId.ToString & "','" & ArticleTheme.ToString & "')"">"
                                        If objItem.IsApproved Then
                                            html += Localize("EditComment")
                                        Else
                                            html += Localize("ApproveComment")
                                        End If
                                        html += "</a>"                                        
                                    End If
                                Case "COMMENT"
                                    html += objItem.Comment
                                Case "CREATEDBY"
                                    html += strDisplayname                                    
                                Case "CREATEDATE"
                                    html += objItem.CreatedDate.ToShortDateString
                                Case "CREATETIME"
                                    html += objItem.CreatedDate.ToShortTimeString
                                Case "GRAVATARURL"

                                    If strEmail <> "" Then

                                        Dim strUrl As String = "http://www.gravatar.com/avatar/"

                                        Dim md5 As New MD5CryptoServiceProvider
                                        Dim encoder As New UTF8Encoding
                                        Dim md5Hasher As New MD5CryptoServiceProvider
                                        Dim hashedBytes As Byte() = md5Hasher.ComputeHash(encoder.GetBytes(strEmail))
                                        Dim sb As New StringBuilder(hashedBytes.Length * 2)
                                        For i As Integer = 0 To hashedBytes.Length - 1
                                            sb.Append(hashedBytes(i).ToString("X2"))
                                        Next

                                        strUrl += sb.ToString().ToLower()
                                        strUrl += ".jpg"
                                        strUrl += "?r=G"
                                        strUrl += "&s=80"

                                        Dim strDefaultImage As String = "http://" & PortalSettings.PortalAlias.HTTPAlias & "/Desktopmodules/Nuntio.Articles/templates/" & ArticleTheme & "/images/gravatar.png"
                                        strUrl += "&d=" & Server.UrlEncode(strDefaultImage)

                                        html += strUrl
                                        
                                    End If

                            End Select
                        End If
                    Next

                End If

            Next

        End Sub

        Protected Sub ProcessSurroundingTemplates(ByRef controls As System.Web.UI.ControlCollection, ByVal Template As String)

            Dim literal As New Literal
            Dim delimStr As String = "[]"
            Dim delimiter As Char() = delimStr.ToCharArray()

            Dim templateArray As String()
            templateArray = Template.Split(delimiter)

            For iPtr As Integer = 0 To templateArray.Length - 1 Step 2
                controls.Add(New LiteralControl(Helpers.ProcessImages(templateArray(iPtr).ToString())))
                If iPtr < templateArray.Length - 1 Then
                    Select Case templateArray(iPtr + 1)
                        Case "PORTALID"
                            controls.Add(New LiteralControl(PortalId.ToString))
                        Case "CURRENTURL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = NavigateURL(NewsModuleTab)
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)
                        Case "CURRENTNEWS"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Localize("CurrentNews.Token")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)
                        Case "ARCHIVEMESSAGE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Localize("ArchiveMessage.Token")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)
                        Case "RSS"
                            Dim objLiteral As New Literal
                            objLiteral.Text = "<a href=""" & Navigate(False, "act=rss") & """><img src=""" & Me.ResolveUrl("icon_xml.gif") & """ border=""0""></a>"
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)
                        Case "CANADD"
                            If CanPublish = False Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/CANADD") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If
                        Case "ADDARTICLEURL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = "javascript:NuntioArticlesPublishForm('" & ModuleId.ToString & "', '" & NewsModuleId.ToString & "', '-1')"
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)
                    End Select
                End If
            Next

        End Sub

        Protected Sub ProcessMailBody(ByRef Body As String, ByVal objSubscribe As SubscriptionInfo, ByVal objItem As ArticleInfo, ByVal modid As Integer)
            Body = Body.Replace("[CONFIRMUNSUBSCRIBEURL]", NavigateURL(Me.NewsModuleTab, "", "act=unsubscribe", "key=" & objSubscribe.Key, "NewsModule=" & modid.ToString, "email=" & Server.UrlEncode(objSubscribe.Email)))
            Body = Body.Replace("[DEAR]", Localize("Dear.Mail"))
            Body = Body.Replace("[RECIPIENT_NAME]", objSubscribe.Name)
            Body = Body.Replace("[UNSUBSCRIBECONFIRMMESSAGE]", String.Format(Localize("UnsubscribeConfirmMessage.Mail"), PortalSettings.PortalName))
            Body = Body.Replace("[CONFIRMUNSUBSCRIBELINK]", Localize("ConfirmUnsubscribeLink.Mail"))
        End Sub

#End Region

    End Class
End Namespace

