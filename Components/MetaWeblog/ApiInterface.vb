Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports CookComputing.XmlRpc

Namespace dnnWerk.Modules.Nuntio.Articles.MetaWeblog

    Public Interface IMetaWeblog

#Region "Wordpress API"

        <XmlRpcMethod("wp.getCategories")> _
        Function getCategoriesHierarchical(ByVal blogid As String, ByVal username As String, ByVal password As String) As CategoryInfo()

#End Region

#Region "MT API"

        <XmlRpcMethod("mt.setPostCategories")> _
        Function setPostCategories(ByVal postid As String, ByVal username As String, ByVal password As String, ByVal categories As CategoryInfo()) As Boolean

        <XmlRpcMethod("mt.getPostCategories")> _
        Function getPostCategories(ByVal postid As String, ByVal username As String, ByVal password As String) As CategoryInfo()


#End Region

#Region "MetaWeblog API"

        <XmlRpcMethod("metaWeblog.newPost")> _
        Function AddPost(ByVal blogid As String, ByVal username As String, ByVal password As String, ByVal post As Post, ByVal publish As Boolean) As String

        <XmlRpcMethod("metaWeblog.editPost")> _
        Function UpdatePost(ByVal postid As String, ByVal username As String, ByVal password As String, ByVal post As Post, ByVal publish As Boolean) As Boolean

        <XmlRpcMethod("metaWeblog.getPost")> _
        Function GetPost(ByVal postid As String, ByVal username As String, ByVal password As String) As Post

        <XmlRpcMethod("metaWeblog.getCategories")> _
        Function GetCategories(ByVal blogid As String, ByVal username As String, ByVal password As String) As CategoryInfo()

        <XmlRpcMethod("metaWeblog.getRecentPosts")> _
        Function GetRecentPosts(ByVal blogid As String, ByVal username As String, ByVal password As String, ByVal numberOfPosts As Integer) As Post()

        <XmlRpcMethod("metaWeblog.newMediaObject")> _
        Function NewMediaObject(ByVal blogid As String, ByVal username As String, ByVal password As String, ByVal mediaObject As MediaObject) As MediaObjectInfo

#End Region

#Region "Blogger API"

        <XmlRpcMethod("blogger.deletePost")> _
        Function DeletePost(ByVal key As String, ByVal postid As String, ByVal username As String, ByVal password As String, ByVal publish As Boolean) As <XmlRpcReturnValue(Description:="Returns true.")> Boolean

        <XmlRpcMethod("blogger.getUsersBlogs")> _
        Function GetUsersBlogs(ByVal key As String, ByVal username As String, ByVal password As String) As BlogInfo()

        <XmlRpcMethod("blogger.getUserInfo")> _
        Function GetUserInfo(ByVal key As String, ByVal username As String, ByVal password As String) As UserInfo

#End Region

    End Interface

End Namespace