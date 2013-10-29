
Imports Telerik.Web.UI

Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization



Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class Settings
        Inherits ArticleModuleBase
        Implements IActionable


#Region "Private Members"

        Public Property ControlToLoad() As String
            Get
                Dim m_controlToLoad As String = ""
                m_controlToLoad = txtControl.Text
                Return m_controlToLoad
            End Get
            Set(ByVal Value As String)
                txtControl.Text = Value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            For Each item As ListItem In drpView.Items
                item.Text = Localization.GetString(item.Value, Me.LocalResourceFile)
            Next

            If Not Page.IsPostBack Then
                If ModuleView <> "" Then
                    drpView.ClearSelection()
                    drpView.SelectedValue = ModuleView.ToLower
                    ReadSettings()
                End If
            End If

            If ControlToLoad() <> "" Then
                LoadControlType()
            End If

        End Sub

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            DotNetNuke.Framework.AJAX.RegisterScriptManager()

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

        Private Sub ReadSettings()

            'load control based on module settings

            If ModuleView <> "" Then
                Select Case ModuleView.ToLower
                    Case "articlepanel"
                        ControlToLoad = Me.ModuleDirectory & "/Modules/ArticlePanel/uc_PanelSettings.ascx"
                    Case "articlelist"
                        ControlToLoad = Me.ModuleDirectory & "/Modules/ArticleList/uc_ListSettings.ascx"
                    Case "threadedlist"
                        ControlToLoad = Me.ModuleDirectory & "/Modules/ThreadedList/uc_ListSettings.ascx"
                    Case "articlerotator"
                        ControlToLoad = Me.ModuleDirectory & "/Modules/ArticleRotator/uc_RotateSettings.ascx"
                    Case "rssbrowser"
                        ControlToLoad = Me.ModuleDirectory & "/Modules/RssBrowser/uc_RssBrowserSettings.ascx"
                    Case "categorybrowser"
                        ControlToLoad = Me.ModuleDirectory & "/Modules/CategoryBrowser/uc_CategoryBrowserSettings.ascx"
                    Case "subscribeform"
                        ControlToLoad = Me.ModuleDirectory & "/Modules/SubscriberForm/uc_SubscriberSettings.ascx"
                    Case "archivebrowser"
                        ControlToLoad = Me.ModuleDirectory & "/Modules/ArchiveBrowser/uc_ArchiveSettings.ascx"
                    Case "bannerviewer"
                        ControlToLoad = Me.ModuleDirectory & "/Modules/BannerViewer/uc_BannerSettings.ascx"
                    Case "authorbrowser"
                        ControlToLoad = Me.ModuleDirectory & "/Modules/AuthorBrowser/uc_AuthorBrowserSettings.ascx"
                End Select
            End If


        End Sub

        Private Sub LoadControlType()

            If ControlToLoad <> "" Then

                plhControls.Controls.Clear()
                '
                Dim objPortalModuleBase As SettingsBase = CType(Me.LoadControl(ControlToLoad), PortalModuleBase)
                objPortalModuleBase.ModuleConfiguration = Me.ModuleConfiguration
                objPortalModuleBase.ID = System.IO.Path.GetFileNameWithoutExtension(ControlToLoad)
                'objPortalModuleBase.BuildModuleTitleBoxes()
                objPortalModuleBase.LoadForm()
                ' Load the appropriate control
                '
                plhControls.Controls.Add(objPortalModuleBase)
                '
            End If

        End Sub



#End Region

#Region "Optional Interfaces"

        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New Entities.Modules.Actions.ModuleActionCollection

                If Request.QueryString("act") Is Nothing _
                    Or Request.QueryString("act") <> "ManageSettings" Then

                    Actions.Add(GetNextActionID, Localize("SETTINGS.Action"), Entities.Modules.Actions.ModuleActionType.AddContent, "", "icon_hostsettings_16px.gif", NavigateURL(TabId, "", "mid=" & ModuleId, "act=ManageSettings"), False, Security.SecurityAccessLevel.Edit, True, False)

                End If

                If Request.QueryString("act") Is Nothing _
                    Or Request.QueryString("act") <> "ManageSubscriptions" Then

                    Actions.Add(GetNextActionID, Localize("LISTSUBSCRIPTIONS.Action"), Entities.Modules.Actions.ModuleActionType.AddContent, "", "icon_users_16px.gif", NavigateURL(TabId, "", "mid=" & ModuleId, "act=ManageSubscriptions"), False, Security.SecurityAccessLevel.Edit, True, False)

                End If

                If IsModule() Then
                    If Not Request.QueryString("act") Is Nothing Then
                        Actions.Add(GetNextActionID, Localize("Back"), Entities.Modules.Actions.ModuleActionType.AddContent, "", "lt.gif", NavigateURL(TabId), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
                    End If
                End If

                Return Actions
            End Get
        End Property

#End Region



        Private Sub drpView_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpView.SelectedIndexChanged

            Select Case drpView.SelectedValue
                Case "articlepanel"
                    ControlToLoad = Me.ModuleDirectory & "/Modules/ArticlePanel/uc_PanelSettings.ascx"
                Case "articlelist"
                    ControlToLoad = Me.ModuleDirectory & "/Modules/ArticleList/uc_ListSettings.ascx"
                Case "threadedlist"
                    ControlToLoad = Me.ModuleDirectory & "/Modules/ThreadedList/uc_ListSettings.ascx"
                Case "articlerotator"
                    ControlToLoad = Me.ModuleDirectory & "/Modules/ArticleRotator/uc_RotateSettings.ascx"
                Case "rssbrowser"
                    ControlToLoad = Me.ModuleDirectory & "/Modules/RssBrowser/uc_RssBrowserSettings.ascx"
                Case "categorybrowser"
                    ControlToLoad = Me.ModuleDirectory & "/Modules/CategoryBrowser/uc_CategoryBrowserSettings.ascx"
                Case "subscribeform"
                    ControlToLoad = Me.ModuleDirectory & "/Modules/SubscriberForm/uc_SubscriberSettings.ascx"
                Case "archivebrowser"
                    ControlToLoad = Me.ModuleDirectory & "/Modules/ArchiveBrowser/uc_ArchiveSettings.ascx"
                Case "bannerviewer"
                    ControlToLoad = Me.ModuleDirectory & "/Modules/BannerViewer/uc_BannerSettings.ascx"
                Case "authorbrowser"
                    ControlToLoad = Me.ModuleDirectory & "/Modules/AuthorBrowser/uc_AuthorBrowserSettings.ascx"
            End Select

            LoadControlType()

        End Sub


    End Class
End Namespace
