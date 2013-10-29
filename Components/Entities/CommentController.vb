
Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Xml
Imports System.Security.Cryptography
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Search

Imports DotNetNuke.Data
Imports DotNetNuke.Framework
Imports System.ComponentModel

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class CommentController

#Region "Private Methods"

        Private Function GetNull(ByVal Field As Object) As Object
            Return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value)
        End Function

#End Region

#Region "Public Methods"

        Public Function [Get](ByVal itemId As Integer) As CommentInfo

            Return CBO.FillObject(Of CommentInfo)(DotNetNuke.Data.DataProvider.Instance().ExecuteReader("pnc_NewsComment_Get", itemId))

        End Function

        <DataObjectMethod(DataObjectMethodType.Select, True)> _
        Public Function [List](ByVal ArticleId As Integer) As List(Of CommentInfo)

            Return CBO.FillCollection(Of CommentInfo)(DotNetNuke.Data.DataProvider.Instance().ExecuteReader("pnc_NewsComment_List", ArticleId))

        End Function


        <DataObjectMethod(DataObjectMethodType.Insert, True)> _
        Public Function Add(ByVal objComment As CommentInfo) As Integer

            Return CType(DotNetNuke.Data.DataProvider.Instance().ExecuteScalar("pnc_NewsComment_Add", objComment.ArticleId, objComment.CreatedBy, objComment.CreatedDate, objComment.Comment, objComment.IsApproved, GetNull(objComment.ApprovedBy), objComment.RemoteAddress, GetNull(objComment.Displayname), GetNull(objComment.Email), objComment.LoadGravatar), Integer)

        End Function

        <DataObjectMethod(DataObjectMethodType.Update, True)> _
        Public Sub Update(ByVal objComment As CommentInfo)

            DotNetNuke.Data.DataProvider.Instance().ExecuteNonQuery("pnc_NewsComment_Update", objComment.ItemId, objComment.ArticleId, objComment.CreatedBy, objComment.CreatedDate, objComment.Comment, objComment.IsApproved, GetNull(objComment.ApprovedBy), objComment.RemoteAddress, GetNull(objComment.Displayname), GetNull(objComment.Email), objComment.LoadGravatar)

        End Sub

        <DataObjectMethod(DataObjectMethodType.Delete, True)> _
        Public Sub Delete(ByVal objComment As CommentInfo)

            DotNetNuke.Data.DataProvider.Instance().ExecuteNonQuery("pnc_NewsComment_Delete", GetNull(objComment.ItemId))

        End Sub

#End Region

    End Class


End Namespace
