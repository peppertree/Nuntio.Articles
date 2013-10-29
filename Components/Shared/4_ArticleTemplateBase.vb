Imports System.ComponentModel
Imports Telerik.Web.UI

Imports System.IO

Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Users
Imports System.Xml
Imports System.Security.Cryptography
Imports System.Globalization
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Controllers
Imports DotNetNuke.Services.FileSystem


Namespace dnnWerk.Modules.Nuntio.Articles
    Public Class TemplateBase
        Inherits ArticleSettings


        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property CommentTemplate() As String
            Get

                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Comment_Item), PortalSettings, CurrentLocale)

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property HeaderTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Listing_Header), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property FooterTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Listing_Footer), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property JournalTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Journal_Item), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ItemTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Listing_Item), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property FeaturedItemTemplate() As String
            Get

                Dim strTemplate As String = ""
                strTemplate = Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Listing_ItemFeatured), PortalSettings, CurrentLocale)

                If strTemplate = "" Then
                    strTemplate = Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Listing_Item), PortalSettings, CurrentLocale)
                End If

                Return strTemplate

            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AlternatingItemTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Listing_ItemAlternate), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property FirstItemTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Listing_ItemFirst), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property EmailItemTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Email_Item), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property EmailContainerTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Email_Container), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property DetailTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Detail_View), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ImageTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Image_Item), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AttachmentTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Attachment_Item), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ScrollingTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Scroller_Item), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property MailTemplate() As String
            Get
                Return Templates.GetTemplate(GetNewsletterTemplatePath(ArticleTheme, Templates.NewsletterTemplate.Mailing_Item), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property MailHeaderTemplate() As String
            Get
                Return Templates.GetTemplate(GetNewsletterTemplatePath(ArticleTheme, Templates.NewsletterTemplate.Mailing_Header), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property MailFooterTemplate() As String
            Get
                Return Templates.GetTemplate(GetNewsletterTemplatePath(ArticleTheme, Templates.NewsletterTemplate.Mailing_Footer), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property UnsubscribeTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Mailing_Unsubscribe), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property SubscribeTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Mailing_Subscribe), PortalSettings, CurrentLocale)
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property CommentNotificationTemplate() As String
            Get
                Return Templates.GetTemplate(GetArticlesTemplatePath(ArticleTheme, Templates.ArticlesTemplate.Comment_Notify), PortalSettings, CurrentLocale)
            End Get
        End Property

        Private Function GetArticlesTemplatePath(ByVal Theme As String, ByVal Templatename As Templates.ArticlesTemplate) As String

            Dim path As String = Server.MapPath(Me.ModuleDirectory & "/templates/" & Theme & "/" & [Enum].GetName(GetType(Templates.ArticlesTemplate), Templatename) & ".txt")

            If System.IO.File.Exists(path) Then
                Return path
            End If

            Return ""

        End Function

        Private Function GetNewsletterTemplatePath(ByVal PortalId As Integer, ByVal Templatename As Templates.NewsletterTemplate) As String

            Dim path As String = Server.MapPath(Me.ModuleDirectory & "/Templates/Portal/" & PortalId.ToString & "/Newsletter/" & [Enum].GetName(GetType(Templates.NewsletterTemplate), Templatename) & ".txt")

            If System.IO.File.Exists(path) Then
                Return path
            End If

            Return ""

        End Function

    End Class
End Namespace

