
Imports Telerik.Web.UI

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Message
        Inherits ArticleModuleBase

#Region "Private Members"


#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            CType(Me.Page.FindControl("lblTitle"), Literal).Text = "Select File..."

            Dim msg As String = Request.QueryString("msg")
            plhControls.Controls.Add(New LiteralControl(msg))

        End Sub

#End Region

#Region "Private Methods"



#End Region


        Protected Sub btnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClose.Click
            Me.ctlAjax.ResponseScripts.Add("Close();")
        End Sub

    End Class
End Namespace
