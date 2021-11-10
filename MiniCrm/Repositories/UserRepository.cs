using MiniCrm.Models;
using MiniCrm.Models.ViewModels;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Linq;

namespace MiniCrm.Repositories
{
    public static class UserRepository
    {
        const string USER_INDEX_NAME = "mini_crm_index_master";
        const string USER_FOLDER = "{A9F781D8-EF20-416A-9455-E18125E4D0A1}";
        const string USER_TEMPLATE = "{138A3836-3A68-4CDA-BF88-3C2A0C2D3A9D}";

        public static List<UserListViewModel> GetAllUsers(string searchTerm)
        {
            // Initialize the query. 
            var query = PredicateBuilder.True<SearchResultItem>();

            // Filter out everything except User template.
            var templatesExpression = PredicateBuilder.True<SearchResultItem>();
            templatesExpression = templatesExpression.And(i => i.TemplateId == new ID(USER_TEMPLATE));
            query = query.And(templatesExpression);

            // If search term was provided, filter on it.
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchExpression = PredicateBuilder.True<SearchResultItem>();
                searchExpression = templatesExpression.And(i => (i["email_t"].Contains(searchTerm) || i["first_name_t"].Contains(searchTerm)));
                query = query.And(searchExpression);
            }

            // Qery the Solr index.
            var users = new List<SearchResultItem>();
            using (var context = ContentSearchManager.GetIndex(USER_INDEX_NAME).CreateSearchContext())
            {
                users = context.GetQueryable<SearchResultItem>(new CultureExecutionContext(Sitecore.Context.Language.CultureInfo))
                .Where(query)
                .Take(1000) // Override any limitations in the configuration on # of results returned.
                .ToList();
            }

            var userList = new List<UserListViewModel>();
            foreach (var user in users.OrderBy(i => i.Name)) // Order the results by item name.
            {
                // Build the list.
                var model = new UserListViewModel
                {
                    Address = GetFormattedAddress(user),
                    Email = user.Fields["email"].ToString(),
                    Id = user.ItemId.ToString(),
                    Name = string.Format("{0}, {1}", user.Fields["last_name"].ToString(), user.Fields["first_name"].ToString()),
                    Phone = user.Fields["phone"].ToString()
                };
                userList.Add(model);
            }

            return userList;
        }

        public static UserModel GetUserById(string id)
        {
            // Use the master database.
            var db = Factory.GetDatabase("master");

            // Get the item from Sitecore.
            var user = db.GetItem(id);
            if (user == null)
            {
                return null;
            }

            // Populate the UserModel.
            return new UserModel
            {
                Address1 = user.Fields["Address 1"].Value,
                Address2 = user.Fields["Address 2"].Value,
                City = user.Fields["City"].Value,
                Email = user.Fields["Email"].Value,
                FirstName = user.Fields["First Name"].Value,
                Id = user.ID.ToString(),
                LastName = user.Fields["Last Name"].Value,
                Phone = user.Fields["Phone"].Value,
                State = user.Fields["State"].Value,
                Zip = user.Fields["Zip"].Value
            };
        }

        public static void UpdateUser(UserModel model)
        {
            // Use the master database.
            var db = Factory.GetDatabase("master");

            // Disable security.
            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                Item item = null;
                if (model.Id == "0")
                {
                    // Create a new user.
                    var userFolder = db.GetItem(USER_FOLDER); // Get the root User folder.
                    var userTemplate = new TemplateID(new ID(USER_TEMPLATE)); // Get a template ID for the User template.
                    item = userFolder.Add(model.LastName + " " + model.FirstName, userTemplate); // Create the new item - use LastName + Firstname to name the item.
                }
                else
                {
                    // Update an existing user.
                    item = db.GetItem(model.Id);
                }

                if (item != null)
                {
                    // Update the item fields from the model.
                    item.Editing.BeginEdit();

                    item.Fields["Address 1"].Value = model.Address1;
                    item.Fields["Address 2"].Value = model.Address2;
                    item.Fields["City"].Value = model.City;
                    item.Fields["Email"].Value = model.Email;
                    item.Fields["First Name"].Value = model.FirstName;
                    item.Fields["Last Name"].Value = model.LastName;
                    item.Fields["Phone"].Value = model.Phone;
                    item.Fields["State"].Value = model.State;
                    item.Fields["Zip"].Value = model.Zip;

                    item.Editing.EndEdit();
                }

                // Give the index a second or two to update.
                System.Threading.Thread.Sleep(2000);
            }
        }

        public static void DeleteUserById(string id)
        {
            // Use the master database.
            var db = Factory.GetDatabase("master");

            // Disable security.
            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                // Get the item from Sitecore.
                var user = db.GetItem(id);
                if (user != null)
                {
                    // Delete it or recycle it depending on settings.
                    if (Settings.RecycleBinActive)
                    {
                        user.Recycle();
                    }
                    else
                    {
                        user.Delete();
                    }
                }
            }
        }

        private static string GetFormattedAddress(SearchResultItem user)
        {
            var address = string.Empty;

            try
            { 
            address = user.Fields["address_1"].ToString();
            if (user.Fields["address_2"] != null)
                {
                    address += "<br />" + user.Fields["address_2"].ToString();
                }
            }
            catch (KeyNotFoundException) { }

            address += string.Format("<br />{0}, {1} {2}", user.Fields["city"].ToString(), user.Fields["state"].ToString(), user.Fields["zip"].ToString());

            return address;
        }
    }
}