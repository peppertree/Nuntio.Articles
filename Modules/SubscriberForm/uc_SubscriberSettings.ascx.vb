Imports DotNetNuke
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports System.IO
Imports DotNetNuke.Security.Permissions

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_SubscriberSettings
        Inherits SettingsBase

#Region "Private Methods"

        Private Sub LoadSettings()

            Try
                Dim modulecontrol As New ModuleController
                modulecontrol.DeleteTabModuleSetting(TabModuleId, "NewsmoduleIdList")
                modulecontrol.DeleteTabModuleSetting(TabModuleId, "ProceedAfterSubmit")
                modulecontrol.DeleteTabModuleSetting(TabModuleId, "UnsubscribeTemplate")
            Catch
            End Try

            If Settings.Contains("NewsmoduleIdList") Then
                Dim Modules As String() = CType(Me.Settings("NewsmoduleIdList"), String).Split(Char.Parse(","))
                For Each sModule As String In Modules
                    Try
                        Me.chkNewsModules.Items.FindByValue(sModule).Selected = True
                    Catch
                    End Try
                Next
            End If

            If Settings.Contains("ProceedAfterSubmit") Then
                Dim sTab As String = CType(Me.Settings("ProceedAfterSubmit"), String)
                Me.drpTabs.SelectedValue = sTab
            End If

            Me.drpThemes.SelectedValue = Me.ArticleTheme

            BindTitleBoxes()

        End Sub

        Private Sub SaveSettings()

            Dim modulecontrol As New ModuleController

            Dim sModules As String = ""
            For Each item As ListItem In Me.chkNewsModules.Items
                If item.Selected Then
                    sModules = sModules & item.Value & ","
                End If
            Next

            If sModules <> "" Then
                modulecontrol.UpdateModuleSetting(ModuleId, "NewsmoduleIdList", sModules)
            Else
                modulecontrol.DeleteModuleSetting(ModuleId, "NewsmoduleIdList")
            End If

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ProceedAfterSubmit", Me.drpTabs.SelectedValue)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ArticleTheme", Me.drpThemes.SelectedValue)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ModuleView", "SubscribeForm")

            UpdateModuletitle()

        End Sub

        Private Sub BindTabs()
            Me.drpTabs.DataSource = TabController.GetPortalTabs(PortalSettings.PortalId, True, True, False, True, True)
            Me.drpTabs.DataBind()
        End Sub

        Private Sub BindArticleModules()

            Me.chkNewsModules.Items.Clear()


            Dim objModules As New ModuleController
            Dim arrModules As New ArrayList
            Dim objModule As ModuleInfo

            Dim tabs As List(Of DotNetNuke.Entities.Tabs.TabInfo) = TabController.GetPortalTabs(PortalSettings.PortalId, Null.NullInteger, False, "", True, False, False, False, False)

            Dim dicModules As System.Collections.Generic.Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo)

            For Each oTab As DotNetNuke.Entities.Tabs.TabInfo In tabs
                dicModules = objModules.GetTabModules(oTab.TabID)
                For Each objModule In dicModules.Values
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "VIEW", objModule) And objModule.IsDeleted = False Then
                        If IsArticleModule(objModule, oTab.TabID) Then

                            Dim chkItem As New ListItem
                            chkItem.Text = oTab.TabPath.Replace("//", "&nbsp;&raquo;&nbsp;") & ": " & objModule.ModuleTitle
                            chkItem.Value = objModule.ModuleID
                            Me.chkNewsModules.Items.Add(chkItem)

                        End If
                    End If
                Next
            Next

            If chkNewsModules.Items.Count = 0 Then
                Me.chkNewsModules.Visible = False
                Me.lblNoModule.Text = Localization.GetString("NoModuleFound", Me.LocalResourceFile)
                Me.lblNoModule.Visible = True
                Me.cmdUpdate.Enabled = False
            Else
                Me.lblNoModule.Visible = False
                Me.chkNewsModules.Visible = True
            End If

        End Sub

        Private Sub UpdateModuletitle()

            Dim i As Integer
            For i = 0 To SupportedLocales.Count - 1

                Dim objModuleSettings As New ModuleController
                Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(CType(SupportedLocales(i).Value, Locale).Code)
                Dim txtBox As TextBox
                Dim strBoxID As String = DotNetNuke.Common.Globals.CreateValidID("txtTitle_" & Convert.ToString(info.LCID))
                Dim plhBoxes As Control = Me.FindControl("plhModultitle")
                Dim ctlBox As Control = plhBoxes.FindControl(strBoxID)

                txtBox = CType(ctlBox, TextBox)
                If txtBox.Text.Length > 0 Then
                    objModuleSettings.UpdateModuleSetting(ModuleId, "PNC_NEWS_MODULETITLE_" & Convert.ToString(info.LCID), txtBox.Text)
                Else
                    objModuleSettings.DeleteModuleSetting(ModuleId, "PNC_NEWS_MODULETITLE_" & Convert.ToString(info.LCID))
                End If

            Next
        End Sub

        Private Sub BindTitleBoxes()

            Dim i As Integer
            For i = 0 To SupportedLocales.Count - 1

                Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(CType(SupportedLocales(i).Value, Locale).Code)

                If Not Settings("PNC_NEWS_MODULETITLE_" & Convert.ToString(info.LCID)) Is Nothing Then
                    Dim txtBox As TextBox
                    Dim strBoxID As String = DotNetNuke.Common.Globals.CreateValidID("txtTitle_" & Convert.ToString(info.LCID))
                    Dim plhBoxes As Control = Me.FindControl("plhModultitle")
                    Dim ctlBox As System.Web.UI.Control = plhBoxes.FindControl(strBoxID)
                    txtBox = CType(ctlBox, TextBox)
                    txtBox.Text = CType(Settings("PNC_NEWS_MODULETITLE_" & Convert.ToString(info.LCID)), String)
                End If

            Next

        End Sub

        Private Sub BuildModuleTitleBoxes(ByRef plhModultitle As System.Web.UI.WebControls.PlaceHolder)


            Dim _openingTable As New Literal
            _openingTable.Text = "<table>"
            plhModultitle.Controls.Add(_openingTable)

            Dim i As Integer
            For i = 0 To SupportedLocales.Count - 1
                Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(CType(SupportedLocales(i).Value, Locale).Code)

                Dim _BoxRowOpen As New Literal
                _BoxRowOpen.Text = "<tr><td align=""left""><span class=""normal"">" & System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.DisplayName) & "</span></td><td>&nbsp;</td><td align=""left"">"
                plhModultitle.Controls.Add(_BoxRowOpen)

                Dim _txtBox As New TextBox
                _txtBox.ID = DotNetNuke.Common.Globals.CreateValidID("txtTitle_" & Convert.ToString(info.LCID))
                plhModultitle.Controls.Add(_txtBox)

                Dim _BoxRowClose As New Literal
                _BoxRowClose.Text = "</td></tr>"
                plhModultitle.Controls.Add(_BoxRowClose)

            Next

            Dim _closingTable As New Literal
            _closingTable.Text = "</table>"
            plhModultitle.Controls.Add(_closingTable)

        End Sub

#End Region

#Region "Event Handlers"

        Public Overrides Sub LoadForm()

            BuildModuleTitleBoxes(Me.plhModultitle)

            BindThemes()
            BindArticleModules()
            BindTabs()

            LoadSettings()



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

        Private Sub BindThemes()
            Me.drpThemes.Items.Clear()
            Me.drpThemes.Items.Add(New ListItem("Default", "Default"))

            Dim oP As New DotNetNuke.Entities.Portals.PortalController
            Dim hPortal As DotNetNuke.Entities.Portals.PortalInfo = oP.GetPortal(PortalSettings.PortalId)

            'If UserInfo.IsSuperUser Then
            For Each folder As String In Directory.GetDirectories(Server.MapPath(Me.ModuleDirectory & "/templates/"))
                Dim sFolder As String = folder.Substring(folder.LastIndexOf("\") + 1)
                If sFolder.ToLower = "portal" Then
                    For Each portalfolder As String In Directory.GetDirectories(folder)
                        Dim pid As String = portalfolder.Substring(portalfolder.LastIndexOf("\") + 1)
                        If IsNumeric(pid) Then
                            Dim oPortal As DotNetNuke.Entities.Portals.PortalInfo = oP.GetPortal(pid)
                            If Not oPortal Is Nothing Then
                                For Each templatefolder As String In Directory.GetDirectories(portalfolder)

                                    Dim value As String = "Portal/" & oPortal.PortalID.ToString & "/" & templatefolder.Substring(templatefolder.LastIndexOf("\") + 1)
                                    Dim text As String = templatefolder.Substring(templatefolder.LastIndexOf("\") + 1)

                                    If Not text.ToLower = "newsletter" Then
                                        Me.drpThemes.Items.Add(New ListItem(text, value))
                                    End If

                                Next
                            End If
                        End If
                    Next
                Else
                    If Not sFolder.ToLower = "default" And Not sFolder.ToLower = "newsletter" Then
                        Me.drpThemes.Items.Add(New ListItem(sFolder, sFolder))
                    End If
                End If
            Next
            'Else
            '    Dim portalpath As String = Server.MapPath(Me.ModuleDirectory & "/templates/Portal/" & PortalSettings.PortalId.ToString)
            '    If Directory.Exists(portalpath) Then
            '        For Each folder As String In Directory.GetDirectories(portalpath)
            '            Dim value As String = "Portal/" & PortalSettings.PortalId.ToString & "/" & folder.Substring(folder.LastIndexOf("\") + 1)
            '            Dim text As String = folder.Substring(folder.LastIndexOf("\") + 1)
            '            If Not text.ToLower = "newsletter" Then
            '                Me.drpThemes.Items.Add(New ListItem(text, value))
            '            End If
            '        Next
            '    End If
            'End If

        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            If Page.IsValid Then
                SaveSettings()
            End If
            Try
                Response.Redirect(NavigateURL())
            Catch
            End Try
        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL())
            Catch
            End Try
        End Sub

    End Class
End Namespace
