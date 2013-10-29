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
    Public Class JournalIntegration

        Public Shared Sub SaveArticleToJournal(ArticleId As Integer, TabId As Integer, UserId As Integer, PortalId As Integer, strUrl As String, strTitle As String, strSummary As String, strImageUrl As String)

            If JournalSupported() = False Then Exit Sub

            'OK, we're in dnn 6.2 at least

            Dim strEntryType As String = ""
            Dim strKey As String = "NUA_ArticleEntry_" & ArticleId.ToString

            Dim asm As System.Reflection.Assembly
            asm = System.Reflection.Assembly.Load("DotNetNuke")

            Dim ac As Object = Nothing
            ac = asm.CreateInstance("DotNetNuke.Services.Journal.JournalController")

            Dim ji As Object = Nothing
            ji = asm.CreateInstance("DotNetNuke.Services.Journal.JournalItem")

            Dim di As Object = Nothing
            di = asm.CreateInstance("DotNetNuke.Services.Journal.ItemData")

            If Not ac Is Nothing Then


                ji = ac.Instance().GetJournalItemByKey(PortalId, strKey)
                If Not ji Is Nothing Then
                    ac.Instance().DeleteJournalItemByKey(PortalId, strKey)
                End If

                If ji Is Nothing Then
                    ji = asm.CreateInstance("DotNetNuke.Services.Journal.JournalItem")
                End If

                'let's post!

                ji.PortalId = PortalId
                ji.ProfileId = UserId
                ji.UserId = UserId
                ji.Title = strTitle
                ji.ItemData = di
                ji.ItemData.Url = strUrl
                ji.ItemData.ImageUrl = strImageUrl
                ji.Summary = strSummary
                ji.Body = Nothing
                ji.JournalTypeId = 15
                ji.ObjectKey = strKey
                ji.SecuritySet = "E,"

                ac.Instance().SaveJournalItem(ji, TabId)

            End If

        End Sub

        Public Shared Function JournalSupported() As Boolean

            Dim iMajor As Integer = DotNetNuke.Common.Globals.DataBaseVersion.Major
            Dim iMinor As Integer = DotNetNuke.Common.Globals.DataBaseVersion.Minor
            Dim iRevision As Integer = DotNetNuke.Common.Globals.DataBaseVersion.Revision

            If iMajor < 6 Then
                Return False
            End If

            If iMinor >= 2 Then
                Return True
            End If

            Return False

        End Function

    End Class
End Namespace

