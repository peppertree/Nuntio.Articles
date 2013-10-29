Imports System.Web
Imports System.IO
Imports System.Text
Imports System.Xml

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Users


Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Class RSS

        Inherits System.Web.UI.Page

#Region " Private Members "

        Private _categoryid As Integer = Null.NullInteger
        Private _tabid As Integer = Null.NullInteger
        Private _tabinfo As TabInfo
        Private _moduleid As Integer = Null.NullInteger
        Private _portalid As Integer = Null.NullInteger
        Private _createdby As Integer = Null.NullInteger
        Private _locale As String = ""
        Private _startdate As DateTime = Null.NullDate
        Private _maxcount As Integer = 25
        Private _futureitems As Boolean = False
        Private _pastitems As Boolean = False
        Private _featureditems As Boolean = True
        Private _notfeatureditems As Boolean = True
        Private _title As String = Null.NullString
        Private _includeimages As Boolean = False
        Private _portalsettings As PortalSettings
        Private _lastupdatedate As DateTime = Date.Now
        Private _ttl As String = "60"

#End Region

#Region " Private Methods "

        Private Sub ReadQueryString()

            If (Request("TTL") <> "") Then
                _ttl = Convert.ToString(Request("TTL"))
            End If

            If (Request("IncludeImages") <> "") Then
                _includeimages = Convert.ToBoolean(Request("IncludeImages"))
            End If

            If (Request("FutureItems") <> "") Then
                _futureitems = Convert.ToBoolean(Request("FutureItems"))
            End If

            If (Request("FeaturedItems") <> "") Then
                _featureditems = Convert.ToBoolean(Request("FeaturedItems"))
            End If

            If (Request("NotFeaturedItems") <> "") Then
                _notfeatureditems = Convert.ToBoolean(Request("NotFeaturedItems"))
            End If

            If (Request("PastItems") <> "") Then
                _pastitems = Convert.ToBoolean(Request("PastItems"))
            End If

            If (Request("ModuleId") <> "") Then
                If (IsNumeric(Request("ModuleId"))) Then
                    _moduleid = Convert.ToInt32(Request("moduleId"))
                End If
            End If

            If (Request("TabId") <> "") Then
                If (IsNumeric(Request("TabId"))) Then
                    _tabid = Convert.ToInt32(Request("TabId"))
                End If
            End If

            If (Request("PortalId") <> "") Then
                If (IsNumeric(Request("PortalId"))) Then
                    _portalid = Convert.ToInt32(Request("PortalId"))
                End If
            End If

            If (Request("Locale") <> "") Then
                _locale = Request("Locale")
            End If

            If (Request("StartDate") <> "") Then
                _startdate = Date.Parse(Request("StartDate"))
            Else
                _startdate = Date.Now
            End If

            If (Request("CategoryId") <> "") Then
                If (IsNumeric(Request("CategoryId"))) Then
                    _categoryid = Convert.ToInt32(Request("CategoryId"))
                End If
            End If

            If (Request("MaxCount") <> "") Then
                If (IsNumeric(Request("MaxCount"))) Then
                    _maxcount = Convert.ToInt32(Request("MaxCount"))
                End If
            End If

            If (Request("CreatedBy") <> "") Then
                If (IsNumeric(Request("CreatedBy"))) Then
                    _createdby = Convert.ToInt32(Request("CreatedBy"))
                End If
            End If


        End Sub

        Private Function GetParentPortal(ByVal sportalalias As String) As String
            If (sportalalias.IndexOf("localhost") < 0) Then
                If (sportalalias.IndexOf("/") > 0) Then
                    sportalalias = sportalalias.Substring(0, sportalalias.IndexOf("/"))
                End If
            End If

            GetParentPortal = sportalalias
        End Function

        Private Function FormatTitle(ByVal title As String) As String

            Return OnlyAlphaNumericChars(title) & ".aspx"

        End Function

        Public Function OnlyAlphaNumericChars(ByVal OrigString As String) As String
            '***********************************************************
            'INPUT:  Any String
            'OUTPUT: The Input String with all non-alphanumeric characters 
            '        removed
            'EXAMPLE Debug.Print OnlyAlphaNumericChars("Hello World!")
            'output = "HelloWorld")
            'NOTES:  Not optimized for speed and will run slow on long
            '        strings.  If you plan on using long strings, consider 
            '        using alternative method of appending to output string,
            '        such as the method at
            '        http://www.freevbcode.com/ShowCode.Asp?ID=154
            '***********************************************************
            Dim lLen As Integer
            Dim sAns As String = ""
            Dim lCtr As Integer
            Dim sChar As String

            OrigString = Trim(OrigString)
            lLen = Len(OrigString)
            For lCtr = 1 To lLen
                sChar = Mid(OrigString, lCtr, 1)
                If IsAlphaNumeric(Mid(OrigString, lCtr, 1)) Then
                    sAns = sAns & sChar
                End If
            Next

            OnlyAlphaNumericChars = sAns

        End Function

        Private Function IsAlphaNumeric(ByVal sChr As String) As Boolean
            IsAlphaNumeric = sChr Like "[0-9A-Za-z]"
        End Function

        Public Function StripTags(ByVal HTML As String) As String
            Return System.Text.RegularExpressions.Regex.Replace(HTML, "<[^>]*>", "")
        End Function

        Private Sub OpenDocument(ByRef writer As XmlTextWriter)

            writer.WriteStartDocument()
            writer.WriteStartElement("rss")
            writer.WriteAttributeString("version", "2.0")
            writer.WriteAttributeString("xmlns:blogChannel", "http://backend.userland.com/blogChannelModule")
            writer.WriteStartElement("channel")
            writer.WriteElementString("title", _title)
            writer.WriteElementString("link", "http://" & _portalsettings.PortalAlias.HTTPAlias)
            writer.WriteElementString("description", _portalsettings.Description)
            writer.WriteElementString("copyright", _portalsettings.FooterText)
            writer.WriteElementString("generator", "Nuntio Articles Feed Generator")
            writer.WriteElementString("lastBuildDate", _lastupdatedate.ToString("r"))
            writer.WriteElementString("ttl", _ttl)
            writer.WriteStartElement("image")
            writer.WriteElementString("url", "http://" & (_portalsettings.PortalAlias.HTTPAlias & "/" & _portalsettings.HomeDirectory & "/" & _portalsettings.LogoFile).Replace("//", "/"))
            writer.WriteElementString("title", _title)
            writer.WriteElementString("link", "http://" & _portalsettings.PortalAlias.HTTPAlias)
            writer.WriteEndElement()

        End Sub

        Private Sub AddItem(ByRef writer As XmlTextWriter, ByVal item As ArticleInfo)

            Dim url As String = NavigateURL(_tabid, "", "ArticleId=" & item.itemId.ToString).ToLower.Replace("default.aspx", FormatTitle(item.Title))
            Dim description As String = item.Summary
            If String.IsNullOrEmpty(description) Then description = item.Content

            If _includeimages Then
                Dim strimage As String = ""

                Dim lstImages As New List(Of ArticleFileInfo)
                Dim ctrl As New ArticleFileController
                lstImages = ctrl.GetImages(item.ItemId, _moduleid, _locale, False)
                For Each oImage As ArticleFileInfo In lstImages
                    If oImage.IsPrimary Then
                        strimage = "<img src=""http://" & (_portalsettings.PortalAlias.HTTPAlias & "/" & oImage.Url).Replace("//", "/") & """ alt="""" width=""120"" style=""width:120px;float:left;margin-right: 20px;margin-bottom: 7px;"" />"
                    End If
                Next
                description = strimage & description

            End If

            writer.WriteStartElement("item")
            writer.WriteElementString("title", item.Title)
            writer.WriteElementString("author", UserController.GetUserById(_portalsettings.PortalId, item.CreatedByUser).Email)
            writer.WriteElementString("link", url)
            writer.WriteElementString("description", Server.HtmlDecode(description))
            writer.WriteElementString("pubDate", item.PublishDate.ToString("r"))
            writer.WriteEndElement()

        End Sub

        Private Sub CloseDocument(ByRef writer As XmlTextWriter)

            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndDocument()

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            _portalsettings = PortalController.GetCurrentPortalSettings
            _title = _portalsettings.PortalName

            '''first: read paramters from querystring
            ReadQueryString()

            Dim categories As New List(Of Integer)
            If _categoryid <> Null.NullInteger Then
                categories.Add(_categoryid)
            End If

            '''now load articles
            Dim nc As New ArticleController
            Dim news As New List(Of ArticleInfo)
            Dim total As Integer = 0

            news = nc.GetArticlesPaged(total, _moduleid, _locale, _maxcount, 0, _startdate, Null.NullInteger, Null.NullInteger, ArticleBase.ArticleSortOrder.publishdatedesc, categories, True, _createdby, _futureitems, _pastitems, _featureditems, _notfeatureditems, False, True)

            '''try to get last updated date
            If news.Count > 0 Then
                _lastupdatedate = news(0).PublishDate.ToString("r")
            End If

            Dim writer As XmlTextWriter = New XmlTextWriter(Response.OutputStream, System.Text.Encoding.UTF8)

            OpenDocument(writer)

            '''now loop through each element
            For Each oItem As ArticleInfo In news
                AddItem(writer, oItem)
            Next

            'For Each oNews As ArticleInfo In news
            CloseDocument(writer)
            writer.Flush()
            writer.Close()

            Response.ContentEncoding = System.Text.Encoding.UTF8
            Response.ContentType = "text/xml"
            Response.Cache.SetCacheability(HttpCacheability.Public)
            Response.End()

        End Sub

#End Region

    End Class
End Namespace
