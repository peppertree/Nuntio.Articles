Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions

Imports Telerik.Web.UI

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Subscribe
        Inherits ArticleModuleBase

        Private Enum UnsubscribeStatus
            AnonymousRequest
            AuthenticatedRequest
            EmailNotFound
            LoginRequired
            MailNotSent
        End Enum

        Private Enum SubscribeStatus
            AnonymousRequest
            AuthenticatedRequest
            LoginRequired
            MailNotSent
        End Enum

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            DotNetNuke.Framework.AJAX.RegisterScriptManager()
        End Sub

        Private Sub ProcessSubscriptionLink()

            pnlForm.Visible = False
            pnlResult.Visible = True

            'subscribe
            Dim blnFinalized As Boolean = False
            Dim strTitle As String = ""
            Dim strEmail As String = Server.UrlDecode(Request.QueryString("Email"))
            Dim strKey As String = Server.UrlDecode(Request.QueryString("SubscriptionKey"))
            Dim intModule As Integer = Null.NullInteger

            Try
                intModule = Integer.Parse(Server.UrlDecode(Request.QueryString("NewsModuleId")))
            Catch
            End Try


            If Not strEmail Is Nothing AndAlso Not strKey Is Nothing AndAlso Not intModule = Null.NullInteger Then

                Dim ctlSub As New SubscriptionController
                Dim oSub As SubscriptionInfo = ctlSub.GetSubscription(strKey, strEmail, intModule)

                If Not oSub Is Nothing Then

                    oSub.Verified = True
                    ctlSub.VerifySubscription(oSub)
                    blnFinalized = True

                    Dim mc As New Entities.Modules.ModuleController                   

                    Dim sh As Hashtable = mc.GetModuleSettings(intModule)
                    Dim sKey As String = "PNC_NEWS_MODULETITLE_" & Convert.ToString(CType(Page, PageBase).PageCulture.LCID)

                    Try
                        If Not sh(sKey) Is Nothing Then
                            strTitle = CType(sh(sKey), String)
                        End If
                    Catch
                    End Try

                End If

            End If            

            If blnFinalized Then
                lblResult.Text = String.Format(Localization.GetString("SubscriptionFinalized", Me.LocalResourceFile), strTitle)
            Else
                lblResult.Text = Localization.GetString("SubscriptionFinalized.Error", Me.LocalResourceFile)
            End If

        End Sub

        Private Sub ProcessUnsubscribeLink()

            pnlForm.Visible = False
            pnlResult.Visible = True

            'subscribe

            Dim blnFinalized As Boolean = False
            Dim strTitle As String = ""
            Dim strEmail As String = Server.UrlDecode(Request.QueryString("Email"))
            Dim strKey As String = Server.UrlDecode(Request.QueryString("SubscriptionKey"))
            Dim intModule As Integer = Null.NullInteger

            Try
                intModule = Integer.Parse(Server.UrlDecode(Request.QueryString("NewsModuleId")))
            Catch
            End Try

            If Not strEmail Is Nothing AndAlso Not strKey Is Nothing AndAlso Not intModule = Null.NullInteger Then

                Dim ctlSub As New SubscriptionController
                Dim oSub As SubscriptionInfo = ctlSub.GetSubscription(strKey, strEmail, intModule)

                If Not oSub Is Nothing Then

                    oSub.Verified = True
                    ctlSub.UnSubscribe(oSub)
                    blnFinalized = True

                    Dim mc As New Entities.Modules.ModuleController

                    Dim sh As Hashtable = mc.GetModuleSettings(intModule)
                    Dim sKey As String = "PNC_NEWS_MODULETITLE_" & Convert.ToString(CType(Page, PageBase).PageCulture.LCID)

                    Try
                        If Not sh(sKey) Is Nothing Then
                            strTitle = CType(sh(sKey), String)
                        End If
                    Catch
                    End Try

                End If

            End If

            If blnFinalized Then
                lblResult.Text = String.Format(Localization.GetString("UnSubscriptionFinalized", Me.LocalResourceFile), strTitle)
            Else
                lblResult.Text = Localization.GetString("UnSubscriptionFinalized.Error", Me.LocalResourceFile)
            End If

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                LocalizeForm()

                If Not Request.QueryString("Action") Is Nothing Then

                    If Not Request.QueryString("ModuleId") Is Nothing Then
                        If Integer.Parse(Request.QueryString("ModuleId")) = ModuleId Then

                            'we are in charge here, as the called moduleid is ours

                            If Request.QueryString("Action").ToLower = "subscribe" Then
                                ProcessSubscriptionLink()
                            End If

                            If Request.QueryString("Action").ToLower = "unsubscribe" Then
                                ProcessUnsubscribeLink()
                            End If

                        End If
                    End If
                   

                Else

                    If Not Page.IsPostBack Then
                        BindLocales()
                        BindForm()
                        BindModules()
                    End If

                    'the bloody acer touchpad jums for and backwards as it likes it. Oh man.

                End If

                

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Private Methods"

        Private Sub BindModules()

            Me.chkModules.Items.Clear()
            If Settings.Contains("NewsmoduleIdList") Then
                Dim Modules As String() = CType(Me.Settings("NewsmoduleIdList"), String).Split(Char.Parse(","))
                For Each sModule As String In Modules

                    If sModule <> "" Then
                        Dim mc As New Entities.Modules.ModuleController
                        Dim m As Entities.Modules.ModuleInfo
                        m = mc.GetModule(Integer.Parse(sModule), Null.NullInteger)

                        Dim sh As Hashtable = mc.GetModuleSettings(Integer.Parse(sModule))
                        Dim sKey As String = "PNC_NEWS_MODULETITLE_" & Convert.ToString(CType(Page, PageBase).PageCulture.LCID)

                        Try
                            If Not sh(sKey) Is Nothing Then
                                m.ModuleTitle = CType(sh(sKey), String)
                            End If
                        Catch
                        End Try

                        Dim blnIsSubscriberModule As Boolean = False

                        Dim _modulesettings As New Hashtable
                        _modulesettings = mc.GetModuleSettings(Integer.Parse(sModule))
                        Dim nItem As New ListItem
                        nItem.Text = m.ModuleTitle
                        nItem.Value = sModule
                        If _modulesettings.Contains("AllowSubscriptions") Then
                            If CType(_modulesettings("AllowSubscriptions"), Boolean) = True Then

                                blnIsSubscriberModule = True

                                If _modulesettings.Contains("OnlyRegisteredUsersCanSubscribe") Then
                                    If CType(_modulesettings("OnlyRegisteredUsersCanSubscribe"), Boolean) = True Then

                                        If Request.IsAuthenticated = False Then
                                            nItem.Enabled = False
                                            nItem.Text = m.ModuleTitle & " " & Localize("onlyregisteredusers")
                                        Else
                                            nItem.Enabled = True
                                            nItem.Text = m.ModuleTitle
                                        End If

                                    End If
                                End If

                                If Request.IsAuthenticated Then

                                    Dim blnIsAutosubscribed As Boolean = False
                                    If _modulesettings.Contains("SubscribedRoles") Then
                                        'check if visitor is in of the subscribed roles
                                        Dim roles As String() = CType(_modulesettings("SubscribedRoles"), String).Split(Char.Parse(","))
                                        If Not roles Is Nothing And roles.Length > 0 Then
                                            For Each role As String In roles
                                                If UserInfo.IsInRole(role) Then
                                                    blnIsAutosubscribed = True
                                                    nItem.Selected = True
                                                    nItem.Enabled = True
                                                    nItem.Text = m.ModuleTitle
                                                End If
                                            Next
                                        End If
                                    End If
                                    If Not blnIsAutosubscribed Then
                                        If UserIsSubscribed(UserInfo, Integer.Parse(sModule)) Then
                                            nItem.Selected = True
                                            nItem.Enabled = True
                                            nItem.Text = m.ModuleTitle
                                        End If
                                    End If

                                End If

                            End If
                        End If

                        If blnIsSubscriberModule Then
                            Me.chkModules.Items.Add(nItem)
                        End If

                    End If
                Next
            End If

            If Me.chkModules.Items.Count = 1 Then

                chkModules.Items(0).Selected = True
                Me.rowModules.Visible = False

                If UserIsSubscribed(UserInfo, Integer.Parse(chkModules.Items(0).Value)) Then
                    lblHelp.Text = String.Format(Localization.GetString("lblSingleSelect_Subscribed", Me.LocalResourceFile), chkModules.Items(0).Text)
                    btnSubscribe.Enabled = False
                Else
                    If Request.IsAuthenticated Then
                        lblHelp.Text = String.Format(Localization.GetString("lblSingleSelect_Auth", Me.LocalResourceFile), chkModules.Items(0).Text)
                    Else
                        lblHelp.Text = String.Format(Localization.GetString("lblSingleSelect", Me.LocalResourceFile), chkModules.Items(0).Text)
                    End If

                    btnSubscribe.Enabled = True
                End If


            ElseIf Me.chkModules.Items.Count > 1 Then

                Me.rowModules.Visible = True
                lblHelp.Text = Localization.GetString("lblMultipleSelect", Me.LocalResourceFile)

            End If

        End Sub

        Private Function UserIsSubscribed(ByVal User As UserInfo, ByVal NewsModule As Integer) As Boolean

            If Request.IsAuthenticated = False Then
                Return False
            End If

            If UserInfo Is Nothing Then
                Return False
            End If

            If String.IsNullOrEmpty(UserInfo.Email) Then
                Return False
            End If

            Dim objSubscribe As SubscriptionInfo
            Dim SubscriptionController As New SubscriptionController
            'let's assume that subscription is allowed
            Dim IsMigrated As Boolean = False

            objSubscribe = SubscriptionController.GetSubscription(NewsModule, UserInfo.Email, Null.NullInteger)
            If objSubscribe Is Nothing Then
                'no subscription active
                'check wether subscription exists with provided user account
                objSubscribe = SubscriptionController.GetSubscription(NewsModule, Null.NullString, UserInfo.UserID)
                If objSubscribe Is Nothing Then
                    Return False
                End If
            Else
                'user has subscribed before adding a user accout
                'we delete the existing subscription and add a new one
                'with the user account
                SubscriptionController.UnSubscribe(objSubscribe)
                objSubscribe = New SubscriptionInfo(UserInfo.UserID, NewsModule)
                SubscriptionController.Subscribe(objSubscribe)
                Return True
            End If
            Return True
        End Function

        Private Sub BindLocales()
            Try
                If SupportedLocales.Count > 1 Then

                    For Each objLocale As Locale In SupportedLocales.Values
                        Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
                        Me.drpLanguage.Items.Add(New RadComboBoxItem(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.DisplayName), System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name).ToUpper))
                    Next

                    Try
                        Me.drpLanguage.Items.FindItemByValue(CType(Page, PageBase).PageCulture.Name.ToUpper).Selected = True
                    Catch
                    End Try

                Else

                    Me.rowLanguage.Visible = False

                End If
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Sub LocalizeForm()
            lblHelp.Text = Localization.GetString("lblHelp", Me.LocalResourceFile)
            lblEmail.Text = Localization.GetString("lblEmail", Me.LocalResourceFile)
            lblLanguage.Text = Localization.GetString("lblLanguage", Me.LocalResourceFile)
            lblName.Text = Localization.GetString("lblName", Me.LocalResourceFile)
            btnSubscribe.Text = Localization.GetString("btnSubscribe", Me.LocalResourceFile)
            btnUnSubscribe.Text = Localization.GetString("btnUnSubscribe", Me.LocalResourceFile)
        End Sub

#End Region

        Private Sub BindForm()

            If Request.IsAuthenticated Then
                txtEmail.Text = UserInfo.Email
                txtEmail.Enabled = False
                txtName.Text = UserInfo.DisplayName
                txtName.Enabled = False

                If SupportedLocales.Count > 1 Then

                    If Not UserInfo.Profile.PreferredLocale Is Nothing Then
                        Try
                            Me.drpLanguage.Items.FindItemByValue(UserInfo.Profile.PreferredLocale.ToUpper).Selected = True
                        Catch ex As Exception
                            UserInfo.Profile.SetProfileProperty("PreferredLocale", CType(Page, PageBase).PageCulture.Name)
                            Me.drpLanguage.Items.FindItemByValue(CType(Page, PageBase).PageCulture.Name.ToUpper).Selected = True
                        End Try
                    Else
                        UserInfo.Profile.SetProfileProperty("PreferredLocale", CType(Page, PageBase).PageCulture.Name)
                        Me.drpLanguage.Items.FindItemByValue(CType(Page, PageBase).PageCulture.Name.ToUpper).Selected = True
                    End If

                    drpLanguage.Enabled = False

                End If
                
            End If
        End Sub

        Private Function IsNotificationEnabled(ByVal MID As Integer) As Boolean
            Dim objModuleController As New DotNetNuke.Entities.Modules.ModuleController
            Dim modsettings As Hashtable
            modsettings = objModuleController.GetModuleSettings(MID)
            If Not modsettings Is Nothing Then
                If modsettings.Contains("NotifyAdmin") Then
                    Return CType(modsettings("NotifyAdmin"), Boolean)
                End If
            End If
            Return False
        End Function

        Private Function Unsubscribe(ByRef Status As UnsubscribeStatus) As Boolean

            Dim objSubscribe As SubscriptionInfo = Nothing
            Dim SubscriptionController As New SubscriptionController
            Dim total As Integer = 0
            Dim arrUsers As ArrayList = Nothing
            Dim unsubscriptions As New List(Of SubscriptionInfo)

            If Request.IsAuthenticated = False Then
                arrUsers = UserController.GetUsersByEmail(PortalId, txtEmail.Text, 0, 10, total)
                If total > 0 Then
                    'user found 
                    Status = UnsubscribeStatus.LoginRequired
                    Return False
                End If
            End If

            For Each item As ListItem In Me.chkModules.Items
                If (item.Selected) Then

                    'unsubscribe from news
                    If Request.IsAuthenticated Then

                        Status = UnsubscribeStatus.AuthenticatedRequest

                        If Not Me.IsRoleSubscribed(True, Integer.Parse(item.Value)) Then
                            objSubscribe = SubscriptionController.GetSubscription(Integer.Parse(item.Value), Null.NullString, UserId)
                            If Not objSubscribe Is Nothing Then
                                SubscriptionController.UnSubscribe(objSubscribe)
                            End If
                        End If

                    Else

                        Status = UnsubscribeStatus.AnonymousRequest

                        objSubscribe = SubscriptionController.GetSubscription(Integer.Parse(item.Value), Me.txtEmail.Text, Null.NullInteger)
                        If Not objSubscribe Is Nothing Then
                            objSubscribe.Name = txtName.Text
                            objSubscribe.ModuleTitle = item.Text
                            unsubscriptions.Add(objSubscribe)
                        End If

                    End If

                End If
            Next

            If Status = UnsubscribeStatus.AnonymousRequest Then

                If SendVerification(unsubscriptions, False) Then

                    Status = UnsubscribeStatus.AnonymousRequest
                    Return True

                Else
                    Status = UnsubscribeStatus.MailNotSent
                    Return False

                End If

            Else

                Status = UnsubscribeStatus.AuthenticatedRequest
                Return True

            End If


        End Function

        Private Function Subscribe(ByRef Status As SubscribeStatus) As Boolean

            Dim objSubscribe As SubscriptionInfo = Nothing
            Dim SubscriptionController As New SubscriptionController
            Dim total As Integer = 0
            Dim arrUsers As ArrayList = Nothing
            Dim CanSubscribe As Boolean = True
            Dim IsMigrated As Boolean = False

            If Request.IsAuthenticated = False Then
                arrUsers = UserController.GetUsersByEmail(PortalId, txtEmail.Text, 0, 10, total)
                If total > 0 Then
                    'user found 
                    Status = SubscribeStatus.LoginRequired
                    Return False
                End If
            End If

            Dim subscriptions As New List(Of SubscriptionInfo)


            For Each item As ListItem In Me.chkModules.Items
                If (item.Selected) Then

                    'subscribe to news modules
                    If Request.IsAuthenticated Then

                        Status = SubscribeStatus.AuthenticatedRequest

                        If Not Me.IsRoleSubscribed(True, Integer.Parse(item.Value)) Then
                            'check for existing subscriptions with e-mail address of account
                            objSubscribe = SubscriptionController.GetSubscription(Integer.Parse(item.Value), UserInfo.Email, Null.NullInteger)
                            If objSubscribe Is Nothing Then
                                'check for existing subscriptions with userid address of account
                                objSubscribe = SubscriptionController.GetSubscription(Integer.Parse(item.Value), Null.NullString, UserInfo.UserID)
                                If objSubscribe Is Nothing Then
                                    'not subscribey yet
                                    objSubscribe = New SubscriptionInfo(UserInfo.UserID, Integer.Parse(item.Value))
                                    SubscriptionController.Subscribe(objSubscribe)
                                    If IsNotificationEnabled(Integer.Parse(item.Value)) Then
                                        Me.NotfiyWebmaster(UserInfo.Email, UserInfo.Profile.PreferredLocale, True, UserInfo.Username, UserInfo.DisplayName)
                                    End If
                                End If
                            Else
                                'user has subscribed before adding a user accout
                                'we delete the existing subscription and add a new one
                                'with the user account
                                SubscriptionController.UnSubscribe(objSubscribe)
                                objSubscribe = New SubscriptionInfo(UserInfo.UserID, Integer.Parse(item.Value))
                                SubscriptionController.Subscribe(objSubscribe)
                                If IsNotificationEnabled(Integer.Parse(item.Value)) Then
                                    Me.NotfiyWebmaster(UserInfo.Email, UserInfo.Profile.PreferredLocale, True, UserInfo.Username, UserInfo.DisplayName)
                                End If
                            End If
                        End If

                    Else

                        'anonymous mode
                        Status = SubscribeStatus.AnonymousRequest

                        Dim locale As String = PortalSettings.DefaultLanguage
                        If SupportedLocales.Count > 1 Then
                            locale = drpLanguage.SelectedValue
                        End If

                        objSubscribe = SubscriptionController.GetSubscription(Integer.Parse(item.Value), txtEmail.Text, Null.NullInteger)
                        If objSubscribe Is Nothing Then

                            'new subscription

                            objSubscribe = New SubscriptionInfo(txtEmail.Text, txtName.Text, locale, Integer.Parse(item.Value))
                            objSubscribe.Key = SubscriptionController.Subscribe(objSubscribe)

                            If IsNotificationEnabled(Integer.Parse(item.Value)) Then
                                Me.NotfiyWebmaster(txtEmail.Text, locale, False, Nothing, txtName.Text)
                            End If

                            objSubscribe.ModuleTitle = item.Text
                            subscriptions.Add(objSubscribe)

                        Else

                            'gibts schon
                            objSubscribe.Name = txtName.Text
                            objSubscribe.ModuleTitle = item.Text
                            objSubscribe.Locale = locale
                            subscriptions.Add(objSubscribe)

                        End If

                    End If

                End If

            Next

            If Status = SubscribeStatus.AnonymousRequest Then

                If SendVerification(subscriptions, True) Then

                    Status = SubscribeStatus.AnonymousRequest
                    Return True

                Else

                    Status = SubscribeStatus.MailNotSent
                    Return False

                End If

            Else

                Status = SubscribeStatus.AuthenticatedRequest
                Return True

            End If
            
        End Function

        Protected Function SendVerification(ByVal objSubscriptions As List(Of SubscriptionInfo), ByVal Subscribe As Boolean) As Boolean

            Dim strResponse As String = ""

            For Each oSub As SubscriptionInfo In objSubscriptions

                Dim strSubscriberLink As String = Null.NullString

                If Subscribe Then
                    strSubscriberLink = NavigateURL(TabId, "", "ModuleId=" & ModuleId, "Action=Subscribe", "SubscriptionKey=" & Server.UrlEncode(oSub.Key), "Email=" & Server.UrlEncode(oSub.Email), "NewsModuleId=" & oSub.ModuleId.ToString)
                Else
                    strSubscriberLink = NavigateURL(TabId, "", "ModuleId=" & ModuleId, "Action=UnSubscribe", "SubscriptionKey=" & Server.UrlEncode(oSub.Key), "Email=" & Server.UrlEncode(oSub.Email), "NewsModuleId=" & oSub.ModuleId.ToString)
                End If

                If Not strSubscriberLink.StartsWith("http://") Then
                    strSubscriberLink = "/" & strSubscriberLink
                    strSubscriberLink = PortalSettings.PortalAlias.HTTPAlias & strSubscriberLink
                    strSubscriberLink = strSubscriberLink.Replace("//", "/")
                    strSubscriberLink = "http://" & strSubscriberLink
                End If

                Dim template As String = ""
                Dim body As String = ""
                Dim sb As New StringBuilder

                If Subscribe Then
                    'subscription mode
                    template = SubscribeTemplate
                Else
                    'unsubscription mode
                    template = UnsubscribeTemplate
                End If

                Dim literal As New Literal
                Dim delimStr As String = "[]"
                Dim delimiter As Char() = delimStr.ToCharArray()

                Dim templateArray As String()
                templateArray = template.Split(delimiter)

                For iPtr As Integer = 0 To templateArray.Length - 1 Step 2
                    sb.Append(templateArray(iPtr).ToString())
                    If iPtr < templateArray.Length - 1 Then

                        Select Case templateArray(iPtr + 1)
                            Case "SUBSCRIPTIONMODULE"
                                sb.Append(oSub.ModuleTitle)
                            Case "SUBSCRIBERNAME"
                                sb.Append(oSub.Name)
                            Case "PORTALNAME"
                                sb.Append(PortalSettings.PortalName)
                            Case "SUBSCRIBELINK"
                                sb.Append(strSubscriberLink)
                            Case "UNSUBSCRIBELINK"
                                sb.Append(strSubscriberLink)
                        End Select

                    End If
                Next

                Dim strSubject As String = String.Format(Localization.GetString("SubscriptionStatusChange", Me.LocalResourceFile), PortalSettings.PortalName)
                Dim strBody As String = sb.ToString
                Dim strRecipient As String = oSub.Email
                Dim strSender As String = PortalSettings.Email

                strResponse = DotNetNuke.Services.Mail.Mail.SendMail(strSender, strRecipient, "", strSubject, strBody, "", "HTML", "", "", "", "")

            Next

            If strResponse = "" Then
                Return True
            Else
                Return False
            End If

        End Function

        Private Sub btnSubscribe_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubscribe.Click

            Dim blnIsValid As Boolean = True
            Dim blnIsValidEmail As Boolean = True
            Dim blnIsValidName As Boolean = True

            If txtEmail.Text.Length = 0 Then
                'must fill in email
                blnIsValid = False
                blnIsValidEmail = False
            Else

                If Not Regex.IsMatch(txtEmail.Text.Trim, "\b[a-zA-Z0-9._%\-+']+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,4}\b", RegexOptions.IgnoreCase) Then
                    'must fill in valid email
                    blnIsValid = False
                    blnIsValidEmail = False
                End If

            End If

            If txtName.Text.Length = 0 Then
                'must fill in name
                blnIsValid = False
                blnIsValidName = False
            End If

            If blnIsValid Then

                pnlForm.Visible = False
                pnlResult.Visible = True

                Dim status As SubscribeStatus

                If Subscribe(status) Then

                    Select Case status
                        Case SubscribeStatus.AnonymousRequest
                            lblResult.Text = Localization.GetString("Success_Anonymous", Me.LocalResourceFile)
                        Case SubscribeStatus.AuthenticatedRequest
                            lblResult.Text = Localization.GetString("Success_Authenticated", Me.LocalResourceFile)
                        Case Else
                            lblResult.Text = Localization.GetString("lblError", Me.LocalResourceFile)
                    End Select

                Else

                    Select Case status
                        Case SubscribeStatus.LoginRequired
                            'ask for login
                            lblResult.Text = String.Format(Localization.GetString("Error_LoginRequired", Me.LocalResourceFile), LoginUrl)

                        Case Else
                            'generic error
                            lblResult.Text = Localization.GetString("lblError", Me.LocalResourceFile)

                    End Select

                End If

            Else

                pnlForm.Visible = True
                pnlResult.Visible = False

                Dim strError As New LiteralControl
                strError.Text = "<ul>"

                If blnIsValidEmail = False Then
                    strError.Text += "<li>" & Localization.GetString("InvalidEmailAddress.Text", Me.LocalResourceFile) & "</li>"
                End If

                If blnIsValidName = False Then
                    strError.Text += "<li>" & Localization.GetString("InvalidEmailName.Text", Me.LocalResourceFile) & "</li>"
                End If

                strError.Text += "</ul>"

                BindLocales()
                pnlForm.Controls.AddAt(0, strError)

            End If

            

        End Sub

        Private Sub btnUnSubscribe_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUnSubscribe.Click


            Dim blnIsValidEmail As Boolean = (txtEmail.Text.Length > 0)


            If blnIsValidEmail Then


                pnlForm.Visible = False
                pnlResult.Visible = True

                Dim status As UnsubscribeStatus

                If Unsubscribe(status) Then

                    Select Case status
                        Case SubscribeStatus.AnonymousRequest
                            lblResult.Text = Localization.GetString("Success_Anonymous", Me.LocalResourceFile)
                        Case SubscribeStatus.AuthenticatedRequest
                            lblResult.Text = Localization.GetString("Success_Unsubscribe", Me.LocalResourceFile)
                        Case Else
                            lblResult.Text = Localization.GetString("lblError", Me.LocalResourceFile)
                    End Select

                Else
                    Select Case status
                        Case UnsubscribeStatus.EmailNotFound
                            lblResult.Text = Localization.GetString("Error_NoSuchEmail", Me.LocalResourceFile)
                        Case UnsubscribeStatus.LoginRequired
                            lblResult.Text = String.Format(Localization.GetString("Error_LoginRequired", Me.LocalResourceFile), LoginUrl)
                        Case Else
                            'generic error
                            lblResult.Text = Localization.GetString("lblError", Me.LocalResourceFile)

                    End Select

                End If

            Else

                pnlForm.Visible = True
                pnlResult.Visible = False

                Dim strError As New LiteralControl
                strError.Text = "<ul>"
                strError.Text += "<li>" & Localization.GetString("InvalidEmailAddress.Text", Me.LocalResourceFile) & "</li>"
                strError.Text += "</ul>"

                pnlForm.Controls.AddAt(0, strError)

            End If

        End Sub

        Private Function LoginUrl() As String
            If PortalSettings.LoginTabId <> Null.NullInteger Then
                Return NavigateURL(PortalSettings.LoginTabId, "", "ReturnUrl=" & Request.RawUrl)
            Else
                Return NavigateURL(TabId, "", "ctl=Login", "ReturnUrl=" & Request.RawUrl)
            End If
        End Function

    End Class
End Namespace
