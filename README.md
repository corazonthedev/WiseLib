# WiseLib
WiseLib is a physical library management program using SQLite and Winforms

# Getting Started
- Install WiseLib and Launch

### First Setup

- Follow first setup steps

![1](https://github.com/user-attachments/assets/f9beb1fa-a264-47b1-9769-2af7402bfd11)

- Enter your library logo and name

![2](https://github.com/user-attachments/assets/50b89f5a-6c46-4a4c-a6a5-209ab83cc7d8)

- WiseLib will create a local database and launch 

![3](https://github.com/user-attachments/assets/711f53e3-bf7a-4008-b17b-3f0b7e60a758)

### Adding a new member

- You can use "new member" button on top-left or you can add a new member in members in right panel > add new member

![5](https://github.com/user-attachments/assets/b76228dd-0ab9-4bb3-9df4-37a61d3b615d)

- You have to enter  your member's name and surname. You can select a photo or use your camera

![6](https://github.com/user-attachments/assets/611a4bdb-6bc9-4139-9953-329752f539ba)

- After confirming your new member is added and their card is generated, you can print it with print button on bottom-right

![7](https://github.com/user-attachments/assets/726dee05-e382-4a1e-978a-603746100425)

- Your member is added succesfully. You can view member details but cannot edit name, surname and photo. So be careful when entering new member

![8](https://github.com/user-attachments/assets/abf12269-0cf9-494d-a0f2-6bb89f418432)

### Adding a new book

- Enter books on right-panel, here you can manage your books

![9](https://github.com/user-attachments/assets/6326c064-3d37-4753-a586-765c410b6049)

- Click add book button and enter your book info

![10](https://github.com/user-attachments/assets/542448bb-3837-4b80-a36f-e70dffb3075d)

- You can check ISBN with pasting or writing and clicking "check ISBN" button

![11](https://github.com/user-attachments/assets/3e9c1b8c-37b2-48b4-b13f-3ed4c859ebf5)

- You can also get your book info online. WiseLib will use [open library api search](https://openlibrary.org/search.json?isbn=) for fetching info

![12](https://github.com/user-attachments/assets/c7bc6937-c998-4c8b-b0ea-7b44a29f885a)
![13](https://github.com/user-attachments/assets/b5d51cce-0d28-4761-805d-2459b84d3bb5)

- After fetching your book info will be filled

![14](https://github.com/user-attachments/assets/fe8e4576-c87f-41d0-a4d3-f8a059ab2162)

- You can add book category just by typing category and confirming book. You can use "unknown" for your unknown category books

![15](https://github.com/user-attachments/assets/d1a21007-d4f1-461a-bcee-9df28b06250b)

- After confirming your book is succesfully added

![16](https://github.com/user-attachments/assets/ce604eed-3900-469a-a9f4-debabe658983)

### Lending Book

- Enter your member id

![17](https://github.com/user-attachments/assets/c1b35ae8-681d-41fa-bf44-ef59db8d8964)

- Confirm your member

![18](https://github.com/user-attachments/assets/8a4ae694-7175-4bf2-a90b-5626468f00bd)

- Select your book on right grid

![19](https://github.com/user-attachments/assets/94a2c888-5955-4705-a866-72c5c329bb3c)

- Click lend button

![20](https://github.com/user-attachments/assets/8eb2aa1c-5979-4368-b87f-8a5a7f5dcb44)

- And book is lended, book return date will be displayed and your member cannot borrow anymore book untill return

![21](https://github.com/user-attachments/assets/175e7c79-962f-4840-adce-1ab952ee6915)

- For returning book type and confirm your member and click "return"

![22](https://github.com/user-attachments/assets/9a2d1c59-8f93-49a9-b267-b0c72389030b)

### Managing Members

- Click members on your right-panel, here you can manage your members

![23](https://github.com/user-attachments/assets/eec8edd2-3c1d-4ac6-a954-e5fa124edd18)

- Click view or double click on your member for detailed info

![24](https://github.com/user-attachments/assets/50bf66e6-8ddb-43d1-8609-ce9681e6ea99)

- You can quickly create a report for your selected member for any problems

![25](https://github.com/user-attachments/assets/81e2d81b-8b5d-412d-b93f-3510df824034)

- After confirming member score will be decreased by 1. All members have 3 points default, if you ban member score will drop to -1. Also if a member's credit score drops to -1 it will be banned automatically. You can also edit credit score manually.

![26](https://github.com/user-attachments/assets/c353b3da-154d-4bc7-9d55-cad7bdd8cd4a)

- If your selected member has any active reports "view report" button will be displayed

![27](https://github.com/user-attachments/assets/caec588a-deed-4271-8e63-855a9bfc322e)

- You can also regenerate member's card

![28](https://github.com/user-attachments/assets/752d615a-d120-4317-b72a-09964df09eb5)

- You can see your member's log. Borrowed and returned books will be displayed here

![29](https://github.com/user-attachments/assets/7c130868-67c7-45ee-991c-ad9bed466fdc)

- You can ban your members for 15 days

![30](https://github.com/user-attachments/assets/34866f7c-2b65-47f8-a0dc-ea6226137666)

### Importing Books

- In books section you can import your books from excel file. Here is an example excel file:
- You have to add name, surname and join date. If you add any book borrow info you have to enter all of them


![31](https://github.com/user-attachments/assets/2efbe567-3f24-46b4-9fd4-c9fc3f0dbfbc)

- After you set your excel, click Select File button on bottom-left and select your excel file

![32](https://github.com/user-attachments/assets/38b460ce-17ff-48ac-8dac-d4b63cfc4c66)

- You have to select your columns on comboboxes to preview output

![33](https://github.com/user-attachments/assets/f871ce61-516e-451f-8606-12960a6c508e)

- After clicking preview your output will be displayed on right grid

![34](https://github.com/user-attachments/assets/5234020e-7b34-4f3c-98c7-fdd518d9642c)

- Click "Check Members" for checking wheter members are in your database or not

![35](https://github.com/user-attachments/assets/87dc9da4-7ff7-448b-8d60-1082a1ba2ac4)

- Click "Add Members" for adding members, empty lines will be skipped. You can see the progress in STATE columns for each row

![36](https://github.com/user-attachments/assets/aebd282d-29e1-4cdf-959e-c0c023bfda47)

- After that your members will be added automatically, you check in members section

![37](https://github.com/user-attachments/assets/90f34b9a-9ca6-4b88-b575-8c7a1f3d1f85)

- Currently imported members do not have any member cards or images, will be ad in future updates

![38](https://github.com/user-attachments/assets/a73be00d-3edf-4589-a9e9-064eb30bc534)

### Managing books

- You can use quick searches for quickly seeing overdue/missing etc. books

![39](https://github.com/user-attachments/assets/d64dcd48-963f-4e58-9a42-2f6ddd045fd8)

- In your book details you can quickly create a report for your book's problems

![40](https://github.com/user-attachments/assets/48972de1-8fd6-40cd-939e-f1eb491ac71d)

- You can also see your book's log

![41](https://github.com/user-attachments/assets/b309b992-a2c9-4bb0-8e44-1d37cbf4a495)

- You can edit your book's info and set book missing status

![42](https://github.com/user-attachments/assets/0d6f6ebc-de38-4f5f-9fb2-6796d1478845)

### Importing Books

- You can also import your books from an excel file. example excel file:

![43](https://github.com/user-attachments/assets/0b8e881c-a486-4657-a94c-d27cba9e925b)

- You need ISBN,TITLE,AUTHOR,PAGE,PUBLISHER,PUBLISH DATE info, you can also add category
- Select your columns from comboboxes and click "preview"

![44](https://github.com/user-attachments/assets/31de1820-80d0-4404-b842-c80435f2f47e)

- First check your books, wheter books in excel are in your database or not

![45](https://github.com/user-attachments/assets/a2678a28-8f7b-48fe-9554-66b7501a21e7)

- Click "Add Books" for adding all your books from selected excel

![46](https://github.com/user-attachments/assets/4fc34c2a-80a1-4940-ab08-1826ce380f01)

- You can get your book covers from [open library api search](https://openlibrary.org/search.json?isbn=) but it may not be accurate

![47](https://github.com/user-attachments/assets/7fe9fd0c-c749-4fe9-9f04-a879475d18d5)

- Images will be added one by one and it may take a while

![48](https://github.com/user-attachments/assets/45141426-e669-453a-b2d0-5669190b5b34)

- After adding all your book, you can check your books in books section

![49](https://github.com/user-attachments/assets/577d7dfb-f418-4c7a-86a7-6d36e66dab09)

### Reporting

- In reports you can manage your reports with details

![50](https://github.com/user-attachments/assets/709d304e-d54e-4cdb-848b-8a58310fd458)

- You can create a report manually, it will automatically create a report no, so you don't need to enter by hand

![51](https://github.com/user-attachments/assets/5dd1ee87-9fc2-482f-a8f0-96a3b903644b)

- You can also edit a created book and set as "solved"

![52](https://github.com/user-attachments/assets/c0630e94-feba-494a-8407-97fec03e34f6)

- In your notifications, you can see your license, overdue members, ending member bans, active reports. More will be added in future

![53](https://github.com/user-attachments/assets/4c55aeae-36ee-42b0-b74e-e22f0d29e44d)

- In history on your right-panel, you can see transaction log, here you can see what you done with details. it may take some time to load. Will be improved in future

![54](https://github.com/user-attachments/assets/5730e89b-27e5-49c4-a636-1d18d1e190f4)

- In right-panel you have a green connection button, if it goes red that means your database connection is lost. Here you can manage your connection settings

![55](https://github.com/user-attachments/assets/b3528ed5-dcee-4d47-a350-378279f09a8a)

- You can select a previous database backup freely, but it will delete your current database

![56](https://github.com/user-attachments/assets/78d25617-d56d-4920-aaaf-97bd1addb984)


# More about WiseLib

WiseLib will backup everytime you quit your app and it uses SQLite for quick and simple managing. This is my first devexpress-winforms project and not totally finished yet. Since it's not totally finished you might encounter with some bugs. Some more features will be added in future.
You can contribute freely.

