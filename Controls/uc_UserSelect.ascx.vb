
Imports Telerik.Web.UI

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_UserSelect
        Inherits ArticleModuleBase

#Region "Private Members"


#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            CType(Me.Page.FindControl("lblTitle"), Literal).Text = Localize("SelectUser")
            btnClose.Text = Localize("btnClose")

            If Not Page.IsPostBack Then
                btnUpdate.Text = Localize("CheckUser")
            End If

        End Sub

        Private Sub txtName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtName.TextChanged
            lblMessage.Text = ""
            btnUpdate.CommandName = "Check"
            btnUpdate.Text = Localize("CheckUser")
            txtUsername.Text = ""
            txtName.CssClass = ""
        End Sub

        Private Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
            Select Case (CType(sender, Button).CommandName)
                Case "Check"
                    CheckUser()
                Case "Update"
                    UpdateUser()
            End Select
        End Sub

#End Region

#Region "Private Methods"

        Private Sub CheckUser()

            Dim oUser As Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.GetUserByName(PortalSettings.PortalId, txtName.Text.Trim)

            If oUser Is Nothing Then
                lblMessage.Text = Localize("UserNotFound")
                btnUpdate.CommandName = "Check"
                btnUpdate.Text = Localize("CheckUser")
                txtName.Text = ""
                txtUsername.Text = ""
            Else
                lblMessage.Text = ""
                btnUpdate.CommandName = "Update"
                btnUpdate.Text = Localize("ApplyUser")
                txtName.Text = oUser.DisplayName
                txtName.CssClass = "checked"
                txtUsername.Text = oUser.UserID.ToString
            End If
        End Sub

        Private Sub UpdateUser()
            Me.ctlAjax.ResponseScripts.Add("setUser('" & txtUsername.Text & "','" & txtName.Text & "');")
        End Sub

#End Region

        Private Sub btnClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClose.Click
            Me.ctlAjax.ResponseScripts.Add("dialogClose();")
        End Sub

    End Class
End Namespace
