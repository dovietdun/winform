using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace De2_Bai2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DataClasses1DataContext data = new DataClasses1DataContext();
        private void Form1_Load(object sender, EventArgs e)
        {
            data = new DataClasses1DataContext();
            Column1.DataSource = data.tblItemLists;
            Column1.DisplayMember = "ItemID";
            Column1.ValueMember = "ItemID";
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
            //
            cbx_khachhang.DataSource = data.tblCustomerLists;
            cbx_khachhang.DisplayMember = "CustomerID";
            cbx_khachhang.ValueMember = "CustomerID";
        }



       
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private string GetItemNameByItemID(String itemID)
        {
            // Sử dụng DataContext để lấy thông tin từ tblItemLists
            DataClasses1DataContext data = new DataClasses1DataContext();

            // Tìm đối tượng tblItemList tương ứng với ItemID
            var selectedItem = data.tblItemLists.SingleOrDefault(item => item.ItemID.Equals(itemID));

            // Kiểm tra nếu đối tượng được tìm thấy
            if (selectedItem != null)
            {
                // Trả về giá trị ItemName
                return selectedItem.ItemName;
            }

            // Nếu không tìm thấy, trả về chuỗi trống hoặc giá trị mặc định khác tùy thuộc vào yêu cầu của bạn
            return string.Empty;
        }
        private string GetDVTByItemID(String itemID)
        {
            // Sử dụng DataContext để lấy thông tin từ tblItemLists
            DataClasses1DataContext data = new DataClasses1DataContext();

            // Tìm đối tượng tblItemList tương ứng với ItemID
            var selectedItem = data.tblItemLists.SingleOrDefault(item => item.ItemID.Equals(itemID));

            // Kiểm tra nếu đối tượng được tìm thấy
            if (selectedItem != null)
            {
                // Trả về giá trị ItemName
                return selectedItem.InvUnitOfMeasr;
            }

            // Nếu không tìm thấy, trả về chuỗi trống hoặc giá trị mặc định khác tùy thuộc vào yêu cầu của bạn
            return string.Empty;
        }
        private void CalculateTotalQuantity1()
        {
            decimal totalQuantity = 0;

            // Lặp qua tất cả các hàng trong DataGridView và cộng số lượng
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["soluong"].Value != null)
                {
                    decimal soluong;
                    if (decimal.TryParse(row.Cells["soluong"].Value.ToString(), out soluong))
                    {
                        totalQuantity += soluong;
                    }
                }
            }

            // Gán giá trị tổng vào TextBox
            txt_soluong.Text = totalQuantity.ToString();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Kiểm tra xem hàng có tồn tại không
                if (row != null)
                {
                    DataGridViewCell cell = row.Cells[e.ColumnIndex];

                    // Kiểm tra xem ô cụ thể có tồn tại không
                    if (cell != null)
                    {
                        // Lấy giá trị số lượng và đơn giá từ ô DataGridView
                        object soluongObj = row.Cells["soluong"].Value;
                        object dongiaObj = row.Cells["dongia"].Value;

                        // Kiểm tra xem cả hai giá trị đã được nhập đủ chưa
                        if (soluongObj != null && dongiaObj != null)
                        {
                            decimal soluong, dongia;

                            if (decimal.TryParse(soluongObj.ToString(), out soluong) && decimal.TryParse(dongiaObj.ToString(), out dongia))
                            {
                                // Tính thành tiền và gán vào cột thanhtien
                                row.Cells["thanhtien"].Value = soluong * dongia;
                                // Gọi phương thức để tính tổng số lượng
                                CalculateTotalQuantity1();
                            }
                        }
                    }
                }
            }
        }
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.OwningColumn.Name == "Column1" && e.Control is ComboBox)
            {
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            }

        }
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            // Lấy tên của mục đã chọn từ ComboBox
            string selectedName = GetItemNameByItemID(comboBox.Text);
            string dvt = GetDVTByItemID(comboBox.Text);
            // Lấy chỉ số hàng và cột của ô ComboBox
            int rowIndex = dataGridView1.CurrentCell.RowIndex;
            int columnIndex = dataGridView1.CurrentCell.ColumnIndex;

            // Gán tên vào Column2 của hàng đã được chọn
            dataGridView1.Rows[rowIndex].Cells["Column2"].Value = selectedName;
            dataGridView1.Rows[rowIndex].Cells["Column3"].Value = dvt;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txt_soluong_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbx_khachhang_SelectedValueChanged(object sender, EventArgs e)
        {
            tblCustomerList customerList = data.tblCustomerLists.Where(kh => kh.CustomerID.Equals(cbx_khachhang.Text)).FirstOrDefault();
            if (customerList != null)
            {
                txt_tenkhachhang.Text = customerList.CustomerName;
                txt_diachi.Text = customerList.Address;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                tblOrderMaster newObj = new tblOrderMaster();
                newObj.OrderDate = Convert.ToDateTime(txt_date.Text);
                newObj.OrderNo = txt_sohoadon.Text;
                newObj.CustomerID = cbx_khachhang.Text;
                newObj.TotalAmount = Convert.ToInt32(txt_soluong.Text);
                data.tblOrderMasters.InsertOnSubmit(newObj);
                data.SubmitChanges();
                int lineNumber = 1; // Biến đếm dòng
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Kiểm tra xem hàng có dữ liệu không
                    if (!row.IsNewRow)
                    {
                        // Tạo đối tượng tblOrderDetail cho mỗi hàng
                        tblOrderDetail newDetail = new tblOrderDetail();
                        newDetail.OrderID = newObj.OrderID; // Liên kết với OrderNo của tblOrderMaster
                        newDetail.ItemID = row.Cells["Column1"].Value.ToString(); // Thay "Column1" bằng tên cột thích hợp
                        newDetail.LineNumber = lineNumber; // Gán giá trị của biến đếm
                        lineNumber++; // Tăng giá trị biến đếm
                        newDetail.Quantity = Convert.ToInt32(row.Cells["soluong"].Value); // Thay "soluong" bằng tên cột thích hợp
                        newDetail.Price = Convert.ToDouble(row.Cells["thanhtien"].Value); // Thay "dongia" bằng tên cột thích hợp
                        newDetail.Amount = Convert.ToInt32(row.Cells["soluong"].Value); // Thay "thanhtien" bằng tên cột thích hợp

                        // Thêm đối tượng tblOrderDetail vào tblOrderDetails
                        data.tblOrderDetails.InsertOnSubmit(newDetail);
                    }
                }
                data.SubmitChanges();
                MessageBox.Show("Thêm thành công");
            }
            catch(Exception ex)
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            data = new DataClasses1DataContext();
            Column1.DataSource = data.tblItemLists;
            Column1.DisplayMember = "ItemID";
            Column1.ValueMember = "ItemID";
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
            //
            cbx_khachhang.DataSource = data.tblCustomerLists;
            cbx_khachhang.DisplayMember = "CustomerID";
            cbx_khachhang.ValueMember = "CustomerID";
            //
            txt_sohoadon.Text = "";
            txt_soluong.Text = "";
            txt_diachi.Text = "";
            txt_tenkhachhang.Text = "";
        }
    }
}
