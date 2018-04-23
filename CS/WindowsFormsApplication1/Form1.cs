using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraScheduler;

namespace WindowsFormsApplication1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        public static Random RandomInstance = new Random();

        private BindingList<CustomResource> CustomResourceCollection = new BindingList<CustomResource>();
        private BindingList<CustomAppointment> CustomEventList = new BindingList<CustomAppointment>();

        private void Form1_Load(object sender, EventArgs e) {
            InitResources();
            InitAppointments();
            schedulerControl1.Start = DateTime.Now;
            schedulerStorage1.AppointmentDeleting += new PersistentObjectCancelEventHandler(schedulerStorage1_AppointmentDeleting);
            schedulerControl1.GroupType = DevExpress.XtraScheduler.SchedulerGroupType.Resource;
        }
        List<CustomAppointment> cache = new List<CustomAppointment>();
        void schedulerStorage1_AppointmentDeleting(object sender, PersistentObjectCancelEventArgs e)
        {
           Appointment appt = e.Object as Appointment;
           CustomAppointment deletedAppointment = appt.GetSourceObject(schedulerStorage1) as CustomAppointment;
           cache.Add(deletedAppointment);
        }

        private void InitResources() {
            ResourceMappingInfo mappings = this.schedulerStorage1.Resources.Mappings;
            mappings.Id = "ResID";
            mappings.Caption = "Name";

            CustomResourceCollection.Add(CreateCustomResource(1, "Max Fowler", Color.PowderBlue));
            CustomResourceCollection.Add(CreateCustomResource(2, "Nancy Drewmore", Color.PaleVioletRed));
            CustomResourceCollection.Add(CreateCustomResource(3, "Pak Jang", Color.PeachPuff));

            this.schedulerStorage1.Resources.DataSource = CustomResourceCollection;
        }

        private CustomResource CreateCustomResource(int res_id, string caption, Color ResColor) {
            CustomResource cr = new CustomResource();
            cr.ResID = res_id;
            cr.Name = caption;
            return cr;
        }



        private void InitAppointments() {
            this.schedulerStorage1.Appointments.ResourceSharing = true;
            AppointmentMappingInfo mappings = this.schedulerStorage1.Appointments.Mappings;
            mappings.Start = "StartTime";
            mappings.End = "EndTime";
            mappings.Subject = "Subject";
            mappings.AllDay = "AllDay";
            mappings.Description = "Description";
            mappings.Label = "Label";
            mappings.Location = "Location";
            mappings.RecurrenceInfo = "RecurrenceInfo";
            mappings.ReminderInfo = "ReminderInfo";
            mappings.ResourceId = "OwnerId";
            mappings.Status = "Status";
            mappings.Type = "EventType";

            GenerateEvents(CustomEventList);
            GenerateMultiResourceAppointment(CustomEventList);
            this.schedulerStorage1.Appointments.DataSource = CustomEventList;
        }

        private void GenerateEvents(BindingList<CustomAppointment> eventList)
        {
            int count = schedulerStorage1.Resources.Count;

            for (int i = 0; i < count; i++)
            {
                Resource resource = schedulerStorage1.Resources[i];
                string subjPrefix = resource.Caption + "'s ";
                string id = String.Format("<ResourceIds>\r\n<ResourceId Type=\"System.Int32\" Value=\"{0}\" />\r\n</ResourceIds>", resource.Id);
                eventList.Add(CreateEvent(subjPrefix + "meeting", id, 2, 5));
                eventList.Add(CreateEvent(subjPrefix + "travel", id, 3, 6));
                eventList.Add(CreateEvent(subjPrefix + "phone call", id, 0, 10));
            }
        }


        private void GenerateMultiResourceAppointment(BindingList<CustomAppointment> eventList) {
            int count = schedulerStorage1.Resources.Count;
            string resourceIds = "<ResourceIds>\r\n";
            for (int i = 0; i < count; i++)
            {
                Resource resource = schedulerStorage1.Resources[i];
                resourceIds += String.Format("<ResourceId Type=\"System.Int32\" Value=\"{0}\" />", resource.Id);
            }
            resourceIds += "\r\n</ResourceIds>";

            eventList.Add(CreateEvent("Appointment shared between multiple resources", resourceIds, 1, 1));


        }
        private CustomAppointment CreateEvent(string subject, string resourceId, int status, int label) {
            CustomAppointment apt = new CustomAppointment();
            apt.Subject = subject;
            apt.OwnerId = resourceId;
            Random rnd = RandomInstance;
            int rangeInMinutes = 60 * 24;
            apt.StartTime = DateTime.Today + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes));
            apt.EndTime = apt.StartTime + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes / 4));
            apt.Status = status;
            apt.Label = label;
            return apt;
        }

 

    }
}
