Imports System
Imports System.Data
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports CookComputing.XmlRpc

Namespace dnnWerk.Modules.Nuntio.Articles.MetaWeblog


#Region "Structs"

    Public Structure BlogInfo

        Public blogid As String

        Public url As String

        Public blogName As String

    End Structure

    Public Structure Category

        Public categoryId As String

        Public categoryName As String

    End Structure

    <XmlRpcMissingMapping(MappingAction.Ignore)> _
    Public Structure CategoryInfo

        Public description As String

        Public htmlUrl As String

        Public rssUrl As String

        Public categoryName As String

        Public categoryId As String

        Public parentId As String

        Public isPrimary As Boolean

    End Structure

    <XmlRpcMissingMapping(MappingAction.Ignore)> _
    Public Structure Enclosure

        Public length As Integer

        Public type As String

        Public url As String

    End Structure

    <XmlRpcMissingMapping(MappingAction.Ignore)> _
    Public Structure Post

        Public dateCreated As DateTime

        Public description As String

        Public title As String

        'Public categories As String()

        Public permalink As String

        Public postid As Object

        Public userid As String

    End Structure

    <XmlRpcMissingMapping(MappingAction.Ignore)> _
    Public Structure Source

        Public name As String

        Public url As String

    End Structure

    Public Structure UserInfo

        Public userid As String

        Public firstname As String

        Public lastname As String

        Public nickname As String

        Public email As String

        Public url As String

    End Structure

    <XmlRpcMissingMapping(MappingAction.Ignore)> _
    Public Structure MediaObject

        Public name As String

        Public type As String

        Public bits As Byte()

    End Structure

    <Serializable()> _
    Public Structure MediaObjectInfo

        Public url As String

    End Structure

#End Region

End Namespace