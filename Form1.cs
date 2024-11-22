using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CRUDProject
{

    public partial class CRUDTest : Form
    {

        DateTime today = DateTime.Today;
        public CRUDTest()
        {
            InitializeComponent();

            Check_Tasks_Today();
            update_Date_Label();
            task_Completion_Bar_Update();
            read_Data_Table_Date();

        }

        SqlConnection con = new SqlConnection(@"Your-Connection-String-Here");

        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();

            if (SearchTextBox.Text != "")
            {
                DateTime selectedTime = today;
                string dateString = selectedTime.ToString("yyyy-MM-dd");
                MessageBox.Show(dateString);

                using (SqlCommand cmd = new SqlCommand("dbo.Task_Search", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Task", SearchTextBox.Text);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    DataGridView.DataSource = dataTable;
                }


            }
            else
            {
                DateTime selectedTime = today;
                string dateString = selectedTime.ToString("yyyy-MM-dd");
                MessageBox.Show(dateString);

                SqlCommand cmd = new SqlCommand("dbo.Date_Search", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskDate", dateString);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                DataGridView.DataSource = dataTable;

            }
            con.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DateTime selectedTime = today;
            string dateString = selectedTime.ToString("yyyy-MM-dd");
            int rows = DataGridView.RowCount - 1;

            MessageBox.Show(rows.ToString());
            for (int i = 0; i < rows; i++)
            {
                Debug.WriteLine(DataGridView.Rows[i].Cells[0].Value);
                Debug.WriteLine(DataGridView.Rows[i].Cells[1].Value);
                Debug.WriteLine(DataGridView.Rows[i].Cells[2].Value);

                string task_update = DataGridView.Rows[i].Cells[0].Value.ToString();
                string task_completion_update = DataGridView.Rows[i].Cells[1].Value.ToString();
                string permatask_update = DataGridView.Rows[i].Cells[2].Value.ToString();
                update_With_Values(task_update, task_completion_update, dateString, permatask_update);
            }

            task_Completion_Bar_Update();
        }


        private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ////When the date picker changes value >>
            //DateTime selectedTime = Date.Value;
            //string dateString = selectedTime.ToString("yyyy-MM-dd");
            ////MessageBox.Show(dateString);
            //con.Open();
            ////create a new command "request"
            //SqlCommand cmd = new SqlCommand("dbo.Date_Search", con);
            //cmd.CommandType = CommandType.StoredProcedure;
            ////Add values to that command
            //cmd.Parameters.AddWithValue("@TaskDate", dateString);
            ////Adapt with the output of the command
            //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //DataTable dataTable = new DataTable();
            //adapter.Fill(dataTable);
            //DataGridView.DataSource = dataTable;
            //con.Close();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (SearchTextBox.Text == "")
            {
                return;
            }
            else
            {
                //would probably make sense to check if the selected task is permatask or not
                string deltask = SearchTextBox.Text;

                con.Open();

                SqlCommand cmd = new SqlCommand("dbo.Task_delete", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Task", deltask);

                cmd.ExecuteNonQuery();

                con.Close();

                MessageBox.Show("Successfully deleted task!");
                LoadAllRecords();
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void TodaysHealthBar_Click(object sender, EventArgs e)
        {

        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            if (SearchTextBox.Text == "")
            {
                return;
            }
            else
            {
                try
                {
                    string newtask = SearchTextBox.Text;
                    DateTime TodaysDate = DateTime.Today;
                    string dateString = TodaysDate.ToString("yyyy-MM-dd");
                    con.Open();

                    SqlCommand cmd = new SqlCommand("dbo.New_Task_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Task", newtask);
                    cmd.Parameters.AddWithValue("@TaskCompletion", "False");
                    cmd.Parameters.AddWithValue("@TaskDate", dateString);
                    cmd.Parameters.AddWithValue("PermaTask", "False");
                    cmd.ExecuteNonQuery();

                    con.Close();

                    MessageBox.Show("Successfully added new task!");
                    LoadAllRecords();
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void Check_Tasks_Today()
        {
            DateTime TodaysDate = DateTime.Today;
            string dateString = TodaysDate.ToString("yyyy-MM-dd");
            //MessageBox.Show(dateString);
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.Date_Search", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TaskDate", dateString);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            DataGridView.DataSource = dataTable;
            con.Close();
        }

        void LoadAllRecords()
        {
            SqlCommand cmd = new SqlCommand("dbo.Task_view", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            DataGridView.DataSource = dataTable;
            task_Completion_Bar_Update();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void update_Date_Label()
        {
            DateTime TodaysDate = DateTime.Today;
            string dateString = TodaysDate.ToString("yyyy-MM-dd");
            DateLabel.Text = ("Date: " + dateString);
        }

        private void update_On_Date_Change()
        {
            //Delete all tasks that are not "PermaTask"
            delete_Non_Permanent_Tasks();
            //Change remaining task_date on tasks to today( replace where TaskDate != Today)
            change_All_Task_Dates();
            //Uncomplete them all ( replace where TaskCompletion == true)
            reset_Task_Completion();
            LoadAllRecords();
        }

        void delete_Non_Permanent_Tasks()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.Delete_Non_Perma_Tasks", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            DataGridView.DataSource = dataTable;
            con.Close();
        }

        void change_All_Task_Dates()
        {
            DateTime TodaysDate = DateTime.Today;
            string dateString = TodaysDate.ToString("yyyy-MM-dd");
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.Update_Tasks_Date", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Task_Date", dateString);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            DataGridView.DataSource = dataTable;
            con.Close();
        }

        void read_Data_Table_Date()
        {
            //format date the same way SQLReader does
            DateTime TodaysDate = DateTime.Today;
            string dateString = TodaysDate.ToString("dd/MM/yyyy HH:mm:ss");
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from ToDoTable", con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader DReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            List<string> datesList = new List<string>();
            while (DReader.Read())
            {
                datesList.Add(DReader[2].ToString());
            }

            con.Close();



            if (datesList[0] != dateString)
            {
                Debug.WriteLine("The dates do not match");
                Debug.WriteLine(datesList[0] + " " + dateString);
                update_On_Date_Change();
                MessageBox.Show("Hello! Welcome to the new day! Hope you're ready to conquer it!");
            }
            else
            {
                Debug.WriteLine("They do match");
            }
        }

        void reset_Task_Completion()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.Reset_Task_Completion", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            DataGridView.DataSource = dataTable;
            con.Close();
        }

        void update_With_Values(string task, string task_completion, string date, string perma_task)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.Task_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Task", task);
            cmd.Parameters.AddWithValue("@TaskCompletion", task_completion);
            cmd.Parameters.AddWithValue("@TaskDate", date);
            cmd.Parameters.AddWithValue("@PermaTask", perma_task);
            cmd.ExecuteNonQuery();
            con.Close();

        }

        void task_Completion_Bar_Update()
        {
            int completionNumber = 0;
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from ToDoTable", con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader DReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            List<string> completionList = new List<string>();
            while (DReader.Read())
            {
                completionList.Add(DReader[1].ToString());
            }

            con.Close();

            for (int i = 0; i < completionList.Count; i++)
            {
                if (completionList[i] == "True")
                {
                    completionNumber++;
                }
            }
            Debug.WriteLine("Completed: " + completionNumber);
            float completionPercentage = ((float)completionNumber / (float)completionList.Count) * 100;
            Debug.WriteLine(completionPercentage + "%");
            TodaysHealthBar.Value = (int)completionPercentage;
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView.CurrentRow.Selected = true;
            SearchTextBox.Text = DataGridView.Rows[e.RowIndex].Cells["Task"].Value.ToString();
        }
    }
}
