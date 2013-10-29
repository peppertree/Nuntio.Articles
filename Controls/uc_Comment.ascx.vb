Imports DotNetNuke
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization.Localization

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Comment
        Inherits ArticleModuleBase

        Private _myfilename As String = "uc_Comment.ascx"
        Private _tabid As Integer = Null.NullInteger

        Public Property MyFileName()
            Get
                Return _myfilename
            End Get
            Set(ByVal value)
                _myfilename = value
            End Set
        End Property

#Region "Private Methods"


#End Region


#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            DotNetNuke.Framework.AJAX.RegisterScriptManager()

            Dim strFuncRefresh As String = "Nuntio_Refresh_" & ModuleId.ToString
            Dim strFuncRedirectLogin As String = "Nuntio_RedirectToLogin"
            Dim strFuncRedirectRegister As String = "Nuntio_RedirectToRegister"

            Dim strScript As String = vbCrLf & "<script type=""text/javascript"">" & vbCrLf & vbCrLf
            strScript += "  function CloseWindow(){" & vbCrLf
            strScript += "      var wnd = GetRadWindow();" & vbCrLf
            strScript += "      if(typeof wnd.BrowserWindow." & strFuncRefresh & " == 'function') {" & vbCrLf
            strScript += "          wnd.BrowserWindow." & strFuncRefresh & "();" & vbCrLf
            strScript += "          wnd.close();" & vbCrLf
            strScript += "          return false;" & vbCrLf
            strScript += "      }" & vbCrLf
            strScript += "  }" & vbCrLf & vbCrLf

            strScript += "  function RedirectToLogin(){" & vbCrLf
            strScript += "      var wnd = GetRadWindow();" & vbCrLf
            strScript += "      if(typeof wnd.BrowserWindow." & strFuncRedirectLogin & " == 'function') {" & vbCrLf
            strScript += "          wnd.BrowserWindow." & strFuncRedirectLogin & "();" & vbCrLf
            strScript += "          wnd.close();" & vbCrLf
            strScript += "          return false;" & vbCrLf
            strScript += "      }" & vbCrLf
            strScript += "  }" & vbCrLf & vbCrLf

            strScript += "  function RedirectToRegister(){" & vbCrLf
            strScript += "      var wnd = GetRadWindow();" & vbCrLf
            strScript += "      if(typeof wnd.BrowserWindow." & strFuncRedirectRegister & " == 'function') {" & vbCrLf
            strScript += "          wnd.BrowserWindow." & strFuncRedirectRegister & "();" & vbCrLf
            strScript += "          wnd.close();" & vbCrLf
            strScript += "          return false;" & vbCrLf
            strScript += "      }" & vbCrLf
            strScript += "  }" & vbCrLf
            strScript += "</script>" & vbCrLf & vbCrLf

            Page.ClientScript.RegisterStartupScript(Me.[GetType](), "Nuntio_CommentScripts_" & ModuleId.ToString, strScript)

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load

            If Not Request.QueryString("TabId") Is Nothing Then
                If IsNumeric(Request.QueryString("TabId")) Then
                    _tabid = Convert.ToInt32(Request.QueryString("TabId"))
                End If
            End If

            If Not Request.QueryString("amp;TabId") Is Nothing Then
                If IsNumeric(Request.QueryString("amp;TabId")) Then
                    _tabid = Convert.ToInt32(Request.QueryString("amp;TabId"))
                End If
            End If

            CType(Me.Page.Header.FindControl("lblTitle"), Literal).Text = GetString("lblTitle", GetResourceFile(Me, MyFileName))

            Me.btnSend.Text = GetString("btnSend", GetResourceFile(Me, MyFileName))
            Me.btnLogin.Text = GetString("btnLogin", GetResourceFile(Me, MyFileName))
            Me.btnRegister.Text = GetString("btnRegister", GetResourceFile(Me, MyFileName))
            Me.chkAuthorized.Text = GetString("chkAuthorized", GetResourceFile(Me, MyFileName))
            Me.lblApprove.Text = GetString("lblApprove", GetResourceFile(Me, MyFileName))
            Me.btnTryAgain.Text = GetString("btnTryAgain", GetResourceFile(Me, MyFileName))
            Me.btnClose1.Text = GetString("btnClose", GetResourceFile(Me, MyFileName))
            Me.btnClose2.Text = GetString("btnClose", GetResourceFile(Me, MyFileName))
            Me.btnClose3.Text = GetString("btnClose", GetResourceFile(Me, MyFileName))
            Me.lblGravatar.Text = GetString("lblGravatar", GetResourceFile(Me, MyFileName))
            Me.txtEmail.EmptyMessage = GetString("txtEmail", GetResourceFile(Me, MyFileName))
            Me.txtName.EmptyMessage = GetString("txtName", GetResourceFile(Me, MyFileName))
            Me.txtComment.EmptyMessage = GetString("YourComment", GetResourceFile(Me, MyFileName))

            If (Request.IsAuthenticated = False AndAlso AllowCommentsForAnnonymous = False) Then

                pnlForm.Visible = False
                pnlNotAuthorized.Visible = True

                If PortalSettings.UserRegistration = 0 Then
                    btnRegister.Visible = False
                    lblNotAuthorized.Text = GetString("NotAuthorized", GetResourceFile(Me, MyFileName)) & GetString("lblCannotRegister", GetResourceFile(Me, MyFileName))
                Else
                    btnRegister.Visible = True
                    lblNotAuthorized.Text = GetString("NotAuthorized", GetResourceFile(Me, MyFileName)) & GetString("lblRegister", GetResourceFile(Me, MyFileName))
                End If

            Else

                pnlForm.Visible = True
                pnlNotAuthorized.Visible = False

                BindComment()

            End If


        End Sub

        Private Sub BindComment()

            If Not Page.IsPostBack Then

                Dim nc As New ArticleController
                Dim oNews As ArticleInfo = nc.GetArticle(ArticleId, CurrentLocale, True)
                If Not oNews Is Nothing Then

                    If oNews.CreatedByUser = UserInfo.UserID Then

                        'article author mode
                        chkAuthorized.Visible = True
                        lblGravatar.Visible = False

                        If CommentId <> Null.NullInteger Then

                            Dim cc As New CommentController
                            Dim c As CommentInfo = cc.Get(CommentId)

                            If Not c Is Nothing Then

                                txtComment.Text = c.Comment
                                chkAuthorized.Checked = c.IsApproved

                                Dim ouser As UserInfo = UserController.GetUserById(PortalId, c.CreatedBy)

                                If Not ouser Is Nothing Then

                                    txtName.Text = ouser.DisplayName
                                    txtName.Enabled = False

                                    txtEmail.Text = ouser.Email
                                    txtEmail.Enabled = False

                                    lblCommentBy.Text = "<a href=""mailto:" & ouser.Email & """>" & ouser.DisplayName & "</a>, " & c.RemoteAddress
                                    pnlCommentBy.Visible = True

                                Else

                                    lblCommentBy.Text = "<a href=""mailto:" & c.Email & """>" & c.Displayname & "</a>, " & c.RemoteAddress
                                    pnlCommentBy.Visible = True

                                    txtName.Text = c.Displayname
                                    txtName.Enabled = True

                                    txtEmail.Text = c.Email
                                    txtEmail.Enabled = True

                                End If

                                btnSend.Text = GetString("btnUpdate", GetResourceFile(Me, MyFileName))
                                btnDelete.Text = GetString("btnDelete", GetResourceFile(Me, MyFileName))
                                btnDelete.Visible = True
                                pnlNeedsApproval.Visible = False

                            End If

                        End If
                    Else

                        'user comment mode
                        chkAuthorized.Visible = False
                        pnlCommentBy.Visible = False
                        pnlNeedsApproval.Visible = True

                        If Request.IsAuthenticated Then

                            txtEmail.Text = UserInfo.Email
                            txtEmail.Enabled = False

                            txtName.Text = UserInfo.DisplayName
                            txtName.Enabled = False

                        End If

                    End If

                End If

            Else

            End If

        End Sub

        Private Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click

            If Request.IsAuthenticated = False Then
                If txtComment.Text.Length = 0 Or txtName.Text.Length = 0 Or txtEmail.Text.Length = 0 Then
                    pnlResult.Visible = True
                    pnlForm.Visible = False
                    btnClose1.Visible = True
                    btnTryAgain.Visible = True
                    lblResult.Text = GetString("lblResult_FillInAll", GetResourceFile(Me, MyFileName))
                    Exit Sub
                End If
            Else
                If txtComment.Text.Length = 0 Then
                    pnlResult.Visible = True
                    pnlForm.Visible = False
                    btnClose1.Visible = True
                    btnTryAgain.Visible = True
                    lblResult.Text = GetString("lblResult_FillInComment", GetResourceFile(Me, MyFileName))
                    Exit Sub
                End If
            End If

            If txtComment.Text.Length > 0 Then
                If txtComment.Text <> GetString("YourComment", GetResourceFile(Me, MyFileName)) Then

                    Dim cc As New CommentController

                    If CommentId <> Null.NullInteger Then

                        Dim c As CommentInfo = cc.Get(CommentId)

                        If Not c Is Nothing Then

                            c.Comment = txtComment.Text
                            c.IsApproved = chkAuthorized.Checked
                            c.ApprovedBy = UserInfo.UserID

                            If c.CreatedBy <> Null.NullInteger Then
                                c.Displayname = ""
                                c.Email = ""
                            Else
                                c.Displayname = txtName.Text
                                c.Email = txtEmail.Text
                            End If

                            cc.Update(c)

                            If c.IsApproved Then
                                lblResult.Text = Localization.GetString("CommentApproved", Me.LocalResourceFile)
                            Else
                                lblResult.Text = Localization.GetString("CommentNotApproved", Me.LocalResourceFile)
                            End If

                            pnlForm.Visible = False
                            pnlResult.Visible = True
                            btnClose1.Visible = True
                            btnTryAgain.Visible = False

                        End If

                    Else

                        Dim objComment As New CommentInfo
                        objComment.ApprovedBy = Null.NullInteger
                        objComment.Comment = txtComment.Text

                        If Request.IsAuthenticated Then
                            objComment.CreatedBy = UserInfo.UserID
                            objComment.Displayname = Null.NullString
                            objComment.Email = Null.NullString
                            objComment.IsApproved = AutoApproveAuthenticatedtComments
                        Else
                            objComment.CreatedBy = Null.NullInteger
                            objComment.Displayname = txtName.Text
                            objComment.Email = txtEmail.Text
                            objComment.IsApproved = AutoApproveAnonymousComments
                        End If

                        objComment.CreatedDate = Date.Now

                        objComment.ArticleId = ArticleId.ToString
                        objComment.RemoteAddress = Request.UserHostAddress
                        objComment.ItemId = cc.Add(objComment)

                        Try
                            NotifyAuthor(objComment)
                        Catch
                        End Try

                        pnlForm.Visible = False
                        pnlResult.Visible = True

                        If objComment.IsApproved Then
                            lblResult.Text = GetString("lblResult_Approved", GetResourceFile(Me, MyFileName))
                        Else
                            lblResult.Text = GetString("lblResult", GetResourceFile(Me, MyFileName))
                        End If

                        btnClose1.Visible = True
                        btnTryAgain.Visible = False

                    End If


                End If
            End If



        End Sub

#End Region

        Private Sub NotifyAuthor(ByVal objComment As CommentInfo)

            Dim ctl As New ArticleController

            Dim oNews As ArticleInfo = ctl.GetArticle(objComment.ArticleId, CurrentLocale, True)
            If Not oNews Is Nothing Then

                Dim oAuthor As UserInfo = UserController.GetUserById(PortalSettings.PortalId, oNews.CreatedByUser)
                If Not oAuthor Is Nothing Then

                    Dim strArticleLink As String = NavigateURL(_tabid, "", "ArticleId=" & oNews.itemId.ToString)
                    Dim strAuthorName As String = oAuthor.DisplayName
                    Dim strAuthorEmail As String = oAuthor.Email
                    Dim strArticle As String = oNews.Title
                    Dim strPortal As String = PortalSettings.PortalName
                    Dim strComment As String = objComment.Comment
                    Dim strSender As String = PortalSettings.Email
                    Dim strBody As String = ""
                    Dim strSubject As String = String.Format(Localization.GetString("NewArticleComment", Me.LocalResourceFile), PortalSettings.PortalName)

                    Dim template As String = ""
                    Dim body As String = ""
                    Dim sb As New StringBuilder
                    template = CommentNotificationTemplate

                    Dim literal As New Literal
                    Dim delimStr As String = "[]"
                    Dim delimiter As Char() = delimStr.ToCharArray()

                    Dim templateArray As String()
                    templateArray = template.Split(delimiter)

                    For iPtr As Integer = 0 To templateArray.Length - 1 Step 2
                        sb.Append(templateArray(iPtr).ToString())
                        If iPtr < templateArray.Length - 1 Then

                            Select Case templateArray(iPtr + 1)
                                Case "AUTHORNAME"
                                    sb.Append(strAuthorName)
                                Case "ARTICLETITLE"
                                    sb.Append(strArticle)
                                Case "PORTALNAME"
                                    sb.Append(PortalSettings.PortalName)
                                Case "ARTICLELINK"
                                    sb.Append(strArticleLink)
                                Case "COMMENT"
                                    sb.Append(strComment)
                            End Select

                        End If
                    Next

                    strBody = sb.ToString

                    DotNetNuke.Services.Mail.Mail.SendMail(strSender, strAuthorEmail, "", strSubject, strBody, "", "HTML", "", "", "", "")

                End If

            End If

        End Sub


        Private Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click

            ctlAjax.ResponseScripts.Add("RedirectToLogin();")

        End Sub

        Private Sub btnRegister_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRegister.Click

            ctlAjax.ResponseScripts.Add("RedirectToRegister();")

        End Sub

        Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click

            Dim cc As New CommentController

            If CommentId <> Null.NullInteger Then

                Dim c As CommentInfo = cc.Get(CommentId)

                If Not c Is Nothing Then

                    cc.Delete(c)

                End If

            End If

            ctlAjax.ResponseScripts.Add("CloseWindow();")

        End Sub

        Private Sub btnClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClose1.Click, btnClose2.Click, btnClose3.Click
            ctlAjax.ResponseScripts.Add("CloseWindow();")
        End Sub

        Private Sub btnTryAgain_Click(sender As Object, e As System.EventArgs) Handles btnTryAgain.Click
            pnlForm.Visible = True
            pnlResult.Visible = False
        End Sub

    End Class


End Namespace
