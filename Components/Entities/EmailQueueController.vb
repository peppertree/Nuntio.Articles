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

    Public Class EmailQueueController

#Region "Public Methods"

        Public Function GetItem(ByVal itemId As Integer) As EmailQueueInfo

            Return FillObject(DotNetNuke.Data.DataProvider.Instance().ExecuteReader("pnc_News_EmailQueue_Get", itemId))

        End Function

        <DataObjectMethod(DataObjectMethodType.Select, True)> _
        Public Function GetItems() As List(Of EmailQueueInfo)

            Return GetItems(Null.NullInteger, Null.NullInteger)

        End Function

        <DataObjectMethod(DataObjectMethodType.Select, True)> _
        Public Function GetItemsByModule(ByVal ModuleId As Integer) As List(Of EmailQueueInfo)

            Return GetItems(Null.NullInteger, ModuleId)

        End Function

        <DataObjectMethod(DataObjectMethodType.Select, True)> _
        Public Function GetItemsByPortal(ByVal PortalId As Integer) As List(Of EmailQueueInfo)

            Return GetItems(PortalId, Null.NullInteger)

        End Function

        <DataObjectMethod(DataObjectMethodType.Select, True)> _
        Private Function GetItems(ByVal PortalId As Integer, ByVal ModuleId As Integer) As List(Of EmailQueueInfo)

            Return FillList(DotNetNuke.Data.DataProvider.Instance().ExecuteReader("pnc_News_EmailQueue_List", GetNull(PortalId), GetNull(ModuleId)))

        End Function

        <DataObjectMethod(DataObjectMethodType.Insert, True)> _
        Public Function Add(ByVal objEmailQueue As EmailQueueInfo) As Integer

            Return CType(DotNetNuke.Data.DataProvider.Instance().ExecuteScalar("pnc_News_EmailQueue_Add", objEmailQueue.PortalId, objEmailQueue.ModuleId, objEmailQueue.Recipient, objEmailQueue.Sender, objEmailQueue.Subject, objEmailQueue.Message, objEmailQueue.AddedToQueue, objEmailQueue.DeliveryAttempts, GetNull(objEmailQueue.LastDeliveryAttempt), GetNull(objEmailQueue.LastError)), Integer)

        End Function

        <DataObjectMethod(DataObjectMethodType.Update, True)> _
        Public Sub Update(ByVal objEmailQueue As EmailQueueInfo)

            DotNetNuke.Data.DataProvider.Instance().ExecuteNonQuery("pnc_News_EmailQueue_Update", objEmailQueue.ItemId, objEmailQueue.PortalId, objEmailQueue.ModuleId, objEmailQueue.Recipient, objEmailQueue.Sender, objEmailQueue.Subject, objEmailQueue.Message, objEmailQueue.AddedToQueue, objEmailQueue.DeliveryAttempts, GetNull(objEmailQueue.LastDeliveryAttempt), GetNull(objEmailQueue.LastError))

        End Sub

        <DataObjectMethod(DataObjectMethodType.Delete, True)> _
        Public Sub Delete(ByVal ItemId As Integer)

            DotNetNuke.Data.DataProvider.Instance().ExecuteNonQuery("pnc_News_EmailQueue_Delete", ItemId)

        End Sub

#End Region

#Region "Helper Functions"

        Private Function GetNull(ByVal Field As Object) As Object
            Return Null.GetNull(Field, DBNull.Value)
        End Function

        Private Function FillList(ByVal dr As IDataReader) As List(Of EmailQueueInfo)

            Dim infolist As New List(Of EmailQueueInfo)

            Try

                Dim objQueue As EmailQueueInfo = Nothing

                While dr.Read

                    objQueue = New EmailQueueInfo

                    Try
                        objQueue.PortalId = Convert.ToInt32(dr("PortalId"))
                    Catch
                    End Try
                    Try
                        objQueue.ModuleId = Convert.ToInt32(dr("ModuleId"))
                    Catch
                    End Try
                    Try
                        objQueue.Recipient = Convert.ToString(dr("Recipient"))
                    Catch
                    End Try
                    Try
                        objQueue.Sender = Convert.ToString(dr("Sender"))
                    Catch
                    End Try
                    Try
                        objQueue.Subject = Convert.ToString(dr("Subject"))
                    Catch
                    End Try
                    Try
                        objQueue.Message = Convert.ToString(dr("Message"))
                    Catch
                    End Try
                    Try
                        objQueue.AddedToQueue = Convert.ToDateTime(dr("AddedToQueue"))
                    Catch
                    End Try
                    Try
                        objQueue.DeliveryAttempts = Convert.ToInt32(dr("DeliveryAttempts"))
                    Catch
                    End Try
                    Try
                        objQueue.LastDeliveryAttempt = Convert.ToDateTime(dr("LastDeliveryAttempt"))
                    Catch
                    End Try
                    Try
                        objQueue.ItemId = Convert.ToInt32(dr("ItemId"))
                    Catch
                    End Try
                    Try
                        objQueue.LastError = Convert.ToString(dr("LastError"))
                    Catch
                    End Try

                    infolist.Add(objQueue)

                End While

            Finally

                If Not dr Is Nothing Then
                    dr.Close()
                    dr.Dispose()
                    dr = Nothing
                End If

            End Try

            Return infolist

        End Function

        Private Function FillObject(ByVal dr As System.Data.IDataReader) As EmailQueueInfo

            Try

                Dim objQueue As EmailQueueInfo = Nothing

                While dr.Read

                    objQueue = New EmailQueueInfo

                    Try
                        objQueue.PortalId = Convert.ToInt32(dr("PortalId"))
                    Catch
                    End Try
                    Try
                        objQueue.ModuleId = Convert.ToInt32(dr("ModuleId"))
                    Catch
                    End Try
                    Try
                        objQueue.Recipient = Convert.ToString(dr("Recipient"))
                    Catch
                    End Try
                    Try
                        objQueue.Sender = Convert.ToString(dr("Sender"))
                    Catch
                    End Try
                    Try
                        objQueue.Subject = Convert.ToString(dr("Subject"))
                    Catch
                    End Try
                    Try
                        objQueue.Message = Convert.ToString(dr("Message"))
                    Catch
                    End Try
                    Try
                        objQueue.AddedToQueue = Convert.ToDateTime(dr("AddedToQueue"))
                    Catch
                    End Try
                    Try
                        objQueue.DeliveryAttempts = Convert.ToInt32(dr("DeliveryAttempts"))
                    Catch
                    End Try
                    Try
                        objQueue.LastDeliveryAttempt = Convert.ToDateTime(dr("LastDeliveryAttempt"))
                    Catch
                    End Try
                    Try
                        objQueue.ItemId = Convert.ToInt32(dr("ItemId"))
                    Catch
                    End Try
                    Try
                        objQueue.LastError = Convert.ToString(dr("LastError"))
                    Catch
                    End Try

                End While


                Return objQueue

            Finally

                If Not dr Is Nothing Then
                    dr.Close()
                    dr.Dispose()
                    dr = Nothing
                End If

            End Try

            Return Nothing

        End Function

#End Region

    End Class

End Namespace

