Imports Telerik.Web.UI

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Publications
        Inherits ArticleModuleBase


#Region "Private Members"

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.btnAddPublicationRelation.ImageUrl = ImagesDirectory & "nuntio_add.png"

            If Not Page.IsPostBack Then
                BindPublications()
            End If
        End Sub

        Private Sub drpRelatedPublicationItem_ItemsRequested(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs) Handles drpRelatedPublicationItem.ItemsRequested

            drpRelatedPublicationItem.Items.Clear()

            Dim sql As String = "Select * from " & DotNetNuke.Data.DataProvider.Instance().ObjectQualifier & "pnc_Localization_LocalizedItems where ModuleId = " & ModuleId.ToString & " and Content Like '%" & e.Text & "%' and [Key] = 'TITLE'"
            Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance().ExecuteSQL(sql)
            If Not dr Is Nothing Then
                While dr.Read
                    drpRelatedPublicationItem.Items.Add(New RadComboBoxItem(Convert.ToString(dr("Content")), Convert.ToString(dr("SourceItemId"))))
                End While
                dr.Close()
                dr.Dispose()
            End If

        End Sub

        Private Sub ctlTree_NodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles ctlTree.NodeClick
            If e.Node.Level = 0 Then
                pnlAddForm.Visible = True
                pnlRemoveForm.Visible = False
                lblIntro.Text = "Select an article to add to this publication"
            Else
                pnlAddForm.Visible = False
                pnlRemoveForm.Visible = True
                lblRemove.Text = "You remove the selected article from its publication"
            End If
        End Sub

        Private Sub btnAddPublicationRelation_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAddPublicationRelation.Click

            Dim id As Integer = Null.NullInteger
            Dim text As String = Null.NullString

            If Not drpRelatedPublicationItem.SelectedValue Is Nothing Then
                If drpRelatedPublicationItem.SelectedValue <> "" Then
                    If IsNumeric(drpRelatedPublicationItem.SelectedValue) Then
                        id = Convert.ToInt32(drpRelatedPublicationItem.SelectedValue)                      
                    End If
                End If
            End If

            If id <> Null.NullInteger Then

                Dim oArticle As ArticleInfo = ArticleController.GetArticle(id, CurrentLocale, True)
                If Not oArticle Is Nothing Then

                    If chkNew.Checked Then

                        If oArticle.IsPublication Then
                            lblResult.Text = "The selected article is a publication already and cannot be made another publication"
                        Else
                            If oArticle.ParentId <> Null.NullInteger Then
                                lblResult.Text = "The selected article is part of a publication and cannot be made a publication itself"
                            Else
                                oArticle.ParentId = Null.NullInteger
                                oArticle.IsPublication = True
                                ArticleController.UpdateArticle(oArticle)
                                BindPublications()
                                lblResult.Text = "The article has been made a publication."
                                SelectArticle(oArticle.itemId)
                            End If
                        End If

                    Else

                        Dim parentid As Integer = Convert.ToInt32(ctlTree.SelectedNode.Value)
                        If oArticle.IsPublication Then
                            lblResult.Text = "The selected article is a publication itself and cannot be added to another publication"
                        Else
                            oArticle.ParentId = parentid
                            ArticleController.UpdateArticle(oArticle)
                            BindPublications()
                            lblResult.Text = "The article has been added to the selected publication."
                            SelectArticle(parentid)
                        End If

                    End If

                End If

            End If

        End Sub

        Private Sub cmdNewPublication_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdNewPublication.Click

            pnlAddForm.Visible = True
            lblIntro.Text = "Select an article that acts as the publication"
            chkNew.Checked = True

        End Sub

        Private Sub cmdRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRemove.Click

            If Not ctlTree.SelectedNode Is Nothing Then
                If ctlTree.SelectedNode.Level = 1 Then

                    Dim oArticle As ArticleInfo = ArticleController.GetArticle(Convert.ToInt32(ctlTree.SelectedNode.Value), CurrentLocale, True)
                    If Not oArticle Is Nothing Then
                        oArticle.ParentId = Null.NullInteger
                        ArticleController.UpdateArticle(oArticle)
                        BindPublications()
                    End If
                End If
            End If

        End Sub

#End Region

#Region "Private Methods"

        Private Sub BindPublications()

            ctlTree.Nodes.Clear()

            Dim lstPublications As New List(Of ArticleInfo)
            lstPublications = ArticleController.GetArticlesPaged(New Integer, ModuleId, CurrentLocale, -1, 0, Date.Now, Null.NullInteger, Null.NullInteger, Me.SortOrder, New List(Of Integer), True, Null.NullInteger, True, True, True, True, True, False)

            For Each objPublication As ArticleInfo In lstPublications

                Dim PublicationNode As New RadTreeNode(objPublication.Title, objPublication.itemId)
                ctlTree.Nodes.Add(PublicationNode)

                For Each objArticle As ArticleInfo In objPublication.ChildArticles

                    Dim ArticleNode As New RadTreeNode(objArticle.Title, objArticle.itemId)
                    PublicationNode.Nodes.Add(ArticleNode)

                Next
            Next

            If lstPublications.Count < 10 Then
                ctlTree.ExpandAllNodes()
            End If

        End Sub

        Private Sub SelectArticle(ByVal ArticleId As Integer)

            ctlTree.UnselectAllNodes()
            ctlTree.FindNodeByValue(ArticleId.ToString).ExpandChildNodes()
            ctlTree.FindNodeByValue(ArticleId.ToString).Selected = True

        End Sub

#End Region

    End Class
End Namespace
