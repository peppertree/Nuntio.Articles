Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports CookComputing.XmlRpc
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Modules
Imports System.IO

Namespace dnnWerk.Modules.Nuntio.Articles.MetaWeblog

    Public Class MetaWeblog
        Inherits XmlRpcService
        Implements IMetaWeblog


#Region "Private Members"

        Private Portalsettings As Portals.PortalSettings = Nothing
        Private CurrentLocale As String = "de-DE"
        Private ModuleDirectory As String = "~/Desktopmodules/Nuntio.Articles/"
        Private objUser As Entities.Users.UserInfo = Nothing
        'Private postUrl As String = "http://magic/Desktopmodules/Nuntio.Articles/ArticlePost.ashx"

#End Region

#Region "Public Constructors"

        Public Sub New()
        End Sub

#End Region

#Region "IMetaWeblog Members"

        Private Function AddPost(ByVal blogid As String, ByVal username As String, ByVal password As String, ByVal post As Post, ByVal publish As Boolean) As String Implements IMetaWeblog.AddPost

            Portalsettings = Portals.PortalController.GetCurrentPortalSettings

            If ValidateUser(username, password) Then

                Dim id As String = Null.NullInteger
                Dim moduleid As Integer = Convert.ToInt32(blogid)
                Dim controller As New ArticleController
                Dim article As New ArticleInfo

                article.ApprovedBy = objUser.UserID
                article.ApprovedDate = Date.Now
                article.Comments = 0
                article.Content = post.description
                article.CreatedByUser = objUser.UserID
                article.CreatedDate = Date.Now
                article.IsApproved = True
                article.IsLocaleTranslated = False
                article.IsNotified = False
                article.IsOriginal = True
                article.Locale = CurrentLocale
                article.moduleId = moduleid
                article.NewWindow = False
                article.PortalID = Portalsettings.PortalId
                article.PublishDate = Date.Now
                article.Summary = ""
                article.Title = post.title
                article.TrackClicks = False
                article.Url = ""
                article.ViewOrder = 1
                article.itemId = controller.AddArticle(article)

                id = article.itemId

                Return id
            End If

            Throw New XmlRpcFaultException(0, "User is not valid!")
        End Function

        Private Function UpdatePost(ByVal postid As String, ByVal username As String, ByVal password As String, ByVal post As Post, ByVal publish As Boolean) As Boolean Implements IMetaWeblog.UpdatePost

            If ValidateUser(username, password) Then

                Dim result As Boolean = False

                Dim controller As New ArticleController
                Dim article As ArticleInfo = controller.GetArticle(Convert.ToInt32(postid), CurrentLocale, True)

                If Not article Is Nothing Then
                    article.Locale = CurrentLocale
                    article.Title = post.title
                    article.Content = post.description
                    article.LastUpdatedBy = objUser.UserID
                    article.LastUpdatedDate = Date.Now
                    controller.UpdateArticle(article)
                    result = True
                End If

                Return result

            End If


            Throw New XmlRpcFaultException(0, "User is not valid!")
        End Function

        Private Function GetPost(ByVal postid As String, ByVal username As String, ByVal password As String) As Post Implements IMetaWeblog.GetPost

            If ValidateUser(username, password) Then

                Dim controller As New ArticleController()
                Dim post As New Post()
                Dim article As ArticleInfo = controller.GetArticle(Convert.ToInt32(postid), CurrentLocale, True)

                If Not article Is Nothing Then
                    post.dateCreated = article.CreatedDate
                    post.description = HttpUtility.HtmlDecode(article.Content)
                    post.permalink = ""
                    post.postid = postid
                    post.title = article.Title
                    post.userid = article.CreatedByUser.ToString
                End If

                Return post

            End If


            Throw New XmlRpcFaultException(0, "User is not valid!")
        End Function

        Public Function GetCategories(ByVal blogid As String, ByVal username As String, ByVal password As String) As CategoryInfo() Implements IMetaWeblog.GetCategories
            Dim infolist As New List(Of CategoryInfo)
            Return infolist.ToArray()
        End Function

        Private Function getCategoriesHierarchical(ByVal blogid As String, ByVal username As String, ByVal password As String) As CategoryInfo() Implements IMetaWeblog.getCategoriesHierarchical

            Portalsettings = Portals.PortalController.GetCurrentPortalSettings

            If ValidateUser(username, password) Then


                Dim tabid As Integer = Null.NullInteger

                'Dim tabs As ArrayList = DotNetNuke.Common.Globals.GetPortalTabs(Me.Portalsettings.DesktopTabs, True, True)
                Dim tabs As List(Of DotNetNuke.Entities.Tabs.TabInfo) = TabController.GetPortalTabs(Portalsettings.PortalId, True, True, False, True, True)

                Dim objModules As New ModuleController
                Dim arrModules As New ArrayList
                Dim objModule As ModuleInfo

                Dim dicModules As System.Collections.Generic.Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo)
                Dim blnTabbAdded As Boolean = False

                For Each oTab As DotNetNuke.Entities.Tabs.TabInfo In tabs
                    dicModules = objModules.GetTabModules(oTab.TabID)
                    For Each objModule In dicModules.Values
                        If objModule.ModuleID.ToString = blogid Then
                            tabid = oTab.TabID
                        End If
                    Next
                    blnTabbAdded = False
                Next

                Dim categoryInfos As New List(Of CategoryInfo)()

                Dim articleCategories As New List(Of dnnWerk.Modules.Nuntio.Articles.CategoryInfo)
                Dim cc As New CategoryController
                articleCategories = cc.ListCategoryItems(Convert.ToInt32(blogid), Me.CurrentLocale, Date.Now, True, True, True)

                ' TODO: Implement your own logic to get category info and set the categoryInfos

                For Each objCategory As dnnWerk.Modules.Nuntio.Articles.CategoryInfo In articleCategories

                    Dim categoryInfo As New CategoryInfo
                    categoryInfo.categoryId = objCategory.CategoryID
                    categoryInfo.description = objCategory.CategoryName
                    categoryInfo.htmlUrl = NavigateURL(tabid, "", "CategoryID=" & objCategory.CategoryID.ToString)
                    categoryInfo.rssUrl = Me.ModuleDirectory & "/Rss.aspx?TabID=" & tabid.ToString & "&moduleId=" & blogid & "&CategoryID=" & objCategory.CategoryID.ToString & "&Locale=" & Me.CurrentLocale
                    categoryInfo.categoryName = objCategory.CategoryName
                    categoryInfo.parentId = Convert.ToInt32(objCategory.ParentID)
                    categoryInfos.Add(categoryInfo)

                Next

                Return categoryInfos.ToArray()

            End If


            Throw New XmlRpcFaultException(0, "User is not valid!")
        End Function

        Public Function setPostCategories(ByVal postid As String, ByVal username As String, ByVal password As String, ByVal categories As CategoryInfo()) As Boolean Implements IMetaWeblog.setPostCategories

            If ValidateUser(username, password) Then

                Dim cc As New CategoryController

                'delete old relations
                cc.DeleteRelation(postid)

                'add each new category
                For Each objCategory In categories
                    cc.AddRelation(postid, Integer.Parse(objCategory.categoryId))
                Next

                Return True

            End If

            Throw New XmlRpcFaultException(0, "User is not valid!")

        End Function

        Public Function getPostCategories(ByVal postid As String, ByVal username As String, ByVal password As String) As CategoryInfo() Implements IMetaWeblog.getPostCategories

            If Not IsNumeric(postid) Then
                Throw New XmlRpcInvalidParametersException("Could not load article from given id: [" & postid.ToString & "]!")
            End If

            If ValidateUser(username, password) Then

                Dim infoList As New List(Of CategoryInfo)
                Dim moduleid As Integer = GetModuleId(postid)
                Dim catcontroller As New CategoryController
                Dim lst As New List(Of Integer)
                lst = catcontroller.GetRelationsByitemId(postid)

                For Each item As Integer In lst
                    Dim oCat As dnnWerk.Modules.Nuntio.Articles.CategoryInfo = catcontroller.GetCategory(item, moduleid, CurrentLocale, True)
                    If Not oCat Is Nothing Then

                        Dim blogCat As New CategoryInfo
                        blogCat.categoryId = oCat.CategoryID
                        blogCat.categoryName = oCat.CategoryName
                        blogCat.isPrimary = False
                        infoList.Add(blogCat)

                    End If
                Next

                Return infoList.ToArray()

            End If

            Throw New XmlRpcFaultException(0, "User is not valid!")

        End Function

        Private Function GetRecentPosts(ByVal blogid As String, ByVal username As String, ByVal password As String, ByVal numberOfPosts As Integer) As Post() Implements IMetaWeblog.GetRecentPosts


            If ValidateUser(username, password) Then


                Dim posts As New List(Of Post)()

                Dim controller As New ArticleController
                Dim articles As New List(Of ArticleInfo)
                Dim TotalCount As Integer = 0
                articles = controller.GetArticlesPaged(TotalCount, Convert.ToInt32(blogid), Me.CurrentLocale, numberOfPosts, 0, Date.Now, Null.NullInteger, Null.NullInteger, ArticleBase.ArticleSortOrder.publishdatedesc, New List(Of Integer), True, objUser.UserID, True, True, True, True, True, True)
                For Each article As ArticleInfo In articles

                    Dim post As New Post
                    post.dateCreated = article.CreatedDate
                    post.description = HttpUtility.HtmlDecode(article.Content)
                    post.permalink = ""
                    post.postid = article.itemId.ToString
                    post.title = article.Title
                    post.userid = article.CreatedByUser.ToString
                    posts.Add(post)

                Next

                Return posts.ToArray()

            End If


            Throw New XmlRpcFaultException(0, "User is not valid!")
        End Function

        Private Function NewMediaObject(ByVal blogid As String, ByVal username As String, ByVal password As String, ByVal mediaObject As MediaObject) As MediaObjectInfo Implements IMetaWeblog.NewMediaObject

            Portalsettings = Portals.PortalController.GetCurrentPortalSettings

            If ValidateUser(username, password) Then


                Dim info As MediaObjectInfo

                Try

                    Dim virtualPath As String
                    Dim mediaObjectName As String = String.Empty
                    Dim fullFilePathAndName As String = String.Empty
                    info.url = ""

                    Try

                        ' Shorten WindowsLiveWriter and create one file name.
                        mediaObjectName = mediaObject.name.Replace("WindowsLiveWriter", "WLW")
                        mediaObjectName = mediaObjectName.Replace("/", "-")

                        virtualPath = "/Nuntio/" & blogid.ToString() & "/" & mediaObjectName

                        fullFilePathAndName = Portalsettings.HomeDirectoryMapPath + virtualPath.Replace("/", "\")
                        fullFilePathAndName = fullFilePathAndName.Replace("\\", "\")

                        '_provider.BeforeMediaSaved(fullFilePathAndName);

                        Dim output As New FileStream(CreateFoldersForFilePath(fullFilePathAndName), FileMode.Create)
                        Dim bw As BinaryWriter = New BinaryWriter(output)
                        bw.Write(mediaObject.bits)

                        output.Close()
                    Catch exc As IOException
                        Throw New XmlRpcFaultException(0, "An error occurred while saving an image related to this article post.")
                    End Try
                    Dim finalUrl As String = Portalsettings.HomeDirectory.Replace("\", "/")

                    finalUrl = finalUrl & virtualPath
                    info.url = finalUrl.Replace("//", "/")

                Catch ex As Exception

                    LogException(ex)

                    Throw New XmlRpcFaultException(0, "An error occurred while saving an image related to this blog post.")

                End Try

                Return info


            End If


            Throw New XmlRpcFaultException(0, "User is not valid!")
        End Function

        Private Function CreateFoldersForFilePath(ByVal folderPath As String) As String
            Dim path As String = folderPath.Substring(0, folderPath.LastIndexOf("\"))
            If Not Directory.Exists(path) Then
                Directory.CreateDirectory(path)
            End If
            Return folderPath
        End Function

        Private Function DeletePost(ByVal key As String, ByVal postid As String, ByVal username As String, ByVal password As String, ByVal publish As Boolean) As Boolean Implements IMetaWeblog.DeletePost


            If ValidateUser(username, password) Then


                Dim result As Boolean = False



                ' TODO: Implement your own logic to delete the post and set the result




                Return result
            End If


            Throw New XmlRpcFaultException(0, "User is not valid!")
        End Function

        Private Function GetUsersBlogs(ByVal key As String, ByVal username As String, ByVal password As String) As BlogInfo() Implements IMetaWeblog.GetUsersBlogs

            Portalsettings = Portals.PortalController.GetCurrentPortalSettings

            If ValidateUser(username, password) Then

                Dim infoList As New List(Of BlogInfo)()

                Dim tabs As List(Of DotNetNuke.Entities.Tabs.TabInfo) = TabController.GetPortalTabs(Portalsettings.PortalId, True, True, False, True, True)

                Dim objModules As New ModuleController
                Dim arrModules As New ArrayList
                Dim objModule As ModuleInfo

                Dim dicModules As System.Collections.Generic.Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo)
                Dim blnTabbAdded As Boolean = False

                For Each oTab As DotNetNuke.Entities.Tabs.TabInfo In tabs
                    dicModules = objModules.GetTabModules(oTab.TabID)
                    For Each objModule In dicModules.Values


                        If IsArticleModule(objModule, oTab.TabID) Then

                            Dim oItem As New BlogInfo
                            oItem.blogid = objModule.ModuleID
                            oItem.blogName = objModule.ModuleTitle
                            oItem.url = NavigateURL(oTab.TabID)
                            infoList.Add(oItem)

                        End If

                    Next
                    blnTabbAdded = False
                Next

                Return infoList.ToArray()

            End If

            Throw New XmlRpcFaultException(0, "User is not valid!")

        End Function

        Private Function GetUserInfo(ByVal key As String, ByVal username As String, ByVal password As String) As UserInfo Implements IMetaWeblog.GetUserInfo


            If ValidateUser(username, password) Then


                Dim info As New UserInfo()



                ' TODO: Implement your own logic to get user info objects and set the info




                Return info
            End If


            Throw New XmlRpcFaultException(0, "User is not valid!")
        End Function

#End Region

#Region "Private Methods"

        Private Function GetModuleIdByBlogId(ByVal blogId As String) As Integer

            Dim tabs As List(Of DotNetNuke.Entities.Tabs.TabInfo) = TabController.GetPortalTabs(Portalsettings.PortalId, True, True, False, True, True)

            Dim objModules As New ModuleController
            Dim arrModules As New ArrayList
            Dim objModule As ModuleInfo

            Dim dicModules As System.Collections.Generic.Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo)
            Dim blnTabbAdded As Boolean = False

            For Each oTab As DotNetNuke.Entities.Tabs.TabInfo In tabs
                dicModules = objModules.GetTabModules(oTab.TabID)
                For Each objModule In dicModules.Values
                    If objModule.ModuleID.ToString = blogId Then
                        Return objModule.ModuleID
                    End If
                Next
                blnTabbAdded = False
            Next

        End Function

        Private Function GetModuleId(ByVal postid As String) As Integer

            Dim moduleid As Integer = Null.NullInteger
            Dim strSQL As String = "Select ModuleId from {objectQualifier}pnc_NewsItems Where ItemId = " & postid
            Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance.ExecuteSQL(strSQL)
            While dr.Read
                moduleid = Convert.ToInt32(dr("ModuleId"))
            End While
            dr.Close()
            dr.Dispose()

            Return moduleid

        End Function

        Private Function IsAuthorized(ByVal objUser As Entities.Users.UserInfo, ByVal Roles As String) As Boolean
            Return True
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
                    mModuleId = CType(mSettings("NewsModuleId"), Integer)
                End If

                If mSettings.Contains("NewsModuleTab") Then
                    mTabId = CType(mSettings("NewsModuleTab"), Integer)
                End If

                If (mTabId = TabId) AndAlso (mModuleId = objModule.ModuleID) Then
                    Return True
                End If

            End If

            Return False

        End Function

        Private Function ValidateUser(ByVal username As String, ByVal password As String) As Boolean

            Portalsettings = Portals.PortalController.GetCurrentPortalSettings

            Dim status As DotNetNuke.Security.Membership.UserLoginStatus = DotNetNuke.Security.Membership.UserLoginStatus.LOGIN_FAILURE
            Dim oUser As Entities.Users.UserInfo = Users.UserController.GetUserByName(Portalsettings.PortalId, username)

            If Not oUser Is Nothing Then

                DotNetNuke.Entities.Users.UserController.UserLogin(Portalsettings.PortalId, username, password, "", Portalsettings.PortalName, HttpContext.Current.Request.UserHostAddress, status, False)

                If (status = DotNetNuke.Security.Membership.UserLoginStatus.LOGIN_SUCCESS Or status = DotNetNuke.Security.Membership.UserLoginStatus.LOGIN_SUPERUSER) Then
                    objUser = oUser
                    Return True
                End If

            End If


            Return False

        End Function



#End Region

    End Class

End Namespace