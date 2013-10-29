Imports DotNetNuke
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports Telerik.Web.UI

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_ListSubscriptions
        Inherits ArticleModuleBase

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try

                If Not Page.IsPostBack Then

                    BindLocales()

                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Public Sub cmdApprove_Click(ByVal sender As System.Object, ByVal e As EventArgs)


            Dim id As Integer = Convert.ToInt32(CType(sender, LinkButton).CommandArgument)


            Dim SubscriptionController As New SubscriptionController
            Dim objSubscription As SubscriptionInfo = SubscriptionController.GetSubscription(id)

            If Not objSubscription Is Nothing Then
                SubscriptionController.VerifySubscription(objSubscription)
            End If

            grdSubscriptions.Rebind()

        End Sub

        Public Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As EventArgs)


            Dim id As Integer = Convert.ToInt32(CType(sender, LinkButton).CommandArgument)


            Dim SubscriptionController As New SubscriptionController
            Dim objSubscription As SubscriptionInfo = SubscriptionController.GetSubscription(id)

            If Not objSubscription Is Nothing Then
                SubscriptionController.UnSubscribe(objSubscription)
            End If

            grdSubscriptions.Rebind()

        End Sub

        Private Sub cboLocale_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboLocale.SelectedIndexChanged
            grdSubscriptions.Rebind()
        End Sub

        Private Sub grdSubscriptions_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles grdSubscriptions.ItemDataBound

            If e.Item.ItemType = Telerik.Web.UI.GridItemType.AlternatingItem Or e.Item.ItemType = Telerik.Web.UI.GridItemType.Item Then
                Dim objSubscription As SubscriptionInfo = CType(e.Item.DataItem, SubscriptionInfo)
                If objSubscription.UserID > 0 Then

                    Dim ctrl As Control = e.Item.Cells(8).Controls(0)
                    ctrl.Visible = False

                End If
            End If

        End Sub

        Private Sub grdSubscriptions_NeedDataSource(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grdSubscriptions.NeedDataSource
            Dim subscriberscontroller As New SubscriptionController
            Dim subscriptions As New List(Of SubscriptionInfo)
            subscriptions = subscriberscontroller.ListSubscriptions(ModuleId, Me.cboLocale.SelectedValue)
            grdSubscriptions.DataSource = subscriptions

        End Sub

        Private Sub BindLocales()

            cboLocale.Items.Clear()

            For Each objLocale As Locale In SupportedLocales.Values
                Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
                Me.cboLocale.Items.Add(New ListItem(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.DisplayName), System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name).ToUpper))
            Next

            Try
                cboLocale.Items.FindByValue(CurrentLocale.ToUpper).Selected = True
            Catch
            End Try

        End Sub

        Public Function FormatUserID(ByVal intID As String) As String
            If Integer.Parse(intID) <> -1 Then
                Return intID
            Else
                Return "-"
            End If
        End Function

#End Region

        Private Sub btnUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpload.Click

            Dim intLines As Integer = 0
            Dim intSucess As Integer = 0
            Dim intError As Integer = 0

            If ctlImported.HasFile Then
                Try

                    Dim sr As New System.IO.StreamReader(ctlImported.PostedFile.InputStream, System.Text.Encoding.UTF8)

                    Do While sr.Peek() >= 0

                        Dim strLine As String = sr.ReadLine

                        'skip first line
                        If intLines > 0 Then

                            Dim Values() As String = New String() {}

                            If strLine.Contains(";") Then
                                Values = strLine.Split(Char.Parse(";"))
                            ElseIf strLine.Contains(",") Then
                                Values = strLine.Split(Char.Parse(","))
                            End If

                            If Values.Length > 0 Then

                                Dim ctrl As New SubscriptionController

                                Dim strName As String = ""
                                Dim strEmail As String = ""

                                Try
                                    strName += Values(0).ToString & " "
                                Catch
                                End Try
                                Try
                                    strName += Values(1).ToString
                                Catch
                                End Try
                                Try
                                    strEmail = Values(2).ToString
                                Catch
                                End Try

                                If strName.Length > 0 AndAlso strEmail.Length > 0 Then

                                    Dim intUsers As Integer = 0
                                    Dim oUsers As ArrayList = UserController.GetUsersByEmail(PortalId, strEmail, 0, 9999, intUsers)
                                    Dim oSub As SubscriptionInfo = Nothing

                                    If intUsers > 0 Then
                                        Dim oUser As UserInfo = CType(oUsers(0), UserInfo)
                                        oSub = New SubscriptionInfo(oUser.UserID, ModuleId)
                                    Else
                                        oSub = New SubscriptionInfo(strEmail, strName, cboLocale.SelectedValue, ModuleId)
                                    End If

                                    If Not oSub Is Nothing Then

                                        oSub.Verified = True

                                        If Not String.IsNullOrEmpty(ctrl.Subscribe(oSub)) Then
                                            intSucess += 1
                                        Else
                                            intError += 1
                                        End If

                                    Else
                                        intError += 1
                                    End If


                                Else
                                    intError += 1
                                End If

                            End If

                        End If

                        intLines += 1

                    Loop

                    sr.Close()
                    sr.Dispose()
                    ctlImported.FileContent.Close()

                    lblImportResult.Text = "Read " & intLines.ToString & " lines.<br />Created entries: " & intSucess.ToString & "<br />Errors on lines: " & intError.ToString

                    grdSubscriptions.Rebind()

                Catch ex As Exception
                    lblImportResult.Text = "ERROR: " & ex.Message.ToString() & "<br />Last line read: " & intLines.ToString & "<br />Created entries: " & intSucess.ToString & "<br />Errors on lines: " & intError.ToString
                End Try
            Else
                lblImportResult.Text = "You have not specified a file."
            End If


        End Sub

        Private Sub grdSubscriptions_UpdateCommand(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles grdSubscriptions.UpdateCommand

            Dim editableItem = (DirectCast(e.Item, GridEditableItem))
            Dim itemId = DirectCast(editableItem.GetDataKeyValue("ItemId"), Integer)
            Dim ctrl As New SubscriptionController

            Dim objSubscription As SubscriptionInfo = ctrl.GetSubscription(itemId)
            ctrl.UnSubscribe(objSubscription)

            If Not objSubscription Is Nothing Then
                Try
                    editableItem.UpdateValues(objSubscription)
                    ctrl.Subscribe(objSubscription)
                Catch
                End Try
            End If

            grdSubscriptions.Rebind()

        End Sub

    End Class
End Namespace
