Imports System
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class TemplateHelp

        Private _token As String = ""
        Private _helptext As String = ""

        Public Property Token() As String
            Get
                Return _token
            End Get
            Set(ByVal value As String)
                _token = value
            End Set
        End Property

        Public Property HelpText() As String
            Get
                Return _helptext
            End Get
            Set(ByVal value As String)
                _helptext = value
            End Set
        End Property

    End Class
End Namespace

