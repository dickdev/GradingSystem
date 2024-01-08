﻿Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

Public Class frmStudent

    'GENERATE ID
    Private newrec As Boolean
    Public maxNumber As Integer
    Private idnum As String
    Sub getStudID()
        Dim Year As Integer
        Dim idnum As Integer
        Dim studID As String
        Year = Convert.ToInt32(Now.ToString("yyyy")) '2023
        maxNumber = Convert.ToInt32(getmaxStudID()) '3
        idnum = maxNumber + 1 '1 + 1 idnum=4
        studID = "STUD-" & Year & "-" & idnum 'EMP-2023-4
        txtID.Text = studID
    End Sub


    Sub loadgrid()

        Try
            If (MyCon.State = ConnectionState.Open) Then MyCon.Close()
            MyCon.Open()
            dt = New DataTable

            With MyCommand
                .Connection = MyCon
                .CommandText = "Select S_ID as 'Student ID', S_FNAME as 'Firstname', S_MNAME as 'Middlename', S_LNAME as 'Lastname', S_COURSE as 'Course', S_SECTION as 'Section' FROM student order by S_NUM asc"
            End With
            MyAdapter.SelectCommand = MyCommand
            MyAdapter.Fill(dt)
            dgStudent.DataSource = dt
            dgStudent.RowHeadersVisible = False
            MyCon.Close()
            MyCommand.Dispose()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        newrec = True
        Button2.Text = "SAVE"
        gbStudent.Enabled = True
        Button1.Enabled = False
        Button2.Enabled = True
        getStudID()
        btnCancel.Enabled = True
        dgStudent.Enabled = False
        txtSearch.Enabled = False
        btnEdit.Enabled = False

    End Sub

    Private Sub frmStudent_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Button1.Text = "ADD"
        Button2.Text = "SAVE"
        Button2.Enabled = False
        gbStudent.Enabled = False
        btnCancel.Enabled = False

        loadgrid()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click


        If txtFname.Text = "" Or txtMname.Text = "" Or txtLname.Text = "" Or cboCourse.Text = "" Or cboSection.Text = "" Then
            MsgBox("ALL FIELDS ARE REQUIRED! ", MessageBoxIcon.Error, "Required")
            Exit Sub
        End If


        Try
            If (newrec = True) Then
                MyCon.Open()
                MyCommand.Connection = MyCon
                MyCommand.CommandText = "SELECT * FROM student WHERE S_ID='" & txtID.Text & "' "
                MyAdapter.SelectCommand = MyCommand
                Dim MySQLData As MySqlDataReader = MyCommand.ExecuteReader
                If MySQLData.HasRows = 0 Then
                    MyCon.Close()

                    ExecInsertButton("INSERT INTO student VALUES('0','" & txtID.Text & "','" & txtFname.Text.ToUpper() & "', '" & txtMname.Text.ToUpper() & "','" & txtLname.Text.ToUpper() & "',  '" & cboCourse.Text.ToUpper() & "', '" & cboSection.Text.ToUpper() & "' )")
                    MessageBox.Show("Insert Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    clearText()
                    Button1.Enabled = True
                    Button2.Enabled = False
                    gbStudent.Enabled = False
                    btnCancel.Enabled = False
                    btnEdit.Enabled = True
                    txtSearch.Enabled = True
                    dgStudent.Enabled = True
                    loadgrid()
                    MyCon.Close()
                Else
                    MsgBox("Firstname and Lastname already exist.", MsgBoxStyle.Information, "Success")
                    MyCon.Close()
                    Exit Sub
                End If
            ElseIf (newrec = False) Then
                ExecInsertButton("Update student SET S_FNAME='" & txtFname.Text.ToUpper() & "', S_MNAME='" & txtMname.Text.ToUpper() & "', S_LNAME='" & txtLname.Text.ToUpper() & "',  S_COURSE='" & cboCourse.Text.ToUpper() & "', S_SECTION='" & cboSection.Text.ToUpper() & "' WHERE S_ID='" & txtID.Text & "'")
                MessageBox.Show("Update Successfully!", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
                clearText()
                Button1.Enabled = True
                Button2.Enabled = False
                btnCancel.Enabled = False
                gbStudent.Enabled = False
                btnEdit.Enabled = True
                txtSearch.Enabled = True
                dgStudent.Enabled = True
                loadgrid()
                Button2.Text = "SAVE"
                MyCon.Close()

            Else
                MsgBox("Unknown command.", MsgBoxStyle.Information, "Error")
            End If
        Catch ex As Exception

        End Try
    End Sub

    Sub clearText()
        txtID.Text = ""
        txtFname.Text = ""
        txtMname.Text = ""
        txtLname.Text = ""
        cboCourse.Text = Nothing
        cboSection.Text = Nothing
    End Sub

    Private Sub frmStudent_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Me.Close()

    End Sub

    Private Sub txtLname_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtLname.Leave
        Dim strLastname As String = "^[a-zA-Z\s]+$"

        Dim reLastname As New Regex(strLastname) 'Attach Pattern To lastname Textbox

        If Not reLastname.IsMatch(txtLname.Text) Then

            MessageBox.Show("Please Enter Alphabetic Characters for LastName!")

            txtLname.Focus()

            txtLname.Clear()

        End If
    End Sub


    Private Sub btnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        newrec = False
        If txtID.Text = "" Then
            MessageBox.Show("Select ID from row!", "Required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            gbStudent.Enabled = True
            Button2.Text = "UPDATE"
            Button2.Enabled = True
            btnCancel.Enabled = True
            btnEdit.Enabled = False
            txtSearch.Enabled = False

        End If
    End Sub

    Private Sub DataGridView1_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgStudent.CellClick
        Button1.Enabled = False
        btnCancel.Enabled = True
        On Error Resume Next
        Dim i As Integer
        i = dgStudent.CurrentRow.Index
        Me.txtID.Text = dgStudent.Item(0, i).Value
        Me.txtFname.Text = dgStudent.Item(1, i).Value
        Me.txtMname.Text = dgStudent.Item(2, i).Value
        Me.txtLname.Text = dgStudent.Item(3, i).Value
        Me.cboCourse.Text = dgStudent.Item(4, i).Value
        Me.cboSection.Text = dgStudent.Item(5, i).Value

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        txtSearch.Enabled = True
        dgStudent.Enabled = True
        clearText()
        Button1.Enabled = True
        Button2.Enabled = False
        btnCancel.Enabled = False
        btnEdit.Enabled = True
        gbStudent.Enabled = False
        Button2.Text = "SAVE"
    End Sub

    Private Sub txtSearch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearch.TextChanged
        Try
            If (MyCon.State = ConnectionState.Open) Then
                MyCon.Close()
            End If
            dgStudent.Refresh()
            MyCon.Open()
            MyCommand.Connection = MyCon
            MyCommand.CommandText = "SELECT  S_ID as 'Student ID', S_FNAME as 'Firstname', S_MNAME as 'Middlename', S_LNAME as 'Lastname', S_COURSE as 'Course', S_SECTION as 'Section' FROM student WHERE S_FNAME LIKE '" & txtSearch.Text & "%' OR S_MNAME LIKE '" & txtSearch.Text & "%' OR S_LNAME LIKE '" & txtSearch.Text & "%' "
            MyAdapter.SelectCommand = MyCommand
            dt = New DataTable
            dt.Clear()
            MyAdapter.Fill(dt)
            dgStudent.DataSource = dt
            dgStudent.RowHeadersVisible = False
            MyCon.Close()
        Catch ex As Exception

        End Try
    End Sub
End Class