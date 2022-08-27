using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace AnonFilesUploader
{
#pragma warning disable CS8600
#pragma warning disable CS8602

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            selectFile.ShowDialog();
            textBox1.Text = selectFile.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var filePath = selectFile.FileName;

            if(!selectFile.CheckFileExists || !selectFile.CheckPathExists || filePath == null || filePath == "")
            {
                MessageBox.Show("You have to select a file to upload!", "AnonFiles Uploader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (WebClient client = new WebClient())
            {
                var jsonResp = client.UploadFile("https://api.anonfiles.com/upload", filePath);
                client.Dispose();

                var body = Encoding.UTF8.GetString(jsonResp);


                dynamic resp = JsonConvert.DeserializeObject(body);

                bool success = (bool)resp.status;
                if (!success)
                {
                    MessageBox.Show("Error! The file could not be uploaded.\n"
                        + resp.error.message + " Error code: " + resp.error.code,
                        "AnonFiles Uploader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Clipboard.SetText((string)resp.data.file.url.full);

                MessageBox.Show("The file was uploaded successfully!\nShort link was copied to the clipboard.\nFile ID: "
                    + resp.data.file.metadata.id + "\n Full URL: " + resp.data.file.url.full
                    , "AnonFiles Uploader", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}