Imports System
Imports System.Configuration
Imports System.Data

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class ArchiveInfo

        Implements DotNetNuke.Entities.Modules.IHydratable

#Region " Private Methods "

        ' local property declarations
        Private _day As Integer
        Private _month As Integer
        Private _year As Integer
        Private _count As Integer
        Private _keyid As Integer

#End Region

#Region " Public Properties "

        Public Property Day() As Integer
            Get
                Return _day
            End Get
            Set(ByVal Value As Integer)
                _day = Value
            End Set
        End Property

        Public Property Month() As Integer
            Get
                Return _month
            End Get
            Set(ByVal Value As Integer)
                _month = Value
            End Set
        End Property

        Public Property Year() As Integer
            Get
                Return _year
            End Get
            Set(ByVal Value As Integer)
                _year = Value
            End Set
        End Property

        Public Property Count() As Integer
            Get
                Return _count
            End Get
            Set(ByVal Value As Integer)
                _count = Value
            End Set
        End Property

#End Region

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements DotNetNuke.Entities.Modules.IHydratable.Fill

            If Not IsDBNull(dr("Day")) Then
                Integer.TryParse(dr("Day"), _day)
            End If

            If Not IsDBNull(dr("Month")) Then
                Integer.TryParse(dr("Month"), _month)
            End If

            If Not IsDBNull(dr("Year")) Then
                Integer.TryParse(dr("Year"), _year)
            End If

            If Not IsDBNull(dr("Count")) Then
                Integer.TryParse(dr("Count"), _count)
            End If         

        End Sub

        Public Property KeyID As Integer Implements DotNetNuke.Entities.Modules.IHydratable.KeyID
            Get
                Return _keyid
            End Get
            Set(ByVal value As Integer)
                _keyid = value
            End Set
        End Property

    End Class

End Namespace

