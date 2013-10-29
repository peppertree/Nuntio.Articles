Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Xml
Imports System.Security.Cryptography
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Search
Imports DotNetNuke.Entities.Users



Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class SubscriptionController


        Public Function GetSubscription(ByVal SubscriptionId As Integer)

            Dim obj As SubscriptionInfo = CBO.FillObject(Of SubscriptionInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("pnc_NewsItem_Subscription_GetByItemId", SubscriptionId))
            Return obj

        End Function

        Public Function GetSubscription(ByVal Key As String, ByVal Email As String, ByVal NewsModuleId As Integer)

            Dim obj As SubscriptionInfo = CBO.FillObject(Of SubscriptionInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("pnc_NewsItem_Subscription_GetByKeyAndEmail", Key, Email, NewsModuleId))
            Return obj

        End Function

        Public Function GetSubscription(ByVal moduleId As Integer, ByVal Email As String, ByVal UserID As Integer) As SubscriptionInfo
            Dim obj As SubscriptionInfo = Nothing
            If Email <> Null.NullString Then
                obj = CBO.FillObject(Of SubscriptionInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("pnc_NewsItem_Subscription_GetByEmail", moduleId, Email))
            End If
            If UserID <> Null.NullInteger Then
                obj = CBO.FillObject(Of SubscriptionInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("pnc_NewsItem_Subscription_GetByUserID", moduleId, UserID))
            End If
            Return obj
        End Function

        Public Function Subscribe(ByVal subscription As SubscriptionInfo) As String
            Dim key As String = GetKey()
            DotNetNuke.Data.DataProvider.Instance.ExecuteScalar("pnc_NewsItem_Subscription_Add", subscription.ModuleId, GetNull(subscription.UserID), GetNull(subscription.Email), GetNull(subscription.Name), GetNull(subscription.Locale), key, subscription.DateCreated, subscription.Verified)
            Return key
        End Function

        Public Sub UnSubscribe(ByVal subscription As SubscriptionInfo)
            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("pnc_NewsItem_Subscription_Delete", subscription.ModuleId, GetNull(subscription.Email), GetNull(subscription.UserID), GetNull(subscription.Key))
        End Sub

        Public Sub VerifySubscription(ByVal subscription As SubscriptionInfo)
            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("pnc_NewsItem_Subscription_Verify", subscription.Email)
        End Sub

        Public Function ListSubscriptions(ByVal ModuleId As Integer, ByVal Locale As String) As List(Of SubscriptionInfo)

            Return CBO.FillCollection(Of SubscriptionInfo)(DotNetNuke.Data.DataProvider.Instance.ExecuteReader("pnc_NewsItem_Subscription_List", ModuleId, GetNull(Locale)))

        End Function

        Public Sub ClearNotificationQueue()
            DotNetNuke.Data.DataProvider.Instance.ExecuteNonQuery("pnc_NewsItem_SetNotifiedTrue")
        End Sub

        Private Function GetNull(ByVal Field As Object) As Object
            Return Null.GetNull(Field, DBNull.Value)
        End Function

        Private Function GetKey() As String

            Dim strData As String = DateTime.Now.ToString().GetHashCode.ToString("x")

            Return strData

        End Function

    End Class


End Namespace

