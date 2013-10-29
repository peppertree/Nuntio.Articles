Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Xml
Imports System.Security.Cryptography
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Search
Imports DotNetNuke.Entities.Users



Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class AuthorController

        Public Function GetAuthors(ByVal ModuleID As Integer, ByVal StartDate As DateTime, ByVal IncludeFutureArticles As Boolean, ByVal IncludePastArticles As Boolean, ByVal IncludeExpired As Boolean) As List(Of AuthorInfo)
            Dim dr As IDataReader = Nothing
            Return CBO.FillCollection(Of AuthorInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetAuthors", GetNull(ModuleID), StartDate, IncludeFutureArticles, IncludePastArticles, IncludeExpired))
        End Function

        Private Function GetNull(ByVal Field As Object) As Object
            Return Null.GetNull(Field, DBNull.Value)
        End Function

    End Class


End Namespace

