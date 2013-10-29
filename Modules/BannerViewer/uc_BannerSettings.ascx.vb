Imports DotNetNuke
Imports dnnWerk

Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Security.Permissions

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_BannerSettings

        Inherits SettingsBase
        Implements IActionable

#Region "Private Methods"

        Private Sub LoadSettings()

            'select show from...
            Try
                Me.cboModules.Items.FindByValue(Me.NewsModuleTab.ToString & ";" & Me.NewsModuleId.ToString).Selected = True
            Catch
            End Try

            Try
                Dim modulecontrol As New ModuleController
                modulecontrol.DeleteModuleSetting(Me.ModuleId, "NewsModuleID")
                modulecontrol.DeleteModuleSetting(Me.ModuleId, "NewsmoduleID")
                modulecontrol.DeleteModuleSetting(Me.ModuleId, "NewsmoduleId")
            Catch
            End Try

            Try
                Me.chkHideOnAllNews.Checked = Me.HideOnAllNews
                Me.optSource.SelectedValue = Me.BannerSource
                Me.cboType.SelectedValue = Me.BannerType
                Me.txtCount.Text = Me.BannerCount
                Me.rblTransitionType.SelectedValue = Me.TransitionType
                Me.drpTransition.SelectedValue = Me.TransitionEffect
                Me.txtScrollHeight.Text = Me.ScrollHeight
                Me.txtScrollWidth.Text = Me.ScrollWidth
                Me.txtScrollTimeout.Text = Me.ScrollTimeOut
                Me.txtScrollSpeed.Text = Me.ScrollSpeed
                Me.drpScrollDirection.SelectedValue = ScrollDirection
            Catch
            End Try

            BindTitleBoxes()

        End Sub

        Private Sub SaveSettings()

            Dim modulecontrol As New ModuleController

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ModuleView", "BannerViewer")

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleTab", Me.cboModules.SelectedValue.Split(Char.Parse(";"))(0))
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleId", Me.cboModules.SelectedValue.Split(Char.Parse(";"))(1))

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "HideOnAllNews", Me.chkHideOnAllNews.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "BannerSource", Me.optSource.SelectedValue)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "BannerType", Me.cboType.SelectedValue)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "BannerCount", Me.txtCount.Text.Trim)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "TransitionType", Me.rblTransitionType.SelectedValue)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "TransitionEffect", Me.drpTransition.SelectedValue)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ScrollHeight", Me.txtScrollHeight.Text.Trim)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ScrollWidth", Me.txtScrollWidth.Text.Trim)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ScrollTimeout", Me.txtScrollTimeout.Text.Trim)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ScrollSpeed", Me.txtScrollSpeed.Text.Trim)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ScrollDirection", Me.drpScrollDirection.SelectedValue)


            UpdateModuletitle()

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

                If Not Settings("PNC_NEWSBANNER_MODULETITLE_" & Convert.ToString(info.LCID)) Is Nothing Then
                    Dim txtBox As TextBox
                    Dim strBoxID As String = DotNetNuke.Common.Globals.CreateValidID("txtTitle_" & Convert.ToString(info.LCID))
                    Dim plhBoxes As Control = Me.FindControl("plhModultitle")
                    Dim ctlBox As System.Web.UI.Control = plhBoxes.FindControl(strBoxID)
                    txtBox = CType(ctlBox, TextBox)
                    txtBox.Text = CType(Settings("PNC_NEWSBANNER_MODULETITLE_" & Convert.ToString(info.LCID)), String)
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

        Private Sub BindScrollDirections()
            Me.drpScrollDirection.Items.Clear()
            Me.drpScrollDirection.Items.Add(New ListItem(Services.Localization.Localization.GetString("ScrollUp", Me.LocalResourceFile), "4"))
            Me.drpScrollDirection.Items.Add(New ListItem(Services.Localization.Localization.GetString("ScrollDown", Me.LocalResourceFile), "8"))
            Me.drpScrollDirection.Items.Add(New ListItem(Services.Localization.Localization.GetString("ScrollLeft", Me.LocalResourceFile), "1"))
            Me.drpScrollDirection.Items.Add(New ListItem(Services.Localization.Localization.GetString("ScrollRight", Me.LocalResourceFile), "2"))
        End Sub

        Private Sub BindBannerTypes()

            Dim objBannerTypes As New Services.Vendors.BannerTypeController

            cboType.DataSource = objBannerTypes.GetBannerTypes
            cboType.DataBind()
            cboType.Items.Insert(0, New ListItem(Services.Localization.Localization.GetString("AllTypes", LocalResourceFile), "-1"))

        End Sub

#End Region

#Region "Event Handlers"

        Public Overrides Sub LoadForm()

            BuildModuleTitleBoxes(Me.plhModultitle)

            BindModules(cboModules, False)
            BindBannerTypes()
            BindScrollDirections()

            If cboModules.Items.Count > 0 Then
                LoadSettings()
            Else
                Me.Controls.Clear()
                Me.Controls.Add(New LiteralControl("<div style=""margin:20px;padding:20px;border:1px dashed #ccc;background:yellow;font-weight:bold;"">In order to work with the Banner Rotator, you first have to create an article listing somewhere in your portal to connect to.</div>"))
            End If

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

#Region "Optional Interfaces"
        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Localize("Back"), ModuleActionType.AddContent, "", "lt.gif", NavigateURL(TabId), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property
#End Region

    End Class
End Namespace
