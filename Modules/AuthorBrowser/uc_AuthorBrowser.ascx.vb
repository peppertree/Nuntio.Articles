Imports DotNetNuke
Imports DotNetNuke.Entities.tabs
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports Telerik.Web.UI
Imports dnnWerk.Modules.Nuntio.Articles.Helpers

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_AuthorBrowser
        Inherits ArticleModuleBase

#Region "Private Methods"

        Private Sub UnselectAllNodes(ByVal treeView As RadTreeView)
            Dim node As RadTreeNode
            For Each node In treeView.GetAllNodes()
                node.Selected = False
            Next node
        End Sub 'UnselectAllNodes

        Private Sub BindAuthors()

            Dim objController As New AuthorController

            Dim RootNode As New RadTreeNode(Localization.GetString("RootNode", Me.LocalResourceFile))
            RootNode.NavigateUrl = Navigate(Me.NewsModuleTab, False)
            RootNode.Expanded = True

            For Each author As AuthorInfo In ArticleAuthors

                Dim NewNode As RadTreeNode = New RadTreeNode(author.DisplayName & GetArticleCountInTreeNode(author.Articles), author.userId.ToString)
                NewNode.NavigateUrl = Navigate(Me.NewsModuleTab, False, OnlyAlphaNumericChars(author.DisplayName) & ".aspx", "AuthorId=" & author.userId.ToString)
                If Me.ShowFolderIconsInAuthors Then
                    NewNode.ImageUrl = Me.ImagesDirectory & "nuntio_folder.gif"
                    NewNode.ExpandedImageUrl = Me.ImagesDirectory & "nuntio_folderopen.gif"
                End If

                If Me.IncludeRootInAuthors Then
                    RootNode.Nodes.Add(NewNode)
                Else
                    Me.AuthorsTree.Nodes.Add(NewNode)
                End If

            Next

            If Me.IncludeRootInAuthors Then
                AuthorsTree.Nodes.Add(RootNode)
            End If

        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            Try

            Catch
            End Try
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                Me.AuthorsTree.Skin = Me.TreeSkinInAuthors
                Me.AuthorsTree.ShowLineImages = Me.ShowLinesInAuthors

                UnselectAllNodes(Me.AuthorsTree)

                If Not Page.IsPostBack Then
                    BindAuthors()
                End If

                If IsModuleCall() Then
                    If AuthorId <> Null.NullInteger Then
                        Dim node As RadTreeNode
                        For Each node In Me.AuthorsTree.GetAllNodes()
                            If node.Value = AuthorId.ToString Then
                                If Not node.Parent Is Nothing Then
                                    Try
                                        node.ParentNode.Expanded = True
                                    Catch
                                    End Try
                                End If
                                node.Selected = True
                                node.Expanded = True
                            End If
                        Next node
                    End If
                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class
End Namespace
