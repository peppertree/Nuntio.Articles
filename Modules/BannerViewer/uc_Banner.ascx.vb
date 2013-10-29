Imports DotNetNuke
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports Telerik.Web.UI
Imports DotNetNuke.Security.Permissions

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Banner
        Inherits ArticleModuleBase
        Implements IActionable

#Region "Private Methods"


#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            Try

            Catch
            End Try
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                Dim blnHideOnAllNews As Boolean = Me.HideOnAllNews

                Dim sGroup As String = ""
                Dim Categories As New List(Of Integer)
                Dim cc As New CategoryController

                If Me.ArticleId <> Null.NullInteger Then
                    Categories = cc.GetRelationsByitemId(ArticleId)
                Else
                    If Me.CategoryID <> Null.NullInteger Then
                        Categories.Clear()
                        Categories.Add(Me.CategoryID)
                    Else
                        If Not blnHideOnAllNews Then
                            For Each objCat As CategoryInfo In cc.ListCategoryItems(Me.NewsModuleId, Me.CurrentLocale, Date.Now, ShowFutureItems, ShowPastItems, True)
                                Categories.Add(objCat.CategoryID)
                            Next
                        End If
                    End If
                End If

                If Categories.Count > 0 Then

                    Dim objBanners As New Services.Vendors.BannerController

                    Dim intPortalId As Integer = PortalSettings.PortalId
                    Dim intBannerTypeId As Integer = 1
                    Dim intBanners As Integer = 0
                    Dim banners As New ArrayList

                    Select Case Me.BannerSource
                        Case "G"
                            intPortalId = Null.NullInteger
                        Case "L"
                            intPortalId = PortalId
                    End Select
                    intBannerTypeId = Integer.Parse(Me.BannerType)
                    intBanners = Integer.Parse(Me.BannerCount)

                    For Each intCategory As Integer In Categories
                        Dim category As CategoryInfo = cc.GetCategory(intCategory, NewsModuleId, CurrentLocale, UseOriginalVersion)
                        If Not category Is Nothing Then
                            For Each banner As Services.Vendors.BannerInfo In objBanners.LoadBanners(intPortalId, NewsModuleId, intBannerTypeId, category.CategoryName, intBanners)
                                banners.Add(banner)
                            Next
                        End If
                    Next

                    If banners.Count > 0 Then
                        lstBanners.DataSource = banners
                        Select Case Me.TransitionType.ToLower
                            Case "scroll"
                                lstBanners.RotatorType = RotatorType.AutomaticAdvance
                            Case "slideshow"
                                lstBanners.RotatorType = RotatorType.SlideShow
                        End Select
                        lstBanners.ScrollDirection = ScrollDirection
                        lstBanners.ScrollDuration = ScrollSpeed
                        lstBanners.FrameDuration = ScrollTimeOut

                        If Me.ScrollWidth.EndsWith("%") Then
                            Try
                                Dim intWidth As Integer = Integer.Parse(ScrollWidth.Replace("%", "").Trim)
                                lstBanners.Width = Unit.Percentage(intWidth)
                            Catch
                                lstBanners.Width = Unit.Percentage(100)
                            End Try
                        ElseIf Me.ScrollWidth.EndsWith("px") Then
                            Try
                                Dim intWidth As Integer = Integer.Parse(ScrollWidth.ToLower.Replace("px", "").Trim)
                                lstBanners.Width = Unit.Pixel(intWidth)
                            Catch
                                lstBanners.Width = Unit.Pixel(400)
                            End Try
                        End If
                        If Me.ScrollHeight.EndsWith("%") Then
                            Try
                                Dim intHeight As Integer = Integer.Parse(ScrollHeight.Replace("%", "").Trim)
                                lstBanners.Height = Unit.Percentage(intHeight)
                            Catch
                                lstBanners.Height = Unit.Percentage(100)
                            End Try
                        ElseIf Me.ScrollHeight.EndsWith("px") Then
                            Try
                                Dim intHeight As Integer = Integer.Parse(ScrollHeight.ToLower.Replace("px", "").Trim)
                                lstBanners.Height = Unit.Pixel(intHeight)
                            Catch
                                lstBanners.Height = Unit.Pixel(300)
                            End Try
                        End If
                        lstBanners.DataBind()
                        Dim lit As New Literal
                        lit.Text = "Showing " & banners.Count.ToString & " banners for " & Categories.Count.ToString & " category."
                        'Me.Controls.Add(Lit)
                    Else
                        HideModule()
                    End If


                Else
                    HideModule()
                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub HideModule()

            If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then
                Me.lstBanners.Visible = False
            Else
                Dim o As Control = Me 'the module object to hide/show 
                Do
                    o = o.Parent
                    If TypeOf o Is Page Then
                        'never found an ascx parent control 
                        'we're at the page so we're done 
                        Exit Do
                    End If
                    Try
                        'if the name of the parent object is an ascx file then 
                        'this must be the parent container (we hope!) 
                        If o.ToString.IndexOf("_ascx") <> -1 Then
                            o.Visible = False 'set to hide/show the container of oControl 
                            Exit Do
                        End If
                    Catch
                    End Try
                Loop
            End If




        End Sub

#Region "Public Methods"
        Public Function FormatItem(ByVal VendorId As Integer, ByVal BannerId As Integer, ByVal BannerTypeId As Integer, ByVal BannerName As String, ByVal ImageFile As String, ByVal Description As String, ByVal URL As String, ByVal Width As Integer, ByVal Height As Integer) As String
            Dim objBanners As New Services.Vendors.BannerController
            Return objBanners.FormatBanner(VendorId, BannerId, BannerTypeId, BannerName, ImageFile, Description, URL, Width, Height, Me.BannerSource, PortalSettings.HomeDirectory)
        End Function
#End Region

#End Region

#Region "Optional Interfaces"
        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Localize("BANNEROPTIONS.Action"), ModuleActionType.EditContent, "", "icon_hostsettings_16px.gif", NavigateURL(TabId, "BannerOptions", "mid=" & ModuleId.ToString), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property
#End Region

    End Class
End Namespace
