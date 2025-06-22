using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace PhoneBook
{

    public partial class RegData : Form
    {
        private PhoneBook parentForm;
        private bool isReadOnly;


        public RegData(PhoneBook parent, Contact contact, bool readOnly = false)
        {
            InitializeComponent();
            this.parentForm = parent;

            this.isReadOnly = readOnly;

            // Show the contact data
            textBoxFirstName.Text = contact.FirstName;
            textBoxLastName.Text = contact.LastName;
            textBoxPhoneNumber.Text = contact.PhoneNumber;
            textBoxEmail.Text = contact.Email;

            // 🔒 Make fields read-only if View mode
            if (isReadOnly)
            {
                textBoxFirstName.ReadOnly = true;
                textBoxLastName.ReadOnly = true;
                textBoxPhoneNumber.ReadOnly = true;
                textBoxEmail.ReadOnly = true;
                button1.Enabled = false;         // Disable save button
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string firstName = textBoxFirstName.Text.Trim();
            string lastName = textBoxLastName.Text.Trim();
            string phone = textBoxPhoneNumber.Text.Trim();
            string email = textBoxEmail.Text.Trim();

            // 1. Check if any field is empty
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Validate email
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Validate phone (only digits and length between 7-15)
            if (!phone.All(char.IsDigit) || phone.Length < 7 || phone.Length > 15)
            {
                MessageBox.Show("Phone number must be numeric and 7–15 digits long.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. Create Contact object
            // 4. Read existing contacts from file
            List<Contact> contactList = new List<Contact>();
            string filePath = Path.Combine(Application.StartupPath, "phonebook.json");

            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                contactList = JsonConvert.DeserializeObject<List<Contact>>(existingJson) ?? new List<Contact>();
            }

            // 5. Determine next positive ID
            int nextId = 1;
            if (contactList.Any())
            {
                nextId = contactList.Max(c => c.Id) + 1;
            }

            Contact newContact = new Contact
            {
                Id = nextId, // Unique ID
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone
            };

            // 5. Read existing contacts from file

            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                contactList = JsonConvert.DeserializeObject<List<Contact>>(existingJson) ?? new List<Contact>();
            }



            // 6. Add new contact and save
            contactList.Add(newContact);

            // Sort and save
            contactList = contactList.OrderBy(c => c.FirstName).ToList();
            string updatedJson = JsonConvert.SerializeObject(contactList, Formatting.Indented);
            File.WriteAllText(filePath, updatedJson);

            MessageBox.Show("Contact saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Optionally clear input fields
            textBoxFirstName.Text = "";
            textBoxLastName.Text = "";
            textBoxEmail.Text = "";
            textBoxPhoneNumber.Text = "";


            parentForm.ReloadContactList(); // ✅ Call parent form method
            this.Close(); // close the AddContact form


        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


    }
}
