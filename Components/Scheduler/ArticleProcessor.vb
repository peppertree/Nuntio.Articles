
Imports System.Collections
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports System.Web
Imports System.Web.Hosting
Imports System.Threading
Imports System.Collections.Specialized
Imports System.Text
Imports System.Xml
Imports System.Globalization
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules


Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class ArticleNotification
        Inherits DotNetNuke.Services.Scheduling.SchedulerClient

#Region "Private Members"

        Private _writer As System.IO.StreamWriter = Nothing
        Private _Subject As String = ""

#End Region

        Public Sub New(ByVal objScheduleHistoryItem As DotNetNuke.Services.Scheduling.ScheduleHistoryItem)
            MyBase.new()
            Me.ScheduleHistoryItem = objScheduleHistoryItem
        End Sub

        Public Overrides Sub DoWork()
            Try
                VerifyDebugging()
                Me.Progressing()
                SendNotifications()
                FinalizeDebugging()
                Me.ScheduleHistoryItem.Succeeded = True     'REQUIRED
                Me.ScheduleHistoryItem.AddLogNote(Me.Status)

            Catch exc As Exception      'REQUIRED
                Me.ScheduleHistoryItem.Succeeded = False    'REQUIRED
                Me.ScheduleHistoryItem.AddLogNote("Notification processing failed.")    'OPTIONAL
                'notification that we have errored
                Me.Errored(exc)         'REQUIRED
                FinalizeDebugging()
                'log the exception
                LogException(exc)       'OPTIONAL
            End Try
        End Sub

        Private Sub SendNotifications()

            'define variable members
            Dim newsitems As New System.Collections.Generic.List(Of ArticleInfo)
            Dim subscriptions As New System.Collections.Generic.List(Of SubscriptionInfo)
            Dim newscontroller As New ArticleController
            Dim ctrlSub As New SubscriptionController

            Dim mid As Integer = -1
            'Dim pid As Integer = -1
            'Dim loc As String = ""
            Dim intSent As Integer = 0
            Dim intNotSent As Integer = 0
            Dim intItems As Integer = 0
            Dim NewsModules As New List(Of Integer)


            'first get all module instances that might have news to send from
            Try
                newsitems = newscontroller.GetArticlesForNotificationQueue(Null.NullInteger, Now)

                If newsitems.Count > 0 Then
                    Log(newsitems.Count.ToString & " articles are in the newsletter queue.")
                Else
                    Log("No articles are in the newsletter queue, quitting job.")
                    Exit Sub
                End If

                Dim iModule As Integer = 0
                For Each item As ArticleInfo In newsitems
                    If item.moduleId <> iModule Then
                        iModule = item.moduleId
                        NewsModules.Add(item.moduleId)
                    End If
                Next

                If newsitems.Count > 0 Then
                    Log("Articles in the queue come from " & NewsModules.Count.ToString & " module(s)")
                End If


            Catch ex As Exception
                LogException(ex)
            End Try


            Me.Status = "Sending Notifications..."

            'second step though each module instance 
            'and see if there are notifications to be sent
            For Each moduleId As Integer In NewsModules
                mid = moduleId
                'load the settings of the innstabce into the global hashtable
                'LoadModulesettings(mid)

                Log("Loaded settings for module " & mid.ToString)

                ''get the from address
                'Dim sFrom As String = ""
                'Try
                '    If Me._Modulesettings.Contains("FromAddress") Then
                '        If CType(Me._Modulesettings("FromAddress"), String) <> "" Then
                '            sFrom = CType(Me._Modulesettings("FromAddress"), String)
                '        End If
                '    End If
                'Catch ex As Exception
                '    LogException(ex)
                'End Try

                'Log("From Address is " & sFrom)

                'get the from address
                Dim blnUseOriginalVersion As Boolean = True
                Try
                    blnUseOriginalVersion = CType(GetSetting("NUNTIO_UseOriginalVersion", mid), Boolean)
                Catch ex As Exception
                    LogException(ex)
                End Try

                'get all anonymous subscriptions, add them to the global 
                'subscription list
                Try

                    subscriptions = ctrlSub.ListSubscriptions(moduleId, Nothing)
                Catch ex As Exception
                    LogException(ex)
                End Try

                Log(subscriptions.Count.ToString & " anonymous subscribers found for that module")

                'get all user subscriptions,add them to the global 
                'subscription list    
                Try

                    Dim strSubscribedRoles As String = ""
                    Try
                        strSubscribedRoles = GetSetting("NUNTIO_SubscribedRoles", mid)
                    Catch
                    End Try
                    If strSubscribedRoles <> "" Then

                        Log("This module also contains role based subscriptions")

                        Dim rolecontroller As New DotNetNuke.Security.Roles.RoleController
                        Dim Roles As String() = strSubscribedRoles.Split(Char.Parse(","))
                        For Each role As String In Roles

                            If role <> "" Then

                                Log("Looking up users in role " & role)

                                Dim arrUsers As New ArrayList
                                arrUsers = rolecontroller.GetUsersByRoleName(GetSetting("NUNTIO_PortalId", moduleId), role)
                                If Not arrUsers Is Nothing Then
                                    If arrUsers.Count > 0 Then

                                        Log("..." & arrUsers.Count.ToString & " users found")

                                        For Each user As UserInfo In arrUsers
                                            Log("...verifying " & user.DisplayName)
                                            If user.Membership.Approved Then
                                                Log("..." & user.DisplayName & " is approved")
                                                Dim objRoleSubscription As New SubscriptionInfo
                                                objRoleSubscription.Name = user.DisplayName
                                                objRoleSubscription.Email = user.Email
                                                objRoleSubscription.Key = "ROLESUBSCRIPTION"
                                                objRoleSubscription.Verified = True
                                                objRoleSubscription.ModuleId = mid
                                                objRoleSubscription.Locale = user.Profile.PreferredLocale
                                                If Not IsInList(subscriptions, objRoleSubscription.Email) Then
                                                    Log("..." & user.DisplayName & " added to list")
                                                    subscriptions.Add(objRoleSubscription)
                                                Else
                                                    Log("..." & user.DisplayName & " is already in subscription list")
                                                End If
                                            Else
                                                Log("..." & user.DisplayName & " is not approved yet")
                                            End If
                                        Next
                                    Else
                                        Log("...no users found")
                                    End If
                                End If
                            End If
                        Next

                    Else
                        Log("No role based subscriptions enabled for this module")
                    End If
                Catch ex As Exception
                    LogException(ex)
                End Try


                'now step through each subscription entity and 
                'load portalsettings for localization purposes
                'note that there is no context available here
                Try
                    If Not subscriptions Is Nothing And subscriptions.Count > 0 Then

                        For Each subscription As SubscriptionInfo In subscriptions

                            If subscription.Verified Then

                                Log("Collecting articles for subscriber " & subscription.ItemId.ToString & "(" & subscription.Email & ")")

                                'send notification

                                Dim sBody As String = ""
                                _Subject = ""

                                Dim iArticlesInMail As Integer = 0
                                'step though each newsitem to add it to the mail 
                                'body of the recipient
                                For Each newsitem As ArticleInfo In newsitems
                                    If newsitem.moduleId = moduleId Then

                                        Log("Appending article """ & newsitem.Title & """ (ID " & newsitem.itemId.ToString & ") to current e-mail body")

                                        'If pid <> newsitem.PortalID Then
                                        '    pid = newsitem.PortalID
                                        '    LoadPortalsettings(newsitem.PortalID)
                                        '    Log("Portalsettings for current module instance loaded")
                                        'End If

                                        'If sFrom = "" Then
                                        '    sFrom = Me._Portalsettings.Email
                                        '    Log("Sender hasn't been configured in the module, using portal e-mail address")
                                        'End If

                                        If String.IsNullOrEmpty(subscription.Locale) Then
                                            subscription.Locale = GetSetting("NUNTIO_DefaultLanguage", newsitem.moduleId)
                                            Log("Subscriber has not specified locale, using portal default locale")
                                        Else
                                            Log("Subscribers locale is " & subscription.Locale)
                                        End If

                                        Dim objItem As ArticleInfo = newscontroller.GetArticle(newsitem.itemId, subscription.Locale, blnUseOriginalVersion)

                                        If Not objItem Is Nothing Then
                                            Log("Article loaded for formatting")
                                            sBody = sBody & Me.MailBody(objItem, subscription)
                                            Log("Appended article content to current body")
                                            iArticlesInMail += 1
                                        Else
                                            Log("No article translation for subscribers locale found, Fallback is set to off in module")
                                        End If

                                    End If
                                Next

                                If sBody <> "" Then

                                    Log("E-Mail Body is loaded")
                                    Log("Loading module title...")

                                    'set default subject
                                    Try
                                        _Subject = GetSetting("NUNTIO_Portalname", moduleId) & ": " & GetSetting("NUNTIO_MODULETITLE_" & subscription.Locale.ToUpper, moduleId)
                                    Catch
                                    End Try
                                    Log("Moduletitle is " & _Subject)


                                    Dim sOpen As String = Me.GetMailSurround("HEADER", subscription)
                                    If sOpen.Length > 0 Then
                                        Log("E-Mail Header loaded")
                                    Else
                                        Log("E-Mail Header is empty")
                                    End If

                                    Dim sClose As String = Me.GetMailSurround("FOOTER", subscription)
                                    If sClose.Length > 0 Then
                                        Log("E-Mail footer is loaded")
                                    Else
                                        Log("E-Mail footer is empty")
                                    End If

                                    sBody = sOpen & sBody & sClose

                                    Log("E-Mail formatted")
                                    Log("Now adding e-mail to " & subscription.Email & ", containing " & iArticlesInMail.ToString & " article(s) into email queue")


                                    Dim queueentry As New EmailQueueInfo
                                    queueentry.AddedToQueue = Date.Now
                                    queueentry.DeliveryAttempts = 0
                                    queueentry.LastDeliveryAttempt = Null.NullDate
                                    queueentry.LastError = Null.NullString
                                    queueentry.Message = sBody
                                    queueentry.ModuleId = mid
                                    queueentry.PortalId = GetSetting("NUNTIO_PortalId", moduleId)
                                    queueentry.Recipient = subscription.Email
                                    queueentry.Sender = GetSetting("NUNTIO_Email", mid)
                                    queueentry.Subject = _Subject

                                    Dim ctl As New EmailQueueController
                                    ctl.Add(queueentry)

                                    Log("Message added to queue")

                                End If
                            End If
                        Next 'next subscription

                    Else
                        Log("No subscribers found")
                    End If
                Catch ex As Exception
                    LogException(ex)
                End Try

            Next

            Log("All subscribers processed, now removing all articles from the queue")
            ctrlSub.ClearNotificationQueue()
            Log(subscriptions.Count.ToString & " messages added to e-mail queue")

            Me.Status = subscriptions.Count.ToString & " messages added to e-mail queue"
            Me.ScheduleHistoryItem.Succeeded = True

        End Sub


#Region "Template Methods"

        Private Function GetTemplate(ByVal template As String, ByVal locale As String, ByVal ModuleId As Integer) As String
            Dim path As String = ""

            path = HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\Templates\Portal\" & GetSetting("NUNTIO_PortalId", ModuleId).ToString & "\Newsletter\" & template & "." & locale & ".txt"
            If System.IO.File.Exists(path) Then
                Dim sr As New StreamReader(path)
                Return sr.ReadToEnd
            End If
            path = HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\Templates\Portal\" & GetSetting("NUNTIO_PortalId", ModuleId).ToString & "\Newsletter\" & template & ".txt"
            If System.IO.File.Exists(path) Then
                Dim sr As New StreamReader(path)
                Return sr.ReadToEnd
            End If

            Try
                System.IO.Directory.CreateDirectory(HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\Templates\Portal\")
            Catch
            End Try
            Try
                System.IO.Directory.CreateDirectory(HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\Templates\Portal\" & GetSetting("NUNTIO_PortalId", ModuleId).ToString & "\")
            Catch
            End Try
            Try
                System.IO.Directory.CreateDirectory(HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\Templates\Portal\" & GetSetting("NUNTIO_PortalId", ModuleId).ToString & "\Newsletter\")
            Catch
            End Try

            Dim copyfrom As String = HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\Templates\Newsletter\" & template & ".txt"
            If System.IO.File.Exists(copyfrom) AndAlso System.IO.Directory.Exists(HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\Templates\Portal\" & GetSetting("NUNTIO_PortalId", ModuleId).ToString & "\Newsletter\") Then
                System.IO.File.Copy(copyfrom, path)
                If System.IO.File.Exists(path) Then
                    Dim sr As New StreamReader(path)
                    Return sr.ReadToEnd
                End If
            End If



            Return path
        End Function

        Private Function GetMailSurround(ByVal MessageType As String, ByVal objSubscription As SubscriptionInfo) As String

            Dim newsbase As New ArticleBase
            Dim msg As String = ""
            Dim sb As New StringBuilder
            Dim strContent As String = ""

            Select Case MessageType
                Case "HEADER"
                    msg = GetTemplate("Mailing_Header", objSubscription.Locale, objSubscription.ModuleId)
                Case "FOOTER"
                    msg = GetTemplate("Mailing_Footer", objSubscription.Locale, objSubscription.ModuleId)
            End Select

            Dim literal As New Literal
            Dim delimStr As String = "[]"
            Dim delimiter As Char() = delimStr.ToCharArray()

            Dim templateArray As String()
            templateArray = msg.Split(delimiter)

            For iPtr As Integer = 0 To templateArray.Length - 1 Step 2

                sb.Append(templateArray(iPtr).ToString())

                If iPtr < templateArray.Length - 1 Then

                    Select Case templateArray(iPtr + 1)

                        Case "RECIPIENT_NAME"

                            sb.Append(objSubscription.Name)

                        Case "RECIPIENT_EMAIL"

                            sb.Append(objSubscription.Email)

                        Case "PORTALNAME"

                            sb.Append(GetSetting("NUNTIO_Portalname", objSubscription.ModuleId))

                        Case "SUBJECT"

                            sb.Append(_Subject)

                        Case "NEWSMODULETITLE"

                            sb.Append(GetSetting("NUNTIO_MODULETITLE_" & objSubscription.Locale, objSubscription.ModuleId))

                        Case "ISROLESUBSCRIBED"

                            If objSubscription.Key <> "ROLESUBSCRIPTION" Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/ISROLESUBSCRIBED") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "ISSUBSCRIBED"

                            If objSubscription.Key = "ROLESUBSCRIPTION" Then
                                While (iPtr < templateArray.Length - 1)
                                    If (templateArray(iPtr + 1) = "/ISSUBSCRIBED") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "UNSUBSCRIBEURL"

                            sb.Append(GetUnsubscribeUrl(Null.NullInteger, objSubscription))

                        Case "ALLNEWSURL"

                            sb.Append(GetModuleUrl(Null.NullInteger, objSubscription))

                        Case Else

                            If (templateArray(iPtr + 1).ToUpper().StartsWith("SUBJECT:")) Then

                                _Subject = templateArray(iPtr + 1).Substring(8, templateArray(iPtr + 1).Length - 8)

                            End If

                            If (templateArray(iPtr + 1).ToUpper().StartsWith("UNSUBSCRIBEURL:")) Then

                                Dim tabid As String = templateArray(iPtr + 1).Substring(15, templateArray(iPtr + 1).Length - 15)
                                If IsNumeric(tabid) Then

                                    sb.Append(GetUnsubscribeUrl(Convert.ToInt32(tabid), objSubscription))

                                End If

                            End If

                    End Select

                End If

            Next

            strContent = sb.ToString
            strContent = ProcessFileLinks(strContent, objSubscription.ModuleId)

            Return strContent

        End Function

        Private Function MailBody(ByVal objItem As ArticleInfo, ByVal objSubscription As SubscriptionInfo) As String


            Dim objUser As New UserInfo
            Try
                objUser = UserController.GetUserById(Convert.ToInt32(GetSetting("NUNTIO_PortalId", objSubscription.ModuleId)), objItem.CreatedByUser)
            Catch
            End Try

            Dim newsbase As New ArticleBase

            Dim msg As String = GetTemplate("Mailing_Item", objSubscription.Locale, objItem.moduleId)

            msg = msg.Replace("[PUBLISHDATE]", objItem.PublishDate.ToShortDateString)
            Try
                msg = msg.Replace("[CREATEDBY]", objUser.DisplayName)
            Catch
                msg = msg.Replace("[CREATEDBY]", "(deleted)")
            End Try

            msg = msg.Replace("[SUMMARY]", HttpUtility.HtmlDecode(objItem.Summary))
            msg = msg.Replace("[CONTENT]", HttpUtility.HtmlDecode(objItem.Content))
            msg = msg.Replace("[TITLE]", objItem.Title)
            msg = msg.Replace("[NEWSLINK]", objItem.Url)
            msg = msg.Replace("[NEWSLINKURL]", objItem.Url)

            Dim ctrlImages As New ArticleFileController
            Dim imageUrl As String = "http://" & GetSetting("NUNTIO_PortalAlias", objSubscription.ModuleId) & "/images/spacer.gif"

            objItem.Images = ctrlImages.GetImages(objItem.ItemId, objSubscription.ModuleId, objSubscription.Locale, False)

            If Not objItem.Images Is Nothing Then
                For Each objImage As ArticleFileInfo In objItem.Images
                    If objImage.IsPrimary Then
                        imageUrl = "http://" & GetSetting("NUNTIO_PortalAlias", objSubscription.ModuleId) & imageUrl
                    End If
                Next
            End If

            msg = msg.Replace("[PRIMARYIMAGE]", "<img src=""" & imageUrl & """ alt=""" & objItem.Title & """ />")

            Dim tabid As Integer = Null.NullInteger
            Try
                tabid = GetSetting("NUNTIO_TabId", objSubscription.ModuleId)
            Catch
            End Try
            msg = msg.Replace("[DETAILLINKURL]", "http://" & GetSetting("NUNTIO_PortalAlias", objSubscription.ModuleId) & "/" & DotNetNuke.Common.glbDefaultPage & "?tabid=" & tabid.ToString & "&ArticleId=" & objItem.itemId.ToString)
            msg = msg.Replace("[MONTHNEWSURL]", "http://" & GetSetting("NUNTIO_PortalAlias", objSubscription.ModuleId) & "/" & DotNetNuke.Common.glbDefaultPage & "?tabid=" & tabid.ToString & "&month=" & Month(objItem.PublishDate).ToString & "&year=" & Year(objItem.PublishDate).ToString)
            msg = msg.Replace("[ALLNEWSURL]", "http://" & GetSetting("NUNTIO_PortalAlias", objSubscription.ModuleId) & "/" & DotNetNuke.Common.glbDefaultPage & "?tabid=" & tabid.ToString)
            msg = msg.Replace("[PORTALNAME]", GetSetting("NUNTIO_Portalname", objItem.moduleId))

            msg = ProcessFileLinks(msg, objItem.moduleId)

            Return msg

        End Function

#End Region

#Region "Private Helper Methods"

        Public Function GetSetting(ByVal SettingName As String, ByVal ModuleId As Integer) As String

            Dim settings As Hashtable
            settings = Me.ScheduleHistoryItem.GetSettings
            If Not settings Is Nothing Then
                Try
                    Return settings(SettingName & "_" & ModuleId.ToString)
                Catch
                End Try
            End If
            Return Null.NullString

        End Function

        Private Function IsInList(ByVal list As System.Collections.Generic.List(Of SubscriptionInfo), ByVal email As String) As Boolean
            For Each obj As SubscriptionInfo In list
                If obj.Email = email Then
                    Return True
                End If
            Next
            Return False
        End Function

        Private Function ProcessFileLinks(ByVal strHTML As String, ByVal ModuleId As Integer) As String

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
                If strWork.ToLower.IndexOf(GetSetting("NUNTIO_HomeDirectory", ModuleId).ToLower) <> -1 Then
                    'parse out portal home dir
                    Dim path As String = "http://" & GetSetting("NUNTIO_PortalAlias", ModuleId) & GetSetting("NUNTIO_HomeDirectory", ModuleId).ToLower
                    Dim intStart As Integer = strWork.ToLower.IndexOf(GetSetting("NUNTIO_HomeDirectory", ModuleId).ToLower)
                    intStart = intStart + GetSetting("NUNTIO_HomeDirectory", ModuleId).Length
                    strImage = strWork.Substring(intStart)
                    strImage = path & strImage
                Else
                    strImage = strWork
                End If

                Try
                    'replace our work in the html string
                    strHTML = strHTML.Replace(strWork, strImage)
                Catch ex As Exception
                    Log("Error replacing " & strWork & " with " & strImage & "Message: " & ex.Message)
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
                    Dim url As String = GetSetting("NUNTIO_PortalAlias", ModuleId) & "/" & strWork
                    strLink = "http://" & url.Replace("//", "/")
                Else
                    strLink = strWork
                End If

                Try
                    'replace our work in the html string
                    strHTML = strHTML.Replace(strWork, strLink)
                Catch ex As Exception
                    Log("Error replacing " & strWork & " with " & strLink & "Message: " & ex.Message)
                End Try


                'set start position for next search
                'posNext = strHTML.IndexOf(strImage)

                'set new start position for the next loop otherwise this will never end...
                posLink = strHTML.ToLower.IndexOf("<a ", posLink + 3)
                posLink = posLink

            End While

            Return strHTML

        End Function

        Private Function GetUnsubscribeUrl(ByVal TabId As Integer, ByVal objSubscription As SubscriptionInfo) As String

            Dim url As String = "http://" & GetSetting("NUNTIO_PortalAlias", objSubscription.ModuleId) & "/" & DotNetNuke.Common.glbDefaultPage & ""

            url += "?Action=Unsubscribe"
            url += "&SubscriptionKey=" & objSubscription.Key
            url += "&Email=" & objSubscription.Email
            url += "&NewsModuleId=" & objSubscription.ModuleId.ToString

            If TabId <> Null.NullInteger Then
                url += "&TabId=" & TabId.ToString
            Else
                url += "&TabId=" & GetSetting("NUNTIO_TabId", objSubscription.ModuleId)
            End If

            Return url

        End Function

        Private Function GetModuleUrl(ByVal TabId As Integer, ByVal objSubscription As SubscriptionInfo) As String

            Dim url As String = "http://" & GetSetting("NUNTIO_PortalAlias", objSubscription.ModuleId) & "/" & DotNetNuke.Common.glbDefaultPage & ""

            If TabId <> Null.NullInteger Then
                url += "?TabId=" & TabId.ToString
            Else
                url += "?TabId=" & GetSetting("NUNTIO_TabId", objSubscription.ModuleId)
            End If

            Return url

        End Function

#End Region

#Region "Debugging Methods"

        Private Sub VerifyDebugging()

            Dim blnDebuggingEnabled As Boolean = False

            Try
                Dim Path As String = HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\NotificationDebug.txt"
                blnDebuggingEnabled = System.IO.File.Exists(Path)
            Catch
            End Try

            If blnDebuggingEnabled Then

                Dim logpath As String = HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\logs"
                If Not System.IO.Directory.Exists(logpath) Then
                    System.IO.Directory.CreateDirectory(logpath)
                End If

                logpath += "\Debug_" & Date.Now.ToShortDateString.Replace("\", ".").Replace("/", ".") & "_" & Date.Now.Hour.ToString & "." & Date.Now.Minute.ToString & "." & Date.Now.Second.ToString & ".txt"
                If Not System.IO.File.Exists(logpath) Then

                    Dim log As New FileInfo(logpath)
                    If Not log.Exists Then
                        _writer = log.CreateText()
                    End If

                End If

                If Not _writer Is Nothing Then
                    Exit Sub
                End If

                Try
                    _writer = New System.IO.StreamWriter(logpath)
                Catch
                End Try

            End If

        End Sub

        Private Sub Log(ByVal strText As String)

            If Not _writer Is Nothing Then
                Try
                    _writer.WriteLine(Date.Now.ToShortTimeString & ":" & vbTab & strText)
                Catch
                End Try
            End If

        End Sub

        Private Sub FinalizeDebugging()

            If Not _writer Is Nothing Then
                _writer.Close()
                _writer.Dispose()
            End If

        End Sub

#End Region

    End Class


End Namespace

