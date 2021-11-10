# MiniCrm
Login to Sitecore and install package
Or Do this in Sitecore

/sitecore/templates/User (this is the User template)
/sitecore/layout/Renderings/User List Rendering (this is a Controller Rendering for the user list method)
/sitecore/layout/Renderings/User Edit Rendering (this is a Controller Rendering for the user edit method)
/sitecore/layout/Renderings/User List Rendering (this is a Controller Rendering for the user delete method)
/sitecore/content/Users (this is the bucket folder to store User items)

Sitecore Package
1.First ensure the Connection String for Solr is correct in your connection strings config (typically “connectionstrings.config”).
2. Then create a Data Template in Sitecore - User 
3. Solr creates the property on the Solr server for all the default and custom fields. To use those fields in code, map them in a Class given at VS project
4. Create a View Rendering to display the content that will be indexed using your standard Sitecore development methods.
5./sitecore/content/Users (this is the bucket folder to store User items)
6.Next, we'll set up the presentation items. Add three new items to your content tree: "UserList", "UserEdit" and "UserDelete".
7. Configure the presentation details for each item (here's an example for the UserList item)
