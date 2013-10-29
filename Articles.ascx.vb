
Imports Telerik.Web.UI

Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions



Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class Articles
        Inherits ArticleModuleBase
        Implements IActionable


#Region "Private Members"

        Private m_controlToLoad As String = ""
        Private blnIsHost As Boolean = False
        Private blnIsAdmin As Boolean = False
        Private blnCanEdit As Boolean = False
        Private blnCanAdd As Boolean = False

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            'Me.ModuleConfiguration.ModuleTitle = Localize("ControlTitle_View")
            Dim sTitle As String = Null.NullString
            Try
                sTitle = CType(Settings("PNC_NEWS_MODULETITLE_" & Convert.ToString(CType(Page, PageBase).PageCulture.LCID)), String)
            Catch
            End Try
            If sTitle <> Null.NullString Then
                If sTitle.Length > 0 Then
                    Me.ModuleConfiguration.ModuleTitle = sTitle
                End If
            End If

            Dim permController As New ModulePermissionController


            If Request.IsAuthenticated Then

                blnIsHost = UserInfo.IsSuperUser
                blnIsAdmin = UserInfo.IsInRole(PortalSettings.AdministratorRoleName)
                blnCanEdit = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration)
                blnCanAdd = CanPublish

                Dim ctrl As New ModuleController
                Dim objModule As ModuleInfo = ctrl.GetModule(NewsModuleId)
                blnCanEdit = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", objModule)

            End If

            Dim blnIsUnsubscribeRequest As Boolean = False

            'is action in querystring?
            If Not Request.QueryString("Action") Is Nothing Then

                'is action unsubscribe?
                If Request.QueryString("Action").ToLower = "unsubscribe" Then

                    'is news moduleid in querystring?
                    If Not Request.QueryString("NewsModuleId") Is Nothing Then

                        'is moduleid numeric?
                        If IsNumeric(Request.QueryString("NewsModuleId")) Then

                            'is newsmoduleid in querystring the same than this module?
                            If (Convert.ToInt32(Request.QueryString("NewsModuleId")) = ModuleId) Then

                                'key in querystring?
                                If Not Request.QueryString("SubscriptionKey") Is Nothing Then

                                    'email in querystring?
                                    If Not Request.QueryString("Email") Is Nothing Then

                                        'is unsubscribe request
                                        blnIsUnsubscribeRequest = True

                                    End If

                                End If
                            End If
                        End If
                    End If
                End If
            End If

            If blnIsUnsubscribeRequest Then
                '
                ProcessUnsubscribeLink()
                '
            Else
                '
                ' load client scripts
                '
                LoadScripts()
                '
                ' Assign parameters
                '
                ReadQueryString()
                '
                ' Load appropriate type
                '
                LoadControlType()
                '' 
            End If



        End Sub

#End Region

#Region "Private Methods"

        Private Function IsModule() As Boolean
            If Not Request.QueryString("mid") Is Nothing Then
                If Integer.Parse(Request.QueryString("mid")) = ModuleId Then
                    Return True
                End If
            End If
            Return False
        End Function

        Private Sub ReadQueryString()

            Try
                Dim sTitle As String = Null.NullString
                Try
                    sTitle = CType(Settings("PNC_LINKS_MODULETITLE_" & Convert.ToString(CType(Page, PageBase).PageCulture.LCID)), String)
                Catch
                End Try

                If sTitle <> Null.NullString Then
                    If sTitle.Length > 0 Then
                        Me.ModuleConfiguration.ModuleTitle = sTitle
                    End If
                End If
            Catch
            End Try

            If Not (Request("act") Is Nothing) Then
                Select Case Request("act").ToLower

                    Case "managesubscriptions"

                        m_controlToLoad = Me.ModuleDirectory & "/controls/uc_ListSubscriptions.ascx"
                        Me.ModuleConfiguration.ModuleTitle = Localize("ControlTitle_MANAGESUBSCRIPTIONS")

                    Case "managesettings"

                        m_controlToLoad = Me.ModuleDirectory & "/Settings.ascx"
                        Me.ModuleConfiguration.ModuleTitle = Localize("ControlTitle_CONTROLPANEL")

                    Case "thankyou"

                        m_controlToLoad = Me.ModuleDirectory & "/controls/uc_ThankYou.ascx"
                        Me.ModuleConfiguration.ModuleTitle = Localize("ControlTitle_THANKYOU")

                    Case Else


                End Select

            End If

            If m_controlToLoad = "" Then

                'load control based on module settings

                Select Case ModuleView.ToLower
                    Case "articlepanel"
                        m_controlToLoad = Me.ModuleDirectory & "/Modules/ArticlePanel/uc_Panel.ascx"
                    Case "articlelist"
                        m_controlToLoad = Me.ModuleDirectory & "/Modules/ArticleList/uc_List.ascx"
                    Case "threadedlist"
                        m_controlToLoad = Me.ModuleDirectory & "/Modules/ThreadedList/uc_List.ascx"
                    Case "articlerotator"
                        m_controlToLoad = Me.ModuleDirectory & "/Modules/ArticleRotator/uc_Rotate.ascx"
                    Case "rssbrowser"
                        m_controlToLoad = Me.ModuleDirectory & "/Modules/RssBrowser/uc_RssBrowser.ascx"
                    Case "categorybrowser"
                        m_controlToLoad = Me.ModuleDirectory & "/Modules/CategoryBrowser/uc_CategoryBrowser.ascx"
                    Case "subscribeform"
                        m_controlToLoad = Me.ModuleDirectory & "/Modules/SubscriberForm/uc_Subscribe.ascx"
                    Case "archivebrowser"
                        m_controlToLoad = Me.ModuleDirectory & "/Modules/ArchiveBrowser/uc_Archive.ascx"
                    Case "bannerviewer"
                        m_controlToLoad = Me.ModuleDirectory & "/Modules/BannerViewer/uc_Banner.ascx"
                    Case "authorbrowser"
                        m_controlToLoad = Me.ModuleDirectory & "/Modules/AuthorBrowser/uc_AuthorBrowser.ascx"
                    Case Else
                        m_controlToLoad = Me.ModuleDirectory & "/Controls/uc_Configure.ascx"
                End Select

            End If

        End Sub

        Private Sub LoadControlType()

            Dim objPortalModuleBase As ArticleBase = CType(Me.LoadControl(m_controlToLoad), ArticleBase)
            objPortalModuleBase.ModuleConfiguration = Me.ModuleConfiguration
            objPortalModuleBase.ID = System.IO.Path.GetFileNameWithoutExtension(m_controlToLoad)
            ' Load the appropriate control
            '
            plhNewsControls.Controls.Add(objPortalModuleBase)

        End Sub

        Private Sub ProcessUnsubscribeLink()

            'unsubscribe

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

            Dim lblResult As New LiteralControl

            lblResult.Text = "<p class=""Normal"">"
            If blnFinalized Then
                lblResult.Text += String.Format(Localization.GetString("UnSubscriptionFinalized", Me.LocalResourceFile), strTitle)
            Else
                lblResult.Text += Localization.GetString("UnSubscriptionFinalized.Error", Me.LocalResourceFile)
            End If
            lblResult.Text += "</p>"

            plhNewsControls.Controls.Clear()
            plhNewsControls.Controls.Add(lblResult)

        End Sub

#End Region

#Region "Optional Interfaces"

        Private Sub VerifyActionIcons()

            Try

                Dim targetpath As String = Server.MapPath(ResolveUrl("~/images/nuntio_templates.png"))
                If Not System.IO.File.Exists(targetpath) Then
                    Dim orgpath As String = Server.MapPath(Me.ModuleDirectory & "/images/nuntio_templates.png")
                    System.IO.File.Copy(orgpath, targetpath, True)
                End If

                targetpath = Server.MapPath(ResolveUrl("~/images/nuntio_add.png"))
                If Not System.IO.File.Exists(targetpath) Then
                    Dim orgpath As String = Server.MapPath(Me.ModuleDirectory & "/images/nuntio_add.png")
                    System.IO.File.Copy(orgpath, targetpath, True)
                End If

                targetpath = Server.MapPath(ResolveUrl("~/images/nuntio_unapproved.png"))
                If Not System.IO.File.Exists(targetpath) Then
                    Dim orgpath As String = Server.MapPath(Me.ModuleDirectory & "/images/nuntio_unapproved.png")
                    System.IO.File.Copy(orgpath, targetpath, True)
                End If

                targetpath = Server.MapPath(ResolveUrl("~/images/nuntio_calendar.png"))
                If Not System.IO.File.Exists(targetpath) Then
                    Dim orgpath As String = Server.MapPath(Me.ModuleDirectory & "/images/nuntio_calendar.png")
                    System.IO.File.Copy(orgpath, targetpath, True)
                End If

                targetpath = Server.MapPath(ResolveUrl("~/images/nuntio_clock.png"))
                If Not System.IO.File.Exists(targetpath) Then
                    Dim orgpath As String = Server.MapPath(Me.ModuleDirectory & "/images/nuntio_clock.png")
                    System.IO.File.Copy(orgpath, targetpath, True)
                End If

                targetpath = Server.MapPath(ResolveUrl("~/images/nuntio_remove.png"))
                If Not System.IO.File.Exists(targetpath) Then
                    Dim orgpath As String = Server.MapPath(Me.ModuleDirectory & "/images/nuntio_remove.png")
                    System.IO.File.Copy(orgpath, targetpath, True)
                End If

                targetpath = Server.MapPath(ResolveUrl("~/images/nuntio_back.png"))
                If Not System.IO.File.Exists(targetpath) Then
                    Dim orgpath As String = Server.MapPath(Me.ModuleDirectory & "/images/nuntio_back.png")
                    System.IO.File.Copy(orgpath, targetpath, True)
                End If

                targetpath = Server.MapPath(ResolveUrl("~/images/nuntio_bin.png"))
                If Not System.IO.File.Exists(targetpath) Then
                    Dim orgpath As String = Server.MapPath(Me.ModuleDirectory & "/images/nuntio_bin.png")
                    System.IO.File.Copy(orgpath, targetpath, True)
                End If

            Catch ex As Exception
                LogException(ex)
            End Try


        End Sub

        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New Entities.Modules.Actions.ModuleActionCollection

                Actions.Add(GetNextActionID, Localize("SETTINGS.Action"), Entities.Modules.Actions.ModuleActionType.ModuleSettings, "", "icon_hostsettings_16px.gif", NavigateURL(TabId, "Config", "Mid=" & ModuleId.ToString), False, Security.SecurityAccessLevel.Edit, True, False)

                If ModuleView.ToLower = "articlelist" OrElse ModuleView.ToLower = "threadedlist" Then

                    If Request.IsAuthenticated Then

                        VerifyActionIcons()

                        Actions.Add(GetNextActionID, Localize("TEMPLATES.Action"), Entities.Modules.Actions.ModuleActionType.ModuleSettings, "", "nuntio_templates.png", "", "NuntioArticlesTemplateForm('" & ArticleTheme & "')", False, Security.SecurityAccessLevel.Admin, True, False)

                        If AllowSubscriptions() Then
                            Actions.Add(GetNextActionID, Localize("LISTSUBSCRIPTIONS.Action"), Entities.Modules.Actions.ModuleActionType.ModuleSettings, "", "icon_users_16px.gif", NavigateURL(TabId, "", "mid=" & NewsModuleId, "act=ManageSubscriptions"), False, Security.SecurityAccessLevel.Edit, True, False)
                        End If

                        If blnCanAdd Then
                            Actions.Add(GetNextActionID, Localize("ADDARTICLE.Action"), Entities.Modules.Actions.ModuleActionType.AddContent, "", "nuntio_add.png", "", "NuntioArticlesAddForm('" & ModuleId.ToString & "', '" & NewsModuleId.ToString & "', '-1')", False, Security.SecurityAccessLevel.View, True, False)
                        End If

                        Dim iUnapproved As Integer = UnapprovedArticlesCount
                        Dim iExpired As Integer = ExpiredArticleCount
                        Dim iNotYetPublished As Integer = NotYetPublishedArticleCount
                        Dim iDeletedCount As Integer = DeletedArticlesCount

                        If Not Request.QueryString("view") Is Nothing Then
                            If Request.QueryString("view") = "unapproved" Then
                                If Request.IsAuthenticated Then
                                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then
                                        Actions.Add(GetNextActionID, Localize("APPROVED.Action"), Entities.Modules.Actions.ModuleActionType.AddContent, "", "nuntio_back.png", NavigateURL(TabId), False, Security.SecurityAccessLevel.Edit, True, False)
                                    End If
                                End If
                            End If
                        Else
                            If iUnapproved > 0 Then
                                Actions.Add(GetNextActionID, String.Format(Localize("UNAPPROVED.Action"), iUnapproved.ToString), Entities.Modules.Actions.ModuleActionType.AddContent, "", "nuntio_clock.png", NavigateURL(TabId, "", "view=unapproved"), False, Security.SecurityAccessLevel.Edit, True, False)
                            End If
                        End If

                        If Not Request.QueryString("view") Is Nothing Then
                            If Request.QueryString("view") = "expired" Then
                                If Request.IsAuthenticated Then
                                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then
                                        Actions.Add(GetNextActionID, Localize("APPROVED.Action"), Entities.Modules.Actions.ModuleActionType.AddContent, "", "nuntio_back.png", NavigateURL(TabId), False, Security.SecurityAccessLevel.Edit, True, False)
                                    End If
                                End If
                            End If
                        Else
                            If iExpired > 0 Then
                                Actions.Add(GetNextActionID, String.Format(Localize("EXPIRED.Action"), iExpired.ToString), Entities.Modules.Actions.ModuleActionType.AddContent, "", "nuntio_remove.png", NavigateURL(TabId, "", "view=expired"), False, Security.SecurityAccessLevel.Edit, True, False)
                            End If
                        End If

                        If Not Request.QueryString("view") Is Nothing Then
                            If Request.QueryString("view") = "notyetpublished" Then
                                If Request.IsAuthenticated Then
                                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then
                                        Actions.Add(GetNextActionID, Localize("APPROVED.Action"), Entities.Modules.Actions.ModuleActionType.AddContent, "", "nuntio_back.png", NavigateURL(TabId), False, Security.SecurityAccessLevel.Edit, True, False)
                                    End If
                                End If
                            End If
                        Else
                            If iNotYetPublished > 0 Then
                                Actions.Add(GetNextActionID, String.Format(Localize("NOTYETPUBLISHED.Action"), iNotYetPublished.ToString), Entities.Modules.Actions.ModuleActionType.AddContent, "", "nuntio_calendar.png", NavigateURL(TabId, "", "view=notyetpublished"), False, Security.SecurityAccessLevel.Edit, True, False)
                            End If
                        End If

                        If Not Request.QueryString("view") Is Nothing Then
                            If Request.QueryString("view") = "deleted" Then
                                If Request.IsAuthenticated Then
                                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then
                                        Actions.Add(GetNextActionID, Localize("UNDELETED.Action"), Entities.Modules.Actions.ModuleActionType.AddContent, "", "nuntio_back.png", NavigateURL(TabId), False, Security.SecurityAccessLevel.Edit, True, False)
                                    End If
                                End If
                            End If
                        Else
                            If iDeletedCount > 0 Then
                                Actions.Add(GetNextActionID, String.Format(Localize("DELETED.Action"), iDeletedCount.ToString), Entities.Modules.Actions.ModuleActionType.AddContent, "", "nuntio_bin.png", NavigateURL(TabId, "", "view=deleted"), False, Security.SecurityAccessLevel.Edit, True, False)
                            End If
                        End If





                    End If

                Else



                End If

                Return Actions
            End Get
        End Property

#End Region

        Private Sub LoadScripts()

            Dim varNuntioScript As String = "__NuntioArticlesScriptCollection"

            If Page.ClientScript.IsStartupScriptRegistered(varNuntioScript) = False Then

                Dim strScript As String = vbCrLf & "<script type=""text/javascript"">" & vbCrLf & vbCrLf


                strScript += "  function Nuntio_ShowAdvancedSearch() {" & vbCrLf
                strScript += "     jQuery('.nuntio-advancedsearch').show();" & vbCrLf
                strScript += "     jQuery('.nuntio-advancedbutton').hide();" & vbCrLf
                strScript += "     return true;" & vbCrLf
                strScript += "  }" & vbCrLf & vbCrLf

                strScript += "  function GetViewport(direction) {" & vbCrLf
                strScript += "      var viewportwidth;" & vbCrLf
                strScript += "      var viewportheight;" & vbCrLf
                strScript += "      var width = 915;" & vbCrLf
                strScript += "      var height = 750;" & vbCrLf
                strScript += "      if (typeof window.innerWidth != 'undefined') {viewportwidth = window.innerWidth,viewportheight = window.innerHeight}else if (typeof document.documentElement != 'undefined' && typeof document.documentElement.clientWidth != 'undefined' && document.documentElement.clientWidth != 0) {viewportwidth = document.documentElement.clientWidth,viewportheight = document.documentElement.clientHeight}else {viewportwidth = document.getElementsByTagName('body')[0].clientWidth,viewportheight = document.getElementsByTagName('body')[0].clientHeight}if (viewportheight < height) {height = viewportheight - 50;}if (viewportwidth < width) {width = viewportwidth - 50;}if (direction == 'height') {return height;}if (direction == 'width') {return width;}" & vbCrLf
                strScript += "  }" & vbCrLf & vbCrLf

                strScript += "  var height = GetViewport('height');" & vbCrLf
                strScript += "  var width = GetViewport('width');" & vbCrLf & vbCrLf

                Dim strRegisterUrl As String = NavigateURL(TabId, "", "ctl=register", "returnurl=" & Server.UrlEncode(NavigateURL(Convert.ToInt32(Request.QueryString("TabId")))))
                If PortalSettings.UserTabId <> Null.NullInteger Then
                    strRegisterUrl = NavigateURL(PortalSettings.UserTabId, "", "returnurl=" & Server.UrlEncode(NavigateURL(Convert.ToInt32(Request.QueryString("TabId")))))
                End If
                strRegisterUrl = strRegisterUrl

                strScript += "  function Nuntio_RedirectToRegister() {" & vbCrLf
                strScript += "      window.location.href = '" & strRegisterUrl & "';" & vbCrLf
                strScript += "  }" & vbCrLf & vbCrLf

                Dim strLoginUrl As String = NavigateURL(TabId, "", "ctl=login", "returnurl=" & Server.UrlEncode(NavigateURL(Convert.ToInt32(Request.QueryString("TabId")))))

                If PortalSettings.LoginTabId <> Null.NullInteger Then
                    strLoginUrl = NavigateURL(PortalSettings.LoginTabId, "", "returnurl=" & Server.UrlEncode(NavigateURL(Convert.ToInt32(Request.QueryString("TabId")))))
                End If
                strLoginUrl = strLoginUrl

                strScript += "  function Nuntio_RedirectToLogin() {" & vbCrLf
                strScript += "      window.location.href = '" & strLoginUrl & "';" & vbCrLf
                strScript += "  }" & vbCrLf & vbCrLf

                If Request.IsAuthenticated Then

                    strScript += "  function NuntioArticlesTemplateForm(ArticleTheme) {" & vbCrLf
                    strScript += "      var oManager = GetRadWindowManager();" & vbCrLf
                    strScript += "      var oWnd = oManager.open('" & ModuleDirectory & "/Windows/Window.aspx?ctl=uc_Templates&PortalId=" & PortalId.ToString & "&TabId=" & TabId.ToString & "&tmpl=' + ArticleTheme);" & vbCrLf
                    strScript += "      oWnd.set_height(550);" & vbCrLf
                    strScript += "      oWnd.set_width(950);" & vbCrLf
                    strScript += "      oWnd.center();   " & vbCrLf
                    strScript += "      return false;" & vbCrLf
                    strScript += "  }" & vbCrLf & vbCrLf

                    strScript += "  function NuntioArticlesEditForm(ItemId,NewsModuleId,ModuleId) {" & vbCrLf
                    strScript += "      var oManager = GetRadWindowManager();" & vbCrLf
                    strScript += "      var oWnd = oManager.open('" & ModuleDirectory & "/Windows/Window.aspx?ctl=uc_Edit&Language=" & CurrentLocale & "&ItemId=' + ItemId + '&ModuleId=' + NewsModuleId + '&TabId=" & TabId.ToString & "&OpenerModuleId=' + ModuleId + '&PortalId=" & PortalId.ToString & "');" & vbCrLf
                    strScript += "      oWnd.set_height(height);" & vbCrLf
                    strScript += "      oWnd.set_width(width);" & vbCrLf
                    strScript += "      oWnd.center();   " & vbCrLf
                    strScript += "  }" & vbCrLf & vbCrLf

                    strScript += "  function NuntioArticlesEmailForm(ItemId,NewsModuleId,ModuleId) {" & vbCrLf
                    strScript += "      var oManager = GetRadWindowManager();" & vbCrLf
                    strScript += "      var oWnd = oManager.open('" & ModuleDirectory & "/Windows/Window.aspx?ctl=uc_SendEmail&Language=" & CurrentLocale & "&ItemId=' + ItemId + '&ModuleId=' + NewsModuleId + '&TabId=" & TabId.ToString & "&OpenerModuleId=' + ModuleId + '&PortalId=" & PortalId.ToString & "');" & vbCrLf
                    strScript += "      oWnd.set_height(height);" & vbCrLf
                    strScript += "      oWnd.set_width(width);" & vbCrLf
                    strScript += "      oWnd.center();   " & vbCrLf
                    strScript += "  }" & vbCrLf & vbCrLf

                    strScript += "  function NuntioArticlesAddForm(ModuleId,NewsModuleId,CategoryId) {" & vbCrLf
                    strScript += "      var oManager = GetRadWindowManager();" & vbCrLf
                    strScript += "      var oWnd = oManager.open('" & ModuleDirectory & "/Windows/Window.aspx?ctl=uc_Edit&Language=" & CurrentLocale & "&ItemId=-1&ModuleId=' + NewsModuleId + '&PortalId=" & PortalId.ToString & "&CategoryId=' + CategoryId + '&OpenerModuleId=' + ModuleId);" & vbCrLf
                    strScript += "      oWnd.set_height(height);" & vbCrLf
                    strScript += "      oWnd.set_width(width);" & vbCrLf
                    strScript += "      oWnd.center();   " & vbCrLf
                    strScript += "      return false;" & vbCrLf
                    strScript += "  }" & vbCrLf & vbCrLf

                    strScript += "  function NuntioArticlesPublishForm(ModuleId,NewsModuleId,CategoryId) {" & vbCrLf
                    strScript += "      var oManager = GetRadWindowManager();" & vbCrLf
                    strScript += "      var oWnd = oManager.open('" & ModuleDirectory & "/Windows/Window.aspx?ctl=uc_Edit&Language=" & CurrentLocale & "&ItemId=-1&ModuleId=' + NewsModuleId + '&PortalId=" & PortalId.ToString & "&CategoryId=' + CategoryId + '&OpenerModuleId=' + ModuleId);" & vbCrLf
                    strScript += "      oWnd.set_height(height);" & vbCrLf
                    strScript += "      oWnd.set_width(width);" & vbCrLf
                    strScript += "      oWnd.center();   " & vbCrLf
                    strScript += "  }" & vbCrLf & vbCrLf

                    strScript += "  function NuntioArticlesPublicationsForm(ModuleId) {" & vbCrLf
                    strScript += "      var oManager = GetRadWindowManager();" & vbCrLf
                    strScript += "      var oWnd = oManager.open('" & ModuleDirectory & "/Windows/Window.aspx?ctl=uc_Publications&ModuleId=' + ModuleId + '&PortalId=" & PortalId.ToString & "');" & vbCrLf
                    strScript += "      oWnd.set_height(height);" & vbCrLf
                    strScript += "      oWnd.set_width(width);" & vbCrLf
                    strScript += "      oWnd.center();   " & vbCrLf
                    strScript += "      return false;" & vbCrLf
                    strScript += "  }" & vbCrLf & vbCrLf

                End If


                strScript += "  function NuntioArticlesCommentForm(ArticleId, commentid, tabid, moduleid, portalid, theme) {" & vbCrLf
                strScript += "      var oManager = GetRadWindowManager();" & vbCrLf
                strScript += "      var oWnd = oManager.open('" & ModuleDirectory & "/Windows/Window.aspx?ctl=uc_Comment&ArticleId=' + ArticleId + '&TabId=' + tabid + '&CommentId=' + commentid + '&ModuleId=' + moduleid + '&PortalId=' + portalid + '&ArticleTheme=' + theme);" & vbCrLf
                strScript += "      oWnd.set_height(500);" & vbCrLf
                strScript += "      oWnd.set_width(400);" & vbCrLf
                strScript += "      oWnd.center();   " & vbCrLf
                strScript += "  }" & vbCrLf & vbCrLf

                strScript += "  function NuntioArticlesShowArticle(ArticleId,mid,width,height,theme) {" & vbCrLf
                strScript += "      var oManager = GetRadWindowManager();" & vbCrLf
                strScript += "      var oWnd = oManager.open('" & ModuleDirectory & "/Windows/Window.aspx?ctl=uc_Detail&ArticleId=' + ArticleId + '&TabId=" & TabId.ToString & "&ModuleId=' + mid + '&ArticleTheme=' + theme);" & vbCrLf
                strScript += "      oWnd.set_height(height);" & vbCrLf
                strScript += "      oWnd.set_width(width);" & vbCrLf
                strScript += "      oWnd.center();   " & vbCrLf
                strScript += "  }" & vbCrLf & vbCrLf

                strScript += "</script>" & vbCrLf

                Page.ClientScript.RegisterStartupScript(Me.[GetType](), varNuntioScript, strScript)

            End If

        End Sub

    End Class
End Namespace
