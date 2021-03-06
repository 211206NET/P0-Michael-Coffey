namespace DL;

using Microsoft.Data.SqlClient;
using System.Data;
using CustomExceptions;
using Serilog;


///<summary>
///This relates to the information in the database, as it reveals important data and allows the user to manipulate it.
///</summary>
public class DBRepo : IMRepo{ 

///<summary>
///This string connects the repo to the databse
///</summary>
    private string _connectionString; 

///<summary>
///This constructor establishes the connection between the repo and the database.
///</summary>
///<param name="connectionString">This gives a value to the _connectionString variable.</param>
    public DBRepo(string connectionString){
        _connectionString = connectionString;
        //Console.WriteLine(_connectionString);
    }

    ///<summary>
    ///This returns the values of each storefront from the database to a list of storefront objects.
    ///</summary>
    ///<returns>A list of the storefronts from the database</returns>
    public List<Storefront> GetAllStorefronts(){
        List<Storefront> allStorefronts = new List<Storefront>();
        using SqlConnection connection = new SqlConnection(_connectionString);
        string stoSelect = "SELECT * FROM Storefront";
        string invSelect = "SELECT * FROM Inventory";
        string histSelect = "SELECT * FROM StoreOrderHistory";
        string proSelect = "SELECT * FROM Product";
        string ordSelect = "SELECT * FROM ItemOrder";
        DataSet FSSet = new DataSet();
        using SqlDataAdapter stoAdapter = new SqlDataAdapter(stoSelect, connection);
        using SqlDataAdapter invAdapter = new SqlDataAdapter(invSelect, connection);
        using SqlDataAdapter hisAdapter = new SqlDataAdapter(histSelect, connection);
        using SqlDataAdapter proAdapter = new SqlDataAdapter(proSelect, connection);
        using SqlDataAdapter ordAdapter = new SqlDataAdapter(ordSelect, connection);
        stoAdapter.Fill(FSSet, "Storefront");
        invAdapter.Fill(FSSet, "Inventory");
        hisAdapter.Fill(FSSet, "StoreOrderHistory");
        proAdapter.Fill(FSSet, "Product");
        ordAdapter.Fill(FSSet, "ItemOrder");
        DataTable? StorefrontTable = FSSet.Tables["Storefront"];
        DataTable? InventoryTable = FSSet.Tables["Inventory"];
        DataTable? HistoryTable = FSSet.Tables["StoreOrderHistory"];
        DataTable? ProductTable = FSSet.Tables["Product"];
        DataTable? OrderTable = FSSet.Tables["ItemOrder"];
        if(StorefrontTable != null && InventoryTable != null){
            foreach(DataRow row in StorefrontTable.Rows){
                Storefront nsto = new Storefront(row);
                // nsto.InventoryID = InventoryTable.AsEnumerable().Where(r => (int) r["InventoryID"] == nsto.InvnetoryID).Select(
                //     r => 
                //         new Inventory{
                //             InventoryID = (int) r["InventoryID"],
                //             ProductID = (int) r["ProductID"],
                //             Quantity = (int) r["Quantity"]
                //         }
                // ).ToList();
                // nsto.SOrderHistoryID = HistoryTable.AsEnumerable().Where(r => (int) r["SOrderHistoryID"] == nsto.SOrderHistoryID).Select(
                //     r => 
                //          new Order{
                //             OrderID = (int) r["OrderID"],
                //             DateOfOrder = (Date) r["DateOfOrder"],
                //             CustomerID = (int) r["CustomerID"],
                //             StoreID = (int) r["StoreID"],
                //             Total = (decimal) r["Total"],
                //             LineItemID = (int) r["LineItemID"]
                //          }
                // ).ToList();
                allStorefronts.Add(nsto);
            }
        }
        return allStorefronts;
    }

///<summary>
///This returns the values of each customer from the database into a list of customer objects.
///</summary>
///<returns>A list of customers from the database</returns>
    public List<Customer> GetAllCustomers(){
        List<Customer> allCustomers = new List<Customer>();
        using SqlConnection connection = new SqlConnection(_connectionString);
        string custCollect = "SELECT * FROM Customer";
        string histCollect = "SELECT * FROM CustomerOrderHistory";
        string ordCollect = "SELECT * FROM ItemOrder";
        string linorCollect = "SELECT * FROM LineOrder";
        string proCollect = "SELECT * FROM Product";
        DataSet cusSet = new DataSet();
        using SqlDataAdapter custAdapter = new SqlDataAdapter(custCollect, connection);
        using SqlDataAdapter histCollector = new SqlDataAdapter(histCollect, connection);
        using SqlDataAdapter ordCollector = new SqlDataAdapter(ordCollect, connection);
        using SqlDataAdapter linorCollector = new SqlDataAdapter(linorCollect, connection);
        using SqlDataAdapter proCollector = new SqlDataAdapter(proCollect, connection);
        custAdapter.Fill(cusSet, "Customer");
        histCollector.Fill(cusSet, "CustomerOrderHistory");
        ordCollector.Fill(cusSet, "ItemOrder");
        linorCollector.Fill(cusSet, "LineOrder");
        proCollector.Fill(cusSet, "Product");
        DataTable? CustomerTable = cusSet.Tables["Customer"];
        DataTable? HistoryTable = cusSet.Tables["CustomerOrderHistory"];
        DataTable? OrderTable = cusSet.Tables["ItemOrder"];
        DataTable? LineOrderTable = cusSet.Tables["LineOrder"];
        DataTable? ProductTable = cusSet.Tables["Product"];
        if(CustomerTable != null && HistoryTable != null){
            foreach(DataRow row in CustomerTable.Rows){
                Customer cus = new Customer(row);
                // cus.Orders = HistoryTable.AsEnumerable().Where(r => (int) r["COrderHistoryID"] = cus.Orders).Select(
                //     r => 
                //           new Order{
                //               OrderID = (int) row["OrderID"],
                //               DateOfOrder = (Date) row["DateOfOrder"],
                //               CustomerID = (int) row["CustomerID"],
                //               StoreID = (int) row["StoreID"],
                //               Total = (decimal) row["Total"],
                //               LineItemID = (int) row["LineItemID"]
                //           }
                // ).ToList();
                allCustomers.Add(cus);
            }
        }
        return allCustomers;
    }

///<summary>
///This function adds a storefront value to the database along with the needed data.
///</summary>
///<param i ="_name">The name of the new storefront</param>
///<param i = "_address">The address of the new store</param>
///<param i = "_inventoryid">The id for the inventory of the new storefront</param>
    public void AddStorefront(string _name, string _address, int _inventoryid){
        createStorefront:
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string cmdForSql = "INSERT INTO Storefront (Name, Address, InventoryID) VALUES (@nam, @addr, @invid)";
            string sOrderSql = "INSERT INTO StoreOrderHistory DEFAULT VALUES";
            using(SqlCommand cmd0 = new SqlCommand(sOrderSql, connection)){
                cmd0.ExecuteNonQuery();
            }
            using(SqlCommand cmd = new SqlCommand(cmdForSql, connection)){
                SqlParameter param = new SqlParameter("@nam", _name);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@addr", _address);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@invid", _inventoryid);
                cmd.Parameters.Add(param);
                // param = new SqlParameter("@soh", _sorderhistoryid);
                // cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
        try{
            Storefront nStorefront = new Storefront(_address, _name, _inventoryid);
        }
        catch(InputInvalidException ex){
            Console.WriteLine(ex.Message);
            goto createStorefront;
        }
        catch(DuplicateException ex){
            Console.WriteLine(ex.Message);
            goto createStorefront;
        }
        Log.Information("new storefront added {_name}", _name);
    }

///<summary>
///This adds an inventory value to the database.
///</summary>
///<param i = "idOfItem">Id of the item found in the inventory</param>
///<param i = "amount">The amount of a particular item in the new inventory</param>
    public void AddInventory(int idOfItem, int amount){
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string sqlCmd = "INSERT INTO Inventory (ProductID, Quantity) VALUES (@proID, @quan)";
            using(SqlCommand cmd = new SqlCommand(sqlCmd, connection)){
                // SqlParameter param = new SqlParameter("@invID", idOfInventory);
                // cmd.Parameters.Add(param);
                SqlParameter param = new SqlParameter("@proID", idOfItem);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@quan", amount);
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    ///<summary>
    ///This refils the stock of a particular inventory.
    ///</summary>
    ///<param i = "idOfItem">Id of the item found in the particular inventory</param>
    ///<param i = "idOfInventory">Id of the inventory that needs more supplies</param>
    ///<param i = "numberToAdd">Amount that will be added to the inventory</param>
    public void ReplenishStock(int idOfItem, int idOfInventory, int numberToAdd){
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string sqlCmd = "UPDATE Inventory SET Quantity += @quan WHERE InventoryID = @invID AND ProductID = @proID";
            using(SqlCommand cmd = new SqlCommand(sqlCmd, connection)){
                SqlParameter param = new SqlParameter("@invID", idOfInventory);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@proID", idOfItem);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@quan", numberToAdd);
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
        Log.Information("user has added {numberToAdd} to {idOfInventory}", numberToAdd, idOfInventory);
    }

    ///<summary>
    ///This allows the user to place an order through the app.
    ///</summary>
    ///<param i = "idOfItem">ID for the item the user wants to get</param>
    ///<param i = "numberOfItems">Amount that the user wnats to buy</param>
    ///<param i = "nStore">ID for the store with the particular item</param>
    ///<param i = "cusid">ID for the customer that is making the purchase</param>
    public void PlaceAnOrder(int idOfItem, int numberOfItems, int nStore, int cusid){
        List<Customer> allCustomers = GetAllCustomers();
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string sqlCmd = "INSERT INTO LineOrder (ProductID, Quantity) VALUES (@proID, @quan)";
            string sqlCmd2 = "UPDATE Inventory SET Quantity -= @quan WHERE ProductID = @proID";
            string orderCmd = "INSERT INTO ItemOrder (DateOfOrder, CustomerID, StoreID, LineItemID) VALUES (GETDATE(), @cusID, @stoID, (SELECT LineItemID FROM LineOrder WHERE ProductID = @proID AND Quantity = @quan))";
            //string cusSelect = "SELECT * FROM Customer WHERE UserName = @cusNam";
            using(SqlCommand cmd = new SqlCommand(sqlCmd, connection)){
                // SqlParameter param = new SqlParameter("@linID", idOfLineOrder);
                // cmd.Parameters.Add(param);
                SqlParameter param = new SqlParameter("@proID", idOfItem);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@quan", numberOfItems);
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
            using(SqlCommand cmd2 = new SqlCommand(sqlCmd2, connection)){
                SqlParameter param2 = new SqlParameter("@quan", numberOfItems);
                cmd2.Parameters.Add(param2);
                param2 = new SqlParameter("@proID", idOfItem);
                cmd2.Parameters.Add(param2);
                cmd2.ExecuteNonQuery();
            }
            using(SqlCommand cmd3 = new SqlCommand(orderCmd, connection)){
                SqlParameter param3 = new SqlParameter("@cusID", cusid);
                cmd3.Parameters.Add(param3);
                param3 = new SqlParameter("@stoID", nStore);
                cmd3.Parameters.Add(param3);
                param3 = new SqlParameter("@proID", idOfItem);
                cmd3.Parameters.Add(param3);
                param3 = new SqlParameter("@quan", numberOfItems);
                cmd3.Parameters.Add(param3);
                cmd3.ExecuteNonQuery();
                // param3 = new SqlParameter("@liorID", idOfLineOrder);
                // cmd3.ExecuteNonQuery();
            }
            connection.Close();
        }
        Log.Information("new order placed into the database {cusid} {idOfItem}", cusid, idOfItem);
    }

    ///<summary>
    ///Adds a customer to the database.
    ///</summary>
    ///<param i = "_username">Username for the new customer</param>
    ///<param i = "_email">Email address of the new customer</param>
    ///<param i = "_password">Password for the new customer's account</param>
    public void AddCustomer(string _username, string _email, string _password){
        createCustomer:
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string cmdSql = "INSERT INTO Customer (UserName, Email, Password) VALUES (@usna, @emai, @paswor)";
            string histCmd = "INSERT INTO CustomerOrderHistory DEFAULT VALUES";
            using(SqlCommand cmd0 = new SqlCommand(histCmd, connection)){
                cmd0.ExecuteNonQuery();
            }
            using(SqlCommand cmd = new SqlCommand(cmdSql, connection)){
                SqlParameter param = new SqlParameter("@usna", _username);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@emai", _email);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@paswor", _password);
                cmd.Parameters.Add(param);
                // param = new SqlParameter("@coh", _corderhistoryid);
                // cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
        try{
            Customer nCustomer = new Customer(_username, _password, _email);
        }
        catch(InputInvalidException ex){
            Console.WriteLine(ex.Message);
            goto createCustomer;
        }
        catch(DuplicateException ex){
            Console.WriteLine(ex.Message);
            goto createCustomer;
        }
        Log.Information("new customer added to the database {_username}", _username);
    }

///<summary>
///Adds a product to the database.
///</summary>
///<param i = "_ID">ID of the new product</param>
///<param i = "_name">Title of the film</param>
///<param i = "_price">Price of the new product</param>
///<param i = "_year">ID for the release year of the film</param>
///<param i = "_director">ID for the director of the film</param>
///<param i = "_rating">ID for the MPA Rating of the film</param>
    public void AddProduct(int _ID, string _name, decimal _price, int _year, int _director, int _rating){
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string sqlPro = "INSERT INTO Product (ProductID, Title, Price, YearID, DirectorID, RatingID) VALUES (@proid, @tle, @pri, @yea, @dir, @rat)";
            using(SqlCommand cmd = new SqlCommand(sqlPro, connection)){
                SqlParameter param = new SqlParameter("@proid", _ID);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@tle", _name);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@pri", _price);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@yea", _year);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@dir", _director);
                cmd.Parameters.Add(param);
                param = new SqlParameter("@rat", _rating);
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

///<summary>
///Deletes a customer from the database.
///</summary>
///<param i = "_userName">username of the user</param>
    public void DeleteCustomer(string _userName){
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string delCmd = "DELETE FROM Customer WHERE UserName = @usnam";
            using(SqlCommand cmd = new SqlCommand(delCmd, connection)){
                // SqlParameter param = new SqlParameter("@cusid", _cusID);
                // cmd.Parameters.Add(param);
                SqlParameter param = new SqlParameter("@usnam", _userName);
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

///<summary>
///Returns the inventory of a store.
///</summary>
///<param i = "_storeid">ID of the store</param>
///<returns>The products found in a storefront</returns>
public List<Product> GetInventory(int _storeid){
    List<Product> allProducts = new List<Product>();
    using(SqlConnection connection = new SqlConnection(_connectionString)){
        connection.Open();
        string proCmd = $"SELECT Inventory.InventoryID, Product.Title, Product.Price, Director.DirectorName, ReleaseYear.Year, MPARating.Rating, Inventory.Quantity FROM Inventory INNER JOIN Product ON Inventory.ProductID = Product.ProductID INNER JOIN Director ON Product.DirectorID = Director.DirectorID INNER JOIN ReleaseYear ON Product.YearID = ReleaseYear.YearID INNER JOIN MPARating ON Product.RatingID = MPARating.RatingID WHERE Inventory.InventoryID = (SELECT InventoryID FROM Storefront WHERE StoreID = {_storeid})";
        using(SqlCommand cmd = new SqlCommand(proCmd, connection)){
            using(SqlDataReader reader = cmd.ExecuteReader()){
                while(reader.Read()){
                    Product nProduct = new Product();
                    nProduct.InventoryID = reader.GetInt32(0);
                    nProduct.ProductName = reader.GetString(1);
                    nProduct.Price = reader.GetDecimal(2);
                    nProduct.Director = reader.GetString(3);
                    nProduct.ReleaseYear = reader.GetInt32(4);
                    nProduct.MPARating = reader.GetString(5);
                    nProduct.Quantity = reader.GetInt32(6);
                }
            }
        }
        connection.Close();
    }
    return allProducts;
}
///<summary>
///Returns the order history of the customer organized by the date of the order.
///</summary>
///<param i = "_userid">ID of the user</param>
///<returns>The orders of the customer by date</returns>
    public List<Order> GetCustomerOrderHistoryDate(int _userid){
        List<Order> ordHistory = new List<Order>();
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string selectOrHis = $"SELECT ItemOrder.OrderID, ItemOrder.DateOfOrder, Customer.UserName, Storefront.Name, LineOrder.Quantity*Product.Price AS Total, LineOrder.LineItemID FROM ItemOrder INNER JOIN LineOrder ON ItemOrder.LineItemID = LineOrder.LineItemID INNER JOIN Product ON LineOrder.ProductID = Product.ProductID INNER JOIN Customer ON ItemOrder.CustomerID = Customer.CustomerID INNER JOIN Storefront ON ItemOrder.StoreID = Storefront.StoreID WHERE ItemOrder.CustomerID = {_userid} ORDER BY ItemOrder.DateOfOrder DESC";
            using(SqlCommand cmd = new SqlCommand(selectOrHis, connection)){
                using(SqlDataReader reader = cmd.ExecuteReader()){
                    while(reader.Read()){
                        Order nOrder = new Order();
                        nOrder.OrderNumber = reader.GetInt32(0);
                        nOrder.OrderDate = reader.GetDateTime(1);
                        nOrder.CustomerName = reader.GetString(2);
                        nOrder.StoreName = reader.GetString(3);
                        nOrder.Total = reader.GetDecimal(4);
                        nOrder.LineItemID = reader.GetInt32(5);
                        ordHistory.Add(nOrder);
                    }
                }
            }
            connection.Close();
        }
        // DataSet chSet = new DataSet();
        // using SqlDataAdapter hisAdapter = new SqlDataAdapter(selectOrHis, connection);
        // hisAdapter.Fill(chSet, "ItemOrder");
        // DataTable? HistoryTable = chSet.Tables["ItemOrder"];
        // if(HistoryTable != null){
        //     foreach(DataRow row in HistoryTable.Rows){
        //         Order nord = new Order(row);
        //         ordHistory.Add(nord);
        //     }
        // }
        return ordHistory;
    }

///<summary>
///Returns the order hsitory of the customer organized by the cost of the order. 
///</summary>
///<param i = "_userid">ID for the customer</param>
///<returns>A list of the customer's orders by the cost</returns>
    public List<Order> GetCustomerOrderHistoryCost(int _userid){
        List<Order> ordHistory = new List<Order>();
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string selectOrHis = $"SELECT ItemOrder.OrderID, ItemOrder.DateOfOrder, Customer.UserName, Storefront.Name, LineOrder.Quantity*Product.Price AS Total, LineOrder.LineItemID FROM ItemOrder INNER JOIN LineOrder ON ItemOrder.LineItemID = LineOrder.LineItemID INNER JOIN Product ON LineOrder.ProductID = Product.ProductID INNER JOIN Customer ON ItemOrder.CustomerID = Customer.CustomerID INNER JOIN Storefront ON ItemOrder.StoreID = Storefront.StoreID WHERE ItemOrder.CustomerID = {_userid} ORDER BY Total DESC";
            using(SqlCommand cmd = new SqlCommand(selectOrHis, connection)){
                using(SqlDataReader reader = cmd.ExecuteReader()){
                    while(reader.Read()){
                        Order nOrder = new Order();
                        nOrder.OrderNumber = reader.GetInt32(0);
                        nOrder.OrderDate = reader.GetDateTime(1);
                        nOrder.CustomerName = reader.GetString(2);
                        nOrder.StoreName = reader.GetString(3);
                        nOrder.Total = reader.GetDecimal(4);
                        nOrder.LineItemID = reader.GetInt32(5);
                        ordHistory.Add(nOrder);
                    }
                }
            }
            connection.Close();
        }
        // DataSet chSet = new DataSet();
        // using SqlDataAdapter hisAdapter = new SqlDataAdapter(selectOrHis, connection);
        // hisAdapter.Fill(chSet, "ItemOrder");
        // DataTable? HistoryTable = chSet.Tables["ItemOrder"];
        // DataTable? CustomerTable = chSet.Tables["Customer"];
        // DataTable? StorefrontTable = chSet.Tables["Storefront"];
        // DataTable? LineOrderTable = chSet.Tables["LineOrder"];
        // DataTable? ProductTable = chSet.Tables["Product"];
        // if(HistoryTable != null){
        //     foreach(DataRow row in HistoryTable.Rows){
        //         Order nord = new Order(row);
        //         ordHistory.Add(nord);
        //     }
        // }
        return ordHistory;
    }

///<summary>
///Returns the order history of a storefront organized by the date of the order.
///</summary>
///<param i = "_storeid">The ID of the store</param>
///<returns>List of the orders from the storefront by date</returns>
    public List<Order> GetStorefrontOrderHistoryDate(int _storeid){
        List<Order> ordHistory = new List<Order>();
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string selectOrHis = $"SELECT ItemOrder.OrderID, ItemOrder.DateOfOrder, Customer.UserName, Storefront.Name, LineOrder.Quantity*Product.Price AS Total, LineOrder.LineItemID FROM ItemOrder INNER JOIN LineOrder ON LineOrder.LineItemID = ItemOrder.LineItemID INNER JOIN Product ON LineOrder.ProductID = Product.ProductID INNER JOIN Customer ON ItemOrder.CustomerID = Customer.CustomerID INNER JOIN Storefront ON ItemOrder.StoreID = Storefront.StoreID WHERE ItemOrder.StoreID = {_storeid} ORDER BY DateOfOrder DESC";
            using(SqlCommand cmd = new SqlCommand(selectOrHis, connection)){
                using(SqlDataReader reader = cmd.ExecuteReader()){
                    while(reader.Read()){
                        Order nOrder = new Order();
                        nOrder.OrderNumber = reader.GetInt32(0);
                        nOrder.OrderDate = reader.GetDateTime(1);
                        nOrder.CustomerName = reader.GetString(2);
                        nOrder.StoreName = reader.GetString(3);
                        nOrder.Total = reader.GetDecimal(4);
                        nOrder.LineItemID = reader.GetInt32(5);
                        ordHistory.Add(nOrder);
                    }
                }
            }
            connection.Close();
        }
        // DataSet chSet = new DataSet();
        // using SqlDataAdapter hisAdapter = new SqlDataAdapter(selectOrHis, connection);
        // hisAdapter.Fill(chSet, "ItemOrder");
        // DataTable? HistoryTable = chSet.Tables["ItemOrder"];
        // if(HistoryTable != null){
        //     foreach(DataRow row in HistoryTable.Rows){
        //         Order nord = new Order(row);
        //         ordHistory.Add(nord);
        //     }
        // }
        return ordHistory;
    }

///<summary>
///Returns the order history of a storefront organized by the cost.
///</summary>
///<param i = "_storeid">ID for the store</param>
///<returns>list of orders from the store by cost</returns>
    public List<Order> GetStorefrontOrderHistoryCost(int _storeid){
        List<Order> ordHistory = new List<Order>();
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Open();
            string selectOrHis = $"SELECT ItemOrder.OrderID, ItemOrder.DateOfOrder, Customer.UserName, Storefront.Name, LineOrder.Quantity*Product.Price AS Total, LineOrder.LineItemID FROM ItemOrder INNER JOIN LineOrder ON LineOrder.LineItemID = ItemOrder.LineItemID INNER JOIN Product ON LineOrder.ProductID = Product.ProductID INNER JOIN Customer ON ItemOrder.CustomerID = Customer.CustomerID INNER JOIN Storefront ON ItemOrder.StoreID = Storefront.StoreID WHERE ItemOrder.StoreID = {_storeid} ORDER BY Total DESC";
            using(SqlCommand cmd = new SqlCommand(selectOrHis, connection)){
                using(SqlDataReader reader = cmd.ExecuteReader()){
                    while(reader.Read()){
                        Order nOrder = new Order();
                        nOrder.OrderNumber = reader.GetInt32(0);
                        nOrder.OrderDate = reader.GetDateTime(1);
                        nOrder.CustomerName = reader.GetString(2);
                        nOrder.StoreName = reader.GetString(3);
                        nOrder.Total = reader.GetDecimal(4);
                        nOrder.LineItemID = reader.GetInt32(5);
                        ordHistory.Add(nOrder);
                    }
                }
            }
            connection.Close();
        }
        // DataSet chSet = new DataSet();
        // using SqlDataAdapter hisAdapter = new SqlDataAdapter(selectOrHis, connection);
        // hisAdapter.Fill(chSet, "ItemOrder");
        // DataTable? HistoryTable = chSet.Tables["ItemOrder"];
        // if(HistoryTable != null){
        //     foreach(DataRow row in HistoryTable.Rows){
        //         Order nord = new Order(row);
        //         ordHistory.Add(nord);
        //     }
        // }
        return ordHistory;
    }

///<summary>
//Checks if a new storefront is a duplicate of a preexisting store.
///</summary>
///<param i = "storefront">The new storefront that is used for analysis</param>
///<returns>A boolean that indicates the stores relation with the rest of the database</param>
    public bool IsStorefrontDuplicate(Storefront storefront){
        string searchQuery = $"SELECT * FROM Storefront WHERE Name='{storefront.Name}' AND Address='{storefront.Address}'";
        using SqlConnection connection = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(searchQuery, connection);
        connection.Open();
        using SqlDataReader reader = cmd.ExecuteReader();
        if(reader.HasRows){
            return true;
        }
        return false;
    }

///<summary>
///Checks is a new customer is a duplicate of a preexisting account.
///</summary>
///<param i = "customer">Customer that is used for analysis</param>
///<returns>A boolean that determines the customer's relationship with the database.</returns>
    public bool IsCustomerDuplicate(Customer customer){
        string searchQuery = $"SELECT * FROM Customer WHERE Username='{customer.UserName}' AND Email = '{customer.Email}'";
        using SqlConnection connection = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(searchQuery, connection);
        connection.Open();
        using SqlDataReader reader = cmd.ExecuteReader();
        if(reader.HasRows){
            return true;
        }
        return false;
    }

    // public List<Storefront> SearchStorefronts(string searchItem){
    //     string searchStatement = $"SELECT * FROM Storefront WHERE Name LIKE '%{searchItem}%' OR Address LIKE '%{searchItem}%'";
    //     using SqlConnection connection = new SqlConnection(_connectionString);
    //     using SqlCommand cmd = new SqlCommand(searchStatement, connection);
    //     using SqlDataAdapter adapter = new SqlDataAdapter(cmd);
    //     DataSet StorefrontSets = new DataSet();
    //     adapter.Fill(StorefrontSets, "Storefront");
    //     DataTable storefrontTable = StorefrontSets.Tables["Storefront"];
    //     List<Storefront> searchResults = new List<Storefront>();
    //     foreach(DataRow row in storefrontTable.Row){
    //         Storefront sto = new Storefront(row);
    //         searchResults.Add(sto);
    //     }
    //     return searchResults;
    // }

    // public List<Product> SearchInventories(string searchItem){
    //     string searchTerms = $"SELECT Inventory.InventoryID, Product.Title, Product.Price, Director.DirectorName, ReleaseYear.Year, MPARating.Rating FROM Invenotry INNER JOIN Product ON Inventory.ProductID=Product.ProductID INNER JOIN Director ON Product.DirectorID = Director.DirectorID INNER JOIN ReleaseYear ON Product.YearID = ReleaseYear.YearID INNER JOIN MPARating ON MPARating.RatingID = Product.RatingID WHERE Product.Title LIKE '%{searchItem}%' OR Director.DirectorName LIKE '%{searchItem}%'";
    //     using SqlConnection connection = new SqlConnection(_connectionString);
    //     using SqlCommand cmd = new SqlCommand(searchTerms, connection);
    //     using SqlDataAdapter adapter = new SqlDataAdapter(cmd);
    //     DataSet InventorySets = new DataSet();
    //     adapter.Fill(InventorySets, "Inventory");
    //     DataTable inventoryTable = InventorySets.Tables["Inventory"];
    //     List<Product> searchResults = new List<Product>();
    //     foreach(DataRow row in inventoryTable.Row){
    //         Product pro = new Product(row);
    //         searchResults.Add(pro);
    //     }
    //     return searchResults;
    // }
}