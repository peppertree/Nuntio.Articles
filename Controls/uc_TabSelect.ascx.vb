
Imports Telerik.Web.UI
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_TabSelect
        Inherits ArticleModuleBase

#Region "Private Members"


#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            CType(Me.Page.FindControl("lblTitle"), Literal).Text = Localize("SelectTab")
            btnClose.Text = Localize("btnClose")
            If Not Page.IsPostBack Then
                BindTree()

                Dim TabToSelect As Integer = Utilities.Null.NullInteger
                If Not Request.QueryString("Select") Is Nothing Then
                    Try
                        TabToSelect = Integer.Parse(Request.QueryString("Select"))
                    Catch
                    End Try
                End If

                If TabToSelect <> Utilities.Null.NullInteger Then
                    Try
                        ctlPages.FindNodeByValue(TabToSelect).Selected = True
                        ctlPages.FindNodeByValue(TabToSelect).ExpandParentNodes()
                    Catch
                    End Try
                End If

            End If
        End Sub

        Private Sub ctlPages_NodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles ctlPages.NodeClick
            Me.ctlAjax.ResponseScripts.Add("setTab('" & e.Node.Text & "', '" & e.Node.Value & "');")
        End Sub

#End Region

#Region "Private Methods"

        Private Sub BindTree()

            Dim tabs As New List(Of TabInfo)
            tabs = TabController.GetPortalTabs(PortalSettings.PortalId, Null.NullInteger, False, "", True, False, True, True, False)

            For Each oTab As TabInfo In tabs
                If oTab.Level = 0 Then
                    Dim node As New RadTreeNode
                    node.Text = oTab.TabName
                    node.Value = oTab.TabID.ToString
                    ctlPages.Nodes.Add(node)
                    AddChildren(node)
                End If
            Next
        End Sub

        Private Sub AddChildren(ByRef treenode As RadTreeNode)
            For Each objTab As TabInfo In TabController.GetPortalTabs(PortalSettings.PortalId, Null.NullInteger, False, "", True, False, True, True, False)
                If objTab.ParentId = Integer.Parse(treenode.Value) Then
                    Dim node As New RadTreeNode
                    node.Text = objTab.TabName
                    node.Value = objTab.TabID.ToString
                    treenode.Nodes.Add(node)
                    AddChildren(node)
                End If
            Next

        End Sub

#End Region

        Private Sub btnClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClose.Click
            Me.ctlAjax.ResponseScripts.Add("dialogClose();")
        End Sub

    End Class
End Namespace
