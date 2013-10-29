Namespace dnnWerk.Modules.Nuntio.Articles
    Partial Public Class uc_Category
        Inherits ArticleModuleBase


        Private _categoryid As Integer = Null.NullInteger
        Private _moduleid As Integer = Null.NullInteger
        Private _mode As String = "add"
        Private _userid As Integer = Null.NullInteger

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            'DotNetNuke.Framework.AJAX.RegisterScriptManager()
        End Sub


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            BuildCategoryNameBoxes()

            If Not Request.QueryString("mode") Is Nothing Then

                _mode = Request.QueryString("mode")

            End If

            If Not Request.QueryString("UserId") Is Nothing Then

                _userid = Request.QueryString("UserId")

            End If

            If Not Request.QueryString("ModuleId") Is Nothing Then

                _moduleid = Convert.ToInt32(Request.QueryString("ModuleId"))

            End If

            If Not Request.QueryString("CategoryId") Is Nothing Then

                Try
                    _categoryid = Convert.ToInt32(Request.QueryString("CategoryId"))
                Catch
                End Try

                If _categoryid <> Null.NullInteger Then

                    If _mode = "edit" Then
                        BindCategory()
                    End If

                End If

            End If

            If _mode = "edit" Then
                CType(Me.Page.FindControl("lblTitle"), Literal).Text = "Edit Category"
            Else
                CType(Me.Page.FindControl("lblTitle"), Literal).Text = "Add Category"
            End If

            Me.btnDelete.Visible = (_mode = "edit")

        End Sub

        Private Sub BindCategory()

            Dim cc As New CategoryController
            For Each objLocale As Locale In SupportedLocales.Values
                Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
                Dim txtBox As TextBox
                Dim strBoxID As String = DotNetNuke.Common.Globals.CreateValidID("txtTitle_" & Convert.ToString(info.LCID))
                Dim plhBoxes As Control = Me.FindControl("plhControls")
                Dim ctlBox As System.Web.UI.Control = plhBoxes.FindControl(strBoxID)
                txtBox = CType(ctlBox, TextBox)
                txtBox.Text = cc.GetCategory(_categoryid, _moduleid, Convert.ToString(info.Name), True).CategoryName
            Next

        End Sub

        Private Sub BuildCategoryNameBoxes()


            Dim _openingTable As New Literal
            _openingTable.Text = "<table>"
            plhControls.Controls.Add(_openingTable)

            For Each objLocale As Locale In SupportedLocales.Values
                Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)

                Dim _BoxRowOpen As New Literal
                _BoxRowOpen.Text = "<tr><td align=""left"" nowrap><span class=""normal"">" & System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.DisplayName) & "</span></td><td>&nbsp;</td><td align=""left"">"
                plhControls.Controls.Add(_BoxRowOpen)

                Dim _txtBox As New TextBox
                _txtBox.ID = DotNetNuke.Common.Globals.CreateValidID("txtTitle_" & Convert.ToString(info.LCID))
                plhControls.Controls.Add(_txtBox)

                Dim _BoxRowClose As New Literal
                _BoxRowClose.Text = "</td></tr>"
                plhControls.Controls.Add(_BoxRowClose)

            Next

            Dim _closingTable As New Literal
            _closingTable.Text = "</table>"
            plhControls.Controls.Add(_closingTable)

        End Sub

        Private Sub Update()

            Dim cc As New CategoryController
            Dim c As CategoryInfo = Nothing

            Select Case _mode
                Case "add"

                    c = New CategoryInfo

                    Dim intId As Integer = Null.NullInteger
                    Dim blnAdded As Boolean = False

                    For Each objLocale As Locale In SupportedLocales.Values
                        Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
                        Dim txtBox As TextBox
                        Dim strBoxID As String = DotNetNuke.Common.Globals.CreateValidID("txtTitle_" & Convert.ToString(info.LCID))
                        Dim plhBoxes As Control = Me.FindControl("plhControls")
                        Dim ctlBox As Control = plhBoxes.FindControl(strBoxID)

                        txtBox = CType(ctlBox, TextBox)

                        c.CategoryName = txtBox.Text
                        c.Locale = Convert.ToString(info.Name)
                        c.moduleId = _moduleid
                        c.ParentID = _categoryid
                        c.PortalID = PortalSettings.PortalId
                        c.ViewOrder = 0

                        Try
                            'c.ViewOrder = Integer.Parse(Me.txtViewOrder.Text)
                        Catch
                        End Try

                        If blnAdded = False Then
                            intId = cc.AddCategory(c, _userid)
                            blnAdded = True
                        Else
                            c.CategoryID = intId
                            cc.UpdateCategoryItem(c, _userid)
                        End If

                    Next

                Case "edit"
                    c = cc.GetCategory(_categoryid, _moduleid, Me.CurrentLocale, True)

                    For Each objLocale As Locale In SupportedLocales.Values
                        Dim info As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(objLocale.Code)
                        Dim txtBox As TextBox
                        Dim strBoxID As String = DotNetNuke.Common.Globals.CreateValidID("txtTitle_" & Convert.ToString(info.LCID))
                        Dim plhBoxes As Control = Me.FindControl("plhControls")
                        Dim ctlBox As Control = plhBoxes.FindControl(strBoxID)

                        txtBox = CType(ctlBox, TextBox)

                        c.CategoryName = txtBox.Text
                        c.Locale = Convert.ToString(info.Name)
                        c.ViewOrder = 0

                        Try
                            'c.ViewOrder = Integer.Parse(Me.txtViewOrder.Text)
                        Catch
                        End Try

                        cc.UpdateCategoryItem(c, _userid)

                    Next

            End Select



        End Sub

        Private Sub Delete()

        End Sub

        Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            Me.ctlAjax.ResponseScripts.Add("CancelEdit()")
        End Sub

        Private Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
            Update()
            Me.ctlAjax.ResponseScripts.Add("CloseAndRebind()")
        End Sub

        Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
            Delete()
            Me.ctlAjax.ResponseScripts.Add("CloseAndRebind()")
        End Sub

    End Class
End Namespace
