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
    Partial Public Class uc_RotateSettings
        Inherits SettingsBase
        Implements IActionable

#Region "Private Methods"

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

        Private Sub BindScrollDirections()
            Me.drpScrollDirection.Items.Clear()
            Me.drpScrollDirection.Items.Add(New ListItem(Services.Localization.Localization.GetString("ScrollUp", Me.LocalResourceFile), "4"))
            Me.drpScrollDirection.Items.Add(New ListItem(Services.Localization.Localization.GetString("ScrollDown", Me.LocalResourceFile), "8"))
            Me.drpScrollDirection.Items.Add(New ListItem(Services.Localization.Localization.GetString("ScrollLeft", Me.LocalResourceFile), "1"))
            Me.drpScrollDirection.Items.Add(New ListItem(Services.Localization.Localization.GetString("ScrollRight", Me.LocalResourceFile), "2"))
        End Sub

        Private Sub LoadSettings()

            'select show from...
            Try
                Me.cboModules.Items.FindByValue(Me.NewsModuleTab.ToString & ";" & Me.NewsModuleId.ToString).Selected = True
            Catch
            End Try

            'bind categories
            BindCategories()
            If CategoryList <> "" Then
                For Each node As RadTreeNode In Me.treeCategories.GetAllNodes
                    If node.Value <> "-1" Then
                        If CategoryList.Contains(Integer.Parse(node.Value)) Then
                            node.Checked = True
                            treeCategories.Nodes(0).ExpandChildNodes()
                        End If
                    End If
                Next
            End If

            Me.rowSortOrder.Visible = True
            Me.rowShowFutureItems.Visible = True

            If Me.SortOrder = ArticleSortOrder.publishdateasc Then
                Me.rblSortOrder.SelectedValue = "0"
            ElseIf Me.SortOrder = ArticleSortOrder.publishdatedesc Then
                Me.rblSortOrder.SelectedValue = "1"
            ElseIf Me.SortOrder = ArticleSortOrder.authorasc Then
                Me.rblSortOrder.SelectedValue = "2"
            ElseIf Me.SortOrder = ArticleSortOrder.authordesc Then
                Me.rblSortOrder.SelectedValue = "3"
            End If

            Me.chkShowFutureItems.Checked = Me.ShowFutureItems
            Me.rowShowFutureItems.Visible = True
            Me.chkShowPastItems.Checked = Me.ShowPastItems
            Me.chkUseOriginalVersion.Checked = Me.UseOriginalVersion


            Me.drpThemes.SelectedValue = Me.ArticleTheme
            Me.txtRowCount.Value = Me.RowCount.ToString

            Me.txtScrollHeight.Text = ScrollHeight
            Me.txtScrollSpeed.Text = ScrollSpeed
            Me.txtScrollTimeout.Text = ScrollTimeOut
            Me.txtScrollWidth.Text = ScrollWidth
            Me.drpScrollDirection.SelectedValue = ScrollDirection

            BindTitleBoxes()

        End Sub

        Private Sub SaveSettings()

            Dim modulecontrol As New ModuleController

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ModuleView", "ArticleRotator")

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ScrollSpeed", Me.txtScrollSpeed.Text)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ScrollTimeOut", Me.txtScrollTimeout.Text)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ScrollHeight", Me.txtScrollHeight.Text)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ScrollWidth", Me.txtScrollWidth.Text)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ScrollDirection", Me.drpScrollDirection.SelectedValue)

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "RowCount", Me.txtRowCount.Text)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ArticleTheme", Me.drpThemes.SelectedValue)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleTab", Me.cboModules.SelectedValue.Split(Char.Parse(";"))(0))
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleId", Me.cboModules.SelectedValue.Split(Char.Parse(";"))(1))

            Dim categories As String = ""
            For Each node As RadTreeNode In Me.treeCategories.CheckedNodes
                If node.Value <> "" Then
                    If node.Value = "-1" Then
                        categories = "-1"
                        Exit For
                    End If
                    categories += node.Value & ";"
                End If
            Next

            If categories <> "" Then
                If categories.EndsWith(";") Then
                    categories = categories.Substring(0, categories.Length - 1)
                End If
                modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowCategory", categories)
            Else
                modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowCategory", "-1")
            End If

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "SortOrder", Me.rblSortOrder.SelectedValue.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowFutureItems", Me.chkShowFutureItems.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowPastItems", Me.chkShowPastItems.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "UseOriginalVersion", Me.chkUseOriginalVersion.Checked)

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

        Private Sub BuildModuleTitleBoxes()


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
                _txtBox.Width = System.Web.UI.WebControls.Unit.Pixel(250)
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

        Private Sub BindCategories()

            Dim inNewsModule As Integer = NewsModuleId
            If Not Me.cboModules.SelectedValue Is Nothing Then
                If Me.cboModules.SelectedValue <> "" Then

                    inNewsModule = Integer.Parse(Me.cboModules.SelectedValue.Split(Char.Parse(";"))(1))

                    Me.treeCategories.Nodes.Clear()

                    Dim node As New RadTreeNode(Localize("AllCategories"), "-1")

                    Me.treeCategories.Nodes.Add(node)
                    Me.treeCategories.Nodes(0).Expanded = False
                    Me.treeCategories.Nodes(0).Selected = False
                    Me.treeCategories.Nodes(0).Checked = False
                    Me.treeCategories.Nodes(0).Checkable = True

                    Dim Categories As New List(Of CategoryInfo)
                    Dim cc As New CategoryController
                    Categories = cc.ListCategoryItems(inNewsModule, Me.CurrentLocale, Date.Now, True, True, True)
                    BindNodes(Me.treeCategories.Nodes(0), Categories)

                    Me.treeCategories.ExpandAllNodes()

                End If
            End If

        End Sub

        Private Sub BindNodes(ByRef ParentNode As RadTreeNode, ByVal Categories As List(Of CategoryInfo))
            For Each Category As CategoryInfo In Categories

                If Category.ParentID = Null.NullInteger Then
                    If ParentNode.Value = Null.NullInteger.ToString Then
                        Dim NewNode As RadTreeNode = New RadTreeNode(Category.CategoryName, Category.CategoryID.ToString)
                        ParentNode.Nodes.Add(NewNode)
                        BindNodes(NewNode, Categories)
                    End If
                Else
                    If ParentNode.Value <> "" Then
                        If Category.ParentID = Integer.Parse(ParentNode.Value) Then
                            Dim NewNode As RadTreeNode = New RadTreeNode(Category.CategoryName, Category.CategoryID.ToString)
                            ParentNode.Nodes.Add(NewNode)
                            BindNodes(NewNode, Categories)
                        End If
                    End If
                End If
            Next
        End Sub

#End Region

#Region "Event Handlers"

        Public Overrides Sub LoadForm()

            BuildModuleTitleBoxes()

            BindThemes()
            BindModules(cboModules, True)
            BindScrollDirections()
            LoadSettings()

            lblVersion.Text = "Nuntio.Articles, built " & System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString()

            Me.rblSortOrder.Items(0).Text = Localize("asc")
            Me.rblSortOrder.Items(1).Text = Localize("desc")

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

#Region "Optional Interfaces"

        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Localize("Back"), ModuleActionType.AddContent, "", "lt.gif", NavigateURL(TabId), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property

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


#Region "Private Helper Functions"


#End Region

        Private Sub cboModules_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboModules.SelectedIndexChanged
            BindCategories()
        End Sub

    End Class
End Namespace
