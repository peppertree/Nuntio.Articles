Imports DotNetNuke
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Security.Permissions

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_AuthorBrowserSettings
        Inherits SettingsBase
        Implements IActionable

#Region "Private Methods"

        Private Sub LoadSettings()

            Try
                Dim modulecontrol As New ModuleController
                modulecontrol.DeleteTabModuleSetting(Me.TabModuleId, "NewsModuleId")
                modulecontrol.DeleteTabModuleSetting(Me.TabModuleId, "NewsModuleTab")
                modulecontrol.DeleteTabModuleSetting(Me.TabModuleId, "TreeSkin")
                modulecontrol.DeleteTabModuleSetting(Me.TabModuleId, "IncludeRoot")
                modulecontrol.DeleteTabModuleSetting(Me.TabModuleId, "ShowLines")
            Catch
            End Try

            'select show from...
            Try
                Me.cboModules.Items.FindByValue(Me.NewsModuleTab.ToString & ";" & Me.NewsModuleId.ToString).Selected = True
            Catch
            End Try


            Try

                Me.drpSkin.SelectedValue = Me.TreeSkinInCategories
                Me.chkIncludeRoot.Checked = Me.IncludeRootInCategories
                Me.chkShowLines.Checked = Me.ShowLinesInCategories
                Me.chkShowFolderIcon.Checked = Me.ShowFolderIconsInCategories
                Me.chkShowNumbers.Checked = Me.ShowArticleCount
                Me.chkShowFutureItems.Checked = Me.ShowFutureItems
                Me.chkShowPastItems.Checked = Me.ShowPastItems
                Me.chkIncludeExpired.Checked = Me.IncludeExpired
            Catch
            End Try

            BindTitleBoxes()

        End Sub

        Private Sub SaveSettings()

            Dim modulecontrol As New ModuleController

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ModuleView", "AuthorBrowser")

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleTab", Me.cboModules.SelectedValue.Split(Char.Parse(";"))(0))
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleId", Me.cboModules.SelectedValue.Split(Char.Parse(";"))(1))

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "TreeSkin", Me.drpSkin.SelectedValue)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "IncludeRoot", Me.chkIncludeRoot.Checked)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowLines", Me.chkShowLines.Checked)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowFolderIcons", Me.chkShowFolderIcon.Checked)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowArticleCount", Me.chkShowNumbers.Checked)

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowFutureItems", Me.chkShowFutureItems.Checked)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowPastItems", Me.chkShowPastItems.Checked)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "IncludeExpired", Me.chkIncludeExpired.Checked)

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

            BindSkins()

            BuildModuleTitleBoxes(Me.plhModultitle)

            BindModules(cboModules, False)

            If cboModules.Items.Count > 0 Then
                LoadSettings()
            Else
                Me.Controls.Clear()
                Me.Controls.Add(New LiteralControl("<div style=""margin:20px;padding:20px;border:1px dashed #ccc;background:yellow;font-weight:bold;"">In order to work with the Author Browser, you first have to create an article listing somewhere in your portal to connect to.</div>"))
            End If


        End Sub

        Private Sub BindSkins()
            drpSkin.Items.Clear()
            For Each Skin As String In UiSkins
                drpSkin.Items.Add(New ListItem(Skin, Skin))
            Next
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
