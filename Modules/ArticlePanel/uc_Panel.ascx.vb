
Imports Telerik.Web.UI

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions
Imports dnnWerk.Modules.Nuntio.Articles.Helpers
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Panel
        Inherits ArticleModuleBase

#Region "Private Members"

        Private ReadOnly Property ArticleTabId As Integer
            Get
                If drpModules.SelectedValue.Contains(";") Then
                    Return Convert.ToInt32(drpModules.SelectedValue.Split(Char.Parse(";"))(0))
                End If
                Return Null.NullInteger
            End Get
        End Property

        Private ReadOnly Property ArticleModuleId As Integer
            Get
                If drpModules.SelectedValue.Contains(";") Then
                    Return Convert.ToInt32(drpModules.SelectedValue.Split(Char.Parse(";"))(1))
                End If
                Return Null.NullInteger
            End Get
        End Property

        Private ReadOnly Property GridViewMode As String
            Get
                Select Case drpView.SelectedIndex
                    Case 0
                        Return "P"
                    Case 1
                        Return "A"
                End Select
                Return "P"
            End Get
        End Property

        Public Property IsSearchResult() As Boolean
            Get
                Return Me.chkIsSearchResult.Checked
            End Get
            Set(ByVal value As Boolean)
                Me.chkIsSearchResult.Checked = value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            RegisterScripts()



        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            LocalizeForm()

            If Not Page.IsPostBack Then

                BindModules()
                BindFilters()



                If Settings.Contains("DefaultModule") Then
                    Try
                        Dim value As String = CType(Settings("DefaultModule"), String)
                        drpModules.SelectedValue = value
                    Catch
                    End Try
                End If
                If Settings.Contains("DefaultFilter") Then
                    Try
                        Dim index As Integer = Convert.ToInt32(Settings("DefaultFilter"))
                        drpFilter.Items(index).Selected = True
                    Catch
                    End Try
                End If
                If Settings.Contains("DefaultMode") Then
                    Try
                        Dim index As Integer = Convert.ToInt32(Settings("DefaultMode"))
                        drpView.Items(index).Selected = True
                    Catch
                    End Try
                End If


            End If


            Me.ctlSearch.NewsModuleID = ArticleModuleId
            Me.ctlSearch.ShowFutureItems = (drpFilter.SelectedIndex = 1)
            Me.ctlSearch.ShowPastItems = (drpFilter.SelectedIndex = 0)
            Me.ctlSearch.IncludeFeaturedItems = (drpFilter.SelectedIndex = 5)
            Me.ctlSearch.IncludeNonFeaturedItems = (drpFilter.SelectedIndex = 0)
            Me.ctlSearch.IncludePublications = (Me.GridViewMode = "P")
            Me.ctlSearch.IncludeNonPublications = (Me.GridViewMode = "A")
            Me.ctlSearch.UseOriginalVersion = True
            Me.ctlSearch.CurrentLocale = Me.CurrentLocale
            Me.ctlSearch.RowCount = 25

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            SetupGrid()
        End Sub

        Private Sub drpModules_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles drpModules.SelectedIndexChanged
            grdArticles.Rebind()
        End Sub

        Private Sub drpView_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles drpView.SelectedIndexChanged
            grdArticles.Rebind()
        End Sub

        Private Sub RebindFilters()
            BindFilters()
            If Not Session("NP_SelectedFilter") Is Nothing Then
                If IsNumeric(Session("NP_SelectedFilter")) Then
                    drpFilter.SelectedIndex = Convert.ToInt32(Session("NP_SelectedFilter"))
                End If
            End If
        End Sub

        Private Sub drpFilter_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles drpFilter.SelectedIndexChanged
            Session("NP_SelectedFilter") = drpFilter.SelectedIndex.ToString
            grdArticles.Rebind()
        End Sub

        Private Sub ctlAjax_AjaxRequest(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs) Handles ctlAjax.AjaxRequest

            Dim strAction As String = ""
            Dim strArgument As String = ""

            If e.Argument.Contains(",") Then
                strAction = e.Argument.Split(Char.Parse(","))(0)
                strArgument = e.Argument.Split(Char.Parse(","))(1)
            End If

            If strAction.ToLower = "refresh" Then
                ClearCache()
                RebindFilters()
                grdArticles.Rebind()
            End If

            If strAction.ToLower = "delete" Then
                If IsNumeric(strArgument) Then

                    ClearCache()
                    ClearArticleCache()

                    Dim ArticleId As Integer = Convert.ToInt32(strArgument)
                    ArticleController.DeleteArticle(ArticleId)
                    RebindFilters()
                    grdArticles.Rebind()

                End If
            End If

            If strAction.ToLower = "harddelete" Then
                If IsNumeric(strArgument) Then

                    ClearCache()
                    ClearArticleCache()

                    Dim ArticleId As Integer = Convert.ToInt32(strArgument)
                    ArticleController.HardDeleteArticle(ArticleId, ArticleModuleId)
                    RebindFilters()
                    grdArticles.Rebind()

                End If
            End If

            If strAction.ToLower = "restore" Then
                If IsNumeric(strArgument) Then

                    ClearCache()
                    ClearArticleCache()

                    Dim ArticleId As Integer = Convert.ToInt32(strArgument)
                    ArticleController.RestoreArticle(ArticleId)
                    RebindFilters()
                    grdArticles.Rebind()

                End If
            End If

        End Sub

        Private Sub grdArticles_DetailTableDataBind(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridDetailTableDataBindEventArgs) Handles grdArticles.DetailTableDataBind

            Dim parentitem As GridDataItem = e.DetailTableView.ParentItem
            Dim parentid As Integer = Convert.ToInt32(parentitem.GetDataKeyValue("ItemId"))

            e.DetailTableView.DataSource = ArticleController.GetArticlesInPublication(parentid, ArticleModuleId, True, True, Date.Now, CurrentLocale)

        End Sub

        Private Sub grdArticles_NeedDataSource(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grdArticles.NeedDataSource

            Dim dtCurrent As DateTime = Date.Now

            Dim intRows As Integer = RowCount
            Dim _year As Integer = Null.NullInteger
            Dim _month As Integer = Null.NullInteger

            Dim mid As Integer = ArticleModuleId
            Dim lst As New List(Of ArticleInfo)
            Dim _totalcount As Integer = Null.NullInteger
            Dim _pageindex As Integer = 0

            Select Case drpFilter.SelectedIndex
                Case 0 'Published

                    Dim blnIncludeNonPublications As Boolean = (GridViewMode = "A")
                    Dim strCacheKey As String = "NUNTIO_ARTICLES_LIST_PUBLISHED_MODULEID" & ArticleModuleId.ToString
                    If blnIncludeNonPublications = False Then
                        strCacheKey += "_PUBLICATIONS"
                    End If

                    Dim blnLoaded As Boolean = False
                    If Not DotNetNuke.Common.Utilities.DataCache.GetCache(strCacheKey) Is Nothing Then
                        Try
                            lst = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(strCacheKey), List(Of ArticleInfo))
                            blnLoaded = True
                        Catch
                            blnLoaded = False
                        End Try
                    End If

                    If blnLoaded = False Then
                        lst = ArticleController.GetArticlesPaged(_totalcount, mid, Me.CurrentLocale, -1, _pageindex, dtCurrent, _month, _year, ArticleSortOrder.publishdatedesc, New List(Of Integer), UseOriginalVersion, AuthorId, False, True, True, True, True, blnIncludeNonPublications)
                        DataCache.SetCache(strCacheKey, lst)
                    End If

                Case 1 'Not yet published

                    Dim blnIncludeNonPublications As Boolean = (GridViewMode = "A")
                    Dim strCacheKey As String = "NUNTIO_ARTICLES_LIST_NOTYETPUBLISHED_MODULEID" & ArticleModuleId.ToString
                    If blnIncludeNonPublications = False Then
                        strCacheKey += "_PUBLICATIONS"
                    End If

                    Dim blnLoaded As Boolean = False
                    If Not DotNetNuke.Common.Utilities.DataCache.GetCache(strCacheKey) Is Nothing Then
                        Try
                            lst = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(strCacheKey), List(Of ArticleInfo))
                            blnLoaded = True
                        Catch
                            blnLoaded = False
                        End Try
                    End If

                    If blnLoaded = False Then
                        lst = ArticleController.GetArticlesPaged(_totalcount, mid, Me.CurrentLocale, -1, _pageindex, dtCurrent, _month, _year, ArticleSortOrder.publishdatedesc, New List(Of Integer), UseOriginalVersion, AuthorId, True, False, True, True, True, blnIncludeNonPublications)
                        DataCache.SetCache(strCacheKey, lst)
                    End If

                Case 2 'needs reviewing

                    Dim blnLoaded As Boolean = False
                    If Not DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_NEEDSREVIEWING_MODULEID" & ArticleModuleId.ToString) Is Nothing Then
                        Try
                            lst = CType(DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_NEEDSREVIEWING_MODULEID" & ArticleModuleId.ToString), List(Of ArticleInfo))
                            blnLoaded = True
                        Catch
                            blnLoaded = False
                        End Try
                    End If

                    If blnLoaded = False Then
                        lst = ArticleController.GetNeedsReviewing(ArticleModuleId, PortalId, CurrentLocale, Date.Now)
                        DataCache.SetCache("NUNTIO_ARTICLES_LIST_NEEDSREVIEWING_MODULEID" & ArticleModuleId.ToString, lst)
                    End If

                Case 3 'waiting approval   

                    Dim blnLoaded As Boolean = False
                    If Not DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_WAITINGAPPROVAL_MODULEID" & ArticleModuleId.ToString) Is Nothing Then
                        Try
                            lst = CType(DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_WAITINGAPPROVAL_MODULEID" & ArticleModuleId.ToString), List(Of ArticleInfo))
                            blnLoaded = True
                        Catch
                            blnLoaded = False
                        End Try
                    End If

                    If blnLoaded = False Then
                        lst = ArticleController.GetUnapprovedArticles(ArticleModuleId, PortalId, CurrentLocale)
                        DataCache.SetCache("NUNTIO_ARTICLES_LIST_WAITINGAPPROVAL_MODULEID" & ArticleModuleId.ToString, lst)
                    End If

                Case 4 'expired

                    Dim blnLoaded As Boolean = False
                    If Not DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_EXPIRED_MODULEID" & ArticleModuleId.ToString) Is Nothing Then
                        Try
                            lst = CType(DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_EXPIRED_MODULEID" & ArticleModuleId.ToString), List(Of ArticleInfo))
                            blnLoaded = True
                        Catch
                            blnLoaded = False
                        End Try
                    End If

                    If blnLoaded = False Then
                        lst = ArticleController.GetExpiredArticles(ArticleModuleId, PortalId, CurrentLocale)
                        DataCache.SetCache("NUNTIO_ARTICLES_LIST_EXPIRED_MODULEID" & ArticleModuleId.ToString, lst)
                    End If

                Case 5 'featured

                    Dim blnIncludeNonPublications As Boolean = (GridViewMode = "A")
                    Dim strCacheKey As String = "NUNTIO_ARTICLES_LIST_FEATURED_MODULEID" & ArticleModuleId.ToString
                    If blnIncludeNonPublications = False Then
                        strCacheKey += "_PUBLICATIONS"
                    End If

                    Dim blnLoaded As Boolean = False
                    If Not DotNetNuke.Common.Utilities.DataCache.GetCache(strCacheKey) Is Nothing Then
                        Try
                            lst = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(strCacheKey), List(Of ArticleInfo))
                            blnLoaded = True
                        Catch
                            blnLoaded = False
                        End Try
                    End If

                    If blnLoaded = False Then
                        lst = ArticleController.GetArticlesPaged(_totalcount, mid, Me.CurrentLocale, -1, _pageindex, dtCurrent, _month, _year, ArticleSortOrder.publishdatedesc, New List(Of Integer), UseOriginalVersion, AuthorId, True, True, True, False, True, blnIncludeNonPublications)
                        DataCache.SetCache(strCacheKey, lst)
                    End If

                Case 6

                    Dim blnLoaded As Boolean = False
                    If Not DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_DELETED_MODULEID" & ArticleModuleId.ToString) Is Nothing Then
                        Try
                            lst = CType(DotNetNuke.Common.Utilities.DataCache.GetCache("NUNTIO_ARTICLES_LIST_DELETED_MODULEID" & ArticleModuleId.ToString), List(Of ArticleInfo))
                            blnLoaded = True
                        Catch
                            blnLoaded = False
                        End Try
                    End If

                    If blnLoaded = False Then
                        lst = ArticleController.GetDeleted(ArticleModuleId, PortalId, CurrentLocale)
                        DataCache.SetCache("NUNTIO_ARTICLES_LIST_DELETED_MODULEID" & ArticleModuleId.ToString, lst)
                    End If

            End Select

            grdArticles.DataSource = lst

        End Sub

        Public Sub drpRelatedPublicationItem_ItemsRequested(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs)


            Dim combo As RadComboBox = CType(sender, RadComboBox)
            Dim textbox As TextBox = CType(combo.Parent.FindControl("txtArticleId"), TextBox)

            combo.Items.Clear()

            Dim sql As String = "Select * from " & DotNetNuke.Data.DataProvider.Instance().ObjectQualifier & "pnc_Localization_LocalizedItems where ModuleId = " & ArticleModuleId.ToString & " and Content Like '%" & e.Text & "%' and [Key] = 'TITLE' and IsApproved = 1 And SourceItemId <> " & textbox.Text
            Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance().ExecuteSQL(sql)
            If Not dr Is Nothing Then
                While dr.Read
                    CType(sender, RadComboBox).Items.Add(New RadComboBoxItem(Convert.ToString(dr("Content")), Convert.ToString(dr("SourceItemId"))))
                End While
                dr.Close()
                dr.Dispose()
            End If

        End Sub

        Public Sub btnManagePublicationRelation_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)

            Dim ArticleId As Integer = Convert.ToInt32(CType(sender, ImageButton).CommandArgument)
            Dim objArticle As ArticleInfo = ArticleController.GetArticle(ArticleId, CurrentLocale, True)

            Dim RelatedArticleId As Integer = Null.NullInteger
            Dim objRelatedArticle As ArticleInfo = ArticleController.GetArticle(ArticleId, CurrentLocale, True)

            Dim objComboBox As RadComboBox = Nothing
            Dim objParentContainer As Control = CType(sender, ImageButton).Parent

            If Not objParentContainer Is Nothing Then
                objComboBox = CType(objParentContainer.FindControl("drpRelatedPublicationItem"), RadComboBox)
            End If

            If Not objComboBox Is Nothing Then
                Integer.TryParse(objComboBox.SelectedValue, RelatedArticleId)
            End If

            If RelatedArticleId <> Null.NullInteger Then
                objRelatedArticle = ArticleController.GetArticle(RelatedArticleId, CurrentLocale, True)
            End If

            If objArticle.IsPublication Then
                If Not objRelatedArticle.IsPublication Then
                    objRelatedArticle.ParentId = objArticle.ItemId
                    ArticleController.UpdateArticle(objRelatedArticle)
                End If
            Else
                If objArticle.ParentId <> Null.NullInteger Then
                    objArticle.ParentId = Null.NullInteger
                Else
                    objArticle.ParentId = objRelatedArticle.ItemId
                End If
                ArticleController.UpdateArticle(objArticle)
            End If

            grdArticles.Rebind()

        End Sub

#End Region

#Region "Private Methods"

        Private Sub ClearCache()

            DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_PUBLISHED_MODULEID" & ArticleModuleId.ToString)
            DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_NOTYETPUBLISHED_MODULEID" & ArticleModuleId.ToString)
            DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_WAITINGAPPROVAL_MODULEID" & ArticleModuleId.ToString)
            DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_EXPIRED_MODULEID" & ArticleModuleId.ToString)
            DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_FEATURED_MODULEID" & ArticleModuleId.ToString)
            DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_DELETED_MODULEID" & ArticleModuleId.ToString)
            DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_PUBLISHED_MODULEID" & ArticleModuleId.ToString & "_PUBLICATIONS")
            DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_NOTYETPUBLISHED_MODULEID" & ArticleModuleId.ToString & "_PUBLICATIONS")
            DataCache.RemoveCache("NUNTIO_ARTICLES_LIST_FEATURED_MODULEID" & ArticleModuleId.ToString & "_PUBLICATIONS")

        End Sub

        Private Sub SetupGrid()
            Select Case drpView.SelectedIndex
                Case 0
                    grdArticles.MasterTableView.HierarchyLoadMode = GridChildLoadMode.ServerOnDemand
                Case 1

                    Dim nestedViewItems As GridItem() = grdArticles.MasterTableView.GetItems(GridItemType.NestedView)
                    For Each nestedViewItem As GridNestedViewItem In nestedViewItems
                        For Each nestedView As GridTableView In nestedViewItem.NestedTableViews
                            Dim cell As TableCell = nestedView.ParentItem("ExpandColumn")
                            cell.Controls(0).Visible = False
                            cell.Text = " "
                            nestedViewItem.Visible = False
                        Next
                    Next
            End Select
        End Sub

        Private Sub BindFilters()

            drpFilter.Items.Clear()

            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_Published")))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_NotYetPublished") & " (" & ArticleController.GetNotYetPublishedCount(ArticleModuleId, PortalId) & ")"))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_NeedsReviewing") & " (" & ArticleController.GetNeedsReviewingCount(ArticleModuleId, PortalId, Date.Now) & ")"))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_AwaitingApproval") & " (" & ArticleController.GetUnapprovedArticleCount(ArticleModuleId, PortalId) & ")"))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_Expired") & " (" & ArticleController.GetExpiredArticleCount(ArticleModuleId, PortalId) & ")"))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_Featured") & " (" & ArticleController.GetFeaturedCount(ArticleModuleId, PortalId) & ")"))
            drpFilter.Items.Add(New RadComboBoxItem(Localize("Filter_Deleted") & " (" & ArticleController.GetDeletedCount(ArticleModuleId, PortalId) & ")"))

            drpView.Items.Clear()
            drpView.Items.Add(New RadComboBoxItem(Localize("ViewMode_Publications")))
            drpView.Items.Add(New RadComboBoxItem(Localize("ViewMode_Articles")))

        End Sub

        Private Sub BindModules()

            drpModules.Items.Clear()


            Dim tabs As List(Of DotNetNuke.Entities.Tabs.TabInfo) = TabController.GetPortalTabs(PortalSettings.PortalId, Null.NullInteger, False, "", True, False, False, False, False)

            Dim objModules As New ModuleController
            Dim arrModules As New ArrayList
            Dim objModule As ModuleInfo

            Dim dicModules As System.Collections.Generic.Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo)

            For Each oTab As DotNetNuke.Entities.Tabs.TabInfo In tabs
                dicModules = objModules.GetTabModules(oTab.TabID)
                For Each objModule In dicModules.Values
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "VIEW", objModule) And objModule.IsDeleted = False Then
                        If IsArticleModule(objModule) Then
                            drpModules.Items.Add(New RadComboBoxItem("[" & oTab.TabName & "] - " & objModule.ModuleTitle, oTab.TabID.ToString & ";" & objModule.ModuleID.ToString))
                        End If
                    End If
                Next
            Next

            If drpModules.Items.Count = 0 Then
                drpModules.Items.Add(New RadComboBoxItem("[No article module found...]", "-1"))
            End If

        End Sub

        Private Sub RegisterScripts()

            Dim strScript As String = "<script type=""text/javascript""> " & vbCrLf
            strScript += "  function Nuntio_Refresh_" & ModuleId.ToString & "() {" & vbCrLf
            strScript += "      $find(""" & ctlAjax.ClientID & """).ajaxRequest('refresh,0');" & vbCrLf
            strScript += "  }" & vbCrLf
            strScript += "  function Nuntio_Delete_" & ModuleId.ToString & "(articleid) {" & vbCrLf
            strScript += "      $find(""" & ctlAjax.ClientID & """).ajaxRequest('delete,' + articleid);" & vbCrLf
            strScript += "  }" & vbCrLf
            strScript += "  function Nuntio_HardDelete_" & ModuleId.ToString & "(articleid) {" & vbCrLf
            strScript += "      $find(""" & ctlAjax.ClientID & """).ajaxRequest('harddelete,' + articleid);" & vbCrLf
            strScript += "  }" & vbCrLf
            strScript += "  function Nuntio_Restore_" & ModuleId.ToString & "(articleid) {" & vbCrLf
            strScript += "      $find(""" & ctlAjax.ClientID & """).ajaxRequest('restore,' + articleid);" & vbCrLf
            strScript += "  }" & vbCrLf
            strScript += "</script>" & vbCrLf

            'RadScriptBlock1.Controls.Add(New LiteralControl(strScript))

            Page.ClientScript.RegisterStartupScript(Me.[GetType](), "Nuntio_List_Refresh_" & ModuleId.ToString, strScript)

            DotNetNuke.Framework.AJAX.RegisterScriptManager()

        End Sub

        Private Sub LocalizeForm()

            grdArticles.MasterTableView.NoMasterRecordsText = Localize("NoArticles")
            grdArticles.MasterTableView.DetailTables(0).NoDetailRecordsText = Localize("NoArticlesInPublication.Text")
            grdArticles.MasterTableView.DetailTables(0).ShowHeadersWhenNoRecords = False

            grdArticles.MasterTableView.Columns(0).HeaderText = Localize("Title.Head")
            grdArticles.MasterTableView.Columns(1).HeaderText = Localize("PublishDate.Head")
            grdArticles.MasterTableView.Columns(2).HeaderText = Localize("ExpiryDate.Head")
            grdArticles.MasterTableView.Columns(3).HeaderText = Localize("ReviewDate.Head")

            lblStatus.Text = Localize("ArticleStatus")
            lblView.Text = Localize("GridViewMode")

        End Sub

#End Region

#Region "Public Methods"

        Public Function GetContent(ByVal strContent As String, ByVal strSummary As String) As String

            strContent = StripHtml(Server.HtmlDecode(strContent))
            strSummary = StripHtml(Server.HtmlDecode(strSummary))

            If strContent.Length > 300 Then
                Return strContent.Substring(0, 300) & "..."
            ElseIf strContent.Length > 0 Then
                Return strContent
            End If

            If strSummary.Length > 300 Then
                Return strSummary.Substring(0, 300) & "..."
            ElseIf strSummary.Length > 0 Then
                Return strSummary
            End If

            Return ""

        End Function

        Public Function GetAuthor(ByVal AuthorId As Integer) As String

            Dim objUser As UserInfo = UserController.GetUserById(PortalId, AuthorId)

            If Not objUser Is Nothing Then
                Return objUser.DisplayName
            End If

            Return "n/a"

        End Function

        Public Function GetDate(ByVal objDate As Object) As String

            Dim dt As DateTime = Null.NullDate
            Date.TryParse(objDate.ToString, dt)

            If dt <> Null.NullDate Then
                Return dt.ToShortDateString
            End If

            Return "-"

        End Function

        Public Function GetArticleUrl(ByVal ArticleId As String) As String

            Dim objItem As ArticleInfo = ArticleController.GetArticle(ArticleId, CurrentLocale, True)
            If Not objItem Is Nothing Then
                Return Navigate(ArticleTabId, False, Helpers.OnlyAlphaNumericChars(objItem.Title) & ".aspx", "ArticleId=" & ArticleId.ToString)
            End If

            Return "#"

        End Function

        Public Function GetEditUrl(ByVal ArticleId As String) As String

            Return "javascript:NuntioArticlesEditForm('" & ArticleId & "','" & ArticleModuleId.ToString & "','" & ModuleId.ToString & "')"

        End Function

        Public Function GetRestoreUrl(ByVal ArticleId As String) As String

            Return "javascript:Nuntio_Restore_" & ModuleId.ToString & "(" & ArticleId & ")"

        End Function

        Public Function GetDeleteUrl(ByVal ArticleId As String) As String

            Return "javascript:Nuntio_Delete_" & ModuleId.ToString & "(" & ArticleId & ")"

        End Function

        Public Function GetHardDeleteUrl(ByVal ArticleId As String) As String

            Return "javascript:Nuntio_HardDelete_" & ModuleId.ToString & "(" & ArticleId & ")"

        End Function

        Public Function GetManageResource(ByVal IsPublication As Object, ByVal ParentId As Object) As String

            Dim intParent As Integer = Null.NullInteger
            Dim blnIsPublication As Boolean = False

            Boolean.TryParse(IsPublication.ToString, blnIsPublication)
            Integer.TryParse(ParentId.ToString, intParent)

            If blnIsPublication Then
                Return Localize("AddArticleToPublication")
            Else
                If intParent <> Null.NullInteger Then
                    Return Localize("RemoveFromPublication")
                Else
                    Return Localize("AddToPublication")
                End If
            End If

        End Function

        Public Function ManageRelationButtonUrl(ByVal ParentId As Object) As String

            Dim intParent As Integer = Null.NullInteger
            Integer.TryParse(ParentId.ToString, intParent)

            If intParent <> Null.NullInteger Then
                Return ImagesDirectory & "nuntio_delete.png"
            Else
                Return ImagesDirectory & "nuntio_add.png"
            End If

        End Function

        Public Function GetArticleTypeResource(ByVal IsPublication As Object, ByVal ParentId As Object, ByVal PublicationTitle As Object, ByVal Articles As Object) As String

            Dim intParent As Integer = Null.NullInteger
            Dim blnIsPublication As Boolean = False

            Boolean.TryParse(IsPublication.ToString, blnIsPublication)
            Integer.TryParse(ParentId.ToString, intParent)

            If blnIsPublication Then
                Return String.Format(Localize("Type_ArticleIsPublication"), Articles.ToString)
            Else
                If intParent <> Null.NullInteger Then
                    Return String.Format(Localize("Type_ArticleBelongsToPublication"), PublicationTitle.ToString)
                Else
                    Return Localize("Type_ArticleIsArticle")
                End If
            End If

        End Function

#End Region

        Private Sub ctlSearch_SearchResultsBound(sender As Object, e As SearchResultEventArgs) Handles ctlSearch.SearchResultsBound

            If e.ContainsResult Then

                Me.IsSearchResult = True
                grdArticles.DataSource = e.SearchResult

            End If

            grdArticles.Rebind()

        End Sub
    End Class
End Namespace
