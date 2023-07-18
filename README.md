



# RandomBoxAPI

The project is an API for a loot box and inventory management system. Users can register, log in, and perform various actions related to managing their inventories, obtaining items from boxes, buying and selling items, and accessing user-related information.

---
In this project, I worked with Entity Framework to configure its interaction with SQL databases, which was a key aspect of this project.

I learned how to set up the database context, define models and relationships, and perform various database operation with Entity Framework.

Next, I using the concept of repositories and services. These components played a important role on structure and logic for this project.

But let me tell you, the real challenge was connecting my API to the front-end part. Making them work together felt like solving a puzzle.

You can access the RandomBox web application by checking my github [HERE](https://github.com/slrlwtb2/RandomBoxWebApplication).

## Controller

![enter image description here](https://i.postimg.cc/NG2xfyK3/Screenshot-2023-07-15-173026.png)

**User Controller**

 - GetUsers: An endpoint that retrieves a list of users from a
   repository and returns them.
 - GetBalance: An endpoint that retrieves the balance of a user based on
   their user ID extracted from the JWT token.
 - Register: An endpoint that allows users to register by creating a new
   user with a username, password hash, and password salt.
 - Login: An endpoint that verifies user credentials (username and
   password) and generates a JWT token for authentication.

---

![enter image description here](https://i.postimg.cc/ZKkx9T3C/Screenshot-2023-07-15-173019.png)

**Item Controller**
 - GetItems: An endpoint that retrieves a list of items.
 - GetItemNameById: An endpoint that retrieves the name of an item based
   on its ID.
 - GetItemDiscriminator: An endpoint that determines the discriminator
   (whether an item is a box or a regular item) based on its ID.
 - CreateItem: An endpoint for creating a new item.
 - CreateBox: An endpoint for creating a new box.
 - DeleteItem: An endpoint for deleting an item.
 - UpdateItem: An endpoint for updating an existing item.
 - AddItemToBox: An endpoint for adding an item to a box.
 - GetBoxes: An endpoint that retrieves a list of boxes.
 - GetItemsInBox: An endpoint that retrieves a list of items within a
   box.
 - UploadPicture: An endpoint for uploading an image to an item based on
   its ID.
 - GetImage: An endpoint that retrieves the image of an item based on
   its ID.

---
![enter image description here](https://i.postimg.cc/1t2crSj7/Screenshot-2023-07-15-173005.png)

**Inventory Controller**

 - GetById: An endpoint that retrieves inventory items based on the user
   ID extracted from the JWT token.
 - AddItemtoInv: An endpoint for adding an item to the inventory based
   on the item ID, user ID, and quantity.
 - SellItemfromInventory: An endpoint for selling an item from the
   inventory based on the item ID, user ID, and quantity.
 - OpenBox: An endpoint for opening a box and obtaining items from it
   based on the box ID and user ID.
 - BuyBox: An endpoint for buying a box and adding it to the inventory
   based on the box ID and user ID.
 - UploadPicture: An endpoint for uploading an image to an item based on
   its ID, associating the image with the item.

## ER DIAGRAM

![enter image description here](https://i.postimg.cc/L5W5G6pL/imageedit-2-7847646189.png)
