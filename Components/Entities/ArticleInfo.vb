Imports System
Imports System.Configuration
Imports System.Data

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class ArticleInfo

        Implements DotNetNuke.Entities.Modules.IHydratable

#Region "Private Members"

        Private _itemId As Integer
        Private _moduleId As Integer
        Private _PortalId As Integer
        Private _Locale As String
        Private _Title As String
        Private _Url As String
        Private _Summary As String
        Private _Content As String
        Private _ViewOrder As Integer
        Private _CreatedByUser As Integer
        Private _CreatedDate As DateTime
        Private _TrackClicks As Boolean
        Private _NewWindow As Boolean
        Private _PublishDate As DateTime
        Private _ExpiryDate As DateTime = Null.NullDate
        Private _ReviewDate As DateTime = Null.NullDate
        Private _IsNotified As Boolean
        Private _IsOriginal As Boolean
        Private _IsApproved As Boolean
        Private _ApprovedBy As Integer
        Private _ApprovedDate As DateTime
        Private _LastUpdatedBy As Integer
        Private _LastUpdatedDate As DateTime
        Private _Comments As Integer = 0
        Private _IsLocaleTranslated As Boolean
        Private _Images As List(Of ArticleFileInfo)
        Private _Attachments As List(Of ArticleFileInfo)
        Private _IsFeatured As Boolean = False
        Private _AnchorLink As String = Null.NullString

        Private _ParentId As Integer = Null.NullInteger
        Private _RelatedArticles As List(Of ArticleInfo) = New List(Of ArticleInfo)
        Private _ChildArticles As List(Of ArticleInfo) = New List(Of ArticleInfo)
        Private _IsPublication As Boolean

        Private _Articles As Integer = 0
        Private _PublicationTitle As String = Null.NullString

        Private _isDeleted As Boolean

        Private _categories As List(Of Integer)

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

#End Region

#Region "Properties"

        Public Property IsDeleted() As Boolean
            Get
                Return _isDeleted
            End Get
            Set(ByVal Value As Boolean)
                _isDeleted = Value
            End Set
        End Property

        Public Property IsPublication() As Boolean
            Get
                Return _IsPublication
            End Get
            Set(ByVal Value As Boolean)
                _IsPublication = Value
            End Set            
        End Property

        Public Property ParentId() As Integer
            Get
                Return _ParentId
            End Get
            Set(ByVal Value As Integer)
                _ParentId = Value
            End Set
        End Property

        Public Property ChildArticles() As List(Of ArticleInfo)
            Get
                Return _ChildArticles
            End Get
            Set(ByVal Value As List(Of ArticleInfo))
                _ChildArticles = Value
            End Set
        End Property

        Public Property RelatedArticles() As List(Of ArticleInfo)
            Get
                Return _RelatedArticles
            End Get
            Set(ByVal Value As List(Of ArticleInfo))
                _RelatedArticles = Value
            End Set
        End Property

        Public Property Images() As List(Of ArticleFileInfo)
            Get
                Return _Images
            End Get
            Set(ByVal Value As List(Of ArticleFileInfo))
                _Images = Value
            End Set
        End Property

        Public Property Attachments() As List(Of ArticleFileInfo)
            Get
                Return _Attachments
            End Get
            Set(ByVal Value As List(Of ArticleFileInfo))
                _Attachments = Value
            End Set
        End Property

        Public Property Comments() As Integer
            Get
                Return _Comments
            End Get
            Set(ByVal Value As Integer)
                _Comments = Value
            End Set
        End Property

        Public Property ItemId() As Integer
            Get
                Return _itemId
            End Get
            Set(ByVal Value As Integer)
                _itemId = Value
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

        Public Property PortalID() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal Value As Integer)
                _PortalId = Value
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

        Public Property Title() As String
            Get
                Return _Title
            End Get
            Set(ByVal Value As String)
                _Title = Value
            End Set
        End Property

        Public Property Url() As String
            Get
                Return _Url
            End Get
            Set(ByVal Value As String)
                _Url = Value
            End Set
        End Property

        Public Property ViewOrder() As Integer
            Get
                Return _ViewOrder
            End Get
            Set(ByVal Value As Integer)
                _ViewOrder = Value
            End Set
        End Property

        Public Property Summary() As String
            Get
                Return _Summary
            End Get
            Set(ByVal Value As String)
                _Summary = Value
            End Set
        End Property

        Public Property AnchorLink() As String
            Get
                Return _AnchorLink
            End Get
            Set(ByVal Value As String)
                _AnchorLink = Value
            End Set
        End Property

        Public Property Content() As String
            Get
                Return _Content
            End Get
            Set(ByVal Value As String)
                _Content = Value
            End Set
        End Property

        Public Property CreatedByUser() As Integer
            Get
                Return _CreatedByUser
            End Get
            Set(ByVal Value As Integer)
                _CreatedByUser = Value
            End Set
        End Property

        Public Property CreatedDate() As DateTime
            Get
                Return _CreatedDate
            End Get
            Set(ByVal Value As Date)
                _CreatedDate = Value
            End Set
        End Property

        Public Property TrackClicks() As Boolean
            Get
                Return _TrackClicks
            End Get
            Set(ByVal Value As Boolean)
                _TrackClicks = Value
            End Set
        End Property

        Public Property NewWindow() As Boolean
            Get
                Return _NewWindow
            End Get
            Set(ByVal Value As Boolean)
                _NewWindow = Value
            End Set
        End Property

        Public Property PublishDate() As DateTime
            Get
                Return _PublishDate
            End Get
            Set(ByVal Value As Date)
                _PublishDate = Value
            End Set
        End Property

        Public Property ReviewDate() As DateTime
            Get
                Return _ReviewDate
            End Get
            Set(ByVal Value As Date)
                _ReviewDate = Value
            End Set
        End Property

        Public Property ExpiryDate() As DateTime
            Get
                Return _ExpiryDate
            End Get
            Set(ByVal Value As Date)
                _ExpiryDate = Value
            End Set
        End Property

        Public Property IsNotified() As Boolean
            Get
                Return _IsNotified
            End Get
            Set(ByVal Value As Boolean)
                _IsNotified = Value
            End Set
        End Property

        Public Property IsOriginal() As Boolean
            Get
                Return _IsOriginal
            End Get
            Set(ByVal value As Boolean)
                _IsOriginal = value
            End Set
        End Property

        Public Property IsFeatured() As Boolean
            Get
                Return _IsFeatured
            End Get
            Set(ByVal value As Boolean)
                _IsFeatured = value
            End Set
        End Property

        Public Property IsApproved() As Boolean
            Get
                Return _IsApproved
            End Get
            Set(ByVal value As Boolean)
                _IsApproved = value
            End Set
        End Property

        Public Property ApprovedBy() As Integer
            Get
                Return _ApprovedBy
            End Get
            Set(ByVal Value As Integer)
                _ApprovedBy = Value
            End Set
        End Property

        Public Property ApprovedDate() As DateTime
            Get
                Return _ApprovedDate
            End Get
            Set(ByVal value As DateTime)
                _ApprovedDate = value
            End Set
        End Property

        Public Property LastUpdatedBy() As Integer
            Get
                Return _LastUpdatedBy
            End Get
            Set(ByVal Value As Integer)
                _LastUpdatedBy = Value
            End Set
        End Property

        Public Property LastUpdatedDate() As DateTime
            Get
                Return _LastUpdatedDate
            End Get
            Set(ByVal value As DateTime)
                _LastUpdatedDate = value
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

        Public ReadOnly Property Articles() As Integer
            Get
                Return _Articles
            End Get
        End Property

        Public Property PublicationTitle() As String
            Get
                Return _PublicationTitle
            End Get
            Set(ByVal value As String)
                _PublicationTitle = value
            End Set
        End Property

        Public ReadOnly Property Categories As List(Of Integer)
            Get
                Return _categories
            End Get
        End Property

#End Region

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements DotNetNuke.Entities.Modules.IHydratable.Fill

            Try
                If Not IsDBNull(dr("Categories")) Then
                    _categories = New List(Of Integer)
                    Dim strCategories As String = Convert.ToString(dr("Categories"))
                    If Not String.IsNullOrEmpty(strCategories) Then
                        If strCategories.Contains(",") Then
                            For Each cValue As String In strCategories.Split(Char.Parse(","))
                                If IsNumeric(cValue) Then
                                    _categories.Add(Convert.ToInt32(cValue))
                                End If
                            Next
                        Else
                            If IsNumeric(strCategories) Then
                                _categories.Add(strCategories)
                            End If
                        End If
                    End If
                End If
            Catch
            End Try

            If Not IsDBNull(dr("ApprovedBy")) Then
                Integer.TryParse(dr("ApprovedBy"), _ApprovedBy)
            End If

            If Not IsDBNull(dr("Articles")) Then
                Integer.TryParse(dr("Articles"), _Articles)
            End If

            If Not IsDBNull(dr("ApprovedDate")) Then
                DateTime.TryParse(dr("ApprovedDate"), _ApprovedDate)
            End If

            If Not IsDBNull(dr("CreatedByUser")) Then
                Integer.TryParse(dr("CreatedByUser"), _CreatedByUser)
            End If

            If Not IsDBNull(dr("CreatedDate")) Then
                DateTime.TryParse(dr("CreatedDate"), _CreatedDate)
            End If

            If Not IsDBNull(dr("LastUpdatedBy")) Then
                Integer.TryParse(dr("LastUpdatedBy"), _LastUpdatedBy)
            End If

            If Not IsDBNull(dr("LastUpdatedDate")) Then
                DateTime.TryParse(dr("LastUpdatedDate"), _LastUpdatedDate)
            End If

            If Not IsDBNull(dr("ExpiryDate")) Then
                DateTime.TryParse(dr("ExpiryDate"), _ExpiryDate)
            End If

            If Not IsDBNull(dr("ReviewDate")) Then
                DateTime.TryParse(dr("ReviewDate"), _ReviewDate)
            End If

            If Not IsDBNull(dr("PublishDate")) Then
                DateTime.TryParse(dr("PublishDate"), _PublishDate)
            End If

            If Not IsDBNull(dr("IsApproved")) Then
                Boolean.TryParse(dr("IsApproved"), _IsApproved)
            End If

            If Not IsDBNull(dr("IsDeleted")) Then
                Boolean.TryParse(dr("IsDeleted"), _isDeleted)
            End If

            If Not IsDBNull(dr("IsFeatured")) Then
                Boolean.TryParse(dr("IsFeatured"), _IsFeatured)
            End If

            If Not IsDBNull(dr("IsNotified")) Then
                Boolean.TryParse(dr("IsNotified"), _IsNotified)
            End If

            If Not IsDBNull(dr("itemId")) Then
                Integer.TryParse(dr("itemId"), _itemId)
            End If

            If Not IsDBNull(dr("moduleId")) Then
                Integer.TryParse(dr("moduleId"), _moduleId)
            End If

            If Not IsDBNull(dr("PortalID")) Then
                Integer.TryParse(dr("PortalID"), _PortalId)
            End If

            If Not IsDBNull(dr("ParentId")) Then
                Integer.TryParse(dr("ParentId"), _ParentId)
            End If

            If Not IsDBNull(dr("ViewOrder")) Then
                Integer.TryParse(dr("ViewOrder"), _ViewOrder)
            End If

            If Not IsDBNull(dr("Comments")) Then
                Integer.TryParse(dr("Comments"), _Comments)
            End If

            If Not IsDBNull(dr("IsPublication")) Then
                Boolean.TryParse(dr("IsPublication"), _IsPublication)
            End If

            If Not IsDBNull(dr("Anchorlink")) Then
                _AnchorLink = Convert.ToString(dr("AnchorLink"))
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

