Imports System
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users

Namespace dnnWerk.Modules.Nuntio.Articles
    Public Class Templates

        Public Enum ArticlesTemplate
            Email_Container
            Email_Item
            Listing_Header
            Listing_Item
            Listing_ItemFirst
            Listing_ItemAlternate
            Listing_ItemFeatured
            Listing_Footer
            Detail_View
            Comment_Item
            Comment_Notify
            Mailing_Subscribe
            Mailing_Unsubscribe
            Scroller_Item
            Image_Item
            Attachment_Item
            Journal_Item
        End Enum

        Public Enum NewsletterTemplate
            Mailing_Header
            Mailing_Item
            Mailing_Footer
        End Enum

        Public Shared Function GetTemplate(ByVal strPath As String, ByVal Portalsettings As DotNetNuke.Entities.Portals.PortalSettings, ByVal Locale As String) As String

            Dim path As String = strPath.Replace(".txt", "." & Locale & ".txt")

            If Not System.IO.File.Exists(path) Then
                path = strPath
            End If

            If System.IO.File.Exists(path) Then
                Dim templ As String = ""
                Dim sr As New System.IO.StreamReader(path)
                templ = sr.ReadToEnd
                sr.Close()
                sr.Dispose()
                Return templ
            Else
                Return ""
            End If

        End Function

        Public Shared Sub UpdateTemplate(ByVal path As String, ByVal content As String)
            Dim sw As System.IO.StreamWriter = System.IO.File.CreateText(path)
            sw.Write(content)
            sw.Close()
            sw.Dispose()
        End Sub

    End Class
End Namespace

