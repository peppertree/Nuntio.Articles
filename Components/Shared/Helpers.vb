Imports System.Globalization
Imports DotNetNuke.Services.FileSystem

Namespace dnnWerk.Modules.Nuntio.Articles

    Public Class Helpers

        Public Shared Function RemoveDiacritics(ByVal s As String) As String
            s = s.Normalize(System.Text.NormalizationForm.FormD)
            Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder()
            Dim i As Integer
            For i = 0 To s.Length - 1
                Dim sTemp As String = s(i)
                If CharUnicodeInfo.GetUnicodeCategory(sTemp) <> UnicodeCategory.NonSpacingMark Then
                    sb.Append(sTemp)
                End If
            Next
            Dim sReturn As String = sb.ToString
            Return sReturn
        End Function

        Public Shared Function IsAlphaNumeric(ByVal sChr As String) As Boolean
            IsAlphaNumeric = sChr Like "[0-9A-Za-z ]"
        End Function

        Public Shared Function FormatSearchString(ByVal sOutput As String, ByVal SearchKey As String) As String

            If SearchKey.Length > 0 Then
                For Each sWord As String In SearchKey.Split(Char.Parse(" "))
                    If sWord.Length > 0 Then
                        If sOutput.Contains(sWord) Then
                            sOutput = sOutput.Replace(sWord, "<span class=""SearchWord"">" & sWord & "</span>")
                        End If
                    End If
                Next
            End If

            Return sOutput

        End Function

        Public Shared Function ProcessImages(ByVal html As String) As String
            Dim htmlTransformed As String = html
            htmlTransformed = htmlTransformed.Replace("src=""~/images/", "src=""" & ResolveUrl("Images") & "/")
            Return htmlTransformed
        End Function

        Public Shared Function StripHtml(ByVal html As String) As String
            Dim pattern As String = "<(.|\n)*?>"
            Return Regex.Replace(html, pattern, String.Empty)
        End Function

        Public Shared Function OnlyAlphaNumericChars(ByVal OrigString As String) As String

            '***********************************************************
            'INPUT:  Any String
            'OUTPUT: The Input String with all non-alphanumeric characters 
            '        removed
            'EXAMPLE Debug.Print OnlyAlphaNumericChars("Hello World!")
            'output = "HelloWorld")
            'NOTES:  Not optimized for speed and will run slow on long
            '        strings.  If you plan on using long strings, consider 
            '        using alternative method of appending to output string,
            '        such as the method at
            '        http://www.freevbcode.com/ShowCode.Asp?ID=154
            '***********************************************************
            Dim lLen As Integer
            Dim sAns As String = ""
            Dim lCtr As Integer
            Dim sChar As String

            OrigString = RemoveDiacritics(Trim(OrigString))

            lLen = Len(OrigString)
            For lCtr = 1 To lLen
                sChar = Mid(OrigString, lCtr, 1)
                If IsAlphaNumeric(Mid(OrigString, lCtr, 1)) Then
                    sAns = sAns & sChar
                End If
            Next

            OnlyAlphaNumericChars = Replace(sAns, " ", "-")

        End Function

        Public Shared Function IsInParamList(ByVal CheckParam As String, ByVal ParamArray AdditionalParams() As String) As Boolean
            If Not AdditionalParams Is Nothing Then
                For Each sCheck As String In AdditionalParams
                    Dim sKey As String = sCheck.Substring(0, sCheck.IndexOf("="))
                    If sKey.ToLower = CheckParam.ToLower Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function

    End Class

End Namespace

