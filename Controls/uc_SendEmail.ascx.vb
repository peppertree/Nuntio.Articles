Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_SendEmail
        Inherits ArticleModuleBase


#Region "Private Members"

        Private _article As ArticleInfo = Nothing

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            'DotNetNuke.Framework.AJAX.RegisterScriptManager()
            RegisterCss()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not Page.IsPostBack Then
                LoadOptions()
                BindArticle(False)
            End If

        End Sub

        Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            Me.ctlAjax.ResponseScripts.Add("CloseDialog()")
        End Sub

        Private Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click

            SaveOptions()
            SendEmail()

        End Sub

        Private Sub btnRefresh_Click(sender As Object, e As System.EventArgs) Handles btnRefresh.Click
            BindArticle(True)
        End Sub

        Private Sub ctlTabs_TabClick(sender As Object, e As Telerik.Web.UI.RadTabStripEventArgs) Handles ctlTabs.TabClick

            SaveOptions()

            Select Case e.Tab.Index

                Case 0 'edit html

                    'do nothing

                Case 1 'edit text

                    'do nothing
                    txtEditText.Text = MakeText(ctlEditHtml.Content)

                Case 2 'set options

                    'do nothing

            End Select

        End Sub

#End Region

#Region "Private Methods"

        Private Sub RegisterCss()

            Dim strCssUrl1 As String = Me.ModuleDirectory & "/templates/" & ArticleTheme & "/template.css"
            Dim strCssUrl2 As String = Me.ModuleDirectory & "/Css/Edit.css"

            Dim blnAlreadyRegistered As Boolean = False
            For Each ctrl As Control In Me.Page.Header.Controls

                If TypeOf (ctrl) Is HtmlLink Then
                    Dim ctrlCss As HtmlLink = CType(ctrl, HtmlLink)
                    If ctrlCss.Href.ToLower = strCssUrl1.ToLower Then
                        blnAlreadyRegistered = True
                        Exit For
                    End If
                End If

            Next

            If Not blnAlreadyRegistered Then

                Dim ctrlLink1 As New HtmlLink
                ctrlLink1.Href = strCssUrl1
                ctrlLink1.Attributes.Add("rel", "stylesheet")
                ctrlLink1.Attributes.Add("type", "text/css")
                ctrlLink1.Attributes.Add("media", "screen")

                Dim ctrlLink2 As New HtmlLink
                ctrlLink2.Href = strCssUrl2
                ctrlLink2.Attributes.Add("rel", "stylesheet")
                ctrlLink2.Attributes.Add("type", "text/css")
                ctrlLink2.Attributes.Add("media", "screen")

                Me.Page.Header.Controls.Add(ctrlLink1)
                Me.Page.Header.Controls.Add(ctrlLink2)

            End If

        End Sub

        Private Sub BindArticle(RefreshArticle As Boolean)

            If Not ArticleId = Null.NullInteger Then

                Dim StoredArticleId As Integer = CType(DotNetNuke.Services.Personalization.Personalization.GetProfile("NUNTIO-NL", "NL_ArticleId"), Integer)
                If (StoredArticleId = ArticleId) And RefreshArticle = False Then

                    ctlEditHtml.Content = CType(DotNetNuke.Services.Personalization.Personalization.GetProfile("NUNTIO-NL", "NL_HTMLVersion"), String)
                    txtEditText.Text = CType(DotNetNuke.Services.Personalization.Personalization.GetProfile("NUNTIO-NL", "NL_TextVersion"), String)

                Else

                    Dim ctrl As New ArticleController
                    _article = ctrl.GetArticle(ArticleId, CurrentLocale, True)

                    If Not _article Is Nothing Then

                        Dim htmlItem As String = ""
                        ProcessItemBody(htmlItem, _article, EmailItemTemplate, "", True, TabId)
                        Dim htmlEmail As String = EmailContainerTemplate.Replace("[SUBJECT]", txtSubject.Text).Replace("[ARTICLE]", htmlItem)
                        ctlEditHtml.Content = htmlEmail

                        txtEditText.Text = MakeText(htmlEmail)

                    End If

                End If

            End If

        End Sub

        Private Function MakeText(strHtml As String) As String

            'todo: remove styles
            Dim posStyle1 As Integer = strHtml.IndexOf("<style type=""text/css"">")
            Dim posStyle2 As Integer = strHtml.IndexOf("</style>")

            Try
                Dim strRemove As String = strHtml.Substring(posStyle1, (posStyle2 + 8) - posStyle1)
                strHtml = strHtml.Replace(strRemove, "")
            Catch
            End Try

            strHtml = strHtml.Replace("<p>", "")
            strHtml = strHtml.Replace("</p>", "[br][br]")
            strHtml = strHtml.Replace("[br]", "[br]")
            strHtml = strHtml.Replace("<h1>", "[br][br]")
            strHtml = strHtml.Replace("</h1>", "[br]********************************************************[br][br]")
            strHtml = strHtml.Replace("<h2>", "[br][br]")
            strHtml = strHtml.Replace("</h2>", "[br]--------------------------------------------------------[br][br]")
            strHtml = strHtml.Replace("<h3>", "[br][br]")
            strHtml = strHtml.Replace("</h3>", "[br]........................................................[br][br]")
            strHtml = strHtml.Replace("<ul>", "[br][br]")
            strHtml = strHtml.Replace("</li>", "[br]")
            strHtml = strHtml.Replace("<li>", "- ")
            strHtml = strHtml.Replace("</ul>", "[br][br]")

            strHtml = DotNetNuke.Common.Utilities.HtmlUtils.Clean(strHtml, False)

            strHtml = strHtml.Replace("[br]", Environment.NewLine)

            Return strHtml

        End Function

        Private Sub SendEmail()

            Dim strListServer As String = txtListServer.Text
            Dim strSenderUsername As String = txtSenderUsername.Text.Trim
            Dim strSenderPassword As String = txtSenderPassword.Text.Trim
            Dim strRecipient As String = txtRecipient.Text
            Dim strSubject As String = txtSubject.Text.Trim
            Dim strBodyHtml As String = ctlEditHtml.Content
            Dim strBodyText As String = txtEditText.Text
            Dim lstImages As New List(Of System.Net.Mail.LinkedResource)

            'set up a new mail message object, set basic properties
            Dim mailMessage As System.Net.Mail.MailMessage = New System.Net.Mail.MailMessage()
            mailMessage.From = New System.Net.Mail.MailAddress(strSenderUsername)
            mailMessage.To.Add(strRecipient)
            mailMessage.Subject = strSubject


            'parse html body for images and populate the image ressource list
            strBodyHtml = ProcessFileLinks(strBodyHtml, lstImages)

            'create two views, one text, one HTML.
            Dim plainTextView As System.Net.Mail.AlternateView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(strBodyText, Nothing, "text/plain")
            Dim htmlView As System.Net.Mail.AlternateView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(strBodyHtml, Nothing, "text/html")


            'add embedded resources to html view
            For Each objImage As System.Net.Mail.LinkedResource In lstImages
                htmlView.LinkedResources.Add(objImage)
            Next

            'add views to message object
            mailMessage.AlternateViews.Add(plainTextView)
            mailMessage.AlternateViews.Add(htmlView)


            'set up auth credentials for the smtp client
            Dim credentials As New System.Net.NetworkCredential
            credentials.Domain = ""
            credentials.UserName = strSenderUsername
            credentials.Password = strSenderPassword

            'set up the smtp client
            Dim smtpClient As New System.Net.Mail.SmtpClient()
            smtpClient.Credentials = credentials
            smtpClient.DeliveryMethod = Net.Mail.SmtpDeliveryMethod.Network
            smtpClient.EnableSsl = False
            smtpClient.Host = strListServer
            smtpClient.Port = 25

            'send mail
            Try
                smtpClient.Send(mailMessage)
                lblMessage.Text = "Message sent sucessfully"
                pnlMessage.Visible = True
            Catch sEx As System.Net.Mail.SmtpException
                lblMessage.Text = sEx.Message
                pnlMessage.Visible = True
            Catch ex As Exception
                lblMessage.Text = ex.InnerException.Message
                pnlMessage.Visible = True
            End Try


        End Sub

        Private Function ProcessFileLinks(ByVal strHTML As String, ByRef images As List(Of System.Net.Mail.LinkedResource)) As String

            'try to decode the string first
            'strHTML = Server.HtmlDecode(strHTML)

            Dim strWork As String = ""
            Dim strFile As String = ""
            Dim posImgStart As Integer
            Dim posImgEnd As Integer
            Dim posNext As Integer = 0
            Dim strImage As String = ""

            Dim posLinkEnd As Integer
            Dim posLinkStart As Integer
            Dim strLink As String = ""

            Dim FileID As Integer = Null.NullInteger



            'get first position of first image tag
            Dim posImg As Integer = strHTML.ToLower.IndexOf("<img", 0)

            'loop as long as image tags are found
            While posImg <> -1

                'starting point in string
                posImgStart = strHTML.ToLower.IndexOf("<img", posImg)

                'strip out tag, starting at first occurence
                strWork = strHTML.Substring(posImgStart)

                'find end of image tag
                posImgEnd = strWork.IndexOf(">")

                'again, strip out mage tag, ending at the last occurence of >
                strWork = strWork.Substring(0, posImgEnd + 1)

                'not needed
                'posNext = posImgStart + posImgEnd

                'now find the beginning of the path in our string
                posImgStart = strWork.ToLower.IndexOf("src=")
                'define starting point for next stripping point
                strWork = strWork.Substring(posImgStart + 5)

                'find the end of the string to be parsed for in current string
                posImgEnd = strWork.IndexOf("""")

                strWork = strWork.Substring(0, posImgEnd)

                'check if the image tag links to an internal file
                If strWork.ToLower.IndexOf(PortalSettings.HomeDirectory.ToLower) <> -1 Then

                    Dim fId As String = ""

                    Dim path As String = Server.MapPath(strWork)
                    If System.IO.File.Exists(path) Then

                        Dim f As New System.IO.FileInfo(path)

                        Dim objResource As New System.Net.Mail.LinkedResource(path)
                        objResource.ContentId = "IMG_" & f.Name
                        fId = objResource.ContentId
                        images.Add(objResource)

                    End If


                    'parse out portal home dir
                    Dim ressourcepath As String = "cid:" & fId
                    strImage = ressourcepath

                Else
                    strImage = strWork
                End If

                Try
                    'replace our work in the html string
                    strHTML = strHTML.Replace(strWork, strImage)
                Catch ex As Exception
                End Try


                'set start position for next search
                'posNext = strHTML.IndexOf(strImage)

                'set new start position for the next loop otherwise this will never end...
                posImg = strHTML.ToLower.IndexOf("<img", posImg + 4)
                posImg = posImg

            End While

            'get first position of first image tag
            Dim posLink As Integer = strHTML.ToLower.IndexOf("<a ", 0)

            'loop as long as image tags are found
            While posLink <> -1

                'starting point in string
                posLinkStart = strHTML.ToLower.IndexOf("<a ", posLink)

                'strip out tag, starting at first occurence
                strWork = strHTML.Substring(posLinkStart)

                'find end of image tag
                posLinkEnd = strWork.IndexOf(">")

                'again, strip out mage tag, ending at the last occurence of >
                strWork = strWork.Substring(0, posLinkEnd + 1)

                'not needed
                'posNext = posImgStart + posImgEnd

                'now find the beginning of the path in our string
                posLinkStart = strWork.ToLower.IndexOf("href=")
                'define starting point for next stripping point
                strWork = strWork.Substring(posLinkStart + 6)

                'find the end of the string to be parsed for in current string
                posLinkEnd = strWork.IndexOf("""")

                strWork = strWork.Substring(0, posLinkEnd)

                'check if the image tag links to an internal file
                If strWork.ToLower.IndexOf("http://") = -1 Then
                    Dim url As String = PortalSettings.PortalAlias.HTTPAlias & "/" & strWork
                    strLink = "http://" & url.Replace("//", "/")
                Else
                    strLink = strWork
                End If

                Try
                    'replace our work in the html string
                    strHTML = strHTML.Replace(strWork, strLink)
                Catch ex As Exception
                End Try


                'set start position for next search
                'posNext = strHTML.IndexOf(strImage)

                'set new start position for the next loop otherwise this will never end...
                posLink = strHTML.ToLower.IndexOf("<a ", posLink + 3)
                posLink = posLink

            End While

            Return strHTML

        End Function

        Private Sub SaveOptions()

            DotNetNuke.Services.Personalization.Personalization.SetProfile("NUNTIO-NL", "NL_ListServer", txtListServer.Text.Trim)
            DotNetNuke.Services.Personalization.Personalization.SetProfile("NUNTIO-NL", "NL_SenderUsername", txtSenderUsername.Text.Trim)
            DotNetNuke.Services.Personalization.Personalization.SetProfile("NUNTIO-NL", "NL_SenderPassword", txtSenderPassword.Text.Trim)
            DotNetNuke.Services.Personalization.Personalization.SetProfile("NUNTIO-NL", "NL_Recipient", txtRecipient.Text.Trim)
            DotNetNuke.Services.Personalization.Personalization.SetProfile("NUNTIO-NL", "NL_Subject", txtSubject.Text.Trim)
            DotNetNuke.Services.Personalization.Personalization.SetProfile("NUNTIO-NL", "NL_ArticleId", ArticleId.ToString)
            DotNetNuke.Services.Personalization.Personalization.SetProfile("NUNTIO-NL", "NL_HTMLVersion", ctlEditHtml.Content)
            DotNetNuke.Services.Personalization.Personalization.SetProfile("NUNTIO-NL", "NL_TextVersion", txtEditText.Text)

        End Sub

        Private Sub LoadOptions()

            Try
                txtListServer.Text = CType(DotNetNuke.Services.Personalization.Personalization.GetProfile("NUNTIO-NL", "NL_ListServer"), String)
            Catch
            End Try
            Try
                txtSenderUsername.Text = CType(DotNetNuke.Services.Personalization.Personalization.GetProfile("NUNTIO-NL", "NL_SenderUsername"), String)
            Catch
            End Try
            Try
                txtSenderPassword.Text = CType(DotNetNuke.Services.Personalization.Personalization.GetProfile("NUNTIO-NL", "NL_SenderPassword"), String)
            Catch
            End Try
            Try
                txtRecipient.Text = CType(DotNetNuke.Services.Personalization.Personalization.GetProfile("NUNTIO-NL", "NL_Recipient"), String)
            Catch
            End Try
            Try
                txtSubject.Text = CType(DotNetNuke.Services.Personalization.Personalization.GetProfile("NUNTIO-NL", "NL_Subject"), String)
            Catch
            End Try

        End Sub

#End Region




    End Class
End Namespace
