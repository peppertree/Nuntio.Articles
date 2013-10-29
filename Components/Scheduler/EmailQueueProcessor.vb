
Imports System.Collections
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports System.Web
Imports System.Web.Hosting
Imports System.Threading
Imports System.Collections.Specialized
Imports System.Text
Imports System.Xml
Imports System.Globalization
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules


Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class EmailQueueProcessor
        Inherits DotNetNuke.Services.Scheduling.SchedulerClient

#Region "Private Members"
        Private _writer As System.IO.StreamWriter = Nothing
        Private _intSucess As Integer = 0
        Private _intError As Integer = 0
#End Region

        Public Sub New(ByVal objScheduleHistoryItem As DotNetNuke.Services.Scheduling.ScheduleHistoryItem)
            MyBase.new()
            Me.ScheduleHistoryItem = objScheduleHistoryItem
        End Sub

        Public Overrides Sub DoWork()
            Try
                'SetHttpContextWithSimulatedRequest()
                VerifyDebugging()
                Me.Progressing()

                ProcessQueue()
                FinalizeDebugging()

                Me.ScheduleHistoryItem.Succeeded = True     'REQUIRED
                Me.ScheduleHistoryItem.AddLogNote("E-Mail queue processed - " & _intSucess.ToString & " messages delivered, " & _intError.ToString & " failed.")

            Catch exc As Exception      'REQUIRED
                Me.ScheduleHistoryItem.Succeeded = False    'REQUIRED
                Me.ScheduleHistoryItem.AddLogNote("E-Mail processing failed.")   'OPTIONAL
                'notification that we have errored
                Me.Errored(exc)         'REQUIRED
                FinalizeDebugging()
                'log the exception
                LogException(exc)       'OPTIONAL
            End Try
        End Sub

        Public Shared Sub SetHttpContextWithSimulatedRequest()

            Dim page As String = (HttpRuntime.AppDomainAppVirtualPath + "/default.aspx").TrimStart("/")
            Dim query As String = ""
            Dim output As StringWriter = New StringWriter

            Thread.GetDomain.SetData(".hostingInstallDir", HttpRuntime.AspInstallDirectory)
            Thread.GetDomain.SetData(".hostingVirtualPath", HttpRuntime.AppDomainAppVirtualPath)
            Dim workerRequest As SimpleWorkerRequest = New SimpleWorkerRequest(page, query, output)
            HttpContext.Current = New HttpContext(workerRequest)

        End Sub

#Region "Processing Methods"

        Private Sub ProcessQueue()

            Dim ctl As New EmailQueueController
            Dim queue As New List(Of EmailQueueInfo)

            Log("Loading e-mail queue...")
            queue = ctl.GetItems()

            Log(queue.Count.ToString & " entries in queue")
            If queue.Count > 0 Then

                For Each objQueue As EmailQueueInfo In queue

                    Log("Trying to deliver e-mail to " & objQueue.Recipient)

                    Dim strMessage As String = SendMail(objQueue)
                    Dim blnDelivered As Boolean = False

                    If strMessage Is Nothing Then
                        blnDelivered = True
                    Else
                        If strMessage = "" Then
                            blnDelivered = True
                        End If
                    End If

                    If blnDelivered Then
                        Log("...email delivered")
                        _intSucess += 1
                        'remove from queue
                        ctl.Delete(objQueue.ItemId)
                    Else
                        Log("...email not delivered. Error: " & strMessage)
                        _intError += 1
                        'update queue entry
                        objQueue.DeliveryAttempts += 1
                        objQueue.LastDeliveryAttempt = Date.Now
                        objQueue.LastError = strMessage
                        ctl.Update(objQueue)
                    End If

                Next

                Log("Queue processed - " & _intSucess.ToString & " messages delivered, " & _intError.ToString & " failed.")

            End If

        End Sub

        Private Function SendMail(ByVal objQueue As EmailQueueInfo) As String

            Dim strMessage As String = ""
            strMessage = DotNetNuke.Services.Mail.Mail.SendMail(objQueue.Sender, objQueue.Recipient, "", objQueue.Subject, objQueue.Message, "", "HTML", "", "", "", "")
            Return strMessage

        End Function

#End Region

#Region "Logging Methods"

        Private Sub VerifyDebugging()

            Dim blnDebuggingEnabled As Boolean = False

            Try
                Dim Path As String = HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\NotificationDebug.txt"
                blnDebuggingEnabled = System.IO.File.Exists(Path)
            Catch
            End Try

            If blnDebuggingEnabled Then

                Dim logpath As String = HttpRuntime.AppDomainAppPath & "Desktopmodules\Nuntio.Articles\logs"
                If Not System.IO.Directory.Exists(logpath) Then
                    System.IO.Directory.CreateDirectory(logpath)
                End If

                logpath += "\Debug_Email_" & Date.Now.ToShortDateString.Replace("\", ".").Replace("/", ".") & "_" & Date.Now.Hour.ToString & "." & Date.Now.Minute.ToString & "." & Date.Now.Second.ToString & ".txt"
                If Not System.IO.File.Exists(logpath) Then

                    Dim log As New FileInfo(logpath)
                    If Not log.Exists Then
                        _writer = log.CreateText()
                    End If

                End If

                If Not _writer Is Nothing Then
                    Exit Sub
                End If

                Try
                    _writer = New System.IO.StreamWriter(logpath)
                Catch
                End Try

            End If

        End Sub

        Private Sub Log(ByVal strText As String)

            If Not _writer Is Nothing Then
                Try
                    _writer.WriteLine(Date.Now.ToShortTimeString & ":" & vbTab & strText)
                Catch
                End Try
            End If

        End Sub

        Private Sub FinalizeDebugging()

            If Not _writer Is Nothing Then
                _writer.Close()
                _writer.Dispose()
            End If

        End Sub

#End Region

    End Class


End Namespace

