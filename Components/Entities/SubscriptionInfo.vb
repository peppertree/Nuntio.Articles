Imports System
Imports System.Configuration
Imports System.Data

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class SubscriptionInfo

        Implements DotNetNuke.Entities.Modules.IHydratable

#Region " Private Members "

        ' local property declarations
        Private _email As String
        Private _name As String
        Private _locale As String
        Private _itemid As Integer
        Private _userid As Integer = Null.NullInteger
        Private _key As String
        Private _datecreated As DateTime
        Private _verified As Boolean = False
        Private _moduleid As Integer = Null.NullInteger
        Private _moduletitle As String = ""

#End Region

#Region " Public Properties "

        Public Sub New()
        End Sub

        Public Sub New(ByVal userid As Integer, ByVal moduleid As Integer)
            Me._userid = userid
            Me._datecreated = Date.Now
            Me._verified = True
            Me._moduleid = moduleid
        End Sub

        Public Sub New(ByVal email As String, ByVal name As String, ByVal locale As String, ByVal moduleid As Integer)
            Me._email = email
            Me._name = name
            Me._locale = locale
            Me._datecreated = Date.Now
            Me._verified = False
            Me._moduleid = moduleid
        End Sub

        Public Property ModuleTitle() As String
            Get
                Return _moduletitle
            End Get
            Set(ByVal value As String)
                _moduletitle = value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return _moduleid
            End Get
            Set(ByVal value As Integer)
                _moduleid = value
            End Set
        End Property

        Public Property DateCreated() As DateTime
            Get
                Return _datecreated
            End Get
            Set(ByVal value As DateTime)
                _datecreated = value
            End Set
        End Property

        Public Property ItemId() As Integer
            Get
                Return _itemid
            End Get
            Set(ByVal Value As Integer)
                _itemid = Value
            End Set
        End Property
        Public Property Email() As String
            Get
                Return _email
            End Get
            Set(ByVal Value As String)
                _email = Value
            End Set
        End Property
        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal Value As String)
                _name = Value
            End Set
        End Property
        Public Property Locale() As String
            Get
                Return _locale
            End Get
            Set(ByVal Value As String)
                _locale = Value
            End Set
        End Property
        Public Property UserID() As Integer
            Get
                Return _userid
            End Get
            Set(ByVal Value As Integer)
                _userid = Value
            End Set
        End Property
        Public Property Verified() As Boolean
            Get
                Return _verified
            End Get
            Set(ByVal Value As Boolean)
                _verified = Value
            End Set
        End Property
        Public Property Key() As String
            Get
                Return _key
            End Get
            Set(ByVal Value As String)
                _key = Value
            End Set
        End Property

#End Region


        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements DotNetNuke.Entities.Modules.IHydratable.Fill

            If Not dr("Email") Is Nothing Then
                If Not IsDBNull(dr("Email")) Then
                    _email = Convert.ToString(dr("Email"))
                End If
            End If

            If Not dr("ModuleId") Is Nothing Then
                If Not IsDBNull(dr("ModuleId")) Then
                    Integer.TryParse(dr("ModuleId"), _moduleid)
                End If
            End If

            If Not dr("ItemID") Is Nothing Then
                If Not IsDBNull(dr("ItemID")) Then
                    Integer.TryParse(dr("ItemID"), _itemid)
                End If
            End If

            If Not dr("Key") Is Nothing Then
                If Not IsDBNull(dr("Key")) Then
                    _key = Convert.ToString(dr("Key"))
                End If
            End If

            If Not dr("Locale") Is Nothing Then
                If Not IsDBNull(dr("Locale")) Then
                    _locale = Convert.ToString(dr("Locale"))
                End If
            End If

            If Not dr("Name") Is Nothing Then
                If Not IsDBNull(dr("Name")) Then
                    _name = Convert.ToString(dr("Name"))
                End If
            End If

            Try
                If Not dr("UserID") Is Nothing Then
                    If Not IsDBNull(dr("UserID")) Then
                        Integer.TryParse(dr("UserID"), _userid)
                    End If
                End If
            Catch
            End Try

            If Not dr("Verified") Is Nothing Then
                If Not IsDBNull(dr("Verified")) Then
                    Boolean.TryParse(dr("Verified"), _verified)
                End If
            End If

            If Not dr("DateCreated") Is Nothing Then
                If Not IsDBNull(dr("DateCreated")) Then
                    Date.TryParse(dr("DateCreated"), _datecreated)
                End If
            End If

        End Sub

        Public Property KeyID As Integer Implements DotNetNuke.Entities.Modules.IHydratable.KeyID
            Get
                Return _itemid
            End Get
            Set(ByVal value As Integer)
                _itemid = value
            End Set
        End Property

    End Class

End Namespace

