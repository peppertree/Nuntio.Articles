Imports System
Imports System.Configuration
Imports System.Data

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class ArticleFileInfo

        Implements DotNetNuke.Entities.Modules.IHydratable

#Region " Private Members "

        Private _ArticleFileId As Integer
        Private _FileId As Integer
        Private _ArticleId As Integer
        Private _IsPrimary As Boolean
        Private _Folder As String
        Private _FileName As String
        Private _Url As String
        Private _ImageTitle As String
        Private _ImageDescription As String
        Private _Locale As String
        Private _IsLocaleTranslated As Boolean
        Private _IsOriginal As Boolean
        Private _IsImage As Boolean
        Private _Extension As String
#End Region

#Region " Public Properties "

        Public Property ArticleFileId() As Integer
            Get
                Return _ArticleFileId
            End Get
            Set(ByVal value As Integer)
                _ArticleFileId = value
            End Set
        End Property

        Public Property FileId() As Integer
            Get
                Return _FileId
            End Get
            Set(ByVal value As Integer)
                _FileId = value
            End Set
        End Property

        Public Property ArticleId() As Integer
            Get
                Return _ArticleId
            End Get
            Set(ByVal value As Integer)
                _ArticleId = value
            End Set
        End Property

        Public Property IsPrimary() As Boolean
            Get
                Return _IsPrimary
            End Get
            Set(ByVal Value As Boolean)
                _IsPrimary = Value
            End Set
        End Property

        Public Property IsImage() As Boolean
            Get
                Return _IsImage
            End Get
            Set(ByVal Value As Boolean)
                _IsImage = Value
            End Set
        End Property

        Public Property Extension() As String
            Get
                Return _Extension
            End Get
            Set(ByVal Value As String)
                _Extension = Value
            End Set
        End Property

        Public Property Folder() As String
            Get
                Return _Folder
            End Get
            Set(ByVal Value As String)
                _Folder = Value
            End Set
        End Property

        Public Property FileName() As String
            Get
                Return _FileName
            End Get
            Set(ByVal Value As String)
                _FileName = Value
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

        Public Property ImageTitle() As String
            Get
                Return _ImageTitle
            End Get
            Set(ByVal Value As String)
                _ImageTitle = Value
            End Set
        End Property

        Public Property ImageDescription() As String
            Get
                Return _ImageDescription
            End Get
            Set(ByVal Value As String)
                _ImageDescription = Value
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

        Public Property IsLocaleTranslated() As Boolean
            Get
                Return _IsLocaleTranslated
            End Get
            Set(ByVal value As Boolean)
                _IsLocaleTranslated = value
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

#End Region

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements DotNetNuke.Entities.Modules.IHydratable.Fill

            If Not IsDBNull(dr("ArticleId")) Then
                Integer.TryParse(dr("ArticleId"), _ArticleId)
            End If

            If Not IsDBNull(dr("FileId")) Then
                Integer.TryParse(dr("FileId"), _FileId)
            End If

            If Not IsDBNull(dr("FileName")) Then
                _FileName = Convert.ToString(dr("FileName"))
            End If

            If Not IsDBNull(dr("Folder")) Then
                _Folder = Convert.ToString(dr("Folder"))
            End If

            If Not IsDBNull(dr("ArticleFileId")) Then
                Integer.TryParse(dr("ArticleFileId"), _ArticleFileId)
            End If

            If Not IsDBNull(dr("IsPrimary")) Then
                Boolean.TryParse(dr("IsPrimary"), _IsPrimary)
            End If

            If Not IsDBNull(dr("Folder")) AndAlso Not IsDBNull(dr("FileName")) Then
                _Url = GetPortalSettings.HomeDirectory & _Folder & _FileName
            End If

            If Not IsDBNull(dr("IsImage")) Then
                Boolean.TryParse(dr("IsImage"), _IsImage)
            End If

            If Not IsDBNull(dr("Extension")) Then
                _Extension = Convert.ToString(dr("Extension"))
            End If

        End Sub

        Public Property KeyID As Integer Implements DotNetNuke.Entities.Modules.IHydratable.KeyID
            Get
                Return _ArticleFileId
            End Get
            Set(ByVal value As Integer)
                _ArticleFileId = value
            End Set
        End Property

    End Class

End Namespace

