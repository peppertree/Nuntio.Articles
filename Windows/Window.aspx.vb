Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class Window
        Inherits DotNetNuke.Framework.PageBase

#Region " Private Members "

        Private m_controlToLoad As String

#End Region


        Private Sub Window_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            DotNetNuke.Framework.AJAX.AddScriptManager(Me)

            Page.ClientScript.RegisterClientScriptInclude("dnncore", ResolveUrl("~/js/dnncore.js"))
            DotNetNuke.UI.Utilities.ClientAPI.HandleClientAPICallbackEvent(Me)

            DotNetNuke.Framework.AJAX.RegisterScriptManager()

            BindCss()

            If Not Request.QueryString("ctl") Is Nothing Then
                m_controlToLoad = Request.QueryString("ctl") & ".ascx"
                If m_controlToLoad = "uc_DMXSelect.ascx" Then
                    Dim url As String = ResolveUrl("~/Desktopmodules/DMXBrowser.ContentProvider/DMXBrowser.aspx?PortalId=" & PortalSettings.PortalId.ToString & "&Path=/")
                    Try
                        Response.Redirect(url)
                    Catch
                    End Try
                End If
                LoadControlType()
            End If

        End Sub

        Private Sub BindCss()
            Dim strCss As String = "<link href=""" & Me.ResolveUrl("~/Desktopmodules/Nuntio.Articles/module.css") & """ rel=""stylesheet"" type=""text/css"" />"
            Try
                Me.Page.FindControl("CSS").Controls.Add(New LiteralControl(strCss))
            Catch
            End Try
        End Sub

        Private Sub LoadControlType()

            Dim controlPath As String = Me.ResolveUrl("~/Desktopmodules/Nuntio.Articles/controls/" & m_controlToLoad)

            Dim objPortalModuleBase As ArticleBase = CType(Me.LoadControl(controlPath), ArticleBase)

            If Not (objPortalModuleBase Is Nothing) Then

                'objPortalModuleBase.ModuleConfiguration = Me.ModuleConfiguration
                objPortalModuleBase.ID = System.IO.Path.GetFileNameWithoutExtension(m_controlToLoad)
                plhControls.Controls.Add(objPortalModuleBase)

            End If

        End Sub

    End Class
End Namespace
