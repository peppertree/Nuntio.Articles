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

    Public Class EmailQueueInfo

#Region "Private Members"
        Private _itemId As Int32
        Private _portalId As Int32
        Private _moduleId As Int32
        Private _recipient As String
        Private _sender As String
        Private _subject As String
        Private _message As String
        Private _addedToQueue As DateTime
        Private _deliveryAttempts As Int32 = 0
        Private _lastDeliveryAttempt As DateTime
        Private _lasterror As String
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

        Public Property PortalId() As Int32
            Get
                Return _portalId
            End Get
            Set(ByVal Value As Int32)
                _portalId = Value
            End Set
        End Property

        Public Property ModuleId() As Int32
            Get
                Return _moduleId
            End Get
            Set(ByVal Value As Int32)
                _moduleId = Value
            End Set
        End Property

        Public Property Recipient() As String
            Get
                Return _recipient
            End Get
            Set(ByVal Value As String)
                _recipient = Value
            End Set
        End Property

        Public Property Sender() As String
            Get
                Return _sender
            End Get
            Set(ByVal Value As String)
                _sender = Value
            End Set
        End Property

        Public Property Subject() As String
            Get
                Return _subject
            End Get
            Set(ByVal Value As String)
                _subject = Value
            End Set
        End Property

        Public Property Message() As String
            Get
                Return _message
            End Get
            Set(ByVal Value As String)
                _message = Value
            End Set
        End Property

        Public Property AddedToQueue() As DateTime
            Get
                Return _addedToQueue
            End Get
            Set(ByVal Value As DateTime)
                _addedToQueue = Value
            End Set
        End Property

        Public Property DeliveryAttempts() As Int32
            Get
                Return _deliveryAttempts
            End Get
            Set(ByVal Value As Int32)
                _deliveryAttempts = Value
            End Set
        End Property

        Public Property LastDeliveryAttempt() As DateTime
            Get
                Return _lastDeliveryAttempt
            End Get
            Set(ByVal Value As DateTime)
                _lastDeliveryAttempt = Value
            End Set
        End Property

        Public Property LastError() As String
            Get
                Return _lasterror
            End Get
            Set(ByVal value As String)
                _lasterror = value
            End Set
        End Property

#End Region

    End Class

End Namespace

