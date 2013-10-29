
Imports Telerik.Web.UI

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports Telerik.Web.UI.Editor.DialogControls

Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_FileSelect
        Inherits ArticleModuleBase

#Region "Private Members"


#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


            CType(Me.Page.FindControl("lblTitle"), Literal).Text = Localize("SelectFile")

            Dim path As String = RemoveTrailingSlash(PortalSettings.HomeDirectory)




            Me.ctlFiles.Configuration.SearchPatterns = New String() {"*.*"}
            Me.ctlFiles.Configuration.ViewPaths = New String() {path}
            Me.ctlFiles.Configuration.DeletePaths = New String() {path}
            Me.ctlFiles.Configuration.MaxUploadFileSize = 604800000
            Me.ctlFiles.Configuration.UploadPaths = New String() {path}
            Me.ctlFiles.LocalizationPath = Me.ModuleDirectory & "/Controls/App_LocalResources/"
            Me.ctlFiles.Language = CurrentLocale

            Me.ctlFiles.Configuration.ContentProviderTypeName = "DotNetNuke.Providers.RadEditorProvider.TelerikFileBrowserProvider, DotNetNuke.RadEditorProvider"

            ctlFiles.OnClientFileOpen = "OnFileLinkSelect"
            ctlFiles.Configuration.SearchPatterns = New String() {"*.*"}


            Dim FileToSelect As String = Utilities.Null.NullString
            If Not Request.QueryString("Select") Is Nothing Then
                Try
                    FileToSelect = Request.QueryString("Select")
                Catch
                End Try
            End If

            If FileToSelect <> Utilities.Null.NullString And Not FileToSelect.ToLower = "none" Then
                Try
                    ctlFiles.InitialPath = ResolveUrl("~" & FileToSelect)
                Catch
                End Try
            End If

            'AddGridColumn("", "Thumb", False)

        End Sub

        Private Sub AddGridColumn(ByVal name As String, ByVal uniqueName As String, ByVal sortable As Boolean)
            RemoveGridColumn(uniqueName)
            Dim gridTemplateColumn1 As New GridTemplateColumn()
            gridTemplateColumn1.HeaderText = name
            If sortable Then
                gridTemplateColumn1.SortExpression = uniqueName
            End If
            gridTemplateColumn1.UniqueName = uniqueName
            gridTemplateColumn1.DataField = uniqueName
            ctlFiles.Grid.Columns.Add(gridTemplateColumn1)
        End Sub

        Private Sub RemoveGridColumn(ByVal uniqueName As String)
            If Not [Object].Equals(ctlFiles.Grid.Columns.FindByUniqueNameSafe(uniqueName), Nothing) Then
                ctlFiles.Grid.Columns.Remove(ctlFiles.Grid.Columns.FindByUniqueNameSafe(uniqueName))
            End If
        End Sub

        Private Function RemoveTrailingSlash(ByVal path As String) As String
            If path.EndsWith("/") Then
                Return path.Substring(0, path.Length - 1)
            End If
            Return path
        End Function

#End Region

#Region "Private Methods"



#End Region


    End Class
End Namespace
