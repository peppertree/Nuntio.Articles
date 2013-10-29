Imports System
Imports System.Configuration
Imports System.Data

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class CommentInfo
        Implements DotNetNuke.Entities.Modules.IHydratable

#Region "Private Members"
        Private _itemId As Int32
        Private _ArticleId As Int32
        Private _createdBy As Int32 = Null.NullInteger
        Private _createdDate As DateTime
        Private _comment As String
        Private _isApproved As Boolean = False
        Private _approvedBy As Int32 = Null.NullInteger
        Private _remoteAddress As String
        Private _displayname As String = Null.NullString
        Private _email As String = Null.NullString
        Private _loadgravatar As Boolean = True
#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"

        Public Property ItemId() As Int32
            Get
                Return _itemId
            End Get
            Set(ByVal Value As Int32)
                _itemId = Value
            End Set
        End Property

        Public Property ArticleId() As Int32
            Get
                Return _ArticleId
            End Get
            Set(ByVal Value As Int32)
                _ArticleId = Value
            End Set
        End Property

        Public Property CreatedBy() As Int32
            Get
                Return _createdBy
            End Get
            Set(ByVal Value As Int32)
                _createdBy = Value
            End Set
        End Property

        Public Property CreatedDate() As DateTime
            Get
                Return _createdDate
            End Get
            Set(ByVal Value As DateTime)
                _createdDate = Value
            End Set
        End Property

        Public Property Comment() As String
            Get
                Return _comment
            End Get
            Set(ByVal Value As String)
                _comment = Value
            End Set
        End Property

        Public Property IsApproved() As Boolean
            Get
                Return _isApproved
            End Get
            Set(ByVal Value As Boolean)
                _isApproved = Value
            End Set
        End Property

        Public Property ApprovedBy() As Int32
            Get
                Return _approvedBy
            End Get
            Set(ByVal Value As Int32)
                _approvedBy = Value
            End Set
        End Property

        Public Property RemoteAddress() As String
            Get
                Return _remoteAddress
            End Get
            Set(ByVal Value As String)
                _remoteAddress = Value
            End Set
        End Property

        Public Property Email As String
            Get
                Return _email
            End Get
            Set(ByVal Value As String)
                _email = Value
            End Set
        End Property

        Public Property Displayname As String
            Get
                Return _displayname
            End Get
            Set(ByVal Value As String)
                _displayname = Value
            End Set
        End Property

        Public Property LoadGravatar As Boolean
            Get
                Return _loadgravatar
            End Get
            Set(ByVal Value As Boolean)
                _loadgravatar = Value
            End Set
        End Property

#End Region

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements DotNetNuke.Entities.Modules.IHydratable.Fill

            If Not IsDBNull(dr("NewsId")) Then
                Integer.TryParse(dr("NewsId"), _ArticleId)
            End If

            If Not IsDBNull(dr("CreatedBy")) Then
                Integer.TryParse(dr("CreatedBy"), _createdBy)
            End If

            If Not IsDBNull(dr("CreatedDate")) Then
                Date.TryParse(dr("CreatedDate"), _createdDate)
            End If

            If Not IsDBNull(dr("Comment")) Then
                _comment = Convert.ToString(dr("Comment"))
            End If

            If Not IsDBNull(dr("IsApproved")) Then
                Boolean.TryParse(dr("IsApproved"), _isApproved)
            End If

            If Not IsDBNull(dr("ApprovedBy")) Then
                Integer.TryParse(dr("ApprovedBy"), _approvedBy)
            End If

            If Not IsDBNull(dr("RemoteAddress")) Then
                _remoteAddress = Convert.ToString(dr("RemoteAddress"))
            End If

            If Not IsDBNull(dr("ItemId")) Then
                Integer.TryParse(dr("ItemId"), _itemId)
            End If

        End Sub

        Public Property KeyID As Integer Implements DotNetNuke.Entities.Modules.IHydratable.KeyID
            Get
                Return _itemId
            End Get
            Set(ByVal value As Integer)
                _itemId = value
            End Set
        End Property

    End Class


End Namespace
