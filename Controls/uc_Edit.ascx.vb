
Imports Telerik.Web.UI

Imports dnnWerk
Imports System.IO

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Security.Permissions
Imports Telerik.Web.UI.Editor.DialogControls
Imports DotNetNuke.Entities.Users
Imports dnnWerk.Libraries.Nuntio.Localization
Imports DotNetNuke.UI.UserControls

Namespace dnnWerk.Modules.Nuntio.Articles

    Partial Class uc_Edit
        Inherits ArticleModuleBase

#Region "Private Members"

        Protected WithEvents txtBody As Global.DotNetNuke.UI.UserControls.TextEditor
        Protected WithEvents txtSummary As Global.DotNetNuke.UI.UserControls.TextEditor
        Private _currentimageurl As String = ""

        Private Property CurrentLocaleIndex() As Integer
            Get
                If Not ViewState("CurrentLocaleIndex") Is Nothing Then
                    Return CType(ViewState("CurrentLocaleIndex"), Integer)
                End If
                Return Null.NullInteger
            End Get
            Set(ByVal value As Integer)
                ViewState("CurrentLocaleIndex") = value
            End Set
        End Property

        Private Property IsNew() As Boolean
            Get
                If Not ViewState("IsNew") Is Nothing Then
                    Return CType(ViewState("IsNew"), Boolean)
                End If
                Return False
            End Get
            Set(ByVal value As Boolean)
                ViewState("IsNew") = value
            End Set
        End Property

        Private Overloads ReadOnly Property CurrentLocale
            Get

                Dim strLocale As String = MyBase.CurrentLocale
                If SupportedLocales.Count > 1 Then
                    strLocale = Me.drpLocale.SelectedValue
                End If
                Return strLocale

            End Get
        End Property

        Private Overloads ReadOnly Property ModuleConfiguration As Entities.Modules.ModuleInfo
            Get
                If MyBase.ModuleConfiguration Is Nothing Then
                    Dim mc As New Entities.Modules.ModuleController
                    Dim m As Entities.Modules.ModuleInfo = mc.GetModule(Me.ModuleId)
                    If Not m Is Nothing Then
                        Return m
                    End If
                End If
                Return MyBase.ModuleConfiguration
            End Get
        End Property

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            'DotNetNuke.Framework.AJAX.RegisterScriptManager()
            BindCss()

            Dim strFunction1 As String = "Nuntio_Refresh_" & ModuleId.ToString
            Dim strFunction2 As String = "Nuntio_Refresh_" & OpenerModuleId.ToString

            Dim strScript As String = vbCrLf & "<script type=""text/javascript"">" & vbCrLf
            strScript += "  function CloseWindow(){" & vbCrLf
            strScript += "      var wnd = GetRadWindow();" & vbCrLf
            strScript += "      if(typeof wnd.BrowserWindow." & strFunction1 & " == 'function') {" & vbCrLf
            strScript += "          wnd.BrowserWindow." & strFunction1 & "();" & vbCrLf
            If OpenerModuleId <> ModuleId Then
                strScript += "          if(typeof wnd.BrowserWindow." & strFunction2 & " == 'function') {" & vbCrLf
                strScript += "              wnd.BrowserWindow.setTimeout('" & strFunction2 & "()', 1000);" & vbCrLf
                strScript += "          }" & vbCrLf
            End If
            strScript += "      }" & vbCrLf
            If OpenerModuleId <> ModuleId Then
                strScript += "      else {" & vbCrLf
                strScript += "          if(typeof wnd.BrowserWindow." & strFunction2 & " == 'function') {" & vbCrLf
                strScript += "              wnd.BrowserWindow." & strFunction2 & "();" & vbCrLf
                strScript += "          }" & vbCrLf
                strScript += "      }" & vbCrLf
            End If
            strScript += "      wnd.close();" & vbCrLf
            strScript += "  }" & vbCrLf
            strScript += "</script>" & vbCrLf & vbCrLf

            Page.ClientScript.RegisterStartupScript(Me.[GetType](), "Nuntio_List_Refresh_" & ModuleId.ToString, strScript)


        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            BindImageManager()
            BindFileManager()

            SetupAjaxControl()

            Dim blnHasAccess As Boolean = False

            If CanAdmin Then
                blnHasAccess = True
            Else
                If CanEditArticle Then
                    blnHasAccess = True
                Else
                    If CanPublish Then
                        blnHasAccess = True
                    End If
                End If
            End If

            If blnHasAccess = False Then
                Me.ctlAjax.ResponseScripts.Add("CloseWindow();")
            End If

            FixFCKAjax()

            drpLocale.AutoPostBack = (ItemId <> Null.NullInteger)

            Try

                LocalizeForm()

                If Not Page.IsPostBack Then

                    BindForm()
                    SetupImagesTab()
                    SetupAttachmentsTab()

                    If ItemId <> Null.NullInteger Then
                        BindItem()
                    End If

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub ctlAjax_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs) Handles ctlAjax.AjaxRequest
            Select Case e.Argument.Split(Char.Parse(","))(0).ToLower
                Case "categories"
                    BindCategoryTree()
                    BindCategories()
                Case "deletecategory"
                    DeleteCategory(Convert.ToInt32(e.Argument.Split(Char.Parse(","))(1)))
                    BindCategoryTree()
                Case "bindgallery"
                    UpdateImagesStore(e.Argument.Split(Char.Parse(","))(1).ToLower)
                Case "bindattachments"
                    UpdateAttachmentStore(e.Argument.Split(Char.Parse(","))(1).ToLower)
                Case "bindfile"

                    Dim filepath As String = Server.UrlDecode(e.Argument.Split(Char.Parse(","))(1).ToLower)
                    Dim fullpath As String = e.Argument.Split(Char.Parse(","))(2)
                    Dim displayname As String = ""
                    Dim fileurl As String = ""
                    Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = Nothing

                    If Not filepath.StartsWith("/") Then
                        filepath = "/" & filepath
                    End If

                    If filepath.ToLower = "/linkclick.aspx" Then 'secure file

                        Dim qs As String = fullpath.Split(Char.Parse("?"))(1)
                        Dim params As String() = qs.Split(Char.Parse("&"))

                        For Each param As String In params
                            If param.ToLower.StartsWith("fileticket") Then

                                Dim strTicket As String = param.Replace("fileticket=", "")
                                Dim strFileId As String = UrlUtils.DecryptParameter(strTicket)

                                If IsNumeric(strFileId) Then

                                    Dim fileid As Integer = Convert.ToInt32(strFileId)

                                    objFile = DotNetNuke.Services.FileSystem.FileManager.Instance.GetFile(fileid)
                                    If Not objFile Is Nothing Then
                                        fileurl = "FileId=" & objFile.FileId.ToString
                                        displayname = objFile.FileName
                                    End If

                                End If
                            End If
                        Next

                    Else

                        Dim FileName As String = Path.GetFileName(filepath)
                        Dim FolderName As String = filepath.Replace(FileName, "")
                        FolderName = FolderName.Replace(PortalSettings.HomeDirectory.ToLower, "")

                        If FolderName = "/" Then
                            FolderName = ""
                        End If
                        If FolderName = "//" Then
                            FolderName = ""
                        End If
                        If FolderName.Length > 0 Then
                            If Not FolderName.EndsWith("/") Then
                                FolderName = FolderName & "/"
                            End If
                        End If

                        Dim objFolder As FolderInfo = FolderManager.Instance.GetFolder(PortalId, FolderName)
                        If Not objFolder Is Nothing Then
                            objFile = FileManager.Instance.GetFile(objFolder, filename)
                            If Not objFile Is Nothing Then
                                fileurl = "FileId=" & objFile.FileId.ToString
                                displayname = objFile.FileName
                            End If
                        End If

                    End If

                    txtPath.Text = fileurl
                    txtUrl.Text = displayname

            End Select
        End Sub

        Private Sub rblType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblType.SelectedIndexChanged

            BindUrlControl(rblType.SelectedValue, "", "")

        End Sub

        Private Sub btnLinkSelect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLinkSelect.Click
            Select Case rblType.SelectedValue
                Case "TAB"
                    ctlAjax.ResponseScripts.Add("openSelect('Tab','" & txtPath.Text & "')")
                Case "USER"
                    ctlAjax.ResponseScripts.Add("openSelect('User','" & txtPath.Text & "')")
                Case "DMX"
                    ctlAjax.ResponseScripts.Add("openSelect('DMX','" & txtPath.Text & "')")
            End Select
        End Sub

        Private Sub ctlTools_ButtonClick(ByVal sender As Object, ByVal e As RadToolBarEventArgs) Handles ctlTools.ButtonClick
            Select Case e.Item.Index
                Case 0
                    Me.ctlAjax.ResponseScripts.Add("CloseWindow();")
                Case 1
                    Update(False, False)
                Case 2
                    Update(False, True)
                Case 3
                    Update(True, False)
                Case 4
                    Delete()
                Case 5
                    Restore()
            End Select
        End Sub

        Private Sub drpLocale_SelectedIndexChanged(ByVal o As Object, ByVal e As RadComboBoxSelectedIndexChangedEventArgs) Handles drpLocale.SelectedIndexChanged

            If ItemId <> Null.NullInteger Then
                BindCategoryTree()
                BindItem()
                grdImages.Rebind()
            End If

        End Sub

        Public Sub cmdRestoreVersion_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim Version As Integer = Convert.ToInt32(CType(sender, LinkButton).CommandArgument)

            Dim ctrl As New ContentController(ModuleId, CurrentLocale, False)
            Dim lst As List(Of ContentInfo) = ctrl.GetAllContent

            Dim versions As New List(Of ContentInfo)
            Dim lstArticleContent As New List(Of ContentInfo)

            lstArticleContent = lst.FindAll(Function(x) ((x.Key = "TITLE" Or x.Key = "CONTENT" Or x.Key = "SUMMARY" Or x.Key = "URL") And x.SourceItemId = ItemId))

            Dim blnTitleUpdated As Boolean = False
            Dim blnBodyUpdated As Boolean = False
            Dim blnSummaryUpdated As Boolean = False
            Dim blnUrlUpdated As Boolean = False

            For Each objContent As ContentInfo In lstArticleContent

                If objContent.Version = Version Then

                    Select Case objContent.Key
                        Case "TITLE"
                            txtTitle.Text = objContent.Content
                            blnTitleUpdated = True
                        Case "CONTENT"
                            txtBody.Text = objContent.Content
                            blnBodyUpdated = True
                        Case "SUMMARY"
                            txtSummary.Text = objContent.Content
                            blnSummaryUpdated = True
                        Case "URL"

                            Dim url As String = objContent.Content

                            If url <> "" Then

                                If url.ToLower.StartsWith("fileid=") Then

                                    Dim f As Services.FileSystem.FileInfo = FileManager.Instance.GetFile(Convert.ToInt32(url.ToLower.Replace("fileid=", "")))
                                    If Not f Is Nothing Then
                                        'Dim folder As FolderInfo = FolderManager.Instance.GetFolder(f.FolderId)
                                        'If Not folder Is Nothing Then
                                        '    Dim folderpath As String = PortalSettings.HomeDirectory & folder.FolderPath
                                        '    If Not folderpath.EndsWith("/") Then
                                        '        folderpath = folderpath & "/"
                                        '    End If
                                        '    folderpath = folderpath & f.FileName
                                        '    BindUrlControl("FILE", f.FileName, folderpath)
                                        'End If
                                        'BindUrlControl("FILE", f.FileName, "FileId=" & f.FileId.ToString)
                                    End If

                                ElseIf url.ToLower.StartsWith("userid=") Then

                                    Dim userid As Integer = Integer.Parse(url.Replace("UserId=", ""))
                                    Dim u As Entities.Users.UserInfo = Entities.Users.UserController.GetUserById(PortalId, userid)
                                    If Not u Is Nothing Then
                                        BindUrlControl("USER", u.DisplayName, userid.ToString)
                                    End If

                                ElseIf IsNumeric(url) Then

                                    Dim tc As New Entities.Tabs.TabController
                                    Dim t As Entities.Tabs.TabInfo = tc.GetTab(Convert.ToInt32(url), PortalId, True)
                                    If Not t Is Nothing Then
                                        BindUrlControl("TAB", t.TabName, t.TabID)
                                    End If

                                Else

                                    BindUrlControl("URL", url, url)

                                End If

                            End If

                            blnUrlUpdated = True

                    End Select

                End If

                If blnBodyUpdated And blnTitleUpdated And blnSummaryUpdated And blnUrlUpdated Then
                    Exit For
                End If

            Next

        End Sub

        Private Sub cmdAddImage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddImage.Click
            Dim path As String = ""
            ctlAjax.ResponseScripts.Add("ShowImageManager('" & path & "');")
        End Sub

        Private Sub cmdAddAttachment_Click(sender As Object, e As System.EventArgs) Handles cmdAddAttachment.Click
            Dim path As String = ""
            ctlAjax.ResponseScripts.Add("ShowFileManager('" & path & "');")
        End Sub

        Private Sub grdAttachments_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grdAttachments.ItemDataBound
            If e.Item.ItemType = GridItemType.AlternatingItem Or e.Item.ItemType = GridItemType.Item Then

                Dim objFile = CType(e.Item.DataItem, ArticleFileInfo)



                Dim imgArticle As Image = CType(e.Item.FindControl("imgArticle"), Image)
                If Not imgArticle Is Nothing Then
                    Dim imgPath As String = Me.ImagesDirectory & "ext/" & objFile.Extension & ".gif"
                    If System.IO.File.Exists(Server.MapPath(imgPath)) Then
                        imgArticle.ImageUrl = imgPath
                    Else
                        imgArticle.ImageUrl = Me.ImagesDirectory & "ext/file.gif"
                    End If
                End If

                Dim txtAttachmentTitle As RadTextBox = CType(e.Item.FindControl("txtAttachmentTitle"), RadTextBox)
                If Not txtAttachmentTitle Is Nothing Then
                    txtAttachmentTitle.EmptyMessage = Localize("txtEmptyTitle")
                    txtAttachmentTitle.Text = objFile.ImageTitle
                End If

                Dim txtAttachmentDescription As RadTextBox = CType(e.Item.FindControl("txtImageDescription"), RadTextBox)
                If Not txtAttachmentDescription Is Nothing Then
                    txtAttachmentDescription.EmptyMessage = Localize("txtEmptyDescription")
                    txtAttachmentDescription.Text = objFile.ImageDescription
                End If

                Dim cmdMakeAttachmentPrimary As LinkButton = CType(e.Item.FindControl("cmdMakeAttachmentPrimary"), LinkButton)
                If Not cmdMakeAttachmentPrimary Is Nothing Then
                    cmdMakeAttachmentPrimary.Text = Localize("cmdMakeAttachmentPrimary")
                End If

                cmdMakeAttachmentPrimary.Visible = (objFile.IsPrimary = False)

                Dim cmdDeleteAttachment As LinkButton = CType(e.Item.FindControl("cmdDeleteAttachment"), LinkButton)
                If Not cmdDeleteAttachment Is Nothing Then
                    cmdDeleteAttachment.Text = Localize("cmdDeleteAttachment")
                End If

            End If
        End Sub

        Private Sub grdImages_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles grdImages.ItemDataBound
            If e.Item.ItemType = GridItemType.AlternatingItem Or e.Item.ItemType = GridItemType.Item Then

                Dim objImage = CType(e.Item.DataItem, ArticleFileInfo)

                Dim txtImageTitle As RadTextBox = CType(e.Item.FindControl("txtImageTitle"), RadTextBox)
                If Not txtImageTitle Is Nothing Then
                    txtImageTitle.EmptyMessage = Localize("txtEmptyTitle")
                    txtImageTitle.Text = objImage.ImageTitle
                End If

                Dim txtImageDescription As RadTextBox = CType(e.Item.FindControl("txtImageDescription"), RadTextBox)
                If Not txtImageDescription Is Nothing Then
                    txtImageDescription.EmptyMessage = Localize("txtEmptyDescription")
                    txtImageDescription.Text = objImage.ImageDescription
                End If

                Dim btnMakeImagePrimary As LinkButton = CType(e.Item.FindControl("cmdMakeImagePrimary"), LinkButton)
                If Not btnMakeImagePrimary Is Nothing Then
                    btnMakeImagePrimary.Text = Localize("cmdMakeImagePrimary")
                End If

                btnMakeImagePrimary.Visible = (objImage.IsPrimary = False)

                Dim btnDeleteImage As LinkButton = CType(e.Item.FindControl("cmdDeleteImage"), LinkButton)
                If Not btnDeleteImage Is Nothing Then
                    btnDeleteImage.Text = Localize("cmdDeleteImage")
                End If

            End If
        End Sub

        Protected Sub cmdDeleteAttachment_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            UpdateAttachments()

            Dim fileid As Integer = Convert.ToInt32(CType(sender, LinkButton).CommandArgument)
            Dim ctrl As New ArticleFileController
            Dim objFile As ArticleFileInfo = ctrl.GetFile(fileid, NewsModuleId, CurrentLocale, False)
            If Not objFile Is Nothing Then
                ctrl.DeleteFile(fileid)
            End If

            grdAttachments.Rebind()

        End Sub

        Protected Sub cmdMakeAttachmentPrimary_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            UpdateAttachments()

            Dim fileid As Integer = Convert.ToInt32(CType(sender, LinkButton).CommandArgument)
            Dim ctrl As New ArticleFileController

            Dim lstFiles As New System.Collections.Generic.List(Of ArticleFileInfo)
            lstFiles = ctrl.GetAttachments(ItemId, NewsModuleId, CurrentLocale, False)
            For Each objFile As ArticleFileInfo In lstFiles
                objFile.IsPrimary = (objFile.ArticleFileId = fileid)
                ctrl.UpdateFile(objFile, NewsModuleId, PortalId, UserId, chkOriginal.Checked)
            Next

            grdAttachments.Rebind()

        End Sub

        Protected Sub cmdDeleteImage_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            UpdateImages()

            Dim imageid As Integer = Convert.ToInt32(CType(sender, LinkButton).CommandArgument)
            Dim ctrl As New ArticleFileController
            Dim objImage As ArticleFileInfo = ctrl.GetFile(imageid, NewsModuleId, CurrentLocale, False)
            If Not objImage Is Nothing Then
                ctrl.DeleteFile(imageid)
            End If

            grdImages.Rebind()

        End Sub

        Protected Sub cmdMakeImagePrimary_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            UpdateImages()

            Dim imageid As Integer = Convert.ToInt32(CType(sender, LinkButton).CommandArgument)
            Dim ctrl As New ArticleFileController

            Dim lstImages As New System.Collections.Generic.List(Of ArticleFileInfo)
            lstImages = ctrl.GetImages(ItemId, NewsModuleId, CurrentLocale, False)
            For Each objImage As ArticleFileInfo In lstImages
                objImage.IsPrimary = (objImage.ArticleFileId = imageid)
                ctrl.UpdateFile(objImage, NewsModuleId, PortalId, UserId, chkOriginal.Checked)
            Next

            grdImages.Rebind()

        End Sub

        Private Sub grdImages_NeedDataSource(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grdImages.NeedDataSource

            Dim ctrl As New ArticleFileController
            Dim images As New List(Of ArticleFileInfo)
            images = ctrl.GetImages(ItemId, NewsModuleId, CurrentLocale, False)
            grdImages.DataSource = images

        End Sub

        Private Sub grdAttachments_NeedDataSource(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grdAttachments.NeedDataSource

            Dim ctrl As New ArticleFileController
            Dim files As New List(Of ArticleFileInfo)
            files = ctrl.GetAttachments(ItemId, NewsModuleId, CurrentLocale, False)
            grdAttachments.DataSource = files

        End Sub

        Private Sub treeCategories_NodeDrop(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeDragDropEventArgs) Handles treeCategories.NodeDrop

            Dim CurrentViewOrder As Integer = Convert.ToInt32(e.SourceDragNode.Attributes("ViewOrder"))
            Dim CurrentCategoryId As Integer = Convert.ToInt32(e.SourceDragNode.Value)
            Dim RelatedViewOrder As Integer = Convert.ToInt32(e.DestDragNode.Attributes("ViewOrder"))
            Dim RelatedCategoryId As Integer = Convert.ToInt32(e.DestDragNode.Value)

            Dim NewViewOrder As Integer = 0

            Dim tmpList As New List(Of CategoryInfo)
            Dim tmpViewOrder As Integer = 0
            Dim parentNode As RadTreeNode = e.DestDragNode.ParentNode

            If e.DropPosition = RadTreeViewDropPosition.Over Then
                parentNode = e.DestDragNode
            End If

            For Each childnode As RadTreeNode In parentNode.Nodes
                tmpViewOrder += 10
                Dim tmpCategory As New CategoryInfo
                tmpCategory.CategoryID = Integer.Parse(childnode.Value)
                tmpCategory.ViewOrder = tmpViewOrder
                tmpList.Add(tmpCategory)
            Next

            Select Case e.DropPosition
                Case RadTreeViewDropPosition.Above
                    NewViewOrder = -1
                Case RadTreeViewDropPosition.Below
                    NewViewOrder = +1
                Case RadTreeViewDropPosition.Over
                    NewViewOrder = 0
            End Select

            Dim ctrl As New CategoryController
            Dim objRelated As CategoryInfo = ctrl.GetCategory(RelatedCategoryId, ModuleId, CurrentLocale, True)
            Dim objNewViewOrder As CategoryInfo = ctrl.GetCategory(CurrentCategoryId, ModuleId, CurrentLocale, True)

            If tmpList.Count = 0 Then 'new node under parent node

                objNewViewOrder.ViewOrder = 0
                objNewViewOrder.ParentID = objRelated.CategoryID
                ctrl.UpdateCategoryItem(objNewViewOrder, UserId)

            Else

                If (e.DropPosition = RadTreeViewDropPosition.Above Or e.DropPosition = RadTreeViewDropPosition.Below) Then

                    For Each tmpCategory As CategoryInfo In tmpList
                        If tmpCategory.CategoryID = objRelated.CategoryID Then

                            objNewViewOrder.ViewOrder = tmpCategory.ViewOrder + NewViewOrder
                            objNewViewOrder.ParentID = objRelated.ParentID
                            ctrl.UpdateCategoryItem(objNewViewOrder, UserId)

                            Dim objUpdate As CategoryInfo = ctrl.GetCategory(tmpCategory.CategoryID, ModuleId, CurrentLocale, True)
                            objUpdate.ViewOrder = tmpCategory.ViewOrder
                            ctrl.UpdateCategoryItem(objUpdate, UserId)

                        Else

                            If Not tmpCategory.CategoryID = objNewViewOrder.CategoryID Then
                                Dim objUpdate As CategoryInfo = ctrl.GetCategory(tmpCategory.CategoryID, ModuleId, CurrentLocale, True)
                                objUpdate.ViewOrder = tmpCategory.ViewOrder
                                ctrl.UpdateCategoryItem(objUpdate, UserId)
                            End If

                        End If

                    Next

                Else

                    objNewViewOrder.ViewOrder = 0
                    objNewViewOrder.ParentID = RelatedCategoryId
                    ctrl.UpdateCategoryItem(objNewViewOrder, UserId)

                    For Each tmpCategory As CategoryInfo In tmpList
                        Dim objUpdate As CategoryInfo = ctrl.GetCategory(tmpCategory.CategoryID, ModuleId, CurrentLocale, True)
                        objUpdate.ViewOrder = tmpCategory.ViewOrder
                        ctrl.UpdateCategoryItem(objUpdate, UserId)
                    Next

                End If

            End If

            BindCategoryTree()

        End Sub

        Private Sub drpPublicationMode_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles drpPublicationMode.SelectedIndexChanged
            BindPublicationForm()
            BindPublicationItems()
        End Sub

        Private Sub btnAddPublicationRelation_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAddPublicationRelation.Click

            Dim id As Integer = Null.NullInteger
            Dim text As String = Null.NullString

            If Not drpRelatedPublicationItem.SelectedValue Is Nothing Then
                If drpRelatedPublicationItem.SelectedValue <> "" Then
                    If IsNumeric(drpRelatedPublicationItem.SelectedValue) Then
                        id = Convert.ToInt32(drpRelatedPublicationItem.SelectedValue)
                        text = drpRelatedPublicationItem.Text
                    End If
                End If
            End If

            If id <> Null.NullInteger Then
                treePublication.Nodes(0).Nodes.Add(New RadTreeNode(text, id.ToString))
            End If

            treePublication.ExpandAllNodes()

            drpRelatedPublicationItem.Items.Clear()
            drpRelatedPublicationItem.Text = ""

        End Sub

        Private Sub btnAddArticleRelation_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAddArticleRelation.Click
            Dim id As Integer = Null.NullInteger
            Dim text As String = Null.NullString

            If Not drpRelatedArticleItem.SelectedValue Is Nothing Then
                If drpRelatedArticleItem.SelectedValue <> "" Then
                    If IsNumeric(drpRelatedArticleItem.SelectedValue) Then
                        id = Convert.ToInt32(drpRelatedArticleItem.SelectedValue)
                        text = drpRelatedArticleItem.Text
                    End If
                End If
            End If

            If id <> Null.NullInteger Then
                ctlRelatedArticles.Items.Add(New RadListBoxItem(text, id))
            End If

            drpRelatedArticleItem.Items.Clear()
            drpRelatedArticleItem.Text = ""

        End Sub

        Private Sub drpRelatedArticleItem_ItemsRequested(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs) Handles drpRelatedArticleItem.ItemsRequested

            Dim combo As RadComboBox = CType(sender, RadComboBox)
            combo.Items.Clear()

            Dim sql As String = "Select * from " & DotNetNuke.Data.DataProvider.Instance().ObjectQualifier & "pnc_Localization_LocalizedItems where ModuleId = " & ModuleId.ToString & " and Content Like '%" & e.Text & "%' and [Key] = 'TITLE' and IsApproved = 1 and SourceItemId <> " & ArticleId.ToString
            Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance().ExecuteSQL(sql)
            If Not dr Is Nothing Then
                While dr.Read

                    Dim text As String = Convert.ToString(dr("Content"))
                    Dim value As String = Convert.ToString(dr("SourceItemId"))

                    Dim blnHasBeenAddedAlready As Boolean = False

                    If ctlRelatedArticles.Visible = True Then
                        If ctlRelatedArticles.Items.Count > 0 Then
                            For Each item As RadListBoxItem In ctlRelatedArticles.Items
                                If item.Value = value.ToString Then
                                    blnHasBeenAddedAlready = True
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                    If Not blnHasBeenAddedAlready Then
                        combo.Items.Add(New RadComboBoxItem(text, value))
                    End If

                End While
                dr.Close()
                dr.Dispose()
            End If

        End Sub

        Private Sub drpRelatedPublicationItem_ItemsRequested(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs) Handles drpRelatedPublicationItem.ItemsRequested

            Dim combo As RadComboBox = CType(sender, RadComboBox)
            combo.Items.Clear()

            Dim sql As String = "Select * from " & DotNetNuke.Data.DataProvider.Instance().ObjectQualifier & "pnc_Localization_LocalizedItems where ModuleId = " & ModuleId.ToString & " and Content Like '%" & e.Text & "%' and [Key] = 'TITLE' and IsApproved = 1 and SourceItemId <> " & ArticleId.ToString
            Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance().ExecuteSQL(sql)
            If Not dr Is Nothing Then
                While dr.Read

                    Dim text As String = Convert.ToString(dr("Content"))
                    Dim value As String = Convert.ToString(dr("SourceItemId"))

                    Dim blnHasBeenAddedAlready As Boolean = False

                    If treePublication.Visible = True Then
                        If treePublication.Nodes.Count > 0 Then
                            For Each node As RadTreeNode In treePublication.Nodes(0).Nodes
                                If node.Value = value.ToString Then
                                    blnHasBeenAddedAlready = True
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                    If Not blnHasBeenAddedAlready Then
                        combo.Items.Add(New RadComboBoxItem(text, value))
                    End If

                End While
                dr.Close()
                dr.Dispose()
            End If

        End Sub

        Private Sub rptVersions_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVersions.ItemDataBound

            If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then

                Dim cmdRestoreVersion As LinkButton = CType(e.Item.FindControl("cmdRestoreVersion"), LinkButton)
                If Not cmdRestoreVersion Is Nothing Then
                    cmdRestoreVersion.Text = Localize("cmdRestoreVersion")
                End If

            End If

        End Sub

        Private Sub rptVersions_PreRender(sender As Object, e As System.EventArgs) Handles rptVersions.PreRender
            For Each item As RepeaterItem In rptVersions.Items
                If item.ItemType = ListItemType.AlternatingItem Or item.ItemType = ListItemType.Item Then
                    Dim cmd As LinkButton = CType(item.FindControl("cmdRestoreVersion"), LinkButton)
                    If Not cmd Is Nothing Then
                        ctlAjax.AjaxSettings.AddAjaxSetting(cmd, pnlForm, lpDefault)
                    End If
                End If
            Next
        End Sub

#End Region

#Region "Private Initialization Methods"

        Private Sub BindCss()

            Dim strCssUrl As String = Me.ModuleDirectory & "/Css/Edit.css"

            Dim blnAlreadyRegistered As Boolean = False
            For Each ctrl As Control In Me.Page.Header.Controls

                If TypeOf (ctrl) Is HtmlLink Then
                    Dim ctrlCss As HtmlLink = CType(ctrl, HtmlLink)
                    If ctrlCss.Href.ToLower = strCssUrl.ToLower Then
                        blnAlreadyRegistered = True
                        Exit For
                    End If
                End If

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

        Private Sub BindForm()

            Me.wndManager.Skin = GetTelerikSkin()

            Me.btnAddPublicationRelation.ImageUrl = ImagesDirectory & "nuntio_add.png"
            Me.btnAddArticleRelation.ImageUrl = ImagesDirectory & "nuntio_add.png"

            Me.ctlTools.Items(0).ImageUrl = ImagesDirectory() & "nuntio_back.png"
            Me.ctlTools.Items(1).ImageUrl = ImagesDirectory() & "accept.png"
            Me.ctlTools.Items(2).ImageUrl = ImagesDirectory() & "accept.png"
            Me.ctlTools.Items(3).ImageUrl = ImagesDirectory() & "save.png"
            Me.ctlTools.Items(4).ImageUrl = ImagesDirectory() & "nuntio_delete.png"
            Me.ctlTools.Items(5).ImageUrl = ImagesDirectory() & "nuntio_refresh.png"
            'Me.icnMessage.ImageUrl = ImagesDirectory() & "alert.gif"

            Me.FirstNode.Items(0).ImageUrl = ImagesDirectory() & "nuntio_add.png"
            Me.CategoryNodes.Items(0).ImageUrl = ImagesDirectory() & "nuntio_add.png"
            Me.CategoryNodes.Items(1).ImageUrl = ImagesDirectory() & "nuntio_edit.png"
            Me.CategoryNodes.Items(2).ImageUrl = ImagesDirectory() & "nuntio_delete.png"

            BindLocales()
            BindCategoryTree()

            Me.ctlPublishDate.SelectedDate = Date.Now
            Me.rblType.SelectedIndex = 0

            If Not ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then
                Me.chkPublished.Visible = False
                Me.chkPublished.Checked = False
                Me.chkOriginal.Visible = False
                Me.tabNewsletter.Visible = False
                Me.ctlTabstrip.Tabs(3).Visible = False
            End If


            Me.ctlTabstrip.Tabs(1).Visible = Me.EnableSummary
            Me.ctlTabstrip.Tabs(3).Visible = True '(Me.AllowSubscriptions(True) Or Me.AllowTwitterPosts(True))
            pnlQueue.Visible = AllowSubscriptions()
            pnlTwitter.Visible = AllowTwitterPosts()

            Me.chkAddToQueue.Enabled = Me.AllowSubscriptions()
            Me.btnLinkSelect.Visible = False

            Try
                txtTwitterAccount.Text = CType(DotNetNuke.Services.Personalization.Personalization.GetProfile("Nuntio_TwitterAccount", "e-mail"), String)
            Catch
            End Try

        End Sub

        Private Sub SetupImagesTab()
            If ItemId <> Null.NullInteger Then
                pnlImagesWarning.Visible = False
                pnlImagesNote.Visible = True
                cmdAddImage.Enabled = True
            Else
                pnlImagesWarning.Visible = True
                pnlImagesNote.Visible = False
                cmdAddImage.Enabled = False
            End If
        End Sub

        Private Sub SetupAttachmentsTab()
            If ItemId <> Null.NullInteger Then
                pnlAttachmentsWarning.Visible = False
                pnlAttachmentsNote.Visible = True
                cmdAddAttachment.Enabled = True
            Else
                pnlAttachmentsWarning.Visible = True
                pnlAttachmentsNote.Visible = False
                cmdAddAttachment.Enabled = False
            End If
        End Sub

        Private Sub SetupAjaxControl()

            If Not IsFCKEditor() Then
                ctlAjax.AjaxSettings.AddAjaxSetting(ctlTools, pnlForm, lpDefault)
                ctlAjax.AjaxSettings.AddAjaxSetting(drpLocale, pnlForm, lpDefault)
                ctlAjax.AjaxSettings.AddAjaxSetting(wndManager, pnlForm, lpDefault)
            End If

        End Sub

#End Region

#Region "Private Databinding Methods"

        Private Sub BindCategoryTree()

            Me.treeCategories.Nodes.Clear()

            Dim node As New RadTreeNode(Localize("AllCategories.Treenode"))

            Me.treeCategories.Nodes.Add(node)
            Me.treeCategories.Nodes(0).Expanded = True
            Me.treeCategories.Nodes(0).Selected = False
            Me.treeCategories.Nodes(0).Checked = False
            Me.treeCategories.Nodes(0).Checkable = False
            Me.treeCategories.Nodes(0).ContextMenuID = "FirstNode"

            Dim Categories As New List(Of CategoryInfo)
            Dim cc As New CategoryController
            Categories = cc.ListCategoryItems(ModuleId, CurrentLocale, Date.Now, True, True, True)
            BindNodes(Me.treeCategories.Nodes(0), Categories)

            'Me.treeCategories.ExpandAllNodes()


            If ItemId = Null.NullInteger AndAlso CategoryID <> Null.NullInteger Then
                Dim selectednode As RadTreeNode = treeCategories.FindNodeByValue(CategoryID.ToString)
                If Not selectednode Is Nothing Then
                    selectednode.Checked = True
                End If
            End If

        End Sub

        Private Sub BindNodes(ByRef ParentNode As RadTreeNode, ByVal Categories As List(Of CategoryInfo))
            For Each Category As CategoryInfo In Categories

                If Category.ParentID = Null.NullInteger Then
                    If ParentNode.Value = "" Then
                        Dim NewNode As RadTreeNode = New RadTreeNode(Category.CategoryName, Category.CategoryID.ToString)
                        NewNode.Attributes.Add("ViewOrder", Category.ViewOrder)
                        ParentNode.Nodes.Add(NewNode)
                        NewNode.Expanded = True
                        NewNode.ContextMenuID = "CategoryNodes"
                        BindNodes(NewNode, Categories)
                    End If
                Else
                    If ParentNode.Value <> "" Then
                        If Category.ParentID = Integer.Parse(ParentNode.Value) Then
                            Dim NewNode As RadTreeNode = New RadTreeNode(Category.CategoryName, Category.CategoryID.ToString)
                            NewNode.Attributes.Add("ViewOrder", Category.ViewOrder)
                            ParentNode.Nodes.Add(NewNode)
                            NewNode.Expanded = False
                            NewNode.ContextMenuID = "CategoryNodes"
                            BindNodes(NewNode, Categories)
                        End If
                    End If
                End If
            Next
        End Sub

        Private Function GetLocaleStatusImagePath(ByVal Locale As String) As String

            Dim strPath As String = ImagesDirectory()
            If ItemId <> Null.NullInteger Then

                Dim itemcontroller As New ArticleController
                Dim item As ArticleInfo = itemcontroller.GetArticle(ItemId, Locale, True)
                If Not item Is Nothing Then
                    If item.Content <> "" AndAlso item.Title <> "" Then
                        strPath += "nuntio_iconFully.png"
                        Return strPath
                    End If
                    If item.Content <> "" Or item.Title <> "" Then
                        strPath += "nuntio_iconPartially.png"
                        Return strPath
                    End If
                End If

            End If

            strPath += "nuntio_iconNot.png"
            Return ResolveUrl(strPath)

        End Function

        Private Sub BindLocales()
            Dim intLocales As Integer = SupportedLocales.Count
            If intLocales > 1 Then
                'only show language selector if more than one
                'locale is enabled

                'add current locale first
                'set path and text properties
                Dim strPath As String = ImagesDirectory() & "iconNot.png"
                Dim strText As String = Localize("NotTranslated")

                Dim info As System.Globalization.CultureInfo = CType(Page, PageBase).PageCulture

                If ItemId <> Null.NullInteger Then

                    Dim itemcontroller As New ArticleController
                    Dim objItem As ArticleInfo = itemcontroller.GetArticle(ItemId, info.Name, False)

                    If Not objItem Is Nothing Then
                        If objItem.Content = "" And objItem.Title = "" Then
                            strPath = ImagesDirectory() & "nuntio_iconNot.png"
                            strText = Localize("NotTranslated")
                        End If
                        If objItem.Content <> "" Or objItem.Title <> "" Then
                            strPath = ImagesDirectory() & "nuntio_iconPartially.png"
                            strText = Localize("PartiallyTranslated")
                        End If
                        If objItem.Content <> "" AndAlso objItem.Title <> "" Then
                            strPath = ImagesDirectory() & "nuntio_iconFully.png"
                            strText = Localize("FullyTranslated")
                        End If
                    Else
                        strPath = ImagesDirectory() & "nuntio_iconNot.png"
                        strText = Localize("NotTranslated")
                    End If

                End If

                Dim item As New RadComboBoxItem
                item.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.DisplayName)
                item.Value = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name)
                item.Attributes.Add("ImagePath", ImagesDirectory() & "flags/" & info.Name.Substring(info.Name.IndexOf("-") + 1) & ".gif")
                item.Attributes.Add("IsTranslatedImage", strPath)
                item.Attributes.Add("IsTranslatedText", strText)

                Me.drpLocale.Items.Add(item)

                For Each objLocale As Locale In SupportedLocales.Values
                    info = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)

                    If info.Name <> CType(Page, PageBase).PageCulture.Name Then

                        If ItemId <> Null.NullInteger Then

                            Dim itemcontroller As New ArticleController
                            Dim objItem As ArticleInfo = itemcontroller.GetArticle(ItemId, info.Name, False)

                            If Not objItem Is Nothing Then
                                If objItem.Content = "" And objItem.Title = "" Then
                                    strPath = ImagesDirectory() & "nuntio_iconNot.png"
                                    strText = Localize("NotTranslated")
                                End If
                                If objItem.Content <> "" Or objItem.Title <> "" Then
                                    strPath = ImagesDirectory() & "nuntio_iconPartially.png"
                                    strText = Localize("PartiallyTranslated")
                                End If
                                If objItem.Content <> "" AndAlso objItem.Title <> "" Then
                                    strPath = ImagesDirectory() & "nuntio_iconFully.png"
                                    strText = Localize("FullyTranslated")
                                End If
                            Else
                                strPath = ImagesDirectory() & "iconNot.png"
                                strText = Localize("NotTranslated")
                            End If

                        End If

                        item = New RadComboBoxItem
                        item.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.DisplayName)
                        item.Value = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name).ToUpper
                        item.Attributes.Add("ImagePath", ImagesDirectory() & "flags/" & info.Name.Substring(info.Name.IndexOf("-") + 1) & ".gif")
                        item.Attributes.Add("IsTranslatedImage", strPath)
                        item.Attributes.Add("IsTranslatedText", strText)

                        Me.drpLocale.Items.Add(item)

                    End If

                Next

                Dim iCt As Integer = 0
                While iCt < drpLocale.Items.Count
                    drpLocale.Items(iCt).DataBind()
                    iCt = iCt + 1
                End While

                Me.drpLocale.Visible = True

            Else
                'only one locale exists, hide language selector
                Me.drpLocale.Visible = False
                Me.chkOriginal.Visible = False
                Me.ctlTools.Items(2).Visible = False
            End If
        End Sub

        Private Sub Delete()

            'first: clear cache
            ClearArticleCache()

            Dim item As ArticleInfo = Nothing
            Dim itemcontroller As New ArticleController
            If ItemId <> Null.NullInteger Then

                Dim objArticle As ArticleInfo = ArticleController.GetArticle(ItemId, CurrentLocale, True)
                If objArticle.IsDeleted Then
                    ArticleController.HardDeleteArticle(ItemId, ModuleId)
                Else
                    ArticleController.DeleteArticle(ItemId)
                End If
                Me.ctlAjax.ResponseScripts.Add("CloseWindow();")

            End If

        End Sub

        Private Sub Restore()

            'first: clear cache
            ClearArticleCache()

            Dim item As ArticleInfo = Nothing
            Dim itemcontroller As New ArticleController
            If ItemId <> Null.NullInteger Then
                ArticleController.RestoreArticle(ItemId)
            End If

            BindItem()

        End Sub

        Private Function GetSelectedPublishdateDate() As DateTime
            If Me.ctlPublishDate.DbSelectedDate Is Nothing Then
                Return Date.Now
            Else
                Return Me.ctlPublishDate.DbSelectedDate
            End If
        End Function

        Private Function GetSelectedReviewDate() As DateTime
            If Me.ctlReviewDate.DbSelectedDate Is Nothing Then
                Return Null.NullDate
            Else
                Return Me.ctlReviewDate.DbSelectedDate
            End If
        End Function

        Private Function GetSelectedAnchorLink() As String
            If String.IsNullOrEmpty(txtAnchor.Text) Then
                Return "Nuntio-"
            End If
            Return txtAnchor.Text
        End Function

        Private Function GetSelectedExpiryDate() As DateTime
            If Me.ctlExpiryDate.DbSelectedDate Is Nothing Then
                Return Null.NullDate
            Else
                Return Me.ctlExpiryDate.DbSelectedDate
            End If
        End Function

        Private Function GetArticleOriginality() As Boolean
            If Me.SupportedLocales.Count > 1 Then
                If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then
                    Return Me.chkOriginal.Checked
                Else
                    Return True
                End If
            Else
                Return True
            End If
        End Function

        Private Function GetArticleUrl() As String

            Select Case Me.rblType.SelectedValue
                Case "URL"
                    If Me.txtUrl.Text <> "http://" Then
                        Return Me.txtUrl.Text
                    End If
                Case "DMX"
                    Return Me.txtPath.Text
                    'Case "FILE"

                    '    If Me.txtPath.Text.Length > 0 Then
                    '        Dim filepath As String = txtPath.Text.Trim.ToLower

                    '        Dim FileName As String = Path.GetFileName(filepath)
                    '        Dim FolderName As String = filepath.Replace(FileName, "")
                    '        FolderName = FolderName.Replace(PortalSettings.HomeDirectory.ToLower, "")
                    '        If FolderName = "/" Then
                    '            FolderName = ""
                    '        End If
                    '        If FolderName = "//" Then
                    '            FolderName = ""
                    '        End If
                    '        If FolderName.Length > 0 Then
                    '            If Not FolderName.EndsWith("/") Then
                    '                FolderName = FolderName & "/"
                    '            End If
                    '        End If


                    '        Dim objFolder As FolderInfo = FolderManager.Instance().GetFolder(PortalId, FolderName)
                    '        If objFolder Is Nothing Then
                    '            'folder not found, ignore url property
                    '        Else
                    '            Dim f As Services.FileSystem.FileInfo = FileManager.Instance().GetFile(objFolder, FileName)
                    '            If Not f Is Nothing Then
                    '                Return "FileId=" & f.FileId.ToString
                    '            End If
                    '        End If

                    '    End If
                Case "TAB"
                    If Me.txtPath.Text.Length > 0 Then
                        If IsNumeric(Me.txtPath.Text.Trim) Then
                            Return Me.txtPath.Text.Trim
                        End If
                    End If
                Case "USER"
                    If Me.txtPath.Text.Length > 0 Then
                        If IsNumeric(Me.txtPath.Text.Trim) Then
                            Return "UserId=" & Me.txtPath.Text.Trim
                        End If
                    End If
            End Select

            Return String.Empty

        End Function

        Private Sub ManageUrlTracking(ByVal objArticle As ArticleInfo)
            If objArticle.Url <> "" Then
                Dim Trackingcontroller As New DotNetNuke.Common.Utilities.UrlController
                Trackingcontroller.UpdateUrl(PortalId, objArticle.Url, GetURLType(objArticle.Url).ToString("g").Substring(0, 1), Me.chkTrackUsers.Checked, Me.chkTrackClicks.Checked, ModuleId, Me.chkNewWindow.Checked)
            End If
        End Sub

        Private Sub ManageModerationNotification(ByVal objArticle As ArticleInfo)
            If Not ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then
                If CanPublish Then

                    Dim RoleController As New DotNetNuke.Security.Roles.RoleController()
                    Dim objRole As DotNetNuke.Security.Roles.RoleInfo = RoleController.GetRoleByName(PortalId, ModeratorRole)
                    Dim objUserRoles As ArrayList = RoleController.GetUserRolesByRoleName(PortalId, ModeratorRole)
                    For Each objUserRole As UserRoleInfo In objUserRoles
                        DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUserRole.Email, "", Services.Localization.Localization.GetString("NewItem_Subject", Me.LocalResourceFile), String.Format(Services.Localization.Localization.GetString("NewItem_Body", Me.LocalResourceFile), PortalSettings.PortalName, NavigateURL(TabId, "EDIT", "mid=" & ModuleId.ToString, "itemId=" & objArticle.ItemId.ToString)), "", "Text", "", "", "", "")
                    Next

                End If
            End If
        End Sub

        Private Sub Update(ByVal ExitForm As Boolean, ByVal Translate As Boolean)

            'first: clear cache
            ClearArticleCache()

            'exit if not title or no body provided
            If txtBody.Text.Length = 0 Or txtTitle.Text.Length = 0 Then
                ShowMessage(Localize("MsgRequired"))
                Exit Sub
            End If

            Dim strResponse As String = ""
            Dim blnIsError As Boolean = False

            Dim objArticle As ArticleInfo = Nothing
            Dim ArticleController As New ArticleController

            If ItemId <> Null.NullInteger And IsNew = False Then
                objArticle = ArticleController.GetArticle(ItemId, CurrentLocale, True)
            Else
                objArticle = New ArticleInfo
                objArticle.CreatedByUser = UserInfo.UserID
                objArticle.CreatedDate = Date.Now
            End If

            objArticle.ApprovedBy = UserInfo.UserID
            objArticle.ApprovedDate = Date.Now
            objArticle.IsApproved = Me.chkPublished.Checked
            objArticle.IsFeatured = Me.chkFeatured.Checked
            objArticle.Summary = Me.txtSummary.Text
            objArticle.Content = Me.txtBody.Text
            objArticle.Title = Me.txtTitle.Text
            objArticle.PublishDate = GetSelectedPublishdateDate()
            objArticle.ExpiryDate = GetSelectedExpiryDate()
            objArticle.IsNotified = (Me.chkAddToQueue.Checked = False)
            objArticle.IsOriginal = GetArticleOriginality()
            objArticle.Locale = CurrentLocale
            objArticle.moduleId = ModuleId
            objArticle.PortalID = PortalId
            objArticle.ViewOrder = 1
            objArticle.Url = GetArticleUrl()
            objArticle.LastUpdatedBy = UserInfo.UserID
            objArticle.LastUpdatedDate = Date.Now
            objArticle.ReviewDate = GetSelectedReviewDate()


            ManageUrlTracking(objArticle)

            'relations
            Select Case drpPublicationMode.SelectedIndex
                Case 0
                    objArticle.ParentId = Null.NullInteger
                    objArticle.IsPublication = False
                Case 1
                    objArticle.ParentId = Null.NullInteger
                    objArticle.IsPublication = True
                Case 2
                    If Not drpRelatedPublicationItem.SelectedValue Is Nothing Then
                        If IsNumeric(drpRelatedPublicationItem.SelectedValue) Then
                            objArticle.ParentId = Convert.ToInt32(drpRelatedPublicationItem.SelectedValue)
                        End If
                    End If
                    objArticle.IsPublication = False
            End Select

            If ItemId <> Null.NullInteger And IsNew = False Then

                UpdateImages()
                UpdateAttachments()

                Try

                    'anchor Link

                    If String.IsNullOrEmpty(txtAnchor.Text) Then
                        objArticle.AnchorLink = "NUA-" & objArticle.ItemId.ToString
                    Else
                        objArticle.AnchorLink = txtAnchor.Text
                    End If

                    ArticleController.UpdateArticle(objArticle)
                    strResponse = Localize("MsgArticleUpdated")
                Catch ex As Exception
                    strResponse = Localize("MsgError") & " " & ex.Message
                    blnIsError = True
                End Try

                If objArticle.IsOriginal Then
                    Try
                        'ManageOriginality(objArticle)
                    Catch
                    End Try
                End If

            Else

                If Not String.IsNullOrEmpty(txtAnchor.Text) Then
                    objArticle.AnchorLink = txtAnchor.Text
                Else
                    objArticle.AnchorLink = "NUA-New"
                End If

                objArticle.ItemId = ArticleController.AddArticle(objArticle)
                ItemId = objArticle.ItemId


                If objArticle.ItemId <> Null.NullInteger Then

                    If String.IsNullOrEmpty(txtAnchor.Text) Then
                        'update anchor link with itemid
                        objArticle.AnchorLink = "NUA-" & objArticle.ItemId.ToString
                        ArticleController.UpdateArticle(objArticle)
                    End If


                    ManageModerationNotification(objArticle)
                    strResponse = Localize("MsgArticleAdded")

                Else
                    strResponse = Localize("MsgError")
                    blnIsError = True
                End If

            End If

            Dim cc As New CategoryController
            cc.DeleteRelation(ItemId)
            For Each node As RadTreeNode In Me.treeCategories.CheckedNodes
                If node.Value <> "" Then
                    cc.AddRelation(ItemId, Integer.Parse(node.Value))
                End If
            Next

            Dim RelatedArticleList As New List(Of Integer)
            For Each item As RadListBoxItem In ctlRelatedArticles.Items
                RelatedArticleList.Add(Convert.ToInt32(item.Value))
            Next
            ArticleController.UpdateArticleRelation(ItemId, RelatedArticleList)

            If objArticle.IsPublication Then
                If Not treePublication.Nodes Is Nothing Then
                    If treePublication.Nodes.Count > 0 Then
                        For Each node As RadTreeNode In treePublication.Nodes(0).Nodes
                            If IsNumeric(node.Value) Then

                                Dim oChild As ArticleInfo = ArticleController.GetArticle(Convert.ToInt32(node.Value), CurrentLocale, True)
                                If Not oChild Is Nothing Then
                                    oChild.ParentId = objArticle.ItemId
                                    oChild.IsPublication = False
                                    ArticleController.UpdateArticle(oChild)
                                End If

                            End If
                        Next
                    End If
                End If

            End If

            If blnIsError Then
                ShowMessage(strResponse)
                Exit Sub
            End If

            Dim strTwitterResponse = ManageTwitterPost()
            If strTwitterResponse <> "" Then
                'strTwitterResponse = "Twitter Error Check your!"
                ShowMessage(strTwitterResponse)
                Exit Sub
            End If

            If blnIsError = False Then

                If AllowJournalUpdates Then

                    Dim strJournalTitle As String = objArticle.Title
                    Dim strJournalContent As String = ""
                    ProcessJournalItem(strJournalContent, objArticle, JournalTemplate, _currentimageurl)

                    JournalIntegration.SaveArticleToJournal(ItemId, TabId, UserId, PortalId, Navigate(TabId, False, Helpers.OnlyAlphaNumericChars(objArticle.Title) & ".aspx", "ArticleId=" & objArticle.ItemId), strJournalTitle, strJournalContent, _currentimageurl)

                End If

                If ExitForm Then

                    Me.ctlAjax.ResponseScripts.Add("CloseWindow();")

                Else

                    If Translate Then
                        If Me.SupportedLocales.Count > 1 Then
                            Me.SelectNextLocale()
                            Me.chkOriginal.Checked = (Me.chkOriginal.Checked = False)
                        End If
                    End If

                    Me.chkOriginal.Enabled = True
                    SetupImagesTab()
                    SetupAttachmentsTab()

                    If ItemId <> Null.NullInteger Then
                        BindItem()
                        grdImages.Rebind()
                    End If

                    If strResponse <> "" Then
                        ShowMessage(strResponse)
                    End If

                End If

            End If

        End Sub

        Private Sub BindUrlControl(ByVal UrlMode As String, ByVal Url As String, ByVal UrlInternal As String)
            Select Case UrlMode
                Case "URL"
                    btnLinkSelect.Visible = False
                    If Url.Length > 0 Then
                        txtUrl.Text = Url
                    Else
                        txtUrl.Text = "http://"
                    End If
                    txtUrl.Enabled = True
                Case "TAB"
                    btnLinkSelect.Visible = True
                    Me.btnLinkSelect.Text = Localize("btnSelectTab")
                    txtUrl.Text = Url
                    txtUrl.Enabled = False
                Case "USER"
                    btnLinkSelect.Visible = True
                    Me.btnLinkSelect.Text = Localize("btnSelectUser")
                    txtUrl.Text = Url
                    txtUrl.Enabled = False
                    'Case "FILE"
                    '    btnLinkSelect.Visible = True
                    '    Me.btnLinkSelect.Text = Localize("btnSelectFile")
                    '    txtUrl.Text = Url
                    '    txtUrl.Enabled = False
                Case "DMX"
                    btnLinkSelect.Visible = True
                    Me.btnLinkSelect.Text = Localize("btnSelectFile")
                    txtUrl.Text = Url
                    txtUrl.Enabled = False
            End Select
            rblType.SelectedValue = UrlMode
            txtPath.Text = UrlInternal
        End Sub

        Private Sub BindVersions(ByVal ItemId)
            Dim objArticle As ArticleInfo = ArticleController.GetArticle(ItemId, CurrentLocale, True)
            BindVersions(objArticle)
        End Sub

        Private Sub BindVersions(ByVal objArticle As ArticleInfo)

            Dim ctrl As New ContentController(objArticle.moduleId, CurrentLocale, True)
            Dim lst As List(Of ContentInfo) = ctrl.GetAllContent

            Dim versions As New List(Of ContentInfo)
            Dim lstArticleContent As New List(Of ContentInfo)

            lstArticleContent = lst.FindAll(Function(x) ((x.Key = "TITLE" Or x.Key = "CONTENT" Or x.Key = "SUMMARY" Or x.Key = "URL") And x.SourceItemId = objArticle.ItemId))
            lstArticleContent.Sort(Function(a, b) (a.CreatedDate < b.CreatedDate))

            Dim dtCreated As DateTime = Null.NullDate
            For Each objContent As ContentInfo In lstArticleContent
                If objContent.CreatedDate > dtCreated Then
                    versions.Add(objContent)
                End If
                dtCreated = objContent.CreatedDate
            Next

            versions.Sort(Function(a, b) (a.CreatedDate > b.CreatedDate))

            rptVersions.DataSource = versions
            rptVersions.DataBind()

        End Sub

        Private Sub BindCategories()

            If ItemId <> Null.NullInteger Then

                Dim categories As New List(Of Integer)
                Dim cc As New CategoryController
                categories = cc.GetRelationsByitemId(ItemId)
                For Each node As RadTreeNode In Me.treeCategories.GetAllNodes
                    If node.Value <> "" Then
                        If categories.Contains(Integer.Parse(node.Value)) Then
                            node.Checked = True
                        End If
                    End If
                Next

                For Each cat As Integer In categories
                    Try
                        Dim node As RadTreeNode = Me.treeCategories.FindNodeByValue(cat.ToString)
                        node.Checked = True
                        node.Expanded = True
                    Catch
                    End Try
                Next

            End If

        End Sub

        Private Sub BindItem()

            Dim item As ArticleInfo = Nothing
            Dim itemcontroller As New ArticleController

            If ItemId <> Null.NullInteger Then

                'bind categories

                BindCategories()

                item = itemcontroller.GetArticle(ItemId, CurrentLocale, True)

                If item.IsDeleted Then
                    ctlTools.Items(1).Visible = False
                    ctlTools.Items(2).Visible = False
                    ctlTools.Items(3).Visible = False
                    ctlTools.Items(4).Text = Localize("cmdHardDelete.Text")
                    ctlTools.Items(5).Visible = True
                Else
                    ctlTools.Items(1).Visible = True
                    ctlTools.Items(2).Visible = (drpLocale.Visible = True)
                    ctlTools.Items(3).Visible = True
                    ctlTools.Items(4).Text = Localize("cmdDelete.Text")
                    ctlTools.Items(5).Visible = False
                End If


                Me.lblArticleId.Text = "ArticleId: " & ItemId.ToString
                Me.txtAnchor.Text = "NUA-" & ItemId.ToString

                Try
                    Me.lblLastUpdated.Text = String.Format(Localize("lblCreatedByNote"), item.CreatedDate.ToShortDateString, UserController.GetUserById(PortalId, item.CreatedByUser).DisplayName)

                    If item.LastUpdatedDate > item.CreatedDate Then
                        lblLastUpdated.Text += String.Format(Localize("lblLastUpdateNote"), item.LastUpdatedDate.ToShortDateString, UserController.GetUserById(PortalId, item.LastUpdatedBy).DisplayName)
                    End If
                Catch ex As Exception

                End Try
                

                Me.txtTitle.Text = item.Title
                Me.ctlPublishDate.DbSelectedDate = item.PublishDate

                If Not item.ExpiryDate = Null.NullDate Then
                    ctlExpiryDate.SelectedDate = item.ExpiryDate
                End If

                If Not item.ReviewDate = Null.NullDate Then
                    ctlReviewDate.SelectedDate = item.ReviewDate
                End If

                BindPublication(item)
                BindRelations(item)
                BindVersions(item)

                Me.txtBody.Text = item.Content
                Me.txtSummary.Text = item.Summary

                Me.chkPublished.Checked = item.IsApproved
                Me.chkAddToQueue.Checked = (item.IsNotified = False)
                Me.chkOriginal.Checked = item.IsOriginal
                Me.chkFeatured.Checked = item.IsFeatured

                If item.Url <> "" Then

                    If item.Url.ToLower.StartsWith("fileid=") Then

                        'not longer supported since 6.1.2

                    ElseIf item.Url.ToLower.StartsWith("userid=") Then

                        Dim userid As Integer = Integer.Parse(item.Url.Replace("UserId=", ""))
                        Dim u As Entities.Users.UserInfo = Entities.Users.UserController.GetUserById(PortalId, userid)
                        If Not u Is Nothing Then
                            BindUrlControl("USER", u.DisplayName, userid.ToString)
                        End If

                    ElseIf IsNumeric(item.Url) Then

                        Dim tc As New Entities.Tabs.TabController
                        Dim t As Entities.Tabs.TabInfo = tc.GetTab(Convert.ToInt32(item.Url), PortalId, True)
                        If Not t Is Nothing Then
                            BindUrlControl("TAB", t.TabName, t.TabID)
                        End If

                    Else

                        BindUrlControl("URL", item.Url, item.Url)

                    End If

                    Dim objUrls As New UrlController
                    Dim objUrlTracking As UrlTrackingInfo = objUrls.GetUrlTracking(PortalSettings.PortalId, item.Url, ModuleId)
                    If Not objUrlTracking Is Nothing Then
                        Me.chkTrackClicks.Checked = True
                        Me.chkTrackUsers.Checked = objUrlTracking.LogActivity
                        Me.chkNewWindow.Checked = objUrlTracking.NewWindow
                    Else
                        Me.chkTrackClicks.Checked = False
                        Me.chkTrackUsers.Checked = False
                        Me.chkNewWindow.Checked = False
                    End If

                End If

            End If

            If item.IsLocaleTranslated = False Then
                'ShowMessage(Localize("NotFullyTranslated"))
            Else
                'Me.pnlMessage.Visible = False
            End If


        End Sub

        Private Sub BindRelations(ByVal objArticle As ArticleInfo)

            ctlRelatedArticles.Items.Clear()

            For Each objRelated As ArticleInfo In objArticle.RelatedArticles
                ctlRelatedArticles.Items.Add(New RadListBoxItem(objRelated.Title, objRelated.ItemId.ToString))
            Next

        End Sub

        Private Sub BindPublication(ByVal objArticle As ArticleInfo)

            If objArticle.IsPublication Then
                drpPublicationMode.SelectedIndex = 1
            Else
                If objArticle.ParentId <> Null.NullInteger Then
                    drpPublicationMode.SelectedIndex = 2
                Else
                    drpPublicationMode.SelectedIndex = 0
                End If
            End If

            BindPublicationForm()
            BindPublicationItems(objArticle)

        End Sub

        Private Sub BindPublicationItems()
            If ArticleId <> Null.NullInteger Then
                Dim objArticle As ArticleInfo = ArticleController.GetArticle(ArticleId, CurrentLocale, True)
                BindPublicationItems(objArticle)
            End If
        End Sub

        Private Sub BindPublicationItems(ByVal objArticle As ArticleInfo)

            If objArticle Is Nothing Then
                Exit Sub
            End If

            If objArticle.ParentId <> Null.NullInteger Then

                Dim objParent As ArticleInfo = ArticleController.GetArticle(objArticle.ParentId, CurrentLocale, True)
                If Not objParent Is Nothing Then
                    drpRelatedPublicationItem.Items.Clear()
                    drpRelatedPublicationItem.Items.Add(New RadComboBoxItem(objParent.Title, objParent.ItemId))
                End If

            Else
                If objArticle.IsPublication Then
                    For Each objChild As ArticleInfo In objArticle.ChildArticles
                        treePublication.Nodes(0).Nodes.Add(New RadTreeNode(objChild.Title, objChild.ItemId.ToString))
                    Next
                End If
            End If

        End Sub

        Private Sub BindPublicationForm()
            Select Case drpPublicationMode.SelectedIndex
                Case 0
                    lblBelongsTo.Text = ""
                    drpRelatedPublicationItem.Visible = False
                    btnAddPublicationRelation.Visible = False
                Case 1
                    lblBelongsTo.Text = Localize("lblBelongsTo1.Text")
                    drpRelatedPublicationItem.Visible = True
                    btnAddPublicationRelation.Visible = True
                    SetupPublicationTree()
                Case 2
                    lblBelongsTo.Text = Localize("lblBelongsTo2.Text")
                    drpRelatedPublicationItem.Visible = True
                    btnAddPublicationRelation.Visible = False
            End Select
        End Sub

        Private Sub SetupPublicationTree()

            treePublication.Nodes.Clear()
            treePublication.Nodes.Add(New RadTreeNode(txtTitle.Text, ArticleId.ToString))

        End Sub

        Private Function EndsWithSlash(ByVal strIn As String) As String
            If strIn.EndsWith("/") Then
                Return strIn
            Else
                Return strIn & "/"
            End If
        End Function

        Private Sub UpdateImagesStore(ByVal path As String)

            path = Server.UrlDecode(path)

            Dim filepath As String = EndsWithSlash(path.Substring(0, path.LastIndexOf("/"))).Replace(PortalSettings.HomeDirectory.ToLower, "")
            Dim filename As String = Server.UrlDecode(path).Substring(path.LastIndexOf("/") + 1)
            Dim extension As String = filename.Substring(filename.LastIndexOf(".") + 1).ToLower

            filepath = EndsWithSlash(filepath)

            If filepath = "/" Then
                filepath = ""
            End If

            If ItemId <> Null.NullInteger Then

                Dim blnExists As Boolean = False
                Dim objUpdate As ArticleFileInfo = Nothing

                Dim ctrl As New ArticleFileController
                Dim images As New List(Of ArticleFileInfo)
                images = ctrl.GetImages(ItemId, NewsModuleId, CurrentLocale, False)

                For Each objImage As ArticleFileInfo In images
                    If objImage.Folder.ToLower = filepath.ToLower AndAlso objImage.FileName.ToLower = filename.ToLower Then
                        'Update existing    
                        blnExists = True
                        objUpdate = objImage
                        Exit For
                    End If
                Next

                If objUpdate Is Nothing Then
                    objUpdate = New ArticleFileInfo
                    objUpdate.ImageDescription = ""
                    objUpdate.ImageTitle = ""
                    objUpdate.IsPrimary = (images.Count = 0)
                    objUpdate.IsImage = True
                    objUpdate.Extension = extension
                End If

                objUpdate.ArticleId = ItemId
                objUpdate.Locale = CurrentLocale

                Dim folderid As Integer = Null.NullInteger
                Dim fileid As Integer = Null.NullInteger

                folderid = FolderManager.Instance.GetFolder(PortalId, filepath).FolderID
                fileid = FileManager.Instance.GetFile(PortalId, filepath & filename).FileId


                If fileid <> Null.NullInteger Then
                    objUpdate.FileId = fileid
                End If

                If blnExists Then
                    ctrl.UpdateFile(objUpdate, NewsModuleId, PortalId, UserId, chkOriginal.Checked)
                Else
                    ctrl.AddFile(objUpdate, NewsModuleId, PortalId, UserId, chkOriginal.Checked)
                End If

                grdImages.Rebind()

            End If
        End Sub

        Private Sub UpdateAttachmentStore(ByVal path As String)

            path = Server.UrlDecode(path)

            Dim filepath As String = EndsWithSlash(path.Substring(0, path.LastIndexOf("/"))).Replace(PortalSettings.HomeDirectory.ToLower, "")
            Dim filename As String = Server.UrlDecode(path).Substring(path.LastIndexOf("/") + 1)
            Dim extension As String = filename.Substring(filename.LastIndexOf(".") + 1).ToLower

            filepath = EndsWithSlash(filepath)

            If filepath = "/" Then
                filepath = ""
            End If

            If ItemId <> Null.NullInteger Then

                Dim blnExists As Boolean = False
                Dim objUpdate As ArticleFileInfo = Nothing

                Dim ctrl As New ArticleFileController
                Dim files As New List(Of ArticleFileInfo)
                files = ctrl.GetAttachments(ItemId, NewsModuleId, CurrentLocale, False)

                For Each objFile As ArticleFileInfo In files
                    If objFile.Folder.ToLower = filepath.ToLower AndAlso objFile.FileName.ToLower = filename.ToLower Then
                        'Update existing    
                        blnExists = True
                        objUpdate = objFile
                        Exit For
                    End If
                Next

                If objUpdate Is Nothing Then
                    objUpdate = New ArticleFileInfo
                    objUpdate.ImageDescription = ""
                    objUpdate.ImageTitle = ""
                    objUpdate.IsPrimary = (files.Count = 0)
                    objUpdate.IsImage = False
                    objUpdate.Extension = extension
                End If

                objUpdate.ArticleId = ItemId
                objUpdate.Locale = CurrentLocale

                Dim folderid As Integer = Null.NullInteger
                Dim fileid As Integer = Null.NullInteger

                folderid = FolderManager.Instance.GetFolder(PortalId, filepath).FolderID
                fileid = FileManager.Instance.GetFile(PortalId, filepath & filename).FileId


                If fileid <> Null.NullInteger Then
                    objUpdate.FileId = fileid
                End If

                If blnExists Then
                    ctrl.UpdateFile(objUpdate, NewsModuleId, PortalId, UserId, chkOriginal.Checked)
                Else
                    ctrl.AddFile(objUpdate, NewsModuleId, PortalId, UserId, chkOriginal.Checked)
                End If

                grdAttachments.Rebind()

            End If
        End Sub

        Private Sub BindImageManager()

            Dim path As String = RemoveTrailingSlash(PortalSettings.HomeDirectory)

            ctlImages.HandlerUrl = Me.ModuleDirectory & "/DialogHandler.aspx?TabId=" & TabId.ToString
            ctlImages.Skin = GetTelerikSkin()

            Dim imageManagerParameters As New FileManagerDialogParameters
            imageManagerParameters.SearchPatterns = New String() {"*.jpg", "*.png", "*.gif"}
            imageManagerParameters.ViewPaths = New String() {path}
            imageManagerParameters.DeletePaths = New String() {path}
            imageManagerParameters.MaxUploadFileSize = 604800000
            imageManagerParameters.UploadPaths = New String() {path}
            imageManagerParameters.FileBrowserContentProviderTypeName = "DotNetNuke.Providers.RadEditorProvider.TelerikFileBrowserProvider, DotNetNuke.RadEditorProvider"

            Dim ctlImageManager As New ImageManagerDialog
            ctlImageManager.Language = CurrentLocale
            ctlImageManager.LocalizationPath = Me.ModuleDirectory & "/Controls/App_LocalResources/"

            Dim defImageManager As DialogDefinition = New DialogDefinition(ctlImageManager.GetType(), imageManagerParameters)
            defImageManager.ClientCallbackFunction = "SetExplorerImage"
            defImageManager.Width = Unit.Pixel(694)
            defImageManager.Height = Unit.Pixel(440)
            defImageManager.VisibleTitlebar = False
            defImageManager.Parameters("IsSkinTouch") = False
            ctlImages.DialogDefinitions.Add("ImageManager", defImageManager)

            Dim imageEditorParameters As New FileManagerDialogParameters
            imageEditorParameters.ViewPaths = New String() {path}
            imageEditorParameters.DeletePaths = New String() {path}
            imageEditorParameters.UploadPaths = New String() {path}
            imageEditorParameters.FileBrowserContentProviderTypeName = "DotNetNuke.Providers.RadEditorProvider.TelerikFileBrowserProvider, DotNetNuke.RadEditorProvider"
            imageEditorParameters.MaxUploadFileSize = 604800000

            Dim ctlImageEditor As New ImageEditorDialog
            Dim defImageEditor As DialogDefinition = New DialogDefinition(ctlImageEditor.GetType(), imageEditorParameters)
            defImageEditor.Width = Unit.Pixel(832)
            defImageEditor.Height = Unit.Pixel(520)
            defImageEditor.Parameters("IsSkinTouch") = False
            ctlImages.DialogDefinitions.Add("ImageEditor", defImageEditor)

        End Sub

        Private Sub UpdateImages()

            Dim ctrl As New ArticleFileController

            For Each item As GridDataItem In grdImages.Items
                If item.ItemType = GridItemType.AlternatingItem Or item.ItemType = GridItemType.Item Then

                    Dim objImage As ArticleFileInfo = Nothing

                    Dim itemid As Integer = item.OwnerTableView.DataKeyValues(item.ItemIndex)("ArticleFileId")
                    objImage = ctrl.GetFile(itemid, NewsModuleId, CurrentLocale, False)

                    If Not objImage Is Nothing Then

                        If objImage.IsPrimary Then
                            _currentimageurl = objImage.Url
                        End If

                        Dim txtImageTitle As RadTextBox = CType(item.FindControl("txtImageTitle"), RadTextBox)
                        If Not txtImageTitle Is Nothing Then
                            objImage.ImageTitle = txtImageTitle.Text
                        End If

                        Dim txtImageDescription As RadTextBox = CType(item.FindControl("txtImageDescription"), RadTextBox)
                        If Not txtImageDescription Is Nothing Then
                            objImage.ImageDescription = txtImageDescription.Text
                        End If

                        If objImage.Locale Is Nothing Then
                            objImage.Locale = CurrentLocale
                        End If

                        objImage.IsImage = True

                        ctrl.UpdateFile(objImage, NewsModuleId, PortalId, UserId, chkOriginal.Checked)

                    End If


                End If
            Next


        End Sub

        Private Sub UpdateAttachments()

            Dim ctrl As New ArticleFileController

            For Each item As GridDataItem In grdAttachments.Items
                If item.ItemType = GridItemType.AlternatingItem Or item.ItemType = GridItemType.Item Then

                    Dim objAttachment As ArticleFileInfo = Nothing

                    Dim itemid As Integer = item.OwnerTableView.DataKeyValues(item.ItemIndex)("ArticleFileId")
                    objAttachment = ctrl.GetFile(itemid, NewsModuleId, CurrentLocale, False)

                    If Not objAttachment Is Nothing Then

                        Dim txtAttachmentTitle As RadTextBox = CType(item.FindControl("txtAttachmentTitle"), RadTextBox)
                        If Not txtAttachmentTitle Is Nothing Then
                            objAttachment.ImageTitle = txtAttachmentTitle.Text
                        End If

                        Dim txtAttachmentDescription As RadTextBox = CType(item.FindControl("txtAttachmentDescription"), RadTextBox)
                        If Not txtAttachmentDescription Is Nothing Then
                            objAttachment.ImageDescription = txtAttachmentDescription.Text
                        End If

                        If objAttachment.Locale Is Nothing Then
                            objAttachment.Locale = CurrentLocale
                        End If

                        objAttachment.IsImage = False

                        ctrl.UpdateFile(objAttachment, NewsModuleId, PortalId, UserId, chkOriginal.Checked)

                    End If


                End If
            Next


        End Sub

        Private Sub BindFileManager()

            Dim path As String = RemoveTrailingSlash(PortalSettings.HomeDirectory)

            ctlFiles.HandlerUrl = Me.ModuleDirectory & "/DialogHandler.aspx?TabId=" & TabId.ToString
            ctlFiles.Skin = GetTelerikSkin()

            Dim documentManagerParameters As New FileManagerDialogParameters
            documentManagerParameters.SearchPatterns = New String() {"*.*"}
            documentManagerParameters.ViewPaths = New String() {path}
            documentManagerParameters.DeletePaths = New String() {path}
            documentManagerParameters.MaxUploadFileSize = 604800000
            documentManagerParameters.UploadPaths = New String() {path}
            documentManagerParameters.FileBrowserContentProviderTypeName = "DotNetNuke.Providers.RadEditorProvider.TelerikFileBrowserProvider, DotNetNuke.RadEditorProvider"


            Dim ctlDocumentManager As New DocumentManagerDialog
            ctlDocumentManager.Language = CurrentLocale
            ctlDocumentManager.LocalizationPath = Me.ModuleDirectory & "/Controls/App_LocalResources/"

            Dim defDocumentManager As DialogDefinition = New DialogDefinition(ctlDocumentManager.GetType(), documentManagerParameters)
            defDocumentManager.ClientCallbackFunction = "SetExplorerFile"
            defDocumentManager.Width = Unit.Pixel(694)
            defDocumentManager.Height = Unit.Pixel(440)
            defDocumentManager.VisibleTitlebar = False
            defDocumentManager.Parameters("IsSkinTouch") = False
            ctlFiles.DialogDefinitions.Add("DocumentManager", defDocumentManager)

        End Sub

        Private Sub BindDMXManager()

            Dim path As String = RemoveTrailingSlash(PortalSettings.HomeDirectory)

            ctlFiles.HandlerUrl = Me.ModuleDirectory & "/DialogHandler.aspx?TabId=" & TabId.ToString
            ctlFiles.Skin = GetTelerikSkin()

            Dim documentManagerParameters As New FileManagerDialogParameters
            documentManagerParameters.SearchPatterns = New String() {"*.*"}
            documentManagerParameters.ViewPaths = New String() {path}
            documentManagerParameters.DeletePaths = New String() {path}
            documentManagerParameters.MaxUploadFileSize = 604800000
            documentManagerParameters.UploadPaths = New String() {path}
            documentManagerParameters.FileBrowserContentProviderTypeName = "Bring2mind.DNN.Modules.DMX.RadEditorContentProvider.ContentProvider, "

            Dim ctlDocumentManager As New DocumentManagerDialog
            ctlDocumentManager.Language = CurrentLocale
            ctlDocumentManager.LocalizationPath = Me.ModuleDirectory & "/Controls/App_LocalResources/"

            Dim defDocumentManager As DialogDefinition = New DialogDefinition(ctlDocumentManager.GetType(), documentManagerParameters)
            defDocumentManager.ClientCallbackFunction = "SetFileLink"
            defDocumentManager.Width = Unit.Pixel(694)
            defDocumentManager.Height = Unit.Pixel(440)
            defDocumentManager.VisibleTitlebar = False
            defDocumentManager.Parameters("IsSkinTouch") = False
            ctlFiles.DialogDefinitions.Add("DocumentManager", defDocumentManager)

        End Sub

        Private Function RemoveTrailingSlash(ByVal path As String) As String
            If path.EndsWith("/") Then
                Return path.Substring(0, path.Length - 1)
            End If
            Return path
        End Function

#End Region

#Region "Private Helper Methods"

        Private Function GetTelerikSkin()

            Dim objEditor As RadEditor = Nothing

            If Not objEditor Is Nothing Then
                Return objEditor.Skin
            End If

            Return "Default"

        End Function

        Private Function ManageTwitterPost() As String

            If chkTwitter.Checked Then
                If txtTwitterAccount.Text <> "" Then

                    Try
                        Dim strTwitterMessage As String = DotNetNuke.Services.Mail.Mail.SendMail(UserInfo.Email, txtTwitterAccount.Text.Trim, UserInfo.Email, txtTitle.Text, HtmlUtils.Shorten(HtmlUtils.Clean(txtBody.Text, False), 135, "..."), "", "TEXT", "", "", "", "")
                        If strTwitterMessage <> "" Then
                            Return Localize("TwitterError") ' & " - " & strTwitterMessage
                        End If

                    Catch ex As Exception
                        Return Localize("TwitterError") ' & " - " & ex.ToString
                    End Try

                    DotNetNuke.Services.Personalization.Personalization.SetProfile("Nuntio_TwitterAccount", "e-mail", txtTwitterAccount.Text)
                    Return ""

                Else
                    Return Localize("TwitterAccountError")
                End If
            End If

            Return ""

        End Function

        Private Sub ShowMessage(ByVal strMessage As String)

            strMessage = strMessage.Replace("<", "").Replace(">", "")

            If IsFCKEditor() Then
                Dim radalertscript As String = "<script language='javascript'>function f(){radalert('" & strMessage & "', 330, 210); Sys.Application.remove_load(f);}; Sys.Application.add_load(f);</script>"
                Page.ClientScript.RegisterStartupScript(Me.[GetType](), "NuntioAlert", radalertscript)
            Else
                ctlAjax.ResponseScripts.Add("ModuleMessage('" & strMessage & "');")
                'Dim scriptstring As String = "radalert('" & strMessage & "', 330, 210);"
                'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "NuntioAlert", scriptstring, True)
            End If

        End Sub

        'Private Sub ManageOriginality(ByVal item As ArticleInfo)
        '    Dim locItems As New pnc.Publisher.Localization.LocalizedItemsCollection
        '    Dim locale As String = ""
        '    For Each objLocale As Locale In SupportedLocales.Values
        '        Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
        '        locale = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name).ToUpper
        '        If locale.ToUpper <> item.Locale.ToUpper Then
        '            Dim locitem As New pnc.Publisher.Localization.LocalizedItem
        '            Try
        '                locItems = pnc.Publisher.Localization.ContentLocalization.LocalizedItems(ModuleId)
        '                locitem = locItems.Lookup(ModuleId, item.ItemId, "TITLE", locale)
        '                If Not locitem Is Nothing Then
        '                    locitem.IsOriginal = False
        '                    locItems.Update(locitem)
        '                End If
        '            Catch
        '            End Try
        '            Try
        '                locItems = pnc.Publisher.Localization.ContentLocalization.LocalizedItems(ModuleId)
        '                locitem = locItems.Lookup(ModuleId, item.ItemId, "SUMMARY", locale)
        '                If Not locitem Is Nothing Then
        '                    locitem.IsOriginal = False
        '                    locItems.Update(locitem)
        '                End If
        '            Catch
        '            End Try
        '            Try
        '                locItems = pnc.Publisher.Localization.ContentLocalization.LocalizedItems(ModuleId)
        '                locitem = locItems.Lookup(ModuleId, item.ItemId, "CONTENT", locale)
        '                If Not locitem Is Nothing Then
        '                    locitem.IsOriginal = False
        '                    locItems.Update(locitem)
        '                End If
        '            Catch
        '            End Try
        '            Try
        '                locItems = pnc.Publisher.Localization.ContentLocalization.LocalizedItems(ModuleId)
        '                locitem = locItems.Lookup(ModuleId, item.ItemId, "URL", locale)
        '                If Not locitem Is Nothing Then
        '                    locitem.IsOriginal = False
        '                    locItems.Update(locitem)
        '                End If
        '            Catch
        '            End Try
        '        End If
        '    Next

        'End Sub

        Private Sub SelectNextLocale()
            CurrentLocaleIndex = Me.drpLocale.SelectedIndex
            Try
                If Me.drpLocale.Items.Count >= CurrentLocaleIndex Then
                    Me.drpLocale.SelectedIndex = CurrentLocaleIndex + 1
                Else
                    Me.drpLocale.SelectedIndex = CurrentLocaleIndex - 1
                End If
            Catch
            End Try
        End Sub

        Private Sub DeleteCategory(ByVal CategoryId As Integer)

            Dim cc As New CategoryController
            Dim categories As New List(Of CategoryInfo)
            categories = cc.ListCategoryItems(NewsModuleId, CurrentLocale, Date.Now, True, True, True)
            DeleteChildCategories(CategoryId, categories)
            cc.DeleteCategory(CategoryId, NewsModuleId)

        End Sub

        Private Sub DeleteChildCategories(ByVal ParentId As Integer, ByVal Categories As List(Of CategoryInfo))
            Dim cc As New CategoryController
            For Each oCat As CategoryInfo In Categories
                If oCat.ParentID = ParentId Then
                    DeleteChildCategories(oCat.CategoryID, Categories)
                    cc.DeleteCategory(oCat.CategoryID, Me.NewsModuleId)
                End If
            Next
        End Sub

        Private Sub LocalizeForm()

            'labels

            Me.lblPublicationsHead.Text = Localize("lblPublicationsHead")
            Me.lblPublicationsIntro.Text = Localize("lblPublicationsIntro")


            Me.drpPublicationMode.Items(0).Text = Localize("ModeNoPublication")
            Me.drpPublicationMode.Items(1).Text = Localize("ModeIsPublication")
            Me.drpPublicationMode.Items(2).Text = Localize("ModeBelongsToPublication")

            Me.lblRelatedHead.Text = Localize("lblRelatedHead")
            Me.lblRelatedIntro.Text = Localize("lblRelatedIntro")

            Me.lblAnchorNote.Text = Localize("lblAnchorNote")
            Me.lblArticleInformationHead.Text = Localize("lblArticleInformationHead")
            Me.lblImagesWarning.Text = Localize("lblImagesWarning")
            Me.lblImagesIntro.Text = Localize("lblImagesIntro")
            Me.lblAttachmentsWarning.Text = Localize("lblImagesWarning")
            Me.lblAttachmentsIntro.Text = Localize("lblAttachmentsIntro")
            Me.lblExpiryDate.Text = Localize("lblExpiryDate")
            Me.lblEmailHeader.Text = Localize("lblEmailHeader")
            Me.lblCategoriesHelp.Text = Localize("lblCategoriesHelp")
            Me.lblHeadCategories.Text = Localize("lblHeadCategories")
            Me.lblHeadLink.Text = Localize("lblHeadLink")
            Me.lblLinkHelp.Text = Localize("lblLinkHelp")
            Me.lblMeilQueueHelp.Text = Localize("lblMeilQueueHelp")
            Me.lblTwitterAccount.Text = Localize("TwitterAccount")
            'Me.lblOriginalHelp.Text = Localize("lblOriginalHelp")
            Me.lblPublishDate.Text = Localize("lblPublishDate")
            Me.lblTitle.Text = Localize("lblTitle")
            Me.lblTwitterHeader.Text = Localize("TwitterHeader")
            Me.lblTwitterHelp.Text = Localize("TwitterHelp")
            Me.chkTwitter.Text = Localize("TwitterCheckbox")

            'checkboxes
            Me.chkAddToQueue.Text = String.Format(Localize("chkAddToQueue"), GetNextMailRun())
            Me.chkNewWindow.Text = Localize("chkNewWindow")
            Me.chkOriginal.Text = Localize("chkOriginal")
            Me.chkPublished.Text = Localize("chkPublished")
            Me.chkTrackClicks.Text = Localize("chkTrackClicks")
            Me.chkTrackUsers.Text = Localize("chkTrackUsers")

            'tabstrip
            Me.ctlTabstrip.Tabs(0).Text = Localize("Body.Tab")
            Me.ctlTabstrip.Tabs(1).Text = Localize("Summary.Tab")
            Me.ctlTabstrip.Tabs(2).Text = Localize("Advanced.Tab")
            Me.ctlTabstrip.Tabs(3).Text = Localize("Newsletter.Tab")
            Me.ctlTabstrip.Tabs(4).Text = Localize("Attachments.Tab")
            Me.ctlTabstrip.Tabs(5).Text = Localize("Images.Tab")
            Me.ctlTabstrip.Tabs(6).Text = Localize("Related.Tab")
            Me.ctlTabstrip.Tabs(7).Text = Localize("Versioning.Tab")

            'toolbar
            Me.ctlTools.Items(0).Text = Localize("cmdCancel")
            Me.ctlTools.Items(1).Text = Localize("cmdSave.Text")
            Me.ctlTools.Items(2).Text = Localize("cmdSaveTranslate.Text")
            Me.ctlTools.Items(3).Text = Localize("cmdSaveExit.Text")
            Me.ctlTools.Items(4).Text = Localize("cmdDelete.Text")
            Me.ctlTools.Items(5).Text = Localize("cmdRestore.Text")

            'radiobuttonlists
            Me.rblType.Items(0).Text = Localize("rblUrl")
            Me.rblType.Items(1).Text = Localize("rblTab")
            Me.rblType.Items(2).Text = Localize("rblUser")

            If DMXSupport Then
                Me.rblType.Items(3).Text = Localize("rblDMX")
            Else
                If rblType.Items.Count > 3 Then
                    Me.rblType.Items.RemoveAt(3)
                End If
            End If

            'contextmenus
            Me.treeCategories.ContextMenus(0).Items(0).Text = Localize("AddCategory.Menu")
            Me.treeCategories.ContextMenus(1).Items(0).Text = Localize("AddCategory.Menu")
            Me.treeCategories.ContextMenus(1).Items(1).Text = Localize("EditCategory.Menu")
            Me.treeCategories.ContextMenus(1).Items(2).Text = Localize("DeleteCategory.Menu")

            'datepicker
            Me.ctlPublishDate.Calendar.FastNavigationSettings.CancelButtonCaption = Localize("CancelButton")
            Me.ctlPublishDate.Calendar.FastNavigationSettings.TodayButtonCaption = Localize("TodayButton")
            Me.ctlPublishDate.Calendar.FastNavigationSettings.OkButtonCaption = Localize("OkButton")
            Me.ctlPublishDate.TimeView.HeaderText = Localize("TimePicker")

            Me.ctlPublishDate.Culture = CType(Page, PageBase).PageCulture
            Me.cmdAddImage.Text = Localize("cmdAddImage")
            Me.cmdAddAttachment.Text = Localize("cmdAddAttachment")
            Me.grdImages.MasterTableView.NoMasterRecordsText = Localize("NoImagesYet")
            Me.grdAttachments.MasterTableView.NoMasterRecordsText = Localize("NoAttachmentsYet")
        End Sub

        Private Function GetNextMailRun() As String


            Dim lstSchedule As New ArrayList
            lstSchedule = DotNetNuke.Services.Scheduling.SchedulingProvider.Instance.GetSchedule()

            If Not lstSchedule Is Nothing Then

                For Each objSchedule As DotNetNuke.Services.Scheduling.ScheduleItem In lstSchedule
                    If objSchedule.TypeFullName = "dnnWerk.Modules.Nuntio.Articles.ArticleNotification, dnnWerk.Nuntio.Articles" Then

                        Return objSchedule.NextStart.ToString

                    End If
                Next

            End If

            Return "-"

        End Function

        Private Function IsFCKEditor() As Boolean
            Dim tp As Type = Me.txtBody.RichText.GetType
            If tp.FullName.ToLower.Contains("fckhtmleditorprovider") Then
                Return True
            End If
            Return False
        End Function

        Private Sub FixFCKAjax()

            If IsFCKEditor() Then
                Dim sm As ScriptManager = ScriptManager.GetCurrent(Me.Page)
                If Not sm Is Nothing Then
                    Dim strScript As String = "for ( var i = 0; i < parent.frames.length; ++i ) if ( parent.frames[i].FCK ) parent.frames[i].FCK.UpdateLinkedField();"
                    ScriptManager.RegisterOnSubmitStatement(Me.Page, Me.GetType(), "FCKAjaxHack", strScript)
                End If
            End If

        End Sub

#End Region

#Region "Public Functions"

        Public Function GetDisplayname(ByVal UserId As Object)

            Dim strUser As String = ""
            If Not UserId Is Nothing Then
                If IsNumeric(UserId) Then
                    Dim objUser As UserInfo = UserController.GetUserById(PortalId, Convert.ToInt32(UserId))
                    If Not objUser Is Nothing Then
                        strUser = objUser.DisplayName
                    End If
                End If
            End If

            Return strUser

        End Function

#End Region

    End Class

End Namespace