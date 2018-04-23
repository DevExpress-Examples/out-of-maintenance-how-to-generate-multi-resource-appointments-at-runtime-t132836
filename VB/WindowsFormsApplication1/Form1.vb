Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Linq
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler

Namespace WindowsFormsApplication1
    Partial Public Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Shared RandomInstance As New Random()

        Private CustomResourceCollection As New BindingList(Of CustomResource)()
        Private CustomEventList As New BindingList(Of CustomAppointment)()

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            InitResources()
            InitAppointments()
            schedulerControl1.Start = Date.Now
            AddHandler schedulerStorage1.AppointmentDeleting, AddressOf schedulerStorage1_AppointmentDeleting
            schedulerControl1.GroupType = DevExpress.XtraScheduler.SchedulerGroupType.Resource
        End Sub
        Private cache As New List(Of CustomAppointment)()
        Private Sub schedulerStorage1_AppointmentDeleting(ByVal sender As Object, ByVal e As PersistentObjectCancelEventArgs)
           Dim appt As Appointment = TryCast(e.Object, Appointment)
           Dim deletedAppointment As CustomAppointment = TryCast(appt.GetSourceObject(schedulerStorage1), CustomAppointment)
           cache.Add(deletedAppointment)
        End Sub

        Private Sub InitResources()
            Dim mappings As ResourceMappingInfo = Me.schedulerStorage1.Resources.Mappings
            mappings.Id = "ResID"
            mappings.Caption = "Name"

            CustomResourceCollection.Add(CreateCustomResource(1, "Max Fowler", Color.PowderBlue))
            CustomResourceCollection.Add(CreateCustomResource(2, "Nancy Drewmore", Color.PaleVioletRed))
            CustomResourceCollection.Add(CreateCustomResource(3, "Pak Jang", Color.PeachPuff))

            Me.schedulerStorage1.Resources.DataSource = CustomResourceCollection
        End Sub

        Private Function CreateCustomResource(ByVal res_id As Integer, ByVal caption As String, ByVal ResColor As Color) As CustomResource
            Dim cr As New CustomResource()
            cr.ResID = res_id
            cr.Name = caption
            Return cr
        End Function



        Private Sub InitAppointments()
            Me.schedulerStorage1.Appointments.ResourceSharing = True
            Dim mappings As AppointmentMappingInfo = Me.schedulerStorage1.Appointments.Mappings
            mappings.Start = "StartTime"
            mappings.End = "EndTime"
            mappings.Subject = "Subject"
            mappings.AllDay = "AllDay"
            mappings.Description = "Description"
            mappings.Label = "Label"
            mappings.Location = "Location"
            mappings.RecurrenceInfo = "RecurrenceInfo"
            mappings.ReminderInfo = "ReminderInfo"
            mappings.ResourceId = "OwnerId"
            mappings.Status = "Status"
            mappings.Type = "EventType"

            GenerateEvents(CustomEventList)
            GenerateMultiResourceAppointment(CustomEventList)
            Me.schedulerStorage1.Appointments.DataSource = CustomEventList
        End Sub

        Private Sub GenerateEvents(ByVal eventList As BindingList(Of CustomAppointment))
            Dim count As Integer = schedulerStorage1.Resources.Count

            For i As Integer = 0 To count - 1
                Dim resource As Resource = schedulerStorage1.Resources(i)
                Dim subjPrefix As String = resource.Caption & "'s "
                Dim id As String = String.Format("<ResourceIds>" & ControlChars.CrLf & "<ResourceId Type=""System.Int32"" Value=""{0}"" />" & ControlChars.CrLf & "</ResourceIds>", resource.Id)
                eventList.Add(CreateEvent(subjPrefix & "meeting", id, 2, 5))
                eventList.Add(CreateEvent(subjPrefix & "travel", id, 3, 6))
                eventList.Add(CreateEvent(subjPrefix & "phone call", id, 0, 10))
            Next i
        End Sub


        Private Sub GenerateMultiResourceAppointment(ByVal eventList As BindingList(Of CustomAppointment))
            Dim count As Integer = schedulerStorage1.Resources.Count
            Dim resourceIds As String = "<ResourceIds>" & ControlChars.CrLf
            For i As Integer = 0 To count - 1
                Dim resource As Resource = schedulerStorage1.Resources(i)
                resourceIds &= String.Format("<ResourceId Type=""System.Int32"" Value=""{0}"" />", resource.Id)
            Next i
            resourceIds &= ControlChars.CrLf & "</ResourceIds>"

            eventList.Add(CreateEvent("Appointment shared between multiple resources", resourceIds, 1, 1))


        End Sub
        Private Function CreateEvent(ByVal subject As String, ByVal resourceId As String, ByVal status As Integer, ByVal label As Integer) As CustomAppointment
            Dim apt As New CustomAppointment()
            apt.Subject = subject
            apt.OwnerId = resourceId
            Dim rnd As Random = RandomInstance
            Dim rangeInMinutes As Integer = 60 * 24
            apt.StartTime = Date.Today + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes))
            apt.EndTime = apt.StartTime.Add(TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes \ 4)))
            apt.Status = status
            apt.Label = label
            Return apt
        End Function



    End Class
End Namespace
