
Imports Telerik.Web.UI
Imports System.IO
Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Templates
        Inherits ArticleModuleBase

#Region "Private Members"

        Private Enum ToolbarButton
            SaveButton = 0
            CopyButton = 1
            DeleteButton = 2
            LanguageButton = 3
            CancelButton = 4
            CloseButton = 5
        End Enum

        Private ReadOnly Property TemplateToLoad() As String
            Get
                If Not Request.QueryString("tmpl") Is Nothing Then
                    Return Request.QueryString("tmpl")
                End If
                If Not Request.QueryString("amp;tmpl") Is Nothing Then
                    Return Request.QueryString("amp;tmpl")
                End If
                Return "Default"
            End Get
        End Property

        Private ReadOnly Property PortalToLoad() As String
            Get
                If Not Request.QueryString("PortalId") Is Nothing Then
                    Return Request.QueryString("PortalId")
                End If
                If Not Request.QueryString("amp;PortalId") Is Nothing Then
                    Return Request.QueryString("amp;PortalId")
                End If
                Return Null.NullInteger.ToString
            End Get
        End Property

        Private Property SelectedLocale()
            Get
                If Not ViewState("SelectedLocale") Is Nothing Then
                    Return CType(ViewState("SelectedLocale"), String)
                End If
                Return PortalSettings.DefaultLanguage
            End Get
            Set(ByVal value)
                ViewState("SelectedLocale") = value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        Private Sub SetButton(ByVal Button As ToolbarButton, ByVal IsVisible As Boolean, ByVal IsEnabled As Boolean)
            Me.TemplateTools.Items(Button).Visible = IsVisible
            Me.TemplateTools.Items(Button).Enabled = IsEnabled
        End Sub

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            'DotNetNuke.Framework.AJAX.RegisterScriptManager()
            BindCss()

        End Sub

        Private Sub BindCss()

            Dim strCssUrl As String = Me.ModuleDirectory & "/Css/Edit.css"

            Dim blnAlreadyRegistered As Boolean = False
            For Each ctrl As Control In Me.Page.Header.Controls

                Try
                    Dim ctrlCss As HtmlLink = CType(ctrl, HtmlLink)
                    If ctrlCss.Href.ToLower = strCssUrl.ToLower Then
                        blnAlreadyRegistered = True
                        Exit For
                    End If
                Catch ex As Exception
                    'skip registration
                End Try

            Next

            If Not blnAlreadyRegistered Then

                Dim ctrlLink As New HtmlLink
                ctrlLink.Href = strCssUrl
                ctrlLink.Attributes.Add("rel", "stylesheet")
                ctrlLink.Attributes.Add("type", "text/css")
                ctrlLink.Attributes.Add("media", "screen")

                Me.Page.Header.Controls.Add(ctrlLink)

            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Page.IsPostBack Then

                Me.TemplateTools.Items(ToolbarButton.SaveButton).ImageUrl = Me.ImagesDirectory & "tools_update.gif"
                Me.TemplateTools.Items(ToolbarButton.CopyButton).ImageUrl = Me.ImagesDirectory & "tools_copy.gif"
                Me.TemplateTools.Items(ToolbarButton.DeleteButton).ImageUrl = Me.ImagesDirectory & "tools_delete.gif"
                Me.TemplateTools.Items(ToolbarButton.CancelButton).ImageUrl = Me.ImagesDirectory & "tools_cancel.gif"
                Me.TemplateTools.Items(ToolbarButton.CloseButton).ImageUrl = Me.ImagesDirectory & "tools_cancel.gif"

                Me.pane_Help.IconUrl = Me.ImagesDirectory & "nuntio_help.gif"
                Me.pane_Tree.IconUrl = Me.ImagesDirectory & "nuntio_file.gif"

                'toolbar
                Me.TemplateTools.Items(0).Text = Localize("SaveFile")
                Me.TemplateTools.Items(1).Text = Localize("CopySelected")
                Me.TemplateTools.Items(2).Text = Localize("DeleteSelected")
                Me.TemplateTools.Items(3).Text = Localize("SelectLanguage")
                Me.TemplateTools.Items(4).Text = Localize("TemplateCancel")
                Me.TemplateTools.Items(5).Text = Localize("TemplateClose")
                lblName.Text = Localize("TemplateName")
                lblMode.Text = Localize("TemplateMode")
                cmdCreate.Text = Localize("TemplateCreate")

                BindLocales()
                BindTree()

                For Each item As RadToolBarItem In TemplateTools.Items
                    item.Enabled = False
                    item.Visible = True
                Next
                SetButton(ToolbarButton.CloseButton, True, True)
                SetButton(ToolbarButton.CancelButton, False, False)

                For Each node As RadTreeNode In tree_Skins.Nodes

                    Dim pid As String = node.Attributes("portalId")
                    Dim val As String = node.Value.ToLower
                    Dim tmpl As String = TemplateToLoad.ToLower

                    If (pid = PortalToLoad And val = tmpl) Or (pid = Null.NullInteger.ToString And val = tmpl) Then
                        node.Selected = True
                        node.Expanded = True
                    End If

                Next

                SetButton(ToolbarButton.CopyButton, True, True)

            End If

            Me.tree_Skins.Nodes(0).AllowEdit = False

        End Sub

        Private Sub BindLocales()

            Dim drpButton As RadToolBarDropDown = CType(Me.TemplateTools.Items(ToolbarButton.LanguageButton), RadToolBarDropDown)
            drpButton.ImageUrl = Me.ImagesDirectory & "tools_locale.gif"
            drpButton.Buttons.Clear()

            Dim intLocales As Integer = SupportedLocales.Count
            If intLocales > 1 Then
                'only show language selector if more than one
                'locale is enabled
                For Each objLocale As Locale In SupportedLocales.Values
                    Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
                    Dim btn As New RadToolBarButton
                    btn.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.DisplayName)
                    btn.CommandName = "ChangeLocale"
                    btn.CommandArgument = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name).ToUpper
                    btn.ImageUrl = Me.ImagesDirectory & "flags/" & info.Name.Substring(info.Name.IndexOf("-") + 1) & ".gif"
                    drpButton.Buttons.Add(btn)
                Next
            Else
                'only one locale exists, hide language selector
                drpButton.Visible = False
            End If
        End Sub

        Private Sub BindTree()

            Me.tree_Skins.Nodes.Clear()
            Me.pnlEditor.Visible = False
            Me.pnlForm.Visible = False
            Me.pnlFiles.Visible = False

            Dim oP As New DotNetNuke.Entities.Portals.PortalController
            Dim hPortal As DotNetNuke.Entities.Portals.PortalInfo = oP.GetPortal(PortalSettings.PortalId)

            'create mail folder in the portal directory if necessary
            'and copy all mail templates there
            Dim mailpath As String = Server.MapPath(Me.ModuleDirectory & "/templates/Portal/")
            If Not System.IO.Directory.Exists(mailpath) Then
                System.IO.Directory.CreateDirectory(mailpath)
            End If
            mailpath += PortalSettings.PortalId.ToString & "\"
            If Not System.IO.Directory.Exists(mailpath) Then
                System.IO.Directory.CreateDirectory(mailpath)
            End If
            mailpath += "Newsletter\"
            If Not System.IO.Directory.Exists(mailpath) Then
                System.IO.Directory.CreateDirectory(mailpath)

                Dim tmpMailPath As String = Server.MapPath(Me.ModuleDirectory & "/templates/Newsletter/")
                For Each File As String In Directory.GetFiles(tmpMailPath)
                    System.IO.File.Copy(File, mailpath & File.Substring(File.LastIndexOf("\") + 1))
                Next

            End If

            CreateTemplateNode("Default", True, hPortal)

            If UserInfo.IsSuperUser Then
                For Each folder As String In Directory.GetDirectories(Server.MapPath(Me.ModuleDirectory & "/templates/"))
                    Dim sFolder As String = folder.Substring(folder.LastIndexOf("\") + 1)
                    If sFolder.ToLower = "portal" Then
                        For Each portalfolder As String In Directory.GetDirectories(folder)
                            Dim pid As String = portalfolder.Substring(portalfolder.LastIndexOf("\") + 1)
                            If IsNumeric(pid) Then
                                Dim oPortal As DotNetNuke.Entities.Portals.PortalInfo = oP.GetPortal(pid)
                                If Not oPortal Is Nothing Then
                                    For Each templatefolder As String In Directory.GetDirectories(portalfolder)
                                        CreateTemplateNode(templatefolder.Substring(templatefolder.LastIndexOf("\") + 1), False, oPortal)
                                    Next
                                End If
                            End If
                        Next
                    Else
                        If Not sFolder.ToLower = "default" And Not sFolder.ToLower = "newsletter" Then
                            CreateTemplateNode(sFolder, True, hPortal)
                        End If
                    End If
                Next
            Else
                Dim portalpath As String = Server.MapPath(Me.ModuleDirectory & "/templates/Portal/" & PortalSettings.PortalId.ToString)
                If Directory.Exists(portalpath) Then
                    For Each folder As String In Directory.GetDirectories(portalpath)
                        CreateTemplateNode(folder.Substring(folder.LastIndexOf("\") + 1), False, hPortal)
                    Next
                End If
            End If

            SetButton(ToolbarButton.DeleteButton, False, False)

        End Sub

        Private Sub CreateTemplateNode(ByVal TemplateName As String, ByVal isHost As Boolean, ByVal Portal As Entities.Portals.PortalInfo)
            Dim rootnode As New RadTreeNode

            rootnode.ImageUrl = Me.ImagesDirectory & "nuntio_folder.gif"
            rootnode.Value = TemplateName
            If isHost Then
                rootnode.Text = "All Portals: " & TemplateName
                rootnode.Attributes.Add("portalId", Null.NullInteger.ToString)
                rootnode.Attributes.Add("absPath", Server.MapPath(Me.ModuleDirectory & "/templates/" & TemplateName & "/"))
                rootnode.Attributes.Add("relPath", Me.ModuleDirectory & "/templates/" & TemplateName & "/")

                Dim imagesnode As New RadTreeNode
                imagesnode.Text = "images"
                imagesnode.ImageUrl = Me.ImagesDirectory & "nuntio_folder.gif"
                imagesnode.Attributes.Add("portalId", Null.NullInteger.ToString)
                imagesnode.Attributes.Add("absPath", Server.MapPath(Me.ModuleDirectory & "/templates/" & TemplateName & "/images/"))
                imagesnode.Attributes.Add("relPath", Me.ModuleDirectory & "/templates/" & TemplateName & "/images")
                rootnode.Nodes.Add(imagesnode)

            Else
                rootnode.Text = Portal.PortalName & ": " & TemplateName
                rootnode.Attributes.Add("portalId", Portal.PortalID.ToString)
                rootnode.Attributes.Add("absPath", Server.MapPath(Me.ModuleDirectory & "/templates/portal/" & PortalSettings.PortalId.ToString & "/" & TemplateName & "/"))
                rootnode.Attributes.Add("relPath", Me.ModuleDirectory & "/templates/portal/" & PortalSettings.PortalId.ToString & "/" & TemplateName & "/")

                Dim imagesnode As New RadTreeNode
                imagesnode.Text = "images"
                imagesnode.ImageUrl = Me.ImagesDirectory & "nuntio_folder.gif"
                imagesnode.Attributes.Add("portalId", Portal.PortalID.ToString)
                imagesnode.Attributes.Add("absPath", Server.MapPath(Me.ModuleDirectory & "/templates/portal/" & PortalSettings.PortalId.ToString & "/" & TemplateName & "/images/"))
                imagesnode.Attributes.Add("relPath", Me.ModuleDirectory & "/templates/portal/" & PortalSettings.PortalId.ToString & "/" & TemplateName & "/images/")
                rootnode.Nodes.Add(imagesnode)

            End If





            Dim templates As String() = {}

            If TemplateName = "Newsletter" Then
                templates = [Enum].GetNames(GetType(Templates.NewsletterTemplate))
            Else
                templates = [Enum].GetNames(GetType(Templates.ArticlesTemplate))
            End If

            For Each enummember As String In templates
                Dim subnode As New RadTreeNode
                subnode.Text = enummember
                subnode.Value = enummember
                Dim path As String = ""
                If isHost Then
                    path = Server.MapPath(Me.ModuleDirectory & "/templates/" & TemplateName & "/" & enummember & ".txt")
                    subnode.Attributes.Add("absPath", path)
                    subnode.Attributes.Add("relPath", Me.ModuleDirectory & "/templates/" & TemplateName & "/" & enummember & ".txt")
                    subnode.Attributes.Add("tmplName", enummember)
                Else
                    path = Server.MapPath(Me.ModuleDirectory & "/templates/portal/" & PortalSettings.PortalId.ToString & "/" & TemplateName & "/" & enummember & ".txt")
                    subnode.Attributes.Add("absPath", path)
                    subnode.Attributes.Add("relPath", Me.ModuleDirectory & "/templates/portal/" & PortalSettings.PortalId.ToString & "/" & TemplateName & "/" & enummember & ".txt")
                    subnode.Attributes.Add("tmplName", enummember)
                End If

                If Not File.Exists(path) Then
                    'copy from default
                    File.Copy(path.ToLower.Replace(TemplateName.ToLower, "default"), path)
                End If

                subnode.ImageUrl = Me.ImagesDirectory & "ext/html.gif"
                rootnode.Nodes.Add(subnode)
            Next

            If TemplateName <> "Newsletter" Then
                Dim cssnode As New RadTreeNode
                cssnode.Text = "Template.css"
                cssnode.Value = "Template.css"
                If isHost Then
                    cssnode.Attributes.Add("absPath", Server.MapPath(Me.ModuleDirectory & "/templates/" & TemplateName & "/Template.css"))
                    cssnode.Attributes.Add("relPath", Me.ModuleDirectory & "/templates/" & TemplateName & "/Template.css")
                    cssnode.Attributes.Add("tmplName", "css")
                Else
                    cssnode.Attributes.Add("absPath", Server.MapPath(Me.ModuleDirectory & "/templates/portal/" & PortalSettings.PortalId.ToString & "/" & TemplateName & "/Template.css"))
                    cssnode.Attributes.Add("relPath", Me.ModuleDirectory & "/templates/portal/" & PortalSettings.PortalId.ToString & "/" & TemplateName & "/Template.css")
                    cssnode.Attributes.Add("tmplName", "css")
                End If
                cssnode.ImageUrl = Me.ImagesDirectory & "ext/css.gif"
                rootnode.Nodes.Add(cssnode)
            End If

            Me.tree_Skins.Nodes.Add(rootnode)

        End Sub

        Private Sub tree_Skins_NodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles tree_Skins.NodeClick

            If e.Node.ParentNode Is Nothing Then
                'is root node
                'BindForm()
                Me.pnlEditor.Visible = False
                Me.pnlForm.Visible = False
                Me.pnlFiles.Visible = False
                SetButton(ToolbarButton.SaveButton, True, False)
                SetButton(ToolbarButton.CopyButton, True, True)
                SetButton(ToolbarButton.LanguageButton, True, False)
                SetButton(ToolbarButton.CancelButton, True, True)
                If Not e.Node.Value.ToLower = "default" AndAlso Not e.Node.Value.ToLower.Contains("newsletter") Then
                    SetButton(ToolbarButton.DeleteButton, True, True)
                Else
                    SetButton(ToolbarButton.DeleteButton, True, False)
                End If
                If e.Node.Value.ToLower.Contains("newsletter") Then
                    SetButton(ToolbarButton.CopyButton, False, False)
                End If
            Else

                If e.Node.Text = "images" Then
                    'is template file
                    BindFolder()
                    Me.pnlFiles.Visible = True
                    Me.pnlEditor.Visible = False
                    Me.pnlForm.Visible = False
                Else
                    'is template file
                    BindFile(SelectedLocale)
                    Me.pnlFiles.Visible = False
                    Me.pnlEditor.Visible = True
                    Me.pnlForm.Visible = False
                End If


            End If

        End Sub

        Private Sub BindFolder()

            SetButton(ToolbarButton.SaveButton, False, False)
            SetButton(ToolbarButton.CopyButton, False, False)
            SetButton(ToolbarButton.DeleteButton, False, False)
            SetButton(ToolbarButton.CancelButton, False, False)


            Dim path As String = Me.tree_Skins.SelectedNode.Attributes("absPath")
            If Not Directory.Exists(path) Then
                Directory.CreateDirectory(path)
            End If

            grdFiles.Rebind()

        End Sub

        Public Function GetFileName(ByVal Path As String) As String
            Return Path.Substring(Path.LastIndexOf("\") + 1)
        End Function

        Public Function GetFileUrl(ByVal Path As String) As String
            Dim url As String = Me.ModuleDirectory & "/templates/"
            Dim imagepath As String = Path.Substring(Path.IndexOf("\templates\") + 11)
            Dim imagerelpath As String = imagepath.Replace("\", "/")
            url += imagerelpath
            Return url
        End Function

        Private Sub BindFile(ByVal Locale As String)

            Dim strMessage As String = ""
            Dim strText As String = ""
            Dim csspath As String = Me.tree_Skins.SelectedNode.ParentNode.Attributes("relPath") & "Template.css"
            If Not File.Exists(Server.MapPath(csspath)) Then
                csspath = Me.ModuleDirectory & "/templates/default/template.css"
            End If
            Dim path As String = Me.tree_Skins.SelectedNode.Attributes("absPath")

            If Not File.Exists(path) Then
                'copy from default
                File.Copy(path.ToLower.Replace(Me.tree_Skins.SelectedNode.ParentNode.Value.ToLower, "default"), path)
            End If

            Dim localepath As String = path.Substring(0, path.LastIndexOf(".") + 1) & Locale & ".txt"

            strMessage += Localize("LocaleVersion") & ": " & Locale

            If Locale.ToLower <> PortalSettings.DefaultLanguage.ToLower Then
                If File.Exists(localepath) = False Then
                    File.Copy(path, localepath)
                End If
                path = localepath
            Else
                strMessage += Localize("PortalDefault")
            End If


            If File.Exists(path) Then
                Dim sr As New StreamReader(path)
                strText = sr.ReadToEnd
                sr.Close()
                sr.Dispose()
                Me.txtTemplate.Text = strText
            Else                                
                strMessage = Localize("CouldNotLoadTemplate")
            End If



            SetButton(ToolbarButton.SaveButton, True, True)
            SetButton(ToolbarButton.CopyButton, False, False)
            SetButton(ToolbarButton.DeleteButton, False, False)
            SetButton(ToolbarButton.CancelButton, False, False)

            If path.ToLower.Contains("template.css") Then
                lblMessage.Visible = False
                SetButton(ToolbarButton.LanguageButton, True, False)
            Else
                lblMessage.Visible = True
                SetButton(ToolbarButton.LanguageButton, True, True)
            End If

            If Me.tree_Skins.SelectedNode.ParentNode.Value.ToLower = "default" Then
                SetButton(ToolbarButton.SaveButton, True, False)
                SetButton(ToolbarButton.LanguageButton, True, False)
                strMessage = Localize("CannotSaveDefault")
            Else
                SetButton(ToolbarButton.SaveButton, True, True)
            End If

            Me.lblMessage.Text = strMessage

            grdTemplateHelp.Rebind()

        End Sub

        Private Sub TemplateTools_ButtonClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadToolBarEventArgs) Handles TemplateTools.ButtonClick
            Dim btn As RadToolBarButton = CType(e.Item, RadToolBarButton)
            Select Case btn.CommandName

                Case "ChangeLocale"

                    BindFile(btn.CommandArgument)
                    SelectedLocale = btn.CommandArgument

                Case "Create"
                    Me.pnlFiles.Visible = False
                    Me.pnlEditor.Visible = False
                    Me.pnlForm.Visible = True
                    BindForm()
                    Me.TemplateTools.Items(ToolbarButton.CancelButton).Visible = True
                Case "Cancel"
                    Dim path As String = tree_Skins.SelectedNode.Attributes("absPath")
                    BindTree()
                    Me.tree_Skins.Nodes.FindNodeByAttribute("absPath", path).Selected = True
                    Me.tree_Skins.Nodes.FindNodeByAttribute("absPath", path).Expanded = True
                Case "Save"

                    Dim path As String = Me.tree_Skins.SelectedNode.Attributes("absPath")
                    Dim localepath As String = path.Substring(0, path.LastIndexOf(".") + 1) & SelectedLocale & ".txt"

                    If SelectedLocale.ToLower <> PortalSettings.DefaultLanguage.ToLower Then
                        If File.Exists(localepath) = True Then
                            path = localepath
                        End If
                    End If

                    Templates.UpdateTemplate(path, Me.txtTemplate.Text)

                Case "Delete"

                    If Me.tree_Skins.SelectedNode.ParentNode Is Nothing And (Not Me.tree_Skins.SelectedNode.Value.ToLower = "default") Then
                        Dim path As String = Me.tree_Skins.SelectedNode.Attributes("absPath")
                        If Directory.Exists(path) Then
                            Directory.Delete(path, True)
                        End If
                        BindTree()
                        Me.tree_Skins.Nodes(0).Selected = True
                        Me.tree_Skins.Nodes(0).Expanded = True

                    End If

                Case "Close"

                    Me.RadAjaxManager.ResponseScripts.Add("CloseDialog();")

            End Select
        End Sub

        Private Sub BindForm()


            Dim selectedNode As RadTreeNode = Me.tree_Skins.SelectedNode
            If Not selectedNode Is Nothing Then
                If selectedNode.ParentNode Is Nothing Then
                    Me.lblCopy.Text = String.Format(Localize("YouSelected"), selectedNode.Text)
                    Me.lblCopy.Text += Localize("PleaseEnterName")
                    If UserInfo.IsSuperUser Then
                        Me.rblMode.SelectedValue = "HOST"
                        Me.rblMode.Visible = True
                        Me.lblCopy.Text += Localize("SelectWetherToStore")
                    Else
                        Me.rblMode.SelectedValue = "PORTAL"
                        Me.rblMode.Visible = False
                        Me.lblCopy.Text += "."
                    End If
                    Me.lblCopy.CssClass = "pnc_EditForm_Message"
                    Me.cmdCreate.Enabled = True
                    Me.cmdCreate.CssClass = "cmdCreate"
                Else
                    Me.lblCopy.Text = Localize("OnlyFiles")
                    Me.lblCopy.CssClass = "pnc_EditForm_Error"
                    Me.cmdCreate.Enabled = False
                    Me.cmdCreate.CssClass = "cmdError"
                End If
            Else
                Me.lblCopy.Text = Localize("SelectBaseSkin")
                Me.lblCopy.CssClass = "pnc_EditForm_Error"
                Me.cmdCreate.Enabled = False
                Me.cmdCreate.CssClass = "cmdError"
            End If
        End Sub

        Protected Sub cmdCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCreate.Click

            If Me.txtName.Text.Length = 0 Then
                Me.lblCopy.Text = Localize("MustProvideName")
                Me.lblCopy.CssClass = "pnc_EditForm_Error"
                Exit Sub
            End If

            Dim sourcepath As String = Me.tree_Skins.SelectedNode.Attributes("absPath")
            Dim newpath As String = CreateTheme(Me.rblMode.SelectedValue, sourcepath, txtName.Text.Trim)

            BindTree()

            Dim node As RadTreeNode = Me.tree_Skins.Nodes.FindNodeByAttribute("absPath", newpath)

            If Not node Is Nothing Then
                'node.ExpandChildNodes()
                node.Expanded = True
                node.Selected = True
            End If
        End Sub

        Private Sub tree_Skins_NodeEdit(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEditEventArgs) Handles tree_Skins.NodeEdit

            Dim oldPath As String = e.Node.Attributes("absPath").ToLower
            Dim oldName As String = e.Node.Value.ToLower
            Dim oldText As String = e.Node.Text.ToLower

            Dim newName As String = e.Text
            Dim newText As String = oldText.Replace(oldName, newName)
            Dim newPath As String = oldPath.ToLower.Replace(oldName, newName)

            If Directory.Exists(oldPath) Then
                Try
                    Directory.CreateDirectory(newPath)
                    Directory.CreateDirectory(newPath & "images")

                    For Each File As String In Directory.GetFiles(oldPath)
                        System.IO.File.Copy(File, newPath & File.Substring(File.LastIndexOf("\") + 1))
                    Next

                    For Each folder As String In Directory.GetDirectories(oldPath)
                        If Not folder.ToLower.EndsWith("\") Then
                            folder = folder & "\"
                        End If
                        If folder.EndsWith("images\") Then
                            For Each File As String In Directory.GetFiles(folder)
                                System.IO.File.Copy(File, newPath & "images\" & File.Substring(File.LastIndexOf("\") + 1))
                            Next
                        End If
                    Next
                    System.IO.Directory.Delete(oldPath, True)
                    BindTree()
                    Dim node As RadTreeNode = Me.tree_Skins.Nodes.FindNodeByAttribute("absPath", newPath)
                    If Not node Is Nothing Then
                        node.Selected = True
                        node.Expanded = True
                    End If
                Catch ex As Exception
                    e.Node.Text = oldText
                    'todo: error
                End Try

            End If


        End Sub

        Private Sub grdTemplateHelp_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles grdTemplateHelp.NeedDataSource
            BindHelp()
        End Sub

#End Region

#Region "Private Methods"

        Private Sub BindHelp()

            Dim lstHelp As New List(Of TemplateHelp)
            Dim objHelp As TemplateHelp = Nothing

            Try
                Dim tmpl As String = Me.tree_Skins.SelectedNode.Attributes("tmplName")
                If tmpl <> "" Then


                    If tmpl.ToLower <> "css" Then

                        Try

                            Dim template As Templates.ArticlesTemplate = [Enum].Parse(GetType(Templates.ArticlesTemplate), tmpl)

                            Select Case template

                                Case Templates.ArticlesTemplate.Journal_Item

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASPRIMARYIMAGE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOPRIMARYIMAGE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PRIMARYIMAGEURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[TITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASSUMMARY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOSUMMARY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[SUMMARY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CONTENT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CONTENT:xxx]"
                                    lstHelp.Add(objHelp)

                                    lblHelp.Text = Localization.GetString("JournalTemplate.Help", Me.LocalResourceFile)

                                Case Templates.ArticlesTemplate.Listing_Item, Templates.ArticlesTemplate.Listing_ItemAlternate, Templates.ArticlesTemplate.Listing_ItemFirst, Templates.ArticlesTemplate.Detail_View, Templates.ArticlesTemplate.Scroller_Item, Templates.ArticlesTemplate.Listing_ItemFeatured

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ANCHORLINK]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PORTALID]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[SCRIPT:URL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CSS:URL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ITEMID]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ArticleId]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[TEMPLATEPATH]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ORIGINALLINK]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[EDIT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PUBLISHDATE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PUBLISHDAY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PUBLISHMONTH]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PUBLISHMONTHSHORT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PUBLISHMONTHNAME]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PUBLISHYEAR]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PUBLISHDATELONG]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PUBLISHDATESHORT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CREATEDDATELONG]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CREATEDDATESHORT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CREATEDBYUSER]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CREATEDBYUSERID]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "LASTUPDATEDDATE"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "LASTUPDATEDTIME"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "LASTUPDATEDBYUSER"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "LASTUPDATEDBYUSERID"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[SUMMARY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASSUMMARY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[SUMMARY:xxx]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOSUMMARY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CONTENT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASCONTENT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOCONTENT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CONTENT:xxx]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[TITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[NEWSLINKURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[NEWSLINKTITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[NEWSLINKTARGET]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[DETAILLINKURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[DETAILWINDOWLINK:W:H:S]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[DETAILWINDOWURL:W:H:S]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[YEARLINKURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[MONTHLINKURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[AUTHORLINKURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ALLNEWSLINKURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CATEGORYLIST]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASCOMMENTS]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOCOMMENTS]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASLINK]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOLINK]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASCATEGORY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOCATEGORY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[COMMENTURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[COMMENTCOUNT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[COMMENTLIST]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ISLINKEDTOIMAGE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ISNOTLINKEDTOIMAGE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[TWITTERTEXT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[SHAREDTITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[SHAREDURL]"
                                    lstHelp.Add(objHelp)

                                    'attachments

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ATTACHMENTS]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASATTACHMENTS]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOATTACHMENTS]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASPRIMARYATTACHMENT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOPRIMARYATTACHMENT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PRIMARYATTACHMENTURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PRIMARYATTACHMENTTITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PRIMARYATTACHMENTDESCRIPTION]"
                                    lstHelp.Add(objHelp)

                                    'gallery

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[IMAGEGALLERY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASIMAGES]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOIMAGES]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASPRIMARYIMAGE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[HASNOPRIMARYIMAGE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PRIMARYIMAGEURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PRIMARYIMAGETITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PRIMARYIMAGEDESCRIPTION]"
                                    lstHelp.Add(objHelp)

                                Case Templates.ArticlesTemplate.Image_Item

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[IMAGEURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ITEMID]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FILEID]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FOLDERNAME]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[IMAGEDESCRIPTION]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[IMAGETITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[IMAGEID]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ARTICLETITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ARTICLESUMMARY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ARTICLECONTENT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FILENAME]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FILEEXTENSION]"
                                    lstHelp.Add(objHelp)

                                    lblHelp.Text = Localization.GetString("ImageTemplate.Help", Me.LocalResourceFile)

                                Case Templates.ArticlesTemplate.Attachment_Item

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FILEURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ITEMID]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FILEID]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FOLDERNAME]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FILEDESCRIPTION]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FILETITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FILEID]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ARTICLETITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ARTICLESUMMARY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ARTICLECONTENT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FILENAME]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[FILEEXTENSION]"
                                    lstHelp.Add(objHelp)

                                    lblHelp.Text = Localization.GetString("AttachmentTemplate.Help", Me.LocalResourceFile)

                                Case Templates.ArticlesTemplate.Comment_Item

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ISARTICLEOWNER]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[APPROVE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[COMMENT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CREATEDBY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CREATEDATE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CREATETIME]"
                                    lstHelp.Add(objHelp)


                                    lblHelp.Text = Localization.GetString("CommentTemplate.Help", Me.LocalResourceFile)

                                Case Templates.ArticlesTemplate.Listing_Footer

                                    lblHelp.Text = Localization.GetString("FooterTemplate.Help", Me.LocalResourceFile)

                                Case Templates.ArticlesTemplate.Listing_Header

                                    lblHelp.Text = Localization.GetString("HeaderTemplate.Help", Me.LocalResourceFile)

                            End Select

                            Select Case template
                                Case Templates.ArticlesTemplate.Detail_View
                                    lblHelp.Text = Localization.GetString("ItemDetailTemplate.Help", Me.LocalResourceFile)
                                Case Templates.ArticlesTemplate.Listing_Item
                                    lblHelp.Text = Localization.GetString("ItemTemplate.Help", Me.LocalResourceFile)
                                Case Templates.ArticlesTemplate.Listing_ItemAlternate
                                    lblHelp.Text = Localization.GetString("ItemAlternateTemplate.Help", Me.LocalResourceFile)
                                Case Templates.ArticlesTemplate.Listing_ItemFirst
                                    lblHelp.Text = Localization.GetString("ItemFirstTemplate.Help", Me.LocalResourceFile)
                            End Select

                        Catch
                        End Try

                        Try

                            Dim template As Templates.NewsletterTemplate = [Enum].Parse(GetType(Templates.NewsletterTemplate), tmpl)

                            Select Case template

                                Case Templates.NewsletterTemplate.Mailing_Item

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PUBLISHDATE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CREATEDBY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[SUMMARY]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[CONTENT]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[TITLE]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PORTALNAME]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[DETAILLINKURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[MONTHNEWSURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ALLNEWSURL]"
                                    lstHelp.Add(objHelp)

                                    lblHelp.Text = Localization.GetString("MailingItemTemplate.Help", Me.LocalResourceFile)

                                Case Templates.NewsletterTemplate.Mailing_Footer

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[RECIPIENT_NAME]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[RECIPIENT_EMAIL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PORTALNAME]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ALLNEWSURL]"
                                    lstHelp.Add(objHelp)


                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[UNSUBSCRIBEURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[UNSUBSCRIBEURL:TabId]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ISROLESUBSCRIBED]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[/ISROLESUBSCRIBED]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ISSUBSCRIBED]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[/ISSUBSCRIBED]"
                                    lstHelp.Add(objHelp)


                                    lblHelp.Text = Localization.GetString("MailingFooterTemplate.Help", Me.LocalResourceFile)

                                Case Templates.NewsletterTemplate.Mailing_Header


                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[SUBJECT:Custom]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[RECIPIENT_NAME]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[RECIPIENT_EMAIL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[PORTALNAME]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ALLNEWSURL]"
                                    lstHelp.Add(objHelp)


                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[UNSUBSCRIBEURL]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[UNSUBSCRIBEURL:TabId]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ISROLESUBSCRIBED]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[/ISROLESUBSCRIBED]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[ISSUBSCRIBED]"
                                    lstHelp.Add(objHelp)

                                    objHelp = New TemplateHelp
                                    objHelp.Token = "[/ISSUBSCRIBED]"
                                    lstHelp.Add(objHelp)


                                    lblHelp.Text = Localization.GetString("MailingHeaderTemplate.Help", Me.LocalResourceFile)

                            End Select

                        Catch
                        End Try

                    Else

                        lblHelp.Text = Localization.GetString("Css.Help", Me.LocalResourceFile)

                    End If

                End If

                For Each objHelp In lstHelp
                    objHelp.HelpText = Localization.GetString(objHelp.Token.Replace("[", "").Replace("]", "") & ".Token", Me.LocalResourceFile)
                Next

            Catch
            End Try

            If lstHelp.Count > 0 Then
                grdTemplateHelp.DataSource = lstHelp
                grdTemplateHelp.Visible = True
            Else
                grdTemplateHelp.Visible = False
            End If


        End Sub

#End Region


        Private Sub grdFiles_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles grdFiles.NeedDataSource
            If tree_Skins.SelectedNode.Text = "images" Then
                Dim path As String = Me.tree_Skins.SelectedNode.Attributes("absPath")

                Dim files As New List(Of FileInfo)
                For Each File As String In Directory.GetFiles(path)
                    Dim oFile As FileInfo = New FileInfo(File)
                    If oFile.Extension.Replace(".", "").ToLower = "jpg" Or _
                        oFile.Extension.Replace(".", "").ToLower = "png" Or _
                        oFile.Extension.Replace(".", "").ToLower = "gif" Or _
                        oFile.Extension.Replace(".", "").ToLower = "jpeg" Or _
                        oFile.Extension.Replace(".", "").ToLower = "bmp" Then

                        files.Add(oFile)

                    End If

                Next

                grdFiles.DataSource = files

            End If
        End Sub

    End Class
End Namespace
