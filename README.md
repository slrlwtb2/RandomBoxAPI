


# RandomBoxAPI

The project is an API for a loot box and inventory management system. Users can register, log in, and perform various actions related to managing their inventories, obtaining items from boxes, buying and selling items, and accessing user-related information.

---
In this project, I worked with Entity Framework to configure its interaction with SQL databases, which was a key aspect of this project.

I learned how to set up the database context, define models and relationships, and perform various database operation with Entity Framework.

Next, I using the concept of repositories and services. These components played a important role on structure and logic for this project.

But let me tell you, the real challenge was connecting my API to the front-end part. Making them work together felt like solving a puzzle.

You can access the RandomBox web application by checking my github [HERE](https://github.com/slrlwtb2/RandomBoxWebApplication).

## Controller

![enter image description here](https://lh3.googleusercontent.com/pw/AIL4fc_7QT7BnXCvmXoLzUOJ2aTW24QokATPYlP5yo84xPp6oSugzPuyCev2ASIwpQl3ZoK_bmt10YI5qU6MLKqkQx8AEt5kr38Sr1j87D5vCwyM1g7xPLSbAc0eZE14UUYQCQxqiojlvQ480jieNoOK_4J-WQIrZud913EJW5_hGF4JESEoBRs2BtmQi_wBDWbP3D7a2MMF5oyIcFIHehzqJcO3hNct4v8yFa_mfD9OdMAIlR8AvapjjsRVAwp1sej8sDTr1n22Kp0-QCWdYsVdbHiAe5vFxVC6pFd4OF60yEgqsJq8hkD8qMaD8wIVj3-zla-hHLUVu2p8z4ZCxw-rJPyvwt2AyCIltFH6wOJFZcyUTXiPoZ_iiz6Q2cFSR5w5WdYai4xM8sYP70Huoaa2XXtPlFc8167d1oj3Ur6yv507QQAuVg76LQpaquNnDE8PdQcaCwTImJijJPyxWRB9H1Rz1j5w1NGo3VvqN4zo-AYvHGxltGwrBY1Sq4_0sJkwRY44qyiee_cUN_6uzkUfiA-wJJ6Vh--NtWysxX_SXVjA-C2hXoTBnnfJ9Vb8thCqppJyx3ZN9cU9ZOPg3-0cRqSreVYGSWdQ_w6Dt1PDqCYI4IijOafLscO2CAVnDRUYBOAssqq2Q9-2iEeevLQWFul09tFlYae6MtO7XEOIDr29oOvZK1oQMm6XWLD-GJjPA6VrhfADZTR3inTD5Ta8jf9LNt0q7-wTAmWNWYW0pNqR5oE31gmw84qfnhyCwqKHMjyX_J3oyO95OIu6bW3ftjeDvrYhV8RO6MHj61mqpEbFcyu0VYElFcQldbfkV9jeV1xzW-LUotPrBCWiVSVtjG2Jv_NfQSUOWGfASrmiVnxMDb-dGowYgcbo0E1zNJZubqW4uLK1xlPMfisZVYomRw=w349-h307-s-no?authuser=0)

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
![enter image description here](https://lh3.googleusercontent.com/pw/AIL4fc-DN0SKdA0qqhhdWBUsrVlXrSweYNtrbAMr9D24dU4hDdAKM12s9Kyv7XnyOYLh7wSzUyO4Q3b05kKfJHwlSU06zXOTvXNPwk5infkI6gZG72J_uUFZ2ZkyNrNJmdvnhH5XvwCnn1M1G8ZrfAavuGXPglhSpN9mIivlFjB2iF9H01IbGjRZJ5jcKs46kFDxkl-S7AI0VezzEHkDonz3KsR-0MgG6tHQP-3mcN19THwL_BRuf0EHxKid6dq_x376r9Ts3jJR2H7UT-BYlajcVfJjRvvpnDXQidU-m6b6TnEBpjEzYHpXSdQcIuBIxziKCukgLv37B2vVguFn-b5y6WWVAt1G9jbFr8sM4tFkd_n9ABodX9KCEK3IH612Owuu3BPQ5xxqqQap9jQnZoADqY7rq8ryHuTUQOSLn8B637n7PyqiS_u4L310LqxK1wDWGm9pRXLfcD4VPzN4Xi_slqlca4Zq4Ov0jQMeteYFFZiXeZKi6UIpAiAYrB0kzCPQBXUSp3OjnYmz_K65KR17VgNRC39j0KCGfGRcLS0ji49HM_NHthoI5ggDMP97Zxd5Ore2jEgSTI5YX0qsHFbUEhPr7IcNKyhLzH_OmTJXuTjcN1Sgksd90eA68mPSFovMWTBuvGY9jw9pUqVNB26NiFM-zpU2SRp7VSn8ytasD3DO6liyHyJjU2f243h8e7HWCcBRsTDRZmQX8p7TbMQOIo-yrg7uwctjdNia0eu01FzOysJKo9ZsqWjWywr60O_mMryHpfJepLXpRSb7rtI-Mh4_08jwlg7fordmpET-wKEeL0IFn6TNbSxaMA-37CTwCmDQi5ma-Nap_pHNezknAj9Vgyxlr7RNRRd9Y_YHPyZO-01LjxagcpPjl9KD2fngxjrS8gylh8F4m2y-NBchqA=w644-h791-s-no?authuser=0)

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
![enter image description here](https://lh3.googleusercontent.com/pw/AIL4fc-B2rC5_y2w40OAGLtxuGoyc0IWynC_13CZFDa6Np5ld9NHIUZHjOTxD3KSvOW7eNfsZwG7IfVpOr559snJiJSYpqn2R6doSExIbR2R5tiHI8dmkzBLDUezpmURI5GmCo3uJL_KC3dwXKqBN8OTWaoqOhCAC8oJl6QvF9VlbSewiuuSp6zK5pSageSiL5_jvhOHiCDWhRujIpVMky_ypUIN1sAJiG0BwTXaU-6txX2PswOGhUlHesxLcfNGf80-DcgOSCwyeNV6PBApZl1MsH6zquXnYMJCHeS5Z2Uajh4bJt-5w6etpHq_7YwwTQBuwysS2kiu3KHDvHm5HySBlIBxOI55jy1CNAeWumavnw6CFjdTa_7u8WjzZTZYhKKPDgGjLtUKByF-tc-_vUV3Dg8BC6dhXkzIkemQQ5K4_nr1XR40Qe0xUGBZwDKHLwFAUljAhgXbgijWfLRbTCDCT0RyQI-xrFDIc3w5rLganvgX0uU0LkW_lA2mCX67gQycO4oqdMuY26769K6qk2HeR1leoyLyjWv1lOHtLftTjkLr3KpunB29jnwrVO55KUfOwfCfiEMTE682xGbP62MOcg6nKXX7TFqDPIZ-lhXBqTq8qhsiaHQDwF3h4TzaIq_H7nvcAhYrZuOIEbvzSVYeNL3ADsD4so8TBo4I7vXHlk_IBjOT6Lp87qb-TCsmuNQGHyCJqN8M4KPnzy-LgotWsHGaAwIwZH8xF4gVgod9MogZVE5ogFZgo4X2HMJN0LFfrgOcK9wnP_Xwz3EJ_CzRS0qsRzckHNiKN7itNqJuPaBzlExU3KK6MYMXP8mvQD0ktc6hvu50HauJiZPI1PO19K-L4XoP7wXe2wtUBnondvQu9wDIpfSUdVsp4OPlbMDEnaW4GUlWh5I6nXhl34XRcg=w729-h432-s-no?authuser=0)

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

![enter image description here](https://lh3.googleusercontent.com/pw/AIL4fc_EMrE8ZgwWduExVUaNvLkPQQAxk0PwK7GeWBqDvqbmO1DG-vGeLeUkY7ydRJebgpwhtV2jbUi_zFLeAPygH5_iLZZyz3p51vyeTQM-hxqri9g0CXUhtPcOqRqPszjPzNoOPC0sX0kQj17wI-t5kj_y156wp7FcIxIhrYR-7AS-rHytqn0rGBOT_QDzklm9xkyCpJjv2itWBuLPClzqrttA7rotzXKQsAg6BE9UGdGDJ3aeeIcH9zY_CpAIsoMqXn2F1rPEHj4if4cGs609g2nwYhTWFlO0Kf1FpMPZ1pf7GIntjPVTE0jXSRAzEYONIuZib5nCFv-JHhTpizgtpjl1VA0BK7Nt4g_eRIWL0PZIhXVmasP2-JipHLhNtvxi4Mj-SOMN8lAGj8t0_gEWkhhdszVbtDl2uEjmO_gWW9_ApNLyfA7ez4QAUYFm3zto7vs_7BwTFJIlagrOiCvOdqfDwj1pURAZ5fAiuwUGpnDPPuvZ1l9KV48d9TNJxaT6bVBVAKGf9iAYP250A2HWi7UPgxjIewDe6AGsxS-kQ4I4EPJWV3Q1YqfjE-WGFizYzKGf_YNt9NnhvfKr_qK6QivBtiW3mxJTcf283nMfe5IXaZ2uSGaDTU6hT7KV66X9-iRL2-Ilc6pE_7rUhYNBTUbYkFYLoAIhOifu-L1_Yjd0uCKpRbfiPxx8Ez1gChM2UY7ojthTp7wqT07JFb5oLSwaoW_b_ke4sYairCwVzPf9ktu2ea0_jJY5CJSIjxG8xZ_r3hqL1GNT0BR4cs1XF1tKPDnQpgQlN8VMzO_GcjUElVa1pUXCjmCgkGPQ8fZpiP7TBGK944ny0qHdd-IaP9mW9pNKqoYMl9bfqlhH8zfU2_BKfLcpx5FMwVs0emp1uRNu79JoSOI9s91oet3Wng=w1316-h556-s-no?authuser=0)
