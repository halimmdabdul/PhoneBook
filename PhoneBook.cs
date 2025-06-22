using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace PhoneBook
{
    public partial class PhoneBook : Form
    {
        List<Contact> contacts = new List<Contact>();

        public PhoneBook()
        {
            InitializeComponent();
            this.Load += PhoneBook_Load;
            this.dataGridView1.CellClick += dataGridView1_CellClick;

        }
        public void ReloadContactList()
        {
            string filePath = Path.Combine(Application.StartupPath, "phonebook.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                contacts = JsonConvert.DeserializeObject<List<Contact>>(json) ?? new List<Contact>();
            }
            LoadContacts();
        }


        private void PhoneBook_Load(object sender, EventArgs e)
        {
            if (File.Exists("phonebook.json"))
            {
                string json = File.ReadAllText("phonebook.json");
                contacts = JsonConvert.DeserializeObject<List<Contact>>(json);
            }

            LoadContacts();
        }



        private void LoadContacts()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = contacts;

            // Add Edit Button
            DataGridViewButtonColumn editButton = new DataGridViewButtonColumn();
            // Add View Button
            DataGridViewButtonColumn viewButton = new DataGridViewButtonColumn();
            viewButton.Text = "View";
            viewButton.UseColumnTextForButtonValue = true;
            viewButton.Name = "View";


            dataGridView1.Columns.Add(viewButton);
            editButton.Text = "Edit";
            editButton.UseColumnTextForButtonValue = true;
            editButton.Name = "Edit";
            dataGridView1.Columns.Add(editButton);

            // Add Delete Button
            DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn();
            deleteButton.Text = "Delete";
            deleteButton.UseColumnTextForButtonValue = true;
            deleteButton.Name = "Delete";
            dataGridView1.Columns.Add(deleteButton);

        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
                int contactId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                Contact selectedContact = contacts.FirstOrDefault(c => c.Id == contactId);

                if (selectedContact == null)
                {
                    MessageBox.Show("Contact not found.");
                    return;
                }

                if (columnName == "View")
                {
                    RegData viewForm = new RegData(this, selectedContact);
                    viewForm.ShowDialog(); // Open view form
                }
                else if (columnName == "Edit")
                {
                    // Optional: You can use same RegData form for edit with a flag
                    RegData editForm = new RegData(this, selectedContact);
                    editForm.ShowDialog();
                }
                else if (columnName == "Delete")
                {
                    var result = MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        contacts.RemoveAll(c => c.Id == contactId);
                        File.WriteAllText("phonebook.json", JsonConvert.SerializeObject(contacts, Formatting.Indented));
                        LoadContacts();
                    }
                }
            }
        }



        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Contact emptyContact = new Contact();
            RegData regForm = new RegData(this, emptyContact);
            regForm.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

            try
            {
                System.Diagnostics.Process.Start("calc.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open calculator: " + ex.Message);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

            AboutUs regForm = new AboutUs();
            regForm.Show();
        }
    }
}
