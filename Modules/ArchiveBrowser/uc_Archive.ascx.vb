Imports DotNetNuke
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports Telerik.Web.UI
Imports dnnWerk.Modules.Nuntio.Articles.Helpers

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Archive
        Inherits ArticleModuleBase
        Implements IActionable

#Region "Private Methods"

        Private Sub BindTree()
            Dim FromDate As DateTime = Nothing

            FromDate = Now

            ArchiveTree.Nodes.Clear()

            Dim RootNode As New RadTreeNode(Localize("RootArchive"))
            RootNode.NavigateUrl = Navigate(Me.NewsModuleTab, False)
            RootNode.Expanded = True



            Dim objController As New ArchiveController

            Dim year As Integer = 0
            For Each obj As ArchiveInfo In ArticlesArchive
                If obj.Year <> year Then

                    Dim Yearnode As New RadTreeNode

                    Yearnode.Text = obj.Year
                    Yearnode.Value = obj.Year.ToString

                    If Me.ShowFolderIconsInArchive Then
                        Yearnode.ImageUrl = Me.ImagesDirectory & "nuntio_folder.gif"
                        Yearnode.ExpandedImageUrl = Me.ImagesDirectory & "nuntio_folderopen.gif"
                    End If

                    Yearnode.NavigateUrl = Navigate(Me.NewsModuleTab, False, OnlyAlphaNumericChars(obj.Year.ToString) & ".aspx", "Year=" & obj.Year.ToString, "NewsModule=" & NewsModuleId.ToString)

                    If Me.IncludeMonth Then
                        AddChildren(Yearnode, ArticlesArchive)
                    End If

                    If Me.IncludeRootInArchive Then
                        RootNode.Nodes.Add(Yearnode)
                    Else
                        ArchiveTree.Nodes.Add(Yearnode)
                    End If

                    year = obj.Year
                End If
            Next

            If Me.IncludeRootInArchive Then
                ArchiveTree.Nodes.Add(RootNode)
            End If


            If IsModuleCall() Then
                If Not Request.QueryString("Year") Is Nothing Then

                    Dim Yearnode As RadTreeNode = Nothing
                    Dim Monthnode As RadTreeNode = Nothing

                    Dim sYear As String = Request.QueryString("Year")
                    Dim sMonth As String = Request.QueryString("Month")

                    If Not sYear Is Nothing Then
                        Yearnode = ArchiveTree.FindNodeByValue(sYear)
                        Yearnode.Expanded = True
                    End If

                    If Not sMonth Is Nothing Then
                        Monthnode = ArchiveTree.FindNodeByValue(sYear & "_" & sMonth)
                        Monthnode.Expanded = True
                    End If

                Else
                    Dim Yearnode As RadTreeNode = ArchiveTree.Nodes.FindNodeByValue(Date.Now.Year.ToString)
                    If Not Yearnode Is Nothing Then
                        Yearnode.Expanded = True
                    End If
                End If
            Else
                If Me.IncludeRootInArchive = True Then
                    Dim Yearnode As RadTreeNode = ArchiveTree.Nodes.FindNodeByValue(Date.Now.Year.ToString)
                    If Not Yearnode Is Nothing Then
                        Yearnode.Expanded = True
                    End If
                End If

            End If

        End Sub

        Private Sub AddChildren(ByRef Node As RadTreeNode, ByVal archive As List(Of ArchiveInfo))
            Dim ct As Integer = 0
            For Each obj As ArchiveInfo In archive
                If obj.Year.ToString = Node.Value Then
                    Dim archiveDate As DateTime = New DateTime(obj.Year, obj.Month, obj.Day)
                    Dim monthnode As New RadTreeNode
                    monthnode.Value = obj.Year.ToString & "_" & obj.Month.ToString
                    monthnode.Text = archiveDate.ToString("MMMM") & GetArticleCountInTreeNode(obj.Count)

                    If Me.ShowFolderIconsInArchive Then
                        monthnode.ImageUrl = Me.ImagesDirectory & "nuntio_folder.gif"
                        monthnode.ExpandedImageUrl = Me.ImagesDirectory & "nuntio_folderopen.gif"
                    End If

                    monthnode.NavigateUrl = Navigate(Me.NewsModuleTab, False, OnlyAlphaNumericChars(obj.Year.ToString & "-" & obj.Month.ToString) & ".aspx", "Year=" & archiveDate.Year.ToString, "Month=" & archiveDate.Month.ToString, "NewsModule=" & NewsModuleId.ToString)
                    Node.Nodes.Add(monthnode)
                    ct = ct + obj.Count
                End If
            Next
            Node.Text += " (" & ct.ToString & ")"
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

                BindTree()

                Me.ArchiveTree.Skin = Me.TreeSkinInArchive
                Me.ArchiveTree.ShowLineImages = Me.ShowLinesInArchive


            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Optional Interfaces"
        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Localize("ARCHIVEOPTIONS.Action"), ModuleActionType.EditContent, "", "icon_hostsettings_16px.gif", NavigateURL(TabId, "ArchiveOptions", "mid=" & ModuleId.ToString), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property
#End Region

    End Class
End Namespace
