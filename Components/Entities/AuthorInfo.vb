Imports System
Imports System.Configuration
Imports System.Data

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class AuthorInfo

        Implements DotNetNuke.Entities.Modules.IHydratable

#Region " Private Methods "

        ' local property declarations
        Private _userid As Integer
        Private _displayname As String
        Private _articles As Integer

#End Region

#Region " Public Properties "

        Public Property userId() As Integer
            Get
                Return _userid
            End Get
            Set(ByVal value As Integer)
                _userid = value
            End Set
        End Property

        Public Property DisplayName() As String
            Get
                Return _displayname
            End Get
            Set(ByVal Value As String)
                _displayname = Value
            End Set
        End Property

        Public Property Articles() As Integer
            Get
                Return _articles
            End Get
            Set(ByVal value As Integer)
                _articles = value
            End Set
        End Property


#End Region

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements DotNetNuke.Entities.Modules.IHydratable.Fill

            If Not IsDBNull(dr("Displayname")) Then
                _displayname = Convert.ToString(dr("Displayname"))
            End If

            If Not IsDBNull(dr("UserId")) Then
                Integer.TryParse(dr("UserId"), _userid)
            End If

            If Not IsDBNull(dr("Articles")) Then
                Integer.TryParse(dr("Articles"), _articles)
            End If

        End Sub

        Public Property KeyID As Integer Implements DotNetNuke.Entities.Modules.IHydratable.KeyID
            Get
                Return _userid
            End Get
            Set(ByVal value As Integer)
                _userid = value
            End Set
        End Property

    End Class

End Namespace

