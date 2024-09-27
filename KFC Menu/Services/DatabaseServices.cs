using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KFC_Menu.Models;
using SQLite;
using static KFC_Menu.OrderPage;

namespace KFC_Menu.Services
{
    public class DatabaseServices
    {
        private static DatabaseServices instance;
        private static SQLiteAsyncConnection db;
        private static readonly string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KFC_Menu.db3");
        private Models.User loggedInUser;

        public static DatabaseServices Instance => instance ??= new DatabaseServices();
        public static async Task initializeDatabase()
        {
            try
            {
                if (db == null)
                {
                    db = new SQLiteAsyncConnection(dbPath);
                    await db.CreateTableAsync<User>();
                    await db.CreateTableAsync<MenuItem>();
                    await db.CreateTableAsync<Favorite>();
                    await db.CreateTableAsync<Order>();
                    await db.CreateTableAsync<OrderItem>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task<int> addUser(User user)
        {
            try
            {
                await DatabaseServices.initializeDatabase();
                var result = await db.InsertAsync(user);
                Console.WriteLine($"User added with result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

        }

        public static Task<User> getUser(string email, string password)
        {
            return db.Table<User>().FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public Task<User> getLoggedInUser()
        {
            return Task.FromResult(loggedInUser);
        }

        public Task setLoggedInUser(User user)
        {
            loggedInUser = user;
            return Task.CompletedTask;
        }

        public Task signOutUser()
        {
            loggedInUser = null;
            return Task.CompletedTask;
        }

        public async Task<int> addOrder(Order order)
        {
            await initializeDatabase();
            return await db.InsertAsync(order);
        }

        public async Task<int> addOrderItem(OrderItem orderItem)
        {
            await initializeDatabase();
            return await db.InsertAsync(orderItem);
        }

        public async Task<List<Order>> getOrdersForUser(int userId)
        {
            await initializeDatabase();
            return await db.Table<Order>().Where(o => o.UserId == userId).ToListAsync();
        }

        public async Task<int> updateOrderItem(OrderItem orderItem)
        {
            await DatabaseServices.initializeDatabase();
            return await db.UpdateAsync(orderItem);
        } 

        public async Task<int> removeOrderItem(int orderItemId)
        {
            await initializeDatabase();
            return await db.DeleteAsync<OrderItem>(orderItemId);
        }

        public async Task<List<OrderItem>> getOrderItem(int userId)
        {
            await DatabaseServices.initializeDatabase();
            return await db.Table<OrderItem>().Where(oi => oi.UserId == userId).ToListAsync();
        }
        public async Task addFavorite(int userId, MenuItem item)
        {
            var favorite = new Favorite
            {
                UserId = userId,
                ItemId = item.Id,
                ItemName = item.Name,
                ItemPrice = item.Price,
                ItemImage = item.Image,
                ItemDescription = item.Description,
                ItemCategory = item.Category
            };
            await db.InsertAsync(favorite);
        }

        public async Task<Favorite> getFavoriteItem(int userId, int itemId)
        {
            await DatabaseServices.initializeDatabase();
            return await db.Table<Favorite>().FirstOrDefaultAsync(fi => fi.UserId == userId && fi.ItemId == itemId);
        }

        public async Task<List<MenuItem>> getFavoritesForUser(int userId)
        {
            await DatabaseServices.initializeDatabase();
            var userFavorites = await db.Table<Favorite>().Where(f => f.UserId == userId).ToListAsync();
            return userFavorites.Select(f => new MenuItem
            {
                Id = f.ItemId,
                Name = f.ItemName,
                Price = f.ItemPrice,
                Image = f.ItemImage,
                Description = f.ItemDescription,
                Category = f.ItemCategory
            }).ToList();   
        }

        public async Task removeFromFavorites(int userId, int itemId)
        {
            await DatabaseServices.initializeDatabase();
            var favorite = await getFavoriteItem(userId, itemId);
            if (favorite != null)
            {
                await db.DeleteAsync(favorite);
            }
        }
    }

    
}
