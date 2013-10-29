Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports System.IO

Imports Telerik.Web.UI
Imports DotNetNuke.Security.Permissions

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_PanelSettings
        Inherits SettingsBase

#Region "Private Methods"

        Private Sub BindArticleModules()

            drpModules.Items.Clear()

            Dim tabs As List(Of DotNetNuke.Entities.Tabs.TabInfo) = TabController.GetPortalTabs(PortalSettings.PortalId, Null.NullInteger, False, "", True, False, False, False, False)

            Dim objModules As New ModuleController
            Dim arrModules As New ArrayList
            Dim objModule As ModuleInfo

            Dim dicModules As System.Collections.Generic.Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo)

            For Each oTab As DotNetNuke.Entities.Tabs.TabInfo In tabs
                dicModules = objModules.GetTabModules(oTab.TabID)
                For Each objModule In dicModules.Values
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "VIEW", objModule) And objModule.IsDeleted = False Then
                        If IsArticleModule(objModule, oTab.TabID) Then
                            drpModules.Items.Add(New RadComboBoxItem("[" & oTab.TabName & "] - " & objModule.ModuleTitle, oTab.TabID.ToString & ";" & objModule.ModuleID.ToString))
                        End If
                    End If
                Next
            Next

            If drpModules.Items.Count = 0 Then
                drpModules.Items.Add(New RadComboBoxItem("[No article module found...]", "-1"))
            End If

        End Sub

        Private Sub BindFilters()

            drpFilter.Items.Clear()
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_Published")))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_NotYetPublished")))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_NeedsReviewing")))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_AwaitingApproval")))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_Expired")))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_Featured")))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_Deleted")))

            drpView.Items.Clear()
            drpView.Items.Add(New RadComboBoxItem(Localize("ViewMode_Publications")))
            drpView.Items.Add(New RadComboBoxItem(Localize("ViewMode_Articles")))

        End Sub

        Private Sub LoadSettings()

            If Settings.Contains("DefaultModule") Then
                Try
                    Dim value As String = CType(Settings("DefaultModule"), String)
                    drpModules.SelectedValue = value
                Catch
                End Try
            End If
            If Settings.Contains("DefaultFilter") Then
                Try
                    Dim index As Integer = Convert.ToInt32(Settings("DefaultFilter"))
                    drpFilter.Items(index).Selected = True
                Catch
                End Try
            End If
            If Settings.Contains("DefaultMode") Then
                Try
                    Dim index As Integer = Convert.ToInt32(Settings("DefaultMode"))
                    drpView.Items(index).Selected = True
                Catch
                End Try
            End If

        End Sub

        Private Sub SaveSettings()

            Dim modulecontrol As New ModuleController

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ModuleView", "ArticlePanel")
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "DefaultModule", drpModules.SelectedValue)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "DefaultFilter", drpFilter.SelectedIndex.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "DefaultMode", drpView.SelectedIndex.ToString)

        End Sub

#End Region

#Region "Event Handlers"

        Public Overrides Sub LoadForm()

            BindArticleModules()
            BindFilters()
            LoadSettings()

            lblVersion.Text = "Nuntio.Articles, built " & System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString()
            lblDefaultFilter.Text = Localization.GetString("lblDefaultFilter", LocalResourceFile)
            lblDefaultMode.Text = Localization.GetString("lblDefaultMode", LocalResourceFile)
            lblDefaultModule.Text = Localization.GetString("lblDefaultModule", LocalResourceFile)

        End Sub

        Protected Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            If Page.IsValid Then
                SaveSettings()
            End If
            Try
                Response.Redirect(NavigateURL())
            Catch
            End Try
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL())
            Catch
            End Try
        End Sub

#End Region

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

        End Sub

#End Region

    End Class
End Namespace
