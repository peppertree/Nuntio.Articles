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

    Public Class ArchiveController

        Public Function GetNewsArchive(ByVal moduleId As Integer, ByVal Locale As String, ByVal StartDate As DateTime, ByVal ShowFuture As Boolean, ByVal ShowPast As Boolean) As List(Of ArchiveInfo)
            Return CBO.FillCollection(Of ArchiveInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("NuntioArticles_GetArchiveTree", moduleId, GetNull(StartDate), ShowFuture, ShowPast))
        End Function

        Private Function GetNull(ByVal Field As Object) As Object
            Return Null.GetNull(Field, DBNull.Value)
        End Function

    End Class


End Namespace

