Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports System.IO

Imports Telerik.Web.UI
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Controllers

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_ListSettings
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

        Private Sub LoadSettings(ByVal RebindModule As Boolean)

            chkAnonymousComments.Checked = AllowCommentsForAnnonymous
            chkAutoApproveAnonymousComments.Checked = AutoApproveAnonymousComments
            chkAutoApproveAuthenticatedComments.Checked = AutoApproveAuthenticatedtComments

            If RebindModule Then
                Try
                    If NewsModuleId <> ModuleId Then
                        Me.cboModules.Items.FindByValue(Me.NewsModuleTab.ToString & ";" & Me.NewsModuleId.ToString).Selected = True
                    End If
                Catch
                End Try
            End If


            Dim ShowOnlySatellite As Boolean = (cboModules.SelectedValue <> "")

            If ShowOnlySatellite Then 'we are in satellite mode

                Me.rowSearch.Visible = False
                Me.rowEnableSummary.Visible = False
                Me.rowAllowSubscriptions.Visible = False
                Me.rowOnlyRegisteredUsers.Visible = False
                Me.rowNotificationDebug.Visible = False
                Me.rowNotifyAdmin.Visible = False
                Me.rowModerate.Visible = False
                Me.rowAllowTwitter.Visible = False
                Me.rowAnonymousComments.Visible = False
                Me.rowAutoApproveAnonymousComments.Visible = False
                Me.rowAutoApproveAuthenticatedComments.Visible = False

                Me.rowJournal.Visible = False

            Else

                Me.rowJournal.Visible = JournalIntegration.JournalSupported

                Me.rowSearch.Visible = True
                Me.chkEnableDnnSearch.Checked = Me.EnableDnnSearch
                Me.chkEnableModuleSearch.Checked = Me.EnableModuleSearch

                Me.rowEnableSummary.Visible = True
                Me.chkEnableSummary.Checked = Me.EnableSummary

                rowModerate.Visible = True
                If Me.IsModerated Then

                    Me.chkModerateNews.Checked = True

                    'bind notification email
                    Me.rowModeratorRole.Visible = True
                    drpModeratorRole.SelectedValue = ModeratorRole

                    'bind authorized roles
                    Me.rowAuthorizedRoles.Visible = True
                    'Me.BindAuthorizedRoles()
                    If AuthorizedRoles.Contains(",") Then
                        For Each role As String In AuthorizedRoles.Split(Char.Parse(","))
                            Try
                                Me.chkAuthorizedRoles.Items.FindByValue(role).Selected = True
                            Catch
                            End Try
                        Next
                    End If

                Else
                    Me.chkModerateNews.Checked = False
                    Me.rowModeratorRole.Visible = False
                    Me.rowAuthorizedRoles.Visible = False
                End If

                Me.rowAllowSubscriptions.Visible = False
                Me.rowAutoSubscribedRoles.Visible = False
                Me.rowOnlyRegisteredUsers.Visible = False
                Me.rowNotificationDebug.Visible = False
                Me.rowNotifyAdmin.Visible = False
                Me.rowAllowTwitter.Visible = False

                Me.rowAllowSubscriptions.Visible = True
                Me.rowAllowTwitter.Visible = True
                Me.chkAllowTwitter.Checked = AllowTwitterPosts
                Me.chkAddToJournal.Checked = AllowJournalUpdates

                If Me.AllowSubscriptions Then

                    Me.chkAllowSubscriptions.Checked = True

                    Me.rowOnlyRegisteredUsers.Visible = True
                    Me.chkOnlyRegisteredUsers.Checked = Me.OnlyRegisteredUsersCanSubscribe

                    Me.rowNotificationDebug.Visible = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Host, "EDIT", Me.ModuleConfiguration)
                    Me.chkNotificationDebug.Checked = Me.NotificationDebugEnabled

                    Me.rowNotifyAdmin.Visible = True
                    Me.chkNotifyAdmin.Checked = Me.NotifyAdmin

                    Me.rowFromAddress.Visible = True
                    Me.txtFromAddress.Text = Me.FromAddress

                    Me.rowAutoSubscribedRoles.Visible = True
                    If SubscribedRoles.Contains(",") Then
                        For Each role As String In SubscribedRoles.Split(Char.Parse(","))
                            Try
                                Me.chkAutoSubscribeRoles.Items.FindByValue(role).Selected = True
                            Catch
                            End Try
                        Next
                    End If

                Else

                    Me.chkAllowSubscriptions.Checked = False
                    Me.rowAutoSubscribedRoles.Visible = False
                    Me.rowOnlyRegisteredUsers.Visible = False
                    Me.rowNotificationDebug.Visible = False
                    Me.rowNotifyAdmin.Visible = False
                    Me.rowFromAddress.Visible = False

                End If

            End If

            'bind categories
            BindCategories()
            If CategoryList <> "" Then

                If CategoryList = "-1" Then
                    For Each node As RadTreeNode In Me.treeCategories.GetAllNodes
                        If node.Value = "-1" Then
                            node.Checked = True
                            treeCategories.Nodes(0).ExpandChildNodes()
                        End If
                    Next
                Else

                    Dim arrCategories As String() = CategoryList.Split(Char.Parse(";"))

                    For Each node As RadTreeNode In Me.treeCategories.GetAllNodes
                        If node.Value <> "-1" Then

                            For Each strCategory As String In arrCategories
                                If strCategory = node.Value Then
                                    node.Checked = True
                                    treeCategories.Nodes(0).ExpandChildNodes()
                                    Exit For
                                End If
                            Next

                        End If
                    Next
                End If

            End If

            Me.rowShowFutureItems.Visible = True
            Me.rowIncludeNonPublications.Visible = True
            Me.rowIncludePublications.Visible = True
            Me.rowShowPastItems.Visible = True
            Me.rowShowFeatured.Visible = True
            Me.rowShowNonFeatured.Visible = True
            Me.rowMakeFeaturedSticky.Visible = True

            Me.chkEnforceEditPermissions.Checked = Me.EnforceEditPermissions
            Me.chkIncludePublications.Checked = Me.IncludePublications
            Me.chkIncludeNonPublications.Checked = Me.IncludeNonPublications
            Me.chkShowFutureItems.Checked = Me.ShowFutureItems
            Me.chkShowPastItems.Checked = Me.ShowPastItems
            Me.chkShowFeatured.Checked = Me.IncludeFeaturedItems
            Me.chkShowNonFeatured.Checked = Me.IncludeNonFeaturedItems
            Me.chkMakeFeaturedSticky.Checked = Me.MakeFeaturedSticky

            Me.rowSortOrder.Visible = True
            If Me.SortOrder = ArticleSortOrder.publishdateasc Then
                Me.rblSortOrder.SelectedValue = "0"
            ElseIf Me.SortOrder = ArticleSortOrder.publishdatedesc Then
                Me.rblSortOrder.SelectedValue = "1"
            ElseIf Me.SortOrder = ArticleSortOrder.authorasc Then
                Me.rblSortOrder.SelectedValue = "2"
            ElseIf Me.SortOrder = ArticleSortOrder.authordesc Then
                Me.rblSortOrder.SelectedValue = "3"
            ElseIf Me.SortOrder = ArticleSortOrder.titleasc Then
                Me.rblSortOrder.SelectedValue = "4"
            ElseIf Me.SortOrder = ArticleSortOrder.titledesc Then
                Me.rblSortOrder.SelectedValue = "5"
            End If

            Me.chkUseOriginalVersion.Checked = UseOriginalVersion

            Me.rowPaging.Visible = True
            Me.chkEnablePaging.Checked = EnablePaging
            Me.rowPagingMaxCount.Visible = EnablePaging
            If EnablePaging Then
                Me.txtPagingMaxCount.Value = PagingMaxCount
            End If

            Me.drpThemes.SelectedValue = Me.ArticleTheme
            Me.txtRowCount.Value = Me.RowCount.ToString




            'load module titles
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

            BindTitleBoxes()

        End Sub

        Private Sub SaveSettings()

            ClearArticleCache()

            Dim modulecontrol As New ModuleController

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "EnforceEditPermissions", chkEnforceEditPermissions.Checked.ToString)

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "AllowCommentsForAnnonymous", chkAnonymousComments.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "AutoApproveAnonymousComments", chkAutoApproveAnonymousComments.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "AutoApproveAuthenticatedtComments", chkAutoApproveAuthenticatedComments.Checked.ToString)

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ModuleView", "ArticleList")
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "RowCount", Me.txtRowCount.Text)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ArticleTheme", Me.drpThemes.SelectedValue)

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "AllowJournalUpdates", chkAddToJournal.Checked)

            Try
                If cboModules.SelectedValue.Contains(";") Then
                    modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleTab", Me.cboModules.SelectedValue.Split(Char.Parse(";"))(0))
                    modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleId", Me.cboModules.SelectedValue.Split(Char.Parse(";"))(1))
                Else
                    modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleTab", TabId.ToString)
                    modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleId", ModuleId.ToString)
                End If
            Catch ex As Exception
                modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleTab", TabId.ToString)
                modulecontrol.UpdateModuleSetting(Me.ModuleId, "NewsModuleId", ModuleId.ToString)
            End Try



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

            Dim blnShowFromOther As Boolean = (cboModules.SelectedValue <> "")

            If Not blnShowFromOther Then

                modulecontrol.UpdateModuleSetting(ModuleId, "IsModerated", Me.chkModerateNews.Checked.ToString)
                If Me.chkModerateNews.Checked Then
                    modulecontrol.UpdateModuleSetting(ModuleId, "NUNTIO_MODERATORROLE", Me.drpModeratorRole.SelectedValue)
                    Dim saRoles As String = ""
                    For Each item As ListItem In Me.chkAuthorizedRoles.Items
                        If item.Selected Then
                            saRoles = saRoles & item.Value & ","
                        End If
                    Next
                    If saRoles <> "" Then
                        modulecontrol.UpdateModuleSetting(ModuleId, "PNCNEWS_AUTHORIZEDROLES", saRoles)
                    Else
                        modulecontrol.DeleteModuleSetting(ModuleId, "PNCNEWS_AUTHORIZEDROLES")
                    End If
                Else
                    modulecontrol.DeleteModuleSetting(ModuleId, "PNCNEWS_NOTIFYADDRESS")
                    modulecontrol.DeleteModuleSetting(ModuleId, "PNCNEWS_AUTHORIZEDROLES")
                End If

                'own instance, update all settings
                modulecontrol.UpdateModuleSetting(Me.ModuleId, "EnableModuleSearch", Me.chkEnableModuleSearch.Checked.ToString)
                modulecontrol.UpdateModuleSetting(Me.ModuleId, "EnableDnnSearch", Me.chkEnableDnnSearch.Checked.ToString)
                modulecontrol.UpdateModuleSetting(Me.ModuleId, "EnableSummary", Me.chkEnableSummary.Checked.ToString)

                modulecontrol.UpdateModuleSetting(Me.ModuleId, "AllowSubscriptions", Me.chkAllowSubscriptions.Checked.ToString)
                If Me.chkAllowSubscriptions.Checked Then
                    ManageSchedulerJob("Start")
                    modulecontrol.UpdateModuleSetting(Me.ModuleId, "OnlyRegisteredUsersCanSubscribe", Me.chkOnlyRegisteredUsers.Checked.ToString)
                    modulecontrol.UpdateModuleSetting(Me.ModuleId, "NotifyAdmin", Me.chkNotifyAdmin.Checked.ToString)
                    modulecontrol.UpdateModuleSetting(Me.ModuleId, "FromAddress", Me.txtFromAddress.Text)
                Else
                    ManageSchedulerJob("Stop")
                    modulecontrol.DeleteModuleSetting(Me.ModuleId, "OnlyRegisteredUsersCanSubscribe")
                    modulecontrol.DeleteModuleSetting(Me.ModuleId, "NotifyAdmin")
                    modulecontrol.DeleteModuleSetting(Me.ModuleId, "ShowRegister")
                    modulecontrol.DeleteModuleSetting(Me.ModuleId, "FromAddress")
                End If

                If Me.chkNotificationDebug.Checked Then
                    Try
                        System.IO.File.Create(Server.MapPath("/Desktopmodules/Nuntio.Articles/NotificationDebug.txt"))
                    Catch
                    End Try
                Else
                    Try
                        System.IO.File.Delete(Server.MapPath("/Desktopmodules/Nuntio.Articles/NotificationDebug.txt"))
                    Catch
                    End Try
                End If
                Dim sRoles As String = ""
                For Each item As ListItem In Me.chkAutoSubscribeRoles.Items
                    If item.Selected Then
                        sRoles = sRoles & item.Value & ","
                    End If
                Next
                If sRoles <> "" Then
                    modulecontrol.UpdateModuleSetting(Me.ModuleId, "SubscribedRoles", sRoles)
                Else
                    modulecontrol.DeleteModuleSetting(Me.ModuleId, "SubscribedRoles")
                End If

            End If

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "EnablePaging", Me.chkEnablePaging.Checked)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "PagingMaxCount", Me.txtPagingMaxCount.Text.Trim)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "AllowTwitterPosts", Me.chkAllowTwitter.Checked)

            modulecontrol.UpdateModuleSetting(Me.ModuleId, "UseOriginalVersion", Me.chkUseOriginalVersion.Checked)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "SortOrder", Me.rblSortOrder.SelectedValue.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowFutureItems", Me.chkShowFutureItems.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "IncludePublications", Me.chkIncludePublications.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "IncludeNonPublications", Me.chkIncludeNonPublications.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "ShowPastItems", Me.chkShowPastItems.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "IncludeFeaturedItems", Me.chkShowFeatured.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "IncludeNonFeaturedItems", Me.chkShowNonFeatured.Checked.ToString)
            modulecontrol.UpdateModuleSetting(Me.ModuleId, "MakeFeaturedSticky", Me.chkMakeFeaturedSticky.Checked.ToString)

            UpdateModuletitle()
            ClearArticleCache()

        End Sub

        Private Sub BindAutosubscribeRoles()

            Dim objRole As New RoleController
            Dim Role As RoleInfo

            Dim roles As ArrayList = objRole.GetPortalRoles(PortalId)

            If Not roles Is Nothing Then
                For Each Role In roles
                    Dim item As New ListItem
                    item.Text = CType(Role.RoleName, String)
                    item.Value = CType(Role.RoleName, String)
                    chkAutoSubscribeRoles.Items.Add(item)
                Next
            End If

        End Sub

        Private Sub BindModeratorRoles()

            Me.drpModeratorRole.Items.Clear()

            Dim objRole As New RoleController
            Dim Role As RoleInfo

            Dim roles As ArrayList = objRole.GetPortalRoles(PortalId)

            If Not roles Is Nothing Then
                For Each Role In roles
                    Dim item As New ListItem
                    item.Text = CType(Role.RoleName, String)
                    item.Value = CType(Role.RoleName, String)
                    Me.drpModeratorRole.Items.Add(item)
                Next
            End If

        End Sub

        Private Sub BindAuthorizedRoles()

            Dim objRole As New RoleController
            Dim Role As RoleInfo

            Dim roles As ArrayList = objRole.GetPortalRoles(PortalId)

            If Not roles Is Nothing Then
                For Each Role In roles
                    Dim item As New ListItem
                    item.Text = CType(Role.RoleName, String)
                    item.Value = CType(Role.RoleName, String)
                    Me.chkAuthorizedRoles.Items.Add(item)
                Next
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

        Private Sub BuildModuleTitleBoxes()


            Dim _openingTable As New Literal
            _openingTable.Text = "<table>"
            plhModultitle.Controls.Add(_openingTable)

            Dim i As Integer
            For i = 0 To SupportedLocales.Count - 1
                Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(CType(SupportedLocales(i).Value, Locale).Code)

                Dim _BoxRowOpen As New Literal
                _BoxRowOpen.Text = "<tr><td align=""left""><span class=""Normal"">" & System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.DisplayName) & "</span></td><td>&nbsp;</td><td align=""left"">"
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

        Private Sub BindCategories()

            Dim inNewsModule As Integer = NewsModuleId

            If Me.cboModules.SelectedIndex > 0 Then
                inNewsModule = Integer.Parse(Me.cboModules.SelectedValue.Split(Char.Parse(";"))(1))
            End If

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

#End Region

#Region "Event Handlers"

        Public Overrides Sub LoadForm()
            Try

                BuildModuleTitleBoxes()

                BindThemes()
                BindAutosubscribeRoles()
                BindAuthorizedRoles()
                BindModeratorRoles()
                BindModules(cboModules, True)

                LoadSettings(True)

                Me.chkEnableDnnSearch.Text = Localize("DnnSearch.Text")
                Me.chkEnableModuleSearch.Text = Localize("ModuleSearch.Text")

                lblVersion.Text = "Nuntio.Articles, built " & System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString()

                Me.rblSortOrder.Items(0).Text = Localize("publishdateasc")
                Me.rblSortOrder.Items(1).Text = Localize("publishdatedesc")
                Me.rblSortOrder.Items(2).Text = Localize("authorasc")
                Me.rblSortOrder.Items(3).Text = Localize("authordesc")
                Me.rblSortOrder.Items(4).Text = Localize("titleasc")
                Me.rblSortOrder.Items(5).Text = Localize("titledesc")

                Me.cboModules.Items(0).Text = Localization.GetString("thismodule", Me.LocalResourceFile)

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub chkModerateLinks_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkModerateNews.CheckedChanged
            Me.rowAuthorizedRoles.Visible = Me.chkModerateNews.Checked
            Me.rowModeratorRole.Visible = Me.chkModerateNews.Checked
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

        Protected Sub cboModules_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboModules.SelectedIndexChanged
            LoadSettings(False)
        End Sub

        Protected Sub chkEnablePaging_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkEnablePaging.CheckedChanged
            Me.rowPagingMaxCount.Visible = Me.chkEnablePaging.Checked
        End Sub

        Protected Sub chkAllowSubscriptions_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAllowSubscriptions.CheckedChanged
            Me.rowOnlyRegisteredUsers.Visible = Me.chkAllowSubscriptions.Checked
            Me.rowNotificationDebug.Visible = Me.chkAllowSubscriptions.Checked
            Me.rowNotifyAdmin.Visible = Me.chkAllowSubscriptions.Checked
            Me.rowFromAddress.Visible = False
            Me.rowAutoSubscribedRoles.Visible = Me.chkAllowSubscriptions.Checked
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

            'BuildModuleTitleBoxes(Me.plhModultitle)

        End Sub

#End Region

#Region "Private Helper Functions"

        Private Function CanDisableJob() As Boolean

            Dim objDesktopModuleInfo As DotNetNuke.Entities.Modules.DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName("Nuntio Articles", PortalId)
            If Not (objDesktopModuleInfo Is Nothing) Then
                Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
                Dim arrModules As ArrayList = objModules.GetAllModules()
                For Each objModule As DotNetNuke.Entities.Modules.ModuleInfo In arrModules
                    If (objModule.DesktopModuleID = objDesktopModuleInfo.DesktopModuleID) Then
                        Dim modulecontrol As New ModuleController
                        Dim settings As Hashtable
                        settings = modulecontrol.GetModuleSettings(objModule.ModuleID)
                        If Not settings Is Nothing Then
                            If settings.Contains("AllowSubscriptions") Then
                                If CType(settings("AllowSubscriptions"), Boolean) = True Then
                                    Return False
                                End If
                            End If
                        End If
                    End If
                Next
            End If
            Return True
        End Function

        Private Sub SetSchedulerMode()

            Dim OriginalSchedulerMode As Services.Scheduling.SchedulerMode
            OriginalSchedulerMode = DotNetNuke.Entities.Host.Host.SchedulerMode

            If OriginalSchedulerMode = Services.Scheduling.SchedulerMode.DISABLED Or OriginalSchedulerMode = Services.Scheduling.SchedulerMode.REQUEST_METHOD Then


                HostController.Instance.Update("SchedulerMode", Convert.ToString(Services.Scheduling.SchedulerMode.TIMER_METHOD))
                DataCache.ClearHostCache(True)

                Dim newThread As New Threading.Thread(AddressOf Services.Scheduling.SchedulingProvider.Instance.Start)
                newThread.IsBackground = True
                newThread.Start()

            End If


        End Sub

        Private Sub ManageSchedulerJob(ByVal sMode As String)

            Dim objScheduleItem1 As DotNetNuke.Services.Scheduling.ScheduleItem
            Dim objScheduleItem2 As DotNetNuke.Services.Scheduling.ScheduleItem

            Try
                objScheduleItem1 = DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.GetSchedule("pnc.Publisher.MagicNews.NewsNotification, pnc.Publisher.MagicNews", Nothing)
                If Not objScheduleItem1 Is Nothing Then
                    DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.DeleteSchedule(objScheduleItem1)
                End If
            Catch
            End Try

            objScheduleItem1 = DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.GetSchedule("dnnWerk.Modules.Nuntio.Articles.ArticleNotification, dnnWerk.Nuntio.Articles", Nothing)

            If Not objScheduleItem1 Is Nothing Then
                Select Case sMode
                    Case "Start"

                        SetSchedulerMode()

                        objScheduleItem1.Enabled = True
                        UpdateSchedulerSettings(objScheduleItem1.ScheduleID)

                        DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.UpdateSchedule(objScheduleItem1)
                        DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.ReStart("Nuntio Article Collection Scheduler Job Started.")

                    Case "Stop"
                        If CanDisableJob() Then
                            objScheduleItem1.Enabled = False
                            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.UpdateSchedule(objScheduleItem1)
                            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.ReStart("Nuntio Article Collection Scheduler Job Stopped.")
                        End If
                End Select
            Else
                If sMode = "Start" Then

                    SetSchedulerMode()

                    objScheduleItem1 = New DotNetNuke.Services.Scheduling.ScheduleItem
                    objScheduleItem1.TypeFullName = "dnnWerk.Modules.Nuntio.Articles.ArticleNotification, dnnWerk.Nuntio.Articles"
                    objScheduleItem1.TimeLapse = 1
                    objScheduleItem1.TimeLapseMeasurement = "h"
                    objScheduleItem1.RetryTimeLapse = 30
                    objScheduleItem1.RetryTimeLapseMeasurement = "m"
                    objScheduleItem1.RetainHistoryNum = 10
                    objScheduleItem1.AttachToEvent = ""
                    objScheduleItem1.CatchUpEnabled = False
                    objScheduleItem1.Enabled = True
                    objScheduleItem1.ObjectDependencies = ""
                    objScheduleItem1.Servers = ""

                    objScheduleItem1.ScheduleID = DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddSchedule(objScheduleItem1)

                    UpdateSchedulerSettings(objScheduleItem1.ScheduleID)


                    If DotNetNuke.Services.Scheduling.SchedulingProvider.SchedulerMode = DotNetNuke.Services.Scheduling.SchedulerMode.TIMER_METHOD Then
                        DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.ReStart("Nuntio Article Collection Scheduler Job Added.")
                    End If

                End If
            End If


            objScheduleItem2 = DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.GetSchedule("dnnWerk.Modules.Nuntio.Articles.EmailQueueProcessor, dnnWerk.Nuntio.Articles", Nothing)

            If Not objScheduleItem2 Is Nothing Then
                Select Case sMode
                    Case "Start"

                        SetSchedulerMode()

                        objScheduleItem2.Enabled = True
                        DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.UpdateSchedule(objScheduleItem2)
                        DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.ReStart("Nuntio Email Queue Processor Scheduler Job Started.")

                    Case "Stop"
                        If CanDisableJob() Then
                            objScheduleItem2.Enabled = False
                            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.UpdateSchedule(objScheduleItem2)
                            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.ReStart("Nuntio Email Queue Processor Scheduler Job Stopped.")
                        End If
                End Select
            Else
                If sMode = "Start" Then

                    SetSchedulerMode()

                    objScheduleItem2 = New DotNetNuke.Services.Scheduling.ScheduleItem
                    objScheduleItem2.TypeFullName = "dnnWerk.Modules.Nuntio.Articles.EmailQueueProcessor, dnnWerk.Nuntio.Articles"
                    objScheduleItem2.TimeLapse = 45
                    objScheduleItem2.TimeLapseMeasurement = "m"
                    objScheduleItem2.RetryTimeLapse = 15
                    objScheduleItem2.RetryTimeLapseMeasurement = "m"
                    objScheduleItem2.RetainHistoryNum = 10
                    objScheduleItem2.AttachToEvent = ""
                    objScheduleItem2.CatchUpEnabled = False
                    objScheduleItem2.Enabled = True
                    objScheduleItem2.ObjectDependencies = ""
                    objScheduleItem2.Servers = ""
                    DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddSchedule(objScheduleItem2)

                    If DotNetNuke.Services.Scheduling.SchedulingProvider.SchedulerMode = DotNetNuke.Services.Scheduling.SchedulerMode.TIMER_METHOD Then
                        DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.ReStart("Nuntio Email Queue Processor Scheduler Job Added.")
                    End If

                End If
            End If

        End Sub

        Private Sub UpdateSchedulerSettings(ByVal SchedulingId)

            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_HomeDirectory_" & Me.ModuleId.ToString, PortalSettings.HomeDirectory)
            If Me.txtFromAddress.Text <> "" Then
                DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_Email_" & Me.ModuleId.ToString, Me.txtFromAddress.Text)
            Else
                DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_Email_" & Me.ModuleId.ToString, PortalSettings.Email)
            End If
            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_DefaultLanguage_" & Me.ModuleId.ToString, PortalSettings.DefaultLanguage)
            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_Portalname_" & Me.ModuleId.ToString, PortalSettings.PortalName)
            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_TabId_" & Me.ModuleId.ToString, Me.TabId.ToString)
            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_PortalId_" & Me.ModuleId.ToString, PortalSettings.PortalId.ToString)
            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_PortalAlias_" & Me.ModuleId.ToString, PortalSettings.PortalAlias.HTTPAlias)
            DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_UseOriginalVersion_" & Me.ModuleId.ToString, chkUseOriginalVersion.Checked.ToString)
            Dim sRoles As String = ""
            For Each item As ListItem In Me.chkAutoSubscribeRoles.Items
                If item.Selected Then
                    sRoles = sRoles & item.Value & ","
                End If
            Next
            If sRoles <> "" Then
                DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_SubscribedRoles_" & Me.ModuleId.ToString, sRoles)
            Else
                DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_SubscribedRoles_" & Me.ModuleId.ToString, "")
            End If

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
                    DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_MODULETITLE_" & Convert.ToString(info.Name).ToUpper & "_" & Me.ModuleId.ToString, txtBox.Text)
                Else
                    DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.AddScheduleItemSetting(SchedulingId, "NUNTIO_MODULETITLE_" & Convert.ToString(info.Name).ToUpper & "_" & Me.ModuleId.ToString, Me.ModuleConfiguration.ModuleTitle)
                End If
            Next


        End Sub

#End Region

    End Class
End Namespace
