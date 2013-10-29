Imports DotNetNuke
Imports DotNetNuke.Entities.tabs
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports Telerik.Web.UI

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_RssBrowser
        Inherits ArticleModuleBase
        Implements IActionable


#Region "Private Methods"

        Private Sub BindTree()

            Dim RootNode As New RadTreeNode(Localize("AllFeeds"))
            RootNode.NavigateUrl = Me.ModuleDirectory & "/Rss.aspx?TabID=" & NewsModuleTab & "&moduleId=" & NewsModuleId & "&Locale=" & Me.CurrentLocale & "&FutureItems=" & ShowFutureItems.ToString & "&PastItems=" & ShowPastItems.ToString
            RootNode.Expanded = True
            RootNode.Target = "_blank"

            Dim Categories As New List(Of CategoryInfo)
            Dim cc As New CategoryController
            Categories = cc.ListCategoryItems(NewsModuleId, Me.CurrentLocale, Date.Now, ShowFutureItems, ShowPastItems, (UseOriginalVersion))

            For Each Category As CategoryInfo In Categories
                If Category.ParentID = Null.NullInteger Then

                    Dim NewNode As RadTreeNode = New RadTreeNode(Category.CategoryName & GetArticleCountInTreeNode(Category.Count), Category.CategoryID.ToString)
                    NewNode.NavigateUrl = Me.ModuleDirectory & "/Rss.aspx?TabID=" & NewsModuleTab & "&moduleId=" & NewsModuleId & "&CategoryID=" & Category.CategoryID.ToString & "&Locale=" & Me.CurrentLocale & "&FutureItems=" & ShowFutureItems.ToString & "&PastItems=" & ShowPastItems.ToString
                    NewNode.Target = "_blank"

                    If Me.ShowFolderIconsInCategories Then
                        NewNode.ImageUrl = Me.ImagesDirectory & "nuntio_rss.gif"
                        NewNode.ImageUrl = Me.ImagesDirectory & "nuntio_rss.gif"
                    End If

                    AddChildren(NewNode, Categories)

                    If Me.IncludeRootInCategories Then
                        RootNode.Nodes.Add(NewNode)
                    Else
                        Me.RssTree.Nodes.Add(NewNode)
                    End If

                End If
            Next

            If Me.IncludeRootInCategories Then
                RssTree.Nodes.Add(RootNode)
            End If


        End Sub

        Private Sub AddChildren(ByRef ParentNode As RadTreeNode, ByVal Categories As List(Of CategoryInfo))
            For Each Category As CategoryInfo In Categories

                If Category.ParentID = Null.NullInteger Then
                    If ParentNode.Value = "" Then
                        Dim NewNode As RadTreeNode = New RadTreeNode(Category.CategoryName & GetArticleCountInTreeNode(Category.Count), Category.CategoryID.ToString)
                        NewNode.NavigateUrl = Me.ModuleDirectory & "/Rss.aspx?TabID=" & NewsModuleTab & "&moduleId=" & NewsModuleId & "&CategoryID=" & Category.CategoryID.ToString & "&Locale=" & Me.CurrentLocale & "&FutureItems=" & ShowFutureItems.ToString & "&PastItems=" & ShowPastItems.ToString
                        If Me.ShowFolderIconsInCategories Then
                            NewNode.ImageUrl = Me.ImagesDirectory & "nuntio_rss.gif"
                            NewNode.ImageUrl = Me.ImagesDirectory & "nuntio_rss.gif"
                        End If
                        NewNode.Target = "_blank"
                        ParentNode.Nodes.Add(NewNode)
                        AddChildren(NewNode, Categories)
                    End If
                Else
                    If ParentNode.Value <> "" Then
                        If Category.ParentID = Integer.Parse(ParentNode.Value) Then
                            Dim NewNode As RadTreeNode = New RadTreeNode(Category.CategoryName & GetArticleCountInTreeNode(Category.Count), Category.CategoryID.ToString)
                            NewNode.NavigateUrl = Me.ModuleDirectory & "/Rss.aspx?TabID=" & TabId & "&moduleId=" & ModuleId & "&CategoryID=" & Category.CategoryID.ToString & "&Locale=" & Me.CurrentLocale & "&FutureItems=" & ShowFutureItems.ToString & "&PastItems=" & ShowPastItems.ToString
                            If Me.ShowFolderIconsInCategories Then
                                NewNode.ImageUrl = Me.ImagesDirectory & "nuntio_rss.gif"
                                NewNode.ImageUrl = Me.ImagesDirectory & "nuntio_rss.gif"
                            End If
                            NewNode.Target = "_blank"
                            ParentNode.Nodes.Add(NewNode)
                            AddChildren(NewNode, Categories)
                        End If
                    End If
                End If

            Next
        End Sub

        Private Sub UnselectAllNodes(ByVal treeView As RadTreeView)
            Dim node As RadTreeNode
            For Each node In treeView.GetAllNodes()
                node.Selected = False
            Next node
        End Sub 'UnselectAllNodes

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            Try

            Catch
            End Try
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                UnselectAllNodes(Me.RssTree)

                If Not Page.IsPostBack Then
                    BindTree()
                End If

                RssTree.Skin = TreeSkinInRss
                RssTree.ShowLineImages = Me.ShowLinesInRSSTree

                If CategoryID <> Null.NullInteger Then
                    Dim node As RadTreeNode
                    For Each node In Me.RssTree.GetAllNodes()
                        If node.Value = CategoryID.ToString Then
                            If Not node.ParentNode Is Nothing Then
                                Try
                                    node.ParentNode.Expanded = True
                                Catch
                                End Try
                            End If
                            node.Selected = True
                        End If
                    Next node
                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

        Public ReadOnly Property ModuleActions() As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection Implements DotNetNuke.Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Localize("RSSBROWSER.Action"), ModuleActionType.EditContent, "", "icon_hostsettings_16px.gif", NavigateURL(TabId, "RssOptions", "mid=" & ModuleId.ToString), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property
    End Class
End Namespace
