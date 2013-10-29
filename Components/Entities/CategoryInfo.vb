Imports System
Imports System.Configuration
Imports System.Data

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class CategoryInfo

        Implements DotNetNuke.Entities.Modules.IHydratable

#Region " Private Methods "

        ' local property declarations
        Private _CategoryName As String
        Private _Locale As String
        Private _CategoryID As Integer
        Private _ParentID As Integer
        Private _moduleId As Integer
        Private _PortalID As Integer
        Private _ViewOrder As Integer
        Private _Count As Integer
        Private _IsLocaleTranslated As Boolean
        Private _IsOriginal As Boolean

#End Region

#Region " Public Properties "

        Public Property IsOriginal() As Boolean
            Get
                Return _IsOriginal
            End Get
            Set(ByVal value As Boolean)
                _IsOriginal = value
            End Set
        End Property

        Public Property IsLocaleTranslated() As Boolean
            Get
                Return _IsLocaleTranslated
            End Get
            Set(ByVal value As Boolean)
                _IsLocaleTranslated = value
            End Set
        End Property

        Public Property Count() As Integer
            Get
                Return _Count
            End Get
            Set(ByVal value As Integer)
                _Count = value
            End Set
        End Property

        Public Property ViewOrder() As Integer
            Get
                Return _ViewOrder
            End Get
            Set(ByVal value As Integer)
                _ViewOrder = value
            End Set
        End Property

        Public Property CategoryName() As String
            Get
                Return _CategoryName
            End Get
            Set(ByVal Value As String)
                _CategoryName = Value
            End Set
        End Property

        Public Property Locale() As String
            Get
                Return _Locale
            End Get
            Set(ByVal Value As String)
                _Locale = Value
            End Set
        End Property

        Public Property moduleId() As Integer
            Get
                Return _moduleId
            End Get
            Set(ByVal Value As Integer)
                _moduleId = Value
            End Set
        End Property

        Public Property CategoryID() As Integer
            Get
                Return _CategoryID
            End Get
            Set(ByVal Value As Integer)
                _CategoryID = Value
            End Set
        End Property

        Public Property ParentID() As Integer
            Get
                Return _ParentID
            End Get
            Set(ByVal Value As Integer)
                _ParentID = Value
            End Set
        End Property

        Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

#End Region

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements DotNetNuke.Entities.Modules.IHydratable.Fill

            If Not IsDBNull(dr("CategoryID")) Then
                Integer.TryParse(dr("CategoryID"), _CategoryID)
            End If

            If Not IsDBNull(dr("moduleId")) Then
                Integer.TryParse(dr("moduleId"), _moduleId)
            End If

            If Not IsDBNull(dr("PortalID")) Then
                Integer.TryParse(dr("PortalID"), _PortalID)
            End If

            If Not IsDBNull(dr("ViewOrder")) Then
                Integer.TryParse(dr("ViewOrder"), _ViewOrder)
            End If

            If Not IsDBNull(dr("ParentID")) Then
                Integer.TryParse(dr("ParentID"), _ParentID)
            End If

            If Not IsDBNull(dr("Count")) Then
                Integer.TryParse(dr("Count"), _Count)
            End If            

        End Sub

        Public Property KeyID As Integer Implements DotNetNuke.Entities.Modules.IHydratable.KeyID
            Get
                Return _CategoryID
            End Get
            Set(ByVal value As Integer)
                _CategoryID = value
            End Set
        End Property

    End Class

End Namespace

